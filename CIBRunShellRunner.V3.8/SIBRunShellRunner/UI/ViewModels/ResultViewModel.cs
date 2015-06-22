using System;
using CIBRunShellRunner.Models;

namespace CIBRunShellRunner.ViewModels
{
    class ResultViewModel:ViewModelBase
    {
        private readonly Result Result;

        public ResultViewModel(Result result)
        {
            Result = result;
        }

        public string InputFile
        {
            get { return Result.InputFile; }
            set
            {
                Result.InputFile = value;
                OnPropertyChanged("InputFile");
            }
        }

        public string Script
        {
            get { return Result.Script; }
            set
            {
                Result.Script = value;
                OnPropertyChanged("Script");
            }
        }

        public string OutputDirectory
        {
            get { return Result.OutputDirectory; }
            set
            {
                Result.OutputDirectory = value;
                OnPropertyChanged("OutputDirectory");
            }
        }

        public StatusEnum Status
        {
            get { return Result.Status; }
            set
            {
                Result.Status = value;
                OnPropertyChanged("Status");
            }
        }

        public Int64 Memory
        {
            get{return Result.Memory;}
            set
            {
                Result.Memory = value;
                OnPropertyChanged("Memory");
            }
        }

        public TimeSpan Time
        {
            get{return Result.Time;}
            set
            {
                Result.Time = value;
                OnPropertyChanged("Time");
            }
        }

        public BitEnum Bit
        {
            get { return Result.Bit; }
            set
            {
                Result.Bit = value;
                OnPropertyChanged("Bit");
            }
        }

        public string Error
        {
            get { return Result.Error; }
            set
            {
                Result.Error = value;
                OnPropertyChanged("Error");
            }
        }
    }
}
