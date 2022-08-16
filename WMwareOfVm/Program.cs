using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace WMwareOfVm
{
    class Program
    {
       
        static void Main(string[] args)
        {
            //ClientSSH ssh = new ClientSSH();

            // var phrase = ssh.ssh("10.8.0.184", "root", "crashtest123", "vim-cmd vmsvc/getallvms");

            // List<InfoVM> listVm =   ssh.GetListVmPowerStatus(ssh.GetParseListVm(phrase), "10.8.0.184", "root", "crashtest123");

            // foreach (var item in listVm)
            // {
            //     Console.WriteLine(item);

            // }

            // listVm.Add(new InfoVM { VmId = "123", Name = "win", Power = InfoVM.StatePower.off });
            // listVm.Add(new InfoVM { VmId = "123", Name = "win", Power = InfoVM.StatePower.off });
            // listVm.Add(new InfoVM { VmId = "123", Name = "win", Power = InfoVM.StatePower.off });
            // listVm.Add(new InfoVM { VmId = "123", Name = "win", Power = InfoVM.StatePower.off });
            // listVm.Add(new InfoVM { VmId = "123", Name = "win", Power = InfoVM.StatePower.off });
            // listVm.Add(new InfoVM { VmId = "123", Name = "win", Power = InfoVM.StatePower.off });
            // listVm.Add(new InfoVM { VmId = "123", Name = "win", Power = InfoVM.StatePower.off });

            // listVm = listVm.Where(c => c.Power == InfoVM.StatePower.off).ToList();
            // Console.WriteLine("***************************");
            // Console.WriteLine(listVm.Count);

            // int resultCount = 0;
            // int x = listVm.Count % 4;
            // if (x > 0) { resultCount = listVm.Count / 4 + 1;}

            // List<List<InlineKeyboardButton>> rowListButton = new List<List<InlineKeyboardButton>>();

            // for (int i = 0; i < resultCount; i++)
            // {
            //         List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            //         foreach (var item in listVm.Take(4))
            //         {
            //             buttons.Add(new InlineKeyboardButton(item.Name) { Text = item.Name, CallbackData = item.VmId });
            //             listVm.Remove(item);

            //         }
            //         rowListButton.Add(buttons);
            // }

            //var p = phrase.ToArray();

            //foreach (var g in p)
            //{

            //    if(g.ToString() == "\n".ToString())
            //    {
            //        g == ';';
            //    }

            //}



            //string[] subs = phrase.Split(' ');
            //Console.WriteLine("Hello World!");
            // ClientSSH clientSSH = new ClientSSH();
            // //  clientSSH.SshStatus("10.8.0.184", "root", "crashtest123", "vim-cmd vmsvc/power.on 1");
            //var phrase =  clientSSH.SshStatus("10.8.0.184", "root", "crashtest123", "vim-cmd vmsvc/getallvms");
            //while (phrase.Contains("  ")) { phrase = phrase.Replace("  ", " "); }
            // //  phrase.Replace("\n", ";");
            // string[] subs = phrase.Split(' ');


            //foreach (var g in subs)
            //{

            //   Console.WriteLine(g);

            //}
            //Console.WriteLine("***************************");
            //Console.ForegroundColor = ConsoleColor.Magenta;

            //Console.WriteLine(phrase);
            //Console.WriteLine("***************************");
            ////Console.ResetColor();
            //Console.WriteLine("***************************");
            //string[] words = phrase.Split(' ');

            //foreach (var word in words)
            //{
            //    System.Console.WriteLine($"<{word}>");
            //}

            //res.Split

            //TelegramSample telegram = new TelegramSample();
            //telegram.StartBot();
            //   telegram.Send();
            // clientSSH.SshStatus("10.8.0.184", "root", "crashtest123", "vim-cmd vmsvc/power.getstate");

            //Console.WriteLine(ConfigurationTelegram.BotToken);

            //ConfigurationVM vM = new ConfigurationVM();
            //foreach (var t in vM.GetTokenAsync().GetAwaiter().GetResult().VmList)
            //{
            //    Console.WriteLine(t.id);
            //}

            //int[] numbers = { -3, -2, 3 };
            //var result = numbers.Take(5);

            //foreach (int i in result)
            //    Console.WriteLine(i);
            Telegram telegram = new Telegram();
            telegram.StartBotAsync();
            //ClientSSH ssh = new ClientSSH();

            //  ssh.GetListVmPowerStatusQ("10.8.0.184", "root", "crashtest123");

            Console.ReadLine();
        }  

       
           
    }
}

