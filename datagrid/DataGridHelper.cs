using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace datagrid.UIHelper
{
    public static class DataGridHelper
    {
        /// <summary>         
        /// Gets the visual child of an element         
        /// </summary>         
        /// <typeparam name="T">Expected type</typeparam>         
        /// <param name="parent">The parent of the expected element</param>         
        /// <returns>A visual child</returns>         
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// Gets the specified cell of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <param name="row">The row of the cell</param>
        /// <param name="column">The column index of the cell</param>
        /// <returns>A cell of the DataGrid</returns>
        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }
        /// <summary>
        /// Gets the specified cell of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <param name="row">The row index of the cell</param>
        /// <param name="column">The column index of the cell</param>
        /// <returns>A cell of the DataGrid</returns>
        public static DataGridCell GetCell(this DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = grid.GetRow(row);
            return grid.GetCell(rowContainer, column);
        }
        /// <summary>
        /// Gets the specified row of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <param name="index">The index of the row</param>
        /// <returns>A row of the DataGrid</returns>
        public static DataGridRow GetRow(this DataGrid grid, int index)
        {

            if (grid.Items.Count > index)
            {
                DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
                if (row == null)
                {
                    // May be virtualized, bring into view and try again.
                    grid.UpdateLayout();
                    grid.ScrollIntoView(grid.Items[index]);
                    row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
                }
                return row;
            }
            return null;

        }
        /// <summary>
        /// Gets the selected row of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <returns></returns>
        public static DataGridRow GetSelectedRow(this DataGrid grid)
        {
            return (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
        }

        public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
        {
            if (!dataGridCellInfo.IsValid)
            {
                return null;
            }

            var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            if (cellContent != null)
            {
                return (DataGridCell)cellContent.Parent;
            }
            else
            {
                return null;
            }
        }

        public static int GetRowIndex(DataGridCell dataGridCell)
        {
            // Use reflection to get DataGridCell.RowDataItem property value.
            PropertyInfo rowDataItemProperty = dataGridCell.GetType().GetProperty("RowDataItem", BindingFlags.Instance | BindingFlags.NonPublic);

            DataGrid dataGrid = GetDataGridFromChild(dataGridCell);

            return dataGrid.Items.IndexOf(rowDataItemProperty.GetValue(dataGridCell, null));
        }

        public static DataGrid GetDataGridFromChild(DependencyObject dataGridPart)
        {
            if (VisualTreeHelper.GetParent(dataGridPart) == null)
            {
                throw new NullReferenceException("Control is null.");
            }
            if (VisualTreeHelper.GetParent(dataGridPart) is DataGrid)
            {
                return (DataGrid)VisualTreeHelper.GetParent(dataGridPart);
            }
            else
            {
                return GetDataGridFromChild(VisualTreeHelper.GetParent(dataGridPart));
            }
        }

        public static DataGridTemplateColumn GenTemplateColumn(string strHeader, string strElement, double nWidth, DataGridLengthUnitType lenType)
        {
            DataGridTemplateColumn item = new DataGridTemplateColumn();
            item.Header = strHeader;
            item.Width = new DataGridLength(nWidth, lenType);
            item.CanUserReorder = false;
            item.CanUserResize = false;
            item.CanUserSort = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("<DataTemplate");
            sb.Append(" xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'");
            sb.Append(" xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>");
            sb.Append(strElement);
            sb.Append("</DataTemplate>");

            StringReader reader = new StringReader(sb.ToString());
            XmlTextReader xmlReader = new XmlTextReader(reader);
            item.CellTemplate = (DataTemplate)XamlReader.Load(xmlReader);
            return item;
        }

        public static DataGridTemplateColumn GenTemplateColumnAllText(string strHeader, string strHeaderElement, string strCellElement, double nWidth, DataGridLengthUnitType lenType)
        {
            DataGridTemplateColumn item = new DataGridTemplateColumn();
            item.Header = strHeader;
            item.Width = new DataGridLength(nWidth, lenType);
            item.CanUserReorder = false;
            item.CanUserResize = false;
            item.CanUserSort = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("<DataTemplate");
            sb.Append(" xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'");
            sb.Append(" xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>");
            sb.Append(strCellElement);
            sb.Append("</DataTemplate>");

            StringReader reader = new StringReader(sb.ToString());
            XmlTextReader xmlReader = new XmlTextReader(reader);
            item.CellTemplate = (DataTemplate)XamlReader.Load(xmlReader);

            sb.Clear();
            sb.Append("<DataTemplate");
            sb.Append(" xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'");
            sb.Append(" xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>");
            sb.Append(strHeaderElement);
            sb.Append("</DataTemplate>");

            reader = new StringReader(sb.ToString());
            xmlReader = new XmlTextReader(reader);
            item.HeaderTemplate = (DataTemplate)XamlReader.Load(xmlReader);
            return item;
        }
    }
}
