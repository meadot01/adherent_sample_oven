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
        // logger - main logger
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // sampleLogger - logger to log only sample run information - should all be INFO level
        private static Logger sampleLogger = NLog.LogManager.GetLogger("SampleLogger");
        // running - is a sample run currently in progress?
        private Boolean running = false;
        // dispatchTimer - created during a run - device will be checked and display updated after every tick
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        // settings - Application settings
        private DataObjects.Settings settings = null;
        // ovenManager - instance of oven manager to read hardware
        private HardwareInterface.SampleOvenManager ovenManager;
        // lastTimeNoError - the last time the devices were scanned with no error
        private DateTime lastTimeNoError = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            logger.Trace("After InitializeComponent");
            // Add all the sample stations to the main grid
            addStationControlToGrid(sampleGrid);
            logger.Trace("Added station control to grid");
            // Create a dispatch timer that will be triggered every second.
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            logger.Trace("Timer created");

            // Search for the NLog config named sampleFile (or wrapped_sampleFile) and show the location on the screen
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

        /*
         * configMenuItem_Click
         *   Handles the configMenuItem_Click event - open the Config2 Dialog 
         */
        private void configMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Config2 configWindow = new Config2();
            configWindow.Owner = this;
            configWindow.ShowDialog();
        }

        /*
         * printMenuItem_Click
         *   Handles the printMenuItem_Click event - builds a DockPanel showing the sample grid and other stuff for print format - then sends to a PrintDlg
         *   
         */
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
            colorDisabledStations(printSampleGrid);
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

                printDlg.PrintVisual(printDockPanel, "Adherent Oven Sample Run");
            }
        }

        /*
         * addStationControlToGrid
         *    Will add the visual components for the 30 sample positions to the given grid
         *    
         */
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

        /*
         * colorDisabledStations
         *   Shade all stations that have not been configured
         *   
         */
        private void colorDisabledStations(Grid aGrid)
        {
            if (settings != null && settings.SampleConfigurationDictionary != null)
            {
                SolidColorBrush enabledBrush = new SolidColorBrush(Colors.White);
                SolidColorBrush disabledBrush = new SolidColorBrush(Colors.WhiteSmoke);
                for (byte i = 1; i <= 30; i++)
                {
                    Object stationObject = LogicalTreeHelper.FindLogicalNode(aGrid, "stationGrid" + i);
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
            }
        }

        /*
         * startStopButton_Click
         *   Handle the startStopButton_Click event - if a sample run is running then stop it - otherwise start one
         *   
         */
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

        /*
         * startRun
         *    start a new sample run
         *    
         */
        private void startRun()
        {
            sampleLogger.Info("Run Started");
            // Get a fresh copy of the application settings
            settings = DataObjects.SettingsManager.Instance.ApplicationSettings;
            // Shade unused stations
            colorDisabledStations(sampleGrid);
            // Get a new OvenManager
            ovenManager = new SampleOvenManager(settings);
            // Add event handler for dispatch timer tick and start the timer
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
            // Change start/stop button to show stop
            startStopButton.Content = "Stop";
            // Disable the config menu while running
            configMenuItem.IsEnabled = false;
            running = true;
        }

        /*
         * stopRun
         *    stop the current sample run
         *    
         */
        private void stopRun()
        {
            sampleLogger.Info("Run Stopped");
            dispatcherTimer.Stop();
            startStopButton.Content = "Start";
            configMenuItem.IsEnabled = true;
            running = false;
        }

        /*
         * dispatcherTimer_Tick
         *   Handle the dispatcherTimer_Tick event.  Will get new results from the oven Manager and update the display
         *   
         */
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Get currnt device status from ovenManager
            ovenManager.updateResults();
            // Show current elapsed time
            timeFromStartValue.Text = ovenManager.ElapsedTime.ToString(@"hh\:mm\:ss");
            // Show status message - this would contain device errors if any occurred
            statusText.Text = ovenManager.StatusMessage;
            statusText.ToolTip = ovenManager.StatusMessage;
            // Check if status message was empty
            if (String.IsNullOrWhiteSpace(ovenManager.StatusMessage))
            {
                // If empty then reset the last time with no error
                lastTimeNoError = DateTime.Now;
            }
            else
            {
                // If status message contains an error - check how long it has been since no error.  If longer
                // than timeout then log the error and stop the current run.
                if ((DateTime.Now - lastTimeNoError).TotalSeconds > settings.SecondsBeforeErrorTimeout)
                {
                    logger.Error("Run Ending in Error - see log for more details - " + ovenManager.StatusMessage);
                    stopRun();
                    runCompletedText.Visibility = Visibility.Visible;
                }
            }

            // Display current temperature 
            if (settings.TemperatureFormat == TemperatureFormatEnum.Farenheit)
            {
                currentTemperatureValue.Text = System.Math.Round(ovenManager.OvenTemperature) + "°F";
            } else
            {
                currentTemperatureValue.Text = System.Math.Round(ovenManager.OvenTemperature) + "°C";
            }
            // Update station results
            updateSampleGrid(sampleGrid);
            // If all sample stations have been triggered then show completed and stop the current run.
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

        /*
         * updateSampleGrid
         *   update results for all stations in Grid
         *   
         */
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

        /*
         * updateSampleBlock
         *   update the inforation in the given sample station in the Grid
         *   
         */
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




        /*
         * OnClosing
         *   handle the application closing event
         *   
         */
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (dispatcherTimer != null) dispatcherTimer.Stop();
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        } 

    }
}
