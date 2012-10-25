using System;
using System.Collections;
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
        private static readonly object myLock = new object();

        public IDictionary<Jid, Presence> contactList;
        public IDictionary<Jid, IList<Message>> conversations;
        
        private ContactManager() {
            contactList = new Dictionary<Jid, Presence>();
            conversations = new Dictionary<Jid, IList<Message>>();
        }


        public static ContactManager getInstance() {
            lock (myLock) 
            { 
                if (instance == null) instance = new ContactManager();
                return instance; 
            } 
        }
    }
}
