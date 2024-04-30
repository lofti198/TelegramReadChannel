using PintrastAPI.Services.StreamStorage;
using System.Collections.Concurrent;

namespace PintrastAPI.Services.StreamStoring
{
    public class InMemoryStreamStorage : IStreamStorage
    {
        private readonly ConcurrentDictionary<string, byte[]> _streams = new ConcurrentDictionary<string, byte[]>();

        public async Task SaveStreamAsync(string userPhone, Stream stream)
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            _streams[userPhone] = memoryStream.ToArray();
        }

        public Task<Stream> GetStreamAsync(string userPhone)
        {
            if (_streams.TryGetValue(userPhone, out var buffer))
            {
                return Task.FromResult<Stream>(new MemoryStream(buffer));
            }

            return Task.FromResult<Stream>(new MemoryStream());
        }
    }

}
