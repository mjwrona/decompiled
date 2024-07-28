// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.GitHubContributorActivity
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
  [Table("AnalyticsModel.vw_GitHubContributorActivity")]
  [ModelTableMapping("Model.GitHubContributorActivity")]
  [DatabaseHide(0, 53)]
  [LocalizedDisplayName("ENTITY_SET_NAME_GITHUB_CONTRIBUTOR_ACTIVITIES", false)]
  public class GitHubContributorActivity : IPartitionScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public long ContributorActivitySK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ActivityDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_DATE", false)]
    public DateTimeOffset? ActivityDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ActivityDateSK")]
    public CalendarDate ActivityOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_TYPE", false)]
    public GitHubContributorActivityType ActivityType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_TYPE", false)]
    public GitHubContributorActivityArtifactType ActivityArtifactType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_IDENTIFIER", false)]
    public string ActivityArtifactIdentifier { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? RepositorySK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,RepositorySK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_REPOSITORY", false)]
    public GitHubRepository Repository { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? AccountSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,AccountSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACCOUNT", false)]
    public GitHubAccount Account { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ActivityBySK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ActivityBySK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACTIVITY_BY", false)]
    public GitHubUser ActivityBy { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
