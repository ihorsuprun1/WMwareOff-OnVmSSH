using System;
using System.Collections.Generic;
using System.Text;

namespace WMwareOfVm
{
    class InfoVM
    {

        public string VmId { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Guest { get; set; }
        public string OS { get; set; }
        public string Version { get; set; }
        public string Anotation { get; set; }
        public StatePower Power { get; set; }

        public enum StatePower 
        {
            unknow,
            off,
            on
        }

        public override string ToString()
        {
            return $"VMId : {VmId}  Name : {Name} Power :  {Power} ";
        }
    }
}
