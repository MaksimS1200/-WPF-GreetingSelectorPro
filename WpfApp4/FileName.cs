using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp4
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) =>
            _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) =>
            _execute(parameter);

        public void UpdateExecuteAction(Action<object> newExecute)
        {
            _execute = newExecute ?? throw new ArgumentNullException(nameof(newExecute));
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private string _userName;
        private string _selectedLanguage;
        private string _outputText;

        private readonly Dictionary<string, Func<string, string>> _greetings = new()
        {
            { "Русский",    name => $"Привет, {name}!" },
            { "English",    name => $"Hello, {name}!" },
            { "Deutsch",    name => $"Hallo, {name}!" },
            { "Греческий",  name => $"Γεια σου, {name}!" },
            { "Болгарский", name => $"Здравей, {name}!" }
        };

        public MainViewModel()
        {
            Languages = new List<string>(_greetings.Keys);
            GreetCommand = new RelayCommand(_ => { }, _ => true);

            if (Languages.Count > 0)
                SelectedLanguage = Languages[0];
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                    UpdateCommandLogic();
                }
            }
        }

        public string OutputText
        {
            get => _outputText;
            set { _outputText = value; OnPropertyChanged(); }
        }

        public List<string> Languages { get; }
        public RelayCommand GreetCommand { get; }

        private void UpdateCommandLogic()
        {
            if (_greetings.TryGetValue(SelectedLanguage, out var greetingFunc))
            {
                GreetCommand.UpdateExecuteAction(_ =>
                {
                    OutputText = greetingFunc(UserName);
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
