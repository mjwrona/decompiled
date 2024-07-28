// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientEncryptionKeyInlineCore
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
  internal sealed class ClientEncryptionKeyInlineCore : ClientEncryptionKeyCore
  {
    internal ClientEncryptionKeyInlineCore(
      CosmosClientContext clientContext,
      DatabaseInternal database,
      string keyId)
      : base(clientContext, database, keyId)
    {
    }

    public override Task<ClientEncryptionKeyResponse> ReadAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ClientEncryptionKeyResponse>(nameof (ReadAsync), requestOptions, (Func<ITrace, Task<ClientEncryptionKeyResponse>>) (trace => base.ReadAsync(requestOptions, cancellationToken)), (Func<ClientEncryptionKeyResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ClientEncryptionKeyProperties>((Response<ClientEncryptionKeyProperties>) response)));
    }

    public override Task<ClientEncryptionKeyResponse> ReplaceAsync(
      ClientEncryptionKeyProperties clientEncryptionKeyProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ClientEncryptionKeyResponse>(nameof (ReplaceAsync), requestOptions, (Func<ITrace, Task<ClientEncryptionKeyResponse>>) (trace => base.ReplaceAsync(clientEncryptionKeyProperties, requestOptions, cancellationToken)), (Func<ClientEncryptionKeyResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ClientEncryptionKeyProperties>((Response<ClientEncryptionKeyProperties>) response)));
    }
  }
}
