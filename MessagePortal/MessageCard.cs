using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessagePortal
{
    public partial class MessageCard : UserControl
    {
        public event EventHandler MessageCardCompleted;
        public MessageCard()
        {
            InitializeComponent();
        }

        protected virtual void OnMessageCardCompleted(EventArgs e)
        {
            EventHandler handler = this.MessageCardCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [Browsable(true)]
        public new string Text
        {
            get => label1.Text;
            set => label1.Text = value;
        }

        [Browsable(true)]
        public bool IsEnabled
        {
            get => button2.Enabled;
            set => button2.Enabled = value;
        }

        [Browsable(true)]
        public Color Color
        {
            get => label1.ForeColor;
            set => label1.ForeColor = value;
        }
        public string SessionId { get; internal set; }

        private void button2_Click(object sender, EventArgs e)
        {
            this.OnMessageCardCompleted(EventArgs.Empty);
        }
    }
}
