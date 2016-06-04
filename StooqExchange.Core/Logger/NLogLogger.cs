using System;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;

namespace StooqExchange.Core.Logger
{
    /// <summary>
    /// Class is responsible for log messages using NLog.
    /// </summary>
    public class NLogLogger : IStooqLogger
    {
        private readonly NLog.Logger logger;

        public NLogLogger()
        {
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "NLog.config")))
            {
                var consoleMessageTarget = new ColoredConsoleTarget
                {
                    Layout = @"${date:format=HH\:mm\:ss} ${message:withException=false}${exception}",
                    ErrorStream = false
                };

                var rule = new LoggingRule("*", LogLevel.Info, consoleMessageTarget);

                var config = new LoggingConfiguration();
                config.AddTarget("console", consoleMessageTarget);
                config.LoggingRules.Add(rule);

                LogManager.Configuration = config;
            }

            logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Warning(Exception exception)
        {
            logger.Warn(exception, string.Empty);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(Exception exception)
        {
            logger.Error(exception, string.Empty);
        }

        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        public void Fatal(Exception exception)
        {
            logger.Fatal(exception, string.Empty);
        }
    }
}