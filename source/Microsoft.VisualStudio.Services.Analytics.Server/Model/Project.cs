// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Project
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_PROJECTS")]
  [Table("AnalyticsModel.vw_Project")]
  [ModelTableMapping("Model.Project")]
  public class Project : IPartitionScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public Guid ProjectSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT_ID", false)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT_NAME", false)]
    public string ProjectName { get; set; }

    [IgnoreDataMember]
    public bool IsDeleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ModelTableMapping("Model.Area")]
    public ICollection<Area> Areas { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ModelTableMapping("Model.Iteration")]
    public ICollection<Iteration> Iterations { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ModelTableMapping("Model.Team")]
    public ICollection<Team> Teams { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [DatabaseHide(0, 41)]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT_VISIBILITY", false)]
    [DatabaseHide(0, 54)]
    [ODataHideByServiceVersion(0, 54)]
    public Microsoft.VisualStudio.Services.Analytics.Model.ProjectVisibility? ProjectVisibility { get; set; }

    [IgnoreDataMember]
    public Guid? ProcessId { get; set; }

    [IgnoreDataMember]
    public string ProcessName { get; set; }

    [IgnoreDataMember]
    public string ProcessReferenceName { get; set; }

    [IgnoreDataMember]
    public string ProcessDescription { get; set; }

    [IgnoreDataMember]
    public string ProcessVersion { get; set; }

    [IgnoreDataMember]
    public string ProcessStatus { get; set; }

    [IgnoreDataMember]
    public bool? IsSystemProcess { get; set; }

    [IgnoreDataMember]
    public string ProcessLevel0 { get; set; }

    [IgnoreDataMember]
    public string ProcessLevel1 { get; set; }

    [IgnoreDataMember]
    public string ProcessLevel2 { get; set; }

    [IgnoreDataMember]
    public string ProcessLevel3 { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }
  }
}
