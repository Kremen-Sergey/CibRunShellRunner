using System.Collections.Generic;
using System.Windows;
using CIBRunShellRunner.Models;
using CIBRunShellRunner.Properties;
using CIBRunShellRunner.ViewModels;

namespace CIBRunShellRunner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LaunchParameters launchParameters = new LaunchParameters()
            { 
                Bit32 = Settings.Default.Bit32Checked,
                Bit64 = Settings.Default.Bit64Checked,
                CIBRunShellDirectory = Settings.Default.CIBRunShellDirectoryText,
                MemoryLimit = Settings.Default.MemoryLimitText,
                OutputDirectory = Settings.Default.OutputDirectoryText,
                ProcessCountLimitList = new List<int>(),
                TimeLimit = Settings.Default.TimeLimitText,
                BitIsSelectedFlag=true,
            };
           for (int i = 1; i <= 10; i++)
            {
                launchParameters.ProcessCountLimitList.Add(i);
            }
            MainWindow view = new MainWindow();
            MainViewModel viewModel = new MainViewModel(launchParameters);
            view.DataContext = viewModel;
            view.Show();
        }
    }
}
