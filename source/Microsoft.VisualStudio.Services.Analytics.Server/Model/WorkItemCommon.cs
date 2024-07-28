// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.WorkItemCommon
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Annotations;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public abstract class WorkItemCommon : WorkItemPrimitive
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public override Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public virtual int WorkItemRevisionSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId, ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    [Required]
    public Project Project { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId, AreaSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA", false)]
    [QueryShapeAllowFlattenAnnotationAttribute]
    public Area Area { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual Guid? AreaSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId, IterationSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION", false)]
    [QueryShapeAllowFlattenAnnotationAttribute]
    public Iteration Iteration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual Guid? IterationSK { get; set; }

    [DatabaseHide(0, 33)]
    [IgnoreDataMember]
    public virtual int? TeamFieldSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, AssignedToUserSK")]
    [ReferenceName("System.AssignedTo")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ASSIGNED_TO", false)]
    public User AssignedTo { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? AssignedToUserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, ChangedByUserSK")]
    [ReferenceName("System.ChangedBy")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CHANGED_BY", false)]
    public User ChangedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ChangedByUserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, CreatedByUserSK")]
    [ReferenceName("System.CreatedBy")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CREATED_BY", false)]
    public User CreatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? CreatedByUserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, ActivatedByUserSK")]
    [ReferenceName("Microsoft.VSTS.Common.ActivatedBy")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVATED_BY", false)]
    public User ActivatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ActivatedByUserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, ClosedByUserSK")]
    [ReferenceName("Microsoft.VSTS.Common.ClosedBy")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CLOSED_BY", false)]
    public User ClosedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ClosedByUserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, ResolvedByUserSK")]
    [ReferenceName("Microsoft.VSTS.Common.ResolvedBy")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESOLVED_BY", false)]
    public User ResolvedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ResolvedByUserSK { get; set; }

    [DataMember(IsRequired = false)]
    [QueryShapeAllowAnyAllAnnotation]
    [ModelTableMapping("Model.WorkItemTag")]
    public ICollection<Tag> Tags { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ActivatedDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ChangedDateSK { get; set; }

    [ForeignKey("PartitionId,ChangedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate ChangedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ClosedDateSK { get; set; }

    [ForeignKey("PartitionId,ClosedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate ClosedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? CreatedDateSK { get; set; }

    [ForeignKey("PartitionId,CreatedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate CreatedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ResolvedDateSK { get; set; }

    [ForeignKey("PartitionId,ResolvedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate ResolvedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? StateChangeDateSK { get; set; }

    [ForeignKey("PartitionId,StateChangeDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate StateChangeOn { get; set; }

    [DatabaseHide(0, 20)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual int? InProgressDateSK { get; set; }

    [DatabaseHide(0, 20)]
    [ForeignKey("PartitionId,InProgressDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual CalendarDate InProgressOn { get; set; }

    [DatabaseHide(0, 20)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual int? CompletedDateSK { get; set; }

    [DatabaseHide(0, 20)]
    [ForeignKey("PartitionId,CompletedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual CalendarDate CompletedOn { get; set; }
  }
}
