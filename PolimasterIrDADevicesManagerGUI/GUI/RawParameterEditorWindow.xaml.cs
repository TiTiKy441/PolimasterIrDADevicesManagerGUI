using PolimasterIrDADevicesManagerGUI.Device.Protocols;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.ComponentModel;
using System.Windows;

namespace PolimasterIrDADevicesManagerGUI.GUI
{
    /// <summary>
    /// Interaction logic for CommandMapperWindow.xaml
    /// </summary>
    public partial class RawParameterEditorWindow : Window
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

        private List<MCValue> _values = new();

        public RawParameterEditorWindow(PMDeviceBaseProtocol device)
        {
            InitializeComponent();
            Device = device;
        }

        private void discover_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new();
            values_table_listview.Items.Clear();
            Task.Run(async () =>
            {
                for (byte i = 0; (i < byte.MaxValue) && !_cancellationToken.IsCancellationRequested; i++)
                {
                    if (AvoidValues.Contains(i)) continue;

                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            Title = string.Format("Discovering (address {0})...", i);
                        });
                        byte[] read = await Device.ReadFromMCAsync(i, _cancellationToken);

                        bool editable = false;

                        try
                        {
                            if (read.Length == 2)
                            {
                                await Device.Write2ToMCAsync(i, read, _cancellationToken);
                            }
                            else
                            {
                                await Device.WriteArrToMCAsync(i, read, _cancellationToken);
                            }
                            editable = true;
                        }
                        catch (Exception)
                        {

                        }

                        MCValue val = new MCValue(i, read, editable);
                        _values.Add(val);
                        this.Dispatcher.Invoke(() =>
                        {
                            values_table_listview.Items.Add(val);
                        });
                    }
                    catch (Exception)
                    {
                    }
                }
            }).ContinueWith(t =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (t.IsFaulted)
                    {
                        Title = "Discovery failed";
                        MessageBox.Show(t.Exception?.Message, "Unable to discover");
                    }
                    else
                    {
                        Title = "Discovery completed";
                    }
                });
            });
        }

        private void read_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new();
            Task.Run(async () =>
            {
                foreach (MCValue value in _values)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Title = string.Format("Reading (address {0})...", value.Address);
                    });
                    value.Bytes = await Device.ReadFromMCAsync(value.Address, _cancellationToken);
                }
            }).ContinueWith(t =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (t.IsFaulted)
                    {
                        Title = "Reading failed";
                        MessageBox.Show(t.Exception?.Message, "Unable to read");
                    }
                    else
                    {
                        Title = "Reading completed";
                    }
                });
            });
        }

        private void write_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new();
            Task.Run(async () =>
            {
                foreach (MCValue value in _values)
                {
                    if (!value.Editable) continue;
                    this.Dispatcher.Invoke(() =>
                    {
                        Title = string.Format("Writing (address {0})...", value.Address);
                    });

                    if (value.Bytes.Length == 2)
                    {
                        await Device.Write2ToMCAsync(value.Address, value.Bytes, _cancellationToken);
                    }
                    else
                    {
                        await Device.WriteArrToMCAsync(value.Address, value.Bytes, _cancellationToken );
                    }
                }
            }).ContinueWith(t =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (t.IsFaulted)
                    {
                        Title = "Writing failed";
                        MessageBox.Show(t.Exception?.Message, "Unable to write");
                    }
                    else
                    {
                        Title = "Writing completed";
                    }
                });
            });
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private void RenderList()
        {
            using (Dispatcher.DisableProcessing())
            {
                values_table_listview.Items.Clear();
                foreach (MCValue value in _values.OrderBy(s => s.Address))
                {
                    values_table_listview.Items.Add(value);
                }
            }
        }


        private class MCValue : INotifyPropertyChanged
        {

            public byte Address { get; }

            public byte[] _bytes;
            public byte[] Bytes 
            { 
                get
                {
                    return _bytes;
                }
                set
                {
                    _bytes = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }

            public string BytesValueAsString
            {
                get
                {
                    string output = string.Empty;
                    foreach (byte b in Bytes)
                    {
                        output += b.ToString() + " ";
                    }
                    return output;
                }
                set
                {
                    string[] splits = value.Split(' ');
                    List<byte> bytes = new(splits.Length);
                    foreach (string split in splits)
                    {
                        if (byte.TryParse(split, out byte result))
                        {
                            bytes.Add(result);
                        }
                    }
                    Bytes = bytes.ToArray();
                }
            }

            public ushort? UshortValue
            {
                get
                {
                    if (_bytes.Length != 2)
                    {
                        return null;
                    }
                    return ByteHelper.ReadUshort(Bytes);
                }
                set
                {
                    if ((_bytes.Length != 2) || (value is null))
                    {
                        return;
                    }
                    byte[] n = new byte[2];
                    ByteHelper.WriteUshort(n, (ushort)value);
                    Bytes = n;
                }
            }

            private bool _editable;
            public bool Editable
            {
                get
                {
                    return _editable;
                }
                set
                {   
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public MCValue(byte address, byte[] bytesValues, bool editable)
            {
                Address = address;
                _bytes = bytesValues;
                _editable = editable;
            }
        }
    }
}
