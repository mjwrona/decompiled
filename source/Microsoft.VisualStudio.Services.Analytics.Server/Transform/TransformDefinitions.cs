// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformDefinitions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  public static class TransformDefinitions
  {
    private static readonly Guid TriggerGroupBuildTaskResult = new Guid("A0B54250-9D2B-4626-9EB6-7183DC565125");
    private static readonly Guid TriggerGroupPipelineJob = new Guid("67B45FF0-E4C4-469C-AC8B-C5720DF7F908");
    public static readonly TransformDefinition[] All = new TransformDefinition[207]
    {
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "merge", 8, "Model.Project", "merge", "batch", "StageProject_ModelProject_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Process", "merge", 8, "Model.Project", "merge", "batch", "StageProject_ModelProject_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "delete", 3, "Model.Project", "merge", "table", "StageProject_ModelProject_TableDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "replace", 8, "Internal.WorkItemTypeState", "replace", "batch", "Any_InternalWorkItemTypeState_BatchReplace", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Process", "replace", 8, "Internal.WorkItemTypeState", "replace", "batch", "Any_InternalWorkItemTypeState_BatchReplace", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Process", "replace", 8, "Internal.ProcessField", "merge", "batch", "StageProcess_InternalProcessField_BatchMerge", 9),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "replace", 5, "Model.WorkItemTypeField", "replace", "batch", "Any_ModelWorkItemTypeField_BatchReplace", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Process", "replace", 5, "Model.WorkItemTypeField", "replace", "batch", "Any_ModelWorkItemTypeField_BatchReplace", 2),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemDestroyed", "insert", 5, "WorkItemRevision", "delete", "batch", "StageWorkItemDestroyed_StageWorkItemRevision_BatchDelete", 2),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemDestroyed", "insert", 5, "WorkItemLink", "delete", "batch", "StageWorkItemDestroyed_StageWorkItemLink_BatchDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(30, new int?(), "WorkItemRevision", "insert", 8, "Internal.WorkItemRevisionReserved", "insert", "batch", "StageWorkItemRevision_InternalWorkItemRevisionReserved_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemRevision", "merge", 8, "Model.Tag", "insert", "batch", "StageWorkItemRevision_ModelTag_BatchInsert", 5),
      (TransformDefinition) new TriggeredTransformDefinition(30, new int?(), "WorkItemRevision", "merge", 8, "Model.WorkItemTag", "replace", "batch", "StageWorkItemRevision_ModelWorkItemTag_BatchMerge", 8),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "WorkItemRevision", "merge", 8, "Model.TeamField", "merge", "batch", "StageWorkItemRevision_ModelTeamField_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(30, new int?(), "WorkItemRevision", "merge", 8, "Model.WorkItemProcess", "replace", "batch", "StageWorkItemRevision_ModelWorkItemProcess_BatchReplace_v2", 1),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "WorkItemRevision", "merge", 8, "Model.User", "merge", "batch", "StageWorkItemRevision_ModelUser_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemRevision", "merge", 8, "Internal.WorkItemRevisionKanban", "merge", "batch", "StageWorkItemRevision_InternalWorkItemRevisionKanban_BatchMerge", 3),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemRevision", "delete", 5, "Internal.WorkItemRevisionKanban", "delete", "batch", "StageWorkItemRevision_InternalWorkItemRevisionKanban_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(11, new int?(29), "WorkItemRevision", "merge", 8, "Model.WorkItem", "merge", "batch", "StageWorkItemRevision_ModelWorkItem_BatchMerge", 2),
      (TransformDefinition) new TriggeredTransformDefinition(30, new int?(), "WorkItemRevision", "insert", 8, "Model.WorkItem", "merge", "batch", "StageWorkItemRevision_ModelWorkItem_BatchMerge_FromInsert", 1),
      (TransformDefinition) new TriggeredTransformDefinition(30, new int?(), "WorkItemRevision", "update", 5, "Model.WorkItem", "merge", "batch", "StageWorkItemRevision_ModelWorkItem_BatchMerge_FromUpdate", 1),
      (TransformDefinition) new TriggeredTransformDefinition(11, new int?(), "WorkItemRevision", "delete", 5, "Model.WorkItem", "delete", "batch", "StageWorkItemRevision_ModelWorkItem_BatchDelete", 3),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(29), "WorkItemRevision", "merge", 8, "Model.WorkItemTag", "replace", "batch", "StageWorkItemRevision_ModelWorkItemTag_BatchMerge", 8),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(29), "WorkItemRevision", "merge", 8, "Model.WorkItemProcess", "replace", "batch", "StageWorkItemRevision_ModelWorkItemProcess_BatchReplace_v2", 0),
      (TransformDefinition) new TriggeredTransformDefinition(24, new int?(), "Internal.ProcessField", "merge", 5, "Model.WorkItem", "update", "batch", "InternalProcessField_ModelWorkItem_BatchUpdate", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemArea", "merge", 5, "Model.Area", "merge", "batch", "StageWorkItemArea_ModelArea_BatchMerge", 3),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemArea", "delete", 3, "Model.Area", "delete", "table", "StageWorkItemArea_ModelArea_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemIteration", "merge", 5, "Model.Iteration", "merge", "batch", "StageWorkItemIteration_ModelIteration_BatchMerge", 7),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemIteration", "delete", 3, "Model.Iteration", "delete", "table", "StageWorkItemIteration_ModelIteration_TableDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Tag", "merge", 8, "Model.Tag", "merge", "batch", "StageTag_ModelTag_BatchMerge", 2),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Tag", "delete", 3, "Model.Tag", "delete", "table", "StageTag_ModelTag_TableDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(14, new int?(36), "Tag", "merge", 5, "Model.WorkItem", "update", "batch", "StageTag_ModelWorkItem_BatchUpdate", 3),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "merge", 5, "Model.Tag", "merge", "batch", "StageProject_ModelTag_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "delete", 3, "Model.Tag", "delete", "table", "StageProject_ModelTag_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(13, new int?(), "Model.WorkItem", "delete", 5, "Model.WorkItemTag", "delete", "batch", "ModelWorkItem_ModelWorkItemTag_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(37, new int?(), "Model.Tag", "merge", 5, "Model.WorkItem", "update", "batch", "ModelTag_ModelWorkItem_BatchUpdate", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.Tag", "delete", 3, "Model.WorkItemTag", "delete", "table", "ModelTag_ModelWorkItemTag_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "merge", 5, "Model.Area", "update", "batch", "StageWorkItemArea_ModelArea_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Project", "merge", 5, "Model.Iteration", "update", "batch", "StageWorkItemIteration_ModelIteration_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "KanbanBoardColumn", "merge", 8, "Model.BoardLocation", "merge", "batch", "Any_ModelBoardLocation_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "KanbanBoardColumn", "delete", 3, "Model.BoardLocation", "delete", "table", "StageKanbanBoardColumn_ModelBoardLocation_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "KanbanBoardRow", "merge", 8, "Model.BoardLocation", "merge", "batch", "Any_ModelBoardLocation_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "KanbanBoardRow", "delete", 3, "Model.BoardLocation", "delete", "table", "StageKanbanBoardRow_ModelBoardLocation_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "TeamSetting", "merge", 8, "Model.BoardLocation", "replace", "batch", "Any_ModelBoardLocation_BatchMerge", 4),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Internal.WorkItemRevisionKanban", "merge", 8, "Model.WorkItemBoardLocation", "replace", "batch", "InternalWorkItemRevisionKanban_ModelWorkItemBoardLocation_BatchMerge", 8),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Internal.WorkItemRevisionKanban", "delete", 5, "Model.WorkItemBoardLocation", "delete", "batch", "InternalWorkItemRevisionKanban_ModelWorkItemBoardLocation_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "TeamSetting", "merge", 5, "Model.Team", "merge", "batch", "StageTeamSetting_ModelTeam_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "TeamSetting", "delete", 3, "Model.Team", "delete", "table", "StageTeamSetting_ModelTeam_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "TeamSetting", "delete", 3, "Model.BoardLocation", "delete", "table", "StageTeamSetting_ModelBoardLocation_TableDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.BoardLocation", "merge", 5, "Model.WorkItemBoardLocation", "replace", "batch", "ModelBoardLocation_ModelWorkItemBoardLocation_BatchMerge", 8),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.BoardLocation", "delete", 3, "Model.WorkItemBoardLocation", "delete", "table", "ModelBoardLocation_ModelWorkItemBoardLocation_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(13, new int?(), "Model.WorkItem", "delete", 5, "Model.WorkItemBoardLocation", "delete", "batch", "ModelWorkItem_ModelWorkItemBoardLocation_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Process", "merge", 5, "Model.TeamField", "merge", "batch", "Any_ModelTeamField_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Project", "merge", 5, "Model.TeamField", "merge", "batch", "Any_ModelTeamField_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "TeamSetting", "merge", 5, "Model.TeamField", "merge", "batch", "Any_ModelTeamField_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Model.Area", "merge", 5, "Model.TeamField", "merge", "batch", "Any_ModelTeamField_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "TeamSetting", "merge", 5, "Model.TeamToTeamField", "replace", "batch", "StageTeamSetting_ModelTeamToTeamField_BatchReplace", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Model.Area", "merge", 5, "Model.TeamToTeamField", "replace", "batch", "ModelArea_ModelTeamToTeamField_BatchReplace", 0),
      (TransformDefinition) new TriggeredTransformDefinition(58, new int?(), "Model.TeamField", "merge", 7, "Model.TeamToTeamField", "replace", "batch", "ModelTeamField_ModelTeamToTeamField_BatchReplace", 0, "Analytics.Transform.ModelTeamField"),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Model.TeamField", "delete", 3, "Model.TeamToTeamField", "delete", "table", "ModelTeamField_ModelTeamToTeamField_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Model.Team", "delete", 3, "Model.TeamToTeamField", "delete", "table", "ModelTeam_ModelTeamToTeamField_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "TeamSetting", "merge", 5, "Model.TeamIteration", "replace", "batch", "StageTeamSetting_ModelTeamIteration_BatchMerge", 2),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.Iteration", "insert", 5, "Model.TeamIteration", "insert", "batch", "ModelIteration_ModelTeamIteration_BatchInsert", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.Iteration", "delete", 3, "Model.TeamIteration", "delete", "table", "ModelIteration_ModelTeamIteration_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.Team", "delete", 3, "Model.TeamIteration", "delete", "table", "ModelTeam_ModelTeamIteration_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "TeamSetting", "merge", 5, "Model.TeamArea", "replace", "batch", "StageTeamSetting_ModelTeamArea_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.Area", "merge", 5, "Model.TeamArea", "merge", "batch", "ModelArea_ModelTeamArea_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.Area", "delete", 3, "Model.TeamArea", "delete", "table", "ModelArea_ModelTeamArea_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Model.Team", "delete", 3, "Model.TeamArea", "delete", "table", "ModelTeam_ModelTeamArea_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "Process", "merge", 5, "Model.Process", "merge", "batch", "Any_ModelProcess_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "TeamSetting", "merge", 5, "Model.Process", "merge", "batch", "Any_ModelProcess_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "Project", "merge", 5, "Model.Process", "merge", "batch", "Any_ModelProcess_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "TeamSetting", "delete", 5, "Model.Process", "delete", "batch", "StageTeamSetting_ModelProcess_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(25), "Model.Process", "merge", 5, "Model.WorkItemProcess", "insert", "batch", "ModelProcess_ModelWorkItemProcess_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Model.Process", "merge", 5, "Model.WorkItemProcess", "insert", "batch", "ModelProcess_ModelWorkItemProcess_BatchInsert_v2", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "Model.Process", "delete", 3, "Model.WorkItemProcess", "delete", "table", "ModelProcess_ModelWorkItemProcess_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(25), "Model.TeamArea", "merge", 5, "Model.WorkItemProcess", "insert", "batch", "ModelProcess_ModelWorkItemProcess_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(25), "Model.TeamArea", "delete", 3, "Model.WorkItemProcess", "delete", "batch", "ModelTeamArea_ModelWorkItemProcess_BatchDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Model.TeamToTeamField", "merge", 5, "Model.WorkItemProcess", "insert", "batch", "ModelProcess_ModelWorkItemProcess_BatchInsert_v2", 0),
      (TransformDefinition) new TriggeredTransformDefinition(26, new int?(), "Model.TeamToTeamField", "delete", 3, "Model.WorkItemProcess", "delete", "batch", "ModelTeamToTeamField_ModelWorkItemProcess_BatchDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(17, new int?(), "Model.WorkItem", "delete", 3, "Model.WorkItemProcess", "delete", "batch", "ModelWorkItem_ModelWorkItemProcess_BatchDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemLink", "merge", 8, "Model.WorkItemLinkHistory", "merge", "batch", "StageWorkItemLink_WorkItemLinkHistory_BatchMerge", 6),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemLink", "delete", 5, "Model.WorkItemLinkHistory", "delete", "batch", "StageWorkItemLink_WorkItemLinkHistory_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemLinkType", "merge", 5, "Model.WorkItemLinkHistory", "merge", "batch", "StageWorkItemLinkType_WorkItemLinkHistory_BatchMerge", 3),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "WorkItemLinkType", "delete", 3, "Model.WorkItemLinkHistory", "delete", "table", "StageWorkItemLinkType_WorkItemLinkHistory_TableDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(31, new int?(), "WorkItemRevision", "insert", 7, "Model.WorkItemDescendant", "insert", "batch", "StageWorkItemRevision_ModelWorkItemDescendant_BatchInsert", 1),
      (TransformDefinition) new TriggeredTransformDefinition(31, new int?(), "Model.WorkItemLinkHistory", "merge", 7, "Model.WorkItemDescendant", "replace", "batch", "ModelWorkItemLinkHistory_ModelWorkItemDescendant_BatchReplace", 4),
      (TransformDefinition) new TriggeredTransformDefinition(32, new int?(), "WorkItemLink", "delete", 5, "Model.WorkItemDescendant", "delete", "batch", "StageWorkItemLink_ModelWorkItemDescendant_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(34, new int?(), "WorkItemRevision", "delete", 5, "Model.WorkItemDescendant", "delete", "batch", "StageWorkItemRevision_ModelWorkItemDescendant_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "WorkItemRevision", "insert", 7, "Model.WorkItemDescendantHistory", "insert", "batch", "StageWorkItemRevision_ModelWorkItemDescendantHistory_BatchInsert", 1),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "Model.WorkItemLinkHistory", "merge", 7, "Model.WorkItemDescendantHistory", "replace", "batch", "ModelWorkItemLinkHistory_ModelWorkItemDescendantHistory_BatchMerge", 4),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "WorkItemLink", "delete", 5, "Model.WorkItemDescendantHistory", "delete", "batch", "StageWorkItemLink_ModelWorkItemDescendantHistory_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(34, new int?(), "WorkItemRevision", "delete", 5, "Model.WorkItemDescendantHistory", "delete", "batch", "StageWorkItemRevision_ModelWorkItemDescendantHistory_BatchDelete", 1),
      (TransformDefinition) new TriggeredTransformDefinition(12, new int?(), "Model.WorkItemLinkHistory", "merge", 5, "Model.WorkItem", "update", "batch", "ModelWorkItemLinkHistory_ModelWorkItem_BatchUpdate", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Collection", "merge", 8, "Internal.CollectionTimeZone", "merge", "batch", "StageCollection_InternalCollectionTimeZone_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.Date", "update", "table", "InternalCollectionTimeZone_ModelDate_TableUpdate", 3),
      (TransformDefinition) new TriggeredTransformDefinition(1, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.Iteration", "update", "table", "InternalCollectionTimeZone_ModelIteration_TableUpdate", 0),
      (TransformDefinition) new TriggeredTransformDefinition(12, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.WorkItem", "update", "table", "InternalCollectionTimeZone_ModelWorkItem_TableUpdate", 2),
      (TransformDefinition) new TriggeredTransformDefinition(20, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.WorkItemLinkHistory", "update", "table", "InternalCollectionTimeZone_ModelWorkItemLinkHistory_TableUpdate", 2),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.WorkItemDescendantHistory", "update", "table", "InternalCollectionTimeZone_ModelWorkItemDescendantHistory_TableUpdate", 2),
      (TransformDefinition) new TriggeredTransformDefinition(12, new int?(), "Internal.WorkItemTypeState", "merge", 5, "Model.WorkItem", "update", "batch", "InternalWorkItemTypeState_ModelWorkItem_BatchUpdate", 2),
      (TransformDefinition) new TriggeredTransformDefinition(28, new int?(), "Model.Project", "merge", 5, "Model.TeamField", "insert", "batch", "ModelProject_ModelTeamField_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(28, new int?(), "Model.Project", "merge", 5, "Model.WorkItem", "update", "batch", "ModelProject_ModelWorkItem_TableUpdate", 0),
      (TransformDefinition) new TriggeredTransformDefinition(28, new int?(), "Model.Project", "merge", 5, "Model.WorkItemProcess", "replace", "batch", "ModelProject_ModelWorkItemProcess_BatchReplace", 0),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "BuildDefinition", "merge", 7, "Model.BuildPipeline", "merge", "batch", "StageBuildDefinition_ModelBuildPipeline_BatchMerge", 0, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(32, new int?(), "TaskDefinitionReference", "merge", 7, "Model.BuildPipelineTask", "insert", "batch", "StageTaskDefinitionReference_ModelBuildPipelineTask_BatchInsert", 1, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(32, new int?(), "Build", "merge", 7, "Model.Branch", "insert", "batch", "StageBuild_ModelBranch_BatchInsert", 0, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "Build", "merge", 7, "Model.BuildPipeline", "insert", "batch", "StageBuild_ModelBuildPipeline_BatchInsert", 0, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "Build", "merge", 7, "Model.Build", "merge", "batch", "StageBuild_ModelBuild_BatchMerge", 0, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(38, new int?(), "TaskTimelineRecord", "merge", TransformDefinitions.TriggerGroupPipelineJob, 5, "Model.PipelineJob", "insert", "batch", "Any_ModelPipelineJob_BatchInsert", 0, "Analytics.Transform.PipelineJob"),
      (TransformDefinition) new TriggeredTransformDefinition(38, new int?(), "Build", "merge", TransformDefinitions.TriggerGroupPipelineJob, 5, "Model.PipelineJob", "insert", "batch", "Any_ModelPipelineJob_BatchInsert", 0, "Analytics.Transform.PipelineJob"),
      (TransformDefinition) new TriggeredTransformDefinition(38, new int?(), "TaskPlan", "merge", TransformDefinitions.TriggerGroupPipelineJob, 5, "Model.PipelineJob", "insert", "batch", "Any_ModelPipelineJob_BatchInsert", 0, "Analytics.Transform.PipelineJob"),
      (TransformDefinition) new TriggeredTransformDefinition(38, new int?(), "TaskDefinitionReference", "merge", TransformDefinitions.TriggerGroupPipelineJob, 5, "Model.PipelineJob", "insert", "batch", "Any_ModelPipelineJob_BatchInsert", 0, "Analytics.Transform.PipelineJob"),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "TaskTimelineRecord", "merge", TransformDefinitions.TriggerGroupBuildTaskResult, 5, "Model.BuildTaskResult", "insert", "batch", "StageTaskTimelineRecord_ModelBuildTaskResult_BatchInsert", 1, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "Build", "merge", TransformDefinitions.TriggerGroupBuildTaskResult, 5, "Model.BuildTaskResult", "insert", "batch", "Any_ModelBuildTaskResult_BatchInsert", 1, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "TaskPlan", "merge", TransformDefinitions.TriggerGroupBuildTaskResult, 5, "Model.BuildTaskResult", "insert", "batch", "Any_ModelBuildTaskResult_BatchInsert", 1, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(33, new int?(), "TaskDefinitionReference", "merge", TransformDefinitions.TriggerGroupBuildTaskResult, 5, "Model.BuildTaskResult", "insert", "batch", "StageTaskDefinitionReference_ModelBuildTaskResult_BatchInsert", 2, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(40, new int?(), "TaskTimelineRecord", "merge", 5, "Model.PipelineJob", "update", "batch", "Any_ModelPipelineJob_Stage_BatchUpdate", 0, "Analytics.Transform.PipelineJobUpdate"),
      (TransformDefinition) new TriggeredTransformDefinition(40, new int?(), "TaskTimelineRecord", "merge", 5, "Model.PipelineJob", "update", "batch", "Any_ModelPipelineJob_Phase_BatchUpdate", 0, "Analytics.Transform.PipelineJobUpdate"),
      (TransformDefinition) new TriggeredTransformDefinition(40, new int?(), "Build", "merge", 5, "Model.PipelineJob", "update", "batch", "Any_ModelPipelineJob_Stage_BatchUpdate", 0, "Analytics.Transform.PipelineJobUpdate"),
      (TransformDefinition) new TriggeredTransformDefinition(40, new int?(), "Build", "merge", 5, "Model.PipelineJob", "update", "batch", "Any_ModelPipelineJob_Phase_BatchUpdate", 0, "Analytics.Transform.PipelineJobUpdate"),
      (TransformDefinition) new TriggeredTransformDefinition(40, new int?(), "TaskPlan", "merge", 5, "Model.PipelineJob", "update", "batch", "Any_ModelPipelineJob_Stage_BatchUpdate", 0, "Analytics.Transform.PipelineJobUpdate"),
      (TransformDefinition) new TriggeredTransformDefinition(40, new int?(), "TaskPlan", "merge", 5, "Model.PipelineJob", "update", "batch", "Any_ModelPipelineJob_Phase_BatchUpdate", 0, "Analytics.Transform.PipelineJobUpdate"),
      (TransformDefinition) new TriggeredTransformDefinition(36, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.Build", "update", "table", "InternalCollectionTimeZone_ModelBuild_TableUpdate", 0, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(36, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.BuildTaskResult", "update", "table", "InternalCollectionTimeZone_ModelBuildTaskResult_TableUpdate", 0, "Analytics.Transform.Build"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "TaskPlan", "delete", 3, "TaskTimelineRecord", "delete", "batch", "StageTaskPlan_StageTimeLineRecord_BatchDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(54, new int?(), "TaskAgentRequest", "merge", 7, "Model.BuildPipeline", "insert", "batch", "StageTaskAgentRequest_ModelBuildPipeline_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(54, new int?(), "TaskAgentRequest", "merge", 5, "Model.TaskAgentRequest", "merge", "batch", "StageTaskAgentRequest_ModelTaskAgentRequest_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(55, new int?(), "ParallelPipelineJobs", "insert", 5, "Model.ParallelPipelineJobs", "insert", "batch", "StageParallelPipelineJobs_ModelParallelPipelineJobs_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(55, new int?(), "TaskAgentPoolSize", "insert", 5, "Model.TaskAgentPoolSize", "insert", "batch", "StageTaskAgentPoolSize_ModelTaskAgentPoolSize_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(56, new int?(), "TaskAgentPool", "merge", 5, "Model.TaskAgentPool", "merge", "batch", "StageTaskAgentPool_ModelTaskAgentPool_BatchMerge", 0),
      (TransformDefinition) new TimedTransformDefinition(44, new int?(), 1440, 3, "Model.BuildTaskResult", "delete", "table", "Daily_ModelBuildTaskResult_TableDelete", 0),
      (TransformDefinition) new TimedTransformDefinition(44, new int?(), 1440, 3, "Model.Build", "delete", "table", "Daily_ModelBuild_TableDelete", 0),
      (TransformDefinition) new TimedTransformDefinition(57, new int?(), 1440, 3, "Model.TaskAgentRequest", "delete", "table", "Daily_ModelTaskAgentRequest_TableDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(46, new int?(), "AzDevGitCommit", "merge", 3, "Model.Repository", "merge", "batch", "StageAzDevGitCommit_ModelRepository_BatchMerge", 0, "Analytics.Transform.LeadTime"),
      (TransformDefinition) new TriggeredTransformDefinition(46, new int?(), "AzDevGitCommit", "merge", 3, "Model.Commit", "merge", "batch", "StageAzDevGitCommit_ModelCommit_BatchMerge", 0, "Analytics.Transform.LeadTime"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "PipelineEnvironment", "insert", 3, "Model.PipelineEnvironment", "insert", "batch", "StagePipelineEnvironment_ModelPipelineEnvironment_BatchInsert", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.PipelineEnvironment", "update", "table", "InternalCollectionTimeZone_ModelPipelineEnvironment_TableUpdate", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "TaskTimelineRecord", "insert", 3, "Model.CommitToDeployment", "insert", "batch", "StageTaskTimelineRecord_ModelCommitToDeployment_BatchInsert", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "JobCommitMapping", "insert", 3, "Model.CommitToDeployment", "insert", "batch", "StageJobCommitMapping_ModelCommitToDeployment_BatchInsert", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "Model.Commit", "insert", 3, "Model.CommitToDeployment", "insert", "batch", "ModelCommit_ModelCommitToDeployment_BatchInsert", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "Build", "insert", 3, "Model.CommitToDeployment", "insert", "batch", "StageBuild_ModelCommitToDeployment_BatchInsert", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "TaskPlan", "insert", 3, "Model.CommitToDeployment", "insert", "batch", "StageTaskPlan_ModelCommitToDeployment_BatchInsert", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "Model.PipelineEnvironment", "insert", 3, "Model.CommitToDeployment", "insert", "batch", "ModelPipelineEnvironment_ModelCommitToDeployment_BatchInsert", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(50, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.CommitToDeployment", "update", "table", "InternalCollectionTimeZone_ModelCommitToDeployment_TableUpdate", 0, "Analytics.Transform.LeadTimeCommitToDeployment"),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "TestCaseReference", "merge", 7, "Model.Test", "merge", "batch", "StageTestCaseReference_ModelTest_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "TestRun", "merge", 7, "Model.Branch", "insert", "batch", "StageTestRun_ModelBranch_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "TestRun", "merge", 7, "Model.Build", "insert", "batch", "StageTestRun_ModelBuild_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(27, new int?(), "TestRun", "merge", 7, "Model.BuildPipeline", "insert", "batch", "StageTestRun_ModelBuildPipeline_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "TestRun", "merge", 5, "Model.TestRun", "merge", "batch", "StageTestRun_ModelTestRun_BatchMerge", 1),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "TestResult", "insert", 5, "Model.Test", "insert", "batch", "StageTestResult_ModelTest_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "TestResult", "insert", 5, "Model.TestResult", "insert", "batch", "StageTestResult_ModelTestResult_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "TestResult", "insert", 5, "Model.TestRun", "update", "batch", "StageTestResult_ModelTestRun_BatchUpdate", 0),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "Model.TestRun", "insert", 5, "Model.TestResult", "insert", "batch", "ModelTestRun_ModelTestResult_BatchInsert", 0),
      (TransformDefinition) new TriggeredTransformDefinition(27, new int?(), "Model.TestResult", "insert", 5, "Model.TestResultDaily", "merge", "batch", "ModelTestResult_ModelTestResultDaily_BatchMerge", 0),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.TestRun", "update", "table", "InternalCollectionTimeZone_ModelTestRun_TableUpdate", 0),
      (TransformDefinition) new TriggeredTransformDefinition(15, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.TestResult", "update", "table", "InternalCollectionTimeZone_ModelTestResult_TableUpdate", 0),
      (TransformDefinition) new TriggeredTransformDefinition(16, new int?(), "TestRun", "delete", 3, "TestResult", "delete", "batch", "StageTestRun_StageTestResult_BatchDelete", 0),
      (TransformDefinition) new TriggeredTransformDefinition(40, new int?(), "TestConfiguration", "merge", 7, "Model.TestConfiguration", "merge", "batch", "StageTestConfiguration_ModelTestConfiguration_BatchMerge", 0, "Analytics.Transform.TestConfiguration"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "TestSuite", "merge", 7, "Model.TestSuite", "merge", "batch", "StageTestSuite_ModelTestSuite_BatchMerge", 1, "Analytics.Transform.TestSuite"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "WorkItemRevision", "merge", 5, "Model.TestSuite", "update", "batch", "StageWorkItemRevision_ModelTestSuite_BatchUpdate", 0, "Analytics.Transform.TestSuite"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "TestPlan", "merge", 7, "Model.TestSuite", "update", "batch", "StageTestPlan_ModelTestSuite_BatchUpdate", 0, "Analytics.Transform.TestSuite"),
      (TransformDefinition) new TriggeredTransformDefinition(42, new int?(), "TestPoint", "merge", 7, "Model.User", "merge", "batch", "StageTestPoint_ModelUser_BatchMerge", 0, "Analytics.Transform.TestPoint"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "TestPoint", "merge", 7, "Model.TestPoint", "merge", "batch", "StageTestPoint_ModelTestPoint_BatchMerge", 0, "Analytics.Transform.TestPoint"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "Model.TestSuite", "merge", 7, "Model.TestPoint", "update", "batch", "ModelTestSuite_ModelTestPoint_BatchUpdate", 2, "Analytics.Transform.TestPoint"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "Model.TestConfiguration", "merge", 7, "Model.TestPoint", "update", "batch", "ModelTestConfiguration_ModelTestPoint_BatchUpdate", 2, "Analytics.Transform.TestPoint"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "WorkItemRevision", "merge", 5, "Model.TestPoint", "update", "batch", "StageWorkItemRevision_ModelTestPoint_BatchUpdate", 0, "Analytics.Transform.TestPoint"),
      (TransformDefinition) new TriggeredTransformDefinition(41, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.TestPoint", "update", "table", "InternalCollectionTimeZone_ModelTestPoint_TableUpdate", 0, "Analytics.Transform.TestPoint"),
      (TransformDefinition) new TriggeredTransformDefinition(43, new int?(), "TestPointHistory", "merge", 7, "Model.User", "merge", "batch", "StageTestPointHistory_ModelUser_BatchMerge", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(43, new int?(45), "TestPointHistory", "merge", 7, "Model.TestPointHistory", "merge", "batch", "Any_ModelTestPointHistory_BatchMerge", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(46, new int?(), "TestPointHistory", "merge", 7, "Model.TestPointHistory", "merge", "batch", "StageTestPointHistory_ModelTestPointHistory_BatchMerge", 1, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(43, new int?(45), "Model.TestConfiguration", "insert", 7, "Model.TestPointHistory", "merge", "batch", "Any_ModelTestPointHistory_BatchMerge", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(46, new int?(), "Model.TestConfiguration", "insert", 7, "Model.TestPointHistory", "merge", "batch", "ModelTestConfiguration_ModelTestPointHistory_BatchMerge", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(43, new int?(45), "Model.TestSuite", "insert", 7, "Model.TestPointHistory", "merge", "batch", "Any_ModelTestPointHistory_BatchMerge", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(46, new int?(), "Model.TestSuite", "insert", 7, "Model.TestPointHistory", "merge", "batch", "ModelTestSuite_ModelTestPointHistory_BatchMerge", 1, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(43, new int?(45), "Model.TestPoint", "merge", 7, "Model.TestPointHistory", "merge", "batch", "Any_ModelTestPointHistory_BatchMerge", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(46, new int?(), "Model.TestPoint", "merge", 7, "Model.TestPointHistory", "merge", "batch", "ModelTestPoint_ModelTestPointHistory_BatchMerge", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(43, new int?(), "Internal.CollectionTimeZone", "merge", 3, "Model.TestPointHistory", "update", "table", "InternalCollectionTimeZone_ModelTestPointHistory_TableUpdate", 0, "Analytics.Transform.TestPointHistory"),
      (TransformDefinition) new TriggeredTransformDefinition(48, new int?(), "GitHubRepository", "merge", 3, "Model.User", "merge", "batch", "StageGitHubRepository_ModelUser_BatchMerge", 0, "Analytics.Transform.GitHubUser"),
      (TransformDefinition) new TriggeredTransformDefinition(48, new int?(), "GitHubCommit", "merge", 3, "Model.User", "merge", "batch", "StageGitHubCommit_ModelUser_BatchMerge", 0, "Analytics.Transform.GitHubUser"),
      (TransformDefinition) new TriggeredTransformDefinition(48, new int?(), "GitHubPullRequest", "merge", 3, "Model.User", "merge", "batch", "StageGitHubPullRequest_ModelUser_BatchMerge", 0, "Analytics.Transform.GitHubUser"),
      (TransformDefinition) new TriggeredTransformDefinition(51, new int?(), "GitHubAccount", "merge", 5, "Model.GitHubAccount", "merge", "batch", "StageGitHubAccount_ModelGitHubAccount_BatchMerge", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubUser", "merge", 5, "Model.GitHubUser", "merge", "batch", "StageGitHubUser_ModelGitHubUser_BatchMerge", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubTeam", "merge", 5, "Model.GitHubTeam", "merge", "batch", "StageGitHubTeam_ModelGitHubTeam_BatchMerge", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubRepositoryGHI", "merge", 5, "Model.GitHubRepository", "merge", "batch", "StageGitHubRepositoryGHI_ModelGitHubRepository_BatchMerge", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubCommitGHI", "merge", 5, "Model.GitHubAccount", "insert", "batch", "StageGitHubCommitGHI_ModelGitHubAccount_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubCommitGHI", "merge", 5, "Model.GitHubRepository", "insert", "batch", "StageGitHubCommitGHI_ModelGitHubRepository_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubCommitGHI", "merge", 5, "Model.GitHubUser", "insert", "batch", "StageGitHubCommitGHI_ModelGitHubUser_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubCommitGHI", "merge", 5, "Model.GitHubCommit", "insert", "batch", "StageGitHubCommitGHI_ModelGitHubCommit_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubTeamUserMap", "merge", 5, "Model.GitHubTeam", "insert", "batch", "StageGitHubTeamUserMap_ModelGitHubTeam_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubTeamUserMap", "merge", 5, "Model.GitHubUser", "insert", "batch", "StageGitHubTeamUserMap_ModelGitHubUser_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(52, new int?(), "GitHubTeamUserMap", "merge", 5, "Model.GitHubTeamUserMap", "merge", "batch", "StageGitHubTeamUserMap_ModelGitHubTeamUser_BatchMerge", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(53, new int?(), "GitHubBranch", "merge", 5, "Model.GitHubBranch", "merge", "batch", "StageGitHubBranch_ModelGitHubBranch_BatchMerge", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(53, new int?(), "GitHubPullRequestGHI", "merge", 5, "Model.GitHubBranch", "insert", "batch", "StageGitHubPullRequestGHI_ModelGitHubBranch_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(53, new int?(), "GitHubPullRequestGHI", "merge", 5, "Model.GitHubRepository", "insert", "batch", "StageGitHubPullRequestGHI_ModelGitHubRepository_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(53, new int?(), "GitHubPullRequestGHI", "merge", 5, "Model.GitHubAccount", "insert", "batch", "StageGitHubPullRequestGHI_ModelGitHubAccount_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(53, new int?(), "GitHubPullRequestGHI", "merge", 5, "Model.GitHubUser", "insert", "batch", "StageGitHubPullRequestGHI_ModelGitHubUser_BatchInsert", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TriggeredTransformDefinition(53, new int?(), "GitHubPullRequestGHI", "merge", 5, "Model.GitHubPullRequest", "merge", "batch", "StageGitHubPullRequestGHI_ModelGitHubPullRequest_BatchMerge", 0, "Analytics.Model.GitHubInsightsCode"),
      (TransformDefinition) new TimedTransformDefinition(21, new int?(), 1440, 3, "Model.TestResult", "delete", "table", "Daily_ModelTestResult_TableDelete", 0),
      (TransformDefinition) new TimedTransformDefinition(27, new int?(), 1440, 3, "Model.TestResultDaily", "delete", "table", "Daily_ModelTestResultDaily_TableDelete", 0),
      (TransformDefinition) new TimedTransformDefinition(21, new int?(), 1440, 3, "Model.TestRun", "delete", "table", "Daily_ModelTestRun_TableDelete", 0),
      (TransformDefinition) new TimedTransformDefinition(21, new int?(), 1440, 3, "TestRun", "delete", "table", "Daily_StageTestRun_TableDelete", 0, "Analytics.Transform.StageTestRetention"),
      (TransformDefinition) new TimedTransformDefinition(21, new int?(), 1440, 3, "TestResult", "delete", "table", "Daily_StageTestResult_TableDelete", 0, "Analytics.Transform.StageTestResultRetention"),
      (TransformDefinition) new TimedTransformDefinition(21, new int?(), 1440, 3, "Model.Test", "delete", "table", "Daily_ModelTest_TableDelete", 0),
      (TransformDefinition) new TimedTransformDefinition(35, new int?(), 10080, 3, "Internal.WorkitemCompletedTimePredictModel", "insert", "table", "Weekly_InternalWorkitemCompletedTimePredictModel_TableInsert", 0, "Analytics.Transform.WorkItemCompletionPredict"),
      (TransformDefinition) new TimedTransformDefinition(44, new int?(), 10080, 3, "Internal.WorkitemTagsPredictModel", "insert", "table", "Weekly_InternalWorkitemTagsPredictModel_TableInsert", 0, "Analytics.Transform.WorkItemTagsPredict"),
      (TransformDefinition) new TimedTransformDefinition(44, new int?(), 10080, 3, "Internal.WorkitemTagsPredictModel", "delete", "table", "Weekly_InternalWorkitemTagsPredictModel_TableDelete", 0, "Analytics.Transform.WorkItemTagsPredict")
    };

    private static int OpMap(string op)
    {
      switch (op)
      {
        case "insert":
          return 1;
        case "update":
          return 2;
        case "delete":
          return 4;
        case "time":
          return 8;
        case "merge":
          return 3;
        case "replace":
          return 7;
        default:
          return 0;
      }
    }

    public static void Validate(IEnumerable<TransformDefinition> definitions)
    {
      TransformDefinition transformDefinition1 = definitions.Select((t, index) => new
      {
        def = t,
        order = index
      }).Join(definitions.Select((t, index) => new
      {
        def = t,
        order = index
      }), t1 => t1.def.TriggerTable, t2 => t2.def.TriggerTable, (t1, t2) => new
      {
        t1 = t1,
        t2 = t2
      }).Where(_param1 => _param1.t1.def.OperationScope == "batch" && _param1.t2.def.OperationScope == "batch" && (TransformDefinitions.OpMap(_param1.t1.def.TriggerOperation) & TransformDefinitions.OpMap(_param1.t2.def.TriggerOperation)) > 0 && _param1.t1.order < _param1.t2.order && _param1.t1.def.TransformPriority < _param1.t2.def.TransformPriority).Select(_param1 => _param1.t1.def).FirstOrDefault<TransformDefinition>();
      if (transformDefinition1 != null)
        throw new InvalidOperationException(AnalyticsResources.TRANSFORM_PRIORITY_CONFLICTS((object) transformDefinition1.TriggerTable, (object) transformDefinition1.SprocName));
      TransformDefinition transformDefinition2 = definitions.Select((t, index) => new
      {
        def = t,
        order = index
      }).Join(definitions.Select((t, index) => new
      {
        def = t,
        order = index
      }), t1 => t1.def.TriggerGroup, t1tg => t1tg.def.TriggerGroup, (t1, t1tg) => new
      {
        t1 = t1,
        t1tg = t1tg
      }).Join(definitions.Select((t, index) => new
      {
        def = t,
        order = index
      }), _param1 => _param1.t1.def.TriggerTable, t2 => t2.def.TriggerTable, (_param1, t2) => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        t2 = t2
      }).Where(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier0.t1.def.OperationScope == "batch" && _param1.\u003C\u003Eh__TransparentIdentifier0.t1tg.def.OperationScope == "batch" && _param1.t2.def.OperationScope == "batch" && (TransformDefinitions.OpMap(_param1.\u003C\u003Eh__TransparentIdentifier0.t1tg.def.TriggerOperation) & TransformDefinitions.OpMap(_param1.t2.def.TriggerOperation)) > 0 && _param1.\u003C\u003Eh__TransparentIdentifier0.t1tg.order < _param1.t2.order && _param1.\u003C\u003Eh__TransparentIdentifier0.t1tg.def.TransformPriority < _param1.t2.def.TransformPriority).Select(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier0.t1tg.def).FirstOrDefault<TransformDefinition>();
      if (transformDefinition2 != null)
        throw new InvalidOperationException(AnalyticsResources.TRANSFORM_PRIORITY_CONFLICTS((object) transformDefinition2.TriggerTable, (object) transformDefinition2.SprocName));
      TransformDefinition transformDefinition3 = definitions.Where<TransformDefinition>((Func<TransformDefinition, bool>) (d => d.TriggerOperation == "time" && d.OperationScope == "batch")).FirstOrDefault<TransformDefinition>();
      if (transformDefinition3 != null)
        throw new InvalidOperationException(AnalyticsResources.INVALID_TRANSFORM_DEFINITION((object) transformDefinition3.SprocName));
    }

    public static IEnumerable<string> EnableWithFeatureNames(
      IEnumerable<TransformDefinition> definitions = null)
    {
      definitions = (IEnumerable<TransformDefinition>) ((object) definitions ?? (object) TransformDefinitions.All);
      return definitions.Where<TransformDefinition>((Func<TransformDefinition, bool>) (d => d.EnableWithFeatureNames != null)).SelectMany<TransformDefinition, string>((Func<TransformDefinition, IEnumerable<string>>) (d => (IEnumerable<string>) d.EnableWithFeatureNames)).Distinct<string>();
    }

    public static IEnumerable<string> DisableWithFeatureNames(
      IEnumerable<TransformDefinition> definitions = null)
    {
      definitions = (IEnumerable<TransformDefinition>) ((object) definitions ?? (object) TransformDefinitions.All);
      return definitions.Where<TransformDefinition>((Func<TransformDefinition, bool>) (d => d.DisableWithFeatureNames != null)).SelectMany<TransformDefinition, string>((Func<TransformDefinition, IEnumerable<string>>) (d => (IEnumerable<string>) d.DisableWithFeatureNames)).Distinct<string>();
    }
  }
}
