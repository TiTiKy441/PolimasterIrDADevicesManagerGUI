using Microsoft.Win32;
using PolimasterIrDADevicesManagerGUI.Device;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;

namespace PolimasterIrDADevicesManagerGUI.GUI
{
    /// <summary>
    /// Interaction logic for EEPROMManagerWindow.xaml
    /// </summary>
    public partial class EEPROMManagerWindow : Window
    {

        public readonly IDirectEEPROMAccessDevice Device;

        private CancellationTokenSource _cancellationTokenSource = new();

        private CancellationToken _cancellationToken
        {
            get
            {
                return _cancellationTokenSource.Token;
            }
        }

        private Dictionary<ushort, MemoryValue> _valuesDictionary = new();

        public const string ModuleName = nameof(EEPROMManagerWindow);

        public EEPROMManagerWindow(IDirectEEPROMAccessDevice irdaDevice)
        {
            InitializeComponent();
            Device = irdaDevice;
            this.Closing += EEPROMManagerWindow_Closing;
        }

        private void EEPROMManagerWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void read_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            ushort startAddress = Convert.ToUInt16(start_address_textblock.Text);
            ushort endAddress = Convert.ToUInt16(end_address_textblock.Text);
            if (startAddress > endAddress) return;

            Task.Run(async () =>
            {
                for (ushort i = startAddress; i <= endAddress; i += 2)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Title = string.Format("Reading (address {0})...", i);
                    });
                    try
                    {
                        _cancellationToken.ThrowIfCancellationRequested();

                        byte[] bytes = await Device.ReadBytesFromEEPROMAsync(i, _cancellationToken);

                        _valuesDictionary[(ushort)(i + 1)] = new MemoryValue((ushort)(i + 1), bytes[1]);
                        _valuesDictionary[(ushort)i] = new MemoryValue(i, bytes[0], _valuesDictionary[(ushort)(i + 1)]);
                        if (_valuesDictionary.ContainsKey((ushort)(i - 1))) _valuesDictionary[(ushort)(i - 1)].NextValue = _valuesDictionary[i];
                        else Logger.Log(string.Format("Memory display sequencing tail at address {0}", i), ModuleName);

                        this.Dispatcher.Invoke(() =>
                        {
                            RenderList();
                        });
                    }
                    catch (Exception)
                    {

                    }
                }
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    MessageBox.Show(t.Exception?.Message, "Read failed");
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Title = "Read completed";
                    });
                }
            });
        }

        private void write_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                List<ushort> writtenAddress = new(_valuesDictionary.Values.Count);
                foreach (MemoryValue value in _valuesDictionary.Values)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    if (writtenAddress.Contains(value.Address)) continue;
                    this.Dispatcher.Invoke(() =>
                    {
                        Title = string.Format("Writing (address {0})...", value.Address);
                    });
                    if (_valuesDictionary.ContainsKey((ushort)(value.Address + 1)))
                    {
                        await Device.WriteBytesToEEPROMAsync(value.Address, new byte[] { value.ByteValue, _valuesDictionary[(ushort)(value.Address + 1)].ByteValue }, _cancellationToken);
                        writtenAddress.Add(value.Address);
                        writtenAddress.Add((ushort)(value.Address + 1));
                    }
                    else
                    {
                        await Device.WriteByteToEEPROMAsync(value.Address, value.ByteValue, _cancellationToken);
                        writtenAddress.Add(value.Address);
                    }
                }
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    MessageBox.Show(t.Exception?.Message, "Write failed");
                }
                else
                {
                    this.Dispatcher.Invoke(() => 
                    {
                        Title = "Write completed";
                    });
                }
            });
        }


        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            memory_table_listview.Items.Clear();
            _valuesDictionary.Clear();
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = false,
                Title = "Save memory"
            };
            bool? result = fileDialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    string fileName = fileDialog.FileName;
                    StringBuilder builder = new(_valuesDictionary.Count);
                    foreach (MemoryValue value in _valuesDictionary.Values)
                    {
                        builder = builder.AppendLine(string.Format(">{0}={1}", value.Address, value.ByteValue));
                    }
                    File.WriteAllText(fileName, builder.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to save file");
                }
            }
        }

        private void load_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                Title = "Load memory file"
            };
            bool? result = fileDialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    memory_table_listview.Items.Clear();
                    _valuesDictionary.Clear();
                    string fileName = fileDialog.FileName;
                    string[] lines = File.ReadAllLines(fileName);
                    foreach (string line in lines)
                    {
                        if (!line.StartsWith(">")) continue;
                        if (!line.Contains('=')) continue;
                        ushort address = Convert.ToUInt16(line.Split('=')[0].Remove(0, 1));
                        byte value = Convert.ToByte(line.Split('=')[1]);

                        _valuesDictionary[(ushort)address] = new MemoryValue(address, value);
                        
                    }
                    foreach (MemoryValue value in _valuesDictionary.Values)
                    {
                        if (_valuesDictionary.ContainsKey((ushort)(value.Address - 1))) _valuesDictionary[(ushort)(value.Address - 1)].NextValue = _valuesDictionary[value.Address];
                        else Logger.Log(string.Format("Memory display sequencing tail at address {0}", value.Address), ModuleName);
                    }
                    RenderList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to load file");
                }
            }
        }

        private void RenderList()
        {
            using (Dispatcher.DisableProcessing())
            {
                memory_table_listview.Items.Clear();
                foreach (MemoryValue value in _valuesDictionary.Values.ToArray().OrderBy(s => s.Address))
                {
                    memory_table_listview.Items.Add(value);
                }
            }
        }

        private class MemoryValue : INotifyPropertyChanged
        {
            public ushort Address { get; }

            private byte _byteValue;
            public byte ByteValue
            {
                get
                {
                    return _byteValue;
                }
                set
                {
                    _byteValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }

            public string BitValue
            {
                get
                {
                    return ByteHelper.GetBoolArrayAsString(ByteHelper.GetBits(ByteValue).ToArray());
                }

                set
                {
                    List<bool> bits = new();
                    foreach (char c in value)
                    {
                        if ((c != '1') && (c != '0')) return;
                        bits.Add(c == '1');
                    }
                    if (bits.Count != 8) return;

                    ByteValue = ByteHelper.ReadByte(bits.ToArray());
                }
            }

            public MemoryValue? NextValue;

            public ushort UshortValue
            {
                get
                {
                    return (ushort)((int)ByteValue | (int)(NextValue is null ? 0 : NextValue.UshortValue) << 8);
                }
                set
                {
                    byte[] bytes = new byte[] { (byte)value, (byte)(value >> 8) };
                    if ((bytes[1] != 0) && (NextValue == null)) throw new Exception();
                    ByteValue = bytes[0];
                    if (NextValue != null) NextValue.ByteValue = bytes[1];
                }
            }

            public MemoryValue(ushort address, byte byteValue, MemoryValue? nextValue = null)
            {
                Address = address;
                ByteValue = byteValue;
                NextValue = nextValue;
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
}
