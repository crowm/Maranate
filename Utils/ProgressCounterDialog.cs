using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utils
{
    public class ProgressCounterDialog
    {
        public Action WorkerAction { get; set; }
        public bool Cancelled { get; set; }

        public ProgressCounterDialog()
        {
            Cancelled = false;
        }
        public ProgressCounterDialog(Action workerAction)
            : this()
        {
            WorkerAction = workerAction;
        }

        public void ShowDialog(System.Windows.Forms.Control owner, string text, string caption, Utils.ProgressDialog.Animations animation = ProgressDialog.Animations.None)
        {
            Exception workerException = null;
            var workerThread = new System.Threading.Thread(() =>
            {
                try
                {
                    WorkerAction.Invoke();
                }
                catch (Exception e)
                {
                    workerException = e;
                }
            });
            workerThread.Name = "ProgressCounterDialog worker";

            var dialogUpdateEvent = new AutoResetEvent(false);
            var dialogCloseEvent = new ManualResetEvent(false);

            using (var workerProgress = new ProgressCounter(workerThread, 1))
            {
                workerProgress.StatusChanged += (List<string> status) =>
                {
                    dialogUpdateEvent.Set();
                };
                workerProgress.ProgressChanged += (double percent) =>
                {
                    dialogUpdateEvent.Set();
                };

                workerThread.Start();

                var dialog = new Utils.ProgressDialog();
                dialog.Title = caption;
                dialog.Line1 = text;
                dialog.Line2 = workerProgress.Status;
                dialog.Animation = animation;
                dialog.CancelMessage = "Please wait while the operation is cancelled";
                dialog.Maximum = 100;
                dialog.Value = (uint)(workerProgress.GetProgress() * dialog.Maximum);
                dialog.Modal = true;
                dialog.NoTime = true;

                IntPtr handle = IntPtr.Zero;
                if (owner != null)
                {
                    if (owner.InvokeRequired)
                        owner.Invoke(new Action(delegate { handle = owner.Handle; }));
                    else
                        handle = owner.Handle;
                }

                dialog.ShowDialog(handle);

                while (workerThread.IsAlive)
                {
                    if (dialog.HasUserCancelled)
                    {
                        workerProgress.Abort();
                        Cancelled = true;
                        break;
                    }

                    int waitResult = WaitHandle.WaitAny(new WaitHandle[] { dialogUpdateEvent, dialogCloseEvent }, 50);
                    if (waitResult == WaitHandle.WaitTimeout)
                        continue;
                    if (waitResult == 1) // dialogCloseEvent
                        break;

                    var statusList = workerProgress.GetStatusList();
                    if (statusList.Count >= 1)
                    {
                        dialog.Line2 = statusList[0];
                        if (statusList.Count >= 2)
                            dialog.Line3 = statusList[1];
                        else
                            dialog.Line3 = "";
                    }
                    else
                    {
                        dialog.Line2 = "";
                    }
                    dialog.Value = (uint)(workerProgress.GetProgress() * dialog.Maximum);
                }

                dialog.CloseDialog();
                dialog = null;

                if (workerException != null)
                {
                    throw new Exception(workerException.Message, workerException);
                }

                if (owner != null)
                {
                    owner.Invoke(new Action(delegate
                    {
                        var form = owner as System.Windows.Forms.Form;
                        if (form != null)
                            form.Activate();
                        else
                            owner.Focus();
                    }));
                }


            }
        }

    }
}
