using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoffeeUniversal.Controls
{
    public sealed partial class StatusControl : UserControl
    {
        public ListView StatusList
        {
            get { return statusList; }
        }

        public Visibility HeaderVisibility
        {
            set
            {
                statusHeader.Visibility = value;
            }
        }

        public StatusControl()
        {
            InitializeComponent();
        }

        public void Log(Control nextControl, string s)
        {
            ShowMessage(s);
            nextControl.Focus(FocusState.Programmatic);
        }

        public void Log(string s)
        {
            ShowMessage(s);
        }

        async private void ShowMessage(string s)
        {
            string message = string.Format("{0}{1}", s, Environment.NewLine);

            if (CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
                statusList.Items.Add(message);
                ScrollToBottom(statusList);
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        statusList.Items.Add(message);
                        ScrollToBottom(statusList);
                    });
            }
        }

        async private void ScrollToBottom(ListView status)
        {
            try
            {
                var child = VisualTreeHelper.GetChild(status, 0);
                if (child != null)
                {
                    int childCount = VisualTreeHelper.GetChildrenCount(child) - 1;
                    for (int i = 0; i <= childCount; i++)
                    {
                        object obj = VisualTreeHelper.GetChild(child, i);
                        if (!(obj is ScrollViewer)) continue;

                        // HACK There's a bug in ScrollViewer.ChangeView, where we must insert a delay before calling 
                        // it, otherwise it doesn't work as documented.
                        await Task.Delay(100);

                        ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DiagnosticsHelper.ScrollToBottom: " + ex.ToString());
            }
        }
    }
}
