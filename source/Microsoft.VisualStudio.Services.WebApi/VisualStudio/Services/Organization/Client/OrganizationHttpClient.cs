// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Organization.Client
{
  [ResourceArea("0D55247A-1C47-4462-9B1F-5E2125590EE6")]
  public class OrganizationHttpClient : OrganizationCompatHttpClientBase
  {
    public OrganizationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OrganizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OrganizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OrganizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OrganizationHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task DeleteCollectionAvatarAsync(
      Guid collectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("c8dee5f9-f31f-4b10-918c-01ab73892f64"), (object) new
      {
        collectionId = collectionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateCollectionAvatarAsync(
      Guid collectionId,
      byte[] imageData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OrganizationHttpClient organizationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c8dee5f9-f31f-4b10-918c-01ab73892f64");
      object obj1 = (object) new
      {
        collectionId = collectionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<byte[]>(imageData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      OrganizationHttpClient organizationHttpClient2 = organizationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await organizationHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<bool> UpdateCollectionPropertiesAsync(
      Guid collectionId,
      JsonPatchDocument patchDocument,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a0f9c508-a3c4-456b-a812-3fb0c4743521");
      object obj1 = (object) new
      {
        collectionId = collectionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<bool>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Collection> BackfillPreferredGeographyAsync(
      Guid collectionId,
      string geographyName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      object routeValues = (object) new
      {
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (geographyName), geographyName);
      return this.SendAsync<Collection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Collection> CreateCollectionAsync(
      Collection resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      HttpContent httpContent = (HttpContent) new ObjectContent<Collection>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Collection>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<bool> DeleteCollectionAsync(
      Guid collectionId,
      int? gracePeriodToRestoreInHours = null,
      bool? violatedTerms = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      object routeValues = (object) new
      {
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (gracePeriodToRestoreInHours.HasValue)
        keyValuePairList.Add(nameof (gracePeriodToRestoreInHours), gracePeriodToRestoreInHours.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (violatedTerms.HasValue)
        keyValuePairList.Add(nameof (violatedTerms), violatedTerms.Value.ToString());
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Collection> ForceUpdateCollectionOwnerAsync(
      string forceUpdateReason,
      Guid collectionId,
      Guid newOwner,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      object obj1 = (object) new
      {
        collectionId = collectionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(forceUpdateReason, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (newOwner), newOwner.ToString());
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
      return this.SendAsync<Collection>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<Collection> GetCollectionAsync(
      string collectionId,
      IEnumerable<string> propertyNames = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      object routeValues = (object) new
      {
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (propertyNames != null && propertyNames.Any<string>())
        keyValuePairList.Add(nameof (propertyNames), string.Join(",", propertyNames));
      return this.SendAsync<Collection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Collection>> GetCollectionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Collection>>(new HttpMethod("GET"), new Guid("668b5607-0db2-49bb-83f8-5f46f1094250"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Collection>> GetCollectionsAsync(
      CollectionSearchKind searchKind,
      string searchValue,
      bool? includeDeletedCollections = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (searchKind), searchKind.ToString());
      keyValuePairList.Add(nameof (searchValue), searchValue);
      if (includeDeletedCollections.HasValue)
        keyValuePairList.Add(nameof (includeDeletedCollections), includeDeletedCollections.Value.ToString());
      return this.SendAsync<List<Collection>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> IsEligibleForTakeOverAsync(
      Guid collectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<bool>(new HttpMethod("POST"), new Guid("668b5607-0db2-49bb-83f8-5f46f1094250"), (object) new
      {
        collectionId = collectionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> RestoreCollectionAsync(
      Guid collectionId,
      string collectionName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      object routeValues = (object) new
      {
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (collectionName), collectionName);
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Collection> UpdateCollectionAsync(
      JsonPatchDocument patchDocument,
      string collectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("668b5607-0db2-49bb-83f8-5f46f1094250");
      object obj1 = (object) new
      {
        collectionId = collectionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Collection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<Geography>> GetGeographiesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Geography>>(new HttpMethod("GET"), new Guid("cb720f78-c6e5-4c09-acff-4ed2ef748f72"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> UpdateOrganizationLogoAsync(
      Guid organizationId,
      Logo logo,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9eeec19-85b4-40ae-8a52-b4f697260ac4");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Logo>(logo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<bool>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<OrganizationMigrationBlob> ExportOrganizationMigrationBlobAsync(
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OrganizationMigrationBlob>(new HttpMethod("GET"), new Guid("93f69239-28ba-497e-b4d4-33e51e6303c3"), (object) new
      {
        organizationId = organizationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task ImportOrganizationMigrationBlobAsync(
      OrganizationMigrationBlob migrationBlob,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OrganizationHttpClient organizationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("93f69239-28ba-497e-b4d4-33e51e6303c3");
      HttpContent httpContent = (HttpContent) new ObjectContent<OrganizationMigrationBlob>(migrationBlob, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      OrganizationHttpClient organizationHttpClient2 = organizationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await organizationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<bool> UpdateOrganizationPropertiesAsync(
      Guid organizationId,
      JsonPatchDocument patchDocument,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("103707c6-236d-4434-a0a2-9031fbb65fa6");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<bool>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Organization.Client.Organization> CreateOrganizationAsync(
      Microsoft.VisualStudio.Services.Organization.Client.Organization resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("95f49097-6cdc-4afe-a039-48b4d4c4cbf7");
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Organization.Client.Organization>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Organization.Client.Organization>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Organization.Client.Organization> GetOldestOrganizationByTenantAsync(
      Guid tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("95f49097-6cdc-4afe-a039-48b4d4c4cbf7");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (tenantId), tenantId.ToString());
      return this.SendAsync<Microsoft.VisualStudio.Services.Organization.Client.Organization>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Organization.Client.Organization> GetOrganizationAsync(
      string organizationId,
      IEnumerable<string> propertyNames = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("95f49097-6cdc-4afe-a039-48b4d4c4cbf7");
      object routeValues = (object) new
      {
        organizationId = organizationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (propertyNames != null && propertyNames.Any<string>())
        keyValuePairList.Add(nameof (propertyNames), string.Join(",", propertyNames));
      return this.SendAsync<Microsoft.VisualStudio.Services.Organization.Client.Organization>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Organization.Client.Organization>> GetOrganizationsAsync(
      OrganizationSearchKind searchKind,
      string searchValue,
      bool? isActivated = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("95f49097-6cdc-4afe-a039-48b4d4c4cbf7");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (searchKind), searchKind.ToString());
      keyValuePairList.Add(nameof (searchValue), searchValue);
      if (isActivated.HasValue)
        keyValuePairList.Add(nameof (isActivated), isActivated.Value.ToString());
      return this.SendAsync<List<Microsoft.VisualStudio.Services.Organization.Client.Organization>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Organization.Client.Organization> UpdateOrganizationAsync(
      JsonPatchDocument patchDocument,
      string organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("95f49097-6cdc-4afe-a039-48b4d4c4cbf7");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Organization.Client.Organization>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<Region>> GetRegionsAsync(
      bool? includeRegionsWithNoAvailableHosts = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f84936f-1801-46f6-94fa-1817545d366d");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeRegionsWithNoAvailableHosts.HasValue)
        keyValuePairList.Add(nameof (includeRegionsWithNoAvailableHosts), includeRegionsWithNoAvailableHosts.Value.ToString());
      return this.SendAsync<List<Region>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
