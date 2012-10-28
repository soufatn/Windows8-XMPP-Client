using agsXMPP;
using agsXMPP.protocol.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtalkClient
{
    public class UserJabber
    {
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public Jid jid { get; set; }
        public Presence pres { get; set; }
        public UserJabber() { }
    }
}
