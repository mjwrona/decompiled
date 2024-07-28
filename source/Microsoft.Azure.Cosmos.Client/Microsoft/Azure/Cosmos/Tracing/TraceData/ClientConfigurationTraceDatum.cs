// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.ClientConfigurationTraceDatum
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Telemetry;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal sealed class ClientConfigurationTraceDatum : TraceDatum
  {
    internal readonly UserAgentContainer UserAgentContainer;
    private ReadOnlyMemory<byte> cachedSerializedJson;
    private int cachedNumberOfClientCreated;
    private int cachedNumberOfActiveClient;
    private string cachedUserAgentString;
    private string cachedMachineId;
    private string cachedVMRegion;

    public ClientConfigurationTraceDatum(
      CosmosClientContext cosmosClientContext,
      DateTime startTime)
    {
      this.ClientCreatedDateTimeUtc = startTime;
      this.UserAgentContainer = cosmosClientContext.DocumentClient.ConnectionPolicy.UserAgentContainer;
      this.GatewayConnectionConfig = new GatewayConnectionConfig(cosmosClientContext.ClientOptions.GatewayModeMaxConnectionLimit, cosmosClientContext.ClientOptions.RequestTimeout, cosmosClientContext.ClientOptions.WebProxy, cosmosClientContext.ClientOptions.HttpClientFactory);
      this.RntbdConnectionConfig = cosmosClientContext.DocumentClient.RecordTcpSettings(this);
      this.OtherConnectionConfig = new OtherConnectionConfig(cosmosClientContext.ClientOptions.LimitToEndpoint, cosmosClientContext.ClientOptions.AllowBulkExecution);
      this.ConsistencyConfig = new ConsistencyConfig(cosmosClientContext.ClientOptions.ConsistencyLevel, cosmosClientContext.ClientOptions.ApplicationPreferredRegions, cosmosClientContext.ClientOptions.ApplicationRegion);
      this.cachedNumberOfClientCreated = CosmosClient.numberOfClientsCreated;
      this.cachedNumberOfActiveClient = CosmosClient.NumberOfActiveClients;
      this.cachedUserAgentString = this.UserAgentContainer.UserAgent;
      this.cachedMachineId = VmMetadataApiHandler.GetMachineId();
      this.ProcessorCount = Environment.ProcessorCount;
      this.ConnectionMode = cosmosClientContext.ClientOptions.ConnectionMode;
      this.cachedVMRegion = VmMetadataApiHandler.GetMachineRegion();
      this.cachedSerializedJson = this.GetSerializedDatum();
    }

    public DateTime ClientCreatedDateTimeUtc { get; }

    public GatewayConnectionConfig GatewayConnectionConfig { get; }

    public RntbdConnectionConfig RntbdConnectionConfig { get; }

    public OtherConnectionConfig OtherConnectionConfig { get; }

    public ConsistencyConfig ConsistencyConfig { get; }

    public int ProcessorCount { get; }

    public ConnectionMode ConnectionMode { get; }

    public ReadOnlyMemory<byte> SerializedJson
    {
      get
      {
        if (this.cachedUserAgentString != this.UserAgentContainer.UserAgent || this.cachedNumberOfClientCreated != CosmosClient.numberOfClientsCreated || this.cachedNumberOfActiveClient != CosmosClient.NumberOfActiveClients || (object) this.cachedMachineId != (object) VmMetadataApiHandler.GetMachineId() || (object) this.cachedVMRegion != (object) VmMetadataApiHandler.GetMachineRegion())
        {
          this.cachedNumberOfActiveClient = CosmosClient.NumberOfActiveClients;
          this.cachedNumberOfClientCreated = CosmosClient.numberOfClientsCreated;
          this.cachedUserAgentString = this.UserAgentContainer.UserAgent;
          this.cachedMachineId = VmMetadataApiHandler.GetMachineId();
          this.cachedVMRegion = VmMetadataApiHandler.GetMachineRegion();
          this.cachedSerializedJson = this.GetSerializedDatum();
        }
        return this.cachedSerializedJson;
      }
    }

    internal override void Accept(ITraceDatumVisitor traceDatumVisitor) => traceDatumVisitor.Visit(this);

    private ReadOnlyMemory<byte> GetSerializedDatum()
    {
      IJsonWriter jsonWriter = JsonWriter.Create(JsonSerializationFormat.Text);
      jsonWriter.WriteObjectStart();
      jsonWriter.WriteFieldName("Client Created Time Utc");
      jsonWriter.WriteStringValue(this.ClientCreatedDateTimeUtc.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
      jsonWriter.WriteFieldName("MachineId");
      jsonWriter.WriteStringValue(this.cachedMachineId);
      if (this.cachedVMRegion != null)
      {
        jsonWriter.WriteFieldName("VM Region");
        jsonWriter.WriteStringValue(this.cachedVMRegion);
      }
      jsonWriter.WriteFieldName("NumberOfClientsCreated");
      jsonWriter.WriteNumber64Value((Number64) (long) this.cachedNumberOfClientCreated);
      jsonWriter.WriteFieldName("NumberOfActiveClients");
      jsonWriter.WriteNumber64Value((Number64) (long) this.cachedNumberOfActiveClient);
      jsonWriter.WriteFieldName("ConnectionMode");
      jsonWriter.WriteStringValue(this.ConnectionMode.ToString());
      jsonWriter.WriteFieldName("User Agent");
      jsonWriter.WriteStringValue(this.cachedUserAgentString);
      jsonWriter.WriteFieldName("ConnectionConfig");
      jsonWriter.WriteObjectStart();
      jsonWriter.WriteFieldName("gw");
      jsonWriter.WriteStringValue(this.GatewayConnectionConfig.ToString());
      jsonWriter.WriteFieldName("rntbd");
      jsonWriter.WriteStringValue(this.RntbdConnectionConfig.ToString());
      jsonWriter.WriteFieldName("other");
      jsonWriter.WriteStringValue(this.OtherConnectionConfig.ToString());
      jsonWriter.WriteObjectEnd();
      jsonWriter.WriteFieldName("ConsistencyConfig");
      jsonWriter.WriteStringValue(this.ConsistencyConfig.ToString());
      jsonWriter.WriteFieldName("ProcessorCount");
      jsonWriter.WriteNumber64Value((Number64) (long) this.ProcessorCount);
      jsonWriter.WriteObjectEnd();
      return jsonWriter.GetResult();
    }
  }
}
