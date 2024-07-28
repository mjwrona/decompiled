// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Jobs.BatchProcessWithRetry`1
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Jobs
{
  public abstract class BatchProcessWithRetry<T>
  {
    private readonly IBatchQueue<T> batchQueue;
    private readonly int maxRetryCount;
    private readonly int batchSize;

    public BatchProcessWithRetry(IBatchQueue<T> batchQueue, int maxRetryCount, int batchSize)
    {
      this.batchQueue = batchQueue;
      this.maxRetryCount = maxRetryCount;
      this.batchSize = batchSize;
    }

    protected abstract int RetryCount { get; set; }

    protected abstract bool LastBatchFailed { get; }

    public int ProcessNextBatch()
    {
      int num = 0;
      if (this.LastBatchFailed)
      {
        if (this.RetryCount < this.maxRetryCount)
        {
          ++this.RetryCount;
          for (int index = 0; index < this.batchSize; ++index)
          {
            IEnumerable<T> objs = this.batchQueue.Peek(1);
            if (objs.Any<T>())
            {
              num += this.Process(objs);
              this.batchQueue.Dequeue();
            }
            else
              break;
          }
          this.RetryCount = 0;
          return num;
        }
        IEnumerable<T> source = this.batchQueue.Peek(1);
        if (source != null && source.Any<T>())
          this.Skip(source.First<T>());
        this.batchQueue.Dequeue();
        this.RetryCount = 0;
      }
      IEnumerable<T> objs1 = this.batchQueue.Peek(this.batchSize);
      if (objs1.Any<T>())
      {
        num += this.Process(objs1);
        this.batchQueue.Dequeue();
      }
      return num;
    }

    protected abstract int Process(IEnumerable<T> items);

    protected abstract void Skip(T item);
  }
}
