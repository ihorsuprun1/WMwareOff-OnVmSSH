using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;


namespace WMwareOfVm
{
    class Telegram
    {
        private  TelegramBotClient Bot;
        public async Task StartBotAsync()
        {
            
            Bot = new TelegramBotClient(ConfigurationTelegram.BotToken);
            HandlersTelegram handlers = new HandlersTelegram();
            User me = await Bot.GetMeAsync();
            Console.Title = me.Username ?? "My awesome Bot";

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new ReceiverOptions() { AllowedUpdates = { } };
            Bot.StartReceiving(handlers.HandleUpdateAsync,
                               handlers.HandleErrorAsync,
                               receiverOptions,
                               cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}
