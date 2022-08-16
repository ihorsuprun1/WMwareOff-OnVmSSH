using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WMwareOfVm
{
    class ConfigurationUserAcces
    {
        public async Task<AccesListUsers> GetTokenAsync()
        {
            //чтение данных
            using (FileStream fs = new FileStream("UsersAcces.json", FileMode.OpenOrCreate))
            {
                AccesListUsers usersAcces = await JsonSerializer.DeserializeAsync<AccesListUsers>(fs);
                foreach (var t in usersAcces.AccesUsers)
                {
                    Console.WriteLine(t.id);
                    Console.WriteLine(t.name);
                }
                return usersAcces;
               
            }
        }
    }

    class AccesListUsers
    {
        public List<AccesUser> AccesUsers { get; set; }
    }
    class AccesUser
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
