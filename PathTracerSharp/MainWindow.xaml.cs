using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

using PathTracerSharp.Core;
using PathTracerSharp.Rendering;
using PathTracerSharp.Modules.PathTracer;
using PathTracerSharp.Modules.PathTracer.Shapes;
using System.Windows.Controls;

/*
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Runtime.CompilerServices;
*/

namespace PathTracerSharp
{
    public partial class MainWindow : Window
    {
        private int TabCounter;

        public MainWindow()
        {
            InitializeComponent();
            AddTab();
        }

        private void AddTab() 
        {
            int idx = Tabs.Items.Count;
            Tabs.SelectedIndex = idx - 1;
            TabItem item = new TabItem()
            {
                Header = $"Render #{++TabCounter}",
                Content = new Frame() 
                { 
                    Content = new RenderPage() 
                }
            };

            item.MouseRightButtonUp += (s, e) => {
                if (Tabs.Items.Count > 2) 
                {
                    Tabs.Items.Remove(item);
                    Tabs.SelectedIndex = Tabs.Items.Count - 2;
                }
            };

            Tabs.Items.Insert(idx - 1, item);
            Tabs.SelectedIndex = Tabs.Items.Count - 2;
        }

        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddTab();
            e.Handled = true;
        }
    }
}
