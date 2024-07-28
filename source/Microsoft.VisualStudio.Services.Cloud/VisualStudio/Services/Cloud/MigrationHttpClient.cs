// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.MigrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class MigrationHttpClient : VssHttpClientBase
  {
    public MigrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MigrationHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<DeploymentInformation> GetDeploymentInformationAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DeploymentInformation>(new HttpMethod("GET"), new Guid("ddbecdb7-24de-49fb-862d-5c6df20ddf2e"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task CopyNewTargetBlobsToSourceForRollbackAsync(
      CopyNewTargetBlobsToSourceForRollbackRequest copyNewTargetBlobsToSourceForRollbackRequest,
      Guid migrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MigrationHttpClient migrationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("63dadd8e-2ada-487f-97c2-04df66de9ab7");
      object obj1 = (object) new
      {
        migrationId = migrationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CopyNewTargetBlobsToSourceForRollbackRequest>(copyNewTargetBlobsToSourceForRollbackRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MigrationHttpClient migrationHttpClient2 = migrationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<MigrationJobInformation> GetMigrationJobInformationAsync(
      Guid migrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<MigrationJobInformation>(new HttpMethod("GET"), new Guid("eff73468-ce4d-4aca-bb85-7e0137375dd2"), (object) new
      {
        migrationId = migrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task StopMigrationJobsAsync(
      Guid migrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("eff73468-ce4d-4aca-bb85-7e0137375dd2"), (object) new
      {
        migrationId = migrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<SasTokenInfo> CreateSasTokenAsync(
      SasTokenInfo request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d6978331-6b7a-4c2a-905f-7797d31a92a0");
      HttpContent httpContent = (HttpContent) new ObjectContent<SasTokenInfo>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SasTokenInfo>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
