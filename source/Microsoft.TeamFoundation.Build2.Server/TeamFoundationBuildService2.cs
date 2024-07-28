// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TeamFoundationBuildService2
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class TeamFoundationBuildService2 : 
    ITeamFoundationBuildService2,
    IVssFrameworkService
  {
    private readonly IBuildSecurityProvider SecurityProvider;
    private readonly IReportGeneratorFactory m_reportGeneratorFactory;
    private Guid m_acsServiceIdentityId;
    private int m_daysToKeepDailyMetrics = BuildConstants.DefaultDaysToKeepDailyProjectMetrics;
    private int m_daysToKeepHourlyMetrics = BuildConstants.DefaultDaysToKeepHourlyProjectMetrics;
    private const string c_buildAcsServiceIdentityPath = "/Service/Build2/Settings/ServiceIdentity";
    private const string c_ServiceEndPointTaskInputTypePrefix = "connectedService:";
    private static readonly RegistryQuery s_buildSettingsQuery = (RegistryQuery) "/Service/Build2/Settings/...";
    private static readonly RegistryQuery s_metricsQuery = (RegistryQuery) "/Service/Build/Settings/Metrics/Project/.../DaysToKeep";

    public TeamFoundationBuildService2()
      : this((IBuildSecurityProvider) new BuildSecurityProvider(), (IReportGeneratorFactory) new ReportGeneratorFactory())
    {
    }

    internal TeamFoundationBuildService2(
      IBuildSecurityProvider securityProvider,
      IReportGeneratorFactory reportGeneratorFactory)
    {
      this.SecurityProvider = securityProvider;
      this.m_reportGeneratorFactory = reportGeneratorFactory;
    }

    public IEnumerable<BuildMetric> GetProjectMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      string metricAggregationType,
      DateTime? minMetricsTime)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope("Service", nameof (GetProjectMetrics)))
      {
        TeamFoundationBuildService2.MetricAggregationType result;
        if (string.IsNullOrEmpty(metricAggregationType) || !Enum.TryParse<TeamFoundationBuildService2.MetricAggregationType>(metricAggregationType, true, out result))
          throw new MetricAggregationTypeNotSupportedException(BuildServerResources.MetricAggregationTypeNotSupported((object) (metricAggregationType ?? string.Empty)));
        DateTime projectMetricsTime = this.GetNormalizedMinProjectMetricsTime(requestContext, result, minMetricsTime, DateTime.Now.Date);
        Microsoft.TeamFoundation.Core.WebApi.TeamProject teamProject = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId).ToTeamProject(requestContext, false);
        List<BuildMetric> list;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          list = component.GetProjectMetrics(teamProject.Id, projectMetricsTime).ToList<BuildMetric>();
        if (result == TeamFoundationBuildService2.MetricAggregationType.Daily)
          list = list.GroupBy(m =>
          {
            string name = m.Name;
            DateTime? date = m.Date;
            ref DateTime? local = ref date;
            DateTime? nullable = local.HasValue ? new DateTime?(local.GetValueOrDefault().Date) : new DateTime?();
            return new{ Name = name, Date = nullable };
          }).Select<IGrouping<\u003C\u003Ef__AnonymousType10<string, DateTime?>, BuildMetric>, BuildMetric>(ma => new BuildMetric()
          {
            Name = ma.Key.Name,
            Date = ma.Key.Date,
            IntValue = ma.Sum<BuildMetric>((Func<BuildMetric, int>) (m => m.IntValue))
          }).ToList<BuildMetric>();
        return (IEnumerable<BuildMetric>) list;
      }
    }

    internal DateTime GetNormalizedMinProjectMetricsTime(
      IVssRequestContext requestContext,
      TeamFoundationBuildService2.MetricAggregationType aggregationType,
      DateTime? minMetricsTime,
      DateTime currentDate)
    {
      DateTime projectMetricsTime;
      switch (aggregationType)
      {
        case TeamFoundationBuildService2.MetricAggregationType.Hourly:
          projectMetricsTime = !minMetricsTime.HasValue || (currentDate - minMetricsTime.Value).TotalDays >= (double) this.m_daysToKeepHourlyMetrics ? currentDate.AddDays((double) (-this.m_daysToKeepHourlyMetrics + 1)) : minMetricsTime.Value.Date.AddHours((double) minMetricsTime.Value.Hour);
          break;
        case TeamFoundationBuildService2.MetricAggregationType.Daily:
          if (!minMetricsTime.HasValue)
          {
            projectMetricsTime = currentDate.AddDays(-7.0);
            break;
          }
          DateTime dateTime1 = currentDate;
          DateTime dateTime2 = minMetricsTime.Value;
          DateTime date = dateTime2.Date;
          if ((dateTime1 - date).TotalDays >= (double) this.m_daysToKeepDailyMetrics)
          {
            projectMetricsTime = currentDate.AddDays((double) (-this.m_daysToKeepDailyMetrics + 1));
            break;
          }
          dateTime2 = minMetricsTime.Value;
          projectMetricsTime = dateTime2.Date;
          break;
        default:
          projectMetricsTime = currentDate;
          break;
      }
      return projectMetricsTime;
    }

    public void CreateTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope("Service", nameof (CreateTeamProject)))
      {
        this.SecurityProvider.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericWrite);
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          component.CreateTeamProject(projectId);
        requestContext.GetService<IDistributedTaskPoolService>().CreateTeamProject(requestContext, projectId);
        if (requestContext.IsPipelines())
          return;
        requestContext.GetService<IDistributedTaskEnvironmentService>().CreateTeamProject(requestContext, projectId);
      }
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      int defaultValue = 1000;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/Retention/MaxBuildBatchSize", defaultValue);
      using (requestContext.TraceScope("Service", nameof (DeleteTeamProject)))
      {
        this.SecurityProvider.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.Delete);
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IBuildDefinitionService service = vssRequestContext.GetService<IBuildDefinitionService>();
        IEnumerable<BuildDefinition> deletedDefinitions = service.GetDeletedDefinitions(vssRequestContext, projectId);
        List<BuildDefinition> list = service.GetDefinitions(vssRequestContext, projectId, count: int.MaxValue).ToList<BuildDefinition>();
        HashSet<int> hashSet = deletedDefinitions.Select<BuildDefinition, int>((Func<BuildDefinition, int>) (d => d.Id)).ToHashSet<int>();
        IEnumerable<BuildDefinition> second = deletedDefinitions;
        IEnumerable<BuildDefinition> buildDefinitions = list.Concat<BuildDefinition>(second);
        IBuildService buildService = vssRequestContext.GetService<IBuildService>();
        foreach (BuildDefinition buildDefinition in buildDefinitions)
        {
          IEnumerable<BuildData> builds = (IEnumerable<BuildData>) new List<BuildData>();
          do
          {
            builds = (IEnumerable<BuildData>) buildService.GetBuildsLegacy(vssRequestContext, projectId, num, (IEnumerable<int>) new int[1]
            {
              buildDefinition.Id
            }, queryDeletedOption: QueryDeletedOption.IncludeDeleted).ToList<BuildData>();
            IEnumerable<DeleteBuildSpec> deleteBuildSpecs = builds.Select<BuildData, DeleteBuildSpec>(closure_0 ?? (closure_0 = (Func<BuildData, DeleteBuildSpec>) (b => new DeleteBuildSpec()
            {
              BuildId = b.Id,
              DeleteBuildRecord = true,
              DeletedReason = BuildServerResources.DeleteTeamProjectsBuildDeletedReason((object) projectId)
            })));
            IReadOnlyList<RetentionLease> leases = requestContext.RunSynchronously<IReadOnlyList<RetentionLease>>(closure_1 ?? (closure_1 = (Func<Task<IReadOnlyList<RetentionLease>>>) (() => buildService.GetRetentionLeasesForRuns(requestContext, projectId, builds.Select<BuildData, int>((Func<BuildData, int>) (b => b.Id))))));
            if (leases.Count > 0)
              requestContext.RunSynchronously((Func<Task>) (() => buildService.RemoveRetentionLeases(requestContext, projectId, (IReadOnlyList<int>) leases.Select<RetentionLease, int>((Func<RetentionLease, int>) (l => l.Id)).ToList<int>())));
            try
            {
              buildService.DeleteBuilds(vssRequestContext, projectId, deleteBuildSpecs, ignoreLowPriorityLeases: true);
              buildService.DestroyBuilds(vssRequestContext, projectId, buildDefinition.Id, DateTime.MaxValue, num);
            }
            catch (Exception ex)
            {
              requestContext.TraceException("Service", ex);
            }
          }
          while (builds.Count<BuildData>() == num);
          try
          {
            if (!hashSet.Contains(buildDefinition.Id))
              service.DeleteDefinition(vssRequestContext, projectId, buildDefinition.Id);
            service.DestroyDefinition(vssRequestContext, projectId, buildDefinition.Id);
          }
          catch (Exception ex)
          {
            requestContext.TraceException("Service", ex);
          }
        }
        using (Build2Component component = vssRequestContext.CreateComponent<Build2Component>())
          component.DeleteTeamProject(projectId);
        if (requestContext.IsPipelines())
          return;
        vssRequestContext.GetService<IDistributedTaskPoolService>().DeleteTeamProject(vssRequestContext, projectId);
        vssRequestContext.GetService<IDistributedTaskLibraryService>().DeleteTeamProject(vssRequestContext, projectId);
        vssRequestContext.GetService<IDistributedTaskEnvironmentService>().DeleteTeamProject(vssRequestContext, projectId);
      }
    }

    public IEnumerable<string> GetFilterBuildTags(
      IVssRequestContext requestContext,
      Guid projectId,
      MinimalBuildDefinition definition = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope("Service", nameof (GetFilterBuildTags)))
      {
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          if (definition != null)
            return this.SecurityProvider.HasDefinitionPermission(requestContext, projectId, definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds) ? component.GetFilterBuildTags(projectId, new int?(definition.Id)).Select<BuildTagFilter, string>((Func<BuildTagFilter, string>) (t => t.Tag)).Distinct<string>() : (IEnumerable<string>) new List<string>();
          IEnumerable<BuildTagFilter> filterBuildTags = component.GetFilterBuildTags(projectId, new int?());
          return (this.SecurityProvider.HasProjectPermission(requestContext, projectId, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds) ? filterBuildTags : this.FilterPermittedDefinitionTags(requestContext, projectId, filterBuildTags)).Select<BuildTagFilter, string>((Func<BuildTagFilter, string>) (t => t.Tag)).Distinct<string>();
        }
      }
    }

    private IEnumerable<BuildTagFilter> FilterPermittedDefinitionTags(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<BuildTagFilter> allTags)
    {
      return allTags.Where<BuildTagFilter>((Func<BuildTagFilter, bool>) (t => this.SecurityProvider.HasDefinitionPermission(requestContext, projectId, t.Definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds)));
    }

    public IEnumerable<string> GetTags(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope("Service", nameof (GetTags)))
      {
        if (!this.SecurityProvider.HasProjectPermission(requestContext, projectId, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds))
          return (IEnumerable<string>) new List<string>();
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.GetTags(projectId);
      }
    }

    public IReportGenerator GetReportGenerator(
      IVssRequestContext requestContext,
      string reportType = "Html",
      bool throwIfNotFound = true)
    {
      using (requestContext.TraceScope("Service", nameof (GetReportGenerator)))
        return this.m_reportGeneratorFactory.GetReportGenerator(requestContext, reportType, throwIfNotFound);
    }

    public RetentionPolicy SetDefaultRetentionPolicy(
      IVssRequestContext requestContext,
      RetentionPolicy retentionPolicy)
    {
      using (requestContext.TraceScope("Service", nameof (SetDefaultRetentionPolicy)))
      {
        ArgumentUtility.CheckForNull<RetentionPolicy>(retentionPolicy, nameof (retentionPolicy));
        this.SecurityProvider.CheckCollectionPermission(requestContext, AdministrationPermissions.ManageBuildResources);
        RetentionPolicy maximumRetentionPolicy = this.GetMaximumRetentionPolicy(requestContext);
        RetentionPolicy retentionPolicy1 = (RetentionPolicy) null;
        if (retentionPolicy.DaysToKeep < 0 || retentionPolicy.DaysToKeep > maximumRetentionPolicy.DaysToKeep)
        {
          retentionPolicy1 = this.GetDefaultRetentionPolicy(requestContext);
          retentionPolicy.DaysToKeep = retentionPolicy1.DaysToKeep;
        }
        if (retentionPolicy.MinimumToKeep < 0 || retentionPolicy.MinimumToKeep > maximumRetentionPolicy.MinimumToKeep)
        {
          if (retentionPolicy1 == null)
            retentionPolicy1 = this.GetDefaultRetentionPolicy(requestContext);
          retentionPolicy.MinimumToKeep = retentionPolicy1.MinimumToKeep;
        }
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        service.SetValue<int>(requestContext, "/Service/Build/Settings/Retention/DefaultPolicy/DaysToKeep", retentionPolicy.DaysToKeep);
        service.SetValue<int>(requestContext, "/Service/Build/Settings/Retention/DefaultPolicy/MinimumToKeep", retentionPolicy.MinimumToKeep);
        return this.GetDefaultRetentionPolicy(requestContext);
      }
    }

    public RetentionPolicy SetMaximumRetentionPolicy(
      IVssRequestContext requestContext,
      RetentionPolicy retentionPolicy)
    {
      using (requestContext.TraceScope("Service", nameof (SetMaximumRetentionPolicy)))
      {
        ArgumentUtility.CheckForNull<RetentionPolicy>(retentionPolicy, nameof (retentionPolicy));
        this.SecurityProvider.CheckCollectionPermission(requestContext, AdministrationPermissions.ManageBuildResources);
        RetentionPolicy retentionPolicy1 = (RetentionPolicy) null;
        if (retentionPolicy.DaysToKeep < 0)
        {
          retentionPolicy1 = this.GetMaximumRetentionPolicy(requestContext);
          retentionPolicy.DaysToKeep = retentionPolicy1.DaysToKeep;
        }
        if (retentionPolicy.MinimumToKeep < 0)
        {
          if (retentionPolicy1 == null)
            retentionPolicy1 = this.GetMaximumRetentionPolicy(requestContext);
          retentionPolicy.MinimumToKeep = retentionPolicy1.MinimumToKeep;
        }
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        service.SetValue<int>(requestContext, "/Service/Build/Settings/Retention/MaximumPolicy/DaysToKeep", retentionPolicy.DaysToKeep);
        service.SetValue<int>(requestContext, "/Service/Build/Settings/Retention/MaximumPolicy/MinimumToKeep", retentionPolicy.MinimumToKeep);
        return this.GetMaximumRetentionPolicy(requestContext);
      }
    }

    public int SetDaysToKeepDeletedBuildsBeforeDestroy(
      IVssRequestContext requestContext,
      int daysToKeep)
    {
      using (requestContext.TraceScope("Service", nameof (SetDaysToKeepDeletedBuildsBeforeDestroy)))
      {
        ArgumentUtility.CheckForNonnegativeInt(daysToKeep, nameof (daysToKeep));
        this.SecurityProvider.CheckCollectionPermission(requestContext, AdministrationPermissions.ManageBuildResources);
        requestContext.GetService<IVssRegistryService>().SetValue<int>(requestContext, "/Service/Build/Settings/Retention/DaysBeforeDestroy", daysToKeep);
        return daysToKeep;
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetServiceIdentity(
      IVssRequestContext requestContext,
      BuildAuthorizationScope scope,
      Guid projectId = default (Guid))
    {
      using (requestContext.TraceScope("Service", nameof (GetServiceIdentity)))
      {
        if (scope == BuildAuthorizationScope.Project)
          ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        Guid guid = scope == BuildAuthorizationScope.Project ? projectId : requestContext.ServiceHost.InstanceId;
        return IdentityHelper.GetFrameworkIdentity(requestContext.Elevate(), FrameworkIdentityType.ServiceIdentity, "Build", guid.ToString("D"));
      }
    }

    public RetentionPolicy GetDefaultRetentionPolicy(IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope("Service", nameof (GetDefaultRetentionPolicy)))
      {
        int num1 = 10;
        int num2 = 1;
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/Build/Settings/Retention/DefaultPolicy/DaysToKeep";
        ref RegistryQuery local1 = ref registryQuery;
        int defaultValue1 = num1;
        int num3 = service.GetValue<int>(requestContext1, in local1, defaultValue1);
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Build/Settings/Retention/DefaultPolicy/MinimumToKeep";
        ref RegistryQuery local2 = ref registryQuery;
        int defaultValue2 = num2;
        int num4 = service.GetValue<int>(requestContext2, in local2, defaultValue2);
        RetentionPolicy maximumRetentionPolicy = this.GetMaximumRetentionPolicy(requestContext);
        if (num3 < 0 || num3 > maximumRetentionPolicy.DaysToKeep)
          num3 = num1;
        if (num4 < 0 || num4 > maximumRetentionPolicy.MinimumToKeep)
          num4 = num2;
        return new RetentionPolicy()
        {
          DaysToKeep = num3,
          MinimumToKeep = num4,
          DeleteBuildRecord = true,
          DeleteTestResults = true,
          Branches = {
            "+refs/heads/*"
          },
          ArtifactTypesToDelete = {
            "FilePath",
            "SymbolStore",
            "SymbolRequest",
            "PipelineArtifact"
          }
        };
      }
    }

    public RetentionPolicy GetMaximumRetentionPolicy(IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope("Service", nameof (GetMaximumRetentionPolicy)))
      {
        int num1 = 30;
        int num2 = 10;
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/Build/Settings/Retention/MaximumPolicy/DaysToKeep";
        ref RegistryQuery local1 = ref registryQuery;
        int defaultValue1 = num1;
        int num3 = service.GetValue<int>(requestContext1, in local1, defaultValue1);
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Build/Settings/Retention/MaximumPolicy/MinimumToKeep";
        ref RegistryQuery local2 = ref registryQuery;
        int defaultValue2 = num2;
        int num4 = service.GetValue<int>(requestContext2, in local2, defaultValue2);
        if (num3 < 0)
          num3 = num1;
        if (num4 < 0)
          num4 = num2;
        return new RetentionPolicy()
        {
          DaysToKeep = num3,
          MinimumToKeep = num4,
          DeleteBuildRecord = true,
          DeleteTestResults = true,
          Branches = {
            "+refs/heads/*"
          },
          ArtifactTypesToDelete = {
            "FilePath",
            "SymbolStore"
          }
        };
      }
    }

    public int GetDaysToKeepDeletedBuildsBeforeDestroy(IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope("Service", nameof (GetDaysToKeepDeletedBuildsBeforeDestroy)))
      {
        int defaultValue = 30;
        int buildsBeforeDestroy = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/Retention/DaysBeforeDestroy", defaultValue);
        if (buildsBeforeDestroy < 0)
          buildsBeforeDestroy = defaultValue;
        return buildsBeforeDestroy;
      }
    }

    public BuildResourceUsage GetBuildResourceUsage(IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope("Service", nameof (GetBuildResourceUsage)))
      {
        int xaml = 0;
        int dtAgents = 0;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          dtAgents += TeamFoundationBuildService2.GetTaskAgentCount(requestContext);
          xaml += TeamFoundationBuildService2.GetXamlControllerCount(requestContext);
        }
        else
        {
          TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
          foreach (TeamFoundationServiceHostProperties serviceHostProperties in service.QueryServiceHostProperties(requestContext, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children.Where<TeamFoundationServiceHostProperties>((Func<TeamFoundationServiceHostProperties, bool>) (x => x.HostType == TeamFoundationHostType.ProjectCollection)))
          {
            using (IVssRequestContext requestContext1 = service.BeginRequest(requestContext, serviceHostProperties.Id, RequestContextType.SystemContext, true, true))
            {
              dtAgents += TeamFoundationBuildService2.GetTaskAgentCount(requestContext1);
              xaml += TeamFoundationBuildService2.GetXamlControllerCount(requestContext1);
            }
          }
        }
        int paidAgentSlots = int.MaxValue;
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          try
          {
            TaskHubLicenseDetails hubLicenseDetails = requestContext.GetService<ITaskHubLicenseService>().GetTaskHubLicenseDetails(requestContext, "Build");
            paidAgentSlots = hubLicenseDetails.FreePrivateLicenseCount + hubLicenseDetails.PurchasedPrivateLicenseCount + hubLicenseDetails.EnterpriseUsersCount;
          }
          catch (Exception ex)
          {
            requestContext.TraceException("ResourceAvailability", ex);
          }
        }
        bool isThrottlingEnabled = requestContext.IsFeatureEnabled("WebAccess.BuildAndRelease.ResourceLimits");
        return new BuildResourceUsage(xaml, dtAgents, paidAgentSlots, isThrottlingEnabled);
      }
    }

    private static int GetTaskAgentCount(IVssRequestContext requestContext)
    {
      int taskAgentCount = 0;
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      foreach (TaskAgentPool agentPool in (IEnumerable<TaskAgentPool>) service.GetAgentPools(requestContext))
      {
        if (!agentPool.IsHosted)
          taskAgentCount += service.GetAgents(requestContext, agentPool.Id).Where<TaskAgent>((Func<TaskAgent, bool>) (x => x.Enabled.HasValue && x.Enabled.Value)).Count<TaskAgent>();
      }
      return taskAgentCount;
    }

    private static int GetXamlControllerCount(IVssRequestContext requestContext)
    {
      BuildControllerQueryResult controllerQueryResult = requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildControllers(requestContext, new BuildControllerSpec()
      {
        IncludeAgents = false,
        ServiceHostName = "*",
        Name = "*"
      });
      Dictionary<string, BuildServiceHost> serviceHosts = new Dictionary<string, BuildServiceHost>();
      foreach (BuildServiceHost serviceHost in controllerQueryResult.ServiceHosts)
        serviceHosts.Add(serviceHost.Uri, serviceHost);
      return controllerQueryResult.Controllers.Where<Microsoft.TeamFoundation.Build.Server.BuildController>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, bool>) (x => !serviceHosts[x.ServiceHostUri].IsVirtual && x.Enabled)).Count<Microsoft.TeamFoundation.Build.Server.BuildController>();
    }

    internal bool GetAcsServiceIdentity(
      IVssRequestContext requestContext,
      out ServiceIdentity serviceIdentity)
    {
      using (requestContext.TraceScope("Service", nameof (GetAcsServiceIdentity)))
      {
        serviceIdentity = (ServiceIdentity) null;
        IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
        TeamFoundationAccessControlService service2 = requestContext.GetService<TeamFoundationAccessControlService>();
        if (this.m_acsServiceIdentityId != Guid.Empty)
        {
          requestContext.TraceVerbose(0, "Service", "Found existing service identity with ID {0}", (object) this.m_acsServiceIdentityId);
          serviceIdentity = ((IEnumerable<ServiceIdentity>) service2.QueryServiceIdentities(requestContext, new Guid[1]
          {
            this.m_acsServiceIdentityId
          }, false)).FirstOrDefault<ServiceIdentity>();
          if (serviceIdentity != null)
            requestContext.TraceVerbose(0, "Service", "Found service identity with ID {0}", (object) this.m_acsServiceIdentityId);
          else
            requestContext.TraceVerbose(0, "Service", "Service identity with ID {0} was not found. Provisioning a new identity", (object) this.m_acsServiceIdentityId);
        }
        IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(BuildWellKnownGroups.BuildServicesIdentifier.Value);
        if (serviceIdentity == null)
        {
          requestContext.TraceInfo(0, "Service", "Provisioning a new service identity {0}", (object) BuildServerResources.ServiceIdentityName());
          IdentityDescriptor[] addToGroups = new IdentityDescriptor[1]
          {
            foundationDescriptor
          };
          ServiceIdentityInfo identityInfo = new ServiceIdentityInfo()
          {
            Name = BuildServerResources.ServiceIdentityName()
          };
          serviceIdentity = service2.ProvisionServiceIdentity(requestContext, identityInfo, addToGroups);
          if (serviceIdentity != null)
          {
            service1.SetValue<Guid>(requestContext.Elevate(), "/Service/Build2/Settings/ServiceIdentity", serviceIdentity.Identity.Id);
            requestContext.TraceInfo(0, "Service", "Successfully provisioned service identity {0} with ID {1}", (object) serviceIdentity.IdentityInfo.Name, (object) serviceIdentity.Identity.Id);
            this.m_acsServiceIdentityId = serviceIdentity.Identity.Id;
          }
          else
            requestContext.TraceError(0, "Service", "Failed to provision the service identity");
        }
        else
          requestContext.GetService<IdentityService>().AddMemberToGroup(requestContext, foundationDescriptor, serviceIdentity.Identity);
        requestContext.TraceLeave(0, "Service", "EnsureIdentityProvisioned");
        return serviceIdentity != null;
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ProvisionServiceIdentity(
      IVssRequestContext requestContext,
      BuildAuthorizationScope scope,
      Guid projectId = default (Guid),
      bool setPermissions = false,
      IServicingContext servicingContext = null)
    {
      using (requestContext.TraceScope("Service", nameof (ProvisionServiceIdentity)))
      {
        if (scope == BuildAuthorizationScope.Project)
          ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        Guid guid = scope == BuildAuthorizationScope.Project ? projectId : requestContext.ServiceHost.InstanceId;
        bool flag = false;
        IVssRequestContext vssRequestContext1 = requestContext.Elevate();
        IVssRequestContext vssRequestContext2 = IdentityHelper.GetRequestContextForFrameworkIdentity(requestContext).Elevate();
        IdentityService service1 = vssRequestContext1.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(vssRequestContext1, FrameworkIdentityType.ServiceIdentity, "Build", guid.ToString("D"));
        string displayName = BuildServerResources.ProjectCollectionBuildService((object) requestContext.ServiceHost.Name);
        if (scope == BuildAuthorizationScope.Project)
          displayName = BuildServerResources.ProjectBuildService((object) requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId), (object) requestContext.ServiceHost.Name);
        if (frameworkIdentity != null)
        {
          vssRequestContext1.TraceVerbose(0, "Service", "Found service identity with ID {0}", (object) frameworkIdentity.Id);
          servicingContext?.LogInfo("Found service identity with ID {0}.", (object) frameworkIdentity.Id);
          if (!frameworkIdentity.IsActive)
          {
            vssRequestContext1.TraceError(12030048, "Service", "Service identity with ID {0} is not active in scope {1}", (object) frameworkIdentity.Id, (object) vssRequestContext1.ServiceHost.InstanceId);
            servicingContext?.LogInfo("Service identity with ID {0} is not currently active in scope {1}.", (object) frameworkIdentity.Id, (object) vssRequestContext1.ServiceHost.InstanceId);
            Microsoft.VisualStudio.Services.Identity.Identity identity = service1.GetIdentity(vssRequestContext1, GroupWellKnownIdentityDescriptors.SecurityServiceGroup);
            if (identity != null)
            {
              if (service1.AddMemberToGroup(vssRequestContext1, identity.Descriptor, frameworkIdentity))
              {
                vssRequestContext1.TraceError(12030049, "Service", "Service identity with ID {0} has been successfully added to security service group {1}", (object) frameworkIdentity.Id, (object) identity.Id);
                servicingContext?.LogInfo("Service identity with ID {0} has been successfully added to security service group {1}.", (object) frameworkIdentity.Id, (object) identity.Id);
              }
              else
              {
                vssRequestContext1.TraceError(12030050, "Service", "Service identity with ID {0} is marked inactive but was not added to security service group {1}", (object) frameworkIdentity.Id, (object) identity.Id);
                servicingContext?.LogInfo("Service identity with ID {0} is marked inactive but was not added to security service group {1}.", (object) frameworkIdentity.Id, (object) identity.Id);
              }
            }
          }
          if (!frameworkIdentity.DisplayName.Equals(displayName, StringComparison.Ordinal))
          {
            string customDisplayName = frameworkIdentity.CustomDisplayName;
            frameworkIdentity.CustomDisplayName = displayName;
            try
            {
              if (vssRequestContext2.GetService<IdentityService>().UpdateIdentities(vssRequestContext2, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
              {
                frameworkIdentity
              }))
              {
                requestContext.TraceAlways(12030097, TraceLevel.Info, "Build2", "Service", "Successfully renamed project service identity from '{0}' to '{1}'", (object) customDisplayName, (object) displayName);
                servicingContext?.LogInfo("Successfully renamed project service identity from '{0}' to '{1}'.", (object) customDisplayName, (object) displayName);
              }
              else
              {
                requestContext.TraceAlways(12030097, TraceLevel.Warning, "Build2", "Service", "Received a non-successful response while renaming project service identtiy from '{0}' to '{1}'", (object) customDisplayName, (object) displayName);
                servicingContext?.LogInfo("Received a non-successful response while renaming project service identtiy from '{0}' to '{1}'.", (object) customDisplayName, (object) displayName);
              }
            }
            catch (Exception ex)
            {
              servicingContext?.LogInfo("Failed to update project service identity from '{0}' to '{1}': {2}.", (object) customDisplayName, (object) displayName, (object) ex.ToString());
              requestContext.TraceError(12030097, "Service", "Failed to update project service identity from '{0}' to '{1}':\n{2}", (object) customDisplayName, (object) displayName, (object) ex.ToString());
            }
          }
        }
        else
        {
          vssRequestContext1.TraceInfo(0, "Service", "Service identity with ID {0} was not found. Provisioning a new service identity.", (object) guid);
          servicingContext?.LogInfo("Service identity with ID {0} was not found. Provisioning a new service identity.", (object) guid);
          frameworkIdentity = vssRequestContext2.GetService<IdentityService>().CreateFrameworkIdentity(vssRequestContext2, FrameworkIdentityType.ServiceIdentity, "Build", guid.ToString("D"), displayName);
          if (frameworkIdentity != null)
          {
            flag = true;
            Microsoft.VisualStudio.Services.Identity.Identity identity = service1.GetIdentity(requestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup);
            if (identity != null && service1.AddMemberToGroup(requestContext, identity.Descriptor, frameworkIdentity.Descriptor))
              requestContext.TraceInfo(12030049, "Service", "Added the service identity {0} to the security service group {1}", (object) frameworkIdentity.Descriptor, (object) identity.Descriptor);
            vssRequestContext1.TraceInfo(0, "Service", "Successfully provisioned service identity {0} with VSID {1}", (object) frameworkIdentity.DisplayName, (object) frameworkIdentity.Id);
            servicingContext?.LogInfo("Successfully provisioned service identity {0} with VSID {1}", (object) frameworkIdentity.DisplayName, (object) frameworkIdentity.Id);
          }
        }
        if (frameworkIdentity != null && ((!flag ? 0 : (scope == BuildAuthorizationScope.Project ? 1 : 0)) | (setPermissions ? 1 : 0)) != 0)
        {
          IProjectService service2 = requestContext.GetService<IProjectService>();
          if (scope == BuildAuthorizationScope.Project || !flag)
          {
            servicingContext?.LogInfo("Setting permissions for service identity {0} only on project {1}", (object) frameworkIdentity.DisplayName, (object) service2.GetProjectName(requestContext, projectId));
            this.SetServiceIdentityPermissions(vssRequestContext1, projectId, frameworkIdentity, new int?());
          }
          else
          {
            foreach (ProjectInfo project in service2.GetProjects(vssRequestContext1))
            {
              if (project.Id == projectId || project.State == ProjectState.WellFormed)
              {
                servicingContext?.LogInfo("Setting permissions for service identity {0} on project {1}", (object) frameworkIdentity.DisplayName, (object) service2.GetProjectName(requestContext, project.Id));
                this.SetServiceIdentityPermissions(vssRequestContext1, project.Id, frameworkIdentity, new int?());
              }
            }
          }
        }
        return frameworkIdentity;
      }
    }

    public void SetServiceIdentityPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity,
      int? definitionId = null)
    {
      using (requestContext.TraceScope("Service", nameof (SetServiceIdentityPermissions)))
      {
        IProjectService service1 = requestContext.GetService<IProjectService>();
        ITeamFoundationSecurityService service2 = requestContext.GetService<ITeamFoundationSecurityService>();
        IdentityService service3 = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity = service3.GetIdentity(requestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup);
        if (identity != null && service3.AddMemberToGroup(requestContext, identity.Descriptor, serviceIdentity.Descriptor) && definitionId.HasValue)
          requestContext.TraceError(12030049, "Service", "Added the service identity {0} to the security service group {1}", (object) serviceIdentity.Descriptor, (object) identity.Descriptor);
        ProjectInfo project = service1.GetProject(requestContext, projectId);
        int allPermissions = TaggingPermissions.AllPermissions;
        string securityToken1 = TaggingService.GetSecurityToken(new Guid?(projectId));
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry1 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, allPermissions, 0);
        IVssSecurityNamespace securityNamespace1 = service2.GetSecurityNamespace(requestContext, FrameworkSecurity.TaggingNamespaceId);
        IAccessControlEntry accessControlEntry2 = securityNamespace1.GetAccessControlEntry(requestContext, securityToken1, serviceIdentity.Descriptor);
        if (accessControlEntry2 != null)
          accessControlEntry1.Allow |= accessControlEntry2.Allow;
        securityNamespace1.SetAccessControlEntry(requestContext, securityToken1, (IAccessControlEntry) accessControlEntry1, false);
        int allow1 = TeamProjectPermissions.GenericRead | TeamProjectPermissions.PublishTestResults | TeamProjectPermissions.ViewTestResults | TeamProjectPermissions.ManageTestConfigurations;
        string securityToken2 = service1.GetSecurityToken(requestContext, project.Uri);
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry3 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, allow1, 0);
        IVssSecurityNamespace securityNamespace2 = service2.GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
        IAccessControlEntry accessControlEntry4 = securityNamespace2.GetAccessControlEntry(requestContext, securityToken2, serviceIdentity.Descriptor);
        if (accessControlEntry4 != null)
          accessControlEntry3.Allow |= accessControlEntry4.Allow;
        securityNamespace2.SetAccessControlEntry(requestContext, securityToken2, (IAccessControlEntry) accessControlEntry3, false);
        CommonStructureNodeInfo[] rootNodes = requestContext.GetService<CommonStructureService>().GetRootNodes(requestContext.Elevate(), project.Uri);
        IVssSecurityNamespace securityNamespace3 = service2.GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
        foreach (CommonStructureNodeInfo structureNodeInfo in ((IEnumerable<CommonStructureNodeInfo>) rootNodes).Where<CommonStructureNodeInfo>((Func<CommonStructureNodeInfo, bool>) (x => TFStringComparer.CssStructureType.Equals(x.StructureType, "ProjectModelHierarchy"))))
        {
          int allow2 = 49;
          Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry5 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, allow2, 0);
          IAccessControlEntry accessControlEntry6 = securityNamespace3.GetAccessControlEntry(requestContext, structureNodeInfo.Uri, serviceIdentity.Descriptor);
          if (accessControlEntry6 != null)
            accessControlEntry5.Allow |= accessControlEntry6.Allow;
          securityNamespace3.SetAccessControlEntry(requestContext, structureNodeInfo.Uri, (IAccessControlEntry) accessControlEntry5, false);
        }
        if (!(bool) requestContext.Items.GetValueOrDefault<string, object>("DelayQueryProvision", (object) false))
          this.SetWorkItemQueryFoldersPermission(requestContext, projectId, serviceIdentity, service2);
        VersionedItemPermissions allow3 = VersionedItemPermissions.Read | VersionedItemPermissions.PendChange | VersionedItemPermissions.Checkin | VersionedItemPermissions.Label | VersionedItemPermissions.Lock | VersionedItemPermissions.CheckinOther | VersionedItemPermissions.Merge | VersionedItemPermissions.ManageBranch;
        string token1 = VersionControlPath.Combine("$/", projectId.ToString("D"));
        IVssSecurityNamespace securityNamespace4 = service2.GetSecurityNamespace(requestContext, SecurityConstants.RepositorySecurity2NamespaceGuid);
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry7 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, (int) allow3, 0);
        IAccessControlEntry accessControlEntry8 = securityNamespace4.GetAccessControlEntry(requestContext, token1, serviceIdentity.Descriptor);
        if (accessControlEntry8 != null)
          accessControlEntry7.Allow |= accessControlEntry8.Allow;
        securityNamespace4.SetAccessControlEntry(requestContext, token1, (IAccessControlEntry) accessControlEntry7, false);
        GitRepositoryPermissions allow4 = GitRepositoryPermissions.GenericRead | GitRepositoryPermissions.CreateTag;
        string securable = GitUtils.CalculateSecurable(projectId, Guid.Empty, (string) null);
        IVssSecurityNamespace securityNamespace5 = service2.GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry9 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, (int) allow4, 0);
        IAccessControlEntry accessControlEntry10 = securityNamespace5.GetAccessControlEntry(requestContext, securable, serviceIdentity.Descriptor);
        if (accessControlEntry10 != null)
          accessControlEntry9.Allow |= accessControlEntry10.Allow;
        securityNamespace5.SetAccessControlEntry(requestContext, securable, (IAccessControlEntry) accessControlEntry9, false);
        int allow5 = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.EditBuildQuality | Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ManageBuildQueue | Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.OverrideBuildCheckInValidation | Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation | Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuildDefinition | Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds;
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry11 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, allow5, 0);
        IVssSecurityNamespace securityNamespace6 = service2.GetSecurityNamespace(requestContext, BuildSecurity.BuildNamespaceId);
        IAccessControlEntry accessControlEntry12 = securityNamespace6.GetAccessControlEntry(requestContext, projectId.ToString("D"), serviceIdentity.Descriptor);
        if (accessControlEntry12 != null)
          accessControlEntry11.Allow |= accessControlEntry12.Allow;
        securityNamespace6.SetAccessControlEntry(requestContext, projectId.ToString("D"), (IAccessControlEntry) accessControlEntry11, false);
        if (!definitionId.HasValue)
          return;
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry13 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, accessControlEntry11.Allow, 0);
        string token2 = projectId.ToString() + (object) BuildSecurity.NamespaceSeparator + (object) definitionId;
        IAccessControlList accessControlList = securityNamespace6.QueryAccessControlList(requestContext, token2, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          serviceIdentity.Descriptor
        }, false);
        IAccessControlEntry accessControlEntry14 = accessControlList.QueryAccessControlEntry(serviceIdentity.Descriptor);
        if (accessControlEntry14 == null && !accessControlList.InheritPermissions)
        {
          securityNamespace6.SetAccessControlEntry(requestContext, token2, (IAccessControlEntry) accessControlEntry13, false);
        }
        else
        {
          if (accessControlEntry14 == null)
            return;
          if (accessControlList.InheritPermissions)
            accessControlEntry13.Allow = accessControlEntry14.Allow & ~accessControlEntry13.Allow;
          else
            accessControlEntry13.Allow |= accessControlEntry14.Allow;
          if (accessControlEntry13.Allow == 0)
            securityNamespace6.RemoveAccessControlEntries(requestContext, token2, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
            {
              serviceIdentity.Descriptor
            });
          else
            securityNamespace6.SetAccessControlEntry(requestContext, token2, (IAccessControlEntry) accessControlEntry13, false);
        }
      }
    }

    public void SetWorkItemQueryFoldersPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity,
      ITeamFoundationSecurityService securityService)
    {
      IEnumerable<QueryHierarchyItem> queries = requestContext.GetService<IWorkItemQueryRemotableService>().GetQueries(requestContext, projectId);
      string str = "$/" + (object) projectId + (object) '/';
      IVssSecurityNamespace securityNamespace = securityService.GetSecurityNamespace(requestContext, new Guid("71356614-AAD7-4757-8F2C-0FB3BFF6F680"));
      int allow = 1;
      Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry1 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, allow, 0);
      foreach (QueryHierarchyItem queryHierarchyItem in queries.Where<QueryHierarchyItem>((Func<QueryHierarchyItem, bool>) (q => q.IsPublic.GetValueOrDefault())))
      {
        string token = str + (object) queryHierarchyItem.Id;
        IAccessControlEntry accessControlEntry2 = securityNamespace.GetAccessControlEntry(requestContext, token, serviceIdentity.Descriptor);
        if (accessControlEntry2 != null)
          accessControlEntry1.Allow |= accessControlEntry2.Allow;
        securityNamespace.SetAccessControlEntry(requestContext, token, (IAccessControlEntry) accessControlEntry1, false);
      }
    }

    private void OnServiceIdentityChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      RegistryEntry registryEntry = changedEntries.FirstOrDefault<RegistryEntry>((Func<RegistryEntry, bool>) (x => x.Path.Equals("/Service/Build2/Settings/ServiceIdentity", StringComparison.OrdinalIgnoreCase)));
      if (registryEntry == null)
        return;
      this.m_acsServiceIdentityId = registryEntry.GetValue<Guid>(Guid.Empty);
    }

    private void OnMetricsSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      RegistryEntry entry1;
      if (changedEntries.TryGetValue("/Service/Build/Settings/Metrics/Project/Hourly/DaysToKeep", out entry1))
        this.m_daysToKeepHourlyMetrics = entry1.GetValue<int>();
      RegistryEntry entry2;
      if (!changedEntries.TryGetValue("/Service/Build/Settings/Metrics/Project/Daily/DaysToKeep", out entry2))
        return;
      this.m_daysToKeepDailyMetrics = entry2.GetValue<int>();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnServiceIdentityChanged));
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnMetricsSettingsChanged));
      if (this.m_reportGeneratorFactory == null || !(this.m_reportGeneratorFactory is IDisposable generatorFactory))
        return;
      generatorFactory.Dispose();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnServiceIdentityChanged), false, in TeamFoundationBuildService2.s_buildSettingsQuery);
      this.m_acsServiceIdentityId = service.ReadEntries(requestContext, TeamFoundationBuildService2.s_buildSettingsQuery).GetValueFromPath<Guid>("/Service/Build2/Settings/ServiceIdentity", Guid.Empty);
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnMetricsSettingsChanged), false, in TeamFoundationBuildService2.s_metricsQuery);
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(requestContext, TeamFoundationBuildService2.s_metricsQuery);
      this.m_daysToKeepHourlyMetrics = registryEntryCollection.GetValueFromPath<int>("/Service/Build/Settings/Metrics/Project/Hourly/DaysToKeep", BuildConstants.DefaultDaysToKeepHourlyProjectMetrics);
      this.m_daysToKeepDailyMetrics = registryEntryCollection.GetValueFromPath<int>("/Service/Build/Settings/Metrics/Project/Daily/DaysToKeep", BuildConstants.DefaultDaysToKeepDailyProjectMetrics);
    }

    private enum DeleteOnCompletionBehavior
    {
      None,
      LeaveBuildRecord,
      DeleteBuildRecord,
    }

    internal enum MetricAggregationType
    {
      Hourly,
      Daily,
    }
  }
}
