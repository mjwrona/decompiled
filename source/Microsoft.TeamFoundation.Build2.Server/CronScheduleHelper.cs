// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CronScheduleHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.Common;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class CronScheduleHelper
  {
    private const string c_area = "Build2";
    private const string c_layer = "CronScheduleHelper";

    public static List<DateTime> GetCronJobsForWeek(
      IVssRequestContext requestContext,
      string cronExpression,
      DateTime startTime,
      DateTime endTime,
      out bool exceedsThreshold)
    {
      List<DateTime> cronJobsForWeek = (List<DateTime>) null;
      exceedsThreshold = false;
      CrontabSchedule crontabSchedule = CrontabSchedule.TryParse(cronExpression, new CrontabSchedule.ParseOptions()
      {
        IncludingSeconds = false
      });
      if (crontabSchedule != null)
      {
        List<DateTime> list = crontabSchedule.GetNextOccurrences(startTime, endTime).ToList<DateTime>();
        if (list != null)
        {
          int count = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/CronSchedulesPerJobThreshold", true, 1000);
          cronJobsForWeek = list.ToList<DateTime>();
          exceedsThreshold = list.Count > count;
          if (exceedsThreshold)
          {
            requestContext.TraceAlways(12030223, TraceLevel.Warning, "Build2", nameof (CronScheduleHelper), string.Format("Maximum threshold of '{0}' schedules per week has been reached for cron syntax '{1}', no more times will be added for this cron schedule this week.", (object) count, (object) cronExpression));
            cronJobsForWeek = list.Take<DateTime>(count).ToList<DateTime>();
          }
        }
      }
      return cronJobsForWeek;
    }

    public static List<TeamFoundationJobSchedule> ConvertSchedule(
      IVssRequestContext requestContext,
      CronSchedule schedule,
      DateTime startTime,
      DateTime endTime)
    {
      List<TeamFoundationJobSchedule> foundationJobScheduleList = new List<TeamFoundationJobSchedule>();
      if (schedule.ScheduleType == ScheduleType.Cron)
      {
        List<DateTime> cronJobsForWeek = CronScheduleHelper.GetCronJobsForWeek(requestContext, schedule.CronExpression, startTime, endTime, out bool _);
        if (cronJobsForWeek != null)
        {
          foreach (DateTime dateTime in cronJobsForWeek)
            foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
            {
              Interval = 0,
              ScheduledTime = dateTime,
              TimeZoneId = TimeZoneInfo.Utc.ToString(),
              PriorityLevel = JobPriorityLevel.Lowest
            });
        }
      }
      return foundationJobScheduleList;
    }

    public static void UpdateCronSchedules(
      IVssRequestContext requestContext,
      List<BuildDefinition> yamlDefinitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      bool manualRun = false,
      List<string> manualBranches = null)
    {
      if (!manualRun)
        ArgumentUtility.CheckForNull<RepositoryUpdateInfo>(repositoryUpdateInfo, nameof (repositoryUpdateInfo));
      if (!manualRun && (repositoryUpdateInfo.RefUpdates.Count == 0 || !yamlDefinitions.Any<BuildDefinition>()))
      {
        requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (CronScheduleHelper), "Tried to update schedules for YAML, but wither there were not real updates or no YAML definitions.");
      }
      else
      {
        Dictionary<string, List<BuildDefinition>> yamlDefinitionMap = new Dictionary<string, List<BuildDefinition>>();
        foreach (BuildDefinition yamlDefinition in yamlDefinitions)
        {
          YamlProcess process = yamlDefinition.GetProcess<YamlProcess>();
          if (process == null || !manualRun && !yamlDefinition.RepositoryMatchesUpdateInfo(requestContext, repositoryUpdateInfo))
          {
            requestContext.TraceAlways(12030223, TraceLevel.Error, "Build2", nameof (CronScheduleHelper), "Definition provided is not YAML based or definition's repository ID " + yamlDefinition.Repository?.Id + " does not match the Repository of the update " + repositoryUpdateInfo?.RepositoryId + ", ending this call.");
          }
          else
          {
            string yamlFilename = process.YamlFilename;
            List<BuildDefinition> buildDefinitionList;
            if (yamlDefinitionMap.TryGetValue(yamlFilename, out buildDefinitionList))
              buildDefinitionList.Add(yamlDefinition);
            else
              yamlDefinitionMap.Add(yamlFilename, new List<BuildDefinition>()
              {
                yamlDefinition
              });
          }
        }
        requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "Discovered '{0}' YAML files to check for YAML updates in the supplied refUpdates or '{1}' definitions.", (object) yamlDefinitionMap.Count, (object) yamlDefinitions.Count);
        if (manualRun)
          requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), string.Format("Beginning manually checking YAML definitions files for schedules to update across '{0}' branches. Some of the branches are {1}", (object) manualBranches?.Count, (object) string.Join(", ", manualBranches.Take<string>(5))));
        else
          requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "Beginning checking '{0}' updates for changes to YAML definition files.", (object) repositoryUpdateInfo?.RefUpdates.Count);
        Dictionary<(string, int), CronScheduleHelper.DefinitionSchedulePair> branches = CronScheduleHelper.FilterSchedulesToBranches(requestContext, yamlDefinitionMap, repositoryUpdateInfo, manualRun, manualBranches);
        if (branches.Count > 0)
        {
          foreach (KeyValuePair<(string, int), CronScheduleHelper.DefinitionSchedulePair> keyValuePair in branches)
            CronScheduleHelper.CreateAndUpdateSchedules(requestContext, keyValuePair.Value.Definition, keyValuePair.Key.Item1, keyValuePair.Value.Schedules);
        }
        else if (manualRun)
          requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "No YAML based schedule updates found for manual run on '{0}' branches.", (object) manualBranches?.Count);
        else
          requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "No YAML based schedule updates found for push with '{0}' refUpdates.", (object) repositoryUpdateInfo?.RefUpdates.Count);
      }
    }

    internal static void AddDefinitionSchedulesPairToMap(
      CronScheduleHelper.DefinitionSchedulePair pair,
      string branchName,
      Dictionary<(string, int), CronScheduleHelper.DefinitionSchedulePair> map)
    {
      if (map.ContainsKey((branchName, pair.Definition.Id)))
        return;
      map.Add((branchName, pair.Definition.Id), pair);
    }

    internal static void AddingSchedulesPerBranchToDictionaryHelper(
      IVssRequestContext requestContext,
      Dictionary<string, List<BuildDefinition>> yamlDefinitionMap,
      Dictionary<(string, int), CronScheduleHelper.DefinitionSchedulePair> schedulesToBranches,
      string branchName,
      RepositoryUpdateInfo repositoryUpdateInfo = null,
      bool newBranch = false,
      List<string> filesChanged = null,
      bool manualRun = false)
    {
      foreach (KeyValuePair<string, List<BuildDefinition>> yamlDefinition in yamlDefinitionMap)
      {
        string yamlFile = yamlDefinition.Key;
        if (yamlFile.StartsWith("./"))
          yamlFile = yamlFile.Substring(1);
        else if (!yamlFile.StartsWith("/"))
          yamlFile = "/" + yamlFile;
        if (manualRun | newBranch || filesChanged != null && filesChanged.Contains(yamlFile))
        {
          foreach (BuildDefinition buildDefinition in yamlDefinition.Value)
          {
            if (repositoryUpdateInfo != null && !buildDefinition.RepositoryMatchesUpdateInfo(requestContext, repositoryUpdateInfo))
              requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "Repository Id '{0}' for Definition '{1}' does not match Repository Id '{2}' for these changes, skipping this definition.", (object) buildDefinition?.Repository?.Id, (object) buildDefinition.Name, (object) repositoryUpdateInfo?.RepositoryId);
            else if (!schedulesToBranches.ContainsKey((branchName, buildDefinition.Id)))
            {
              requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "Conditions for manual update of schedules on branch '{0}' for definition '{1}' were met, adding to the map", (object) branchName, (object) buildDefinition.Name);
              YamlProcess process = buildDefinition.GetProcess<YamlProcess>();
              if (process != null)
              {
                CronScheduleHelper.DefinitionSchedulePair schedulesFromYaml = CronScheduleHelper.ParseSchedulesFromYaml(requestContext, buildDefinition, branchName, process, yamlFile);
                if (schedulesFromYaml != null)
                {
                  CronScheduleHelper.AddDefinitionSchedulesPairToMap(schedulesFromYaml, branchName, schedulesToBranches);
                  requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (CronScheduleHelper), string.Format("'{0}' schedules have been found to be added for for definition '{1}' in branch '{2}'.", (object) schedulesFromYaml.Schedules.Count, (object) buildDefinition.Name, (object) branchName));
                }
              }
            }
            else
              requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "An update to the YAML based schedules for definition '{0}' on branch '{1}' has already been encountered. Since we read from current version, ignore this commit and continue looking through the list.", (object) buildDefinition.Name, (object) branchName);
          }
        }
      }
    }

    internal static void CreateAndUpdateSchedules(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      string branchName,
      List<PipelineSchedule> cronSchedules)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<string>(branchName, nameof (branchName));
      cronSchedules.ForEach((Action<PipelineSchedule>) (x => x.ScheduleJobId = Guid.NewGuid()));
      List<CronSchedule> deletedSchedules = (List<CronSchedule>) null;
      List<CronSchedule> addedSchedules = (List<CronSchedule>) null;
      using (Build2Component component = requestContext.CreateComponent<Build2Component>())
      {
        component.UpdateCronSchedules(definition, branchName, cronSchedules, out deletedSchedules, out addedSchedules);
        requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (CronScheduleHelper), string.Format("Deleted '{0}' and added '{1}' schedules to tbl_Schedule as part of YAML schedule update for definition '{2}' branch '{3}'.", (object) deletedSchedules?.Count, (object) addedSchedules?.Count, (object) definition.Name, (object) branchName));
      }
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (addedSchedules != null && __nonvirtual (addedSchedules.Count) == 0 && deletedSchedules != null && __nonvirtual (deletedSchedules.Count) > 0)
        requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (CronScheduleHelper), string.Format("Deleted '{0}' schedules without adding new schedules to tbl_Schedule as part of YAML schedule update for definition '{1}' branch '{2}'.", (object) deletedSchedules.Count, (object) definition.Name, (object) branchName));
      List<Guid> jobsToDelete = new List<Guid>();
      if (deletedSchedules != null)
      {
        foreach (CronSchedule cronSchedule in deletedSchedules)
          jobsToDelete.Add(cronSchedule.ScheduleJobId);
      }
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
      ITeamFoundationJobService service = requestContext.Elevate().GetService<ITeamFoundationJobService>();
      if (addedSchedules != null)
      {
        foreach (CronSchedule schedule in addedSchedules)
        {
          DateTime utcNow = DateTime.UtcNow;
          jobUpdates.Add(CronScheduleHelper.GetJobForSchedule(requestContext, definition, schedule, utcNow, utcNow.AddDays(7.0), service.IsIgnoreDormancyPermitted));
        }
      }
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (CronScheduleHelper), string.Format("Deleted '{0}' and added '{1}' schedule jobs from the Job Service as part of YAML schedule update for definition '{2}' branch '{3}'.", (object) jobsToDelete?.Count, (object) jobUpdates?.Count, (object) definition.Name, (object) branchName));
    }

    internal static List<PipelineSchedule> FilterSchedulesByBranch(
      IVssRequestContext requestContext,
      List<PipelineSchedule> schedules,
      string branchName,
      string yamlDefName)
    {
      if (schedules.Count > 0)
      {
        int num = schedules.RemoveAll((Predicate<PipelineSchedule>) (schedule => schedule.BranchFilters.Count == 0 || !BuildSourceProviders.GitProperties.IsRepositoryBranchIncluded((IEnumerable<string>) schedule.BranchFilters, branchName)));
        requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (CronScheduleHelper), string.Format("Removed '{0}' schedules from provided schedules for definition '{1}' on branch '{2}' due to branch filter details.", (object) num, (object) yamlDefName, (object) branchName));
      }
      return schedules;
    }

    internal static Dictionary<(string, int), CronScheduleHelper.DefinitionSchedulePair> FilterSchedulesToBranches(
      IVssRequestContext requestContext,
      Dictionary<string, List<BuildDefinition>> yamlDefinitionMap,
      RepositoryUpdateInfo repositoryUpdateInfo,
      bool manualRun,
      List<string> manualBranches)
    {
      Dictionary<(string, int), CronScheduleHelper.DefinitionSchedulePair> branches = new Dictionary<(string, int), CronScheduleHelper.DefinitionSchedulePair>();
      if (!manualRun)
      {
        foreach (RefUpdateInfo refUpdate in repositoryUpdateInfo.RefUpdates)
        {
          string refName = refUpdate.RefName;
          if (string.IsNullOrEmpty(refName))
            requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "YAML schedules are not allowed for invalid branches");
          else if (!refName.StartsWith("refs/heads/", StringComparison.Ordinal))
            requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "YAML schedules are not allowed for branch '{0}'.", (object) refName);
          else if (Sha1Id.IsNullOrEmpty(refUpdate.NewObjectId) || new Sha1Id(refUpdate.NewObjectId).IsEmpty)
          {
            requestContext.TraceAlways(12030223, TraceLevel.Info, "Build2", nameof (CronScheduleHelper), "Branch '" + refName + "' appears to have been unpublished/deleted. Passing along an empty lists of schedules to make sure that any schedules associated with the branch are removed.");
            foreach (KeyValuePair<string, List<BuildDefinition>> yamlDefinition in yamlDefinitionMap)
            {
              foreach (BuildDefinition def in yamlDefinition.Value)
                CronScheduleHelper.AddDefinitionSchedulesPairToMap(new CronScheduleHelper.DefinitionSchedulePair(def, new List<PipelineSchedule>()), refName, branches);
            }
          }
          else
          {
            bool newBranch = Sha1Id.IsNullOrEmpty(refUpdate.OldObjectId);
            List<string> filesChanged = (List<string>) null;
            if (!newBranch)
              FilteredBuildTriggerHelper.TryGetFilesChanged(requestContext, yamlDefinitionMap.SelectMany<KeyValuePair<string, List<BuildDefinition>>, BuildDefinition>((Func<KeyValuePair<string, List<BuildDefinition>>, IEnumerable<BuildDefinition>>) (pair => (IEnumerable<BuildDefinition>) pair.Value)), repositoryUpdateInfo, refUpdate, out filesChanged);
            if (newBranch || filesChanged != null && filesChanged.Count > 0)
              CronScheduleHelper.AddingSchedulesPerBranchToDictionaryHelper(requestContext, yamlDefinitionMap, branches, refName, repositoryUpdateInfo, newBranch, filesChanged);
          }
        }
      }
      else if (manualBranches != null)
      {
        foreach (string branchName in manualBranches.Where<string>((Func<string, bool>) (branch => !string.IsNullOrWhiteSpace(branch))))
        {
          if (!branchName.StartsWith("refs/heads/", StringComparison.Ordinal))
            requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "YAML schedules are not allowed for branch '{0}'.", (object) branchName);
          else
            CronScheduleHelper.AddingSchedulesPerBranchToDictionaryHelper(requestContext, yamlDefinitionMap, branches, branchName, manualRun: manualRun);
        }
      }
      return branches;
    }

    internal static TeamFoundationJobDefinition GetJobForSchedule(
      IVssRequestContext requestContext,
      BuildDefinition def,
      CronSchedule schedule,
      DateTime startOfSchedules,
      DateTime endOfSchedules,
      bool ignoreDormancyPermitted)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlNode element1 = (XmlNode) xmlDocument.CreateElement("BuildDefinition");
      XmlNode element2 = (XmlNode) xmlDocument.CreateElement("ProjectId");
      element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(def.ProjectId.ToString()));
      XmlNode element3 = (XmlNode) xmlDocument.CreateElement("DefinitionId");
      element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(schedule.DefinitionId.ToString()));
      XmlNode element4 = (XmlNode) xmlDocument.CreateElement("BranchFilters");
      element4.AppendChild((XmlNode) xmlDocument.CreateTextNode(schedule.BranchName));
      XmlNode element5 = (XmlNode) xmlDocument.CreateElement("TriggerType");
      element5.AppendChild((XmlNode) xmlDocument.CreateTextNode(DefinitionTriggerType.Schedule.ToString("G")));
      XmlNode element6 = (XmlNode) xmlDocument.CreateElement("ScheduleOnlyWithChanges");
      element6.AppendChild((XmlNode) xmlDocument.CreateTextNode(schedule.ScheduleOnlyWithChanges.ToString()));
      XmlNode element7 = (XmlNode) xmlDocument.CreateElement("ScheduleName");
      element7.AppendChild((XmlNode) xmlDocument.CreateTextNode(schedule.ScheduleName));
      XmlNode element8 = (XmlNode) xmlDocument.CreateElement("RetriesCount");
      element8.AppendChild((XmlNode) xmlDocument.CreateTextNode("0"));
      XmlNode element9 = (XmlNode) xmlDocument.CreateElement("BatchSchedules");
      element9.AppendChild((XmlNode) xmlDocument.CreateTextNode(schedule.Batch.ToString()));
      element1.AppendChild(element2);
      element1.AppendChild(element3);
      element1.AppendChild(element5);
      element1.AppendChild(element4);
      element1.AppendChild(element6);
      element1.AppendChild(element7);
      element1.AppendChild(element8);
      element1.AppendChild(element9);
      TeamFoundationJobDefinition jobForSchedule = new TeamFoundationJobDefinition();
      jobForSchedule.Data = element1;
      jobForSchedule.EnabledState = TeamFoundationJobEnabledState.Enabled;
      jobForSchedule.ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.BuildScheduleJobExtension";
      jobForSchedule.JobId = schedule.ScheduleJobId;
      jobForSchedule.Name = BuildServerResources.ScheduleJobName();
      jobForSchedule.IgnoreDormancy = ignoreDormancyPermitted;
      jobForSchedule.PriorityClass = JobPriorityClass.High;
      jobForSchedule.Schedule.AddRange((IEnumerable<TeamFoundationJobSchedule>) CronScheduleHelper.ConvertSchedule(requestContext, schedule, startOfSchedules, endOfSchedules));
      return jobForSchedule;
    }

    internal static CronScheduleHelper.DefinitionSchedulePair ParseSchedulesFromYaml(
      IVssRequestContext requestContext,
      BuildDefinition yamlDefinition,
      string branchName,
      YamlProcess yamlProcess,
      string yamlFile)
    {
      IYamlPipelineLoaderService service = requestContext.GetService<IYamlPipelineLoaderService>();
      CronScheduleHelper.DefinitionSchedulePair schedulesFromYaml = (CronScheduleHelper.DefinitionSchedulePair) null;
      YamlPipelineLoadResult pipelineLoadResult;
      try
      {
        RepositoryResource repositoryResource = yamlDefinition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, branchName);
        PipelineBuilder pipelineBuilder = yamlDefinition.GetPipelineBuilder(requestContext);
        requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "Attempting to load schedules from YAML file '{0}' for Definition '{1}' on branch '{2}'.", (object) yamlFile, (object) yamlDefinition.Name, (object) branchName);
        pipelineLoadResult = service.Load(requestContext, yamlDefinition.ProjectId, repositoryResource, yamlFile, pipelineBuilder, retrieveOptions: RetrieveOptions.PipelineSchedules);
      }
      catch (YamlFileNotFoundException ex)
      {
        requestContext.TraceAlways(12030223, TraceLevel.Warning, "Build2", nameof (CronScheduleHelper), "Yaml file '{0}' does not exist for project '{1}', repo '{2}', ref '{3}'. It must have been deleted. Sending along data to remove schedules for definition '{4}' on branch '{3}'.", (object) yamlFile, (object) yamlDefinition.ProjectName, (object) yamlDefinition.Repository.Name, (object) branchName, (object) yamlDefinition.Name);
        return new CronScheduleHelper.DefinitionSchedulePair(yamlDefinition, new List<PipelineSchedule>());
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(12030223, TraceLevel.Error, "Build2", nameof (CronScheduleHelper), "Exception '" + ex.ToString() + "' encountered when trying to Load Yaml file '" + yamlFile + "' for project '" + yamlDefinition?.ProjectName + "', repo '" + yamlDefinition?.Repository?.Name + "', on branch '" + branchName + "' and definition '" + yamlDefinition?.Name + "'.");
        return schedulesFromYaml;
      }
      if (pipelineLoadResult != null && pipelineLoadResult.Template.Errors.Count > 0)
      {
        requestContext.TraceAlways(12030223, TraceLevel.Error, "Build2", nameof (CronScheduleHelper), string.Format("'{0}' Load errors were encountered when trying to Load Yaml file '{1}' for project '{2}', repo '{3}', on branch '{4}' and definition '{5}'.", (object) pipelineLoadResult.Template.Errors.Count, (object) yamlFile, (object) yamlDefinition?.ProjectName, (object) yamlDefinition?.Repository?.Name, (object) branchName, (object) yamlDefinition?.Name));
        foreach (PipelineValidationError error in (IEnumerable<PipelineValidationError>) pipelineLoadResult.Template.Errors)
          requestContext.TraceError(12030223, nameof (CronScheduleHelper), "Error while trying to Load Yaml file: {0}", (object) error.Message);
        return schedulesFromYaml;
      }
      requestContext.TraceInfo(12030223, nameof (CronScheduleHelper), "YAML file '{0}' for definition '{1}' on branch '{2}' successfully parsed, filtering results and adding them to the list to use for update.", (object) yamlFile, (object) yamlDefinition.Name, (object) branchName);
      return new CronScheduleHelper.DefinitionSchedulePair(yamlDefinition, CronScheduleHelper.FilterSchedulesByBranch(requestContext, new List<PipelineSchedule>((IEnumerable<PipelineSchedule>) pipelineLoadResult.Template.Schedules), branchName, yamlDefinition.Name));
    }

    internal class DefinitionSchedulePair
    {
      public DefinitionSchedulePair(BuildDefinition def, List<PipelineSchedule> schedules)
      {
        this.Definition = def;
        this.Schedules = schedules;
      }

      public BuildDefinition Definition { get; set; }

      public List<PipelineSchedule> Schedules { get; set; }
    }
  }
}
