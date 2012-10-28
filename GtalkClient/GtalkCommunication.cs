using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;

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
        private string packetId;

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
                if (item.Jid.Server != "public.talk.google.com")
                {
                    if (!cm.contactList.ContainsKey(item.Jid.Bare))
                    {
                        cm.contactList.Add(item.Jid.Bare,item.Jid);
                        VcardIq viq = new VcardIq(IqType.get, new Jid(item.Jid.Bare));
                        packetId = viq.Id;
                        xmppCon.IqGrabber.SendIq(viq, new IqCB(VcardResult), item.Jid);
                    }


                }
            }

        }

        private void VcardResult(object sender, IQ iq, object data)
        {
            Jid jid = (Jid)data;
            if (!cm.users.ContainsKey(jid.Bare))
            {
                if (iq.Type == IqType.result)
                {
                    UserJabber user = new UserJabber();
                    Vcard vcard = iq.Vcard;
                    if (vcard != null)
                    {
                        user.FullName = vcard.Fullname;
                        user.Nickname = vcard.Nickname;

                        user.Description = vcard.Description;
                        user.jid = jid;

                        cm.users.Add(jid.Bare, user);


                    }


                }
            }
        }

        private void OnRosterEnd(object sender)
        {
            xmppCon.SendMyPresence();
            //cW.Refresh();
        }

        private void OnLogin(object sender)
        {
            mW.showChat();
        }

        private void OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if (pres.From.Server != "public.talk.google.com") {
                if (!cm.PresenceList.ContainsKey(pres.From.Bare)) {
                    cm.PresenceList[pres.From.Bare] = pres;
                    cW.Refresh();
                }
                VcardIq viq = new VcardIq(IqType.get, new Jid(pres.From.Bare));
                packetId = viq.Id;
                xmppCon.IqGrabber.SendIq(viq, new IqCB(VcardResult), pres.From);

            }
            

        }

        public void Send(Jid to, string b)
        {
            Jid from = UserJid;
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = to;
            msg.Body = b;
            if (cm.conversations.ContainsKey(from.Bare))
            {
                IList<Message> msgs = cm.conversations[msg.From.Bare];
                msgs.Add(msg);
                cm.conversations[msg.From.Bare]= msgs;
            }
            else {
                IList<Message> msgs = new List<Message>();
                msgs.Add(msg);
                cm.conversations.Add(msg.From.Bare, msgs);
            }

            


            xmppCon.Send(msg);
        }

        void OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            // ignore empty messages (events)
            if (msg.Body == null)
                return;
            if (cm.conversations.ContainsKey(msg.To.Bare))
            {
                IList<Message> msgs = cm.conversations[msg.To.Bare];
                msgs.Add(msg);
                cm.conversations[msg.To.Bare] = msgs;
            }
            else
            {
                IList<Message> msgs = new List<Message>();
                msgs.Add(msg);
                cm.conversations.Add(msg.To.Bare, msgs);
            }
        }
	
    }
}
