using System;
using Eto.Forms;
using Eto.Drawing;

namespace NetworkClipboard
{
    public partial class MainWindow
    {
        private Command pasteCommand;
        private Command quitCommand;
        private Command closeCommand;
        private TabControl tabs;

        private void Layout()
        {
            Title = "Network clipboard";
            MinimumSize = new Size(300, 400);
            Closing += MainWindow_Closing;

            pasteCommand = new Command();
            pasteCommand.MenuText = "&Paste";
            pasteCommand.Shortcut = Application.Instance.CommonModifier | Keys.V;
            pasteCommand.Executed += PasteCommand_Executed;

            closeCommand = new Command()
            {
                    MenuText = "&Close",
                    Shortcut = Application.Instance.CommonModifier | Keys.W
            };
            closeCommand.Executed += CloseCommand_Executed;

            quitCommand = new Command();
            quitCommand.MenuText = "&Quit";
            quitCommand.Shortcut = Application.Instance.CommonModifier | Keys.Q;
            quitCommand.Executed += QuitCommand_Executed;

            Menu = new MenuBar()
            {
                Items = 
                {
                    new ButtonMenuItem()
                    {
                        Text = "&File",
                        Items = 
                        {
                            pasteCommand,
                            closeCommand,
                            quitCommand
                        }
                    }
                }
            };
            
            TabPage plusPage = new TabPage();
            plusPage.Text = "+";
            plusPage.Click += PlusPage_Click;

            tabs = new TabControl();
            tabs.Pages.Add(plusPage);
            foreach (string s in Program.Config.Channels)
            {
                AddNewChannel(s);
            }

            Content = tabs;
        }

        private TextArea ReadOnlyTextArea()
        {
            TextArea output = new TextArea();
            output.ReadOnly = true;
            return output;
        }
    }
}

