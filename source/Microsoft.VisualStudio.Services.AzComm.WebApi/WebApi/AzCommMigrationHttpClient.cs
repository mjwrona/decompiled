// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.WebApi.AzCommMigrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.AzComm.WebApi
{
  [ResourceArea("89B27BCD-F2BA-4306-845C-345136711714")]
  public class AzCommMigrationHttpClient : VssHttpClientBase
  {
    public AzCommMigrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AzCommMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AzCommMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AzCommMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AzCommMigrationHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task DualWriteDefaultAccessLevelResourcesAsync(
      DualWriteDefaultAccessLevelRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("29fe3989-e0a2-4d13-89ef-1c94bb4bb7c9");
      HttpContent httpContent = (HttpContent) new ObjectContent<DualWriteDefaultAccessLevelRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task MigrateResourcesAsync(
      DataMigrationRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4c90d984-4108-452a-83c7-327eff54c01e");
      HttpContent httpContent = (HttpContent) new ObjectContent<DataMigrationRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task ClearStaleOrganizationAsync(
      StaleOrganizationRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("55a7d679-1854-4de9-bc5f-3c7456c70fc5");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      migrationHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (request), (object) request);
      using (await migrationHttpClient.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DualWriteOrgMeterResourcesAsync(
      DualWriteSRURequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("55a7d679-1854-4de9-bc5f-3c7456c70fc5");
      HttpContent httpContent = (HttpContent) new ObjectContent<DualWriteSRURequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DualWriteOrgResourcesAsync(
      DualWriteARARequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("55a7d679-1854-4de9-bc5f-3c7456c70fc5");
      HttpContent httpContent = (HttpContent) new ObjectContent<DualWriteARARequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetStaleOrganizationAsync(
      StaleOrganizationRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("55a7d679-1854-4de9-bc5f-3c7456c70fc5");
      HttpContent httpContent = (HttpContent) new ObjectContent<StaleOrganizationRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DualWriteOrgTagsResourcesAsync(
      DualWriteTagsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("336bc59e-17e6-47dc-870a-ef28042b4b50");
      HttpContent httpContent = (HttpContent) new ObjectContent<DualWriteTagsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DualWriteSubResourcesAsync(
      DualWriteSubscriptionRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dfa5a7b3-635f-4327-ab40-d7a4acc6f324");
      HttpContent httpContent = (HttpContent) new ObjectContent<DualWriteSubscriptionRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
