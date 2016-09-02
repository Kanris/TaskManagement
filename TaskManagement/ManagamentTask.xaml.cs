using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for ManagamentTask.xaml
    /// </summary>
    public partial class ManagamentTask : Window
    {
        DBHandler db = new DBHandler(); //Подключение к БД

        string operation = string.Empty; //Операция
        string date = string.Empty; //Дата недели
        int taskID = -1;

        bool isUserChangeTime = false;

        public ManagamentTask(string operation, int id)
        {
            InitializeComponent();
            this.operation = operation;

            addWeeks(date); //Добавление элементов в cmbWeeks
            addGoals(); //Добавление целей
            clearTime(); //Очищаем поля с временем

            this.taskID = id; //ID задачи для редактирования

            initWindowElements();

        }

        public ManagamentTask(string operation, string date)
        {
            InitializeComponent();
            this.operation = operation;
            cbDone.Visibility = System.Windows.Visibility.Collapsed;
            btnRemoveTask.Visibility = System.Windows.Visibility.Collapsed;

            addWeeks(date); //Добавление элементов в cmbWeeks

            addGoals(); //Добавление целей

            clearTime(); //Очищаем поля с временем
        }

        private void initWindowElements()
        {
            List<String> listOfTask = db.getTaskByID(this.taskID);

            txName.Text = listOfTask[0];
            txTime.Text = listOfTask[3];

            if (listOfTask[4].Length > 0 && listOfTask[5].Length > 0)
            {
                timeStart.Value = Convert.ToDateTime(listOfTask[4]);
                timeEnd.Value = Convert.ToDateTime(listOfTask[5]);
            }

            cmbPriority.SelectedIndex = Convert.ToInt32(listOfTask[7]);

            cbDone.IsChecked = Convert.ToBoolean(listOfTask[8]);

            cmbGoal.SelectedValue = listOfTask[1];

            if (listOfTask[2].Length > 0)
            {
                DateTime dtWeek = Convert.ToDateTime(listOfTask[2]);
                cmbWeek.SelectedValue = dtWeek.ToShortDateString();
            }

            if (listOfTask[6].Length > 0)
            {
                cmbDay.SelectedIndex = Convert.ToInt32(listOfTask[6]);
            }

        }

        private void addWeeks(string date)
        {
            this.date = date; //Неделя задачи

            ObservableCollection<string> listWeek = new ObservableCollection<string>(db.getWeeksFromTable()); //Список для ComboBox
            listWeek.Add("Без недели"); //Пункт без недели

            cmbWeek.ItemsSource = listWeek;

            cmbWeek_changeSelectedIndex(listWeek); //Изменить выбраный элемент
        }

        private void addGoals()
        {
            List<string> listOfGoal = db.getGoalsListFromTable(); //Получение списка целей
            ObservableCollection<string> listGoal = new ObservableCollection<string>(listOfGoal); //Список для ComboBox
            cmbGoal.ItemsSource = listGoal;

            cmbGoal.SelectedIndex = 0;
        }

        private void clearTime()
        {
            DateTime dt = new DateTime(2014, 11, 1, 12, 0, 0);
            timeStart.Value = dt;
            timeEnd.Value = dt;
        }

        private void cmbWeek_changeSelectedIndex(ObservableCollection<string> list)
        {
            int index = cmbWeek.Items.Count - 1; //Индекс пункта "Без недели"

            if (this.date != string.Empty && list.Count > 0) //Если была передана дата и есть недели в БД
            {
                index = list.IndexOf(this.date); //Найти индекс переданой даты
            }

            cmbWeek.SelectedIndex = index; //Изменить выбраный элемент в cmbWeek
        }

        private bool calculateTimeNeeded()
        {
            DateTime timeStartValue = (DateTime)timeStart.Value;
            DateTime timeEndValue = (DateTime)timeEnd.Value;

            TimeSpan span = timeEndValue.Subtract(timeStartValue);

            double minutes = span.TotalMinutes;

            if (minutes >= 0)
            {
                txTime.Text = minutes.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void initTimeValues(out DateTime? timeStartString, out DateTime? timeEndString, 
            out int time, string timeNeeded)
        {
            DateTime dt = new DateTime(2014, 11, 1, 12, 0, 0);
            time = 0;

            timeStartString = null; //Время начала задачи
            timeEndString = null; //Время конца задачи

            if (timeStart.Value != dt || timeEnd.Value != dt) //Если диапазон времени был изменен
            {
                timeStartString = (DateTime)timeStart.Value;
                timeEndString = (DateTime)timeEnd.Value;

                double convertFromDouble = Convert.ToDouble(timeNeeded);
                time = Convert.ToInt32(convertFromDouble);
            }
            else
            {
                double convertFromDouble = Convert.ToDouble(timeNeeded);
                time = Convert.ToInt32(convertFromDouble);
            }
        }

        private void initWeekDayValues(out int weekID, out int day, string weekDate)
        {
            weekID = -1; //Неделя не выбрана - Поток задач
            day = -1; //Неделя не выбрана - не выберается день

            if (cmbWeek.SelectedIndex != cmbWeek.Items.Count - 1) //Если выбрана неделя
            {
                weekID = db.getWeekID(weekDate); //ID недели
                day = cmbDay.SelectedIndex; //День недели
            }
        }

        private void btnSaveTask_Click(object sender, RoutedEventArgs e)
        {
            string taskName = txName.Text;

            if (taskName.Length > 0)
            {
                string timeNeeded = txTime.Text; //Время на задачу
                int time;
                DateTime? timeStartString; //Время начала задачи
                DateTime? timeEndString; //Время конца задачи
                initTimeValues(out timeStartString, out timeEndString, out time, timeNeeded);

                string weekDate = cmbWeek.SelectedValue.ToString();
                int weekID; //Неделя не выбрана - Поток задач
                int day; //Неделя не выбрана - не выберается день
                initWeekDayValues(out weekID, out day, weekDate);

                int goalID = db.getGoalID(cmbGoal.SelectedValue.ToString()); //ID цели
                string status = cbDone.IsChecked.ToString(); //Статус выполнения задачи
                int priority = cmbPriority.SelectedIndex; //Приоритет задачи

                if (operation == "Add") //добавление задачи
                {
                    db.addTask(weekID, goalID, taskName, time, timeStartString, timeEndString, day, priority, status);
                }
                else //изменение задачи
                {
                    db.updateTask(taskID, weekID, goalID, taskName, time, timeStartString, timeEndString, day, priority, status);
                }

                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите название задачи!");
            }
        }

        private void btnRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Вы действительно хотите удалить задачу?";
            string sCaption = "Удаление цели!";

            MessageBoxResult result = DataGridHelper.createAskDialog(sMessageBoxText, sCaption); //Создание диалого с прежупреждением

            if (result == MessageBoxResult.Yes) //Пользователь поддтвердил удаление
            {
                db.removeTask(taskID); //Удаляем задачу из БД

                DialogResult = true;

                this.Close();
            }
        }

        //Изменение видимости выбора дня
        private void visibilityChooseOfDay(Visibility visibility)
        {
            cmbDay.Visibility = visibility;
            lblDay.Visibility = visibility;
            cmbDay.SelectedIndex = 0;
        }

        private void cmbWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Если неделя не задана спрятать выбор дней и наоборот
            if (cmbWeek.SelectedIndex != cmbWeek.Items.Count - 1) visibilityChooseOfDay(System.Windows.Visibility.Visible);
            else visibilityChooseOfDay(System.Windows.Visibility.Collapsed);
        }

        private void txTime_KeyUp(object sender, KeyEventArgs e)
        {
            clearTime();
        }

        //Ввод только цифр в txTime
        private void txTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void timeStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (timeStart != null && timeEnd != null && !isUserChangeTime)
            {
                if (!calculateTimeNeeded()) timeEnd.Value = timeStart.Value;

                calculateTimeNeeded();
            }
        }

        private void timeEnd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (timeStart != null && timeEnd != null && !isUserChangeTime)
            {
                DateTime valueStart = (DateTime)timeStart.Value;
                DateTime valueEnd = (DateTime)timeEnd.Value;

                if (!calculateTimeNeeded()) timeEnd.Value = timeStart.Value;
            }
        }

        private void txTime_GotFocus(object sender, RoutedEventArgs e)
        {
            isUserChangeTime = true;
        }

        private void txTime_LostFocus(object sender, RoutedEventArgs e)
        {
            isUserChangeTime = false;
            if (txTime.Text.Length == 0) txTime.Text = "0";
        }

    }
}
