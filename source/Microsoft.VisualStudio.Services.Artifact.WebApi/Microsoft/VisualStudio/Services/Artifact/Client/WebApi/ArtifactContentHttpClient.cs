// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Artifact.Client.WebApi.ArtifactContentHttpClient
// Assembly: Microsoft.VisualStudio.Services.Artifact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D39C0B4C-25E7-402A-9BC9-E3DFE7654881
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Artifact.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Artifact.Client.WebApi
{
  public class ArtifactContentHttpClient : ArtifactHttpClientBase
  {
    public ArtifactContentHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ArtifactContentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ArtifactContentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ArtifactContentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ArtifactContentHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public override async Task<Stream> GetContentAsync(
      Guid project,
      string artifactId,
      string format,
      string subPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArtifactContentHttpClient contentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("GET");
      Guid guid = new Guid("f2984867-8ebd-433e-8fff-6e19d7c68c60");
      object obj = (object) new
      {
        project = project,
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (format), format);
      if (subPath != null)
        collection.Add(nameof (subPath), subPath);
      string str1;
      if (string.Equals(format, "zip", StringComparison.OrdinalIgnoreCase))
      {
        str1 = "application/zip";
      }
      else
      {
        if (!string.Equals(format, "file", StringComparison.OrdinalIgnoreCase))
          throw new NotImplementedException("Format is unrecognized: " + format);
        str1 = "application/octet-stream";
      }
      ArtifactContentHttpClient contentHttpClient2 = contentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 1);
      List<KeyValuePair<string, string>> queryParameters = collection;
      string str2 = str1;
      CancellationToken cancellationToken1 = cancellationToken;
      string mediaType = str2;
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await contentHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, cancellationToken: cancellationToken1, mediaType: mediaType).ConfigureAwait(false))
        httpResponseMessage = await contentHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }
  }
}
