using System;

namespace StooqExchange.Core.Logger
{
    public interface IStooqLogger
    {
        void Info(string message);
        void Warning(string message);
        void Warning(Exception exception);
        void Error(string message);
        void Error(Exception exception);
        void Fatal(string message);
        void Fatal(Exception exception);
    }
}