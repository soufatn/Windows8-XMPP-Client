using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;
using GtalkClient.Properties;

namespace GtalkClient
{
    public class GtalkCommunication
    {
        public Jid UserJid {get;set;}
        private XmppClientConnection xmppCon;
        private bool connected;
        private ContactManager cm;
        private MainWindow mW;
        private ChatWindows cW;
        private string packetId;
        private bool save;

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
        public void Connect(string password, bool _save)
        {
            this.save = _save;
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
                xmppCon.OnAuthError += new XmppElementHandler(AuthError);
                xmppCon.OnError += new ErrorHandler(xmppCon_OnError);
                xmppCon.OnClose += new ObjectHandler(xmppCon_OnClose);
                xmppCon.Open();
                connected = true;
            }
        }

        private void AuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            connected = false;
            Console.WriteLine("Error Login");
        }

        void xmppCon_OnClose(object sender)
        {
            
        }

        void xmppCon_OnError(object sender, Exception ex)
        {
            Console.WriteLine(ex);
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
           
            if (save)
            {
                Settings.Default["email"] = UserJid.Bare;
                Settings.Default["password"] = xmppCon.Password;
                Settings.Default.Save(); // Saves settings in application configuration file
            }
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
            MetroTalkMessage m = new MetroTalkMessage();
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = to;
            msg.Body = b;

            m.To = to;
            m.Body = b;
            m.From = from;
            m.Date = DateTime.Now;

            if (cm.conversations.ContainsKey(from.Bare))
            {
                IList<MetroTalkMessage> msgs = cm.conversations[from.Bare];
                msgs.Add(m);
                cm.conversations[from.Bare] = msgs;
            }
            else {
                IList<MetroTalkMessage> msgs = new List<MetroTalkMessage>();
                msgs.Add(m);
                cm.conversations.Add(from.Bare, msgs);
            }

            cW.RefreshConversation();


            xmppCon.Send(msg);
        }

        void OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            // ignore empty messages (events)
            if (msg.Body == null)
                return;
            MetroTalkMessage m = new MetroTalkMessage();

            m.Date = DateTime.Now;
            m.To = msg.To;
            m.From = msg.From;
            m.Body = msg.Body;

            if (cm.conversations.ContainsKey(msg.To.Bare))
            {
                IList<MetroTalkMessage> msgs = cm.conversations[msg.To.Bare];
                msgs.Add(m);
                cm.conversations[msg.To.Bare] = msgs;
            }
            else
            {
                IList<MetroTalkMessage> msgs = new List<MetroTalkMessage>();
                msgs.Add(m);
                cm.conversations.Add(msg.To.Bare, msgs);
            }

            cW.newMessage(m);
        }
	
    }
}
