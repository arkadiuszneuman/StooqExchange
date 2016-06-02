﻿using System;
using Moq;
using StooqExchange.Core.Exceptions;
using StooqExchange.Core.ExchangeRateFinder;
using StooqExchange.Core.HttpDownloader;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using StooqExchange.Core;

namespace StooqExchange.UnitTest
{
    public class StooqCsvExchangeFinderTest
    {
        private readonly Mock<IHttpDownloader> httpDownloaderMock = new Mock<IHttpDownloader>();
        private readonly Mock<IDateTimeGetter> dateTimeGetterMock = new Mock<IDateTimeGetter>();

        [Theory, MemberData("Data")]
        public async void StooqCsvExchangeRateFinder_should_return_valid_exchange_rate(string csv, decimal expected)
        {
            DateTime now = DateTime.Now;

            httpDownloaderMock.Setup(x => x.DownloadAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => csv));

            dateTimeGetterMock.Setup(x => x.GetDateTime())
                .Returns(now);

            StooqCsvExchangeRateFinder finder = new StooqCsvExchangeRateFinder(httpDownloaderMock.Object, dateTimeGetterMock.Object);

            ExchangeRateValue result = await finder.FindExchangeAsync("WIG");

            Assert.Equal(expected, result.Value);
        }

        [Theory, MemberData("InvalidData")]
        public void StooqCsvExchangeRateFinder_should_throw_on_invalid_csv_text(string csv)
        {
            httpDownloaderMock.Setup(x => x.DownloadAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => csv));

            dateTimeGetterMock.Setup(x => x.GetDateTime())
                .Returns(DateTime.Now);

            StooqCsvExchangeRateFinder finder = new StooqCsvExchangeRateFinder(httpDownloaderMock.Object, dateTimeGetterMock.Object);

            Assert.ThrowsAsync<ExchangeRateFindException>(async () => await finder.FindExchangeAsync("WIG"));
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    new object[] { @"Symbol,Data,Czas,Otwarcie,Najwyzszy,Najnizszy,Zamkniecie,Wolumen\r\n
                                    WIG,2016-05-31,15:49:00,46710.74,46713.89,46305.43,46366.03,20399580", 46366.03 },

                    new object[] { @"Symbol,Data,Czas,Otwarcie,Najwyzszy,Najnizszy,Zamkniecie,Wolumen\r\n
                                    WIG20,2016-06-01,15:55:15,1809.97,1809.97,1759.02,1759.5,22298478", 1759.5 }
                };
            }
        }

        public static IEnumerable<object[]> InvalidData
        {
            get
            {
                return new[]
                {
                    new object[] { "asdasd"},

                    new object[] { @"Symbol,Data,Czas,Otwarcie,Najwyzszy,Najnizszy,Zamkniecie,Wolumen\r\n
                                    WIG,2016-05-31,15:49:00,46710.74,46713.89,46305.43,46366.03,20399580\r\n
                                    asdasdasd"},

                    new object[] { @"Symbol,Data,Czas,Otwarcie,Najwyzszy,Najnizszy,Zamkniecie,Wolumen\r\n
                                    WIG20,2016-06-01,15:55:15,1809.97,1759.02,1759.5,22298478"},

                    new object[] { @"Symbol,Data,Czas,Otwarcie,Najwyzszy,Najnizszy,Zamkniecie,Wolumen\r\n
                                    WIG,2016-05-31,15:49:00,46710.74,46713.89,46305.43,46366.03a,20399580"},
                };
            }
        }
    }
}