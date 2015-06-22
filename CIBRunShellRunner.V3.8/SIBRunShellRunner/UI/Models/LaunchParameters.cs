using System.Collections.Generic;

namespace CIBRunShellRunner.Models
{
    internal class LaunchParameters
    {       
        public string CIBRunShellDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public int MemoryLimit { get; set; }
        public int TimeLimit { get; set; }
        public List<int> ProcessCountLimitList { get; set; }
        public bool Bit32 { get; set; }
        public bool Bit64 { get; set; }
        public bool BitIsSelectedFlag { get; set; }
    }

}
