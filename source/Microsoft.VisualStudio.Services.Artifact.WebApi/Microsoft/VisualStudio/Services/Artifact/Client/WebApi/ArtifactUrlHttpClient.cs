// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Artifact.Client.WebApi.ArtifactUrlHttpClient
// Assembly: Microsoft.VisualStudio.Services.Artifact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D39C0B4C-25E7-402A-9BC9-E3DFE7654881
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Artifact.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Artifact.Client.WebApi
{
  [ResourceArea("2A313F99-F039-49A7-B2DD-792D5DDAB990")]
  public class ArtifactUrlHttpClient : ArtifactContentHttpClient
  {
    public ArtifactUrlHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ArtifactUrlHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ArtifactUrlHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ArtifactUrlHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ArtifactUrlHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<Uri> GetContentUriAsync(
      Guid projectId,
      string artifactId,
      string format)
    {
      return await this.GetContentUriAsync(projectId, artifactId, format, string.Empty);
    }

    public virtual async Task<Uri> GetContentUriAsync(
      Guid projectId,
      string artifactId,
      string format,
      string subPath)
    {
      Guid locationId = new Guid("f2984867-8ebd-433e-8fff-6e19d7c68c60");
      return await this.GetContentUriInternalAsync(projectId, locationId, artifactId, format, subPath, new ApiResourceVersion(5.0, 1));
    }

    public virtual async Task<Uri> GetSignedContentUriAsync(
      Guid projectId,
      string artifactId,
      string format)
    {
      return await this.GetSignedContentUriAsync(projectId, artifactId, format, string.Empty);
    }

    public virtual async Task<Uri> GetSignedContentUriAsync(
      Guid projectId,
      string artifactId,
      string format,
      string subPath)
    {
      Guid locationId = new Guid("5C8988D4-6ACA-4810-9BBD-31D3E12D69A2");
      return await this.GetContentUriInternalAsync(projectId, locationId, artifactId, format, subPath, new ApiResourceVersion(5.2, 1));
    }

    private async Task<Uri> GetContentUriInternalAsync(
      Guid projectId,
      Guid locationId,
      string artifactId,
      string format,
      string subPath,
      ApiResourceVersion apiResourceVersion)
    {
      ArtifactUrlHttpClient artifactUrlHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      object routeValues = (object) new
      {
        project = projectId.ToString(),
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(format))
        keyValuePairList.Add(nameof (format), format);
      if (!string.IsNullOrEmpty(subPath))
        keyValuePairList.Add(nameof (subPath), subPath);
      return (await artifactUrlHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, apiResourceVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList)).RequestUri;
    }

    public static string CreateArtifactId(
      string organizationName,
      Guid projectId,
      int pipelineId,
      string artifactName)
    {
      return UrlEncodingUtility.UrlTokenEncode(string.Format("pipelineartifact://{0}/projectId/{1}/buildId/{2}/artifactName/{3}", (object) organizationName, (object) projectId, (object) pipelineId, (object) WebUtility.UrlEncode(artifactName)));
    }
  }
}
