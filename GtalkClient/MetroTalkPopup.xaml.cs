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
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for MetroTalkPopup.xaml
    /// </summary>
    /// 

    public partial class MetroTalkPopup : MetroWindow
    {
        private DispatcherTimer _timer;

        public MetroTalkPopup(string _message)
        {
            this.Width = 300f;
            this.Height = 150f;
            _timer = new DispatcherTimer();

            // Set the Interval to 2 seconds
            _timer.Interval = TimeSpan.FromMilliseconds(5000);

            // Set the callback to just show the time ticking away
            // NOTE: We are using a control so this has to run on 
            // the UI thread
            _timer.Tick += new EventHandler(delegate(object s, EventArgs a)
            {
                this.Close();
            });

            // Start the timer

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            Console.WriteLine(desktopWorkingArea);
            Console.WriteLine(this.Width + ";" + this.Height);
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            
            InitializeComponent();
            message.Text = _message;
            _timer.Start();
        }
    }
}
