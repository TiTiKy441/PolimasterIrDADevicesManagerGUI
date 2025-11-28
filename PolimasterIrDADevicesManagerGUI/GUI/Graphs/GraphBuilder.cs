using PolimasterIrDADevicesManagerGUI.Device;
using PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations;
using ScottPlot;
using ScottPlot.WPF;
using static PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations.PM1401GNProtocolDevice;

namespace PolimasterIrDADevicesManagerGUI.GUI.Graphs
{
    /**
     * Not very OOP :)
     * 
     * But well, it works, whatever
     **/
    internal static class GraphBuilder
    {

        public static Type[] SupportedEventRecordTypes { get; private set; } = Array.Empty<Type>();


        static GraphBuilder()
        {
            SupportedEventRecordTypes = new Type[] { typeof(PM1401GNProtocolDevice.PM1401GNEventRecord[] ) };
        }

        public static bool IsSupported(Type t)
        {
            return SupportedEventRecordTypes.Contains(t);
        }

        public static WpfPlot GetWpfPlot(HistoryEventRecord[] history, double? calibrationValue = null)
        {
            if (history is PM1401GNProtocolDevice.PM1401GNEventRecord[] pm1401gnHistory)
            {
                if (calibrationValue == null) throw new ArgumentException("You must provide a calibration value for this type of record!");

                DateTime now = DateTime.Now;
                List<double[]> gammaDots = new();
                List<double[]> neutronDots = new();
                List<double[]> gammaAlarmDots = new();
                List<double[]> neutronAlarmDots = new();

                WpfPlot plt = new();

                foreach (PM1401GNEventRecord record1 in pm1401gnHistory)
                {
                    double timeDiff;
                    double neutron;
                    double gamma;
                    double[] neutronDot;
                    double[] gammaDot;
                    switch (record1.Type)
                    {
                        case PM1401GNEventType.CalibrationDoneCPS:
                        case PM1401GNEventType.BackgroundRecordCPS:

                        case PM1401GNEventType.CalibrationDoneDoseRate:
                        case PM1401GNEventType.BackgroundRecordDoseRate:
                        case PM1401GNEventType.AlarmGammaDoseRate:

                        case PM1401GNEventType.AlarmNeutron:
                        case PM1401GNEventType.AlarmGammaCPS:
                            timeDiff = record1.TimeStamp.ToOADate();
                            neutron = Convert.ToDouble(record1.GetStandardizedValue2());
                            gamma = (double)(Convert.ToDouble(record1.GetStandardizedValue1()) * (((record1.Type == PM1401GNEventType.AlarmGammaDoseRate) || (record1.Type == PM1401GNEventType.BackgroundRecordDoseRate) || (record1.Type == PM1401GNEventType.CalibrationDoneDoseRate)) ? calibrationValue : 1));
                            gammaDot = new double[2] { timeDiff, gamma };
                            neutronDot = new double[2] { timeDiff, neutron };

                            if (record1.Type == PM1401GNEventType.AlarmNeutron) neutronAlarmDots.Add(neutronDot);
                            neutronDots.Add(neutronDot);
                            gammaDots.Add(gammaDot);
                            if ((record1.Type == PM1401GNEventType.AlarmGammaDoseRate) || (record1.Type == PM1401GNEventType.AlarmGammaCPS)) gammaAlarmDots.Add(gammaDot);
                            break;

                    }
                }
                List<double> gammaTimeDots = new(gammaDots.Count);
                List<double> gammaValueDots = new(gammaDots.Count);
                foreach (double[] values in gammaDots)
                {
                    gammaTimeDots.Add(values[0]);
                    gammaValueDots.Add(values[1]);
                }
                plt.Plot.Add.ScatterLine(gammaTimeDots, gammaValueDots);

                List<double> gammaAlarmTimeDots = new(gammaAlarmDots.Count);
                List<double> gammaAlarmValueDots = new(gammaAlarmDots.Count);
                foreach (double[] values in gammaAlarmDots)
                {
                    gammaAlarmTimeDots.Add(values[0]);
                    gammaAlarmValueDots.Add(values[1]);
                }
                plt.Plot.Add.Scatter(gammaAlarmTimeDots, gammaAlarmValueDots).LineWidth = 0;

                List<double> neutronTimeDots = new(neutronDots.Count);
                List<double> neutronValueDots = new(neutronDots.Count);
                foreach (double[] values in neutronDots)
                {
                    neutronTimeDots.Add(values[0]);
                    neutronValueDots.Add(values[1]);
                }
                plt.Plot.Add.ScatterLine(neutronTimeDots, neutronValueDots);

                List<double> neutronAlarmTimeDots = new(neutronAlarmDots.Count);
                List<double> neutronAlarmValueDots = new(neutronAlarmDots.Count);
                foreach (double[] values in neutronAlarmDots)
                {
                    neutronAlarmTimeDots.Add(values[0]);
                    neutronAlarmValueDots.Add(values[1]);
                }
                plt.Plot.Add.Scatter(neutronAlarmTimeDots, neutronAlarmValueDots).LineWidth = 0;

                plt.Plot.Legend.IsVisible = true;

                LegendItem item1 = new()
                {
                    LineColor = Colors.Blue,
                    MarkerFillColor = Colors.Blue,
                    MarkerLineColor = Colors.Blue,
                    LineWidth = 1,
                    LabelText = "Gamma"
                };

                LegendItem item2 = new()
                {
                    LineColor = Colors.Green,
                    MarkerFillColor = Colors.Green,
                    MarkerLineColor = Colors.Green,
                    LineWidth = 1,
                    LabelText = "Neutron"
                };

                LegendItem item3 = new()
                {
                    MarkerColor = Colors.Orange,
                    MarkerFillColor = Colors.Orange,
                    MarkerLineColor = Colors.Orange,
                    MarkerSize = 5,
                    MarkerShape = MarkerShape.FilledCircle,
                    LabelText = "Gamma alarm"
                };

                LegendItem item4 = new()
                {
                    MarkerColor = Colors.Red,
                    MarkerFillColor = Colors.Red,
                    MarkerLineColor = Colors.Red,
                    MarkerSize = 5,
                    MarkerShape = MarkerShape.FilledCircle,
                    LabelText = "Neutron alarm"
                };

                LegendItem[] items = { item1, item2, item3, item4 };
                plt.Plot.ShowLegend(items);

                //plt.Title(string.Format("PM1401GN #{0} records", device.ReadSerialNumber()));
                plt.Plot.XLabel("Date and time");
                plt.Plot.YLabel("Countrate (CPS)");
                plt.Plot.Axes.DateTimeTicksBottom();

                //plt.SavePng("test.png", 800, 600).LaunchFile();
                return plt;
            }
            throw new ArgumentException("Plotting not supported");
        }
    }
}
