// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.WorkItemLink
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_WORK_ITEM_LINKS")]
  [DataContract]
  [Table("AnalyticsModel.vw_WorkItemLink")]
  [ModelTableMapping("Model.WorkItemLinkHistory")]
  public class WorkItemLink : 
    IPartitionScoped,
    IContinuation<int>,
    IIncremental,
    IProjectNavigate,
    IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int WorkItemLinkSK { get; set; }

    [DataMember(IsRequired = true)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SOURCE_WORK_ITEM_ID", false)]
    public int SourceWorkItemId { get; set; }

    [DataMember(IsRequired = true)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TARGET_WORK_ITEM_ID", false)]
    public int TargetWorkItemId { get; set; }

    [ForeignKey("PartitionId, SourceWorkItemId")]
    [InverseProperty("Links")]
    [DataMember(IsRequired = false)]
    public WorkItem SourceWorkItem { get; set; }

    [ForeignKey("PartitionId, TargetWorkItemId")]
    [DataMember(IsRequired = false)]
    [NavigationRequiresPermissionCheck]
    public WorkItem TargetWorkItem { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CREATED_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public DateTimeOffset CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DELETED_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public DateTimeOffset? DeletedDate { get; set; }

    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMENT", false)]
    public string Comment { get; set; }

    [DataMember(IsRequired = true)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINK_TYPE_ID", false)]
    public int LinkTypeId { get; set; }

    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINK_TYPE_REFERENCE_NAME", false)]
    public string LinkTypeReferenceName { get; set; }

    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINK_TYPE_NAME", false)]
    public string LinkTypeName { get; set; }

    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINK_TYPE_IS_ACYCLIC", false)]
    public bool? LinkTypeIsAcyclic { get; set; }

    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LINK_TYPE_IS_DIRECTIONAL", false)]
    public bool? LinkTypeIsDirectional { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId, ProjectSK")]
    public Project Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }
  }
}
