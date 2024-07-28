// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupBlobMultipartHttpClient
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [ResourceArea("01E4817C-857E-485C-9401-0334A33200DA")]
  public class DedupBlobMultipartHttpClient : 
    ArtifactHttpClient,
    IDedupBlobMultipartHttpClient,
    IArtifactHttpClient
  {
    public readonly Guid SessionsResourceId = ResourceIds.SessionsCollectionId;
    public readonly Guid PartsResourceId = ResourceIds.PartsCollectionId;
    private static readonly HttpClient _contentStitcherHttpClient = new HttpClient()
    {
      Timeout = TimeSpan.FromMinutes(15.0)
    };
    private const string ApplicationOctetStream = "application/octet-stream";

    public DedupBlobMultipartHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DedupBlobMultipartHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DedupBlobMultipartHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DedupBlobMultipartHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DedupBlobMultipartHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public override Guid ResourceId => ResourceIds.MultiDomainDedupBlobsResourceId;

    protected string TargetVersion => "5.1";

    public virtual async Task<string> PostFileStreamAsync(
      IDomainId domainId,
      Stream file,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient1 = this;
      HttpContent httpContent1 = (HttpContent) new StreamContent(file);
      httpContent1.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      DedupBlobMultipartHttpClient multipartHttpClient2 = multipartHttpClient1;
      HttpMethod post = HttpMethod.Post;
      Guid resourceId = multipartHttpClient1.ResourceId;
      HttpContent httpContent2 = httpContent1;
      var routeValues = new
      {
        domainId = domainId.Serialize()
      };
      ApiResourceVersion version = new ApiResourceVersion(multipartHttpClient1.TargetVersion);
      HttpContent content = httpContent2;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpRequestMessage message = await multipartHttpClient2.CreateRequestMessageAsync(post, resourceId, (object) routeValues, version, content, cancellationToken: cancellationToken1).ConfigureAwait(false);
      return (await multipartHttpClient1.SendAsync(message).ConfigureAwait(false)).Headers.Location.AbsoluteUri;
    }

    public virtual async Task<Stream> GetFileStreamAsync(
      IDomainId domainId,
      DedupIdentifier nodeId,
      bool requestCompression,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient = this;
      Stream fileStreamAsync;
      using (HttpRequestMessage msg = await multipartHttpClient.CreateRequestMessageAsync(HttpMethod.Get, multipartHttpClient.ResourceId, (object) new
      {
        domainId = domainId.Serialize(),
        nodeId = nodeId.ValueString
      }, new ApiResourceVersion(multipartHttpClient.TargetVersion), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
      {
        HttpResponseMessage httpResponseMessage = await multipartHttpClient.SendAsync(msg, (object) HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode == HttpStatusCode.Found)
        {
          HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpResponseMessage.Headers.Location);
          if (requestCompression)
            request.Headers.AcceptEncoding.ParseAdd("gzip");
          httpResponseMessage = await DedupBlobMultipartHttpClient._contentStitcherHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).EnforceCancellation<HttpResponseMessage>(cancellationToken, (Func<string>) (() => string.Format("Timed out waiting for response from ContentStitcher. requestCompression: {0}", (object) requestCompression)), "D:\\a\\_work\\1\\s\\BlobStore\\Client\\WebApi\\DedupBlobMultipartHttpClient.cs", nameof (GetFileStreamAsync), 101).ConfigureAwait(false);
        }
        if (httpResponseMessage.IsSuccessStatusCode)
        {
          fileStreamAsync = await httpResponseMessage.Content.ReadAsStreamAsync().EnforceCancellation<Stream>(cancellationToken, (Func<string>) (() => "Timed out reading response stream."), "D:\\a\\_work\\1\\s\\BlobStore\\Client\\WebApi\\DedupBlobMultipartHttpClient.cs", nameof (GetFileStreamAsync), 110).ConfigureAwait(false);
        }
        else
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            throw new DedupNotFoundException(string.Format("Dedup id {0} not found in domain {1}.", (object) nodeId, (object) domainId));
          throw new InvalidOperationException(string.Format("Get file stream error: {0}\n Error message: {1}", (object) httpResponseMessage.StatusCode, (object) httpResponseMessage.ReasonPhrase));
        }
      }
      return fileStreamAsync;
    }

    public virtual async Task<Uri> GetDownloadRedirectAsync(
      IDomainId domainId,
      DedupIdentifier nodeId,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient = this;
      Uri location;
      using (HttpRequestMessage msg = await multipartHttpClient.CreateRequestMessageAsync(HttpMethod.Get, multipartHttpClient.ResourceId, (object) new
      {
        domainId = domainId.Serialize(),
        nodeId = nodeId.ValueString
      }, new ApiResourceVersion(multipartHttpClient.TargetVersion), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
      {
        HttpResponseMessage httpResponseMessage = await multipartHttpClient.SendAsync(msg, (object) HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode == HttpStatusCode.Found)
        {
          location = httpResponseMessage.Headers.Location;
        }
        else
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            throw new DedupNotFoundException(string.Format("Dedup id {0} not found in domain {1}.", (object) nodeId, (object) domainId));
          throw new InvalidOperationException(string.Format("Get file stream error: {0}\n Error message: {1}", (object) httpResponseMessage.StatusCode, (object) httpResponseMessage.ReasonPhrase));
        }
      }
      return location;
    }

    public virtual async Task<Guid> CreateSessionAsync(
      IDomainId domainId,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient = this;
      HttpRequestMessage message = await multipartHttpClient.CreateRequestMessageAsync(HttpMethod.Post, multipartHttpClient.SessionsResourceId, (object) new
      {
        domainId = domainId.Serialize()
      }, new ApiResourceVersion(multipartHttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
      return new Guid(await (await multipartHttpClient.SendAsync(message).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    public virtual async Task<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session> GetSessionAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient = this;
      HttpRequestMessage message = await multipartHttpClient.CreateRequestMessageAsync(HttpMethod.Get, multipartHttpClient.SessionsResourceId, (object) new
      {
        domainId = domainId.Serialize(),
        sessionId = sessionId.ToString()
      }, new ApiResourceVersion(multipartHttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
      return JsonSerializer.Deserialize<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session>(await (await multipartHttpClient.SendAsync(message).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    public virtual async Task DeleteSessionAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient = this;
      HttpRequestMessage message = await multipartHttpClient.CreateRequestMessageAsync(HttpMethod.Delete, multipartHttpClient.SessionsResourceId, (object) new
      {
        domainId = domainId.Serialize(),
        sessionId = sessionId.ToString()
      }, new ApiResourceVersion(multipartHttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await multipartHttpClient.SendAsync(message).ConfigureAwait(false);
    }

    public virtual async Task<string> CompleteSessionAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient = this;
      HttpRequestMessage message = await multipartHttpClient.CreateRequestMessageAsync(new HttpMethod("PATCH"), multipartHttpClient.SessionsResourceId, (object) new
      {
        domainId = domainId.Serialize(),
        sessionId = sessionId.ToString()
      }, new ApiResourceVersion(multipartHttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
      return await (await multipartHttpClient.SendAsync(message).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<Part>> GetPartsAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient = this;
      HttpRequestMessage message = await multipartHttpClient.CreateRequestMessageAsync(HttpMethod.Get, multipartHttpClient.PartsResourceId, (object) new
      {
        domainId = domainId.Serialize(),
        sessionId = sessionId.ToString()
      }, new ApiResourceVersion(multipartHttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
      return JsonSerializer.Deserialize<IEnumerable<Part>>(await (await multipartHttpClient.SendAsync(message).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    public virtual async Task UploadPartsAsync(
      IDomainId domainId,
      Guid sessionId,
      Stream file,
      long from,
      long to,
      long totalSize,
      CancellationToken cancellationToken)
    {
      DedupBlobMultipartHttpClient multipartHttpClient1 = this;
      HttpContent httpContent1 = (HttpContent) new StreamContent(file);
      httpContent1.Headers.ContentRange = new ContentRangeHeaderValue(from, to, totalSize);
      httpContent1.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      DedupBlobMultipartHttpClient multipartHttpClient2 = multipartHttpClient1;
      HttpMethod put = HttpMethod.Put;
      Guid partsResourceId = multipartHttpClient1.PartsResourceId;
      HttpContent httpContent2 = httpContent1;
      var routeValues = new
      {
        domainId = domainId.Serialize(),
        sessionId = sessionId.ToString()
      };
      ApiResourceVersion version = new ApiResourceVersion(multipartHttpClient1.TargetVersion);
      HttpContent content = httpContent2;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpRequestMessage message = await multipartHttpClient2.CreateRequestMessageAsync(put, partsResourceId, (object) routeValues, version, content, cancellationToken: cancellationToken1).ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await multipartHttpClient1.SendAsync(message).ConfigureAwait(false);
    }
  }
}
