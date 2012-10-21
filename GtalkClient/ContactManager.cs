using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;

namespace GtalkClient
{
    class ContactManager
    {
        private static ContactManager instance;
        private ContactManager() {}
        private static readonly object myLock = new object();

        public IDictionary<Jid, Presence> contactList = new Dictionary<Jid, Presence>();

        public static ContactManager getInstance() {
            lock (myLock) 
            { 
                if (instance == null) instance = new ContactManager();
                return instance; 
            } 
        }
    }
}
