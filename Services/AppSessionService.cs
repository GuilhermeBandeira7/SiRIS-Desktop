using EntityMtwServer;
using EntityMtwServer.Entities;
using EntityMtwServer.Services;
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
        private AppSessionService()
        {
            Context = new();
            UsersService = new(Context);
            AccessRulesService = new(Context, UsersService);
            SessionService = new(Context, AccessRulesService, UsersService);
            CoursesService = new();
            CurriculumCoursesService = new(Context);
        
        }

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
        
        public MasterServerContext Context { get; private set; }
        public User User { get; set; } = new User();
        public DVC Device { get; set; } = new DVC();
        public long RunningSessionId { get; set; }

        private AccessRulesService AccessRulesService { get; set; }
        public UsersService UsersService { get; set; }
        public SessionsService SessionService { get; set; }
        public CurriculumCoursesService CurriculumCoursesService { get; set; }
        public CoursesService CoursesService { get; set; }
    }
}
