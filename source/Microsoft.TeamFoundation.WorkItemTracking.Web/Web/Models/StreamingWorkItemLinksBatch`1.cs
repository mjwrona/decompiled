// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.StreamingWorkItemLinksBatch`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  public sealed class StreamingWorkItemLinksBatch<T> : ReportingWorkItemLinksBatch<T>
  {
    private Microsoft.TeamFoundation.WorkItemTracking.Web.Models.StreamedBatchImpl<T> StreamedBatchImpl { get; set; }

    internal StreamingWorkItemLinksBatch(
      int bigLoopBatchSize,
      Func<SmallLoopBatch<T>> getSmallLoopBatch,
      Func<int, string, string> getNextLink,
      Func<string> getContinuationToken)
    {
      this.StreamedBatchImpl = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.StreamedBatchImpl<T>(bigLoopBatchSize, getSmallLoopBatch, getNextLink, getContinuationToken);
    }

    [JsonConverter(typeof (IEnumerableStreamingJsonConverter))]
    public override IEnumerable<T> Values
    {
      get => this.StreamedBatchImpl.Values;
      set
      {
      }
    }

    public override string NextLink
    {
      get => this.StreamedBatchImpl.NextLink;
      set
      {
      }
    }

    public override string ContinuationToken
    {
      get => this.StreamedBatchImpl.ContinuationToken;
      set
      {
      }
    }

    public override bool IsLastBatch
    {
      get => this.StreamedBatchImpl.IsLastBatch;
      set
      {
      }
    }
  }
}
