// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosObjectDisposedException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Pipeline;
using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosObjectDisposedException : ObjectDisposedException
  {
    private readonly ObjectDisposedException originalException;
    private readonly CosmosClient cosmosClient;

    internal CosmosObjectDisposedException(
      ObjectDisposedException originalException,
      CosmosClient cosmosClient,
      ITrace trace)
      : base(originalException.ObjectName)
    {
      this.cosmosClient = cosmosClient ?? throw new ArgumentNullException("CosmosClient");
      this.originalException = originalException ?? throw new ArgumentNullException(nameof (originalException));
      string str = string.Format("CosmosClient Endpoint: {0}; Created at: {1};", (object) this.cosmosClient.Endpoint, (object) this.cosmosClient.ClientConfigurationTraceDatum.ClientCreatedDateTimeUtc.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture)) + " UserAgent: " + this.cosmosClient.ClientConfigurationTraceDatum.UserAgentContainer.UserAgent + ";";
      this.Message = this.cosmosClient.DisposedDateTimeUtc.HasValue ? "Cannot access a disposed 'CosmosClient'. Follow best practices and use the CosmosClient as a singleton. CosmosClient was disposed at: " + this.cosmosClient.DisposedDateTimeUtc.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + "; " + str : originalException.Message + " The CosmosClient is still active and NOT disposed of. " + str;
      this.Diagnostics = trace != null ? (CosmosDiagnostics) new CosmosTraceDiagnostics(trace) : throw new ArgumentNullException(nameof (trace));
    }

    public override string Source
    {
      get => this.originalException.Source;
      set => this.originalException.Source = value;
    }

    public override string Message { get; }

    public override string StackTrace => this.originalException.StackTrace;

    public override IDictionary Data => this.originalException.Data;

    public CosmosDiagnostics Diagnostics { get; }

    public override string HelpLink
    {
      get => this.originalException.HelpLink;
      set => this.originalException.HelpLink = value;
    }

    public override Exception GetBaseException() => this.originalException.GetBaseException();

    public override string ToString() => string.Format("{0} {1}CosmosDiagnostics: {2} StackTrace: {3}", (object) this.Message, (object) Environment.NewLine, (object) this.Diagnostics, (object) this.StackTrace);

    internal static void RecordOtelAttributes(
      CosmosObjectDisposedException exception,
      DiagnosticScope scope)
    {
      scope.AddAttribute("db.cosmosdb.regions_contacted", ClientTelemetryHelper.GetContactedRegions(exception.Diagnostics));
      scope.AddAttribute("exception.message", exception.Message);
      CosmosDbEventSource.RecordDiagnosticsForExceptions(exception.Diagnostics);
    }
  }
}
