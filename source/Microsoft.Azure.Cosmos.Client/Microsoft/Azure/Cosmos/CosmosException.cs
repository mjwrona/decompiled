// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Pipeline;
using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  public class CosmosException : Exception
  {
    private readonly string stackTrace;
    private readonly Lazy<string> lazyMessage;

    internal CosmosException(
      HttpStatusCode statusCode,
      string message,
      string stackTrace,
      Headers headers,
      ITrace trace,
      Error error,
      Exception innerException)
      : base(string.Empty, innerException)
    {
      CosmosException cosmosException = this;
      this.ResponseBody = message;
      this.stackTrace = stackTrace;
      this.StatusCode = statusCode;
      this.Headers = headers ?? new Headers();
      this.Error = error;
      this.Trace = trace;
      this.Diagnostics = (CosmosDiagnostics) new CosmosTraceDiagnostics(this.Trace ?? (ITrace) NoOpTrace.Singleton);
      this.lazyMessage = new Lazy<string>((Func<string>) (() => CosmosException.GetMessageHelper(statusCode, cosmosException.Headers, cosmosException.ResponseBody, cosmosException.Diagnostics)));
    }

    public CosmosException(
      string message,
      HttpStatusCode statusCode,
      int subStatusCode,
      string activityId,
      double requestCharge)
      : base(message)
    {
      this.stackTrace = (string) null;
      this.StatusCode = statusCode;
      this.ResponseBody = message;
      this.Trace = (ITrace) NoOpTrace.Singleton;
      this.lazyMessage = new Lazy<string>((Func<string>) (() => message));
      this.Headers = new Headers()
      {
        SubStatusCode = (SubStatusCodes) subStatusCode,
        RequestCharge = requestCharge
      };
      if (string.IsNullOrEmpty(activityId))
        return;
      this.Headers.ActivityId = activityId;
    }

    public override string Message => this.lazyMessage.Value;

    public virtual string ResponseBody { get; }

    public virtual HttpStatusCode StatusCode { get; }

    public virtual int SubStatusCode => Headers.GetIntValueOrDefault(this.Headers.SubStatusCodeLiteral);

    public virtual double RequestCharge => this.Headers.RequestCharge;

    public virtual string ActivityId => this.Headers.ActivityId;

    public virtual TimeSpan? RetryAfter => this.Headers.RetryAfter;

    public virtual Headers Headers { get; }

    public virtual CosmosDiagnostics Diagnostics { get; }

    public override string StackTrace => this.stackTrace != null ? this.stackTrace : base.StackTrace;

    internal virtual ITrace Trace { get; }

    internal virtual Error Error { get; set; }

    public virtual bool TryGetHeader(string headerName, out string value)
    {
      if (this.Headers != null)
        return this.Headers.TryGetValue(headerName, out value);
      value = (string) null;
      return false;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.GetType().FullName);
      stringBuilder.Append(" : ");
      this.ToStringHelper(stringBuilder);
      return stringBuilder.ToString();
    }

    internal ResponseMessage ToCosmosResponseMessage(RequestMessage request)
    {
      Headers headers = this.Headers;
      RequestMessage requestMessage = request;
      CosmosException cosmosException = this;
      return new ResponseMessage(this.StatusCode, requestMessage, headers, cosmosException, this.Trace);
    }

    private static string GetMessageHelper(
      HttpStatusCode statusCode,
      Headers headers,
      string responseBody,
      CosmosDiagnostics diagnostics)
    {
      StringBuilder stringBuilder = new StringBuilder();
      CosmosException.AppendMessageWithoutDiagnostics(stringBuilder, statusCode, headers, responseBody);
      switch (statusCode)
      {
        case HttpStatusCode.NotFound:
          if (headers.SubStatusCode != SubStatusCodes.PartitionKeyRangeGone)
            break;
          goto case HttpStatusCode.RequestTimeout;
        case HttpStatusCode.RequestTimeout:
        case HttpStatusCode.InternalServerError:
        case HttpStatusCode.ServiceUnavailable:
          stringBuilder.Append("; Diagnostics:");
          stringBuilder.Append(diagnostics.ToString());
          break;
      }
      return stringBuilder.ToString();
    }

    private static void AppendMessageWithoutDiagnostics(
      StringBuilder stringBuilder,
      HttpStatusCode statusCode,
      Headers headers,
      string responseBody)
    {
      stringBuilder.Append("Response status code does not indicate success: ");
      stringBuilder.Append(string.Format("{0} ({1})", (object) statusCode, (object) (int) statusCode));
      stringBuilder.Append("; Substatus: ");
      stringBuilder.Append(headers?.SubStatusCodeLiteral ?? "0");
      stringBuilder.Append("; ActivityId: ");
      stringBuilder.Append(headers?.ActivityId ?? string.Empty);
      stringBuilder.Append("; Reason: (");
      stringBuilder.Append(responseBody ?? string.Empty);
      stringBuilder.Append(");");
    }

    private string ToStringHelper(StringBuilder stringBuilder)
    {
      if (stringBuilder == null)
        throw new ArgumentNullException(nameof (stringBuilder));
      CosmosException.AppendMessageWithoutDiagnostics(stringBuilder, this.StatusCode, this.Headers, this.ResponseBody);
      stringBuilder.AppendLine();
      if (this.InnerException != null)
      {
        stringBuilder.Append(" ---> ");
        stringBuilder.Append((object) this.InnerException);
        stringBuilder.AppendLine();
        stringBuilder.Append("   ");
        stringBuilder.Append("--- End of inner exception stack trace ---");
        stringBuilder.AppendLine();
      }
      if (this.StackTrace != null)
      {
        stringBuilder.Append(this.StackTrace);
        stringBuilder.AppendLine();
      }
      if (this.Diagnostics != null)
      {
        stringBuilder.Append("--- Cosmos Diagnostics ---");
        stringBuilder.Append((object) this.Diagnostics);
      }
      return stringBuilder.ToString();
    }

    internal static void RecordOtelAttributes(CosmosException exception, DiagnosticScope scope)
    {
      scope.AddAttribute<HttpStatusCode>("db.cosmosdb.status_code", exception.StatusCode);
      scope.AddAttribute<double>("db.cosmosdb.request_charge", exception.RequestCharge);
      scope.AddAttribute("db.cosmosdb.regions_contacted", ClientTelemetryHelper.GetContactedRegions(exception.Diagnostics));
      scope.AddAttribute("exception.message", exception.Message);
      CosmosDbEventSource.RecordDiagnosticsForExceptions(exception.Diagnostics);
    }
  }
}
