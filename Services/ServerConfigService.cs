using Microsoft.AspNetCore.Authentication;
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
        string serversFilePath = "_Configs\\servers.txt";
        string ftpFilePath = "_Configs\\ftp.txt";

        public ServerConfig GetServerConfig()
        {
            ServerConfig serverConfig = new();
            if (System.IO.File.Exists(configServerFilePath))
            {
                List<string> lines = System.IO.File.ReadAllLines(configServerFilePath).ToList();
                if (lines.Count == 5)
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


        public Dictionary<string, string> GetServers()
        {
            Dictionary<string, string> serverNames = new();
            if (System.IO.File.Exists(serversFilePath))
            {
                List<string> lines = System.IO.File.ReadAllLines(serversFilePath).ToList();
                foreach (string line in lines)
                {
                    List<string> lineValues = line.Split(":").ToList();
                    serverNames.Add(lineValues.First(), lineValues.Last());
                }
            }
            else
            {
                System.IO.File.WriteAllLines(serversFilePath, new string[] { "localhost:local" });
                serverNames.Add("localhost", "local");
            }

            return serverNames;
        }

        public void AddServer(string serverIp, string serverName)
        {
            Dictionary<string, string> servers = GetServers();
            if (!servers.ContainsKey(serverIp) && !servers.ContainsValue(serverName))
            {
                List<string> lines = System.IO.File.ReadAllLines(serversFilePath).ToList();
                lines.Add($"{serverIp}:{serverName}");
                System.IO.File.WriteAllLines(serversFilePath, lines.ToArray());
            }
        }

        public string GetFtpConfig()
        {
            if (System.IO.File.Exists(ftpFilePath))
            {
                List<string> lines = System.IO.File.ReadAllLines(@ftpFilePath).ToList();
                return lines.First();
            }

            else
                return "localhost";
        }

        public void SetFtpConfig(string ip, string username, string password)
        {
            System.IO.File.WriteAllLines(ftpFilePath, new string[]
            {
                ip,
                username,
                password
            });
        }

    }
}
