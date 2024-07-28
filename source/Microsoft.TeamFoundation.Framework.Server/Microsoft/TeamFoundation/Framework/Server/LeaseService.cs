// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LeaseService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LeaseService : ILeaseService, IVssFrameworkService, IInternalLeaseService
  {
    private const string c_area = "LeaseService";
    private const string c_layer = "IVssFrameworkService";
    private const int c_maxLeaseTimeInSeconds = 300;
    private const int c_minLeaseTimeInSeconds = 1;
    private const int c_maxRequestAcquiringLease = 5000;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ILeaseInfo AcquireLease(
      IVssRequestContext requestContext,
      string leaseName,
      TimeSpan leaseTime,
      TimeSpan timeout,
      bool aggressive = false)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) "LeaseService.AcquireLease").AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionMaxConcurrentRequests(5000));
      return new CommandService<ILeaseInfo>(requestContext, setter, (Func<ILeaseInfo>) (() =>
      {
        (ILeaseInfo leaseInfo2, TimeSpan timeSpan2) = this.AcquireLeaseInternal(requestContext, leaseName, leaseTime, timeout, aggressive);
        return leaseInfo2 != null ? leaseInfo2 : throw new AcquireLeaseTimeoutException(leaseName, leaseTime, timeout, timeSpan2);
      })).Execute();
    }

    public bool TryAcquireLease(
      IVssRequestContext requestContext,
      string leaseName,
      TimeSpan leaseTime,
      TimeSpan timeout,
      out ILeaseInfo leaseInfo,
      bool aggressive = false)
    {
      (leaseInfo, _) = this.AcquireLeaseInternal(requestContext, leaseName, leaseTime, timeout, aggressive);
      return leaseInfo != null;
    }

    private (ILeaseInfo leaseInfo, TimeSpan acquireTime) AcquireLeaseInternal(
      IVssRequestContext requestContext,
      string leaseName,
      TimeSpan leaseTime,
      TimeSpan timeout,
      bool aggressive)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1031385303, nameof (LeaseService), "IVssFrameworkService", nameof (AcquireLeaseInternal));
      try
      {
        ArgumentUtility.CheckForNull<string>(leaseName, nameof (leaseName));
        ArgumentUtility.CheckForOutOfRange<double>(leaseTime.TotalSeconds, nameof (leaseTime), 1.0, 300.0);
        ArgumentUtility.CheckForOutOfRange<double>(timeout.TotalSeconds, nameof (timeout), 0.0);
        Guid leaseOwner = Guid.NewGuid();
        Guid processId = requestContext.To(TeamFoundationHostType.Deployment).GetService<IInternalTeamFoundationHostManagementService>().ProcessId;
        Stopwatch stopwatch = Stopwatch.StartNew();
        int attempt = 0;
        while (true)
        {
          bool flag = false;
          using (LeaseComponent component = requestContext.CreateComponent<LeaseComponent>())
          {
            flag = component.AcquireLease(leaseName, leaseTime, processId, leaseOwner);
            ++attempt;
          }
          if (!flag)
          {
            double num = timeout.TotalMilliseconds - stopwatch.Elapsed.TotalMilliseconds;
            if (num > 0.0)
            {
              TimeSpan minBackoff = aggressive ? TimeSpan.FromMilliseconds(50.0) : TimeSpan.FromSeconds(1.0);
              TimeSpan deltaBackoff = aggressive ? TimeSpan.FromMilliseconds(50.0) : TimeSpan.FromSeconds(1.0);
              TimeSpan timeSpan = TimeSpan.FromMilliseconds(Math.Max((aggressive ? TimeSpan.FromMilliseconds(200.0) : TimeSpan.FromSeconds(30.0)).TotalMilliseconds, stopwatch.Elapsed.TotalMilliseconds / 10.0));
              minBackoff = minBackoff.TotalMilliseconds < num ? minBackoff : TimeSpan.FromMilliseconds(num);
              TimeSpan maxBackoff = timeSpan.TotalMilliseconds < num ? timeSpan : TimeSpan.FromMilliseconds(num);
              TimeSpan exponentialBackoff = BackoffTimerHelper.GetExponentialBackoff(attempt, minBackoff, maxBackoff, deltaBackoff);
              requestContext.Trace(755484177, TraceLevel.Info, nameof (LeaseService), "IVssFrameworkService", "Failed to acquire lease on {0}, {1}, {2}", (object) leaseName, (object) leaseOwner, (object) stopwatch.ElapsedMilliseconds);
              requestContext.CancellationToken.WaitHandle.WaitOne(exponentialBackoff);
              requestContext.ThrowIfCanceled();
            }
            else
              break;
          }
          else
            goto label_11;
        }
        requestContext.TraceAlways(755484178, TraceLevel.Error, nameof (LeaseService), "IVssFrameworkService", "Failed to acquire a lease after multiple attempts. Lease name: {0}. Attempts: {1}. Elapsed time: {2}ms.", (object) leaseName, (object) attempt, (object) stopwatch.ElapsedMilliseconds);
        return ((ILeaseInfo) null, stopwatch.Elapsed);
label_11:
        if (attempt > 1)
          requestContext.TraceAlways(755484179, TraceLevel.Warning, nameof (LeaseService), "IVssFrameworkService", "Acquiring a lease took multiple attempts. Lease name: {0}. Attempts: {1}. Elapsed time: {2}ms.", (object) leaseName, (object) attempt, (object) stopwatch.ElapsedMilliseconds);
        requestContext.Trace(1413375250, TraceLevel.Info, nameof (LeaseService), "IVssFrameworkService", "Acquired lease on {0}, {1}, {2}", (object) leaseName, (object) leaseOwner, (object) stopwatch.ElapsedMilliseconds);
        ILeaseInfo lease = (ILeaseInfo) new LeaseService.LeaseInfo(requestContext, leaseName, leaseOwner, leaseTime, processId);
        requestContext.RequestContextInternal().AddLease(lease);
        return (lease, stopwatch.Elapsed);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1712518514, nameof (LeaseService), "IVssFrameworkService", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(962553118, nameof (LeaseService), "IVssFrameworkService", nameof (AcquireLeaseInternal));
      }
    }

    public void RenewLease(IVssRequestContext requestContext, ILeaseInfo info)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(14727182, nameof (LeaseService), "IVssFrameworkService", nameof (RenewLease));
      try
      {
        ArgumentUtility.CheckForNull<ILeaseInfo>(info, nameof (info));
        if (!(info is LeaseService.LeaseInfo leaseInfo))
          throw new ArgumentException("info must be of type LeaseInfo", nameof (info));
        using (LeaseComponent component = requestContext.CreateComponent<LeaseComponent>())
        {
          component.RenewLease(info.LeaseName, info.LeaseOwner, info.LeaseTime);
          DateTime dateTime = DateTime.UtcNow.Add(info.LeaseTime);
          leaseInfo.LeaseExpires = dateTime;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(306849578, nameof (LeaseService), "IVssFrameworkService", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(75478559, nameof (LeaseService), "IVssFrameworkService", nameof (RenewLease));
      }
    }

    void IInternalLeaseService.ReleaseLease(
      IVssRequestContext requestContext,
      string leaseName,
      Guid leaseOwner)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(369930317, nameof (LeaseService), "IVssFrameworkService", "ReleaseLease");
      try
      {
        ArgumentUtility.CheckForNull<string>(leaseName, nameof (leaseName));
        ArgumentUtility.CheckForEmptyGuid(leaseOwner, nameof (leaseOwner));
        using (LeaseComponent component = requestContext.CreateComponent<LeaseComponent>())
          component.ReleaseLease(leaseName, leaseOwner);
        requestContext.RequestContextInternal().RemoveLease(leaseName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(306849578, nameof (LeaseService), "IVssFrameworkService", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(49078382, nameof (LeaseService), "IVssFrameworkService", "ReleaseLease");
      }
    }

    private class LeaseInfo : ILeaseInfo, IDisposable
    {
      private IVssRequestContext m_requestContext;

      internal LeaseInfo(
        IVssRequestContext requestContext,
        string leaseName,
        Guid leaseOwner,
        TimeSpan leaseTime,
        Guid processId)
      {
        this.m_requestContext = requestContext;
        this.LeaseName = leaseName;
        this.LeaseOwner = leaseOwner;
        this.LeaseObtained = DateTime.UtcNow;
        this.LeaseExpires = DateTime.UtcNow.Add(leaseTime);
        this.LeaseTime = leaseTime;
        this.ProcessId = processId;
      }

      public void Dispose()
      {
        if (this.m_requestContext == null)
          return;
        this.m_requestContext.GetService<IInternalLeaseService>().ReleaseLease(this.m_requestContext, this.LeaseName, this.LeaseOwner);
        this.m_requestContext = (IVssRequestContext) null;
      }

      public void Renew()
      {
        if (this.m_requestContext == null)
          throw new ObjectDisposedException(nameof (LeaseInfo));
        this.m_requestContext.GetService<ILeaseService>().RenewLease(this.m_requestContext, (ILeaseInfo) this);
      }

      public string LeaseName { get; private set; }

      public Guid LeaseOwner { get; private set; }

      public DateTime LeaseObtained { get; private set; }

      public DateTime LeaseExpires { get; internal set; }

      public Guid ProcessId { get; private set; }

      public TimeSpan LeaseTime { get; private set; }
    }
  }
}
