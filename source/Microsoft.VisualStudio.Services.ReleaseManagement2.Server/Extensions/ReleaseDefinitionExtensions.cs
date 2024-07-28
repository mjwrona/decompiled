// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "As per design")]
  public static class ReleaseDefinitionExtensions
  {
    private const int WeeklyInterval = 604800;
    private const int DailyInterval = 86400;
    private static readonly ReleaseDefinitionIdentityRetriever IdentityRetriever = new ReleaseDefinitionIdentityRetriever();

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "To keep existing behaviour")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition GetReleaseDefinition(
      this ReleaseDefinitionsService definitionsService,
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      IEnumerable<string> propertyFilters,
      bool includeLastRelease = false)
    {
      ReleaseDefinitionsService definitionsService1 = definitionsService != null ? definitionsService : throw new ArgumentNullException(nameof (definitionsService));
      IVssRequestContext context1 = context;
      Guid projectId1 = projectId;
      int definitionId1 = definitionId;
      IEnumerable<string> strings = propertyFilters;
      int num = includeLastRelease ? 1 : 0;
      IEnumerable<string> propertyFilters1 = strings;
      return definitionsService1.GetReleaseDefinition(context1, projectId1, definitionId1, includeLastRelease: num != 0, propertyFilters: propertyFilters1).ToContract(context, projectId);
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> ToContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition> serverDefinitions,
      IVssRequestContext context,
      Guid projectId)
    {
      if (serverDefinitions == null)
        throw new ArgumentNullException(nameof (serverDefinitions));
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseDefinitionExtensions.ModelToWebApi", 1961014))
      {
        List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> webApiDefinitions = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>();
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition in serverDefinitions)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition contractInternal = ReleaseDefinitionExtensions.ToContractInternal(serverDefinition, context, projectId);
          webApiDefinitions.Add(contractInternal);
        }
        ReleaseDefinitionExtensions.IdentityRetriever.PopulateWebApiDefinitions((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) webApiDefinitions, context);
        return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) webApiDefinitions;
      }
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition ToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      IVssRequestContext context,
      Guid projectId)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseDefinitionExtensions.ModelToWebApi", 1961008))
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition contractInternal = ReleaseDefinitionExtensions.ToContractInternal(serverDefinition, context, projectId);
        ReleaseDefinitionExtensions.IdentityRetriever.PopulateWebApiDefinition(contractInternal, context);
        return contractInternal;
      }
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition FromContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition,
      IVssRequestContext context,
      Guid projectId)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseDefinitionExtensions.WebApiToModel", 1961005))
      {
        ReleaseDefinitionExtensions.EnsureDefinitionDefaults(context, projectId, webApiDefinition);
        ReleaseDefinitionExtensions.ValidateArtifacts(context, webApiDefinition);
        webApiDefinition.Validate(context, projectId);
        webApiDefinition.ValidateWorkflowTasks(context, projectId);
        ReleaseDefinitionExtensions.EnsurePrimaryArtifactIsSet(webApiDefinition.Artifacts);
        webApiDefinition.RemoveDuplicateConditions();
        webApiDefinition.ValidateEnvironmentConditionsLength();
        ReleaseDefinitionValidations.ValidateRetentionPolicy(webApiDefinition);
        webApiDefinition.ValidateEnvironmentRanks();
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition = ReleaseDefinitionConverter.FromWebApi(context, webApiDefinition);
        ReleaseDefinitionExtensions.IdentityRetriever.PopulateServerDefinition(serverDefinition, context);
        ArtifactExtensions.PopulateArtifactsWithSourceId(context, serverDefinition.LinkedArtifacts);
        return serverDefinition;
      }
    }

    public static void ConvertToShallowContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> webApiDefinitions,
      ReleaseDefinitionExpands expands)
    {
      if (webApiDefinitions == null)
        return;
      bool flag1 = (expands & ReleaseDefinitionExpands.Environments) == ReleaseDefinitionExpands.Environments;
      bool flag2 = (expands & ReleaseDefinitionExpands.Artifacts) == ReleaseDefinitionExpands.Artifacts;
      bool flag3 = (expands & ReleaseDefinitionExpands.Variables) == ReleaseDefinitionExpands.Variables;
      bool flag4 = (expands & ReleaseDefinitionExpands.Tags) == ReleaseDefinitionExpands.Tags;
      bool flag5 = (expands & ReleaseDefinitionExpands.LastRelease) == ReleaseDefinitionExpands.LastRelease;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition in webApiDefinitions)
      {
        if (!flag5)
          webApiDefinition.LastRelease = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReference) null;
        if (!flag3)
          webApiDefinition.Variables = (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) null;
        webApiDefinition.VariableGroups = (IList<int>) null;
        if (webApiDefinition.Triggers.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase>())
          webApiDefinition.Triggers = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase>) null;
        if (!flag2 && webApiDefinition.Artifacts.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>())
          webApiDefinition.Artifacts = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) null;
        if (!flag4 && webApiDefinition.Tags.IsNullOrEmpty<string>())
          webApiDefinition.Tags = (IList<string>) null;
        if (!flag1 && webApiDefinition.Environments.IsNullOrEmpty<ReleaseDefinitionEnvironment>())
        {
          webApiDefinition.Environments = (IList<ReleaseDefinitionEnvironment>) null;
        }
        else
        {
          foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) webApiDefinition.Environments)
          {
            if (!flag3)
              environment.Variables = (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) null;
            environment.PreDeployApprovals = (ReleaseDefinitionApprovals) null;
            environment.PostDeployApprovals = (ReleaseDefinitionApprovals) null;
            environment.ExecutionPolicy = (EnvironmentExecutionPolicy) null;
            environment.EnvironmentOptions = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions) null;
            if (environment.Demands.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Demand>())
              environment.Demands = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Demand>) null;
            if (environment.Conditions.IsNullOrEmpty<Condition>())
              environment.Conditions = (IList<Condition>) null;
            if (environment.DeployPhases.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>())
              environment.DeployPhases = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) null;
          }
        }
      }
    }

    private static void ValidateArtifactForUniqueAlias(IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> linkedArtifacts)
    {
      Dictionary<string, int> dictionary = linkedArtifacts.GroupBy<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, string>) (artifact => artifact.Alias), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>>((Func<IGrouping<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>, bool>) (g => g.Count<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>() > 1)).ToDictionary<IGrouping<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>, string, int>((Func<IGrouping<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>, string>) (x => x.Key), (Func<IGrouping<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>, int>) (y => y.Count<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>()));
      if (dictionary.Count > 0)
      {
        string str = string.Empty;
        foreach (KeyValuePair<string, int> keyValuePair in dictionary)
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0},{1}", (object) str, (object) keyValuePair.Key);
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DuplicateArtifactSourceAliasFound, (object) str));
      }
    }

    public static void ValidateArtifacts(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (webApiDefinition == null)
        throw new ArgumentNullException(nameof (webApiDefinition));
      ArtifactTypeServiceBase artifactTypeService = context.GetService<ArtifactTypeServiceBase>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) webApiDefinition.Artifacts)
      {
        if (!artifact.HasAlias())
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ArtifactSourceAliasCannotBeEmpty);
        artifact.Validate(context, (Func<string, ArtifactTypeBase>) (typeId => artifactTypeService.GetArtifactType(context, typeId)));
      }
      ReleaseDefinitionExtensions.ValidateArtifactForUniqueAlias(webApiDefinition.Artifacts);
    }

    private static void EnsurePrimaryArtifactIsSet(IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts)
    {
      if (artifacts == null || !artifacts.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>())
        return;
      int num = artifacts.Count<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, bool>) (artifact => artifact.IsPrimary));
      if (num > 1)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.InvalidPrimaryArtifactCount);
      if (num != 0)
        return;
      artifacts.First<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>().IsPrimary = true;
    }

    private static void PopulateArtifactTypeName(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition webApiDefinition,
      Guid projectId)
    {
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) webApiDefinition.LinkedArtifacts)
        linkedArtifact.PopulateReleaseArtifact(context, projectId);
    }

    public static IList<Guid> GetAllScheduleJobIds(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition)
    {
      HashSet<Guid> definitionScheduleJobIds;
      IDictionary<Guid, ISet<int>> environmentScheduleJobIds;
      definition.GetScheduledJobIds(out definitionScheduleJobIds, out environmentScheduleJobIds);
      return (IList<Guid>) definitionScheduleJobIds.AddRange<Guid, HashSet<Guid>>((IEnumerable<Guid>) environmentScheduleJobIds.Keys).ToList<Guid>();
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "value is set always and cant be null")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "value is set always and cant be null")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "internal")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "internal")]
    public static void GetScheduledJobIds(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition,
      out HashSet<Guid> definitionScheduleJobIds,
      out IDictionary<Guid, ISet<int>> environmentScheduleJobIds)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      definitionScheduleJobIds = new HashSet<Guid>();
      environmentScheduleJobIds = (IDictionary<Guid, ISet<int>>) new Dictionary<Guid, ISet<int>>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger scheduledTrigger in definition.ScheduledTriggers)
        definitionScheduleJobIds.Add(scheduledTrigger.Schedule.JobId);
      foreach (DefinitionEnvironment definitionEnvironment in definition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>())))
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) definitionEnvironment.Schedules)
        {
          if (environmentScheduleJobIds.ContainsKey(schedule.JobId))
            environmentScheduleJobIds[schedule.JobId].Add(definitionEnvironment.Id);
          else
            environmentScheduleJobIds[schedule.JobId] = (ISet<int>) new HashSet<int>()
            {
              definitionEnvironment.Id
            };
        }
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "internal")]
    public static void SetScheduleJobIdForNewSchedules(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition,
      HashSet<Guid> existingDefinitionScheduleJobIds,
      IReadOnlyDictionary<Guid, ISet<int>> existingEnvironmentScheduleJobIdsMap)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (existingDefinitionScheduleJobIds == null)
        throw new ArgumentNullException(nameof (existingDefinitionScheduleJobIds));
      if (existingEnvironmentScheduleJobIdsMap == null)
        throw new ArgumentNullException(nameof (existingEnvironmentScheduleJobIdsMap));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger scheduledTrigger in definition.ScheduledTriggers)
      {
        if (!existingDefinitionScheduleJobIds.Contains(scheduledTrigger.Schedule.JobId))
          scheduledTrigger.SetReleaseScheduleJobId(Guid.NewGuid());
      }
      foreach (DefinitionEnvironment definitionEnvironment in definition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>())))
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) definitionEnvironment.Schedules)
        {
          ISet<int> intSet;
          if (existingEnvironmentScheduleJobIdsMap.TryGetValue(schedule.JobId, out intSet))
          {
            if (!intSet.Contains(definitionEnvironment.Id))
              schedule.JobId = Guid.NewGuid();
          }
          else
            schedule.JobId = Guid.NewGuid();
        }
      }
    }

    public static IList<TeamFoundationJobDefinition> GetScheduleJobDefinitionsToAdd(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition,
      IVssRequestContext requestContext,
      Guid projectId,
      bool isIgnoreDormancyPermitted)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      List<TeamFoundationJobDefinition> definitionsToAdd = new List<TeamFoundationJobDefinition>();
      bool registryKeyValue = requestContext.GetRegistryKeyValue<bool>("/Service/ReleaseManagement/Settings/JobPriorityAboveNormal/ScheduleReleaseJob", true);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger scheduledTrigger in definition.ScheduledTriggers)
        definitionsToAdd.Add(ReleaseDefinitionExtensions.CreateScheduleReleaseJobDefinition(scheduledTrigger.Schedule, definition.Id, projectId, isIgnoreDormancyPermitted, registryKeyValue));
      foreach (DefinitionEnvironment definitionEnvironment in definition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>())))
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) definitionEnvironment.Schedules)
          definitionsToAdd.Add(ReleaseDefinitionExtensions.CreateScheduleEnvironmentJobDefinition(requestContext, schedule, definition.Id, definitionEnvironment.Id, projectId, isIgnoreDormancyPermitted));
      }
      return (IList<TeamFoundationJobDefinition>) definitionsToAdd;
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This method needs to return 2 lists, to update and to delete")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Out parameters to be initialized within function")]
    public static void GetScheduleJobsToUpdateAndDelete(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionAfterUpdate,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionBeforeUpdate,
      out List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> jobsToUpdate,
      out List<Guid> jobIdsToDelete)
    {
      if (definitionAfterUpdate == null)
        throw new ArgumentNullException(nameof (definitionAfterUpdate));
      if (definitionBeforeUpdate == null)
        throw new ArgumentNullException(nameof (definitionBeforeUpdate));
      jobsToUpdate = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>();
      jobIdsToDelete = new List<Guid>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> jobsToUpdate1 = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>();
      List<Guid> jobIdsToDelete1 = new List<Guid>();
      definitionAfterUpdate.GetScheduleJobDefinitionsToUpdateAndDeleteFromTriggers(definitionBeforeUpdate, out jobsToUpdate1, out jobIdsToDelete1);
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> jobsToUpdate2 = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>();
      List<Guid> jobIdsToDelete2 = new List<Guid>();
      definitionAfterUpdate.GetScheduleJobDefinitionsToUpdateAndDeleteFromEnvironments(definitionBeforeUpdate, out jobsToUpdate2, out jobIdsToDelete2);
      jobsToUpdate.AddRange((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) jobsToUpdate1);
      jobsToUpdate.AddRange((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) jobsToUpdate2);
      jobIdsToDelete.AddRange((IEnumerable<Guid>) jobIdsToDelete1);
      jobIdsToDelete.AddRange((IEnumerable<Guid>) jobIdsToDelete2);
    }

    public static IList<int> GetEnvironmentDefinitionIdsWithSchedulesRemoved(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionAfterUpdate,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionBeforeUpdate)
    {
      if (definitionAfterUpdate == null)
        throw new ArgumentNullException(nameof (definitionAfterUpdate));
      if (definitionBeforeUpdate == null)
        throw new ArgumentNullException(nameof (definitionBeforeUpdate));
      List<int> schedulesRemoved = new List<int>();
      IEnumerable<DefinitionEnvironment> source = definitionBeforeUpdate.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>()));
      foreach (DefinitionEnvironment definitionEnvironment in definitionAfterUpdate.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => !e.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>())))
      {
        DefinitionEnvironment env = definitionEnvironment;
        if (source.Any<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Id == env.Id)))
          schedulesRemoved.Add(env.Id);
      }
      return (IList<int>) schedulesRemoved;
    }

    public static int GetDefinitionEnvironmentIdForSchedule(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionAfterUpdate,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule)
    {
      if (definitionAfterUpdate == null)
        throw new ArgumentNullException(nameof (definitionAfterUpdate));
      if (schedule == null)
        throw new ArgumentNullException(nameof (schedule));
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) definitionAfterUpdate.Environments)
      {
        if (environment.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, bool>) (s => s.JobId == schedule.JobId)))
          return environment.Id;
      }
      return 0;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition ToWebApiDefinitionV2(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition)
    {
      if (webApiDefinition == null)
        throw new ArgumentNullException(nameof (webApiDefinition));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinitionV2 = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition();
      webApiDefinitionV2.Source = webApiDefinition.Source;
      webApiDefinitionV2.Id = webApiDefinition.Id;
      webApiDefinitionV2.Revision = webApiDefinition.Revision;
      webApiDefinitionV2.Name = webApiDefinition.Name;
      webApiDefinitionV2.Description = webApiDefinition.Description;
      webApiDefinitionV2.CreatedBy = webApiDefinition.CreatedBy;
      webApiDefinitionV2.CreatedOn = webApiDefinition.CreatedOn;
      webApiDefinitionV2.ModifiedBy = webApiDefinition.ModifiedBy;
      webApiDefinitionV2.ModifiedOn = webApiDefinition.ModifiedOn;
      webApiDefinitionV2.Environments = webApiDefinition.Environments;
      webApiDefinitionV2.Artifacts = webApiDefinition.Artifacts;
      webApiDefinitionV2.ReleaseNameFormat = webApiDefinition.ReleaseNameFormat;
      webApiDefinitionV2.Path = webApiDefinition.Path;
      webApiDefinitionV2.LastRelease = webApiDefinition.LastRelease;
      webApiDefinitionV2.IsDisabled = webApiDefinition.IsDisabled;
      webApiDefinitionV2.Variables = webApiDefinition.Variables;
      webApiDefinitionV2.Triggers = webApiDefinition.Triggers;
      webApiDefinitionV2.Comment = webApiDefinition.Comment;
      webApiDefinitionV2.Url = webApiDefinition.Url;
      webApiDefinitionV2.Links = webApiDefinition.Links;
      webApiDefinitionV2.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) webApiDefinition.VariableGroups);
      webApiDefinitionV2.Properties.AddRange<KeyValuePair<string, object>, PropertiesCollection>((IEnumerable<KeyValuePair<string, object>>) webApiDefinition.Properties);
      webApiDefinitionV2.PipelineProcess = webApiDefinition.PipelineProcess;
      return webApiDefinitionV2;
    }

    public static void EnsureDefinitionDefaults(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition)
    {
      if (webApiDefinition == null)
        throw new ArgumentNullException(nameof (webApiDefinition));
      if (webApiDefinition.Environments == null || webApiDefinition.Environments.Count == 0)
        return;
      if (webApiDefinition.Environments.Count == 1 && webApiDefinition.Environments[0].Rank == 0)
        webApiDefinition.Environments[0].Rank = 1;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) webApiDefinition.Environments)
        ReleaseDefinitionExtensions.EnsureEnvironmentDefaults(context, projectId, environment);
    }

    public static void EnsureEnvironmentDefaults(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinitionEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.DeployStep == null)
        environment.DeployStep = new ReleaseDefinitionDeployStep()
        {
          Tasks = new List<WorkflowTask>()
        };
      if (environment.PreDeployApprovals == null)
        environment.PreDeployApprovals = environment.PreDeployApprovals.PopulateAutomatedApprovalIfApprovalsNotExist();
      if (environment.PostDeployApprovals == null)
        environment.PostDeployApprovals = environment.PostDeployApprovals.PopulateAutomatedApprovalIfApprovalsNotExist();
      if (environment.RetentionPolicy == null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy environmentRetentionPolicy = context.GetService<ReleaseSettingsService>().GetReleaseSettings(context, projectId).RetentionSettings.DefaultEnvironmentRetentionPolicy;
        environment.RetentionPolicy = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy()
        {
          DaysToKeep = environmentRetentionPolicy.DaysToKeep,
          ReleasesToKeep = environmentRetentionPolicy.ReleasesToKeep,
          RetainBuild = environmentRetentionPolicy.RetainBuild
        };
      }
      foreach (RunOnServerDeployPhase serverDeployPhase in environment.DeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (x => x.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.RunOnServer)))
      {
        if (serverDeployPhase.DeploymentInput.ParallelExecution.ParallelExecutionType == ParallelExecutionTypes.MultiConfiguration)
          ((ParallelExecutionInputBase) serverDeployPhase.DeploymentInput.ParallelExecution).MaxNumberOfAgents = 50;
      }
    }

    public static void SanitizeAndValidateNoDuplicateTaskRefNamesInPhase(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (releaseDefinition.Environments == null)
        return;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
      {
        if (environment != null)
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) environment.DeployPhases)
          {
            if (deployPhase != null)
            {
              IList<WorkflowTask> workflowTasks = deployPhase.WorkflowTasks;
              if (workflowTasks != null)
                workflowTasks.SanitizeTaskRefNames();
            }
            if (deployPhase != null)
            {
              IList<WorkflowTask> workflowTasks = deployPhase.WorkflowTasks;
              if (workflowTasks != null)
                workflowTasks.EnsureNoDuplicateRefNames();
            }
          }
        }
      }
    }

    internal static void GetScheduleJobDefinitionsToUpdateAndDeleteFromTriggers(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionAfterUpdate,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionBeforeUpdate,
      out List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> jobsToUpdate,
      out List<Guid> jobIdsToDelete)
    {
      List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger>> schedulesAddedWithParent;
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> schedulesUpdated;
      ReleaseDefinitionExtensions.FindDeltaSchedules<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger>(definitionBeforeUpdate, definitionAfterUpdate, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger>>) (serverDef => (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger>) serverDef.ScheduledTriggers), (Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>>) (scheduledTrigger => (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>()
      {
        scheduledTrigger.Schedule
      }), out schedulesAddedWithParent, out schedulesUpdated, out jobIdsToDelete);
      ReleaseDefinitionExtensions.SetJobIdForNewSchedules<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger>(schedulesAddedWithParent, (Action<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger>>) (scheduleAndParent => scheduleAndParent.Value.SetReleaseScheduleJobId(Guid.NewGuid())));
      jobsToUpdate = ReleaseDefinitionExtensions.GetScheduleJobsToUpdate<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger>(schedulesAddedWithParent, schedulesUpdated);
    }

    private static void FindDeltaSchedules<TScheduleParent>(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition before,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition after,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IList<TScheduleParent>> getScheduleParents,
      Func<TScheduleParent, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>> getSchedules,
      out List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>> schedulesAddedWithParent,
      out List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> schedulesUpdated,
      out List<Guid> jobIdsToDelete)
    {
      schedulesUpdated = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>();
      jobIdsToDelete = new List<Guid>();
      Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent> serverDefinition1 = ReleaseDefinitionExtensions.GetSchedulesFromServerDefinition<TScheduleParent>(before, getScheduleParents, getSchedules, out List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>> _);
      Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent> serverDefinition2 = ReleaseDefinitionExtensions.GetSchedulesFromServerDefinition<TScheduleParent>(after, getScheduleParents, getSchedules, out schedulesAddedWithParent);
      HashSet<Guid> guidSet = new HashSet<Guid>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule key in serverDefinition2.Keys)
      {
        TScheduleParent scheduleParent;
        serverDefinition1.TryGetValue(key, out scheduleParent);
        if ((object) scheduleParent == null)
          schedulesUpdated.Add(key);
        guidSet.Add(key.JobId);
      }
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule key in serverDefinition1.Keys)
      {
        if (!guidSet.Contains(key.JobId))
          jobIdsToDelete.Add(key.JobId);
      }
    }

    private static Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent> GetSchedulesFromServerDefinition<TScheduleParent>(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IList<TScheduleParent>> getScheduleParents,
      Func<TScheduleParent, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>> getSchedules,
      out List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>> schedulesAddedWithParent)
    {
      schedulesAddedWithParent = new List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>>();
      Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent> serverDefinition1 = new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>();
      foreach (TScheduleParent scheduleParent in (IEnumerable<TScheduleParent>) getScheduleParents(serverDefinition))
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule key in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) getSchedules(scheduleParent))
        {
          if (key.JobId == Guid.Empty)
            schedulesAddedWithParent.Add(new KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>(key, scheduleParent));
          else
            serverDefinition1[key] = scheduleParent;
        }
      }
      return serverDefinition1;
    }

    internal static void GetScheduleJobDefinitionsToUpdateAndDeleteFromEnvironments(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionAfterUpdate,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definitionBeforeUpdate,
      out List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> jobsToUpdate,
      out List<Guid> jobIdsToDelete)
    {
      List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, DefinitionEnvironment>> schedulesAddedWithParent;
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> schedulesUpdated;
      ReleaseDefinitionExtensions.FindDeltaSchedules<DefinitionEnvironment>(definitionBeforeUpdate, definitionAfterUpdate, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IList<DefinitionEnvironment>>) (serverDef => serverDef.Environments), (Func<DefinitionEnvironment, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>>) (envDef => envDef.Schedules), out schedulesAddedWithParent, out schedulesUpdated, out jobIdsToDelete);
      ReleaseDefinitionExtensions.SetJobIdForNewSchedules<DefinitionEnvironment>(schedulesAddedWithParent, (Action<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, DefinitionEnvironment>>) (scheduleAndParent => scheduleAndParent.Key.JobId = Guid.NewGuid()));
      jobsToUpdate = ReleaseDefinitionExtensions.GetScheduleJobsToUpdate<DefinitionEnvironment>(schedulesAddedWithParent, schedulesUpdated);
    }

    private static void SetJobIdForNewSchedules<TScheduleParent>(
      List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>> schedulesAddedWithParent,
      Action<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>> setJobIdForNewSchedule)
    {
      foreach (KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent> keyValuePair in schedulesAddedWithParent)
        setJobIdForNewSchedule(keyValuePair);
    }

    private static List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> GetScheduleJobsToUpdate<TScheduleParent>(
      List<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>> schedulesAddedWithParent,
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> schedulesUpdated)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule> scheduleJobsToUpdate = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>();
      scheduleJobsToUpdate.AddRange((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) schedulesAddedWithParent.Select<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>((Func<KeyValuePair<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule, TScheduleParent>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) (kvp => kvp.Key)).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>());
      scheduleJobsToUpdate.AddRange((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) schedulesUpdated);
      return scheduleJobsToUpdate;
    }

    public static TeamFoundationJobDefinition CreateScheduleReleaseJobDefinition(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule,
      int releaseDefinitionId,
      Guid projectId,
      bool isIgnoreDormancyPermitted,
      bool isDefaultJobPriorityHigh)
    {
      if (schedule == null)
        throw new ArgumentNullException(nameof (schedule));
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Scheduled Release Job");
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new ScheduleReleaseJobData()
      {
        ProjectId = projectId,
        ReleaseDefinitionId = releaseDefinitionId,
        ScheduleOnlyWithChanges = schedule.ScheduleOnlyWithChanges
      });
      TeamFoundationJobDefinition releaseJobDefinition = new TeamFoundationJobDefinition();
      releaseJobDefinition.Data = xml;
      releaseJobDefinition.EnabledState = TeamFoundationJobEnabledState.Enabled;
      releaseJobDefinition.ExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtensions.ScheduleReleaseJobExtension";
      releaseJobDefinition.JobId = schedule.JobId;
      releaseJobDefinition.Name = str;
      releaseJobDefinition.IgnoreDormancy = isIgnoreDormancyPermitted;
      releaseJobDefinition.PriorityClass = isDefaultJobPriorityHigh ? JobPriorityClass.AboveNormal : JobPriorityClass.Normal;
      JobPriorityLevel priorityLevel = isDefaultJobPriorityHigh ? JobPriorityLevel.Highest : JobPriorityLevel.Normal;
      releaseJobDefinition.Schedule.AddRange((IEnumerable<TeamFoundationJobSchedule>) ReleaseDefinitionExtensions.ConvertSchedules(schedule, priorityLevel));
      return releaseJobDefinition;
    }

    public static TeamFoundationJobDefinition CreateScheduleEnvironmentJobDefinition(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      Guid projectId,
      bool isIgnoreDormancyPermitted)
    {
      if (schedule == null)
        throw new ArgumentNullException(nameof (schedule));
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Scheduled Environment Job");
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new ScheduleEnvironmentJobData()
      {
        ProjectId = projectId,
        ReleaseDefinitionId = releaseDefinitionId,
        EnvironmentId = definitionEnvironmentId
      });
      TeamFoundationJobDefinition environmentJobDefinition = new TeamFoundationJobDefinition()
      {
        Data = xml,
        EnabledState = TeamFoundationJobEnabledState.Enabled,
        ExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtensions.ScheduleEnvironmentJobExtension",
        JobId = schedule.JobId,
        Name = str,
        IgnoreDormancy = isIgnoreDormancyPermitted,
        PriorityClass = JobPriorityClass.High
      };
      environmentJobDefinition.Schedule.AddRange((IEnumerable<TeamFoundationJobSchedule>) ReleaseDefinitionExtensions.ConvertSchedules(schedule));
      if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.UseDirectReleaseStageScheduling"))
        environmentJobDefinition.EnabledState = TeamFoundationJobEnabledState.SchedulesDisabled;
      return environmentJobDefinition;
    }

    public static TeamFoundationJobDefinition CreateScheduleStageJobDefinition(
      int releaseId,
      int releaseDefinitionId,
      int stageId,
      int stageDefinitionId,
      Guid projectId,
      bool isIgnoreDormancyPermitted)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new ScheduleStageJobData()
      {
        ProjectId = projectId,
        ReleaseId = releaseId,
        ReleaseDefinitionId = releaseDefinitionId,
        StageId = stageId,
        StageDefinitionId = stageDefinitionId
      });
      ArgumentUtility.CheckForNull<XmlNode>(xml, "jobData");
      return new TeamFoundationJobDefinition()
      {
        JobId = Guid.NewGuid(),
        Data = xml,
        EnabledState = TeamFoundationJobEnabledState.Enabled,
        ExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtensions.ScheduleStageJobExtension",
        Name = "Scheduled Stage Job",
        IgnoreDormancy = isIgnoreDormancyPermitted,
        PriorityClass = JobPriorityClass.High
      };
    }

    public static IList<TeamFoundationJobSchedule> ConvertSchedules(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Highest)
    {
      List<TeamFoundationJobSchedule> foundationJobScheduleList = new List<TeamFoundationJobSchedule>();
      DateTime dateTime1 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, schedule.TimeZoneId);
      DateTime dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, schedule.StartHours, schedule.StartMinutes, 0, 0);
      DateTime utc = TimeZoneInfo.ConvertTimeToUtc(dateTime2, TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId));
      DateTime dateTime3;
      ref DateTime local = ref dateTime3;
      DateTime utcNow = DateTime.UtcNow;
      int year = utcNow.Year;
      utcNow = DateTime.UtcNow;
      int month = utcNow.Month;
      utcNow = DateTime.UtcNow;
      int day = utcNow.Day;
      local = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
      while (dateTime3.DayOfWeek != DayOfWeek.Sunday)
        dateTime3 = dateTime3.AddDays(-1.0);
      DateTime dateTime4 = dateTime3.Add(new TimeSpan(utc.Hour, utc.Minute, 0));
      int num = 0;
      if (utc.Day != dateTime2.Day)
        num = utc.Subtract(dateTime2).Ticks > 0L ? 1 : -1;
      if (schedule.DaysToRelease == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.All)
      {
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 86400,
          ScheduledTime = dateTime4.AddDays((double) num),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
        return (IList<TeamFoundationJobSchedule>) foundationJobScheduleList;
      }
      if ((schedule.DaysToRelease & Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.Sunday) != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 604800,
          ScheduledTime = dateTime4.AddDays((double) num),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
      if ((schedule.DaysToRelease & Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.Monday) != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 604800,
          ScheduledTime = dateTime4.AddDays((double) (1 + num)),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
      if ((schedule.DaysToRelease & Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.Tuesday) != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 604800,
          ScheduledTime = dateTime4.AddDays((double) (2 + num)),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
      if ((schedule.DaysToRelease & Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.Wednesday) != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 604800,
          ScheduledTime = dateTime4.AddDays((double) (3 + num)),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
      if ((schedule.DaysToRelease & Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.Thursday) != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 604800,
          ScheduledTime = dateTime4.AddDays((double) (4 + num)),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
      if ((schedule.DaysToRelease & Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.Friday) != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 604800,
          ScheduledTime = dateTime4.AddDays((double) (5 + num)),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
      if ((schedule.DaysToRelease & Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.Saturday) != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = 604800,
          ScheduledTime = dateTime4.AddDays((double) (6 + num)),
          TimeZoneId = schedule.TimeZoneId,
          PriorityLevel = priorityLevel
        });
      return (IList<TeamFoundationJobSchedule>) foundationJobScheduleList;
    }

    private static void SetReleaseScheduleJobId(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger trigger, Guid jobId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule = trigger.Schedule;
      schedule.JobId = jobId;
      trigger.Schedule = schedule;
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition ToContractInternal(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      IVssRequestContext context,
      Guid projectId)
    {
      ReleaseDefinitionExtensions.PopulateArtifactTypeName(context, serverDefinition, projectId);
      return ReleaseDefinitionConverter.ToWebApi(context, projectId, serverDefinition);
    }

    public static void ValidateWorkflowTasks(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition,
      IVssRequestContext context,
      Guid projectId)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      List<TaskDefinition> allTaskDefinitions = TaskDefinitionsHelper.GetAllTaskDefinitions(context, projectId, definition.HasMetaTask());
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
        environment.DeployPhases.ValidateTasks(environment.Name, (IList<TaskDefinition>) allTaskDefinitions);
    }

    public static void ValidateEnvironmentRanks(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (definition.Environments.Count <= 0)
        return;
      IOrderedEnumerable<ReleaseDefinitionEnvironment> orderedEnumerable = definition.Environments.OrderBy<ReleaseDefinitionEnvironment, int>((Func<ReleaseDefinitionEnvironment, int>) (x => x.Rank));
      int num = 1;
      foreach (ReleaseDefinitionEnvironment definitionEnvironment in (IEnumerable<ReleaseDefinitionEnvironment>) orderedEnumerable)
      {
        if (definitionEnvironment.Rank != num++)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.DefinitionEnvironmentRanksNotValid, (object) definitionEnvironment.Name));
      }
    }

    public static void RemoveDuplicateConditions(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
      {
        if (environment.Conditions.Count > 1)
        {
          List<Condition> conditionList = new List<Condition>();
          foreach (Condition condition1 in (IEnumerable<Condition>) environment.Conditions)
          {
            Condition condition = condition1;
            if (!conditionList.Any<Condition>((Func<Condition, bool>) (c => c.Equals((object) condition))))
              conditionList.Add(condition);
          }
          environment.Conditions.Clear();
          environment.Conditions.AddRange<Condition, IList<Condition>>((IEnumerable<Condition>) conditionList);
        }
      }
    }

    public static void ValidateEnvironmentConditionsLength(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      List<string> stringList = new List<string>();
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
      {
        if ((environment.Conditions != null ? JsonConvert.SerializeObject((object) environment.Conditions) : string.Empty).Length > 4000)
          stringList.Add(environment.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      if (stringList.Any<string>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.InvalidEnvironmentConditionsLength, (object) string.Join(",", (IEnumerable<string>) stringList)));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder ToDataModelQueryOrder(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionQueryOrder queryOrder)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder dataModelQueryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.IdAscending;
      switch (queryOrder)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionQueryOrder.IdAscending:
          dataModelQueryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.IdAscending;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionQueryOrder.IdDescending:
          dataModelQueryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.IdDescending;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionQueryOrder.NameAscending:
          dataModelQueryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.NameAscending;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionQueryOrder.NameDescending:
          dataModelQueryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionQueryOrder.NameDescending;
          break;
      }
      return dataModelQueryOrder;
    }

    public static ArtifactSpec CreateArtifactSpec(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      Guid dataspaceIdentifier)
    {
      if (serverDefinition == null)
        throw new ArgumentNullException(nameof (serverDefinition));
      return new ArtifactSpec(ReleaseManagementArtifactPropertyKinds.ReleaseDefinition, serverDefinition.Id, 0, dataspaceIdentifier);
    }

    public static void PopulateProperties(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier,
      IEnumerable<string> propertiesFilter)
    {
      if (serverDefinition == null)
        throw new ArgumentNullException(nameof (serverDefinition));
      ITeamFoundationPropertyService foundationPropertyService = requestContext != null ? requestContext.GetService<ITeamFoundationPropertyService>() : throw new ArgumentNullException(nameof (requestContext));
      if (foundationPropertyService.GetArtifactKinds(requestContext).Any<ArtifactKind>((Func<ArtifactKind, bool>) (ak => ak.Kind.Equals(ReleaseManagementArtifactPropertyKinds.ReleaseDefinition))))
      {
        using (TeamFoundationDataReader properties = foundationPropertyService.GetProperties(requestContext, serverDefinition.CreateArtifactSpec(dataspaceIdentifier), propertiesFilter))
        {
          foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
            serverDefinition.Properties.AddRange<PropertyValue, IList<PropertyValue>>((IEnumerable<PropertyValue>) current.PropertyValues);
        }
      }
      List<ArtifactSpec> source = new List<ArtifactSpec>();
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) serverDefinition.Environments)
      {
        if (environment != null && foundationPropertyService.GetArtifactKinds(requestContext).Any<ArtifactKind>((Func<ArtifactKind, bool>) (ak => ak.Kind.Equals(ReleaseManagementArtifactPropertyKinds.DefinitionEnvironment))))
          source.Add(new ArtifactSpec(ReleaseManagementArtifactPropertyKinds.DefinitionEnvironment, environment.Id, 0, dataspaceIdentifier));
      }
      if (!source.Any<ArtifactSpec>())
        return;
      using (TeamFoundationDataReader properties = foundationPropertyService.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) source, propertiesFilter))
      {
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
        {
          ArtifactPropertyValue artifact = current;
          if (artifact != null && artifact.Spec != null && artifact.Spec.Id != null && artifact.Spec.Id.Length >= 4)
          {
            DefinitionEnvironment definitionEnvironment = serverDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Id == ReleaseManagementArtifactPropertyKinds.ToInt32(artifact.Spec.Id))).FirstOrDefault<DefinitionEnvironment>();
            if (definitionEnvironment != null)
              definitionEnvironment.Properties.AddRange<PropertyValue, IList<PropertyValue>>((IEnumerable<PropertyValue>) artifact.PropertyValues);
          }
        }
      }
    }

    public static void AddScheduledJobs(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition newlyCreatedDefinition,
      IVssRequestContext context,
      Guid projectId)
    {
      ITeamFoundationJobService jobService = ReleaseOperationHelper.GetJobService(context);
      IList<TeamFoundationJobDefinition> definitionsToAdd = newlyCreatedDefinition.GetScheduleJobDefinitionsToAdd(context, projectId, jobService.IsIgnoreDormancyPermitted);
      if (!definitionsToAdd.Any<TeamFoundationJobDefinition>())
        return;
      ReleaseOperationHelper.UpdateJobs(context, (IEnumerable<TeamFoundationJobDefinition>) definitionsToAdd, (IEnumerable<Guid>) null);
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference ToContract(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionShallowReference releaseDefinitionShallowReference)
    {
      int definitionId = releaseDefinitionShallowReference != null ? releaseDefinitionShallowReference.Id : throw new ArgumentNullException(nameof (releaseDefinitionShallowReference));
      Guid projectId = releaseDefinitionShallowReference.ProjectId;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference contract = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference();
      contract.Id = definitionId;
      contract.ProjectReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ProjectReference()
      {
        Id = projectId
      };
      contract.Name = releaseDefinitionShallowReference.Name;
      string definitionWebAccessUri = WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(requestContext, projectId.ToString(), definitionId);
      string definitionRestUrl = WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(requestContext, projectId, definitionId);
      contract.Links.AddLink("web", definitionWebAccessUri);
      contract.Links.AddLink("self", definitionRestUrl);
      return contract;
    }
  }
}
