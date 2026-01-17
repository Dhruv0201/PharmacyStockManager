namespace PharmacyStockManager.Helpers
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class NumericTextboxProperty
    {
        public static readonly DependencyProperty IsNumericOnlyProperty =
            DependencyProperty.RegisterAttached(
                "IsNumericOnly",
                typeof(bool),
                typeof(NumericTextboxProperty),
                new UIPropertyMetadata(false, OnIsNumericOnlyChanged));

        public static bool GetIsNumericOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumericOnlyProperty);
        }

        public static void SetIsNumericOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumericOnlyProperty, value);
        }

        private static void OnIsNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    textBox.TextChanged += TextBox_TextChanged;
                    textBox.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(TextBox_Pasting));
                }
                else
                {
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                    textBox.RemoveHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(TextBox_Pasting));
                }
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == null || textBox.Text == "")
                {
                    textBox.Text = "0";
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                foreach (char c in text)
                {
                    if (!char.IsDigit(c) && c != '.')
                    {
                        e.CancelCommand();
                        return;
                    }
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
