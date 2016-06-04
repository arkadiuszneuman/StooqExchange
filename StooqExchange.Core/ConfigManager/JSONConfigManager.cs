using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using StooqExchange.Core.Logger;
using StooqExchange.Core.Exceptions;

namespace StooqExchange.Core.ConfigManager
{
    /// <summary>
    /// Class is responsible for save and load config to/from JSON file. 
    /// By default file is saved to application directory as config.json.
    /// </summary>
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
            try
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
            catch (JsonReaderException)
            {
                throw new ConfigException("Invalid config file");
            }
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