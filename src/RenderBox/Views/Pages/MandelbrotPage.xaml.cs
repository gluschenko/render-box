using System.Windows.Controls;
using RenderBox.Services.Options;
using RenderBox.Services.Renderers;
using RenderBox.Shared.Modules.Mandelbrot.Filters;

namespace RenderBox.Views.Pages
{
    public partial class MandelbrotPage : Page, IOptionsPage<MandelbrotRenderer>
    {
        private MandelbrotRenderer? _source;
        private MandelbrotRenderer Source => _source ?? throw new InvalidOperationException($"{nameof(UseSource)} must be called before using this page.");

        public MandelbrotPage()
        {
            InitializeComponent();

            ApplyButton.Click += ApplyButton_Click;

            var filters = typeof(IPaletteFilter).GetSubclasses();

            EffectsPanel.Children.Clear();

            var noFilterButton = new Button
            {
                Content = "No filter",
                Height = 20
            };

            noFilterButton.Click += (s, e) =>
            {
                Source.Filter = null;
                Source.Render(Dispatcher);
            };

            EffectsPanel.Children.Add(noFilterButton);

            foreach (var filter in filters)
            {
                var instance = Activator.CreateInstance(filter);
                if (instance is not IPaletteFilter paletteFilter)
                {
                    continue;
                }

                var button = new Button
                {
                    Content = filter.Name,
                    Height = 20
                };

                button.Click += (s, e) =>
                {
                    Source.Filter = paletteFilter;
                    Source.Render(Dispatcher);
                };

                EffectsPanel.Children.Add(button);
            }
        }

        public void UseSource(MandelbrotRenderer source)
        {
            _source = source;

            Iterations.Text = Source.Iterations.ToString();
            Extent.Text = Source.Extent.ToString();
            BatchSize.Text = Source.BatchSize.ToString();
        }

        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Source.Iterations = int.TryParse(Iterations.Text, out var a) ? a : 0;
            Source.Extent = double.TryParse(Extent.Text, out var b) ? b : 0;
            Source.BatchSize = int.TryParse(BatchSize.Text, out var c) ? c : 0;

            Source.Render(Dispatcher);
        }

    }
}
