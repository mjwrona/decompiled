// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.WorkItemPrimitive
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DebuggerDisplay("{WorkItemId} : {Title}")]
  public abstract class WorkItemPrimitive : IProjectScoped, IPartitionScoped, IContinuation<int>
  {
    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom01 Custom01 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom02 Custom02 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom03 Custom03 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom04 Custom04 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom05 Custom05 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom06 Custom06 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom07 Custom07 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 31)]
    public WorkItemCustom08 Custom08 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 57)]
    public WorkItemCustom09 Custom09 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 57)]
    public WorkItemCustom10 Custom10 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 59)]
    public WorkItemCustom11 Custom11 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 59)]
    public WorkItemCustom12 Custom12 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom13 Custom13 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom14 Custom14 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom15 Custom15 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom16 Custom16 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom17 Custom17 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom18 Custom18 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom19 Custom19 { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, WorkItemRevisionNSK")]
    [DatabaseHide(0, 60)]
    public WorkItemCustom20 Custom20 { get; set; }

    [IgnoreDataMember]
    public virtual int PartitionId { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [ReferenceName("System.Id")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WORKITEM_ID", false, Force = true)]
    public virtual int WorkItemId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [ReferenceName("System.Rev")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REVISION", false, Force = true)]
    public virtual int? Revision { get; set; }

    [IgnoreDataMember]
    [DatabaseHide(0, 31)]
    [SuppressDataMemberCheck("WorkItemRevisionNSK is only used for join with custom fields")]
    public int? WorkItemRevisionNSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.Watermark")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WATERMARK", false)]
    public virtual int? Watermark { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.Title")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TITLE", false)]
    public virtual string Title { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.WorkItemType")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WORK_ITEM_TYPE", false)]
    public virtual string WorkItemType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.ChangedDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CHANGED_DATE", false)]
    public virtual DateTimeOffset? ChangedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.CreatedDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CREATED_DATE", false)]
    public virtual DateTimeOffset? CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.State")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STATE", false)]
    public virtual string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.Reason")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REASON", false)]
    public virtual string Reason { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Build.FoundIn")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_FOUND_IN", false)]
    public virtual string FoundIn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Build.IntegrationBuild")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_INTEGRATION_BUILD", false)]
    public virtual string IntegrationBuild { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.ActivatedDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVATED_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public virtual DateTimeOffset? ActivatedDate { get; set; }

    [ReferenceName("Microsoft.VSTS.Common.Activity")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVITY", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual string Activity { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.BacklogPriority")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BACKLOG_PRIORITY", false)]
    public virtual double? BacklogPriority { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.BusinessValue")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BUSINESS_VALUE", false)]
    public virtual int? BusinessValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.ClosedDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CLOSED_DATE", false)]
    public virtual DateTimeOffset? ClosedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.Discipline")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DISCIPLINE", false)]
    public virtual string Discipline { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.Issue")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ISSUE", false)]
    public virtual string Issue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.Priority")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PRIORITY", false)]
    public virtual int? Priority { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.Rating")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RATING", false)]
    public virtual string Rating { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.ResolvedDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESOLVED_DATE", false)]
    public virtual DateTimeOffset? ResolvedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.ResolvedReason")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESOLVED_REASON", false)]
    public virtual string ResolvedReason { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.Risk")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RISK", false)]
    public virtual string Risk { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.Severity")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SEVERITY", false)]
    public virtual string Severity { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.StackRank")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STACK_RANK", false)]
    public virtual double? StackRank { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.TimeCriticality")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TIME_CRITICALITY", false)]
    public virtual double? TimeCriticality { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.Triage")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TRIAGE", false)]
    public virtual string Triage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.ValueArea")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_VALUE_AREA", false)]
    public virtual string ValueArea { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.CompletedWork")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMPLETED_WORK", false)]
    public virtual double? CompletedWork { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.DueDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DUE_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public virtual DateTimeOffset? DueDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.Effort")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_EFFORT", false)]
    public virtual double? Effort { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.FinishDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_FINISH_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public virtual DateTimeOffset? FinishDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.OriginalEstimate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ORIGINAL_ESTIMATE", false)]
    public virtual double? OriginalEstimate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.RemainingWork")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REMAINING_WORK", false)]
    public virtual double? RemainingWork { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.Size")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SIZE", false)]
    public virtual double? Size { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.StartDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_START_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public virtual DateTimeOffset? StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.StoryPoints")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STORY_POINTS", false)]
    public virtual double? StoryPoints { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Scheduling.TargetDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TARGET_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public virtual DateTimeOffset? TargetDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.Blocked")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BLOCKED", false)]
    public virtual string Blocked { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.Committed")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITTED", false)]
    public virtual string Committed { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.Escalate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ESCALATE", false)]
    public virtual string Escalate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.FoundInEnvironment")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_FOUND_IN_ENVIRONMENT", false)]
    public virtual string FoundInEnvironment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.HowFound")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_HOW_FOUND", false)]
    public virtual string HowFound { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.Probability")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROBABILITY", false)]
    public virtual int? Probability { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.RequirementType")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REQUIREMENT_TYPE", false)]
    public virtual string RequirementType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.RequiresReview")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REQUIRES_REVIEW", false)]
    public virtual string RequiresReview { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.RequiresTest")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REQUIRES_TEST", false)]
    public virtual string RequiresTest { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.RootCause")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ROOT_CAUSE", false)]
    public virtual string RootCause { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.SubjectMatterExpert1")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_1", false)]
    public virtual string SubjectMatterExpert1 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.SubjectMatterExpert2")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_2", false)]
    public virtual string SubjectMatterExpert2 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.SubjectMatterExpert3")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_3", false)]
    public virtual string SubjectMatterExpert3 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.TargetResolveDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TARGET_RESOLVE_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public virtual DateTimeOffset? TargetResolveDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.TaskType")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TASK_TYPE", false)]
    public virtual string TaskType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.CMMI.UserAcceptanceTest")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_ACCEPTANCE_TEST", false)]
    public virtual string UserAcceptanceTest { get; set; }

    [IgnoreDataMember]
    [ReferenceName("System.IsDeleted")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_DELETED", false)]
    public virtual bool IsDeleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PARENT_WORK_ITEM_ID", false)]
    public int? ParentWorkItemId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_TAGS", false)]
    [ReferenceName("System.Tags")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TagNames { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_STATE_CATEGORY", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StateCategory { get; set; }

    [DatabaseHide(0, 20)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IN_PROGRESS_DATE", false)]
    public virtual DateTimeOffset? InProgressDate { get; set; }

    [DatabaseHide(0, 20)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMPLETED_DATE", false)]
    public virtual DateTimeOffset? CompletedDate { get; set; }

    [DatabaseHide(0, 20)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LEAD_TIME_DAYS", false)]
    public virtual double? LeadTimeDays { get; set; }

    [DatabaseHide(0, 20)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CYCLE_TIME_DAYS", false)]
    public virtual double? CycleTimeDays { get; set; }

    [ReferenceName("Microsoft.VSTS.TCM.AutomatedTestId")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AUTOMATED_TEST_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestId { get; set; }

    [ReferenceName("Microsoft.VSTS.TCM.AutomatedTestName")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AUTOMATED_TEST_NAME", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestName { get; set; }

    [ReferenceName("Microsoft.VSTS.TCM.AutomatedTestStorage")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AUTOMATED_TEST_STORAGE", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestStorage { get; set; }

    [ReferenceName("Microsoft.VSTS.TCM.AutomatedTestType")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AUTOMATED_TEST_TYPE", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestType { get; set; }

    [ReferenceName("Microsoft.VSTS.TCM.AutomationStatus")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AUTOMATION_STATUS", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomationStatus { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("Microsoft.VSTS.Common.StateChangeDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STATE_CHANGE_DATE", false)]
    public virtual DateTimeOffset? StateChangeDate { get; set; }

    [SuppressDisplayName("This property is intended to be used in certain aggregation queries.")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual long Count { get; set; }

    [DatabaseHide(0, 36)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.CommentCount")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMENT_COUNT", false)]
    public virtual int? CommentCount { get; set; }
  }
}
