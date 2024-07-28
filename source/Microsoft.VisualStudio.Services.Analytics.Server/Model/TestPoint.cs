// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestPoint
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_TEST_POINTS")]
  [ODataHide(0, 2)]
  [DatabaseHide(0, 44)]
  [Table("AnalyticsModel.vw_TestPoint")]
  [ModelTableMapping("Model.TestPoint")]
  public class TestPoint : TestPointCommon
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_STATE", false)]
    public TestResultState? LastResultState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_OUTCOME", false)]
    public TestOutcome? LastResultOutcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_CHANGED_DATE", false)]
    public DateTimeOffset ChangedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ChangedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ChangedDateSK")]
    public CalendarDate ChangedOn { get; set; }
  }
}
