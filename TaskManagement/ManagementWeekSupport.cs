using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using TaskManagement.DatabaseHandler;
using TaskManagement.DataGridHelper;

namespace TaskManagement
{
    static class ManagementWeekSupport
    {

        public static void ChangeFormView(Operation operation, ComboBox cbWeeks, Button btnCreateWeek, DatePicker dpWeek, ManagementWeek window, DBHandler db)
        {
            switch (operation)
            {
                case Operation.Add: //На добавление
                    window.Title = "Добавить неделю"; //Изменить заголовок формы
                    cbWeeks.Visibility = System.Windows.Visibility.Collapsed; //Убрать ListBox с формы
                    break;

                case Operation.Edit: //На удаление
                    window.Title = "Изменить неделю"; //Изменить заголовок формы
                    initCBWeeks(db, cbWeeks);
                    btnCreateWeek.Content = "Изменить";
                    break;

                case Operation.Remove: //На удаление
                    window.Title = "Удалить неделю"; //Изменить заголовок формы
                    initCBWeeks(db, cbWeeks);
                    cbWeeks.Margin = new Thickness(35, 44, 0, 0); //Смещение ListBox на уровень с кнопкой
                    btnCreateWeek.Content = "Удалить";
                    dpWeek.Visibility = System.Windows.Visibility.Collapsed; //Убрать DatePicker с формы

                    break;
            }
        }

        public static void managmentWeek(DateTime? dtSelectedDate, ComboBox cbWeeks, DBHandler db, ManagementWeek window, Operation operation)
        {
            if (dtSelectedDate.HasValue)
            {
                string date = dtSelectedDate.Value.ToShortDateString(); //Получение даты в текстовом формате

                if (!db.isWeekExist(date)) //Проверка на то, что недели с выбраной датой не существует
                {
                    if (operation == Operation.Add) db.addWeek(date); //Добавление недели в БД
                    else db.updateWeek(cbWeeks.Text, date);

                    window.DialogResult = true;
                    window.Close();
                }
                else
                {
                    MessageBox.Show("Выбраная неделя уже добавлена в таблицу!");
                }
            }
            else
            {
                MessageBox.Show("Выберите неделю!");
            }

        }

        public static void removeWeek(ComboBox cbWeeks, ManagementWeek window, DBHandler db)
        {
            string date = cbWeeks.Text; //Получение выбраной недели из ListBox

            string sMessageBoxText = "Вы действительно хотите удалить выбраную неделю? (" + date + ")";
            string sCaption = "Удаление недели!";

            MessageBoxResult result = DGHelper.createAskDialog(sMessageBoxText, sCaption); //Диалог с предупреждение об удалении

            if (result == MessageBoxResult.Yes) //Пользователь поддтвердил удаление
            {
                db.removeWeek(date); //Удаление недели из БД
                
                window.DialogResult = true;
                window.Close();
            }
        }

        public static void ChangeSelectedDate(SelectionChangedEventArgs e, DatePicker dpWeek, ref DateTime? dtSelectedDate)
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

        private static void initCBWeeks(DBHandler db, ComboBox cbWeeks)
        {
            List<String> listOfWeek = db.getWeeksFromTable(); //Получение списка недель
            ObservableCollection<string> list = new ObservableCollection<string>(listOfWeek);

            cbWeeks.ItemsSource = list;
            cbWeeks.SelectedIndex = 0;
        }
    }
}
