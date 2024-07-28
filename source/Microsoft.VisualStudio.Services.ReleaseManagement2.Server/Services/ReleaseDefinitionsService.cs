// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseDefinitionsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
  public class ReleaseDefinitionsService : ReleaseManagement2ServiceBase
  {
    private readonly SecretsHelper secretsHelper;
    private readonly Func<IVssRequestContext, ReleasesService> getReleasesService;
    private readonly Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer;
    private readonly Func<IVssRequestContext, Guid, int, bool, bool, ReleaseDefinition> getReleaseDefinitionWithinUsingWithComponent;

    protected ReleaseDefinitionsService(
      Func<IVssRequestContext, ReleasesService> getReleasesService,
      Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer,
      Func<IVssRequestContext, Guid, int, bool, bool, ReleaseDefinition> getReleaseDefinitionWithinUsingWithComponent)
    {
      this.getReleasesService = getReleasesService;
      this.getDataAccessLayer = getDataAccessLayer;
      this.getReleaseDefinitionWithinUsingWithComponent = getReleaseDefinitionWithinUsingWithComponent;
      this.secretsHelper = new SecretsHelper();
    }

    public ReleaseDefinitionsService()
      : this(ReleaseDefinitionsService.\u003C\u003EO.\u003C0\u003E__GetReleasesService ?? (ReleaseDefinitionsService.\u003C\u003EO.\u003C0\u003E__GetReleasesService = new Func<IVssRequestContext, ReleasesService>(ReleaseDefinitionsService.GetReleasesService)), ReleaseDefinitionsService.\u003C\u003EO.\u003C1\u003E__GetDataAccessLayer ?? (ReleaseDefinitionsService.\u003C\u003EO.\u003C1\u003E__GetDataAccessLayer = new Func<IVssRequestContext, Guid, IDataAccessLayer>(ReleaseDefinitionsService.GetDataAccessLayer)), ReleaseDefinitionsService.\u003C\u003EO.\u003C2\u003E__GetReleaseDefinitionWithinUsingWithComponent ?? (ReleaseDefinitionsService.\u003C\u003EO.\u003C2\u003E__GetReleaseDefinitionWithinUsingWithComponent = new Func<IVssRequestContext, Guid, int, bool, bool, ReleaseDefinition>(ReleaseDefinitionsService.GetReleaseDefinitionWithinUsingWithComponent)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public static void ThrowIfAnySecretIsTooLong(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      ReleaseDefinitionsService.ThrowIfAnySecretsTooLongWorker(releaseDefinition.Variables, releaseDefinition.Name);
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        ReleaseDefinitionsService.ThrowIfAnySecretsTooLongWorker(environment.Variables, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) releaseDefinition.Name, (object) environment.Name));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static void QueueReleaseDefinitionDeletionJob(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds,
      string comment,
      int delaySeconds = 0)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) ReleaseDefinitionDeletionJobData.CreateJobData(projectId, releaseDefinitionIds.ToList<int>(), comment));
      ArgumentUtility.CheckForNull<XmlNode>(xml, "jobData");
      Guid guid = requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, "OnReleaseDefinitionDeleted", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.ReleaseDefinitionDeletionJob", xml, JobPriorityLevel.BelowNormal, TimeSpan.FromSeconds((double) delaySeconds));
      requestContext.Trace(1976389, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "QueueReleaseDefinitionDeletionJob: jobId: {0}, jobData: {1}", (object) guid, (object) xml.OuterXml);
    }

    public virtual IEnumerable<ReleaseDefinition> GetAllReleaseDefinitions(
      IVssRequestContext context,
      Guid projectId)
    {
      IEnumerable<ReleaseDefinition> releaseDefinitions = this.GetAllServerReleaseDefinitions(context, projectId);
      return (IEnumerable<ReleaseDefinition>) ReleaseManagementSecurityProcessor.FilterComponents<ReleaseDefinition>(context, releaseDefinitions, (Func<ReleaseDefinition, ReleaseManagementSecurityInfo>) (definition => ReleaseManagementSecurityProcessor.GetSecurityInfo(projectId, definition.Path, definition.Id, ReleaseManagementSecurityPermissions.ViewReleaseDefinition)), false, out IList<ReleaseDefinition> _);
    }

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1113:CommaMustBeOnSameLineAsPreviousParameter", Justification = "Reviewed. Suppression is OK here.")]
    public virtual IEnumerable<ReleaseDefinition> GetReleaseDefinitions(
      IVssRequestContext context,
      Guid projectId,
      string nameFilter,
      string artifactType,
      IEnumerable<string> sourceIds,
      bool isDeleted,
      ReleaseDefinitionExpands definitionExpands,
      DateTime? maxModifiedTime,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder queryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.IdAscending,
      string minReleaseDefinitionId = "0",
      int maxReleaseDefinitionsCount = 0,
      string path = null,
      bool isExactNameMatch = false,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<int> definitionIdFilter = null,
      bool searchTextContainsFolderName = false)
    {
      bool includeEnvironments = (definitionExpands & ReleaseDefinitionExpands.Environments) == ReleaseDefinitionExpands.Environments;
      bool includeArtifacts = (definitionExpands & ReleaseDefinitionExpands.Artifacts) == ReleaseDefinitionExpands.Artifacts;
      bool includeVariables = (definitionExpands & ReleaseDefinitionExpands.Variables) == ReleaseDefinitionExpands.Variables;
      bool includeTags = (definitionExpands & ReleaseDefinitionExpands.Tags) == ReleaseDefinitionExpands.Tags;
      bool includeTriggers = (definitionExpands & ReleaseDefinitionExpands.Triggers) == ReleaseDefinitionExpands.Triggers;
      bool includeLatestRelease = (definitionExpands & ReleaseDefinitionExpands.LastRelease) == ReleaseDefinitionExpands.LastRelease;
      int top = maxReleaseDefinitionsCount > 0 ? maxReleaseDefinitionsCount + 1 : 0;
      IEnumerable<ReleaseDefinition> releaseDefinitions = this.GetServerReleaseDefinitions(context, projectId, nameFilter, artifactType, sourceIds, isDeleted, includeEnvironments, includeArtifacts, includeVariables, includeTags, includeTriggers, includeLatestRelease, maxModifiedTime, queryOrder, top, minReleaseDefinitionId, path, isExactNameMatch, tagFilter, definitionIdFilter, searchTextContainsFolderName);
      IList<ReleaseDefinition> excludedComponents;
      IList<ReleaseDefinition> allDefinitions = ReleaseManagementSecurityProcessor.FilterComponents<ReleaseDefinition>(context, releaseDefinitions, (Func<ReleaseDefinition, ReleaseManagementSecurityInfo>) (definition => ReleaseManagementSecurityProcessor.GetSecurityInfo(projectId, definition.Path, definition.Id, ReleaseManagementSecurityPermissions.ViewReleaseDefinition)), false, out excludedComponents);
      if (maxReleaseDefinitionsCount == 0)
        return (IEnumerable<ReleaseDefinition>) ReleaseDefinitionsService.PopulateProperties(context, projectId, propertyFilters, allDefinitions);
      while (excludedComponents.Count > 0 && releaseDefinitions.Count<ReleaseDefinition>() == top && allDefinitions.Count < maxReleaseDefinitionsCount)
      {
        minReleaseDefinitionId = ReleaseDefinitionsService.ComputeNextContinuationToken(queryOrder, releaseDefinitions);
        releaseDefinitions = this.GetServerReleaseDefinitions(context, projectId, nameFilter, artifactType, sourceIds, isDeleted, includeEnvironments, includeArtifacts, includeVariables, includeTags, includeTriggers, includeLatestRelease, maxModifiedTime, queryOrder, top, minReleaseDefinitionId, path, isExactNameMatch, tagFilter, definitionIdFilter, searchTextContainsFolderName);
        IList<ReleaseDefinition> source = ReleaseManagementSecurityProcessor.FilterComponents<ReleaseDefinition>(context, releaseDefinitions, (Func<ReleaseDefinition, ReleaseManagementSecurityInfo>) (definition => ReleaseManagementSecurityProcessor.GetSecurityInfo(projectId, definition.Path, definition.Id, ReleaseManagementSecurityPermissions.ViewReleaseDefinition)), false, out excludedComponents);
        allDefinitions.AddRange<ReleaseDefinition, IList<ReleaseDefinition>>(source.Where<ReleaseDefinition>((Func<ReleaseDefinition, bool>) (d => allDefinitions.All<ReleaseDefinition>((Func<ReleaseDefinition, bool>) (x => d.Id != x.Id)))));
      }
      allDefinitions = ReleaseDefinitionsService.PopulateProperties(context, projectId, propertyFilters, allDefinitions);
      return allDefinitions.Take<ReleaseDefinition>(maxReleaseDefinitionsCount);
    }

    public virtual IList<ReleaseTriggerBase> GetReleaseTriggersWithMatchingRepositoryData(
      IVssRequestContext context,
      ReleaseTriggerType releaseTriggerType,
      string repositoryName,
      string connectionId)
    {
      IList<ReleaseTriggerBase> matchingRepositoryData = (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
      if (!string.IsNullOrEmpty(repositoryName) && !string.IsNullOrEmpty(connectionId))
      {
        using (ReleaseManagementTimer.Create(context, "DataAccessLayer", nameof (GetReleaseTriggersWithMatchingRepositoryData), 1961209))
        {
          Func<ReleaseDefinitionSqlComponent, IList<ReleaseTriggerBase>> action = (Func<ReleaseDefinitionSqlComponent, IList<ReleaseTriggerBase>>) (component => component.GetReleaseTriggersWithMatchingRepositoryData(releaseTriggerType, repositoryName, connectionId));
          matchingRepositoryData = (IList<ReleaseTriggerBase>) context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IList<ReleaseTriggerBase>>(action).ToList<ReleaseTriggerBase>();
        }
      }
      return matchingRepositoryData;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual IEnumerable<int> GetHardDeleteReleaseDefinitionCandidates(
      IVssRequestContext context,
      Guid projectId,
      DateTime? maxModifiedTime,
      int maxReleaseDefinitionsCount = 0,
      int continuationToken = 0)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (projectId.Equals(Guid.Empty))
        throw new ArgumentNullException(nameof (projectId));
      if (!context.IsSystemContext)
        throw new UnauthorizedAccessException();
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "RmDefinitionsController.GetHardDeleteReleaseDefinitionCandidates", 1961232))
      {
        Func<ReleaseDefinitionSqlComponent, IEnumerable<int>> action = (Func<ReleaseDefinitionSqlComponent, IEnumerable<int>>) (component => component.ListHardDeleteReleaseDefinitionCandidates(projectId, maxModifiedTime, maxReleaseDefinitionsCount, continuationToken));
        return (IEnumerable<int>) context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IEnumerable<int>>(action).ToList<int>();
      }
    }

    private static IList<ReleaseDefinition> PopulateProperties(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<string> propertyFilters,
      IList<ReleaseDefinition> allDefinitions)
    {
      if (allDefinitions != null && propertyFilters != null && propertyFilters.Any<string>())
      {
        using (TeamFoundationDataReader properties = context.GetService<ITeamFoundationPropertyService>().GetProperties(context, allDefinitions.Select<ReleaseDefinition, ArtifactSpec>((Func<ReleaseDefinition, ArtifactSpec>) (x => x.CreateArtifactSpec(projectId))), propertyFilters))
          ReleaseManagementArtifactPropertyKinds.MatchProperties<ReleaseDefinition>(properties, (IList<ReleaseDefinition>) allDefinitions.ToList<ReleaseDefinition>(), (Func<ReleaseDefinition, int>) (x => x.Id), (Action<ReleaseDefinition, IList<Microsoft.TeamFoundation.Framework.Server.PropertyValue>>) ((x, y) => x.Properties.AddRange<Microsoft.TeamFoundation.Framework.Server.PropertyValue, IList<Microsoft.TeamFoundation.Framework.Server.PropertyValue>>((IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) y)));
      }
      return allDefinitions;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue> GetProperties(
      IVssRequestContext context,
      Guid projectId,
      int releaseDefinitionId)
    {
      ArtifactSpec artifactSpec = new ArtifactSpec(ReleaseManagementArtifactPropertyKinds.ReleaseDefinition, releaseDefinitionId, 0, projectId);
      using (TeamFoundationDataReader properties = context.GetService<ITeamFoundationPropertyService>().GetProperties(context, artifactSpec, (IEnumerable<string>) null))
      {
        if (properties != null)
        {
          ArtifactPropertyValue artifactPropertyValue = properties.CurrentEnumerable<ArtifactPropertyValue>().FirstOrDefault<ArtifactPropertyValue>();
          if (artifactPropertyValue != null)
            return (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) artifactPropertyValue.PropertyValues.ToList<Microsoft.TeamFoundation.Framework.Server.PropertyValue>();
        }
      }
      return (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) new List<Microsoft.TeamFoundation.Framework.Server.PropertyValue>();
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference> GetAllShallowDefinitionsForProject(
      IVssRequestContext context,
      Guid projectId)
    {
      Func<ReleaseDefinitionSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference>> action = (Func<ReleaseDefinitionSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference>>) (component => component.QueryAllDefinitionsForDataProvider(projectId));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference> list = context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference>>(action).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference>();
      IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(context, projectId, (IList<int>) list.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference, int>) (releaseDefinition => releaseDefinition.Id)).ToList<int>());
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference> components = list.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference>) (e => new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
      {
        Id = e.Id,
        Name = e.Name,
        Path = folderPaths[e.Id],
        Links = (ReferenceLinks) null,
        Url = (string) null
      }));
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference>) ReleaseManagementSecurityProcessor.FilterComponents<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference>(context, components, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference, ReleaseManagementSecurityInfo>) (definition => ReleaseManagementSecurityProcessor.GetSecurityInfo(projectId, folderPaths[definition.Id], definition.Id, ReleaseManagementSecurityPermissions.ViewReleaseDefinition)), false);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public bool HasAnyReleaseDefinition(IVssRequestContext requestContext, Guid projectId) => this.HasAnyReleaseDefinitionWithArtifacts(requestContext, projectId, (string) null, (IEnumerable<string>) null);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public bool HasAnyReleaseDefinitionWithArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      string artifactType,
      IEnumerable<string> artifactSourceIds)
    {
      return this.GetServerReleaseDefinitions(requestContext, projectId, string.Empty, artifactType, artifactSourceIds, false, false, false, false, false, false, false, new DateTime?(), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.IdDescending, 1, (string) null, (string) null, false, (IEnumerable<string>) null, (IEnumerable<int>) null, false).Any<ReleaseDefinition>();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public virtual ReleaseDefinition GetReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      bool includeDeleted = false,
      bool includeLastRelease = false,
      IEnumerable<string> propertyFilters = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      ReleaseDefinition serverDefinition = this.getReleaseDefinitionWithinUsingWithComponent(context, projectId, definitionId, includeDeleted, includeLastRelease);
      if (serverDefinition != null)
      {
        if (!context.HasPermission(projectId, serverDefinition.Path, serverDefinition.Id, ReleaseManagementSecurityPermissions.ViewReleaseDefinition, true))
        {
          ResourceAccessException innerException = new ResourceAccessException(context.GetUserId().ToString(), ReleaseManagementSecurityPermissions.ViewReleaseDefinition);
          throw new UnauthorizedRequestException(innerException.Message, (Exception) innerException);
        }
        serverDefinition.PopulateProperties(context, projectId, propertyFilters);
      }
      return serverDefinition;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public virtual ReleaseDefinition GetReleaseDefinitionServerObject(
      IVssRequestContext context,
      Guid projectId,
      int definitionId)
    {
      bool isDefaultToLatestArtifactVersionEnabled = context != null ? context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion") : throw new ArgumentNullException(nameof (context));
      Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> action = (Func<ReleaseDefinitionSqlComponent, ReleaseDefinition>) (component => component.GetReleaseDefinition(projectId, definitionId, isDefaultToLatestArtifactVersionEnabled: isDefaultToLatestArtifactVersionEnabled));
      return context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, ReleaseDefinition>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public ReleaseDefinition AddReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      string comment)
    {
      try
      {
        if (context.GetClient<BuildHttpClient>().GetBuildGeneralSettingsAsync(projectId).Result.DisableClassicReleasePipelineCreation.Value)
          throw new InvalidOperationException(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ClassicPipelinesDisabled);
      }
      catch (HttpRequestException ex)
      {
        context.Trace(1976495, TraceLevel.Error, "ReleaseManagementService", "DataAccessLayer", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.CannotGetPipelineGeneralSettings, (object) projectId, (object) ex.Message);
      }
      Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> addOrUpdateReleaseDefinition = (Func<ReleaseDefinitionSqlComponent, ReleaseDefinition>) (component => component.AddReleaseDefinition(projectId, releaseDefinition));
      ReadOnlyDictionary<Guid, ISet<int>> existingEnvironmentScheduleJobIdsMap = new ReadOnlyDictionary<Guid, ISet<int>>((IDictionary<Guid, ISet<int>>) new Dictionary<Guid, ISet<int>>());
      releaseDefinition.SetScheduleJobIdForNewSchedules(new HashSet<Guid>(), (IReadOnlyDictionary<Guid, ISet<int>>) existingEnvironmentScheduleJobIdsMap);
      ReleaseDefinitionsService.ValidateRetentionPolicy(context, projectId, false, releaseDefinition, (ReleaseDefinition) null);
      if (releaseDefinition.Path != null)
      {
        releaseDefinition.Path = FolderValidator.ValidateAndSanitizePath(releaseDefinition.Path);
        FolderValidator.CheckForFolderNamesWithOnlyDigits(context, releaseDefinition.Path);
      }
      ReleaseDefinitionsService.PublishReleaseDefinitionChangingEvent(context, projectId, releaseDefinition, (ReleaseDefinition) null, comment);
      ReleaseDefinition releaseDefinition1 = this.AddOrUpdateReleaseDefinition(context, projectId, releaseDefinition, addOrUpdateReleaseDefinition, 1961004);
      releaseDefinition1.AddScheduledJobs(context, projectId);
      releaseDefinition1.ProjectId = projectId;
      EventsHelper.FireDefinitionCreatedEvent(context, releaseDefinition1);
      EventsHelper.FireReleaseDefinitionChangedEvent(context, projectId, releaseDefinition1, (ReleaseDefinition) null);
      return releaseDefinition1;
    }

    public ReleaseDefinition UpdateReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      string comment)
    {
      return this.UpdateReleaseDefinition(context, projectId, releaseDefinition, (ReleaseDefinition) null, comment);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This method should be refactored.")]
    public ReleaseDefinition UpdateReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      ReleaseDefinition definitionBeforeUpdate,
      string comment)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (definitionBeforeUpdate == null)
        definitionBeforeUpdate = this.getReleaseDefinitionWithinUsingWithComponent(context, projectId, releaseDefinition.Id, false, false);
      if (string.Compare(definitionBeforeUpdate.Path, releaseDefinition.Path, StringComparison.OrdinalIgnoreCase) != 0 && !context.HasPermission(projectId, releaseDefinition.Path, -1, ReleaseManagementSecurityPermissions.EditReleaseDefinition))
        throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.UserNotHavingPermissionsDestinationFolder, (object) releaseDefinition.Path));
      if (releaseDefinition.Revision != definitionBeforeUpdate.Revision)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.DefinitionAlreadyUpdated));
      ReleaseDefinitionsService.ValidateNewEnvironmentIds(releaseDefinition, definitionBeforeUpdate);
      ReleaseDefinitionsService.ValidateStepIds(releaseDefinition, definitionBeforeUpdate);
      ReleaseDefinitionsService.ValidateRetentionPolicy(context, projectId, true, releaseDefinition, definitionBeforeUpdate);
      if (releaseDefinition.Path != null)
        releaseDefinition.Path = FolderValidator.ValidateAndSanitizePath(releaseDefinition.Path);
      if (!string.Equals(definitionBeforeUpdate.Path, releaseDefinition.Path, StringComparison.OrdinalIgnoreCase))
        FolderValidator.CheckForFolderNamesWithOnlyDigits(context, releaseDefinition.Path);
      HashSet<Guid> definitionScheduleJobIds;
      IDictionary<Guid, ISet<int>> environmentScheduleJobIds;
      definitionBeforeUpdate.GetScheduledJobIds(out definitionScheduleJobIds, out environmentScheduleJobIds);
      releaseDefinition.SetScheduleJobIdForNewSchedules(definitionScheduleJobIds, (IReadOnlyDictionary<Guid, ISet<int>>) new ReadOnlyDictionary<Guid, ISet<int>>(environmentScheduleJobIds));
      ITeamFoundationJobService jobService = ReleaseOperationHelper.GetJobService(context);
      List<ReleaseSchedule> jobsToUpdate = new List<ReleaseSchedule>();
      List<Guid> jobIdsToDelete = new List<Guid>();
      releaseDefinition.GetScheduleJobsToUpdateAndDelete(definitionBeforeUpdate, out jobsToUpdate, out jobIdsToDelete);
      IList<int> schedulesRemoved = releaseDefinition.GetEnvironmentDefinitionIdsWithSchedulesRemoved(definitionBeforeUpdate);
      ReleaseDefinition releaseDefinition1;
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "RmDefinitionsController.UpdateReleaseDefinition", 1962011))
      {
        ReleaseDefinitionsService.PublishReleaseDefinitionChangingEvent(context, projectId, releaseDefinition, definitionBeforeUpdate, comment);
        Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> addOrUpdateReleaseDefinition = (Func<ReleaseDefinitionSqlComponent, ReleaseDefinition>) (component => component.UpdateReleaseDefinition(projectId, releaseDefinition, ReleaseManagementSecurityHelper.GetToken(projectId, definitionBeforeUpdate.Path, definitionBeforeUpdate.Id), ReleaseManagementSecurityHelper.GetToken(projectId, releaseDefinition.Path, releaseDefinition.Id)));
        releaseDefinition1 = this.AddOrUpdateReleaseDefinition(context, projectId, releaseDefinition, addOrUpdateReleaseDefinition, 1961013);
        EventsHelper.FireReleaseDefinitionChangedEvent(context, projectId, releaseDefinition1, definitionBeforeUpdate);
      }
      List<TeamFoundationJobDefinition> foundationJobDefinitionList = new List<TeamFoundationJobDefinition>();
      bool registryKeyValue = context.GetRegistryKeyValue<bool>("/Service/ReleaseManagement/Settings/JobPriorityAboveNormal/ScheduleReleaseJob", true);
      foreach (ReleaseSchedule schedule in jobsToUpdate)
      {
        int environmentIdForSchedule = releaseDefinition1.GetDefinitionEnvironmentIdForSchedule(schedule);
        if (environmentIdForSchedule > 0)
          foundationJobDefinitionList.Add(ReleaseDefinitionExtensions.CreateScheduleEnvironmentJobDefinition(context, schedule, releaseDefinition1.Id, environmentIdForSchedule, projectId, jobService.IsIgnoreDormancyPermitted));
        else
          foundationJobDefinitionList.Add(ReleaseDefinitionExtensions.CreateScheduleReleaseJobDefinition(schedule, releaseDefinition1.Id, projectId, jobService.IsIgnoreDormancyPermitted, registryKeyValue));
      }
      if (foundationJobDefinitionList.Count<TeamFoundationJobDefinition>() > 0 || jobIdsToDelete.Count<Guid>() > 0)
        ReleaseOperationHelper.UpdateJobs(context, (IEnumerable<TeamFoundationJobDefinition>) foundationJobDefinitionList, (IEnumerable<Guid>) jobIdsToDelete);
      if (schedulesRemoved.Any<int>())
        this.ResetScheduledReleaseEnvironments(context, projectId, releaseDefinition1.Id, (IEnumerable<int>) schedulesRemoved);
      ReleaseDefinitionsService.QueueUpdateRetainBuildJobIfRequired(context, projectId, releaseDefinition1, definitionBeforeUpdate);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ReleaseDefinitionsService.ProcessQueuedEnvironmentsIfQueueSettingModified(context, projectId, releaseDefinition1, definitionBeforeUpdate, ReleaseDefinitionsService.\u003C\u003EO.\u003C3\u003E__QueueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments ?? (ReleaseDefinitionsService.\u003C\u003EO.\u003C3\u003E__QueueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments = new Action<IVssRequestContext, Guid, int, IList<int>>(ReleaseDefinitionsService.QueueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments)));
      RmTelemetryFactory.GetLogger(context).PublishDefinitionUpdated(context, releaseDefinition1);
      return releaseDefinition1;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public void SoftDeleteReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      string comment,
      bool forceDelete)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      ReleaseDefinition usingWithComponent = ReleaseDefinitionsService.GetServerDefinitionWithinUsingWithComponent(context, projectId, definitionId);
      IList<Guid> allScheduleJobIds = usingWithComponent.GetAllScheduleJobIds();
      Guid requestorId = context.GetUserId(true);
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleaseDefinitionsService.DeleteReleaseDefinition", 1961207))
      {
        ReleaseDefinitionsService.PublishReleaseDefinitionChangingEvent(context, projectId, (ReleaseDefinition) null, usingWithComponent, comment);
        Action<ReleaseDefinitionSqlComponent> action = (Action<ReleaseDefinitionSqlComponent>) (component => component.SoftDeleteReleaseDefinition(projectId, requestorId, definitionId, comment, forceDelete));
        context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent>(action);
        if (usingWithComponent.Environments.Any<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.RetentionPolicy.RetainBuild)))
        {
          UpdateRetainBuildData updateRetainBuildData = UpdateRetainBuildData.GetUpdateRetainBuildData(UpdateRetainBuildReason.DeleteReleaseDefinition, projectId, definitionId);
          QueueJobUtility.QueueUpdateRetainBuildJob(context, updateRetainBuildData);
        }
        EventsHelper.FireReleaseDefinitionChangedEvent(context, projectId, (ReleaseDefinition) null, usingWithComponent);
      }
      if (allScheduleJobIds.Count<Guid>() > 0)
        ReleaseOperationHelper.UpdateJobs(context, (IEnumerable<TeamFoundationJobDefinition>) null, (IEnumerable<Guid>) allScheduleJobIds);
      IVssRequestContext requestContext = context;
      Guid projectId1 = projectId;
      List<int> releaseDefinitionIds = new List<int>();
      releaseDefinitionIds.Add(definitionId);
      string definitionDeleted = Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseDefinitionDeleted;
      ReleaseDefinitionsService.QueueReleaseDefinitionDeletionJob(requestContext, projectId1, (IEnumerable<int>) releaseDefinitionIds, definitionDeleted);
      ReleaseDefinitionsService.PublishCustomerIntelligenceDefinitionMarkedAsDeleted(context, projectId, requestorId, definitionId);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public void SoftDeleteReleaseDefinitions(
      IVssRequestContext context,
      Guid projectId,
      string comment)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Func<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>> action1 = (Func<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>) (component => component.ListReleaseDefinitions(projectId, string.Empty, includeTriggers: true));
      List<ReleaseDefinition> list = context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>(action1).ToList<ReleaseDefinition>();
      List<Guid> guidList = new List<Guid>();
      foreach (ReleaseDefinition definition in list)
        guidList.AddRange((IEnumerable<Guid>) definition.GetAllScheduleJobIds());
      Guid requestorId = context.GetUserId(true);
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleaseDefinitionsService.SoftDeleteReleaseDefinitions", 1961211))
      {
        Action<ReleaseDefinitionSqlComponent> action2 = (Action<ReleaseDefinitionSqlComponent>) (component => component.SoftDeleteReleaseDefinition(projectId, requestorId, comment: comment));
        context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent>(action2);
      }
      if (guidList.Count<Guid>() > 0)
        ReleaseOperationHelper.UpdateJobs(context, (IEnumerable<TeamFoundationJobDefinition>) null, (IEnumerable<Guid>) guidList);
      if (list.Count<ReleaseDefinition>() == 0)
        return;
      ReleaseDefinitionsService.QueueReleaseDefinitionDeletionJob(context, projectId, list.Select<ReleaseDefinition, int>((Func<ReleaseDefinition, int>) (rd => rd.Id)), comment);
    }

    public void HardDeleteReleaseDefinitions(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<int> definitionIds)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (definitionIds == null || !definitionIds.Any<int>())
        return;
      foreach (int definitionId in definitionIds)
        this.HardDeleteReleaseDefinition(context, projectId, definitionId);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public ReleaseDefinition UndeleteReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      string comment)
    {
      Guid requestorId = context != null ? context.GetUserId(true) : throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleaseDefinitionsService.UndeleteReleaseDefinition", 1961227))
      {
        Action<ReleaseDefinitionSqlComponent> action = (Action<ReleaseDefinitionSqlComponent>) (component => component.UndeleteReleaseDefinition(projectId, requestorId, definitionId, comment));
        context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent>(action);
      }
      ReleaseDefinition newlyCreatedDefinition = this.getReleaseDefinitionWithinUsingWithComponent(context, projectId, definitionId, false, false);
      if (newlyCreatedDefinition != null)
        newlyCreatedDefinition.AddScheduledJobs(context, projectId);
      return newlyCreatedDefinition;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ReleaseDefinitionsService", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetFolderPaths", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    public IDictionary<int, string> GetFolderPaths(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds)
    {
      try
      {
        using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleaseDefinitionsService.GetFolderPaths", 1961229))
        {
          List<int> distinctDefinitionIds = releaseDefinitionIds.Distinct<int>().ToList<int>();
          Func<ReleaseDefinitionSqlComponent, IDictionary<int, string>> action = (Func<ReleaseDefinitionSqlComponent, IDictionary<int, string>>) (component => component.GetFolderPaths(projectId, (IEnumerable<int>) distinctDefinitionIds));
          IDictionary<int, string> definitionIdToFolderNameMap = context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IDictionary<int, string>>(action);
          return (IDictionary<int, string>) distinctDefinitionIds.ToDictionary<int, int, string>((Func<int, int>) (id => id), (Func<int, string>) (id => !definitionIdToFolderNameMap.ContainsKey(id) ? (string) null : definitionIdToFolderNameMap[id]));
        }
      }
      catch (Exception ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.DefinitionFolderPathsNotFoundMessage, (object) string.Join<int>(",", (IEnumerable<int>) releaseDefinitionIds.Distinct<int>().ToArray<int>()), (object) ex.Message);
        context.TraceException(1961229, TraceLevel.Error, "ReleaseManagementService", "Service", (Exception) new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.FolderNotFoundException(message));
        context.TraceException(1961229, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
        throw new InvalidDataException(message, ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ReleaseDefinitionsService", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetRedeployTriggerEnvironmentDGPhaseData", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    public virtual IList<RedeployTriggerEnvironmentDGPhaseData> GetRedeployTriggerEnvironmentDGPhaseData(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<int> deploymentGroupIds)
    {
      try
      {
        using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleaseDefinitionsService.GetRedeployTriggerEnvironmentDGPhaseData", 1976427))
        {
          Func<ReleaseDefinitionSqlComponent, IList<RedeployTriggerEnvironmentDGPhaseData>> action = (Func<ReleaseDefinitionSqlComponent, IList<RedeployTriggerEnvironmentDGPhaseData>>) (component => component.GetRedeployTriggerEnvironmentDGPhaseData(projectId, deploymentGroupIds));
          return context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IList<RedeployTriggerEnvironmentDGPhaseData>>(action);
        }
      }
      catch (Exception ex)
      {
        context.TraceException(1976427, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
        throw new InvalidDataException("ReleaseDefinitionsService.GetRedeployTriggerEnvironmentDGPhaseData", ex);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By design")]
    public AllPipelinesViewData GetAllPipelinesViewData(
      IVssRequestContext requestContext,
      Guid projectId,
      string queriedFolderPath)
    {
      if (string.IsNullOrWhiteSpace(queriedFolderPath))
        throw new ArgumentNullException(nameof (queriedFolderPath));
      Func<ReleaseDataProviderSqlComponent, AllPipelinesViewData> action = (Func<ReleaseDataProviderSqlComponent, AllPipelinesViewData>) (component => component.GetAllPipelinesViewData(projectId, queriedFolderPath));
      return requestContext.ExecuteWithinUsingWithComponent<ReleaseDataProviderSqlComponent, AllPipelinesViewData>(action);
    }

    internal static IEnumerable<string> AppendProjectIdToBuildArtifactSourceIdsIfRequired(
      Guid projectId,
      string artifactTypeId,
      IEnumerable<string> artifactSourceIds)
    {
      return string.IsNullOrWhiteSpace(artifactTypeId) || string.Compare(artifactTypeId, "Build", StringComparison.OrdinalIgnoreCase) != 0 || artifactSourceIds == null || !artifactSourceIds.Any<string>() ? artifactSourceIds : artifactSourceIds.Select<string, string>((Func<string, string>) (sourceId => sourceId.Contains(":") ? sourceId : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId.ToString("D"), (object) sourceId)));
    }

    internal static void ProcessQueuedEnvironmentsIfQueueSettingModified(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition updatedDefinition,
      ReleaseDefinition definitionBeforeUpdate,
      Action<IVssRequestContext, Guid, int, IList<int>> queueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments)
    {
      List<int> settingModifiedEnvironments = new List<int>();
      foreach (DefinitionEnvironment environment1 in (IEnumerable<DefinitionEnvironment>) definitionBeforeUpdate.Environments)
      {
        DefinitionEnvironment oldEnv = environment1;
        DefinitionEnvironment environment2 = updatedDefinition.Environments.FirstOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (x => x.Id == oldEnv.Id));
        if (environment2 != null && !oldEnv.ExecutionPolicyEquals(environment2))
          settingModifiedEnvironments.Add(environment2.Id);
      }
      if (!settingModifiedEnvironments.Any<int>())
        return;
      if (context.IsFeatureEnabled("VisualStudio.ReleaseManagement.UseTaskDispatcherForEvents"))
      {
        context.Fork<ReleaseManagementEventDispatcher>((Func<IVssRequestContext, Task>) (forkedContext =>
        {
          using (ReleaseManagementTimer.Create(forkedContext, "Service", "ReleaseDefinitionsService.ProcessQueuedEnvironmentsIfQueueSettingModified", 1900042))
          {
            queueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments(forkedContext, projectId, updatedDefinition.Id, (IList<int>) settingModifiedEnvironments);
            return Task.CompletedTask;
          }
        }), nameof (ProcessQueuedEnvironmentsIfQueueSettingModified));
      }
      else
      {
        using (ReleaseManagementTimer.Create(context, "Service", "ReleaseDefinitionsService.ProcessQueuedEnvironmentsIfQueueSettingModified", 1900042))
          queueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments(context, projectId, updatedDefinition.Id, (IList<int>) settingModifiedEnvironments);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to continue on error")]
    private static void QueueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      IList<int> environmentIds)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string empty = string.Empty;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ReleaseDefinitionsService.QueueStartEnvironmentJobsToProcessQueueSettingUpdatedEnvironments: projectId: {0}, definitionId: {1}, definitionEnvironments: {2}", (object) projectId, (object) definitionId, (object) string.Join<int>(", ", (IEnumerable<int>) environmentIds));
      stringBuilder.AppendLine(str);
      stringBuilder.Append("DefinitionEnvironmentId::JobId::");
      int num1 = 0;
      int num2 = 0;
      foreach (int environmentId in (IEnumerable<int>) environmentIds)
      {
        try
        {
          StartEnvironmentData startEnvironmentData = StartEnvironmentData.GetStartEnvironmentData(projectId, 0, definitionId, environmentId, 0);
          ArgumentUtility.CheckForNull<StartEnvironmentData>(startEnvironmentData, "environmentData");
          if (context.IsFeatureEnabled("AzureDevops.ReleaseManagement.StartReleaseEnvironmentActionRequestProcessorJob"))
          {
            ActionRequestService service = context.GetService<ActionRequestService>();
            ActionRequestsProcessorHelper.AddStartReleaseEnvironmentActionRequest(context, service, startEnvironmentData);
            service.QueueActionRequestsProcessorJob(context, ActionRequestType.StartReleaseEnvironment, false);
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}, ", (object) environmentId);
          }
          else
          {
            XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) startEnvironmentData);
            ArgumentUtility.CheckForNull<XmlNode>(xml, "jobData");
            Guid guid = context.GetService<TeamFoundationJobService>().QueueOneTimeJob(context, "OnStartEnvironment", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.StartEnvironmentJob", xml, JobPriorityLevel.Normal);
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}::{1}, ", (object) environmentId, (object) guid);
          }
          ++num1;
        }
        catch (Exception ex)
        {
          ++num2;
          context.TraceException(1900042, TraceLevel.Warning, "ReleaseManagementService", "Service", ex);
        }
      }
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Passed:{0}, Failed: {1}", (object) num1, (object) num2);
      stringBuilder.AppendLine();
      context.Trace(1900042, TraceLevel.Info, "ReleaseManagementService", "Service", stringBuilder.ToString());
    }

    private static string ComputeNextContinuationToken(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder queryOrder,
      IEnumerable<ReleaseDefinition> definitions)
    {
      string continuationToken = (string) null;
      ReleaseDefinition releaseDefinition = definitions.LastOrDefault<ReleaseDefinition>();
      if (releaseDefinition != null)
      {
        switch (queryOrder)
        {
          case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.IdAscending:
          case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.IdDescending:
            continuationToken = releaseDefinition.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            break;
          case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.NameAscending:
          case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.NameDescending:
            continuationToken = releaseDefinition.Name;
            break;
        }
      }
      return continuationToken;
    }

    private static void QueueUpdateRetainBuildJobIfRequired(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      ReleaseDefinition definitionBeforeUpdate)
    {
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      foreach (DefinitionEnvironment environment1 in (IEnumerable<DefinitionEnvironment>) definitionBeforeUpdate.Environments)
      {
        foreach (DefinitionEnvironment environment2 in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        {
          if (environment2.Id == environment1.Id)
          {
            if (environment2.RetentionPolicy.RetainBuild != environment1.RetentionPolicy.RetainBuild)
            {
              if (environment2.RetentionPolicy.RetainBuild)
              {
                intList1.Add(environment2.Id);
                break;
              }
              intList2.Add(environment2.Id);
              break;
            }
            break;
          }
        }
      }
      if (!intList1.Any<int>() && !intList2.Any<int>())
        return;
      UpdateRetainBuildData updateRetainBuildData = UpdateRetainBuildData.GetUpdateRetainBuildData(UpdateRetainBuildReason.EditReleaseDefinition, projectId, releaseDefinition.Id, new Collection<int>((IList<int>) intList1), new Collection<int>((IList<int>) intList2));
      QueueJobUtility.QueueUpdateRetainBuildJob(context, updateRetainBuildData);
    }

    private static void PublishReleaseDefinitionChangingEvent(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition newDefinition,
      ReleaseDefinition oldDefinition,
      string comment)
    {
      IReleaseManagementEventService service = context.GetService<IReleaseManagementEventService>();
      DefinitionChangingServerEvent changingServerEvent = new DefinitionChangingServerEvent()
      {
        ReleaseDefinition = newDefinition,
        PreviousReleaseDefinition = oldDefinition,
        ProjectId = projectId,
        Comment = comment
      };
      IVssRequestContext requestContext = context;
      DefinitionChangingServerEvent notificationEvent = changingServerEvent;
      service.PublishDecisionPoint(requestContext, (object) notificationEvent);
    }

    private static ReleaseDefinition GetReleaseDefinitionWithinUsingWithComponent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      bool includeDeleted = false,
      bool includeLastRelease = false)
    {
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(context, "Service", "ReleaseDefinitionsService.GetReleaseDefinition", 1961001))
      {
        ReleaseDefinition usingWithComponent = ReleaseDefinitionsService.GetServerDefinitionWithinUsingWithComponent(context, projectId, definitionId, includeDeleted, includeLastRelease);
        releaseManagementTimer.RecordLap("DataAccessLayer", "GetReleaseDefinitionSql", 1961203);
        return usingWithComponent;
      }
    }

    private static ReleaseDefinition GetServerDefinitionWithinUsingWithComponent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      bool includeDeleted = false,
      bool includeLastRelease = false)
    {
      bool isDefaultToLatestArtifactVersionEnabled = context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion");
      Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> action = (Func<ReleaseDefinitionSqlComponent, ReleaseDefinition>) (component => component.GetReleaseDefinition(projectId, definitionId, includeDeleted, isDefaultToLatestArtifactVersionEnabled, includeLastRelease));
      return context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, ReleaseDefinition>(action);
    }

    private static IDataAccessLayer GetDataAccessLayer(IVssRequestContext context, Guid projectId) => (IDataAccessLayer) new DataAccessLayer(context, projectId);

    private static ReleasesService GetReleasesService(IVssRequestContext context) => context.GetService<ReleasesService>();

    private static void ThrowIfAnySecretsTooLongWorker(
      IDictionary<string, ConfigurationVariableValue> releaseDefinitionVariables,
      string variablesContainerName)
    {
      foreach (string key in (IEnumerable<string>) releaseDefinitionVariables.Keys)
      {
        if (releaseDefinitionVariables[key].IsSecret && key.Length > 400)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.SecretParameterNameTooLong, (object) key.Substring(0, 12), (object) variablesContainerName, (object) key.Length, (object) 400));
      }
    }

    private static void PublishCustomerIntelligenceDefinitionDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int definitionId)
    {
      RmTelemetryFactory.GetLogger(requestContext).PublishDefinitionDeleted(requestContext, projectId, requestorId, definitionId);
    }

    private static void PublishCustomerIntelligenceDefinitionMarkedAsDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int definitionId)
    {
      CustomerIntelligenceHelper.PublishDefinitionMarkedAsDeleted(requestContext, projectId, requestorId, definitionId);
    }

    private static void ValidateRetentionPolicy(
      IVssRequestContext context,
      Guid projectId,
      bool isEditRd,
      ReleaseDefinition newReleaseDefinition,
      ReleaseDefinition existingReleaseDefinition)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy environmentRetentionPolicy = context.GetService<ReleaseSettingsService>().GetReleaseSettings(context, projectId).RetentionSettings.MaximumEnvironmentRetentionPolicy;
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) newReleaseDefinition.Environments)
      {
        DefinitionEnvironment newEnv = environment;
        DefinitionEnvironment definitionEnvironment = (DefinitionEnvironment) null;
        if (isEditRd)
          definitionEnvironment = existingReleaseDefinition.Environments.FirstOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (x => x.Id == newEnv.Id));
        if (!isEditRd || definitionEnvironment == null || !definitionEnvironment.RetentionPolicy.Equals(newEnv.RetentionPolicy))
          ReleaseDefinitionsService.ValidateEnvironmentRetentionPolicy(newEnv, environmentRetentionPolicy);
      }
    }

    private static void ValidateNewEnvironmentIds(
      ReleaseDefinition newReleaseDefinition,
      ReleaseDefinition existingReleaseDefinition)
    {
      IEnumerable<int> second = existingReleaseDefinition.Environments.Select<DefinitionEnvironment, int>((Func<DefinitionEnvironment, int>) (x => x.Id));
      List<int> list = newReleaseDefinition.Environments.Select<DefinitionEnvironment, int>((Func<DefinitionEnvironment, int>) (x => x.Id)).ToList<int>();
      List<int> newlyAddedEnvironments = list.Except<int>(second).ToList<int>();
      IEnumerable<int> ints1 = newlyAddedEnvironments.Where<int>((Func<int, bool>) (x => x > 0));
      if (!ints1.IsNullOrEmpty<int>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.InvalidNewEnvironmentId, (object) string.Join<int>(",", ints1)));
      IEnumerable<int> ints2 = list.Where<int>((Func<int, bool>) (id => id > 0)).GroupBy<int, int>((Func<int, int>) (s => s)).SelectMany<IGrouping<int, int>, int>((Func<IGrouping<int, int>, IEnumerable<int>>) (g => g.Skip<int>(1)));
      if (ints2.Any<int>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionEnvironmentIdsCannotBeIdentical, (object) string.Join<int>(",", ints2)));
      foreach (DefinitionEnvironment definitionEnvironment in newReleaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => newlyAddedEnvironments.Contains(e.Id))))
      {
        foreach (DefinitionEnvironmentStep getStepsForTest in (IEnumerable<DefinitionEnvironmentStep>) definitionEnvironment.GetStepsForTests)
          getStepsForTest.Id = 0;
      }
    }

    private static void ValidateStepIds(
      ReleaseDefinition newReleaseDefinition,
      ReleaseDefinition existingReleaseDefinition)
    {
      foreach (DefinitionEnvironment definitionEnvironment in newReleaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Id > 0)))
      {
        DefinitionEnvironment updatedEnv = definitionEnvironment;
        ReleaseDefinitionsService.ValidateStepIds(updatedEnv, existingReleaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (existingEnv => existingEnv.Id == updatedEnv.Id)).Single<DefinitionEnvironment>());
      }
    }

    private static void ValidateStepIds(
      DefinitionEnvironment updatedDefinitionEnvironment,
      DefinitionEnvironment existingDefinitionEnvironment)
    {
      IEnumerable<DefinitionEnvironmentStep> definitionEnvironmentSteps = updatedDefinitionEnvironment.GetStepsForTests.Where<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, bool>) (step => step.Id > 0));
      IList<DefinitionEnvironmentStep> getStepsForTests = existingDefinitionEnvironment.GetStepsForTests;
      foreach (DefinitionEnvironmentStep definitionEnvironmentStep in definitionEnvironmentSteps)
      {
        DefinitionEnvironmentStep step = definitionEnvironmentStep;
        if (getStepsForTests.Where<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, bool>) (oldstep => oldstep.Id == step.Id && oldstep.StepType == step.StepType)).Count<DefinitionEnvironmentStep>() != 1)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidStepIdForEnvironment, (object) step.Id, (object) updatedDefinitionEnvironment.Name));
      }
    }

    private static void ValidateEnvironmentRetentionPolicy(
      DefinitionEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy maxEnvironmentRetentionPolicy)
    {
      string empty = string.Empty;
      if (environment.RetentionPolicy.DaysToKeep > maxEnvironmentRetentionPolicy.DaysToKeep)
        empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidDaysToKeepRetentionPolicy, (object) maxEnvironmentRetentionPolicy.DaysToKeep);
      if (environment.RetentionPolicy.ReleasesToKeep > maxEnvironmentRetentionPolicy.ReleasesToKeep)
      {
        if (!string.IsNullOrEmpty(empty))
          empty += Environment.NewLine;
        empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidReleasesToKeepRetentionPolicy, (object) maxEnvironmentRetentionPolicy.ReleasesToKeep);
      }
      if (!string.IsNullOrEmpty(empty))
        throw new InvalidRequestException(empty);
    }

    private static ArtifactSpec CreateArtifactSpec(
      DefinitionEnvironment definitionEnvironment,
      Guid dataspaceIdentifier)
    {
      return new ArtifactSpec(ReleaseManagementArtifactPropertyKinds.DefinitionEnvironment, definitionEnvironment.Id, 0, dataspaceIdentifier);
    }

    private static void UpdatePropertyValueDatatype(IList<Microsoft.TeamFoundation.Framework.Server.PropertyValue> properties)
    {
      foreach (Microsoft.TeamFoundation.Framework.Server.PropertyValue propertyValue in properties.Where<Microsoft.TeamFoundation.Framework.Server.PropertyValue>((Func<Microsoft.TeamFoundation.Framework.Server.PropertyValue, bool>) (p => p.Value is bool)))
        propertyValue.Value = (object) propertyValue.Value.ToString();
    }

    private ReleaseDefinition AddOrUpdateReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition definitionWithSecrets,
      Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> addOrUpdateReleaseDefinition,
      int timerTracepoint)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleaseDefinitionsService.AddOrUpdateReleaseDefinition", timerTracepoint))
      {
        ReleaseDefinitionsService.ThrowIfAnySecretIsTooLong(definitionWithSecrets);
        ReleaseDefinition definitionWithoutSecrets = context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, ReleaseDefinition>(addOrUpdateReleaseDefinition);
        ReleaseManagementArtifactPropertyKinds.CopyProperties(definitionWithoutSecrets.Properties, definitionWithSecrets.Properties);
        bool flag = context.IsFeatureEnabled("AzureDevOps.ReleaseManagement.PropertyValueDataTypeConversionEnabled");
        if (flag)
          ReleaseDefinitionsService.UpdatePropertyValueDatatype(definitionWithSecrets.Properties);
        for (int i = 0; i < definitionWithoutSecrets.Environments.Count; i++)
        {
          DefinitionEnvironment definitionEnvironment = definitionWithSecrets.Environments.FirstOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Name.Equals(definitionWithoutSecrets.Environments[i].Name)));
          ReleaseManagementArtifactPropertyKinds.CopyProperties(definitionWithoutSecrets.Environments[i].Properties, definitionEnvironment.Properties);
          if (flag)
            ReleaseDefinitionsService.UpdatePropertyValueDatatype(definitionEnvironment.Properties);
        }
        ReleaseDefinition releaseDefinition = definitionWithoutSecrets.DeepClone();
        this.secretsHelper.CopySecrets(context, projectId, definitionWithSecrets, releaseDefinition);
        this.secretsHelper.StoreSecrets(context, projectId, releaseDefinition);
        if (releaseDefinition.Properties != null && releaseDefinition.Properties.Any<Microsoft.TeamFoundation.Framework.Server.PropertyValue>())
          this.getDataAccessLayer(context, projectId).SaveProperties(releaseDefinition.CreateArtifactSpec(projectId), (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) releaseDefinition.Properties);
        List<ArtifactPropertyValue> artifactPropertyValue = new List<ArtifactPropertyValue>();
        foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        {
          if (environment != null && environment.Properties.Any<Microsoft.TeamFoundation.Framework.Server.PropertyValue>())
            artifactPropertyValue.Add(new ArtifactPropertyValue(ReleaseDefinitionsService.CreateArtifactSpec(environment, projectId), (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) environment.Properties));
        }
        this.getDataAccessLayer(context, projectId).SaveProperties((IEnumerable<ArtifactPropertyValue>) artifactPropertyValue);
        return definitionWithoutSecrets;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to continue on error")]
    private void HardDeleteReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      int definitionId)
    {
      Guid requestorId = context != null ? context.GetUserId(true) : throw new ArgumentNullException(nameof (context));
      try
      {
        using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "ReleaseDefinitionsService.HardDeleteReleaseDefinition", 1961212))
        {
          context.GetService<ReleaseDefinitionHistoryService>().DeleteHistory(context, projectId, definitionId);
          Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> action1 = (Func<ReleaseDefinitionSqlComponent, ReleaseDefinition>) (component => component.GetReleaseDefinition(projectId, definitionId, true));
          ReleaseDefinition releaseDefinition = context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, ReleaseDefinition>(action1);
          this.secretsHelper.DeleteSecrets(context, projectId, releaseDefinition);
          ITeamFoundationPropertyService service = context.GetService<ITeamFoundationPropertyService>();
          service.DeleteArtifacts(context, (IEnumerable<ArtifactSpec>) new List<ArtifactSpec>()
          {
            releaseDefinition.CreateArtifactSpec(projectId)
          }, PropertiesOptions.AllVersions);
          IList<ArtifactSpec> artifactSpecList = (IList<ArtifactSpec>) new List<ArtifactSpec>();
          foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
            artifactSpecList.Add(ReleaseDefinitionsService.CreateArtifactSpec(environment, projectId));
          service.DeleteArtifacts(context, (IEnumerable<ArtifactSpec>) artifactSpecList, PropertiesOptions.AllVersions);
          Action<ReleaseDefinitionSqlComponent> action2 = (Action<ReleaseDefinitionSqlComponent>) (component => component.HardDeleteReleaseDefinition(projectId, definitionId));
          context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent>(action2);
          string token = ReleaseManagementSecurityHelper.GetToken(projectId, releaseDefinition.Path, definitionId);
          ReleaseManagementSecurityProcessor.RemoveAccessControlLists(context, (IEnumerable<string>) new string[1]
          {
            token
          });
        }
      }
      catch (Exception ex)
      {
        context.TraceException(1961213, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
      }
      ReleaseDefinitionsService.PublishCustomerIntelligenceDefinitionDeleted(context, projectId, requestorId, definitionId);
    }

    private void ResetScheduledReleaseEnvironments(
      IVssRequestContext context,
      Guid projectId,
      int releaseDefinitionId,
      IEnumerable<int> definitionEnvironmentIds)
    {
      this.getReleasesService(context).ResetScheduledReleaseEnvironments(context, projectId, releaseDefinitionId, definitionEnvironmentIds);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    private IEnumerable<ReleaseDefinition> GetServerReleaseDefinitions(
      IVssRequestContext context,
      Guid projectId,
      string nameFilter,
      string artifactType,
      IEnumerable<string> sourceIds,
      bool isDeleted,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeVariables,
      bool includeTags,
      bool includeTriggers,
      bool includeLatestRelease,
      DateTime? maxModifiedTime,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder queryOrder,
      int top,
      string continuationToken,
      string path,
      bool isExactNameMatch,
      IEnumerable<string> tagFilter,
      IEnumerable<int> definitionIdFilter,
      bool searchTextContainsFolderName)
    {
      bool isDefaultToLatestArtifactVersionEnabled = context != null ? context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion") : throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "RmDefinitionsController.ListReleaseDefinitions", 1961209))
      {
        Func<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>> action = (Func<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>) (component => component.ListReleaseDefinitions(projectId, nameFilter, ReleaseDefinitionsService.AppendProjectIdToBuildArtifactSourceIdsIfRequired(projectId, artifactType, sourceIds), artifactType, isDeleted, includeEnvironments, includeArtifacts, includeTriggers, includeLatestRelease, maxModifiedTime, queryOrder, continuationToken, top, path, isExactNameMatch, includeTags, tagFilter, includeVariables, definitionIdFilter, isDefaultToLatestArtifactVersionEnabled, searchTextContainsFolderName));
        return (IEnumerable<ReleaseDefinition>) context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>("QueryReleaseDefinitions", action).ToList<ReleaseDefinition>();
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    private IEnumerable<ReleaseDefinition> GetAllServerReleaseDefinitions(
      IVssRequestContext context,
      Guid projectId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "RmDefinitionsController.ListAllReleaseDefinitions", 1961209))
        return (IEnumerable<ReleaseDefinition>) context.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>(new Func<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>(GetAllReleaseDefinitions)).ToList<ReleaseDefinition>();

      IEnumerable<ReleaseDefinition> GetAllReleaseDefinitions(
        ReleaseDefinitionSqlComponent component)
      {
        return component.ListAllReleaseDefinitions(projectId);
      }
    }
  }
}
