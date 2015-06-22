using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CIBRunShellRunner.Models;

namespace CIBRunShellRunner.ViewModels
{
    class LaunchParametersViewModel:ViewModelBase, IDataErrorInfo
    {
        private LaunchParameters LaunchParameters;
        private ObservableCollection<int> processCountLimit;

        public LaunchParametersViewModel(LaunchParameters launchParameters)
        {
            LaunchParameters = launchParameters;
            processCountLimit=new ObservableCollection<int>(LaunchParameters.ProcessCountLimitList.Select(p=>p));
            BitIsSelectedFlag = true;
        }

        public string CIBRunShellDirectory
        {
            get { return LaunchParameters.CIBRunShellDirectory; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ApplicationException("Fild is required");
                }
                LaunchParameters.CIBRunShellDirectory = value;
                OnPropertyChanged("CIBRunShellDirectory");
            } 
        }

        public string OutputDirectory { 
            get { return LaunchParameters.OutputDirectory; }
            set
            {
                LaunchParameters.OutputDirectory = value;
                OnPropertyChanged("OutputDirectory");
            } 
        }

        public int MemoryLimit
        {
            get { return LaunchParameters.MemoryLimit; }
            set
            {
                LaunchParameters.MemoryLimit = value;
                OnPropertyChanged("MemoryLimit");
            }
        }

        public int TimeLimit
        {
            get { return LaunchParameters.TimeLimit; }
            set
            {
                LaunchParameters.TimeLimit = value;
                OnPropertyChanged("TimeLimit");
            }
        }
        
        public ObservableCollection<int> ProcessCountLimitList
        {
            get { return processCountLimit; }
            set { processCountLimit = value; }
        }

        public bool Bit32
        {
            get { return LaunchParameters.Bit32; }
            set
            {
                LaunchParameters.Bit32=value;
                if (value || LaunchParameters.Bit64)
                    BitIsSelectedFlag = true;
                else
                    BitIsSelectedFlag = false;
                OnPropertyChanged("Bit32");
            }
        }
        public bool Bit64
        {
            get { return LaunchParameters.Bit64; }
            set
            {
                LaunchParameters.Bit64 = value;
                if (value || LaunchParameters.Bit32)
                    BitIsSelectedFlag = true;
                else
                    BitIsSelectedFlag = false;
                OnPropertyChanged("Bit64");
            }
        }

        public bool BitIsSelectedFlag
        {
            get { return LaunchParameters.BitIsSelectedFlag; }
            set
            {
                LaunchParameters.BitIsSelectedFlag = value;
                OnPropertyChanged("BitIsSelectedFlag");
            }
        }

        private bool _isValidLaunchParameters;

        public bool IsValidLaunchParameters
        {
            get { return _isValidLaunchParameters; }
            set
            {
                _isValidLaunchParameters = value;
                OnPropertyChanged("IsValidLaunchParameters");
            }
        }

        #region IDataErrorInfo memders
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result =null;
                if (columnName == "CIBRunShellDirectory")
                {
                    if (string.IsNullOrEmpty(CIBRunShellDirectory))
                        result += "CIBRunShellDirectory is required. ";
                    if (!File.Exists(Path.Combine(CIBRunShellDirectory, "32bit", "CibRsh.exe")))
                        result +="CIBRunShell 32 bit was not found on current directory. ";
                    if (!File.Exists(Path.Combine(CIBRunShellDirectory, "64bit", "CibRsh64.exe")))
                        result += "CIBRunShell 64 bit was not found on current directory. ";
                    
                }
                if (columnName == "OutputDirectory")
                {
                    if (string.IsNullOrEmpty(OutputDirectory))
                        result = "Output directory is required";
                }
                if (columnName == "BitIsSelectedFlag")
                {
                    if (!BitIsSelectedFlag)
                        result = "CIBRunshell Bit is not selected";
                }
                if (result != null)
                    IsValidLaunchParameters = false;
                else
                    IsValidLaunchParameters = true;
                return result;
            }
        }
        #endregion
    }
}
