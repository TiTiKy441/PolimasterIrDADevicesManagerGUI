using PolimasterIrDADevicesManagerGUI.Device;
using PolimasterIrDADevicesManagerGUI.GUI.Graphs;
using System.Windows;
using System.Windows.Controls;

namespace PolimasterIrDADevicesManagerGUI.GUI
{
    /// <summary>
    /// Interaction logic for HistoryViewerWindow.xaml
    /// </summary>
    public partial class HistoryViewerWindow : Window
    {

        public readonly IHistoryAccessDevice Device;

        private CancellationTokenSource _cancellationTokenSource = new();

        private CancellationToken _cancellationToken { get { return _cancellationTokenSource.Token; } }

        private HistoryEventRecord[]? _readHistory;

        private double? _calibrationValue;

        private GraphViewerWindow? _graphViewerWindow;

        public HistoryViewerWindow(IHistoryAccessDevice historyAccessDevice, double? calibrationValue = null)
        {
            InitializeComponent();
            Device = historyAccessDevice;
            _calibrationValue = calibrationValue;
            Closing += HistoryViewer_Closing;
        }

        private void HistoryViewer_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_graphViewerWindow != null)
            {
                _graphViewerWindow.Close();
            }
        }

        private void read_history_button_Click(object sender, RoutedEventArgs e)
        {
            read_history_button.IsEnabled = false;
            cancel_button.IsEnabled = true;
            Device.ReadHistoryAsync(_cancellationToken).ContinueWith((task) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    read_history_button.IsEnabled = true;
                    if (task.IsCompletedSuccessfully && GraphBuilder.IsSupported(task.Result.GetType()))
                    {
                        plot_button.IsEnabled = true;
                    }
                });
                if (task.IsCompletedSuccessfully)
                {
                    HistoryEventRecord[] records = task.Result;
                    _readHistory = records;
                    this.Dispatcher.Invoke(() =>
                    {
                        using (history_stackpanel.Dispatcher.DisableProcessing())
                        {
                            foreach (HistoryEventRecord record in records)
                            {
                                history_stackpanel.Children.Add(new Label() { Content=record.ToString() });
                            }
                        }
                    });
                }
            });
        }

        private void plot_button_Click(object sender, RoutedEventArgs e)
        {
            if ((_readHistory == null) || (_readHistory?.Length == 0))
            {
                return;
            }
            if ((_graphViewerWindow != null))
            {
                _graphViewerWindow.Close();
            }
            _graphViewerWindow = new GraphViewerWindow(GraphBuilder.GetWpfPlot(_readHistory, _calibrationValue));
            _graphViewerWindow.Show();
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            if (!_cancellationTokenSource.TryReset()) _cancellationTokenSource = new();
        }
    }
}
