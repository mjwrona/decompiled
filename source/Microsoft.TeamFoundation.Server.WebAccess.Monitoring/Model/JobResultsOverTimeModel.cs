// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model.JobResultsOverTimeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model
{
  [DataContract]
  public class JobResultsOverTimeModel
  {
    public JobResultsOverTimeModel(
      IVssRequestContext requestContext,
      TeamFoundationJobReportingResultsOverTime result)
    {
      TeamFoundationJobReportingService service = requestContext.GetService<TeamFoundationJobReportingService>();
      this.JobId = result.JobId;
      this.JobName = service.GetJobName(requestContext, result.JobId);
      this.Succeeded = result.SucceededCount;
      this.PartiallySucceeded = result.PartiallySucceededCount;
      this.Failed = result.FailedCount;
      this.Stopped = result.StoppedCount;
      this.Killed = result.KilledCount;
      this.Blocked = result.BlockedCount;
      this.ExtensionNotFound = result.ExtensionNotFoundCount;
      this.Inactive = result.InactiveCount;
      this.Disabled = result.DisabledCount;
    }

    [DataMember(Name = "JobId")]
    public Guid JobId { get; set; }

    [DataMember(Name = "TotalRunTimeInSeconds")]
    public int TotalRunTimeInSeconds { get; set; }

    [DataMember(Name = "Succeeded")]
    public int Succeeded { get; set; }

    [DataMember(Name = "PartiallySucceeded")]
    public int PartiallySucceeded { get; set; }

    [DataMember(Name = "Failed")]
    public int Failed { get; set; }

    [DataMember(Name = "Stopped")]
    public int Stopped { get; set; }

    [DataMember(Name = "Killed")]
    public int Killed { get; set; }

    [DataMember(Name = "Blocked")]
    public int Blocked { get; set; }

    [DataMember(Name = "ExtensionNotFound")]
    public int ExtensionNotFound { get; set; }

    [DataMember(Name = "Inactive")]
    public int Inactive { get; set; }

    [DataMember(Name = "Disabled")]
    public int Disabled { get; set; }

    [DataMember(Name = "JobName")]
    public string JobName { get; set; }
  }
}
