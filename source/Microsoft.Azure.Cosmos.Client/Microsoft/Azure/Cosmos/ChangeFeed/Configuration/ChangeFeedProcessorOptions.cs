// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Configuration.ChangeFeedProcessorOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Monitoring;
using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Configuration
{
  internal class ChangeFeedProcessorOptions
  {
    private static readonly TimeSpan DefaultFeedPollDelay = TimeSpan.FromSeconds(5.0);
    private DateTime? startTime;

    public ChangeFeedProcessorOptions() => this.FeedPollDelay = ChangeFeedProcessorOptions.DefaultFeedPollDelay;

    public TimeSpan FeedPollDelay { get; set; }

    public int? MaxItemCount { get; set; }

    public string StartContinuation { get; set; }

    public DateTime? StartTime
    {
      get => this.startTime;
      set
      {
        if (value.HasValue && value.Value.Kind == DateTimeKind.Unspecified)
          throw new ArgumentException("StartTime cannot have DateTimeKind.Unspecified", nameof (value));
        this.startTime = value;
      }
    }

    public bool StartFromBeginning { get; set; }

    public ChangeFeedProcessorHealthMonitorCore HealthMonitor { get; set; } = new ChangeFeedProcessorHealthMonitorCore();
  }
}
