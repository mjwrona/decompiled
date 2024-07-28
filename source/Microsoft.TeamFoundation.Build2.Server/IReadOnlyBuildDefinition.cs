// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IReadOnlyBuildDefinition
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public interface IReadOnlyBuildDefinition
  {
    Guid AuthoredBy { get; }

    bool BadgeEnabled { get; }

    IReadOnlyList<BuildCompletionTrigger> BuildCompletionTriggers { get; }

    string BuildNumberFormat { get; }

    string BuildOptionsString { get; }

    string Comment { get; }

    DateTime CreatedDate { get; }

    Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? DefinitionQuality { get; }

    List<Demand> Demands { get; }

    string DemandsString { get; }

    string Description { get; }

    IReadOnlyList<IReadOnlyBuildDefinition> Drafts { get; }

    string DropLocation { get; }

    int Id { get; }

    BuildAuthorizationScope JobAuthorizationScope { get; }

    int JobCancelTimeoutInMinutes { get; }

    int JobTimeoutInMinutes { get; }

    IReadOnlyBuildData LatestBuild { get; }

    IReadOnlyBuildData LatestCompletedBuild { get; }

    IReadOnlyList<BuildMetric> Metrics { get; }

    string Name { get; }

    IReadOnlyList<BuildOption> Options { get; }

    IReadOnlyBuildDefinition ParentDefinition { get; }

    string Path { get; }

    BuildProcess Process { get; }

    ProcessParameters ProcessParameters { get; }

    string ProcessParametersString { get; }

    Guid ProjectId { get; }

    string ProjectName { get; }

    PropertiesCollection Properties { get; }

    AgentPoolQueue Queue { get; }

    DefinitionQueueStatus QueueStatus { get; }

    BuildRepository Repository { get; }

    string RepositoryString { get; }

    int? Revision { get; }

    IReadOnlyList<string> Tags { get; }

    IReadOnlyList<BuildTrigger> Triggers { get; }

    string TriggersString { get; }

    DefinitionType Type { get; }

    Uri Uri { get; }

    string Url { get; }

    IReadOnlyList<VariableGroup> VariableGroups { get; }

    string VariableGroupsString { get; }

    IReadOnlyDictionary<string, BuildDefinitionVariable> Variables { get; }

    string VariablesString { get; }

    string GetToken();
  }
}
