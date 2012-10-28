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
        private ObservableCollection<Message> listOfConv;
        private GtalkCommunication gc;
        private bool test =false;
        public ChatWindows()
        {
           
            cm = ContactManager.getInstance();
            listOfContact = new ObservableCollection<UserJabber>();
            listOfConv = new ObservableCollection<Message>();

            InitializeComponent();
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

                    //listBox1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate()
                    //{
                        addContact(keyvalue);
                    //}));
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
        }

        void RefreshConversation()
        {
            listOfConv.Clear();
            UserJabber user = (UserJabber)listBox1.SelectedItem;
            if (cm.conversations.ContainsKey(user.jid.Bare))
            {
                foreach (Message m in cm.conversations[user.jid.Bare])
                {
                    if (m.To.Bare == user.jid.Bare || m.From.Bare == user.jid.Bare)
                    {
                        listOfConv.Add(m);
                    }
                }
            }
        }

        public void Send(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                UserJabber user = (UserJabber)listBox1.SelectedItem;

                gc.Send(user.jid, body.Text);

                body.Text = "";

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
    }
}
