using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DBTaskHandler;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for OverviewTask.xaml
    /// </summary>
    public partial class OverviewTask : Window
    {
        private DBHandler db = new DBHandler(); //Подключение к БД
        private string date = string.Empty;

        public OverviewTask(string date)
        {
            InitializeComponent();
            this.date = date;
            
            string endWeekDay = Convert.ToDateTime(date).AddDays(6).ToShortDateString(); //последний день недели
            this.Title = "Обзор недели - (" + this.date + " - " + endWeekDay + ")";
        }

        private void setDayValue(Item item, int day, string dayLength)
        {
            string fieldName = "day" + day.ToString();
            Type type = item.GetType();
            PropertyInfo prop = type.GetProperty(fieldName);

            prop.SetValue(item, dayLength, null);  
        }

        private string getDayValue(Item item, int day)
        {
            string fieldName = "day" + day.ToString();
            string dayValue = item.GetType().GetProperty(fieldName).GetValue(item, null) as string;

            return dayValue;
        }

        private void addGlobalSumm()
        {
            if (dgOverview.Items.Count > 0)
            {
                int weekID = db.getWeekID(this.date);
                Item newRow = new Item() { Task = "Сумма", taksID = "-1" };
                for (int i = 0; i < 7; ++i)
                {
                    int timeForDay = db.getTimeForDay(i, weekID, -1);
                    string dayLength = DataGridHelper.getDayLength(timeForDay);
                    setDayValue(newRow, i, dayLength);               
                }
                
                dgOverview.Items.Add(newRow);
            }
        }

        //Добавление задач в dgOverview
        private void addTasksToDG()
        {
            int weekID = db.getWeekID(date); //Получаем ID недели
            List<List<string>> listOfTasks = db.getTasks(weekID, "False"); //Поиск всех задач связаных с этой неделью
            
            foreach(List<string> task in listOfTasks) //Добавляем информацию об задачах
            {
                Item newRow = new Item() { taksID = task[0], Goal = task[1], Task = task[2], Priority = task[3] };

                int day = Convert.ToInt32(task[4]);
                int timeForDay = Convert.ToInt32(task[5]);
                string minutes = DataGridHelper.getDayLength(timeForDay);

                if (task.Count > 5)
                {
                    if (task[6] != "")
                    {
                        string timeFrom = DataGridHelper.getShortTime(task[6]);
                        string timeTo = DataGridHelper.getShortTime(task[7]);

                        minutes += "(" + timeFrom + "-" + timeTo + ")";

                    }
                }
                setDayValue(newRow, day, minutes);

                dgOverview.Items.Add(newRow);
            }
        }

        private void hideUnusedColumn()
        {
            bool[] columnHasRow = new bool[7]; //есть ли в столбце строки с данными

            for (int i = 0; i < dgOverview.Items.Count; ++i) //проход по всем строкмам
            {
                Item drv = dgOverview.Items[i] as Item; //Содержимое строки
                
                for (int j = 0; j < columnHasRow.Length; ++j)
                {
                    string day = getDayValue(drv, j);
                    if (day != null && day.Length > 0) columnHasRow[j] = true;
                }
            }

            for (int i = 0; i < columnHasRow.Length; ++i) //проход по всем столбцам
            {
                if (columnHasRow[i]) dgOverview.Columns[i + 4].Visibility = System.Windows.Visibility.Visible; //Показать столбец
                else dgOverview.Columns[i + 4].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        //Добавление новой задачи
        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            ManagamentTask taskWindow = new ManagamentTask("Add", this.date); //Вывозов окна добавления задачи
            if (taskWindow.ShowDialog() == true)
            {
                updatedgOverview(); //обновляем dgOverview
            }
        }

        private void dgOverview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                        updatedgOverview(); //обновляем dgOverview
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updatedgOverview();
        }

        //Обновление информации в dgOverview
        private void updatedgOverview()
        {
            dgOverview.Items.Clear(); //Очищаем все строки
            addTasksToDG(); //Заполняем информацией
            DataGridHelper.actualDataGridSize(dgOverview);

            dgOverview.UpdateLayout();
            hideUnusedColumn();
            DataGridHelper.addColorsToGoals(dgOverview, db); //Добавляем цвет целям
            DataGridHelper.addColorsToTasks(dgOverview); //Добавляем цвет задачам
            addGlobalSumm();
        }

    }
}
