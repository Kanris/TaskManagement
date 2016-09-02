using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Запуск одной копии приложения
        System.Threading.Mutex mut;
        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            string mutName = "TaskManagement.exe";
            mut = new System.Threading.Mutex(true, mutName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Приложение уже запущено!");
                Shutdown();
            } 

        }
    }
}
