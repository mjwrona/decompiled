// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestPointHistorySnapshot
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_TEST_POINT_HISTORY_SNAPSHOT")]
  [ODataHide(0, 2)]
  [DatabaseHide(0, 45)]
  [Table("AnalyticsModel.vw_TestPointHistorySnapshot")]
  [ModelTableMapping("Model.TestPointHistory")]
  public class TestPointHistorySnapshot : TestPointCommon
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_OUTCOME", false)]
    public TestOutcome? ResultOutcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Key]
    public int DateSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used mainly for filtering.")]
    public Period IsLastDayOfPeriod { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId,DateSK")]
    public CalendarDate Date { get; set; }
  }
}
