using Xunit;
using StooqExchange.Core.HttpDownloader;
using System.Linq;
using System.Collections.Generic;
using System;
using StooqExchange.Core.Exceptions;

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
        public void Should_return_throw_when_index_doesnt_exists()
        {
            StooqCsvDownloader downloader = new StooqCsvDownloader();

            ExchangeRateFindException exception = Assert.ThrowsAsync<ExchangeRateFindException>(
                async () => await downloader.DownloadAsync("index_doesnt_exists")).Result;

            Assert.Equal("Cannot find index index_doesnt_exists", exception.Message);
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

            DateTime expectedDate;
            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    expectedDate = DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0));
                    break;
                case DayOfWeek.Sunday:
                    expectedDate = DateTime.Today.Subtract(new TimeSpan(2, 0, 0, 0));
                    break;
                default:
                    expectedDate = DateTime.Today;
                    break;
            }

            Assert.Equal(expectedDate.Date, Convert.ToDateTime(result[1]));
        }
    }
}