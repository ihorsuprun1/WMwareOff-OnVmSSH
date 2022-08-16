using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WMwareOfVm
{
    class ConfigurationVM
    {
       
        public async Task<Vms> GetTokenAsync()
        {
            //чтение данных
            using (FileStream fs = new FileStream("VM.json", FileMode.OpenOrCreate))
            {
                Vms vms = await JsonSerializer.DeserializeAsync<Vms>(fs);
                foreach (var t in vms.VmList)
                {
                    Console.WriteLine(t.id);
                    Console.WriteLine(t.name);
                }
                return vms;
                // Console.WriteLine($"Name: {token.Token}  ");
            }
        }
    }

    class Vms
    {
       public List<Vm> VmList { get; set; }
    }
    class Vm
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
