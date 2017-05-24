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

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            OverviewTaskWithoutWeekSupport.AddTask(dgOverview, boolType, db);
        }

        private void dgOverview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OverviewTaskWithoutWeekSupport.EditTask(dgOverview, boolType, db);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OverviewTaskWithoutWeekSupport.updatedgOverview(dgOverview, boolType, db);
        }
    }
}
