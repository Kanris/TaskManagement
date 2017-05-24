using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskManagement.DatabaseHandler;
using TaskManagement.DataGridHelper;

namespace TaskManagement
{
    static class OverviewTaskSupport
    {
        public static void AddGlobalSumm(DataGrid dgOverview, DBHandler db, string date)
        {
            if (dgOverview.Items.Count > 0)
            {
                int weekID = db.getWeekID(date);
                Item newRow = new Item() { Task = "Сумма", taksID = "-1" };
                for (int i = 0; i < 7; ++i)
                {
                    int timeForDay = db.getTimeForDay(i, weekID, -1);
                    string dayLength = DGHelper.getDayLength(timeForDay);
                    SetDayValue(newRow, i, dayLength);
                }

                dgOverview.Items.Add(newRow);
            }
        }

        //Добавление задач в dgOverview
        public static void addTasksToDG(DBHandler db, string date, DataGrid dgOverview)
        {
            int weekID = db.getWeekID(date); //Получаем ID недели
            List<List<string>> listOfTasks = db.getTasks(weekID, "False"); //Поиск всех задач связаных с этой неделью

            foreach (List<string> task in listOfTasks) //Добавляем информацию об задачах
            {
                Item newRow = new Item() { taksID = task[0], Goal = task[1], Task = task[2], Priority = task[3] };

                int day = Convert.ToInt32(task[4]);
                int timeForDay = Convert.ToInt32(task[5]);
                string minutes = DGHelper.getDayLength(timeForDay);

                if (task.Count > 5)
                {
                    if (task[6] != "")
                    {
                        string timeFrom = DGHelper.getShortTime(task[6]);
                        string timeTo = DGHelper.getShortTime(task[7]);

                        minutes += "(" + timeFrom + "-" + timeTo + ")";

                    }
                }
                SetDayValue(newRow, day, minutes);

                dgOverview.Items.Add(newRow);
            }
        }

        public static void HideUnusedColumn(DataGrid dgOverview)
        {
            bool[] columnHasRow = new bool[7]; //есть ли в столбце строки с данными

            for (int i = 0; i < dgOverview.Items.Count; ++i) //проход по всем строкмам
            {
                Item drv = dgOverview.Items[i] as Item; //Содержимое строки

                for (int j = 0; j < columnHasRow.Length; ++j)
                {
                    string day = GetDayValue(drv, j);
                    if (day != null && day.Length > 0) columnHasRow[j] = true;
                }
            }

            for (int i = 0; i < columnHasRow.Length; ++i) //проход по всем столбцам
            {
                if (columnHasRow[i]) dgOverview.Columns[i + 4].Visibility = System.Windows.Visibility.Visible; //Показать столбец
                else dgOverview.Columns[i + 4].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static void OpenTaskDialog(DataGrid dgOverview, DBHandler db, string date)
        {
            if (dgOverview.SelectedItem != null)
            {
                Item drv = dgOverview.SelectedItem as Item;

                int taskID = Convert.ToInt32(drv.taksID); //id задачи

                if (taskID > -1)
                {
                    ManagamentTask managmentWindow = new ManagamentTask("Edit", taskID); //Вывозов окна изменения задачи

                    if (managmentWindow.ShowDialog() == true)
                    {
                        updatedgOverview(dgOverview, db, date); //обновляем dgOverview
                    }
                }
            }
        }

        //Обновление информации в dgOverview
        public static void updatedgOverview(DataGrid dgOverview, DBHandler db, string date)
        {
            dgOverview.Items.Clear(); //Очищаем все строки
            OverviewTaskSupport.addTasksToDG(db, date, dgOverview);
            DGHelper.actualDataGridSize(dgOverview);

            dgOverview.UpdateLayout();
            OverviewTaskSupport.HideUnusedColumn(dgOverview);

            DGHelper.addColorsToGoals(dgOverview, db); //Добавляем цвет целям
            DGHelper.addColorsToTasks(dgOverview); //Добавляем цвет задачам
            OverviewTaskSupport.AddGlobalSumm(dgOverview, db, date);
        }

        private static string GetDayValue(Item item, int day)
        {
            string fieldName = "day" + day.ToString();
            string dayValue = item.GetType().GetProperty(fieldName).GetValue(item, null) as string;

            return dayValue;
        }

        private static void SetDayValue(Item item, int day, string dayLength)
        {
            string fieldName = "day" + day.ToString();
            Type type = item.GetType();
            PropertyInfo prop = type.GetProperty(fieldName);

            prop.SetValue(item, dayLength, null);
        }

    }
}
