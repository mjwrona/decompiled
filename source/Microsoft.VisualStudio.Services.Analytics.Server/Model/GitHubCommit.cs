// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.GitHubCommit
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
  [ODataHide(0, 3)]
  [Table("AnalyticsModel.vw_GitHubCommit")]
  [ModelTableMapping("Model.GitHubCommit")]
  [DatabaseHide(0, 51)]
  [LocalizedDisplayName("ENTITY_SET_NAME_GITHUB_COMMITS", false)]
  public class GitHubCommit : IPartitionScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public long CommitSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? CommitDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMIT_DATE", false)]
    public DateTimeOffset? CommitDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,CommitDateSK")]
    public CalendarDate CommitOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_SHA", false)]
    public string Sha { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? RepositorySK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,RepositorySK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_REPOSITORY", false)]
    public GitHubRepository Repository { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_ISMERGECOMMIT", false)]
    public bool? IsMergeCommit { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_INPULLREQUEST", false)]
    public bool? InPullRequest { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_LINES_ADDED", false)]
    public int? LinesAdded { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_LINES_DELETED", false)]
    public int? LinesDeleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_LINES_ADDED_NEW", false)]
    public int? LinesAddedNew { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_LINES_REFACTORED", false)]
    public int? LinesRefactored { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_LINES_SELFCHURNED", false)]
    public int? LinesSelfChurned { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_LINES_CHURNEDBYOTHERS", false)]
    public int? LinesChurnedByOthers { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? AccountSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,AccountSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_ACCOUNT", false)]
    public GitHubAccount Account { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? AuthorSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,AuthorSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_AUTHOR", false)]
    public GitHubUser Author { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? CommitterSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,CommitterSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMITS_COMMITER", false)]
    public GitHubUser Committer { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
