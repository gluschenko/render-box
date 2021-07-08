using RenderBox.Modules;
using RenderBox.Options;
using System.Windows.Controls;

namespace RenderBox.Pages
{
    public partial class PathTracerPage : Page, IOptionsPage
    {
        private PathRenderer _source;

        public PathTracerPage()
        {
            InitializeComponent();
        }

        public void UseSource(object source)
        {
            _source = source as PathRenderer;

            ShowNormals.IsChecked = _source.ShowNormals;
            ShowDepth.IsChecked = _source.ShowDepth;
            BatchSize.Text = _source.BatchSize.ToString();
        }

        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _source.ShowNormals = ShowNormals.IsChecked ?? false;
            _source.ShowDepth = ShowDepth.IsChecked ?? false;
            _source.BatchSize = int.TryParse(BatchSize.Text, out int c) ? c : 0;

            _source.Render(Dispatcher);
        }
    }
}
