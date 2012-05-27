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
using WpfApplication1.DataObjects;

namespace WpfApplication1
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
            IDictionary<byte, MccPortInformation> sampleConfigDictionary = SampleInformationProvider.Instance.SampleConfigurationDictionary;

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
                    if (sampleConfigDictionary.ContainsKey(stationNumber))
                    {
                        MccPortInformation portInfo = sampleConfigDictionary[stationNumber];
                        if (portInfo != null)
                        {
                            portComboBox.SelectedItem = portInfo.Name;
                        }
                    }
                    portComboBox.SelectionChanged += Combobox_SelectionChanged;
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

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
    }
}
