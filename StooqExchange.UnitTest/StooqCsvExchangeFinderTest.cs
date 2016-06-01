using StooqExchange.Core.Exceptions;
using StooqExchange.Core.ExchangeRateFinder;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace StooqExchange.UnitTest
{
    public class StooqCsvExchangeFinderTest
    {
        [Theory, MemberData("Data")]
        public void StooqCsvExchangeRateFinder_should_return_valid_exchange_rate(string csv, decimal expected)
        {
            StooqCsvExchangeRateFinder finder = new StooqCsvExchangeRateFinder();

            decimal result = finder.FindExchange(csv);

            Assert.Equal(expected, result);
        }

        [Theory, MemberData("InvalidData")]
        public void StooqCsvExchangeRateFinder_should_throw_on_invalid_csv_text(string csv)
        {
            StooqCsvExchangeRateFinder finder = new StooqCsvExchangeRateFinder();

            Assert.Throws<ExchangeRateFindException>(() => finder.FindExchange(csv));
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