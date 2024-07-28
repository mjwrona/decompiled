// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Diagnostics
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Common
{
  internal class Diagnostics
  {
    internal static readonly string ELeadCategory = "ELead";

    internal static void ReportException(Exception exception) => Diagnostics.ReportException(Diagnostics.ELeadCategory, exception, (string[]) null);

    internal static void ReportException(string eventCategory, Exception exception) => Diagnostics.ReportException(eventCategory, exception, (string[]) null);

    internal static void ReportException(
      string eventCategory,
      Exception exception,
      string[] additionalInfo)
    {
      if (!WatsonReport.VerifyConfiguration())
        return;
      WatsonReport report = WatsonReport.CreateReport(WatsonReport.WatsonReportType.ServerDefault, eventCategory, exception);
      report.ApplicationName = "TFS";
      if (additionalInfo != null && additionalInfo.Length != 0)
      {
        string tempFileName = FileSpec.GetTempFileName();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string str in additionalInfo)
          stringBuilder.AppendLine(str);
        using (StreamWriter streamWriter = new StreamWriter(tempFileName, false, Encoding.Unicode))
          streamWriter.Write(stringBuilder.ToString());
        report.ReportDataFiles.Add(tempFileName);
      }
      StringBuilder stringBuilder1 = new StringBuilder();
      if (HttpContext.Current != null)
      {
        HttpRequest request = HttpContext.Current.Request;
        stringBuilder1.AppendLine("Request Details");
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Url={0} [method = {1}]", (object) request.Url.ToString(), (object) request.RequestType));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User Agent={0}", (object) request.UserAgent));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Headers={0}", (object) request.Headers));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Path={0}", (object) request.Path));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Local Request={0}", (object) request.IsLocal));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User Host Address={0}", (object) request.UserHostAddress));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User={0} [auth = {1}]", (object) request.LogonUserIdentity.Name, (object) request.LogonUserIdentity.AuthenticationType));
      }
      report.ApplicationInformation = stringBuilder1.ToString();
      report.FileReport();
    }
  }
}
