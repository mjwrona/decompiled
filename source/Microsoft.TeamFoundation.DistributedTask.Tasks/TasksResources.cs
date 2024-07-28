// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.TasksResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  internal static class TasksResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (TasksResources), typeof (TasksResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => TasksResources.s_resMgr;

    private static string Get(string resourceName) => TasksResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? TasksResources.Get(resourceName) : TasksResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) TasksResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? TasksResources.GetInt(resourceName) : (int) TasksResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) TasksResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? TasksResources.GetBool(resourceName) : (bool) TasksResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => TasksResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = TasksResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string AgentConnectedSuccessfully(object arg0) => TasksResources.Format(nameof (AgentConnectedSuccessfully), arg0);

    public static string AgentConnectedSuccessfully(object arg0, CultureInfo culture) => TasksResources.Format(nameof (AgentConnectedSuccessfully), culture, arg0);

    public static string AgentConnectionTimeout(object arg0) => TasksResources.Format(nameof (AgentConnectionTimeout), arg0);

    public static string AgentConnectionTimeout(object arg0, CultureInfo culture) => TasksResources.Format(nameof (AgentConnectionTimeout), culture, arg0);

    public static string AgentNotAssignedRequestTimeout(object arg0) => TasksResources.Format(nameof (AgentNotAssignedRequestTimeout), arg0);

    public static string AgentNotAssignedRequestTimeout(object arg0, CultureInfo culture) => TasksResources.Format(nameof (AgentNotAssignedRequestTimeout), culture, arg0);

    public static string FailedProvisiongTooManyTimes(object arg0, object arg1) => TasksResources.Format(nameof (FailedProvisiongTooManyTimes), arg0, arg1);

    public static string FailedProvisiongTooManyTimes(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TasksResources.Format(nameof (FailedProvisiongTooManyTimes), culture, arg0, arg1);
    }

    public static string PoolProviderDelayingProvision(object arg0) => TasksResources.Format(nameof (PoolProviderDelayingProvision), arg0);

    public static string PoolProviderDelayingProvision(object arg0, CultureInfo culture) => TasksResources.Format(nameof (PoolProviderDelayingProvision), culture, arg0);

    public static string ProvisioningRequestAccepted() => TasksResources.Get(nameof (ProvisioningRequestAccepted));

    public static string ProvisioningRequestAccepted(CultureInfo culture) => TasksResources.Get(nameof (ProvisioningRequestAccepted), culture);

    public static string ProvisioningSucceeded(object arg0) => TasksResources.Format(nameof (ProvisioningSucceeded), arg0);

    public static string ProvisioningSucceeded(object arg0, CultureInfo culture) => TasksResources.Format(nameof (ProvisioningSucceeded), culture, arg0);

    public static string ProvisionRequestFailedByProvider() => TasksResources.Get(nameof (ProvisionRequestFailedByProvider));

    public static string ProvisionRequestFailedByProvider(CultureInfo culture) => TasksResources.Get(nameof (ProvisionRequestFailedByProvider), culture);

    public static string ProvisionRequestFailedToSend(object arg0) => TasksResources.Format(nameof (ProvisionRequestFailedToSend), arg0);

    public static string ProvisionRequestFailedToSend(object arg0, CultureInfo culture) => TasksResources.Format(nameof (ProvisionRequestFailedToSend), culture, arg0);

    public static string ReceivedDeprovisionEvent(object arg0) => TasksResources.Format(nameof (ReceivedDeprovisionEvent), arg0);

    public static string ReceivedDeprovisionEvent(object arg0, CultureInfo culture) => TasksResources.Format(nameof (ReceivedDeprovisionEvent), culture, arg0);

    public static string SendingDeprovisionRequest(object arg0) => TasksResources.Format(nameof (SendingDeprovisionRequest), arg0);

    public static string SendingDeprovisionRequest(object arg0, CultureInfo culture) => TasksResources.Format(nameof (SendingDeprovisionRequest), culture, arg0);

    public static string SendingProvisioningRequest(object arg0) => TasksResources.Format(nameof (SendingProvisioningRequest), arg0);

    public static string SendingProvisioningRequest(object arg0, CultureInfo culture) => TasksResources.Format(nameof (SendingProvisioningRequest), culture, arg0);

    public static string WaitingForAgentConnection() => TasksResources.Get(nameof (WaitingForAgentConnection));

    public static string WaitingForAgentConnection(CultureInfo culture) => TasksResources.Get(nameof (WaitingForAgentConnection), culture);
  }
}
