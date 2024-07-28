// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformResult
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  [DataContract]
  public class TransformResult
  {
    [DataMember]
    public bool AllProcessingComplete { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool AlreadyActive { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Deferred { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DeferredReason { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Held { get; set; }

    [DataMember]
    public int PartitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long? BatchId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? SubBatchCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? AttemptCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TableName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Sproc { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? SprocVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TriggerTableName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long? TriggerBatchIdStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long? TriggerBatchIdEnd { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long? StateData { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string EndState { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long? EndStateData { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Ready { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Failed { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? FailedAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FailedMessage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? InsertedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? UpdatedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? DeletedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? DurationMS { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TotalInsertedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TotalUpdatedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TotalDeletedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TotalDurationMS { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TotalFailedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ReworkAttemptCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Priority { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? SqlExecutionTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ConcurrencyCounter { get; set; }
  }
}
