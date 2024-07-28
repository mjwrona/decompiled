// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.UriHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class UriHelper
  {
    public const string CleanupPath = "path";
    public const string CleanupProjectId = "projectId";
    public const string CleanupDefinitionId = "definitionId";

    public static Uri CreateBuildUri(int buildId) => UriHelper.CreateArtifactUri("Build", buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public static Uri CreateBuildUriForDistributedTask(Guid projectId, int buildId) => UriHelper.CreateArtifactUri("Build", projectId.ToString() + "/" + buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public static Uri CreateArtifactUri(string artifactTypeName, string toolSpecificId) => new Uri(LinkingUtilities.EncodeUri(new ArtifactId()
    {
      ToolSpecificId = toolSpecificId,
      ArtifactType = artifactTypeName,
      Tool = "Build"
    }), UriKind.Absolute);

    public static Uri CreateArtifactUriWithParam(
      string artifactTypeName,
      string toolSpecificId,
      string key,
      string value)
    {
      return UriHelper.CreateArtifactUriWithParams(artifactTypeName, toolSpecificId, new (string, string)[1]
      {
        (key, value)
      });
    }

    public static Uri CreateArtifactUriWithParams(
      string artifactTypeName,
      string toolSpecificId,
      (string key, string value)[] queryParams)
    {
      ArtifactId artifactId = new ArtifactId();
      artifactId.ToolSpecificId = toolSpecificId;
      artifactId.ArtifactType = artifactTypeName;
      artifactId.Tool = "Build";
      string str = string.Join("&", ((IEnumerable<(string, string)>) queryParams).Select<(string, string), string>((Func<(string, string), string>) (t => t.key + "=" + t.value)));
      return new Uri(LinkingUtilities.EncodeUri(artifactId) + "?" + str, UriKind.Absolute);
    }

    public static bool TryGetBuildId(Uri artifactUri, out int buildId) => UriHelper.TryGetBuildInformation(artifactUri, out buildId, out int _);

    public static bool TryGetBuildInformation(
      Uri artifactUri,
      out int buildId,
      out int orchestrationType)
    {
      ArgumentUtility.CheckForNull<Uri>(artifactUri, nameof (artifactUri));
      string[] values = HttpUtility.ParseQueryString(artifactUri.Query).GetValues("BuildOrchestrationType");
      if (values != null && values.Length != 0)
      {
        string s = values[0];
        orchestrationType = int.Parse(s);
      }
      else
        orchestrationType = 1;
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri.ToString());
      int length = artifactId.ToolSpecificId.IndexOf("?");
      return length < 0 ? int.TryParse(artifactId.ToolSpecificId, out buildId) : int.TryParse(artifactId.ToolSpecificId.Substring(0, length), out buildId);
    }

    public static bool TryGetBuildCleanupInformation(
      Uri artifactUri,
      out Guid projectId,
      out string path,
      out int definitionId)
    {
      ArgumentUtility.CheckForNull<Uri>(artifactUri, nameof (artifactUri));
      NameValueCollection queryString = HttpUtility.ParseQueryString(artifactUri.Query);
      projectId = Guid.Empty;
      path = string.Empty;
      definitionId = 0;
      path = queryString[nameof (path)];
      return (path == null ? 0 : (Guid.TryParse(queryString[nameof (projectId)], out projectId) ? 1 : 0)) != 0 && int.TryParse(queryString[nameof (definitionId)], out definitionId);
    }
  }
}
