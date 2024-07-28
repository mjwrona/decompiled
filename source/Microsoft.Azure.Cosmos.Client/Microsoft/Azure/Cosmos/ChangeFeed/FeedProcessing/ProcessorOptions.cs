// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing.ProcessorOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing
{
  internal class ProcessorOptions
  {
    public string LeaseToken { get; set; }

    public int? MaxItemCount { get; set; }

    public TimeSpan FeedPollDelay { get; set; }

    public string StartContinuation { get; set; }

    public bool StartFromBeginning { get; set; }

    public DateTime? StartTime { get; set; }

    public TimeSpan RequestTimeout { get; set; } = CosmosHttpClient.GatewayRequestTimeout;
  }
}
