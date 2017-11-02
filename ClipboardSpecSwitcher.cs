using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CopyPasteHelper
{
    public class ClipboardSpecSwitcher
    {

        public string ConfigFilePath;
        private Dictionary<string, string> dictionary;
        private DateTime lastWriteTimeUtc;

        public ClipboardSpecSwitcher()
        {
            this.InitDefaults();
        }

        public ClipboardSpecSwitcher(string configFilePath) : base()
        {
            this.ConfigFilePath = configFilePath;
        }

        public void OpenConfigFile()
        {
            if (IsConfigFileExists())
            {
                Process.Start(this.ConfigFilePath);
            }
        }

        public void SwitchClipboard()
        {
            if (Clipboard.ContainsText())
            {
                RichTextBox richTextBox = new RichTextBox();
                string rtf = Clipboard.GetText(TextDataFormat.Rtf);
                if (string.IsNullOrEmpty(rtf))
                {
                    return;
                }

                richTextBox.Rtf = rtf;
                richTextBox.SelectAll();
                int textLength = richTextBox.SelectedText.Length;
                for (int i = textLength; i >= 0; --i)
                {
                    richTextBox.Select(i, 1);
                    if (richTextBox.SelectionFont == null)
                    {
                        continue;
                    }
                    else if (richTextBox.SelectionFont.Style == FontStyle.Strikeout
                        || richTextBox.SelectionFont.Style == (FontStyle.Strikeout | FontStyle.Italic))
                    {
                        richTextBox.SelectedText = string.Empty;
                    }
                    else if (richTextBox.SelectionFont.Name == "Wingdings")
                    {
                        richTextBox.Select(i, 20);
                        var selectedText = richTextBox.SelectedText;
                        int index = selectedText.IndexOf(" ") + 1;
                        string withoutTopicNumber = selectedText.Substring(index);
                        richTextBox.SelectedText = withoutTopicNumber;
                    }
                }

                var clipboardText = richTextBox.Text;
                var configDictionary = GetConfigFile();
                foreach (var character in configDictionary)
                {
                    if (clipboardText.Contains(character.Key))
                    {
                        clipboardText = clipboardText.Replace(character.Key, character.Value);
                    }
                }

                Clipboard.Clear();
                if (!string.IsNullOrEmpty(clipboardText))
                {
                    Clipboard.SetText(clipboardText, TextDataFormat.Text);
                }
            }
        }

        private Dictionary<string, string> GetConfigFile()
        {
            if (IsConfigFileExists())
            {
                if (IsConfigFileChanged())
                {
                    this.dictionary = new Dictionary<string, string>();
                    using (var reader = new StreamReader(this.ConfigFilePath))
                    {
                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] lineSplited = line.Split(',');
                            if (lineSplited.Length >= 2)
                            {
                                this.dictionary.Add(lineSplited[0], lineSplited[1]);
                            }
                        }
                    }

                    this.lastWriteTimeUtc = DateTime.UtcNow;
                    File.SetLastWriteTimeUtc(this.ConfigFilePath, this.lastWriteTimeUtc);
                }
            }

            return dictionary;
        }

        private void InitDefaults()
        {
            this.ConfigFilePath = "config.txt";
            this.dictionary = new Dictionary<string, string>();
            this.lastWriteTimeUtc = DateTime.UtcNow;
        }

        private bool IsConfigFileExists()
        {
            return File.Exists(this.ConfigFilePath);
        }

        private bool IsConfigFileChanged()
        {
            return File.GetLastWriteTimeUtc(this.ConfigFilePath) != lastWriteTimeUtc;
        }
    }
}
