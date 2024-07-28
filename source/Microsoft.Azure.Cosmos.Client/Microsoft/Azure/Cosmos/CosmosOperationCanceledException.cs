// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosOperationCanceledException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Pipeline;
using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos
{
  [Serializable]
  public class CosmosOperationCanceledException : OperationCanceledException
  {
    private readonly OperationCanceledException originalException;
    private readonly Lazy<string> lazyMessage;
    private readonly Lazy<string> toStringMessage;
    private readonly bool tokenCancellationRequested;

    public CosmosOperationCanceledException(
      OperationCanceledException originalException,
      CosmosDiagnostics diagnostics)
      : base(originalException.CancellationToken)
    {
      this.originalException = originalException ?? throw new ArgumentNullException(nameof (originalException));
      this.Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof (diagnostics));
      this.tokenCancellationRequested = originalException.CancellationToken.IsCancellationRequested;
      this.toStringMessage = this.CreateToStringMessage();
      this.lazyMessage = this.CreateLazyMessage();
    }

    internal CosmosOperationCanceledException(
      OperationCanceledException originalException,
      ITrace trace)
      : base(originalException.CancellationToken)
    {
      this.originalException = originalException ?? throw new ArgumentNullException(nameof (originalException));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      using (ITrace trace1 = trace.StartChild(nameof (CosmosOperationCanceledException)))
        trace1.AddDatum("Operation Cancelled Exception", (object) originalException);
      this.Diagnostics = (CosmosDiagnostics) new CosmosTraceDiagnostics(trace);
      this.tokenCancellationRequested = originalException.CancellationToken.IsCancellationRequested;
      this.toStringMessage = this.CreateToStringMessage();
      this.lazyMessage = this.CreateLazyMessage();
    }

    protected CosmosOperationCanceledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.originalException = (OperationCanceledException) info.GetValue(nameof (originalException), typeof (OperationCanceledException));
      this.tokenCancellationRequested = (bool) info.GetValue(nameof (tokenCancellationRequested), typeof (bool));
      this.lazyMessage = new Lazy<string>((Func<string>) (() => (string) info.GetValue(nameof (lazyMessage), typeof (string))));
      this.toStringMessage = new Lazy<string>((Func<string>) (() => (string) info.GetValue(nameof (toStringMessage), typeof (string))));
      this.Diagnostics = (CosmosDiagnostics) new CosmosTraceDiagnostics((ITrace) NoOpTrace.Singleton);
    }

    public override string Source
    {
      get => this.originalException.Source;
      set => this.originalException.Source = value;
    }

    public override string Message => this.lazyMessage.Value;

    public override string StackTrace => this.originalException.StackTrace;

    public override IDictionary Data => this.originalException.Data;

    public CosmosDiagnostics Diagnostics { get; }

    public override string HelpLink
    {
      get => this.originalException.HelpLink;
      set => this.originalException.HelpLink = value;
    }

    public override Exception GetBaseException() => this.originalException.GetBaseException();

    public override string ToString() => this.toStringMessage.Value;

    private Lazy<string> CreateLazyMessage() => new Lazy<string>((Func<string>) (() => string.Format("{0}{1}Cancellation Token has expired: {2}. Learn more at: https://aka.ms/cosmosdb-tsg-request-timeout{3}CosmosDiagnostics: {4}", (object) this.originalException.Message, (object) Environment.NewLine, (object) this.tokenCancellationRequested, (object) Environment.NewLine, (object) this.Diagnostics)));

    private Lazy<string> CreateToStringMessage() => new Lazy<string>((Func<string>) (() => string.Format("{0}{1}Cancellation Token has expired: {2}. Learn more at: https://aka.ms/cosmosdb-tsg-request-timeout{3}CosmosDiagnostics: {4}", (object) this.originalException, (object) Environment.NewLine, (object) this.tokenCancellationRequested, (object) Environment.NewLine, (object) this.Diagnostics)));

    internal static void RecordOtelAttributes(
      CosmosOperationCanceledException exception,
      DiagnosticScope scope)
    {
      scope.AddAttribute("db.cosmosdb.regions_contacted", ClientTelemetryHelper.GetContactedRegions(exception.Diagnostics));
      scope.AddAttribute("exception.message", exception.GetBaseException().Message);
      CosmosDbEventSource.RecordDiagnosticsForExceptions(exception.Diagnostics);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("originalException", (object) this.originalException);
      info.AddValue("tokenCancellationRequested", this.tokenCancellationRequested);
      info.AddValue("lazyMessage", (object) this.lazyMessage.Value);
      info.AddValue("toStringMessage", (object) this.toStringMessage.Value);
    }
  }
}
