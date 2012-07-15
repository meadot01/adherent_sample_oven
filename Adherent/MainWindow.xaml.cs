using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Layouts;
using AdherentSampleOven.DataObjects;
using AdherentSampleOven.HardwareInterface;


namespace AdherentSampleOven
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    
    public partial class MainWindow : Window
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static Logger sampleLogger = NLog.LogManager.GetLogger("SampleLogger");
        private Boolean running = false;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private DataObjects.Settings settings = null;
        private HardwareInterface.SampleOvenManager ovenManager;

        public MainWindow()
        {
            InitializeComponent();
            logger.Trace("After InitializeComponent");
            addStationControlToGrid(sampleGrid);
            logger.Trace("Added station control to grid");
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            logger.Trace("Timer created");
            LoggingConfiguration config = LogManager.Configuration;
            foreach (Target currentTarget in config.AllTargets)
            {
                if (currentTarget is FileTarget)
                {
                    if (currentTarget.Name.StartsWith("sampleFile"))
                    {

                        FileTarget standardTarget = currentTarget as FileTarget;
                        string expandedFileName = NLog.Layouts.SimpleLayout.Evaluate(standardTarget.FileName.ToString());
                        logFileLocationTextBlock.Text = expandedFileName;
                        break;
                    }
                }
            } 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logger.Trace("Wndow Loaded");

        }

        private void configMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Config2 configWindow = new Config2();
            configWindow.Owner = this;
            configWindow.ShowDialog();
        }


        private void printMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Grid printSampleGrid = new Grid();
            printSampleGrid.Name = "printSampleGrid";
            printSampleGrid.ShowGridLines = true;
            RowDefinition sampleRowdef1 = new RowDefinition();
            sampleRowdef1.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition sampleRowdef2 = new RowDefinition();
            sampleRowdef2.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition sampleRowdef3 = new RowDefinition();
            sampleRowdef3.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition sampleRowdef4 = new RowDefinition();
            sampleRowdef4.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition sampleRowdef5 = new RowDefinition();
            sampleRowdef5.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition sampleRowdef6 = new RowDefinition();
            sampleRowdef6.Height = new GridLength(1, GridUnitType.Star);

            ColumnDefinition sampleColdef1 = new ColumnDefinition();
            sampleColdef1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition sampleColdef2 = new ColumnDefinition();
            sampleColdef2.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition sampleColdef3 = new ColumnDefinition();
            sampleColdef3.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition sampleColdef4 = new ColumnDefinition();
            sampleColdef4.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition sampleColdef5 = new ColumnDefinition();
            sampleColdef5.Width = new GridLength(1, GridUnitType.Star);
            printSampleGrid.RowDefinitions.Add(sampleRowdef1);
            printSampleGrid.RowDefinitions.Add(sampleRowdef2);
            printSampleGrid.RowDefinitions.Add(sampleRowdef3);
            printSampleGrid.RowDefinitions.Add(sampleRowdef4);
            printSampleGrid.RowDefinitions.Add(sampleRowdef5);
            printSampleGrid.RowDefinitions.Add(sampleRowdef6);
            printSampleGrid.ColumnDefinitions.Add(sampleColdef1);
            printSampleGrid.ColumnDefinitions.Add(sampleColdef2);
            printSampleGrid.ColumnDefinitions.Add(sampleColdef3);
            printSampleGrid.ColumnDefinitions.Add(sampleColdef4);
            printSampleGrid.ColumnDefinitions.Add(sampleColdef5);

            addStationControlToGrid(printSampleGrid);
            updateSampleGrid(printSampleGrid);

            DockPanel printDockPanel = new DockPanel();
            printDockPanel.Margin = new Thickness(80, 60, 80, 60);

            TextBlock nameLabelText = new TextBlock();
            nameLabelText.Text = "Name: ________________________________";
            nameLabelText.Padding = new Thickness(20, 0, 0, 20);
            nameLabelText.FontSize = 20;
            printDockPanel.Children.Add(nameLabelText);
            DockPanel.SetDock(nameLabelText, Dock.Top);

            printDockPanel.Children.Add(printSampleGrid);
            DockPanel.SetDock(printSampleGrid, Dock.Bottom);

 
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
            {

                var printCapabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

                var size = new Size(printCapabilities.PageImageableArea.ExtentWidth,
                     printCapabilities.PageImageableArea.ExtentHeight);

                printDockPanel.Measure(size);
                printDockPanel.Arrange(new Rect(new Point(printCapabilities.PageImageableArea.OriginWidth,
                    printCapabilities.PageImageableArea.OriginHeight), size));

                printDlg.PrintVisual(printDockPanel, "Print ListView");
            }
        }

        private void addStationControlToGrid(Grid aGrid)
        {
            int stationNumber = 30;
            for (int y = 0; y < 6; y++)
            {
                for (int x = 4; x >= 0; x--)
                {

                    Grid currentSampleGrid = new Grid();
                    currentSampleGrid.Name = "stationGrid" + stationNumber;

                    RowDefinition rowdef1 = new RowDefinition();
                    rowdef1.Height = new GridLength(2, GridUnitType.Star);
                    RowDefinition rowdef2 = new RowDefinition();
                    rowdef2.Height = new GridLength(2, GridUnitType.Star);
                    RowDefinition rowdef3 = new RowDefinition();
                    rowdef3.Height = new GridLength(2, GridUnitType.Star);
                    RowDefinition rowdef4 = new RowDefinition();
                    rowdef4.Height = new GridLength(2, GridUnitType.Star);

                    ColumnDefinition coldef1 = new ColumnDefinition();
                    coldef1.Width = new GridLength(4, GridUnitType.Star);
                    ColumnDefinition coldef2 = new ColumnDefinition();
                    coldef2.Width = new GridLength(3, GridUnitType.Star);

                    currentSampleGrid.RowDefinitions.Add(rowdef1);
                    currentSampleGrid.RowDefinitions.Add(rowdef2);
                    currentSampleGrid.RowDefinitions.Add(rowdef3);
                    currentSampleGrid.RowDefinitions.Add(rowdef4);
                    currentSampleGrid.ColumnDefinitions.Add(coldef1);
                    currentSampleGrid.ColumnDefinitions.Add(coldef2);


                    TextBlock stationLabelText = new TextBlock();
                    stationLabelText.Text = "Station: " + stationNumber;
                    Viewbox stationLabelViewbox = new Viewbox();
                    stationLabelViewbox.Child = stationLabelText;
                    currentSampleGrid.Children.Add(stationLabelViewbox);
                    Grid.SetColumn(stationLabelViewbox, 0);
                    Grid.SetRow(stationLabelViewbox, 0);
                    Grid.SetColumnSpan(stationLabelViewbox, 2);

                    TextBlock elapsedTimeLabelText = new TextBlock();
                    elapsedTimeLabelText.Text = "Elapsed Time:";
                    Viewbox elapsedTimeLabelViewbox = new Viewbox();
                    elapsedTimeLabelViewbox.Margin = new Thickness { Left = 8 };
                    elapsedTimeLabelViewbox.Child = elapsedTimeLabelText;
                    currentSampleGrid.Children.Add(elapsedTimeLabelViewbox);
                    Grid.SetColumn(elapsedTimeLabelViewbox, 0);
                    Grid.SetRow(elapsedTimeLabelViewbox, 2);

                    TextBlock temperatureLabelText = new TextBlock();
                    temperatureLabelText.Text = "Temperature:";
                    Viewbox temperatureLabelViewbox = new Viewbox();
                    temperatureLabelViewbox.Margin = new Thickness { Left = 8 };
                    temperatureLabelViewbox.Child = temperatureLabelText;
                    currentSampleGrid.Children.Add(temperatureLabelViewbox);
                    Grid.SetColumn(temperatureLabelViewbox, 0);
                    Grid.SetRow(temperatureLabelViewbox, 3);

                    TextBlock elapsedTimeValueText = new TextBlock();
                    elapsedTimeValueText.Name = "elapsedTimeValueText" + stationNumber;
                    Viewbox elapsedTimeValueViewbox = new Viewbox();
                    elapsedTimeValueViewbox.Margin = new Thickness { Left = 8, Right = 8 };
                    elapsedTimeValueViewbox.Child = elapsedTimeValueText;
                    elapsedTimeValueViewbox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    currentSampleGrid.Children.Add(elapsedTimeValueViewbox);
                    Grid.SetColumn(elapsedTimeValueViewbox, 1);
                    Grid.SetRow(elapsedTimeValueViewbox, 2);

                    TextBlock temperatureValueText = new TextBlock();
                    temperatureValueText.Name = "temperatureValueText" + stationNumber; 
                    Viewbox temperatureValueViewbox = new Viewbox();
                    temperatureValueViewbox.Margin = new Thickness { Left = 8, Right = 8 };
                    temperatureValueViewbox.Child = temperatureValueText;
                    temperatureValueViewbox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    currentSampleGrid.Children.Add(temperatureValueViewbox);
                    Grid.SetColumn(temperatureValueViewbox, 1);
                    Grid.SetRow(temperatureValueViewbox, 3);

                    aGrid.Children.Add(currentSampleGrid);
                    Grid.SetColumn(currentSampleGrid, x);
                    Grid.SetRow(currentSampleGrid, y);
                    stationNumber--;
                }
            }
            


        }

        private void startStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (running)
            {
                stopRun();

            }
            else
            {
                startRun();
            }
            

        }

        private void startRun()
        {
            sampleLogger.Info("Run Started");
            settings = DataObjects.SettingsManager.Instance.ApplicationSettings;
            SolidColorBrush enabledBrush = new SolidColorBrush(Colors.White);
            SolidColorBrush disabledBrush = new SolidColorBrush(Colors.WhiteSmoke);
            for (byte i = 1; i <= 30; i++)
            {
                Object stationObject = LogicalTreeHelper.FindLogicalNode(sampleGrid, "stationGrid" + i);
                if (stationObject is Grid)
                {
                    Grid stationGrid = stationObject as Grid;
                    if (settings.SampleConfigurationDictionary.ContainsKey(i) && settings.SampleConfigurationDictionary[i] != null)
                    {
                        stationGrid.Background = enabledBrush;
                    }
                    else
                    {
                        stationGrid.Background = disabledBrush;
                    }
                }

            }
            ovenManager = new SampleOvenManager(settings);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
            startStopButton.Content = "Stop";
            configMenuItem.IsEnabled = false;
            running = true;
        }

        private void stopRun()
        {
            sampleLogger.Info("Run Stopped");
            dispatcherTimer.Stop();
            startStopButton.Content = "Start";
            configMenuItem.IsEnabled = true;
            running = false;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            logger.Trace("Timer Tick");
            ovenManager.updateResults();
            timeFromStartValue.Text = ovenManager.ElapsedTime.ToString(@"hh\:mm\:ss");
            statusText.Text = ovenManager.StatusMessage;
            statusText.ToolTip = ovenManager.StatusMessage;
            if (settings.TemperatureFormat == TemperatureFormatEnum.Farenheit)
            {
                currentTemperatureValue.Text = System.Math.Round(ovenManager.OvenTemperature) + "°F";
            } else
            {
                currentTemperatureValue.Text = System.Math.Round(ovenManager.OvenTemperature) + "°C";
            }
            updateSampleGrid(sampleGrid);
            if (ovenManager.RunCompleted)
            {
                sampleLogger.Info("Run Completed - all samples triggered");
                stopRun();
                runCompletedText.Visibility = Visibility.Visible;
            }
            else
            {
                runCompletedText.Visibility = Visibility.Hidden;
            }

        }

        private void updateSampleGrid(Grid aGrid)
        {
            if (ovenManager != null && ovenManager.SampleDictionary != null)
            {
                for (byte i = 0; i <= 30; i++)
                {
                    if (ovenManager.SampleDictionary.ContainsKey(i))
                    {
                        updateSampleBlock(aGrid, i, ovenManager.SampleDictionary[i]);
                    }
                    else
                    {
                        updateSampleBlock(aGrid, i, null);
                    }
                }
            }

        }

        private void updateSampleBlock(Grid aGrid, byte sampleNumber, SampleOvenManager.SampleData? sampleData)
        {

            Object timeObject = LogicalTreeHelper.FindLogicalNode(aGrid, "elapsedTimeValueText" + sampleNumber);
            if (timeObject is TextBlock)
            {
                TextBlock timeTextBlock = timeObject as TextBlock;
                if (sampleData.HasValue)
                {
                    timeTextBlock.Text = sampleData.Value.finalTime.ToString(@"hh\:mm");
                }
                else
                {
                    timeTextBlock.Text = "";
                }
            }
            Object tempObject = LogicalTreeHelper.FindLogicalNode(aGrid, "temperatureValueText" + sampleNumber);
            if (tempObject is TextBlock)
            {
                TextBlock elapsedTemperatureTextBlock = tempObject as TextBlock;
                if (sampleData.HasValue)
                {
                    if (settings.TemperatureFormat == TemperatureFormatEnum.Farenheit)
                    {
                        elapsedTemperatureTextBlock.Text = System.Math.Round(sampleData.Value.finalTemp) + "°F";
                    }
                    else
                    {
                        elapsedTemperatureTextBlock.Text = System.Math.Round(sampleData.Value.finalTemp) + "°C";
                    }
                }
                else
                {
                    elapsedTemperatureTextBlock.Text = "";
                }
            }
        }





        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (dispatcherTimer != null) dispatcherTimer.Stop();
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        } 

    }
}
