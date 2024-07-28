// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.CalendarDate
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [Table("AnalyticsModel.tbl_Date")]
  [DisplayNameAnnotation("ENTITY_SET_NAME_DATES")]
  [ModelTableMapping("Model.Date")]
  public class CalendarDate : IPartitionScoped
  {
    private static readonly GregorianCalendar calendar = new GregorianCalendar();

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DATE", false)]
    public DateTimeOffset Date { get; set; }

    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int DateSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DAY_NAME", false)]
    public string DayName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DAY_SHORT_NAME", false)]
    public string DayShortName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DAY_OF_WEEK", false)]
    public int DayOfWeek { get; set; }

    [DatabaseHide(0, 19)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DAY_OF_MONTH", false)]
    public int DayOfMonth { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DAY_OF_YEAR", false)]
    public int DayOfYear { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WEEK_STARTING_DATE", false)]
    public DateTimeOffset WeekStartingDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WEEK_ENDING_DATE", false)]
    public DateTimeOffset WeekEndingDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_MONTH", false)]
    public string Month { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_MONTH_NAME", false)]
    public string MonthName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_MONTH_SHORT_NAME", false)]
    public string MonthShortName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_MONTH_OF_YEAR", false)]
    public int MonthOfYear { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_YEAR_MONTH", false)]
    public int YearMonth { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_YEAR", false)]
    public int Year { get; set; }

    [DatabaseHide(0, 19)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used mainly for filtering.")]
    public Period IsLastDayOfPeriod { get; set; }
  }
}
