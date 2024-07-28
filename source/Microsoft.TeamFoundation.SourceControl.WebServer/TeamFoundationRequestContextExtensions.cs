// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TeamFoundationRequestContextExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TeamFoundationRequestContextExtensions
  {
    internal static Sha1Id? GetCommitIdFromVersionDescriptor(
      this IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor)
    {
      if (versionDescriptor != null)
      {
        TfsGitCommit commitFromVersion = GitVersionParser.GetCommitFromVersion(requestContext, repository, versionDescriptor);
        if (commitFromVersion != null)
          return new Sha1Id?(commitFromVersion.ObjectId);
      }
      return new Sha1Id?();
    }

    public static DateTime? ParseFromDate(this IVssRequestContext requestContext, string inputValue) => requestContext.ParseDate(inputValue, "ErrorInvalidFromDateFormat");

    public static DateTime? ParseToDate(this IVssRequestContext requestContext, string inputValue) => requestContext.ParseDate(inputValue, "ErrorInvalidToDateFormat");

    private static DateTime? ParseDate(
      this IVssRequestContext requestContext,
      string inputValue,
      string invalidFormatMessage)
    {
      DateTime? date1 = new DateTime?();
      if (!string.IsNullOrWhiteSpace(inputValue))
      {
        DateTime date2;
        if (!requestContext.GetTimeZone().TryParseDate(inputValue, out date2))
        {
          string str = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HH{0}mm{0}ss", (object) CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator);
          throw new TeamFoundationServiceException(Resources.Format(invalidFormatMessage, (object) str));
        }
        date1 = new DateTime?(date2);
      }
      return date1;
    }

    internal static TimeZoneInfo GetTimeZone(this IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<IUserPreferencesService>().GetUserPreferences(requestContext).TimeZone ?? requestContext.GetCollectionTimeZone();
    }

    private static TimeZoneInfo GetCollectionTimeZone(this IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(requestContext) ?? TimeZoneInfo.Local;
    }

    internal static bool TryParseDate(
      this TimeZoneInfo timeZoneInfo,
      string dateString,
      out DateTime date)
    {
      return timeZoneInfo.TryParseDateImpl(dateString, out date, out TimeSpan _, false);
    }

    internal static bool TryParseDate(
      this TimeZoneInfo timeZoneInfo,
      string dateString,
      out DateTime date,
      out TimeSpan utcOffset)
    {
      return timeZoneInfo.TryParseDateImpl(dateString, out date, out utcOffset, true);
    }

    private static bool TryParseDateImpl(
      this TimeZoneInfo timeZoneInfo,
      string dateString,
      out DateTime date,
      out TimeSpan utcOffset,
      bool calcOffset)
    {
      utcOffset = TimeSpan.Zero;
      if (DateTime.TryParse(dateString, (IFormatProvider) null, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal, out date))
      {
        if (date.Kind == DateTimeKind.Utc)
        {
          if (calcOffset)
          {
            DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(dateString, (IFormatProvider) null, DateTimeStyles.AllowWhiteSpaces);
            utcOffset = dateTimeOffset.Offset;
          }
          return true;
        }
        if (calcOffset)
          utcOffset = timeZoneInfo.GetUtcOffset(date);
        date = TimeZoneInfo.ConvertTime(date, timeZoneInfo, TimeZoneInfo.Utc);
        return true;
      }
      date = DateTime.MinValue;
      return false;
    }
  }
}
