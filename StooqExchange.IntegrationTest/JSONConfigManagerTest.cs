using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using StooqExchange.Core.Logger;
using StooqExchange.Core.ConfigManager;

namespace StooqExchange.IntegrationTest
{
    public class JSONConfigManagerTest
    {
        private readonly Mock<IStooqLogger> loggerMock = new Mock<IStooqLogger>();
        private readonly JSONConfigManager configManager;

        public JSONConfigManagerTest()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables("APPDATA")
                .Build();

            configManager = new JSONConfigManager(loggerMock.Object);
            configManager.Path = Path.Combine(config.GetChildren().First().Value, "config.json");
        }

        [Theory, MemberData("Data")]
        public void JSONExchangeRateFileManager_should_save_valid_json_file(Config config, string expectedJson)
        {
            configManager.Save(config);

            var loadedJson = File.ReadAllText(configManager.Path);

            File.Delete(configManager.Path);

            Assert.Equal(expectedJson, loadedJson);
        }

        [Theory, MemberData("Data")]
        public void JSONExchangeRateFileManager_should_load_valid_config(Config expectedConfig, string json)
        {
            File.WriteAllText(configManager.Path, json);

            Config loadedConfig = configManager.Load();

            File.Delete(configManager.Path);

            Assert.Equal(expectedConfig, loadedConfig, new IntervalStockIndicesEqualityComparer());
        }

        [Theory, MemberData("Data")]
        public void JSONExchangeRateFileManager_get_should_load_data_once(Config expectedConfig, string json)
        {
            Mock<JSONConfigManager> configManagerMock = new Mock<JSONConfigManager>(loggerMock.Object)
            {
                CallBase = true
            };

            File.WriteAllText(configManager.Path, json);

            configManagerMock.Object.Get();
            configManagerMock.Object.Get();
            configManagerMock.Object.Get();

            File.Delete(configManager.Path);

            configManagerMock.Verify(x => x.Load(), Times.Once);
        }

        [Theory, MemberData("Data")]
        public void JSONExchangeRateFileManager_get_should_load_valid_data(Config expectedConfig, string json)
        {
            Mock<JSONConfigManager> configManagerMock = new Mock<JSONConfigManager>(loggerMock.Object)
            {
                CallBase = true
            };
            configManagerMock.Setup(x => x.Load())
                .Returns(expectedConfig);

            File.WriteAllText(configManager.Path, json);

            var result = configManagerMock.Object.Get();

            File.Delete(configManager.Path);

            Assert.Equal(expectedConfig, result, new IntervalStockIndicesEqualityComparer());
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                       new Config()
                       {
                           Interval = 120,
                           StockIndices = new[] { "WIG", "WIG20", "FW20" }
                       },
                       "{\"Interval\":120,\"StockIndices\":[\"WIG\",\"WIG20\",\"FW20\"]}"
                    }
                };
            }
        }

        private sealed class IntervalStockIndicesEqualityComparer : IEqualityComparer<Config>
        {
            public bool Equals(Config x, Config y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Interval == y.Interval && x.StockIndices.SequenceEqual(y.StockIndices);
            }

            public int GetHashCode(Config obj)
            {
                unchecked
                {
                    return (obj.Interval * 397) ^ (obj.StockIndices != null ? obj.StockIndices.GetHashCode() : 0);
                }
            }
        }
    }
}
