using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.Services
{
    public class ServerConfigService
    {
        #region SINGLETON

        private static ServerConfigService? instance = null;

        private ServerConfigService()
        {


        }

        public static ServerConfigService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerConfigService();
                }
                return instance;
            }
        }

        #endregion


        string configServerFilePath = "_Configs\\serverConfig.txt";

        public ServerConfig GetServerConfig()
        {
            ServerConfig serverConfig = new();
            if(System.IO.File.Exists(configServerFilePath))
            {
                List<string> lines = System.IO.File.ReadAllLines(configServerFilePath).ToList();
                if(lines.Count == 5)
                {
                    serverConfig.Ip = lines[0];
                    serverConfig.Database = lines[1];
                    serverConfig.Username = lines[2];
                    serverConfig.Password = lines[3];
                    serverConfig.SerialNumber = lines[4];
                }
            }

            return serverConfig;
        }

        public void SetServerConfig(ServerConfig serverConfig)
        {
            System.IO.File.WriteAllLines(configServerFilePath, new string[]
            {
                serverConfig.Ip,
                serverConfig.Database,
                serverConfig.Username,
                serverConfig.Password,
                serverConfig.SerialNumber,
            });
        }
    }
}
