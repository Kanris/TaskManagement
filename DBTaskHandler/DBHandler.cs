using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace TaskManagement.DatabaseHandler
{
    public class DBHandler
    {
        SqlConnection dbConnection; //Подключение к БД
        SqlCommand sqlCommand; //Переменная для запросов к БД
        const string DB_FILE_NAME = "dbTaskManagement.mdf";

        public DBHandler()
        {
            if (dbConnection == null)
            {
                string directoryName = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                string pathToDB =  $"{directoryName}\\{DB_FILE_NAME}";

                string ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30", pathToDB);
                dbConnection = new SqlConnection(ConnectionString);

                sqlCommand = new SqlCommand(); //Переменная для запросов к БД
                sqlCommand.Connection = dbConnection; //Подключение переменной к БД
            }
        }

        //------------------------ Запросы к dbo.Week
        //Получение списка недель из таблицы dbo.Week
        public List<String> getWeeksFromTable()
        {
            dbConnection.Open(); //Открываем подключение

            sqlCommand.CommandText = "SELECT startDate FROM week ORDER BY startDate"; //Запрос к БД на получение списка недель
            SqlDataReader dr = sqlCommand.ExecuteReader();

            List<String> listOfWeeks = new List<string>(); //список нелель

            if (dr.HasRows) //Если в БД есть недели
            {
                while (dr.Read())
                {
                    DateTime week = Convert.ToDateTime(dr[0].ToString()); //Получение даты из БД
                    listOfWeeks.Add(week.ToShortDateString()); //Запись короткой даты в список
                }
            }

            dbConnection.Close(); //Закрываем подключение

            return listOfWeeks; //Возвращаем полученый список недель
        }

        //Получение количества недель из dbo.Week
        public int getWeeksCount()
        {
            dbConnection.Open();

            sqlCommand.CommandText = "SELECT COUNT(startDate) FROM week"; //Запрос на получение количества недель в БД
            int result = (int)sqlCommand.ExecuteScalar(); //Получение результата

            dbConnection.Close();

            return result;
        }

        //Проверка на то, что неделя с выбраной датой уже существует dbo.Week
        public bool isWeekExist(string date)
        {
            dbConnection.Open();
            DateTime sd = Convert.ToDateTime(date);

            //Запрос на получение ИД недели с указаными названием
            sqlCommand.CommandText = "SELECT weekId FROM Week WHERE startDate=@date";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@date", sd);


            bool result = sqlCommand.ExecuteScalar() != null ? true : false; //Проверка на то что ИД недели была найдена

            dbConnection.Close();

            return result;
        }

        //Добавление новой недели dbo.Week
        public void addWeek(string date)
        {
            dbConnection.Open();
            DateTime sd = Convert.ToDateTime(date);

            sqlCommand.CommandText = "INSERT INTO Week(startDate) VALUES(@date)";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@date", sd);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }

        public void updateWeek(string oldDate, string newDate)
        {
            dbConnection.Open();

            DateTime dOldDate = Convert.ToDateTime(oldDate);
            DateTime dNewDate = Convert.ToDateTime(newDate);

            sqlCommand.CommandText = "UPDATE WEEK SET startDate=@newDate WHERE startDate=@oldDate";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@oldDate", dOldDate);
            sqlCommand.Parameters.AddWithValue("@newDate", dNewDate);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }

        //Удаление недели dbo.Week
        public void removeWeek(string date)
        {
            dbConnection.Open();
            DateTime sd = Convert.ToDateTime(date);

            sqlCommand.CommandText = "DELETE FROM week WHERE startDate=@startDate";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@startDate", sd);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }

        //Получение ID по дате недели
        public int getWeekID(string date)
        {
            dbConnection.Open();
            DateTime sd = Convert.ToDateTime(date);

            sqlCommand.CommandText = "SELECT weekID FROM week WHERE startDate=@startDate";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@startDate", sd);

            SqlDataReader dr = sqlCommand.ExecuteReader();
            int id = -1; //ID цели

            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    id = Convert.ToInt32(dr[0]);
                }
            }

            dbConnection.Close();

            return id;
        }

        //------------------------------- Запросы к dbo.Goal
        //Проверка на то, что цель с выбраным название уже существует dbo.Goal
        public bool isGoalExist(string goalName)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "SELECT name FROM Goal WHERE name=@Name";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@Name", goalName);

            SqlDataReader dr = sqlCommand.ExecuteReader(); //Считывание результата
            bool result = false;

            if (dr.HasRows) result = true; //Цель с таким именем была найдена

            dbConnection.Close();

            return result;
        }

        //Добавление новой цели dbo.Goal
        public void addGoal(string goalname, string color)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "INSERT INTO Goal(name, color) VALUES(@name, @color)";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@name", goalname);
            sqlCommand.Parameters.AddWithValue("@color", color);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }

        //Получение ID по имени цели
        public int getGoalID(string goalName)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "SELECT goalID FROM goal WHERE name=@name";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@name", goalName);

            SqlDataReader dr = sqlCommand.ExecuteReader();
            int id = -1; //ID цели

            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    id = Convert.ToInt32(dr[0]);
                }
            }

            dbConnection.Close();

            return id;
        }

        //Получение цвета по имени цели
        public string getGoalColor(string goalName)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "SELECT color FROM goal WHERE name=@name"; //Запрос на получение цвета

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@name", goalName);

            string color = (string)sqlCommand.ExecuteScalar();

            dbConnection.Close();

            return color;
        }

        //Добавить изменения цели в БД
        public void updateGoal(int id, string goalName, string color)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "UPDATE GOAL SET name=@name, color=@color WHERE goalid=@id"; //Запрос на изменение цели

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@name", goalName);
            sqlCommand.Parameters.AddWithValue("@color", color);
            sqlCommand.Parameters.AddWithValue("@id", id);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }

        //Удаление цели dbo.Goal
        public void removeGoal(int goalID)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "DELETE FROM Goal WHERE goalID=@goalID";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@goalID", goalID);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }

        //Получение списка целей из таблицы dbo.Goal
        public Dictionary<String, String> getGoalsFromTable()
        {
            dbConnection.Open();

            sqlCommand.CommandText = "SELECT name, color FROM Goal"; //Запрос на получение списка целей в формате Имя/Цвет
            SqlDataReader dr = sqlCommand.ExecuteReader();

            Dictionary<String, String> listOfGoals = new Dictionary<String, String>();

            if (dr.HasRows) //Если есть цели в БД
            {
                while (dr.Read())
                {
                    //Формируем список, где имя цели - ключ, а данные - цвет
                    string name = dr[0].ToString();
                    string color = dr[1].ToString();

                    listOfGoals.Add(name, color);
                }
            }

            dbConnection.Close();

            return listOfGoals;
        }

        //Получение списка целей из таблицы dbo.Goal
        public List<string> getGoalsListFromTable()
        {
            dbConnection.Open();

            sqlCommand.CommandText = "SELECT name FROM Goal"; //Запрос на получение списка целей в формате Имя/Цвет
            SqlDataReader dr = sqlCommand.ExecuteReader();

            List<string> listOfGoals = new List<string>();

            if (dr.HasRows) //Если есть цели в БД
            {
                while (dr.Read())
                {
                    string name = dr[0].ToString();
                    listOfGoals.Add(name);
                }
            }

            dbConnection.Close();

            return listOfGoals;
        }

        //------------------------------- Запросы к dbo.Task
        //Добавление Задачи
        public void addTask(int weekID, int goalID, string name, int time, DateTime? timeFrom, DateTime? timeTo, int day, int priority, string status)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "INSERT INTO Task(weekID, goalID, name, time, timeFrom, timeTo, day, priority, status) VALUES(@weekID, @goalID, @Name, @Time, @TimeFrom, @TimeTo, @Day, @Priority, @Status)";

            sqlCommand.Parameters.Clear();
            if (weekID != -1) sqlCommand.Parameters.AddWithValue("@weekId", weekID);
            else sqlCommand.Parameters.AddWithValue("@weekId", DBNull.Value);

            sqlCommand.Parameters.AddWithValue("@goalID", goalID);
            sqlCommand.Parameters.AddWithValue("@Name", name);
            sqlCommand.Parameters.AddWithValue("@Time", time);

            if (timeFrom.HasValue)
            {
                DateTime timeFromDT = timeFrom.Value;
                DateTime timeToDT = timeTo.Value;

                sqlCommand.Parameters.AddWithValue("@TimeFrom", timeFromDT);
                sqlCommand.Parameters.AddWithValue("@TimeTo", timeToDT);
            }
            else
            {
                sqlCommand.Parameters.AddWithValue("@TimeFrom", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@TimeTo", DBNull.Value);
            }

            sqlCommand.Parameters.AddWithValue("@Day", day);
            sqlCommand.Parameters.AddWithValue("@Priority", priority);
            sqlCommand.Parameters.AddWithValue("@Status", status);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }


        //Удаление Задачи
        public void removeTask(int taskID)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "DELETE FROM Task WHERE taskID=@id";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@id", taskID);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();
        }

        //Изменение задачи
        public void updateTask(int taskID, int weekID, int goalID, string name, int time, DateTime? timeFrom, DateTime? timeTo, int day, int priority, string status)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "UPDATE TASK SET weekID=@weekID, goalID=@goalID, name=@Name, time=@Time, timeFrom=@TimeFrom, timeTo=@TimeTo, day=@Day, priority=@Priority, status=@Status WHERE taskID=@taskID";

            sqlCommand.Parameters.Clear();
            if (weekID != -1) sqlCommand.Parameters.AddWithValue("@weekId", weekID);
            else sqlCommand.Parameters.AddWithValue("@weekId", DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@goalID", goalID);
            sqlCommand.Parameters.AddWithValue("@Name", name);
            sqlCommand.Parameters.AddWithValue("@Time", time);
            if (timeFrom.HasValue)
            {
                DateTime timeFromDT = timeFrom.Value;
                DateTime timeToDT = timeTo.Value;

                sqlCommand.Parameters.AddWithValue("@TimeFrom", timeFromDT);
                sqlCommand.Parameters.AddWithValue("@TimeTo", timeToDT);
            }
            else
            {
                sqlCommand.Parameters.AddWithValue("@TimeFrom", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@TimeTo", DBNull.Value);
            }
            sqlCommand.Parameters.AddWithValue("@Day", day);
            sqlCommand.Parameters.AddWithValue("@Priority", priority);
            sqlCommand.Parameters.AddWithValue("@Status", status);
            sqlCommand.Parameters.AddWithValue("@taskID", taskID);

            sqlCommand.ExecuteNonQuery();

            dbConnection.Close();

        }

        //Получение всей информации о задачи по ID
        public List<string> getTaskByID(int taskID)
        {
            dbConnection.Open();
            sqlCommand.CommandText = "SELECT Task.name, Goal.name, Week.startDate, time, timeFrom, timeTo, day, priority, status FROM Task INNER JOIN Goal ON task.goalID = goal.goalID LEFT OUTER JOIN Week ON task.weekID = week.weekID WHERE taskID=@taskID";
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@taskID", taskID);

            SqlDataReader dr = sqlCommand.ExecuteReader();

            List<string> listOfTask = new List<string>();

            if (dr.HasRows) //Если задача найдена
            {
                if (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; ++i) listOfTask.Add(dr[i].ToString());
                }
            }

            dbConnection.Close();

            return listOfTask;
        }

        public int getTaskCount(int weekID)
        {
            dbConnection.Open();

            int count = 0;

            sqlCommand.CommandText = "SELECT COUNT(taskID) FROM task WHERE weekID = @weekID AND status = @status";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@weekID", weekID);
            sqlCommand.Parameters.AddWithValue("@status", "false");

            SqlDataReader dr = sqlCommand.ExecuteReader();

            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    count = Convert.ToInt32(dr[0].ToString());
                }
            }

            dbConnection.Close();
            return count;
        }

        //Получение количества задач недели
        public List<int> getTaskCount(int goalID, int weekID, string isComplete)
        {
            dbConnection.Open();
            List<int> taskCount = new List<int>(); //Список, в котором будут хранится количество задач по приоритетам
            const int priorityCount = 4; //Количество приоритетов

            sqlCommand.CommandText = "SELECT COUNT(taskID) FROM task WHERE goalID = @goalID AND weekID = @weekID AND status = @status AND priority = @priority";

            for (int i = 0; i < priorityCount; ++i)
            {
                sqlCommand.Parameters.Clear();
                sqlCommand.Parameters.AddWithValue("@goalID", goalID);
                sqlCommand.Parameters.AddWithValue("@weekID", weekID);
                sqlCommand.Parameters.AddWithValue("@status", isComplete);
                sqlCommand.Parameters.AddWithValue("@priority", i);

                SqlDataReader dr = sqlCommand.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        int value = Convert.ToInt32(dr[0].ToString()); //Получение значения приоритета
                        taskCount.Add(value);
                    }
                }

                dr.Close();
            }

            dbConnection.Close();

            return taskCount;
        }

        //Получение количества задач без недели или выполненых
        public List<int> getTaskCount(int goalID, string isComplete)
        {
            dbConnection.Open();
            List<int> taskCount = new List<int>(); //Список, в котором будут хранится количество задач по приоритетам
            const int priorityCount = 4; //Количество приоритетов

            if (isComplete == "False") //Выборка - "Поток задач"
            {
                sqlCommand.CommandText = "SELECT COUNT(taskID) FROM task WHERE goalID = @goalID AND weekID IS NULL AND status = @status AND priority = @priority";
            }
            else //Выборка - "Выполенные"
            {
                sqlCommand.CommandText = "SELECT COUNT(taskID) FROM task WHERE goalID = @goalID AND status = @status AND priority = @priority";
            }

            for (int i = 0; i < priorityCount; ++i)
            {
                sqlCommand.Parameters.Clear();
                sqlCommand.Parameters.AddWithValue("@goalID", goalID);
                sqlCommand.Parameters.AddWithValue("@status", isComplete);
                sqlCommand.Parameters.AddWithValue("@priority", i);

                SqlDataReader dr = sqlCommand.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        int value = Convert.ToInt32(dr[0].ToString()); //Получение значения приоритета
                        taskCount.Add(value);
                    }
                }

                dr.Close();
            }

            dbConnection.Close();

            return taskCount;
        }

        public List<List<string>> getTasks(int weekID, string status)
        {
            dbConnection.Open();
            sqlCommand.CommandText = "SELECT taskID, Goal.Name, Task.name, priority, day, time, timeFrom, timeTo FROM TASK INNER JOIN Goal on task.goalID = goal.goalID WHERE weekID=@weekID AND status=@status ORDER BY Goal.Name, priority, time ASC";
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@weekID", weekID);
            sqlCommand.Parameters.AddWithValue("@status", status);

            SqlDataReader dr = sqlCommand.ExecuteReader();
            List<List<string>> listOfTask = new List<List<string>>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    List<string> taskInfo = new List<string>();
                    for (int i = 0; i < dr.FieldCount; ++i)
                    {
                        taskInfo.Add(dr[i].ToString());
                    }

                    listOfTask.Add(taskInfo);
                }
            }

            dbConnection.Close();

            return listOfTask;
        }

        public List<List<string>> getTasks(string status)
        {
            dbConnection.Open();
            if (status == "False")
            {
                sqlCommand.CommandText = "SELECT taskID, Goal.Name, Task.name, priority, time, timeFrom, timeTo FROM TASK INNER JOIN Goal on task.goalID = goal.goalID WHERE weekID is NULL AND status=@status ORDER BY Goal.Name, priority, time ASC";

            }
            else
            {
                sqlCommand.CommandText = "SELECT taskID, Goal.Name, Task.name, priority, time, timeFrom, timeTo FROM TASK INNER JOIN Goal on task.goalID = goal.goalID WHERE status=@status ORDER BY Goal.Name, priority, time ASC";
            }
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@status", status);

            SqlDataReader dr = sqlCommand.ExecuteReader();
            List<List<string>> listOfTask = new List<List<string>>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    List<string> taskInfo = new List<string>();
                    for (int i = 0; i < dr.FieldCount; ++i)
                    {
                        taskInfo.Add(dr[i].ToString());
                    }

                    listOfTask.Add(taskInfo);
                }
            }

            dbConnection.Close();

            return listOfTask;
        }

        public int getTimeForDay(int day, int weekid, int taskID)
        {
            dbConnection.Open();

            sqlCommand.CommandText = "SELECT SUM(time) FROM Task WHERE day=@day AND weekid=@weekid AND taskID <> @taskID AND status=@status";
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@day", day);
            sqlCommand.Parameters.AddWithValue("@weekid", weekid);
            sqlCommand.Parameters.AddWithValue("@taskID", taskID);
            sqlCommand.Parameters.AddWithValue("@status", "False");

            SqlDataReader dr = sqlCommand.ExecuteReader();
            int count = 0;

            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    if (dr[0].ToString().Length > 0)
                    {
                        count = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }

            dbConnection.Close();

            return count;

        }

        public int getTimeForDay(string status)
        {
            dbConnection.Open();

            if (status == "False") sqlCommand.CommandText = "SELECT SUM(time) FROM Task WHERE weekID is NULL AND status = @status";
            else sqlCommand.CommandText = "SELECT SUM(time) FROM Task WHERE status = @status";

            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@status", status);

            SqlDataReader dr = sqlCommand.ExecuteReader();
            int count = 0;

            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    if (dr[0].ToString().Length > 0)
                    {
                        count = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }

            dbConnection.Close();

            return count;

        }
    }
}
