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
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using WpfApplication1.DataObjects;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();
            List<string> a = new List<string>() {
            "test1", "test2","test3" };
           // portComboBox.ItemsSource = a;
            //comboBox1.ItemsSource = new DataObjects.MccPortInformationProvider().MccPortInformationList;

     //       dataGrid1.DataContext = DataObjects.SampleInformationProvider.Instance.SampleInformationList;
           // portInfoCombo.ItemsSource = DataObjects.MccPortInformationAccessor.MccPortInformationList;
           // portInfoCombo.SelectedItemBinding = "PortInfo.Name";
            //testCombo.ItemsSource = DataObjects.MccPortInformationAccessor.Instance.MccPortNameList;
            //ItemsSource="{Binding MccPortNameList, Source={x:Static local:MccPortInformationAccessor.Instance}}"
           // portInfoCombo.DisplayMemberPath = "Name";
            subscribeToSampleInformationPropertyChanges();
            //DataObjects.SampleInformationProvider.Instance.SampleInformationList.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SampleInformationCollectionChanged);
            //MessageBox.Show("Prop = " + Properties.Settings.Default["Sample1"]);
         //   DataObjects.SampleInformationProvider.Instance.SampleInformationList.RemoveAt(3);
         //   portComboBox2.CellTemplate.Template.ItemsSource = a;
            //portComboBox.IsReadOnly = false;
            //new DataObjects.MccPortInformationProvider().MccPortInformationList;

            // <DataGridComboBoxColumn x:Name="portInfoCombo" DataFieldBinding="{Binding PortName} TextBinding="{Binding Path=PortName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" ItemsSource="{Binding MccPortNameList, Source={x:Static local:MccPortInformationAccessor.Instance}}" Header="Port Name" CanUserResize="False" Width="100">
        }



        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            unsubscribeFromSampleInformationPropertyChanges();
            base.OnClosing(e);
        }

        private void subscribeToSampleInformationPropertyChanges()
        {
            foreach (SampleInformation si in SampleInformationProvider.Instance.SampleInformationList)
            {
                si.PropertyChanged += SampleInformationChanged;
            }
        }

        private void unsubscribeFromSampleInformationPropertyChanges()
        {
            foreach (SampleInformation si in SampleInformationProvider.Instance.SampleInformationList)
            {
                si.PropertyChanged -= SampleInformationChanged;
            }
        }



        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        void SampleInformationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PortInformation")
            {
                foreach (SampleInformation si in SampleInformationProvider.Instance.SampleInformationList)
                {
                    if ((si.SampleNumber != ((SampleInformation)sender).SampleNumber) &&
                        (si.PortName == ((SampleInformation)sender).PortName))
                    {
                        si.PortInformation = null;
                    }
                }
                dataGrid1.CommitEdit();
                dataGrid1.CancelEdit();
                //dataGrid1.Items.Refresh();
              //  dataGrid1.Dispatcher.BeginInvoke(new Action(() => dataGrid1.Items.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
                CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource).Refresh();
            }
        }
    }
}
