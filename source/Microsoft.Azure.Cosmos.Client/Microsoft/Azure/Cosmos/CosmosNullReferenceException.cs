// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosNullReferenceException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Pipeline;
using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosNullReferenceException : NullReferenceException
  {
    private readonly NullReferenceException originalException;

    internal CosmosNullReferenceException(NullReferenceException originalException, ITrace trace)
    {
      this.originalException = originalException ?? throw new ArgumentNullException(nameof (originalException));
      this.Diagnostics = trace != null ? (CosmosDiagnostics) new CosmosTraceDiagnostics(trace) : throw new ArgumentNullException(nameof (trace));
    }

    public override string Source
    {
      get => this.originalException.Source;
      set => this.originalException.Source = value;
    }

    public override string Message => this.originalException.Message + this.Diagnostics.ToString();

    public override string StackTrace => this.originalException.StackTrace;

    public override IDictionary Data => this.originalException.Data;

    public CosmosDiagnostics Diagnostics { get; }

    public override string HelpLink
    {
      get => this.originalException.HelpLink;
      set => this.originalException.HelpLink = value;
    }

    public override Exception GetBaseException() => this.originalException.GetBaseException();

    public override string ToString() => string.Format("{0} {1} CosmosDiagnostics: {2}", (object) this.originalException, (object) Environment.NewLine, (object) this.Diagnostics);

    internal static void RecordOtelAttributes(
      CosmosNullReferenceException exception,
      DiagnosticScope scope)
    {
      scope.AddAttribute("db.cosmosdb.regions_contacted", ClientTelemetryHelper.GetContactedRegions(exception.Diagnostics));
      scope.AddAttribute("exception.message", exception.GetBaseException().Message);
      CosmosDbEventSource.RecordDiagnosticsForExceptions(exception.Diagnostics);
    }
  }
}
