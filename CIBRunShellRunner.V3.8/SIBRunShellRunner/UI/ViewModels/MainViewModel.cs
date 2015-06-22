using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CIBRunShellRunner.Commands;
using CIBRunShellRunner.Models;
using System.IO;
using MessageBox = System.Windows.MessageBox;

namespace CIBRunShellRunner.ViewModels
{
    class MainViewModel : ViewModelBase, IDataErrorInfo
    {

        #region Fields
        private int _processCount;
        private bool _isEnabledAllExceptStopFlag;
        private int _processedRecordsCount;
        private int _progressBarFilledInPercent;
        private TimeSpan _timePassed;
        private BackgroundWorker _bgWorker;
        private BackgroundWorker _statusBarWorker;
        private ResultViewModel _selectedResult;
        private bool _inputFileCountFlag;
        private bool _isEnabledStartButton;
        private bool _inputFilesAndScriptsListsNotEmptyFlagForBinding;
        private string _lastFolderForInputFilesList;
        private string _lastFolderForScriptsList;
        #endregion

        #region Properties
        public ObservableCollection<string> InputFilesList { get; private set; }
        
        public ObservableCollection<string> ScriptFilesList { get; private set; }
        public ObservableCollection<ResultViewModel> ResultsList { get; private set; }
        public LaunchParametersViewModel LaunchParameters { get; private set; }

        public void setInputFilesAndScriptsListsNotEmptyFlagOnCollectionChange(object source, EventArgs e)
        {
            if ((InputFilesList.Count != 0) && (ScriptFilesList.Count != 0))
                InputFilesAndScriptsListsNotEmptyFlagForBinding = true;
            else
                InputFilesAndScriptsListsNotEmptyFlagForBinding = false;
            OnPropertyChanged("InputFilesAndScriptsListsNotEmptyFlagForBinding");
        }

        public int ProcessCount
        {
            get { return _processCount; }
            set
            {
                _processCount = value;
                OnPropertyChanged("ProcessCount");
            }
        }
        public bool IsEnabledAllExceptStopFlag
        {
            get { return _isEnabledAllExceptStopFlag; }
            set
            {
                _isEnabledAllExceptStopFlag = value;
                OnPropertyChanged("IsEnabledAllExceptStopFlag");
            }
        }

        public int ProcessedRecordsCount
        {
            get { return _processedRecordsCount; }
            set
            {
                _processedRecordsCount = value;
                OnPropertyChanged("ProcessedRecordsCount");
            }
        }
        public int ProgressBarFilledInPercent
        {
            get { return _progressBarFilledInPercent; }
            set
            {
                _progressBarFilledInPercent = value;
                OnPropertyChanged("ProgressBarFilledInPercent");
            }
        }
        public TimeSpan TimePassed
        {
            get { return _timePassed; }
            set
            {
                _timePassed = value;
                OnPropertyChanged("TimePassed");
            }
        }

        public ResultViewModel SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                _selectedResult = value; 
                OnPropertyChanged("SelectedResult");
            }
        }

        public bool InputFileCountFlag
        {
            get { return _inputFileCountFlag; }
            set
            {
                _inputFileCountFlag = value;
                OnPropertyChanged("InputFileCountFlag");
            }
        }

        public bool InputFilesAndScriptsListsNotEmptyFlagForBinding
        {
            get { return _inputFilesAndScriptsListsNotEmptyFlagForBinding; }
            set
            {
                if ((InputFilesList.Count == 0) || (ScriptFilesList.Count == 0))
                    _inputFilesAndScriptsListsNotEmptyFlagForBinding = false;
                else
                    _inputFilesAndScriptsListsNotEmptyFlagForBinding = true;
                OnPropertyChanged("InputFilesAndScriptsListsNotEmptyFlagForBinding");
                OnPropertyChanged("IsEnabledStartButton");
            }
        }

        public bool IsEnabledStartButton
        {
            get
            {
                return _isEnabledStartButton && LaunchParameters.IsValidLaunchParameters && IsEnabledAllExceptStopFlag && LaunchParameters.BitIsSelectedFlag && InputFilesAndScriptsListsNotEmptyFlagForBinding;
            }
            set
            {
                _isEnabledStartButton = value;
                OnPropertyChanged("IsEnabledStartButton");
            }
        }

        public string LastFolderForInputFilesList
        {
            get { return _lastFolderForInputFilesList; }
            set
            {
                _lastFolderForInputFilesList = value;
                OnPropertyChanged("LastFolderForInputFilesList");
            }
        }

        public string LastFolderForScriptsList
        {
            get { return _lastFolderForScriptsList; }
            set
            {
                _lastFolderForScriptsList = value;
                OnPropertyChanged("LastFolderForScriptsList");
            }
        }
        #endregion

        #region constructor

        public MainViewModel( LaunchParameters launchParameters)
        {
            InputFilesList = new ObservableCollection<string>();
            ScriptFilesList = new ObservableCollection<string>();
            InputFilesList.CollectionChanged += setInputFilesAndScriptsListsNotEmptyFlagOnCollectionChange;
            ScriptFilesList.CollectionChanged += setInputFilesAndScriptsListsNotEmptyFlagOnCollectionChange;
            ResultsList = new ObservableCollection<ResultViewModel>();
            LaunchParameters = new LaunchParametersViewModel(launchParameters);
            IsEnabledAllExceptStopFlag = true;
            ProcessedRecordsCount = 0;
            ProgressBarFilledInPercent = 0;
            TimePassed = new TimeSpan();
            InputFileCountFlag = true;
            IsEnabledStartButton = true;
            InputFilesAndScriptsListsNotEmptyFlagForBinding = true;
            _lastFolderForInputFilesList = @"C:\";
            _lastFolderForScriptsList = @"C:\";
            //_lastFolderForInputFilesList = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            //_lastFolderForScriptsList = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        }
        #endregion

        #region Commands

        #region Start command
        private DelegateCommand _runCommand;
        public ICommand RunCommand
        {
            get
            {
                if (_runCommand == null)
                    _runCommand = new DelegateCommand(Run);
                return _runCommand;
            }
        }

        private void Run()
        {
            try
            {
                string filePathBit32 = Get32BitCIBRunShellPath();
                string filePathBit64 = Get64BitCIBRunShellPath();
                if (!File.Exists(filePathBit32) && (LaunchParameters.Bit32))
                    throw new FileNotFoundException("CIBRunShell was not found in specified directory");
                if (!File.Exists(filePathBit64) && (LaunchParameters.Bit64))
                    throw new FileNotFoundException("CIBRunShell was not found in specified directory");
                if ((LaunchParameters.Bit32 == false) && (LaunchParameters.Bit64 == false))
                    throw new FileNotFoundException("CIBRunShell bit is not selected");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                IsEnabledAllExceptStopFlag = true;
                IsEnabledStartButton = true;
                _statusBarWorker.CancelAsync();
                return;
            }
            IsEnabledAllExceptStopFlag = false;
            IsEnabledStartButton = false;
            try
            {
                if (String.IsNullOrEmpty(LaunchParameters.CIBRunShellDirectory))
                    throw new ArgumentException("CIBRunShell directory is empty");
                if (String.IsNullOrEmpty(LaunchParameters.OutputDirectory))
                    throw new ArgumentException("Output directory is empty");
                if (InputFilesList.Count == 0)
                    throw new ArgumentException("File List is Empty");
                if (ScriptFilesList.Count == 0)
                    throw new ArgumentException("Script List is Empty");
                if (ResultsList.Count!=0)
                    ResultsList.Clear();
                if (InputFileCountFlag)
                {
                    foreach (string filePath in InputFilesList)
                    {
                        foreach (string scriptPath in ScriptFilesList)
                        {
                            if (LaunchParameters.Bit32)
                            {
                                ResultViewModel currentResult = new ResultViewModel(new Result(filePath, scriptPath, LaunchParameters.OutputDirectory, BitEnum.Bit32));
                                ResultsList.Add(currentResult);
                            }
                            if (LaunchParameters.Bit64)
                            {
                                ResultViewModel currentResult = new ResultViewModel(new Result(filePath, scriptPath, LaunchParameters.OutputDirectory, BitEnum.Bit64));
                                ResultsList.Add(currentResult);
                            }
                        }
                    }                    
                }
                else
                {
                    foreach (string scriptPath in ScriptFilesList)
                    {
                        if (LaunchParameters.Bit32)
                        {

                            ResultViewModel currentResult = new ResultViewModel(new Result(InputFilesList[0], scriptPath, LaunchParameters.OutputDirectory, BitEnum.Bit32));
                            ResultsList.Add(currentResult);
                        }
                        if (LaunchParameters.Bit64)
                        {
                            ResultViewModel currentResult = new ResultViewModel(new Result(InputFilesList[0], scriptPath, LaunchParameters.OutputDirectory, BitEnum.Bit64));
                            ResultsList.Add(currentResult);
                        }
                    }
                }
                for (int i = 0; i < ResultsList.Count; i++)
                {
                    string extension = Path.GetExtension(ResultsList[i].Script);
                    if ((extension == ".xml") || (extension == ".ini"))
                    {
                        string argPath = Path.ChangeExtension(ResultsList[i].Script, ".arg");
                        var element = ResultsList.FirstOrDefault(e => e.Script == argPath);
                        if (element != null)
                        {
                            ResultsList.Remove(element);
                            i--;
                        }
                    }
                }
                OnPropertyChanged("ResultsList");
                _bgWorker = new BackgroundWorker();
                _bgWorker.WorkerReportsProgress = true;
                _bgWorker.DoWork += bgWorker_DoWork;
                _bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
                _bgWorker.WorkerSupportsCancellation = true;
                _bgWorker.RunWorkerAsync(ResultsList);

                _statusBarWorker = new BackgroundWorker();
                _statusBarWorker.WorkerReportsProgress = true;
                _statusBarWorker.WorkerSupportsCancellation = true;
                _statusBarWorker.DoWork += statusBarWorker_DoWork;
                _statusBarWorker.RunWorkerAsync();
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
                IsEnabledAllExceptStopFlag = true;
                IsEnabledStartButton = true;
                _statusBarWorker.CancelAsync();
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsEnabledAllExceptStopFlag = true;
            IsEnabledStartButton = true;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker)
            {
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = ProcessCount;
                Parallel.For(0, ResultsList.Count, po, (i, loopState) =>
                {
                    if (!((BackgroundWorker)sender).CancellationPending)
                    {
                        Run(ResultsList[i]);
                    }
                    else
                    {
                        loopState.Stop();
                    }
                });
            }
            else
            {
                throw new ArgumentException("Sender must be BackgroundWorker type");
            }
        }

        void statusBarWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessedRecordsCount = 0;
            ProgressBarFilledInPercent = 0;
            Stopwatch processTimer=new Stopwatch();
            processTimer.Start();
            while (ProcessedRecordsCount!=ResultsList.Count)
            {
                ProcessedRecordsCount =
                    ResultsList.Count(r => r.Status == StatusEnum.Completed) +
                    ResultsList.Count(r => r.Status == StatusEnum.Failed) +
                    ResultsList.Count(r => r.Status == StatusEnum.KilledByMemory) +
                    ResultsList.Count(r => r.Status == StatusEnum.KilledByTime);
                ProgressBarFilledInPercent = (ProcessedRecordsCount * 100 / ResultsList.Count);              
                TimePassed = processTimer.Elapsed;
                Thread.Sleep(100);
                if (sender is BackgroundWorker)
                {
                    if (((BackgroundWorker)sender ).CancellationPending)
                        break;
                }
                else
                {
                    throw new ArgumentException("Sender must be BackgroundWorker type");
                }
            }
        }

        private string Get32BitCIBRunShellPath()
        {
            return Path.Combine(LaunchParameters.CIBRunShellDirectory, "32bit", "CibRsh.exe");
        }
        private string Get64BitCIBRunShellPath()
        {
            return Path.Combine(LaunchParameters.CIBRunShellDirectory, "64bit", "CibRsh64.exe");
        }

        private void Run(ResultViewModel vm)
        {
            if (vm == null) throw new ArgumentException("ViewModell is null");
            string inputFilePath = vm.InputFile;
            string scriptPath = vm.Script;
            string script;
            try
            {
                script= File.ReadAllText(scriptPath);  
            }
            catch (Exception e)
            {
                vm.Error+=(e.Message);
                vm.Status=StatusEnum.Failed;
                return;
            }
            string filePath=vm.Bit.Equals(BitEnum.Bit32)?Get32BitCIBRunShellPath():Get64BitCIBRunShellPath();
            //Output directory path
            //string outputDirectoryPath =               
            //    LaunchParameters.OutputDirectory + @"\result_"+
            //    Path.GetFileName(vm.InputFile) + " file with script " + Path.GetFileName(vm.Script) +" "+vm.Bit+" "+
            //    String.Format(@"{0}", Guid.NewGuid());
            string outputDirectoryPath =
                Path.Combine(
                LaunchParameters.OutputDirectory,
                DateTime.Now.ToString("MM_dd_HH_mm_ss_") +
                Path.GetFileName(vm.InputFile) + "_" + Path.GetFileName(vm.Script) + "_" + (vm.Bit==BitEnum.Bit32?"x32":"x64"));
            int numFolder = 1;
            while (true)
            {
                if (Directory.Exists(outputDirectoryPath))
                {
                    if (Directory.Exists(outputDirectoryPath + "_" + numFolder))
                        numFolder++;
                    else
                    {
                        outputDirectoryPath += "_";
                        outputDirectoryPath += numFolder;
                        try
                        {
                            vm.OutputDirectory = outputDirectoryPath;
                            Directory.CreateDirectory(outputDirectoryPath);
                            Directory.CreateDirectory(outputDirectoryPath + @"\Output");
                            Directory.CreateDirectory(outputDirectoryPath + @"\Input");
                            break;
                        }
                        catch (IOException e)
                        {
                        }
                    }
                }
                else
                {
                    try
                    {
                        vm.OutputDirectory = outputDirectoryPath;
                        Directory.CreateDirectory(outputDirectoryPath);
                        Directory.CreateDirectory(outputDirectoryPath + @"\Output");
                        Directory.CreateDirectory(outputDirectoryPath + @"\Input");
                        break;
                    }
                    catch (IOException e)
                    {
                    }
                }
            }
            //vm.OutputDirectory = outputDirectoryPath;
            //Directory.CreateDirectory(outputDirectoryPath);
            //Directory.CreateDirectory(outputDirectoryPath + @"\Output");
            //Directory.CreateDirectory(outputDirectoryPath + @"\Input");
            string replaceCfgPath = AppDomain.CurrentDomain.BaseDirectory;
            string inputFiles = "";
            if (!InputFileCountFlag)
            {
                for (int i = 0; i < InputFilesList.Count - 1; i++)
                {
                    inputFiles += ("\"" + InputFilesList[i] + "\"" + ";");
                }
                inputFiles += ("\"" + InputFilesList[InputFilesList.Count - 1] + "\"");
            }
            //if script xml or ini
            if (Path.GetExtension(scriptPath).ToLower().Equals(".xml".ToLower()) || (Path.GetExtension(scriptPath).ToLower().Equals(".ini".ToLower())))
            {
                var argFilePath = Path.ChangeExtension(scriptPath, ".arg");
                if (!File.Exists(argFilePath))
                {
                    try
                    {
                        if (Path.GetExtension(scriptPath).ToLower().Equals(".xml".ToLower()))
                        {
                            argFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Default",
                                "defaultXml.arg");
                            if (!File.Exists(argFilePath))
                                throw new ArgumentException("Default arg file  for xml defaultXml.arg was not found");
                        }
                        if (Path.GetExtension(scriptPath).ToLower().Equals(".ini".ToLower()))
                        {
                            argFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Default",
                                "defaultIni.arg");
                            if (!File.Exists(argFilePath))
                                throw new ArgumentException("Default arg file for ini defaultIni.arg was not found");
                        }
                    }
                    catch (ArgumentException e)
                    {
                        vm.Error += (e.Message);
                        vm.Status = StatusEnum.Failed;
                        return;
                    }
                }
                string[] xmlDocByStrings = File.ReadAllLines(scriptPath);
                for (int i = 0; i < xmlDocByStrings.Length; i++)
                {
                    xmlDocByStrings[i] = xmlDocByStrings[i].Replace("$InputFile$", inputFilePath);
                    xmlDocByStrings[i] = xmlDocByStrings[i].Replace("$OutDir$", outputDirectoryPath + @"\Output");
                    if (File.Exists(replaceCfgPath))
                    {
                        string[] replaceCfg = File.ReadAllLines(replaceCfgPath);
                        foreach (string paramStr in replaceCfg)
                        {
                            string value = paramStr.Substring(paramStr.IndexOf('='));
                            string key = paramStr.Substring(1, (paramStr.Length - paramStr.IndexOf('=') - 1));
                            xmlDocByStrings[i] = xmlDocByStrings[i].Replace(key, value);
                        }                        
                    }
                    if (!InputFileCountFlag)
                        script = script.Replace("\"$InputFiles$\"", inputFiles);                   
                }
                String SP_xmlFilePath = outputDirectoryPath + @"\Input\" + "SP_" + Path.GetFileName(scriptPath);
                    string xmlText = "";
                    for (int j = 0; j < xmlDocByStrings.Length; j++)
                    {
                        xmlText += xmlDocByStrings[j];
                    }
                    using (FileStream fs = File.Create(SP_xmlFilePath))
                    {   
                        Byte[] info = new UTF8Encoding(true).GetBytes(xmlText);
                        fs.Write(info, 0, info.Length);
                    }
                    script = File.ReadAllText(argFilePath);
                    script = script.Replace("$XmlPath$", SP_xmlFilePath);
                    script = script.Replace("$IniPath$", SP_xmlFilePath);
            }

            //for any script
            while (script.Contains(Environment.NewLine))
                script = script.Replace(Environment.NewLine,String.Empty);
            script = script.Replace("$InputFile$", inputFilePath);
            script = script.Replace("$OutDir$", outputDirectoryPath + @"\Output");
            if (!InputFileCountFlag)
                script = script.Replace("\"$InputFiles$\"", inputFiles);
            if (File.Exists(replaceCfgPath))
            {
                string[] replaceCfg = File.ReadAllLines(replaceCfgPath);
                foreach (string paramStr in replaceCfg)
                {
                    string value = paramStr.Substring(paramStr.IndexOf('='));
                    string key=paramStr.Substring(1,(paramStr.Length-paramStr.IndexOf('=')-1));
                    script=script.Replace(key, value);
                }
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(filePath);
                vm.Status = StatusEnum.Running;
                startInfo.Arguments = script;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.StandardErrorEncoding = Encoding.ASCII;
                string workinfDirectory = Path.Combine(LaunchParameters.CIBRunShellDirectory, (vm.Bit == BitEnum.Bit32 ? "32bit" : "64bit"));
                startInfo.WorkingDirectory = workinfDirectory;
                var p = new Process();
                p.StartInfo = startInfo;
                p.OutputDataReceived += OnOutputReceived;
                p.ErrorDataReceived += OnOutputReceived;
                p.Start();
                Stopwatch timer = Stopwatch.StartNew();
                vm.Memory = 0;
                while (true)
                {
                    try
                    {
                        if (p.HasExited)
                        {
                            vm.Status = StatusEnum.Completed;
                            p.WaitForExit();
                            break;
                        }
                        p.Refresh();
                        if (IsEnabledStartButton)
                        {
                            p.Kill();
                            vm.Status = StatusEnum.Aborted;
                            break;
                        }
                        vm.Time = timer.Elapsed;
                        if (p.HasExited == false)
                        {
                            if (vm.Memory < p.PrivateMemorySize64)
                                vm.Memory = p.PrivateMemorySize64;
                        }
                        if (vm.Memory > (LaunchParameters.MemoryLimit*1024*1024))
                        {
                            p.Kill();
                            vm.Status = StatusEnum.KilledByMemory;
                            break;
                        }
                        if (timer.ElapsedMilliseconds > (LaunchParameters.TimeLimit*1000*60))
                        {
                            p.Kill();
                            vm.Status = StatusEnum.KilledByTime;
                            break;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                    }
                }
                    p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    using (FileStream fs = File.Create(outputDirectoryPath + @"\Output\" + "ErrorMessage.txt"))
                    {
                        Byte[] info;
                        if (vm.Status != StatusEnum.Aborted)
                        {
                            string errorMessage = p.StandardError.ReadToEnd();
                            while (errorMessage.Contains(Environment.NewLine))
                            {
                                errorMessage = errorMessage.Replace(Environment.NewLine, String.Empty);
                            }
                            info = new UTF8Encoding(true).GetBytes(errorMessage);
                            vm.Error += errorMessage;
                        }
                        else
                        {
                            info = new UTF8Encoding(true).GetBytes("Process was aborted");
                            vm.Error += "Process was aborted. ";
                        }
                        fs.Write(info, 0, info.Length);
                    }
                    if ((vm.Status != StatusEnum.KilledByMemory) && (vm.Status != StatusEnum.KilledByTime) && (vm.Status != StatusEnum.Aborted))
                        vm.Status = StatusEnum.Failed;
                }
                using (FileStream fs = File.Create(Path.ChangeExtension(outputDirectoryPath + @"\Input\" + "SP_" + Path.GetFileName(scriptPath),".arg" )))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(script);
                    fs.Write(info, 0, info.Length);
                }
                using (FileStream fs = File.Create(outputDirectoryPath + @"\Input\" + "Info.txt"))
                {
                    string textInfo = "";
                    if (InputFileCountFlag)
                    {
                        textInfo = "File name:" + inputFilePath + Environment.NewLine +
                                   "Capacity: " + vm.Bit;
                    }
                    else
                    {
                        foreach (string path in InputFilesList)
                        {
                            textInfo+=("File name:" + path + Environment.NewLine);
                        }
                        textInfo += "Capacity: " + vm.Bit;
                    }
                    Byte[] info =new UTF8Encoding(true).GetBytes(textInfo);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception e)
            {
                vm.Error += (e.Message);
                vm.Status=StatusEnum.Failed;
                //throw;
            }
            if (vm.Error != "")
            {
                if (!File.Exists(outputDirectoryPath + @"\Output\" + "ErrorMessage.txt"))
                {
                    using (FileStream fs = File.Create(outputDirectoryPath + @"\Output\" + "ErrorMessage.txt"))
                    {
                        Byte[] info;
                        info = new UTF8Encoding(true).GetBytes(Error);
                        fs.Write(info, 0, info.Length);
                    }
                }
            }

        }

        private void OnOutputReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
        # endregion

        #region Stop command
        private DelegateCommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new DelegateCommand(Cancel);
                return _cancelCommand;
            }
        }

        public void Cancel()
        {
            _bgWorker.CancelAsync();
            _statusBarWorker.CancelAsync();
            IsEnabledAllExceptStopFlag = true;
            IsEnabledStartButton = true;
        }
        #endregion

        #region Import to CSV command
        private DelegateCommand _importToCSV;
        public ICommand ImportToCSVCommand
        {
            get
            {
                if (_importToCSV == null)
                    _importToCSV = new DelegateCommand(ImportToCSV);
                return _importToCSV;
            }
        }

        public void ImportToCSV()
        {
            
        }
        #endregion

        #endregion

        #region IDataErrorInfo members
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "InputFilesAndScriptsListsNotEmptyFlagForBinding")
                {
                    if (InputFilesList.Count == 0)
                        result += "Input files list is empty. ";
                    if (ScriptFilesList.Count == 0)
                        result += "Scripts list is empty";
                }
                //if (result != null)
                //    InputFilesAndScriptsListsNotEmptyFlag = false;
                //else
                //    InputFilesAndScriptsListsNotEmptyFlag = true;
                return result;
            }
        }
        #endregion

        #region Methods for event handlers

        #region Open result folder
        public void OpenRF()
        {
            try
            {
                Process.Start(SelectedResult.OutputDirectory);
            }
            catch (Exception)
            {
                MessageBox.Show("Output directory not found");
            }
        }
        #endregion

        #region Open file
        internal void OpenFile(string path)
        {
            Process.Start(path);
        }
        #endregion

        #region Open file with
        internal void OpenFileWith(string path)
        {
            Process.Start("rundll32.exe", "shell32.dll, OpenAs_RunDLL " + path);
        }
        #endregion

        #endregion
    }
}
