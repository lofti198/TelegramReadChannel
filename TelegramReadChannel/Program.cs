using PintrastAPI.Services.StreamStorage;
using PintrastAPI.Services.StreamStoring;
using PintrastAPI.Services.Telegram;
using static PintrastAPI.Services.Telegram.WTelegramBot;

namespace TelegramReadChannel
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IStreamStorage storage = new InMemoryStreamStorage();

            WTelegramBot bot = new WTelegramBot(storage);

            while (true)
            {
                var requestParams = new TelegramReadChannelRequest()
                {
                    ApiId = "",
                    ApiHash = "",
                    PhoneNumber = "",
                    VerificationCode = "",
                    ChannelUsername = "https://t.me/ArtificialIntelligencedl",
                    NumberOfPosts = 2
                };

                Console.WriteLine("Enter verification code (leave blank if not required):");
                string verificationCode = Console.ReadLine();
                requestParams.VerificationCode = verificationCode;

                try
                {
                    var result = await bot.ReadRecentPosts(requestParams);
                }
                catch(Exception ex) 
                {
                    Console.WriteLine($"Error {ex.Message}");
                }
            }

        }
    }
}
