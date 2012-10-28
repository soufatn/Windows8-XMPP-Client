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
using GtalkClient.Properties;
using System.Media;

namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //TODO deplacer le gestionnaire agsXMPP to GtalkCommunication
        
        private ContactManager cm;
        private GtalkCommunication gC;
        private ChatWindows c;


        
        public MainWindow()
        {
            InitializeComponent();
            cm = ContactManager.getInstance();
            c = new ChatWindows();
            gC = new GtalkCommunication(this,c);
            c.SetGC(gC);

            email.Text = (string)Settings.Default["email"];
            password.Password = (string)Settings.Default["password"];
        }

        public void Connect(object sender, RoutedEventArgs e)
        {

            if (email.Text.Contains("@"))
            {
                Console.WriteLine(email.Text);
                gC.UserJid = new Jid(email.Text);
            } else {
                Console.WriteLine(email.Text);
                gC.UserJid = new Jid(email.Text + "@gmail.com");
            }
            Console.WriteLine(email.Text);
            bool isCheck = (bool) save.IsChecked;
            gC.Connect(password.Password, isCheck);    

        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                gC.UserJid = new Jid(email.Text + "@gmail.com");
                bool isCheck = (bool)save.IsChecked;
                gC.Connect(password.Password, isCheck);      
            }
        }

        public void showChat() {
            this.Dispatcher.Invoke(new Action(delegate() {
                this.Hide();
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

        }
    }
}
