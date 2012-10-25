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

namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for ChatWindows.xaml
    /// </summary>
    public partial class ChatWindows : Window
    {

        private ContactManager cm;

        public ChatWindows()
        {
            cm = ContactManager.getInstance();

            InitializeComponent();
        }

        public void RefreshList()
        {

        }
    }
}
