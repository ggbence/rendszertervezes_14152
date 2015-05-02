using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using unoClient;

namespace unoClientForm
{
    public partial class form_main : Form
    {
        private ClientGame clientGame;

        private delegate void WriteLineCallBack(string message);

        private delegate void WriteCallBack(string message);

        private delegate void ClearCallBack();

        public form_main()
        {
            InitializeComponent();
            clientGame = ClientGame.Instance;
            var cm = ConnectionManager.Instance;
            cm.ConnectToServer();
            ClientGame.Instance.Start(this);
        }

        public void WriteLine(string message="")
        {
            if (text_console.InvokeRequired)
            {
                WriteLineCallBack d = new WriteLineCallBack(WriteLine);
                this.Invoke(d, new object[] { message });

            }
            else
                text_console.AppendText(message.ToString() + Environment.NewLine);
        }

        public void WriteLine(int message)
        {
            if (text_console.InvokeRequired)
            {
                WriteLineCallBack d = new WriteLineCallBack(WriteLine);
                this.Invoke(d, new object[] {message.ToString()});

            }
            else
            {
                text_console.AppendText(message.ToString()+Environment.NewLine);
            }
        }

        public void Write(string message = "")
        {
            if (text_console.InvokeRequired)
            {
                WriteCallBack d = new WriteCallBack(WriteLine);
                this.Invoke(d, new object[] { message });

            }
            else
                text_console.AppendText(message);
        }

        public void Clear()
        {
            if (text_console.InvokeRequired)
            {
                ClearCallBack d=new ClearCallBack(Clear);
                this.Invoke(d);
            }
            else
            {
                text_console.Clear();
            }
        }

        private void but_send_Click(object sender, EventArgs e)
        {
            clientGame.Input(text_command.Text);
            text_command.Clear();
        }

        private void text_command_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                clientGame.Input(text_command.Text);
                text_command.Clear();
            }
        }

     
    }
}
