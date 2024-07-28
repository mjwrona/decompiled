// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingJobInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract(Namespace = "", Name = "ServicingJobInfo")]
  public class ServicingJobInfo : IExtensibleDataObject
  {
    [DataMember(Name = "jobId", Order = 0)]
    public Guid JobId { get; set; }

    [DataMember(Name = "hostId", Order = 1)]
    public Guid HostId { get; set; }

    [DataMember(Name = "operationClass", Order = 2)]
    public string OperationClass { get; set; }

    [DataMember(Name = "databaseId", Order = 3)]
    public int DatabaseId { get; set; }

    [DataMember(Name = "databaseName", Order = 4)]
    public string DatabaseName { get; set; }

    [DataMember(Name = "poolName", Order = 5)]
    public string PoolName { get; set; }

    [DataMember(Name = "poolId", Order = 6)]
    public int PoolId { get; set; }

    [DataMember(Name = "accountId", Order = 7)]
    public Guid AccountId { get; set; }

    [DataMember(Name = "queueTime", Order = 8)]
    public DateTime QueueTime { get; set; }

    [DataMember(Name = "startTime", Order = 9)]
    public DateTime? StartTime { get; set; }

    [DataMember(Name = "endTime", Order = 10)]
    public DateTime? EndTime { get; set; }

    [DataMember(Name = "jobStatus", Order = 11)]
    public ServicingJobStatus JobStatus { get; set; }

    [DataMember(Name = "jobResult", Order = 12)]
    public ServicingJobResult JobResult { get; set; }

    [DataMember(Name = "completedStepCount", Order = 13)]
    public short CompletedStepCount { get; set; }

    [DataMember(Name = "totalStepCount", Order = 14)]
    public short TotalStepCount { get; set; }

    [DataMember(Name = "name", Order = 15)]
    public string Name { get; set; }

    [DataMember(Name = "parentName", Order = 16)]
    public string ParentName { get; set; }

    [DataMember(Name = "logUri", Order = 17)]
    public string LogUri { get; set; }

    [DataMember(Name = "operations", Order = 18)]
    public string Operations { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
