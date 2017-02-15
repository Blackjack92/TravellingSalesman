using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TravellingSalesman.Views
{
    /// <summary>
    /// Interaktionslogik für PointsListView.xaml
    /// </summary>
    public partial class PointsListView : UserControl
    {
        public PointsListView()
        {
            InitializeComponent();

            xPos.PreviewTextInput += NumberPreviewTextInput;
            yPos.PreviewTextInput += NumberPreviewTextInput;
        }

        /// <summary>
        /// This checks that no only numbers between 0-500 are entered for the point coordinations.
        /// </summary>
        /// <param name="sender">The TextBox.</param>
        /// <param name="e">Arguments with the new entered character.</param>
        private void NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if it is a character
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }

            // Check the number > 0 && <= 500
            TextBox input = sender as TextBox;
            int value;
            if (int.TryParse(input.Text + e.Text, out value) && value > 500)
            {
                MessageBox.Show("The point has to be between x[0-500] and y[0-500].");
                e.Handled = true;
            }
        }
    }
}
