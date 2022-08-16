using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WMwareOfVm
{
     class ConfigurationTelegram
     {
        public static readonly string BotToken = GetTokenAsync().GetAwaiter().GetResult().Token;//"2019720083:AAH7nRLP0Ws3AVUjSr_fDBCUtlSLV6v4rZQ";
     
        public static async Task<TokenBot> GetTokenAsync()
        {
            //чтение данных
            using (FileStream fs = new FileStream("Token.json", FileMode.OpenOrCreate))
            {
                TokenBot token = await JsonSerializer.DeserializeAsync<TokenBot>(fs);
                return token;
               // Console.WriteLine($"Name: {token.Token}  ");
            }
        }
       
    }
   class TokenBot
    {
       public string Token { get; set; }
    } 
}
