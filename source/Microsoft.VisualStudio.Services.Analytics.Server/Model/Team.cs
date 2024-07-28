// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Team
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_TEAMS")]
  [ModelTableMapping("Model.Team")]
  public class Team : IPartitionScoped, IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public Guid TeamSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId, ProjectSK")]
    public Project Project { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEAM_ID", false)]
    public Guid TeamId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEAM_NAME", false)]
    public string TeamName { get; set; }

    [DataMember(IsRequired = false)]
    [ModelTableMapping("Model.TeamArea")]
    public ICollection<Area> Areas { get; set; }

    [DataMember(IsRequired = false)]
    [ModelTableMapping("Model.TeamIteration")]
    public ICollection<Iteration> Iterations { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [DatabaseHide(0, 41)]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }
  }
}
