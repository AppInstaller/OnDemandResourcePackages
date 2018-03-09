using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace CoffeeUniversal.Helpers
{
	public class BackgroundTaskHelper
	{
        public static async Task<bool> RegisterTimeZoneChangeBackgroundTask(string name, string entryPoint)
        {
            bool result = false;
            try
            {
                UnregisterBackgroundTasks();

                BackgroundAccessStatus accessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (accessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                    accessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
                    BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                    builder.Name = name;
                    builder.TaskEntryPoint = entryPoint;
                    builder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
                    builder.Register();
                    result = true;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return result;
        }

        public static async Task<bool> RegisterApplicationTriggerBackgroundTask(string name, string entryPoint)
        {
            bool result = false;

            try
            {
                UnregisterBackgroundTasks();

                BackgroundAccessStatus accessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (accessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                    accessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
                    ApplicationTrigger trigger = new ApplicationTrigger();
                    if (trigger != null)
                    {
                        BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                        builder.Name = name;
                        builder.TaskEntryPoint = entryPoint;
                        builder.SetTrigger(trigger);
                        builder.Register();

                        ApplicationTriggerResult triggerResult = await trigger.RequestAsync();
                        if (triggerResult == ApplicationTriggerResult.Allowed)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return result;
        }

        public static void UnregisterBackgroundTasks()
		{
			foreach (var task in BackgroundTaskRegistration.AllTasks)
			{
				task.Value.Unregister(true);
			}
		}

	}
}
