// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobsStillRunningException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class JobsStillRunningException : JobAgentException
  {
    private static readonly Guid[] s_jobsWithExternalIssuesPreventingStop = new Guid[2]
    {
      new Guid("68D12C31-4894-49C3-8E12-4D3E954C98E7"),
      new Guid("544DD581-F72A-45A9-8DE0-8CD3A5F29DFE")
    };

    public JobsStillRunningException()
    {
    }

    public JobsStillRunningException(string message)
      : base(message)
    {
    }

    public JobsStillRunningException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected JobsStillRunningException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    internal JobsStillRunningException(bool processIsStopping, IEnumerable<JobRunner> runners)
      : this(JobsStillRunningException.GetExceptionMessage(processIsStopping, runners))
    {
      foreach (JobRunner runner in runners)
      {
        if (runner.JobDefinition == null || !((IEnumerable<Guid>) JobsStillRunningException.s_jobsWithExternalIssuesPreventingStop).Contains<Guid>(runner.JobDefinition.JobId))
        {
          this.ReportException = true;
          break;
        }
      }
    }

    internal static string GetExceptionMessage(
      bool processIsStopping,
      IEnumerable<JobRunner> runners)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (JobRunner runner in runners)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append("; ");
        stringBuilder.Append(runner.GetStringForExceptionMessage());
      }
      return processIsStopping ? FrameworkResources.JobsStillRunningDuringTerminationError((object) stringBuilder) : FrameworkResources.JobsStillRunningDuringRecycleError((object) stringBuilder);
    }
  }
}
