// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.UserInlineCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class UserInlineCore : UserCore
  {
    internal UserInlineCore(
      CosmosClientContext clientContext,
      DatabaseInternal database,
      string userId)
      : base(clientContext, database, userId)
    {
    }

    public override Task<UserResponse> ReadAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserResponse>(nameof (ReadAsync), requestOptions, (Func<ITrace, Task<UserResponse>>) (trace => this.ReadAsync(requestOptions, trace, cancellationToken)), (Func<UserResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserProperties>((Response<UserProperties>) response)));
    }

    public override Task<UserResponse> ReplaceAsync(
      UserProperties userProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserResponse>(nameof (ReplaceAsync), requestOptions, (Func<ITrace, Task<UserResponse>>) (trace => this.ReplaceAsync(userProperties, requestOptions, trace, cancellationToken)), (Func<UserResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserProperties>((Response<UserProperties>) response)));
    }

    public override Task<UserResponse> DeleteAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserResponse>(nameof (DeleteAsync), requestOptions, (Func<ITrace, Task<UserResponse>>) (trace => this.DeleteAsync(requestOptions, trace, cancellationToken)), (Func<UserResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserProperties>((Response<UserProperties>) response)));
    }

    public override Permission GetPermission(string id) => base.GetPermission(id);

    public override Task<PermissionResponse> CreatePermissionAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<PermissionResponse>(nameof (CreatePermissionAsync), requestOptions, (Func<ITrace, Task<PermissionResponse>>) (trace => this.CreatePermissionAsync(permissionProperties, tokenExpiryInSeconds, requestOptions, trace, cancellationToken)), (Func<PermissionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<PermissionProperties>((Response<PermissionProperties>) response)));
    }

    public override Task<PermissionResponse> UpsertPermissionAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<PermissionResponse>(nameof (UpsertPermissionAsync), requestOptions, (Func<ITrace, Task<PermissionResponse>>) (trace => this.UpsertPermissionAsync(permissionProperties, tokenExpiryInSeconds, requestOptions, trace, cancellationToken)), (Func<PermissionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<PermissionProperties>((Response<PermissionProperties>) response)));
    }

    public override FeedIterator<T> GetPermissionQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetPermissionQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetPermissionQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetPermissionQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }
  }
}
