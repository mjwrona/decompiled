// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildJobDefinitionConstants
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildJobDefinitionConstants
  {
    public const string BuildDefinitionNode = "BuildDefinition";
    public const string ProjectIdNode = "ProjectId";
    public const string DefinitionIdNode = "DefinitionId";
    public const string LastVersionEvaluatedNode = "LastVersionEvaluated";
    public const string BranchFiltersNode = "BranchFilters";
    public const string BranchFilterNode = "BranchFilter";
    public const string TriggerTypeNode = "TriggerType";
    public const string CurrentConnectionIdNode = "CurrentConnectionId";
    public const string LastFailedBuildDateTimeNode = "LastFailedBuildDateTime";
    public const string NumberTimesRunNode = "NumberTimesRun";
    public const string ScheduleOnlyWithChangesNode = "ScheduleOnlyWithChanges";
    public const string ScheduleNameNode = "ScheduleName";
    public const string RetriesCountNode = "RetriesCount";
    public const string BatchSchedules = "BatchSchedules";
    public const int MaxScheduledBuildRetries = 3;
  }
}
