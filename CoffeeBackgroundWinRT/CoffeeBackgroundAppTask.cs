using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.System;
using Windows.UI.Notifications;

namespace CoffeeBackgroundWinRT
{
    public sealed class CoffeeBackgroundAppTask : IBackgroundTask
	{
        private Timer timer;
        private const int MB = 1024 * 1024;
        private const int toastInterval = 30000;
        private const int maxLifetime = 30000 * 2 * 3;  // We'll arbitrarily allow this task to run for 3 mins.

        public void Run(IBackgroundTaskInstance taskInstance)
		{
            timer = new Timer(OnTimer, null, 0, toastInterval);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < maxLifetime)
            {
                ; // do nothing, just keep this task alive so the OnTimer continues to execute.
            }
            SendToast("AppTrigger maxLifetime exceeded.");
		}

        private void OnTimer(object state)
        {
            SendToast(GetMemoryUsage());
        }

        private void SendToast(string message)
        {
            try
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(message));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(DateTime.Now.ToString()));

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private string GetMemoryUsage()
        {
            string memoryUsage = string.Empty;
            try
            {
                ulong usageLimit = MemoryManager.AppMemoryUsageLimit;
                ulong usage = MemoryManager.AppMemoryUsage;
                memoryUsage = string.Format(CultureInfo.CurrentCulture, "AppTrigger cap={0}MB, usage={1}MB",
                    Math.Ceiling((double)usageLimit / MB), Math.Ceiling((double)usage / MB));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return memoryUsage;
        }
    }
}
