using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using TaskManagement.DatabaseHandler;
using TaskManagement.DataGridHelper;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for ManagementWeek.xaml
    /// </summary>
    /// 
    public enum Operation { Add, Edit, Remove }

    public partial class ManagementWeek : Window
    {
        private DBHandler db = new DBHandler(); //Подключение к БД
        private DateTime? dtSelectedDate; //Выбранная дата
        private Operation operation;

        public ManagementWeek(Operation operation)
        {
            InitializeComponent();

            this.operation = operation;
            ManagementWeekSupport.ChangeFormView(operation, cbWeeks, btnCreateWeek, dpWeek, this, db); //изменение формы, в соответсвие с операцией
        }

        //Изминение выбраной даты на понедельник
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ManagementWeekSupport.ChangeSelectedDate(e, dpWeek, ref dtSelectedDate);
        }

        private void btnCreateWeek_Click(object sender, RoutedEventArgs e)
        {

            if (operation == Operation.Remove) //Удаление недели
            {
                ManagementWeekSupport.removeWeek(cbWeeks, this, db);
            }
            else
            {
                ManagementWeekSupport.managmentWeek(dtSelectedDate, cbWeeks, db, this, operation);
            }
        }
    }
}
