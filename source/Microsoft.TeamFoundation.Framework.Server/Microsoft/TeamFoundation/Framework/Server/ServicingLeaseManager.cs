// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingLeaseManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingLeaseManager : IServicingLeaseManager, IDisposable
  {
    private readonly IDictionary<string, ILeaseInfo> m_activeLeases = (IDictionary<string, ILeaseInfo>) new Dictionary<string, ILeaseInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private TeamFoundationTask m_renewalTask;
    private readonly object m_lock = new object();
    private bool m_disposed;
    private readonly IInternalServicingContext m_servicingContext;
    private static readonly TimeSpan s_renewalWindow = TimeSpan.FromMinutes(1.0);
    private static readonly TimeSpan s_minDuration = ServicingLeaseManager.s_renewalWindow + ServicingLeaseManager.s_renewalWindow;
    private static readonly TimeSpan s_timerInterval = TimeSpan.FromSeconds(10.0);

    internal ServicingLeaseManager(IInternalServicingContext servicingContext) => this.m_servicingContext = servicingContext ?? throw new ArgumentNullException(nameof (servicingContext));

    public void AcquireLease(string leaseName, TimeSpan leaseDuration, TimeSpan acquisitionTimeout)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(leaseName, nameof (leaseName));
      ArgumentUtility.CheckForOutOfRange<TimeSpan>(leaseDuration, nameof (leaseDuration), ServicingLeaseManager.s_minDuration);
      lock (this.m_lock)
      {
        this.ThrowIfDisposed();
        if (this.m_activeLeases.ContainsKey(leaseName))
          throw new InvalidOperationException("Servicing lease " + leaseName + " already exists");
        if (this.m_renewalTask == null)
        {
          ITeamFoundationTaskService service = this.m_servicingContext.DeploymentRequestContext.GetService<ITeamFoundationTaskService>();
          this.m_renewalTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.RenewLeasesPeriodically), (object) null, (int) ServicingLeaseManager.s_timerInterval.TotalMilliseconds);
          IVssRequestContext deploymentRequestContext = this.m_servicingContext.DeploymentRequestContext;
          TeamFoundationTask renewalTask = this.m_renewalTask;
          service.AddTask(deploymentRequestContext, renewalTask);
        }
        ILeaseInfo leaseInfo = this.m_servicingContext.DeploymentRequestContext.GetService<ILeaseService>().AcquireLease(this.m_servicingContext.DeploymentRequestContext, leaseName, leaseDuration, acquisitionTimeout);
        this.m_activeLeases.Add(leaseName, leaseInfo);
        this.m_servicingContext.LogInfo(string.Format("Acquired a servicing lease for {0} with a duration {1}, this will be automatically renewed for the duration of the servicing context.", (object) leaseName, (object) leaseDuration));
      }
    }

    public void ThrowIfLeaseNotHeld(string leaseName)
    {
      lock (this.m_lock)
      {
        this.ThrowIfDisposed();
        ILeaseInfo leaseInfo;
        if (!this.m_activeLeases.TryGetValue(leaseName, out leaseInfo) || leaseInfo.LeaseExpires < DateTime.UtcNow)
          throw new LeaseNotHeldException(leaseName);
      }
    }

    private void RenewLeasesPeriodically(IVssRequestContext taskRequestContext, object _)
    {
      lock (this.m_lock)
      {
        this.ThrowIfDisposed();
        foreach (ILeaseInfo leaseInfo in (IEnumerable<ILeaseInfo>) this.m_activeLeases.Values)
        {
          DateTime utcNow = DateTime.UtcNow;
          if (leaseInfo.LeaseExpires <= utcNow)
          {
            this.AbandonShip(taskRequestContext, leaseInfo);
            break;
          }
          if (leaseInfo.LeaseExpires - utcNow < ServicingLeaseManager.s_renewalWindow)
            taskRequestContext.GetService<ILeaseService>().RenewLease(taskRequestContext, leaseInfo);
        }
      }
    }

    private void AbandonShip(
      IVssRequestContext currentThreadRequestContext,
      ILeaseInfo failedLeaseInfo)
    {
      string str = string.Format("A servicing lease named {0} expired after {1} before the operation completed.", (object) failedLeaseInfo.LeaseName, (object) (failedLeaseInfo.LeaseExpires - failedLeaseInfo.LeaseObtained));
      this.m_servicingContext.Error(str);
      this.m_servicingContext.DeploymentRequestContext?.Cancel(str);
      this.m_servicingContext.GetTargetRequestContext(false)?.Cancel(str);
      this.CancelRenewalTask(currentThreadRequestContext);
    }

    public void Dispose()
    {
      lock (this.m_lock)
      {
        this.m_disposed = !this.m_disposed ? true : throw new ObjectDisposedException(nameof (ServicingLeaseManager));
        foreach (string key in (IEnumerable<string>) this.m_activeLeases.Keys.ToList<string>())
        {
          this.m_activeLeases[key].Dispose();
          this.m_activeLeases.Remove(key);
        }
        if (this.m_renewalTask == null)
          return;
        this.CancelRenewalTask(this.m_servicingContext.DeploymentRequestContext);
      }
    }

    private void ThrowIfDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(nameof (ServicingLeaseManager));
    }

    private void CancelRenewalTask(IVssRequestContext currentThreadRequestContext)
    {
      currentThreadRequestContext.GetService<ITeamFoundationTaskService>().RemoveTask(this.m_servicingContext.DeploymentRequestContext, this.m_renewalTask);
      this.m_renewalTask = (TeamFoundationTask) null;
    }

    public void ReleaseLease(string leaseName)
    {
      lock (this.m_lock)
      {
        if (!this.m_activeLeases.ContainsKey(leaseName))
          throw new LeaseNotHeldException(leaseName);
        this.m_activeLeases[leaseName].Dispose();
        this.m_activeLeases.Remove(leaseName);
        this.m_servicingContext.LogInfo("Released a servicing lease for " + leaseName);
      }
    }
  }
}
