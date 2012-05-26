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
                    portComboBox.Name = "portComboBox" + stationNumber;
                    portComboBox.ItemsSource = MccPortInformationAccessor.Instance.MccPortNameList;
                    portComboBox.SelectedValue = null;
                    if (sampleConfigDictionary.ContainsKey(stationNumber))
                    {
                        MccPortInformation portInfo = sampleConfigDictionary[stationNumber];
                        if (portInfo != null)
                        {
                            portComboBox.SelectedValue = portInfo.Name;
                        }
                    }
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
    }
}
