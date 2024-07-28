// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.BatchExecutionHandler`1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class BatchExecutionHandler<T> where T : class
  {
    private const string c_area = "BatchExecutionHandler";
    private static readonly string c_layer = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BatchType<{0}>", (object) typeof (T).Name);
    private readonly Func<IVssRequestContext, List<T>, bool> m_batchProcessor;
    private readonly ConcurrentQueue<T> m_commandExecutionDataQueue;
    private readonly int m_batchSize;
    private readonly object m_batchExecutionLock = new object();

    public BatchExecutionHandler(
      int batchSize,
      Func<IVssRequestContext, List<T>, bool> batchProcessor)
    {
      this.m_batchSize = batchSize;
      this.m_batchProcessor = batchProcessor;
      this.m_commandExecutionDataQueue = new ConcurrentQueue<T>();
    }

    public void Add(IVssRequestContext requestContext, T dataForProcessing)
    {
      this.m_commandExecutionDataQueue.Enqueue(dataForProcessing);
      this.ProcessData(requestContext, false);
    }

    private void ProcessData(IVssRequestContext requestContext, bool flush)
    {
      requestContext.TraceEnter(12061108, nameof (BatchExecutionHandler<T>), BatchExecutionHandler<T>.c_layer, nameof (ProcessData));
      List<T> enumerable = (List<T>) null;
      if (flush || this.m_commandExecutionDataQueue.Count >= this.m_batchSize)
      {
        lock (this.m_batchExecutionLock)
        {
          requestContext.Trace(12061108, TraceLevel.Info, nameof (BatchExecutionHandler<T>), BatchExecutionHandler<T>.c_layer, "Acquired lock, CurrentThread={0}", (object) Environment.CurrentManagedThreadId);
          if (flush || this.m_commandExecutionDataQueue.Count >= this.m_batchSize)
          {
            requestContext.Trace(12061108, TraceLevel.Info, nameof (BatchExecutionHandler<T>), BatchExecutionHandler<T>.c_layer, "Fetching batch, CurrentThread={0}", (object) Environment.CurrentManagedThreadId);
            enumerable = new List<T>();
            int num = 0;
            T result = default (T);
            while (true)
            {
              if (!flush)
              {
                if (num > this.m_batchSize)
                  break;
              }
              if (this.m_commandExecutionDataQueue.TryDequeue(out result))
              {
                enumerable.Add(result);
                result = default (T);
                ++num;
              }
              else
                break;
            }
          }
          else
            requestContext.Trace(12061108, TraceLevel.Info, nameof (BatchExecutionHandler<T>), BatchExecutionHandler<T>.c_layer, "Not enough batch size, CurrentThread={0}", (object) Environment.CurrentManagedThreadId);
        }
      }
      if (!enumerable.IsNullOrEmpty<T>())
      {
        requestContext.Trace(12061108, TraceLevel.Info, nameof (BatchExecutionHandler<T>), BatchExecutionHandler<T>.c_layer, "Executing batch, CurrentThread={0}", (object) Environment.CurrentManagedThreadId);
        int num = this.m_batchProcessor(requestContext, enumerable) ? 1 : 0;
      }
      requestContext.TraceLeave(12061108, nameof (BatchExecutionHandler<T>), BatchExecutionHandler<T>.c_layer, nameof (ProcessData));
    }

    public void Flush(IVssRequestContext requestContext) => this.ProcessData(requestContext, true);
  }
}
