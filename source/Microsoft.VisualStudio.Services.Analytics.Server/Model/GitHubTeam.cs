// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.GitHubTeam
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
  [Table("AnalyticsModel.vw_GitHubTeam")]
  [DisplayNameAnnotation("ENTITY_SET_NAME_GITHUB_TEAMS")]
  [ModelTableMapping("Model.GitHubTeam")]
  [ODataHide(0, 3)]
  [DatabaseHide(0, 51)]
  public class GitHubTeam : IPartitionScoped
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEAM_GHI_CREATIONDATE", false)]
    public DateTime? CreationDate;

    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int TeamSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEAM_ID", false)]
    public string TeamId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEAM_NAME", false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEAM_GHI_DESCRIPTION", false)]
    public string Description { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
