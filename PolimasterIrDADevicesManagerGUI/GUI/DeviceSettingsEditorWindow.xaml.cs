using PolimasterIrDADevicesManagerGUI.Device;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.Windows;
using System.Windows.Controls;

namespace PolimasterIrDADevicesManagerGUI.GUI
{
    /// <summary>
    /// Interaction logic for DeviceSettingsEditorWindow.xaml
    /// </summary>
    public partial class DeviceSettingsEditorWindow : Window
    {

        public readonly ISettingsAccessDevice Device;

        private CancellationTokenSource _cancellationTokenSource = new();

        private CancellationToken _cancellationToken
        {
            get
            {
                return _cancellationTokenSource.Token;
            }
        }

        private Dictionary<DeviceParameterInfo, TextBox> _parameterValueTextBoxes = new();

        private Dictionary<DeviceParameterInfo, Label> _parameterValueLabels = new();

        private Dictionary<DeviceParameterInfo, CheckBox> _parameterValueCheckboxes = new();

        public const string ModuleName = nameof(DeviceSettingsEditorWindow);

        public DeviceSettingsEditorWindow(ISettingsAccessDevice device)
        {
            InitializeComponent();
            Device = device;
            using (parameters_stackpanel.Dispatcher.DisableProcessing())
            {
                foreach (DeviceParameterInfo parameter in device.GetSupportedParameters())
                {
                    WrapPanel parameterWrapPanel = new WrapPanel();
                    parameterWrapPanel.Children.Add(new Label() { Content = parameter.Name, });
                    if (parameter.ValueType == typeof(bool))
                    {
                        CheckBox checkBox = new CheckBox() { IsEnabled = parameter.Editable };
                        _parameterValueCheckboxes.Add(parameter, checkBox);
                        parameterWrapPanel.Children.Add(checkBox);
                    }
                    else
                    {
                        if (parameter.Editable)
                        {
                            TextBox parameterValueTextBox = new TextBox() { Text = string.Empty, Width = 130, Height = 20, IsReadOnly = !parameter.Editable };
                            _parameterValueTextBoxes.Add(parameter, parameterValueTextBox);
                            parameterWrapPanel.Children.Add(parameterValueTextBox);
                        }
                        else
                        {
                            Label parameterValueLabel = new Label() { Content = string.Empty, };
                            _parameterValueLabels.Add(parameter, parameterValueLabel);
                            parameterWrapPanel.Children.Add(parameterValueLabel);
                        }
                    }

                    parameterWrapPanel.Children.Add(new Label() { Content = parameter.Units, });
                    parameters_stackpanel.Children.Add(parameterWrapPanel);
                }
            }
        }

        private void cancel_tasks_button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                _cancellationTokenSource.Cancel();
                await Task.Delay(0);
                if (!_cancellationTokenSource.TryReset()) _cancellationTokenSource = new();
            });
        }

        private string GetDisplayedParameterValue(DeviceParameterInfo info)
        {
            if (info.ValueType == typeof(bool))
            {
                return ((bool)_parameterValueCheckboxes[info].IsChecked) ? "1" : "0";
            }
            if (info.Editable)
            {
                return _parameterValueTextBoxes[info].Text;
            }
            return _parameterValueLabels[info].Content.ToString();
        }

        private void SetDisplayedParameterValue(DeviceParameterInfo info, string value)
        {
            if (info.ValueType == typeof(bool))
            {
                _parameterValueCheckboxes[info].IsChecked = ((value == "1") || (value == "True"));
                return;
            }
            if (info.Editable)
            {
                _parameterValueTextBoxes[info].Text = value;
                return;
            }
            _parameterValueLabels[info].Content = value;
            return;
        }

        private void AddTaskToStats(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                this.Dispatcher.Invoke(() =>
                {
                    running_tasks_label.Content = Convert.ToInt32(running_tasks_label.Content) - 1;
                    successfull_tasks_label.Content = Convert.ToInt32(successfull_tasks_label.Content) + 1;
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    running_tasks_label.Content = Convert.ToInt32(running_tasks_label.Content) - 1;
                    failed_tasks_label.Content = Convert.ToInt32(failed_tasks_label.Content) + 1;
                });
                if (task.IsFaulted)
                {
                    Logger.Error(string.Format("Task failed with ({0})", task.Exception?.Message), ModuleName);
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                if ((Convert.ToInt32(running_tasks_label.Content) > 0) && (fetch_all_button.IsEnabled || save_to_device_button.IsEnabled))
                {
                    fetch_all_button.IsEnabled = false;
                    save_to_device_button.IsEnabled = false;
                }
                else if (Convert.ToInt32(running_tasks_label.Content) == 0)
                {
                    fetch_all_button.IsEnabled = true;
                    save_to_device_button.IsEnabled = true;
                }
            });
        }

        private void fetch_all_button_Click(object sender, RoutedEventArgs e)
        {
            DeviceParameterInfo[] parameters = Device.GetSupportedParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                DeviceParameterInfo parameter = parameters[i];
                running_tasks_label.Content = Convert.ToInt32(running_tasks_label.Content) + 1;
                Device.ReadParameterAsync(parameter.Id, _cancellationToken).ContinueWith((task) =>
                {
                    AddTaskToStats(task);

                    if (task.IsCompletedSuccessfully)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            SetDisplayedParameterValue(parameter, task.Result?.ToString());
                        });
                    }
                });
            }
        }

        private void save_to_device_button_Click(object sender, RoutedEventArgs e)
        {
            foreach (DeviceParameterInfo parameter in Device.GetSupportedParameters())
            {
                if (!parameter.Editable) continue;
                string stringValue = GetDisplayedParameterValue(parameter);
                running_tasks_label.Content = Convert.ToInt32(running_tasks_label.Content) + 1;
                Device.WriteParameterAsStringAsync(parameter.Id, stringValue, _cancellationToken).ContinueWith((task) =>
                {
                    AddTaskToStats(task);
                });
            }
        }
    }
}
