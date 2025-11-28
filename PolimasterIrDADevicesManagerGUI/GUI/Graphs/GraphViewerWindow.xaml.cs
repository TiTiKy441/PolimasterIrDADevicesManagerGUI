using PolimasterIrDADevicesManagerGUI.Utils;
using ScottPlot.WPF;
using System.Windows;
using Microsoft.Win32;

namespace PolimasterIrDADevicesManagerGUI.GUI.Graphs
{
    /// <summary>
    /// Interaction logic for GraphViewerWindow.xaml
    /// </summary>
    public partial class GraphViewerWindow : Window
    {

        private readonly WpfPlot _plot;

        private static readonly string ModuleName = nameof(GraphViewerWindow);

        public GraphViewerWindow(WpfPlot plot)
        {
            InitializeComponent();
            main_plot_grid.Children.Add(plot);
            _plot = plot;
        }
    }
}
