using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdherentSampleOven.DataObjects;

namespace AdherentSampleOven
{
    /// <summary>
    /// Interaction logic for Config2.xaml
    /// </summary>
    public partial class Config2 : Window
    {
        private const string portConfigComboPrefix = "portComboBox";
        
        public Config2()
        {
            InitializeComponent();
            addSampleConfigsToGrid();
        }


        private void addSampleConfigsToGrid()
        {
            /* Seems easier to create the sample config grid programatically than usin xaml */
            Settings settings = SettingsManager.Instance.ApplicationSettings;
            IList<int> intList = new List<int>();
            for (int i = 1; i <100; i++)
            {
                intList.Add(i);
            }
            IList<byte> byteList = new List<byte>();
            for (byte i = 0; i <=7; i++)
            {
                byteList.Add(i);
            }

            tcBoardNumberCombo.ItemsSource = intList;
            tcBoardNumberCombo.SelectedItem = settings.TempBoardNumber;

            tcPortNumberCombo.ItemsSource = byteList;
            tcPortNumberCombo.SelectedItem = settings.TempPortNumber;

            dioBordNumberCombo.ItemsSource = intList;
            dioBordNumberCombo.SelectedItem = settings.DIOBoardNumber;



            byte stationNumber = 30;
            for (int y = 0; y < 6; y++)
            {
                for (int x = 4; x >= 0; x--)
                {

                    Grid currentSampleGrid = new Grid();

                    RowDefinition rowdef1 = new RowDefinition();
                    rowdef1.Height = new GridLength(1, GridUnitType.Star);
                    RowDefinition rowdef2 = new RowDefinition();
                    rowdef2.Height = new GridLength(1, GridUnitType.Star);

                    ColumnDefinition coldef1 = new ColumnDefinition();
                    coldef1.Width = new GridLength(4, GridUnitType.Star);

                    currentSampleGrid.RowDefinitions.Add(rowdef1);
                    currentSampleGrid.RowDefinitions.Add(rowdef2);
                    currentSampleGrid.ColumnDefinitions.Add(coldef1);


                    TextBlock stationLabelText = new TextBlock();
                    stationLabelText.Text = "Station: " + stationNumber;
                    Viewbox stationLabelViewbox = new Viewbox();
                    stationLabelViewbox.Child = stationLabelText;
                    currentSampleGrid.Children.Add(stationLabelViewbox);
                    Grid.SetColumn(stationLabelViewbox, 0);
                    Grid.SetRow(stationLabelViewbox, 0);

                    ComboBox portComboBox = new ComboBox();
                    portComboBox.Name = portConfigComboPrefix + stationNumber;
                    portComboBox.ItemsSource = MccPortInformationAccessor.Instance.MccPortNameList;
                    portComboBox.SelectedItem = null;
                    if (settings.SampleConfigurationDictionary.ContainsKey(stationNumber))
                    {
                        MccPortInformation portInfo = settings.SampleConfigurationDictionary[stationNumber];
                        if (portInfo != null)
                        {
                            portComboBox.SelectedItem = portInfo.Name;
                        }
                    }
                    portComboBox.SelectionChanged += PortCombobox_SelectionChanged;
                    currentSampleGrid.Children.Add(portComboBox);
                    Grid.SetColumn(portComboBox, 0);
                    Grid.SetRow(portComboBox, 1);

                    sampleConfigGrid.Children.Add(currentSampleGrid);
                    Grid.SetColumn(currentSampleGrid, x);
                    Grid.SetRow(currentSampleGrid, y);
                    stationNumber--;
                }
            }
        }

        private void PortCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        /*
         * When a port combobox selection is changed we loop through all the other
         * port comboboxes and if it is a duplicate we clear the other entry.  No two
         * samples should be set to use the same port.
         */
        {
            try
            {
                if (sender is ComboBox)
                {
                    string newPortSelected = (e.AddedItems[0] as String);
                    if (!String.IsNullOrWhiteSpace(newPortSelected))
                    {
                    ComboBox changedComboBox = sender as ComboBox;
                    byte sampleChanged = byte.Parse(changedComboBox.Name.Substring(portConfigComboPrefix.Length));
                    for (byte i = 1; i <= 30; i++)
                    {
                        if (i != sampleChanged)
                        {
                            Object tempObject = LogicalTreeHelper.FindLogicalNode(sampleConfigGrid, portConfigComboPrefix + i);
                            if (tempObject is ComboBox)
                            {
                                ComboBox tempConfigCombo = tempObject as ComboBox;
                                if (newPortSelected == tempConfigCombo.SelectedItem as string)
                                {
                                    tempConfigCombo.SelectedItem = null;
                                }
                            }
                        }
                    }
                    }
                }
            }
            catch (Exception ex)
            {
             //   MessageBox.Show(ex);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings updatedSettings = new Settings();
            updatedSettings.TempBoardNumber = (int)tcBoardNumberCombo.SelectedItem;
            updatedSettings.TempPortNumber = (byte)tcPortNumberCombo.SelectedItem;
            updatedSettings.DIOBoardNumber = (int)dioBordNumberCombo.SelectedItem; 
            IDictionary<byte,MccPortInformation> sampleSettingsDictionary = new Dictionary<byte, MccPortInformation>();
            for (byte i = 1; i <= 30; i++)
            {
                Object tempObject = LogicalTreeHelper.FindLogicalNode(sampleConfigGrid, portConfigComboPrefix + i);
                if (tempObject is ComboBox)
                {
                    ComboBox tempComboBox = tempObject as ComboBox;
                    sampleSettingsDictionary[i] = MccPortInformationAccessor.Instance.portForName( tempComboBox.SelectedItem as String);
                }
            }
            updatedSettings.SampleConfigurationDictionary = sampleSettingsDictionary;
         //   SettingsManager.Instance.updateSampleSettingProperties(sampleSettingsDictionary);
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void removeEventSubscriptions()
        {
            /* Seems like I shouldnt have to do this but I have read stuff that says for
             * WPF I should always unsubscribe from all events.
             */
            for (byte i = 1; i <= 30; i++)
            {
                Object tempObject = LogicalTreeHelper.FindLogicalNode(sampleConfigGrid, portConfigComboPrefix + i);
                if (tempObject is ComboBox)
                {
                    ComboBox tempComboBox = tempObject as ComboBox;
                    tempComboBox.SelectionChanged += PortCombobox_SelectionChanged;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            removeEventSubscriptions();
        }
    }
}
