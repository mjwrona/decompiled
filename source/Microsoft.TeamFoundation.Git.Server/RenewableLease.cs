// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RenewableLease
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class RenewableLease : IDisposable
  {
    private double m_acquireTimeMs;
    private int m_timesRenewed;
    private readonly IVssRequestContext m_requestContext;
    private ILeaseInfo m_leaseInfo;
    private ILeaseService m_leaseSvc;
    private IVssServiceHost m_collectionServiceHost;
    private int m_renewLeaseCount;
    private readonly Stopwatch m_stopwatch;
    private Timer m_renewTimer;

    internal RenewableLease(
      IVssRequestContext requestContext,
      ILeaseService leaseService,
      string leaseName,
      TimeSpan leaseTime,
      TimeSpan renewLeaseTime,
      int renewLeaseCount,
      TimeSpan timeout,
      bool aggressive = false)
    {
      this.m_requestContext = requestContext;
      this.m_leaseSvc = leaseService;
      this.m_collectionServiceHost = requestContext.ServiceHost.CollectionServiceHost;
      this.m_renewLeaseCount = renewLeaseCount;
      this.m_stopwatch = Stopwatch.StartNew();
      this.m_leaseInfo = this.m_leaseSvc.AcquireLease(requestContext, leaseName, leaseTime, timeout, aggressive);
      this.m_acquireTimeMs = this.m_stopwatch.Elapsed.TotalMilliseconds;
      this.m_stopwatch.Restart();
      if (this.m_renewLeaseCount <= 0)
        return;
      this.m_renewTimer = new Timer(new TimerCallback(this.RenewLease), (object) null, renewLeaseTime, renewLeaseTime);
    }

    public void Dispose()
    {
      if (this.m_renewTimer != null)
      {
        using (ManualResetEvent notifyObject = new ManualResetEvent(false))
        {
          if (this.m_renewTimer.Dispose((WaitHandle) notifyObject))
            notifyObject.WaitOne();
          this.m_renewTimer = (Timer) null;
        }
      }
      if (this.m_leaseInfo != null)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        this.m_leaseInfo.Dispose();
        IVssRequestContext requestContext = this.m_requestContext;
        string traceArea = GitServerUtils.TraceArea;
        object[] objArray1 = new object[5]
        {
          (object) this.m_leaseInfo.LeaseName,
          (object) this.m_acquireTimeMs,
          (object) this.m_timesRenewed,
          null,
          null
        };
        TimeSpan elapsed = stopwatch.Elapsed;
        objArray1[3] = (object) elapsed.TotalMilliseconds;
        elapsed = this.m_stopwatch.Elapsed;
        objArray1[4] = (object) elapsed.TotalMilliseconds;
        string format = string.Format("Lease {0}\r\n                        Acquired in: {1:0.00}ms\r\n                        Renewed {2} times\r\n                        Released in: {3:0.00}ms\r\n                        Held for {4:0.00}ms total", objArray1);
        object[] objArray2 = Array.Empty<object>();
        requestContext.TraceAlways(1013879, TraceLevel.Info, traceArea, nameof (RenewableLease), format, objArray2);
      }
      else
        this.m_requestContext.TraceAlways(1013879, TraceLevel.Info, GitServerUtils.TraceArea, nameof (RenewableLease), "Lease was never acquired");
    }

    private void RenewLease(object state)
    {
      try
      {
        if (this.m_renewLeaseCount-- <= 0)
          return;
        using (IVssRequestContext systemContext = this.m_collectionServiceHost.DeploymentServiceHost.CreateSystemContext(false))
        {
          using (IVssRequestContext requestContext = systemContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(systemContext, this.m_collectionServiceHost.InstanceId, RequestContextType.SystemContext, false, false))
          {
            this.m_leaseSvc.RenewLease(requestContext, this.m_leaseInfo);
            ++this.m_timesRenewed;
          }
        }
      }
      catch (Exception ex)
      {
        this.m_renewLeaseCount = 0;
      }
    }
  }
}
