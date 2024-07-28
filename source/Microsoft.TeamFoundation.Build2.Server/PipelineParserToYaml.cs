// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelineParserToYaml
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class PipelineParserToYaml
  {
    private Guid CheckoutGuid = new Guid("6d15af64-176c-496d-b583-fd2ae21d4df4");

    public string Convert(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineProcess pipelineProcess,
      BuildDefinition buildDefinition)
    {
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      if (pipelineProcess.Stages.Count != 1)
        throw new Exception(BuildServerResources.WrongStageCount((object) pipelineProcess.Stages.Count));
      YamlMappingNode yamlMappingNode1 = new YamlMappingNode();
      YamlDocument document = new YamlDocument((YamlNode) yamlMappingNode1);
      string empty = string.Empty;
      this.AddAuthWarning(pipelineProcess.Stages.First<Stage>().Phases, ref empty);
      Dictionary<string, string> parameterMap = new Dictionary<string, string>();
      this.AddVariables(yamlMappingNode1, buildDefinition.Variables, buildDefinition.VariableGroups, buildDefinition.ProcessParameters, ref parameterMap, ref empty);
      this.AddBuildCompletionTrigger(buildDefinition.BuildCompletionTriggers, yamlMappingNode1, requestContext);
      this.AddTriggers(buildDefinition.Triggers, yamlMappingNode1, ref empty);
      if (buildDefinition.BuildNumberFormat != null)
        yamlMappingNode1.Add("name", buildDefinition.BuildNumberFormat);
      YamlSequenceNode yamlSequenceNode1 = new YamlSequenceNode();
      yamlMappingNode1.Add("jobs", (YamlNode) yamlSequenceNode1);
      foreach (PhaseNode phase1 in (IEnumerable<PhaseNode>) pipelineProcess.Stages[0].Phases)
      {
        YamlMappingNode yamlMappingNode2 = new YamlMappingNode();
        yamlMappingNode2.Add("job", phase1.Name);
        yamlMappingNode2.Add("displayName", phase1.DisplayName);
        if (phase1.Target.TimeoutInMinutes != (ExpressionValue<int>) null)
        {
          if (phase1.Target.TimeoutInMinutes != (ExpressionValue<int>) 60)
            yamlMappingNode2.Add("timeoutInMinutes", phase1.Target.TimeoutInMinutes.ToString());
        }
        else if (buildDefinition.JobTimeoutInMinutes != 60)
          yamlMappingNode2.Add("timeoutInMinutes", buildDefinition.JobTimeoutInMinutes.ToString());
        if (phase1.Target.CancelTimeoutInMinutes != (ExpressionValue<int>) null)
        {
          if (phase1.Target.CancelTimeoutInMinutes != (ExpressionValue<int>) 5)
            yamlMappingNode2.Add("cancelTimeoutInMinutes", phase1.Target.CancelTimeoutInMinutes.ToString());
        }
        else if (buildDefinition.JobCancelTimeoutInMinutes != 5)
          yamlMappingNode2.Add("cancelTimeoutInMinutes", buildDefinition.JobCancelTimeoutInMinutes.ToString());
        if (phase1.DependsOn.Count == 1)
        {
          foreach (string str in (IEnumerable<string>) phase1.DependsOn)
            yamlMappingNode2.Add("dependsOn", str);
        }
        else if (phase1.DependsOn.Count > 1)
        {
          YamlSequenceNode yamlSequenceNode2 = new YamlSequenceNode();
          foreach (string child in (IEnumerable<string>) phase1.DependsOn)
            yamlSequenceNode2.Add(child);
          yamlMappingNode2.Add("dependsOn", (YamlNode) yamlSequenceNode2);
        }
        if (phase1.Condition != null && phase1.Condition != string.Empty && phase1.Condition != "succeeded()")
          yamlMappingNode2.Add("condition", phase1.Condition);
        this.AddPool(yamlMappingNode2, phase1.Target, buildDefinition, requestContext, service, projectId, ref empty);
        YamlSequenceNode steps = new YamlSequenceNode();
        foreach (Step step in phase1 is Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase phase2 ? (IEnumerable<Step>) phase2.Steps : (IEnumerable<Step>) null)
          this.AddStep(requestContext, yamlMappingNode1, steps, buildDefinition, service, parameterMap, projectId, step);
        yamlMappingNode2.Add("steps", (YamlNode) steps);
        yamlSequenceNode1.Add((YamlNode) yamlMappingNode2);
      }
      return empty + this.ConvertYamlToString(document);
    }

    private string ConvertYamlToString(YamlDocument document)
    {
      string empty = string.Empty;
      using (StringWriter output = new StringWriter(new StringBuilder()))
      {
        new YamlStream(new YamlDocument[1]{ document }).Save((TextWriter) output, false);
        return output.ToString();
      }
    }

    private void AddAuthWarning(IList<PhaseNode> phases, ref string yamlComments)
    {
      if (!phases.SelectMany<PhaseNode, IVariable>((Func<PhaseNode, IEnumerable<IVariable>>) (x => x.Variables.Where<IVariable>((Func<IVariable, bool>) (v => v is Variable variable && variable.Name == WellKnownDistributedTaskVariables.EnableAccessToken && variable.Value == "true")))).Any<IVariable>())
        return;
      yamlComments += "# 'Allow scripts to access the OAuth token' was selected in pipeline.  Add the following YAML to any steps requiring access:\n";
      yamlComments += "#       env:\n";
      yamlComments += "#           MY_ACCESS_TOKEN: $(System.AccessToken)\n";
    }

    private void AddStep(
      IVssRequestContext requestContext,
      YamlMappingNode rootMapping,
      YamlSequenceNode steps,
      BuildDefinition buildDefinition,
      IDistributedTaskPoolService poolService,
      Dictionary<string, string> parameterMap,
      Guid projectId,
      Step step)
    {
      switch (step)
      {
        case TaskStep taskStep:
          if (taskStep.Reference.Id.Equals(this.CheckoutGuid))
          {
            this.AddCheckoutStep(requestContext, rootMapping, steps, taskStep, buildDefinition);
            break;
          }
          this.AddTaskStep(steps, taskStep, requestContext, poolService, parameterMap);
          break;
        case TaskTemplateStep step1:
          using (IEnumerator<TaskStep> enumerator = requestContext.GetService<IPipelineBuilderService>().GetTaskTemplateStore(requestContext, projectId).ResolveTasks(step1).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              TaskStep current = enumerator.Current;
              this.AddStep(requestContext, rootMapping, steps, buildDefinition, poolService, parameterMap, projectId, (Step) current);
            }
            break;
          }
      }
    }

    private void AddVariables(
      YamlMappingNode rootMapping,
      Dictionary<string, BuildDefinitionVariable> variables,
      List<VariableGroup> variableGroups,
      ProcessParameters processParameters,
      ref Dictionary<string, string> parameterMap,
      ref string yamlComments)
    {
      foreach (KeyValuePair<string, BuildDefinitionVariable> variable in variables)
      {
        string key = variable.Key;
        if (!(key == "BuildConfiguration") && !(key == "BuildPlatform") && !(key == "system.debug"))
          yamlComments = yamlComments + "# Variable '" + variable.Key + "' was defined in the Variables tab\n";
      }
      foreach (VariableGroup variableGroup in variableGroups)
        yamlComments = yamlComments + "# Variable Group '" + variableGroup.Name + "' was defined in the Variables tab\n";
      if (processParameters == null || processParameters.Inputs.Count <= 0)
        return;
      YamlSequenceNode yamlSequenceNode = new YamlSequenceNode();
      foreach (TaskInputDefinitionBase input in (IEnumerable<TaskInputDefinitionBase>) processParameters.Inputs)
      {
        string str = "BuildParameters." + input.Name;
        parameterMap.Add("$(Parameters." + input.Name + ")", "$(" + str + ")");
        yamlSequenceNode.Add((YamlNode) new YamlMappingNode(new YamlNode[2]
        {
          (YamlNode) "name",
          (YamlNode) str
        })
        {
          {
            "value",
            input.DefaultValue
          }
        });
      }
      rootMapping.Add(nameof (variables), (YamlNode) yamlSequenceNode);
    }

    private void AddBuildCompletionTrigger(
      List<BuildCompletionTrigger> buildCompletionTriggers,
      YamlMappingNode rootMapping,
      IVssRequestContext requestContext)
    {
      if (buildCompletionTriggers.Count <= 0)
        return;
      IEnumerable<BuildDefinition> definitions = requestContext.GetService<IBuildDefinitionService>().GetDefinitions(requestContext, buildCompletionTriggers.First<BuildCompletionTrigger>().ProjectId);
      YamlSequenceNode yamlSequenceNode = new YamlSequenceNode();
      foreach (BuildCompletionTrigger completionTrigger in buildCompletionTriggers)
      {
        BuildCompletionTrigger trigger = completionTrigger;
        string str = (definitions != null ? definitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (x => x.Id == trigger.DefinitionId)).FirstOrDefault<BuildDefinition>()?.Name : (string) null) ?? "PipelineNotFound";
        YamlMappingNode child = new YamlMappingNode(new YamlNode[2]
        {
          (YamlNode) "pipeline",
          (YamlNode) str
        });
        child.Add("source", str);
        YamlMappingNode triggerMappings = new YamlMappingNode();
        this.IncludeExcludeTrigger(triggerMappings, "branches", trigger.BranchFilters);
        child.Add("trigger", (YamlNode) triggerMappings);
        yamlSequenceNode.Add((YamlNode) child);
      }
      YamlMappingNode yamlMappingNode = new YamlMappingNode(new YamlNode[2]
      {
        (YamlNode) "pipelines",
        (YamlNode) yamlSequenceNode
      });
      rootMapping.Add("resources", (YamlNode) yamlMappingNode);
    }

    private void AddTriggers(
      List<BuildTrigger> triggers,
      YamlMappingNode rootMapping,
      ref string yamlComments)
    {
      bool flag = false;
      foreach (BuildTrigger trigger in triggers)
      {
        if (!(trigger is ContinuousIntegrationTrigger integrationTrigger))
        {
          if (trigger is ScheduleTrigger scheduleTrigger)
          {
            YamlSequenceNode scheduleSequence = new YamlSequenceNode();
            flag = true;
            foreach (Schedule schedule in scheduleTrigger.Schedules)
            {
              if (schedule.DaysToBuild == ScheduleDays.All)
              {
                this.AddSchedule(scheduleSequence, schedule, schedule.StartMinutes.ToString() + " " + schedule.StartHours.ToString() + " * * *");
              }
              else
              {
                string str = (string) null;
                if ((schedule.DaysToBuild & ScheduleDays.Sunday) != ScheduleDays.None)
                  str = "0";
                if ((schedule.DaysToBuild & ScheduleDays.Monday) != ScheduleDays.None)
                  str = str == null ? "1" : str + ",1";
                if ((schedule.DaysToBuild & ScheduleDays.Tuesday) != ScheduleDays.None)
                  str = str == null ? "2" : str + ",2";
                if ((schedule.DaysToBuild & ScheduleDays.Wednesday) != ScheduleDays.None)
                  str = str == null ? "3" : str + ",3";
                if ((schedule.DaysToBuild & ScheduleDays.Thursday) != ScheduleDays.None)
                  str = str == null ? "4" : str + ",4";
                if ((schedule.DaysToBuild & ScheduleDays.Friday) != ScheduleDays.None)
                  str = str == null ? "5" : str + ",5";
                if ((schedule.DaysToBuild & ScheduleDays.Saturday) != ScheduleDays.None)
                  str = str == null ? "6" : str + ",6";
                this.AddSchedule(scheduleSequence, schedule, schedule.StartMinutes.ToString() + " " + schedule.StartHours.ToString() + " * * " + str);
              }
            }
            rootMapping.Add("schedules", (YamlNode) scheduleSequence);
          }
        }
        else
        {
          YamlMappingNode triggerMappings = new YamlMappingNode();
          if (integrationTrigger.BranchFilters.Count > 0)
            this.IncludeExcludeTrigger(triggerMappings, "branches", integrationTrigger.BranchFilters);
          if (integrationTrigger.PathFilters.Count > 0)
            this.IncludeExcludeTrigger(triggerMappings, "paths", integrationTrigger.PathFilters);
          if (integrationTrigger.BatchChanges)
            triggerMappings.Add("batch", integrationTrigger.BatchChanges.ToString());
          rootMapping.Add("trigger", (YamlNode) triggerMappings);
        }
      }
      if (!flag)
        return;
      yamlComments += "# Cron Schedules have been converted using UTC Time Zone and may need to be updated for your location\n";
    }

    private void AddSchedule(
      YamlSequenceNode scheduleSequence,
      Schedule schedule,
      string cronString)
    {
      YamlMappingNode yamlMappingNode = new YamlMappingNode(new YamlNode[2]
      {
        (YamlNode) "cron",
        (YamlNode) cronString
      });
      if (schedule.BranchFilters.Count > 0)
        this.IncludeExcludeTrigger(yamlMappingNode, "branches", schedule.BranchFilters);
      if (!schedule.ScheduleOnlyWithChanges)
        yamlMappingNode.Add("always", "true");
      scheduleSequence.Add((YamlNode) yamlMappingNode);
    }

    private void IncludeExcludeTrigger(
      YamlMappingNode triggerMappings,
      string branchString,
      List<string> triggerStrings)
    {
      YamlSequenceNode yamlSequenceNode1 = new YamlSequenceNode();
      YamlSequenceNode yamlSequenceNode2 = new YamlSequenceNode();
      bool flag1 = false;
      bool flag2 = false;
      foreach (string triggerString in triggerStrings)
      {
        if (triggerString.StartsWith("+"))
        {
          yamlSequenceNode1.Add(triggerString.TrimStart('+'));
          flag1 = true;
        }
        else
        {
          if (!triggerString.StartsWith("-"))
            throw new Exception();
          yamlSequenceNode2.Add(triggerString.TrimStart('-'));
          flag2 = true;
        }
      }
      YamlMappingNode yamlMappingNode = new YamlMappingNode();
      if (flag1)
        yamlMappingNode.Add("include", (YamlNode) yamlSequenceNode1);
      if (flag2)
        yamlMappingNode.Add("exclude", (YamlNode) yamlSequenceNode2);
      triggerMappings.Add(branchString, (YamlNode) yamlMappingNode);
    }

    private void AddPool(
      YamlMappingNode jobNode,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTarget target,
      BuildDefinition buildDefinition,
      IVssRequestContext requestContext,
      IDistributedTaskPoolService poolService,
      Guid projectId,
      ref string yamlComments)
    {
      switch (target)
      {
        case AgentQueueTarget _:
          YamlMappingNode poolNode = new YamlMappingNode();
          AgentQueueTarget target1 = target as AgentQueueTarget;
          int? nullable1 = new int?();
          string spec = "";
          int? nullable2;
          int num1;
          if (target1 == null)
          {
            num1 = 1;
          }
          else
          {
            nullable2 = target1.Queue?.Id;
            int num2 = 0;
            num1 = !(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue) ? 1 : 0;
          }
          if (num1 != 0)
          {
            nullable1 = new int?(target1.Queue.Id);
            if (target1?.AgentSpecification != null)
            {
              JToken jtoken;
              target1.AgentSpecification.TryGetValue("VMImage", out jtoken);
              if (jtoken != null && jtoken.Type == JTokenType.String)
                spec = jtoken.Value<string>();
            }
          }
          else
          {
            int? nullable3;
            if (buildDefinition == null)
            {
              nullable2 = new int?();
              nullable3 = nullable2;
            }
            else
            {
              // ISSUE: explicit non-virtual call
              AgentPoolQueue queue = __nonvirtual (buildDefinition.Queue);
              if (queue == null)
              {
                nullable2 = new int?();
                nullable3 = nullable2;
              }
              else
                nullable3 = new int?(queue.Id);
            }
            nullable1 = nullable3;
            spec = buildDefinition?.Process is DesignerProcess process ? process.Target?.AgentSpecification?.Identifier : (string) null;
          }
          TaskAgentQueue agentQueue = nullable1.HasValue ? poolService.GetAgentQueue(requestContext, projectId, nullable1.Value) : (TaskAgentQueue) null;
          if (agentQueue == null || agentQueue.Name == null)
            yamlComments += "# Could not determine the agent queue name, please manually add a queue to target.\n";
          else if (agentQueue.Name == "Azure Pipelines")
          {
            string vmImage = this.GetVmImage(spec);
            if (vmImage != string.Empty)
            {
              poolNode.Add("vmImage", vmImage);
            }
            else
            {
              poolNode.Add("name", agentQueue.Name);
              yamlComments += "# Agent Queue 'Azure Pipelines' was used with unrecognized Agent Specification, vmImage property must be specified to determine image - https://docs.microsoft.com/en-us/azure/devops/pipelines/agents/hosted?view=azure-devops&tabs=yaml#software\n";
            }
          }
          else
            poolNode.Add("name", agentQueue.Name);
          if (target1.Execution?.Matrix != (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) null)
            yamlComments += "# Multi-job configuration must be converted to matrix strategy: https://docs.microsoft.com/en-us/azure/devops/pipelines/process/phases?view=azure-devops&tabs=yaml#multi-job-configuration\n";
          else if (target1.Execution?.MaxConcurrency.GetValue() != null)
          {
            ParallelExecutionOptions execution = target1.Execution;
            if ((execution != null ? (execution.MaxConcurrency.GetValue().Value > 1 ? 1 : 0) : 0) != 0)
            {
              YamlMappingNode yamlMappingNode = new YamlMappingNode(new YamlNode[2]
              {
                (YamlNode) "parallel",
                (YamlNode) target1.Execution?.MaxConcurrency.ToString()
              });
              jobNode.Add("strategy", (YamlNode) yamlMappingNode);
            }
          }
          this.AddDemands(poolNode, target1);
          jobNode.Add("pool", (YamlNode) poolNode);
          break;
        case Microsoft.TeamFoundation.DistributedTask.Pipelines.ServerTarget _:
          jobNode.Add("pool", "server");
          break;
        default:
          throw new Exception(BuildServerResources.UnrecognizedPhaseTarget((object) target.GetType()));
      }
    }

    private void AddDemands(YamlMappingNode poolNode, AgentQueueTarget target)
    {
      ISet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demands = target.Demands;
      if (demands == null || demands.Count <= 0)
        return;
      YamlSequenceNode yamlSequenceNode = new YamlSequenceNode();
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demands)
        yamlSequenceNode.Add(demand.ToString());
      poolNode.Add("demands", (YamlNode) yamlSequenceNode);
    }

    private string GetVmImage(string spec)
    {
      switch ((spec ?? string.Empty).ToUpperInvariant())
      {
        case "MACOS 10.14":
        case "MACOS-10.14":
          return "macos-10.14";
        case "MACOS 10.15":
        case "MACOS-10.15":
          return "macos-10.15";
        case "MACOS LATEST":
        case "MACOS-LATEST":
          return "macos-latest";
        case "UBUNTU 16.04":
        case "UBUNTU-16.04":
          return "ubuntu-16.04";
        case "UBUNTU 18.04":
        case "UBUNTU-18.04":
          return "ubuntu-18.04";
        case "UBUNTU 20.04":
        case "UBUNTU-20.04":
          return "ubuntu-20.04";
        case "UBUNTU LATEST":
        case "UBUNTU-LATEST":
          return "ubuntu-latest";
        case "VISUAL STUDIO 2017 ON WINDOWS SERVER 2016":
        case "VS2017-WIN2016":
          return "vs2017-win2016";
        case "WINDOWS LATEST":
        case "WINDOWS-LATEST":
          return "windows-latest";
        case "WINDOWS-2019":
        case "WINDOWS-2019-VS2019":
          return "windows-2019";
        default:
          return "";
      }
    }

    private void AddCheckoutStep(
      IVssRequestContext requestContext,
      YamlMappingNode rootMapping,
      YamlSequenceNode steps,
      TaskStep taskStep,
      BuildDefinition definition)
    {
      string str1 = "self";
      string str2 = "refs/heads/master";
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      ITfsGitRepository repository;
      if ((service != null ? (service.TryFindRepositoryByName(requestContext, definition.ProjectName, definition.Repository.Name, out repository) ? 1 : 0) : 0) != 0)
      {
        str2 = repository.Refs.GetDefault()?.Name;
        repository.Dispose();
      }
      string str3;
      if (definition.Repository.Properties.TryGetValue("skipSyncSource", out str3) && str3 == "true")
        str1 = "none";
      else if (definition.Repository.DefaultBranch != null && !definition.Repository.DefaultBranch.Equals(str2))
      {
        YamlSequenceNode yamlSequenceNode = new YamlSequenceNode(new YamlNode[1]
        {
          (YamlNode) new YamlMappingNode(new YamlNode[2]
          {
            (YamlNode) "repository",
            (YamlNode) "self"
          })
          {
            {
              "type",
              "git"
            },
            {
              "ref",
              definition.Repository.DefaultBranch
            }
          }
        });
        YamlNode yamlNode1;
        if (rootMapping.Children.TryGetValue((YamlNode) "resources", out yamlNode1))
        {
          if (yamlNode1 is YamlMappingNode yamlMappingNode && !yamlMappingNode.Children.Keys.Contains((YamlNode) "repositories"))
            yamlMappingNode.Add("repositories", (YamlNode) yamlSequenceNode);
        }
        else
        {
          YamlNode yamlNode2 = rootMapping[(YamlNode) "jobs"];
          rootMapping.Children.Remove((YamlNode) "jobs");
          rootMapping.Add("resources", (YamlNode) new YamlMappingNode(new YamlNode[2]
          {
            (YamlNode) "repositories",
            (YamlNode) yamlSequenceNode
          }));
          rootMapping.Children.Add((YamlNode) "jobs", yamlNode2);
        }
      }
      YamlMappingNode child = new YamlMappingNode(new YamlNode[2]
      {
        (YamlNode) "checkout",
        (YamlNode) str1
      });
      foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) taskStep.Inputs)
      {
        bool flag = true;
        switch (input.Key)
        {
          case "repository":
            flag = false;
            break;
          case "clean":
            if (input.Value.Equals("false"))
            {
              flag = false;
              break;
            }
            break;
          case "fetchDepth":
            if (input.Value.Equals("0"))
            {
              flag = false;
              break;
            }
            break;
          case "lfs":
            if (input.Value.Equals("false"))
            {
              flag = false;
              break;
            }
            break;
        }
        if (flag)
          child.Add(input.Key, (YamlNode) new YamlScalarNode(input.Value));
      }
      steps.Add((YamlNode) child);
    }

    private void AddTaskStep(
      YamlSequenceNode steps,
      TaskStep taskStep,
      IVssRequestContext requestContext,
      IDistributedTaskPoolService poolService,
      Dictionary<string, string> parameterMap)
    {
      string str = taskStep.Reference.Id.ToString();
      string version = taskStep.Reference.Version;
      TaskDefinition taskDefinition = poolService.GetTaskDefinition(requestContext, taskStep.Reference.Id, version);
      if (taskDefinition != null)
      {
        string contributionIdentifier = taskDefinition.ContributionIdentifier;
        str = contributionIdentifier != null ? contributionIdentifier + "." + taskDefinition.Name : taskDefinition.Name;
        version = taskDefinition.Version.Major.ToString();
      }
      YamlMappingNode child = new YamlMappingNode(new YamlNode[2]
      {
        (YamlNode) "task",
        (YamlNode) (version != null ? str + "@" + version.Replace(".*", "").Replace(".0.0", "") : str)
      });
      if (taskStep.Name != null)
        child.Add("name", taskStep.Name);
      if (taskStep.DisplayName != null)
        child.Add("displayName", taskStep.DisplayName);
      if (taskStep.Condition != null && !taskStep.Condition.Equals("succeeded()"))
        child.Add("condition", taskStep.Condition);
      if (taskStep.ContinueOnError)
        child.Add("continueOnError", taskStep.ContinueOnError.ToString());
      if (taskStep.RetryCountOnTaskFailure != 0)
        child.Add("retryCountOnTaskFailure", taskStep.RetryCountOnTaskFailure.ToString());
      if (!taskStep.Enabled)
        child.Add("enabled", taskStep.Enabled.ToString());
      if (taskStep.TimeoutInMinutes != 0)
        child.Add("timeoutInMinutes", taskStep.TimeoutInMinutes.ToString());
      YamlMappingNode inputNode = new YamlMappingNode();
      foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) taskStep.Inputs)
        this.AddTaskInput(taskDefinition, input, inputNode, parameterMap);
      if (inputNode.Children.Count > 0)
        child.Add("inputs", (YamlNode) inputNode);
      steps.Add((YamlNode) child);
    }

    private void AddTaskInput(
      TaskDefinition taskDefinition,
      KeyValuePair<string, string> input,
      YamlMappingNode inputNode,
      Dictionary<string, string> parameterMap)
    {
      if (input.Value == null)
        return;
      TaskInputDefinition taskInputDefinition = taskDefinition != null ? taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x.Name.Equals(input.Key))).FirstOrDefault<TaskInputDefinition>() : (TaskInputDefinition) null;
      if (taskInputDefinition != null && taskInputDefinition.DefaultValue.Equals(input.Value))
        return;
      string parameter = input.Value;
      if (parameterMap.ContainsKey(input.Value))
        parameter = parameterMap[input.Value];
      inputNode.Add(input.Key, (YamlNode) new YamlScalarNode(parameter));
    }
  }
}
