// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.CheckConfigurationRefExtension
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  public static class CheckConfigurationRefExtension
  {
    public static Uri AddParmetersToUrl(Uri uri, IDictionary<string, string> parameters)
    {
      if (parameters == null || !parameters.Any<KeyValuePair<string, string>>())
        return uri;
      UriBuilder uriBuilder = new UriBuilder(uri);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) parameters)
        queryString.Add(parameter.Key, parameter.Value);
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri;
    }

    public static string GetCheckConfigurationUrl(
      this CheckConfigurationRef checkConfigurationRef,
      IVssRequestContext requestContext,
      Guid projectId,
      int id,
      IDictionary<string, string> parameters = null)
    {
      if (requestContext == null)
        return (string) null;
      Guid empty = Guid.Empty;
      object routeValues = (object) new{ id = id };
      Guid configurationsLocationId = Microsoft.Azure.Pipelines.Checks.WebApi.Constants.ChecksConfigurationsLocationId;
      return CheckConfigurationRefExtension.AddParmetersToUrl(requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "PipelinesChecks", configurationsLocationId, projectId, routeValues), parameters).AbsoluteUri;
    }
  }
}
