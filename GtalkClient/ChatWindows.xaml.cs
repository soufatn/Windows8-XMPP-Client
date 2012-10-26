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

namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for ChatWindows.xaml
    /// </summary>
    public partial class ChatWindows : Window
    {

        private ContactManager cm;
        public IDictionary<Jid, Presence> list;
        private bool test =false;
        public ChatWindows()
        {
            cm = ContactManager.getInstance();
            list = new Dictionary<Jid, Presence>();

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
                listBox1.ItemsSource = list;
            }
                list.Clear();

            foreach (KeyValuePair<Jid, Presence> contact in cm.contactList)
            {
                
                if (contact.Value.Type == PresenceType.available) {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new ParameterizedThreadStart(addContact), contact);
                }
            }


        }

        void addContact(Object _contact) {
            KeyValuePair<Jid, Presence> c = (KeyValuePair<Jid, Presence>)_contact;
            if (!list.ContainsKey(c.Key))
            {
                list.Add(c);
            }
        }
    }
}
