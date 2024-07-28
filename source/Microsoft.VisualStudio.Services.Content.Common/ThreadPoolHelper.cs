// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ThreadPoolHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class ThreadPoolHelper
  {
    private static readonly int MaxMaxThreadCount = 8192;
    private static readonly int MaxToMinThreadCountRatio = 16;

    public static string GetThreadCountSummary()
    {
      int workerThreads1;
      int completionPortThreads1;
      ThreadPool.GetMinThreads(out workerThreads1, out completionPortThreads1);
      int workerThreads2;
      int completionPortThreads2;
      ThreadPool.GetMaxThreads(out workerThreads2, out completionPortThreads2);
      int workerThreads3;
      int completionPortThreads3;
      ThreadPool.GetAvailableThreads(out workerThreads3, out completionPortThreads3);
      return string.Format("WorkerThreadsMin={0},WorkerThreadsMax={1},WorkerThreadsAvailable={2},", (object) workerThreads1, (object) workerThreads2, (object) workerThreads3) + string.Format("CompletionPortThreadsMin={0},CompletionPortThreadsMax={1},CompletionPortThreadsAvailable={2}", (object) completionPortThreads1, (object) completionPortThreads2, (object) completionPortThreads3);
    }

    public static void IncreaseThreadCounts(int workerThreadsPerCore, int completionThreadsPerCore)
    {
      int val2_1 = Math.Min(ThreadPoolHelper.MaxMaxThreadCount, Environment.ProcessorCount * workerThreadsPerCore);
      int val2_2 = Math.Min(ThreadPoolHelper.MaxMaxThreadCount, Environment.ProcessorCount * completionThreadsPerCore);
      int workerThreads1;
      int completionPortThreads1;
      ThreadPool.GetMaxThreads(out workerThreads1, out completionPortThreads1);
      int workerThreads2 = Math.Max(workerThreads1, val2_1);
      int completionPortThreads2 = Math.Max(completionPortThreads1, val2_2);
      if (!ThreadPool.SetMaxThreads(workerThreads2, completionPortThreads2))
        throw new InvalidOperationException(string.Format("ThreadPool.SetMaxThreads({0}, {1}) did not change values", (object) workerThreads2, (object) completionPortThreads2));
      int val2_3 = Math.Min(workerThreads2 / ThreadPoolHelper.MaxToMinThreadCountRatio, Environment.ProcessorCount * workerThreadsPerCore);
      int val2_4 = Math.Min(completionPortThreads2 / ThreadPoolHelper.MaxToMinThreadCountRatio, Environment.ProcessorCount * completionThreadsPerCore);
      ThreadPool.GetMinThreads(out workerThreads1, out completionPortThreads1);
      int workerThreads3 = Math.Max(workerThreads1, val2_3);
      int completionPortThreads3 = Math.Max(completionPortThreads1, val2_4);
      if (!ThreadPool.SetMinThreads(workerThreads3, completionPortThreads3))
        throw new InvalidOperationException(string.Format("ThreadPool.SetMinThreads({0}, {1}) did not change values", (object) workerThreads3, (object) completionPortThreads3));
    }

    public class Constants
    {
      public const int ThreadsPerCore8 = 8;
      public const int ThreadsPerCore16 = 16;
      public const int ThreadsPerCore180 = 180;
      public const int ThreadsPerCore2048 = 2048;
    }
  }
}
