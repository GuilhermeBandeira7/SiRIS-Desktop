using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.ViewModel.FileManagement
{
    public class ReturnSelectedFileEventArg : EventArgs
    {
        public List<string> SelectedFiles { get; set; } =  new();
    }
}
