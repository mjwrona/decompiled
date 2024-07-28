// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Artifact.Client.WebApi.ArtifactHttpClientBase
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
  public abstract class ArtifactHttpClientBase : VssHttpClientBase
  {
    public ArtifactHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ArtifactHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ArtifactHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ArtifactHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ArtifactHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Stream> GetContentAsync(
      string project,
      string artifactId,
      string format,
      string subPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f2984867-8ebd-433e-8fff-6e19d7c68c60");
      object routeValues = (object) new
      {
        project = project,
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (format), format);
      if (subPath != null)
        keyValuePairList.Add(nameof (subPath), subPath);
      return this.SendAsync<Stream>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Stream> GetContentAsync(
      Guid project,
      string artifactId,
      string format,
      string subPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f2984867-8ebd-433e-8fff-6e19d7c68c60");
      object routeValues = (object) new
      {
        project = project,
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (format), format);
      if (subPath != null)
        keyValuePairList.Add(nameof (subPath), subPath);
      return this.SendAsync<Stream>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Stream> GetContentAsync(
      string artifactId,
      string format,
      string subPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f2984867-8ebd-433e-8fff-6e19d7c68c60");
      object routeValues = (object) new
      {
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (format), format);
      if (subPath != null)
        keyValuePairList.Add(nameof (subPath), subPath);
      return this.SendAsync<Stream>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
