using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using TaskManagement.DatabaseHandler;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for ManagementWeek.xaml
    /// </summary>
    public partial class ManagementWeek : Window
    {
        private DBHandler db = new DBHandler(); //Подключение к БД
        private DateTime? dtSelectedDate; //Выбранная дата
        private string operation;

        public ManagementWeek(string operation)
        {
            InitializeComponent();

            this.operation = operation;
            detectOperation(operation); //изменение формы, в соответсвие с операцией
        }

        //Определение операции и изменение формы, в соответсвие с ней
        public void detectOperation(string operation)
        {
            switch (operation)
            {
                case "Add": //На добавление
                    this.Title = "Добавить неделю"; //Изменить заголовок формы
                    cbWeeks.Visibility = System.Windows.Visibility.Collapsed; //Убрать ListBox с формы
                    break;

                case "Edit": //На удаление
                    this.Title = "Изменить неделю"; //Изменить заголовок формы

                    initCBWeeks();

                    btnCreateWeek.Content = "Изменить";

                    break;

                case "Remove": //На удаление
                    this.Title = "Удалить неделю"; //Изменить заголовок формы

                    initCBWeeks();
                    cbWeeks.Margin = new Thickness(35,44,0,0); //Смещение ListBox на уровень с кнопкой

                    btnCreateWeek.Content = "Удалить";

                    dpWeek.Visibility = System.Windows.Visibility.Collapsed; //Убрать DatePicker с формы

                    break;
            }
        }

        private void initCBWeeks()
        {
            List<String> listOfWeek = db.getWeeksFromTable(); //Получение списка недель
            ObservableCollection<string> list = new ObservableCollection<string>(listOfWeek);;

            cbWeeks.ItemsSource = list;
            cbWeeks.SelectedIndex = 0;
        }

        //Изминение выбраной даты на понедельник
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) //если выбрана дата
            {
                DateTime dayOfWeek = (DateTime)e.AddedItems[0]; //считывание выбранной даты

                if (dayOfWeek.DayOfWeek != DayOfWeek.Monday) //если выбранная дата это не понедельник
                {
                    int addDaysCount = -1 * (int)dayOfWeek.DayOfWeek + 1; //количество дней до понедельника
                    dayOfWeek = dayOfWeek.AddDays(addDaysCount); //изминение даты

                    dpWeek.SelectedDate = dayOfWeek; //отображение новой даты в DatePicker
                }

                dtSelectedDate = dayOfWeek;
            }
        }

        private void managmentWeek()
        {
            if (dtSelectedDate.HasValue)
            {
                string date = dtSelectedDate.Value.ToShortDateString(); //Получение даты в текстовом формате

                if (!db.isWeekExist(date)) //Проверка на то, что недели с выбраной датой не существует
                {
                    if (this.operation == "Add") db.addWeek(date); //Добавление недели в БД
                    else db.updateWeek(cbWeeks.Text, date);

                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Выбраная неделя уже добавлена в таблицу!");
                }
            } else
            {
                MessageBox.Show("Выберите неделю!");
            }

        }

        private void removeWeek()
        {
            string date = cbWeeks.Text; //Получение выбраной недели из ListBox

            string sMessageBoxText = "Вы действительно хотите удалить выбраную неделю? (" + date + ")";
            string sCaption = "Удаление недели!";

            MessageBoxResult result = DataGridHelper.createAskDialog(sMessageBoxText, sCaption); //Диалог с предупреждение об удалении

            if (result == MessageBoxResult.Yes) //Пользователь поддтвердил удаление
            {
                db.removeWeek(date); //Удаление недели из БД

                DialogResult = true;
                this.Close();
            }
        }

        private void btnCreateWeek_Click(object sender, RoutedEventArgs e)
        {
            if (operation == "Add") //Добавление новой недели
            {
                managmentWeek();
            }
            else if (operation == "Remove")
            {
                removeWeek();
            }
            else
            {
                managmentWeek();                
            }
        }
    }
}
