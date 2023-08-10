using EntityMtwServer;
using EntityMtwServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.Services
{
    public class AppSessionService
    {
        #region Singleton
        private AppSessionService() { }

        private static AppSessionService? instance;

        public static AppSessionService Instance
        {
            get
            {
                if (instance == null)
                    instance = new AppSessionService();

                return instance;
            }
        }
        #endregion
        
        public MasterServerContext Context { get; private set; } = new MasterServerContext();
        public User User { get; set; } = new User();
        public DVC Device { get; set; } = new DVC();
        public long RunningSessionId { get; set; } 
    }
}
