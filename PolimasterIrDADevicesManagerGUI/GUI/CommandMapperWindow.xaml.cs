using PolimasterIrDADevicesManagerGUI.Device.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PolimasterIrDADevicesManagerGUI.GUI
{
    /// <summary>
    /// Interaction logic for CommandMapperWindow.xaml
    /// </summary>
    public partial class CommandMapperWindow : Window
    {

        public readonly PMDeviceBaseProtocol Device;

        private CancellationTokenSource _cancellationTokenSource = new();

        private CancellationToken _cancellationToken 
        { 
            get
            {
                return _cancellationTokenSource.Token;
            } 
        }

        public readonly byte[] AvoidValues = new byte[] { 64, 66, 0, 156 };

        public CommandMapperWindow(PMDeviceBaseProtocol device)
        {
            InitializeComponent();
            Device = device;
        }

        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            main_textblock.Text = string.Empty;
            main_textblock.Text += string.Format("Mapping starting ({0})" + Environment.NewLine, DateTime.Now);
            main_textblock.Text += string.Format("Device: {0}" + Environment.NewLine, IrDA.IrDAPortManager.ConnectedDevice?.DeviceName);
            main_textblock.Text += string.Format("Avoid values: {0}" + Environment.NewLine, string.Join(" ", AvoidValues.ToList().ConvertAll(x => x.ToString()).ToArray()));

            Task.Run(async () =>
            {
                this.Dispatcher.Invoke(() => 
                {
                    start_button.IsEnabled = false;
                    save_button.IsEnabled = false;
                    stop_button.IsEnabled = true; 
                });
                for (byte i = 0; (i < byte.MaxValue) && !_cancellationToken.IsCancellationRequested; i++)
                {
                    if (AvoidValues.Contains(i)) continue;
                    try
                    {
                        byte[] read = await Device.ReadFromMCAsync(i, _cancellationToken);
                        this.Dispatcher.Invoke(() =>
                        {
                            main_textblock.Text += Environment.NewLine;
                            if (read.Length == 2)
                            {
                                main_textblock.Text += string.Format("MC {0}: {1} ({2})" + Environment.NewLine, i, string.Join(" ", read.ToList().ConvertAll(x => x.ToString()).ToArray()), (ushort)(read[0] | read[1] << 8));
                            }
                            else
                            {
                                main_textblock.Text += string.Format("MC {0}: {1}" + Environment.NewLine, i, string.Join(" ", read.ToList().ConvertAll(x => x.ToString()).ToArray()));
                            }
                            main_scrollviewer.ScrollToBottom();
                        });
                        await Device.Write2ToMCAsync(i, read, _cancellationToken);
                        this.Dispatcher.Invoke(() => { main_textblock.Text += "Editable!" + Environment.NewLine; });
                    }
                    catch (Exception)
                    {
                    }
                }
            }).ContinueWith((task) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        save_button.IsEnabled = true;
                    }
                    stop_button.IsEnabled = false;
                    start_button.IsEnabled = true;
                });
            });
        }

        private void stop_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            Thread.Sleep(1000);
            _cancellationTokenSource = new();
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(main_textblock.Text);
        }
    }
}
