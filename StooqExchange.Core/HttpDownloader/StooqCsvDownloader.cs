﻿using System.Net.Http;
using System.Threading.Tasks;
using StooqExchange.Core.Exceptions;

namespace StooqExchange.Core.HttpDownloader
{
    /// <summary>
    /// Class is responsible for download csv file from stooq.pl site.
    /// </summary>
    public class StooqCsvDownloader : IHttpDownloader
    {
        public async Task<string> DownloadAsync(string stockIndex)
        {
            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = 
                await httpClient.GetAsync($"http://stooq.pl/q/l/?s={stockIndex}&f=sd2t2ohlcv&h&e=csv"))
            {
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();
                if (result.Contains($"{stockIndex.ToUpper()},B/D,B/D,B/D,B/D,B/D,B/D,B/D"))
                    throw new ExchangeRateFindException($"Cannot find index {stockIndex}");

                return result;
            }
        }
    }
}