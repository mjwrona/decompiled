// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.Service.ResourceUsageService
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ResourceUsage.Server.Service
{
  public class ResourceUsageService : IResourceUsageService, IVssFrameworkService
  {
    private const string c_projectLimitRegistryPath = "/Service/Framework/ServerCore/ProjectLimit";
    private const int c_defaultProjectLimit = 1000;
    private static readonly RegistryQuery s_projectLimitRegistryQuery = new RegistryQuery("/Service/Framework/ServerCore/ProjectLimit");
    private const string s_layer = "ResourceUsageService";

    public virtual Dictionary<string, Usage> GetLimits(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(94000021, this.Area, nameof (ResourceUsageService), nameof (GetLimits));
      ResourceUsageCacheService service1 = requestContext.GetService<ResourceUsageCacheService>();
      Guid hostId = requestContext.GetRequestDetails().HostId;
      Dictionary<string, Usage> limits1;
      if (service1.TryGetValue(requestContext, hostId, out limits1))
        return limits1;
      TaggingService service2 = requestContext.GetService<TaggingService>();
      PlatformProjectService service3 = requestContext.GetService<PlatformProjectService>();
      IVssRequestContext requestContext1 = requestContext;
      (int num1, int num2) = service2.GetTagCount(requestContext1);
      int projectCount = service3.GetProjectCount(requestContext);
      int limit = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in ResourceUsageService.s_projectLimitRegistryQuery, true, 1000);
      Dictionary<string, Usage> limits2 = new Dictionary<string, Usage>()
      {
        ["Projects"] = new Usage(projectCount, limit),
        ["WorkItemTags"] = new Usage(num1, num2)
      };
      requestContext.TraceLeave(94000022, this.Area, nameof (ResourceUsageService), nameof (GetLimits));
      service1.TryAdd(requestContext, hostId, limits2);
      return limits2;
    }

    public virtual Dictionary<string, Usage> GetProjectLimits(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      requestContext.TraceEnter(94000041, this.Area, nameof (ResourceUsageService), nameof (GetProjectLimits));
      ResourceUsageCacheService service1 = requestContext.GetService<ResourceUsageCacheService>();
      Dictionary<string, Usage> projectLimits1;
      if (service1.TryGetValue(requestContext, projectId, out projectLimits1))
        return projectLimits1;
      Dictionary<string, Usage> projectLimits2 = new Dictionary<string, Usage>();
      DashboardService service2 = requestContext.GetService<DashboardService>();
      ITeamService service3 = requestContext.GetService<ITeamService>();
      IScaledAgileViewService service4 = requestContext.GetService<IScaledAgileViewService>();
      WorkItemTrackingTreeService service5 = requestContext.GetService<WorkItemTrackingTreeService>();
      projectLimits2["Teams"] = this.GetTeamsUsage(requestContext, service3, projectId);
      projectLimits2["Dashboards"] = this.GetDashboardsUsage(requestContext, service2, projectId);
      projectLimits2["DeliveryPlans"] = this.GetDeliveryPlansUsage(requestContext, service4, projectId);
      projectLimits2["Area"] = this.GetAreaUsage(requestContext, service5, projectId);
      projectLimits2["Iterations"] = this.GetIterationsUsage(requestContext, service5, projectId);
      requestContext.TraceLeave(94000042, this.Area, nameof (ResourceUsageService), nameof (GetProjectLimits));
      service1.TryAdd(requestContext, projectId, projectLimits2);
      return projectLimits2;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private Usage GetTeamsUsage(
      IVssRequestContext requestContext,
      ITeamService teamService,
      Guid projectId)
    {
      (int teamCount, int teamsLimit) andLimitByProject = teamService.GetTeamCountAndLimitByProject(requestContext, projectId);
      return new Usage(andLimitByProject.teamCount, andLimitByProject.teamsLimit);
    }

    private Usage GetDashboardsUsage(
      IVssRequestContext requestContext,
      DashboardService dashboardService,
      Guid projectId)
    {
      (int DashboardCount, int MaxAllowedDashboards) countByProjectId = dashboardService.GetDashboardCountByProjectId(requestContext, projectId);
      return new Usage(countByProjectId.DashboardCount, countByProjectId.MaxAllowedDashboards);
    }

    private Usage GetDeliveryPlansUsage(
      IVssRequestContext requestContext,
      IScaledAgileViewService deliveryPlansService,
      Guid projectId)
    {
      (int plansCount, int maxPlans) plansCount = deliveryPlansService.GetPlansCount(requestContext, projectId);
      return new Usage(plansCount.plansCount, plansCount.maxPlans);
    }

    private Usage GetAreaUsage(
      IVssRequestContext requestContext,
      WorkItemTrackingTreeService workItemTrackingTreeService,
      Guid projectId)
    {
      (int Count, int PathLimit) tuple = workItemTrackingTreeService.FetchAreaAndIterationCountWithLimits(requestContext, projectId, TreeStructureType.Area);
      return new Usage(tuple.Count, tuple.PathLimit);
    }

    private Usage GetIterationsUsage(
      IVssRequestContext requestContext,
      WorkItemTrackingTreeService workItemTrackingTreeService,
      Guid projectId)
    {
      (int Count, int PathLimit) tuple = workItemTrackingTreeService.FetchAreaAndIterationCountWithLimits(requestContext, projectId, TreeStructureType.Iteration);
      return new Usage(tuple.Count, tuple.PathLimit);
    }

    private string Area => "ResourceUsage";

    private string Layer => nameof (ResourceUsageService);
  }
}
