// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackWriterThrottler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitPackWriterThrottler : IDisposable
  {
    private int m_totalDelayTime;
    private int m_totalDelayCount;
    private readonly IVssRequestContext m_rc;
    private readonly Stopwatch m_throttleWatch;
    private readonly Stopwatch m_actualThrottleWatch;
    private readonly bool m_isThrottling;
    private readonly int m_throttleMilliseconds;
    private readonly int m_maxSleepTime;
    private readonly int m_totalObjectsToSerialize;
    private readonly int m_concurrencyTarpit = 50;
    private readonly int m_concurrencySize = 10000;
    private readonly int m_workUnitSize = 32;
    private const int c_throttleMilliSecond = 10;
    private const string c_layer = "GitPackWriterThrottler";
    private static int s_currentLargeCloneAndFetch = 0;
    private static int s_processorCount = Environment.ProcessorCount;

    public GitPackWriterThrottler(
      IVssRequestContext rc,
      int totalObjectsToSerialize,
      out bool isTarpit,
      int throttleMilliSecond = 10)
    {
      this.m_rc = rc;
      this.m_totalObjectsToSerialize = totalObjectsToSerialize;
      this.m_throttleMilliseconds = throttleMilliSecond;
      this.m_maxSleepTime = 10 * throttleMilliSecond;
      this.m_throttleWatch = Stopwatch.StartNew();
      this.m_actualThrottleWatch = new Stopwatch();
      IVssRequestContext vssRequestContext = rc.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      this.m_concurrencySize = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Git/Settings/ConcurrentCloneSelfThrottling/Size", 10000);
      this.m_concurrencyTarpit = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Git/Settings/ConcurrentCloneSelfThrottling/Tarpit", 50);
      this.m_workUnitSize = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Git/Settings/ConcurrentCloneSelfThrottling/WorkUnitSize", 32);
      isTarpit = false;
      if (totalObjectsToSerialize <= this.m_concurrencySize)
        return;
      Interlocked.Increment(ref GitPackWriterThrottler.s_currentLargeCloneAndFetch);
      this.m_isThrottling = true;
      if (GitPackWriterThrottler.s_currentLargeCloneAndFetch * 100 <= this.m_concurrencyTarpit * GitPackWriterThrottler.s_processorCount)
        return;
      isTarpit = true;
      this.m_rc.TraceAlways(1013811, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackWriterThrottler), Resources.Get("GitConcurrentCloneTarpitMessage"));
    }

    public int ThrottleIfBusy()
    {
      if (!this.m_isThrottling || this.m_totalObjectsToSerialize <= this.m_concurrencySize || GitPackWriterThrottler.s_currentLargeCloneAndFetch * 100 <= this.m_concurrencyTarpit * GitPackWriterThrottler.s_processorCount || this.m_throttleWatch.ElapsedMilliseconds <= (long) this.m_throttleMilliseconds)
        return 0;
      int millisecondsTimeout = Math.Min(this.m_throttleMilliseconds * (GitPackWriterThrottler.s_currentLargeCloneAndFetch * 2 / GitPackWriterThrottler.s_processorCount), this.m_maxSleepTime);
      this.m_totalDelayTime += millisecondsTimeout;
      ++this.m_totalDelayCount;
      this.m_actualThrottleWatch.Start();
      Thread.Sleep(millisecondsTimeout);
      this.m_actualThrottleWatch.Stop();
      this.m_throttleWatch.Restart();
      return millisecondsTimeout;
    }

    public void Dispose()
    {
      if (!this.m_isThrottling)
        return;
      Interlocked.Decrement(ref GitPackWriterThrottler.s_currentLargeCloneAndFetch);
      if (this.m_totalDelayTime <= 0)
        return;
      this.m_rc.TraceAlways(1013805, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackWriterThrottler), "This fetch was delayed {0} times by expected {1} ms with actual {2} ms from concurrent clone self-throttling", (object) this.m_totalDelayCount, (object) this.m_totalDelayTime, (object) this.m_actualThrottleWatch.ElapsedMilliseconds);
    }

    internal int ExpectedTotalDelayTimeMs => this.m_totalDelayTime;

    internal long ActualTotalDelayTimeMs => this.m_actualThrottleWatch.ElapsedMilliseconds;

    internal int TotalDelayCount => this.m_totalDelayCount;

    internal int WorkUnitSize => this.m_workUnitSize;
  }
}
