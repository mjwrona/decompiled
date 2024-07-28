// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ServerBuildDefinitionHelpers
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class ServerBuildDefinitionHelpers
  {
    public static BuildDefinition Deserialize(string definitionString)
    {
      BuildDefinition partlySerializedDefinition = JsonUtility.FromString<BuildDefinition>(definitionString);
      if (partlySerializedDefinition?.Process == null)
        partlySerializedDefinition = ServerBuildDefinitionHelpers.GetBuildDefinition(JsonConvert.DeserializeObject<BuildDefinition3_2>(definitionString), partlySerializedDefinition);
      return partlySerializedDefinition;
    }

    public static BuildDefinition GetBuildDefinition(
      BuildDefinition3_2 source,
      BuildDefinition partlySerializedDefinition)
    {
      if (source == null)
        return (BuildDefinition) null;
      BuildDefinition buildDefinition1 = new BuildDefinition();
      buildDefinition1.BadgeEnabled = source.BadgeEnabled;
      buildDefinition1.BuildNumberFormat = source.BuildNumberFormat;
      buildDefinition1.Comment = source.Comment;
      buildDefinition1.CreatedDate = source.CreatedDate;
      Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality? definitionQuality = source.DefinitionQuality;
      buildDefinition1.DefinitionQuality = definitionQuality.HasValue ? new DefinitionQuality?((DefinitionQuality) definitionQuality.GetValueOrDefault()) : new DefinitionQuality?();
      buildDefinition1.Description = source.Description;
      buildDefinition1.DropLocation = source.DropLocation;
      buildDefinition1.Id = source.Id;
      buildDefinition1.JobAuthorizationScope = (BuildAuthorizationScope) source.JobAuthorizationScope;
      buildDefinition1.JobCancelTimeoutInMinutes = source.JobCancelTimeoutInMinutes;
      buildDefinition1.JobTimeoutInMinutes = source.JobTimeoutInMinutes;
      buildDefinition1.Name = source.Name;
      buildDefinition1.ParentDefinition = partlySerializedDefinition.ParentDefinition;
      buildDefinition1.Path = source.Path;
      buildDefinition1.ProcessParameters = source.ProcessParameters;
      buildDefinition1.Queue = partlySerializedDefinition.Queue;
      buildDefinition1.QueueStatus = partlySerializedDefinition.QueueStatus;
      buildDefinition1.Repository = partlySerializedDefinition.Repository;
      buildDefinition1.Revision = source.Revision;
      buildDefinition1.Type = partlySerializedDefinition.Type;
      buildDefinition1.Uri = source.Uri;
      buildDefinition1.Url = source.Url;
      buildDefinition1.Demands = partlySerializedDefinition.Demands;
      buildDefinition1.Metrics = partlySerializedDefinition.Metrics;
      buildDefinition1.Options = partlySerializedDefinition.Options;
      buildDefinition1.Properties = partlySerializedDefinition.Properties;
      buildDefinition1.Tags = partlySerializedDefinition.Tags;
      buildDefinition1.Triggers = partlySerializedDefinition.Triggers;
      buildDefinition1.Variables = partlySerializedDefinition.Variables;
      BuildDefinition buildDefinition2 = buildDefinition1;
      DesignerProcess designerProcess = new DesignerProcess();
      buildDefinition2.Process = (BuildProcess) designerProcess;
      Phase phase = new Phase();
      designerProcess.Phases.Add(phase);
      if (source.Steps.Count > 0)
      {
        foreach (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep step in source.Steps)
        {
          BuildDefinitionStep buildDefinitionStep = new BuildDefinitionStep()
          {
            Inputs = step.Inputs,
            Enabled = step.Enabled,
            ContinueOnError = step.ContinueOnError,
            AlwaysRun = step.AlwaysRun,
            DisplayName = step.DisplayName,
            TimeoutInMinutes = step.TimeoutInMinutes,
            RetryCountOnTaskFailure = step.RetryCountOnTaskFailure,
            Condition = step.Condition,
            RefName = step.RefName,
            Environment = step.Environment
          };
          if (step.TaskDefinition != null)
            buildDefinitionStep.TaskDefinition = new TaskDefinitionReference()
            {
              Id = step.TaskDefinition.Id,
              VersionSpec = step.TaskDefinition.VersionSpec,
              DefinitionType = step.TaskDefinition.DefinitionType
            };
          phase.Steps.Add(buildDefinitionStep);
        }
      }
      return buildDefinition2;
    }

    public static BuildDefinitionTemplate GetTemplateFromStream(Stream stream)
    {
      string end;
      using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
        end = streamReader.ReadToEnd();
      return ServerBuildDefinitionHelpers.DeserializeTemplate(end);
    }

    public static BuildDefinitionTemplate DeserializeTemplate(string templateString)
    {
      BuildDefinitionTemplate partlySerializedTemplate = JsonConvert.DeserializeObject<BuildDefinitionTemplate>(templateString);
      if (partlySerializedTemplate?.Template?.Process == null)
        partlySerializedTemplate = ServerBuildDefinitionHelpers.ToBuildDefinitionTemplate(JsonConvert.DeserializeObject<BuildDefinitionTemplate3_2>(templateString), partlySerializedTemplate);
      return partlySerializedTemplate;
    }

    public static BuildDefinitionTemplate ToBuildDefinitionTemplate(
      BuildDefinitionTemplate3_2 source,
      BuildDefinitionTemplate partlySerializedTemplate)
    {
      if (source == null)
        return (BuildDefinitionTemplate) null;
      return new BuildDefinitionTemplate()
      {
        CanDelete = source.CanDelete,
        Category = source.Category,
        DefaultHostedQueue = source.DefaultHostedQueue,
        Description = source.Description,
        IconTaskId = source.IconTaskId,
        Id = source.Id,
        Name = source.Name,
        Template = ServerBuildDefinitionHelpers.GetBuildDefinition(source.Template, partlySerializedTemplate.Template),
        Icons = partlySerializedTemplate.Icons
      };
    }
  }
}
