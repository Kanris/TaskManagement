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

        //Добавление новой задачи
        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            ManagamentTask taskWindow = new ManagamentTask("Add", this.date); //Вывозов окна добавления задачи
            if (taskWindow.ShowDialog() == true)
            {
                OverviewTaskSupport.updatedgOverview(dgOverview, db, date); //обновляем dgOverview
            }
        }

        private void dgOverview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OverviewTaskSupport.OpenTaskDialog(dgOverview, db, date);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OverviewTaskSupport.updatedgOverview(dgOverview, db, date);
        }

    }
}
