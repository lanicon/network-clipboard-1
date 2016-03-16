using System;
using System.Text;
using Eto.Forms;
using System.Threading.Tasks;

namespace NetworkClipboard
{
	public partial class MainWindow : Form
	{
		private Broadcaster nClipboard;

		public MainWindow()
		{
			nClipboard = new Broadcaster();
			nClipboard.NewPaste += NClipboard_NewPaste;
            nClipboard.RequestHistory("").ContinueWith(HistoryReceived);

			Layout();
		}

        private void HistoryReceived(Task<string> history)
        {
            if (history.Result != "")
            {
                textBox.Text = history.Result;
            }
        }

		private void NClipboard_NewPaste (string text)
		{
			StringBuilder sb = new StringBuilder(textBox.Text);

			sb.Append(DateTime.Now.ToString());
			sb.Append(Environment.NewLine);
			for (int i = 0; i < 20; i++)
			{
				sb.Append("-");
			}
			sb.Append(Environment.NewLine);
			sb.Append(text);
			sb.Append(Environment.NewLine);
			for (int i = 0; i < 20; i++)
			{
				sb.Append("-");
			}
			sb.Append(Environment.NewLine);
			sb.Append(Environment.NewLine);

			textBox.Append(sb.ToString(), true);
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

            if (text.Length > nClipboard.MaxBufferSize)
			{
				MessageBox.Show("Paste size too big. Try something smaller");
				return;
			}

			nClipboard.Paste(text);
		}
	}
}

