namespace PintrastAPI.Services.StreamStorage
{
    public interface IStreamStorage
    {
        Task SaveStreamAsync(string userPhone, Stream stream);
        Task<Stream> GetStreamAsync(string userPhone);
    }
}
