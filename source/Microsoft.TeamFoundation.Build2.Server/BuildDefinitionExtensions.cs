// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.FeatureAvailability.PipelineComponents;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildDefinitionExtensions
  {
    private const string c_oldProcessParameterPrefix = "ProcParam.";
    private const string c_newProcessParameterPrefix = "Parameters.";
    private static readonly HashSet<string> ReservedBuildVariableNames = new HashSet<string>((IEnumerable<string>) new string[5]
    {
      "build.buildId",
      "build.buildNumber",
      "build.buildUri",
      "build.containerId",
      "system.teamProjectId"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly string s_registryIgnoreNewlyCreatedDefinitions = "/Service/Pipelines/providers/github/externalgitevent/ignorenewlycreateddefinitioninseconds";

    public static T GetProcess<T>(this BuildDefinition definition) where T : BuildProcess
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<BuildProcess>(definition.Process, "Process");
      ArgumentUtility.CheckType<T>((object) definition.Process, "Process", nameof (T));
      return definition.Process as T;
    }

    public static YamlPipelineLoadResult LoadYamlPipeline(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      bool authorizeNewResources,
      BuildProcessResources previouslyAuthorizedResources = null)
    {
      return definition.LoadYamlPipeline(requestContext, (string) null, (string) null, authorizeNewResources, previouslyAuthorizedResources);
    }

    public static YamlPipelineLoadResult LoadYamlPipeline(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      string sourceBranch,
      string sourceVersion,
      bool authorizeNewResources,
      BuildProcessResources previouslyAuthorizedResources = null,
      RetrieveOptions retrieveOptions = RetrieveOptions.All,
      Dictionary<string, object> templateParameters = null,
      RepositoryResource repositoryResource = null,
      string yamlOverride = null,
      string yamlFileName = null,
      bool validateLogicalBoundaries = false)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<BuildRepository>(definition.Repository, "Repository");
      YamlProcess process = definition.GetProcess<YamlProcess>();
      previouslyAuthorizedResources = previouslyAuthorizedResources ?? new BuildProcessResources();
      PipelineResources pipelineResources = previouslyAuthorizedResources.ToPipelineResources();
      if (string.IsNullOrEmpty(sourceBranch))
        sourceBranch = definition.Repository.DefaultBranch;
      PipelineBuilder pipelineBuilder = definition.GetPipelineBuilder(requestContext, pipelineResources, authorizeNewResources);
      bool includeCheckoutOptions = requestContext.IsFeatureEnabled("DistributedTask.IncludeCheckoutOptions");
      RepositoryResource repositoryResource1 = repositoryResource ?? definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, sourceBranch, sourceVersion, includeCheckoutOptions);
      IYamlPipelineLoaderService service = requestContext.GetService<IYamlPipelineLoaderService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = definition.ProjectId;
      RepositoryResource repository = repositoryResource1;
      string filePath = yamlFileName ?? process.YamlFilename;
      PipelineBuilder builder = pipelineBuilder;
      int? id = definition.Queue?.Id;
      int retrieveOptions1 = (int) retrieveOptions;
      IDictionary<string, object> dictionary = (IDictionary<string, object>) templateParameters;
      string str = yamlOverride;
      int num = validateLogicalBoundaries ? 1 : 0;
      string yamlOverride1 = str;
      IDictionary<string, object> templateParameters1 = dictionary;
      return service.Load(requestContext1, projectId, repository, filePath, builder, id, retrieveOptions: (RetrieveOptions) retrieveOptions1, validateLogicalBoundaries: num != 0, yamlOverride: yamlOverride1, templateParameters: templateParameters1);
    }

    internal static IEnumerable<Guid> GetScheduleTriggerJobIds(this BuildDefinition definition) => definition.Triggers.Where<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.Schedule)).Cast<ScheduleTrigger>().SelectMany<ScheduleTrigger, Schedule>((Func<ScheduleTrigger, IEnumerable<Schedule>>) (st => (IEnumerable<Schedule>) st.Schedules)).Select<Schedule, Guid>((Func<Schedule, Guid>) (s => s.ScheduleJobId));

    public static void ModernizeOptions(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      IList<IBuildOption> options)
    {
      foreach (IBuildOption option1 in (IEnumerable<IBuildOption>) options)
      {
        IBuildOption buildOption = option1;
        BuildOption option2 = definition.Options.FirstOrDefault<BuildOption>((Func<BuildOption, bool>) (x => x != null && x.Definition != null && x.Definition.Id == buildOption.GetId()));
        if (option2 != null || buildOption.GetId() == new Guid("{57578776-4C22-4526-AEB0-86B6DA17EE9C}"))
          buildOption.AfterDeserialize(requestContext, option2, definition);
      }
    }

    internal static ArtifactSpec CreateArtifactSpec(
      this BuildDefinition value,
      IVssRequestContext context)
    {
      return context.GetService<ITeamFoundationResourceManagementService>().GetServiceVersion(context, "Build2", "Build").Version < 8 ? new ArtifactSpec(ArtifactPropertyKinds.Definition, value.Id, 0) : new ArtifactSpec(ArtifactPropertyKinds.Definition, value.Id, 0, value.ProjectId);
    }

    internal static void PopulateProperties(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      definition.PopulateProperties(requestContext, (IEnumerable<string>) null);
    }

    internal static void PopulateVariableGroups(
      this IReadOnlyList<BuildDefinition> definitions,
      IVssRequestContext requestContext)
    {
      if (definitions == null)
        return;
      foreach (IGrouping<Guid, BuildDefinition> source in definitions.GroupBy<BuildDefinition, Guid>((Func<BuildDefinition, Guid>) (d => d.ProjectId)))
      {
        List<int> list = source.SelectMany<BuildDefinition, int>((Func<BuildDefinition, IEnumerable<int>>) (d => d.VariableGroups.Select<VariableGroup, int>((Func<VariableGroup, int>) (vg => vg.Id)))).Distinct<int>().ToList<int>();
        if (list.Count > 0)
        {
          IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = requestContext.GetService<IVariableGroupService>().GetVariableGroups(requestContext, source.Key, (IList<int>) list);
          if (variableGroups != null && variableGroups.Count > 0)
          {
            Dictionary<int, VariableGroup> dictionary = variableGroups.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, VariableGroup>) (vg => vg.ToBuildDefinitionVariableGroup())).ToDictionary<VariableGroup, int, VariableGroup>((Func<VariableGroup, int>) (x => x.Id), (Func<VariableGroup, VariableGroup>) (x => x));
            foreach (BuildDefinition buildDefinition in (IEnumerable<BuildDefinition>) source)
            {
              for (int index = 0; index < buildDefinition.VariableGroups.Count; ++index)
              {
                VariableGroup variableGroup;
                if (dictionary.TryGetValue(buildDefinition.VariableGroups[index].Id, out variableGroup))
                  buildDefinition.VariableGroups[index] = variableGroup;
              }
            }
          }
        }
      }
    }

    internal static void PopulateVariableGroups(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      ((IReadOnlyList<BuildDefinition>) new BuildDefinition[1]
      {
        definition
      }).PopulateVariableGroups(requestContext);
    }

    public static void PopulateProperties(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      IEnumerable<string> propertiesFilter)
    {
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, definition.CreateArtifactSpec(requestContext), propertiesFilter))
      {
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
          definition.Properties = current.PropertyValues.Convert();
      }
    }

    internal static void ApplyPreValidationOptions(
      this BuildDefinition definition,
      IList<IBuildOption> options,
      IOrchestrationEnvironment environment,
      List<TaskOrchestrationJob> jobs)
    {
      foreach (IBuildOption option1 in (IEnumerable<IBuildOption>) options)
      {
        IBuildOption buildOption = option1;
        BuildOption option2 = definition.Options.FirstOrDefault<BuildOption>((Func<BuildOption, bool>) (x => x.Enabled && x.Definition.Id == buildOption.GetId()));
        if (option2 != null)
          buildOption.ApplyPreValidation(option2, environment, jobs);
      }
    }

    internal static void ApplyPreValidationOptions(
      this BuildDefinition definition,
      IList<IBuildOption> options,
      IOrchestrationEnvironment environment)
    {
      foreach (IBuildOption option1 in (IEnumerable<IBuildOption>) options)
      {
        IBuildOption buildOption = option1;
        BuildOption option2 = definition.Options.FirstOrDefault<BuildOption>((Func<BuildOption, bool>) (x => x.Enabled && x.Definition.Id == buildOption.GetId()));
        if (option2 != null)
          buildOption.ApplyPreValidation(option2, environment);
      }
    }

    internal static void BuildQueued(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      BuildData build,
      IOrchestrationProcess process)
    {
      if (build.Reason != BuildReason.CheckInShelveset)
        return;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "_Build_{0}", (object) build.Id);
      build.Properties.Add(BuildProperties.GatedShelvesetName, (object) str);
      if (!(definition.Triggers.FirstOrDefault<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.GatedCheckIn)) is GatedCheckInTrigger gatedCheckInTrigger))
        return;
      if (process.ProcessType == OrchestrationProcessType.Pipeline)
      {
        PhaseNode phase = (process as PipelineProcess).Stages[0].Phases[0];
        phase.Variables.Add((IVariable) new Variable()
        {
          Name = "build.gated.runci",
          Value = gatedCheckInTrigger.RunContinuousIntegration.ToString()
        });
        phase.Variables.Add((IVariable) new Variable()
        {
          Name = "build.gated.shelvesetname",
          Value = str
        });
      }
      else
      {
        TaskOrchestrationJob orchestrationJob = (process as TaskOrchestrationContainer).GetJobs().FirstOrDefault<TaskOrchestrationJob>();
        if (orchestrationJob == null)
          return;
        orchestrationJob.Variables.Add("build.gated.runci", gatedCheckInTrigger.RunContinuousIntegration.ToString());
        orchestrationJob.Variables.Add("build.gated.shelvesetname", str);
      }
    }

    internal static void PostBuildOperations(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      IList<IBuildOption> options,
      TaskOrchestrationPlan plan,
      BuildData build)
    {
      foreach (IBuildOption option1 in (IEnumerable<IBuildOption>) options)
      {
        IBuildOption buildOption = option1;
        BuildOption option2 = definition.Options.FirstOrDefault<BuildOption>((Func<BuildOption, bool>) (x => x.Enabled && x.Definition.Id == buildOption.GetId()));
        if (option2 != null)
          buildOption.PostBuildOperations(requestContext, option2, plan, build, definition);
      }
    }

    internal static void SetAdditionalContainerInputsOptions(
      this BuildDefinition definition,
      BuildData build,
      IList<IBuildOption> options,
      TaskOrchestrationContainer container)
    {
      foreach (IBuildOption option1 in (IEnumerable<IBuildOption>) options)
      {
        IBuildOption buildOption = option1;
        BuildOption option2 = definition.Options.FirstOrDefault<BuildOption>((Func<BuildOption, bool>) (x => x.Enabled && x.Definition.Id == buildOption.GetId()));
        if (option2 != null)
          buildOption.SetAdditionalContainerInputs(option2, container);
      }
    }

    internal static void CheckSupportedBuildOptions(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      ApiResourceVersion apiResourceVersion)
    {
      if (definition == null)
        return;
      HashSet<Guid> checkedOptions = new HashSet<Guid>();
      foreach (BuildOption buildOption in definition.Options.Where<BuildOption>((Func<BuildOption, bool>) (o => o.Definition != null && !checkedOptions.Contains(o.Definition.Id))))
      {
        BuildOption option = buildOption;
        checkedOptions.Add(option.Definition.Id);
        requestContext.GetExtension<IBuildOption>((Func<IBuildOption, bool>) (bo => bo.GetId() == option.Definition.Id))?.CheckSupported(requestContext, apiResourceVersion, definition.Process != null ? definition.Process.Type : 1);
      }
    }

    public static bool IsTooNewForTriggers(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      if (definition.Revision.HasValue)
      {
        int? revision = definition.Revision;
        int num = 1;
        if (revision.GetValueOrDefault() > num & revision.HasValue)
          return false;
      }
      int num1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) BuildDefinitionExtensions.s_registryIgnoreNewlyCreatedDefinitions, 60);
      bool flag = Math.Abs((DateTime.UtcNow - definition.CreatedDate).TotalSeconds) < (double) num1;
      if (flag)
        requestContext.TraceInfo(12030204, "Build2", "Ignoring definition {0} because it was created w/in {1} seconds of this event. Created: '{2}', Now: '{3}'.", (object) definition.Id, (object) num1, (object) definition.CreatedDate, (object) DateTime.UtcNow);
      return flag;
    }

    internal static void UpdateReferences(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionExtensions), nameof (UpdateReferences)))
      {
        if (definition == null)
          return;
        if (definition.Queue != null && string.IsNullOrEmpty(definition.Queue.Name))
        {
          TaskAgentQueue agentQueue = requestContext.GetService<IDistributedTaskPoolService>().GetAgentQueue(requestContext, definition.ProjectId, definition.Queue.Id);
          if (agentQueue != null)
            agentQueue.CopyTo(definition.Queue);
        }
        if (definition.ProjectId != Guid.Empty)
        {
          IProjectService service = requestContext.GetService<IProjectService>();
          definition.ProjectName = service.GetProjectName(requestContext, definition.ProjectId);
        }
        definition.ConvertTriggerPathsToProjectName(requestContext);
        definition.ConvertTaskParametersToProjectName(requestContext);
        definition.ConvertVariablesToProjectName(requestContext);
      }
    }

    public static Guid? GetAuthoredById(this BuildDefinition definition)
    {
      Guid? authoredById = new Guid?();
      if (definition != null)
      {
        Guid authoredBy = definition.AuthoredBy;
        authoredById = new Guid?(definition.AuthoredBy);
      }
      return authoredById;
    }

    internal static TeamFoundationJobDefinition GetPollingJobDefinition(
      this BuildDefinition definition,
      bool isIgnoreDormancyPermitted = true,
      string lastSourceVersionBuilt = null,
      string currentConnectionId = null,
      string lastFailedBuildUtcDate = null)
    {
      TeamFoundationJobDefinition pollingJobDefinition = (TeamFoundationJobDefinition) null;
      if (definition.ShouldCreateTriggerJobDefinition())
      {
        ContinuousIntegrationTrigger trigger = definition.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>();
        if (trigger.IsPollingEnabled())
        {
          XmlDocument xmlDocument = new XmlDocument();
          XmlNode element1 = (XmlNode) xmlDocument.CreateElement("BuildDefinition");
          XmlNode element2 = (XmlNode) xmlDocument.CreateElement("ProjectId");
          element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(definition.ProjectId.ToString()));
          XmlNode element3 = (XmlNode) xmlDocument.CreateElement("DefinitionId");
          element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(definition.Id.ToString()));
          XmlNode element4 = (XmlNode) xmlDocument.CreateElement("LastVersionEvaluated");
          element4.AppendChild((XmlNode) xmlDocument.CreateTextNode(string.IsNullOrEmpty(lastSourceVersionBuilt) ? string.Empty : lastSourceVersionBuilt));
          XmlNode element5 = (XmlNode) xmlDocument.CreateElement("CurrentConnectionId");
          element5.AppendChild((XmlNode) xmlDocument.CreateTextNode(string.IsNullOrEmpty(currentConnectionId) ? string.Empty : currentConnectionId));
          XmlNode element6 = (XmlNode) xmlDocument.CreateElement("LastFailedBuildDateTime");
          element6.AppendChild((XmlNode) xmlDocument.CreateTextNode(string.IsNullOrEmpty(lastFailedBuildUtcDate) ? string.Empty : lastFailedBuildUtcDate));
          element1.AppendChild(element2);
          element1.AppendChild(element3);
          element1.AppendChild(element4);
          element1.AppendChild(element5);
          element1.AppendChild(element6);
          pollingJobDefinition = new TeamFoundationJobDefinition()
          {
            Data = element1,
            EnabledState = TeamFoundationJobEnabledState.Enabled,
            ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.BuildPollingJobExtension",
            JobId = trigger.PollingJobId,
            Name = BuildServerResources.PollingJobName(),
            IgnoreDormancy = isIgnoreDormancyPermitted,
            PriorityClass = JobPriorityClass.Normal
          };
          TeamFoundationJobSchedule foundationJobSchedule = new TeamFoundationJobSchedule()
          {
            Interval = trigger.PollingInterval.Value,
            ScheduledTime = DateTime.UtcNow,
            PriorityLevel = JobPriorityLevel.BelowNormal
          };
          pollingJobDefinition.Schedule.Add(foundationJobSchedule);
        }
      }
      return pollingJobDefinition;
    }

    internal static List<TeamFoundationJobDefinition> GetScheduleJobDefinitions(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      bool isIgnoreDormancyPermitted = true)
    {
      List<TeamFoundationJobDefinition> scheduleJobDefinitions = new List<TeamFoundationJobDefinition>();
      if (definition.ShouldCreateTriggerJobDefinition())
      {
        List<BuildTrigger> list = definition.Triggers.Where<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.Schedule)).ToList<BuildTrigger>();
        if (list.Count > 0)
        {
          foreach (Schedule schedule in (list[0] as ScheduleTrigger).Schedules)
          {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode element1 = (XmlNode) xmlDocument.CreateElement("BuildDefinition");
            XmlNode element2 = (XmlNode) xmlDocument.CreateElement("ProjectId");
            element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(definition.ProjectId.ToString()));
            XmlNode element3 = (XmlNode) xmlDocument.CreateElement("DefinitionId");
            element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(definition.Id.ToString()));
            XmlNode element4 = (XmlNode) xmlDocument.CreateElement("BranchFilters");
            foreach (string branchFilter in schedule.BranchFilters)
              element4.AppendChild((XmlNode) xmlDocument.CreateElement("BranchFilter")).AppendChild((XmlNode) xmlDocument.CreateTextNode(branchFilter));
            XmlNode element5 = (XmlNode) xmlDocument.CreateElement("TriggerType");
            element5.AppendChild((XmlNode) xmlDocument.CreateTextNode(DefinitionTriggerType.Schedule.ToString("G")));
            XmlNode element6 = (XmlNode) xmlDocument.CreateElement("ScheduleOnlyWithChanges");
            element6.AppendChild((XmlNode) xmlDocument.CreateTextNode(schedule.ScheduleOnlyWithChanges.ToString()));
            XmlNode element7 = (XmlNode) xmlDocument.CreateElement("RetriesCount");
            element7.AppendChild((XmlNode) xmlDocument.CreateTextNode("0"));
            element1.AppendChild(element2);
            element1.AppendChild(element3);
            element1.AppendChild(element5);
            element1.AppendChild(element4);
            element1.AppendChild(element6);
            element1.AppendChild(element7);
            TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition()
            {
              Data = element1,
              EnabledState = TeamFoundationJobEnabledState.Enabled,
              ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.BuildScheduleJobExtension",
              JobId = schedule.ScheduleJobId,
              Name = BuildServerResources.ScheduleJobName(),
              IgnoreDormancy = isIgnoreDormancyPermitted,
              PriorityClass = JobPriorityClass.High
            };
            foundationJobDefinition.Schedule.AddRange((IEnumerable<TeamFoundationJobSchedule>) BuildDefinitionExtensions.ConvertSchedule(schedule, requestContext));
            scheduleJobDefinitions.Add(foundationJobDefinition);
          }
        }
      }
      return scheduleJobDefinitions;
    }

    internal static TeamFoundationJobDefinition GetDeleteDefinitionJobDefinition(
      this BuildDefinition definition,
      bool isIgnoreDormancyPermitted = true,
      int deleteDefinitionJobScheduleInDays = 30)
    {
      TeamFoundationJobDefinition definitionJobDefinition = (TeamFoundationJobDefinition) null;
      if (definition != null)
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlNode element1 = (XmlNode) xmlDocument.CreateElement("BuildDefinition");
        XmlNode element2 = (XmlNode) xmlDocument.CreateElement("ProjectId");
        element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(definition.ProjectId.ToString()));
        XmlNode element3 = (XmlNode) xmlDocument.CreateElement("DefinitionId");
        element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(definition.Id.ToString()));
        XmlNode element4 = (XmlNode) xmlDocument.CreateElement("NumberTimesRun");
        element4.AppendChild((XmlNode) xmlDocument.CreateTextNode("0"));
        element1.AppendChild(element2);
        element1.AppendChild(element3);
        element1.AppendChild(element4);
        definitionJobDefinition = new TeamFoundationJobDefinition()
        {
          Data = element1,
          EnabledState = TeamFoundationJobEnabledState.Enabled,
          ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.DeleteDefinitionJobExtension",
          JobId = Guid.NewGuid(),
          Name = BuildServerResources.DeleteDefinitionJobName(),
          IgnoreDormancy = isIgnoreDormancyPermitted,
          PriorityClass = JobPriorityClass.Idle
        };
        definitionJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = 3600,
          ScheduledTime = DateTime.UtcNow.AddDays((double) deleteDefinitionJobScheduleInDays),
          PriorityLevel = JobPriorityLevel.Idle
        });
      }
      return definitionJobDefinition;
    }

    public static List<TeamFoundationJobSchedule> ConvertSchedule(
      Schedule buildSchedule,
      IVssRequestContext requestContext)
    {
      return BuildDefinitionExtensions.ConvertSchedule(buildSchedule, DateTime.UtcNow, requestContext);
    }

    internal static List<TeamFoundationJobSchedule> ConvertSchedule(
      Schedule buildSchedule,
      DateTime utcNow,
      IVssRequestContext requestContext)
    {
      List<TeamFoundationJobSchedule> foundationJobScheduleList = new List<TeamFoundationJobSchedule>();
      bool isDSTHandlingEnabled = requestContext.IsFeatureEnabled("Build2.HandleBuildJobScheduleDSTTransitions");
      DateTime dateTime1 = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
      while (dateTime1.DayOfWeek != DayOfWeek.Sunday)
        dateTime1 = dateTime1.AddDays(-1.0);
      DateTime dateTime2 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime1, buildSchedule.TimeZoneId);
      DateTime dateTime3 = dateTime2.AddDays(7.0);
      TimeZoneInfo systemTimeZoneById = TimeZoneInfo.FindSystemTimeZoneById(buildSchedule.TimeZoneId);
      if (!isDSTHandlingEnabled && (systemTimeZoneById.IsDaylightSavingTime(dateTime2) != systemTimeZoneById.IsDaylightSavingTime(dateTime3) || utcNow.DayOfWeek == DayOfWeek.Sunday))
      {
        dateTime1 = dateTime1.AddDays(-7.0);
        utcNow = utcNow.AddDays(-7.0);
      }
      DateTime dateTime4 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcNow, buildSchedule.TimeZoneId);
      DateTime localTime = new DateTime(dateTime4.Year, dateTime4.Month, dateTime4.Day, buildSchedule.StartHours, buildSchedule.StartMinutes, 0, 0);
      DateTime utc = BuildDefinitionExtensions.CovertLocalToUTC(systemTimeZoneById, localTime, isDSTHandlingEnabled);
      if (isDSTHandlingEnabled)
      {
        if (dateTime4.DayOfWeek == DayOfWeek.Saturday && utcNow.DayOfWeek == DayOfWeek.Sunday)
          dateTime1 = dateTime1.AddDays(-7.0);
        else if (dateTime4.DayOfWeek == DayOfWeek.Sunday && utcNow.DayOfWeek == DayOfWeek.Saturday)
          dateTime1 = dateTime1.AddDays(7.0);
      }
      int num1 = 604800;
      DateTime dateTime5 = dateTime1.Add(new TimeSpan(utc.Hour, utc.Minute, 0));
      int num2 = 0;
      if (utc.Day != localTime.Day)
        num2 = utc.Subtract(localTime).Ticks > 0L ? 1 : -1;
      if ((buildSchedule.DaysToBuild & ScheduleDays.Sunday) != ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime5.AddDays((double) num2),
          TimeZoneId = buildSchedule.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((buildSchedule.DaysToBuild & ScheduleDays.Monday) != ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime5.AddDays((double) (1 + num2)),
          TimeZoneId = buildSchedule.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((buildSchedule.DaysToBuild & ScheduleDays.Tuesday) != ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime5.AddDays((double) (2 + num2)),
          TimeZoneId = buildSchedule.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((buildSchedule.DaysToBuild & ScheduleDays.Wednesday) != ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime5.AddDays((double) (3 + num2)),
          TimeZoneId = buildSchedule.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((buildSchedule.DaysToBuild & ScheduleDays.Thursday) != ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime5.AddDays((double) (4 + num2)),
          TimeZoneId = buildSchedule.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((buildSchedule.DaysToBuild & ScheduleDays.Friday) != ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime5.AddDays((double) (5 + num2)),
          TimeZoneId = buildSchedule.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((buildSchedule.DaysToBuild & ScheduleDays.Saturday) != ScheduleDays.None)
        foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime5.AddDays((double) (6 + num2)),
          TimeZoneId = buildSchedule.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      return foundationJobScheduleList;
    }

    private static DateTime CovertLocalToUTC(
      TimeZoneInfo localTimeZone,
      DateTime localTime,
      bool isDSTHandlingEnabled)
    {
      if (!isDSTHandlingEnabled || !localTimeZone.IsInvalidTime(localTime) && !localTimeZone.IsAmbiguousTime(localTime))
        return TimeZoneInfo.ConvertTimeToUtc(localTime, localTimeZone);
      localTime = localTime.AddDays(1.0);
      return TimeZoneInfo.ConvertTimeToUtc(localTime, localTimeZone).AddDays(-1.0);
    }

    public static IEnumerable<BuildDefinitionStep> AllSteps(this BuildDefinition definition) => !(definition?.Process is DesignerProcess process) ? Enumerable.Empty<BuildDefinitionStep>() : process.Phases.SelectMany<Phase, BuildDefinitionStep>((Func<Phase, IEnumerable<BuildDefinitionStep>>) (p => (IEnumerable<BuildDefinitionStep>) p.Steps));

    public static PipelineBuilder GetPipelineBuilder(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      PipelineResources authorizedResources = null,
      bool authorizeNewResources = false,
      bool evaluateCounters = false)
    {
      authorizedResources = authorizedResources ?? new PipelineResources();
      List<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference> source = new List<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>();
      foreach (VariableGroup variableGroup in definition.VariableGroups)
      {
        Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference variableGroupReference = new Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference()
        {
          Id = variableGroup.Id
        };
        source.Add(variableGroupReference);
        authorizedResources.VariableGroups.Add(variableGroupReference);
      }
      PipelineBuilder builder = requestContext.GetService<IPipelineBuilderService>().GetBuilder(requestContext, definition.ProjectId, "Build", definition.Id, authorizedResources, authorizeNewResources, evaluateCounters);
      definition.AddWellKnownVariables(requestContext, builder.SystemVariables);
      definition.AddProcessParametersAsVariables(requestContext, builder.SystemVariables);
      builder.UserVariables.AddRange<IVariable, IList<IVariable>>(source.OfType<IVariable>());
      IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
      Dictionary<string, string> secretVariables = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IVssRequestContext requestContext1 = requestContext;
      BuildDefinition definition1 = definition;
      Dictionary<string, string> targetSecretVariables = secretVariables;
      service.ReadSecretVariables(requestContext1, definition1, (IDictionary<string, string>) targetSecretVariables, (IDictionary<string, string>) null, true);
      foreach (KeyValuePair<string, BuildDefinitionVariable> keyValuePair in definition.Variables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (x => x.Value != null)))
      {
        VariableValue variableValue = keyValuePair.Value.ToVariableValue(keyValuePair.Key, (IDictionary<string, string>) secretVariables);
        builder.UserVariables.Add((IVariable) new Variable()
        {
          Name = keyValuePair.Key,
          Value = variableValue.Value,
          Secret = variableValue.IsSecret
        });
      }
      if (definition.Queue != null)
        builder.DefaultQueue = new AgentQueueReference()
        {
          Id = definition.Queue.Id
        };
      return builder;
    }

    public static void AddProcessParametersAsVariables(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> variables)
    {
      ProcessParameters processParameters = definition.ProcessParameters;
      int num1;
      if (processParameters == null)
      {
        num1 = 0;
      }
      else
      {
        int? count = processParameters.Inputs?.Count;
        int num2 = 0;
        num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
      }
      if (num1 == 0)
        return;
      foreach (TaskInputDefinitionBase input in (IEnumerable<TaskInputDefinitionBase>) definition.ProcessParameters.Inputs)
      {
        if (input != null && !string.IsNullOrEmpty(input.Name))
        {
          string key1 = "ProcParam." + input.Name;
          string key2 = "Parameters." + input.Name;
          variables[key1] = (VariableValue) input.DefaultValue;
          variables[key2] = (VariableValue) input.DefaultValue;
        }
      }
    }

    public static void AddWellKnownVariables(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> variables)
    {
      variables["system"] = (VariableValue) "build";
      variables["system.collectionId"] = (VariableValue) requestContext.ServiceHost.InstanceId.ToString("D");
      variables["system.teamProject"] = (VariableValue) definition.ProjectName;
      variables["system.teamProjectId"] = (VariableValue) definition.ProjectId.ToString("D");
      variables["system.definitionId"] = (VariableValue) definition.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      variables["build.definitionName"] = (VariableValue) definition.Name;
      variables["build.repository.id"] = (VariableValue) (definition.Repository?.Id ?? "0");
      variables["build.repository.name"] = (VariableValue) (definition.Repository?.Name ?? string.Empty);
      variables["build.repository.uri"] = (VariableValue) (definition.Repository?.Url?.ToString() ?? string.Empty);
      IDictionary<string, VariableValue> dictionary = variables;
      int? revision = definition.Revision;
      ref int? local1 = ref revision;
      VariableValue variableValue = (VariableValue) ((local1.HasValue ? local1.GetValueOrDefault().ToString((IFormatProvider) CultureInfo.InvariantCulture) : (string) null) ?? "0");
      dictionary["build.definitionVersion"] = variableValue;
      if (requestContext.IsFeatureEnabled("DistributedTask.EnableDynamicTasksAndAgentFeatureFlags"))
      {
        IPipelineComponentsFeatureFlagService service = requestContext.GetService<IPipelineComponentsFeatureFlagService>();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (FeatureAvailabilityInformation feature in service.GetFeatures(requestContext, PipelineComponentType.Agent))
          variables[feature.Name] = (VariableValue) feature.IsEnabled.ToString();
        foreach (FeatureAvailabilityInformation feature in service.GetFeatures(requestContext, PipelineComponentType.Tasks))
          variables[feature.Name] = (VariableValue) feature.IsEnabled.ToString();
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        if (elapsedMilliseconds > 250L)
          requestContext.TraceWarning(12030395, nameof (BuildDefinitionExtensions), "Pipeline component flags were fetched by {0}ms which is longer than expected.", (object) elapsedMilliseconds);
      }
      if (requestContext.IsFeatureEnabled("DistributedTask.DefinitionPathSystemVariable"))
        variables["build.definitionFolderPath"] = (VariableValue) definition.Path;
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.ContinueAfterCancelProcessTreeKillAttempt"))
        variables["VSTSAGENT_CONTINUE_AFTER_CANCEL_PROCESSTREEKILL_ATTEMPT"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.DockerActionRetries"))
        variables["VSTSAGENT_DOCKER_ACTION_RETRIES"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.UseMsalLibrary"))
        variables["USE_MSAL"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.EnableNodeWarnings"))
        variables["VSTSAGENT_ENABLE_NODE_WARNINGS"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.MajorUpgradeDisabled"))
        variables["AZP_AGENT_MAJOR_UPGRADE_DISABLED"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.AgentFailOnIncompatibleOS"))
        variables["AGENT_FAIL_ON_INCOMPATIBLE_OS"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.EnableFCSItemPathFix"))
        variables["ENABLE_FCS_ITEM_PATH_FIX"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.DisableDrainQueuesAfterTask"))
        variables["AGENT_DISABLE_DRAIN_QUEUES_AFTER_TASK"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.PipelineTasks.EnableBuildArtifactsPlusSignWorkaround"))
        variables["AZP_TASK_FF_ENABLE_BUILDARTIFACTS_PLUS_SIGN_WORKAROUND"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.MSRC75787EnableTelemetry"))
        variables["AZP_75787_ENABLE_COLLECT"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.MSRC75787EnableNewAgentNewProcessHandlerSanitizer"))
        variables["AZP_75787_ENABLE_NEW_PH_LOGIC"] = (VariableValue) "true";
      variables["AZP_AGENT_CHECK_FOR_TASK_DEPRECATION"] = !requestContext.IsFeatureEnabled("DistributedTask.CheckForTaskDeprecation") ? (VariableValue) "false" : (VariableValue) "true";
      variables["AZP_AGENT_MOUNT_WORKSPACE"] = !requestContext.IsFeatureEnabled("DistributedTask.Agent.MountWorkspace") ? (VariableValue) "false" : (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.IgnoreVSTSTaskLib"))
        variables["AZP_AGENT_IGNORE_VSTSTASKLIB"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.EnableNewPowerShellInvokeProcessCmdlet"))
        variables["AZP_PS_ENABLE_INVOKE_PROCESS"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.FailDeprecatedTask"))
        variables["FAIL_DEPRECATED_TASK"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.LogTaskNameInUserAgent"))
        variables["AZP_AGENT_LOG_TASKNAME_IN_USERAGENT"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.AllowEnableShellTasksArgsSanitizingToggle"))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        PipelineGeneralSettings pipelineGeneralSettings = requestContext.GetService<ISettingsService>().GetValue<PipelineGeneralSettings>(requestContext, SettingsUserScope.AllUsers, "Project", definition.ProjectId.ToString(), "Pipelines/General/Settings", PipelineGeneralSettings.Default);
        RegistryQuery registryQuery;
        int num;
        if (!pipelineGeneralSettings.EnableShellTasksArgsSanitizing)
        {
          IVssRegistryService registryService = service;
          IVssRequestContext requestContext1 = requestContext;
          registryQuery = (RegistryQuery) Microsoft.TeamFoundation.Build.Common.RegistryKeys.EnableShellTasksArgsSanitizing;
          ref RegistryQuery local2 = ref registryQuery;
          num = registryService.GetValue<bool>(requestContext1, in local2, false) ? 1 : 0;
        }
        else
          num = 1;
        if (num != 0)
          variables["AZP_75787_ENABLE_NEW_LOGIC"] = (VariableValue) "true";
        if (num == 0)
        {
          if (!pipelineGeneralSettings.EnableShellTasksArgsSanitizingAudit)
          {
            IVssRegistryService registryService = service;
            IVssRequestContext requestContext2 = requestContext;
            registryQuery = (RegistryQuery) Microsoft.TeamFoundation.Build.Common.RegistryKeys.EnableShellTasksArgsSanitizingAudit;
            ref RegistryQuery local3 = ref registryQuery;
            if (!registryService.GetValue<bool>(requestContext2, in local3, false))
              goto label_60;
          }
          variables["AZP_75787_ENABLE_NEW_LOGIC_LOG"] = (VariableValue) "true";
        }
      }
label_60:
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.AgentEnablePipelineArtifactLargeChunkSize"))
        variables[Microsoft.TeamFoundation.Build.WebApi.BuildVariables.AgentEnablePipelineArtifactLargeChunkSize] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.FailJobWhenAgentDies"))
        variables["FAIL_JOB_WHEN_AGENT_DIES"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.UseMsdeployTokenAuth"))
        variables["USE_MSDEPLOY_TOKEN_AUTH"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.FixPossibleGitOutOfMemoryProblem"))
        variables["FIX_POSSIBLE_GIT_OUT_OF_MEMORY_PROBLEM"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.DockerInitOption"))
        variables["AZP_AGENT_DOCKER_INIT_OPTION"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Agent.UseMaskingPerformanceEnhancements"))
        variables["agent.agentUseMaskingPerformanceEnhancements"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.HighTaskFailRateDetected"))
        variables["HIGH_TASK_FAIL_RATE_DETECTED"] = (VariableValue) "true";
      if (requestContext.IsFeatureEnabled("DistributedTask.Tasks.RetireAzureRMPowerShellModule"))
        variables["RETIRE_AZURERM_POWERSHELL_MODULE"] = (VariableValue) "true";
      if (!requestContext.IsFeatureEnabled("DistributedTask.Agent.FailDeprecatedBuildTask"))
        return;
      variables["FAIL_DEPRECATED_BUILD_TASK"] = (VariableValue) "true";
    }

    public static void ValidateBuildStepInputs(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> variables = null)
    {
      IInputValidationService service1 = requestContext.GetService<IInputValidationService>();
      IDistributedTaskPoolService service2 = requestContext.GetService<IDistributedTaskPoolService>();
      foreach (BuildDefinitionStep step in definition.AllSteps().Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (x => x.Enabled)))
      {
        TaskDefinition taskDefinition = step.ToTaskDefinition(requestContext, service2);
        if (taskDefinition != null)
        {
          foreach (TaskInputDefinition taskInputDefinition in taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x.Validation != null)))
          {
            string input;
            if (step.Inputs.TryGetValue(taskInputDefinition.Name, out input))
            {
              string str1 = input;
              if (variables != null && variables.Any<KeyValuePair<string, VariableValue>>())
                str1 = VariableUtility.ExpandVariables(input, variables, false);
              if (!BuildCommonUtil.VariableInUse(str1))
              {
                InputValidationRequest inputValidationRequest = new InputValidationRequest();
                InputValidationItem inputValidationItem1 = new InputValidationItem();
                inputValidationItem1.Value = taskInputDefinition.Validation.Expression;
                inputValidationItem1.Reason = taskInputDefinition.Validation.Message;
                inputValidationItem1.Context = new InputBindingContext()
                {
                  Value = str1
                };
                InputValidationItem inputValidationItem2 = inputValidationItem1;
                inputValidationRequest.Inputs.Add(taskInputDefinition.Name, (ValidationItem) inputValidationItem2);
                service1.ValidateInputs(requestContext, inputValidationRequest);
                bool? isValid = inputValidationItem2.IsValid;
                bool flag = false;
                if (isValid.GetValueOrDefault() == flag & isValid.HasValue)
                {
                  string str2 = str1;
                  string key = string.Empty;
                  if (!string.IsNullOrEmpty(input) && input.Length >= 4)
                    key = input.Substring(2, input.Length - 3);
                  VariableValue variableValue;
                  if (variables.TryGetValue(key, out variableValue) && variableValue != null && variableValue.IsSecret)
                    str2 = "***";
                  throw new InvalidDefinitionException(BuildServerResources.BuildDefinitionInputInvalid((object) taskInputDefinition.Name, (object) str2, (object) inputValidationItem2.Value, (object) inputValidationItem2.Reason));
                }
              }
            }
          }
        }
      }
    }

    internal static void ConvertTriggerPathsToProjectId(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (BuildTrigger trigger1 in definition.Triggers)
      {
        if (trigger1 is ContinuousIntegrationTrigger trigger2)
          trigger2.ConvertFilterPathsToProjectId(requestContext);
        if (trigger1 is GatedCheckInTrigger trigger3)
          trigger3.ConvertPathFiltersPathsToProjectId(requestContext);
      }
    }

    internal static void ConvertTriggerPathsToProjectName(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (BuildTrigger trigger1 in definition.Triggers)
      {
        if (trigger1 is ContinuousIntegrationTrigger trigger2)
          trigger2.ConvertFilterPathsToProjectName(requestContext);
        if (trigger1 is GatedCheckInTrigger trigger3)
          trigger3.ConvertPathFiltersPathsToProjectName(requestContext);
      }
    }

    internal static void ConvertTaskParametersToProjectId(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (BuildDefinitionStep allStep in definition.AllSteps())
      {
        foreach (string key in allStep.Inputs.Keys.ToList<string>())
          allStep.Inputs[key] = TFVCPathHelper.ConvertToPathWithProjectGuid(requestContext, allStep.Inputs[key]);
      }
    }

    internal static void ConvertTaskParametersToProjectName(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (BuildDefinitionStep allStep in definition.AllSteps())
      {
        foreach (string key in allStep.Inputs.Keys.ToList<string>())
          allStep.Inputs[key] = TFVCPathHelper.ConvertToPathWithProjectName(requestContext, allStep.Inputs[key]);
      }
    }

    internal static void ConvertVariableGroupsToReferences(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      List<VariableGroup> source = new List<VariableGroup>((IEnumerable<VariableGroup>) definition.VariableGroups);
      definition.VariableGroups.Clear();
      definition.VariableGroups.AddRange(source.Select<VariableGroup, VariableGroup>((Func<VariableGroup, VariableGroup>) (vg => new VariableGroup()
      {
        Id = vg.Id
      })));
    }

    internal static void ConvertVariablesToProjectId(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (KeyValuePair<string, BuildDefinitionVariable> variable in definition.Variables)
        definition.Variables[variable.Key].Value = TFVCPathHelper.ConvertToPathWithProjectGuid(requestContext, variable.Value.Value);
    }

    internal static void ConvertVariablesToProjectName(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (KeyValuePair<string, BuildDefinitionVariable> variable in definition.Variables)
        definition.Variables[variable.Key].Value = TFVCPathHelper.ConvertToPathWithProjectName(requestContext, variable.Value.Value);
    }

    internal static void ValidateVariables(this BuildDefinition definition)
    {
      if (definition == null || definition.Variables.Count <= 0)
        return;
      bool isChineseOs = OSDetails.IsChineseOS;
      foreach (KeyValuePair<string, BuildDefinitionVariable> variable in definition.Variables)
      {
        ArgumentUtility.CheckStringForInvalidCharacters(variable.Key, "definition.Variables.Variable.Key", false, isChineseOs, "Build2");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(variable.Key, "definition.Variables.Variable.Key", "Build2");
        ArgumentUtility.CheckForNull<BuildDefinitionVariable>(variable.Value, "definition.Variables.Variable.Value", "Build2");
        if (BuildDefinitionExtensions.IsReservedVariableName(variable.Key))
          throw new VariableNameIsReservedException(BuildServerResources.VariableNameIsReserved((object) variable.Key)).Expected("Build2");
        if (!variable.Value.IsSecret || variable.Value.Value != null)
          ArgumentUtility.CheckStringForInvalidCharacters(variable.Value.Value, "definition.Variables.Variable.Value", false, isChineseOs, "Build2");
      }
    }

    internal static bool ShouldCreateTriggerJobDefinition(this BuildDefinition definition)
    {
      if (definition != null && definition.Triggers != null)
      {
        DefinitionQuality? definitionQuality1 = definition.DefinitionQuality;
        DefinitionQuality definitionQuality2 = DefinitionQuality.Definition;
        if (definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue)
          return definition.QueueStatus != DefinitionQueueStatus.Disabled;
      }
      return false;
    }

    internal static void ValidateVariableGroups(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      if (definition == null)
        return;
      foreach (VariableGroup variableGroup in definition.VariableGroups)
      {
        if (variableGroup == null)
          throw new ArgumentException(CommonResources.NullElementNotAllowedInCollection(), "VariableGroups").Expected("Build2");
        ArgumentUtility.CheckGreaterThanZero((float) variableGroup.Id, "definition.VariableGroups.VariableGroup.Id", "Build2");
      }
    }

    internal static void ForceEnablePublicProjectBadges(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      if (definition.BadgeEnabled || requestContext.GetService<IProjectService>().GetProjectVisibility(requestContext, definition.ProjectId) != ProjectVisibility.Public)
        return;
      definition.BadgeEnabled = true;
    }

    internal static void SyncCronSchedulesForDefinition(
      BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      if (definition.Repository == null)
        return;
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type);
      sourceProvider.CheckEndpointAuthorization(requestContext, definition.ProjectId, definition.Repository, definition.Repository);
      sourceProvider.SetRepositoryDefaultInfo(requestContext, definition.ProjectId, definition.Repository);
      string str = sourceProvider.NormalizeSourceBranch(definition?.Repository?.DefaultBranch, definition);
      if (requestContext.IsFeatureEnabled("Build2.DoNotSyncScheduleIfDesigner"))
        requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (SyncCronSchedulesForDefinition), string.Format("Syncing cron schedule for definition with id `{0}` and name `{1}` and default branch `{2}`", (object) definition.Id, (object) definition.Name, (object) str));
      CronScheduleHelper.UpdateCronSchedules(requestContext, new List<BuildDefinition>()
      {
        definition
      }, (RepositoryUpdateInfo) null, true, new List<string>()
      {
        str
      });
    }

    public static string GetRestUrl(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      return requestContext.GetService<IBuildRouteService>().GetDefinitionRestUrl(requestContext, definition.ProjectId, definition.Id, definition.Revision);
    }

    public static string GetDesignerUrl(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      return requestContext.GetService<IBuildRouteService>().GetDefinitionDesignerUrl(requestContext, definition.ProjectId, definition.Id);
    }

    public static string GetWebUrl(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      return requestContext.GetService<IBuildRouteService>().GetDefinitionWebUrl(requestContext, definition.ProjectId, definition.Id);
    }

    private static bool IsReservedVariableName(string variableName) => BuildDefinitionExtensions.ReservedBuildVariableNames.Contains(variableName);
  }
}
