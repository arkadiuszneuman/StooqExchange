using System.Threading.Tasks;

namespace StooqExchange.Core.HttpDownloader
{
    public interface IHttpDownloader
    {
        Task<string> DownloadAsync(string stockIndex);
    }
}