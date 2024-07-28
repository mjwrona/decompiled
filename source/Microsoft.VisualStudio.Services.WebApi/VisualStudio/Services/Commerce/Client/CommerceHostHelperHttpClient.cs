// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.CommerceHostHelperHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Client
{
  [ResourceArea("365D9DCD-4492-4AE3-B5BA-AD0FF4AB74B3")]
  [ClientCancellationTimeout(120)]
  public class CommerceHostHelperHttpClient : VssHttpClientBase
  {
    public CommerceHostHelperHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CommerceHostHelperHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CommerceHostHelperHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CommerceHostHelperHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CommerceHostHelperHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<Guid> CreateInfrastructureOrganization(
      string resourceName,
      string collectionHostName,
      string hostRegion,
      string tags,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CommerceHostHelperHttpClient helperHttpClient1 = this;
      Guid infrastructureOrganization;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (CreateInfrastructureOrganization)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (collectionHostName), collectionHostName);
        collection.Add(nameof (hostRegion), hostRegion);
        collection.Add(nameof (tags), tags);
        CommerceHostHelperHttpClient helperHttpClient2 = helperHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid helperLocationId = CommerceResourceIds.CommerceHostHelperLocationId;
        var routeValues = new{ resourceName = resourceName };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        infrastructureOrganization = await helperHttpClient2.SendAsync<Guid>(put, helperLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return infrastructureOrganization;
    }

    public virtual async Task<List<string>> GetInfrastructureOrganizationProperties(
      Guid propertyKind,
      IEnumerable<string> properties,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CommerceHostHelperHttpClient helperHttpClient1 = this;
      List<string> organizationProperties;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetInfrastructureOrganizationProperties)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add<Guid>(nameof (propertyKind), propertyKind);
        if (properties != null)
          helperHttpClient1.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (properties), (object) properties);
        CommerceHostHelperHttpClient helperHttpClient2 = helperHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid helperLocationId = CommerceResourceIds.CommerceHostHelperLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        organizationProperties = await helperHttpClient2.SendAsync<List<string>>(get, helperLocationId, version: version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return organizationProperties;
    }

    public virtual async Task<bool> UpdateCollectionOwner(
      Guid newOwnerId,
      Guid ownerDomain,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CommerceHostHelperHttpClient helperHttpClient1 = this;
      bool flag;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (UpdateCollectionOwner)))
      {
        KeyValuePair<string, string>[] keyValuePairArray = new KeyValuePair<string, string>[2]
        {
          new KeyValuePair<string, string>(nameof (newOwnerId), newOwnerId.ToString()),
          new KeyValuePair<string, string>(nameof (ownerDomain), ownerDomain.ToString())
        };
        CommerceHostHelperHttpClient helperHttpClient2 = helperHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid helperLocationId = CommerceResourceIds.CommerceHostHelperLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairArray;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        flag = await helperHttpClient2.SendAsync<bool>(put, helperLocationId, version: version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return flag;
    }
  }
}
