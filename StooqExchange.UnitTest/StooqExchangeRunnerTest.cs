using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using StooqExchange.Core;
using StooqExchange.Core.ExchangeRateFinder;
using StooqExchange.Core.ExchangeRateArchiveManager;
using Xunit;
using StooqExchange.Core.DecisionMaker;
using System.Linq;
using StooqExchange.Core.Logger;

namespace StooqExchange.UnitTest
{
    public class StooqExchangeRunnerTest
    {
        private readonly Mock<IExchangeFinder> exchangeFinderMock = new Mock<IExchangeFinder>();
        private readonly Mock<JSONExchangeRateArchiveManager> exchangeRateArchiveManagerMock;
        private readonly Mock<INewExchangeRateDecisionMaker> decisionMakerMock = new Mock<INewExchangeRateDecisionMaker>();
        private readonly Mock<IStooqLogger> stooqLoggerMock = new Mock<IStooqLogger>();
        private readonly StooqExchangeRunner exchangeRunner;

        public StooqExchangeRunnerTest()
        {
            exchangeRateArchiveManagerMock = new Mock<JSONExchangeRateArchiveManager>(stooqLoggerMock.Object);
            exchangeRunner = new StooqExchangeRunner(exchangeFinderMock.Object,
                exchangeRateArchiveManagerMock.Object, decisionMakerMock.Object, stooqLoggerMock.Object);
        }

        [Fact]
        public void Should_load_data_for_all_stock_indexes()
        {
            exchangeRunner.RunOnce("WIG", "WIG20", "WIG20 Fut");

            exchangeFinderMock.Verify(x => x.FindExchangeAsync("WIG"), Times.Once);
            exchangeFinderMock.Verify(x => x.FindExchangeAsync("WIG20"), Times.Once);
            exchangeFinderMock.Verify(x => x.FindExchangeAsync("WIG20 Fut"), Times.Once);
        }

        [Fact]
        public void Should_load_once_exchange_rates_archive()
        {
            exchangeRunner.RunOnce("WIG", "WIG20", "WIG20 Fut");

            exchangeRateArchiveManagerMock.Verify(x => x.Get(), Times.Once);
        }

        [Fact]
        public void Should_verify_new_exchange_rate()
        {
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>(GetExchangeRates());
            ExchangeRate exchangeRateWIG = exchangeRates.First();
            ExchangeRate exchangeRateWIG20 = exchangeRates.Last();

            exchangeRateArchiveManagerMock.Setup(x => x.Get())
                .Returns(new[] { exchangeRateWIG, exchangeRateWIG20 });

            ExchangeRateValue newExchangeRateWIG = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56);
            ExchangeRateValue newExchangeRateWIG20 = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)453.43);
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG));
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG20"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG20));

            exchangeRunner.RunOnce("WIG", "WIG20");

            decisionMakerMock.Verify(x => x.ShouldRateBeAdd(exchangeRateWIG, newExchangeRateWIG), Times.Once);
            decisionMakerMock.Verify(x => x.ShouldRateBeAdd(exchangeRateWIG20, newExchangeRateWIG20), Times.Once);
        }

        [Fact]
        public void Should_save_once_exchange_rates_archive()
        {
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>(GetExchangeRates());
            ExchangeRate exchangeRateWIG = exchangeRates.First();
            ExchangeRate exchangeRateWIG20 = exchangeRates.Last();

            exchangeRateArchiveManagerMock.Setup(x => x.Get())
                .Returns(exchangeRates);

            ExchangeRateValue newExchangeRateWIG = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56);
            ExchangeRateValue newExchangeRateWIG20 = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)6412.23);
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG));
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG20"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG20));

            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG, newExchangeRateWIG))
                .Returns(true);
            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG20, newExchangeRateWIG20))
                .Returns(true);

            exchangeRunner.RunOnce("WIG", "WIG20");

            exchangeRateArchiveManagerMock.Verify(x => x.Save(exchangeRates), Times.Once);
        }

        [Fact]
        public void Shouldnt_save_when_no_new_exchange_rates()
        {
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>(GetExchangeRates());
            ExchangeRate exchangeRateWIG = exchangeRates.First();
            ExchangeRate exchangeRateWIG20 = exchangeRates.Last();

            exchangeRateArchiveManagerMock.Setup(x => x.Get())
                .Returns(exchangeRates);

            ExchangeRateValue newExchangeRateWIG = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56);
            ExchangeRateValue newExchangeRateWIG20 = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)6412.23);
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG));
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG20"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG20));

            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG, newExchangeRateWIG))
                .Returns(false);
            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG20, newExchangeRateWIG20))
                .Returns(false);

            exchangeRunner.RunOnce("WIG", "WIG20");

            exchangeRateArchiveManagerMock.Verify(x => x.Save(exchangeRates), Times.Never);
        }

        [Fact]
        public void Should_save_exchange_rates_archive_with_new_elements()
        {
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>(GetExchangeRates());
            ExchangeRate exchangeRateWIG = exchangeRates.First();
            ExchangeRate exchangeRateWIG20 = exchangeRates.Last();

            exchangeRateArchiveManagerMock.Setup(x => x.Get())
                .Returns(exchangeRates);

            ExchangeRateValue newExchangeRateWIG = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56);
            ExchangeRateValue newExchangeRateWIG20 = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)6412.23);
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG));
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG20"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG20));

            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG, newExchangeRateWIG))
                .Returns(true);
            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG20, newExchangeRateWIG20))
                .Returns(true);

            var expected = GetExchangeRates().ToList();
            expected.First().Values.Add(new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56));
            expected.Last().Values.Add(new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)6412.23));
            IEnumerable<ExchangeRate> savedExchangeRates = null;
            exchangeRateArchiveManagerMock.Setup(x => x.Save(It.IsAny<List<ExchangeRate>>()))
                .Callback<IEnumerable<ExchangeRate>>((e) => savedExchangeRates = e);

            exchangeRunner.RunOnce("WIG", "WIG20");

            Assert.Equal(expected, savedExchangeRates, new ExchangeRateEqualityComparer());
        }

        [Fact]
        public void Should_save_exchange_rates_archive_with_new_elements_only_when_new_element_should_be_added()
        {
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>(GetExchangeRates());
            ExchangeRate exchangeRateWIG = exchangeRates.First();
            ExchangeRate exchangeRateWIG20 = exchangeRates.Last();

            exchangeRateArchiveManagerMock.Setup(x => x.Get())
                .Returns(exchangeRates);

            ExchangeRateValue newExchangeRateWIG = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56);
            ExchangeRateValue newExchangeRateWIG20 = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)3426412.23);
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG));
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG20"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG20));

            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG, newExchangeRateWIG))
                .Returns(true);
            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG20, newExchangeRateWIG20))
                .Returns(false);

            var expected = GetExchangeRates().ToList();
            expected.First().Values.Add(new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56));
            IEnumerable<ExchangeRate> savedExchangeRates = null;
            exchangeRateArchiveManagerMock.Setup(x => x.Save(It.IsAny<List<ExchangeRate>>()))
                .Callback<IEnumerable<ExchangeRate>>((e) => savedExchangeRates = e);

            exchangeRunner.RunOnce("WIG", "WIG20");

            Assert.Equal(expected, savedExchangeRates, new ExchangeRateEqualityComparer());
        }

        [Fact]
        public void Should_save_exchange_rates_archive_with_new_stocks()
        {
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>(new[] { GetExchangeRates().First() });
            ExchangeRate exchangeRateWIG = exchangeRates.First();

            exchangeRateArchiveManagerMock.Setup(x => x.Get())
                .Returns(exchangeRates);

            ExchangeRateValue newExchangeRateWIG = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56);
            ExchangeRateValue newExchangeRateWIG20 = new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)6412.23);
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG));
            exchangeFinderMock.Setup(x => x.FindExchangeAsync("WIG20"))
                .Returns(Task.Factory.StartNew(() => newExchangeRateWIG20));

            decisionMakerMock.Setup(x => x.ShouldRateBeAdd(exchangeRateWIG, newExchangeRateWIG))
                .Returns(true);

            var expected = new List<ExchangeRate>(new[] { GetExchangeRates().First() });
            expected.First().Values.Add(new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)1234.56));
            expected.Add(new ExchangeRate("WIG20", new List<ExchangeRateValue>()
                { new ExchangeRateValue(new DateTime(2016, 1, 4), (decimal)6412.23) }));
            IEnumerable<ExchangeRate> savedExchangeRates = null;
            exchangeRateArchiveManagerMock.Setup(x => x.Save(It.IsAny<List<ExchangeRate>>()))
                .Callback<IEnumerable<ExchangeRate>>((e) => savedExchangeRates = e);

            exchangeRunner.RunOnce("WIG", "WIG20");

            Assert.Equal(expected, savedExchangeRates, new ExchangeRateEqualityComparer());
        }

        private IEnumerable<ExchangeRate> GetExchangeRates()
        {
            yield return new ExchangeRate("WIG",
               new List<ExchangeRateValue>
               {
                    new ExchangeRateValue(new DateTime(2016, 1, 1), (decimal) 2031.12),
                    new ExchangeRateValue(new DateTime(2016, 1, 2), (decimal) 432.32)
               });
            yield return new ExchangeRate("WIG20",
                new List<ExchangeRateValue>
                {
                    new ExchangeRateValue(new DateTime(2016, 1, 2), (decimal) 32234.32),
                    new ExchangeRateValue(new DateTime(2016, 1, 3), (decimal) 3426412.23)
                });
        }
    }
}