using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Programacion123
{
    public class LongTaskController
    {
        public  Window? Owner { get { return dialogsOwner; } set { dialogsOwner = value; } }

        UIElement? blocker;
        Window? dialogsOwner;

        public void Init(UIElement? _blocker, Window? _dialogsOwner = null)
        {
            blocker = _blocker;
            dialogsOwner = _dialogsOwner;
        }

        async public Task ExecuteAsync(string title, Action action, float minWaitTime)
        {
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            LongTaskDialog dialog = new();
            if(dialogsOwner != null)  { dialog.Owner = dialogsOwner; }

            dialog.Init(title);
            Stopwatch timer = new();
            timer.Start();
            dialog.Show();
            Task task = new(action);
            task.Start();
            await task;
            timer.Stop();
            int minTime = (int)(Constants.validationTaskMinDuration * 1000);
            if(timer.ElapsedMilliseconds < minTime) { await Task.Delay((int)(minTime - timer.ElapsedMilliseconds)); }
            dialog.Hide();
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }
        }

        async public Task<TResult> ExecuteAsync<TResult>(string title, Func<TResult> function, float minWaitTime)
        {
            TResult result;
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            LongTaskDialog dialog = new();
            if(dialogsOwner != null)  { dialog.Owner = dialogsOwner; }
            else { dialog.Owner = MainWindow.Instance; }
            dialog.Init(title);
            Stopwatch timer = new();
            timer.Start();
            dialog.Show();
            Task<TResult> task = new(function);
            task.Start();
            result = await task;
            timer.Stop();
            int minTime = (int)(Constants.validationTaskMinDuration * 1000);
            if(timer.ElapsedMilliseconds < minTime) { await Task.Delay((int)(minTime - timer.ElapsedMilliseconds)); }
            dialog.Hide();
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            return result;

        }
    }
}
