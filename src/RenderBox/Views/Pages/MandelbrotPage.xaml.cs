﻿using System.Windows.Controls;
using RenderBox.Services.Options;
using RenderBox.Services.Renderers;
using RenderBox.Shared.Modules.Mandelbrot.Filters;

namespace RenderBox.Views.Pages
{
    public partial class MandelbrotPage : Page, IOptionsPage<MandelbrotRenderer>
    {
        private MandelbrotRenderer _source;

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
                _source.Filter = null;
                _source.Render(Dispatcher);
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
                    _source.Filter = (IPaletteFilter)instance;
                    _source.Render(Dispatcher);
                };

                EffectsPanel.Children.Add(button);
            }
        }

        public void UseSource(MandelbrotRenderer source)
        {
            _source = source;

            Iterations.Text = _source.Iterations.ToString();
            Extent.Text = _source.Extent.ToString();
            BatchSize.Text = _source.BatchSize.ToString();
        }

        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _source.Iterations = int.TryParse(Iterations.Text, out var a) ? a : 0;
            _source.Extent = double.TryParse(Extent.Text, out var b) ? b : 0;
            _source.BatchSize = int.TryParse(BatchSize.Text, out var c) ? c : 0;

            _source.Render(Dispatcher);
        }

    }
}
