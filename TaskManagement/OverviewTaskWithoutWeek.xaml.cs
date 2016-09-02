using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for OverviewTaskWithoutWeek.xaml
    /// </summary>
    public partial class OverviewTaskWithoutWeek : Window
    {
        private DBHandler db = new DBHandler();
        private string type = string.Empty;
        private string boolType = "False";

        public OverviewTaskWithoutWeek(string type)
        {
            InitializeComponent();

            this.type = type;

            if (type == "Completed")
            {
                this.Title = "Выполненные задачи";
                btnAddTask.Visibility = System.Windows.Visibility.Collapsed;
                boolType = "True";
            }
        }

        private void addGlobalSumm()
        {
            if (dgOverview.Items.Count > 0)
            {
                Item newRow = new Item() { Task = "Сумма", taksID = "-1" };

                int timeForDay = db.getTimeForDay(boolType);
                newRow.Time = DataGridHelper.getDayLength(timeForDay);
                dgOverview.Items.Add(newRow);
            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            ManagamentTask taskWindow = new ManagamentTask("Add", string.Empty); //Вывозов окна добавления задачи
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

        //Добавление задач в dgOverview
        private void addTasksToDG()
        {
            List<List<string>> listOfTasks = db.getTasks(boolType); ;//Поиск всех задач связаных с этой неделью

            foreach (List<string> task in listOfTasks) //Добавляем информацию об задачах
            {
                Item newRow = new Item() { taksID = task[0], Goal = task[1], Task = task[2], Priority = task[3] };

                int timeForDay = Convert.ToInt32(task[4]);
                string minutes = DataGridHelper.getDayLength(timeForDay);

                if (task.Count > 4)
                {
                    if (task[5] != "")
                    {
                        string timeFrom = DataGridHelper.getShortTime(task[5]);
                        string timeTo = DataGridHelper.getShortTime(task[6]);

                        minutes += "(" + timeFrom + "-" + timeTo + ")";
                    }
                }

                newRow.Time = minutes;

                dgOverview.Items.Add(newRow);
            }

            DataGridHelper.actualDataGridSize(dgOverview);
        }

        //Обновление информации в dgOverview
        private void updatedgOverview()
        {
            dgOverview.Items.Clear(); //Очищаем все строки
            addTasksToDG(); //Заполняем информацией

            dgOverview.UpdateLayout();
            DataGridHelper.addColorsToGoals(dgOverview, db);
            DataGridHelper.addColorsToTasks(dgOverview); //Добавляем цвет задачам
            addGlobalSumm();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updatedgOverview();
        }
    }
}
