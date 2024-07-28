// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodeDiscoveryService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal class ServerNodeDiscoveryService : 
    SmartRouterFrameworkServiceBase,
    IServerNodeDiscoveryService,
    IVssFrameworkService,
    IHasSmartRouterBackgroundJob
  {
    public ServerNodeDiscoveryService()
      : base(SmartRouterBase.TraceLayer.BackEnd, true)
    {
      this.DiscoveredServerRecords = (IReadOnlyList<(ServerNodeRecord, ServerNodeWithHash)>) new List<(ServerNodeRecord, ServerNodeWithHash)>();
    }

    protected override void OnServiceStart(IVssRequestContext requestContext) => this.DiscoveryBackgroundJob.Start(requestContext, new Func<IVssRequestContext, Task>(this.DiscoverServersTimerCallbackAsync), requestContext.GetSmartRouterDiscoverRefreshPeriodSetting());

    protected override void OnServiceEnd(IVssRequestContext requestContext) => this.DiscoveryBackgroundJob.Stop(requestContext);

    public IReadOnlyList<ServerNodeWithHash> GetDiscoveredServerNodes(
      IVssRequestContext requestContext)
    {
      requestContext = this.CheckRequestContext(requestContext);
      if (!this.IsEnabled(requestContext))
        return (IReadOnlyList<ServerNodeWithHash>) Enumerable.Empty<ServerNodeWithHash>().ToList<ServerNodeWithHash>();
      IVssRequestContext deploymentHostContext = requestContext.ToDeploymentHostContext();
      ServerNode localServerNode = deploymentHostContext.GetServerNodePublisherService().GetLastPublishedLocalServerNode(deploymentHostContext);
      DateTime now = DateTime.UtcNow;
      return (IReadOnlyList<ServerNodeWithHash>) this.DiscoveredServerRecords.Where<(ServerNodeRecord, ServerNodeWithHash)>((Func<(ServerNodeRecord, ServerNodeWithHash), bool>) (item => item.ServerRecord.Expiration > now)).Select<(ServerNodeRecord, ServerNodeWithHash), ServerNodeWithHash>((Func<(ServerNodeRecord, ServerNodeWithHash), ServerNodeWithHash>) (item => item.ServerNode)).Where<ServerNodeWithHash>((Func<ServerNodeWithHash, bool>) (node => node.Server.IsColorMatch(localServerNode))).ToList<ServerNodeWithHash>();
    }

    public void Refresh(IVssRequestContext requestContext)
    {
      requestContext = this.CheckRequestContext(requestContext);
      if (!this.IsEnabled(requestContext))
        return;
      this.DiscoveryBackgroundJob.Invoke(requestContext);
    }

    private Task DiscoverServersTimerCallbackAsync(IVssRequestContext requestContext)
    {
      if (!this.IsEnabled(requestContext))
        return Task.CompletedTask;
      try
      {
        List<(ServerNodeRecord, ServerNodeWithHash)> list = requestContext.GetActiveServers("AT").Select<ServerNodeRecord, (ServerNodeRecord, ServerNodeWithHash)>((Func<ServerNodeRecord, (ServerNodeRecord, ServerNodeWithHash)>) (record => (record, this.ServerNodeHasher.CreateWithHash(new ServerNode(record.RoleName, record.RoleInstance, record.IPAddress))))).ToList<(ServerNodeRecord, ServerNodeWithHash)>();
        this.Tracer.TraceInfo(requestContext, SmartRouterBase.TracePoint.DiscoverServersSucceeded, "discovered servers, count={0}", (object) list.Count);
        foreach (ServerNodeWithHash serverNodeWithHash in list.Select<(ServerNodeRecord, ServerNodeWithHash), ServerNodeWithHash>((Func<(ServerNodeRecord, ServerNodeWithHash), ServerNodeWithHash>) (r => r.ServerNode)))
          this.Tracer.TraceInfo(requestContext, SmartRouterBase.TracePoint.DiscoverServersSucceeded, "discovered server, roleInstance={0}, ipAddress={1}, affinityCookie={2}", (object) serverNodeWithHash.Server.RoleInstance, (object) serverNodeWithHash.Server.IPAddress, (object) serverNodeWithHash.AffinityCookie);
        using (this.UpdateLock(requestContext.ToDeploymentHostContext()))
        {
          int count1 = this.DiscoveredServerRecords.Count;
          DateTime? lastUpdatedMax = this.LastUpdatedMax;
          int count2 = list.Count;
          DateTime? nullable1 = count2 > 0 ? new DateTime?(list.Max<(ServerNodeRecord, ServerNodeWithHash), DateTime>((Func<(ServerNodeRecord, ServerNodeWithHash), DateTime>) (item => item.ServerRecord.LastUpdated))) : new DateTime?();
          if (count2 == count1 && lastUpdatedMax.HasValue && nullable1.HasValue)
          {
            DateTime? nullable2 = nullable1;
            DateTime? nullable3 = lastUpdatedMax;
            if ((nullable2.HasValue & nullable3.HasValue ? (nullable2.GetValueOrDefault() > nullable3.GetValueOrDefault() ? 1 : 0) : 0) == 0)
              goto label_17;
          }
          this.LastUpdatedMax = nullable1;
          this.DiscoveredServerRecords = (IReadOnlyList<(ServerNodeRecord, ServerNodeWithHash)>) list;
        }
      }
      catch (Exception ex)
      {
        this.Tracer.TraceException(requestContext, SmartRouterBase.TracePoint.DiscoverServersFailed, ex, (string) null);
      }
label_17:
      return Task.CompletedTask;
    }

    private IDisposable UpdateLock(IVssRequestContext requestContext)
    {
      ILockName lockName = this.CreateLockName(requestContext, nameof (ServerNodeDiscoveryService));
      return requestContext.Lock(lockName);
    }

    SmartRouterBackgroundJob IHasSmartRouterBackgroundJob.BackgroundJob => this.DiscoveryBackgroundJob;

    private IReadOnlyList<(ServerNodeRecord ServerRecord, ServerNodeWithHash ServerNode)> DiscoveredServerRecords { get; set; }

    private DateTime? LastUpdatedMax { get; set; }

    private SmartRouterBackgroundJob DiscoveryBackgroundJob { get; } = new SmartRouterBackgroundJob(nameof (ServerNodeDiscoveryService));

    private IServerNodeHasher ServerNodeHasher { get; } = (IServerNodeHasher) new Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodeHasher();
  }
}
