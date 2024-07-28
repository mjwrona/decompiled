// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.VisualStudioLinkingUtility
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class VisualStudioLinkingUtility
  {
    private const string c_vsWebUrl = "vsweb://vs/";
    private const string c_tfsLinkQueryParameter = "tfslink";
    private const string c_vsWebFixedQueryParametersString = "Product=Visual_Studio&Gen=2013&EncFormat=UTF8";

    public static string GetRepositoryLink(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid repositoryId,
      bool useVsWebFormat = true)
    {
      ArtifactId artifactId = new ArtifactId("Framework", "TeamProject", projectId.ToString());
      return VisualStudioLinkingUtility.GetCollectionRepositoryArtifactLink(requestContext, artifactId, repositoryId, useVsWebFormat: useVsWebFormat);
    }

    public static string GetTeamProjectLink(
      IVssRequestContext requestContext,
      Guid projectId,
      bool useVsWebFormat = true)
    {
      ArtifactId artifactId = new ArtifactId("Framework", "TeamProject", projectId.ToString());
      return VisualStudioLinkingUtility.GetCollectionArtifactLink(requestContext, artifactId, useVsWebFormat: useVsWebFormat);
    }

    public static string GetProjectArtifactLink(
      IVssRequestContext requestContext,
      Guid projectId,
      ArtifactId artifactId,
      IDictionary<string, string> queryParameters = null,
      bool useVsWebFormat = true)
    {
      if (queryParameters == null)
        queryParameters = (IDictionary<string, string>) new Dictionary<string, string>();
      queryParameters["project"] = projectId.ToString();
      return VisualStudioLinkingUtility.GetCollectionArtifactLink(requestContext, artifactId, queryParameters, useVsWebFormat);
    }

    public static string TryGetProjectArtifactLink(
      IVssRequestContext requestContext,
      string projectName,
      ArtifactId artifactId,
      IDictionary<string, string> queryParameters = null,
      bool useVsWebFormat = true)
    {
      Guid? nullable = VisualStudioLinkingUtility.TryResolveProjectName(requestContext, projectName);
      return nullable.HasValue ? VisualStudioLinkingUtility.GetProjectArtifactLink(requestContext, nullable.Value, artifactId, queryParameters, useVsWebFormat) : (string) null;
    }

    private static Guid? TryResolveProjectName(
      IVssRequestContext requestContext,
      string projectName)
    {
      try
      {
        return new Guid?(requestContext.GetService<IProjectService>().GetProject(requestContext.Elevate(), projectName).Id);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "WebApi", nameof (VisualStudioLinkingUtility), ex);
        return new Guid?();
      }
    }

    public static string GetCollectionRepositoryArtifactLink(
      IVssRequestContext requestContext,
      ArtifactId artifactId,
      Guid repositoryId,
      IDictionary<string, string> queryParameters = null,
      bool useVsWebFormat = true)
    {
      if (queryParameters == null)
        queryParameters = (IDictionary<string, string>) new Dictionary<string, string>();
      queryParameters["repo"] = repositoryId.ToString();
      return VisualStudioLinkingUtility.GetCollectionArtifactLink(requestContext, artifactId, queryParameters, useVsWebFormat);
    }

    public static string GetCollectionArtifactLink(
      IVssRequestContext requestContext,
      ArtifactId artifactId,
      IDictionary<string, string> queryParameters = null,
      bool useVsWebFormat = true)
    {
      if (queryParameters == null)
        queryParameters = (IDictionary<string, string>) new Dictionary<string, string>();
      string collectionUrl = VisualStudioLinkingUtility.GetCollectionUrl(requestContext);
      if (string.IsNullOrEmpty(collectionUrl))
        return (string) null;
      if (collectionUrl.Any<char>((Func<char, bool>) (c => c > '\u007F')))
        queryParameters["base64url"] = UriUtility.Base64Encode(Uri.EscapeDataString(collectionUrl));
      else
        queryParameters["url"] = collectionUrl;
      return VisualStudioLinkingUtility.GetArtifactLink(requestContext, artifactId, queryParameters, useVsWebFormat);
    }

    private static string GetArtifactLink(
      IVssRequestContext requestContext,
      ArtifactId artifactId,
      IDictionary<string, string> queryParameters,
      bool useVsWebFormat = true)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(LinkingUtilities.EncodeUri(artifactId));
      if (queryParameters != null && queryParameters.Count > 0)
      {
        stringBuilder.Append("?");
        bool flag = false;
        foreach (KeyValuePair<string, string> queryParameter in (IEnumerable<KeyValuePair<string, string>>) queryParameters)
        {
          if (flag)
            stringBuilder.Append("&");
          stringBuilder.Append(Uri.EscapeDataString(queryParameter.Key));
          stringBuilder.Append("=");
          stringBuilder.Append(Uri.EscapeDataString(queryParameter.Value));
          flag = true;
        }
      }
      return useVsWebFormat ? VisualStudioLinkingUtility.GetVsWebLinkForTfsLink(stringBuilder.ToString()) : stringBuilder.ToString();
    }

    private static string GetCollectionUrl(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);

    public static string GetVsWebLinkForTfsLink(string tfsLink)
    {
      UriBuilder uriBuilder = new UriBuilder("vsweb://vs/");
      NameValueCollection queryString = HttpUtility.ParseQueryString("Product=Visual_Studio&Gen=2013&EncFormat=UTF8");
      queryString["tfslink"] = UriUtility.Base64Encode(tfsLink);
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri.AbsoluteUri;
    }
  }
}
