using System.Windows;

namespace ABPHelper.CodeGenerator
{
    public class Utils
    {
        public static MessageBoxResult MessageBox(string message, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information, params object[] parameters)
        {
            string msg = message;
            if (parameters.Length > 0)
            {
                msg = string.Format(message, parameters);
            }
            return System.Windows.MessageBox.Show(msg, "ABPHelper.CodeGenerator", button, icon);
        }

    }
}