using System;
using Eto.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkClipboard
{
    public class TextInputDialog : Dialog
    {
        private TextBox input;
        private Button okButton;
        private Button cancelButton;
        private string userInput;

        public TextInputDialog()
        {
            input = new TextBox();
            okButton = new Button()
            {
                Text = "Ok"
            };
            okButton.Click += (sender, e) => {
                userInput = input.Text;
                Close();
            };

            cancelButton = new Button()
            {
                Text = "Cancel"
            };
            cancelButton.Click += (sender, e) => {
                Close();
            };

            DefaultButton = okButton;
            AbortButton = cancelButton;

            DynamicLayout root = new DynamicLayout();
            root.BeginVertical();
            root.Add(input);
            root.BeginHorizontal();
            root.Add(okButton);
            root.Add(cancelButton);
            root.EndHorizontal();
            root.EndVertical();

            Content = root;
        }

        public string AskInput()
        {
            ShowModal();

            return userInput;
        }
    }
}

