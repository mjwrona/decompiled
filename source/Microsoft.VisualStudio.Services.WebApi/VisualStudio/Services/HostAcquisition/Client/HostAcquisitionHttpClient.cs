// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostAcquisition.Client.HostAcquisitionHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.HostAcquisition.Client
{
  [ResourceArea("8E128563-B59C-4A70-964C-A3BD7412183D")]
  public class HostAcquisitionHttpClient : VssHttpClientBase
  {
    public HostAcquisitionHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public HostAcquisitionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public HostAcquisitionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public HostAcquisitionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public HostAcquisitionHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Collection> CreateCollectionAsync(
      IDictionary<string, string> properties,
      string collectionName,
      string preferredRegion,
      string ownerDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2bbead06-ca34-4dd7-9fe2-148735723a0a");
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, string>>(properties, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (collectionName), collectionName);
      collection.Add(nameof (preferredRegion), preferredRegion);
      if (ownerDescriptor != null)
        collection.Add(nameof (ownerDescriptor), ownerDescriptor);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Collection>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Collection> CreateCollectionAsync(
      IDictionary<string, string> properties,
      string collectionName,
      string preferredRegion,
      string preferredGeography,
      string ownerDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2bbead06-ca34-4dd7-9fe2-148735723a0a");
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, string>>(properties, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (collectionName), collectionName);
      collection.Add(nameof (preferredRegion), preferredRegion);
      collection.Add(nameof (preferredGeography), preferredGeography);
      if (ownerDescriptor != null)
        collection.Add(nameof (ownerDescriptor), ownerDescriptor);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Collection>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.HostAcquisition.Geography>> GetGeographiesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.VisualStudio.Services.HostAcquisition.Geography>>(new HttpMethod("GET"), new Guid("81f025e6-8678-42bc-aa6f-2a3cc2df3b75"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<NameAvailability> GetNameAvailabilityAsync(
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NameAvailability>(new HttpMethod("GET"), new Guid("01a4cda4-66d1-4f35-918a-212111edc9a4"), (object) new
      {
        name = name
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.HostAcquisition.Region>> GetRegionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.VisualStudio.Services.HostAcquisition.Region>>(new HttpMethod("GET"), new Guid("776ef918-0dad-4eb1-a614-04988ca3a072"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
