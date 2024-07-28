// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionManagementHttpClient
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.AcquisitionRequest;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [ResourceArea("6C2B0933-3600-42AE-BF8B-93D4F7E83594")]
  [ClientCircuitBreakerSettings(6, 80)]
  public class ExtensionManagementHttpClient : VssHttpClientBase
  {
    public ExtensionManagementHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ExtensionManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ExtensionManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ExtensionManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ExtensionManagementHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<AcquisitionOptions> GetAcquisitionOptionsAsync(
      string itemId,
      bool? testCommerce = null,
      bool? isFreeOrTrialInstall = null,
      bool? isAccountOwner = null,
      bool? isLinked = null,
      bool? isConnectedServer = null,
      bool? isBuyOperationValid = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("288dff58-d13b-468e-9671-0fb754e9398c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (itemId), itemId);
      bool flag;
      if (testCommerce.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = testCommerce.Value;
        string str = flag.ToString();
        collection.Add(nameof (testCommerce), str);
      }
      if (isFreeOrTrialInstall.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isFreeOrTrialInstall.Value;
        string str = flag.ToString();
        collection.Add(nameof (isFreeOrTrialInstall), str);
      }
      if (isAccountOwner.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isAccountOwner.Value;
        string str = flag.ToString();
        collection.Add(nameof (isAccountOwner), str);
      }
      if (isLinked.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isLinked.Value;
        string str = flag.ToString();
        collection.Add(nameof (isLinked), str);
      }
      if (isConnectedServer.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isConnectedServer.Value;
        string str = flag.ToString();
        collection.Add(nameof (isConnectedServer), str);
      }
      if (isBuyOperationValid.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isBuyOperationValid.Value;
        string str = flag.ToString();
        collection.Add(nameof (isBuyOperationValid), str);
      }
      return this.SendAsync<AcquisitionOptions>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<ExtensionAcquisitionRequest> RequestAcquisitionAsync(
      ExtensionAcquisitionRequest acquisitionRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("da616457-eed3-4672-92d7-18d21f5c1658");
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionAcquisitionRequest>(acquisitionRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionAcquisitionRequest>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<ExtensionAuditLog> GetAuditLogAsync(
      string publisherName,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ExtensionAuditLog>(new HttpMethod("GET"), new Guid("23a312e0-562d-42fb-a505-5a046b5635db"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<ExtensionAuthorization> RegisterAuthorizationAsync(
      string publisherName,
      string extensionName,
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ExtensionAuthorization>(new HttpMethod("PUT"), new Guid("f21cfc80-d2d2-4248-98bb-7820c74c4606"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        registrationId = registrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<JObject> CreateDocumentByNameAsync(
      JObject doc,
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bbe06c18-1c8b-4fcd-b9c6-1535aaab8749");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        scopeType = scopeType,
        scopeValue = scopeValue,
        collectionName = collectionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JObject>(doc, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<JObject>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task DeleteDocumentByNameAsync(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      string documentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("bbe06c18-1c8b-4fcd-b9c6-1535aaab8749"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        scopeType = scopeType,
        scopeValue = scopeValue,
        collectionName = collectionName,
        documentId = documentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<JObject> GetDocumentByNameAsync(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      string documentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<JObject>(new HttpMethod("GET"), new Guid("bbe06c18-1c8b-4fcd-b9c6-1535aaab8749"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        scopeType = scopeType,
        scopeValue = scopeValue,
        collectionName = collectionName,
        documentId = documentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<JObject>> GetDocumentsByNameAsync(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<JObject>>(new HttpMethod("GET"), new Guid("bbe06c18-1c8b-4fcd-b9c6-1535aaab8749"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        scopeType = scopeType,
        scopeValue = scopeValue,
        collectionName = collectionName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<JObject> SetDocumentByNameAsync(
      JObject doc,
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("bbe06c18-1c8b-4fcd-b9c6-1535aaab8749");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        scopeType = scopeType,
        scopeValue = scopeValue,
        collectionName = collectionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JObject>(doc, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<JObject>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<JObject> UpdateDocumentByNameAsync(
      JObject doc,
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bbe06c18-1c8b-4fcd-b9c6-1535aaab8749");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        scopeType = scopeType,
        scopeValue = scopeValue,
        collectionName = collectionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JObject>(doc, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<JObject>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<ExtensionDataCollection>> QueryCollectionsByNameAsync(
      ExtensionDataCollectionQuery collectionQuery,
      string publisherName,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("56c331f1-ce53-4318-adfd-4db5c52a7a2e");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionDataCollectionQuery>(collectionQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ExtensionDataCollection>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<ExtensionState>> GetStatesAsync(
      bool? includeDisabled = null,
      bool? includeErrors = null,
      bool? includeInstallationIssues = null,
      bool? forceRefresh = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("92755d3d-9a8a-42b3-8a4d-87359fe5aa93");
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeDisabled.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDisabled.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDisabled), str);
      }
      if (includeErrors.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeErrors.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeErrors), str);
      }
      if (includeInstallationIssues.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeInstallationIssues.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeInstallationIssues), str);
      }
      if (forceRefresh.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = forceRefresh.Value;
        string str = flag.ToString();
        collection.Add(nameof (forceRefresh), str);
      }
      return this.SendAsync<List<ExtensionState>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<InstalledExtension>> QueryExtensionsAsync(
      InstalledExtensionQuery query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("046c980f-1345-4ce2-bf85-b46d10ff4cfd");
      HttpContent httpContent = (HttpContent) new ObjectContent<InstalledExtensionQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<InstalledExtension>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<InstalledExtension>> GetInstalledExtensionsAsync(
      bool? includeDisabledExtensions = null,
      bool? includeErrors = null,
      IEnumerable<string> assetTypes = null,
      bool? includeInstallationIssues = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("275424d0-c844-4fe2-bda6-04933a1357d8");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeDisabledExtensions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDisabledExtensions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDisabledExtensions), str);
      }
      if (includeErrors.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeErrors.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeErrors), str);
      }
      if (assetTypes != null && assetTypes.Any<string>())
        keyValuePairList.Add(nameof (assetTypes), string.Join(":", assetTypes));
      if (includeInstallationIssues.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeInstallationIssues.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeInstallationIssues), str);
      }
      return this.SendAsync<List<InstalledExtension>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<InstalledExtension> UpdateInstalledExtensionAsync(
      InstalledExtension extension,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("275424d0-c844-4fe2-bda6-04933a1357d8");
      HttpContent httpContent = (HttpContent) new ObjectContent<InstalledExtension>(extension, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<InstalledExtension>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<InstalledExtension> GetInstalledExtensionByNameAsync(
      string publisherName,
      string extensionName,
      IEnumerable<string> assetTypes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb0da285-f23e-4b56-8b53-3ef5f9f6de66");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (assetTypes != null && assetTypes.Any<string>())
        keyValuePairList.Add(nameof (assetTypes), string.Join(":", assetTypes));
      return this.SendAsync<InstalledExtension>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<InstalledExtension> InstallExtensionByNameAsync(
      string publisherName,
      string extensionName,
      string version = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<InstalledExtension>(new HttpMethod("POST"), new Guid("fb0da285-f23e-4b56-8b53-3ef5f9f6de66"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task UninstallExtensionByNameAsync(
      string publisherName,
      string extensionName,
      string reason = null,
      string reasonCode = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ExtensionManagementHttpClient managementHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("fb0da285-f23e-4b56-8b53-3ef5f9f6de66");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (reason != null)
        keyValuePairList.Add(nameof (reason), reason);
      if (reasonCode != null)
        keyValuePairList.Add(nameof (reasonCode), reasonCode);
      using (await managementHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<UserExtensionPolicy> GetPoliciesAsync(
      string userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UserExtensionPolicy>(new HttpMethod("GET"), new Guid("e5cc8c09-407b-4867-8319-2ae3338cbf6f"), (object) new
      {
        userId = userId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<int> ResolveRequestAsync(
      string rejectMessage,
      string publisherName,
      string extensionName,
      string requesterId,
      ExtensionRequestState state,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("aa93e1f3-511c-4364-8b9c-eb98818f2e0b");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        requesterId = requesterId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(rejectMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (state), state.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<int>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<RequestedExtension>> GetRequestsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<RequestedExtension>>(new HttpMethod("GET"), new Guid("216b978f-b164-424e-ada2-b77561e842b7"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<int> ResolveAllRequestsAsync(
      string rejectMessage,
      string publisherName,
      string extensionName,
      ExtensionRequestState state,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ba93e1f3-511c-4364-8b9c-eb98818f2e0b");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(rejectMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (state), state.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<int>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task DeleteRequestAsync(
      string publisherName,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("f5afca1e-a728-4294-aa2d-4af0173431b5"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<RequestedExtension> RequestExtensionAsync(
      string publisherName,
      string extensionName,
      string requestMessage,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f5afca1e-a728-4294-aa2d-4af0173431b5");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(requestMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<RequestedExtension>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<string> GetTokenAsync(object userState = null, CancellationToken cancellationToken = default (CancellationToken)) => this.SendAsync<string>(new HttpMethod("GET"), new Guid("3a2e24ed-1d6f-4cb2-9f3b-45a96bbfaf50"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
  }
}
