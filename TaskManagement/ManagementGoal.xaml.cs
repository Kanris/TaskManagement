using Xceed.Wpf.Toolkit;
using System;
using System.Windows;
using System.Windows.Media;
using TaskManagement.DatabaseHandler;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for ManagementGoal.xaml
    /// </summary>
    public partial class ManagementGoal : Window
    {
        DBHandler db = new DBHandler(); //Подключение к БД

        string operation = String.Empty;
        int id; //ID цели для изменений
        string goalName = String.Empty;

        //Создание формы для добавление цели
        public ManagementGoal(string operation) 
        {
            InitializeComponent();
            this.operation = operation;

            this.Title = "Добавление цели";
            this.btnRemoveTask.Visibility = System.Windows.Visibility.Collapsed;
        }

        //Создание формы на изменение цели
        public ManagementGoal(string operation, string goal)
        {
            InitializeComponent();

            this.operation = operation;
            this.id = db.getGoalID(goal); //Получаем ID цели по названию
            this.goalName = goal;

            string color = db.getGoalColor(goal); //Получаем цвет цели
            txGoalName.Text = goal; //Вывод имени цели в txGoalName
            cpColor.SelectedColor = (Color)ColorConverter.ConvertFromString(color); //Вывод цвета в cpColor

            this.Title = "Изминение/Удаление цели";
            this.btnSaveGoal.Content = "Изменить";
        }

        private void btnSaveGoal_Click(object sender, RoutedEventArgs e)
        {
            string goalColor = cpColor.SelectedColor.ToString(); //Получаем выбраный цвет

            if (operation == "Add") //Добавление цели
            {
                string goalName = txGoalName.Text.Trim(); //Имя цели

                if (goalName.Length > 0) //Имя цели было введено
                {
                    if (!db.isGoalExist(goalName)) //Проверка на то, что цель с таким именем не сущетсвует
                    {
                        db.addGoal(goalName, goalColor); //Добавление цели в БД
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Цель с таким названием уже существует!");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Введите название цели!");
                }
            }
            else
            {
                string goalName = txGoalName.Text; //Имя цели

                if (goalName.Length > 0) //Имя цели было введено
                {
                    db.updateGoal(id, goalName, goalColor); //Изменение информации в БД
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    System.Windows.MessageBox.Show("Введите название цели!");
                }
            }
        }

        private void btnRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Вы действительно хотите удалить цель - " + this.goalName + "?";
            string sCaption = "Удаление цели!";

            MessageBoxResult result = DataGridHelper.createAskDialog(sMessageBoxText, sCaption); //Создание диалого с прежупреждением

            if (result == MessageBoxResult.Yes) //Пользователь поддтвердил удаление
            {
                db.removeGoal(this.id); //Удаляем цель из БД
                DialogResult = true;
                this.Close();
            }
        }
    }
}
