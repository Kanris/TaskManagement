using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskManagement.DatabaseHandler;
using TaskManagement.DataGridHelper;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DBHandler db = new DBHandler(); //Подключение к БД

        public MainWindow()
        {
            InitializeComponent();
        }

        #region show window
        private void showOverviewWindow(string date)
        {
            OverviewTask overviewWindow = new OverviewTask(date);

            overviewWindow.ShowDialog(); //Вызов окна обзора недель
            updateItemsDataGrid();
        }
        private void showOverviewTaskWithoutWeek(string operation)
        {
            OverviewTaskWithoutWeek overviewWindow = new OverviewTaskWithoutWeek(operation);

            overviewWindow.ShowDialog(); //Вызов окна обзора недель
            updateItemsDataGrid();
        }

        private void showManagementWeek(string operation)
        {
            ManagementWeek weekWindow = new ManagementWeek(operation);

            if (weekWindow.ShowDialog() == true)
            {
                updateWholeDataGrid();
                checkWeekButtonsAvailability();
            }
        }

        private void showManagementGoal(string operation, string name)
        {
            ManagementGoal goalWindow;
            if (name.Length == 0) goalWindow = new ManagementGoal(operation); //Вызов окна на добавление
            else goalWindow = new ManagementGoal(operation, name);

            if (goalWindow.ShowDialog() == true) //Если была добавлена новая цель
            {
                updateWholeDataGrid();
                checkTaskButtonAvailability();
            }
        }
        #endregion

        #region weeks
        //Добавление недели
        private void btnAddWeek_Click(object sender, RoutedEventArgs e)
        {
            showManagementWeek("Add");
        }

        private void btnEditWeek_Click(object sender, RoutedEventArgs e)
        {
            int weeksCount = db.getWeeksCount(); //Получить количество недель в БД

            if (weeksCount > 0) //Если недели есть в БД
            {
                showManagementWeek("Edit");

            }
        }

        //Удаление недели
        private void btnRemoveWeek_Click(object sender, RoutedEventArgs e)
        {
            int weeksCount = db.getWeeksCount(); //Получить количество недель в БД

            if (weeksCount > 0) //Если недели есть в БД
            {
                showManagementWeek("Remove");
            }
        }
        #endregion

        #region goals
        //Добавление цели
        private void btnAddGoal_Click(object sender, RoutedEventArgs e)
        {
            showManagementGoal("Add", String.Empty);
        }
        #endregion

        #region overview
        private void dgTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgTasks.CurrentCell.Column != null)
            {
                int index = dgTasks.CurrentCell.Column.DisplayIndex;

                if (index > 1 && index < dgTasks.Columns.Count - 1) //Выбраный столбец - неделя
                {
                    string date = (string)dgTasks.Columns[index].Header;

                    showOverviewWindow(date);
                }
                else if (index == 1)
                {
                    showOverviewTaskWithoutWeek("Flow");
                }
                else if (index == dgTasks.Columns.Count - 1)
                {
                    showOverviewTaskWithoutWeek("Completed");
                }
                else if (index == 0)
                {
                    Item selectedItem = (Item)dgTasks.SelectedItem;

                    showManagementGoal("Edit", selectedItem.Goal);
                }
            }
        }
        #endregion

        #region task
        //Добавление задачи
        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.Items.Count > 0)
            {
                ManagamentTask taskWindow = new ManagamentTask("Add", String.Empty);

                if (taskWindow.ShowDialog() == true)
                {
                    updateItemsDataGrid();
                }
            }
        }
        #endregion

        //Создание столбцов в dgTasks
        private void createColumnsDG()
        {
            DataGridTextColumn goalColumn = new DataGridTextColumn(); //Столбец с целями
            goalColumn.Header = "Цели"; //Текст заголовка стобца
            goalColumn.Binding = new Binding("Goal");
            goalColumn.CanUserSort = false;
            goalColumn.MinWidth = 100;

            DataTemplate dataTemplate = FindResource("manageAreaCellTemplate") as DataTemplate;
            DataGridTemplateColumn unsortedColumn = DGHelper.createNewColumn("Поток задач", dataTemplate); //Столбец с задачами которым не присвоена неделя
            DataGridTemplateColumn completeColumn = DGHelper.createNewColumn("Выполненные", dataTemplate); //Столбец с выполнеными задачами
            
            dgTasks.Columns.Add(goalColumn); //Добавление в dgTask столбца с целями
            dgTasks.Columns.Add(unsortedColumn); //Добавление в dgTask столбца с задачами

            List<String> listOfWeeks = db.getWeeksFromTable(); //Получение из БД списка недель

            //listOfWeeks.Count != 0 //Если в БД были созданы недели
            foreach (String week in listOfWeeks) //создание столбцов с датой этой недели
            {
                DataGridTemplateColumn weekColumn = DGHelper.createNewColumn(week, dataTemplate);
                dgTasks.Columns.Add(weekColumn); //Добавление столбца в dgTask
            }

            dgTasks.Columns.Add(completeColumn); //Добавление в dgTask столбца с выполнеными задачами
        }

        //Сравнение текущей даты, со всеми неделями dgTasks
        private void checkWeeks()
        {
            if (dgTasks.Columns.Count > 3)
            {
                DateTime currentDate = DateTime.Now;
                Style style = FindResource("OverdueWeek") as Style;

                for (int i = 2; i < dgTasks.Columns.Count - 1; ++i)
                {
                    string coulumnDateString = dgTasks.Columns[i].Header.ToString();
                    DateTime columnDate = Convert.ToDateTime(coulumnDateString).AddDays(6);

                    if (currentDate.CompareTo(columnDate) > 0) dgTasks.Columns[i].HeaderStyle = style;
                    else break;
                    
                }
            }
        }

        //Добавление строк с целями в dgTask
        private void createGoalsDG() 
        {
            Dictionary<String, String> goals = db.getGoalsFromTable(); //Получение списка целей

            foreach (string goal in goals.Keys) //Получение имени цели
            {
                dgTasks.Items.Add(new Item() { Goal = goal }); //Добавление строки в dgTask
            }
        }

        //Получение картинки по приоритету
        private Image getImage(int priority)
        {
            Image Img = new Image();
        
            string baseDirectory = $"pack://application:,,,/Images/{priority}.png"; //путь к картинке

            Uri pathToImages = new Uri(baseDirectory);
            Img.Source = new BitmapImage(pathToImages);

            return Img;
        }
        private StackPanel createStackPanel()
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            return sp;
        }
        //Отображение списка задача в dgTask
        private void addTaskStickerToDG(int goalID, int? weekID, string isCompleted, int row, int column)
        {
            List<int> taskCounts; //Список задач

            if (weekID.HasValue) //Задача с назначеной неделью
            {
                taskCounts = db.getTaskCount(goalID, weekID.Value, isCompleted);
            }
            else //Задача без недели
            {
                taskCounts = db.getTaskCount(goalID, isCompleted);
            }

            int itemAdded = 0;
            List<StackPanel> lsp = new List<StackPanel>();
            StackPanel sp = createStackPanel();
            lsp.Add(sp);

            //Добавляем картинки по приоритетам
            for (int i = 0; i < taskCounts.Count; ++i)
            {
                for (int j = 0; j < taskCounts[i]; ++j)
                {
                    if (itemAdded == 4)
                    {
                        itemAdded = 0;
                        sp = createStackPanel();
                        lsp.Add(sp);
                    }

                    sp.Children.Add(getImage(i));
                    ++itemAdded;
                }
            }

            DataGridCell dgs = DGHelper.GetCell(dgTasks, row, column); //Получаем ячейку dgTask
            StackPanel mainStackPanel = DGHelper.FindVisualChild<StackPanel>(dgs); //Получаем объект в ячейке

            for (int i = 0; i < lsp.Count; ++i)
            {
                mainStackPanel.Children.Add(lsp[i]);
            }
        }

        //Добавление стикеров задач в dgTask
        private void addTasksToDG()
        {
            for (int i = 0; i < dgTasks.Items.Count; ++i)
            {
                DataGridCell dgs = DGHelper.GetCell(dgTasks, i, 0);
                string goalName = (dgs.Content as TextBlock).Text; //Получение значения в первой колонке - "Цели"
                int goalID = db.getGoalID(goalName); //Индекс цели

                addTaskStickerToDG(goalID, null, "False", i, 1);

                for (int j = 2; j < dgTasks.Columns.Count - 1; ++j)
                {
                    string weekName = dgTasks.Columns[j].Header.ToString(); //Получение названия недели
                    int weekID = db.getWeekID(weekName); //Индекс недели

                    addTaskStickerToDG(goalID, weekID, "False", i, j);
                }

                addTaskStickerToDG(goalID, null, "True", i, dgTasks.Columns.Count - 1);

            }

        }

        private void checkWeekButtonsAvailability()
        {
            int weeksCount = db.getWeeksCount();
            bool isButtonEnabled = false;

            if (weeksCount > 0)
            {
                isButtonEnabled = true;
            }

            btnEditWeek.IsEnabled = isButtonEnabled;
            btnRemoveWeek.IsEnabled = isButtonEnabled;
        }

        private void checkTaskButtonAvailability()
        {
            bool isButtonEnabled = false;

            if (dgTasks.Items.Count > 0)
            {
                isButtonEnabled = true;
            }

            btnAddTask.IsEnabled = isButtonEnabled;
        }

        //Заполнить dgTask
        private void filldgTask()
        {
            createColumnsDG(); //Создание столбцов 
            createGoalsDG(); //Создание строк с целями
        }

        private void updateInformationdgTask()
        {
            DGHelper.addColorsToGoals(dgTasks, db);//цвет целям
            addTasksToDG(); //стикеры
            checkWeeks(); //Поиск просроченых недель
        }

        private void updateWholeDataGrid()
        {
            dgTasks.Columns.Clear(); //очищаем DataGrid
            dgTasks.Items.Clear();

            filldgTask(); //Заполняем его текстовой информацией
            
            dgTasks.UpdateLayout();
            updateInformationdgTask();
        }

        private void updateItemsDataGrid()
        {
            dgTasks.Items.Clear();
            createGoalsDG();

            dgTasks.UpdateLayout();
            updateInformationdgTask();
        }

        //После загрузки окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateWholeDataGrid();
            checkWeekButtonsAvailability();
            checkTaskButtonAvailability();
        }
    }
}
