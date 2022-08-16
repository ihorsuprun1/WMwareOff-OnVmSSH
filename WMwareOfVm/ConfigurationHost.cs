using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WMwareOfVm
{
    class ConfigurationHost
    {
        public async Task<Hosts> GetTokenAsync()
        {
            //чтение данных
            using (FileStream fs = new FileStream("Host.json", FileMode.OpenOrCreate))
            {
                Hosts hosts = await JsonSerializer.DeserializeAsync<Hosts>(fs);
                foreach (var t in hosts.HostList)
                {
                    Console.WriteLine(t.IP);
                    Console.WriteLine(t.Login);
                }
                return hosts;
                // Console.WriteLine($"Name: {token.Token}  ");
            }
        }
    }

    class Hosts
    {
        public List<Host> HostList { get; set; }
    }
    class Host
    {
        public string IP { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

}
