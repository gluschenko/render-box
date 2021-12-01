using System.Windows;
using System.Windows.Controls;
using RenderBox.Core;
using RenderBox.Options;
using RenderBox.Renderers;
using RenderBox.Rendering;

namespace RenderBox.Pages
{
    public partial class DrawingPage : Page, IOptionsPage<DrawingRenderer>
    {
        private DrawingRenderer _source;

        public DrawingPage()
        {
            InitializeComponent();
        }

        public void UseSource(DrawingRenderer source)
        {
            _source = source;
        }

        private void BlitButton_Click(object sender, RoutedEventArgs e)
        {
            _source.SetRender((context) =>
            {
                _source.Paint.FillRect(0, 0, context.Width, context.Height, Color.Green);
            });
            _source.Render(Dispatcher);
        }
    }
}
