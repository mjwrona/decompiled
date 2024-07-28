// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleasesService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
  public class ReleasesService : ReleaseManagement2ServiceBase
  {
    private const string TraceLayer = "ReleasesService";
    private const string FolderPathSuffixForSubfolderExpansion = "\\**\\*";
    private const string SelectiveArtifactsDownloadFeatureId = "ms.vss-releaseManagement-web.selective-artifacts-download-feature";
    private const string DownloadArtifactsUsingAgentPlugin = "release.artifact.download.useagentplugin";
    private readonly Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer;
    private readonly SecretsHelper secretsHelper;

    public ReleasesService()
      : this(ReleasesService.\u003C\u003EO.\u003C0\u003E__GetDataAccessLayer ?? (ReleasesService.\u003C\u003EO.\u003C0\u003E__GetDataAccessLayer = new Func<IVssRequestContext, Guid, IDataAccessLayer>(ReleasesService.GetDataAccessLayer)), new SecretsHelper())
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected ReleasesService(
      Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer,
      SecretsHelper secretsHelper)
    {
      this.getDataAccessLayer = getDataAccessLayer;
      this.secretsHelper = secretsHelper;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "It is optional")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "It is optional")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> ListReleases(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int definitionEnvironmentId,
      string searchText,
      string createdBy,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus statusFilter,
      ReleaseEnvironmentStatus environmentStatusFilter,
      DateTime? minCreatedTime,
      DateTime? maxCreatedTime,
      DateTime? maxModifiedTime,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder queryOrder,
      int top,
      int releaseContinuationToken,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      bool includeVariables,
      bool includeEnvironments,
      bool includeTags,
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      bool isDeleted,
      bool includeDeletedReleaseDefinitions,
      IEnumerable<string> propertyFilters,
      IEnumerable<string> tagFilter = null,
      IEnumerable<int> releaseIdFilter = null,
      string path = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>() && projectId == Guid.Empty)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ReleaseIDFilterNotExpectedForCollectionLevel);
      includeEnvironments = includeApprovals | includeManualInterventions | includeEnvironments;
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleasesService.ListReleases", 1971001))
      {
        List<Guid> createdByFilter = new List<Guid>();
        if (!string.IsNullOrWhiteSpace(createdBy))
        {
          createdByFilter = context.GetIdentityIds(createdBy).ToList<Guid>();
          releaseManagementTimer.RecordLap("DataAccessLayer", "ReleasesService.ListReleases.ReadIdentities", 1971040);
          if (!createdByFilter.Any<Guid>() || createdByFilter.Contains(Guid.Empty))
            return Enumerable.Empty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>();
        }
        path = ReleasesService.AddFolderPathRecursiveSuffix(path);
        Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>> action = (Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>) (component => component.ListReleases(projectId, searchText, definitionId, definitionEnvironmentId, statusFilter, environmentStatusFilter, (IEnumerable<Guid>) createdByFilter, minCreatedTime, maxCreatedTime, maxModifiedTime, top, queryOrder, releaseContinuationToken, includeEnvironments, includeArtifacts, includeApprovals, includeManualInterventions, artifactTypeId, sourceId, artifactVersionId, sourceBranchFilter, isDeleted, includeDeletedReleaseDefinitions, includeVariables, includeTags, tagFilter, releaseIdFilter, path));
        IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> source = context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>(action);
        if (source != null && propertyFilters != null && propertyFilters.Any<string>())
        {
          using (TeamFoundationDataReader properties = context.GetService<ITeamFoundationPropertyService>().GetProperties(context, source.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ArtifactSpec>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ArtifactSpec>) (x => x.CreateArtifactSpec(projectId))), propertyFilters))
            ReleaseManagementArtifactPropertyKinds.MatchProperties<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(properties, (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) source.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(), (Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>) (x => x.Id), (Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IList<PropertyValue>>) ((x, y) => x.Properties.AddRange<PropertyValue, IList<PropertyValue>>((IEnumerable<PropertyValue>) y)));
        }
        if (source == null)
          return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) null;
        List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> list = source.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>();
        list.ForEach((Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (i => ReleasesService.EnsurePrimaryArtifactIsSet(context, i)));
        return queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdAscending ? (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) list.OrderBy<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>) (r => r.Id)) : (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) list.OrderByDescending<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>) (r => r.Id));
      }
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> GetHardDeleteReleaseCandidates(
      IVssRequestContext context,
      Guid projectId,
      DateTime? maxModifiedTime,
      int maxReleases,
      int continuationToken)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (projectId.Equals(Guid.Empty))
        throw new ArgumentNullException(nameof (projectId));
      if (!context.IsSystemContext)
        throw new UnauthorizedAccessException();
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleasesService.GetHardDeleteReleaseCandidates", 1961231))
      {
        Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>> action = (Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>) (component => component.ListHardDeleteReleaseCandidates(projectId, maxModifiedTime, maxReleases, continuationToken));
        return context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual KeyValuePair<int, string> GetReleaseDefinitionFolderPathAndId(
      IVssRequestContext context,
      Guid projectId,
      int releaseId)
    {
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleasesService.GetReleaseDefinitionFolderPathAndId", 1971032))
      {
        Func<ReleaseSqlComponent, KeyValuePair<int, string>> action = (Func<ReleaseSqlComponent, KeyValuePair<int, string>>) (component => component.GetReleaseDefinitionFolderPathAndId(projectId, releaseId));
        return context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, KeyValuePair<int, string>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentData GetReleaseEnvironmentData(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      bool includeDeployments,
      bool includeApprovals,
      bool includeArtifacts)
    {
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleasesService.GetReleaseEnvironmentData", 1900019))
      {
        Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentData> action = (Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentData>) (component => component.GetReleaseEnvironmentData(projectId, releaseId, releaseEnvironmentId, includeDeployments, includeApprovals, includeArtifacts));
        return context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentData>(action);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release GetRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      bool isDeleted = false,
      IEnumerable<string> propertyFilters = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (new MethodScope(context, nameof (ReleasesService), nameof (GetRelease)))
      {
        using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleasesService.GetRelease", 1971004))
        {
          context.TraceEnter(0, "ReleaseManagementService", nameof (ReleasesService), nameof (GetRelease));
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release;
          try
          {
            Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> action = (Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.GetRelease(projectId, releaseId, isDeleted));
            release = context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(action);
          }
          finally
          {
            context.TraceLeave(0, "ReleaseManagementService", nameof (ReleasesService), nameof (GetRelease));
          }
          if (release == null)
            return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release) null;
          if (propertyFilters != null && propertyFilters.Any<string>())
            release.PopulateProperties(context, projectId, propertyFilters);
          ReleasesService.EnsurePrimaryArtifactIsSet(context, release);
          return release;
        }
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue> AddAutoTriggerIssues(
      IVssRequestContext context,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue> autoTriggerIssuesList)
    {
      if (autoTriggerIssuesList == null)
        throw new ArgumentNullException(nameof (autoTriggerIssuesList));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue> autoTriggerIssueList = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>();
      if (autoTriggerIssuesList.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>())
      {
        Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>> action = (Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>>) (component => component.AddAutoTriggerIssues(autoTriggerIssuesList));
        autoTriggerIssueList = context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>>(action).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>();
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>) autoTriggerIssueList;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue> GetAutoTriggerIssues(
      IVssRequestContext context,
      Guid projectId,
      string artifactType,
      string artifactVersionId,
      string sourceId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>> action = (Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>>) (component => component.GetAutoTriggerIssues(projectId, artifactType, sourceId, artifactVersionId));
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue> source = context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>>(action);
      return source != null ? (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>) source.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>() : (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue>();
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release CreateRelease(
      IVssRequestContext context,
      ReleaseProjectInfo releaseProjectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      CreateReleaseParameters createReleaseParameters)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "Service", "ReleasesService.CreateRelease", 1971014))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release releaseImplementation = this.CreateReleaseImplementation(context, releaseProjectInfo, releaseDefinition, createReleaseParameters, ReleasesService.\u003C\u003EO.\u003C1\u003E__GetReleaseBuilder ?? (ReleasesService.\u003C\u003EO.\u003C1\u003E__GetReleaseBuilder = new Func<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IReleaseBuilder>(ReleasesService.GetReleaseBuilder)));
        ReleasesService.OnCreateRelease(context, releaseProjectInfo, releaseImplementation);
        return releaseImplementation;
      }
    }

    private static string AddFolderPathRecursiveSuffix(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        return (string) null;
      if (string.Equals("\\", path))
        path = "\\**\\*";
      else if (!path.Contains("\\**\\*"))
        path += "\\**\\*";
      return path;
    }

    private static IReleaseBuilder GetReleaseBuilder(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition)
    {
      return definition.IsYamlDefinition(requestContext) ? (IReleaseBuilder) new YamlReleaseBuilder() : (IReleaseBuilder) new ReleaseBuilder();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of method.")]
    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release CreateReleaseImplementation(
      IVssRequestContext requestContext,
      ReleaseProjectInfo releaseProjectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      CreateReleaseParameters createReleaseParameters,
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IReleaseBuilder> getReleaseBuilder)
    {
      if (releaseProjectInfo == null)
        throw new ArgumentNullException(nameof (releaseProjectInfo));
      if (createReleaseParameters == null)
        throw new ArgumentNullException(nameof (createReleaseParameters));
      if (getReleaseBuilder == null)
        throw new ArgumentNullException(nameof (getReleaseBuilder));
      IDataAccessLayer dataAccessLayer = this.getDataAccessLayer(requestContext, releaseProjectInfo.Id);
      ReleasesService.TraceInformationMessage(requestContext, 1960001, "ReleasesService: CreateRelease: CreateRelease Parameters '{0}'.", (object) createReleaseParameters);
      if (releaseDefinition == null)
        releaseDefinition = dataAccessLayer.GetReleaseDefinition(createReleaseParameters.DefinitionId);
      ReleasesService.ValidateArtifactSourceBranches(createReleaseParameters, releaseDefinition);
      releaseDefinition = TaskGroupHelper.ReplaceTaskGroupWithTasks(requestContext, releaseProjectInfo.Id, releaseDefinition);
      this.secretsHelper.ReadSecrets(requestContext, releaseProjectInfo.Id, releaseDefinition);
      ReleasesService.TraceInformationMessage(requestContext, 1960002, "ReleasesService: CreateRelease: Got Definition Secrets. ReleaseDefinitionId: '{0}'.", (object) releaseDefinition.Id);
      DeployTimeVariableValidations.ValidateDeployTimeVariables(requestContext, releaseProjectInfo.Id, releaseDefinition, createReleaseParameters);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = getReleaseBuilder(requestContext, releaseDefinition).Build(releaseDefinition, createReleaseParameters, requestContext, releaseProjectInfo);
      string comment = createReleaseParameters.Comment;
      DeployTimeVariableValidations.ValidateDeployTimeEndpointVariable(requestContext, releaseProjectInfo.Id, releaseDefinition, release1);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release2 = dataAccessLayer.AddRelease(release1, comment);
      ReleaseManagementArtifactPropertyKinds.CopyProperties(release2.Properties, release1.Properties);
      bool result;
      if (((!release2.Variables.ContainsKey("release.artifact.download.useagentplugin") ? 0 : (bool.TryParse(release2.Variables["release.artifact.download.useagentplugin"].Value, out result) ? 1 : 0)) & (result ? 1 : 0)) == 0)
      {
        IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
        string str = (requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks") || service.IsFeatureEnabled(requestContext, "ms.vss-releaseManagement-web.selective-artifacts-download-feature")).ToString();
        release2.Properties.Add(new PropertyValue("DownloadBuildArtifactsUsingTask", (object) str));
      }
      dataAccessLayer.SaveProperties(release2.CreateArtifactSpec(releaseProjectInfo.Id), (IEnumerable<PropertyValue>) release2.Properties);
      ReleasesService.TraceInformationMessage(requestContext, 1960003, "ReleasesSevice: CreateRelease: Stored Release. ReleaseId: '{0}' ReleaseName:{1}.", (object) release2.Id, (object) release2.Name);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release3 = release2.DeepClone();
      release3.FillSecrets(release1);
      this.secretsHelper.StoreSecrets(requestContext, releaseProjectInfo.Id, release3);
      ReleasesService.TraceInformationMessage(requestContext, 1960004, "ReleasesSevice: CreateRelease: Stored Release Secrets. ReleaseId: '{0}' ReleaseName:{1}.", (object) release2.Id, (object) release2.Name);
      EventsHelper.FireReleaseCreatedEvent(requestContext, releaseProjectInfo.Id, release2);
      ReleasesService.SavePullRequestRelease(requestContext, releaseProjectInfo.Id, release2);
      return release2;
    }

    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release InitiateRelease(
      IVssRequestContext context,
      ReleaseProjectInfo releaseProjectInfo,
      CreateReleaseParameters createReleaseParameters)
    {
      return this.InitiateRelease(context, releaseProjectInfo, createReleaseParameters, (IList<DeploymentAuthorizationInfo>) null);
    }

    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release InitiateRelease(
      IVssRequestContext context,
      ReleaseProjectInfo releaseProjectInfo,
      CreateReleaseParameters createReleaseParameters,
      IList<DeploymentAuthorizationInfo> authorizationInfo)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseProjectInfo == null)
        throw new ArgumentNullException(nameof (releaseProjectInfo));
      if (createReleaseParameters == null)
        throw new ArgumentNullException(nameof (createReleaseParameters));
      OrchestratorServiceProcessorV2 serviceProcessorV2 = new OrchestratorServiceProcessorV2(context, releaseProjectInfo.Id);
      IDataAccessLayer dataAccessLayer = this.getDataAccessLayer(context, releaseProjectInfo.Id);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition = dataAccessLayer.GetReleaseDefinition(createReleaseParameters.DefinitionId);
      IList<ArtifactSource> linkedArtifacts = releaseDefinition.LinkedArtifacts;
      ArtifactUtility.ResolveSourceBranchVariablesForArtifactSources(context, releaseProjectInfo.Id, linkedArtifacts, releaseDefinition.Id);
      ReleasesService.PopulateDefaultArtifactVersions(context, releaseProjectInfo, releaseDefinition, createReleaseParameters);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.CreateRelease(context, releaseProjectInfo, releaseDefinition, createReleaseParameters);
      if (context.IsFeatureEnabled("AzureDevOps.ReleaseManagement.UseDirectReleaseStageScheduling"))
        ReleasesService.AddStageSchedulingJobsForRelease(context, release, releaseDefinition, releaseProjectInfo.Id, dataAccessLayer);
      using (ReleaseManagementTimer.Create(context, "Service", "ReleasesService.InitiateRelease", 1971007))
        return serviceProcessorV2.StartRelease(release, createReleaseParameters.Comment);
    }

    private static void AddStageSchedulingJobsForRelease(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Guid projectId,
      IDataAccessLayer dataAccessLayer)
    {
      ITeamFoundationJobService jobService = ReleaseOperationHelper.GetJobService(context);
      List<TeamFoundationJobDefinition> jobsToUpdate = new List<TeamFoundationJobDefinition>();
      foreach (DefinitionEnvironment definitionEnvironment in releaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Schedules != null && e.Schedules.Count > 0)))
      {
        DefinitionEnvironment stageDefinition = definitionEnvironment;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.First<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => e.DefinitionEnvironmentId == stageDefinition.Id));
        TeamFoundationJobDefinition stageJobDefinition = Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionExtensions.CreateScheduleStageJobDefinition(release.Id, releaseDefinition.Id, releaseEnvironment.Id, stageDefinition.Id, projectId, jobService.IsIgnoreDormancyPermitted);
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) releaseEnvironment.Schedules)
        {
          schedule.JobId = stageJobDefinition.JobId;
          stageJobDefinition.Schedule.AddRange((IEnumerable<TeamFoundationJobSchedule>) Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionExtensions.ConvertSchedules(schedule));
        }
        dataAccessLayer.UpdateReleaseEnvironment(release.Id, releaseEnvironment.Id, new DateTime?(), new Guid?(stageJobDefinition.JobId));
        jobsToUpdate.Add(stageJobDefinition);
      }
      ReleaseOperationHelper.UpdateJobs(context, (IEnumerable<TeamFoundationJobDefinition>) jobsToUpdate, (IEnumerable<Guid>) null);
    }

    private static void PopulateDefaultArtifactVersions(
      IVssRequestContext context,
      ReleaseProjectInfo releaseProjectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      CreateReleaseParameters createReleaseParameters)
    {
      if (context == null || releaseDefinition?.LinkedArtifacts == null || createReleaseParameters?.ArtifactData == null)
        return;
      List<ArtifactSource> artifactSourceList = new List<ArtifactSource>();
      artifactSourceList.AddRange(releaseDefinition.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (definitionArtifact => !createReleaseParameters.ArtifactData.Any<ArtifactMetadata>((Func<ArtifactMetadata, bool>) (releaseArtifact => string.Equals(definitionArtifact.Alias, releaseArtifact.Alias, StringComparison.OrdinalIgnoreCase))))));
      Dictionary<string, InputValue> dictionary = new Dictionary<string, InputValue>();
      if (artifactSourceList.IsNullOrEmpty<ArtifactSource>())
        return;
      ProjectInfo projectInfo = new ProjectInfo()
      {
        Id = releaseProjectInfo.Id
      };
      artifactSourceList.GetLatestArtifactVersions(context.Elevate(), projectInfo, dictionary, true);
      if (dictionary.IsNullOrEmpty<KeyValuePair<string, InputValue>>())
        return;
      foreach (KeyValuePair<string, InputValue> keyValuePair in dictionary)
      {
        ArtifactMetadata fromArtifactVersion = ArtifactVersionsUtility.GetArtifactMetadataFromArtifactVersion(keyValuePair.Key, keyValuePair.Value);
        if (fromArtifactVersion != null)
          createReleaseParameters.ArtifactData.Add(fromArtifactVersion);
      }
    }

    private static void ValidateArtifactSourceBranches(
      CreateReleaseParameters createReleaseParameters,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (createReleaseParameters == null || createReleaseParameters.ArtifactData == null)
        return;
      foreach (ArtifactMetadata artifactMetadata in (IEnumerable<ArtifactMetadata>) createReleaseParameters.ArtifactData)
      {
        ArtifactMetadata artifactData = artifactMetadata;
        if (releaseDefinition.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (x => string.Equals(x.Alias, artifactData.Alias, StringComparison.OrdinalIgnoreCase))) == null)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCouldNotBeFound, (object) artifactData.Alias));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseEnvironment(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata environmentUpdateData,
      bool forceUpdate = false)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (release.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active && !forceUpdate || release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CannotUpdateEnvironmentAsReleaseIsNotActive, (object) environment.Name, (object) release.Name, (object) Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Active));
      ReleasesService.ValidateRequestData(environmentUpdateData, environment);
      using (ReleaseManagementTimer.Create(context, "Service", "ReleasesService.UpdateReleaseEnvironment", 1971037))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return this.UpdateReleaseEnvironmentImplementation(context, projectId, release, environment, environmentUpdateData, ReleasesService.\u003C\u003EO.\u003C2\u003E__UpdateReleaseEnvironmentStatus ?? (ReleasesService.\u003C\u003EO.\u003C2\u003E__UpdateReleaseEnvironmentStatus = new Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata, bool, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(ReleaseEnvironmentStatusResolver.UpdateReleaseEnvironmentStatus)), forceUpdate);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseEnvironmentImplementation(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata environmentUpdateData,
      Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata, bool, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> updateEnvironmentStatus,
      bool forceUpdate = false)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (updateEnvironmentStatus == null)
        throw new ArgumentNullException(nameof (updateEnvironmentStatus));
      if (environmentUpdateData == null)
        throw new ArgumentNullException(nameof (environmentUpdateData));
      release = this.UpdateEnvironmentIfRequired(context, projectId, release, releaseEnvironment, environmentUpdateData.ScheduledDeploymentTime, environmentUpdateData.Status);
      if (environmentUpdateData.Status != ReleaseEnvironmentStatus.Undefined)
        release = updateEnvironmentStatus(context, projectId, release, releaseEnvironment, environmentUpdateData, forceUpdate);
      return release;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseNameAndStatus(
      IVssRequestContext context,
      ReleaseProjectInfo projectInfo,
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseUpdateMetadata releaseUpdateMetadata)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      ReleaseProcessor releaseProcessor = new ReleaseProcessor(context, projectInfo.Id);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.UpdateReleaseNameAndStatusImplementation(context, projectInfo, releaseId, releaseUpdateMetadata, new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseUpdateMetadata, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(releaseProcessor.UpdateReleaseStatus), ReleasesService.\u003C\u003EO.\u003C3\u003E__FillReleaseNameWhileStartingRelease ?? (ReleasesService.\u003C\u003EO.\u003C3\u003E__FillReleaseNameWhileStartingRelease = new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, string>(ReleaseNameFormatter.FillReleaseNameWhileStartingRelease)));
      EventsHelper.FireReleaseUpdatedEvent(context, projectInfo.Id, release);
      return release;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release PatchRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment> releaseEnvironments,
      bool? keepForever,
      string comment)
    {
      Guid requestorId = context != null ? context.GetUserId(true) : throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.UpdateReleaseRetentionSql", 1971038))
      {
        Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> action = (Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component =>
        {
          ReleaseSqlComponent releaseSqlComponent = component;
          Guid projectId1 = projectId;
          int releaseId1 = releaseId;
          Guid modifiedBy = requestorId;
          IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment> releaseEnvironments1 = releaseEnvironments;
          string comment1 = comment;
          bool? nullable = keepForever;
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus? status = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus?();
          bool? keepForever1 = nullable;
          return releaseSqlComponent.PatchRelease(projectId1, releaseId1, modifiedBy, releaseEnvironments1, comment1, status, keepForever1);
        });
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(action);
        EventsHelper.FireReleaseUpdatedEvent(context, projectId, release);
        return release;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual IEnumerable<int> SoftDeleteReleases(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      DateTime maxReleaseModifiedTimeForSoftDelete,
      IEnumerable<DefinitionEnvironment> definitionEnvironments,
      int defaultNumberOfDaysToRetainRelease,
      string comment)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      IEnumerable<int> source = (IEnumerable<int>) null;
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.SoftDeleteReleasesForRetention", 1971039))
      {
        Guid requestorId = context.GetUserId(true);
        Func<ReleaseSqlComponent, IEnumerable<int>> action = (Func<ReleaseSqlComponent, IEnumerable<int>>) (component => component.SoftDeleteReleases(projectId, definitionId, maxReleaseModifiedTimeForSoftDelete, definitionEnvironments, requestorId, defaultNumberOfDaysToRetainRelease));
        source = context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<int>>(action);
      }
      if (source.Count<int>() > 0)
        ReleasesService.QueueReleaseDeletionJob(context, projectId, definitionId, source.ToArray<int>(), comment);
      return source;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual void HardDeleteReleases(
      IVssRequestContext context,
      Guid projectId,
      ICollection<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releases == null)
        throw new ArgumentNullException(nameof (releases));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.HardDeleteReleases", 1971042))
      {
        IList<ArtifactSpec> artifactSpecList = (IList<ArtifactSpec>) new List<ArtifactSpec>();
        DeploymentsService service1 = context.GetService<DeploymentsService>();
        ReleasesService service2 = context.GetService<ReleasesService>();
        ICollection<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> source = (ICollection<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>();
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases)
        {
          if (!DeploymentsHelper.CancelDeployments(context, service2, service1, projectId, release.ReleaseDefinitionId, release.Id, string.Empty, false, true))
          {
            source.Add(release);
            this.CleanupReleaseFootprint(context, projectId, release);
            artifactSpecList.Add(release.CreateArtifactSpec(projectId));
            context.Trace(1971029, TraceLevel.Info, "ReleaseManagementService", "Service", "Cleaned up footprint for releaseId {0}", (object) release.Id);
          }
        }
        ReleasesService.CleanupProperties(context, (IEnumerable<ArtifactSpec>) artifactSpecList);
        IEnumerable<int> releaseIds = source.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>) (r => r.Id));
        Action<ReleaseSqlComponent> action = (Action<ReleaseSqlComponent>) (component => component.HardDeleteReleases(projectId, releaseIds));
        context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent>(action);
      }
    }

    private static bool HasNullOrEmptyArtifactVersion(ArtifactSource artifact)
    {
      bool flag = true;
      InputValue inputValue;
      if (artifact.SourceData != null && artifact.SourceData.TryGetValue("version", out inputValue) && (inputValue == null || !string.IsNullOrEmpty(inputValue.Value)))
        flag = false;
      return flag;
    }

    private static bool PopulateDefaultArtifactVersionsForDraftRelease(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseProjectInfo releaseProjectInfo)
    {
      bool flag = false;
      if (context == null || release?.LinkedArtifacts == null)
        return false;
      List<ArtifactSource> artifactSourceList = new List<ArtifactSource>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      artifactSourceList.AddRange(release.LinkedArtifacts.Where<ArtifactSource>(ReleasesService.\u003C\u003EO.\u003C4\u003E__HasNullOrEmptyArtifactVersion ?? (ReleasesService.\u003C\u003EO.\u003C4\u003E__HasNullOrEmptyArtifactVersion = new Func<ArtifactSource, bool>(ReleasesService.HasNullOrEmptyArtifactVersion))));
      if (!artifactSourceList.IsNullOrEmpty<ArtifactSource>())
      {
        flag = true;
        Dictionary<string, InputValue> dictionary = new Dictionary<string, InputValue>();
        ProjectInfo projectInfo = new ProjectInfo()
        {
          Id = releaseProjectInfo.Id
        };
        artifactSourceList.GetLatestArtifactVersions(context, projectInfo, dictionary, true);
        if (!dictionary.IsNullOrEmpty<KeyValuePair<string, InputValue>>())
        {
          foreach (KeyValuePair<string, InputValue> keyValuePair in dictionary)
          {
            KeyValuePair<string, InputValue> latestArtifactVersion = keyValuePair;
            ArtifactSource artifactSource = release.LinkedArtifacts.First<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => string.Equals(artifact.Alias, latestArtifactVersion.Key, StringComparison.OrdinalIgnoreCase)));
            artifactSource.SourceData["version"] = latestArtifactVersion.Value;
            string str;
            if (latestArtifactVersion.Value?.Data != null && latestArtifactVersion.Value.Data.TryGetValue<string>("branch", out str))
              artifactSource.SourceBranch = str;
          }
        }
      }
      return flag;
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseNameAndStatusImplementation(
      IVssRequestContext context,
      ReleaseProjectInfo projectInfo,
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseUpdateMetadata releaseUpdateMetadata,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseUpdateMetadata, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releaseStatusUpdater,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, string> releaseNameFiller)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (releaseUpdateMetadata == null)
        throw new ArgumentNullException(nameof (releaseUpdateMetadata));
      if (releaseStatusUpdater == null)
        throw new ArgumentNullException(nameof (releaseStatusUpdater));
      if (releaseNameFiller == null)
        throw new ArgumentNullException(nameof (releaseNameFiller));
      IDataAccessLayer dataAccessLayer = this.getDataAccessLayer(context, projectInfo.Id);
      using (ReleaseManagementTimer.Create(context, "Service", "ReleasesService.UpdateReleaseStatus", 1971008))
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = dataAccessLayer.GetRelease(releaseId);
        bool flag1 = false;
        bool flag2 = !releaseUpdateMetadata.Name.IsNullOrEmpty<char>();
        bool flag3 = false;
        if (release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft && releaseUpdateMetadata.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active)
        {
          flag1 = true;
          if (!flag2)
            releaseNameFiller(release, context, projectInfo.Name);
          IList<ArtifactSource> linkedArtifacts = release.LinkedArtifacts;
          ArtifactUtility.ResolveSourceBranchVariablesForArtifactSources(context, projectInfo.Id, linkedArtifacts, release.ReleaseDefinitionId);
          if (ReleasesService.PopulateDefaultArtifactVersionsForDraftRelease(context, release, projectInfo))
            flag3 = true;
        }
        if (flag2)
        {
          release.Name = releaseUpdateMetadata.Name;
          flag3 = true;
        }
        if (flag3)
          release = dataAccessLayer.UpdateDraftRelease(release, releaseUpdateMetadata.Comment);
        if (releaseUpdateMetadata.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Undefined)
          release = releaseStatusUpdater(release, releaseUpdateMetadata);
        if (flag1)
        {
          OnReleaseCreatedOrUpdatedJobData releaseCreationJobData = new OnReleaseCreatedOrUpdatedJobData()
          {
            ProjectId = projectInfo.Id,
            ReleaseId = releaseId,
            ChangeType = ReleaseHistoryChangeTypes.Update
          };
          ReleasesService.QueueJob(context, releaseCreationJobData, "OnReleaseCreatedOrUpdated", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.OnReleaseCreatedOrUpdatedJob");
        }
        context.SendReleaseUpdatedEvent(projectInfo.Id, release.ReleaseDefinitionId, release.Id);
        return release;
      }
    }

    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingServerRelease)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (webApiRelease == null)
        throw new ArgumentNullException(nameof (webApiRelease));
      if (existingServerRelease == null)
        throw new ArgumentNullException(nameof (existingServerRelease));
      if (webApiRelease.DefinitionSnapshotRevision != 0 && webApiRelease.DefinitionSnapshotRevision != existingServerRelease.DefinitionSnapshotRevision)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseAlreadyUpdated));
      using (ReleaseManagementTimer timer = ReleaseManagementTimer.Create(context, "Service", "Service.UpdateRelease", 1971023))
      {
        IDataAccessLayer dataAccessLayer = this.getDataAccessLayer(context, projectId);
        this.ValidateReleaseUpdate(context, projectId, webApiRelease, existingServerRelease, timer);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease = this.ToServerRelease(context, projectId, webApiRelease, existingServerRelease, timer);
        ReleasesService.FillArtifactVersionData(existingServerRelease, serverRelease);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1;
        try
        {
          release1 = dataAccessLayer.UpdateDraftRelease(serverRelease, webApiRelease.Comment);
        }
        catch (ReleaseDefinitionSnapshotRevisionNotMatchedException ex)
        {
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseAlreadyUpdated));
        }
        dataAccessLayer.SaveProperties(serverRelease.CreateArtifactSpec(projectId), (IEnumerable<PropertyValue>) serverRelease.Properties);
        ReleaseManagementArtifactPropertyKinds.CopyProperties(release1.Properties, serverRelease.Properties);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release2 = release1.DeepClone();
        release2.FillSecrets(serverRelease);
        this.secretsHelper.StoreSecrets(context, projectId, release2);
        ReleasesService.OnUpdateRelease(context, projectId, release1);
        EventsHelper.FireReleaseUpdatedEvent(context, projectId, release1);
        return release1;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual void DeleteRelease(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      string comment)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.DeleteRelease", 1971035))
      {
        Guid requestorId = context.GetUserId(true);
        Action<ReleaseSqlComponent> action = (Action<ReleaseSqlComponent>) (component => component.DeleteRelease(projectId, release.Id, requestorId, comment));
        context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent>(action);
        UpdateRetainBuildData updateRetainBuildData = UpdateRetainBuildData.GetUpdateRetainBuildData(UpdateRetainBuildReason.DeleteRelease, projectId, release.ReleaseDefinitionId, releaseIds: new Collection<int>()
        {
          release.Id
        });
        QueueJobUtility.QueueUpdateRetainBuildJob(context, updateRetainBuildData);
      }
      ReleasesService.QueueReleaseDeletionJob(context, projectId, release.ReleaseDefinitionId, new int[1]
      {
        release.Id
      }, Resources.ReleaseDeleted);
      EventsHelper.FireReleaseUpdatedEvent(context, projectId, release);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual void UndeleteRelease(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      string comment)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.UndeleteRelease", 1971047))
      {
        Guid requestorId = context.GetUserId(true);
        Action<ReleaseSqlComponent> action = (Action<ReleaseSqlComponent>) (component => component.UndeleteRelease(projectId, release.Id, requestorId, comment));
        context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent>(action);
        UpdateRetainBuildData updateRetainBuildData = UpdateRetainBuildData.GetUpdateRetainBuildData(UpdateRetainBuildReason.UndeleteRelease, projectId, release.ReleaseDefinitionId, releaseIds: new Collection<int>()
        {
          release.Id
        });
        QueueJobUtility.QueueUpdateRetainBuildJob(context, updateRetainBuildData);
        EventsHelper.FireReleaseUpdatedEvent(context, projectId, release);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionSummary GetReleaseDefinitionSummary(
      IVssRequestContext context,
      Guid projectId,
      int releaseDefinitionId,
      int releaseCount,
      bool includeArtifact,
      string definitionEnvironmentIdsFilter)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.GetReleaseDefinitionSummary", 1971010))
      {
        IEnumerable<int> intList = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToIntList(definitionEnvironmentIdsFilter);
        List<int> distinctIdsList = intList == null ? new List<int>() : intList.Distinct<int>().ToList<int>();
        Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionSummary> action = (Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionSummary>) (component => component.GetReleaseSummaryForReleaseDefinition(projectId, releaseDefinitionId, releaseCount, includeArtifact, (IList<int>) distinctIdsList));
        return context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionSummary>(action);
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> GetReleasesByEnvironmentStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      ReleaseEnvironmentStatus status)
    {
      return this.ListReleases(requestContext, projectId, releaseDefinitionId, definitionEnvironmentId, string.Empty, string.Empty, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Undefined, status, new DateTime?(), new DateTime?(), new DateTime?(), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdDescending, 1000, 0, false, false, false, false, true, false, string.Empty, string.Empty, string.Empty, string.Empty, false, false, (IEnumerable<string>) null);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public void ResetScheduledReleaseEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      IEnumerable<int> definitionEnvironmentIds)
    {
      Guid requestedBy = requestContext.GetUserId(true);
      Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>> action = (Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>) (component => component.ResetScheduledReleaseEnvironments(projectId, releaseDefinitionId, definitionEnvironmentIds, requestedBy, (ReleaseEnvironmentStatusChangeDetails) new EnvironmentStatusChangeByScheduleDeletion(), (string) null));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release in requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>(action))
        requestContext.SendReleaseUpdatedEvent(projectId, release.ReleaseDefinitionId, release.Id);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> FilterReleasesWithViewPermissions(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases)
    {
      IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(context, projectId, (IList<int>) releases.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, int>) (release => release.ReleaseDefinitionReference.Id)).ToList<int>());
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) ReleaseManagementSecurityProcessor.FilterComponents<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(context, releases, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, ReleaseManagementSecurityInfo>) (release => ReleaseManagementSecurityProcessor.GetSecurityInfo(projectId, folderPaths[release.ReleaseDefinitionReference.Id], release.ReleaseDefinitionReference.Id, ReleaseManagementSecurityPermissions.ViewReleases)), false).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
    }

    public virtual void InitiateEnvironmentExecution(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      context.TraceEnter(1960082, "ReleaseManagementService", "Pipeline", nameof (InitiateEnvironmentExecution));
      new OrchestratorServiceProcessorV2(context, projectId).StartReleaseOnEnvironment(releaseId, releaseEnvironmentId);
      context.TraceEnter(1960083, "ReleaseManagementService", "Pipeline", nameof (InitiateEnvironmentExecution));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase> GetDeployPhasesForReleaseEnvironment(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      int attempt)
    {
      Func<ReleaseSqlComponent, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>> action = (Func<ReleaseSqlComponent, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>>) (component => component.GetDeployPhasesForEnvironment(projectId, releaseId, releaseEnvironmentId, releaseDeployPhaseId, attempt));
      return context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<string> GetSourceBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (releaseDefinitionId <= 0)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ReleaseDefinitionWithGivenIdNotFound);
      Func<ReleaseSqlComponent, IEnumerable<string>> action = (Func<ReleaseSqlComponent, IEnumerable<string>>) (component => component.GetSourceBranches(projectId, releaseDefinitionId));
      return requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<string>>(action);
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> QueryActiveReleases(
      IVssRequestContext context,
      Guid projectId,
      int releaseDefinitionId,
      int maxReleasesCount,
      int continuationToken,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      bool includeVariables,
      bool includeTags)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      includeEnvironments = includeApprovals | includeManualInterventions | includeEnvironments;
      using (ReleaseManagementTimer.Create(context, "Service", "ReleasesService.QueryActiveReleases", 1971071))
      {
        Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>> action = (Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>) (component => component.QueryActiveReleases(projectId, releaseDefinitionId, maxReleasesCount, continuationToken, includeEnvironments, includeArtifacts, includeApprovals, includeManualInterventions, includeVariables, includeTags));
        return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>(action).OrderByDescending<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>) (r => r.Id));
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public ActiveDefinitionsData GetLandingPageDataWithPendingApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid userId,
      IList<int> favoriteDefinitions,
      int currentReleaseDefinitionId,
      int maxApprovalCount,
      int maxDeploymentCount,
      int activeDefinitionsCount,
      int recentDefinitionsCount,
      DateTime minModifiedTimePendingApproval,
      DateTime maxModifiedTimePendingApproval,
      DateTime minModifiedTimeCompletedApproval,
      DateTime maxModifiedTimeCompletedApproval,
      DateTime minDeploymentQueueTime,
      DateTime maxDeploymentQueueTime)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      Func<ReleaseDataProviderSqlComponent, ActiveDefinitionsData> action = (Func<ReleaseDataProviderSqlComponent, ActiveDefinitionsData>) (component => component.GetLandingPageDataWithPendingApprovals(projectId, userId, favoriteDefinitions ?? (IList<int>) new List<int>(), currentReleaseDefinitionId, maxApprovalCount, maxDeploymentCount, activeDefinitionsCount, recentDefinitionsCount, minModifiedTimePendingApproval, maxModifiedTimePendingApproval, minModifiedTimeCompletedApproval, maxModifiedTimeCompletedApproval, minDeploymentQueueTime, maxDeploymentQueueTime));
      return requestContext.ExecuteWithinUsingWithComponent<ReleaseDataProviderSqlComponent, ActiveDefinitionsData>(action);
    }

    protected virtual void ValidateReleaseUpdate(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingServerRelease,
      ReleaseManagementTimer timer)
    {
      if (timer == null)
        throw new ArgumentNullException(nameof (timer));
      ReleaseValidations.ValidateRelease(webApiRelease, existingServerRelease, context);
      string folderPath = ReleaseManagementSecurityProcessor.GetFolderPath(context, projectId, existingServerRelease.ReleaseDefinitionId);
      ReleaseEnvironmentSecurityValidations securityValidations = new ReleaseEnvironmentSecurityValidations(context, projectId, folderPath, webApiRelease, existingServerRelease);
      securityValidations.CheckSecurityPermission();
      securityValidations.CheckUsePermissionOnQueuesAddedOrChanged();
      securityValidations.CheckUsePermissionOnMachineGroupsAddedOrChanged();
      timer.RecordLap("Service", "SecurityValidation", 1971030);
    }

    protected virtual Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release ToServerRelease(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingServerRelease,
      ReleaseManagementTimer timer)
    {
      if (timer == null)
        throw new ArgumentNullException(nameof (timer));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease = webApiRelease.FromContract(context);
      timer.RecordLap("Service", "ConvertToModel", 1971020);
      serverRelease.DefinitionSnapshot = ReleaseDefinitionEnvironmentsSnapshotMerger.GetUpdatedReleaseDefinitionSnapshot(existingServerRelease, webApiRelease);
      serverRelease.Description = serverRelease.Description ?? existingServerRelease.Description;
      return serverRelease;
    }

    private static void OnCreateRelease(
      IVssRequestContext context,
      ReleaseProjectInfo releaseProjectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      OnReleaseCreatedOrUpdatedJobData releaseCreationJobData = new OnReleaseCreatedOrUpdatedJobData()
      {
        ProjectId = releaseProjectInfo.Id,
        ReleaseId = serverRelease.Id,
        ChangeType = ReleaseHistoryChangeTypes.Create
      };
      releaseCreationJobData.DefinitionSnapshotRevision = serverRelease.DefinitionSnapshotRevision;
      releaseCreationJobData.DefinitionSnapshot = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) serverRelease.ToDefinitionSnapshotContract(context, releaseProjectInfo.Id));
      ReleasesService.QueueJob(context, releaseCreationJobData, "OnReleaseCreatedOrUpdated", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.OnReleaseCreatedOrUpdatedJob");
      context.SendReleaseCreatedEvent(releaseProjectInfo.Id, serverRelease.ReleaseDefinitionId, serverRelease.Id);
    }

    private static void OnUpdateRelease(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release updatedReleaseWithSecretsCleared)
    {
      string str = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) updatedReleaseWithSecretsCleared.ToDefinitionSnapshotContract(context, projectId));
      OnReleaseCreatedOrUpdatedJobData releaseCreationJobData = new OnReleaseCreatedOrUpdatedJobData()
      {
        ProjectId = projectId,
        ReleaseId = updatedReleaseWithSecretsCleared.Id,
        DefinitionSnapshotRevision = updatedReleaseWithSecretsCleared.DefinitionSnapshotRevision,
        DefinitionSnapshot = str,
        ChangeType = ReleaseHistoryChangeTypes.Update
      };
      ReleasesService.QueueJob(context, releaseCreationJobData, "OnReleaseCreatedOrUpdated", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.OnReleaseCreatedOrUpdatedJob");
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is already logged for diagnosability")]
    private static void QueueJob(
      IVssRequestContext requestContext,
      OnReleaseCreatedOrUpdatedJobData releaseCreationJobData,
      string jobName,
      string jobExtensionName)
    {
      try
      {
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) releaseCreationJobData);
        requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, jobName, jobExtensionName, xml, JobPriorityLevel.Normal);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1900033, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to queue {0} for Project Id {1} ReleaseId {2} DefintionSnapshotRevision{3}. Exception {4}", (object) jobName, (object) releaseCreationJobData.ProjectId, (object) releaseCreationJobData.ReleaseId, (object) releaseCreationJobData.DefinitionSnapshotRevision, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void QueueReleaseDeletionJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int[] releaseIds,
      string comment)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) ReleaseDeletionJobData.CreateJobData(projectId, definitionId, releaseIds, comment));
      ArgumentUtility.CheckForNull<XmlNode>(xml, "jobData");
      Guid guid = requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, "OnReleaseDeleted", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.ReleaseDeletionJob", xml, JobPriorityLevel.BelowNormal);
      requestContext.Trace(1976395, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "QueueReleaseDeletionJob: jobId: {0}, jobData: {1}", (object) guid, (object) xml.OuterXml);
    }

    private static void ValidateRequestData(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata environmentUpdateData,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      if (environmentUpdateData == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ProvideReleaseEnvironmentUpdateData);
      if (environmentUpdateData.ScheduledDeploymentTime.HasValue)
      {
        if (environmentUpdateData.ScheduledDeploymentTime.Value.Kind != DateTimeKind.Utc)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ScheduledTimeNotInUtc);
        if (environmentUpdateData.ScheduledDeploymentTime.Value < DateTime.UtcNow)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ScheduledTimeNotInFuture);
      }
      if (environmentUpdateData.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>())
        return;
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> deployTimeVariables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>(environmentUpdateData.Variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      deployTimeVariables.Remove("release.redeployment.deploymentGroupTargetFilter");
      DeployTimeVariableValidations.ValidateReleaseEnvironmentDeployTimeVariables(environment.Variables, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) deployTimeVariables, environment.Name);
    }

    private static IDataAccessLayer GetDataAccessLayer(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return (IDataAccessLayer) new DataAccessLayer(requestContext, projectId);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void TraceInformationMessage(
      IVssRequestContext requestContext,
      int tracePoint,
      string format,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, tracePoint, TraceLevel.Info, "ReleaseManagementService", "Service", format, args);
    }

    private static void EnsurePrimaryArtifactIsSet(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      using (new MethodScope(requestContext, nameof (ReleasesService), nameof (EnsurePrimaryArtifactIsSet)))
      {
        int artifactSourceId;
        if (!release.LinkedArtifacts.Any<ArtifactSource>() || release.TryGetPrimaryArtifactSource(out artifactSourceId))
          return;
        artifactSourceId = release.LinkedArtifacts.Min<ArtifactSource>((Func<ArtifactSource, int>) (a => a.Id));
        release.GetArtifact(artifactSourceId).IsPrimary = true;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to continue on error")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Logs dont need to be localized")]
    private static void CleanupRunPlans(IVssRequestContext context, Guid projectId, int releaseId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      IEnumerable<Guid> releaseRunPlanIds = ReleasesService.GetReleaseRunPlanIds(context, projectId, releaseId);
      if (releaseRunPlanIds == null || releaseRunPlanIds.ToList<Guid>().Count == 0)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No run plan id found for release id : {0}", (object) releaseId);
        context.Trace(1973137, TraceLevel.Info, "ReleaseManagementService", "Service", message);
      }
      else
      {
        TaskHub taskHub = context.GetService<DistributedTaskHubService>().GetTaskHub(context, "Release");
        ITeamFoundationFileContainerService service = context.GetService<ITeamFoundationFileContainerService>();
        foreach (Guid planId in releaseRunPlanIds)
        {
          TaskOrchestrationPlan plan = taskHub.GetPlan(context, projectId, planId);
          if (plan == null)
          {
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Plan not found in Distributed Task Hub Service for Plan Id : {0} , Project Id : {1}", (object) planId, (object) projectId);
            context.Trace(1973137, TraceLevel.Error, "ReleaseManagementService", "Service", message);
          }
          else
          {
            ITeamFoundationFileContainerService containerService = service;
            IVssRequestContext requestContext = context;
            List<Uri> artifactUris = new List<Uri>();
            artifactUris.Add(plan.ArtifactUri);
            Guid scopeIdentifier = projectId;
            foreach (Microsoft.VisualStudio.Services.FileContainer.FileContainer queryContainer in containerService.QueryContainers(requestContext, (IList<Uri>) artifactUris, scopeIdentifier))
            {
              try
              {
                service.DeleteContainer(context, queryContainer.Id, projectId);
              }
              catch (FileContainerException ex)
              {
                context.TraceException(1973135, TraceLevel.Error, "ReleaseManagementService", "Events", (Exception) ex);
              }
            }
          }
        }
        try
        {
          taskHub.DeletePlans(context.Elevate(), projectId, releaseRunPlanIds);
        }
        catch (Exception ex)
        {
          context.TraceException(1973136, TraceLevel.Error, "ReleaseManagementService", "Events", ex);
        }
        context.GetService<ReleaseHistoryService>().DeleteDefinitionSnapshots(context, projectId, releaseId);
      }
    }

    private static IEnumerable<Guid> GetReleaseRunPlanIds(
      IVssRequestContext context,
      Guid projectId,
      int releaseId)
    {
      Func<ReleaseSqlComponent, ReleaseLogContainers> action = (Func<ReleaseSqlComponent, ReleaseLogContainers>) (component => component.GetReleaseLogContainers(projectId, releaseId, true));
      ReleaseLogContainers releaseLogContainers = context.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, ReleaseLogContainers>(action);
      return releaseLogContainers.DeployPhases.Select<ReleaseDeployPhaseRef, Guid>((Func<ReleaseDeployPhaseRef, Guid>) (e => e.PlanId)).Where<Guid>((Func<Guid, bool>) (planId => planId != Guid.Empty)).Union<Guid>(releaseLogContainers.Gates.Where<DeploymentGateRef>((Func<DeploymentGateRef, bool>) (g => g.RunPlanId.HasValue)).Select<DeploymentGateRef, Guid>((Func<DeploymentGateRef, Guid>) (g => g.RunPlanId.Value))).Union<Guid>(releaseLogContainers.DeploySteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.RunPlanId.HasValue)).Select<ReleaseEnvironmentStep, Guid>((Func<ReleaseEnvironmentStep, Guid>) (s => s.RunPlanId.Value)));
    }

    private static void CleanupProperties(
      IVssRequestContext context,
      IEnumerable<ArtifactSpec> artifactSpecs)
    {
      context.GetService<ITeamFoundationPropertyService>().DeleteArtifacts(context, artifactSpecs, PropertiesOptions.AllVersions);
    }

    private static void FillArtifactVersionData(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingRelease, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release updatedRelease)
    {
      if (existingRelease.LinkedArtifacts.IsNullOrEmpty<ArtifactSource>() || updatedRelease.LinkedArtifacts.IsNullOrEmpty<ArtifactSource>())
        return;
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) existingRelease.LinkedArtifacts)
      {
        ArtifactSource existingArtifact = linkedArtifact;
        ArtifactSource artifactSource = updatedRelease.LinkedArtifacts.SingleOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (a => a.Alias == existingArtifact.Alias));
        if (artifactSource != null && existingArtifact.SourceData.ContainsKey("version") && artifactSource.SourceData.ContainsKey("version"))
        {
          InputValue inputValue1 = artifactSource.SourceData["version"];
          InputValue inputValue2 = existingArtifact.SourceData["version"];
          if (inputValue1.Value == inputValue2.Value && inputValue2.Data != null)
            inputValue1.Data = inputValue2.Data;
        }
      }
    }

    private static void SavePullRequestRelease(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.UseQueryDeploymentsToCancelPullRequestRelease"))
        return;
      try
      {
        PullRequestRelease requestReleaseObject = release.GetPullRequestReleaseObject(requestContext, projectId);
        if (requestReleaseObject == null)
          return;
        requestContext.GetService<PullRequestReleaseService>().CreatePullRequestRelease(requestContext, requestReleaseObject);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(1976454, TraceLevel.Error, "ReleaseManagementService", "Events", "Exception occurred while creating PullRequestRelease object for ReleaseId: {0}. Error: {1}", (object) release.Id, (object) TeamFoundationExceptionFormatter.FormatException(ex, false));
      }
    }

    private void CleanupReleaseFootprint(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      ReleasesService.CleanupRunPlans(context, projectId, release.Id);
      this.secretsHelper.DeleteSecrets(context, projectId, release);
      context.GetService<ReleaseHistoryService>().DeleteDefinitionSnapshots(context, projectId, release.Id);
    }

    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateEnvironmentIfRequired(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      DateTime? scheduledDateTime,
      ReleaseEnvironmentStatus inputEnvironmentStatus)
    {
      if (releaseEnvironment.IsWaitingOnPreDeployApproval())
        return this.getDataAccessLayer(context, projectId).UpdateReleaseEnvironment(release.Id, releaseEnvironment.Id, scheduledDateTime);
      if (inputEnvironmentStatus == ReleaseEnvironmentStatus.Undefined)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UpdateOfEnvironmentNotAllowed, (object) releaseEnvironment.Name, (object) release.Name));
      if (scheduledDateTime.HasValue)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.UpdateOfScheduledTimeNotAllowed);
      return release;
    }
  }
}
