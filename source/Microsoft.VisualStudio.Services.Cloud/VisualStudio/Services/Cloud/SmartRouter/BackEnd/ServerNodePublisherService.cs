// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodePublisherService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal class ServerNodePublisherService : 
    SmartRouterFrameworkServiceBase,
    IServerNodePublisherService,
    IVssFrameworkService,
    IHasSmartRouterBackgroundJob
  {
    public ServerNodePublisherService()
      : base(SmartRouterBase.TraceLayer.BackEnd, true)
    {
    }

    protected override void OnServiceStart(IVssRequestContext requestContext) => this.StartPublishing(requestContext);

    protected override void OnServiceEnd(IVssRequestContext requestContext) => this.StopPublishing(requestContext);

    public ServerNode? GetLastPublishedLocalServerNode(IVssRequestContext requestContext)
    {
      requestContext = this.CheckRequestContext(requestContext);
      return !this.IsEnabled(requestContext) ? (ServerNode) null : this.LastPublishedServerNode;
    }

    private Task PublishTimerCallbackAsync(IVssRequestContext requestContext)
    {
      if (this.IsEnabled(requestContext))
      {
        ServerNodeRecord serverNodeRecord = this.GetLocalhostServerNodeRecord(requestContext);
        if (serverNodeRecord != null)
        {
          this.LastPublishedServerRecord = requestContext.AddOrUpdateServer(serverNodeRecord);
          this.LastPublishedServerNode = new ServerNode(serverNodeRecord.RoleName, serverNodeRecord.RoleInstance, serverNodeRecord.IPAddress);
          this.Tracer.TraceInfo(requestContext, SmartRouterBase.TracePoint.PublishServer, "published server: roleInstance={0}, ipAddress={1}", (object) serverNodeRecord.RoleInstance, (object) serverNodeRecord.IPAddress);
          this.OnUpdated(requestContext);
        }
      }
      return Task.CompletedTask;
    }

    private void Unpublish(IVssRequestContext requestContext)
    {
      ServerNodeRecord publishedServerRecord = this.LastPublishedServerRecord;
      this.LastPublishedServerRecord = (ServerNodeRecord) null;
      if (publishedServerRecord == null)
        return;
      requestContext.DeleteServer(publishedServerRecord.RoleName, publishedServerRecord.RoleInstance);
      this.OnUpdated(requestContext);
    }

    private void StartPublishing(IVssRequestContext requestContext) => this.PublishJob.Start(requestContext, new Func<IVssRequestContext, Task>(this.PublishTimerCallbackAsync), requestContext.GetSmartRouterPublishRefreshPeriodSetting());

    private void StopPublishing(IVssRequestContext requestContext)
    {
      this.PublishJob.Stop(requestContext);
      this.Unpublish(requestContext);
    }

    private void OnUpdated(IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentHostContext = requestContext.ToDeploymentHostContext();
      try
      {
        deploymentHostContext.GetServerNodeDiscoveryService().Refresh(deploymentHostContext);
      }
      catch (Exception ex)
      {
        this.Tracer.TraceException(requestContext, SmartRouterBase.TracePoint.DiscoveryRefreshException, ex, (string) null);
      }
    }

    private ServerNodeRecord? GetLocalhostServerNodeRecord(IVssRequestContext requestContext)
    {
      string localhostIpAddress = this.GetLocalhostIPAddress(requestContext);
      return localhostIpAddress == null ? (ServerNodeRecord) null : ServerNodeRecord.CreateWithDefaults(requestContext, this.CurrentRoleName, this.CurrentRoleInstanceId, localhostIpAddress);
    }

    protected internal virtual string? GetLocalhostIPAddress(IVssRequestContext requestContext) => IpAddressUtility.GetLocalHostIpAddress(requestContext).ipAddress;

    SmartRouterBackgroundJob IHasSmartRouterBackgroundJob.BackgroundJob => this.PublishJob;

    internal ServerNodeRecord? LastPublishedServerRecord { get; private set; }

    internal ServerNode? LastPublishedServerNode { get; private set; }

    protected internal virtual string CurrentRoleName => !AzureRoleUtil.IsAvailable ? "AT" : AzureRoleUtil.Environment.CurrentRoleName;

    protected internal virtual string CurrentRoleInstanceId => !AzureRoleUtil.IsAvailable ? Environment.GetEnvironmentVariable("COMPUTERNAME") : AzureRoleUtil.Environment.CurrentRoleInstanceId;

    private SmartRouterBackgroundJob PublishJob { get; } = new SmartRouterBackgroundJob(nameof (PublishJob));
  }
}
