// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.Clients.FavoriteHttpClient
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi.Clients
{
  public class FavoriteHttpClient : VssHttpClientBase
  {
    public FavoriteHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FavoriteHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FavoriteHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FavoriteHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FavoriteHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<FavoriteProvider>> GetFavoriteProvidersAsync(
      bool? faultInMissingHost = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0c04d86b-e315-464f-8125-4d6222d306c2");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (faultInMissingHost.HasValue)
        keyValuePairList.Add(nameof (faultInMissingHost), faultInMissingHost.Value.ToString());
      return this.SendAsync<List<FavoriteProvider>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Favorite> CreateFavoriteAsync(
      FavoriteCreateParameters favorite,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      HttpContent httpContent = (HttpContent) new ObjectContent<FavoriteCreateParameters>(favorite, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Favorite>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Favorite> CreateFavoriteOfOwnerAsync(
      FavoriteCreateParameters favorite,
      string ownerScopeType,
      Guid ownerScopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      HttpContent httpContent = (HttpContent) new ObjectContent<FavoriteCreateParameters>(favorite, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (ownerScopeType), ownerScopeType);
      collection.Add(nameof (ownerScopeId), ownerScopeId.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Favorite>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteFavoriteByIdAsync(
      Guid favoriteId,
      string artifactType,
      string artifactScopeType,
      string artifactScopeId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FavoriteHttpClient favoriteHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      object routeValues = (object) new
      {
        favoriteId = favoriteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      using (await favoriteHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeleteFavoriteOfOwnerByIdAsync(
      Guid favoriteId,
      string ownerScopeType,
      Guid ownerScopeId,
      string artifactType,
      string artifactScopeType,
      string artifactScopeId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FavoriteHttpClient favoriteHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      object routeValues = (object) new
      {
        favoriteId = favoriteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (ownerScopeType), ownerScopeType);
      keyValuePairList.Add(nameof (ownerScopeId), ownerScopeId.ToString());
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      using (await favoriteHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Favorite> GetFavoriteByArtifactAsync(
      string artifactType,
      string artifactId,
      string artifactScopeType,
      string artifactScopeId = null,
      bool? includeExtendedDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactId), artifactId);
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      if (includeExtendedDetails.HasValue)
        keyValuePairList.Add(nameof (includeExtendedDetails), includeExtendedDetails.Value.ToString());
      return this.SendAsync<Favorite>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Favorite> GetFavoriteByIdAsync(
      Guid favoriteId,
      string artifactScopeType,
      string artifactType,
      string artifactScopeId = null,
      bool? includeExtendedDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      object routeValues = (object) new
      {
        favoriteId = favoriteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      if (includeExtendedDetails.HasValue)
        keyValuePairList.Add(nameof (includeExtendedDetails), includeExtendedDetails.Value.ToString());
      return this.SendAsync<Favorite>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Favorite> GetFavoriteOfOwnerByIdAsync(
      Guid favoriteId,
      string ownerScopeType,
      Guid ownerScopeId,
      string artifactScopeType,
      string artifactType,
      string artifactScopeId = null,
      bool? includeExtendedDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      object routeValues = (object) new
      {
        favoriteId = favoriteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (ownerScopeType), ownerScopeType);
      keyValuePairList.Add(nameof (ownerScopeId), ownerScopeId.ToString());
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      if (includeExtendedDetails.HasValue)
        keyValuePairList.Add(nameof (includeExtendedDetails), includeExtendedDetails.Value.ToString());
      return this.SendAsync<Favorite>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Favorite>> GetFavoritesAsync(
      string artifactType = null,
      string artifactScopeType = null,
      string artifactScopeId = null,
      bool? includeExtendedDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (artifactType != null)
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactScopeType != null)
        keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      if (includeExtendedDetails.HasValue)
        keyValuePairList.Add(nameof (includeExtendedDetails), includeExtendedDetails.Value.ToString());
      return this.SendAsync<List<Favorite>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Favorite>> GetFavoritesOfOwnerAsync(
      string ownerScopeType,
      Guid ownerScopeId,
      string artifactType = null,
      string artifactScopeType = null,
      string artifactScopeId = null,
      bool? includeExtendedDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f13e9a6-aae2-4b89-b683-131ca9564cec");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (ownerScopeType), ownerScopeType);
      keyValuePairList.Add(nameof (ownerScopeId), ownerScopeId.ToString());
      if (artifactType != null)
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactScopeType != null)
        keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      if (includeExtendedDetails.HasValue)
        keyValuePairList.Add(nameof (includeExtendedDetails), includeExtendedDetails.Value.ToString());
      return this.SendAsync<List<Favorite>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
