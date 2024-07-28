// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.GitHubAccount
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_GITHUB_ACCOUNTS")]
  [ODataHide(0, 3)]
  [Table("AnalyticsModel.vw_GitHubAccount")]
  [ModelTableMapping("Model.GitHubAccount")]
  [DatabaseHide(0, 51)]
  public class GitHubAccount : IPartitionScoped
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACCOUNT_CREATIONDATE", false)]
    public DateTime? CreationDate;

    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int AccountSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACCOUNT_ID", false)]
    public string AccountId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACCOUNT_TYPE", false)]
    public GitHubAccountType AccountType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACCOUNT_NAME", false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACCOUNT_DESCRIPTION", false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACCOUNT_EMAIL", false)]
    public string Email { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACCOUNT_COMPANY", false)]
    public string Company { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
