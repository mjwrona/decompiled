// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.StreamedBatchImpl`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  internal class StreamedBatchImpl<T>
  {
    private int BigLoopBatchSize { get; set; }

    private int Counter { get; set; }

    private Func<SmallLoopBatch<T>> GetSmallLoopBatch { get; set; }

    private Func<int, string, string> GetNextLink { get; set; }

    private Func<string> GetContinuationToken { get; set; }

    internal StreamedBatchImpl(
      int bigLoopBatchSize,
      Func<SmallLoopBatch<T>> getSmallLoopBatch,
      Func<int, string, string> getNextLink,
      Func<string> getContinuationToken)
    {
      this.BigLoopBatchSize = bigLoopBatchSize;
      this.Counter = -1;
      this.GetSmallLoopBatch = getSmallLoopBatch;
      this.GetNextLink = getNextLink;
      this.GetContinuationToken = getContinuationToken;
    }

    public IEnumerable<T> Values
    {
      get
      {
        this.Counter = 0;
        while (this.Counter < this.BigLoopBatchSize)
        {
          SmallLoopBatch<T> smallLoopBatch = this.GetSmallLoopBatch();
          this.Counter += smallLoopBatch.RawCount;
          foreach (T obj in smallLoopBatch.Values)
            yield return obj;
          if (smallLoopBatch.IsFinalSmallBatch)
            break;
        }
      }
    }

    public string NextLink
    {
      get
      {
        if (this.Counter < 0)
          throw new ArgumentException("NextLink has been evaluated prior to Values. This must never happen.");
        return this.GetNextLink(this.Counter, this.ContinuationToken);
      }
    }

    public bool IsLastBatch
    {
      get
      {
        if (this.Counter < 0)
          throw new ArgumentException("IsLastBatch has been evaluated prior to Values. This must never happen.");
        return this.Counter < this.BigLoopBatchSize;
      }
    }

    public string ContinuationToken
    {
      get
      {
        if (this.Counter < 0)
          throw new ArgumentException("NextLink has been evaluated prior to Values. This must never happen.");
        return this.GetContinuationToken();
      }
    }
  }
}
