// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.AbuseDetectionMetrics
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class AbuseDetectionMetrics
  {
    public AbuseDetectionMetrics()
    {
    }

    protected AbuseDetectionMetrics(AbuseDetectionMetrics metricToClone)
    {
      this.HostId = metricToClone.HostId;
      this.Score = metricToClone.Score;
      this.MaxParallelism = metricToClone.MaxParallelism;
      this.FirstBuildDate = metricToClone.FirstBuildDate;
      this.TotalJobs = metricToClone.TotalJobs;
      this.InProgressJobs = metricToClone.InProgressJobs;
      this.NumberOfLongRunningRequests = metricToClone.NumberOfLongRunningRequests;
      this.AccountLifetimeInMinutes = metricToClone.AccountLifetimeInMinutes;
      this.HasMaxConcurrentLongRunningBuilds = metricToClone.HasMaxConcurrentLongRunningBuilds;
      this.TotalJobRuntime = metricToClone.TotalJobRuntime;
      this.InProgressJobRuntime = metricToClone.InProgressJobRuntime;
    }

    [DataMember(IsRequired = true)]
    public Guid HostId { get; set; }

    [DataMember(IsRequired = true)]
    public int Score { get; set; }

    [DataMember(IsRequired = true)]
    public int MaxParallelism { get; set; }

    [DataMember(IsRequired = true)]
    public DateTime FirstBuildDate { get; set; }

    [DataMember(IsRequired = true)]
    public int TotalJobs { get; set; }

    [DataMember(IsRequired = true)]
    public int InProgressJobs { get; set; }

    [DataMember(IsRequired = true)]
    public int NumberOfLongRunningRequests { get; set; }

    [DataMember(IsRequired = true)]
    public int AccountLifetimeInMinutes { get; set; }

    [DataMember(IsRequired = true)]
    public int TotalJobRuntime { get; set; }

    [DataMember(IsRequired = true)]
    public int InProgressJobRuntime { get; set; }

    [DataMember(IsRequired = true)]
    public bool HasMaxConcurrentLongRunningBuilds { get; set; }
  }
}
