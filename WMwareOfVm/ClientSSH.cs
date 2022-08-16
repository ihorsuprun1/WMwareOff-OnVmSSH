using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WMwareOfVm
{
    class ClientSSH
    {

        public string ssh(string address, string login, string password, string command )
        {
            string result = null;

            using (SshClient ssh = new SshClient(address, login, password))
            {
                ssh.Connect();
                var sshresult = ssh.RunCommand(command);
                result = sshresult.Result;
                ssh.Disconnect();
            }
            return result;
        }
        public bool PowerOffOnVm(string address, string login, string password, InfoVM.StatePower power, string idVm)
        {
            bool result = false;

            using (SshClient ssh = new SshClient(address, login, password))
            {
                ssh.Connect();
                if( power == InfoVM.StatePower.on)
                {
                    var sshresult = ssh.RunCommand($"vim-cmd vmsvc/power.on {idVm}");
                    var res = ssh.RunCommand($"vim-cmd vmsvc/power.getstate {idVm}").Result;
                    if (res.Contains("Powered on"))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    var sshresult = ssh.RunCommand($"vim-cmd vmsvc/power.off {idVm}");
                    var res = ssh.RunCommand($"vim-cmd vmsvc/power.getstate {idVm}").Result;
                    if (res.Contains("Powered off"))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                } ssh.Disconnect();
            }
            return result;
        }
        public List<InfoVM> GetParseListVm(string input)
        {
            List<InfoVM> listVm = new List<InfoVM>();
            while (input.Contains("  ")) { input = input.Replace("  ", " ").Replace('\n', ';'); }
            var lineString = input.Split(';');
            lineString = lineString.Skip(1).ToArray();
            foreach (var lineSplit in lineString)
            {
                var word = lineSplit.Split(' ');
                if (word.Length == 7)
                {
                    InfoVM infoVM = new InfoVM { VmId = word[0], Name = word[1], File = word[2], Guest = word[3], OS = word[4], Version = word[5], Anotation = word[6] };
                    listVm.Add(infoVM);
                }
                else if (word.Length >= 8)
                {
                    InfoVM infoVM = new InfoVM { VmId = word[0], Name = word[1] + " "+ word[2] + word[3], File = word[3], Guest = word[4], OS = word[5], Version = word[6], Anotation = word[7] };
                    listVm.Add(infoVM);
                }
            }
            return listVm;
        }
        public List<InfoVM> GetListVmPowerStatus (List<InfoVM> listVm, string address, string login, string password)
        {
            foreach (var item in listVm)
            {
                var res = ssh(address, login, password, $"vim-cmd vmsvc/power.getstate {item.VmId}");

                if (res.Contains("Powered on"))
                {
                    item.Power = InfoVM.StatePower.on;
                }
                else if (res.Contains("Powered off"))
                {
                    item.Power = InfoVM.StatePower.off;
                }
                else
                {
                    item.Power = InfoVM.StatePower.unknow;
                }
            }
            return listVm;
        }
        //public List<InfoVM> GetListVmPowerStatus(string address, string login, string password)
        //{
        //    List<InfoVM> listVm = new List<InfoVM>();
        //    using (SshClient ssh = new SshClient(address, login, password))
        //    {
        //        ssh.Connect();
        //        var sshresult = ssh.RunCommand("vim-cmd vmsvc/getallvms | sed -e '1d' -e 's/ \\[.*$//' | awk '$1 ~ /^[0-9]+$/ {print $1\":\"substr($0,8,80)}'");
        //        var input = sshresult.Result;
        //        input = input.Replace('\n', ';');
        //        var lineString = input.Split(';');
        //        foreach (var lineSplit in lineString)
        //        {
        //            var word = lineSplit.Split(':');

        //            if (word.Length == 2)
        //            {
        //                var item = InfoVM.StatePower.unknow;
        //                var sshPower = ssh.RunCommand($"vim-cmd vmsvc/power.getstate {word[0]}");
        //                var resPower = sshPower.Result;
        //                if (resPower.Contains("Powered on"))
        //                {
        //                    item = InfoVM.StatePower.on;
        //                }
        //                else if (resPower.Contains("Powered off"))
        //                {
        //                    item = InfoVM.StatePower.off;
        //                }
        //                else
        //                {
        //                    item = InfoVM.StatePower.unknow;
        //                }
        //                InfoVM infoVM = new InfoVM { VmId = word[0], Name = word[1], Power = item };
        //                listVm.Add(infoVM);
        //            }
        //        }
        //        ssh.Disconnect();
        //    }
        //    return listVm;
        //}
        public List<InfoVM> GetListVmPowerStatus(List<Host> hosts)
        {
            List<InfoVM> listVm = new List<InfoVM>();
            foreach(var host in hosts)
            {
                using (SshClient ssh = new SshClient(host.IP, host.Login, host.Password))
                {
                    ssh.Connect();
                    var sshresult = ssh.RunCommand("vim-cmd vmsvc/getallvms | sed -e '1d' -e 's/ \\[.*$//' | awk '$1 ~ /^[0-9]+$/ {print $1\":\"substr($0,8,80)}'");
                    var input = sshresult.Result;
                    input = input.Replace('\n', ';');
                    var lineString = input.Split(';');
                    foreach (var lineSplit in lineString)
                    {
                        var word = lineSplit.Split(':');

                        if (word.Length == 2)
                        {
                            var item = InfoVM.StatePower.unknow;
                            var sshPower = ssh.RunCommand($"vim-cmd vmsvc/power.getstate {word[0]}");
                            var resPower = sshPower.Result;
                            if (resPower.Contains("Powered on"))
                            {
                                item = InfoVM.StatePower.on;
                            }
                            else if (resPower.Contains("Powered off"))
                            {
                                item = InfoVM.StatePower.off;
                            }
                            else
                            {
                                item = InfoVM.StatePower.unknow;
                            }
                            InfoVM infoVM = new InfoVM { VmId = word[0], Name = word[1], Power = item };
                            listVm.Add(infoVM);
                        }
                    }
                    ssh.Disconnect();
                }
            }
          
            return listVm;
        }
        public string SshShell(string address, string login, string password, string command)
        {
            string result = "";
            try
            {
                // Инициализация клиента Ssh
                SshClient sshClient = new SshClient(address, 22, login, password);

                //Открываем Соеденение
                sshClient.Connect();

                // Инициализация Терминальниека сеанса Ssh
                IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
                ShellStream shellStream = sshClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);


                //Послать команду1
                shellStream.WriteLine(command);
                Console.WriteLine(command);

                Thread.Sleep(750);
                //Результат Команды 
                string resultCommand = shellStream.Read();

                Console.WriteLine(resultCommand);
                result += resultCommand;
                shellStream.Dispose();

            }

            catch (Exception ex)
            {
                Console.WriteLine(address + ": Error 1201 " + ex.ToString());
            }
            return result;

        }
    }
   
     
}
