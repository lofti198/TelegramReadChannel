using PintrastAPI.Services.StreamStorage;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using TL;
using Channel = TL.Channel;

namespace PintrastAPI.Services.Telegram
{
    public class WTelegramBot
    {
        private readonly IStreamStorage _streamStorage;

        public WTelegramBot(IStreamStorage streamStorage)
        {
            _streamStorage = streamStorage;
        }
        public async Task<Messages_MessagesBase> ReadRecentPosts(TelegramReadChannelRequest request)
        {
            // Configure WTelegram client
            string Config(string what)
            {
                switch (what)
                {
                    case "api_id": return request.ApiId;
                    case "api_hash": return request.ApiHash;
                    case "phone_number": return request.PhoneNumber;
                    case "verification_code": return request.VerificationCode;
                    case "password": return request.Password;
                    // case "session_key": return _streamStorage.GetStreamAsync(request.PhoneNumber).ToString();
                    default: return null;
                }
            }
            Stream sessionStore = await _streamStorage.GetStreamAsync(request.PhoneNumber);
            sessionStore.Seek(0, SeekOrigin.Begin); // Ensure the stream is at the start

            using var client = new WTelegram.Client(Config);//, sessionStore);
            
            if (request.VerificationCode == default)
            {
                client.LoginUserIfNeeded();
                Thread.Sleep(5000); //Waiting for code, time can be changed
                throw new VerificationCodeNeededException();
            }
            await client.LoginUserIfNeeded();

            await _streamStorage.SaveStreamAsync(request.PhoneNumber, sessionStore);
            // Resolve channel username
            var resolvedPeer = await client.Contacts_ResolveUsername(NormalizeChannelUsername(request.ChannelUsername));
            var channel = resolvedPeer.chats[resolvedPeer.peer.ID] as Channel;

            // Get recent posts
            var messages = await client.Messages_GetHistory(channel, limit: request.NumberOfPosts + 1);


            return messages;
        }

        private string NormalizeChannelUsername(string username)
        {
            // Remove protocols and leading/trailing spaces
            username = username.Trim().Replace("https://", "").Replace("http://", "");

            // Extract username from t.me links
            var match = Regex.Match(username, @"t\.me/([^/]+)");
            if (match.Success)
            {
                username = match.Groups[1].Value;
            }

            // Remove "@" if it's present
            if (username.StartsWith("@"))
            {
                username = username.Substring(1);
            }

            return username;
        }
        
        public class TelegramReadChannelRequest
        {
            public string ApiId { get; set; }
            public string ApiHash { get; set; }
            public string PhoneNumber { get; set; }
            public string VerificationCode { get; set; } = null;
            public string Password { get; set; } = "";

            public string ChannelUsername { get; set; }
            public int NumberOfPosts { get; set; }
        }

        public class VerificationCodeNeededException : Exception
        {
            public VerificationCodeNeededException() : base("Verification code is required.") { }

            public VerificationCodeNeededException(string message) : base(message) { }

            public VerificationCodeNeededException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
