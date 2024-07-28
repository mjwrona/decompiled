// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.OpenTelemetryAttributeKeys
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal sealed class OpenTelemetryAttributeKeys
  {
    public const string DiagnosticNamespace = "Azure.Cosmos";
    public const string ResourceProviderNamespace = "Microsoft.DocumentDB";
    public const string OperationPrefix = "Cosmos";
    public const string DbSystemName = "db.system";
    public const string DbName = "db.name";
    public const string DbOperation = "db.operation";
    public const string NetPeerName = "net.peer.name";
    public const string ClientId = "db.cosmosdb.client_id";
    public const string MachineId = "db.cosmosdb.hashed_machine_id";
    public const string UserAgent = "db.cosmosdb.user_agent";
    public const string ConnectionMode = "db.cosmosdb.connection_mode";
    public const string OperationType = "db.cosmosdb.operation_type";
    public const string ContainerName = "db.cosmosdb.container";
    public const string RequestContentLength = "db.cosmosdb.request_content_length_bytes";
    public const string ResponseContentLength = "db.cosmosdb.response_content_length_bytes";
    public const string StatusCode = "db.cosmosdb.status_code";
    public const string RequestCharge = "db.cosmosdb.request_charge";
    public const string Region = "db.cosmosdb.regions_contacted";
    public const string RetryCount = "db.cosmosdb.retry_count";
    public const string ItemCount = "db.cosmosdb.item_count";
    public const string RequestDiagnostics = "db.cosmosdb.request_diagnostics";
    public const string ExceptionType = "exception.type";
    public const string ExceptionMessage = "exception.message";
    public const string ExceptionStacktrace = "exception.stacktrace";
  }
}
