// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.GitHubUser
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [Table("AnalyticsModel.vw_GitHubUser")]
  [DisplayNameAnnotation("ENTITY_SET_NAME_GITHUB_USERS")]
  [ModelTableMapping("Model.GitHubUser")]
  [ODataHide(0, 3)]
  [DatabaseHide(0, 51)]
  public class GitHubUser : IPartitionScoped
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_GHI_CREATIONDATE", false)]
    public DateTime? CreationDate;

    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int UserSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_ID", false)]
    public string UserId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_GHI_LOGIN", false)]
    public string Login { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_NAME", false)]
    [EuiiAnnotation]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_EMAIL", false)]
    [EuiiAnnotation]
    public string Email { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_COMPANY", false)]
    [EuiiAnnotation]
    public string Company { get; set; }

    [DataMember(IsRequired = false)]
    [ModelTableMapping("Model.GitHubTeamUserMap")]
    public ICollection<GitHubTeam> Teams { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
