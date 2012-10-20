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
        XmppClientConnection xmppCon = new XmppClientConnection();
        private bool connected = false;
        private string _debug = "";
        public MainWindow()
        {
            InitializeComponent();
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
                //xmppCon.OnRosterEnd += new ObjectHandler(xmppCon_OnRosterEnd);
                xmppCon.OnRosterItem += new agsXMPP.XmppClientConnection.RosterHandler(OnRosterResult);
                xmppCon.OnRosterEnd += new ObjectHandler(OnRosterEnd);
                xmppCon.Open();
                connected = true;
            }
            
        }

        private void OnRosterResult(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            _debug += item.Jid.User.ToString()+"\n";
        }

        private void OnRosterEnd(object sender) {
            Console.WriteLine(_debug);
        }

        private void cmdSend_Click()
        {
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = new Jid("");
            msg.Body = "";

            xmppCon.Send(msg);
        }
	
    }
}
