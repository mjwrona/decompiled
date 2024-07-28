// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.HostDomainHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class HostDomainHttpClient : ArtifactHttpClient, IHostDomainHttpClient, IArtifactHttpClient
  {
    public HostDomainHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public HostDomainHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public HostDomainHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public HostDomainHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public HostDomainHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public override Guid ResourceId => ResourceIds.HostDomainResourceId;

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) HostDomainExceptionMapping.ClientTranslatedExceptions;

    public async Task<IEnumerable<MultiDomainInfo>> GetDomainsAsync(
      CancellationToken cancellationToken)
    {
      HostDomainHttpClient domainHttpClient = this;
      HttpRequestMessage message = await domainHttpClient.CreateRequestMessageAsync(HttpMethod.Get, domainHttpClient.ResourceId, version: new ApiResourceVersion(domainHttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
      return (IEnumerable<MultiDomainInfo>) JsonConvert.DeserializeObject<List<MultiDomainInfo>>(await (await domainHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, (object) null, cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    public async Task<MultiDomainInfo> GetDomainAsync(
      string domainId,
      CancellationToken cancellationToken)
    {
      HostDomainHttpClient domainHttpClient = this;
      try
      {
        HttpRequestMessage message = await domainHttpClient.CreateRequestMessageAsync(HttpMethod.Get, domainHttpClient.ResourceId, (object) new
        {
          domainId = domainId
        }, new ApiResourceVersion(domainHttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<MultiDomainInfo>(await (await domainHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, (object) null, cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
      }
      catch (DomainNotFoundException ex)
      {
        return (MultiDomainInfo) null;
      }
    }

    public async Task<IMultiDomainInfo> CreateProjectDomainsForAdminAsync(
      Guid projectId,
      string physicalDomainId,
      bool isDelete,
      bool forceDelete,
      CancellationToken cancellationToken)
    {
      HostDomainHttpClient domainHttpClient1 = this;
      ProjectDomainRequest projectDomainRequest = new ProjectDomainRequest()
      {
        ProjectId = projectId.ToString(),
        DomainId = physicalDomainId
      };
      HostDomainHttpClient domainHttpClient2 = domainHttpClient1;
      HttpMethod post = HttpMethod.Post;
      Guid resourceId = domainHttpClient1.ResourceId;
      HttpContent httpContent = (HttpContent) new StringContent(JsonConvert.SerializeObject((object) projectDomainRequest), Encoding.UTF8, "application/json");
      ApiResourceVersion version = new ApiResourceVersion(domainHttpClient1.TargetVersion);
      HttpContent content = httpContent;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpRequestMessage message = await domainHttpClient2.CreateRequestMessageAsync(post, resourceId, version: version, content: content, cancellationToken: cancellationToken1).ConfigureAwait(false);
      return (IMultiDomainInfo) JsonConvert.DeserializeObject<MultiDomainInfo>(await (await domainHttpClient1.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, (object) null, cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    protected string TargetVersion => "5.1";
  }
}
