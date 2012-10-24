using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;

namespace GtalkClient
{
    class GtalkCommunication
    {
        public Jid UserJid {get;set;}
        private XmppClientConnection xmppCon;
        private bool connected;
        private ContactManager cm;
        private MainWindow mW;
        private ChatWindows cW;

        public GtalkCommunication(MainWindow _mw, ChatWindows _cw)
        {
            cm = ContactManager.getInstance();
            xmppCon = new XmppClientConnection();
            mW = _mw;
            cW = _cw;
        }
        public GtalkCommunication(Jid _j,MainWindow _mw,ChatWindows _cw)
        {
            cm = ContactManager.getInstance();
            xmppCon = new XmppClientConnection();
            mW = _mw;
            cW = _cw;
            UserJid = _j;
        }
        public void Connect(string password)
        {
            if (!connected)
            {
                xmppCon.Username = UserJid.User;
                xmppCon.Server = UserJid.Server;
                xmppCon.Password = password;
                xmppCon.AutoResolveConnectServer = true;
                //xmppCon.OnRosterStart += new ObjectHandler(xmppCon_OnRosterStart);
                xmppCon.OnRosterItem += new agsXMPP.XmppClientConnection.RosterHandler(OnRosterResult);
                xmppCon.OnRosterEnd += new ObjectHandler(OnRosterEnd);
                xmppCon.OnPresence += new agsXMPP.protocol.client.PresenceHandler(OnPresence);
                xmppCon.OnMessage += new agsXMPP.protocol.client.MessageHandler(OnMessage);
                xmppCon.OnLogin += new ObjectHandler(OnLogin);
                xmppCon.Open();
                connected = true;
            }
        }

        private void OnRosterResult(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            //_debug += item.ToString();
            if (item.Name != null)
            {
                cm.contactList.Add(item.Jid, new Presence(ShowType.NONE, "Offline"));
            }

        }

        private void OnRosterEnd(object sender)
        {
            xmppCon.SendMyPresence();
        }

        private void OnLogin(object sender)
        {
            cW.Show();
        }

        private void OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            cm.contactList[pres.From] = pres;

        }

        private void cmdSend_Click()
        {
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = new Jid("");
            msg.Body = "";

            IList<Message> msgs = cm.conversations[msg.To];
            msgs.Add(msg);
            cm.conversations.Add(msg.From, msgs);

            xmppCon.Send(msg);
        }

        void OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            // ignore empty messages (events)
            if (msg.Body == null)
                return;
            IList<Message> msgs = cm.conversations[msg.From];
            msgs.Add(msg);
            cm.conversations.Add(msg.From, msgs);
        }
	
    }
}
