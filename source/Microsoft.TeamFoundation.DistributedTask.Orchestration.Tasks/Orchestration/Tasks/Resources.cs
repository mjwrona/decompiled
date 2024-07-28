// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(resourceName) : Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(resourceName, culture);
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

    public static string JobCancelTimeout(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobCancelTimeout), arg0, arg1);

    public static string JobCancelTimeout(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobCancelTimeout), culture, arg0, arg1);

    public static string JobAbandoned(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobAbandoned), arg0);

    public static string JobAbandoned(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobAbandoned), culture, arg0);

    public static string JobTimedOut(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobTimedOut), arg0, arg1);

    public static string JobTimedOut(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobTimedOut), culture, arg0, arg1);

    public static string JobCancelFailFast(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobCancelFailFast), arg0);

    public static string JobCancelFailFast(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobCancelFailFast), culture, arg0);

    public static string PipelineJobCancelFailFast(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (PipelineJobCancelFailFast), arg0);

    public static string PipelineJobCancelFailFast(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (PipelineJobCancelFailFast), culture, arg0);

    public static string JobAbandonedNotStarted(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobAbandonedNotStarted), arg0);

    public static string JobAbandonedNotStarted(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobAbandonedNotStarted), culture, arg0);

    public static string JobTimedOutWaitingForEvent(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobTimedOutWaitingForEvent), arg0, arg1);

    public static string JobTimedOutWaitingForEvent(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (JobTimedOutWaitingForEvent), culture, arg0, arg1);

    public static string ServerTaskTimedOutWaitingForEvent(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerTaskTimedOutWaitingForEvent), arg0, arg1);

    public static string ServerTaskTimedOutWaitingForEvent(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerTaskTimedOutWaitingForEvent), culture, arg0, arg1);
    }

    public static string InvalidServerJob() => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (InvalidServerJob));

    public static string InvalidServerJob(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (InvalidServerJob), culture);

    public static string InvalidServerTask() => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (InvalidServerTask));

    public static string InvalidServerTask(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (InvalidServerTask), culture);

    public static string ServerTaskTimedOut(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerTaskTimedOut), arg0);

    public static string ServerTaskTimedOut(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerTaskTimedOut), culture, arg0);

    public static string ServerJobTimedOut(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerJobTimedOut), arg0);

    public static string ServerJobTimedOut(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerJobTimedOut), culture, arg0);

    public static string ServerJob4TimedOut(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerJob4TimedOut), arg0);

    public static string ServerJob4TimedOut(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerJob4TimedOut), culture, arg0);

    public static string UnableToStartPipelineNoStartingPhase(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (UnableToStartPipelineNoStartingPhase), arg0);

    public static string UnableToStartPipelineNoStartingPhase(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (UnableToStartPipelineNoStartingPhase), culture, arg0);

    public static string InvalidDelayForMinutes(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (InvalidDelayForMinutes), arg0, arg1);

    public static string InvalidDelayForMinutes(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (InvalidDelayForMinutes), culture, arg0, arg1);

    public static string DelayForMinutesInputNotFound() => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (DelayForMinutesInputNotFound));

    public static string DelayForMinutesInputNotFound(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (DelayForMinutesInputNotFound), culture);

    public static string ServerTaskCancellationTimedOut(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerTaskCancellationTimedOut), arg0);

    public static string ServerTaskCancellationTimedOut(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerTaskCancellationTimedOut), culture, arg0);

    public static string ServerJobExecutionTimedOut(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerJobExecutionTimedOut), arg0);

    public static string ServerJobExecutionTimedOut(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (ServerJobExecutionTimedOut), culture, arg0);

    public static string DemandsNotMet(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (DemandsNotMet), arg0, arg1);

    public static string DemandsNotMet(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (DemandsNotMet), culture, arg0, arg1);

    public static string TargetOffline(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (TargetOffline), arg0);

    public static string TargetOffline(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (TargetOffline), culture, arg0);

    public static string NoTargetFound() => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (NoTargetFound));

    public static string NoTargetFound(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (NoTargetFound), culture);

    public static string NoTargetFoundWithGivenTags(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (NoTargetFoundWithGivenTags), arg0);

    public static string NoTargetFoundWithGivenTags(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (NoTargetFoundWithGivenTags), culture, arg0);

    public static string DeploymentHealthNotMet(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (DeploymentHealthNotMet), arg0);

    public static string DeploymentHealthNotMet(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (DeploymentHealthNotMet), culture, arg0);

    public static string DeploymentSkippedOnTarget(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (DeploymentSkippedOnTarget), arg0);

    public static string DeploymentSkippedOnTarget(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (DeploymentSkippedOnTarget), culture, arg0);

    public static string TargetExcluded(object arg0) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (TargetExcluded), arg0);

    public static string TargetExcluded(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Format(nameof (TargetExcluded), culture, arg0);

    public static string CouldNotFetchInstallationToken() => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (CouldNotFetchInstallationToken));

    public static string CouldNotFetchInstallationToken(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.Resources.Get(nameof (CouldNotFetchInstallationToken), culture);
  }
}
