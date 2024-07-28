// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Branch
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_BRANCHES")]
  [ODataHide(0, 2)]
  [Table("AnalyticsModel.vw_Branch")]
  [DisableProjectFiltering]
  [ModelTableMapping("Model.Branch")]
  public class Branch : IPartitionScoped, IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    [VisibleCrossProject]
    public int BranchSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REPOSITORY_ID", false)]
    [VisibleCrossProject]
    public string RepositoryId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BRANCH_NAME", false)]
    [VisibleCrossProject]
    public string BranchName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REPOSITORY_VSTS_ID", false)]
    public Guid? RepositoryVstsId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [DatabaseHide(0, 41)]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REPOSITORY_URL", false)]
    [VisibleCrossProject]
    public string RepositoryUrl { get; set; }
  }
}
