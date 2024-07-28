// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiProjectController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiProjectController : AdminAreaController
  {
    private const string c_area = "Project";
    private const string c_layer = "ApiProjectController";

    [HttpPost]
    [ValidateInput(false)]
    [TfsTraceFilter(503040, 503050)]
    public async Task<ActionResult> CreateProject(
      string projectName,
      string projectDescription,
      string processTemplateTypeId,
      string processTemplateId,
      Guid? collectionId,
      string source,
      [ModelBinder(typeof (JsonModelBinder))] CreateProjectData projectData)
    {
      ApiProjectController projectController = this;
      Guid typeId;
      if (processTemplateTypeId != null)
      {
        typeId = Guid.Parse(processTemplateTypeId);
      }
      else
      {
        int legacyProcessId = int.Parse(processTemplateId);
        ITeamFoundationProcessService service = projectController.TfsRequestContext.GetService<ITeamFoundationProcessService>();
        Guid descriptorIdByIntegerId = service.GetSpecificProcessDescriptorIdByIntegerId(projectController.TfsRequestContext, legacyProcessId);
        typeId = service.GetSpecificProcessDescriptor(projectController.TfsRequestContext, descriptorIdByIntegerId).TypeId;
      }
      return await projectController.CreateProject(projectName, projectDescription, typeId, collectionId, source, projectData);
    }

    private async Task<ActionResult> CreateProject(
      string projectName,
      string projectDescription,
      Guid processTemplateTypeId,
      Guid? collectionId,
      string source,
      CreateProjectData projectData)
    {
      ApiProjectController projectController = this;
      ArgumentUtility.CheckForNull<CreateProjectData>(projectData, nameof (projectData));
      IVssRequestContext requestContext = projectController.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ProjectsUtility.ThrowIfProjectOperationsNotAllowed(projectController.TfsRequestContext);
      TeamFoundationHostManagementService service = projectController.TfsRequestContext.GetService<TeamFoundationHostManagementService>();
      if (projectController.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return await projectController.CreateProject(projectController.TfsRequestContext, projectName, projectDescription, processTemplateTypeId, source, projectData);
      List<Guid> collectionIds = (List<Guid>) null;
      if (collectionId.HasValue && !object.Equals((object) collectionId, (object) Guid.Empty))
      {
        collectionIds = new List<Guid>();
        collectionIds.Add(collectionId.Value);
      }
      Guid id;
      try
      {
        IVssRequestContext context = projectController.TfsRequestContext.To(TeamFoundationHostType.Application);
        id = (context.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(context.Elevate(), (IList<Guid>) collectionIds, ServiceHostFilterFlags.None).Single<TeamProjectCollectionProperties>() ?? throw new ArgumentException(string.Format(AdminServerResources.InvalidCollectionId, collectionId.HasValue ? (object) collectionId.ToString() : (object) string.Empty))).Id;
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException(AdminServerResources.CreateProjectDefaultCollectionMissing);
      }
      if (projectController.TfsRequestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, id) == null)
        throw new InvalidOperationException(AdminServerResources.CreateProjectDefaultCollectionMissing);
      using (IVssRequestContext collectionContext = service.BeginRequest(requestContext, id, RequestContextType.UserContext, true, true))
        return await projectController.CreateProject(collectionContext, projectName, projectDescription, processTemplateTypeId, source, projectData);
    }

    private async Task<ActionResult> CreateProject(
      IVssRequestContext collectionContext,
      string projectName,
      string projectDescription,
      Guid processTemplateTypeId,
      string source,
      CreateProjectData projectData)
    {
      ApiProjectController projectController = this;
      ProjectVisibility projectVisibility = projectController.VisibilitySettingToVisibilityEnum(collectionContext, projectData.ProjectVisibilityOption);
      bool flag1 = false;
      OperationReference operationReference = (OperationReference) null;
      SourceControlTypes result;
      if (Enum.TryParse<SourceControlTypes>(projectData.VersionControlOption, true, out result))
      {
        IVssRequestContext requestContext = collectionContext;
        TeamProject projectToCreate = new TeamProject();
        projectToCreate.Name = projectName;
        projectToCreate.Description = projectDescription;
        projectToCreate.Visibility = projectVisibility;
        Guid templateTypeId = processTemplateTypeId;
        int sourceControlType = (int) result;
        (flag1, operationReference) = await ProjectsUtility.TryAssignPreCreatedProject(requestContext, projectToCreate, templateTypeId, (SourceControlTypes) sourceControlType, (string) null);
      }
      bool flag2 = flag1 || !ProjectsUtility.DoesAnyProjectExist(collectionContext);
      if (!flag1)
      {
        Dictionary<string, string> servicingTokens = new Dictionary<string, string>();
        projectData.Populate((IDictionary<string, string>) servicingTokens);
        operationReference = JobOperationsUtility.GetOperationReference(collectionContext, collectionContext.GetService<IProjectWorkflowService>().QueueCreateProject(collectionContext, projectName, projectDescription, processTemplateTypeId, (IDictionary<string, string>) servicingTokens, projectVisibility).JobId);
      }
      IVssRequestContext requestContext1 = collectionContext;
      Dictionary<string, string> projectData1 = new Dictionary<string, string>();
      projectData1.Add(CustomerIntelligenceProperty.Action, nameof (CreateProject));
      projectData1.Add("Type", projectData.VersionControlOption);
      projectData1.Add("Visibility", projectData.ProjectVisibilityOption);
      projectData1.Add("FirstProject", flag2.ToString());
      projectData1.Add("Source", source);
      projectData1.Add("ProcessTemplateTypeId", processTemplateTypeId.ToString());
      bool? createReadMe = projectData.CreateReadMe;
      string str;
      if (!createReadMe.HasValue)
      {
        str = "Undefined";
      }
      else
      {
        createReadMe = projectData.CreateReadMe;
        str = createReadMe.ToString();
      }
      projectData1.Add("CreateReadMe", str);
      projectData1.Add("ProjectName", projectName);
      projectData1.Add("IsPreCreated", flag1.ToString());
      ProjectsUtility.PublishProjectDataToCustomerIntelligence(requestContext1, projectData1);
      CreateProjectModel createProjectModel = new CreateProjectModel()
      {
        JobId = new Guid?(operationReference.Id),
        CollectionContext = collectionContext
      };
      return (ActionResult) projectController.Json((object) createProjectModel.ToJson());
    }

    [HttpPost]
    [ValidateInput(false)]
    [TfsTraceFilter(503050, 503060)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult DeleteProject(string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ProjectsUtility.ThrowIfProjectOperationsNotAllowed(this.TfsRequestContext);
      PlatformProjectService service = this.TfsRequestContext.GetService<PlatformProjectService>();
      Guid guid = service.SoftDeleteProject(this.TfsRequestContext, service.GetProject(this.TfsRequestContext, projectName, false).Id, out ProjectInfo _);
      ProjectsUtility.PublishProjectDataToCustomerIntelligence(this.TfsRequestContext, new Dictionary<string, string>()
      {
        {
          CustomerIntelligenceProperty.Action,
          nameof (DeleteProject)
        },
        {
          "SoftDelete",
          true.ToString()
        }
      });
      return (ActionResult) this.Json((object) new DeleteProjectModel()
      {
        JobId = new Guid?(guid)
      }.ToJson());
    }

    [HttpGet]
    public ActionResult ProcessTemplates()
    {
      ProcessTemplatesModel processTemplatesModel = new ProcessTemplatesModel();
      Guid guid = Guid.Empty;
      if (this.NavigationContext.TopMostLevel == NavigationContextLevels.Collection)
      {
        guid = this.TfsWebContext.TfsRequestContext.ServiceHost.InstanceId;
      }
      else
      {
        TeamProjectCollectionProperties collectionProperties = this.TfsRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(this.TfsRequestContext.Elevate(), (IList<Guid>) null, ServiceHostFilterFlags.None).FirstOrDefault<TeamProjectCollectionProperties>();
        if (collectionProperties != null)
          guid = collectionProperties.Id;
      }
      if (!object.Equals((object) guid, (object) Guid.Empty))
      {
        using (IVssRequestContext vssRequestContext = this.TfsRequestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(this.TfsRequestContext, guid, RequestContextType.SystemContext, true, true))
        {
          ITeamFoundationProcessService service = vssRequestContext.GetService<ITeamFoundationProcessService>();
          Guid defaultProcessTypeId = service.GetDefaultProcessTypeId(vssRequestContext);
          foreach (ProcessDescriptor enabledDescriptor in this.GetActiveEnabledDescriptors(vssRequestContext, service))
            processTemplatesModel.Templates.Add(new ProcessTemplateDescriptorModel(enabledDescriptor, enabledDescriptor.TypeId == defaultProcessTypeId));
          processTemplatesModel.Templates.Sort();
        }
      }
      return (ActionResult) this.Json((object) processTemplatesModel.ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(503060, 503070)]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult ProjectCreateOptions()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      bool flag1 = MicrosoftAadHelpers.IsMicrosoftTenant(this.TfsRequestContext);
      bool flag2 = false;
      if (!vssRequestContext.IsHosted())
      {
        try
        {
          flag2 = vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryResources(vssRequestContext, (IEnumerable<Guid>) new Guid[1]
          {
            CatalogResourceTypes.ReportingConfiguration
          }, CatalogQueryOptions.None).Any<CatalogResource>();
        }
        catch (Exception ex)
        {
          vssRequestContext.TraceException(15118000, this.TraceArea, nameof (ApiProjectController), ex);
        }
      }
      var data = new
      {
        IsReportingConfigured = flag2,
        ShowNewProjectVisibilityDropDown = flag1
      };
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    internal IEnumerable<ProcessDescriptor> GetActiveEnabledDescriptors(
      IVssRequestContext context,
      ITeamFoundationProcessService service)
    {
      ISet<Guid> disabledProcessTypeIds = service.GetDisabledProcessTypeIds(context);
      return (IEnumerable<ProcessDescriptor>) service.GetProcessDescriptors(context).Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (d => !disabledProcessTypeIds.Contains(d.TypeId))).ToList<ProcessDescriptor>();
    }

    private void EnsureDeletePermission(string projectUri)
    {
      IVssSecurityNamespace securityNamespace = this.TfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.TfsRequestContext, securityNamespace, projectUri);
      securityNamespace.CheckPermission(this.TfsRequestContext, token, TeamProjectPermissions.Delete);
    }

    private ProjectVisibility VisibilitySettingToVisibilityEnum(
      IVssRequestContext requestContext,
      string projectVisibilityOption)
    {
      switch (projectVisibilityOption)
      {
        case "Everyone":
          return ProjectVisibility.Public;
        case "EveryoneInTenant":
          return ProjectVisibility.Organization;
        default:
          return ProjectVisibility.Private;
      }
    }

    [HttpGet]
    [ValidateInput(false)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    [TfsHandleFeatureFlag("Project.PreCreation.EnableProjectAssignmentEndpoint", null)]
    public ActionResult Redirect()
    {
      ProjectInfo project1;
      if (ProjectsUtility.TryGetPreCreatedProject(this.TfsRequestContext, out project1))
      {
        PlatformProjectService service = this.TfsRequestContext.GetService<PlatformProjectService>();
        ProjectInfo projectToUpdate = ProjectInfo.GetProjectToUpdate(project1.Id);
        projectToUpdate.Name = "MyFirstProject";
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        ProjectInfo project2 = projectToUpdate;
        service.AssignProject(tfsRequestContext, project2);
      }
      return (ActionResult) this.Redirect(this.TfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(this.TfsRequestContext, Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.SelfReferenceLocationServiceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker));
    }
  }
}
