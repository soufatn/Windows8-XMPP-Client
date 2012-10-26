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
using MahApps.Metro.Controls;

namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //TODO deplacer le gestionnaire agsXMPP to GtalkCommunication
        
        XmppClientConnection xmppCon = new XmppClientConnection();
        private bool connected = false;
        private string _debug = "";
        private ContactManager cm;
        private GtalkCommunication gC;
        ChatWindows c;

        
        public MainWindow()
        {
            InitializeComponent();
            cm = ContactManager.getInstance();
            c = new ChatWindows();
            gC = new GtalkCommunication(this,c);
        }

        public void Connect(object sender, RoutedEventArgs e)
        {
            gC.UserJid = new Jid(email.Text);
            gC.Connect(password.Password);         
        }

        public void showChat() {
            this.Dispatcher.Invoke(new Action(delegate() {
                c.Show();
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
    }
}
