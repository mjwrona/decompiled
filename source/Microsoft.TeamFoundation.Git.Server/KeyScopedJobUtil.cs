// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.KeyScopedJobUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class KeyScopedJobUtil
  {
    public static int QueueFor(
      IVssRequestContext requestContext,
      OdbId odbId,
      string jobName,
      string extensionName,
      JobPriorityLevel priorityLevel,
      JobPriorityClass priorityClass,
      int maxDelaySeconds = 0)
    {
      return KeyScopedJobUtil.QueueFor<OdbJobKey>(requestContext, new OdbJobKey(odbId), jobName, extensionName, priorityLevel, priorityClass, maxDelaySeconds = 0);
    }

    public static int QueueFor<TKey>(
      IVssRequestContext requestContext,
      TKey targetKey,
      string jobName,
      string extensionName,
      JobPriorityLevel priorityLevel,
      JobPriorityClass priorityClass,
      int maxDelaySeconds = 0)
      where TKey : IGitJobKey
    {
      ArgumentUtility.CheckStringForNullOrEmpty(jobName, nameof (jobName));
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      Guid jobId = KeyScopedJobUtil.JobIdForKeyScopedJob<TKey>(targetKey, jobName);
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, jobId);
      if (foundationJobDefinition == null)
      {
        foundationJobDefinition = new TeamFoundationJobDefinition(jobId, FormattableString.Invariant(FormattableStringFactory.Create("{0}, {1}", (object) jobName, (object) targetKey)), extensionName, TeamFoundationSerializationUtility.SerializeToXml((object) targetKey), TeamFoundationJobEnabledState.Enabled, false, priorityClass);
        service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
      }
      return service.QueueJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      }, priorityLevel, maxDelaySeconds, false);
    }

    public static void CreateInitialSchedules<TKey>(
      IVssRequestContext rc,
      TKey targetKey,
      DateTime scheduleTime)
      where TKey : IGitJobKey
    {
      using (IDisposableReadOnlyList<GitKeyedJob<TKey>> gitKeyedJobs = KeyScopedJobUtil.GetGitKeyedJobs<TKey>(rc, (Func<GitKeyedJob<TKey>, bool>) (x => x.ScheduleOnCreate)))
      {
        foreach (GitKeyedJob<TKey> jobInstance in (IEnumerable<GitKeyedJob<TKey>>) gitKeyedJobs)
          KeyScopedJobUtil.ScheduleFor<TKey>(rc, targetKey, jobInstance, scheduleTime);
      }
    }

    public static void ScheduleFor<TKey>(
      IVssRequestContext requestContext,
      TKey targetKey,
      GitKeyedJob<TKey> jobInstance,
      DateTime scheduleTime,
      bool overwriteExistingSchedule = false)
      where TKey : IGitJobKey
    {
      ArgumentUtility.CheckForNull<GitKeyedJob<TKey>>(jobInstance, nameof (jobInstance));
      Type type = jobInstance.GetType();
      string name = type.Name;
      string fullName = type.FullName;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      Guid jobId = KeyScopedJobUtil.JobIdForKeyScopedJob<TKey>(targetKey, name);
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, jobId);
      if (foundationJobDefinition == null)
      {
        foundationJobDefinition = new TeamFoundationJobDefinition(jobId, FormattableString.Invariant(FormattableStringFactory.Create("{0}, {1}", (object) name, (object) targetKey)), fullName, TeamFoundationSerializationUtility.SerializeToXml((object) targetKey), TeamFoundationJobEnabledState.Enabled, false, jobInstance.JobPriorityClass);
      }
      else
      {
        if (!overwriteExistingSchedule && foundationJobDefinition.Schedule.Count > 0)
          return;
        foundationJobDefinition.Schedule.Clear();
      }
      foundationJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        Interval = (int) jobInstance.ScheduleInterval.TotalSeconds,
        ScheduledTime = scheduleTime
      });
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
    }

    public static IDisposableReadOnlyList<GitKeyedJob<T>> GetGitKeyedJobs<T>(
      IVssRequestContext rc,
      Func<GitKeyedJob<T>, bool> filter)
      where T : IGitJobKey
    {
      return rc.GetExtensions<GitKeyedJob<T>>(filter);
    }

    public static Guid JobIdForKeyScopedJob<TKey>(TKey key, string jobName) where TKey : IGitJobKey => DeterministicGuid.Compute(key.ToString() + ":" + jobName);
  }
}
