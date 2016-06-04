namespace StooqExchange.Core.ConfigManager
{
    public interface IConfigManager
    {
        Config Load();
        Config Get();
        void Save(Config config);
    }
}