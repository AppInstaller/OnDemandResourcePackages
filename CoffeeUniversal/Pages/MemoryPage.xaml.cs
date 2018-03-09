using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class MemoryPage : Page
    {

        #region Init

        private const int MB = 1024 * 1024;
        private ulong allocationSize;
		private List<byte[]> consumedMemory;
		private NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public MemoryPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

			MemoryManager.AppMemoryUsageIncreased += OnAppMemoryUsageIncreased;
			MemoryManager.AppMemoryUsageDecreased += OnAppMemoryUsageDecreased;
            MemoryManager.AppMemoryUsageLimitChanging += OnAppMemoryUsageLimitChanging;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

			if (e.NavigationMode == NavigationMode.New)
			{
				consumedMemory = new List<byte[]>();
			}
			GetUsage();
			GetUsageLevel();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
			if (e.NavigationMode == NavigationMode.Back)
			{
				consumedMemory.Clear();
				consumedMemory = null;
                GC.Collect();
            }
			navigationHelper.OnNavigatedFrom(e);
        }

		#endregion


		#region consume/release

		private void consume_Click(object sender, RoutedEventArgs e)
		{
			try
			{
                allocationSize = ulong.Parse(mb.Text) * MB;
				consumedMemory.Add(new byte[allocationSize]);
			}
			catch (OutOfMemoryException)
			{
				status.Log("OOM");
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}

			GetUsage();
		}

		private void release_Click(object sender, RoutedEventArgs e)
		{
			if (consumedMemory.Count > 0)
			{
				consumedMemory.RemoveAt(0);
			}
            GC.Collect();
            GetUsage();
		}

		#endregion


		#region Memory notifications

		private void OnAppMemoryUsageIncreased(object sender, object e)
		{
			status.Log("AppMemoryUsageIncreased");
			GetUsageLevel();
		}

		private void OnAppMemoryUsageDecreased(object sender, object e)
		{
			status.Log("AppMemoryUsageDecreased");
			GetUsageLevel();
		}

        private async void OnAppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            status.Log("AppMemoryUsageLimitChanging");
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                GetUsage();
            });
        }

        #endregion


        #region Get UsageLevel

        private async void GetUsageLevel()
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				try
				{
					AppMemoryUsageLevel usageLevel = MemoryManager.AppMemoryUsageLevel;
					usageLevelText.Text = usageLevel.ToString();
					SolidColorBrush brush = null;
					switch (usageLevel)
					{
						case AppMemoryUsageLevel.High:
							brush = new SolidColorBrush(Colors.Red);
							break;
						case AppMemoryUsageLevel.Medium:
							brush = new SolidColorBrush(Colors.Orange);
							break;
						case AppMemoryUsageLevel.Low:
							brush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
							break;
					}
					usageText.Foreground = brush;
					usageLevelText.Foreground = brush;
				}
				catch (Exception ex)
				{
					status.Log(ex.Message);
				}
			});
		}

        #endregion


        #region Get Usage

        private void GetUsage()
		{
            try
            {
                GetCoreMemoryUsage();
            }
            catch (Exception ex)
            {
                status.Log(ex.ToString());
            }

            try
            {
                GetAppMemoryReport();
            }
            catch (Exception ex)
            {
                status.Log(ex.ToString());
            }

            try
            {
                GetProcessMemoryReport();
            }
            catch (Exception ex)
            {
                status.Log(ex.ToString());
            }

            try
            {
                GetProcessDiagnostics();
            }
            catch (Exception ex)
            {
                status.Log(ex.ToString());
            }
        }

        private void GetCoreMemoryUsage()
		{
			try
			{
				ulong usageLimit = MemoryManager.AppMemoryUsageLimit;
				ulong usage = MemoryManager.AppMemoryUsage;
				//double headroom = (double)(usageLimit - usage) / MB;

				usageLimitText.Text = string.Format(CultureInfo.CurrentCulture, "{0} MB", Math.Ceiling((double)usageLimit / MB));
				usageText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)usage / MB);
				//headroomText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", headroom);
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}

		private void GetAppMemoryReport()
		{
			try
			{
				AppMemoryReport appReport = MemoryManager.GetAppMemoryReport();
                ulong privateCommit = appReport.PrivateCommitUsage;
                ulong peakPrivate = appReport.PeakPrivateCommitUsage;
                ulong totalCommit = appReport.TotalCommitUsage;
                ulong commitLimit = appReport.TotalCommitLimit;

                commitLimitText.Text = string.Format(CultureInfo.CurrentCulture, "{0} MB", Math.Ceiling((double)commitLimit / MB));
                privateCommitText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)privateCommit / MB);
                peakPrivateText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)peakPrivate / MB);
                totalCommitText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)totalCommit / MB);
            }
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}

		private void GetProcessMemoryReport()
		{
			try
			{
                ProcessMemoryReport processReport = MemoryManager.GetProcessMemoryReport();
                ulong privateSet = processReport.PrivateWorkingSetUsage;
                ulong totalSet = processReport.TotalWorkingSetUsage;

                privateSetText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)privateSet / MB);
                totalSetText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)totalSet / MB);
            }
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}

		#endregion


		#region GetProcessDiagnostics

		private void GetProcessDiagnostics()
		{
			try
			{
                ProcessDiagnosticInfo info = ProcessDiagnosticInfo.GetForCurrentProcess();

                //ProcessCpuUsageReport cpuReport = info.CpuUsage.GetReport();
                //TimeSpan kernelTime = cpuReport.KernelTime;
                //TimeSpan userTime = cpuReport.UserTime;

                ProcessMemoryUsageReport memoryReport = info.MemoryUsage.GetReport();
                //ulong nonPagedPool = memoryReport.NonPagedPoolSizeInBytes;
                //ulong pagedPool = memoryReport.PagedPoolSizeInBytes;
                //ulong peakNonPagedPool = memoryReport.PeakNonPagedPoolSizeInBytes;
                //ulong peakPagedPool = memoryReport.PeakPagedPoolSizeInBytes;

                ulong virtualMemory = memoryReport.VirtualMemorySizeInBytes;
                ulong workingSet = memoryReport.WorkingSetSizeInBytes;
                //ulong peakVirtualMemory = memoryReport.PeakVirtualMemorySizeInBytes;
                //ulong peakWorkingSet = memoryReport.PeakWorkingSetSizeInBytes;

                //ulong pageFaults = memoryReport.PageFaultCount;
                //ulong pageFile = memoryReport.PageFileSizeInBytes;
                //ulong privatePages = memoryReport.PrivatePageCount;
                //ulong peakPageFile = memoryReport.PeakPageFileSizeInBytes;

                virtualMemoryText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)virtualMemory / MB);
                workingSetText.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", (double)workingSet / MB);
            }
            catch (Exception ex)
			{
				status.Log(ex.Message);
			}
        }

		#endregion

	}

    public enum AllocationSize { Large, Medium, Small }

}
