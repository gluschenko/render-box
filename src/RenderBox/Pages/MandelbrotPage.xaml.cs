using System;
using System.Windows.Controls;
using RenderBox.Modules;
using RenderBox.Options;
using RenderBox.Shared.Modules.Mandelbrot.Filters;

namespace RenderBox.Pages
{
    public partial class MandelbrotPage : Page, IOptionsPage
    {
        public MandelbrotRenderer Source { get; set; }

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

                var button = new Button
                {
                    Content = filter.Name,
                    Height = 20
                };

                button.Click += (s, e) =>
                {
                    Source.Filter = (IPaletteFilter)instance;
                    Source.Render(Dispatcher);
                };

                EffectsPanel.Children.Add(button);
            }
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
