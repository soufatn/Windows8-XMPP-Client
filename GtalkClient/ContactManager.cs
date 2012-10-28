using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;

namespace GtalkClient
{
    class ContactManager
    {
        private static ContactManager instance;
        private static readonly object myLock = new object();

        public IDictionary<string, Jid> contactList;
        public IDictionary<string, Presence> PresenceList;
        public IDictionary<string, IList<Message>> conversations;
        public IDictionary<string, UserJabber> users;
        
        private ContactManager() {
            contactList = new Dictionary<string, Jid>();
            PresenceList = new Dictionary<string, Presence>();
            conversations = new Dictionary<string, IList<Message>>();
            users = new Dictionary<string, UserJabber>();
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
