// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectsUtility
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Server.Core.Audit;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class ProjectsUtility
  {
    private const int c_defaultMaxProjectDescriptionLength = 15999;
    private const string c_projectGroupCacheKeyPrefix = "ProjectGroup";
    private const string c_defaultCustomerIntelligenceDataValue = "unknown";
    private const string c_initialWaitTime = "InitialWaitTime";
    private const string c_pollingInterval = "PollingInterval";
    private const string c_maxWaitTime = "MaxWaitTime";
    private const string c_alwaysAllowOrganizationProjectsInL2 = "Project.Creation.AlwaysAllowOrganizationProjectsInL2";
    private static readonly RegistryQuery s_maxDescriptionLengthRegistryPath = new RegistryQuery(FrameworkServerConstants.CollectionsRoot + "/MaxProjectDescriptionLength");
    private static readonly RegistryQuery s_deletePreCreatedProjectSettings = new RegistryQuery("/Service/Project/DeletePreCreatedProjectSettings/...");
    private const string c_area = "Project";
    private const string c_layer = "ProjectsUtility";

    public static TeamProject GetTeamProject(
      IVssRequestContext requestContext,
      string projectId,
      bool includeCapabilities,
      bool includeHistory = false)
    {
      return ProjectsUtility.GetTeamProject(requestContext, projectId, includeCapabilities, true, true, includeHistory);
    }

    public static TeamProject GetTeamProject(
      IVssRequestContext requestContext,
      string projectId,
      bool includeCapabilities,
      bool includeCollection,
      bool includeDefaultTeam,
      bool includeHistory = false)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      IProjectService service = requestContext.GetService<IProjectService>();
      Guid result;
      ProjectInfo projectInfo = !Guid.TryParse(projectId, out result) ? service.GetProject(requestContext, projectId, includeHistory) : service.GetProject(requestContext, result);
      if (includeCapabilities)
        projectInfo.PopulateProperties(requestContext, ProjectApiConstants.CapabilitiesProperties);
      return projectInfo.ToTeamProject(requestContext, includeCapabilities, includeCollection, includeDefaultTeam);
    }

    public static IEnumerable<TeamProjectReference> GetTeamProjects(
      IVssRequestContext requestContext,
      ProjectState stateFilter,
      bool getDefaultTeamImageUrl = false)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      IEnumerable<ProjectInfo> projectInfos = requestContext.GetService<IProjectService>().GetProjects(requestContext, stateFilter);
      if (getDefaultTeamImageUrl)
        projectInfos = projectInfos.PopulateProperties(requestContext, TeamConstants.DefaultTeamPropertyName);
      return projectInfos.Select<ProjectInfo, TeamProjectReference>((Func<ProjectInfo, TeamProjectReference>) (project => project.ToTeamProjectReference(requestContext, getDefaultTeamImageUrl)));
    }

    public static IEnumerable<WebApiProject> GetWebApiProjects(
      IVssRequestContext requestContext,
      ProjectState stateFilter,
      bool includeCapabilities,
      int top,
      int skip)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      IEnumerable<ProjectInfo> projectInfos = requestContext.GetService<IProjectService>().GetProjects(requestContext, stateFilter);
      if (includeCapabilities)
        projectInfos = projectInfos.PopulateProperties(requestContext, ProjectApiConstants.CapabilitiesProperties);
      return projectInfos.Select<ProjectInfo, WebApiProject>((Func<ProjectInfo, WebApiProject>) (project => project.ToTeamProject(requestContext, includeCapabilities).ToWebApiProject(requestContext))).Skip<WebApiProject>(skip).Take<WebApiProject>(top);
    }

    public static OperationReference UpdateTeamProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string newName,
      string newAbbreviation,
      string newDescription,
      ProjectVisibility newVisibility)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      return ProjectApiExtensions.Update(requestContext, projectId, newName, newAbbreviation, newDescription, newVisibility);
    }

    public static IEnumerable<TeamProjectReference> GetProjectHistory(
      IVssRequestContext requestContext,
      long minRevision)
    {
      return ProjectsUtility.GetProjectHistoryEntries(requestContext, minRevision).Select<ProjectInfo, TeamProjectReference>((Func<ProjectInfo, TeamProjectReference>) (project => project.ToTeamProjectReference(requestContext)));
    }

    public static IEnumerable<ProjectInfo> GetProjectHistoryEntries(
      IVssRequestContext requestContext,
      long minRevision)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      return (IEnumerable<ProjectInfo>) requestContext.GetService<IProjectService>().GetProjectHistory(requestContext, minRevision);
    }

    public static IEnumerable<TeamProjectReference> GetSoftDeletedTeamProjects(
      IVssRequestContext requestContext,
      bool getDefaultTeamImage = false)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      PlatformProjectService service = requestContext.GetService<PlatformProjectService>();
      string[] strArray1;
      if (!getDefaultTeamImage)
        strArray1 = new string[2]
        {
          "System.SoftDeletedProjectName",
          "System.SoftDeletedTimestamp"
        };
      else
        strArray1 = new string[3]
        {
          "System.SoftDeletedProjectName",
          "System.SoftDeletedTimestamp",
          TeamConstants.DefaultTeamPropertyName
        };
      string[] strArray2 = strArray1;
      List<TeamProjectReference> source = new List<TeamProjectReference>();
      foreach (ProjectInfo populateProperty in service.GetSoftDeletedProjects(requestContext).PopulateProperties(requestContext, strArray2))
      {
        populateProperty.State = ProjectState.Deleted;
        populateProperty.Visibility = ProjectVisibility.Private;
        ProjectProperty projectProperty1 = populateProperty.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (prop => prop.Name.Equals("System.SoftDeletedProjectName", StringComparison.OrdinalIgnoreCase)));
        ProjectProperty projectProperty2 = populateProperty.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (prop => prop.Name.Equals("System.SoftDeletedTimestamp", StringComparison.OrdinalIgnoreCase)));
        if (projectProperty1 == null)
        {
          if (projectProperty2 == null)
          {
            try
            {
              if (service.GetProject(requestContext, populateProperty.Id, true).IsSoftDeleted)
                requestContext.Trace(5500929, TraceLevel.Error, "Project", nameof (ProjectsUtility), string.Format("Unable to retrieve soft-delete properties for project {0}", (object) populateProperty.Id));
              else
                continue;
            }
            catch (ProjectDoesNotExistException ex)
            {
              continue;
            }
          }
        }
        if (projectProperty1 != null && projectProperty1.Value != null)
          populateProperty.Name = projectProperty1.Value.ToString();
        if (projectProperty2 != null && projectProperty2.Value != null && projectProperty2.Value is DateTime dateTime)
          populateProperty.LastUpdateTime = dateTime;
        source.Add(populateProperty.ToTeamProjectReference(requestContext, getDefaultTeamImage));
      }
      return (IEnumerable<TeamProjectReference>) source.OrderByDescending<TeamProjectReference, DateTime>((Func<TeamProjectReference, DateTime>) (projecRef => projecRef.LastUpdateTime));
    }

    public static void ThrowIfProjectOperationsNotAllowed(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (vssRequestContext.GetService<IVssRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.ProjectCreationLockdownPath, false))
        throw new InvalidOperationException(Resources.ProjectLockdown());
      ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
    }

    public static bool DoesAnyProjectExist(IVssRequestContext requestContext)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      return requestContext.GetService<IProjectService>().GetProjects(requestContext.Elevate(), ProjectState.WellFormed).Any<ProjectInfo>();
    }

    public static bool TryGetPreCreatedProject(
      IVssRequestContext requestContext,
      out ProjectInfo project)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      IProjectService service = requestContext.GetService<IProjectService>();
      IEnumerable<ProjectInfo> projects = service.GetProjects(requestContext.Elevate(), ProjectState.WellFormed);
      if (projects.Take<ProjectInfo>(2).Count<ProjectInfo>() == 1)
      {
        project = projects.Single<ProjectInfo>();
        ProjectProperty projectProperty = service.GetProjectProperties(requestContext.Elevate(), project.Id, "System.ProjectPreCreated").SingleOrDefault<ProjectProperty>();
        if (projectProperty != null && bool.Parse(projectProperty.Value.ToString()))
          return true;
      }
      project = (ProjectInfo) null;
      return false;
    }

    public static async Task<(bool isPreCreated, OperationReference opRef)> TryAssignPreCreatedProject(
      IVssRequestContext requestContext,
      TeamProject projectToCreate,
      Guid templateTypeId,
      SourceControlTypes sourceControlType,
      string featuresEnabled)
    {
      ProjectInfo project;
      if (ProjectsUtility.TryGetPreCreatedProject(requestContext, out project))
      {
        PlatformProjectService service = requestContext.GetService<PlatformProjectService>();
        ProjectProperty projectProperty = service.GetProjectProperties(requestContext.Elevate(), project.Id, ProcessTemplateIdPropertyNames.ProcessTemplateType).SingleOrDefault<ProjectProperty>();
        Guid result = Guid.Empty;
        if (projectProperty != null && Guid.TryParse((string) projectProperty.Value, out result))
        {
          if (templateTypeId == ProcessTemplateTypeIdentifiers.DefaultPreCreateProcess)
            templateTypeId = result;
        }
        else
          requestContext.Trace(982548, TraceLevel.Error, "Project", nameof (ProjectsUtility), "Skip using precreated project because we are unable to get its process template type.");
        if (result != Guid.Empty && templateTypeId == result && sourceControlType == SourceControlTypes.Git)
        {
          ProjectInfo projectToUpdate = ProjectInfo.GetProjectToUpdate(project.Id);
          projectToUpdate.Name = projectToCreate.Name;
          projectToUpdate.Visibility = projectToCreate.Visibility;
          OperationReference operationReference = JobOperationsUtility.GetOperationReference(requestContext, service.AssignProject(requestContext, projectToUpdate, featuresEnabled));
          operationReference.Status = OperationStatus.Succeeded;
          ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, templateTypeId);
          IVssRequestContext requestContext1 = requestContext;
          string create = ProjectAuditConstants.Create;
          Dictionary<string, object> data = ProjectAuditData.Create(projectToCreate.Name, processDescriptor.Name, projectToCreate.Visibility);
          Guid id = projectToUpdate.Id;
          Guid targetHostId = new Guid();
          Guid projectId = id;
          requestContext1.LogAuditEvent(create, data, targetHostId, projectId);
          return (true, operationReference);
        }
        service.CheckGlobalPermission(requestContext, TeamProjectCollectionPermissions.CreateProjects);
        ServicingJobDetail servicingJobDetail = ProjectsUtility.QueueDeleteProjectServicingJob(requestContext.Elevate(), ProjectInfo.GetProjectUri(project.Id));
        if (TFStringComparer.TeamProjectName.Equals(projectToCreate.Name, project.Name))
          await ProjectsUtility.AwaitProjectDeletionJobCompletion(requestContext, servicingJobDetail.JobId);
      }
      return (false, (OperationReference) null);
    }

    public static void CheckCreateProjectPermission(IVssRequestContext requestContext)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId).CheckPermission(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceToken, TeamProjectCollectionPermissions.CreateProjects);
    }

    public static void CheckDeleteProjectPermission(
      IVssRequestContext requestContext,
      string projectUri)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectUri);
      securityNamespace.CheckPermission(requestContext, token, TeamProjectPermissions.Delete);
    }

    public static void PublishProjectDataToCustomerIntelligence(
      IVssRequestContext requestContext,
      Dictionary<string, string> projectData)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      foreach (KeyValuePair<string, string> keyValuePair in projectData)
        properties.Add(keyValuePair.Key, keyValuePair.Value ?? "unknown");
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Account, CustomerIntelligenceFeature.TraceAccount, properties);
    }

    public static void CheckProjectVisibility(
      IVssRequestContext requestContext,
      ProjectVisibility visibility)
    {
      if (visibility != ProjectVisibility.Organization && visibility != ProjectVisibility.Public)
        return;
      requestContext.CheckHostedDeployment();
      if (visibility == ProjectVisibility.Public && !ProjectsUtility.AllowPublicProjects(requestContext) || visibility == ProjectVisibility.Organization && !ProjectsUtility.AllowOrganizationProjects(requestContext))
        throw new InvalidOperationException(Resources.InvalidProjectVisibility((object) visibility));
    }

    public static bool AllowPublicProjects(IVssRequestContext requestContext)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess") && service.GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.AllowPublicProjectsRegistryPath, true, true);
      if (flag && requestContext.ExecutionEnvironment.IsHostedDeployment)
        flag = requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowAnonymousAccess", false).EffectiveValue;
      return flag;
    }

    public static bool AllowProjectVisibilityForMicrosoftTenant(IVssRequestContext requestContext) => requestContext.IsOrganizationActivated() || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.UseTenantAsBoundaryForOidProjectVisibility") && requestContext.IsMicrosoftTenant() || requestContext.IsFeatureEnabled("Project.Creation.AlwaysAllowOrganizationProjectsInL2");

    public static bool AllowOrganizationProjects(IVssRequestContext requestContext)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      return ProjectsUtility.AllowProjectVisibilityForMicrosoftTenant(requestContext) && requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowOrgAccess", true).EffectiveValue;
    }

    public static void CheckProjectDescription(
      IVssRequestContext requestContext,
      string projectDescription)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      if (string.IsNullOrEmpty(projectDescription))
        return;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in ProjectsUtility.s_maxDescriptionLengthRegistryPath, 15999);
      if (projectDescription.Length > num)
        throw new ArgumentException(CommonResources.PropertyArgumentExceededMaximumSizeAllowed((object) nameof (projectDescription), (object) num)).Expected(requestContext.ServiceName);
      ArgumentUtility.CheckStringForInvalidCharacters(projectDescription, nameof (projectDescription), true, true, requestContext.ServiceName);
    }

    public static string GetPublicUrlForNotification(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.SelfReferenceLocationServiceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);

    public static string GetProjectScopedGroupCacheKey(string projectUri, string groupName) => ("ProjectGroup_" + projectUri + "_" + groupName).ToLower();

    internal static IList<ProjectProperty> ToProjectProperties(
      this PatchDocument<IDictionary<string, object>> patchDocument)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IList<ProjectProperty>) patchDocument.Operations.Select<IPatchOperation<IDictionary<string, object>>, ProjectProperty>(ProjectsUtility.\u003C\u003EO.\u003C0\u003E__ToProjectProperty ?? (ProjectsUtility.\u003C\u003EO.\u003C0\u003E__ToProjectProperty = new Func<IPatchOperation<IDictionary<string, object>>, ProjectProperty>(ProjectsUtility.ToProjectProperty))).ToList<ProjectProperty>();
    }

    private static ProjectProperty ToProjectProperty(
      IPatchOperation<IDictionary<string, object>> operation)
    {
      if (operation.Operation != Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add && operation.Operation != Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Remove)
        throw new ArgumentException(FrameworkResources.NotSupportedJsonPatchOperation((object) operation.Operation, (object) operation.Path, operation.Value));
      string name = operation.Path.TrimStart(ProjectProperty.JsonPatchOperationPathSeparator);
      object int32 = operation.Value;
      if (int32 is long)
      {
        try
        {
          int32 = (object) Convert.ToInt32(int32);
        }
        catch (OverflowException ex)
        {
        }
      }
      return new ProjectProperty(name, int32);
    }

    public static void SetProjectFeatures(
      IVssRequestContext requestContext,
      Guid projectId,
      string projectFeatures)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(projectFeatures, nameof (projectFeatures));
      HashSet<string> hashSet = ((IEnumerable<string>) projectFeatures.Split(',')).ToHashSet<string, string>((Func<string, string>) (s => s.Trim()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (hashSet.Count <= 0)
        return;
      IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
      foreach (ContributedFeature contributedFeature in service.GetFeaturesForTarget(requestContext, "ms.vss-tfs-web.product-features"))
      {
        if (!hashSet.Contains(contributedFeature.Id))
          service.SetFeatureState(requestContext, contributedFeature.Id, ContributedFeatureEnabledValue.Disabled, SettingsUserScope.AllUsers, "project", projectId.ToString());
      }
    }

    internal static ServicingJobDetail QueueDeleteProjectServicingJob(
      IVssRequestContext requestContext,
      string projectUri)
    {
      ProjectsUtility.ThrowIfProjectOperationsNotAllowed(requestContext);
      Dictionary<string, string> servicingTokens = new Dictionary<string, string>()
      {
        {
          ServicingTokenConstants.RequestingUserName,
          requestContext.AuthenticatedUserName
        },
        {
          ProjectServicingTokenConstants.DeleteWorkspaceData,
          bool.TrueString
        }
      };
      return requestContext.GetService<IProjectWorkflowService>().QueueHardDeleteProject(requestContext, projectUri, (IDictionary<string, string>) servicingTokens);
    }

    internal static async Task AwaitProjectDeletionJobCompletion(
      IVssRequestContext requestContext,
      Guid jobId)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, in ProjectsUtility.s_deletePreCreatedProjectSettings);
      int initialWaitTimeInMilliseconds = registryEntryCollection.GetValueFromPath<int>("InitialWaitTime", 2000);
      int pollingIntervalInMilliseconds = registryEntryCollection.GetValueFromPath<int>("PollingInterval", 1000);
      int maxWaitTimeInMilliseconds = registryEntryCollection.GetValueFromPath<int>("MaxWaitTime", 30000);
      int elapsedTimeInMilliseconds = 0;
      IOperationsService operationService = requestContext.GetService<IOperationsService>();
      while (!operationService.GetOperation(requestContext, Guid.Empty, jobId).Completed)
      {
        if (elapsedTimeInMilliseconds >= maxWaitTimeInMilliseconds)
        {
          requestContext.Trace(868788, TraceLevel.Error, "Project", nameof (ProjectsUtility), "Delete precreated project job timed out after {0} milliseconds.", (object) elapsedTimeInMilliseconds);
          operationService = (IOperationsService) null;
          return;
        }
        int millisecondsDelay = elapsedTimeInMilliseconds == 0 ? initialWaitTimeInMilliseconds : pollingIntervalInMilliseconds;
        await Task.Delay(millisecondsDelay);
        elapsedTimeInMilliseconds += millisecondsDelay;
      }
      requestContext.Trace(868787, TraceLevel.Info, "Project", nameof (ProjectsUtility), "Delete precreated project job succeeded after {0} milliseconds.", (object) elapsedTimeInMilliseconds);
      operationService = (IOperationsService) null;
    }
  }
}
