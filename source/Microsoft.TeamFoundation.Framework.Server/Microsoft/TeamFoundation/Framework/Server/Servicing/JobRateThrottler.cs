// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Servicing.JobRateThrottler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Servicing
{
  internal class JobRateThrottler
  {
    internal int m_maxUpgradeJobs;
    internal int m_currentUpgradeJobs;
    internal int m_heat;
    internal const int c_growth = 2;
    private Stopwatch m_refreshWatch = new Stopwatch();
    internal TimeSpan m_refreshInterval = TimeSpan.FromSeconds(DatabaseManagementConstants.PerfCacheRefreshInterval);
    internal int m_maxReleaseIterations = 2;
    internal int m_maxThrottleIterations = 2;
    protected static readonly string s_area = "UpgradeManagement";
    protected static readonly string s_layer = "Throttler";

    public JobRateThrottler(IVssRequestContext requestContext)
    {
      this.RefreshMaxUpgradeJobs(requestContext);
      this.m_currentUpgradeJobs = Math.Max(this.m_maxUpgradeJobs / 2, 1);
      this.m_refreshWatch.Start();
    }

    protected virtual void RefreshMaxUpgradeJobs(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(106010, JobRateThrottler.s_area, JobRateThrottler.s_layer, nameof (RefreshMaxUpgradeJobs));
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.ServicingMaxUpgradeJobs, 0);
      if (num != this.m_maxUpgradeJobs)
      {
        this.m_maxUpgradeJobs = num;
        requestContext.Trace(106011, TraceLevel.Info, JobRateThrottler.s_area, JobRateThrottler.s_layer, "Updated Max Upgrade Jobs: {0}", (object) this.m_maxUpgradeJobs);
        this.m_currentUpgradeJobs = Math.Min(this.m_currentUpgradeJobs, this.m_maxUpgradeJobs);
      }
      requestContext.TraceLeave(106010, JobRateThrottler.s_area, JobRateThrottler.s_layer, nameof (RefreshMaxUpgradeJobs));
    }

    public int GetMaxUpgradeJobCount(IVssRequestContext requestContext)
    {
      this.RefreshMaxUpgradeJobs(requestContext);
      if (this.m_refreshWatch.Elapsed >= this.m_refreshInterval)
      {
        ITeamFoundationDatabaseManagementService service = requestContext.GetService<ITeamFoundationDatabaseManagementService>();
        ITeamFoundationDatabaseProperties databaseProperties1 = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties;
        IVssRequestContext deploymentContext = requestContext;
        ITeamFoundationDatabaseProperties databaseProperties2 = databaseProperties1;
        string reason;
        ref string local = ref reason;
        this.m_heat = !service.ThrottleDatabaseAccess(deploymentContext, databaseProperties2, out local) ? Math.Max(this.m_heat - 1, -this.m_maxReleaseIterations) : Math.Min(this.m_heat + 1, this.m_maxThrottleIterations);
        if (this.m_heat == this.m_maxThrottleIterations)
          this.Throttle(requestContext, reason);
        else if (this.m_heat == -this.m_maxReleaseIterations)
          this.Release(requestContext);
        this.m_refreshWatch.Restart();
      }
      return this.m_currentUpgradeJobs;
    }

    private void Throttle(IVssRequestContext requestContext, string reason)
    {
      int num = Math.Min(Math.Max(this.m_currentUpgradeJobs - 2, 1), this.m_maxUpgradeJobs);
      if (num == this.m_currentUpgradeJobs)
        return;
      requestContext.TraceAlways(106101, TraceLevel.Info, JobRateThrottler.s_area, JobRateThrottler.s_layer, "Throttling upgrade job count from: {0} to: {1}. Reason: {2}", (object) this.m_currentUpgradeJobs, (object) num, (object) reason);
      this.m_currentUpgradeJobs = num;
    }

    private void Release(IVssRequestContext requestContext)
    {
      int num = Math.Min(this.m_currentUpgradeJobs + 2, this.m_maxUpgradeJobs);
      if (num == this.m_currentUpgradeJobs)
        return;
      requestContext.TraceAlways(106101, TraceLevel.Info, JobRateThrottler.s_area, JobRateThrottler.s_layer, "Raising upgrade job count from: {0} to: {1}.", (object) this.m_currentUpgradeJobs, (object) num);
      this.m_currentUpgradeJobs = num;
    }
  }
}
