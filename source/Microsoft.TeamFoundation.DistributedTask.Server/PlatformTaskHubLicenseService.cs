// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformTaskHubLicenseService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformTaskHubLicenseService : ITaskHubLicenseService, IVssFrameworkService
  {
    private VssRefreshCache<IList<ResourceLimit>> m_resourceLimits;
    private Guid m_hostId;
    private readonly ISecurityProvider m_securityProvider;
    private const string c_layer = "PlatformTaskHubLicenseService";

    internal PlatformTaskHubLicenseService()
      : this((ISecurityProvider) new DefaultSecurityProvider())
    {
    }

    internal PlatformTaskHubLicenseService(ISecurityProvider security) => this.m_securityProvider = security;

    public string GetResourceThrottlingType(
      IVssRequestContext requestContext,
      TaskAgentPoolReference poolReference,
      TaskAgentCloud agentCloud,
      string parallelismTag)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !requestContext.IsFeatureEnabled("DistributedTask.PipelineBillingModel2") || poolReference.PoolType == TaskAgentPoolType.Deployment)
        return "DEFAULTPARALLELISM";
      string parallelismTag1 = parallelismTag;
      int num;
      if (!poolReference.IsHosted)
      {
        if (agentCloud == null)
        {
          num = 0;
        }
        else
        {
          bool? nullable = agentCloud.Internal;
          bool flag = true;
          num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
        }
      }
      else
        num = 1;
      return ResourceLimitUtil.GetResourceThrottlingType(parallelismTag1, num != 0);
    }

    public TaskHubLicenseDetails GetTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      bool includeEnterpriseUsersCount = false,
      bool includeHostedAgentMinutesCount = false)
    {
      using (new MethodScope(requestContext, nameof (PlatformTaskHubLicenseService), nameof (GetTaskHubLicenseDetails)))
      {
        requestContext.CheckServiceHostId(this.m_hostId, (IVssFrameworkService) this);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(hubName, nameof (hubName));
        return this.GetLicenseDetails(requestContext, hubName, includeEnterpriseUsersCount, includeHostedAgentMinutesCount);
      }
    }

    public TaskHubLicenseDetails UpdateTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      TaskHubLicenseDetails taskHubLicenseDetails)
    {
      using (new MethodScope(requestContext, nameof (PlatformTaskHubLicenseService), nameof (UpdateTaskHubLicenseDetails)))
      {
        requestContext.CheckServiceHostId(this.m_hostId, (IVssFrameworkService) this);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(hubName, nameof (hubName));
        ArgumentUtility.CheckForNull<TaskHubLicenseDetails>(taskHubLicenseDetails, nameof (taskHubLicenseDetails));
        this.m_securityProvider.CheckTaskHubLicensePermission(requestContext, 2);
        requestContext.GetExtension<ITaskHubLicensePlugin>().UpdateTaskHubLicenseDetails(requestContext, hubName, taskHubLicenseDetails);
        return this.GetLicenseDetails(requestContext, hubName, true, true);
      }
    }

    public virtual IList<ResourceLimit> GetResourceLimits(
      IVssRequestContext requestContext,
      bool allowStaleValues = true)
    {
      using (new MethodScope(requestContext, nameof (PlatformTaskHubLicenseService), nameof (GetResourceLimits)))
      {
        requestContext.CheckServiceHostId(this.m_hostId, (IVssFrameworkService) this);
        this.m_securityProvider.CheckTaskHubLicensePermission(requestContext, 1);
        return allowStaleValues ? this.m_resourceLimits.Get(requestContext) : this.ComputeResourceLimits(requestContext);
      }
    }

    public void NotifyDataChanged(
      IVssRequestContext requestContext,
      OfferSubscriptionQuantityChangeMessage message)
    {
      requestContext.CheckServiceHostId(this.m_hostId, (IVssFrameworkService) this);
      this.OnCommerceDataChanged(requestContext, (NotificationEventArgs) null);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, "DistributedTask", TaskConstants.CommerceDataChangedNotification, message.MeterGalleryId);
    }

    internal int? GetUsedHostedMinutesForPrivateProjects(IVssRequestContext requestContext)
    {
      requestContext.CheckServiceHostId(this.m_hostId, (IVssFrameworkService) this);
      this.m_securityProvider.CheckTaskHubLicensePermission(requestContext, 1);
      return requestContext.GetExtension<ITaskHubLicensePlugin>().GetUsedHostedMinutesForPrivateProjects(requestContext);
    }

    private IList<ResourceLimit> ComputeResourceLimits(IVssRequestContext requestContext)
    {
      this.m_securityProvider.CheckTaskHubLicensePermission(requestContext, 1);
      return requestContext.GetExtension<ITaskHubLicensePlugin>().GetResourceLimits(requestContext);
    }

    internal int GetEnterpriseUsersCount(IVssRequestContext requestContext)
    {
      requestContext.CheckServiceHostId(this.m_hostId, (IVssFrameworkService) this);
      return requestContext.GetExtension<ITaskHubLicensePlugin>().GetEnterpriseUsersCount(requestContext);
    }

    private static bool ShouldEnforceLicensing(
      IVssRequestContext collectionRequestContext,
      string hubName)
    {
      bool hostedDeployment = collectionRequestContext.ExecutionEnvironment.IsHostedDeployment;
      bool flag = collectionRequestContext.IsFeatureEnabled("WebAccess.BuildAndRelease.ResourceLimits");
      return hubName.Equals("Release", StringComparison.OrdinalIgnoreCase) & flag | hubName.Equals("Build", StringComparison.OrdinalIgnoreCase) & hostedDeployment & flag;
    }

    private TaskHubLicenseDetails GetLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      bool includeEnterpriseUsersCount = false,
      bool includeHostedAgentMinutesCount = false)
    {
      this.m_securityProvider.CheckTaskHubLicensePermission(requestContext, 1);
      TaskHubLicenseDetails hubLicenseDetails = requestContext.GetExtension<ITaskHubLicensePlugin>().GetTaskHubLicenseDetails(requestContext, hubName, includeEnterpriseUsersCount, includeHostedAgentMinutesCount);
      if (!PlatformTaskHubLicenseService.ShouldEnforceLicensing(requestContext, hubName))
      {
        hubLicenseDetails.FreePrivateLicenseCount = int.MaxValue;
        hubLicenseDetails.TotalLicenseCount = int.MaxValue;
        hubLicenseDetails.HasLicenseCountEverUpdated = !requestContext.ExecutionEnvironment.IsHostedDeployment;
      }
      return hubLicenseDetails;
    }

    private void OnCommerceDataChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      this.m_resourceLimits = new VssRefreshCache<IList<ResourceLimit>>(TimeSpan.FromMinutes(30.0), new Func<IVssRequestContext, IList<ResourceLimit>>(this.ComputeResourceLimits), true);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        systemRequestContext.CheckProjectCollectionRequestContext();
      this.m_hostId = systemRequestContext.ServiceHost.InstanceId;
      this.m_resourceLimits = new VssRefreshCache<IList<ResourceLimit>>(TimeSpan.FromMinutes(30.0), new Func<IVssRequestContext, IList<ResourceLimit>>(this.ComputeResourceLimits), true);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "DistributedTask", TaskConstants.CommerceDataChangedNotification, new SqlNotificationHandler(this.OnCommerceDataChanged), true);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "DistributedTask", TaskConstants.CommerceDataChangedNotification, new SqlNotificationHandler(this.OnCommerceDataChanged), false);
      this.m_resourceLimits = (VssRefreshCache<IList<ResourceLimit>>) null;
    }
  }
}
