using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Linq;
using System.Windows.Controls;
using CIBRunShellRunner.ViewModels;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Clipboard=System.Windows.Clipboard;

namespace CIBRunShellRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private string[] ScriptExtentionArray = {".ini",".xml", ".arg"};
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers

        #region Input files event handlers
        private void InputFiles_PreviewDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {

                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }
            if (isCorrect)
            {
                e.Effects = DragDropEffects.All;
                //OnPropertyChanged("InputFilesAndScriptsListsNotEmptyFlag");
            }
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void InputFiles_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {

                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (!InputFiles.Items.Contains(filename))
                        ((MainViewModel)DataContext).InputFilesList.Add(filename);
                }
            }
            e.Handled = true;
        }

        private void AddInputFiles(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = ((MainViewModel)DataContext).LastFolderForInputFilesList;
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Documents|*.pdf;*.rtf;"+"|Images|*.png;*.tiff;"+"|All files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string[] filenames = openFileDialog.FileNames;
                string folder = Path.GetDirectoryName(filenames[0]);
                foreach (string filename in filenames)
                {
                    if (!InputFiles.Items.Contains(filename))
                        ((MainViewModel)DataContext).InputFilesList.Add(filename);
                }
                ((MainViewModel) DataContext).LastFolderForInputFilesList = folder;
            }
        }

        private void RemoveSelectedInputFiles(object sender, RoutedEventArgs e)
        {
            for (int i=0; i<InputFiles.SelectedItems.Count;)
            {
                string filename = InputFiles.SelectedItems[i].ToString();
                ((MainViewModel)DataContext).InputFilesList.Remove(filename);
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InputFiles.SelectedItem == null)
                    throw new InvalidOperationException("No file selected");
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            string path = InputFiles.SelectedItem.ToString();
            ((MainViewModel)DataContext).OpenFile(path);
        }

        private void OpenFileWith(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InputFiles.SelectedItem == null)
                    throw new InvalidOperationException("No file selected");
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            string path = InputFiles.SelectedItem.ToString();
            ((MainViewModel)DataContext).OpenFileWith(path);
        }

        private void InputFiles_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                for (int i = 0; i < InputFiles.SelectedItems.Count; )
                {
                    string filename = InputFiles.SelectedItems[i].ToString();
                    ((MainViewModel)DataContext).InputFilesList.Remove(filename);
                }
            }
        }
        #endregion

        #region Script files event handlers
        private void ScriptFiles_PreviewDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    FileInfo info = new FileInfo(filename);
                    if (!ScriptExtentionArray.Contains(info.Extension))
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }
            if (isCorrect)
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void ScriptFiles_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {

                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    FileInfo info = new FileInfo(filename);
                    if ((!InputFiles.Items.Contains(filename)) && (ScriptExtentionArray.Contains(info.Extension)))
                        ((MainViewModel)DataContext).ScriptFilesList.Add(filename);
                }
            }

            e.Handled = true;
        }

        private void AddScripts(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = ((MainViewModel)DataContext).LastFolderForScriptsList;
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Script files|*.arg;*.ini;*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                string[] filenames = openFileDialog.FileNames;
                foreach (string filename in filenames)
                {
                    FileInfo info = new FileInfo(filename);
                    if ((!ScriptFiles.Items.Contains(filename)) && (ScriptExtentionArray.Contains(info.Extension)))
                        ((MainViewModel)DataContext).ScriptFilesList.Add(filename);
                }
                ((MainViewModel) DataContext).LastFolderForScriptsList = Path.GetDirectoryName(filenames[0]);
            }
        }

        private void RemoveSelectedScriptFiles(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ScriptFiles.SelectedItems.Count; )
            {
                string filename = ScriptFiles.SelectedItems[i].ToString();
                ((MainViewModel)DataContext).ScriptFilesList.Remove(filename);
            }
        }

        private void ScriptFiles_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                for (int i = 0; i < ScriptFiles.SelectedItems.Count; )
                {
                    string filename = ScriptFiles.SelectedItems[i].ToString();
                    ((MainViewModel)DataContext).ScriptFilesList.Remove(filename);
                }
            }
        }

        private void OpenScript(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ScriptFiles.SelectedItem == null)
                    throw new InvalidOperationException("No script selected");
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            string path = ScriptFiles.SelectedItem.ToString();
            ((MainViewModel)DataContext).OpenFile(path);
        }

        private void OpenScriptWith(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ScriptFiles.SelectedItem == null)
                    throw new InvalidOperationException("No script selected");
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            string path = ScriptFiles.SelectedItem.ToString();
            ((MainViewModel)DataContext).OpenFileWith(path);
            
        }
        #endregion

        # region input parameters event handlers
        private void LoadCIBRunShellDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                ((MainViewModel) DataContext).LaunchParameters.CIBRunShellDirectory = folderDialog.SelectedPath;
        }

        private void LoadOutputDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK") 
              ((MainViewModel)DataContext).LaunchParameters.OutputDirectory = folderDialog.SelectedPath;
         }
        #endregion

        #region Result event handlers
        private void OutputTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((((MainViewModel)DataContext).SelectedResult is ResultViewModel) && (((MainViewModel)DataContext)!=null))
            ((MainViewModel)DataContext).OpenRF();
        }
        #endregion

        #region Import results to CSV event hanslers
        private void ImportExel_Click(object sender, RoutedEventArgs e)
        {
            if (((MainViewModel) DataContext).ResultsList.Count > 0)
            {
                string path = Path.Combine(((MainViewModel)DataContext).LaunchParameters.OutputDirectory, "result_table" + DateTime.Now.ToString("_MM_dd_HH_mm_ss") + ".csv");
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = Path.GetFileNameWithoutExtension(path); // Default file name
                dlg.DefaultExt = ".csv"; // Default file extension
                dlg.Filter = "CSV documents|*.csv"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> res = dlg.ShowDialog();

                // Process save file dialog box results
                if (res == true)
                {
                    // Save document
                    path = dlg.FileName;
                }




                OutputTable.SelectAllCells();
                OutputTable.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, OutputTable);
                OutputTable.UnselectAllCells();
                //string path = Path.Combine(((MainViewModel) DataContext).LaunchParameters.OutputDirectory , "result_table"+DateTime.Now.ToString("_MM_dd_HH_mm_ss")+".csv");
                string result = (string) Clipboard.GetData(DataFormats.CommaSeparatedValue);
                while (result.Contains(","))
                    result=result.Replace(",", ";");
                Clipboard.Clear();
                System.IO.StreamWriter file = new System.IO.StreamWriter(path);
                file.WriteLine(result);
                file.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Results list is empty");
            }
        } 
        #endregion

        #endregion

        #region Values saves between sessions
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).LaunchParameters.CIBRunShellDirectory = Properties.Settings.Default.CIBRunShellDirectoryText;
            ((MainViewModel)DataContext).LaunchParameters.OutputDirectory = Properties.Settings.Default.OutputDirectoryText;
            ((MainViewModel)DataContext).LaunchParameters.Bit32 = Properties.Settings.Default.Bit32Checked;
            ((MainViewModel)DataContext).LaunchParameters.Bit64 = Properties.Settings.Default.Bit64Checked;
            ((MainViewModel)DataContext).LaunchParameters.MemoryLimit = Properties.Settings.Default.MemoryLimitText;
            ((MainViewModel)DataContext).LaunchParameters.TimeLimit = Properties.Settings.Default.TimeLimitText;
            ((MainViewModel)DataContext).ProcessCount = Properties.Settings.Default.ProcessCountChecked;
            ((MainViewModel) DataContext).LastFolderForInputFilesList =Properties.Settings.Default.LastFolderForInputFilesListText;
            ((MainViewModel) DataContext).LastFolderForScriptsList =Properties.Settings.Default.LastFolderForScriptsListText;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.CIBRunShellDirectoryText = CIBRunShellDirectory.Text;
            Properties.Settings.Default.OutputDirectoryText = OutputDirectory.Text;
            if (_32BitCheckBox.IsChecked == true)
                Properties.Settings.Default.Bit32Checked = true;
            else
                Properties.Settings.Default.Bit32Checked = false;
            if (_64BitCheckBox.IsChecked == true)
                Properties.Settings.Default.Bit64Checked = true;
            else
                Properties.Settings.Default.Bit64Checked = false;
            Properties.Settings.Default.MemoryLimitText = ((MainViewModel) DataContext).LaunchParameters.MemoryLimit;
            Properties.Settings.Default.TimeLimitText = ((MainViewModel) DataContext).LaunchParameters.TimeLimit;
            Properties.Settings.Default.ProcessCountChecked = ((MainViewModel) DataContext).ProcessCount;
            Properties.Settings.Default.LastFolderForInputFilesListText =((MainViewModel) DataContext).LastFolderForInputFilesList;
            Properties.Settings.Default.LastFolderForScriptsListText =((MainViewModel) DataContext).LastFolderForScriptsList;
            Properties.Settings.Default.Save();    
        }

        #endregion

       
    }
}
