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
using System.Windows.Shapes;
using agsXMPP;
using agsXMPP.protocol.client;
using System.Windows.Threading;
using System.Threading;
using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for ChatWindows.xaml
    /// </summary>
    public partial class ChatWindows : MetroWindow
    {

        private ContactManager cm;
        private ObservableCollection<UserJabber> listOfContact;
        private ObservableCollection<MetroTalkMessage> listOfConv;
        private GtalkCommunication gc;
        private UserJabber userSelected;
        private bool test =false;
        bool isApplicationActive;


        public ChatWindows()
        {
           
            cm = ContactManager.getInstance();
            listOfContact = new ObservableCollection<UserJabber>();
            listOfConv = new ObservableCollection<MetroTalkMessage>();

            InitializeComponent();
        }

        public void SetGC(GtalkCommunication _gc)
        {
            gc=_gc;
        }

        public void Refresh() {
            this.Dispatcher.Invoke(new Action(delegate()
            {
                RefreshList();
            }));
        }
        public void RefreshList()
        {
            if (!test) {
                test = true;
                listBox1.ItemsSource = listOfContact;
                listBoxConv.ItemsSource = listOfConv;
            }

            listOfContact.Clear();
            Console.WriteLine("------------------------PresenceList");
            foreach (KeyValuePair<string, Presence> contact in cm.PresenceList)
            {

                if (cm.users.ContainsKey(contact.Key))
                {
                    UserJabber user = cm.users[contact.Key];
                    Console.WriteLine(user.FullName);
                    KeyValuePair<UserJabber, Presence> keyvalue = new KeyValuePair<UserJabber, Presence>(user, contact.Value);

                    listBox1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate()
                    {
                        addContact(keyvalue);
                    }));
                }
                    
                
            }
            Console.WriteLine("------------------------LIST");
            foreach (var contact in listOfContact)
            {
                Console.WriteLine(contact.FullName);
            }

        }

        public void OnSelect(object sender, RoutedEventArgs e)
        {
            RefreshConversation();
            body.IsEnabled = true;
            body.Text = "";
            userSelected = (UserJabber)listBox1.SelectedItem;
        }
        void App_Activated(object sender, EventArgs e)
        {
            // Application activated 
            this.isApplicationActive = true;
            Console.WriteLine("Active");
        }

        void App_Deactivated(object sender, EventArgs e)
        {
            // Application deactivated 
            this.isApplicationActive = false;
            Console.WriteLine("Desactive");
        }

        [STAThread]
        public void newMessage(MetroTalkMessage m) {
            if (!this.isApplicationActive || m.From.Bare != userSelected.jid.Bare)
            {
                UserJabber u = cm.users[m.From.Bare];
                string s = u.FullName + " : " + m.Body;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate() {
                    MetroTalkPopup p = new MetroTalkPopup(s);
                    p.Show();
                }));

            }

            RefreshConversation();

        }

        public void RefreshConversation()
        {
            if (userSelected != null)
            {
                listBoxConv.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate()
                {
                    listOfConv.Clear();

                    if (cm.conversations.ContainsKey(gc.UserJid.Bare))
                    {
                        foreach (MetroTalkMessage m in cm.conversations[gc.UserJid.Bare])
                        {
                            Console.WriteLine(m);
                            if (m.To.Bare == userSelected.jid.Bare || m.From.Bare == userSelected.jid.Bare)
                            {
                                Console.WriteLine(m.From + " : " + m.Body);
                                listOfConv.Add(m);
                            }
                        }
                    }
                    ScrollToEnd();
                }));
            }

        }

        public void ScrollToEnd()
        {
            Console.WriteLine("Scroll" + (listBoxConv.Items.Count));
            if (listBoxConv.Items.Count > 0)
            {
                Console.WriteLine("Scroll"+(listBoxConv.Items.Count - 1));
                listBoxConv.ScrollIntoView(listBoxConv.Items[listBoxConv.Items.Count - 1]);
            }
            
        }

        public void Send(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if (userSelected != null)
                {
                    gc.Send(userSelected.jid, body.Text);

                    body.Text = "";
                }

            }
        }

        public void Actu(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("------------------------LIST");
            foreach (var contact in listOfContact)
            {
                Console.WriteLine(contact.FullName);
            }

            listBox1.ItemsSource = listOfContact;
        }
        void addContact(Object _contact) {
            KeyValuePair<UserJabber, Presence> c = (KeyValuePair<UserJabber, Presence>)_contact;
            if (!listOfContact.Contains(c.Key))
            {
                listOfContact.Add(c.Key);
            }
        }

        private void listBoxConv_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
