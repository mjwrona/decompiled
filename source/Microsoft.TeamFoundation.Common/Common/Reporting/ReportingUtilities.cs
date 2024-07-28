// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Reporting.ReportingUtilities
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Common.Reporting
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ReportingUtilities
  {
    public static Uri GetReportManagerItemUrl(Uri reportManagerUrl, string itemPath)
    {
      ArgumentUtility.CheckForNull<Uri>(reportManagerUrl, nameof (reportManagerUrl));
      ArgumentUtility.CheckForNull<string>(itemPath, nameof (itemPath));
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/Pages/Folder.aspx?ItemPath={1}", (object) reportManagerUrl, (object) UriUtility.UrlEncode(UriUtility.EnsureStartsWithPathSeparator(itemPath))));
    }

    public static Uri GetReportViewerUrl(
      Uri reportWebServiceUrl,
      string itemPath,
      bool showToolbar)
    {
      ArgumentUtility.CheckForNull<Uri>(reportWebServiceUrl, nameof (reportWebServiceUrl));
      ArgumentUtility.CheckForNull<string>(itemPath, nameof (itemPath));
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&{0}=false", (object) UriUtility.UrlEncode("rc:Toolbar"));
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?{1}{2}", (object) UriUtility.TrimEndingPathSeparator(ReportingUtilities.RemoveKnownWebServicePaths(reportWebServiceUrl).AbsoluteUri), (object) UriUtility.UrlEncode(UriUtility.EnsureStartsWithPathSeparator(itemPath)), showToolbar ? (object) string.Empty : (object) str));
    }

    public static Uri RemoveKnownWebServicePaths(Uri input)
    {
      ArgumentUtility.CheckForNull<Uri>(input, nameof (input));
      return TFCommonUtil.StripEnd(input, ReportingConstants.KnownWebServicePaths);
    }
  }
}
