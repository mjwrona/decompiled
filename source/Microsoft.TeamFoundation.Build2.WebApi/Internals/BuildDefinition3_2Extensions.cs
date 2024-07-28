// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Internals.BuildDefinition3_2Extensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.WebApi.Internals
{
  internal static class BuildDefinition3_2Extensions
  {
    public static BuildDefinition ToBuildDefinition(this BuildDefinition3_2 source)
    {
      if (source == null)
        return (BuildDefinition) null;
      BuildDefinition buildDefinition1 = new BuildDefinition();
      buildDefinition1.AuthoredBy = source.AuthoredBy;
      buildDefinition1.BadgeEnabled = source.BadgeEnabled;
      buildDefinition1.BuildNumberFormat = source.BuildNumberFormat;
      buildDefinition1.Comment = source.Comment;
      buildDefinition1.CreatedDate = source.CreatedDate;
      buildDefinition1.DefinitionQuality = source.DefinitionQuality;
      buildDefinition1.Description = source.Description;
      buildDefinition1.DropLocation = source.DropLocation;
      buildDefinition1.Id = source.Id;
      buildDefinition1.JobAuthorizationScope = source.JobAuthorizationScope;
      buildDefinition1.JobCancelTimeoutInMinutes = source.JobCancelTimeoutInMinutes;
      buildDefinition1.JobTimeoutInMinutes = source.JobTimeoutInMinutes;
      buildDefinition1.LatestBuild = source.LatestBuild;
      buildDefinition1.LatestCompletedBuild = source.LatestCompletedBuild;
      buildDefinition1.Name = source.Name;
      buildDefinition1.ParentDefinition = source.ParentDefinition;
      buildDefinition1.Path = source.Path;
      buildDefinition1.ProcessParameters = source.ProcessParameters;
      buildDefinition1.Project = source.Project;
      buildDefinition1.Queue = source.Queue;
      buildDefinition1.QueueStatus = source.QueueStatus;
      buildDefinition1.Repository = source.Repository;
      buildDefinition1.Revision = source.Revision;
      buildDefinition1.Type = source.Type;
      buildDefinition1.Uri = source.Uri;
      buildDefinition1.Url = source.Url;
      BuildDefinition buildDefinition2 = buildDefinition1;
      if (source.Demands.Count > 0)
        buildDefinition2.Demands.AddRange((IEnumerable<Demand>) source.Demands);
      if (source.Metrics.Count > 0)
        buildDefinition2.Metrics.AddRange((IEnumerable<BuildMetric>) source.Metrics);
      if (source.Options.Count > 0)
        buildDefinition2.Options.AddRange((IEnumerable<BuildOption>) source.Options);
      DesignerProcess designerProcess = new DesignerProcess();
      buildDefinition2.Process = (BuildProcess) designerProcess;
      Phase phase = new Phase();
      designerProcess.Phases.Add(phase);
      if (source.Steps.Count > 0)
        phase.Steps.AddRange((IEnumerable<BuildDefinitionStep>) source.Steps);
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) source.Properties)
        buildDefinition2.Properties.Add(property.Key, property.Value);
      if (source.Tags.Count > 0)
        buildDefinition2.Tags.AddRange((IEnumerable<string>) source.Tags);
      if (source.Triggers.Count > 0)
        buildDefinition2.Triggers.AddRange((IEnumerable<BuildTrigger>) source.Triggers);
      foreach (KeyValuePair<string, BuildDefinitionVariable> variable in (IEnumerable<KeyValuePair<string, BuildDefinitionVariable>>) source.Variables)
        buildDefinition2.Variables.Add(variable.Key, variable.Value);
      return buildDefinition2;
    }
  }
}
