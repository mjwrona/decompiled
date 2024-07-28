// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatch
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  public class TransformBatch
  {
    public long BatchId { get; set; }

    public bool Ready { get; set; }

    public bool Failed { get; set; }

    public bool Active { get; set; }

    public string TriggerTableName { get; set; }

    public long TriggerBatchIdStart { get; set; }

    public long TriggerBatchIdEnd { get; set; }

    public int AttemptCount { get; set; }

    public string TableName { get; set; }

    public string Sproc { get; set; }

    public int SprocVersion { get; set; }

    public int TransformPriority { get; set; }

    public string State { get; set; }

    public long? StateData { get; set; }

    public DateTime CreateDateTime { get; set; }

    public bool Held { get; set; }

    public bool LowPriorityDeferred { get; set; }

    public bool TargetActiveBlocked { get; set; }
  }
}
