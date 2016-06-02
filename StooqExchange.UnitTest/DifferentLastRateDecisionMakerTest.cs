using System;
using StooqExchange.Core.DecisionMaker;
using System.Collections.Generic;
using Xunit;
using StooqExchange.Core;
using StooqExchange.Core.Exceptions;

namespace StooqExchange.UnitTest
{
    public class DifferentLastRateDecisionMakerTest
    {
        DifferentLastRateDecisionMaker decisionMaker = new DifferentLastRateDecisionMaker();

        [Fact]
        public void DifferentLastRateDecisionMaker_should_throw_if_exchange_rate_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => decisionMaker.ShouldRateBeAdd(
                null, new ExchangeRateValue()));
        }

        [Fact]
        public void DifferentLastRateDecisionMaker_should_throw_if_new_exchange_rate_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => decisionMaker.ShouldRateBeAdd(
                new ExchangeRate("WIG", new List<ExchangeRateValue>()), null));
        }

        [Fact]
        public void DifferentLastRateDecisionMaker_should_throw_if_list_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => decisionMaker.ShouldRateBeAdd(
                new ExchangeRate("WIG", null), new ExchangeRateValue()));
        }

        [Fact]
        public void DifferentLastRateDecisionMaker_should_return_true_if_value_collection_is_empty()
        {
            bool result = decisionMaker.ShouldRateBeAdd(
                new ExchangeRate("WIG", new List<ExchangeRateValue>()), new ExchangeRateValue());

            Assert.True(result);
        }

        [Fact]
        public void DifferentLastRateDecisionMaker_should_return_true_if_new_exchange_rate_value_is_different_than_last()
        {
            bool result = decisionMaker.ShouldRateBeAdd(
                new ExchangeRate("WIG", new List<ExchangeRateValue>()
                {
                    new ExchangeRateValue(new DateTime(2016, 1, 2), 21),
                    new ExchangeRateValue(new DateTime(2016, 1, 1), 20)
                }), new ExchangeRateValue(new DateTime(2016, 1, 3), 20));

            Assert.True(result);
        }

        [Fact]
        public void DifferentLastRateDecisionMaker_should_return_false_if_new_exchange_rate_value_equals_last()
        {
            bool result = decisionMaker.ShouldRateBeAdd(
                new ExchangeRate("WIG", new List<ExchangeRateValue>()
                {
                    new ExchangeRateValue(new DateTime(2016, 1, 2), (decimal)21.12),
                    new ExchangeRateValue(new DateTime(2016, 1, 1), 20)
                }), new ExchangeRateValue(new DateTime(2016, 1, 3), (decimal)21.12));

            Assert.False(result);
        }

        [Fact]
        public void DifferentLastRateDecisionMaker_should_throw_if_new_exchange_rate_date_is_earlier_then_last_exchange_rate()
        {
            Assert.Throws<InvalidExchangeRateException>(() => decisionMaker.ShouldRateBeAdd(
                new ExchangeRate("WIG", new List<ExchangeRateValue>()
                {
                    new ExchangeRateValue(new DateTime(2016, 1, 2, 12, 23, 30), 21),
                    new ExchangeRateValue(new DateTime(2016, 1, 1), 20)
                }), new ExchangeRateValue(new DateTime(2016, 1, 2, 12, 23, 29), 20)));
        }
    }
}