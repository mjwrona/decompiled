// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ThreadInformation
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ThreadInformation
  {
    private static readonly object lockObject = new object();
    private static Stopwatch watch;
    private static Task task;

    internal int? AvailableThreads { get; }

    internal int? MinThreads { get; }

    internal int? MaxThreads { get; }

    internal bool? IsThreadStarving { get; }

    internal double? ThreadWaitIntervalInMs { get; }

    public static ThreadInformation Get()
    {
      int? nullable1 = new int?();
      int? nullable2 = new int?();
      int? nullable3 = new int?();
      ThreadInformation threadInformation = (ThreadInformation) null;
      lock (ThreadInformation.lockObject)
      {
        int workerThreads1;
        int completionPortThreads;
        ThreadPool.GetAvailableThreads(out workerThreads1, out completionPortThreads);
        int? availableThreads = new int?(workerThreads1);
        int workerThreads2;
        ThreadPool.GetMinThreads(out workerThreads2, out completionPortThreads);
        int? minThreads = new int?(workerThreads2);
        int workerThreads3;
        ThreadPool.GetMaxThreads(out workerThreads3, out completionPortThreads);
        int? maxThreads = new int?(workerThreads3);
        bool? isThreadStarving = new bool?();
        double? threadWaitIntervalInMs = new double?();
        if (ThreadInformation.watch != null && ThreadInformation.task != null)
        {
          threadWaitIntervalInMs = new double?(ThreadInformation.watch.Elapsed.TotalMilliseconds);
          ref bool? local = ref isThreadStarving;
          double? nullable4 = threadWaitIntervalInMs;
          double num1 = 1000.0;
          int num2 = nullable4.GetValueOrDefault() > num1 & nullable4.HasValue ? 1 : (ThreadInformation.task.IsFaulted ? 1 : 0);
          local = new bool?(num2 != 0);
          if (ThreadInformation.task.IsFaulted && ThreadInformation.watch.IsRunning)
          {
            DefaultTrace.TraceError("Thread Starvation detection task failed. Exception: {0}", (object) ThreadInformation.task.Exception);
            ThreadInformation.watch.Stop();
          }
        }
        threadInformation = new ThreadInformation(availableThreads, minThreads, maxThreads, isThreadStarving, threadWaitIntervalInMs);
        if (ThreadInformation.watch != null)
        {
          if (ThreadInformation.watch.IsRunning)
            goto label_10;
        }
        ThreadInformation.watch = Stopwatch.StartNew();
        ThreadInformation.task = Task.Factory.StartNew((Action) (() => ThreadInformation.watch.Stop()));
      }
label_10:
      return threadInformation;
    }

    private ThreadInformation(
      int? availableThreads,
      int? minThreads,
      int? maxThreads,
      bool? isThreadStarving,
      double? threadWaitIntervalInMs)
    {
      this.AvailableThreads = availableThreads;
      this.MinThreads = minThreads;
      this.MaxThreads = maxThreads;
      this.IsThreadStarving = isThreadStarving;
      this.ThreadWaitIntervalInMs = threadWaitIntervalInMs;
    }

    public void AppendJsonString(StringBuilder stringBuilder)
    {
      stringBuilder.Append("{\"isThreadStarving\":\"");
      if (this.IsThreadStarving.HasValue)
        stringBuilder.Append(this.IsThreadStarving.Value).Append("\",");
      else
        stringBuilder.Append("no info\",");
      double? waitIntervalInMs = this.ThreadWaitIntervalInMs;
      if (waitIntervalInMs.HasValue)
      {
        StringBuilder stringBuilder1 = stringBuilder.Append("\"threadWaitIntervalInMs\":");
        waitIntervalInMs = this.ThreadWaitIntervalInMs;
        string str = waitIntervalInMs.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder1.Append(str).Append(",");
      }
      int? nullable = this.AvailableThreads;
      if (nullable.HasValue)
      {
        StringBuilder stringBuilder2 = stringBuilder.Append("\"availableThreads\":");
        nullable = this.AvailableThreads;
        int num = nullable.Value;
        stringBuilder2.Append(num).Append(",");
      }
      nullable = this.MinThreads;
      if (nullable.HasValue)
      {
        StringBuilder stringBuilder3 = stringBuilder.Append("\"minThreads\":");
        nullable = this.MinThreads;
        int num = nullable.Value;
        stringBuilder3.Append(num).Append(",");
      }
      nullable = this.MaxThreads;
      if (nullable.HasValue)
      {
        StringBuilder stringBuilder4 = stringBuilder.Append("\"maxThreads\":");
        nullable = this.MaxThreads;
        int num = nullable.Value;
        stringBuilder4.Append(num).Append(",");
      }
      --stringBuilder.Length;
      stringBuilder.Append("}");
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.IsThreadStarving.HasValue)
        stringBuilder.Append("IsThreadStarving :").Append(this.IsThreadStarving.Value);
      if (this.ThreadWaitIntervalInMs.HasValue)
        stringBuilder.Append(" ThreadWaitIntervalInMs :").Append(this.ThreadWaitIntervalInMs.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (this.AvailableThreads.HasValue)
        stringBuilder.Append(" AvailableThreads :").Append(this.AvailableThreads.Value);
      if (this.MinThreads.HasValue)
        stringBuilder.Append(" MinThreads :").Append(this.MinThreads.Value);
      if (this.MaxThreads.HasValue)
        stringBuilder.Append(" MaxThreads :").Append(this.MaxThreads.Value);
      return stringBuilder.ToString();
    }
  }
}
