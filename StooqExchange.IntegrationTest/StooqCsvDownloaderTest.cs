using Xunit;
using StooqExchange.Core.HttpDownloader;
using System.Linq;
using System.Collections.Generic;
using System;

namespace StooqExchange.IntegrationTest
{
    public class StooqCsvDownloaderTest
    {
        [Fact]
        public async void Should_download_valid_header()
        {
            StooqCsvDownloader downloader = new StooqCsvDownloader();

            string downloaded = await downloader.DownloadAsync("WIG");
            string result = downloaded.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            Assert.Equal("Symbol,Data,Czas,Otwarcie,Najwyzszy,Najnizszy,Zamkniecie,Wolumen", result);
        }

        [Fact]
        public async void Should_download_valid_data()
        {
            StooqCsvDownloader downloader = new StooqCsvDownloader();

            string downloaded = await downloader.DownloadAsync("WIG");
            string[] result = downloaded.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault()
                .Split(',');

            Assert.Equal(8, result.Length);
            Assert.Equal("WIG", result[0]);
            Assert.Equal(DateTime.Today, Convert.ToDateTime(result[1]));
        }
    }
}