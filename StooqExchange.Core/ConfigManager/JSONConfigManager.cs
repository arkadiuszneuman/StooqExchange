using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using StooqExchange.Core.Logger;

namespace StooqExchange.Core.ConfigManager
{
    public class JSONConfigManager : IConfigManager
    {
        private readonly IStooqLogger logger;
        private Config loadedConfig;

        public JSONConfigManager(IStooqLogger logger)
        {
            this.logger = logger;
        }

        public virtual Config Load()
        {
            if (!File.Exists(Path))
            {
                loadedConfig = new Config();
                Save(loadedConfig);
                return loadedConfig;
            }

            logger.Info("Loading config");
            string json = File.ReadAllText(Path);
            var config = JsonConvert.DeserializeObject<Config>(json);

            logger.Info("Config loaded");

            return loadedConfig = config;
        }

        public virtual Config Get()
        {
            if (loadedConfig == null)
                loadedConfig = Load();

            return loadedConfig;
        }

        public virtual void Save(Config config)
        {
            logger.Info("Saving config");
            string json = JsonConvert.SerializeObject(config);
            File.WriteAllText(Path, json);

            loadedConfig = config;

            logger.Info("Config saved");
        }

        public string Path { get; set; } = System.IO.Path.Combine(System.IO.Path
            .GetDirectoryName(Assembly.GetEntryAssembly().Location), "config.json");
    }
}