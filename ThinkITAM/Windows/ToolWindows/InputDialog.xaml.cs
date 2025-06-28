using System.Windows;

namespace ThinkITAM.Windows.ToolWindows
{
    /// <summary>
    /// InputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InputDialog : Window
    {


        public InputDialog()
        {
            InitializeComponent();
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            string input = InputBox.Text.Trim().ToUpper();


            switch (input)
            {
                case "YES":


                    this.DialogResult = true;

                    break;

                case "NO":

                    this.DialogResult = false;
                    break;


                default:
                    MessageBox.Show("请输入正确的指令", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }



        }

        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }




}
