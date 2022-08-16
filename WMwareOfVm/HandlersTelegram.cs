using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace WMwareOfVm
{
    class HandlersTelegram
    {
        public  Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        public  async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }
        private  async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            var action = message.Text!.Split(' ')[0] switch
            {
                "/GetId" => SendUserId(botClient, message),
                "/offVm" => SendInlineKeyboardOffVm(botClient, message),
                "/onVm" => SendInlineKeyboardOnVm(botClient, message),
                _ => Usage(botClient, message)
            };
            Message sentMessage = await action;

            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");
            Console.WriteLine($"The message was sent with id:'{sentMessage.MessageId}' message in chat {sentMessage.Chat.Id}... {sentMessage.Chat.Username} ");
           
        }
        private async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message)
        {

            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                // first row
                new[]
                {
                        InlineKeyboardButton.WithCallbackData("Off VM", "off"),
                        InlineKeyboardButton.WithCallbackData("On Vm", "on"),
                }
                );

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Choose",
                                                        replyMarkup: inlineKeyboard);
        }
        private async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
           
                const string usage = "Menu:\n" +
                                    "/GetId  - send inline keyboard\n" +
                                    "/offVm   - send inline keyboard\n" +
                                    "/onVm   - send inline keyboard";

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: usage, replyMarkup: new ReplyKeyboardRemove());

        }
        private async Task<Message> SendUserId(ITelegramBotClient botClient, Message message)
        {
            //https://tlgrm.ru/stickers?page=18
            var result = await botClient.SendTextMessageAsync(chatId: message.Chat.Id,text : $"Your id: {message.From.Id}" );
            await Usage(botClient, message);
            return result;
        }
        public async Task<Message> SendSticker(ITelegramBotClient botClient, Message message, VariantSticer variantStiker)
        {
            string stiker = "https://tlgrm.ru/_/stickers/c9e/fd2/c9efd2a0-abac-3538-a5bc-a790ff91f972/11.webp";
            if(variantStiker == VariantSticer.Loading)
            {
                stiker = "https://tlgrm.ru/stickers/animated_funny_puny2";
            }

            //https://tlgrm.ru/stickers?page=18
            var result = await botClient.SendStickerAsync(chatId: message.Chat.Id, sticker: stiker);
            await Usage(botClient, message);
            return result;
        }
        private List<List<InlineKeyboardButton>> GetRowListButtons(List<InfoVM> listVm, InfoVM.StatePower statePower)
        {
            List<List<InlineKeyboardButton>> rowListButtons = new List<List<InlineKeyboardButton>>();
            int resultCount = 0;

            if (listVm.Count >= 4)
            {
                int x = listVm.Count % 4;
                if (x >= 0) { resultCount = listVm.Count / 4 + 1; }
            }
            else
            {
                resultCount = 1;
            }

            for (int i = 0; i < resultCount; i++)
            {
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                foreach (var item in listVm.Take(4).ToList())
                {
                    buttons.Add(new InlineKeyboardButton(item.Name) { Text = item.Name, CallbackData = item.VmId + ":" + statePower });
                    listVm.Remove(item);

                }
                rowListButtons.Add(buttons);
            }
            return rowListButtons;
        }

        private async Task<Message> SendInlineKeyboardOffVm(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            ConfigurationUserAcces accesUser = new ConfigurationUserAcces();
            var accesUsers = accesUser.GetTokenAsync().GetAwaiter().GetResult().AccesUsers;
            var acces = accesUsers.Any(u => u.id == message.From.Id.ToString());

            if (!acces)
            {
                // Simulate longer running task
                return await SendSticker(botClient, message,VariantSticer.NoAcess);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "LOADING...");
                InlineKeyboardMarkup inlineKeyboard = null;
                List<List<InlineKeyboardButton>> rowListVmOff = new List<List<InlineKeyboardButton>>();
                ClientSSH ssh = new ClientSSH();
                //var phrase = ssh.ssh("10.8.0.184", "root", "crashtest123", "vim-cmd vmsvc/getallvms");
                //List<InfoVM> listVm = ssh.GetListVmPowerStatus(ssh.GetParseListVm(phrase), "10.8.0.184", "root", "crashtest123");
                ConfigurationHost configurationHost = new ConfigurationHost();
                var hostslist = configurationHost.GetTokenAsync().GetAwaiter().GetResult().HostList;

                List<InfoVM> listVm = ssh.GetListVmPowerStatus(hostslist);
                //List<InfoVM> listVm = ssh.GetListVmPowerStatus("10.8.0.184", "root", "crashtest123");

                listVm = listVm.Where(c => c.Power == InfoVM.StatePower.off).ToList();
                if (listVm.Count > 0 && listVm != null)
                {
                    rowListVmOff = GetRowListButtons(listVm, InfoVM.StatePower.on);
                  
                    inlineKeyboard = new InlineKeyboardMarkup(rowListVmOff);
                }
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,text: "Choose",replyMarkup: inlineKeyboard);
            }
        }
        private async Task<Message> SendInlineKeyboardOnVm(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            ConfigurationUserAcces accesUser = new ConfigurationUserAcces();
            var accesUsers = accesUser.GetTokenAsync().GetAwaiter().GetResult().AccesUsers;
            var acces = accesUsers.Any(u => u.id == message.From.Id.ToString());

            if (!acces)
            {
                return await SendSticker(botClient, message , VariantSticer.NoAcess);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "LOADING...");
                InlineKeyboardMarkup inlineKeyboard = null;
                List<List<InlineKeyboardButton>> rowListVmOff = new List<List<InlineKeyboardButton>>();
                ClientSSH ssh = new ClientSSH();
                //var phrase = ssh.ssh("10.8.0.184", "root", "crashtest123", "vim-cmd vmsvc/getallvms");
                //List<InfoVM> listVm = ssh.GetListVmPowerStatus(ssh.GetParseListVm(phrase), "10.8.0.184", "root", "crashtest123");
                ConfigurationHost configurationHost = new ConfigurationHost();
                var hostslist = configurationHost.GetTokenAsync().GetAwaiter().GetResult().HostList;
                List<InfoVM> listVm = ssh.GetListVmPowerStatus(hostslist);

                //List<InfoVM> listVm = ssh.GetListVmPowerStatus("10.8.0.184", "root", "crashtest123");
                listVm = listVm.Where(c => c.Power == InfoVM.StatePower.on).ToList();
                if (listVm.Count > 0 && listVm != null)
                {
                    rowListVmOff = GetRowListButtons(listVm, InfoVM.StatePower.off);

                    inlineKeyboard = new InlineKeyboardMarkup(rowListVmOff);
                }
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Choose", replyMarkup: inlineKeyboard);

                // await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Choose", replyMarkup: inlineKeyboard);
                // Simulate longer running task
                //await Task.Delay(500);
                //ConfigurationVM vM = new ConfigurationVM();
                //InlineKeyboardMarkup inlineKeyboard = null;
                //var vms = vM.GetTokenAsync().GetAwaiter().GetResult().VmList;
                //if (vms.Count > 0 && vms != null)
                //{
                //   // List<List<InlineKeyboardButton>> rowListButons = new List<List<InlineKeyboardButton>>();
                //    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                //    List<InlineKeyboardButton> buttons2 = new List<InlineKeyboardButton>();

                //    for (int i = 0; i <= vms.Count; i++)
                //    {
                //        if (i <= 4)
                //        {
                //            buttons.Add(new InlineKeyboardButton(vms[i].name) { Text = vms[i].name, CallbackData = vms[i].id });
                //            if (i == 4)
                //            {
                //                rowListVmOn.Add(buttons);
                //            }
                //        }
                //        else if (i > 4 && i <= 8)
                //        {
                //            buttons2.Add(new InlineKeyboardButton(vms[i].name) { Text = vms[i].name, CallbackData = vms[i].id });
                //            if (i == 8)
                //            {
                //                rowListVmOn.Add(buttons2);
                //            }
                //        }
                //    }
                //    inlineKeyboard = new InlineKeyboardMarkup(rowListVmOn);
                //}
               // return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Choose", replyMarkup: inlineKeyboard);
            }
        }
     
        
        private async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"Run  : {callbackQuery.Data}");
            ClientSSH ssh = new ClientSSH();
            InfoVM.StatePower item = InfoVM.StatePower.unknow;
            var t = callbackQuery.Data.Split(":");
            string id = null ;
            if (t.Length == 2)
            {
                id = t[0];
                if (t[1].Contains("on"))
                {
                    item = InfoVM.StatePower.on;
                }
                else if (t[1].Contains("off"))
                {
                    item = InfoVM.StatePower.off;
                }
                else
                {
                    item = InfoVM.StatePower.unknow;
                }
            }
             
           
           
            var status = ssh.PowerOffOnVm("10.8.0.184", "root", "crashtest123", item, id); 
            Console.WriteLine("**************"+ callbackQuery.Data);

            if(status == false)
            {
                await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Error id VM : {callbackQuery.Data}");
                await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Enter key ");
                ///  await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"Error idVM : {callbackQuery.Data}");
                /// await SendInlineKeyboardOffVm(botClient, callbackQuery.Message);
            }
            else if (status == true)
            {
                await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Done idVM : {callbackQuery.Data}");
               // await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"Done idVM : {callbackQuery.Data}");
             //  await SendInlineKeyboardOffVm(botClient, callbackQuery.Message);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Error Unknow idVM {callbackQuery.Data}");
              //  await botClient.SendTextMessageAsync( chatId: callbackQuery.Message.Chat.Id,text: $"Error Unknow idVM {callbackQuery.Data}");
            }
        }
        private async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent(
                    "hello"
                )
            )
        };

            await botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                   results: results,
                                                   isPersonal: true,
                                                   cacheTime: 0);
        }
        private Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }
        private Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        public enum VariantSticer
        {
            NoAcess,
            Loading
        }
    }
}
