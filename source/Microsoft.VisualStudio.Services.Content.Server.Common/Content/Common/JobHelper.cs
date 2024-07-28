// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JobHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class JobHelper
  {
    public static readonly TimeSpan RetryInterval = TimeSpan.FromMinutes(5.0);
    public const int NumRetries = 10;

    public static string GetNestedExceptionMessage(Exception ex, int levels = 4)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < levels && ex != null; ++index)
      {
        stringBuilder.AppendFormat("[Level {0}] {1}, {2}; ", (object) index, (object) ex.GetType().Name, (object) ex.Message);
        ex = ex.InnerException;
      }
      return stringBuilder.ToString();
    }

    public static TeamFoundationJobQueueEntry GetCurrentJobQueueEntry(
      IVssRequestContext requestContext,
      Guid jobId)
    {
      return requestContext.GetService<ITeamFoundationJobService>().QueryJobQueue(requestContext, jobId);
    }

    public static bool IsJobDisabled(
      IVssRequestContext requestContext,
      JobHelper.JobExecutionState enabledState,
      Guid jobId)
    {
      switch (enabledState)
      {
        case JobHelper.JobExecutionState.Enabled:
          return false;
        case JobHelper.JobExecutionState.DisableScheduled:
          TeamFoundationJobQueueEntry currentJobQueueEntry = JobHelper.GetCurrentJobQueueEntry(requestContext, jobId);
          return currentJobQueueEntry == null || currentJobQueueEntry.QueuedReasons.HasFlag((Enum) TeamFoundationJobQueuedReasons.Scheduled);
        default:
          return true;
      }
    }

    public enum JobExecutionState
    {
      Enabled,
      DisableScheduled,
      DisableAll,
    }
  }
}
