using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.ViewModel.SessionPlayer.Events
{
    class UpdateSourceEventArg : EventArgs 
    {
        public string Source { get; set; } = string.Empty;
        public bool IsEnabled { get; internal set; }
    }
}
