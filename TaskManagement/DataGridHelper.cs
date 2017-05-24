using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DBTaskHandler;

namespace TaskManagement
{
     static class DataGridHelper
    {
        public static void addColorToCell(DataGridCell dgc, string color)
        {
            BrushConverter bc = new BrushConverter();
            dgc.Background = (Brush)bc.ConvertFrom(color); //Конвертирование строки в цвет
        }

        //Выбор цвета взависимости от приоритета
        public static string getPriorityColor(int row, DataGrid dgOverview)
        {
            DataGridCell dgc = GetCell(dgOverview, row, 3); //Колонка с приоритетом задачи
            string priority = (dgc.Content as TextBlock).Text;

            string color = String.Empty;

            switch (priority)
            {
                case "0": //критическая
                    color = "#FFAA6767";
                    break;
                case "1": //Важная
                    color = "#FFCD8125";
                    break;
                case "2": //Стандартная
                    color = "#FF3F9C76";
                    break;
                case "3": //Не важная
                    color = String.Empty;
                    break;
            }

            return color;
        }

        public static void actualDataGridSize(DataGrid dgOverview)
        {
            if (dgOverview.Items.Count > 0)
            {
                for (int i = 1; i < dgOverview.Columns.Count; ++i)
                {
                    dgOverview.Columns[i].Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                }
            }

        }

        //Добавление фона цели
        public static void addColorsToGoals(DataGrid dataGrid, DBHandler db)
        {
            for (int i = 0; i < dataGrid.Items.Count; ++i)
            {
                DataGridCell dgc = GetCell(dataGrid, i, 0);
                string goalName = (dgc.Content as TextBlock).Text;
                string color = db.getGoalColor(goalName);

                addColorToCell(dgc, color);
            }
        }

        //Добавление цветов задачам
        public static void addColorsToTasks(DataGrid dataGrid)
        {
            dataGrid.Columns[3].Visibility = System.Windows.Visibility.Visible; //Отображаем колонку с приоритетами задач

            for (int i = 0; i < dataGrid.Items.Count; ++i)
            {
                string color = getPriorityColor(i, dataGrid); //цвет приоритета
                if (color != String.Empty) //если цвет не пустой
                {
                    DataGridCell dgc = GetCell(dataGrid, i, 1);
                    addColorToCell(dgc, color);
                }
            }
            dataGrid.Columns[3].Visibility = System.Windows.Visibility.Collapsed; //Прячем колонку с приоритетами
        }

        //Метод создание диалогов с вариантами ответа
        public static MessageBoxResult createAskDialog(string sMessageBoxText, string sCaption)
        {
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo; //Кнопки ответов Да/Нет
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning; //Тип диалогового окна Предупреждение

            return System.Windows.MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox); //Возврат созданого окна
        }

        //Получение короткого времени
        public static string getShortTime(string time)
        {
            DateTime dateTime = Convert.ToDateTime(time);
            string shortTime = dateTime.ToShortTimeString();

            return shortTime;
        }

        public static string getDayLength(int minutes)
        {
            var timeSpan = TimeSpan.FromMinutes(minutes);
            string dayLength = String.Empty;

            if (timeSpan.Hours > 0)
            {
                dayLength = timeSpan.Hours.ToString() + "ч " + timeSpan.Minutes.ToString() + "мин\n";
            }
            else
            {
                dayLength = timeSpan.Minutes.ToString() + "мин\n";
            }

            return dayLength;
        }

        static public DataGridCell GetCell(DataGrid dg, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dg, row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dg.ScrollIntoView(rowContainer, dg.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        static public DataGridRow GetRow(DataGrid dg, int index)
        {
            DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dg.ScrollIntoView(dg.Items[index]);
                row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T FindVisualChild<T>(DependencyObject item)
            where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(item);
            var result = item as T;

            for (int i = 0; i < childCount && result == null; i++)
            {
                result = FindVisualChild<T>(VisualTreeHelper.GetChild(item, i));
            }
            return result;
        }

        //Создание нового столбца
        public static DataGridTemplateColumn createNewColumn(string header, DataTemplate dataTemplate)
        {
            DataGridTemplateColumn dgTemplateColumn = new DataGridTemplateColumn();
            dgTemplateColumn.Header = header;
            dgTemplateColumn.CellTemplate = dataTemplate;
            dgTemplateColumn.MinWidth = 100;
            dgTemplateColumn.CanUserSort = false;
           
            return dgTemplateColumn;
        }

        static T GetVisualChild<T>(Visual parent) where T : Visual
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
    }
}
