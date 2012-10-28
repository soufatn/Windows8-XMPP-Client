using agsXMPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtalkClient
{
    public class MetroTalkMessage
    {
        public Jid From { get; set; }
        public Jid To { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public string DateTimeStr
        {
            get { return this.Date.Hour+" : "+this.Date.Minute; }
        } 

        public MetroTalkMessage()
        {
        }
    }
}
