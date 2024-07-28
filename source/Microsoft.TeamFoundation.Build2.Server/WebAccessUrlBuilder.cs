// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.WebAccessUrlBuilder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class WebAccessUrlBuilder
  {
    private const string WorkItemArea = "_workitems";
    private const string TfsGitCommitArea = "_git";
    private const string TfsGitCommitPath = "commit";
    private const string TfsVersionControlCommitArea = "_versionControl";
    private const string TfsVersionControlCommitPath = "changeset";
    private const string TfsGit = "TfsGit";
    private const string TfsVersionControl = "TfsVersionControl";

    public static string GetWorkItemWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      int? id)
    {
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          "_a",
          (object) "edit"
        },
        {
          nameof (id),
          (object) id
        }
      };
      return new Uri(WebAccessUrlBuilder.GetBaseUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext)), WebAccessUrlBuilder.GetRelativeUrl(projectName, "_workitems") + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    public static string GetCommitWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryType,
      string repositoryId,
      string commitId)
    {
      if (repositoryType.Equals("TfsGit", StringComparison.OrdinalIgnoreCase))
        return WebAccessUrlBuilder.GetTfsGitCommitWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, repositoryId, commitId);
      return repositoryType.Equals("TfsVersionControl", StringComparison.OrdinalIgnoreCase) ? WebAccessUrlBuilder.GetTfsVersionControlCommitWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, commitId.Substring(1)) : string.Empty;
    }

    private static Uri GetCollectionUrl(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string uriString = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.PublicAccessMappingMoniker);
      if (uriString == null)
      {
        string absoluteUri = requestContext.RequestUri().AbsoluteUri;
        uriString = absoluteUri.Substring(0, absoluteUri.IndexOf(HttpRouteCollectionExtensions.DefaultRoutePrefix, StringComparison.Ordinal));
      }
      return new Uri(uriString);
    }

    private static string GetTfsGitCommitWebAccessUri(
      Uri collectionUri,
      string projectName,
      string repository,
      string commitId)
    {
      string relativeUri = WebAccessUrlBuilder.GetRelativeUrl(projectName, "_git") + "/" + repository + "/commit/" + commitId;
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), relativeUri).AbsoluteUri;
    }

    private static string GetTfsVersionControlCommitWebAccessUri(
      Uri collectionUri,
      string projectName,
      string changesetId)
    {
      string relativeUri = WebAccessUrlBuilder.GetRelativeUrl(projectName, "_versionControl") + "/changeset/" + changesetId;
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), relativeUri).AbsoluteUri;
    }

    private static Uri GetBaseUri(Uri uri)
    {
      string str = uri.ToString();
      return new Uri(str + (str.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? string.Empty : "/"));
    }

    private static string GetRelativeUrl(string projectName, string areaName) => projectName + "/" + areaName;

    private static string GetQueryParameter(Dictionary<string, object> parameterValues)
    {
      string queryParameter = string.Empty;
      foreach (string key in parameterValues.Keys)
      {
        string str;
        if (!string.IsNullOrEmpty(queryParameter))
          str = queryParameter + "&" + key + "=" + parameterValues[key]?.ToString();
        else
          str = "?" + key + "=" + parameterValues[key]?.ToString();
        queryParameter = str;
      }
      return queryParameter;
    }
  }
}
