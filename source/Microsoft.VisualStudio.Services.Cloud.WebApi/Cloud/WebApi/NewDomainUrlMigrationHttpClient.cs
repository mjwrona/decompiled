// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.NewDomainUrlMigrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [ResourceArea("{48D0B689-8F32-444A-BDA1-6780E34ACA8B}")]
  public class NewDomainUrlMigrationHttpClient : VssHttpClientBase
  {
    public NewDomainUrlMigrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NewDomainUrlMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NewDomainUrlMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NewDomainUrlMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NewDomainUrlMigrationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task CancelRequestAsync(
      Guid requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("91cc4dd2-7aad-4182-bb39-940717b86890"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ServicingOrchestrationRequestStatus> GetRequestStatusAsync(
      Guid requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ServicingOrchestrationRequestStatus>(new HttpMethod("GET"), new Guid("91cc4dd2-7aad-4182-bb39-940717b86890"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task QueueRequestAsync(
      FrameworkNewDomainUrlMigrationRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NewDomainUrlMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("91cc4dd2-7aad-4182-bb39-940717b86890");
      HttpContent httpContent = (HttpContent) new ObjectContent<FrameworkNewDomainUrlMigrationRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NewDomainUrlMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
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

    public async Task ValidateRequestAsync(
      FrameworkNewDomainUrlMigrationRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NewDomainUrlMigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("91cc4dd2-7aad-4182-bb39-940717b86890");
      HttpContent httpContent = (HttpContent) new ObjectContent<FrameworkNewDomainUrlMigrationRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NewDomainUrlMigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
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
