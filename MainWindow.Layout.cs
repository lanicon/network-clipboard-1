using System;
using Eto.Forms;
using Eto.Drawing;

namespace NetworkClipboard
{
    public partial class MainWindow
    {
        private Command pasteCommand;
        private Command quitCommand;
        private TabControl tabs;

        private void Layout()
        {
            Title = "Network clipboard";
            MinimumSize = new Size(300, 400);

            pasteCommand = new Command();
            pasteCommand.MenuText = "&Paste";
            pasteCommand.Shortcut = Application.Instance.CommonModifier | Keys.V;
            pasteCommand.Executed += PasteCommand_Executed;

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
                            quitCommand
                        }
                    }
                }
            };
            
            TabPage plusPage = new TabPage();
            plusPage.Text = "+";
            plusPage.Click += PlusPage_Click;

            tabs = new TabControl();
            tabs.Pages.Add(new TabPage()
            {
                Text = "default",
                Content = ReadOnlyTextArea()
            });
            tabs.Pages.Add(plusPage);
                
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

