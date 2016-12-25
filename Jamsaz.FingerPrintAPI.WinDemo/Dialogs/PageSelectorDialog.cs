using System;
using System.Windows.Forms;

namespace Jamsaz.FingerPrintAPI.WinDemo.Dialogs
{
    public partial class PageSelectorDialog : Form
    {
        public int PageId;
        public PageSelectorDialog()
        {
            InitializeComponent();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PageIdTextBox.Text))
            {
                MessageBox.Show(@"Please enter page Id", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            PageId = int.Parse(PageIdTextBox.Text.Trim());
            if (PageId > 15 || PageId < 0)
            {
                MessageBox.Show(@"Please enter a number between 0 and 15");
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
