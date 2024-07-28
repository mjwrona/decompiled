// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.OpenTelemetryCoreRecorder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal struct OpenTelemetryCoreRecorder : IDisposable
  {
    private const string CosmosDb = "cosmosdb";
    private readonly DiagnosticScope scope;
    private readonly DistributedTracingOptions config;
    internal static IDictionary<Type, Action<Exception, DiagnosticScope>> OTelCompatibleExceptions = (IDictionary<Type, Action<Exception, DiagnosticScope>>) new Dictionary<Type, Action<Exception, DiagnosticScope>>()
    {
      {
        typeof (CosmosNullReferenceException),
        (Action<Exception, DiagnosticScope>) ((exception, scope) => CosmosNullReferenceException.RecordOtelAttributes((CosmosNullReferenceException) exception, scope))
      },
      {
        typeof (CosmosObjectDisposedException),
        (Action<Exception, DiagnosticScope>) ((exception, scope) => CosmosObjectDisposedException.RecordOtelAttributes((CosmosObjectDisposedException) exception, scope))
      },
      {
        typeof (CosmosOperationCanceledException),
        (Action<Exception, DiagnosticScope>) ((exception, scope) => CosmosOperationCanceledException.RecordOtelAttributes((CosmosOperationCanceledException) exception, scope))
      },
      {
        typeof (CosmosException),
        (Action<Exception, DiagnosticScope>) ((exception, scope) => CosmosException.RecordOtelAttributes((CosmosException) exception, scope))
      },
      {
        typeof (ChangeFeedProcessorUserException),
        (Action<Exception, DiagnosticScope>) ((exception, scope) => ChangeFeedProcessorUserException.RecordOtelAttributes((ChangeFeedProcessorUserException) exception, scope))
      }
    };

    public OpenTelemetryCoreRecorder(
      DiagnosticScope scope,
      CosmosClientContext clientContext,
      DistributedTracingOptions config)
    {
      this.scope = scope;
      this.config = config;
      if (!this.IsEnabled)
        return;
      this.scope.Start();
      this.Record(clientContext);
    }

    public bool IsEnabled => this.scope.IsEnabled;

    public void Record(string key, string value)
    {
      if (!this.IsEnabled)
        return;
      this.scope.AddAttribute(key, value);
    }

    public void Record(CosmosClientContext clientContext)
    {
      if (!this.IsEnabled)
        return;
      this.scope.AddAttribute("db.system", "cosmosdb");
      this.scope.AddAttribute("db.cosmosdb.hashed_machine_id", VmMetadataApiHandler.GetMachineId());
      this.scope.AddAttribute("net.peer.name", clientContext.Client?.Endpoint?.Host);
      this.scope.AddAttribute("db.cosmosdb.client_id", clientContext?.Client?.Id ?? "information not available");
      this.scope.AddAttribute("db.cosmosdb.user_agent", clientContext.UserAgent ?? "information not available");
      this.scope.AddAttribute<ConnectionMode>("db.cosmosdb.connection_mode", clientContext.ClientOptions.ConnectionMode);
    }

    public void Record(OpenTelemetryAttributes response)
    {
      if (!this.IsEnabled)
        return;
      this.scope.AddAttribute("db.name", response.DatabaseName);
      this.scope.AddAttribute("db.cosmosdb.container", response.ContainerName);
      this.scope.AddAttribute("db.cosmosdb.request_content_length_bytes", response.RequestContentLength);
      this.scope.AddAttribute("db.cosmosdb.response_content_length_bytes", response.ResponseContentLength);
      this.scope.AddAttribute<HttpStatusCode>("db.cosmosdb.status_code", response.StatusCode);
      this.scope.AddAttribute<double?>("db.cosmosdb.request_charge", response.RequestCharge);
      this.scope.AddAttribute("db.cosmosdb.item_count", response.ItemCount);
      this.scope.AddAttribute("db.cosmosdb.operation_type", response.OperationType);
      if (response.Diagnostics != null)
      {
        this.scope.AddAttribute("db.cosmosdb.regions_contacted", ClientTelemetryHelper.GetContactedRegions(response.Diagnostics) ?? "information not available");
        CosmosDbEventSource.RecordDiagnosticsForRequests(this.config, response);
      }
      else
      {
        this.scope.AddAttribute("db.cosmosdb.regions_contacted", "information not available");
        this.scope.AddAttribute("db.cosmosdb.request_diagnostics", "information not available");
      }
    }

    public void MarkFailed(Exception exception)
    {
      if (!this.IsEnabled)
        return;
      this.scope.AddAttribute("exception.stacktrace", exception.StackTrace);
      this.scope.AddAttribute<Type>("exception.type", exception.GetType());
      if (!OpenTelemetryCoreRecorder.IsExceptionRegistered(exception, this.scope))
        this.scope.AddAttribute("exception.message", exception.Message);
      this.scope.Failed(exception);
    }

    internal static bool IsExceptionRegistered(Exception exception, DiagnosticScope scope)
    {
      foreach (KeyValuePair<Type, Action<Exception, DiagnosticScope>> compatibleException in (IEnumerable<KeyValuePair<Type, Action<Exception, DiagnosticScope>>>) OpenTelemetryCoreRecorder.OTelCompatibleExceptions)
      {
        Type type = exception.GetType();
        if (compatibleException.Key.IsAssignableFrom(type))
        {
          compatibleException.Value(exception, scope);
          return true;
        }
      }
      return false;
    }

    public void Dispose()
    {
      if (!this.scope.IsEnabled)
        return;
      this.scope.Dispose();
    }
  }
}
