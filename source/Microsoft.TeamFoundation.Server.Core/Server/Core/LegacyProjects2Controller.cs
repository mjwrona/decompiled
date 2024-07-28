// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.LegacyProjects2Controller
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.Project.WebServer;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core
{
  [VersionedApiControllerCustomName("core", "projects", 2)]
  public class LegacyProjects2Controller : ServerCoreApiController
  {
    private const string c_area = "Project";
    private const string c_layer = "LegacyProjects2Controller";

    [HttpGet]
    [PublicProjectRequestRestrictions(false, true, "projectid", null)]
    public TeamProject GetProject(
      string projectId,
      bool? includeCapabilities = null,
      bool? includeHistory = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId), this.TfsRequestContext.ServiceName);
      return ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, includeCapabilities.GetValueOrDefault(), includeHistory.GetValueOrDefault());
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<TeamProjectReference>), null, null)]
    [ClientExample("GET__projects.json", null, null, null)]
    public HttpResponseMessage GetProjects(
      ProjectState stateFilter = ProjectState.WellFormed,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null,
      int? continuationToken = null,
      bool getDefaultTeamImageUrl = false)
    {
      List<TeamProjectReference> projects = stateFilter == ProjectState.Deleted ? ProjectsUtility.GetSoftDeletedTeamProjects(this.TfsRequestContext, getDefaultTeamImageUrl).ToList<TeamProjectReference>() : ProjectsUtility.GetTeamProjects(this.TfsRequestContext, stateFilter, getDefaultTeamImageUrl).ToList<TeamProjectReference>();
      ProjectsContinuationToken continuationToken1 = new ProjectsContinuationToken(skip, top, continuationToken, projects.Count);
      return this.Request.CreateResponse<List<TeamProjectReference>>(HttpStatusCode.OK, projects.ApplyPagination(continuationToken1)).WithContinuationTokenHeader(continuationToken1);
    }

    [HttpPost]
    [ClientResponseType(typeof (OperationReference), null, null)]
    [ClientExample("POST__projects.json", null, null, null)]
    [ClientSwaggerOperationId("Create")]
    public virtual async Task<HttpResponseMessage> QueueCreateProject(TeamProject projectToCreate)
    {
      LegacyProjects2Controller projects2Controller = this;
      ArgumentUtility.CheckForNull<TeamProject>(projectToCreate, nameof (projectToCreate), projects2Controller.TfsRequestContext.ServiceName);
      bool flag1 = projectToCreate.Abbreviation == null && projectToCreate.TfsUri == null && projectToCreate.DefaultTeam == null && projectToCreate.Links == null && projectToCreate.Url == null && projectToCreate.State == ProjectState.Unchanged && object.Equals((object) Guid.Empty, (object) projectToCreate.Id) && !string.IsNullOrEmpty(projectToCreate.Name);
      string sourceControlTypeString = (string) null;
      string input = (string) null;
      string featuresEnabled = (string) null;
      Dictionary<string, string> dictionary1;
      Dictionary<string, string> dictionary2;
      bool flag2 = flag1 && projectToCreate.Capabilities != null && (projectToCreate.Capabilities.Count == 2 || projectToCreate.Capabilities.Count == 3) && projectToCreate.Capabilities.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlCapabilityName, out dictionary1) && projectToCreate.Capabilities.TryGetValue(TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityName, out dictionary2) && dictionary1 != null && dictionary1.Count == 1 && dictionary1.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlCapabilityAttributeName, out sourceControlTypeString) && dictionary2 != null && dictionary2.Count == 1 && dictionary2.TryGetValue(TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityTemplateTypeIdAttributeName, out input);
      if (flag2 && projectToCreate.Capabilities.Count == 3)
      {
        Dictionary<string, string> dictionary3;
        flag2 = flag2 && projectToCreate.Capabilities.TryGetValue(TeamProjectCapabilitiesConstants.FeaturesCapabilityName, out dictionary3) && dictionary3 != null && dictionary3.Count == 1 && dictionary3.TryGetValue(TeamProjectCapabilitiesConstants.FeaturesEnabled, out featuresEnabled);
      }
      if (!flag2)
        throw new ArgumentException(Resources.InvalidProjectCreate()).Expected(projects2Controller.TfsRequestContext.ServiceName);
      SourceControlTypes result;
      if (!System.Enum.TryParse<SourceControlTypes>(sourceControlTypeString, true, out result))
        throw new ArgumentException(Resources.InvalidSourceControlType((object) sourceControlTypeString)).Expected(projects2Controller.TfsRequestContext.ServiceName);
      Guid templateTypeId;
      if (!Guid.TryParse(input, out templateTypeId))
        throw new ArgumentException(Resources.InvalidProcessTemplateTypeId((object) input)).Expected(projects2Controller.TfsRequestContext.ServiceName);
      if (!projects2Controller.TfsRequestContext.IsFeatureEnabled("Project.Creation.DisableProjectLimitCheck"))
      {
        int projectCountLimit = projects2Controller.GetProjectCountLimit();
        IEnumerable<TeamProjectReference> teamProjects = ProjectsUtility.GetTeamProjects(projects2Controller.TfsRequestContext, ProjectState.WellFormed);
        if (teamProjects.Count<TeamProjectReference>() >= projectCountLimit)
          throw new InvalidProjectCreateException(Resources.OverProjectCountLimit((object) teamProjects.Count<TeamProjectReference>(), (object) projectCountLimit));
      }
      if (projectToCreate.Visibility == ProjectVisibility.Unchanged)
        projectToCreate.Visibility = ProjectVisibility.Private;
      ProjectsUtility.ThrowIfProjectOperationsNotAllowed(projects2Controller.TfsRequestContext);
      (bool flag3, OperationReference operationReference) = await ProjectsUtility.TryAssignPreCreatedProject(projects2Controller.TfsRequestContext, projectToCreate, templateTypeId, result, featuresEnabled);
      bool flag4 = flag3 || !ProjectsUtility.DoesAnyProjectExist(projects2Controller.TfsRequestContext);
      if (!flag3)
      {
        Dictionary<string, string> servicingTokens = new Dictionary<string, string>()
        {
          {
            ProjectServicingTokenConstants.VersionControlOption,
            sourceControlTypeString
          },
          {
            ProjectServicingTokenConstants.ProjectVisibilityOption,
            (string) null
          }
        };
        if (!string.IsNullOrEmpty(featuresEnabled))
          servicingTokens.Add(ProjectServicingTokenConstants.EnabledProjectFeatures, featuresEnabled);
        ServicingJobDetail project = projects2Controller.TfsRequestContext.GetService<IProjectWorkflowService>().QueueCreateProject(projects2Controller.TfsRequestContext, projectToCreate.Name, projectToCreate.Description, templateTypeId, (IDictionary<string, string>) servicingTokens, projectToCreate.Visibility);
        operationReference = JobOperationsUtility.GetOperationReference(projects2Controller.TfsRequestContext, project.JobId);
      }
      ProjectsUtility.PublishProjectDataToCustomerIntelligence(projects2Controller.TfsRequestContext, new Dictionary<string, string>()
      {
        {
          CustomerIntelligenceProperty.Action,
          "CreateProject"
        },
        {
          "Type",
          sourceControlTypeString
        },
        {
          "Visibility",
          projectToCreate.Visibility.ToString()
        },
        {
          "FirstProject",
          flag4.ToString()
        },
        {
          "Source",
          "RestRequest"
        },
        {
          "ProcessTemplateTypeId",
          templateTypeId.ToString()
        },
        {
          "ProjectName",
          projectToCreate.Name
        },
        {
          "IsPreCreated",
          flag3.ToString()
        }
      });
      HttpResponseMessage response = projects2Controller.Request.CreateResponse<OperationReference>(HttpStatusCode.Accepted, operationReference);
      sourceControlTypeString = (string) null;
      featuresEnabled = (string) null;
      return response;
    }

    [HttpDelete]
    [ClientResponseType(typeof (OperationReference), null, null)]
    [ClientSwaggerOperationId("Delete")]
    public virtual HttpResponseMessage QueueDeleteProject(Guid projectId, [ClientIgnore, FromUri(Name = "hardDelete")] bool? hardDelete = false)
    {
      ProjectsUtility.ThrowIfProjectOperationsNotAllowed(this.TfsRequestContext);
      bool flag1 = hardDelete.HasValue && hardDelete.Value;
      if (flag1 && this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && !this.TfsRequestContext.ExecutionEnvironment.IsDevFabricDeployment)
        throw new InvalidProjectDeleteException(Microsoft.TeamFoundation.Core.WebApi.WebApiResources.InvalidHostedHardDeleteProjectError());
      PlatformProjectService service = this.TfsRequestContext.GetService<PlatformProjectService>();
      ProjectInfo project = service.GetProject(this.TfsRequestContext, projectId, true);
      bool flag2 = flag1 || project.State != ProjectState.WellFormed;
      Guid jobId = flag2 ? ProjectsUtility.QueueDeleteProjectServicingJob(this.TfsRequestContext, ProjectInfo.GetProjectUri(projectId)).JobId : service.SoftDeleteProject(this.TfsRequestContext, projectId, out ProjectInfo _);
      ProjectsUtility.PublishProjectDataToCustomerIntelligence(this.TfsRequestContext, new Dictionary<string, string>()
      {
        {
          CustomerIntelligenceProperty.Action,
          "DeleteProject"
        },
        {
          "HardDelete",
          flag2.ToString()
        }
      });
      return this.Request.CreateResponse<OperationReference>(HttpStatusCode.Accepted, JobOperationsUtility.GetOperationReference(this.TfsRequestContext, jobId));
    }

    [HttpPatch]
    public virtual HttpResponseMessage UpdateProject(Guid projectId, TeamProject projectUpdate)
    {
      ArgumentUtility.CheckForNull<TeamProject>(projectUpdate, "updatedProject", this.TfsRequestContext.ServiceName);
      if ((projectUpdate.Capabilities != null || projectUpdate.Links != null || projectUpdate.Url != null || projectUpdate.State != ProjectState.Unchanged ? 0 : (object.Equals((object) Guid.Empty, (object) projectUpdate.Id) ? 1 : (object.Equals((object) projectId, (object) projectUpdate.Id) ? 1 : 0))) == 0)
        throw new ArgumentException(Resources.InvalidProjectUpdate(), nameof (projectUpdate)).Expected(this.TfsRequestContext.ServiceName);
      ProjectsUtility.ThrowIfProjectOperationsNotAllowed(this.TfsRequestContext);
      ProjectsUtility.UpdateTeamProject(this.TfsRequestContext, projectId, projectUpdate.Name, projectUpdate.Abbreviation, projectUpdate.Description, projectUpdate.Visibility);
      return this.Request.CreateResponse<TeamProject>(HttpStatusCode.OK, ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId.ToString(), false));
    }

    private int GetProjectCountLimit()
    {
      int defaultValue = 1000;
      return this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext.RootContext, (RegistryQuery) "/Service/Framework/ServerCore/ProjectLimit", true, defaultValue);
    }
  }
}
