using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using agsXMPP;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.client;

namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO deplacer le gestionnaire agsXMPP to GtalkCommunication

        XmppClientConnection xmppCon = new XmppClientConnection();
        private bool connected = false;
        private string _debug = "";
        private ContactManager cm;

        
        public MainWindow()
        {
            InitializeComponent();
            cm = ContactManager.getInstance();
        }

        public void Connect(object sender, RoutedEventArgs e)
        {
            if (!connected)
            {
                Jid jidUser = new Jid(email.Text);
                xmppCon.Username = jidUser.User;
                xmppCon.Server = jidUser.Server;
                xmppCon.Password = password.Password;
                xmppCon.AutoResolveConnectServer = true;
                //xmppCon.OnRosterStart += new ObjectHandler(xmppCon_OnRosterStart);
                xmppCon.OnRosterItem += new agsXMPP.XmppClientConnection.RosterHandler(OnRosterResult);
                xmppCon.OnRosterEnd += new ObjectHandler(OnRosterEnd);
                xmppCon.OnPresence += new agsXMPP.protocol.client.PresenceHandler(OnPresence);
                xmppCon.OnMessage += new agsXMPP.protocol.client.MessageHandler(OnMessage);
                xmppCon.Open();
                connected = true;
            }
            
        }

        private void OnRosterResult(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            //_debug += item.ToString();
            if (item.Name != null) {
                cm.contactList.Add(item.Jid,new Presence(ShowType.NONE,"Offline"));
            }
            
        }

        private void OnRosterEnd(object sender) {
            xmppCon.SendMyPresence();        
        }

        void OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            cm.contactList[pres.From] = pres;
            this.Dispatcher.Invoke((Action)(() =>
            {
                if(pres.Show == ShowType.away)
                    consoleGtalk.Text += String.Format("{0} show:{1} status:{2} \n\n", pres.From.ToString(), pres.Show.ToString(), pres.Status);
            }));
           
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
