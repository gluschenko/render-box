using System.Windows.Controls;
using RenderBox.Services.Options;
using RenderBox.Services.Renderers;

namespace RenderBox.Views.Pages
{
    public partial class TerrainPage : Page, IOptionsPage<TerrainRenderer>
    {
        public TerrainPage()
        {
            InitializeComponent();
        }

        public void UseSource(TerrainRenderer source)
        {
        }
    }
}
