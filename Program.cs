using System;
using Eto.Forms;

namespace NetworkClipboard
{
	public static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application().Run(new MainWindow());
		}
	}
}

