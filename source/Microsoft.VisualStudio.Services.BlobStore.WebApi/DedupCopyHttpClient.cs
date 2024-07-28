// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupCopyHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [ResourceArea("dedup")]
  public class DedupCopyHttpClient : ArtifactHttpClient, IDedupCopyHttpClient, IArtifactHttpClient
  {
    public override Guid ResourceId => ResourceIds.DedupCopyResourceId;

    public DedupCopyHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DedupCopyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DedupCopyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DedupCopyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DedupCopyHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<Guid> CopyDedupsAsync(
      ISet<DedupIdentifier> dedupIds,
      IDomainId sourceDomainId,
      IDomainId destDomainId,
      Guid sourceHostId,
      CancellationToken cancellationToken)
    {
      DedupCopyHttpClient dedupCopyHttpClient1 = this;
      HttpContent content1 = JsonSerializer.SerializeToContent<DedupCopyBatch>(new DedupCopyBatch()
      {
        SourceDomainId = sourceDomainId,
        SourceHostId = sourceHostId,
        DedupIds = dedupIds
      });
      DedupCopyHttpClient dedupCopyHttpClient2 = dedupCopyHttpClient1;
      HttpMethod post = HttpMethod.Post;
      Guid dedupCopyResourceId = ResourceIds.DedupCopyResourceId;
      HttpContent httpContent = content1;
      var routeValues = new
      {
        domainId = destDomainId.Serialize()
      };
      ApiResourceVersion version = new ApiResourceVersion(new Version(6, 1));
      HttpContent content2 = httpContent;
      CancellationToken cancellationToken1 = new CancellationToken();
      HttpRequestMessage message = await dedupCopyHttpClient2.CreateRequestMessageAsync(post, dedupCopyResourceId, (object) routeValues, version, content2, cancellationToken: cancellationToken1).ConfigureAwait(false);
      return Guid.Parse(await (await dedupCopyHttpClient1.SendAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }
  }
}
