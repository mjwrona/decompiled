// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DateTimeExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class DateTimeExtensions
  {
    public static readonly DateTime MinAllowedDateTime = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime MaxAllowedDateTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

    public static DateTime? ToUtcDateTime(this DateTime? dateTime) => dateTime.HasValue ? new DateTime?(dateTime.Value.ToUtcDateTime()) : dateTime;

    public static DateTime ToUtcDateTime(this DateTime dateTime) => DateTimeExtensions.ValidateDateTimeField(DateTimeExtensions.EnsureUtc(dateTime));

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should be a method")]
    public static DateTime GetMaxYesterdayDateTimeInUtc()
    {
      DateTime dateTime = DateTime.UtcNow.AddDays(-1.0);
      return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, DateTimeKind.Utc);
    }

    public static DateTime AddDaysSaturating(this DateTime dateTime, double days)
    {
      try
      {
        return DateTimeExtensions.ValidateDateTimeField(dateTime.AddDays(days));
      }
      catch (ArgumentOutOfRangeException ex)
      {
        return days > 0.0 ? DateTimeExtensions.MaxAllowedDateTime : DateTimeExtensions.MinAllowedDateTime;
      }
    }

    private static DateTime EnsureUtc(DateTime dateTime) => dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime.ToUniversalTime();

    private static DateTime ValidateDateTimeField(DateTime dateTime)
    {
      if (dateTime < DateTimeExtensions.MinAllowedDateTime)
        return DateTimeExtensions.MinAllowedDateTime;
      return dateTime > DateTimeExtensions.MaxAllowedDateTime ? DateTimeExtensions.MaxAllowedDateTime : dateTime;
    }
  }
}
