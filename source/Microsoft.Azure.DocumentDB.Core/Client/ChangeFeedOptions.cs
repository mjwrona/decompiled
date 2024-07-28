// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.ChangeFeedOptions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Client
{
  public sealed class ChangeFeedOptions
  {
    private DateTime? startTime;

    public int? MaxItemCount { get; set; }

    public string RequestContinuation { get; set; }

    public string SessionToken { get; set; }

    public string PartitionKeyRangeId { get; set; }

    public PartitionKey PartitionKey { get; set; }

    public bool StartFromBeginning { get; set; }

    public DateTime? StartTime
    {
      get => this.startTime;
      set
      {
        if (value.HasValue && value.Value.Kind == DateTimeKind.Unspecified)
          throw new ArgumentException(RMResources.ChangeFeedOptionsStartTimeWithUnspecifiedDateTimeKind, nameof (value));
        this.startTime = value;
      }
    }

    internal TimeSpan? MaxServiceWaitTime { get; set; }

    internal bool IncludeTentativeWrites { get; set; }
  }
}
