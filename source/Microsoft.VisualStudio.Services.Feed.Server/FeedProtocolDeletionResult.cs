// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedProtocolDeletionResult
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedProtocolDeletionResult
  {
    public FeedProtocolDeletionResult(Guid feedId)
    {
      ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
      this.FeedId = feedId;
      this.ProtocolDeletionCount = (IDictionary<string, int>) new Dictionary<string, int>();
      this.ProtocolEligibleForDeletionCount = (IDictionary<string, int>) new Dictionary<string, int>();
    }

    public Guid FeedId { get; }

    public int PolicyCountLimit { get; set; }

    public int PolicyDaysToKeepRecentlyDownloaded { get; set; }

    public IDictionary<string, int> ProtocolEligibleForDeletionCount { get; }

    public IDictionary<string, int> ProtocolDeletionCount { get; }

    public void SetProtocolEligibleForDeletionCount(string protocol, int eligibleForDeletionCount) => this.ProtocolEligibleForDeletionCount[protocol] = eligibleForDeletionCount;

    public void SetProtocolCount(string protocol, int deletionCount) => this.ProtocolDeletionCount[protocol] = deletionCount;
  }
}
