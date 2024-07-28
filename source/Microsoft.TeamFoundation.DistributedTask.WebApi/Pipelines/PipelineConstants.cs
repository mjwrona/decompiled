// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class PipelineConstants
  {
    public static readonly string AgentVersionDemandName = "Agent.Version";
    public static readonly string AgentName = "Agent.Name";
    public static readonly int DefaultJobCancelTimeoutInMinutes = 5;
    public static readonly string DefaultJobName = "__default";
    public static readonly string DefaultJobDisplayName = "Job";
    public static readonly int DefaultJobTimeoutInMinutes = 60;
    public static readonly int MaxNodeNameLength = 100;
    public static readonly string NoneAlias = "none";
    public static readonly string SelfAlias = "self";
    public static readonly string DesignerRepo = "__designer_repo";
    internal const string DependencyNotFound = "DependencyNotFound";
    internal const string GraphContainsCycle = "GraphContainsCycle";
    internal const string NameInvalid = "NameInvalid";
    internal const string NameNotUnique = "NameNotUnique";
    internal const string StartingPointNotFound = "StartingPointNotFound";
    internal const string CheckpointNodeInstanceNameClaimKey = "nodeInstanceName";
    internal const string CheckpointIdClaimKey = "checkpointId";
    internal const string SingleHostedPoolName = "Azure Pipelines";
    public static readonly TaskDefinition CheckoutTask;

    public static bool IsCheckoutTask(this Step step) => step is TaskStep taskStep && taskStep.Reference.Id == PipelineConstants.CheckoutTask.Id && taskStep.Reference.Version == (string) PipelineConstants.CheckoutTask.Version;

    static PipelineConstants()
    {
      TaskDefinition taskDefinition = new TaskDefinition();
      taskDefinition.Id = new Guid("6d15af64-176c-496d-b583-fd2ae21d4df4");
      taskDefinition.Name = "Checkout";
      taskDefinition.FriendlyName = "Get sources";
      taskDefinition.Author = "Microsoft";
      taskDefinition.RunsOn.Add("Agent");
      taskDefinition.Version = new TaskVersion("1.0.0");
      taskDefinition.Description = "Get sources from a repository. Supports Git, TfsVC, and SVN repositories.";
      taskDefinition.HelpMarkDown = "[More Information](https://go.microsoft.com/fwlink/?LinkId=798199)";
      IList<TaskInputDefinition> inputs1 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition1 = new TaskInputDefinition();
      taskInputDefinition1.Name = PipelineConstants.CheckoutTaskInputs.Repository;
      taskInputDefinition1.Required = true;
      taskInputDefinition1.InputType = "repository";
      inputs1.Add(taskInputDefinition1);
      IList<TaskInputDefinition> inputs2 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition2 = new TaskInputDefinition();
      taskInputDefinition2.Name = PipelineConstants.CheckoutTaskInputs.Clean;
      taskInputDefinition2.Required = false;
      taskInputDefinition2.DefaultValue = bool.TrueString;
      taskInputDefinition2.InputType = "boolean";
      inputs2.Add(taskInputDefinition2);
      IList<TaskInputDefinition> inputs3 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition3 = new TaskInputDefinition();
      taskInputDefinition3.Name = PipelineConstants.CheckoutTaskInputs.Submodules;
      taskInputDefinition3.Required = false;
      taskInputDefinition3.InputType = "string";
      inputs3.Add(taskInputDefinition3);
      IList<TaskInputDefinition> inputs4 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition4 = new TaskInputDefinition();
      taskInputDefinition4.Name = PipelineConstants.CheckoutTaskInputs.Lfs;
      taskInputDefinition4.Required = false;
      taskInputDefinition4.DefaultValue = bool.FalseString;
      taskInputDefinition4.InputType = "boolean";
      inputs4.Add(taskInputDefinition4);
      IList<TaskInputDefinition> inputs5 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition5 = new TaskInputDefinition();
      taskInputDefinition5.Name = PipelineConstants.CheckoutTaskInputs.FetchDepth;
      taskInputDefinition5.Required = false;
      taskInputDefinition5.InputType = "string";
      inputs5.Add(taskInputDefinition5);
      IList<TaskInputDefinition> inputs6 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition6 = new TaskInputDefinition();
      taskInputDefinition6.Name = PipelineConstants.CheckoutTaskInputs.FetchTags;
      taskInputDefinition6.Required = false;
      taskInputDefinition6.InputType = "string";
      inputs6.Add(taskInputDefinition6);
      IList<TaskInputDefinition> inputs7 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition7 = new TaskInputDefinition();
      taskInputDefinition7.Name = PipelineConstants.CheckoutTaskInputs.PersistCredentials;
      taskInputDefinition7.Required = false;
      taskInputDefinition7.DefaultValue = bool.FalseString;
      taskInputDefinition7.InputType = "boolean";
      inputs7.Add(taskInputDefinition7);
      IList<TaskInputDefinition> inputs8 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition8 = new TaskInputDefinition();
      taskInputDefinition8.Name = PipelineConstants.CheckoutTaskInputs.FetchFilter;
      taskInputDefinition8.Required = false;
      taskInputDefinition8.InputType = "string";
      inputs8.Add(taskInputDefinition8);
      taskDefinition.Execution.Add("agentPlugin", JObject.FromObject((object) new Dictionary<string, string>()
      {
        {
          "target",
          "Agent.Plugins.Repository.CheckoutTask, Agent.Plugins"
        }
      }));
      taskDefinition.PostJobExecution.Add("agentPlugin", JObject.FromObject((object) new Dictionary<string, string>()
      {
        {
          "target",
          "Agent.Plugins.Repository.CleanupTask, Agent.Plugins"
        }
      }));
      PipelineConstants.CheckoutTask = taskDefinition;
    }

    public static class CheckoutTaskInputs
    {
      public static readonly string Repository = "repository";
      public static readonly string Ref = "ref";
      public static readonly string Version = "version";
      public static readonly string Token = "token";
      public static readonly string Clean = "clean";
      public static readonly string Submodules = "submodules";
      public static readonly string Lfs = "lfs";
      public static readonly string FetchDepth = "fetchDepth";
      public static readonly string PersistCredentials = "persistCredentials";
      public static readonly string Path = "path";
      public static readonly string WorkspaceRepo = "workspaceRepo";
      public static readonly string FetchTags = "fetchTags";
      public static readonly string FetchFilter = "fetchFilter";

      public static class SubmodulesOptions
      {
        public static readonly string Recursive = "recursive";
        public static readonly string True = "true";
      }
    }

    public static class WorkspaceCleanOptions
    {
      public static readonly string Outputs = "outputs";
      public static readonly string Resources = "resources";
      public static readonly string All = "all";
    }

    public static class EnvironmentVariables
    {
      public static readonly string EnvironmentId = "Environment.Id";
      public static readonly string EnvironmentName = "Environment.Name";
      public static readonly string EnvironmentResourceId = "Environment.ResourceId";
      public static readonly string EnvironmentResourceName = "Environment.ResourceName";
    }

    public static class ScriptStepInputs
    {
      public static readonly string Script = "script";
      public static readonly string WorkingDirectory = "workingDirectory";
      public static readonly string Shell = "shell";
    }

    public static class AgentPlugins
    {
      public static readonly string Checkout = "checkout";
    }

    public static class StepContainerConstants
    {
      public static readonly string Host = "host";
    }

    public static class ResourceLimits
    {
      public static readonly int FreeAgentTimeout = 60;
      public static readonly int PremiumAgentTimeout = 360;
    }

    public static class PostLinesFrequencyInMilliseconds
    {
      public static readonly int Slow = 10000;
      public static readonly int Fast = 500;
    }

    public static class ScheduleType
    {
      public static readonly string Cron = nameof (Cron);
    }
  }
}
