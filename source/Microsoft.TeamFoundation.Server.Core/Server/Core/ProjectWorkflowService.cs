// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectWorkflowService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Server.Core.Audit;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class ProjectWorkflowService : IProjectWorkflowService, IVssFrameworkService
  {
    private static readonly string s_area = "Project";
    private static readonly string s_layer = nameof (ProjectWorkflowService);
    private const int c_descriptionMaxLength = 15999;
    private const int c_defaultMaxProjectsPerXmlProcess = 200;
    private static readonly RegistryQuery s_maxProjectsRegistry = (RegistryQuery) "/Service/ProjectWorkflowService/MaxProjectsPerXmlProcess";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ServicingJobDetail QueueCreateProject(
      IVssRequestContext requestContext,
      string projectName,
      string projectDescription,
      Guid processTemplateTypeId,
      IDictionary<string, string> servicingTokens,
      ProjectVisibility projectVisibility = ProjectVisibility.Private)
    {
      ProjectsUtility.CheckCreateProjectPermission(requestContext);
      if (processTemplateTypeId == ProcessTemplateTypeIdentifiers.DefaultPreCreateProcess)
        processTemplateTypeId = requestContext.GetService<IVssRegistryService>().GetValue<Guid>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/PreCreateProjectProcessTemplateType", true, ProcessTemplateTypeIdentifiers.MsfHydroProcess);
      ProjectWorkflowService.ValidateProjectsPerXmlProcessLimit(requestContext, processTemplateTypeId);
      projectName = ProjectInfo.NormalizeProjectName(projectName, nameof (projectName), checkValidity: true);
      ProjectsUtility.CheckProjectDescription(requestContext, projectDescription);
      ProjectsUtility.CheckProjectVisibility(requestContext, projectVisibility);
      if (projectVisibility == ProjectVisibility.Unchanged || projectVisibility == ProjectVisibility.SystemPrivate)
        throw new ArgumentException(Resources.InvalidCreateProjectVisibilityLevel((object) projectVisibility), nameof (projectVisibility));
      ILeaseInfo lease = (ILeaseInfo) null;
      Guid pendingProjectGuid = Guid.Empty;
      try
      {
        ProcessDescriptor processTemplateDescriptor;
        TeamProjectUtil.PrepareProjectCreation(requestContext, projectName, projectDescription, processTemplateTypeId, projectVisibility, false, out pendingProjectGuid, out lease, out processTemplateDescriptor);
        servicingTokens[ProjectServicingTokenConstants.CreatePendingProjectGuid] = pendingProjectGuid.ToString("D");
        servicingTokens[ProjectServicingTokenConstants.ProjectLeaseOwner] = lease.LeaseOwner.ToString("D");
        string name = processTemplateTypeId.ToString("D");
        if (processTemplateDescriptor != null)
          name = processTemplateDescriptor.Name;
        IVssRequestContext requestContext1 = requestContext;
        string actionId = AuditActionState.Queued(ProjectAuditConstants.Create);
        Dictionary<string, object> data = ProjectAuditData.Create(projectName, name, projectVisibility);
        Guid guid = pendingProjectGuid;
        Guid targetHostId = new Guid();
        Guid projectId = guid;
        requestContext1.LogAuditEvent(actionId, data, targetHostId, projectId);
        ProjectsUtility.PublishProjectDataToCustomerIntelligence(requestContext, new Dictionary<string, string>()
        {
          {
            CustomerIntelligenceProperty.Action,
            nameof (QueueCreateProject)
          },
          {
            "Visibility",
            projectVisibility.ToString()
          },
          {
            "ProcessTemplateTypeId",
            processTemplateTypeId.ToString()
          },
          {
            "ProjectId",
            pendingProjectGuid.ToString()
          },
          {
            "ProjectName",
            projectName
          },
          {
            "IsPreCreated",
            bool.FalseString
          }
        });
        return this.QueueCreateProjectServicingJob(requestContext, requestContext.ServiceHost.InstanceId, projectName, projectDescription, processTemplateTypeId, servicingTokens, projectVisibility);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5500500, ProjectWorkflowService.s_area, ProjectWorkflowService.s_layer, ex);
        if (pendingProjectGuid != Guid.Empty)
        {
          requestContext.GetService<IProjectService>().DeleteReservedProject(requestContext, pendingProjectGuid);
          if (lease != null)
            TeamProjectUtil.ReleaseProjectLease(requestContext, pendingProjectGuid, lease.LeaseOwner);
        }
        throw;
      }
    }

    private static Guid GetProjectTemplateTypeId(ProjectInfo project)
    {
      if (project.Properties.Any<ProjectProperty>())
      {
        ProjectProperty projectProperty = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => p.Name.Equals(ProcessTemplateIdPropertyNames.ProcessTemplateType, StringComparison.OrdinalIgnoreCase)));
        Guid result;
        if (projectProperty != null && Guid.TryParse((string) projectProperty.Value, out result))
          return result;
      }
      return Guid.Empty;
    }

    private static bool IsSharedProcessEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") || requestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload");

    private static void ValidateProjectsPerXmlProcessLimit(
      IVssRequestContext requestContext,
      Guid processTemplateTypeId)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !ProjectWorkflowService.IsSharedProcessEnabled(requestContext))
        return;
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processTemplateTypeId);
      if (!processDescriptor.IsCustom)
        return;
      IProjectService service = requestContext.GetService<IProjectService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IVssRequestContext requestContext2 = requestContext1;
      int count = service.GetProjects(requestContext2, ProjectState.WellFormed).PopulateProperties(requestContext1, ProcessTemplateIdPropertyNames.ProcessTemplateType).Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.Properties.Any<ProjectProperty>() && Guid.TryParse((string) p.Properties[0].Value, out Guid _) && ProjectWorkflowService.GetProjectTemplateTypeId(p) == processTemplateTypeId)).Count<ProjectInfo>();
      int limitCount = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, in ProjectWorkflowService.s_maxProjectsRegistry, 200);
      if (count >= limitCount)
        throw new ProjectPerXmlProcessLimitException(count, limitCount, processDescriptor.Name);
    }

    public ServicingJobDetail QueueHardDeleteProject(
      IVssRequestContext requestContext,
      string projectUri,
      IDictionary<string, string> servicingTokens)
    {
      ProjectsUtility.CheckDeleteProjectPermission(requestContext, projectUri);
      return this.QueueHardDeleteProjectServicingJob(requestContext, projectUri, servicingTokens);
    }

    private ServicingJobDetail QueueCreateProjectServicingJob(
      IVssRequestContext requestContext,
      Guid collectionId,
      string projectName,
      string projectDescription,
      Guid processTemplateTypeId,
      IDictionary<string, string> servicingTokens,
      ProjectVisibility projectVisibility)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), requestContext.ServiceName);
      ServicingJobData servicingJobData = new ServicingJobData(new string[1]
      {
        ServicingOperationConstants.ProjectCreate
      });
      servicingJobData.JobTitle = FrameworkResources.CreateTeamProjectJobTitle((object) projectName);
      servicingJobData.OperationClass = "CreateProject";
      servicingJobData.ServicingHostId = collectionId;
      servicingJobData.ServicingOptions = ServicingFlags.HostMustExist | ServicingFlags.UseSystemTargetRequestContext;
      servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
      {
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Shared,
          LockName = "Servicing-" + collectionId.ToString(),
          LockTimeout = -1
        }
      };
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      ITeamProjectCollectionPropertiesService service1 = context.GetService<ITeamProjectCollectionPropertiesService>();
      IInternalTeamProjectCollectionService service2 = context.GetService<IInternalTeamProjectCollectionService>();
      IVssRequestContext requestContext1 = context;
      Guid collectionId1 = collectionId;
      TeamProjectCollectionProperties collectionProperties = service1.GetCollectionProperties(requestContext1, collectionId1, ServiceHostFilterFlags.None);
      service2.AddCommonTokensAndItems(requestContext, servicingJobData, collectionProperties, true);
      servicingJobData.ServicingTokens.Add(ProjectServicingTokenConstants.IsPreCreate, bool.FalseString);
      servicingJobData.ServicingTokens.Add(ProjectServicingTokenConstants.ProjectName, projectName);
      servicingJobData.ServicingTokens.Add(ProjectServicingTokenConstants.ProjectDescription, projectDescription);
      servicingJobData.ServicingTokens.Add(ProjectServicingTokenConstants.TemplateTypeId, processTemplateTypeId.ToString("D"));
      servicingJobData.ServicingTokens.Add(ProjectServicingTokenConstants.ProjectVisibility, projectVisibility.ToString());
      servicingJobData.ServicingTokens.Add(ServicingTokenConstants.RequestingUserName, requestContext.AuthenticatedUserName);
      if (servicingTokens != null)
      {
        foreach (KeyValuePair<string, string> servicingToken in (IEnumerable<KeyValuePair<string, string>>) servicingTokens)
          servicingJobData.ServicingTokens[servicingToken.Key] = servicingToken.Value;
      }
      return requestContext.GetService<ITeamFoundationServicingService>().QueueServicingJob(requestContext, servicingJobData, JobPriorityClass.High);
    }

    private ServicingJobDetail QueueHardDeleteProjectServicingJob(
      IVssRequestContext requestContext,
      string projectUri,
      IDictionary<string, string> servicingTokens)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri), requestContext.ServiceName);
      Guid projectId1 = ProjectInfo.GetProjectId(projectUri);
      TeamProjectUtil.CheckAcquireProjectLease(requestContext, projectId1);
      ServicingJobData servicingJobData = new ServicingJobData(new string[1]
      {
        ServicingOperationConstants.ProjectDelete
      });
      servicingJobData.JobTitle = FrameworkResources.DeleteTeamProjectJobTitle((object) projectUri);
      servicingJobData.OperationClass = "DeleteProject";
      servicingJobData.ServicingHostId = requestContext.ServiceHost.InstanceId;
      servicingJobData.ServicingOptions = ServicingFlags.HostMustExist;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        servicingJobData.ServicingOptions |= ServicingFlags.UseServicingContextForJobRunner;
      servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
      {
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Shared,
          LockName = "Servicing-" + requestContext.ServiceHost.InstanceId.ToString(),
          LockTimeout = -1
        }
      };
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      ITeamProjectCollectionPropertiesService service1 = vssRequestContext.GetService<ITeamProjectCollectionPropertiesService>();
      IInternalTeamProjectCollectionService service2 = vssRequestContext.GetService<IInternalTeamProjectCollectionService>();
      IVssRequestContext requestContext1 = vssRequestContext;
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      TeamProjectCollectionProperties collectionProperties = service1.GetCollectionProperties(requestContext1, instanceId, ServiceHostFilterFlags.None);
      if (collectionProperties.State != TeamFoundationServiceHostStatus.Paused && collectionProperties.State != TeamFoundationServiceHostStatus.Stopped)
        servicingJobData.ServicingOptions |= ServicingFlags.UseSystemTargetRequestContext;
      service2.AddCommonTokensAndItems(vssRequestContext, servicingJobData, collectionProperties, false);
      servicingJobData.ServicingItems[ServicingItemConstants.RequestingIdentity] = (object) requestContext.UserContext;
      servicingJobData.ServicingTokens.Add(ServicingTokenConstants.ProjectUri, projectUri);
      if (servicingTokens != null)
      {
        foreach (KeyValuePair<string, string> servicingToken in (IEnumerable<KeyValuePair<string, string>>) servicingTokens)
          servicingJobData.ServicingTokens[servicingToken.Key] = servicingToken.Value;
      }
      ProjectInfo projectInfo = requestContext.GetService<PlatformProjectService>().GetProject(requestContext, projectId1, true);
      string name = projectInfo.Name;
      if (projectInfo.IsSoftDeleted)
      {
        projectInfo = ((IEnumerable<ProjectInfo>) new ProjectInfo[1]
        {
          projectInfo
        }).PopulateProperties(requestContext, "System.SoftDeletedProjectName").First<ProjectInfo>();
        if (projectInfo.Properties.Any<ProjectProperty>())
          name = projectInfo.Properties.First<ProjectProperty>().Value.ToString();
      }
      servicingJobData.ServicingTokens[ProjectServicingTokenConstants.ProjectName] = name;
      IVssRequestContext requestContext2 = requestContext;
      string actionId = AuditActionState.Queued(ProjectAuditConstants.HardDelete);
      Dictionary<string, object> data = ProjectAuditData.HardDelete(name);
      Guid id = projectInfo.Id;
      Guid targetHostId = new Guid();
      Guid projectId2 = id;
      requestContext2.LogAuditEvent(actionId, data, targetHostId, projectId2);
      ITeamFoundationServicingService service3 = requestContext.GetService<ITeamFoundationServicingService>();
      ServicingJobInfo servicingJobInfo = service3.GetServicingJobInfo(requestContext, servicingJobData.ServicingHostId, projectId1);
      if (servicingJobInfo != null)
      {
        try
        {
          service3.RequeueServicingJob(requestContext, servicingJobInfo.HostId, servicingJobInfo.JobId);
          return new ServicingJobDetail()
          {
            JobId = servicingJobInfo.JobId,
            HostId = servicingJobInfo.HostId,
            OperationClass = servicingJobInfo.OperationClass,
            Operations = servicingJobData.ServicingOperations,
            JobStatus = ServicingJobStatus.Queued,
            Result = ServicingJobResult.None
          };
        }
        catch (JobDefinitionNotFoundException ex)
        {
          requestContext.TraceException(20042518, ProjectWorkflowService.s_area, ProjectWorkflowService.s_layer, (Exception) ex);
        }
      }
      return service3.QueueServicingJob(requestContext, servicingJobData, JobPriorityClass.Normal, jobId: new Guid?(projectId1));
    }
  }
}
