using System.Collections.Generic;
using System.Linq;

namespace StooqExchange.Core
{
    public sealed class ExchangeRateEqualityComparer : IEqualityComparer<ExchangeRate>
    {
        public bool Equals(ExchangeRate x, ExchangeRate y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.Name, y.Name) && Enumerable.SequenceEqual(x.Values, y.Values, new ExchangeRateValueEqualityComparer());
        }

        public int GetHashCode(ExchangeRate obj)
        {
            unchecked
            {
                return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ (obj.Values != null ? obj.Values.GetHashCode() : 0);
            }
        }

        private sealed class ExchangeRateValueEqualityComparer : IEqualityComparer<ExchangeRateValue>
        {
            public bool Equals(ExchangeRateValue x, ExchangeRateValue y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.DownloadTime.Equals(y.DownloadTime) && x.Value == y.Value;
            }

            public int GetHashCode(ExchangeRateValue obj)
            {
                unchecked
                {
                    return (obj.DownloadTime.GetHashCode() * 397) ^ obj.Value.GetHashCode();
                }
            }
        }
    }
}