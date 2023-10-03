using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.Model
{
    public class MessageModel
    {
        public string Type { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool Buttons { get; set; } = false;
        public bool Loading { get; set; } = false;
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 100;
        public string Image { get; set; } = string.Empty;    
    }
}
