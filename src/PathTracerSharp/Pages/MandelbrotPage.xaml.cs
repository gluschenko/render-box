using System.Windows.Controls;
using PathTracerSharp.Modules;
using PathTracerSharp.Options;

namespace PathTracerSharp.Pages
{
    public partial class MandelbrotPage : Page, IOptionsPage
    {
        public MandelbrotRenderer Source { get; set; }

        public MandelbrotPage()
        {
            InitializeComponent();

            ApplyButton.Click += ApplyButton_Click;
        }

        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Source.Iterations = int.TryParse(Iterations.Text, out int a) ? a : 0;
            Source.Extent = double.TryParse(Extent.Text, out double b) ? b : 0;
            Source.BatchSize = int.TryParse(BatchSize.Text, out int c) ? c : 0;

            Source.Render(Dispatcher);
        }

        public void UseSource(object source)
        {
            Source = (MandelbrotRenderer)source;

            Iterations.Text = Source.Iterations.ToString();
            Extent.Text = Source.Extent.ToString();
            BatchSize.Text = Source.BatchSize.ToString();
        }
    }
}
