using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
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
        private Boolean running = false;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private DataObjects.Settings settings = null;
        private HardwareInterface.SampleOvenManager ovenManager;

        public MainWindow()
        {
            InitializeComponent();
            logger.Trace("After InitializeComponent");
            addStationControlToGrid();
            logger.Trace("Added station control to grid");
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            logger.Trace("Timer created");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
         //   MccDaq.MccBoard DaqBoard = new MccDaq.MccBoard(0);
         //   MccDaq.DigitalLogicState logicState;

         //   DaqBoard.DConfigBit(MccDaq.DigitalPortType.FirstPortA, 0, MccDaq.DigitalPortDirection.DigitalIn);
         //   MccDaq.ErrorInfo errorInfo = DaqBoard.DBitIn(MccDaq.DigitalPortType.FirstPortA, 0, out logicState);

         //   byte tempBoardNumber = Properties.Settings.Default.tempBoardNumber;
         //   this.temperatureTextBlock.Text = logicState.ToString();

           // Properties.Settings.Default[“tempBoardNumber”] = 1;

            //float temp=0.0f;
            //DaqBoard.TIn(0, MccDaq.TempScale.Fahrenheit, out temp, MccDaq.ThermocoupleOptions.Filter);

            //this.temperatureTextBlock.Text=temp.ToString("0.00");
         //   CommentDock.Background = Brushes.BurlyWood;

            logger.Trace("Wndow Loaded");

        }

        private void configMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Config2 configWindow = new Config2();
            configWindow.Owner = this;
            configWindow.ShowDialog();
        }

        private void doSomethingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Object timeObject = LogicalTreeHelper.FindLogicalNode(sampleGrid, "elapsedTimeValueText" + 3);
            if (timeObject is TextBlock)
            {
                TextBlock timeTextBlock = timeObject as TextBlock;
                timeTextBlock.Text = "23:32:11";
            }
            TextBlock tempTextBlock = (TextBlock)sampleGrid.FindName("temperatureValueText" + 3);
            Object tempObject = LogicalTreeHelper.FindLogicalNode(sampleGrid, "temperatureValueText" + 3);
            if (tempObject is TextBlock)
            {
                TextBlock elapsedTemperatureTextBlock = tempObject as TextBlock;
                elapsedTemperatureTextBlock.Text = "32.5F";
            } 
        }

        private void printMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
            {
                System.Printing.PrintCapabilities capabilities =
    printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / sampleGrid.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                               sampleGrid.ActualHeight);

                //Transform the Visual to scale
                sampleGrid.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                sampleGrid.Measure(sz);
                sampleGrid.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.

                printDlg.PrintVisual(sampleGrid, "Test Print Sample Grid");
            }
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void addStationControlToGrid()
        {

            /*
                <Grid Grid.Row ="0" Grid.Column="0" ShowGridLines="False">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="1*" />
                        <RowDefinition Height="2*" />
                        <RowDefinition Height="1*" />
                        <RowDefinition Height="1*" />
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="4*" />
                        <ColumnDefinition Width="3*" />
                    </Grid.ColumnDefinitions>
                    <Viewbox Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2">
                        <TextBlock Text="Station: 26" />
                    </Viewbox>
                    <Viewbox Grid.Row="2" Grid.Column="0" Margin="8,0,0,0">
                        <TextBlock Text="Elapsed Time:" />
                    </Viewbox>
                    <Viewbox Grid.Row="3" Grid.Column="0" Margin="8,0,0,0">
                        <TextBlock Text="Temperature:" />
                    </Viewbox>
                    <Viewbox Grid.Row="2" Grid.Column="1" HorizontalAlignment="Left" Margin="8,0,4,0">
                        <TextBlock Text="01:00:13" HorizontalAlignment="Left" />
                    </Viewbox>
                    <Viewbox Grid.Row="3" Grid.Column="1" HorizontalAlignment="Left" Margin="8,0,4,0">
                        <TextBlock Text="102.4" HorizontalAlignment="Left" VerticalAlignment="Center" />
                    </Viewbox>


                </Grid>


             *
             */
            int stationNumber = 30;
            for (int y = 0; y < 6; y++)
            {
                for (int x = 4; x >= 0; x--)
                {

                    Grid currentSampleGrid = new Grid();

                    RowDefinition rowdef1 = new RowDefinition();
                    rowdef1.Height = new GridLength(2, GridUnitType.Star);
                    RowDefinition rowdef2 = new RowDefinition();
                    rowdef2.Height = new GridLength(1, GridUnitType.Star);
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

                    sampleGrid.Children.Add(currentSampleGrid);
                    Grid.SetColumn(currentSampleGrid, x);
                    Grid.SetRow(currentSampleGrid, y);
                    stationNumber--;
                }
            }
            


        }

        private void startStopButton_Click(object sender, RoutedEventArgs e)
        {
/*            Object timeObject = LogicalTreeHelper.FindLogicalNode(sampleGrid, "elapsedTimeValueText" + 3);
            if (timeObject is TextBlock)
            {
                TextBlock timeTextBlock = timeObject as TextBlock;
                timeTextBlock.Text = "23:32:11";
            }
            TextBlock tempTextBlock = (TextBlock)sampleGrid.FindName("temperatureValueText" + 3);
            Object tempObject = LogicalTreeHelper.FindLogicalNode(sampleGrid, "temperatureValueText" + 3);
            if (tempObject is TextBlock)
            {
                TextBlock elapsedTemperatureTextBlock = tempObject as TextBlock;
                elapsedTemperatureTextBlock.Text = "32.5F";
            } */
            if (running)
            {
                logger.Trace("Stopping");
                dispatcherTimer.Stop();
              //  dispatcherTimer = null;
                startStopButton.Content = "Start";
                running = false;
            }
            else
            {
                logger.Trace("1");
                settings = DataObjects.SettingsManager.Instance.ApplicationSettings;
                logger.Trace("2");
                ovenManager = new SampleOvenManager(settings);
                logger.Trace("3");
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                logger.Trace("4");
                dispatcherTimer.Start();
                logger.Trace("5");
                startStopButton.Content = "Stop";
                logger.Trace("6");
                running = true;
            }
            

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
            for (byte i = 0; i <= 30; i++)
            {
                if (ovenManager.SampleDictionary.ContainsKey(i))
                {
                    updateSampleBlock(i, ovenManager.SampleDictionary[i]);
                }
                else
                {
                    updateSampleBlock(i, null);
                }
            }
            if (ovenManager.RunCompleted)
            {
                runCompletedText.Visibility = Visibility.Visible;
            }
            else
            {
                runCompletedText.Visibility = Visibility.Hidden;
            }
            //foreach (var sample in ovenManager.SampleDictionary)
            //{
            //    updateSampleBlock(sample.Key, sample.Value);
            //}

        }

        private void updateSampleBlock(byte sampleNumber, SampleOvenManager.SampleData? sampleData)
        {
            Object timeObject = LogicalTreeHelper.FindLogicalNode(sampleGrid, "elapsedTimeValueText" + sampleNumber);
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
            Object tempObject = LogicalTreeHelper.FindLogicalNode(sampleGrid, "temperatureValueText" + sampleNumber);
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
           // MessageBox.Show("Closing");
            base.OnClosing(e);
        } 

    }
}
