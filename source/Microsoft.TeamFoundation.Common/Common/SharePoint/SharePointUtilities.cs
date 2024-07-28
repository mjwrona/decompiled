// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.SharePoint.SharePointUtilities
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.SharePoint
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class SharePointUtilities
  {
    public static Uri GetWellKnownProcessGuidancePageUrl(
      ProcessGuidanceType type,
      Uri baseUrl,
      string guidanceFileName)
    {
      if (type == ProcessGuidanceType.WssDocumentLibrary && baseUrl != (Uri) null)
      {
        if (!string.IsNullOrEmpty(guidanceFileName))
          return UriUtility.Combine(baseUrl, guidanceFileName, false);
        baseUrl = UriUtility.Combine(baseUrl, "ProcessGuidance.html", false);
      }
      return baseUrl;
    }
  }
}
