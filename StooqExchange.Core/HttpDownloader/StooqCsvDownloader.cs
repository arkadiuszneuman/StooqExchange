using System.Net.Http;
using System.Threading.Tasks;

namespace StooqExchange.Core.HttpDownloader
{
    public class StooqCsvDownloader : IHttpDownloader
    {
        public async Task<string> DownloadAsync(string stockIndex)
        {
            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = 
                await httpClient.GetAsync($"http://stooq.pl/q/l/?s={stockIndex}&f=sd2t2ohlcv&h&e=csv"))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}