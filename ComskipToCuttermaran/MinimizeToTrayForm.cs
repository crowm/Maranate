using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Maranate
{
    public class MinimizeToTrayForm : SavedLocationForm
    {
        private FormWindowState _previousWindowState = FormWindowState.Normal;
        private NotifyIcon _notifyIcon;
        private bool _loaded = false;

        //protected override void WndProc(ref Message message)
        //{
        //    if (message.Msg == Utils.SingleInstance.WM_SHOWFIRSTINSTANCE)
        //    {
        //        ShowWindow();
        //    }
        //    base.WndProc(ref message);
        //}

        public void ShowWindow()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                RestoreFromTray();
            }
            else
            {
                Utils.Win32.ShowToFront(this.Handle);
            }
        }

        public NotifyIcon NotifyIcon
        {
            get { return _notifyIcon; }
            set
            {
                if (_notifyIcon != null)
                    _notifyIcon.MouseDoubleClick -= new MouseEventHandler(NotifyIcon_MouseDoubleClick);
                _notifyIcon = value;
                if (_notifyIcon != null)
                    _notifyIcon.MouseDoubleClick += new MouseEventHandler(NotifyIcon_MouseDoubleClick);
            }
        }

        public MinimizeToTrayForm()
        {
            _previousWindowState = FormWindowState.Normal;
            NotifyIcon = new NotifyIcon();
            NotifyIcon.Icon = this.Icon;
            NotifyIcon.Visible = true;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            NotifyIcon.Text = this.Text;
            NotifyIcon.BalloonTipTitle = this.Text;
            base.OnTextChanged(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            _loaded = true;
            NotifyIcon.BalloonTipTitle = this.Text;
            NotifyIcon.BalloonTipText = this.Text;
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            NotifyIcon.Dispose();
            NotifyIcon = null;
            base.OnClosed(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (this._loaded == true)
            {

                if (this.WindowState == FormWindowState.Minimized)
                {
                    SendToTray();
                }
                else
                {
                    _previousWindowState = this.WindowState;
                    RestoreFromTray();
                }
            }
            base.OnResize(e);
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this._loaded == true)
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    _previousWindowState = this.WindowState;
                    SendToTray();
                }
                else
                {
                    RestoreFromTray();
                }
            }
        }

        protected void SendToTray()
        {
            if (_notifyIcon == null)
                return;

            if (Settings.Current.MinimiseToTray == false)
                return;

            if (this.WindowState != FormWindowState.Minimized)
                this.WindowState = FormWindowState.Minimized;
            this.Hide();
            // Do not use ShowInTaskbar = false. It kills the WM_INPUT messages.
            //this.ShowInTaskbar = false;
        }

        protected void RestoreFromTray()
        {
            if (_notifyIcon == null)
                return;

            //this.ShowInTaskbar = true;
            this.Show();
            this.WindowState = _previousWindowState;
        }

    }
}
