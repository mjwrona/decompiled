// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.UserAgentClassifier
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal static class UserAgentClassifier
  {
    private static readonly Regex _regex = new Regex("^PowerBI-VSTS(/Desktop)?/\\d+\\.\\d+\\.\\d+(-[0-9A-Za-z-+\\.]+)?/(?<client>(?<templateVersion>\\d+\\.\\d+\\.\\d+)|(?<legacy>VSTS\\.Views)|(?<views>(VSTS\\.)?AnalyticsViews))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static MashupFlavor GetPowerBIFlavor(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      string userAgent = requestContext.UserAgent;
      if (string.IsNullOrWhiteSpace(userAgent))
        return MashupFlavor.None;
      if (userAgent.StartsWith("Microsoft.Data.Mashup"))
      {
        string str = request.Headers?.Referrer?.ToString();
        return str != null && str.StartsWith("PowerBI") ? MashupFlavor.Feeds : MashupFlavor.Generic;
      }
      MatchCollection matchCollection = UserAgentClassifier._regex.Matches(userAgent);
      if (matchCollection == null || matchCollection.Count <= 0)
        return MashupFlavor.None;
      Match match = matchCollection[0];
      if (match.Groups["templateVersion"].Success || match.Groups["legacy"].Success)
        return MashupFlavor.Generic;
      return match.Groups["views"].Success ? MashupFlavor.Views : MashupFlavor.Contents;
    }
  }
}
