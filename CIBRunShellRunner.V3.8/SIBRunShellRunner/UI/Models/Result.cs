using System;

namespace CIBRunShellRunner.Models
{
    enum StatusEnum { Waiting, Running, Completed, Failed, KilledByMemory, KilledByTime, Aborted }
    enum BitEnum {Bit32,Bit64};
    class Result
    {
        public string InputFile { get; set; }
        public string Script { get; set; }
        public string OutputDirectory { get; set; }
        public StatusEnum Status { get; set; }
        public Int64 Memory { get; set; }
        public TimeSpan Time { get; set; }
        public BitEnum Bit { get; set; }
        public string Error { get; set; }
        public Result(string inputFile, string script, string outputDirectory, BitEnum bit)
        {
            InputFile = inputFile;
            Script = script;
            OutputDirectory = outputDirectory;
            Status = StatusEnum.Waiting;
            Memory = 0;
            Time = new TimeSpan();
            Bit = bit;
            Error = "";
        }
    }
}
