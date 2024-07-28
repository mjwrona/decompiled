// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.GitHubPullRequest
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_GITHUBPULLREQUEST")]
  [ODataHide(0, 3)]
  [Table("AnalyticsModel.vw_GitHubPullRequest")]
  [ModelTableMapping("Model.GitHubPullRequest")]
  [DatabaseHide(0, 52)]
  public class GitHubPullRequest : IPartitionScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int PullRequestSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PULLREQUEST_ID", false)]
    public string PullRequestId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TITLE", false)]
    public string Title { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NUMBER", false)]
    public int? Number { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? AccountSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? RepositorySK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,RepositorySK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REPOSITORY", false)]
    public GitHubRepository Repository { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? SourceBranchSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,SourceBranchSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SOURCE_BRANCH", false)]
    public GitHubBranch SourceBranch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? TargetBranchSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,TargetBranchSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TARGET_BRANCH", false)]
    public GitHubBranch TargetBranch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STATE", false)]
    public string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? CreatedDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CREATED_DATE", false)]
    public DateTimeOffset? CreatedDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,CreatedDateSK")]
    public CalendarDate CreatedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? MergedDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_MERGED_DATE", false)]
    public DateTimeOffset? MergedDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,MergedDateSK")]
    public CalendarDate MergedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ClosedDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CLOSED_DATE", false)]
    public DateTimeOffset? ClosedDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ClosedDateSK")]
    public CalendarDate ClosedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ISDRAFT", false)]
    public bool? IsDraft { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMENTS_COUNT", false)]
    public int? CommentsCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REVIEWERS_REQUEST_COUNT", false)]
    public int? ReviewersRequestedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REVIEWERS_VOTE_COUNT", false)]
    public int? ReviewersVotedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ASSIGNEES_COUNT", false)]
    public int? AssigneesCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINES_COVERED", false)]
    public int? LinesCovered { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINES_ADDED", false)]
    public int? LinesAdded { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINES_DELETED", false)]
    public int? LinesDeleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMIT_COUNT", false)]
    public int? CommitCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? AuthorSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,AuthorSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AUTHOR", false)]
    public GitHubUser Author { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? MergedBySK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,MergedBySK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_MERGEDBY", false)]
    public GitHubUser MergedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TIMETOOPEN_INSECONDS", false)]
    public int? TimeToOpenInSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TIMETOMERGE_INSECONDS", false)]
    public int? TimeToMergeInSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TIMETOCLOSE_INSECONDS", false)]
    public int? TimeToCloseInSeconds { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
