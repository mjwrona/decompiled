// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageTypeDefinitions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal static class StageTypeDefinitions
  {
    public const string AxRecordNumberPropertyName = "AxRecordNumber";
    public static readonly IDictionary<string, StageTypeDefinitions.StageTypeDefinition> Definitions = (IDictionary<string, StageTypeDefinitions.StageTypeDefinition>) new Dictionary<string, StageTypeDefinitions.StageTypeDefinition>()
    {
      {
        "WorkItemArea",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_WorkItemArea",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(2, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_WorkItemArea1", "AnalyticsStage.prc_MergeWorkItemAreas")
          },
          Metadata = new StageFieldMetaData[7]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AreaGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AreaId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AreaName", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AreaPath", SqlDbType.NVarChar, 4000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentAreaGuid", SqlDbType.UniqueIdentifier))
          }
        }
      },
      {
        "WorkItemDestroyed",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_WorkItemDestroyed",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(13, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_WorkItemDestroyed", "AnalyticsStage.prc_MergeWorkItemDestroyed")
          },
          Metadata = new StageFieldMetaData[1]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("WorkItemId", SqlDbType.Int))
          }
        }
      },
      {
        "Project",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_Project",
          ExportSprocName = "AnalyticsStage.prc_ExportProjects",
          StageVersions = new StageVersion[2]
          {
            new StageVersion(0, new int?(62), 0),
            new StageVersion(63, new int?(), 1)
          },
          TypeNameVersions = new StageSqlTableType[2]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_Project1", "AnalyticsStage.prc_MergeProjects"),
            new StageSqlTableType(1, "AnalyticsStage.typ_Project2", "AnalyticsStage.prc_MergeProjects")
          },
          Metadata = new StageFieldMetaData[5]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectName", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProcessId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("ProjectVisibility", SqlDbType.TinyInt))
          }
        }
      },
      {
        "WorkItemIteration",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_WorkItemIteration",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(2, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_WorkItemIteration1", "AnalyticsStage.prc_MergeWorkItemIterations")
          },
          Metadata = new StageFieldMetaData[9]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IterationGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IterationId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IterationName", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IterationPath", SqlDbType.NVarChar, 4000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StartDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("EndDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentIterationGuid", SqlDbType.UniqueIdentifier))
          }
        }
      },
      {
        "WorkItemRevision",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_WorkItemRevision",
          DbExtendedTableName = "AnalyticsStage.tbl_WorkItemRevisionExtended",
          DbCustomTableNamePattern = "AnalyticsStage.tbl_WorkItemRevisionCustom00",
          CleanupStreamSprocName = "AnalyticsStage.prc_CleanupWorkItemRevision",
          StageVersions = new StageVersion[5]
          {
            new StageVersion(0, new int?(25), 0),
            new StageVersion(26, new int?(27), 1),
            new StageVersion(28, new int?(43), 2),
            new StageVersion(44, new int?(), 3),
            new StageVersion(53, new int?(), 4)
          },
          TypeNameVersions = new StageSqlTableType[5]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_WorkItemRevision14", "AnalyticsStage.prc_MergeWorkItemRevisions"),
            new StageSqlTableType(1, "AnalyticsStage.typ_WorkItemRevision26", "AnalyticsStage.prc_MergeWorkItemRevisions"),
            new StageSqlTableType(2, "AnalyticsStage.typ_WorkItemRevision28", "AnalyticsStage.prc_MergeWorkItemRevisions"),
            new StageSqlTableType(3, "AnalyticsStage.typ_WorkItemRevision44", "AnalyticsStage.prc_MergeWorkItemRevisions"),
            new StageSqlTableType(4, "AnalyticsStage.typ_WorkItemRevision4", "AnalyticsStage.prc_MergeWorkItemRevisions4")
          },
          Metadata = new StageFieldMetaData[94]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AxRecordNumber", SqlDbType.Int, false, true, SortOrder.Ascending, 0)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_AreaId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_AreaGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_BoardColumn", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_BoardColumnDone", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_BoardLane", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_ChangedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_CreatedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_Id", SqlDbType.Int, false, true, SortOrder.Ascending, 1)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_TeamProject", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_IterationId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_IterationGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_Reason", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_Rev", SqlDbType.Int, false, true, SortOrder.Ascending, 2)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_State", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_Title", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_Watermark", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_WorkItemType", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Build_FoundIn", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Build_IntegrationBuild", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_Blocked", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_Committed", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_Escalate", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_FoundInEnvironment", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_HowFound", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_Probability", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_RequirementType", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_RequiresReview", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_RequiresTest", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_RootCause", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_SubjectMatterExpert1", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_SubjectMatterExpert2", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_SubjectMatterExpert3", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_TargetResolveDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_TaskType", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_CMMI_UserAcceptanceTest", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ActivatedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Activity", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_BacklogPriority", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_BusinessValue", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ClosedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Discipline", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Issue", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Priority", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Rating", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ResolvedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ResolvedReason", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Risk", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Severity", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_StackRank", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_TimeCriticality", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_Triage", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ValueArea", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_CompletedWork", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_DueDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_Effort", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_FinishDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_OriginalEstimate", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_RemainingWork", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_Size", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_StartDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_StoryPoints", SqlDbType.Float)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_Scheduling_TargetDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("System_AssignedToGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("System_AssignedTo", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("System_ChangedByGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("System_ChangedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("System_CreatedByGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("System_CreatedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("Microsoft_VSTS_Common_ActivatedByGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("Microsoft_VSTS_Common_ActivatedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("Microsoft_VSTS_Common_ClosedByGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("Microsoft_VSTS_Common_ClosedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("Microsoft_VSTS_Common_ResolvedByGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(3), new SqlMetaData("Microsoft_VSTS_Common_ResolvedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_Tags", SqlDbType.NVarChar, 1024L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_TCM_AutomatedTestId", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_TCM_AutomatedTestName", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_TCM_AutomatedTestStorage", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_TCM_AutomatedTestType", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Microsoft_VSTS_TCM_AutomationStatus", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("System_BoardExtensionId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("System_AuthorizedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("Microsoft_VSTS_Common_StateChangeDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("Tags", SqlDbType.Xml)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("System_CommentCount", SqlDbType.Int)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("System_AssignedTo", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("System_ChangedBy", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("System_CreatedBy", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ActivatedBy", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ClosedBy", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("Microsoft_VSTS_Common_ResolvedBy", SqlDbType.NVarChar, SqlMetaData.Max))
          }
        }
      },
      {
        "TeamSetting",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TeamSetting",
          StageVersions = new StageVersion[3]
          {
            new StageVersion(0, new int?(32), 0),
            new StageVersion(33, new int?(36), 1),
            new StageVersion(37, new int?(), 2)
          },
          TypeNameVersions = new StageSqlTableType[3]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TeamSetting", "AnalyticsStage.prc_MergeTeamSetting"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TeamSetting2", "AnalyticsStage.prc_MergeTeamSetting"),
            new StageSqlTableType(2, "AnalyticsStage.typ_TeamSetting3", "AnalyticsStage.prc_MergeTeamSetting")
          },
          Metadata = new StageFieldMetaData[9]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TeamGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TeamName", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Areas", SqlDbType.Xml)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BacklogCategories", SqlDbType.Xml)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Iterations", SqlDbType.Xml)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("BugsBehavior", SqlDbType.Int)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("TeamFieldReferenceName", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("TeamFieldValues", SqlDbType.Xml))
          }
        }
      },
      {
        "KanbanBoardColumn",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_KanbanBoardColumn",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(0, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_KanbanBoardColumn4", "AnalyticsStage.prc_MergeKanbanBoardColumns")
          },
          Metadata = new StageFieldMetaData[14]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TeamId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BoardId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BoardExtensionId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BoardCategoryReferenceName", SqlDbType.NVarChar, 70L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Id", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ColumnType", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Order", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ItemLimit", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsSplit", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RevisedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "KanbanBoardRow",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_KanbanBoardRow",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(0, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_KanbanBoardRow17", "AnalyticsStage.prc_MergeKanbanBoardRows")
          },
          Metadata = new StageFieldMetaData[12]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TeamId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BoardId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BoardExtensionId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BoardCategoryReferenceName", SqlDbType.NVarChar, 70L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Id", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Order", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDefault", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RevisedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "Process",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_Process",
          ExportSprocName = "AnalyticsStage.prc_ExportProcess",
          StageVersions = new StageVersion[2]
          {
            new StageVersion(0, new int?(32), 0),
            new StageVersion(33, new int?(), 1)
          },
          TypeNameVersions = new StageSqlTableType[2]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_Process3", "AnalyticsStage.prc_MergeProcess"),
            new StageSqlTableType(1, "AnalyticsStage.typ_Process4", "AnalyticsStage.prc_MergeProcess")
          },
          Metadata = new StageFieldMetaData[14]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProcessId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Inherits", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReferenceName", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Description", SqlDbType.NVarChar, 1024L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Version", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProcessStatus", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsSystem", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Fields", SqlDbType.Xml)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TypeCategories", SqlDbType.Xml)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("States", SqlDbType.Xml)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TypeFields", SqlDbType.Xml)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("Backlogs", SqlDbType.Xml))
          }
        }
      },
      {
        "Tag",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_Tag",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(0, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_Tag", "AnalyticsStage.prc_MergeTags")
          },
          Metadata = new StageFieldMetaData[6]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TagId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Scope", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Status", SqlDbType.NVarChar, 20L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsGlobalScope", SqlDbType.Bit))
          }
        }
      },
      {
        "WorkItemLink",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_WorkItemLink",
          CleanupStreamSprocName = "AnalyticsStage.prc_CleanupWorkItemLink",
          StageVersions = new StageVersion[3]
          {
            new StageVersion(0, new int?(26), 0),
            new StageVersion(27, new int?(29), 1),
            new StageVersion(30, new int?(), 2)
          },
          TypeNameVersions = new StageSqlTableType[3]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_WorkItemLink11", "AnalyticsStage.prc_MergeWorkItemLinks"),
            new StageSqlTableType(1, "AnalyticsStage.typ_WorkItemLink27", "AnalyticsStage.prc_MergeWorkItemLinks"),
            new StageSqlTableType(2, "AnalyticsStage.typ_WorkItemLink30", "AnalyticsStage.prc_MergeWorkItemLinks")
          },
          Metadata = new StageFieldMetaData[17]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("SourceId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TargetId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LinkTypeId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedById", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DeletedById", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DeletedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Comment", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DeletedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("ChangedBy", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("ChangedById", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("ChangedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("IsActive", SqlDbType.Bit)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("ProviderVersion", SqlDbType.Int)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("SourceProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("TargetProjectGuid", SqlDbType.UniqueIdentifier))
          }
        }
      },
      {
        "WorkItemLinkType",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_WorkItemLinkType",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(4, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_WorkItemLinkType", "AnalyticsStage.prc_MergeWorkItemLinkTypes")
          },
          Metadata = new StageFieldMetaData[6]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LinkTypeId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReferenceName", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ForwardName", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReverseName", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsAcyclic", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDirectional", SqlDbType.Bit))
          }
        }
      },
      {
        "Collection",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_Collection",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(16, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_Collection", "AnalyticsStage.prc_MergeCollection")
          },
          Metadata = new StageFieldMetaData[3]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CollectionHostId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AccountHostId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TimeZone", SqlDbType.NVarChar, 50L))
          }
        }
      },
      {
        "TestRun",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestRun",
          CleanupStreamSprocName = "AnalyticsStage.prc_CleanupTestRun",
          StageVersions = new StageVersion[4]
          {
            new StageVersion(42, new int?(48), 0),
            new StageVersion(49, new int?(52), 1),
            new StageVersion(53, new int?(68), 2),
            new StageVersion(69, new int?(), 3)
          },
          TypeNameVersions = new StageSqlTableType[4]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestRun36", "AnalyticsStage.prc_MergeTestRuns"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TestRun49", "AnalyticsStage.prc_MergeTestRuns"),
            new StageSqlTableType(2, "AnalyticsStage.typ_TestRun53", "AnalyticsStage.prc_MergeTestRuns"),
            new StageSqlTableType(3, "AnalyticsStage.typ_TestRun3", "AnalyticsStage.prc_MergeTestRuns")
          },
          Metadata = new StageFieldMetaData[21]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestRunId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestRunTitle", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("DateCompleted", SqlDbType.DateTime)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("DateCompleted", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsAutomated", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("SourceWorkflow", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BuildDefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BuildId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryId", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReleaseDefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReleaseEnvironmentDefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReleaseId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReleaseEnvironmentId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Attempt", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("DateStarted", SqlDbType.DateTime)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("DateStarted", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TotalTests", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(2), new SqlMetaData("DataSourceId", SqlDbType.TinyInt)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("ChangedDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "TestResult",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestResult",
          CleanupStreamSprocName = "AnalyticsStage.prc_CleanupNoOp",
          StageVersions = new StageVersion[4]
          {
            new StageVersion(42, new int?(48), 0),
            new StageVersion(49, new int?(58), 1),
            new StageVersion(59, new int?(68), 2),
            new StageVersion(69, new int?(), 3)
          },
          TypeNameVersions = new StageSqlTableType[4]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestResult36", "AnalyticsStage.prc_MergeTestResults"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TestResult49", "AnalyticsStage.prc_MergeTestResults"),
            new StageSqlTableType(2, "AnalyticsStage.typ_TestResult2", "AnalyticsStage.prc_MergeTestResults"),
            new StageSqlTableType(3, "AnalyticsStage.typ_TestResult3", "AnalyticsStage.prc_MergeTestResults")
          },
          Metadata = new StageFieldMetaData[11]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestRunId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestResultId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestCaseReferenceId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("DateStarted", SqlDbType.DateTime)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("DateStarted", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("DateCompleted", SqlDbType.DateTime)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("DateCompleted", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Outcome", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(2), new SqlMetaData("DataSourceId", SqlDbType.TinyInt)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("IsFlaky", SqlDbType.Bit))
          }
        }
      },
      {
        "TestCaseReference",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestCaseReference",
          StageVersions = new StageVersion[3]
          {
            new StageVersion(30, new int?(32), 0),
            new StageVersion(33, new int?(68), 1),
            new StageVersion(69, new int?(), 2)
          },
          TypeNameVersions = new StageSqlTableType[3]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestCaseReference", "AnalyticsStage.prc_MergeTestCaseReferences"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TestCaseReference34", "AnalyticsStage.prc_MergeTestCaseReferences"),
            new StageSqlTableType(2, "AnalyticsStage.typ_TestCaseReference2", "AnalyticsStage.prc_MergeTestCaseReferences")
          },
          Metadata = new StageFieldMetaData[9]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestCaseReferenceId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AutomatedTestName", SqlDbType.NVarChar, 512L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AutomatedTestStorage", SqlDbType.NVarChar, 512L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestPointId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Priority", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Owner", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestCaseTitle", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(1, new int?(1), new SqlMetaData("DataSourceId", SqlDbType.Int))
          }
        }
      },
      {
        "TestPlan",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestPlan",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(51, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestPlan", "AnalyticsStage.prc_MergeTestPlans")
          },
          Metadata = new StageFieldMetaData[9]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestPlanId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("State", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RootSuiteId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BuildDefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BuildUri", SqlDbType.NVarChar, 64L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReleaseDefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReleaseEnvDefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DataSourceId", SqlDbType.Int))
          }
        }
      },
      {
        "TestSuite",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestSuite",
          StageVersions = new StageVersion[2]
          {
            new StageVersion(36, new int?(50), 0),
            new StageVersion(51, new int?(), 1)
          },
          TypeNameVersions = new StageSqlTableType[2]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestSuite", "AnalyticsStage.prc_MergeTestSuites"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TestSuite51", "AnalyticsStage.prc_MergeTestSuites")
          },
          Metadata = new StageFieldMetaData[12]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestSuiteId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentSuiteId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("SuiteType", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("SuiteType", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RequirementId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestPlanId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("RootSuiteId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DataSourceId", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("SuitePath", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("SequenceNumber", SqlDbType.Int))
          }
        }
      },
      {
        "TestConfiguration",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestConfiguration",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(36, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestConfiguration", "AnalyticsStage.prc_MergeTestConfigurations")
          },
          Metadata = new StageFieldMetaData[5]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestConfigurationId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("State", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DataSourceId", SqlDbType.Int))
          }
        }
      },
      {
        "TestPoint",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestPoint",
          StageVersions = new StageVersion[3]
          {
            new StageVersion(38, new int?(50), 0),
            new StageVersion(51, new int?(51), 1),
            new StageVersion(52, new int?(), 2)
          },
          TypeNameVersions = new StageSqlTableType[3]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestPoint", "AnalyticsStage.prc_MergeTestPoints"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TestPoint51", "AnalyticsStage.prc_MergeTestPoints"),
            new StageSqlTableType(2, "AnalyticsStage.typ_TestPoint2", "AnalyticsStage.prc_MergeTestPoints")
          },
          Metadata = new StageFieldMetaData[17]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestPointId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestPlanId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestSuiteId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestConfigurationId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestCaseId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(1), new SqlMetaData("TesterId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Revision", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("State", SqlDbType.NVarChar, 32L)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("State", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Enabled", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedDate", SqlDbType.DateTime)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DataSourceId", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestResultState", SqlDbType.TinyInt)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestResultOutcome", SqlDbType.TinyInt)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("LastResetToActive", SqlDbType.DateTime)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("Tester", SqlDbType.NVarChar, SqlMetaData.Max))
          }
        }
      },
      {
        "TestPointHistory",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TestPointHistory",
          StageVersions = new StageVersion[3]
          {
            new StageVersion(39, new int?(50), 0),
            new StageVersion(51, new int?(51), 1),
            new StageVersion(52, new int?(), 2)
          },
          TypeNameVersions = new StageSqlTableType[3]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TestPointHistory", "AnalyticsStage.prc_MergeTestPointHistories"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TestPointHistory51", "AnalyticsStage.prc_MergeTestPointHistories"),
            new StageSqlTableType(2, "AnalyticsStage.typ_TestPointHistory2", "AnalyticsStage.prc_MergeTestPointHistories")
          },
          Metadata = new StageFieldMetaData[17]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestPointId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestRunId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TestResultId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Revision", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("State", SqlDbType.NVarChar, 32L)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("State", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Enabled", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedDate", SqlDbType.DateTime)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DataSourceId", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestSuiteId", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestPlanId", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestConfigurationId", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestCaseId", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(1), new SqlMetaData("TesterId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TestResultOutcome", SqlDbType.TinyInt)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("Tester", SqlDbType.NVarChar, SqlMetaData.Max))
          }
        }
      },
      {
        "TaskDefinitionReference",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskDefinitionReference",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(40, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskDefinitionReference", "AnalyticsStage.prc_MergeTaskDefinitionReference")
          },
          Metadata = new StageFieldMetaData[6]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineType", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TaskDefinitionReferenceId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TaskDefinitionGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TaskDefinitionName", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TaskDefinitionVersion", SqlDbType.NVarChar, 64L))
          }
        }
      },
      {
        "TaskDefinitionTimelineRecord",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskDefinitionTimelineRecord",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(40, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskDefinitionTimelineRecord", "AnalyticsStage.prc_MergeTaskDefinitionTimelineRecord")
          },
          Metadata = new StageFieldMetaData[6]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineType", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TimelineRecordGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TaskDefinitionReferenceId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanFinishTime", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "TaskPlan",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskPlan",
          CleanupStreamSprocName = "AnalyticsStage.prc_CleanupTaskPlan",
          StageVersions = new StageVersion[2]
          {
            new StageVersion(40, new int?(40), 0),
            new StageVersion(41, new int?(), 1)
          },
          TypeNameVersions = new StageSqlTableType[2]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskPlan", "AnalyticsStage.prc_MergeTaskPlan"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TaskPlan41", "AnalyticsStage.prc_MergeTaskPlan")
          },
          Metadata = new StageFieldMetaData[25]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineType", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TimelineGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanVersion", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanType", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanGroup", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("ProcessType", SqlDbType.NVarChar, 64L)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("TemplateType", SqlDbType.NVarChar, 64L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangeId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ArtifactUri", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("State", SqlDbType.NVarChar, 64L)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("Result", SqlDbType.NVarChar, 64L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ResultCode", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefinitionName", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OwnerName", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RequestedByGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RequestedForGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StartTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("FinishTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("ProcessType", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("TemplateType", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("State", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("Result", SqlDbType.Int))
          }
        }
      },
      {
        "TaskTimelineRecord",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskTimelineRecord",
          StageVersions = new StageVersion[6]
          {
            new StageVersion(40, new int?(40), 0),
            new StageVersion(41, new int?(47), 1),
            new StageVersion(48, new int?(49), 2),
            new StageVersion(50, new int?(57), 3),
            new StageVersion(58, new int?(60), 4),
            new StageVersion(61, new int?(), 5)
          },
          TypeNameVersions = new StageSqlTableType[6]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskTimelineRecord", "AnalyticsStage.prc_MergeTaskTimelineRecord"),
            new StageSqlTableType(1, "AnalyticsStage.typ_TaskTimelineRecord41", "AnalyticsStage.prc_MergeTaskTimelineRecord"),
            new StageSqlTableType(2, "AnalyticsStage.typ_TaskTimelineRecord48", "AnalyticsStage.prc_MergeTaskTimelineRecord"),
            new StageSqlTableType(3, "AnalyticsStage.typ_TaskTimelineRecord50", "AnalyticsStage.prc_MergeTaskTimelineRecord"),
            new StageSqlTableType(4, "AnalyticsStage.typ_TaskTimelineRecord4", "AnalyticsStage.prc_MergeTaskTimelineRecord"),
            new StageSqlTableType(5, "AnalyticsStage.typ_TaskTimelineRecord5", "AnalyticsStage.prc_MergeTaskTimelineRecord")
          },
          Metadata = new StageFieldMetaData[35]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineType", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TimelineRecordGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentRecordGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TimelineId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Type", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RefName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangeId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Order", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("State", SqlDbType.NVarChar, 64L)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("Result", SqlDbType.NVarChar, 64L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ResultCode", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("WorkerName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LogPath", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LogLineCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StartTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("FinishTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanFinishTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("State", SqlDbType.Int)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("Result", SqlDbType.Int)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("TaskDefinitionReferenceId", SqlDbType.Int)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("Identifier", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("Attempt", SqlDbType.Int)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("IsJobFailureDueToTask", SqlDbType.Bit)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("StageIdentifier", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("StageName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("PhaseIdentifier", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("PhaseName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("JobIdentifier", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(3, new int?(), new SqlMetaData("JobName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("AgentQueueTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(4, new int?(), new SqlMetaData("AgentStartTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(5, new int?(), new SqlMetaData("PlanLastUpdated", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "TaskFailure",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskFailure",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(54, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskFailure", "AnalyticsStage.prc_MergeTaskFailure")
          },
          Metadata = new StageFieldMetaData[5]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TimelineId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TimelineRecordId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BucketId", SqlDbType.Int))
          }
        }
      },
      {
        "TaskAgentRequest",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskAgentRequest",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(64, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskAgentRequest", "AnalyticsStage.prc_MergeTaskAgentRequest")
          },
          Metadata = new StageFieldMetaData[15]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RequestId", SqlDbType.BigInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PoolId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("JobId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("JobName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("QueueTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AssignTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StartTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("FinishTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Result", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineType", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AgentId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastUpdated", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "TaskAgentPool",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskAgentPool",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(66, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskAgentPool", "AnalyticsStage.prc_MergeTaskAgentPool")
          },
          Metadata = new StageFieldMetaData[6]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PoolId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PoolName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PoolType", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsHosted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PoolOptions", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastModified", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "ParallelPipelineJobs",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_ParallelPipelineJobs",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(67, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_ParallelPipelineJobs", "AnalyticsStage.prc_MergeParallelPipelineJobs")
          },
          Metadata = new StageFieldMetaData[7]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HostId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParallelismTag", SqlDbType.NVarChar, 128L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsHosted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TotalCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TotalMinutes", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsPremium", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("FailedToReachAllProviders", SqlDbType.Bit))
          }
        }
      },
      {
        "TaskAgentPoolSize",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_TaskAgentPoolSize",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(68, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_TaskAgentPoolSize", "AnalyticsStage.prc_MergeTaskAgentPoolSize")
          },
          Metadata = new StageFieldMetaData[3]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PoolId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OnlineCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OfflineCount", SqlDbType.Int))
          }
        }
      },
      {
        "Build",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_Build",
          StageVersions = new StageVersion[3]
          {
            new StageVersion(44, new int?(45), 0),
            new StageVersion(46, new int?(60), 1),
            new StageVersion(61, new int?(), 2)
          },
          TypeNameVersions = new StageSqlTableType[3]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_Build", "AnalyticsStage.prc_MergeBuilds"),
            new StageSqlTableType(1, "AnalyticsStage.typ_Build46", "AnalyticsStage.prc_MergeBuilds"),
            new StageSqlTableType(2, "AnalyticsStage.typ_Build2", "AnalyticsStage.prc_MergeBuilds")
          },
          Metadata = new StageFieldMetaData[19]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BuildId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefinitionVersion", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BuildNumberRevision", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("QueueTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StartTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("FinishTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Status", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(0), new SqlMetaData("Reason", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Result", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryId", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryType", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DataSourceId", SqlDbType.TinyInt)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("Reason", SqlDbType.Int)),
            new StageFieldMetaData(2, new int?(), new SqlMetaData("ChangedOn", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "BuildDefinition",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_BuildDefinition",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(45, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_BuildDefinition", "AnalyticsStage.prc_MergeBuildDefinitions")
          },
          Metadata = new StageFieldMetaData[7]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefinitionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefinitionVersion", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefinitionName", SqlDbType.NVarChar, 260L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProcessType", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Deleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DataSourceId", SqlDbType.TinyInt))
          }
        }
      },
      {
        "AzDevGitCommit",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_AzDevGitCommit",
          StageVersions = new StageVersion[2]
          {
            new StageVersion(55, new int?(58), 0),
            new StageVersion(59, new int?(), 1)
          },
          TypeNameVersions = new StageSqlTableType[2]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_AzDevGitCommit", "AnalyticsStage.prc_MergeAzDevGitCommits"),
            new StageSqlTableType(1, "AnalyticsStage.typ_AzDevGitCommit1", "AnalyticsStage.prc_MergeAzDevGitCommits")
          },
          Metadata = new StageFieldMetaData[9]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("SourceRepoGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitId", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitAuthorEmail", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitAuthorName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitAuthoredDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitComment", SqlDbType.NVarChar, 200L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitPushId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitPushDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(1, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier))
          }
        }
      },
      {
        "AzDevGitRepository",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_AzDevGitRepository",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(62, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_AzDevGitRepository", "AnalyticsStage.prc_MergeAzDevGitRepository")
          },
          Metadata = new StageFieldMetaData[6]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepoGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepoName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepoUrl", SqlDbType.NVarChar, 600L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastUpdatedDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "GitHubRepository",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubRepository",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(57, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubRepository", "AnalyticsStage.prc_MergeGitHubRepository")
          },
          Metadata = new StageFieldMetaData[27]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DatabaseId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NameWithOwner", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Owner", SqlDbType.NVarChar, -1L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsPrivate", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefaultBranchRefName", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PrimaryLanguage", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Url", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("UpdatedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PushedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DiskUsage", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsLocked", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDisabled", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsArchived", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsFork", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ForkCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentNameWithOwner", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LicenseId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LicenseName", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CollaboratorCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AssignableUsersCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MentionableUsersCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StargazersCount", SqlDbType.Int))
          }
        }
      },
      {
        "GitHubPullRequest",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubPullRequest",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(57, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubPullRequest", "AnalyticsStage.prc_MergeGitHubPullRequests")
          },
          Metadata = new StageFieldMetaData[44]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DatabaseId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefNodeName", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefNodePrefix", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefNodeRepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefNodeTargetNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefNodeTargetNodeOid", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefNodeTargetNodeType", SqlDbType.NVarChar, 10L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefName", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefOid", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefNodeName", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefNodePrefix", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefNodeRepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefNodeTargetNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefNodeTargetNodeOid", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefNodeTargetNodeType", SqlDbType.NVarChar, 10L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefName", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefOid", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRepositoryNameWithOwner", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsCrossRepository", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("UpdatedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Number", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Title", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Author", SqlDbType.NVarChar, -1L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergeableState", SqlDbType.NVarChar, 50L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PotentialMergeCommitNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Merged", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergeCommitNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergedBy", SqlDbType.NVarChar, -1L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedFiles", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Additions", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Deletions", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Reviews", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsClosed", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ClosedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsLocked", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDraft", SqlDbType.Bit))
          }
        }
      },
      {
        "GitHubCommit",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubCommit",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(57, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubCommit", "AnalyticsStage.prc_MergeGitHubCommit")
          },
          Metadata = new StageFieldMetaData[16]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Sha", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AbbreviatedSha", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Author", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AuthoredDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Committer", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommittedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AuthoredByCommitter", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Headline", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Message", SqlDbType.NVarChar, 1000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PushedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedFiles", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Additions", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Deletions", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Parents", SqlDbType.NVarChar, SqlMetaData.Max))
          }
        }
      },
      {
        "GitHubRef",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubRef",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(57, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubRef", "AnalyticsStage.prc_MergeGitHubRef")
          },
          Metadata = new StageFieldMetaData[7]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Prefix", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TargetNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TargetNodeOid", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TargetNodeType", SqlDbType.NVarChar, 10L))
          }
        }
      },
      {
        "PipelineEnvironment",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_PipelineEnvironment",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(60, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_PipelineEnvironment", "AnalyticsStage.prc_MergePipelineEnvironment")
          },
          Metadata = new StageFieldMetaData[4]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("EnvironmentId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedOn", SqlDbType.DateTime))
          }
        }
      },
      {
        "AzDevGitPullRequest",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_AzDevGitPullRequest",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(59, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_AzDevGitPullRequest", "AnalyticsStage.prc_MergeAzDevGitPullRequests")
          },
          Metadata = new StageFieldMetaData[27]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PullRequestId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Status", SqlDbType.SmallInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Creator", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreationDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ClosedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Title", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Description", SqlDbType.NVarChar, 4000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("SourceBranchName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TargetBranchName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergeStatus", SqlDbType.SmallInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergeId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastMergeSourceCommit", SqlDbType.NVarChar, 42L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastMergeTargetCommit", SqlDbType.NVarChar, 42L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastMergeCommit", SqlDbType.NVarChar, 42L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CompleteWhenMergedAuthority", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("SupportsIterations", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CompletionOptions", SqlDbType.NVarChar, 4000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergeOptions", SqlDbType.NVarChar, 4000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergeFailureType", SqlDbType.SmallInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergeFailureMessage", SqlDbType.NVarChar, 4000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDraft", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CompletionQueueTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AutoCompleteAuthority", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CodeReviewId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("UpdatedDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "JobCommitMapping",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_JobCommitMapping",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(59, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_JobCommitMapping", "AnalyticsStage.prc_MergeJobCommitMapping")
          },
          Metadata = new StageFieldMetaData[17]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DeploymentJobRequestId", SqlDbType.BigInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PipelineRunId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PlanGuid", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StageName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("JobName", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StageIdentifier", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("JobIdentifier", SqlDbType.NVarChar, 256L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("JobAttempt", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitId", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("EnvironmentId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryId", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("SourceBranch", SqlDbType.NVarChar, 250L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("StartTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("FinishTime", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("JobResult", SqlDbType.TinyInt))
          }
        }
      },
      {
        "AzDevGitDiscussionComment",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_AzDevGitDiscussionComment",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(59, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_AzDevGitDiscussionComment", "AnalyticsStage.prc_MergeAzDevGitDiscussionComment")
          },
          Metadata = new StageFieldMetaData[10]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommentId", SqlDbType.SmallInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DiscussionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentCommentId", SqlDbType.SmallInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommentType", SqlDbType.SmallInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedBy", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Content", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDeleted", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LikeCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PublishedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastUpdatedDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "AzDevGitDiscussionThread",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_AzDevGitDiscussionThread",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(59, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_AzDevGitDiscussionThread", "AnalyticsStage.prc_MergeAzDevGitDiscussionThread")
          },
          Metadata = new StageFieldMetaData[10]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DiscussionId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PullRequestId", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Sha1Id", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ThreadStatus", SqlDbType.SmallInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PublishedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LastUpdatedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ExtendedProperties", SqlDbType.NVarChar, SqlMetaData.Max)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ArtifactUri", SqlDbType.NVarChar, 2083L))
          }
        }
      },
      {
        "GitHubAccount",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubAccount",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(64, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubAccount", "AnalyticsStage.prc_MergeGitHubAccount")
          },
          Metadata = new StageFieldMetaData[7]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Description", SqlDbType.NVarChar, 1000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Email", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Company", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Type", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreationDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "GitHubUser",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubUser",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(65, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubUser", "AnalyticsStage.prc_MergeGitHubUser")
          },
          Metadata = new StageFieldMetaData[7]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OrganizationId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("UserId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Login", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Company", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Email", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreationDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "GitHubTeam",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubTeam",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(65, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubTeam", "AnalyticsStage.prc_MergeGitHubTeam")
          },
          Metadata = new StageFieldMetaData[7]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OrganizationId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CombinedSlug", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TeamId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Description", SqlDbType.NVarChar, 1000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ParentId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreationDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "GitHubTeamUserMap",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubTeamUserMap",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(65, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubTeamUserMap", "AnalyticsStage.prc_MergeGitHubTeamUserMap")
          },
          Metadata = new StageFieldMetaData[5]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OrganizationId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("TeamId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("UserId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangeType", SqlDbType.TinyInt)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedAt", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "GitHubRepositoryGHI",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubRepositoryGHI",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(65, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubRepositoryGHI", "AnalyticsStage.prc_MergeGitHubRepositoryGHI")
          },
          Metadata = new StageFieldMetaData[9]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Description", SqlDbType.NVarChar, 1000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsPrivate", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("DefaultBranchRefName", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PrimaryLanguage", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("PushedAt", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsFork", SqlDbType.Bit))
          }
        }
      },
      {
        "GitHubCommitGHI",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubCommitGHI",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(65, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubCommitGHI", "AnalyticsStage.prc_MergeGitHubCommitGHI")
          },
          Metadata = new StageFieldMetaData[17]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OrganizationNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("NodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Sha", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AbbreviatedSha", SqlDbType.NVarChar, 40L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ChangedFiles", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Additions", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Deletions", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Author", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AuthoredDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Committer", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommittedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AuthoredByCommitter", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Headline", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Message", SqlDbType.NVarChar, 1000L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsMergeCommit", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("InPullRequest", SqlDbType.Bit))
          }
        }
      },
      {
        "GitHubBranch",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitHubBranch",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(66, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubBranch", "AnalyticsStage.prc_MergeGitHubBranch")
          },
          Metadata = new StageFieldMetaData[3]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Id", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Name", SqlDbType.NVarChar, 400L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedDate", SqlDbType.DateTimeOffset))
          }
        }
      },
      {
        "GitHubPullRequestGHI",
        new StageTypeDefinitions.StageTypeDefinition()
        {
          DbTableName = "AnalyticsStage.tbl_GitPullRequestGHI",
          StageVersions = new StageVersion[1]
          {
            new StageVersion(66, new int?(), 0)
          },
          TypeNameVersions = new StageSqlTableType[1]
          {
            new StageSqlTableType(0, "AnalyticsStage.typ_GitHubPullRequestGHI", "AnalyticsStage.prc_MergeGitHubPullRequestGHI")
          },
          Metadata = new StageFieldMetaData[21]
          {
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Id", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("HeadRefId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("BaseRefId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("RepositoryNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("OrganizationNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AuthorNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergedByNodeId", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Number", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReviewersRequestedCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ReviewersVotedCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("AssigneesCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("IsDraft", SqlDbType.Bit)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Comments", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CommitCount", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LinesAdded", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("LinesDeleted", SqlDbType.Int)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("Title", SqlDbType.NVarChar, (long) byte.MaxValue)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("State", SqlDbType.NVarChar, 50L)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("CreatedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("MergedDate", SqlDbType.DateTimeOffset)),
            new StageFieldMetaData(0, new int?(), new SqlMetaData("ClosedDate", SqlDbType.DateTimeOffset))
          }
        }
      }
    };
    public static readonly StageTypeDefinitions.StageTypeDefinition ExtendedFieldTableDefinition = new StageTypeDefinitions.StageTypeDefinition()
    {
      DbTableName = (string) null,
      StageVersions = new StageVersion[2]
      {
        new StageVersion(11, new int?(52), 1),
        new StageVersion(53, new int?(), 2)
      },
      TypeNameVersions = new StageSqlTableType[2]
      {
        new StageSqlTableType(1, "AnalyticsStage.typ_ExtendedField11", (string) null),
        new StageSqlTableType(2, "AnalyticsStage.typ_ExtendedField2", (string) null)
      },
      Metadata = new StageFieldMetaData[8]
      {
        new StageFieldMetaData(1, new int?(), new SqlMetaData("AxRecordNumber", SqlDbType.Int, false, true, SortOrder.Ascending, 1)),
        new StageFieldMetaData(1, new int?(), new SqlMetaData("FieldName", SqlDbType.NVarChar, 256L, false, true, SortOrder.Ascending, 0)),
        new StageFieldMetaData(0, new int?(), new SqlMetaData("ValueInt", SqlDbType.BigInt)),
        new StageFieldMetaData(0, new int?(), new SqlMetaData("ValueString", SqlDbType.NVarChar, 256L)),
        new StageFieldMetaData(0, new int?(), new SqlMetaData("ValueFloat", SqlDbType.Float)),
        new StageFieldMetaData(0, new int?(), new SqlMetaData("ValueGuid", SqlDbType.UniqueIdentifier)),
        new StageFieldMetaData(0, new int?(), new SqlMetaData("ValueDatetime", SqlDbType.DateTimeOffset)),
        new StageFieldMetaData(2, new int?(), new SqlMetaData("ValueObject", SqlDbType.NVarChar, SqlMetaData.Max))
      }
    };

    [Conditional("DEBUG")]
    private static void AssertDefinitions()
    {
      foreach (KeyValuePair<string, StageTypeDefinitions.StageTypeDefinition> definition in (IEnumerable<KeyValuePair<string, StageTypeDefinitions.StageTypeDefinition>>) StageTypeDefinitions.Definitions)
      {
        StageFieldMetaData[] metadata = definition.Value.Metadata;
      }
    }

    public static StageTypeDefinitions.StageTypeDefinition GetStageTypeDefinition(string table)
    {
      StageTypeDefinitions.StageTypeDefinition stageTypeDefinition = (StageTypeDefinitions.StageTypeDefinition) null;
      if (!StageTypeDefinitions.Definitions.TryGetValue(table, out stageTypeDefinition))
        throw new ArgumentOutOfRangeException(nameof (table));
      return stageTypeDefinition;
    }

    internal static class ExtendedFieldColumnNames
    {
      public const string RecordNumber = "AxRecordNumber";
      public const string FieldName = "FieldName";
      public const string ValueInt = "ValueInt";
      public const string ValueString = "ValueString";
      public const string ValueFloat = "ValueFloat";
      public const string ValueGuid = "ValueGuid";
      public const string ValueDatetime = "ValueDatetime";
      public const string ValueObject = "ValueObject";
    }

    public class StageTypeDefinition
    {
      public string DbTableName { get; set; }

      public string DbExtendedTableName { get; set; }

      public string DbCustomTableNamePattern { get; set; }

      public string ExportSprocName { get; set; }

      public string CleanupStreamSprocName { get; set; }

      public StageVersion[] StageVersions { get; set; }

      public StageSqlTableType[] TypeNameVersions { get; set; }

      public StageFieldMetaData[] Metadata { get; set; }
    }
  }
}
