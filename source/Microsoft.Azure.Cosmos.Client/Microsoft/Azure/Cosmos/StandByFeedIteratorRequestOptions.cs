// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.StandByFeedIteratorRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  internal class StandByFeedIteratorRequestOptions : RequestOptions
  {
    internal const string IfNoneMatchAllHeaderValue = "*";
    internal static readonly DateTime DateTimeStartFromBeginning = DateTime.MinValue.ToUniversalTime();

    public int? MaxItemCount { get; set; }

    public DateTime? StartTime { get; set; }

    internal override void PopulateRequestOptions(RequestMessage request)
    {
      if (string.IsNullOrEmpty(request.Headers.IfNoneMatch))
      {
        if (!this.StartTime.HasValue)
          request.Headers.IfNoneMatch = "*";
        else if (this.StartTime.HasValue)
        {
          DateTime? startTime = this.StartTime;
          DateTime startFromBeginning = StandByFeedIteratorRequestOptions.DateTimeStartFromBeginning;
          if ((startTime.HasValue ? (startTime.HasValue ? (startTime.GetValueOrDefault() != startFromBeginning ? 1 : 0) : 0) : 1) != 0)
          {
            Headers headers = request.Headers;
            startTime = this.StartTime;
            string str = startTime.Value.ToUniversalTime().ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
            headers.Add("If-Modified-Since", str);
          }
        }
      }
      StandByFeedIteratorRequestOptions.FillMaxItemCount(request, this.MaxItemCount);
      request.Headers.Add("A-IM", "Incremental Feed");
      base.PopulateRequestOptions(request);
    }

    internal static void FillPartitionKeyRangeId(RequestMessage request, string partitionKeyRangeId)
    {
      if (string.IsNullOrEmpty(partitionKeyRangeId))
        return;
      request.PartitionKeyRangeId = new PartitionKeyRangeIdentity(partitionKeyRangeId);
    }

    internal static void FillPartitionKey(RequestMessage request, PartitionKey partitionKey) => request.Headers.PartitionKey = partitionKey.ToJsonString();

    internal static void FillContinuationToken(RequestMessage request, string continuationToken)
    {
      if (string.IsNullOrWhiteSpace(continuationToken))
        return;
      request.Headers.IfNoneMatch = continuationToken;
    }

    internal static void FillMaxItemCount(RequestMessage request, int? maxItemCount)
    {
      if (!maxItemCount.HasValue)
        return;
      request.Headers.Add("x-ms-max-item-count", maxItemCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
