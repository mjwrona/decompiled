// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.GalleryHttpClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption;
using Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [ResourceArea("69D21C00-F135-441B-B5CE-3626378E0819")]
  [ClientCancellationTimeout(20)]
  [ClientCircuitBreakerSettings(10, 50)]
  [ClientSensitiveHeader("X-Market-AccountToken")]
  public class GalleryHttpClient : GalleryCompatHttpClientBase
  {
    public GalleryHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public GalleryHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public GalleryHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public GalleryHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public GalleryHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task ShareExtensionByIdAsync(
      Guid extensionId,
      string accountName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("1f19631b-a0b4-4a03-89c2-d79785d24360"), (object) new
      {
        extensionId = extensionId,
        accountName = accountName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task UnshareExtensionByIdAsync(
      Guid extensionId,
      string accountName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1f19631b-a0b4-4a03-89c2-d79785d24360"), (object) new
      {
        extensionId = extensionId,
        accountName = accountName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task ShareExtensionAsync(
      string publisherName,
      string extensionName,
      string accountName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("a1e66d8f-f5de-4d16-8309-91a4e015ee46"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        accountName = accountName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task UnshareExtensionAsync(
      string publisherName,
      string extensionName,
      string accountName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a1e66d8f-f5de-4d16-8309-91a4e015ee46"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        accountName = accountName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<AcquisitionOptions> GetAcquisitionOptionsAsync(
      string itemId,
      string installationTarget,
      bool? testCommerce = null,
      bool? isFreeOrTrialInstall = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9d0a0105-075e-4760-aa15-8bcf54d1bd7d");
      object routeValues = (object) new{ itemId = itemId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (installationTarget), installationTarget);
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
      return this.SendAsync<AcquisitionOptions>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ExtensionAcquisitionRequest> RequestAcquisitionAsync(
      ExtensionAcquisitionRequest acquisitionRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3adb1f2d-e328-446e-be73-9f6d98071c45");
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

    public async Task<Stream> GetAssetByNameAsync(
      string publisherName,
      string extensionName,
      string version,
      string assetType,
      string accountToken = null,
      bool? acceptDefault = null,
      string accountTokenHeader = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7529171f-a002-4180-93ba-685f358a0482");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version,
        assetType = assetType
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (accountToken != null)
        keyValuePairList1.Add(nameof (accountToken), accountToken);
      if (acceptDefault.HasValue)
        keyValuePairList1.Add(nameof (acceptDefault), acceptDefault.Value.ToString());
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(accountTokenHeader))
        keyValuePairList2.Add("X-Market-AccountToken", accountTokenHeader);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetAssetAsync(
      Guid extensionId,
      string version,
      string assetType,
      string accountToken = null,
      bool? acceptDefault = null,
      string accountTokenHeader = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5d545f3d-ef47-488b-8be3-f5ee1517856c");
      object routeValues = (object) new
      {
        extensionId = extensionId,
        version = version,
        assetType = assetType
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (accountToken != null)
        keyValuePairList1.Add(nameof (accountToken), accountToken);
      if (acceptDefault.HasValue)
        keyValuePairList1.Add(nameof (acceptDefault), acceptDefault.Value.ToString());
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(accountTokenHeader))
        keyValuePairList2.Add("X-Market-AccountToken", accountTokenHeader);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetAssetAuthenticatedAsync(
      string publisherName,
      string extensionName,
      string version,
      string assetType,
      string accountToken = null,
      string accountTokenHeader = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("506aff36-2622-4f70-8063-77cce6366d20");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version,
        assetType = assetType
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (accountToken != null)
        keyValuePairList1.Add(nameof (accountToken), accountToken);
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(accountTokenHeader))
        keyValuePairList2.Add("X-Market-AccountToken", accountTokenHeader);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<AzurePublisher> AssociateAzurePublisherAsync(
      string publisherName,
      string azurePublisherId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("efd202a6-9d87-4ebc-9229-d2b8ae2fdb6d");
      object routeValues = (object) new
      {
        publisherName = publisherName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (azurePublisherId), azurePublisherId);
      return this.SendAsync<AzurePublisher>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AzurePublisher> QueryAssociatedAzurePublisherAsync(
      string publisherName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AzurePublisher>(new HttpMethod("GET"), new Guid("efd202a6-9d87-4ebc-9229-d2b8ae2fdb6d"), (object) new
      {
        publisherName = publisherName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<string>> GetCategoriesAsync(
      string languages = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e0a5a71e-3ac3-43a0-ae7d-0bb5c3046a2a");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (languages != null)
        keyValuePairList.Add(nameof (languages), languages);
      return this.SendAsync<List<string>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<CategoriesResult> GetCategoryDetailsAsync(
      string categoryName,
      string languages = null,
      string product = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("75d3c04d-84d2-4973-acd2-22627587dabc");
      object routeValues = (object) new
      {
        categoryName = categoryName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (languages != null)
        keyValuePairList.Add(nameof (languages), languages);
      if (product != null)
        keyValuePairList.Add(nameof (product), product);
      return this.SendAsync<CategoriesResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProductCategory> GetCategoryTreeAsync(
      string product,
      string categoryId,
      int? lcid = null,
      string source = null,
      string productVersion = null,
      string skus = null,
      string subSkus = null,
      string productArchitecture = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1102bb42-82b0-4955-8d8a-435d6b4cedd3");
      object routeValues = (object) new
      {
        product = product,
        categoryId = categoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (lcid.HasValue)
        keyValuePairList.Add(nameof (lcid), lcid.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (source != null)
        keyValuePairList.Add(nameof (source), source);
      if (productVersion != null)
        keyValuePairList.Add(nameof (productVersion), productVersion);
      if (skus != null)
        keyValuePairList.Add(nameof (skus), skus);
      if (subSkus != null)
        keyValuePairList.Add(nameof (subSkus), subSkus);
      if (productArchitecture != null)
        keyValuePairList.Add(nameof (productArchitecture), productArchitecture);
      return this.SendAsync<ProductCategory>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProductCategoriesResult> GetRootCategoriesAsync(
      string product,
      int? lcid = null,
      string source = null,
      string productVersion = null,
      string skus = null,
      string subSkus = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("31fba831-35b2-46f6-a641-d05de5a877d8");
      object routeValues = (object) new{ product = product };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (lcid.HasValue)
        keyValuePairList.Add(nameof (lcid), lcid.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (source != null)
        keyValuePairList.Add(nameof (source), source);
      if (productVersion != null)
        keyValuePairList.Add(nameof (productVersion), productVersion);
      if (skus != null)
        keyValuePairList.Add(nameof (skus), skus);
      if (subSkus != null)
        keyValuePairList.Add(nameof (subSkus), subSkus);
      return this.SendAsync<ProductCategoriesResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetCertificateAsync(
      string publisherName,
      string extensionName,
      string version = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e905ad6a-3f1f-4d08-9f6d-7d357ff8b7d0");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetContentVerificationLogAsync(
      string publisherName,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c0f1c7c4-3557-4ffb-b774-1e48c4865e99");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task CreateSupportRequestAsync(
      CustomerSupportRequest customerSupportRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8eded385-026a-4c15-b810-b8eb402771f1");
      HttpContent httpContent = (HttpContent) new ObjectContent<CustomerSupportRequest>(customerSupportRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GalleryHttpClient galleryHttpClient2 = galleryHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await galleryHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<ExtensionDraft> CreateDraftForEditExtensionAsync(
      string publisherName,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ExtensionDraft>(new HttpMethod("POST"), new Guid("02b33873-4e61-496e-83a2-59d1df46b7d8"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ExtensionDraft> PerformEditExtensionDraftOperationAsync(
      ExtensionDraftPatch draftPatch,
      string publisherName,
      string extensionName,
      Guid draftId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("02b33873-4e61-496e-83a2-59d1df46b7d8");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        draftId = draftId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionDraftPatch>(draftPatch, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionDraft>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ExtensionDraft> UpdatePayloadInDraftForEditExtensionAsync(
      Stream uploadStream,
      string publisherName,
      string extensionName,
      Guid draftId,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("02b33873-4e61-496e-83a2-59d1df46b7d8");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        draftId = draftId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(fileName))
        collection.Add("X-Market-UploadFileName", fileName);
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionDraft>(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ExtensionDraftAsset> AddAssetForEditExtensionDraftAsync(
      Stream uploadStream,
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("f1db9c47-6619-4998-a7e5-d7f9f41a4617");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        draftId = draftId,
        assetType = assetType
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionDraftAsset>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ExtensionDraft> CreateDraftForNewExtensionAsync(
      Stream uploadStream,
      string publisherName,
      string product,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b3ab127d-ebb9-4d22-b611-4e09593c8d79");
      object obj1 = (object) new
      {
        publisherName = publisherName
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(product))
        collection.Add("X-Market-UploadFileProduct", product);
      if (!string.IsNullOrEmpty(fileName))
        collection.Add("X-Market-UploadFileName", fileName);
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionDraft>(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ExtensionDraft> PerformNewExtensionDraftOperationAsync(
      ExtensionDraftPatch draftPatch,
      string publisherName,
      Guid draftId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b3ab127d-ebb9-4d22-b611-4e09593c8d79");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        draftId = draftId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionDraftPatch>(draftPatch, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionDraft>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ExtensionDraft> UpdatePayloadInDraftForNewExtensionAsync(
      Stream uploadStream,
      string publisherName,
      Guid draftId,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("b3ab127d-ebb9-4d22-b611-4e09593c8d79");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        draftId = draftId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(fileName))
        collection.Add("X-Market-UploadFileName", fileName);
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionDraft>(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ExtensionDraftAsset> AddAssetForNewExtensionDraftAsync(
      Stream uploadStream,
      string publisherName,
      Guid draftId,
      string assetType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("88c0b1c8-b4f1-498a-9b2a-8446ef9f32e7");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        draftId = draftId,
        assetType = assetType
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionDraftAsset>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task<Stream> GetAssetFromEditExtensionDraftAsync(
      string publisherName,
      Guid draftId,
      string assetType,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("88c0b1c8-b4f1-498a-9b2a-8446ef9f32e7");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        draftId = draftId,
        assetType = assetType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (extensionName), extensionName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetAssetFromNewExtensionDraftAsync(
      string publisherName,
      Guid draftId,
      string assetType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("88c0b1c8-b4f1-498a-9b2a-8446ef9f32e7");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        draftId = draftId,
        assetType = assetType
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<ExtensionEvents> GetExtensionEventsAsync(
      string publisherName,
      string extensionName,
      int? count = null,
      DateTime? afterDate = null,
      string include = null,
      string includeProperty = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3d13c499-2168-4d06-bef4-14aba185dcd5");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (count.HasValue)
        keyValuePairList.Add(nameof (count), count.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (afterDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (afterDate), afterDate.Value);
      if (include != null)
        keyValuePairList.Add(nameof (include), include);
      if (includeProperty != null)
        keyValuePairList.Add(nameof (includeProperty), includeProperty);
      return this.SendAsync<ExtensionEvents>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task PublishExtensionEventsAsync(
      IEnumerable<ExtensionEvents> extensionEvents,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0bf2bd3a-70e0-4d5d-8bf7-bd4a9c2ab6e7");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ExtensionEvents>>(extensionEvents, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GalleryHttpClient galleryHttpClient2 = galleryHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await galleryHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<ExtensionQueryResult> QueryExtensionsAsync(
      ExtensionQuery extensionQuery,
      string accountToken = null,
      string accountTokenHeader = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eb9d5ee1-6d43-456b-b80e-8a96fbc014b6");
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionQuery>(extensionQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (accountToken != null)
        collection1.Add(nameof (accountToken), accountToken);
      List<KeyValuePair<string, string>> collection2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(accountTokenHeader))
        collection2.Add("X-Market-AccountToken", accountTokenHeader);
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection2;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionQueryResult>(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<PublishedExtension> CreateExtensionAsync(
      Stream uploadStream,
      string extensionType = null,
      string reCaptchaToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a41192c8-9525-4b58-bc86-179fa549d80d");
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (extensionType != null)
        collection.Add(nameof (extensionType), extensionType);
      if (reCaptchaToken != null)
        collection.Add(nameof (reCaptchaToken), reCaptchaToken);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PublishedExtension>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteExtensionByIdAsync(
      Guid extensionId,
      string version = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a41192c8-9525-4b58-bc86-179fa549d80d");
      object routeValues = (object) new
      {
        extensionId = extensionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (version != null)
        keyValuePairList.Add(nameof (version), version);
      using (await galleryHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PublishedExtension> GetExtensionByIdAsync(
      Guid extensionId,
      string version = null,
      ExtensionQueryFlags? flags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a41192c8-9525-4b58-bc86-179fa549d80d");
      object routeValues = (object) new
      {
        extensionId = extensionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (version != null)
        keyValuePairList.Add(nameof (version), version);
      if (flags.HasValue)
        keyValuePairList.Add(nameof (flags), flags.Value.ToString());
      return this.SendAsync<PublishedExtension>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PublishedExtension> UpdateExtensionByIdAsync(
      Guid extensionId,
      string reCaptchaToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("a41192c8-9525-4b58-bc86-179fa549d80d");
      object routeValues = (object) new
      {
        extensionId = extensionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (reCaptchaToken != null)
        keyValuePairList.Add(nameof (reCaptchaToken), reCaptchaToken);
      return this.SendAsync<PublishedExtension>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PublishedExtension> CreateExtensionWithPublisherAsync(
      Stream uploadStream,
      string publisherName,
      string extensionType = null,
      string reCaptchaToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e11ea35a-16fe-4b80-ab11-c4cab88a0966");
      object obj1 = (object) new
      {
        publisherName = publisherName
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (extensionType != null)
        collection.Add(nameof (extensionType), extensionType);
      if (reCaptchaToken != null)
        collection.Add(nameof (reCaptchaToken), reCaptchaToken);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PublishedExtension>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public async Task DeleteExtensionAsync(
      string publisherName,
      string extensionName,
      string version = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("e11ea35a-16fe-4b80-ab11-c4cab88a0966");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (version != null)
        keyValuePairList.Add(nameof (version), version);
      using (await galleryHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PublishedExtension> GetExtensionAsync(
      string publisherName,
      string extensionName,
      string version = null,
      ExtensionQueryFlags? flags = null,
      string accountToken = null,
      string accountTokenHeader = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e11ea35a-16fe-4b80-ab11-c4cab88a0966");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (version != null)
        keyValuePairList1.Add(nameof (version), version);
      if (flags.HasValue)
        keyValuePairList1.Add(nameof (flags), flags.Value.ToString());
      if (accountToken != null)
        keyValuePairList1.Add(nameof (accountToken), accountToken);
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(accountTokenHeader))
        keyValuePairList2.Add("X-Market-AccountToken", accountTokenHeader);
      return this.SendAsync<PublishedExtension>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PublishedExtension> UpdateExtensionAsync(
      Stream uploadStream,
      string publisherName,
      string extensionName,
      string extensionType = null,
      string reCaptchaToken = null,
      bool? bypassScopeCheck = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("e11ea35a-16fe-4b80-ab11-c4cab88a0966");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (extensionType != null)
        collection.Add(nameof (extensionType), extensionType);
      if (reCaptchaToken != null)
        collection.Add(nameof (reCaptchaToken), reCaptchaToken);
      if (bypassScopeCheck.HasValue)
        collection.Add(nameof (bypassScopeCheck), bypassScopeCheck.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PublishedExtension>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<PublishedExtension> UpdateExtensionPropertiesAsync(
      string publisherName,
      string extensionName,
      PublishedExtensionFlags flags,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("e11ea35a-16fe-4b80-ab11-c4cab88a0966");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (flags), flags.ToString());
      return this.SendAsync<PublishedExtension>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task ShareExtensionWithHostAsync(
      string publisherName,
      string extensionName,
      string hostType,
      string hostName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("328a3af8-d124-46e9-9483-01690cd415b9"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        hostType = hostType,
        hostName = hostName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task UnshareExtensionWithHostAsync(
      string publisherName,
      string extensionName,
      string hostType,
      string hostName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("328a3af8-d124-46e9-9483-01690cd415b9"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        hostType = hostType,
        hostName = hostName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task ExtensionValidatorAsync(
      AzureRestApiRequestModel azureRestApiRequestModel,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("05e8a5e1-8c59-4c2c-8856-0ff087d1a844");
      HttpContent httpContent = (HttpContent) new ObjectContent<AzureRestApiRequestModel>(azureRestApiRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GalleryHttpClient galleryHttpClient2 = galleryHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await galleryHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SendNotificationsAsync(
      NotificationsData notificationData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eab39817-413c-4602-a49f-07ad00844980");
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationsData>(notificationData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GalleryHttpClient galleryHttpClient2 = galleryHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await galleryHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task<Stream> GetPackageAsync(
      string publisherName,
      string extensionName,
      string version,
      string accountToken = null,
      bool? acceptDefault = null,
      string accountTokenHeader = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7cb576f8-1cae-4c4b-b7b1-e4af5759e965");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (accountToken != null)
        keyValuePairList1.Add(nameof (accountToken), accountToken);
      if (acceptDefault.HasValue)
        keyValuePairList1.Add(nameof (acceptDefault), acceptDefault.Value.ToString());
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(accountTokenHeader))
        keyValuePairList2.Add("X-Market-AccountToken", accountTokenHeader);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetAssetWithTokenAsync(
      string publisherName,
      string extensionName,
      string version,
      string assetType,
      string assetToken = null,
      string accountToken = null,
      bool? acceptDefault = null,
      string accountTokenHeader = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364415a1-0077-4a41-a7a0-06edd4497492");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version,
        assetType = assetType,
        assetToken = assetToken
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (accountToken != null)
        keyValuePairList1.Add(nameof (accountToken), accountToken);
      if (acceptDefault.HasValue)
        keyValuePairList1.Add(nameof (acceptDefault), acceptDefault.Value.ToString());
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(accountTokenHeader))
        keyValuePairList2.Add("X-Market-AccountToken", accountTokenHeader);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task DeletePublisherAssetAsync(
      string publisherName,
      string assetType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("21143299-34f9-4c62-8ca8-53da691192f9");
      object routeValues = (object) new
      {
        publisherName = publisherName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (assetType != null)
        keyValuePairList.Add(nameof (assetType), assetType);
      using (await galleryHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task<Stream> GetPublisherAssetAsync(
      string publisherName,
      string assetType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("21143299-34f9-4c62-8ca8-53da691192f9");
      object routeValues = (object) new
      {
        publisherName = publisherName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (assetType != null)
        keyValuePairList.Add(nameof (assetType), assetType);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<IReadOnlyDictionary<string, string>> UpdatePublisherAssetAsync(
      Stream uploadStream,
      string publisherName,
      string assetType = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("21143299-34f9-4c62-8ca8-53da691192f9");
      object obj1 = (object) new
      {
        publisherName = publisherName
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (assetType != null)
        collection1.Add(nameof (assetType), assetType);
      List<KeyValuePair<string, string>> collection2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(fileName))
        collection2.Add("X-Market-UploadFileName", fileName);
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection2;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IReadOnlyDictionary<string, string>>(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<string> FetchDomainTokenAsync(
      string publisherName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("GET"), new Guid("67a609ef-fa74-4b52-8664-78d76f7b3634"), (object) new
      {
        publisherName = publisherName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task VerifyDomainTokenAsync(
      string publisherName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("PUT"), new Guid("67a609ef-fa74-4b52-8664-78d76f7b3634"), (object) new
      {
        publisherName = publisherName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PublisherQueryResult> QueryPublishersAsync(
      PublisherQuery publisherQuery,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2ad6ee0a-b53f-4034-9d1d-d009fda1212e");
      HttpContent httpContent = (HttpContent) new ObjectContent<PublisherQuery>(publisherQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PublisherQueryResult>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Publisher> CreatePublisherAsync(
      Publisher publisher,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4ddec66a-e4f6-4f5d-999e-9e77710d7ff4");
      HttpContent httpContent = (HttpContent) new ObjectContent<Publisher>(publisher, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Publisher>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeletePublisherAsync(
      string publisherName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4ddec66a-e4f6-4f5d-999e-9e77710d7ff4"), (object) new
      {
        publisherName = publisherName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Publisher> GetPublisherAsync(
      string publisherName,
      int? flags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4ddec66a-e4f6-4f5d-999e-9e77710d7ff4");
      object routeValues = (object) new
      {
        publisherName = publisherName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (flags.HasValue)
        keyValuePairList.Add(nameof (flags), flags.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<Publisher>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Publisher> UpdatePublisherAsync(
      Publisher publisher,
      string publisherName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4ddec66a-e4f6-4f5d-999e-9e77710d7ff4");
      object obj1 = (object) new
      {
        publisherName = publisherName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Publisher>(publisher, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Publisher>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<PublisherRoleAssignment>> UpdatePublisherMembersAsync(
      IEnumerable<PublisherUserRoleAssignmentRef> roleAssignments,
      string publisherName,
      bool? limitToCallerIdentityDomain = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4ddec66a-e4f6-4f5d-999e-9e77710d7ff4");
      object obj1 = (object) new
      {
        publisherName = publisherName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<PublisherUserRoleAssignmentRef>>(roleAssignments, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (limitToCallerIdentityDomain.HasValue)
        collection.Add(nameof (limitToCallerIdentityDomain), limitToCallerIdentityDomain.Value.ToString());
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
      return this.SendAsync<List<PublisherRoleAssignment>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<PublishedExtension> PublishExtensionWithPublisherSignatureAsync(
      Stream uploadStream,
      string publisherName,
      string extensionName,
      string extensionType = null,
      string reCaptchaToken = null,
      bool? bypassScopeCheck = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("e11ea35a-16fe-4b80-ab11-c4cab88a0969");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/related");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (extensionType != null)
        collection.Add(nameof (extensionType), extensionType);
      if (reCaptchaToken != null)
        collection.Add(nameof (reCaptchaToken), reCaptchaToken);
      if (bypassScopeCheck.HasValue)
        collection.Add(nameof (bypassScopeCheck), bypassScopeCheck.Value.ToString());
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
      return this.SendAsync<PublishedExtension>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<Publisher> GetPublisherWithoutTokenAsync(
      string publisherName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Publisher>(new HttpMethod("GET"), new Guid("215a2ed8-458a-4850-ad5a-45f1dabc3461"), (object) new
      {
        publisherName = publisherName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<QuestionsResult> GetQuestionsAsync(
      string publisherName,
      string extensionName,
      int? count = null,
      int? page = null,
      DateTime? afterDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c010d03d-812c-4ade-ae07-c1862475eda5");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (count.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = count.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (count), str);
      }
      if (page.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = page.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (page), str);
      }
      if (afterDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (afterDate), afterDate.Value);
      return this.SendAsync<QuestionsResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Concern> ReportQuestionAsync(
      Concern concern,
      string pubName,
      string extName,
      long questionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("784910cd-254a-494d-898b-0728549b2f10");
      object obj1 = (object) new
      {
        pubName = pubName,
        extName = extName,
        questionId = questionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Concern>(concern, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Concern>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Question> CreateQuestionAsync(
      Question question,
      string publisherName,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6d1d9741-eca8-4701-a3a5-235afc82dfa4");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Question>(question, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Question>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteQuestionAsync(
      string publisherName,
      string extensionName,
      long questionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6d1d9741-eca8-4701-a3a5-235afc82dfa4"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        questionId = questionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Question> UpdateQuestionAsync(
      Question question,
      string publisherName,
      string extensionName,
      long questionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6d1d9741-eca8-4701-a3a5-235afc82dfa4");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        questionId = questionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Question>(question, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Question>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Response> CreateResponseAsync(
      Response response,
      string publisherName,
      string extensionName,
      long questionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7f8ae5e0-46b0-438f-b2e8-13e8513517bd");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        questionId = questionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Response>(response, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Response>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteResponseAsync(
      string publisherName,
      string extensionName,
      long questionId,
      long responseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("7f8ae5e0-46b0-438f-b2e8-13e8513517bd"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        questionId = questionId,
        responseId = responseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Response> UpdateResponseAsync(
      Response response,
      string publisherName,
      string extensionName,
      long questionId,
      long responseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7f8ae5e0-46b0-438f-b2e8-13e8513517bd");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        questionId = questionId,
        responseId = responseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Response>(response, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Response>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Stream> GetExtensionReportsAsync(
      string publisherName,
      string extensionName,
      int? days = null,
      int? count = null,
      DateTime? afterDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("79e0c74f-157f-437e-845f-74fbb4121d4c");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (days.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = days.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (days), str);
      }
      if (count.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = count.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (count), str);
      }
      if (afterDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (afterDate), afterDate.Value);
      return this.SendAsync<Stream>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ReviewsResult> GetReviewsAsync(
      string publisherName,
      string extensionName,
      int? count = null,
      ReviewFilterOptions? filterOptions = null,
      DateTime? beforeDate = null,
      DateTime? afterDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5b3f819f-f247-42ad-8c00-dd9ab9ab246d");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (count.HasValue)
        keyValuePairList.Add(nameof (count), count.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (filterOptions.HasValue)
        keyValuePairList.Add(nameof (filterOptions), filterOptions.Value.ToString());
      if (beforeDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (beforeDate), beforeDate.Value);
      if (afterDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (afterDate), afterDate.Value);
      return this.SendAsync<ReviewsResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ReviewSummary> GetReviewsSummaryAsync(
      string pubName,
      string extName,
      DateTime? beforeDate = null,
      DateTime? afterDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b7b44e21-209e-48f0-ae78-04727fc37d77");
      object routeValues = (object) new
      {
        pubName = pubName,
        extName = extName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (beforeDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (beforeDate), beforeDate.Value);
      if (afterDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (afterDate), afterDate.Value);
      return this.SendAsync<ReviewSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Review> CreateReviewAsync(
      Review review,
      string pubName,
      string extName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e6e85b9d-aa70-40e6-aa28-d0fbf40b91a3");
      object obj1 = (object) new
      {
        pubName = pubName,
        extName = extName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Review>(review, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Review>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteReviewAsync(
      string pubName,
      string extName,
      long reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e6e85b9d-aa70-40e6-aa28-d0fbf40b91a3"), (object) new
      {
        pubName = pubName,
        extName = extName,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ReviewPatch> UpdateReviewAsync(
      ReviewPatch reviewPatch,
      string pubName,
      string extName,
      long reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e6e85b9d-aa70-40e6-aa28-d0fbf40b91a3");
      object obj1 = (object) new
      {
        pubName = pubName,
        extName = extName,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReviewPatch>(reviewPatch, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReviewPatch>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ExtensionCategory> CreateCategoryAsync(
      ExtensionCategory category,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("476531a3-7024-4516-a76a-ed64d3008ad6");
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionCategory>(category, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionCategory>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Dictionary<string, object>> GetGalleryUserSettingsAsync(
      string userScope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Dictionary<string, object>>(new HttpMethod("GET"), new Guid("9b75ece3-7960-401c-848b-148ac01ca350"), (object) new
      {
        userScope = userScope
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Dictionary<string, object>> GetGalleryUserSettingsAsync(
      string userScope,
      string key,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Dictionary<string, object>>(new HttpMethod("GET"), new Guid("9b75ece3-7960-401c-848b-148ac01ca350"), (object) new
      {
        userScope = userScope,
        key = key
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task SetGalleryUserSettingsAsync(
      IDictionary<string, object> entries,
      string userScope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9b75ece3-7960-401c-848b-148ac01ca350");
      object obj1 = (object) new{ userScope = userScope };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, object>>(entries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GalleryHttpClient galleryHttpClient2 = galleryHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await galleryHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task GenerateKeyAsync(
      string keyType,
      int? expireCurrentSeconds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("92ed5cf4-c38b-465a-9059-2f2fb7c624b5");
      object routeValues = (object) new{ keyType = keyType };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expireCurrentSeconds.HasValue)
        keyValuePairList.Add(nameof (expireCurrentSeconds), expireCurrentSeconds.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await galleryHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<string> GetSigningKeyAsync(
      string keyType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("GET"), new Guid("92ed5cf4-c38b-465a-9059-2f2fb7c624b5"), (object) new
      {
        keyType = keyType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task UpdateExtensionStatisticsAsync(
      ExtensionStatisticUpdate extensionStatisticsUpdate,
      string publisherName,
      string extensionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a0ea3204-11e9-422d-a9ca-45851cc41400");
      object obj1 = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionStatisticUpdate>(extensionStatisticsUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GalleryHttpClient galleryHttpClient2 = galleryHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await galleryHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<ExtensionDailyStats> GetExtensionDailyStatsAsync(
      string publisherName,
      string extensionName,
      int? days = null,
      ExtensionStatsAggregateType? aggregate = null,
      DateTime? afterDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ae06047e-51c5-4fb4-ab65-7be488544416");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (days.HasValue)
        keyValuePairList.Add(nameof (days), days.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (aggregate.HasValue)
        keyValuePairList.Add(nameof (aggregate), aggregate.Value.ToString());
      if (afterDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (afterDate), afterDate.Value);
      return this.SendAsync<ExtensionDailyStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ExtensionDailyStats> GetExtensionDailyStatsAnonymousAsync(
      string publisherName,
      string extensionName,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ExtensionDailyStats>(new HttpMethod("GET"), new Guid("4fa7adb6-ca65-4075-a232-5f28323288ea"), (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task IncrementExtensionDailyStatAsync(
      string publisherName,
      string extensionName,
      string version,
      string statType,
      string targetPlatform = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("4fa7adb6-ca65-4075-a232-5f28323288ea");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (statType), statType);
      if (targetPlatform != null)
        keyValuePairList.Add(nameof (targetPlatform), targetPlatform);
      using (await galleryHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task<Stream> GetVerificationLogAsync(
      string publisherName,
      string extensionName,
      string version,
      string targetPlatform = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c5523abe-b843-437f-875b-5833064efe4d");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (targetPlatform != null)
        keyValuePairList.Add(nameof (targetPlatform), targetPlatform);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await galleryHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await galleryHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task UpdateVSCodeWebExtensionStatisticsAsync(
      string itemName,
      string version,
      VSCodeWebExtensionStatisicsType statType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("205c91a8-7841-4fd3-ae4f-5a745d5a8df5"), (object) new
      {
        itemName = itemName,
        version = version,
        statType = statType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
