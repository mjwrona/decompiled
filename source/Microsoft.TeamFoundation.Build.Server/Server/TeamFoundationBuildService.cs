// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TeamFoundationBuildService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.Compatibility;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  public sealed class TeamFoundationBuildService : ITeamFoundationBuildService, IVssFrameworkService
  {
    private const string c_UpdateDefinitionsLockResource = "tfs://Build/Definition/Update";
    private const string c_EnsureBuildGroupLock = "tfs://Build/Groups";
    private const int c_DeleteBuildsBatchSizeDefault = 25;
    internal static readonly string RegistrySettingsPath = "/Service/Build/Settings/";
    private IBuild2Converter m_build2Converter;
    private TeamFoundationBuildHost m_buildHost;
    private BuildDefinitionCache m_buildDefinitionCache;
    private XmlSerializer m_stringArraySerializer = new XmlSerializer(typeof (string[]));
    private IProjectService m_projectService;
    private ArtifactKind m_definitionArtifactKind;

    internal TeamFoundationBuildService()
    {
    }

    public List<BuildDefinition> AddBuildDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinition> definitions)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (AddBuildDefinitions));
      TeamFoundationBuildService.CheckXamlEnabled(requestContext);
      Validation.CheckValidatable<BuildDefinition>(requestContext, nameof (definitions), definitions, false, ValidationContext.Add);
      HashSet<string> serverDropControllerUris = new HashSet<string>();
      HashSet<string> gitCustomTemplatecontrollerUris = new HashSet<string>();
      foreach (BuildDefinition definition in (IEnumerable<BuildDefinition>) definitions)
      {
        if (BuildContainerPath.IsServerPath(definition.DefaultDropLocation))
          serverDropControllerUris.Add(definition.BuildControllerUri);
        if (definition.Process != null && BuildSourceProviders.GitProperties.IsGitUri(definition.Process.ServerPath))
          gitCustomTemplatecontrollerUris.Add(definition.BuildControllerUri);
        if (TeamFoundationBuildService.IsGitTeamProject(requestContext, definition.TeamProject.Uri))
        {
          foreach (BuildDefinitionSourceProvider sourceProvider in definition.SourceProviders)
            this.UpdateGitRepositoryUrl(requestContext, sourceProvider);
        }
        this.m_buildHost.SecurityManager.CheckProjectPermission(requestContext, definition.TeamProject, BuildPermissions.EditBuildDefinition);
      }
      definitions.ConvertObjectsToProjectGuid(requestContext);
      TeamFoundationBuildService.CheckDefinitionAndControllerCompatibility(requestContext, definitions, serverDropControllerUris, gitCustomTemplatecontrollerUris);
      TeamFoundationEventService service1 = requestContext.GetService<TeamFoundationEventService>();
      foreach (BuildDefinition definition in (IEnumerable<BuildDefinition>) definitions)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Publishing decision point for definition '{0}'", (object) definition.Uri);
        service1.PublishDecisionPoint(requestContext, (object) new BuildDefinitionChangingEvent(ChangedType.Added, (BuildDefinition) null, definition));
      }
      TeamFoundationIdentity requestedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      TeamFoundationBuildService.BuildDefinitionDatabaseResult dbResults;
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        dbResults = TeamFoundationBuildService.ReadDatabaseResults(component.AddBuildDefinitions((ICollection<BuildDefinition>) definitions, requestedBy, this.WriterId, requestContext.IsSystemContext));
      this.PostProcessDatabaseResults(requestContext, dbResults);
      TeamFoundationJobService service2 = requestContext.Elevate().GetService<TeamFoundationJobService>();
      List<string> uris = new List<string>();
      IEnumerator<BuildDefinition> enumerator = definitions.GetEnumerator();
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
      foreach (BuildDefinition definition in dbResults.Definitions)
      {
        uris.Add(definition.Uri);
        if (!enumerator.MoveNext())
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Result out of sync. Updated definition '{0}'", (object) definition.Uri);
        definition.CopyPropertiesFrom(enumerator.Current);
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Publishing notification for definition '{0}'", (object) definition.Uri);
        service1.PublishNotification(requestContext, (object) new BuildDefinitionChangedEvent(requestContext, definition, ChangedType.Added));
        TeamFoundationJobDefinition scheduleJobDefinition = definition.GetScheduleJobDefinition(service2.IsIgnoreDormancyPermitted);
        if (scheduleJobDefinition != null)
        {
          jobUpdates.Add(scheduleJobDefinition);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added job '{0}' for updating", (object) scheduleJobDefinition.JobId);
        }
      }
      if (jobUpdates.Count > 0)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Updating job definitions");
        service2.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      }
      this.m_buildDefinitionCache.Invalidate(requestContext.Elevate(), (IEnumerable<string>) uris);
      requestContext.TraceLeave(0, "Build", "Service", nameof (AddBuildDefinitions));
      return dbResults.Definitions;
    }

    public void AddBuildQualities(
      IVssRequestContext requestContext,
      string teamProject,
      IList<string> qualities)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (AddBuildQualities));
      ArgumentValidation.Check(nameof (teamProject), (object) teamProject, false);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>(nameof (qualities), qualities, TeamFoundationBuildService.\u003C\u003EO.\u003C0\u003E__CheckQuality ?? (TeamFoundationBuildService.\u003C\u003EO.\u003C0\u003E__CheckQuality = new Validate<string>(Validation.CheckQuality)), false, (string) null);
      TeamProject projectFromGuidOrName = this.m_projectService.GetTeamProjectFromGuidOrName(requestContext, teamProject);
      this.m_buildHost.SecurityManager.CheckProjectPermission(requestContext, projectFromGuidOrName, BuildPermissions.ManageBuildQualities);
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        component.AddBuildQualities(projectFromGuidOrName, qualities);
      requestContext.TraceLeave(0, "Build", "Service", nameof (AddBuildQualities));
    }

    public List<ProcessTemplate> AddProcessTemplates(
      IVssRequestContext requestContext,
      IList<ProcessTemplate> processTemplates)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (AddProcessTemplates));
      Validation.CheckValidatable<ProcessTemplate>(requestContext, nameof (processTemplates), processTemplates, false, ValidationContext.Add);
      foreach (ProcessTemplate processTemplate in (IEnumerable<ProcessTemplate>) processTemplates)
      {
        this.m_buildHost.SecurityManager.CheckProjectPermission(requestContext, processTemplate.TeamProjectObj, BuildPermissions.EditBuildDefinition);
        processTemplate.UpdateCachedProcessParameters(requestContext, (VersionSpec) new LatestVersionSpec());
        if (!processTemplate.FileExists)
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Process template file '{0}' not found", (object) processTemplate.ServerPath);
          throw new BuildServerException(ResourceStrings.VersionControlFileNotFoundException((object) processTemplate.ServerPath));
        }
      }
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
      {
        ResultCollection resultCollection = component.AddProcessTemplates((ICollection<ProcessTemplate>) processTemplates);
        requestContext.TraceLeave(0, "Build", "Service", nameof (AddProcessTemplates));
        return resultCollection.GetCurrent<ProcessTemplate>().Items;
      }
    }

    public void CancelBuilds(IVssRequestContext requestContext, int[] queueIds, Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (CancelBuilds));
      if (queueIds.Length == 1 && this.m_build2Converter != null && this.m_build2Converter.IsBuild2Id(requestContext, projectId, queueIds[0]) && this.m_build2Converter.CancelBuild(requestContext, queueIds[0]) != null)
        return;
      TeamFoundationIdentity requestor = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      List<QueuedBuild> queueIds1 = new List<QueuedBuild>();
      using (TeamFoundationDataReader foundationDataReader = this.QueryQueuedBuildsById(requestContext.Elevate(), (IList<int>) queueIds, (IList<string>) Array.Empty<string>(), QueryOptions.Definitions, projectId))
      {
        BuildQueueQueryResult queueQueryResult = foundationDataReader.Current<BuildQueueQueryResult>();
        int index = 0;
        Dictionary<string, BuildDefinition> dictionary = queueQueryResult.Definitions.ToDictionary<BuildDefinition, string>((Func<BuildDefinition, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (QueuedBuild queuedBuild in queueQueryResult.QueuedBuilds)
        {
          if (queuedBuild == null)
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Queued build '{0}' not found", (object) queueIds[index]);
            throw new QueuedBuildDoesNotExistException(queueIds[index]);
          }
          if (queuedBuild.IsRequestor(requestor))
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Skipped checking permission for queued build '{0}'", (object) queuedBuild.Id);
            ++index;
            queueIds1.Add(queuedBuild);
          }
          else
          {
            BuildDefinition buildDefinition;
            if (dictionary.TryGetValue(queuedBuild.BuildDefinitionUri, out buildDefinition))
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking ManageBuildQueue on definition '{0}'", (object) buildDefinition.Uri);
              this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, buildDefinition, BuildPermissions.ManageBuildQueue);
            }
            else
              requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Definition '{0}' not found", (object) queuedBuild.BuildDefinitionUri);
            ++index;
            queueIds1.Add(queuedBuild);
          }
        }
      }
      List<StartBuildData> items;
      using (BuildQueueComponent component = requestContext.CreateComponent<BuildQueueComponent>("Build"))
        items = component.CancelBuilds((ICollection<QueuedBuild>) queueIds1).GetCurrent<StartBuildData>().Items;
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) items);
      requestContext.TraceLeave(0, "Build", "Service", nameof (CancelBuilds));
    }

    public void CreateTeamProject(
      IVssRequestContext requestContext,
      string projectUri,
      IList<BuildTeamProjectPermission> permissions)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (CreateTeamProject));
      ArgumentValidation.Check(nameof (projectUri), projectUri, false, (string) null);
      ArgumentValidation.CheckArray<BuildTeamProjectPermission>(nameof (permissions), permissions, (Validate<BuildTeamProjectPermission>) ((argumentName, obj, allowNull, errorMessage) => Validation.CheckValidatable(requestContext, argumentName, (IValidatable) obj, allowNull, ValidationContext.Add)), false, (string) null);
      Guid guid = Guid.Parse(LinkingUtilities.DecodeUri(projectUri).ToolSpecificId);
      requestContext.GetService<IDataspaceService>().CreateDataspace(requestContext, "Build", guid, DataspaceState.Active);
      TeamFoundationIdentityService service1 = requestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationSecurityService service2 = requestContext.GetService<TeamFoundationSecurityService>();
      if (!service2.GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId).HasPermission(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceToken, AuthorizationNamespacePermissions.CreateProjects))
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Team project creation access denied");
        throw new AccessDeniedException(requestContext.GetDisplayName(), new string[1]
        {
          "CREATE_PROJECTS"
        });
      }
      List<AccessControlEntry> accessControlEntryList1 = new List<AccessControlEntry>();
      List<AccessControlEntry> accessControlEntryList2 = new List<AccessControlEntry>();
      foreach (BuildTeamProjectPermission permission in (IEnumerable<BuildTeamProjectPermission>) permissions)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Normalizing permissions for '{0}'", (object) permission.IdentityName);
        TeamFoundationIdentity[][] identitiesToSearch1 = service1.ReadIdentities(requestContext, IdentitySearchFactor.AccountName, new string[1]
        {
          permission.IdentityName
        }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
        TeamFoundationIdentity identityForPermission = TeamFoundationBuildService.FindIdentityForPermission(requestContext, identitiesToSearch1, permission, projectUri);
        if (identityForPermission == null)
        {
          TeamFoundationIdentity[][] identitiesToSearch2 = service1.ReadIdentities(requestContext, IdentitySearchFactor.AccountName, new string[1]
          {
            permission.IdentityName
          }, MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource, (IEnumerable<string>) null);
          identityForPermission = TeamFoundationBuildService.FindIdentityForPermission(requestContext, identitiesToSearch2, permission, projectUri);
        }
        if (identityForPermission != null)
        {
          permission.Descriptor = identityForPermission.Descriptor;
          permission.ConvertToPermissionBits();
          accessControlEntryList1.Add(permission.BuildACE);
          accessControlEntryList2.Add(permission.BuildAdministrationACE);
        }
        else
          requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Could not find identity '{0}' - skipping permission set", (object) permission.IdentityName);
      }
      service2.GetSecurityNamespace(requestContext, BuildSecurity.AdministrationNamespaceId).SetAccessControlEntries(requestContext, BuildSecurity.PrivilegesToken, (IEnumerable<IAccessControlEntry>) accessControlEntryList2, true);
      service2.GetSecurityNamespace(requestContext, BuildSecurity.BuildNamespaceId).SetAccessControlEntries(requestContext, guid.ToString("D"), (IEnumerable<IAccessControlEntry>) accessControlEntryList1, true);
      this.CreateBuiltInProcessTemplates(requestContext, projectUri);
      using (IDisposableReadOnlyList<IBuildProjectInstaller> extensions = requestContext.GetExtensions<IBuildProjectInstaller>())
      {
        foreach (IBuildProjectInstaller projectInstaller in (IEnumerable<IBuildProjectInstaller>) extensions)
        {
          string fullName = projectInstaller.GetType().FullName;
          try
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Running project installer {0}", (object) fullName);
            projectInstaller.Install(requestContext, guid);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Succesfully ran project installer {0}", (object) fullName);
          }
          catch (Exception ex)
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Encountered exception while running installer {0}", (object) fullName);
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (CreateTeamProject));
    }

    private static TeamFoundationIdentity FindIdentityForPermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity[][] identitiesToSearch,
      BuildTeamProjectPermission permission,
      string projectUri)
    {
      TeamFoundationIdentity identityForPermission = (TeamFoundationIdentity) null;
      if (identitiesToSearch[0].Length == 0)
        return (TeamFoundationIdentity) null;
      if (identitiesToSearch[0].Length == 1)
      {
        identityForPermission = identitiesToSearch[0][0];
      }
      else
      {
        string str = (string) null;
        if (requestContext.ServiceHost.CollectionServiceHost != null)
          str = TFCommonUtil.GetIdentityDomainScope(requestContext.ServiceHost.CollectionServiceHost.InstanceId);
        foreach (TeamFoundationIdentity foundationIdentity in identitiesToSearch[0])
        {
          string attribute = foundationIdentity.GetAttribute("Domain", (string) null);
          if (attribute.Equals(projectUri, StringComparison.OrdinalIgnoreCase))
          {
            identityForPermission = foundationIdentity;
            break;
          }
          if (str != null && attribute.Equals(str, StringComparison.OrdinalIgnoreCase))
            identityForPermission = foundationIdentity;
        }
      }
      return identityForPermission;
    }

    public void DeleteBuildDefinitions(IVssRequestContext requestContext, IList<string> uris)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DeleteBuildDefinitions));
      ArgumentValidation.CheckUriArray(nameof (uris), uris, "Definition", false, (string) null);
      BuildDefinitionQueryResult definitionQueryResult = this.QueryBuildDefinitionsByUri(requestContext, uris, (IList<string>) BuildConstants.AllPropertyNames, QueryOptions.None, new Guid());
      List<BuildDefinition> buildDefinitionList = new List<BuildDefinition>();
      List<Guid> jobsToDelete = new List<Guid>();
      TeamFoundationEventService eventManager = requestContext.GetService<TeamFoundationEventService>();
      List<BuildDefinitionChangedEvent> definitionChangedEventList = new List<BuildDefinitionChangedEvent>();
      int index = 0;
      foreach (BuildDefinition definition in definitionQueryResult.Definitions)
      {
        if (definition == null)
        {
          requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Definition '{0}' not found", (object) uris[index]);
        }
        else
        {
          this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, definition, BuildPermissions.DeleteBuildDefinition);
          definitionChangedEventList.Add(new BuildDefinitionChangedEvent(requestContext, definition, ChangedType.Deleted));
          buildDefinitionList.Add(definition);
          jobsToDelete.Add(definition.ScheduleJobId);
          ++index;
        }
      }
      if (buildDefinitionList.Count > 0)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Deleting definitions, publishing notifications and updating job definitions");
        using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
          component.DeleteBuildDefinitions((IEnumerable<BuildDefinition>) buildDefinitionList, this.WriterId, this.m_definitionArtifactKind);
        definitionChangedEventList.ForEach((Action<BuildDefinitionChangedEvent>) (x => eventManager.PublishNotification(requestContext, (object) x)));
        this.m_buildDefinitionCache.Invalidate(requestContext, buildDefinitionList.Select<BuildDefinition, string>((Func<BuildDefinition, string>) (x => x.Uri)));
        requestContext.Elevate().GetService<TeamFoundationJobService>().UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) null);
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (DeleteBuildDefinitions));
    }

    public void DeleteBuildQualities(
      IVssRequestContext requestContext,
      string teamProject,
      IList<string> qualities)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DeleteBuildQualities));
      ArgumentValidation.Check(nameof (teamProject), (object) teamProject, false);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>(nameof (qualities), qualities, TeamFoundationBuildService.\u003C\u003EO.\u003C0\u003E__CheckQuality ?? (TeamFoundationBuildService.\u003C\u003EO.\u003C0\u003E__CheckQuality = new Validate<string>(Validation.CheckQuality)), false, (string) null);
      TeamProject projectFromGuidOrName = this.m_projectService.GetTeamProjectFromGuidOrName(requestContext, teamProject);
      this.m_buildHost.SecurityManager.CheckProjectPermission(requestContext, projectFromGuidOrName, BuildPermissions.ManageBuildQualities);
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        component.DeleteBuildQualities(projectFromGuidOrName, qualities);
      requestContext.TraceLeave(0, "Build", "Service", nameof (DeleteBuildQualities));
    }

    public List<BuildDeletionResult> DeleteBuilds(
      IVssRequestContext requestContext,
      IList<string> uris,
      DeleteOptions options,
      bool throwIfInvalidUri,
      Guid projectId = default (Guid),
      bool deleteKeepForever = false)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DeleteBuilds));
      ArgumentValidation.CheckUriArray(nameof (uris), uris, "Build", false, (string) null);
      if (uris.Count == 1 && this.m_build2Converter != null)
      {
        BuildDetail buildDetail = this.m_build2Converter.DeleteBuild(requestContext, uris[0], options.HasFlag((Enum) DeleteOptions.Details));
        if (buildDetail != null)
          return new List<BuildDeletionResult>()
          {
            new BuildDeletionResult() { Build = buildDetail }
          };
      }
      List<BuildDetail> source = new List<BuildDetail>();
      List<BuildDeletionResult> deleteResults = new List<BuildDeletionResult>();
      Dictionary<string, BuildController> controllers = new Dictionary<string, BuildController>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, BuildServiceHost> serviceHosts = new Dictionary<string, BuildServiceHost>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (TeamFoundationDataReader foundationDataReader = this.QueryBuildsByUri(requestContext, uris, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers, QueryDeletedOption.IncludeDeleted, new Guid(), false))
      {
        int index = 0;
        foreach (BuildDetail build in foundationDataReader.Current<BuildQueryResult>().Builds)
        {
          if (throwIfInvalidUri && (build == null || build.IsDeleted || !this.m_projectService.GetTeamProjectFromGuidOrName(requestContext, build.TeamProject).MatchesScope(projectId)))
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Build '{0}' not found", (object) uris[index]);
            throw new InvalidBuildUriException(uris[index]);
          }
          if (build == null || this.m_projectService.GetTeamProjectFromGuidOrName(requestContext, build.TeamProject).MatchesScope(projectId))
          {
            if (build != null)
            {
              this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, build.Definition, BuildPermissions.DeleteBuilds);
              if (BuildCommonUtil.IsDefaultDateTime(build.FinishTime))
              {
                requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Cannot delete in progress build '{0}'", (object) build.Uri);
                throw new CannotDeleteInProgressBuildException(build.BuildNumber);
              }
              if (BuildContainerPath.IsServerPath(build.DropLocationRoot) && (options & (DeleteOptions.DropLocation | DeleteOptions.Details)) == DeleteOptions.Details)
                throw new ArgumentException(ResourceStrings.ServerDropMustDeleteDrop());
              if (build.KeepForever && !deleteKeepForever)
                throw new CannotDeleteRetainedBuildException(build.BuildNumber);
            }
            ++index;
            if (build != null)
              source.Add(build);
            deleteResults.Add(new BuildDeletionResult()
            {
              Build = build
            });
          }
        }
        foundationDataReader.Current<BuildQueryResult>().Controllers.ForEach((Action<BuildController>) (x => controllers[x.Uri] = x));
        foundationDataReader.Current<BuildQueryResult>().ServiceHosts.ForEach((Action<BuildServiceHost>) (x => serviceHosts[x.Uri] = x));
      }
      int size = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/DeleteBuildsBatchSize", 25);
      foreach (IList<BuildDetail> builds in source.Buffer<BuildDetail>(size, true))
        this.DeleteBuildsCore(requestContext, builds, options, deleteResults, controllers);
      requestContext.TraceLeave(0, "Build", "Service", nameof (DeleteBuilds));
      return deleteResults;
    }

    private void DeleteBuildsCore(
      IVssRequestContext requestContext,
      IList<BuildDetail> builds,
      DeleteOptions options,
      List<BuildDeletionResult> deleteResults,
      Dictionary<string, BuildController> controllers)
    {
      TeamFoundationIdentity deletedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      Dictionary<string, DeleteOptions> dictionary1 = new Dictionary<string, DeleteOptions>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, List<SymbolStoreData>> dictionary2 = new Dictionary<string, List<SymbolStoreData>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
      {
        ResultCollection resultCollection = component.DeleteBuilds((IEnumerable<BuildDetail>) builds, options, deletedBy);
        ObjectBinder<KeyValuePair<DeleteOptions, string>> current1 = resultCollection.GetCurrent<KeyValuePair<DeleteOptions, string>>();
        while (current1.MoveNext())
        {
          IVssRequestContext requestContext1 = requestContext;
          KeyValuePair<DeleteOptions, string> current2 = current1.Current;
          // ISSUE: variable of a boxed type
          __Boxed<DeleteOptions> key1 = (Enum) current2.Key;
          current2 = current1.Current;
          string str = current2.Value;
          requestContext1.Trace(0, TraceLevel.Verbose, "Build", "Service", "Adding delete options '<{0},{1}>'", (object) key1, (object) str);
          Dictionary<string, DeleteOptions> dictionary3 = dictionary1;
          current2 = current1.Current;
          string key2 = current2.Value;
          current2 = current1.Current;
          int key3 = (int) current2.Key;
          dictionary3.Add(key2, (DeleteOptions) key3);
        }
        resultCollection.NextResult();
        ObjectBinder<SymbolStoreData> current3 = resultCollection.GetCurrent<SymbolStoreData>();
        while (current3.MoveNext())
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Adding symbol store '{0}' for build '{1}'", (object) current3.Current.StorePath, (object) current3.Current.BuildUri);
          List<SymbolStoreData> symbolStoreDataList;
          if (!dictionary2.TryGetValue(current3.Current.BuildUri, out symbolStoreDataList))
          {
            symbolStoreDataList = new List<SymbolStoreData>();
            dictionary2.Add(current3.Current.BuildUri, symbolStoreDataList);
          }
          symbolStoreDataList.Add(current3.Current);
        }
      }
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      TeamFoundationEventService service1 = requestContext.GetService<TeamFoundationEventService>();
      TeamFoundationVersionControlService service2 = requestContext.GetService<TeamFoundationVersionControlService>();
      foreach (BuildDeletionResult deleteResult in deleteResults)
      {
        DeleteOptions options1;
        if (deleteResult.Build != null && !stringSet.Contains(deleteResult.Build.Uri) && dictionary1.TryGetValue(deleteResult.Build.Uri, out options1))
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Deleting build '{0}'", (object) deleteResult.Build.Uri);
          BuildController buildController;
          if (!controllers.TryGetValue(deleteResult.Build.BuildControllerUri, out buildController) && !controllers.TryGetValue(deleteResult.Build.Definition.BuildControllerUri, out buildController))
          {
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Controller '{0}' not found", (object) deleteResult.Build.BuildControllerUri);
            buildController = (BuildController) null;
          }
          if (buildController != null)
          {
            long? containerId1 = deleteResult.Build.ContainerId;
            if (containerId1.HasValue)
            {
              if ((options1 & DeleteOptions.Details) == DeleteOptions.Details)
              {
                try
                {
                  Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, deleteResult.Build.TeamProject);
                  TeamFoundationFileContainerService service3 = requestContext.GetService<TeamFoundationFileContainerService>();
                  IVssRequestContext requestContext2 = requestContext;
                  containerId1 = deleteResult.Build.ContainerId;
                  long containerId2 = containerId1.Value;
                  Guid scopeIdentifier = projectId;
                  service3.DeleteContainer(requestContext2, containerId2, scopeIdentifier);
                }
                catch (FileContainerException ex)
                {
                  requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Container '{0}' does not exist in the file container service", (object) deleteResult.Build.DropLocationRoot);
                }
              }
            }
            if ((options1 & DeleteOptions.DropLocation) == DeleteOptions.DropLocation)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Initiating drop deletion for build '{0}'", (object) deleteResult.Build.Uri);
              buildController.DeleteBuildDrop(requestContext, deleteResult.Build);
            }
            List<SymbolStoreData> symbolStores;
            if ((options1 & DeleteOptions.Symbols) == DeleteOptions.Symbols && dictionary2.TryGetValue(deleteResult.Build.Uri, out symbolStores))
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Initiating symbol store deletion for build '{0}'", (object) deleteResult.Build.Uri);
              buildController.DeleteBuildSymbols(requestContext, (IList<SymbolStoreData>) symbolStores);
            }
          }
          if ((options1 & DeleteOptions.Label) == DeleteOptions.Label && !string.IsNullOrEmpty(deleteResult.Build.LabelName) && LabelSpec.IsLegalSpec(deleteResult.Build.LabelName))
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Removing label '{0}' from version control for build '{1}'", (object) deleteResult.Build.LabelName, (object) deleteResult.Build.Uri);
            try
            {
              string labelName;
              string labelScope;
              LabelSpec.Parse(deleteResult.Build.LabelName, VersionControlPath.PrependRootIfNeeded(deleteResult.Build.Definition.TeamProject.Name), false, out labelName, out labelScope);
              service2.DeleteLabel(requestContext, labelName, labelScope);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
              deleteResult.LabelFailure = new Failure(ex);
            }
          }
          stringSet.Add(deleteResult.Build.Uri);
          service1.PublishNotification(requestContext, (object) new BuildDeletedEvent(deleteResult.Build, options1, requestContext.UserContext));
        }
      }
    }

    public void DeleteProcessTemplates(IVssRequestContext requestContext, IList<int> templateIds)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DeleteProcessTemplates));
      ArgumentValidation.Check(nameof (templateIds), (object) templateIds, false);
      List<ProcessTemplate> processTemplateList1 = this.QueryProcessTemplatesById(requestContext, templateIds);
      List<ProcessTemplate> processTemplateList2 = new List<ProcessTemplate>();
      int index = 0;
      foreach (ProcessTemplate processTemplate in processTemplateList1)
      {
        if (processTemplate != null)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Processing to delete template '{0}'", (object) processTemplate.Id);
          this.m_buildHost.SecurityManager.CheckProjectPermission(requestContext, processTemplate.TeamProjectObj, BuildPermissions.DeleteBuildDefinition);
          processTemplateList2.Add(processTemplate);
        }
        else
          requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Template '{0}' not found", (object) templateIds[index]);
        ++index;
      }
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        component.DeleteProcessTemplates((ICollection<ProcessTemplate>) processTemplateList2);
      requestContext.TraceLeave(0, "Build", "Service", nameof (DeleteProcessTemplates));
    }

    public bool DeleteTeamProject(IVssRequestContext requestContext, string projectUri)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DeleteTeamProject));
      ArgumentValidation.CheckUri(nameof (projectUri), projectUri, false, (string) null);
      TeamProject teamProjectFromUri = this.m_projectService.GetTeamProjectFromUri(requestContext, projectUri);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string securityToken = this.m_projectService.GetSecurityToken(requestContext, teamProjectFromUri.Uri);
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      int delete = TeamProjectPermissions.Delete;
      securityNamespace.CheckPermission(requestContext1, token, delete);
      using (IDisposableReadOnlyList<IBuildProjectInstaller> extensions = requestContext.GetExtensions<IBuildProjectInstaller>())
      {
        foreach (IBuildProjectInstaller projectInstaller in (IEnumerable<IBuildProjectInstaller>) extensions)
        {
          string fullName = projectInstaller.GetType().FullName;
          try
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Running project installer {0}", (object) fullName);
            projectInstaller.Uninstall(requestContext, teamProjectFromUri.Id);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Succesfully ran project installer {0}", (object) fullName);
          }
          catch (Exception ex)
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Encountered exception while running installer {0}", (object) fullName);
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
      }
      requestContext.GetService<TeamFoundationDeploymentService>().DeleteTeamProject(requestContext.Elevate(), projectUri);
      IEnumerable<string> uris = (IEnumerable<string>) null;
      try
      {
        using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
          uris = component.DeleteTeamProject(teamProjectFromUri.Uri, this.WriterId, this.m_definitionArtifactKind);
      }
      catch (DataspaceNotFoundException ex)
      {
      }
      finally
      {
        this.m_buildDefinitionCache.Invalidate(requestContext, uris);
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (DeleteTeamProject));
      return true;
    }

    public void DestroyBuilds(IVssRequestContext requestContext, IList<string> uris)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (DestroyBuilds));
      ArgumentValidation.CheckUriArray(nameof (uris), uris, "Build", false, (string) null);
      List<BuildDetail> builds = new List<BuildDetail>();
      using (TeamFoundationDataReader foundationDataReader = this.QueryBuildsByUri(requestContext, uris, (IList<string>) null, QueryOptions.Definitions, QueryDeletedOption.IncludeDeleted, new Guid(), false))
      {
        int index = 0;
        foreach (BuildDetail build in foundationDataReader.Current<BuildQueryResult>().Builds)
        {
          if (build != null)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Processing to destroy build '{0}'", (object) build.Uri);
            if (BuildCommonUtil.IsDefaultDateTime(build.FinishTime))
            {
              requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Cannot destroy in progress build '{0}'", (object) build.Uri);
              throw new CannotDeleteInProgressBuildException(build.BuildNumber);
            }
            this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, build.Definition, BuildPermissions.DestroyBuilds);
            builds.Add(build);
          }
          else
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Build '{0}' not found", (object) uris[index]);
          ++index;
        }
      }
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        component.DestroyBuilds((IEnumerable<BuildDetail>) builds);
      requestContext.TraceLeave(0, "Build", "Service", nameof (DestroyBuilds));
    }

    public List<BuildDefinition> GetAffectedBuildDefinitions(
      IVssRequestContext requestContext,
      IList<string> serverItems,
      DefinitionTriggerType continuousIntegrationType)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (GetAffectedBuildDefinitions));
      Validation.CheckVersionControlPath(nameof (serverItems), serverItems, false);
      Func<BuildDefinition, bool> ignoreDefinition = (Func<BuildDefinition, bool>) (definition =>
      {
        bool buildDefinitions = !this.m_buildHost.SecurityManager.BuildSecurity.HasPermission(requestContext, definition.SecurityToken, BuildPermissions.ViewBuildDefinition);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Definition '{0}' is ignored ({0})", (object) definition.Name, (object) buildDefinitions);
        return buildDefinitions;
      });
      requestContext.TraceLeave(0, "Build", "Service", nameof (GetAffectedBuildDefinitions));
      return this.GetAffectedBuildDefinitions(requestContext, (IEnumerable<string>) serverItems, continuousIntegrationType, ignoreDefinition, false).Values.ToList<BuildDefinition>();
    }

    public List<string> GetBuildQualities(IVssRequestContext requestContext, string teamProject)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (GetBuildQualities));
      ArgumentValidation.Check(nameof (teamProject), (object) teamProject, true);
      TeamProject teamProject1 = (TeamProject) null;
      bool flag;
      if (!string.IsNullOrEmpty(teamProject))
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Verifying ViewBuilds permission on team project '{0}'", (object) teamProject);
        teamProject1 = this.m_projectService.GetTeamProjectFromGuidOrName(requestContext, teamProject);
        flag = this.m_buildHost.SecurityManager.HasProjectPermission(requestContext, teamProject1, BuildPermissions.ViewBuilds);
      }
      else
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Verifying permission when connecting from V1");
        flag = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1, false);
      }
      List<string> buildQualities = new List<string>();
      if (flag)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Reading build qualities");
        using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        {
          foreach (string str in component.GetBuildQualities(teamProject1).GetCurrent<string>().Items)
            buildQualities.Add(str);
        }
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (GetBuildQualities));
      return buildQualities;
    }

    public BuildDetail NotifyBuildCompleted(IVssRequestContext requestContext, string buildUri)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (NotifyBuildCompleted));
      ArgumentValidation.CheckUri(nameof (buildUri), buildUri, "Build", false, (string) null);
      BuildUpdateOptions buildUpdateOptions = new BuildUpdateOptions()
      {
        Uri = buildUri,
        Fields = BuildUpdate.FinishTime
      };
      requestContext.TraceLeave(0, "Build", "Service", nameof (NotifyBuildCompleted));
      return this.UpdateBuilds(requestContext, (IList<BuildUpdateOptions>) new BuildUpdateOptions[1]
      {
        buildUpdateOptions
      }, new Guid())[0];
    }

    public Guid RequestIntermediateLogs(IVssRequestContext requestContext, string buildUri)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (RequestIntermediateLogs));
      ArgumentValidation.CheckUri(nameof (buildUri), buildUri, "Build", false, (string) null);
      TeamFoundationIdentity foundationIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      BuildDetail buildDetail = (BuildDetail) null;
      string dropLocation = (string) null;
      BuildController buildController = (BuildController) null;
      using (TeamFoundationDataReader foundationDataReader = this.QueryBuildsByUri(requestContext.Elevate(), (IList<string>) new string[1]
      {
        buildUri
      }, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.BatchedRequests, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
      {
        buildDetail = foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>();
        if (buildDetail == null || !this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.ViewBuilds))
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Invalid build uri '{0}'", (object) buildUri);
          throw new InvalidBuildUriException(buildUri);
        }
        if (buildDetail.Status != BuildStatus.InProgress)
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Build '{0}' is not in progress", (object) buildUri);
          throw new BuildServerException(ResourceStrings.RequestLogs_BuildNotInProgress((object) buildDetail.BuildNumber));
        }
        dropLocation = BuildContainerPath.IsServerPath(buildDetail.LogLocation) ? buildDetail.LogLocation : buildDetail.DropLocation;
        if (string.IsNullOrEmpty(dropLocation))
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Build '{0}' does not have drop location", (object) buildUri);
          throw new BuildServerException(ResourceStrings.RequestLogs_DropLocationNotSpecified((object) buildDetail.BuildNumber));
        }
        foreach (QueuedBuild queuedBuild in foundationDataReader.Current<BuildQueryResult>().QueuedBuilds)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking permission for queued build '{0}'", (object) queuedBuild.Id);
          if (queuedBuild.IsRequestor(foundationIdentity))
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Skipped checking permission for queued build '{0}' because it belongs to the requestor", (object) queuedBuild.Id);
          }
          else
          {
            this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, queuedBuild.Definition, BuildPermissions.StopBuilds);
            break;
          }
        }
        buildController = foundationDataReader.Current<BuildQueryResult>().Controllers.FirstOrDefault<BuildController>();
      }
      Guid requestIdentifier = Guid.NewGuid();
      if (buildDetail != null && buildController != null)
        buildController.RequestIntermediateLogs(requestContext, buildDetail.Uri, dropLocation, foundationIdentity, requestIdentifier);
      requestContext.TraceLeave(0, "Build", "Service", nameof (RequestIntermediateLogs));
      return requestIdentifier;
    }

    public BuildQueueQueryResult QueueBuilds(
      IVssRequestContext requestContext,
      IList<BuildRequest> requests,
      QueueOptions options,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueueBuilds));
      Validation.CheckValidatable<BuildRequest>(requestContext, nameof (requests), requests, false, ValidationContext.Add);
      TeamFoundationVersionControlService service1 = requestContext.GetService<TeamFoundationVersionControlService>();
      Dictionary<string, List<BuildRequest>> dictionary1 = new Dictionary<string, List<BuildRequest>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (BuildRequest request in (IEnumerable<BuildRequest>) requests)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Processing request '{0}'", (object) request);
        List<BuildRequest> buildRequestList;
        if (!dictionary1.TryGetValue(request.BuildDefinitionUri, out buildRequestList))
        {
          buildRequestList = new List<BuildRequest>();
          dictionary1.Add(request.BuildDefinitionUri, buildRequestList);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Grouped requests by definition '{0}'", (object) request.BuildDefinitionUri);
        }
        buildRequestList.Add(request);
        if (request.Priority != QueuePriority.Normal)
        {
          stringSet.Add(request.BuildDefinitionUri);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Definition '{0}' has request with priority different than 'Normal'", (object) request.BuildDefinitionUri);
        }
        if (!string.IsNullOrEmpty(request.ProcessParameters))
        {
          request.BatchId = Guid.NewGuid();
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Assigned batch Id '{0}' because it has set process parameters", (object) request.BatchId);
        }
        if (request.GetOption == GetOption.LatestOnBuild)
        {
          request.CustomGetVersion = new LatestVersionSpec().ToDBString((IVssRequestContext) null);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Assigned custom get version '{0}' for GetOption.LatestOnBuild", (object) request.CustomGetVersion);
        }
        else if (request.GetOption == GetOption.LatestOnQueue)
        {
          request.CustomGetVersion = new ChangesetVersionSpec(service1.GetLatestChangeset(requestContext)).ToDBString((IVssRequestContext) null);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Assigned custom get version '{0}' for GetOption.LatestOnQueue", (object) request.CustomGetVersion);
        }
      }
      string[] array1 = dictionary1.Keys.ToArray<string>();
      BuildDefinitionQueryResult definitionQueryResult = this.QueryBuildDefinitionsByUri(requestContext.Elevate(), (IList<string>) array1, (IList<string>) null, QueryOptions.Controllers, projectId);
      if (definitionQueryResult.Definitions[0] != null)
        TeamFoundationBuildService.CheckXamlEnabled(requestContext);
      if (requests.Count == 1 && requests[0].Reason == BuildReason.CheckInShelveset && definitionQueryResult.Definitions[0] == null)
      {
        QueuedBuild queuedBuild = this.m_build2Converter.QueueBuild(requestContext, projectId, requests[0]);
        return new BuildQueueQueryResult()
        {
          QueuedBuilds = {
            (object) queuedBuild
          }
        };
      }
      int index1 = 0;
      Dictionary<string, BuildController> dictionary2 = definitionQueryResult.Controllers.ToDictionary<BuildController, string, BuildController>((Func<BuildController, string>) (x => x.Uri), (Func<BuildController, BuildController>) (x => x));
      Dictionary<string, BuildServiceHost> dictionary3 = definitionQueryResult.ServiceHosts.ToDictionary<BuildServiceHost, string, BuildServiceHost>((Func<BuildServiceHost, string>) (x => x.Uri), (Func<BuildServiceHost, BuildServiceHost>) (x => x));
      Dictionary<string, BuildDefinition> definitionLookup = new Dictionary<string, BuildDefinition>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string[] array2 = requests.Where<BuildRequest>((Func<BuildRequest, bool>) (x => !string.IsNullOrEmpty(x.BuildControllerUri))).Select<BuildRequest, string>((Func<BuildRequest, string>) (x => x.BuildControllerUri)).Except<string>((IEnumerable<string>) dictionary2.Keys).ToArray<string>();
      if (array2.Length != 0)
      {
        TeamFoundationBuildResourceService service2 = requestContext.GetService<TeamFoundationBuildResourceService>();
        int index2 = 0;
        IVssRequestContext requestContext1 = requestContext.Elevate();
        string[] controllerUris = array2;
        BuildControllerQueryResult controllerQueryResult = service2.QueryBuildControllersByUri(requestContext1, (IList<string>) controllerUris, (IList<string>) null, false);
        foreach (BuildController controller in controllerQueryResult.Controllers)
        {
          dictionary2[controller.Uri] = controller != null ? controller : throw new BuildControllerNotFoundException(array2[index2]);
          ++index2;
        }
        foreach (BuildServiceHost serviceHost in controllerQueryResult.ServiceHosts)
          dictionary3[serviceHost.Uri] = serviceHost;
      }
      foreach (BuildDefinition definition in definitionQueryResult.Definitions)
      {
        if (definition == null)
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Definition '{0}' not found", (object) array1[index1]);
          throw new BuildDefinitionDoesNotExistException(array1[index1]);
        }
        bool flag = this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, definition, BuildPermissions.ViewBuildDefinition, true);
        definitionLookup[definition.Uri] = definition;
        GatedCheckInTicketService service3 = requestContext.GetService<GatedCheckInTicketService>();
        foreach (IGrouping<bool, BuildRequest> grouping in dictionary1[definition.Uri].GroupBy<BuildRequest, bool>((Func<BuildRequest, bool>) (x => x.Reason == BuildReason.CheckInShelveset && !string.IsNullOrEmpty(x.CheckInTicket))))
        {
          if (grouping.Key && service3.HasKey)
          {
            foreach (BuildRequest buildRequest in (IEnumerable<BuildRequest>) grouping)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Validating ticket for request '{0}'", (object) buildRequest);
              service3.ValidateCheckInTicket(requestContext, buildRequest.CheckInTicket, buildRequest.BuildDefinitionUri, buildRequest.ShelvesetName);
            }
            this.CheckSupportedReasons(requestContext, definition, BuildReason.CheckInShelveset);
          }
          else
          {
            if (!flag)
            {
              requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Definition '{0}' not found", (object) definition.Uri);
              throw new BuildDefinitionDoesNotExistException(definition.Uri);
            }
            this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, definition, BuildPermissions.QueueBuilds);
            if (stringSet.Contains(definition.Uri))
              this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, definition, BuildPermissions.ManageBuildQueue);
            foreach (BuildRequest buildRequest in (IEnumerable<BuildRequest>) grouping)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking support reason for request '{0}'", (object) buildRequest);
              this.CheckSupportedReasons(requestContext, definition, buildRequest.Reason);
            }
          }
          foreach (BuildRequest buildRequest in (IEnumerable<BuildRequest>) grouping)
          {
            buildRequest.ProjectId = definition.TeamProject.Id;
            string buildControllerUri = buildRequest.BuildControllerUri;
            if (string.IsNullOrEmpty(buildControllerUri))
              buildControllerUri = definition.BuildControllerUri;
            BuildController buildController;
            if (dictionary2.TryGetValue(buildControllerUri, out buildController))
            {
              string path = buildRequest.DropLocation;
              if (string.IsNullOrEmpty(path))
                path = definition.DefaultDropLocation;
              BuildServiceHost buildServiceHost;
              if (!dictionary3.TryGetValue(buildController.ServiceHostUri, out buildServiceHost))
                throw new NotSupportedException(ResourceStrings.UnableToFindControllersServiceHost((object) buildController.ServiceHostUri, (object) buildController.Name));
              if (BuildContainerPath.IsServerPath(path))
              {
                if (requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build.NoServerDrops"))
                  throw new NotSupportedException(ResourceStrings.BuildContainerDropDisabled());
                if (buildController.Version < 410 && !buildServiceHost.IsVirtual)
                  throw new NotSupportedException(ResourceStrings.BuildContainerDropNotSupported((object) definition.Name, (object) buildController.Name));
              }
              if (buildController.Version < 400)
              {
                if (buildRequest.Reason == BuildReason.CheckInShelveset && definition.BatchSize > 1)
                  throw new NotSupportedException(ResourceStrings.CannotBatchWith2010((object) buildController.Name));
                int num = 0;
                if (definition.Process != null)
                  num = VersionHelper.Parse(definition.Process.Version);
                if (num >= 1100)
                  throw new NotSupportedException(ResourceStrings.IncompatibleController((object) buildController.Name));
              }
              if (definition.Process != null && BuildSourceProviders.GitProperties.IsGitUri(definition.Process.ServerPath) && buildController.Version < 500 && !buildServiceHost.IsVirtual)
                throw new NotSupportedException(ResourceStrings.GitCustomTemplateNotSupported((object) definition.Name, (object) buildController.Name));
            }
            if (!string.IsNullOrEmpty(buildRequest.DropLocation))
            {
              if (VersionControlPath.IsServerItem(buildRequest.DropLocation) && (string.IsNullOrEmpty(definition.DefaultDropLocation) || !VersionControlPath.IsServerItem(definition.DefaultDropLocation) || !VersionControlPath.IsSubItem(buildRequest.DropLocation, definition.DefaultDropLocation)))
                throw new ArgumentException(ResourceStrings.InvalidVersionControlDropLocation((object) buildRequest.DropLocation, (object) definition.DefaultDropLocation));
              if (BuildContainerPath.IsServerPath(buildRequest.DropLocation) && (string.IsNullOrEmpty(definition.DefaultDropLocation) || !BuildContainerPath.IsServerPath(definition.DefaultDropLocation) || !BuildContainerPath.IsSubItem(buildRequest.DropLocation, definition.DefaultDropLocation)))
                throw new ArgumentException(ResourceStrings.InvalidBuildContainerDropLocation((object) buildRequest.DropLocation, (object) definition.DefaultDropLocation));
            }
          }
        }
        ++index1;
      }
      List<QueuedBuild> items1;
      BuildDetailDictionary allBuilds;
      IList<StartBuildData> items2;
      using (BuildQueueComponent component = requestContext.CreateComponent<BuildQueueComponent>("Build"))
      {
        ResultCollection resultCollection = component.QueueBuilds((ICollection<BuildRequest>) requests, options);
        items1 = resultCollection.GetCurrent<QueuedBuild>().Items;
        resultCollection.NextResult();
        allBuilds = new BuildDetailDictionary((IEnumerable<BuildDetail>) resultCollection.GetCurrent<BuildDetail>().Items);
        resultCollection.NextResult();
        items2 = (IList<StartBuildData>) resultCollection.GetCurrent<StartBuildData>().Items;
      }
      BuildController.StartBuilds(requestContext, items2, (IDictionary<string, BuildDefinition>) definitionLookup, (IDictionary<string, BuildController>) dictionary2, (IDictionary<string, BuildServiceHost>) dictionary3);
      HashSet<string> includedBuilds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> includedDefinitions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      BuildQueueQueryResult queueQueryResult = new BuildQueueQueryResult();
      foreach (QueuedBuild queuedBuild in items1)
      {
        BuildDefinition definition;
        if (definitionLookup.TryGetValue(queuedBuild.BuildDefinitionUri, out definition))
        {
          this.MatchBuildsToQueuedBuilds(requestContext, QueryOptions.Definitions, queueQueryResult.Builds, (IList<BuildDefinition>) queueQueryResult.Definitions, (IDictionary<string, BuildDetail>) allBuilds, includedBuilds, includedDefinitions, definition, queuedBuild);
          queueQueryResult.QueuedBuilds.Add((object) queuedBuild);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Queued build '{0}'", (object) queuedBuild.Id);
        }
        else
          requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Definition '{0}' not found", (object) queuedBuild.BuildDefinitionUri);
      }
      if (options != QueueOptions.Preview)
      {
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          QueuedBuild[] array3 = items1.ToArray();
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.BuildsQueued(requestContext, (IList<QueuedBuild>) array3, false);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
          TeamFoundationBuildService.PublishBuildsQueued(requestContext, array3);
        }
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (QueueBuilds));
      return queueQueryResult;
    }

    public BuildQueueQueryResult StartQueuedBuildsNow(
      IVssRequestContext requestContext,
      int[] queueIds,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (StartQueuedBuildsNow));
      TeamFoundationBuildService.CheckXamlEnabled(requestContext);
      BuildDefinitionDictionary definitionDictionary = new BuildDefinitionDictionary();
      List<QueuedBuild> builds = new List<QueuedBuild>();
      using (TeamFoundationDataReader foundationDataReader = this.QueryQueuedBuildsById(requestContext.Elevate(), (IList<int>) queueIds, (IList<string>) Array.Empty<string>(), QueryOptions.Definitions, projectId))
      {
        BuildQueueQueryResult queueQueryResult = foundationDataReader.Current<BuildQueueQueryResult>();
        int index = 0;
        while (queueQueryResult.QueuedBuilds.MoveNext())
        {
          if (queueQueryResult.QueuedBuilds.Current == null)
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Queued build '{0}' not found", (object) queueIds[index]);
            throw new QueuedBuildDoesNotExistException(queueIds[index]);
          }
          ++index;
          builds.Add(queueQueryResult.QueuedBuilds.Current);
        }
        foreach (BuildDefinition definition in queueQueryResult.Definitions)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking permission on definition '{0}'", (object) definition.Uri);
          definitionDictionary.Add(definition);
          this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, definition, BuildPermissions.ManageBuildQueue);
        }
      }
      BuildQueueQueryResult queueQueryResult1 = new BuildQueueQueryResult();
      List<QueuedBuild> items1;
      BuildDetailDictionary allBuilds;
      List<StartBuildData> items2;
      using (BuildQueueComponent component = requestContext.CreateComponent<BuildQueueComponent>("Build"))
      {
        ResultCollection resultCollection = component.StartQueuedBuildsNow((ICollection<QueuedBuild>) builds);
        items1 = resultCollection.GetCurrent<QueuedBuild>().Items;
        resultCollection.NextResult();
        allBuilds = new BuildDetailDictionary((IEnumerable<BuildDetail>) resultCollection.GetCurrent<BuildDetail>().Items);
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<StartBuildData>().Items;
      }
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) items2);
      HashSet<string> includedBuilds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> includedDefinitions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (QueuedBuild queuedBuild in items1)
      {
        BuildDefinition definition;
        if (definitionDictionary.TryGetValue(queuedBuild.BuildDefinitionUri, out definition))
        {
          this.MatchBuildsToQueuedBuilds(requestContext, QueryOptions.Definitions, queueQueryResult1.Builds, (IList<BuildDefinition>) queueQueryResult1.Definitions, (IDictionary<string, BuildDetail>) allBuilds, includedBuilds, includedDefinitions, definition, queuedBuild);
          queueQueryResult1.QueuedBuilds.Enqueue(queuedBuild);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Started queued build '{0}'", (object) queuedBuild.Id);
        }
        else
          requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Definition '{0}' not found", (object) queuedBuild.BuildDefinitionUri);
      }
      if (items1.Count > 0)
      {
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.BuildsQueued(requestContext.Elevate(), (IList<QueuedBuild>) items1, true);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
        TeamFoundationBuildService.PublishBuildsQueued(requestContext, items1.ToArray());
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (StartQueuedBuildsNow));
      return queueQueryResult1;
    }

    private void CheckSupportedReasons(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildReason requiredReason)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (CheckSupportedReasons));
      if (definition.Process == null)
      {
        ProcessTemplate processTemplate = this.QueryProcessTemplatesById(requestContext, (IList<int>) new int[1]
        {
          definition.ProcessTemplateId
        }).FirstOrDefault<ProcessTemplate>();
        if (processTemplate != null && !string.IsNullOrEmpty(processTemplate.ServerPath))
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Process template '{0}' deleted", (object) processTemplate.ServerPath);
          throw new ProcessTemplateDeletedException(processTemplate.ServerPath);
        }
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Process template '{0}' deleted", (object) definition.ProcessTemplateId);
        throw new ProcessTemplateDeletedException();
      }
      if ((definition.Process.SupportedReasons & requiredReason) != requiredReason)
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Build reason '{0}' not supported by definition '{1}'", (object) requiredReason, (object) definition.Uri);
        throw new BuildReasonNotSupportedException(requiredReason, definition.Process.SupportedReasons, definition.Process.ServerPath);
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (CheckSupportedReasons));
    }

    public BuildDefinitionQueryResult QueryBuildDefinitions(
      IVssRequestContext requestContext,
      BuildDefinitionSpec spec,
      bool strict = false,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryBuildDefinitions));
      Validation.CheckValidatable(requestContext, nameof (spec), (IValidatable) spec, false, ValidationContext.Query);
      bool readServices = true;
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Exiting due to access denied: ViewBuildResources");
        readServices = false;
      }
      BuildDefinitionQueryResult definitionQueryResult = new BuildDefinitionQueryResult();
      TeamFoundationBuildService.BuildDefinitionDatabaseResult definitionDatabaseResult;
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        definitionDatabaseResult = TeamFoundationBuildService.ReadDatabaseResults(component.QueryBuildDefinitions(requestContext, BuildCommonUtil.IsStar(spec.TeamProject) ? (string) null : spec.TeamProject, spec.Path, spec.TriggerType, spec.Options, strict), readServices);
      this.PostProcessDatabaseResults(requestContext, definitionDatabaseResult);
      if (readServices)
        this.MatchBuildServices(requestContext, spec.Options, definitionDatabaseResult, (IList<BuildAgent>) definitionQueryResult.Agents, (IList<BuildController>) definitionQueryResult.Controllers, (IList<BuildServiceHost>) definitionQueryResult.ServiceHosts);
      bool flag = requestContext.IsStakeholder() && !StakeholderLicensingHelper.IsBuildAndReleaseEnabledForStakeholders(requestContext);
      foreach (BuildDefinition definition in definitionDatabaseResult.Definitions)
      {
        if (this.BuildHost.SecurityManager.BuildSecurity.HasPermission(requestContext, definition.SecurityToken, BuildPermissions.ViewBuildDefinition, !flag))
          definitionQueryResult.Definitions.Add(definition);
      }
      if (spec.PropertyNameFilters != null && spec.PropertyNameFilters.Any<string>())
      {
        TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
        IEnumerable<ArtifactSpec> artifactSpecs1 = definitionQueryResult.Definitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (x => x != null)).Select<BuildDefinition, ArtifactSpec>((Func<BuildDefinition, ArtifactSpec>) (x => ArtifactHelper.CreateArtifactSpec(x.Uri, BuildPropertyKinds.BuildDefinition, x.TeamProject.Id)));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
        List<string> propertyNameFilters = spec.PropertyNameFilters;
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs2, (IEnumerable<string>) propertyNameFilters))
          ArtifactHelper.MatchProperties<BuildDefinition>(properties, (IList<BuildDefinition>) definitionQueryResult.Definitions, (Func<BuildDefinition, int>) (x => x.Id), (Action<BuildDefinition, List<PropertyValue>>) ((x, y) => x.Properties.AddRange((IEnumerable<PropertyValue>) y)));
      }
      return definitionQueryResult;
    }

    public List<BuildDefinitionQueryResult> QueryBuildDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinitionSpec> specs,
      bool strict = false,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryBuildDefinitions));
      Validation.CheckValidatable<BuildDefinitionSpec>(requestContext, nameof (specs), specs, false, ValidationContext.Query);
      List<BuildDefinitionQueryResult> definitionQueryResultList = new List<BuildDefinitionQueryResult>();
      foreach (BuildDefinitionSpec spec in (IEnumerable<BuildDefinitionSpec>) specs)
        definitionQueryResultList.Add(this.QueryBuildDefinitions(requestContext, spec, strict, projectId));
      return definitionQueryResultList;
    }

    public BuildDefinitionQueryResult QueryBuildDefinitionsByUri(
      IVssRequestContext requestContext,
      IList<string> uris,
      IList<string> propertyNames,
      QueryOptions options,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryBuildDefinitionsByUri));
      ArgumentValidation.CheckUriArray(nameof (uris), uris, "Definition", false, (string) null);
      bool readServices = true;
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Exiting due to access denied: ViewBuildResources");
        readServices = false;
      }
      if (propertyNames == null)
        propertyNames = (IList<string>) Array.Empty<string>();
      BuildDefinitionQueryResult definitionQueryResult = new BuildDefinitionQueryResult();
      TeamFoundationBuildService.BuildDefinitionDatabaseResult definitionDatabaseResult;
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        definitionDatabaseResult = TeamFoundationBuildService.ReadDatabaseResults(component.QueryBuildDefinitionsByUri((IEnumerable<string>) uris, options), readServices);
      this.PostProcessDatabaseResults(requestContext, definitionDatabaseResult);
      if (readServices)
        this.MatchBuildServices(requestContext, options, definitionDatabaseResult, (IList<BuildAgent>) definitionQueryResult.Agents, (IList<BuildController>) definitionQueryResult.Controllers, (IList<BuildServiceHost>) definitionQueryResult.ServiceHosts);
      Dictionary<string, BuildDefinition> dictionary = definitionDatabaseResult.Definitions.ToDictionary<BuildDefinition, string>((Func<BuildDefinition, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string uri in (IEnumerable<string>) uris)
      {
        BuildDefinition buildDefinition;
        if (!dictionary.TryGetValue(uri, out buildDefinition))
          definitionQueryResult.Definitions.Add((BuildDefinition) null);
        else if (!buildDefinition.TeamProject.MatchesScope(projectId) || !this.BuildHost.SecurityManager.BuildSecurity.HasPermission(requestContext, buildDefinition.SecurityToken, BuildPermissions.ViewBuildDefinition, false))
          definitionQueryResult.Definitions.Add((BuildDefinition) null);
        else
          definitionQueryResult.Definitions.Add(buildDefinition);
      }
      if (propertyNames != null && propertyNames.Any<string>())
      {
        TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
        IEnumerable<ArtifactSpec> artifactSpecs1 = definitionQueryResult.Definitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (x => x != null)).Select<BuildDefinition, ArtifactSpec>((Func<BuildDefinition, ArtifactSpec>) (x => ArtifactHelper.CreateArtifactSpec(x.Uri, BuildPropertyKinds.BuildDefinition, x.TeamProject.Id)));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
        IList<string> propertyNameFilters = propertyNames;
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs2, (IEnumerable<string>) propertyNameFilters))
          ArtifactHelper.MatchProperties<BuildDefinition>(properties, (IList<BuildDefinition>) definitionQueryResult.Definitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (x => x != null)).ToList<BuildDefinition>(), (Func<BuildDefinition, int>) (x => x.Id), (Action<BuildDefinition, List<PropertyValue>>) ((x, y) => x.Properties.AddRange((IEnumerable<PropertyValue>) y)));
      }
      return definitionQueryResult;
    }

    public TeamFoundationDataReader QueryBuilds(
      IVssRequestContext requestContext,
      IList<BuildDetailSpec> specs,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryBuilds));
      Validation.CheckValidatable<BuildDetailSpec>(requestContext, nameof (specs), specs, false, ValidationContext.Query);
      CommandQueryBuilds disposableObject = (CommandQueryBuilds) null;
      try
      {
        disposableObject = new CommandQueryBuilds(requestContext, projectId);
        disposableObject.Execute(specs);
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryBuilds));
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Results
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Service", ex);
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryBuildsByUri(
      IVssRequestContext requestContext,
      IList<string> uris,
      IList<string> informationTypes,
      QueryOptions options,
      QueryDeletedOption deletedOption,
      Guid projectId = default (Guid),
      bool xamlBuildsOnly = false)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryBuildsByUri));
      HashSet<string> compatUris = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      ArgumentValidation.CheckArray<string>(nameof (uris), uris, (Validate<string>) ((argumentName, value, allowEmpty, errorMessage) =>
      {
        ArgumentValidation.CheckUri(argumentName, value, "Build", allowEmpty, errorMessage);
        if (value.IndexOf("?queueId=", 0, StringComparison.Ordinal) < 0)
          return;
        compatUris.Add(value);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added uri '{0}' to compatUris", (object) value);
      }), false, (string) null);
      if (compatUris.Count > 0)
      {
        using (CompatibilityComponent component = requestContext.CreateComponent<CompatibilityComponent>("Build"))
        {
          IDictionary<string, string> dictionary = component.ResolveBuildUris((ICollection<string>) compatUris.ToList<string>(), false);
          for (int index = 0; index < uris.Count; ++index)
          {
            string str;
            if (dictionary.TryGetValue(uris[index], out str))
            {
              requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Replacing uri '{0}' by '{1}'", (object) uris[index], (object) str);
              uris[index] = str;
            }
          }
        }
      }
      if (informationTypes == null)
        informationTypes = (IList<string>) Array.Empty<string>();
      CommandQueryBuildsByUri disposableObject = (CommandQueryBuildsByUri) null;
      try
      {
        disposableObject = new CommandQueryBuildsByUri(requestContext, projectId, this.m_build2Converter);
        disposableObject.Execute(uris, informationTypes, options, deletedOption, uris.Count == 1 && !xamlBuildsOnly);
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryBuildsByUri));
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Service", ex);
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryQueuedBuilds(
      IVssRequestContext requestContext,
      IList<BuildQueueSpec> specs,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryQueuedBuilds));
      Validation.CheckValidatable<BuildQueueSpec>(requestContext, nameof (specs), specs, false, ValidationContext.Query);
      CommandQueryQueuedBuilds disposableObject = (CommandQueryQueuedBuilds) null;
      try
      {
        disposableObject = new CommandQueryQueuedBuilds(requestContext, projectId, this.m_build2Converter);
        disposableObject.Execute(specs, specs.Count == 1 && !string.IsNullOrEmpty(specs[0].RequestedFor));
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryQueuedBuilds));
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Results
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Service", ex);
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryQueuedBuildsById(
      IVssRequestContext requestContext,
      IList<int> ids,
      IList<string> informationTypes,
      QueryOptions options,
      Guid projectId = default (Guid))
    {
      ArgumentValidation.Check(nameof (ids), (object) ids, false);
      return this.QueryQueuedBuildsById(requestContext, ids, informationTypes, options, ids.Count == 1, projectId);
    }

    public TeamFoundationDataReader QueryQueuedBuildsById(
      IVssRequestContext requestContext,
      IList<int> ids,
      IList<string> informationTypes,
      QueryOptions options,
      bool includeNewBuilds,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryQueuedBuildsById));
      ArgumentValidation.Check(nameof (ids), (object) ids, false);
      if (informationTypes == null)
        informationTypes = (IList<string>) Array.Empty<string>();
      CommandQueryQueuedBuildsById disposableObject = (CommandQueryQueuedBuildsById) null;
      try
      {
        disposableObject = new CommandQueryQueuedBuildsById(requestContext, projectId, this.m_build2Converter);
        disposableObject.Execute(ids, informationTypes, options, includeNewBuilds);
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryQueuedBuildsById));
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Service", ex);
        disposableObject?.Dispose();
        throw;
      }
    }

    public IDictionary<int, List<int>> GetQueueIdsByBuildIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> buildIds)
    {
      using (BuildQueueComponent component = requestContext.CreateComponent<BuildQueueComponent>())
        return component.GetQueueIdsByBuildIds(projectId, buildIds);
    }

    public void CreateBuiltInProcessTemplates(
      IVssRequestContext requestContext,
      string teamProjectUri,
      bool isUpgrade = false)
    {
      bool flag = TeamFoundationBuildService.IsGitTeamProject(requestContext, teamProjectUri);
      TeamProject teamProjectFromUri = this.m_projectService.GetTeamProjectFromUri(requestContext, teamProjectUri);
      List<ProcessTemplate> processTemplateList1 = this.QueryProcessTemplates(requestContext, teamProjectFromUri.Name, (IList<ProcessTemplateType>) new ProcessTemplateType[3]
      {
        ProcessTemplateType.Custom,
        ProcessTemplateType.Default,
        ProcessTemplateType.Upgrade
      });
      List<ProcessTemplate> first = new List<ProcessTemplate>();
      if (flag)
      {
        string serverPath = "BuildProcessTemplates/GitTemplate/";
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, serverPath, "GitTemplate.12.xaml", ResourceStrings.GitTemplate12Description(), ProcessTemplate.GetTemplate_GitTemplate12(), ProcessTemplateType.Default));
          first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, serverPath, "GitTemplate.xaml", ResourceStrings.GitTemplateDescription(), ProcessTemplate.GetTemplate_GitTemplate11(), ProcessTemplateType.Custom));
          first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, serverPath, "GitContinuousDeploymentTemplate.12.xaml", ResourceStrings.GitContinuousDeploymentTemplate12Description(), ProcessTemplate.GetTemplate_GitContinuousDeploymentTemplate12(), ProcessTemplateType.Custom));
        }
        else
          first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, serverPath, "GitTemplate.12.xaml", ResourceStrings.GitTemplate12Description(), ProcessTemplate.GetTemplate_GitTemplate12(), ProcessTemplateType.Default));
        first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, serverPath, "GitUpgradeTemplate.xaml", ResourceStrings.GitUpgradeTemplateDescription(), ProcessTemplate.GetTemplate_GitUpgradeTemplate(), ProcessTemplateType.Upgrade));
      }
      else
      {
        string serverPath = "BuildProcessTemplates/TfvcTemplate/";
        first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, serverPath, "TfvcTemplate.12.xaml", ResourceStrings.TfvcTemplate12Description(), ProcessTemplate.GetTemplate_TfvcTemplate12(), ProcessTemplateType.Default));
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, serverPath, "TfvcContinuousDeploymentTemplate.12.xaml", ResourceStrings.TfvcContinuousDeploymentTemplate12Description(), ProcessTemplate.GetTemplate_TfvcContinuousDeploymentTemplate12(), ProcessTemplateType.Custom));
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment && !isUpgrade)
          first.Add(this.DeployAndCreateTemplate(requestContext, processTemplateList1, teamProjectFromUri.Name, "BuildProcessTemplates/UpgradeTemplate/", "UpgradeTemplate.xaml", ResourceStrings.UpgradeTemplateDescription(), ProcessTemplate.GetTemplate_UpgradeTemplate(), ProcessTemplateType.Upgrade));
      }
      if (first.Count <= 0)
        return;
      List<ProcessTemplate> processTemplateList2 = new List<ProcessTemplate>();
      foreach (ProcessTemplate processTemplate in first)
      {
        ProcessTemplate t = processTemplate;
        if (processTemplateList1.FirstOrDefault<ProcessTemplate>((Func<ProcessTemplate, bool>) (current => StringComparer.OrdinalIgnoreCase.Equals(current.ServerPath, t.ServerPath))) == null)
          processTemplateList2.Add(t);
      }
      if (processTemplateList2.Count != first.Count)
      {
        List<ProcessTemplate> list = first.Except<ProcessTemplate>((IEnumerable<ProcessTemplate>) processTemplateList2).ToList<ProcessTemplate>();
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Unable to add the following templates because they already existed in the container: '{0}'", (object) string.Join(", ", list.ConvertAll<string>((Converter<ProcessTemplate, string>) (pt => pt.ServerPath)).ToArray()));
      }
      if (processTemplateList2.Count <= 0)
        return;
      this.AddProcessTemplates(requestContext, (IList<ProcessTemplate>) processTemplateList2);
    }

    private ProcessTemplate DeployAndCreateTemplate(
      IVssRequestContext requestContext,
      List<ProcessTemplate> currentTemplates,
      string teamProjectName,
      string serverPath,
      string filename,
      string description,
      string content,
      ProcessTemplateType type)
    {
      string tfs = this.DeployTemplateToTfs(requestContext, content, serverPath, filename);
      ProcessTemplate template = new ProcessTemplate()
      {
        Description = description,
        ServerPath = tfs,
        TeamProject = teamProjectName,
        TemplateType = type
      };
      if (type != ProcessTemplateType.Custom && currentTemplates.FirstOrDefault<ProcessTemplate>((Func<ProcessTemplate, bool>) (ct => ct.TemplateType == type)) != null)
        template.TemplateType = ProcessTemplateType.Custom;
      return template;
    }

    public List<ProcessTemplate> QueryProcessTemplates(
      IVssRequestContext requestContext,
      string teamProject,
      IList<ProcessTemplateType> queryTypes)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryProcessTemplates));
      ArgumentValidation.Check(nameof (teamProject), teamProject, false, (string) null);
      List<ProcessTemplate> processTemplateList1 = this.QueryProcessTemplates(requestContext, teamProject, queryTypes, false);
      List<ProcessTemplate> processTemplateList2 = new List<ProcessTemplate>(processTemplateList1.Count);
      foreach (ProcessTemplate processTemplate in processTemplateList1)
      {
        if (this.m_buildHost.SecurityManager.HasProjectPermission(requestContext, processTemplate.TeamProjectObj, BuildPermissions.ViewBuildDefinition))
        {
          processTemplateList2.Add(processTemplate);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Read template '{0}'", (object) processTemplate.Id);
        }
        else
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Access denied for template '{0}'", (object) processTemplate.Id);
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (QueryProcessTemplates));
      return processTemplateList2;
    }

    public string DeployTemplateToTfs(
      IVssRequestContext requestContext,
      string templateXaml,
      string serverPath,
      string fileName)
    {
      ArgumentValidation.Check(nameof (templateXaml), templateXaml, false, (string) null);
      ArgumentValidation.Check(nameof (serverPath), serverPath, false, (string) null);
      ArgumentValidation.Check(nameof (fileName), fileName, false, (string) null);
      long containerId = long.MinValue;
      string tfs = (string) null;
      string path = serverPath + fileName;
      bool flag = false;
      TeamFoundationFileContainerService service = requestContext.GetService<TeamFoundationFileContainerService>();
      Uri uri = new Uri(LinkingUtilities.VSTFS + "Build" + LinkingUtilities.URISEPARATOR + "BuiltInTemplates" + LinkingUtilities.URISEPARATOR + "BuiltIn");
      List<Uri> artifactUris = new List<Uri>() { uri };
      using (requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, uri.ToString()))
      {
        List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> fileContainerList = service.QueryContainers(requestContext, (IList<Uri>) artifactUris, Guid.Empty);
        if (fileContainerList.Count == 1)
        {
          containerId = fileContainerList[0].Id;
          List<FileContainerItem> fileContainerItemList = service.QueryItems(requestContext, containerId, path, Guid.Empty, false, false);
          if (fileContainerItemList.Count != 0)
          {
            if (fileContainerItemList[0].Status != ContainerItemStatus.Created)
            {
              if (service.QueryItems(requestContext, containerId, serverPath, Guid.Empty, false, false).Where<FileContainerItem>((Func<FileContainerItem, bool>) (item => item.ItemType == ContainerItemType.File)).ToList<FileContainerItem>().Count == 1)
              {
                fileContainerList = new List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
                service.DeleteContainer(requestContext, containerId, Guid.Empty);
              }
              requestContext.Trace(TeamFoundationEventId.FatalErrorDeployingBuildTemplate, TraceLevel.Warning, "Build", "Service", ResourceStrings.BuildProcessTemplateNotFound((object) path, (object) uri));
            }
            else
              tfs = BuildContainerPath.Combine(BuildContainerPath.RootFolder, containerId.ToString((IFormatProvider) CultureInfo.InvariantCulture), path);
          }
        }
        if (tfs == null)
        {
          try
          {
            if (fileContainerList.Count == 0)
            {
              containerId = service.CreateContainer(requestContext, artifactUris[0], "default", "BuiltInTemplates", string.Empty, Guid.Empty, ContainerOptions.None);
              flag = true;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(templateXaml);
            byte[] preamble = new UTF8Encoding(true).GetPreamble();
            byte[] buffer = new byte[bytes.Length + preamble.Length];
            preamble.CopyTo((Array) buffer, 0);
            bytes.CopyTo((Array) buffer, preamble.Length);
            FileContainerItem fileContainerItem1 = new FileContainerItem()
            {
              Path = path,
              ItemType = ContainerItemType.File,
              FileLength = (long) buffer.Length,
              FileHash = (byte[]) null,
              FileEncoding = 1,
              FileType = 1
            };
            using (MemoryStream fileStream = new MemoryStream(buffer, false))
            {
              fileContainerItem1.FileHash = MD5Util.CalculateMD5((Stream) fileStream, true);
              List<FileContainerItem> items = service.CreateItems(requestContext.Elevate(), containerId, (IList<FileContainerItem>) new FileContainerItem[1]
              {
                fileContainerItem1
              }, Guid.Empty);
              FileContainerItem fileContainerItem2 = service.UploadFile(requestContext.Elevate(), containerId, items[0], (Stream) fileStream, 0L, items[0].FileLength, CompressionType.None, Guid.Empty, (byte[]) null);
              if (fileContainerItem2.Status != ContainerItemStatus.Created)
              {
                string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ResourceStrings.FatalErrorDeployingBuildTemplate((object) fileContainerItem2.Path, (object) uri));
                requestContext.Trace(TeamFoundationEventId.FatalErrorDeployingBuildTemplate, TraceLevel.Error, "Build", "Service", message);
                throw new TeamFoundationServerException(message);
              }
            }
            tfs = BuildContainerPath.Combine(BuildContainerPath.RootFolder, containerId.ToString((IFormatProvider) CultureInfo.InvariantCulture), path);
          }
          catch (Exception ex)
          {
            if (flag && containerId >= 0L)
              service.DeleteContainer(requestContext, containerId, Guid.Empty);
            throw;
          }
        }
      }
      return tfs;
    }

    public List<ProcessTemplate> QueryProcessTemplatesById(
      IVssRequestContext requestContext,
      IList<int> templateIds)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryProcessTemplatesById));
      ArgumentValidation.Check(nameof (templateIds), (object) templateIds, false);
      List<ProcessTemplate> processTemplateList = new List<ProcessTemplate>(templateIds.Count);
      Dictionary<int, ProcessTemplate> dictionary;
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        dictionary = component.QueryProcessTemplatesById((ICollection<int>) templateIds).GetCurrent<ProcessTemplate>().Items.ToDictionary<ProcessTemplate, int>((Func<ProcessTemplate, int>) (x => x.Id));
      for (int index = 0; index < templateIds.Count; ++index)
      {
        ProcessTemplate processTemplate;
        if (dictionary.TryGetValue(templateIds[index], out processTemplate))
        {
          if (this.m_buildHost.SecurityManager.HasProjectPermission(requestContext, processTemplate.TeamProjectObj, BuildPermissions.ViewBuildDefinition))
          {
            processTemplateList.Insert(index, processTemplate);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Read template '{0}'", (object) processTemplate.Id);
          }
          else
          {
            processTemplateList.Insert(index, (ProcessTemplate) null);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Access denied for template '{0}'", (object) templateIds[index]);
          }
        }
        else
        {
          processTemplateList.Insert(index, (ProcessTemplate) null);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Template '{0}' not found", (object) templateIds[index]);
        }
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (QueryProcessTemplatesById));
      return processTemplateList;
    }

    public void StopBuilds(IVssRequestContext requestContext, IList<string> uris, Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (StopBuilds));
      ArgumentValidation.CheckUriArray(nameof (uris), uris, "Build", false, (string) null);
      if (uris.Count == 1 && this.m_build2Converter != null && this.m_build2Converter.CancelBuild(requestContext, projectId, uris[0]) != null)
        return;
      List<BuildDetail> buildDetailList = new List<BuildDetail>();
      List<BuildController> buildControllerList = new List<BuildController>();
      TeamFoundationIdentity requestor = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      using (TeamFoundationDataReader foundationDataReader = this.QueryBuildsByUri(requestContext.Elevate(), uris, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.BatchedRequests, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
      {
        List<BuildDetail> list = foundationDataReader.Current<BuildQueryResult>().Builds.ToList<BuildDetail>();
        Dictionary<string, List<QueuedBuild>> dictionary = new Dictionary<string, List<QueuedBuild>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (QueuedBuild queuedBuild in foundationDataReader.Current<BuildQueryResult>().QueuedBuilds)
        {
          foreach (string buildUri in queuedBuild.BuildUris)
          {
            List<QueuedBuild> queuedBuildList;
            if (!dictionary.TryGetValue(buildUri, out queuedBuildList))
            {
              queuedBuildList = new List<QueuedBuild>();
              dictionary.Add(buildUri, queuedBuildList);
            }
            queuedBuildList.Add(queuedBuild);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added build '{0}' to list of build uri '{1}'", (object) queuedBuild.Id, (object) buildUri);
          }
        }
        for (int index = 0; index < list.Count; ++index)
        {
          if (list[index] == null || !this.m_projectService.GetTeamProjectFromGuidOrName(requestContext, list[index].TeamProject).MatchesScope(projectId) || !this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, list[index].Definition, BuildPermissions.ViewBuilds))
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Invalid build uri '{0}'", (object) uris[index]);
            throw new InvalidBuildUriException(uris[index]);
          }
          buildDetailList.Add(list[index]);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added build '{0}' to stop", (object) uris[index]);
          List<QueuedBuild> queuedBuildList;
          if (dictionary.TryGetValue(list[index].Uri, out queuedBuildList))
          {
            foreach (QueuedBuild queuedBuild in queuedBuildList)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking permission for queued build '{0}'", (object) queuedBuild.Id);
              if (queuedBuild.IsRequestor(requestor))
              {
                requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Skipped checking permission for queued build '{0}' because it belongs to the requestor", (object) queuedBuild.Id);
              }
              else
              {
                this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, queuedBuild.Definition, BuildPermissions.StopBuilds);
                break;
              }
            }
          }
        }
        buildControllerList.AddRange((IEnumerable<BuildController>) foundationDataReader.Current<BuildQueryResult>().Controllers);
      }
      buildControllerList.Sort((Comparison<BuildController>) ((x, y) => VssStringComparer.Url.Compare(x.Uri, y.Uri)));
      buildDetailList.Sort((Comparison<BuildDetail>) ((x, y) => VssStringComparer.Url.Compare(x.BuildControllerUri, y.BuildControllerUri)));
      int index1 = 0;
      foreach (BuildController buildController in buildControllerList)
      {
        List<BuildDetail> builds = new List<BuildDetail>();
        for (; index1 < buildDetailList.Count; ++index1)
        {
          BuildDetail buildDetail = buildDetailList[index1];
          if (!string.Equals(buildDetail.BuildControllerUri, buildController.Uri, StringComparison.OrdinalIgnoreCase))
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Done getting builds for controller '{0}'", (object) buildController.Uri);
            break;
          }
          if (BuildCommonUtil.IsDefaultDateTime(buildDetail.FinishTime))
          {
            builds.Add(buildDetail);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added build '{0}' to stop on controller '{1}'", (object) buildDetail.Uri, (object) buildController.Uri);
          }
        }
        if (builds.Count > 0)
          buildController.StopBuilds(requestContext.Elevate(), requestContext.UserContext, (IList<BuildDetail>) builds);
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (StopBuilds));
    }

    public List<BuildDefinition> UpdateBuildDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinition> updates)
    {
      return this.UpdateBuildDefinitions(requestContext, updates, true);
    }

    public List<BuildInformationNode> UpdateBuildInformation(
      IVssRequestContext requestContext,
      IList<InformationChangeRequest> changes)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateBuildInformation));
      Validation.CheckValidatable<InformationChangeRequest>(requestContext, nameof (changes), changes, false, ValidationContext.Update);
      Dictionary<string, InformationAddRequest> dictionary1 = new Dictionary<string, InformationAddRequest>();
      Dictionary<InformationChangeRequest, int> dictionary2 = new Dictionary<InformationChangeRequest, int>();
      Dictionary<string, List<InformationChangeRequest>> dictionary3 = new Dictionary<string, List<InformationChangeRequest>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < changes.Count; ++index)
      {
        dictionary2.Add(changes[index], index);
        List<InformationChangeRequest> informationChangeRequestList;
        if (!dictionary3.TryGetValue(changes[index].BuildUri, out informationChangeRequestList))
        {
          informationChangeRequestList = new List<InformationChangeRequest>();
          dictionary3.Add(changes[index].BuildUri, informationChangeRequestList);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Grouped requests by build '{0}'", (object) changes[index].BuildUri);
        }
        informationChangeRequestList.Add(changes[index]);
        if (changes[index] is InformationAddRequest change)
        {
          string key = change.BuildUri + "_" + (object) change.NodeId;
          if (!dictionary1.ContainsKey(key))
          {
            dictionary1.Add(key, change);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added add request '{0}'", (object) key);
          }
          else
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Duplicate information change request '{0}'", (object) key);
            throw new DuplicateInformationChangeRequestException(change.NodeId, change.BuildUri);
          }
        }
      }
      string[] array1 = dictionary3.Keys.ToArray<string>();
      Dictionary<string, Guid> dictionary4 = new Dictionary<string, Guid>();
      using (TeamFoundationDataReader foundationDataReader = this.QueryBuildsByUri(requestContext, (IList<string>) array1, (IList<string>) null, QueryOptions.Definitions, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
      {
        using (IEnumerator<BuildDetail> enumerator = foundationDataReader.Current<BuildQueryResult>().Builds.GetEnumerator())
        {
          for (int index = 0; index < array1.Length; ++index)
          {
            enumerator.MoveNext();
            BuildDetail current = enumerator.Current;
            if (current == null)
            {
              requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Build '{0}' not found", (object) array1[index]);
              throw new InvalidBuildUriException(array1[index]);
            }
            this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, current.Definition, BuildPermissions.UpdateBuildInformation);
            dictionary4.Add(current.Uri, current.Definition.TeamProject.Id);
          }
        }
      }
      string[] array2 = new string[dictionary1.Keys.Count];
      Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
      dictionary1.Keys.CopyTo(array2, 0);
      foreach (string key1 in array2)
      {
        InformationAddRequest informationAddRequest = dictionary1[key1];
        while (informationAddRequest != null)
        {
          string key2 = informationAddRequest.BuildUri + "_" + (object) informationAddRequest.ParentId;
          if (!dictionary5.ContainsKey(key2) && informationAddRequest.NodeId != informationAddRequest.ParentId)
          {
            string key3 = informationAddRequest.BuildUri + "_" + (object) informationAddRequest.NodeId;
            dictionary1[key3] = (InformationAddRequest) null;
            dictionary5.Add(key3, (object) null);
            if (!dictionary1.TryGetValue(key2, out informationAddRequest) || informationAddRequest == null)
            {
              informationAddRequest = (InformationAddRequest) null;
              dictionary5.Clear();
            }
          }
          else
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Information add request cycle: Key='{0}' ParentKey='{1}'", (object) key1, (object) key2);
            throw new InformationAddRequestCycleException(informationAddRequest.NodeId, informationAddRequest.BuildUri);
          }
        }
      }
      dictionary1.Clear();
      dictionary5.Clear();
      TeamFoundationIdentity requestedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      using (BuildInformationComponent component = requestContext.CreateComponent<BuildInformationComponent>("Build"))
      {
        BuildInformationNode[] source = new BuildInformationNode[changes.Count];
        using (Dictionary<string, List<InformationChangeRequest>>.Enumerator enumerator = dictionary3.GetEnumerator())
        {
label_48:
          while (enumerator.MoveNext())
          {
            KeyValuePair<string, List<InformationChangeRequest>> current1 = enumerator.Current;
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Updating build information for build '{0}'", (object) current1.Key);
            List<InformationChangeRequest> changes1 = current1.Value;
            using (ResultCollection resultCollection = component.UpdateBuildInformation(dictionary4[current1.Key], current1.Key, (ICollection<InformationChangeRequest>) changes1, requestedBy))
            {
              ObjectBinder<BuildInformationRow> current2 = component.ServiceVersion == ServiceVersion.V1 ? resultCollection.GetCurrent<BuildInformationRow>() : (ObjectBinder<BuildInformationRow>) null;
              ObjectBinder<BuildInformationNode> current3 = component.ServiceVersion == ServiceVersion.V1 ? (ObjectBinder<BuildInformationNode>) null : resultCollection.GetCurrent<BuildInformationNode>();
              bool hasMoreData = component.ServiceVersion == ServiceVersion.V1 ? current2.MoveNext() : current3.MoveNext();
              int index1 = 0;
              while (true)
              {
                if (hasMoreData)
                {
                  if (index1 < changes1.Count)
                  {
                    int index2 = dictionary2[changes1[index1]];
                    if (changes1[index1] is InformationDeleteRequest)
                    {
                      source[index2] = (BuildInformationNode) null;
                      requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Build information node index '{0}' deleted", (object) index2);
                    }
                    else
                    {
                      if (component.ServiceVersion == ServiceVersion.V1)
                      {
                        source[index2] = BuildInformationMerger.ReadNode(current2, ref hasMoreData);
                      }
                      else
                      {
                        source[index2] = current3.Current;
                        hasMoreData = current3.MoveNext();
                      }
                      requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Build information node index '{0}' updated with node Id '{1}'", (object) index2, (object) source[index2].NodeId);
                    }
                    ++index1;
                  }
                  else
                    goto label_48;
                }
                else
                  goto label_48;
              }
            }
          }
        }
        requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateBuildInformation));
        return ((IEnumerable<BuildInformationNode>) source).ToList<BuildInformationNode>();
      }
    }

    public BuildQueueQueryResult UpdateQueuedBuilds(
      IVssRequestContext requestContext,
      IList<QueuedBuildUpdateOptions> updates,
      Guid projectId = default (Guid))
    {
      return this.UpdateQueuedBuilds(requestContext, updates, true, projectId);
    }

    public List<BuildDetail> UpdateBuilds(
      IVssRequestContext requestContext,
      IList<BuildUpdateOptions> updateOptions,
      Guid projectId = default (Guid))
    {
      return this.UpdateBuilds(requestContext, updateOptions, false, projectId);
    }

    internal List<BuildDetail> UpdateBuilds(
      IVssRequestContext requestContext,
      IList<BuildUpdateOptions> updateOptions,
      bool serverInitiated,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateBuilds));
      HashSet<string> stringSet = new HashSet<string>();
      string[] uris1 = new string[updateOptions.Count];
      for (int index = 0; index < updateOptions.Count; ++index)
      {
        if (stringSet.Contains(updateOptions[index].Uri))
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Duplicate build '{0}'", (object) updateOptions[index].Uri);
          throw new DuplicateBuildUpdateRequestException(updateOptions[index].Uri);
        }
        stringSet.Add(updateOptions[index].Uri);
        uris1[index] = updateOptions[index].Uri;
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added build '{0}' for update", (object) uris1[index]);
      }
      BuildDetailDictionary detailDictionary = new BuildDetailDictionary();
      using (TeamFoundationDataReader foundationDataReader = this.QueryBuildsByUri(requestContext.Elevate(), (IList<string>) uris1, (IList<string>) null, QueryOptions.Definitions, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
      {
        int index1 = 0;
        StreamingCollection<BuildDetail> builds = foundationDataReader.Current<BuildQueryResult>().Builds;
        List<BuildDetail> buildDetailList = new List<BuildDetail>();
        foreach (BuildDetail buildDetail in builds)
        {
          if (requestContext.IsUserContext && buildDetail != null && BuildContainerPath.IsServerPath(buildDetail.DropLocationRoot))
            updateOptions[index1].Fields &= ~BuildUpdate.DropLocation;
          buildDetailList.Add(buildDetail);
          ++index1;
        }
        Validation.CheckValidatable<BuildUpdateOptions>(requestContext, nameof (updateOptions), updateOptions, false, ValidationContext.Update);
        int index2 = 0;
        foreach (BuildDetail buildDetail in buildDetailList)
        {
          if (buildDetail == null || !this.m_projectService.GetTeamProjectFromGuidOrName(requestContext, buildDetail.TeamProject).MatchesScope(projectId) || !this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.ViewBuilds))
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Invalid build uri '{0}'", (object) uris1[index2]);
            throw new InvalidBuildUriException(uris1[index2]);
          }
          if (updateOptions[index2].Fields == BuildUpdate.Quality)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking permissions to update build qualities for build '{0}'", (object) updateOptions[index2].Uri);
            if (!this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.UpdateBuildInformation))
              this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.EditBuildQuality);
          }
          else if (updateOptions[index2].Fields == BuildUpdate.KeepForever)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking permissions to update build retention policies for build '{0}'", (object) updateOptions[index2].Uri);
            if (!this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.UpdateBuildInformation))
              this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.RetainIndefinitely);
          }
          else
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Checking permissions to update for build '{0}'", (object) updateOptions[index2].Uri);
            this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.UpdateBuildInformation);
          }
          if ((updateOptions[index2].Fields & BuildUpdate.DropLocation) == BuildUpdate.DropLocation)
          {
            string dropLocation = updateOptions[index2].DropLocation;
            if (VersionControlPath.IsServerItem(dropLocation) && (string.IsNullOrEmpty(buildDetail.DropLocationRoot) || !VersionControlPath.IsServerItem(buildDetail.DropLocationRoot) || !VersionControlPath.IsSubItem(dropLocation, buildDetail.DropLocationRoot)))
              throw new ArgumentException(ResourceStrings.InvalidVersionControlDropLocation((object) dropLocation, (object) buildDetail.DropLocationRoot));
            if (BuildContainerPath.IsServerPath(dropLocation) && (!BuildContainerPath.IsServerPath(buildDetail.DropLocationRoot) || !BuildContainerPath.IsSubItem(dropLocation, buildDetail.DropLocationRoot)))
              throw new ArgumentException(ResourceStrings.InvalidBuildContainerDropLocation((object) dropLocation, (object) buildDetail.DropLocationRoot));
          }
          updateOptions[index2].ProjectId = buildDetail.Definition.TeamProject.Id;
          ++index2;
          detailDictionary.Add(buildDetail);
        }
      }
      TeamFoundationIdentity requestedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      List<BuildUpdateOptions> items1;
      List<BuildDetail> items2;
      List<BuildDefinition> items3;
      List<StartBuildData> items4;
      List<AgentReservationData> items5;
      List<SharedResourceData> items6;
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
      {
        ResultCollection resultCollection = component.UpdateBuilds((ICollection<BuildUpdateOptions>) updateOptions, requestedBy, this.WriterId);
        items1 = resultCollection.GetCurrent<BuildUpdateOptions>().Items;
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<BuildDetail>().Items;
        resultCollection.NextResult();
        items3 = resultCollection.GetCurrent<BuildDefinition>().Items;
        resultCollection.NextResult();
        items4 = resultCollection.GetCurrent<StartBuildData>().Items;
        resultCollection.NextResult();
        items5 = resultCollection.GetCurrent<AgentReservationData>().Items;
        resultCollection.NextResult();
        items6 = resultCollection.GetCurrent<SharedResourceData>().Items;
      }
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) items4);
      BuildController.NotifyAgentsAvailable(requestContext, (IEnumerable<AgentReservationData>) items5);
      TeamFoundationBuildResourceService.NotifySharedResourcesAvailable(requestContext, (IEnumerable<SharedResourceData>) items6);
      Dictionary<string, BuildDefinition> dictionary1 = items3.ToDictionary<BuildDefinition, string>((Func<BuildDefinition, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, BuildUpdateOptions> dictionary2 = items1.ToDictionary<BuildUpdateOptions, string>((Func<BuildUpdateOptions, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> uris2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      foreach (BuildDetail buildDetail in items2)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Processing build '{0}' post-update", (object) buildDetail.Uri);
        BuildUpdateOptions buildUpdateOptions;
        if (dictionary2.TryGetValue(buildDetail.Uri, out buildUpdateOptions))
        {
          buildDetail.Definition = dictionary1[buildDetail.BuildDefinitionUri];
          buildDetail.TeamProject = buildDetail.Definition.TeamProject.Name;
          if ((buildUpdateOptions.Fields & BuildUpdate.FinishTime) == BuildUpdate.FinishTime)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Build '{0}' has updated its finish time", (object) buildDetail.Uri);
            uris2.Add(buildDetail.BuildDefinitionUri);
            using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
            {
              foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
              {
                try
                {
                  buildQueueExtension.BuildFinished(requestContext, buildDetail, serverInitiated);
                }
                catch (Exception ex)
                {
                  requestContext.TraceException(0, "Build", "Service", ex);
                }
              }
            }
            TeamFoundationBuildService.PublishBuildFinished(requestContext, service, buildDetail);
          }
          else if ((buildUpdateOptions.Fields & BuildUpdate.StartTime) == BuildUpdate.StartTime)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Build '{0}' has started", (object) buildDetail.Uri);
            TeamFoundationBuildService.PublishBuildStarted(requestContext, service, buildDetail);
          }
          if ((buildUpdateOptions.Fields & BuildUpdate.Quality) == BuildUpdate.Quality)
          {
            service.PublishNotification(requestContext, (object) new BuildQualityChangedNotificationEvent(buildDetail, detailDictionary[buildDetail.Uri].Quality));
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Publishing notification for build '{0}'", (object) buildDetail.Uri);
          }
        }
      }
      if (uris2.Count != 0)
        this.m_buildDefinitionCache.Invalidate(requestContext, (IEnumerable<string>) uris2);
      requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateBuilds));
      return items2;
    }

    private static void PublishBuildFinished(
      IVssRequestContext requestContext,
      TeamFoundationEventService eventManager,
      BuildDetail build)
    {
      if (build.Reason == BuildReason.BatchedCI)
        build.Definition.ScheduleRollingJob(requestContext);
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Publishing notification for build '{0}'", (object) build.Uri);
      eventManager.PublishNotification(requestContext, (object) new BuildCompletionNotificationEvent(build));
    }

    private static void PublishBuildStarted(
      IVssRequestContext requestContext,
      TeamFoundationEventService eventManager,
      BuildDetail build)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Publishing notification for build '{0}'", (object) build.Uri);
      eventManager.PublishNotification(requestContext, (object) new BuildStartedNotificationEvent(build));
    }

    public List<ProcessTemplate> UpdateProcessTemplates(
      IVssRequestContext requestContext,
      IList<ProcessTemplate> processTemplates)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateProcessTemplates));
      Validation.CheckValidatable<ProcessTemplate>(requestContext, nameof (processTemplates), processTemplates, false, ValidationContext.Update);
      foreach (ProcessTemplate processTemplate in (IEnumerable<ProcessTemplate>) processTemplates)
      {
        this.m_buildHost.SecurityManager.CheckProjectPermission(requestContext, processTemplate.TeamProjectObj, BuildPermissions.EditBuildDefinition);
        processTemplate.UpdateCachedProcessParameters(requestContext, (VersionSpec) new LatestVersionSpec());
      }
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
      {
        ResultCollection resultCollection = component.UpdateProcessTemplates((ICollection<ProcessTemplate>) processTemplates, true);
        requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateProcessTemplates));
        return resultCollection.GetCurrent<ProcessTemplate>().Items;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void StopBuildRequest(
      IVssRequestContext requestContext,
      QueuedBuild request,
      string errorMessage,
      out BuildDetail build)
    {
      requestContext.TraceEnter(0, "Build", "Service", "KillBuildRequest");
      build = (BuildDetail) null;
      if (!requestContext.IsSystemContext)
        return;
      TeamFoundationIdentity requestedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      QueuedBuild queuedBuild = (QueuedBuild) null;
      BuildDefinition buildDefinition = (BuildDefinition) null;
      List<StartBuildData> startBuildData = (List<StartBuildData>) null;
      using (BuildQueueComponent component = requestContext.CreateComponent<BuildQueueComponent>("Build"))
      {
        ResultCollection resultCollection = component.StopBuildRequest(request.Definition.TeamProject.Id, request.Id, requestedBy, this.WriterId, errorMessage);
        if (resultCollection != null)
        {
          queuedBuild = resultCollection.GetCurrent<QueuedBuild>().Items.FirstOrDefault<QueuedBuild>();
          resultCollection.NextResult();
          build = resultCollection.GetCurrent<BuildDetail>().Items.FirstOrDefault<BuildDetail>();
          resultCollection.NextResult();
          buildDefinition = resultCollection.GetCurrent<BuildDefinition>().Items.FirstOrDefault<BuildDefinition>();
          resultCollection.NextResult();
          startBuildData = resultCollection.GetCurrent<StartBuildData>().Items;
        }
      }
      if (startBuildData != null)
        BuildController.StartBuilds(requestContext, (IList<StartBuildData>) startBuildData);
      if (queuedBuild == null || build == null)
        return;
      request.BuildUris.Add(build.Uri);
      build.Definition = buildDefinition;
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      TeamFoundationBuildService.PublishBuildFinished(requestContext, service, build);
    }

    internal Guid WriterId { get; set; }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Build", "Service", "ServiceStart");
      this.m_buildHost = systemRequestContext.GetService<TeamFoundationBuildHost>();
      this.m_projectService = systemRequestContext.GetService<IProjectService>();
      this.m_definitionArtifactKind = this.m_buildHost.GetBuildDefinitionArtifactKind(systemRequestContext);
      this.m_buildDefinitionCache = new BuildDefinitionCache(this, systemRequestContext);
      TeamFoundationSqlNotificationService service = systemRequestContext.GetService<TeamFoundationSqlNotificationService>();
      service.RegisterNotification(systemRequestContext, "Build", BuildSqlNotificationEventClasses.BuildDefinitionsChanged, new SqlNotificationCallback(this.OnBuildDefinitionsChanged), true);
      this.WriterId = service.Author;
      this.m_build2Converter = systemRequestContext.GetExtension<IBuild2Converter>();
      systemRequestContext.TraceLeave(0, "Build", "Service", "ServiceStart");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Build", "Service", "ServiceEnd");
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Build", BuildSqlNotificationEventClasses.BuildDefinitionsChanged, new SqlNotificationCallback(this.OnBuildDefinitionsChanged), false);
      if (this.m_buildDefinitionCache != null)
      {
        this.m_buildDefinitionCache.Unload(systemRequestContext);
        this.m_buildDefinitionCache = (BuildDefinitionCache) null;
      }
      if (this.m_build2Converter != null)
        this.m_build2Converter = (IBuild2Converter) null;
      systemRequestContext.TraceLeave(0, "Build", "Service", "ServiceEnd");
    }

    internal TeamFoundationBuildHost BuildHost => this.m_buildHost;

    private void OnBuildDefinitionsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (OnBuildDefinitionsChanged));
      IEnumerable<string> uris = (IEnumerable<string>) null;
      if (!string.IsNullOrEmpty(eventData))
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Deserializing event data '{0}'", (object) eventData);
        try
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          using (StringReader input = new StringReader(eventData))
          {
            using (XmlReader xmlReader = XmlReader.Create((TextReader) input, settings))
              uris = ((IEnumerable<string>) (string[]) this.m_stringArraySerializer.Deserialize(xmlReader)).Select<string, string>((Func<string, string>) (x => DBHelper.CreateArtifactUri("Definition", x)));
          }
        }
        catch (Exception ex)
        {
          uris = (IEnumerable<string>) null;
          requestContext.TraceException(0, "Build", "Service", ex);
        }
      }
      this.m_buildDefinitionCache.Invalidate(requestContext.Elevate(), uris);
      requestContext.TraceLeave(0, "Build", "Service", nameof (OnBuildDefinitionsChanged));
    }

    internal Dictionary<string, BuildDefinition> GetAffectedBuildDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<string> serverItems,
      DefinitionTriggerType continuousIntegrationType,
      Func<BuildDefinition, bool> ignoreDefinition,
      bool readAllItems)
    {
      return this.m_buildDefinitionCache.GetAffectedBuildDefinitions(requestContext, serverItems, continuousIntegrationType, ignoreDefinition, readAllItems);
    }

    internal List<ProcessTemplate> QueryProcessTemplates(
      IVssRequestContext requestContext,
      string teamProject,
      IList<ProcessTemplateType> queryTypes,
      bool includeDeleted)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryProcessTemplates));
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
      {
        ResultCollection resultCollection = component.QueryProcessTemplates(requestContext, teamProject, (IEnumerable<ProcessTemplateType>) queryTypes, includeDeleted);
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryProcessTemplates));
        return resultCollection.GetCurrent<ProcessTemplate>().Items;
      }
    }

    internal List<ProcessTemplate> QueryProcessTemplatesByPath(
      IVssRequestContext requestContext,
      string teamProject,
      IList<string> paths,
      bool includeDeleted,
      bool recursive = false)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryProcessTemplatesByPath));
      List<string> list = paths.Select<string, string>((Func<string, string>) (s => Microsoft.TeamFoundation.Framework.Server.DBPath.UserToDatabasePath(s, true, true))).ToList<string>();
      List<ProcessTemplate> templates;
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        templates = component.QueryProcessTemplatesByPath(requestContext, teamProject, (ICollection<string>) list, includeDeleted, recursive);
      if (recursive)
      {
        for (int i = templates.Count - 1; i >= 0; i--)
        {
          if (!paths.Any<string>((Func<string, bool>) (x => templates[i].ServerPath.StartsWith(x, StringComparison.OrdinalIgnoreCase))))
            templates.RemoveAt(i);
        }
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (QueryProcessTemplatesByPath));
      return templates;
    }

    internal TeamFoundationDataReader QueryChangedBuilds(
      IVssRequestContext requestContext,
      IList<string> teamProjects,
      DateTime minChangedTime,
      BuildStatus statusFilter,
      IList<string> informationTypes,
      int batchSize = 2147483647)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryChangedBuilds));
      CommandQueryChangedBuilds disposableObject = (CommandQueryChangedBuilds) null;
      try
      {
        minChangedTime = minChangedTime.ToUniversalTime();
        if (minChangedTime < DBHelper.MinAllowedDateTime)
          minChangedTime = DBHelper.MinAllowedDateTime;
        disposableObject = new CommandQueryChangedBuilds(requestContext, Guid.Empty);
        disposableObject.Execute(teamProjects, minChangedTime, statusFilter, informationTypes, batchSize);
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryChangedBuilds));
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.QueryResult
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Service", ex);
        disposableObject?.Dispose();
        throw;
      }
    }

    internal void StartPendingBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      string buildDefinitionUri,
      DefinitionTriggerType continuousIntegrationType)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (StartPendingBuild));
      ArgumentValidation.CheckUri(nameof (buildDefinitionUri), buildDefinitionUri, "Definition", false, (string) null);
      int latestChangeset = requestContext.GetService<TeamFoundationVersionControlService>().GetLatestChangeset(requestContext);
      requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Got latest changeset '{0}'", (object) latestChangeset);
      TeamFoundationIdentity buildOwner = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      List<QueuedBuild> items1;
      BuildDefinitionDictionary targetDictionary;
      List<StartBuildData> items2;
      using (BuildQueueComponent component = requestContext.CreateComponent<BuildQueueComponent>("Build"))
      {
        ResultCollection resultCollection = component.StartPendingBuilds(projectId, buildDefinitionUri, continuousIntegrationType, buildOwner, new ChangesetVersionSpec(latestChangeset).ToDBString(requestContext));
        items1 = resultCollection.GetCurrent<QueuedBuild>().Items;
        resultCollection.NextResult();
        targetDictionary = new BuildDefinitionDictionary((IEnumerable<BuildDefinition>) resultCollection.GetCurrent<BuildDefinition>().Items);
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<StartBuildData>().Items;
      }
      if (items1.Count > 0)
      {
        DBHelper.Match<BuildDefinition, QueuedBuild, string>((Dictionary<string, BuildDefinition>) targetDictionary, (IEnumerable<QueuedBuild>) items1, (Func<QueuedBuild, string>) (x => x.BuildDefinitionUri), (Action<BuildDefinition, QueuedBuild>) ((x, y) =>
        {
          y.Definition = x;
          y.ProjectId = x.TeamProject.Id;
        }));
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.BuildsQueued(requestContext, (IList<QueuedBuild>) items1, false);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
        TeamFoundationBuildService.PublishBuildsQueued(requestContext, items1.ToArray());
      }
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) items2);
      requestContext.TraceLeave(0, "Build", "Service", nameof (StartPendingBuild));
    }

    internal List<BuildDefinition> UpdateBuildDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinition> updates,
      bool currentClient)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateBuildDefinitions));
      Validation.CheckValidatable<BuildDefinition>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      List<string> uris = new List<string>();
      HashSet<string> serverDropControllerUris = new HashSet<string>();
      HashSet<string> gitCustomTemplatecontrollerUris = new HashSet<string>();
      foreach (BuildDefinition update in (IEnumerable<BuildDefinition>) updates)
      {
        if (BuildContainerPath.IsServerPath(update.DefaultDropLocation))
          serverDropControllerUris.Add(update.BuildControllerUri);
        if (update.Process != null && BuildSourceProviders.GitProperties.IsGitUri(update.Process.ServerPath))
          gitCustomTemplatecontrollerUris.Add(update.BuildControllerUri);
        uris.Add(update.Uri);
        if (TeamFoundationBuildService.IsGitTeamProject(requestContext, update.TeamProject.Uri))
        {
          foreach (BuildDefinitionSourceProvider sourceProvider in update.SourceProviders)
            this.UpdateGitRepositoryUrl(requestContext, sourceProvider);
        }
        this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, update, BuildPermissions.EditBuildDefinition);
      }
      updates.ConvertObjectsToProjectGuid(requestContext);
      TeamFoundationBuildService.CheckDefinitionAndControllerCompatibility(requestContext, updates, serverDropControllerUris, gitCustomTemplatecontrollerUris);
      List<BuildDefinition> buildDefinitionList = new List<BuildDefinition>();
      using (requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, "tfs://Build/Definition/Update"))
      {
        TeamFoundationJobService service1 = requestContext.Elevate().GetService<TeamFoundationJobService>();
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Acquired lock '{0}' in exclusive mode", (object) "tfs://Build/Definition/Update");
        BuildDefinitionQueryResult definitionQueryResult = this.QueryBuildDefinitionsByUri(requestContext.Elevate(), (IList<string>) uris, (IList<string>) BuildConstants.AllPropertyNames, QueryOptions.None, new Guid());
        BuildDefinitionDictionary definitionDictionary = new BuildDefinitionDictionary();
        foreach (BuildDefinition definition in definitionQueryResult.Definitions)
        {
          if (definition != null)
            definitionDictionary[definition.Uri] = definition;
        }
        TeamFoundationEventService service2 = requestContext.GetService<TeamFoundationEventService>();
        foreach (BuildDefinition update in (IEnumerable<BuildDefinition>) updates)
        {
          BuildDefinition oldDefinition;
          if (definitionDictionary.TryGetValue(update.Uri, out oldDefinition))
          {
            if (!currentClient)
            {
              if (oldDefinition.TriggerType == DefinitionTriggerType.BatchedGatedCheckIn)
              {
                requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Cannot update batched definition '{0}' from old client", (object) oldDefinition.FullPath);
                throw new NotSupportedException(ResourceStrings.CannotUpdateBatchedDefinitionFromOldClient((object) oldDefinition.FullPath));
              }
              if (update.QueueStatus == DefinitionQueueStatus.Enabled && oldDefinition.QueueStatus == DefinitionQueueStatus.Paused)
              {
                update.QueueStatus = DefinitionQueueStatus.Paused;
                requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Set status of definition '{0}' to 'Paused'", (object) update.Uri);
              }
            }
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Publishing decision point for definition '{0}'", (object) update.Uri);
            service2.PublishDecisionPoint(requestContext, (object) new BuildDefinitionChangingEvent(ChangedType.Updated, oldDefinition, update));
          }
          else
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Not found definition '{0}' for publishing decision point", (object) update.Uri);
        }
        TeamFoundationIdentity requestedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
        TeamFoundationBuildService.BuildDefinitionDatabaseResult dbResults;
        IList<StartBuildData> items;
        using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        {
          ResultCollection dbResult = component.UpdateBuildDefinitions(updates, requestedBy, this.WriterId, requestContext.IsServicingContext);
          dbResults = TeamFoundationBuildService.ReadDatabaseResults(dbResult);
          dbResult.NextResult();
          items = (IList<StartBuildData>) dbResult.GetCurrent<StartBuildData>().Items;
        }
        this.PostProcessDatabaseResults(requestContext, dbResults);
        buildDefinitionList.AddRange((IEnumerable<BuildDefinition>) dbResults.Definitions);
        BuildController.StartBuilds(requestContext, items);
        this.m_buildDefinitionCache.Invalidate(requestContext, (IEnumerable<string>) uris);
        List<Guid> jobsToDelete = new List<Guid>();
        List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          IEnumerator<BuildDefinition> enumerator = updates.GetEnumerator();
          DefinitionTriggerType definitionTriggerType = DefinitionTriggerType.BatchedContinuousIntegration | DefinitionTriggerType.Schedule | DefinitionTriggerType.ScheduleForced;
          foreach (BuildDefinition buildDefinition in buildDefinitionList)
          {
            if (!enumerator.MoveNext())
              requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Result out of sync. Updated definition '{0}'", (object) buildDefinition.Uri);
            BuildDefinition oldDefinition;
            if (definitionDictionary.TryGetValue(buildDefinition.Uri, out oldDefinition))
            {
              if (oldDefinition.QueueStatus != buildDefinition.QueueStatus)
              {
                foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
                {
                  try
                  {
                    buildQueueExtension.QueueStatusChanged(requestContext.Elevate(), buildDefinition);
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(0, "Build", "Service", ex);
                  }
                }
              }
              buildDefinition.CopyPropertiesFrom(enumerator.Current);
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Publishing notification for definition '{0}'", (object) buildDefinition.Uri);
              service2.PublishNotification(requestContext, (object) new BuildDefinitionChangedEvent(requestContext, oldDefinition, buildDefinition));
              if ((buildDefinition.TriggerType & definitionTriggerType) == (DefinitionTriggerType) 0 && (oldDefinition.TriggerType & definitionTriggerType) == (DefinitionTriggerType) 0)
              {
                requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Skipped updating jobs for definition '{0}'", (object) buildDefinition.Uri);
              }
              else
              {
                TeamFoundationJobDefinition scheduleJobDefinition = buildDefinition.GetScheduleJobDefinition(service1.IsIgnoreDormancyPermitted);
                if (scheduleJobDefinition != null)
                {
                  jobUpdates.Add(scheduleJobDefinition);
                  requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added job '{0}' for updating", (object) scheduleJobDefinition.JobId);
                }
                else
                {
                  jobsToDelete.Add(buildDefinition.ScheduleJobId);
                  requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added job '{0}' for deleting", (object) buildDefinition.ScheduleJobId);
                }
              }
            }
            else
              requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Not found definition '{0}' for publishing notification", (object) buildDefinition.Uri);
          }
        }
        if (jobsToDelete.Count > 0 || jobUpdates.Count > 0)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Updating job definitions");
          service1.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
        }
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Releasing lock '{0}'", (object) "tfs://Build/Definition/Update");
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateBuildDefinitions));
      return buildDefinitionList;
    }

    private void MatchBuildServices(
      IVssRequestContext requestContext,
      QueryOptions options,
      TeamFoundationBuildService.BuildDefinitionDatabaseResult results,
      IList<BuildAgent> buildAgents,
      IList<BuildController> buildControllers,
      IList<BuildServiceHost> buildServiceHosts)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (MatchBuildServices));
      Dictionary<string, BuildServiceHost> dictionary = new Dictionary<string, BuildServiceHost>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if ((options & QueryOptions.Controllers) == QueryOptions.Controllers)
      {
        foreach (BuildController controller in results.Controllers)
        {
          dictionary[controller.ServiceHostUri] = (BuildServiceHost) null;
          buildControllers.Add(controller);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Read controller '{0}'", (object) controller.Uri);
        }
      }
      if ((options & QueryOptions.Agents) == QueryOptions.Agents)
      {
        foreach (BuildAgent agent in results.Agents)
        {
          dictionary[agent.ServiceHostUri] = (BuildServiceHost) null;
          buildAgents.Add(agent);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Read agent '{0}'", (object) agent.Uri);
        }
      }
      foreach (BuildServiceHost serviceHost in results.ServiceHosts)
      {
        if (dictionary.ContainsKey(serviceHost.Uri))
        {
          buildServiceHosts.Add(serviceHost);
          dictionary[serviceHost.Uri] = serviceHost;
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Read service host '{0}'", (object) serviceHost.Uri);
        }
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (MatchBuildServices));
    }

    internal BuildQueueQueryResult UpdateQueuedBuilds(
      IVssRequestContext requestContext,
      IList<QueuedBuildUpdateOptions> updates,
      bool resetQueueTime,
      Guid projectId = default (Guid))
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateQueuedBuilds));
      Validation.CheckValidatable<QueuedBuildUpdateOptions>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      List<int> ids = new List<int>();
      foreach (QueuedBuildUpdateOptions update in (IEnumerable<QueuedBuildUpdateOptions>) updates)
      {
        ids.Add(update.QueueId);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Added queued build '{0}' for update", (object) update.QueueId);
      }
      BuildDefinitionDictionary definitionDictionary = new BuildDefinitionDictionary();
      Dictionary<int, bool> resumeOrRequeueUpdates = new Dictionary<int, bool>();
      using (TeamFoundationDataReader foundationDataReader = this.QueryQueuedBuildsById(requestContext.Elevate(), (IList<int>) ids, (IList<string>) Array.Empty<string>(), QueryOptions.Definitions, projectId))
      {
        BuildQueueQueryResult queueQueryResult = foundationDataReader.Current<BuildQueueQueryResult>();
        foreach (BuildDefinition definition in queueQueryResult.Definitions)
          definitionDictionary.Add(definition);
        TeamFoundationIdentity requestor = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
        for (int index = 0; index < updates.Count; ++index)
        {
          QueuedBuild queuedBuild = (QueuedBuild) null;
          if (queueQueryResult.QueuedBuilds.MoveNext())
            queuedBuild = queueQueryResult.QueuedBuilds.Current;
          if (queuedBuild == null)
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Queued build '{0}' not found", (object) ids[index]);
            throw new QueuedBuildDoesNotExistException(ids[index]);
          }
          bool flag = queuedBuild.IsRequestor(requestor);
          BuildDefinition buildDefinition;
          if (!definitionDictionary.TryGetValue(queuedBuild.BuildDefinitionUri, out buildDefinition))
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Build definition '{0}' not found", (object) queuedBuild.BuildDefinitionUri);
            throw new BuildDefinitionDoesNotExistException(queuedBuild.BuildDefinitionUri);
          }
          resumeOrRequeueUpdates[queuedBuild.Id] = updates[index].Fields.HasFlag((Enum) QueuedBuildUpdate.Requeue) || updates[index].Fields.HasFlag((Enum) QueuedBuildUpdate.Postponed) && !updates[index].Postponed;
          if (updates[index].Fields.Equals((object) QueuedBuildUpdate.Postponed) & flag)
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Skipped checking permission for queued build '{0}' because it belongs to the requestor", (object) updates[index].QueueId);
            updates[index].ProjectId = buildDefinition.TeamProject.Id;
          }
          else
          {
            if (updates[index].Fields.HasFlag((Enum) QueuedBuildUpdate.Requeue) || updates[index].Fields.HasFlag((Enum) QueuedBuildUpdate.Retry))
            {
              this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, buildDefinition, BuildPermissions.QueueBuilds);
              if (updates[index].Fields.HasFlag((Enum) QueuedBuildUpdate.Postponed) && !flag || updates[index].Fields.HasFlag((Enum) QueuedBuildUpdate.Priority))
                this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, buildDefinition, BuildPermissions.ManageBuildQueue);
            }
            else
              this.m_buildHost.SecurityManager.CheckBuildPermission(requestContext, buildDefinition, BuildPermissions.ManageBuildQueue);
            updates[index].ProjectId = buildDefinition.TeamProject.Id;
          }
        }
      }
      List<QueuedBuild> items1;
      BuildDetailDictionary allBuilds;
      List<StartBuildData> items2;
      using (BuildQueueComponent component = requestContext.CreateComponent<BuildQueueComponent>("Build"))
      {
        ResultCollection resultCollection = component.UpdateBuilds(updates, resetQueueTime);
        items1 = resultCollection.GetCurrent<QueuedBuild>().Items;
        resultCollection.NextResult();
        allBuilds = new BuildDetailDictionary((IEnumerable<BuildDetail>) resultCollection.GetCurrent<BuildDetail>().Items);
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<StartBuildData>().Items;
      }
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) items2);
      BuildQueueQueryResult queueQueryResult1 = new BuildQueueQueryResult();
      HashSet<string> includedBuilds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> includedDefinitions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (QueuedBuild queuedBuild in items1)
      {
        BuildDefinition definition;
        if (definitionDictionary.TryGetValue(queuedBuild.BuildDefinitionUri, out definition))
        {
          this.MatchBuildsToQueuedBuilds(requestContext, QueryOptions.Definitions, queueQueryResult1.Builds, (IList<BuildDefinition>) queueQueryResult1.Definitions, (IDictionary<string, BuildDetail>) allBuilds, includedBuilds, includedDefinitions, definition, queuedBuild);
          queueQueryResult1.QueuedBuilds.Enqueue(queuedBuild);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Updated queued build '{0}'", (object) queuedBuild.Id);
        }
        else
          requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Definition '{0}' not found", (object) queuedBuild.BuildDefinitionUri);
      }
      List<QueuedBuild> list = items1.Where<QueuedBuild>((Func<QueuedBuild, bool>) (x => resumeOrRequeueUpdates[x.Id])).ToList<QueuedBuild>();
      if (list.Count > 0)
      {
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.BuildsQueued(requestContext.Elevate(), (IList<QueuedBuild>) list, false);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
        TeamFoundationBuildService.PublishBuildsQueued(requestContext, list.ToArray());
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateQueuedBuilds));
      return queueQueryResult1;
    }

    internal void EnsureBuildGroups(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (EnsureBuildGroups));
      List<string> stringList = new List<string>();
      using (requestContext.Elevate().GetService<ITeamFoundationLockingService>().AcquireLock(requestContext.Elevate(), TeamFoundationLockMode.Exclusive, "tfs://Build/Groups"))
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Acquired lock '{0}' in exclusive mode", (object) "tfs://Build/Groups");
        using (BuildComponent component = requestContext.Elevate().CreateComponent<BuildComponent>("Build"))
          stringList.AddRange(component.GetBuildGroups());
        CommonStructureService service = requestContext.Elevate().GetService<CommonStructureService>();
        foreach (string projectUri in stringList)
        {
          try
          {
            service.GetProject(requestContext.Elevate(), projectUri);
          }
          catch (ProjectDoesNotExistException ex)
          {
            requestContext.TraceException(0, "Build", "Service", (Exception) ex);
            using (BuildComponent component = requestContext.Elevate().CreateComponent<BuildComponent>("Build"))
              component.DeleteTeamProject(projectUri, this.WriterId, this.m_definitionArtifactKind);
          }
        }
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Releasing lock '{0}'", (object) "tfs://Build/Groups");
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (EnsureBuildGroups));
    }

    internal void RemoveDeletedTeamProject(IVssRequestContext requestContext, string projectUri)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (RemoveDeletedTeamProject));
      ArgumentValidation.CheckUri(nameof (projectUri), projectUri, false, (string) null);
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
        component.DeleteTeamProject(projectUri, this.WriterId, this.m_definitionArtifactKind);
      requestContext.TraceLeave(0, "Build", "Service", nameof (RemoveDeletedTeamProject));
    }

    private void UpdateGitRepositoryUrl(
      IVssRequestContext requestContext,
      BuildDefinitionSourceProvider source)
    {
      foreach (NameValueField field in source.Fields)
      {
        if (field.Name.Equals(BuildSourceProviders.GitProperties.RepositoryUrl))
        {
          Uri result;
          field.Value = Uri.TryCreate(field.Value, UriKind.Absolute, out result) ? result.AbsoluteUri : throw new InvalidGitRepoUriException(field.Value);
          break;
        }
      }
    }

    public static bool IsGitTeamProject(IVssRequestContext requestContext, string teamProjectUri)
    {
      bool flag = false;
      string a;
      if (requestContext.GetService<CommonStructureService>().QueryProjectCatalogNode(requestContext, teamProjectUri).Resource.Properties.TryGetValue("SourceControlCapabilityFlags", out a) && string.Equals(a, "2"))
        flag = true;
      return flag;
    }

    private static TeamFoundationBuildService.BuildDefinitionDatabaseResult ReadDatabaseResults(
      ResultCollection dbResult,
      bool readServices = false)
    {
      TeamFoundationBuildService.BuildDefinitionDatabaseResult definitionDatabaseResult = new TeamFoundationBuildService.BuildDefinitionDatabaseResult();
      definitionDatabaseResult.Definitions = dbResult.GetCurrent<BuildDefinition>().Items;
      dbResult.NextResult();
      definitionDatabaseResult.RetentionPolicies = dbResult.GetCurrent<RetentionPolicy>().Items;
      dbResult.NextResult();
      definitionDatabaseResult.Schedules = dbResult.GetCurrent<Schedule>().Items;
      dbResult.NextResult();
      try
      {
        definitionDatabaseResult.SourceProviders = dbResult.GetCurrent<BuildDefinitionSourceProvider>().Items;
        dbResult.NextResult();
      }
      catch (InvalidCastException ex)
      {
        definitionDatabaseResult.SourceProviders = new List<BuildDefinitionSourceProvider>();
      }
      definitionDatabaseResult.ProcessTemplates = dbResult.GetCurrent<ProcessTemplate>().Items;
      dbResult.NextResult();
      definitionDatabaseResult.WorkspaceTemplates = dbResult.GetCurrent<WorkspaceTemplate>().Items;
      dbResult.NextResult();
      definitionDatabaseResult.WorkspaceMappings = dbResult.GetCurrent<WorkspaceMapping>().Items;
      if (readServices)
      {
        dbResult.NextResult();
        definitionDatabaseResult.Controllers = dbResult.GetCurrent<BuildController>().Items;
        dbResult.NextResult();
        definitionDatabaseResult.Agents = dbResult.GetCurrent<BuildAgent>().Items;
        dbResult.NextResult();
        definitionDatabaseResult.ServiceHosts = dbResult.GetCurrent<BuildServiceHost>().Items;
      }
      return definitionDatabaseResult;
    }

    private void PostProcessDatabaseResults(
      IVssRequestContext requestContext,
      TeamFoundationBuildService.BuildDefinitionDatabaseResult dbResults)
    {
      Dictionary<Tuple<Guid, string>, BuildDefinition> targetDictionary1 = new Dictionary<Tuple<Guid, string>, BuildDefinition>();
      foreach (BuildDefinition definition in dbResults.Definitions)
      {
        Tuple<Guid, string> key = new Tuple<Guid, string>(definition.TeamProject.Id, definition.Uri);
        targetDictionary1.Add(key, definition);
      }
      Dictionary<Tuple<Guid, int>, ProcessTemplate> targetDictionary2 = new Dictionary<Tuple<Guid, int>, ProcessTemplate>();
      foreach (ProcessTemplate processTemplate in dbResults.ProcessTemplates)
        targetDictionary2[new Tuple<Guid, int>(processTemplate.TeamProjectObj.Id, processTemplate.Id)] = processTemplate;
      BuildIdentityResolver identityResolver = new BuildIdentityResolver();
      foreach (WorkspaceTemplate workspaceTemplate in dbResults.WorkspaceTemplates)
        workspaceTemplate.LastModifiedBy = identityResolver.GetUniqueName(requestContext, workspaceTemplate.LastModifiedBy);
      DBHelper.Match<WorkspaceTemplate, WorkspaceMapping, Tuple<Guid, int>>(dbResults.WorkspaceTemplates, dbResults.WorkspaceMappings, (Func<WorkspaceTemplate, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.ProjectId, x.WorkspaceId)), (Func<WorkspaceMapping, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.ProjectId, x.WorkspaceId)), (Action<WorkspaceTemplate, WorkspaceMapping>) ((x, y) => x.Mappings.Add(y)), (Func<Tuple<Guid, int>, Tuple<Guid, int>, bool>) ((x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));
      DBHelper.Match<BuildDefinition, Schedule, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<Schedule>) dbResults.Schedules, (Func<Schedule, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, Schedule>) ((x, y) => x.Schedules.Add(y)));
      DBHelper.Match<BuildDefinition, BuildDefinitionSourceProvider, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<BuildDefinitionSourceProvider>) dbResults.SourceProviders, (Func<BuildDefinitionSourceProvider, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, BuildDefinitionSourceProvider>) ((x, y) => x.SourceProviders.Add(y)));
      DBHelper.Match<BuildDefinition, RetentionPolicy, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<RetentionPolicy>) dbResults.RetentionPolicies, (Func<RetentionPolicy, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, RetentionPolicy>) ((x, y) => x.RetentionPolicies.Add(y)));
      DBHelper.Match<ProcessTemplate, BuildDefinition, Tuple<Guid, int>>(targetDictionary2, (IEnumerable<BuildDefinition>) dbResults.Definitions, (Func<BuildDefinition, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.TeamProject.Id, x.ProcessTemplateId)), (Action<ProcessTemplate, BuildDefinition>) ((x, y) => y.Process = x));
      DBHelper.Match<BuildDefinition, WorkspaceTemplate, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<WorkspaceTemplate>) dbResults.WorkspaceTemplates, (Func<WorkspaceTemplate, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, WorkspaceTemplate>) ((x, y) => x.WorkspaceTemplate = y));
      dbResults.Definitions.ConvertObjectsToProjectName(requestContext);
    }

    private static void PublishBuildsQueued(
      IVssRequestContext requestContext,
      QueuedBuild[] queuedBuilds)
    {
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      requestContext.TraceConditionally(0, TraceLevel.Verbose, "Build", "Service", (Func<string>) (() => string.Format("Publishing notification for queued builds '{0}'", (object) string.Join(", ", ((IEnumerable<QueuedBuild>) queuedBuilds).Select<QueuedBuild, string>((Func<QueuedBuild, string>) (qb => qb.Id.ToString()))))));
      IVssRequestContext requestContext1 = requestContext;
      BuildsQueuedNotificationEvent notificationEvent = new BuildsQueuedNotificationEvent(queuedBuilds);
      service.PublishNotification(requestContext1, (object) notificationEvent);
    }

    private void MatchBuildsToQueuedBuilds(
      IVssRequestContext requestContext,
      QueryOptions options,
      StreamingCollection<BuildDetail> builds,
      IList<BuildDefinition> definitions,
      IDictionary<string, BuildDetail> allBuilds,
      HashSet<string> includedBuilds,
      HashSet<string> includedDefinitions,
      BuildDefinition definition,
      QueuedBuild queuedBuild)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (MatchBuildsToQueuedBuilds));
      bool flag = false;
      if ((options & QueryOptions.Definitions) == QueryOptions.Definitions)
      {
        flag = this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, definition, BuildPermissions.ViewBuildDefinition);
        if (includedDefinitions.Add(definition.Uri) & flag)
        {
          definitions.Add(definition);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Read definition '{0}'", (object) definition.Uri);
        }
      }
      if (includedDefinitions.Contains(definition.Uri))
        queuedBuild.Definition = definition;
      queuedBuild.TeamProject = definition.TeamProject.Name;
      queuedBuild.ProjectId = definition.TeamProject.Id;
      if (!this.m_buildHost.SecurityManager.HasBuildPermission(requestContext, definition, BuildPermissions.ViewBuilds))
      {
        requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Exiting due to access denied: ViewBuilds");
      }
      else
      {
        foreach (BuildDetail buildDetail in allBuilds.Join<KeyValuePair<string, BuildDetail>, string, string, BuildDetail>((IEnumerable<string>) queuedBuild.BuildUris, (Func<KeyValuePair<string, BuildDetail>, string>) (x => x.Key), (Func<string, string>) (x => x), (Func<KeyValuePair<string, BuildDetail>, string, BuildDetail>) ((x, y) => x.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<BuildDetail>())
        {
          buildDetail.TeamProject = definition.TeamProject.Name;
          if (includedBuilds.Add(buildDetail.Uri))
          {
            if (flag && (options & QueryOptions.Definitions) == QueryOptions.Definitions)
            {
              buildDetail.Definition = definition;
              requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Associated definition '{0}' to build '{1}'", (object) definition.Uri, (object) buildDetail.Uri);
            }
            builds.Enqueue(buildDetail);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build '{0}'", (object) buildDetail.Uri);
          }
        }
        requestContext.TraceLeave(0, "Build", "Service", nameof (MatchBuildsToQueuedBuilds));
      }
    }

    private static void CheckDefinitionAndControllerCompatibility(
      IVssRequestContext requestContext,
      IList<BuildDefinition> definitions,
      HashSet<string> serverDropControllerUris,
      HashSet<string> gitCustomTemplatecontrollerUris)
    {
      if (serverDropControllerUris.Count > 0 && requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build.NoServerDrops"))
        throw new BuildDefinitionUpdateException(ResourceStrings.BuildContainerDropDisabled());
      if (serverDropControllerUris.Count <= 0 && gitCustomTemplatecontrollerUris.Count <= 0)
        return;
      BuildControllerQueryResult controllerQueryResult = requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildControllersByUri(requestContext, (IList<string>) serverDropControllerUris.Union<string>((IEnumerable<string>) gitCustomTemplatecontrollerUris, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>(), (IList<string>) null, false);
      IDictionary<string, BuildServiceHost> dictionary = (IDictionary<string, BuildServiceHost>) controllerQueryResult.ServiceHosts.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (BuildController controller1 in controllerQueryResult.Controllers)
      {
        BuildController controller = controller1;
        BuildServiceHost buildServiceHost;
        if (!dictionary.TryGetValue(controller.ServiceHostUri, out buildServiceHost))
          throw new BuildDefinitionUpdateException(controller);
        if (serverDropControllerUris.Contains(controller.Uri) && controller.Version < 410 && !buildServiceHost.IsVirtual)
          throw new BuildDefinitionUpdateException(definitions.First<BuildDefinition>((Func<BuildDefinition, bool>) (x => x.BuildControllerUri == controller.Uri)), controller);
        if (gitCustomTemplatecontrollerUris.Contains(controller.Uri) && controller.Version < 500 && !buildServiceHost.IsVirtual)
          throw new BuildDefinitionUpdateException(ResourceStrings.GitCustomTemplateNotSupported((object) definitions.First<BuildDefinition>((Func<BuildDefinition, bool>) (x => x.BuildControllerUri == controller.Uri)).Name, (object) controller.Name));
      }
    }

    internal static void CheckXamlEnabled(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !requestContext.IsFeatureEnabled("Build.XamlEnabled"))
        throw new XamlBuildDisabledException();
    }

    private class BuildDefinitionDatabaseResult
    {
      public List<BuildDefinition> Definitions;

      public List<RetentionPolicy> RetentionPolicies { get; internal set; }

      public List<Schedule> Schedules { get; internal set; }

      public List<BuildDefinitionSourceProvider> SourceProviders { get; internal set; }

      public List<ProcessTemplate> ProcessTemplates { get; internal set; }

      public List<WorkspaceTemplate> WorkspaceTemplates { get; internal set; }

      public List<WorkspaceMapping> WorkspaceMappings { get; internal set; }

      public List<BuildServiceHost> ServiceHosts { get; internal set; }

      public List<BuildController> Controllers { get; internal set; }

      public List<BuildAgent> Agents { get; internal set; }
    }
  }
}
