using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Serilog;
using Serilog.Sinks.RollingFileAlternate;
using AdherentShear.HardwareInterface;


namespace AdherentShear
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    
    public partial class MainWindow : Window
    {
        // logger - main logger
        //private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // sampleLogger - logger to log only sample run information - should all be INFO level
        //private static Logger sampleLogger = NLog.LogManager.GetLogger("SampleLogger");
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

        private TextBox[] sampleNameTextBoxes = new TextBox[32];

        public MainWindow()
        {
            InitializeComponent();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .WriteTo.RollingFileAlternate("..\\AdherentShearLogs", fileSizeLimitBytes: 100000, retainedFileCountLimit: 30, minimumLevel: Serilog.Events.LogEventLevel.Information)
                //.WriteTo.File("adherent-.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();
            Log.Debug("After InitializeComponent");
            // Add all the sample stations to the main grid
            addStationControlToGrid(sampleGrid, false);
            Log.Debug("Added station control to grid");
            // Create a dispatch timer that will be triggered every second.
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            Log.Debug("Timer created");

            // Search for the NLog config named sampleFile (or wrapped_sampleFile) and show the location on the screen
            /*
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
            */
            startRun();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("Wndow Loaded");

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
            printSampleGrid.ShowGridLines = false;
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

            addStationControlToGrid(printSampleGrid, true);
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

                try
                {
                    printDlg.PrintVisual(printDockPanel, "Adherent Oven Sample Run");
                }
                catch (Exception ex)
                {
                    Log.Warning("Error occurred while printing", ex, null);
                    MessageBox.Show("Error occurred while printing - check log for details");
                }
            }
        }

        /*
         * addStationControlToGrid
         *    Will add the visual components for the 30 sample positions to the given grid
         *    
         */
        private void addStationControlToGrid(Grid aGrid, bool forPrint)
        {
            byte stationNumber = 30;
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
                    coldef1.Width = new GridLength(1, GridUnitType.Star);
                    ColumnDefinition coldef2 = new ColumnDefinition();
                    coldef2.Width = new GridLength(3, GridUnitType.Star);

                    currentSampleGrid.RowDefinitions.Add(rowdef1);
                    currentSampleGrid.RowDefinitions.Add(rowdef2);
                    currentSampleGrid.RowDefinitions.Add(rowdef3);
                    currentSampleGrid.RowDefinitions.Add(rowdef4);
                    currentSampleGrid.ColumnDefinitions.Add(coldef1);
                    currentSampleGrid.ColumnDefinitions.Add(coldef2);

                    if (!forPrint)
                    {
                        Button startStopButton = new Button();
                        startStopButton.Name = "StartButton" + stationNumber;
                        startStopButton.Background = new SolidColorBrush(Colors.Green);
                        startStopButton.Content = "Start";
                        startStopButton.Tag = stationNumber;
                        startStopButton.Click += startStopClicked;
                        currentSampleGrid.Children.Add(startStopButton);
                        Grid.SetColumn(startStopButton, 0);
                        Grid.SetColumnSpan(startStopButton, 2);
                        Grid.SetRow(startStopButton, 1);
                    }

                    Border gridBorder = new Border();
                    gridBorder.BorderBrush = Brushes.Black;
                    gridBorder.BorderThickness = new Thickness(1);
                    currentSampleGrid.Children.Add(gridBorder);
                    Grid.SetColumnSpan(gridBorder, 2);
                    Grid.SetRowSpan(gridBorder, 4);


                    TextBlock stationLabelText = new TextBlock();
                    stationLabelText.Text = "" +  stationNumber;
                    Viewbox stationLabelViewbox = new Viewbox();
                    stationLabelViewbox.Child = stationLabelText;
                    currentSampleGrid.Children.Add(stationLabelViewbox);
                    Grid.SetColumn(stationLabelViewbox, 0);
                    Grid.SetRow(stationLabelViewbox, 0);
                    Grid.SetColumnSpan(stationLabelViewbox, 1);

                    TextBox sampleNameTextBox = new TextBox();
                    sampleNameTextBox.Name = "SampleName" + stationNumber;
                    sampleNameTextBox.Tag = stationNumber;
                    sampleNameTextBoxes[stationNumber] = sampleNameTextBox;
                    sampleNameTextBox.TextChanged += new TextChangedEventHandler(sampleNameChanged);
                    currentSampleGrid.Children.Add(sampleNameTextBox);
                    sampleNameTextBox.Padding = new Thickness { Left = 8, Right = 8 };
                    Grid.SetColumn(sampleNameTextBox, 1);
                    Grid.SetRow(sampleNameTextBox, 0);
                    Grid.SetColumnSpan(sampleNameTextBox, 1);

                    TextBlock startTimeValueText = new TextBlock();
                    startTimeValueText.TextAlignment = TextAlignment.Left;
                    startTimeValueText.FontSize = 8;
                    startTimeValueText.Name = "startTimeValueText" + stationNumber;
                    Viewbox startTimeValueViewbox = new Viewbox();
                    startTimeValueViewbox.HorizontalAlignment = HorizontalAlignment.Left;
                    startTimeValueViewbox.Margin = new Thickness { Left = 8 };
                    startTimeValueViewbox.Child = startTimeValueText;
                    currentSampleGrid.Children.Add(startTimeValueViewbox);
                    Grid.SetColumn(startTimeValueViewbox, 0);
                    Grid.SetRow(startTimeValueViewbox, 2);
                    Grid.SetColumnSpan(startTimeValueViewbox, 2);

                    TextBlock elapsedTimeValueText = new TextBlock();
                    elapsedTimeValueText.TextAlignment = TextAlignment.Left;
                    elapsedTimeValueText.FontSize = 8;
                    elapsedTimeValueText.Name = "elapsedTimeValueText" + stationNumber;
                    Viewbox elapsedTimeValueViewbox = new Viewbox();
                    elapsedTimeValueViewbox.HorizontalAlignment = HorizontalAlignment.Left;
                    elapsedTimeValueViewbox.Margin = new Thickness { Left = 8, Right = 8 };
                    elapsedTimeValueViewbox.Child = elapsedTimeValueText;
                    elapsedTimeValueViewbox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    currentSampleGrid.Children.Add(elapsedTimeValueViewbox);
                    Grid.SetColumn(elapsedTimeValueViewbox, 0);
                    Grid.SetRow(elapsedTimeValueViewbox, 3);
                    Grid.SetColumnSpan(elapsedTimeValueViewbox, 2);

                    aGrid.Children.Add(currentSampleGrid);
                    Grid.SetColumn(currentSampleGrid, x);
                    Grid.SetRow(currentSampleGrid, y);
                    stationNumber--;
                }
            }
        }

        private void sampleNameChanged(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is TextBox)
            {
                TextBox samepleNameTextBox = sender as TextBox;
                byte? stationNumber = samepleNameTextBox.Tag as byte?;
                if (stationNumber != null)
                {
                    byte stn = stationNumber ?? 0;
                    if (ovenManager.SampleDictionary.ContainsKey(stn))
                    {
                        SampleOvenManager.SampleData sampleData = ovenManager.SampleDictionary[stn];
                        sampleData.stationName = samepleNameTextBox.Text;
                        ovenManager.SampleDictionary[stn] = sampleData;
                    }
                }
            }
            //int stationNumber = sender.Tag.toInt;
        }


        private void startStopClicked(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button)
            {
                Button btn = sender as Button;
                byte? stationNumber = btn.Tag as byte?;
                if (stationNumber != null) {
                    byte stn = stationNumber ?? 0;
                    TextBox sampleNameTextBox = sampleNameTextBoxes[stn];
                    String stationString = "" + stn;
                    if (sampleNameTextBox.Text.Length != 0)
                    {
                        stationString = stationString + "(" + sampleNameTextBox.Text + ")";
                    }
                    if (ovenManager.SampleDictionary.ContainsKey(stn))
                    {
                        SampleOvenManager.SampleData sampleData = ovenManager.SampleDictionary[stn];
                        if (sampleData.endDateTime == null)
                        {
                            Log.Information("Reset pressed for station " + stationString);
                            ovenManager.SampleDictionary.Remove(stn);
                            //sampleData.startDateTime = null;
                            //sampleData.endDateTime = null;                        
                        } else
                        {
                            ovenManager.SampleDictionary.Remove(stn);
/*                            sampleData.endDateTime = null;
                            sampleData.startDateTime = DateTime.Now;
                            ovenManager.SampleDictionary[stn] = sampleData; */
                        }
                    } else
                    {
                        Log.Information("Start pressed for station "  + stationString);
                        SampleOvenManager.SampleData sampleData = new SampleOvenManager.SampleData(DateTime.Now, sampleNameTextBox.Text);
                        ovenManager.SampleDictionary[stn] = sampleData;
                    }
                }
            }
            //int stationNumber = sender.Tag.toInt;
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
            Log.Information("Run Started");
            // Get a fresh copy of the application settings
            settings = DataObjects.SettingsManager.Instance.ApplicationSettings;
            // Shade unused stations
            colorDisabledStations(sampleGrid);
            // Get a new OvenManager
            try
            {

                ovenManager = new SampleOvenManager(settings);
            }
            catch (Exception e)
            {
                //sampleLogger.Info("Run Ended - Error configuring device", e, null);
                Log.Error("Run Ended - Error configuring device", e, null);
                return;
            }
            // Add event handler for dispatch timer tick and start the timer
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
            // Change start/stop button to show stop
            //startStopButton.Content = "Stop";
            // Disable the config menu while running
            //configMenuItem.IsEnabled = false;
            //running = true;
        }

        /*
         * stopRun
         *    stop the current sample run
         *    
         */
        private void stopRun()
        {
            Log.Information("Run Stopped");
            dispatcherTimer.Stop();
            //startStopButton.Content = "Start";
            //configMenuItem.IsEnabled = true;
            //running = false;
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
            // timeFromStartValue.Text = String.Format("{0:N0}:{1}", (ovenManager.ElapsedTime.Days * 24 * 60 + ovenManager.ElapsedTime.Hours * 60 + ovenManager.ElapsedTime.Minutes), ovenManager.ElapsedTime.Seconds);

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
                    Log.Error("Run Ending in Error - see log for more details - " + ovenManager.StatusMessage);
                    stopRun();
                    runCompletedText.Visibility = Visibility.Visible;
                }
            }

            // Update station results
            updateSampleGrid(sampleGrid);
            // If all sample stations have been triggered then show completed and stop the current run.
            /*if (ovenManager.RunCompleted)
            {
                sampleLogger.Info("Run Completed - all samples triggered");
                stopRun();
                runCompletedText.Visibility = Visibility.Visible;
            }
            else
            {
                runCompletedText.Visibility = Visibility.Hidden;
            } */

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
            SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);
            SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
            SolidColorBrush grayBrush = new SolidColorBrush(Colors.LightGray);
            Object startButtonObj = LogicalTreeHelper.FindLogicalNode(aGrid, "StartButton" + sampleNumber);
            if (startButtonObj is Button)
            {
                Button startButton = startButtonObj as Button;
                if (sampleData.HasValue)
                {
                    if (sampleData.Value.endDateTime == null)
                    {
                        if (sampleData.Value.startDateTime == null)
                        {
                            startButton.Background = greenBrush;
                            startButton.Content = "Start";
                        } else
                        {
                            startButton.Background = redBrush;
                            startButton.Content = "Reset";
                        }

                    } else
                    {
                        startButton.Background = grayBrush;
                        startButton.Content = "Clear";
                    }
                    // timeTextBlock.Text = String.Format("{0:N0}", (sampleData.Value.finalTime.Days * 24 * 60 + sampleData.Value.finalTime.Hours * 60 + sampleData.Value.finalTime.Minutes));
                }
                else
                {
                    startButton.Background = greenBrush;
                    startButton.Content = "Start";
                }
            }
            if (sampleData.HasValue)
            {
                updateStartTime(aGrid, sampleNumber, sampleData.Value.startDateTime);
                updateElapsedTime(aGrid, sampleNumber, sampleData.Value.startDateTime, sampleData.Value.endDateTime);
            }
            else
            {
                updateStartTime(aGrid, sampleNumber, null);
                updateElapsedTime(aGrid, sampleNumber, null, null);
            }
            /*Object timeObject = LogicalTreeHelper.FindLogicalNode(aGrid, "elapsedTimeValueText" + sampleNumber);
                if (timeObject is TextBlock)
                {
                    TextBlock timeTextBlock = timeObject as TextBlock;
                    if (sampleData.HasValue)
                    {
                       // timeTextBlock.Text = String.Format("{0:N0}", (sampleData.Value.finalTime.Days * 24 * 60 + sampleData.Value.finalTime.Hours * 60 + sampleData.Value.finalTime.Minutes));
                    }
                    else
                    {
                        timeTextBlock.Text = "";
                    }
                } */
        }

        private void updateStartTime(Grid aGrid, byte sampleNumber, DateTime? startValue)
        {
            Object startTimeObj = LogicalTreeHelper.FindLogicalNode(aGrid, "startTimeValueText" + sampleNumber);
            if (startTimeObj is TextBlock)
            {
                TextBlock startTimeTextBlock = startTimeObj as TextBlock;
                if (startValue == null)
                {
                    startTimeTextBlock.Text = "";
                } else
                {
                    DateTime startDateTime = startValue ?? DateTime.Now;
                    startTimeTextBlock.Text = "Start: " + startDateTime.ToString("MM/dd/yy H:mm:ss");
                }

            }
        }

        private void updateElapsedTime(Grid aGrid, byte sampleNumber, DateTime? startValue, DateTime? endValue)
        {
            Object elapsedTimeObj = LogicalTreeHelper.FindLogicalNode(aGrid, "elapsedTimeValueText" + sampleNumber);
            if (elapsedTimeObj is TextBlock)
            {
                TextBlock elapsedTimeTextBlock = elapsedTimeObj as TextBlock;
                if (startValue == null || endValue == null)
                {
                    elapsedTimeTextBlock.Text = "";
                }
                else
                {
                    DateTime startDateTime = startValue ?? DateTime.Now;
                    DateTime endDateTime = endValue ?? DateTime.Now;
                    TimeSpan elapsedTime = endDateTime - startDateTime;
//                    elapsedTimeTextBlock.Text = "Elapsed: " + elapsedTime.ToString(@"dd\:hh\:mm");
                    elapsedTimeTextBlock.Text = "Elapsed: " + Math.Round(elapsedTime.TotalMinutes);
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
