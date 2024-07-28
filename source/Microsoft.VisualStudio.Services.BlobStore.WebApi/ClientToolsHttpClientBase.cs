// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.ClientToolsHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public abstract class ClientToolsHttpClientBase : VssHttpClientBase
  {
    public ClientToolsHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ClientToolsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ClientToolsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ClientToolsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ClientToolsHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ClientToolReleaseInfo> GetReleaseAsync(
      string toolName,
      string version = null,
      string osName = null,
      string osRelease = null,
      string osVersion = null,
      string arch = null,
      string distroName = null,
      string distroVersion = null,
      bool? netfx = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("187ec90d-dd1e-4ec6-8c57-937d979261e5");
      object routeValues = (object) new
      {
        toolName = toolName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (version != null)
        keyValuePairList.Add(nameof (version), version);
      if (osName != null)
        keyValuePairList.Add(nameof (osName), osName);
      if (osRelease != null)
        keyValuePairList.Add(nameof (osRelease), osRelease);
      if (osVersion != null)
        keyValuePairList.Add(nameof (osVersion), osVersion);
      if (arch != null)
        keyValuePairList.Add(nameof (arch), arch);
      if (distroName != null)
        keyValuePairList.Add(nameof (distroName), distroName);
      if (distroVersion != null)
        keyValuePairList.Add(nameof (distroVersion), distroVersion);
      if (netfx.HasValue)
        keyValuePairList.Add(nameof (netfx), netfx.Value.ToString());
      return this.SendAsync<ClientToolReleaseInfo>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ClientSettingsInfo> GetSettingsAsync(
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Client toolName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ClientSettingsInfo>(new HttpMethod("GET"), new Guid("2213dc4f-bf6d-4893-92b2-2ef04c852b40"), (object) new
      {
        toolName = toolName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
