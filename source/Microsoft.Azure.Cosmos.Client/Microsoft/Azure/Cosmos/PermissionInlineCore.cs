// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PermissionInlineCore
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
  internal sealed class PermissionInlineCore : PermissionCore
  {
    internal PermissionInlineCore(CosmosClientContext clientContext, UserCore user, string userId)
      : base(clientContext, user, userId)
    {
    }

    public override Task<PermissionResponse> ReadAsync(
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<PermissionResponse>(nameof (ReadAsync), requestOptions, (Func<ITrace, Task<PermissionResponse>>) (trace => this.ReadAsync(tokenExpiryInSeconds, requestOptions, trace, cancellationToken)), (Func<PermissionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<PermissionProperties>((Response<PermissionProperties>) response)));
    }

    public override Task<PermissionResponse> ReplaceAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<PermissionResponse>(nameof (ReplaceAsync), requestOptions, (Func<ITrace, Task<PermissionResponse>>) (trace => this.ReplaceAsync(permissionProperties, tokenExpiryInSeconds, requestOptions, trace, cancellationToken)), (Func<PermissionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<PermissionProperties>((Response<PermissionProperties>) response)));
    }

    public override Task<PermissionResponse> DeleteAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<PermissionResponse>(nameof (DeleteAsync), requestOptions, (Func<ITrace, Task<PermissionResponse>>) (trace => this.DeleteAsync(requestOptions, trace, cancellationToken)), (Func<PermissionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<PermissionProperties>((Response<PermissionProperties>) response)));
    }
  }
}
