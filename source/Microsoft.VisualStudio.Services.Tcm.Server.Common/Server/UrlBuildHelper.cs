// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.UrlBuildHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public static class UrlBuildHelper
  {
    public static Uri AddParmetersToUrl(Uri url, Dictionary<string, string> parameters)
    {
      if (parameters == null || !parameters.Any<KeyValuePair<string, string>>())
        return url;
      UriBuilder uriBuilder = new UriBuilder(url);
      NameValueCollection queryString = HttpUtility.ParseQueryString(url.Query);
      foreach (KeyValuePair<string, string> parameter in parameters)
        queryString.Add(parameter.Key, parameter.Value);
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri;
    }

    public static string GetResourceUrl(
      IVssRequestContext requestContext,
      Guid serviceIdentifier,
      string serviceArea,
      Guid locationId,
      object routeValues,
      Dictionary<string, string> parameters = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return UrlBuildHelper.AddParmetersToUrl(service.GetLocationData(requestContext, serviceIdentifier).GetResourceUri(requestContext, serviceArea, locationId, routeValues), parameters).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ServerResources.ResourceUrlCreationFailed, ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }
  }
}
