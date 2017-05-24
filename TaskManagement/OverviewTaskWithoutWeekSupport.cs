using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskManagement.DatabaseHandler;
using TaskManagement.DataGridHelper;

namespace TaskManagement
{
    static class OverviewTaskWithoutWeekSupport
    {
        //Обновление информации в dgOverview
        public static void updatedgOverview(DataGrid dgOverview, string boolType, DBHandler db)
        {
            dgOverview.Items.Clear(); //Очищаем все строки
            addTasksToDG(dgOverview, boolType, db); //Заполняем информацией

            dgOverview.UpdateLayout();
            DGHelper.addColorsToGoals(dgOverview, db);
            DGHelper.addColorsToTasks(dgOverview); //Добавляем цвет задачам
            addGlobalSumm(dgOverview, boolType, db);
        }

        public static void AddTask(DataGrid dgOverview, string boolType, DBHandler db)
        {
            ManagamentTask taskWindow = new ManagamentTask("Add", string.Empty); //Вывозов окна добавления задачи
            if (taskWindow.ShowDialog() == true)
            {
                updatedgOverview(dgOverview, boolType, db);
            }
        }

        public static void EditTask(DataGrid dgOverview, string boolType, DBHandler db)
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
                        OverviewTaskWithoutWeekSupport.updatedgOverview(dgOverview, boolType, db);
                    }
                }
            }
        }

        private static void addGlobalSumm(DataGrid dgOverview, string boolType, DBHandler db)
        {
            if (dgOverview.Items.Count > 0)
            {
                Item newRow = new Item() { Task = "Сумма", taksID = "-1" };

                int timeForDay = db.getTimeForDay(boolType);
                newRow.Time = DGHelper.getDayLength(timeForDay);
                dgOverview.Items.Add(newRow);
            }
        }

        //Добавление задач в dgOverview
        private static void addTasksToDG(DataGrid dgOverview, string boolType, DBHandler db)
        {
            List<List<string>> listOfTasks = db.getTasks(boolType); ;//Поиск всех задач связаных с этой неделью

            foreach (List<string> task in listOfTasks) //Добавляем информацию об задачах
            {
                Item newRow = new Item() { taksID = task[0], Goal = task[1], Task = task[2], Priority = task[3] };

                int timeForDay = Convert.ToInt32(task[4]);
                string minutes = DGHelper.getDayLength(timeForDay);

                if (task.Count > 4)
                {
                    if (task[5] != "")
                    {
                        string timeFrom = DGHelper.getShortTime(task[5]);
                        string timeTo = DGHelper.getShortTime(task[6]);

                        minutes += "(" + timeFrom + "-" + timeTo + ")";
                    }
                }

                newRow.Time = minutes;

                dgOverview.Items.Add(newRow);
            }

            DGHelper.actualDataGridSize(dgOverview);
        }

    }
}
