using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThinkITAM.UserControls.InformationDisplay
{
    /// <summary>
    /// ConfirmationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmationDialog : UserControl
    {
        // 定义依赖属性
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ConfirmationDialog), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(
            "Prompt", typeof(string), typeof(ConfirmationDialog), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ConfirmButtonTextProperty = DependencyProperty.Register(
            "ConfirmButtonText", typeof(string), typeof(ConfirmationDialog), new PropertyMetadata("Confirm"));

        public static readonly DependencyProperty TitleColorProperty = DependencyProperty.Register(
            "TitleColor", typeof(Brush), typeof(ConfirmationDialog), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(207, 53, 57))));

        public static readonly DependencyProperty PromptColorProperty = DependencyProperty.Register(
            "PromptColor", typeof(Brush), typeof(ConfirmationDialog), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(207, 53, 57))));

        public static readonly DependencyProperty ConfirmButtonColorProperty = DependencyProperty.Register(
            "ConfirmButtonColor", typeof(Brush), typeof(ConfirmationDialog), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(207, 53, 57))));

        // 属性访问器
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Prompt
        {
            get => (string)GetValue(PromptProperty);
            set => SetValue(PromptProperty, value);
        }

        public string ConfirmButtonText
        {
            get => (string)GetValue(ConfirmButtonTextProperty);
            set => SetValue(ConfirmButtonTextProperty, value);
        }

        public Brush TitleColor
        {
            get => (Brush)GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value);
        }

        public Brush PromptColor
        {
            get => (Brush)GetValue(PromptColorProperty);
            set => SetValue(PromptColorProperty, value);
        }

        public Brush ConfirmButtonColor
        {
            get => (Brush)GetValue(ConfirmButtonColorProperty);
            set => SetValue(ConfirmButtonColorProperty, value);
        }

        public ConfirmationDialog()
        {
            InitializeComponent();
            this.DataContext = this; // 设置DataContext以支持数据绑定

            ConfirmCommand = new RelayCommand<bool>(OnConfirmed);
        }

        public ICommand ConfirmCommand { get; private set; }

        private void OnConfirmed(bool result)
        {
            if (result)
            {
                DialogHost.CloseDialogCommand.Execute(true, null);
            }
            else
            {
                DialogHost.CloseDialogCommand.Execute(false, null);
            }
        }
    }

    // RelayCommand 实现
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
