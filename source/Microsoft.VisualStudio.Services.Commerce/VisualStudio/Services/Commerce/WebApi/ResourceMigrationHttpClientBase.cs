// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.ResourceMigrationHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ResourceArea("FFCFC36A-0BE8-412A-A2BB-93C2ABD4048B")]
  public abstract class ResourceMigrationHttpClientBase : VssHttpClientBase
  {
    public ResourceMigrationHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ResourceMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ResourceMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ResourceMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ResourceMigrationHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    internal virtual async Task CreateAzureSubscriptionForDualWritesAsync(
      ResourceMigrationRequest migrationRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("00432895-b3f6-488c-ba71-792fa5e07383");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourceMigrationRequest>(migrationRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ResourceMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    internal virtual async Task CreateOrDeleteAzureResourceAccountsAsync(
      ResourceMigrationRequest migrationRequest,
      bool createResources,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("00432895-b3f6-488c-ba71-792fa5e07383");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourceMigrationRequest>(migrationRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (createResources), createResources.ToString());
      ResourceMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
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
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    internal virtual async Task CreateSubscriptionResourcesAsync(
      ResourceMigrationRequest migrationRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("00432895-b3f6-488c-ba71-792fa5e07383");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourceMigrationRequest>(migrationRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ResourceMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    internal virtual async Task MigrateResourcesAsync(
      ResourceMigrationRequest migrationRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("00432895-b3f6-488c-ba71-792fa5e07383");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourceMigrationRequest>(migrationRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ResourceMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
