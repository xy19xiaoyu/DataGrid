using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using datagrid.UIHelper;

namespace datagrid
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Test> GridSource = new ObservableCollection<Test>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private Test SelectItem;
        private string Colhead { get; set; }

        private int RowIndex { get; set; } = 0;
        private int ColIndex { get; set; } = 0;

        private DataGridCell Cell { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddNewRow();
            FocusLastRowFirstCell();

        }

        private void AddNewRow()
        {
            bool boAdd = true;
            if (GridSource.Count > 0)
            {
                foreach (Test item in GridSource)
                {
                    if (item.CategoryName == null || String.IsNullOrEmpty(item.CategoryName))
                    {
                        boAdd = false;
                    }
                    else
                    {
                        item.RowFunction = 1;
                    }
                }
            }

            if (boAdd)
            {
                Test newItem = new Test();
                GridSource.Add(newItem);
                this.dataGrid.ItemsSource = GridSource;
                this.dataGrid.ScrollIntoView(newItem);
            }
        }

        private void FocusLastRowFirstCell()
        {
            #region//选中最后一行第一个单元格
            this.dataGrid.ScrollIntoView(GridSource[GridSource.Count - 1]);
            this.dataGrid.CurrentItem = GridSource[GridSource.Count - 1];
            DataGridCell cell = this.dataGrid.GetCell(GridSource.Count - 1, 0);
            cell.IsSelected = true;
            cell.Focus();
            #endregion
        }

        private void SelectNext(int row = -1, int Col = -1)
        {

            if (row == -1 || Col == -1)
            {
                if (dataGrid.CurrentItem == null) return;
                row = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                Col = dataGrid.Columns.IndexOf(dataGrid.CurrentColumn);
                if (dataGrid.CurrentCell.Column.Header.ToString() == "服务")
                {
                    RowIndex = row + 1;
                    ColIndex = 0;
                }
                else
                {
                    RowIndex = row;
                    ColIndex = Col + 1;
                }
            }
            else
            {
                RowIndex = row;
                ColIndex = Col;
            }


            if (dataGrid.Items.Count - 1 <= row)
            {
                AddNewRow();
            }
            Cell = DataGridHelper.GetCell(dataGrid, RowIndex, ColIndex);
            if (Cell != null)
            {
                Cell.IsSelected = true;
                Cell.Focus();
                dataGrid.BeginEdit();

                SelectItem = dataGrid.CurrentItem as Test;

                ppp.PlacementTarget = Cell;
                ppp.IsOpen = false;
                ppp.IsOpen = true;

            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectItem == null) return;
            SelectItem.CategoryName = "test";
            if (Colhead == "服务")
            {
                RowIndex++;
                ColIndex = 0;
            }
            else
            {
                ColIndex++;
            }
            SelectNext(RowIndex, ColIndex);

        }

        private void dataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SelectItem == null) return;

                if (Colhead == "品类")
                {
                    if (string.IsNullOrEmpty(SelectItem.CategoryName)) return;
                    AddNewRow();
                }
                SelectNext();
            }
        }



        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            if (dataGrid.SelectedCells.Count == 0) return;
            Colhead = dataGrid.SelectedCells[0].Column.Header.ToString();

            if (dataGrid.CurrentItem == null) return;
            SelectItem = this.dataGrid.CurrentItem as Test;
            RowIndex = dataGrid.Items.IndexOf(dataGrid.SelectedCells[0].Item);
            ColIndex = dataGrid.Columns.IndexOf(dataGrid.SelectedCells[0].Column);
            if (ColIndex < 0) ColIndex = 0;
            dataGrid.BeginEdit();


            Cell = DataGridHelper.GetCell(dataGrid, RowIndex, ColIndex);
            //ppp.PlacementTarget = Cell;
            //ppp.IsOpen = false;
            //ppp.IsOpen = true;
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (SelectItem == null) return;
            string text = (sender as Button).Content.ToString();
            switch (Colhead)
            {

                case "品类":
                    SelectItem.CategoryName = text;
                    break;
                case "条码":
                    SelectItem.SN = text;
                    break;
                case "名称":
                    SelectItem.Name = text;
                    break;
                case "颜色":
                    SelectItem.Color = text;
                    break;
                case "服务":
                    SelectItem.Service = text;
                    break;
            }
         
            if (Colhead == "服务")
            {
                RowIndex++;
                ColIndex = 0;
            }
            else
            {
                ColIndex++;
            }
            ppp.IsOpen = false;
            SelectNext(RowIndex, ColIndex);


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Test t = btn.DataContext as Test;
            if (t == null) return;
            GridSource.Remove(t);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            Test t = btn.DataContext as Test;
            if (t == null) return;
            GridSource.Add(new Test() { Name = t.Name, CategoryName = t.CategoryName, Color = t.Color, Brand = t.Brand, Service = t.Service });
        }
    }

    public class Test : INotifyPropertyChanged
    {
        public string _CategoryName;
        public string CategoryName
        {
            get { return _CategoryName; }
            set
            {
                _CategoryName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryName"));
            }
        }

        public string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public string _Color;
        public string Color
        {
            get { return _Color; }
            set
            {
                _Color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));
            }
        }

        public string _Brand;
        public string Brand
        {
            get { return _Brand; }
            set
            {
                _Brand = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Brand"));
            }
        }

        public string _Service;
        public string Service
        {
            get { return _Service; }
            set
            {
                _Service = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Service"));
            }
        }

        public string _SN;
        public string SN
        {
            get { return _SN; }
            set
            {
                _SN = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SN"));
            }
        }

        private int _RowFunction;

        public int RowFunction
        {
            get { return _RowFunction; }
            set
            {
                _RowFunction = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RowFunction"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
