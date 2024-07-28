// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WellKnownDataProviderKeys
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WellKnownDataProviderKeys
  {
    public const string MyDefinitions = "TFS.Build.MyDefinitions";
    public const string AllDefinitions = "TFS.Build.AllDefinitions";
    public const string QueuedDefinitions = "TFS.Build.QueuedDefinitions";
    public const string AllBuilds = "TFS.Build.AllBuilds";
    public const string DefinitionSummary = "TFS.Build.DefinitionSummary";
    public const string DefinitionHistory = "TFS.Build.DefinitionHistory";
    public const string DefinitionDeletedHistory = "TFS.Build.DefinitionDeletedHistory";
    public const string Builds = "TFS.Build.Builds";
    public const string Changes = "TFS.Build.Changes";
    public const string Definitions = "TFS.Build.Definitions";
    public const string Folders = "TFS.Build.Folders";
    public const string Queues = "TFS.Build.Queues";
    public const string BuildHistory = "TFS.Build.BuildHistory";
    public const string NewCIWorkflowOptInState = "TFS.Build.NewCIWorkflowOptInState";
    public const string NewCIWorkflowPreviewFeatureState = "TFS.Build.NewCIWorkflowPreviewFeatureState";
    public const string AllDefinitionIds = "TFS.Build.AllDefinitions.DefinitionIds";
    public const string BuildIds = "TFS.Build.Mine.BuildIds";
    public const string HasMyBuilds = "TFS.Build.Mine.HasMyBuilds";
    public const string MyFavoriteDefinitionIds = "TFS.Build.MyFavoriteDefinitionIds";
    public const string TeamFavoriteDefinitionIds = "TFS.Build.TeamFavoriteDefinitionIds";
    public const string BuildsContinuationToken = "TFS.Build.Builds.ContinuationToken";
    public const string DefinitionsContinuationToken = "TFS.Build.Definitions.ContinuationToken";
  }
}
