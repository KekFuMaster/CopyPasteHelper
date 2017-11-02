using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyPasteHelper
{
    public class ClipboardManager
    {
        private static ClipboardManager instance;

        private ClipboardManager() { }

        public static ClipboardManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClipboardManager();
                }

                return instance;
            }
        }

        #region ClipboardSpecSwitcher

        private ClipboardSpecSwitcher clipboardSpecSwitcher;

        /// <summary>
        /// Switch symbols
        /// </summary>
        public ClipboardSpecSwitcher ClipboardSpecSwitcher
        {
            get
            {
                if (clipboardSpecSwitcher == null)
                {
                     clipboardSpecSwitcher = new ClipboardSpecSwitcher();
                }

                return clipboardSpecSwitcher;
            }
        }
        #endregion
    }
}
