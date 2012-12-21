using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ComskipToCuttermaran
{
    public partial class ErrorForm : BaseForm
    {
        public static void Show(Form owner, string message, string title)
        {
            var dialog = new ErrorForm();
            dialog.Message = message;
            dialog.Text = title;

            dialog.ShowDialog(owner);
        }

        public string Message
        {
            get
            {
                return richTextBoxEx1.Text;
            }
            set
            {
                richTextBoxEx1.Text = value;
            }
        }

        public ErrorForm()
        {
            InitializeComponent();
        }
    }
}
