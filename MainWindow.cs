using System;
using System.Text;
using Eto.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NetworkClipboard
{
	public partial class MainWindow : Form
	{
        private Controller controller;

        public MainWindow(Controller c)
		{
            if (c == null)
            {
                throw new ArgumentNullException();
            }
            controller = c;
            controller.NewPaste += Controller_NewPaste;

			Layout();
		}

        private void Controller_NewPaste (string channel, DateTime timestamp, string text)
        {
            TextArea target = null;
            TabPage targetPage = null;
            foreach (TabPage page in tabs.Pages)
            {
                if (page.Text == channel)
                {
                    target = page.Content as TextArea;
                    targetPage = page;
                }
            }

            if (target == null)
            {
                target = new TextArea();
                targetPage = new TabPage()
                {
                    Text = channel,
                    Content = target
                };
                tabs.Pages.Add(targetPage);
                targetPage = tabs.Pages[0];
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(timestamp.ToString());
            sb.AppendLine("--------------------");
            sb.AppendLine(text);
            sb.AppendLine("--------------------");
            sb.AppendLine();

            target.Append(sb.ToString(), true);
            targetPage.Focus();
            return;
        }

		private void QuitCommand_Executed (object sender, EventArgs e)
		{
			Close();
		}

		private void PasteCommand_Executed (object sender, EventArgs e)
		{
			Clipboard c = new Clipboard();
			if (c.Text == null)
			{
				return;
			}
			string text = c.Text;

            if (text.Length > UdpMessenger.MaxBufferSize)
            {
                MessageBox.Show("Paste is too big. Try something smaller");
                return;
            }

            controller.Paste("default", text);
		}
	}
}

