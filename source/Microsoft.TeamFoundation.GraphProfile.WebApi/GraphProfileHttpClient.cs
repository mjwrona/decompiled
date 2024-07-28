// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GraphProfile.WebApi.GraphProfileHttpClient
// Assembly: Microsoft.TeamFoundation.GraphProfile.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10687127-D73A-4F03-AA93-A7EDA3B5980D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.GraphProfile.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.GraphProfile.WebApi
{
  [ResourceArea("4E40F190-2E3F-4D9F-8331-C7788E833080")]
  public class GraphProfileHttpClient : VssHttpClientBase
  {
    public GraphProfileHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public GraphProfileHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public GraphProfileHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public GraphProfileHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public GraphProfileHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<GraphMemberAvatarResponse> GetMemberAvatarAsync(
      string memberDescriptor,
      GraphMemberAvatarSize? size = null,
      string stamp = null,
      string overrideDisplayName = null,
      bool? generateDefaultMemberAvatar = null,
      string ifNoneMatch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphProfileHttpClient profileHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d443431f-b341-42e4-85cf-a5b0d639ed8f");
      object routeValues = (object) new
      {
        memberDescriptor = memberDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (size.HasValue)
        keyValuePairList1.Add(nameof (size), size.Value.ToString());
      if (stamp != null)
        keyValuePairList1.Add(nameof (stamp), stamp);
      if (overrideDisplayName != null)
        keyValuePairList1.Add(nameof (overrideDisplayName), overrideDisplayName);
      if (generateDefaultMemberAvatar.HasValue)
        keyValuePairList1.Add(nameof (generateDefaultMemberAvatar), generateDefaultMemberAvatar.Value.ToString());
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(ifNoneMatch))
        keyValuePairList2.Add("If-None-Match", ifNoneMatch);
      GraphMemberAvatarResponse memberAvatarAsync;
      using (HttpRequestMessage requestMessage = await profileHttpClient.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        GraphMemberAvatarResponse returnObject = new GraphMemberAvatarResponse();
        using (HttpResponseMessage response = await profileHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContentType = profileHttpClient.GetHeaderValue(response, "Content-Type");
          returnObject.Etag = profileHttpClient.GetHeaderValue(response, "Etag");
          returnObject.CacheControl = profileHttpClient.GetHeaderValue(response, "Cache-Control");
          GraphMemberAvatarResponse memberAvatarResponse = returnObject;
          memberAvatarResponse.Avatar = await profileHttpClient.ReadContentAsAsync<GraphMemberAvatar>(response, cancellationToken).ConfigureAwait(false);
          memberAvatarResponse = (GraphMemberAvatarResponse) null;
        }
        memberAvatarAsync = returnObject;
      }
      return memberAvatarAsync;
    }

    public async Task<byte[]> GetMemberAvatarImageDataAsync(
      string memberDescriptor,
      GraphMemberAvatarSize? size = null,
      string stamp = null,
      string overrideDisplayName = null,
      bool? generateDefaultMemberAvatar = null,
      string ifNoneMatch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphProfileHttpClient profileHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d443431f-b341-42e4-85cf-a5b0d639ed8f");
      object routeValues = (object) new
      {
        memberDescriptor = memberDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (size.HasValue)
        keyValuePairList1.Add(nameof (size), size.Value.ToString());
      if (stamp != null)
        keyValuePairList1.Add(nameof (stamp), stamp);
      if (overrideDisplayName != null)
        keyValuePairList1.Add(nameof (overrideDisplayName), overrideDisplayName);
      if (generateDefaultMemberAvatar.HasValue)
        keyValuePairList1.Add(nameof (generateDefaultMemberAvatar), generateDefaultMemberAvatar.Value.ToString());
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(ifNoneMatch))
        keyValuePairList2.Add("If-None-Match", ifNoneMatch);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await profileHttpClient.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, cancellationToken: cancellationToken, mediaType: "image/*").ConfigureAwait(false))
        httpResponseMessage = await profileHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }
  }
}
