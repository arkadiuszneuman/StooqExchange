﻿using StooqExchange.Core.ExchangeRateSaver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using StooqExchange.Core;
using System.IO;

namespace StooqExchange.IntegrationTest
{
    public class JSONExchangeRateFileManagerTest
    {
        private readonly JSONExchangeRateFileManager fileManager = new JSONExchangeRateFileManager();

        public JSONExchangeRateFileManagerTest()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables("APPDATA")
                .Build();

            fileManager.Path = Path.Combine(config.GetChildren().First().Value, "exchange-rates.json");
        }

        [Theory, MemberData("Data")]
        public void JSONExchangeRateFileManager_should_save_valid_json_file(ExchangeRate[] exchangeRates, string expectedJson)
        {
            fileManager.Save(exchangeRates);

            var loadedJson = File.ReadAllText(fileManager.Path);

            File.Delete(fileManager.Path);

            Assert.Equal(expectedJson, loadedJson);
        }

        [Theory, MemberData("Data")]
        public void JSONExchangeRateFileManager_should_load_valid_exchange_rates(ExchangeRate[] expectedExchangeRates, string json)
        {
            File.WriteAllText(fileManager.Path, json);

            IEnumerable<ExchangeRate> loadedExchangeRates = fileManager.Load();

            File.Delete(fileManager.Path);

            Assert.Equal(expectedExchangeRates, loadedExchangeRates, new ExchangeRateEqualityComparer());
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new []
                        {
                            new ExchangeRate("WIG", new List<ExchangeRateValue>()
                            {
                                new ExchangeRateValue(new DateTime(2016, 1, 1), 10),
                                new ExchangeRateValue(new DateTime(2016, 1, 2, 15, 32, 20), (decimal)11.233)
                            }),
                            new ExchangeRate("WIG20", new List<ExchangeRateValue>()
                            {
                                new ExchangeRateValue(new DateTime(2016, 1, 1), (decimal)5.32),
                                new ExchangeRateValue(new DateTime(2016, 1, 2), (decimal)5.43),
                                new ExchangeRateValue(new DateTime(2016, 1, 3), (decimal)6.32),
                            })
                        },
                        "[{\"Name\":\"WIG\",\"Values\":[" +
                         "{\"DownloadTime\":\"2016-01-01T00:00:00\",\"Value\":10.0}," +
                         "{\"DownloadTime\":\"2016-01-02T15:32:20\",\"Value\":11.233}]}," +
                         "{\"Name\":\"WIG20\",\"Values\":[" +
                         "{\"DownloadTime\":\"2016-01-01T00:00:00\",\"Value\":5.32}," +
                         "{\"DownloadTime\":\"2016-01-02T00:00:00\",\"Value\":5.43}," +
                         "{\"DownloadTime\":\"2016-01-03T00:00:00\",\"Value\":6.32}]}]"
                    }
                };
            }
        }
    }
}
