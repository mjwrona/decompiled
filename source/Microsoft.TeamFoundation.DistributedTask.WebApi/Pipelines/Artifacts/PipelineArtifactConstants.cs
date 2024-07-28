// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.PipelineArtifactConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts
{
  public static class PipelineArtifactConstants
  {
    public static readonly TaskDefinition DownloadTask;

    static PipelineArtifactConstants()
    {
      TaskDefinition taskDefinition = new TaskDefinition();
      taskDefinition.Id = new Guid("30f35852-3f7e-4c0c-9a88-e127b4f97211");
      taskDefinition.Name = "Download";
      taskDefinition.FriendlyName = "Download Artifact";
      taskDefinition.Author = "Microsoft";
      taskDefinition.RunsOn.Add("Agent");
      taskDefinition.Version = new TaskVersion("1.0.0");
      taskDefinition.Description = "Downloads pipeline type artifacts.";
      taskDefinition.HelpMarkDown = "[More Information](https://go.microsoft.com/fwlink/?LinkId=798199)";
      IList<TaskInputDefinition> inputs1 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition1 = new TaskInputDefinition();
      taskInputDefinition1.Name = "artifact";
      taskInputDefinition1.Required = true;
      taskInputDefinition1.InputType = "string";
      inputs1.Add(taskInputDefinition1);
      IList<TaskInputDefinition> inputs2 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition2 = new TaskInputDefinition();
      taskInputDefinition2.Name = "patterns";
      taskInputDefinition2.Required = false;
      taskInputDefinition2.DefaultValue = "**";
      taskInputDefinition2.InputType = "string";
      inputs2.Add(taskInputDefinition2);
      IList<TaskInputDefinition> inputs3 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition3 = new TaskInputDefinition();
      taskInputDefinition3.Name = "path";
      taskInputDefinition3.Required = false;
      taskInputDefinition3.InputType = "string";
      inputs3.Add(taskInputDefinition3);
      IList<TaskInputDefinition> inputs4 = taskDefinition.Inputs;
      TaskInputDefinition taskInputDefinition4 = new TaskInputDefinition();
      taskInputDefinition4.Name = "alias";
      taskInputDefinition4.Required = false;
      taskInputDefinition4.InputType = "string";
      inputs4.Add(taskInputDefinition4);
      PipelineArtifactConstants.DownloadTask = taskDefinition;
    }

    internal static class CommonArtifactTaskInputValues
    {
      internal const string DefaultDownloadPath = "$(Pipeline.Workspace)";
      internal const string DefaultDownloadPattern = "**";
    }

    public static class PipelineArtifactTaskInputs
    {
      public const string ArtifactName = "artifactName";
      public const string BuildType = "buildType";
      public const string BuildId = "buildId";
      public const string BuildVersionToDownload = "buildVersionToDownload";
      public const string Definition = "definition";
      public const string DownloadType = "downloadType";
      public const string DownloadPath = "downloadPath";
      public const string FileSharePath = "fileSharePath";
      public const string ItemPattern = "itemPattern";
      public const string Project = "project";
    }

    public static class PipelineArtifactTaskInputValues
    {
      public const string DownloadTypeSingle = "single";
      public const string SpecificBuildType = "specific";
      public const string CurrentBuildType = "current";
      public const string AutomaticMode = "automatic";
      public const string ManualMode = "manual";
    }

    internal static class YamlConstants
    {
      internal const string Connection = "connection";
      internal const string Current = "current";
      internal const string None = "none";
    }

    public static class ArtifactTypes
    {
      public const string AzurePipelineArtifactType = "Pipeline";
    }

    public static class DownloadTaskInputs
    {
      public const string Alias = "alias";
      public const string Artifact = "artifact";
      public const string Mode = "mode";
      public const string Path = "path";
      public const string Patterns = "patterns";
    }

    public static class TraceConstants
    {
      public const string Area = "PipelineArtifacts";
      public const string DownloadPipelineArtifactFeature = "DownloadPipelineArtifact";
      public const string PipelineResourcesUsageFeature = "PipelineResourcesUsage";
      public const string PipelineResourcesUsageForRunInstanceFeature = "PipelineResourcesUsageForRunInstance";
    }

    public static class ReviewAppConstants
    {
      public const string ReviewAppTaskId = "DEEAFED4-0B18-4F58-968D-86655B4D2CE9";
      public const string GithubCommentTaskId = "DEEA6198-ADF8-4B16-9939-7ADDF85708B2";
    }

    public static class PublishMetadataTaskConstants
    {
      public const string PublishMetadataTaskId = "01FA79EB-4C54-41B5-A16F-5CD8D60DB88D";
    }
  }
}
