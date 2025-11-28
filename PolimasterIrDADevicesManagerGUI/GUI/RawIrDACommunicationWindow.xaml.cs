using PolimasterIrDADevicesManagerGUI.IrDA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for RawIrDACommunicationWindow.xaml
    /// </summary>
    public partial class RawIrDACommunicationWindow : Window
    {

        public readonly IrDADevice _irdaDevice;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private CancellationToken _cancellationToken
        {
            get
            {
                return _cancellationTokenSource.Token;
            }
        }

        public RawIrDACommunicationWindow(IrDADevice irdaDevice)
        {
            InitializeComponent();
            _irdaDevice = irdaDevice;
            this.Closing += RawIrDACommunicationWindow_Closing;
        }

        private void RawIrDACommunicationWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private void send_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    List<byte> send = new List<byte>();
                    foreach (string b in send_textbox.Text.Split(' '))
                    {
                        send.Add(Convert.ToByte(b));
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        responses_textblock.Text += string.Format("> {0}", string.Join(" ", send.ToList().ConvertAll(x => x.ToString()).ToArray())) + Environment.NewLine;
                        responses_scrollviewer.ScrollToBottom();
                    });
                    _irdaDevice.TransmitAsync(send.ToArray(), _cancellationToken).ContinueWith((task) =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            this.Dispatcher?.Invoke(() =>
                            {
                                responses_textblock.Text += string.Format("< {0}", string.Join(" ", task.Result.ToList().ConvertAll(x => x.ToString()).ToArray())) + Environment.NewLine;
                                responses_scrollviewer.ScrollToBottom();
                            });
                        }
                    });
                }
                catch (Exception) 
                {
                }
                finally
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        send_textbox.Clear();
                    });
                }
                
            }
        }
    }
}
