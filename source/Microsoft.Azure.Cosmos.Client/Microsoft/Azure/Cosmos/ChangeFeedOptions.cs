// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ChangeFeedOptions
  {
    private DateTime? startTime;

    public int? MaxItemCount { get; set; }

    public string RequestContinuation { get; set; }

    public string SessionToken { get; set; }

    public string PartitionKeyRangeId { get; set; }

    public Microsoft.Azure.Documents.PartitionKey PartitionKey { get; set; }

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

    internal bool IncludeTentativeWrites { get; set; }
  }
}
