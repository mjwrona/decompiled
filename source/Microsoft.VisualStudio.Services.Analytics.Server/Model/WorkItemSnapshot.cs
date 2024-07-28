// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Annotations;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DebuggerDisplay("{DateSK} : {WorkItemId} : {Revision} : {Title}")]
  [Table("AnalyticsModel.vw_WorkItemSnapshot")]
  [ModelTableMapping("Model.WorkItem")]
  [ModelTableMapping("Model.WorkItemHistory")]
  [ModelTableMapping("Model.Date")]
  public class WorkItemSnapshot : 
    WorkItemCommon,
    IHistoricalWorkItem,
    ISnapshot,
    ICurrentProjectNavigate,
    ICurrentProjectScoped
  {
    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public override int WorkItemId { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int DateSK { get; set; }

    [DatabaseHide(0, 19)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DATE_VALUE", false)]
    [SuppressDateConsistencyCheck("Date navigation property exists.")]
    public virtual DateTimeOffset DateValue { get; set; }

    [ForeignKey("PartitionId, DateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate Date { get; set; }

    [DatabaseHide(0, 19)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used mainly for filtering.")]
    public Period IsLastDayOfPeriod { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.RevisedDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REVISED_DATE", false)]
    public virtual DateTimeOffset? RevisedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? RevisedDateSK { get; set; }

    [ForeignKey("PartitionId,RevisedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate RevisedOn { get; set; }

    [DataMember(IsRequired = false)]
    [QueryShapeAllowAnyAllAnnotation]
    [ModelTableMapping("Model.TeamToTeamField")]
    public ICollection<Team> Teams { get; set; }

    [IgnoreDataMember]
    [SuppressDataMemberCheck("CurrentProjectSK is only used for filter and should not be exposed")]
    public Guid? CurrentProjectSK { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, CurrentProjectSK")]
    public Project CurrentProject { get; set; }

    [ODataHide(0, 1)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ModelTableMapping("Model.WorkItemProcess")]
    public ICollection<Process> Processes { get; set; }
  }
}
