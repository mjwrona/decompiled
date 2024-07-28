// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.StringExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Globalization;
using System.Web.Security.AntiXss;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class StringExtensions
  {
    public static string HtmlEncode(this string value) => AntiXssEncoder.HtmlEncode(value, true);

    public static string Format(this string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);

    public static string FormatCurrent(this string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);

    public static string FormatInvariant(this string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);

    public static string FormatUI(this string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
  }
}
