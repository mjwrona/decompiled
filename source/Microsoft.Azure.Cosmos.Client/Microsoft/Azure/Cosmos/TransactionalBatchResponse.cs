// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.TransactionalBatchResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public class TransactionalBatchResponse : 
    IReadOnlyList<TransactionalBatchOperationResult>,
    IEnumerable<TransactionalBatchOperationResult>,
    IEnumerable,
    IReadOnlyCollection<TransactionalBatchOperationResult>,
    IDisposable
  {
    private List<TransactionalBatchOperationResult> results;

    internal TransactionalBatchResponse(
      HttpStatusCode statusCode,
      SubStatusCodes subStatusCode,
      string errorMessage,
      IReadOnlyList<ItemBatchOperation> operations,
      ITrace trace)
      : this(statusCode, subStatusCode, errorMessage, new Headers(), trace, operations, (CosmosSerializerCore) null)
    {
      this.CreateAndPopulateResults(operations, trace);
    }

    protected TransactionalBatchResponse()
    {
    }

    private TransactionalBatchResponse(
      HttpStatusCode statusCode,
      SubStatusCodes subStatusCode,
      string errorMessage,
      Headers headers,
      ITrace trace,
      IReadOnlyList<ItemBatchOperation> operations,
      CosmosSerializerCore serializer)
    {
      this.StatusCode = statusCode;
      this.SubStatusCode = subStatusCode;
      this.ErrorMessage = errorMessage;
      this.Operations = operations;
      this.SerializerCore = serializer;
      this.Headers = headers;
      this.Diagnostics = (CosmosDiagnostics) new CosmosTraceDiagnostics(trace ?? (ITrace) NoOpTrace.Singleton);
    }

    public virtual Headers Headers { get; internal set; }

    public virtual string ActivityId => this.Headers?.ActivityId;

    public virtual double RequestCharge
    {
      get
      {
        Headers headers = this.Headers;
        return headers == null ? 0.0 : headers.RequestCharge;
      }
    }

    public virtual TimeSpan? RetryAfter => this.Headers?.RetryAfter;

    public virtual HttpStatusCode StatusCode { get; internal set; }

    public virtual string ErrorMessage { get; internal set; }

    public virtual bool IsSuccessStatusCode
    {
      get
      {
        int statusCode = (int) this.StatusCode;
        return statusCode >= 200 && statusCode <= 299;
      }
    }

    public virtual int Count
    {
      get
      {
        List<TransactionalBatchOperationResult> results = this.results;
        return results == null ? 0 : __nonvirtual (results.Count);
      }
    }

    public virtual CosmosDiagnostics Diagnostics { get; }

    internal virtual SubStatusCodes SubStatusCode { get; }

    internal virtual CosmosSerializerCore SerializerCore { get; }

    internal IReadOnlyList<ItemBatchOperation> Operations { get; set; }

    public virtual TransactionalBatchOperationResult this[int index] => this.results[index];

    public virtual TransactionalBatchOperationResult<T> GetOperationResultAtIndex<T>(int index)
    {
      TransactionalBatchOperationResult result = this.results[index];
      T resource = default (T);
      if (result.ResourceStream != null)
        resource = this.SerializerCore.FromStream<T>(result.ResourceStream);
      return new TransactionalBatchOperationResult<T>(result, resource);
    }

    public virtual IEnumerator<TransactionalBatchOperationResult> GetEnumerator() => (IEnumerator<TransactionalBatchOperationResult>) this.results.GetEnumerator();

    internal virtual IEnumerable<string> GetActivityIds()
    {
      yield return this.ActivityId;
    }

    public void Dispose() => this.Dispose(true);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    internal static async Task<TransactionalBatchResponse> FromResponseMessageAsync(
      ResponseMessage responseMessage,
      ServerBatchRequest serverRequest,
      CosmosSerializerCore serializer,
      bool shouldPromoteOperationStatus,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      TransactionalBatchResponse transactionalBatchResponse;
      using (trace.StartChild("Create Trace", TraceComponent.Batch, TraceLevel.Info))
      {
        using (responseMessage)
        {
          TransactionalBatchResponse response = (TransactionalBatchResponse) null;
          if (responseMessage.Content != null)
          {
            Stream content = responseMessage.Content;
            if (!responseMessage.Content.CanSeek)
            {
              content = (Stream) new MemoryStream();
              await responseMessage.Content.CopyToAsync(content);
            }
            if (content.ReadByte() == 129)
            {
              content.Position = 0L;
              response = await TransactionalBatchResponse.PopulateFromContentAsync(content, responseMessage, serverRequest, serializer, trace, shouldPromoteOperationStatus) ?? new TransactionalBatchResponse(HttpStatusCode.InternalServerError, SubStatusCodes.Unknown, ClientResources.ServerResponseDeserializationFailure, responseMessage.Headers, trace, serverRequest.Operations, serializer);
            }
            content = (Stream) null;
          }
          if (response == null)
            response = new TransactionalBatchResponse(responseMessage.StatusCode, responseMessage.Headers.SubStatusCode, responseMessage.ErrorMessage, responseMessage.Headers, trace, serverRequest.Operations, serializer);
          if (response.results == null || response.results.Count != serverRequest.Operations.Count)
          {
            if (responseMessage.IsSuccessStatusCode)
              response = new TransactionalBatchResponse(HttpStatusCode.InternalServerError, SubStatusCodes.Unknown, ClientResources.InvalidServerResponse, responseMessage.Headers, trace, serverRequest.Operations, serializer);
            int result = 0;
            string s;
            if (responseMessage.StatusCode == (HttpStatusCode) 429 && (!responseMessage.Headers.TryGetValue("x-ms-retry-after-ms", out s) || s == null || !int.TryParse(s, out result)))
              result = 0;
            response.CreateAndPopulateResults(serverRequest.Operations, trace, result);
          }
          transactionalBatchResponse = response;
        }
      }
      return transactionalBatchResponse;
    }

    private void CreateAndPopulateResults(
      IReadOnlyList<ItemBatchOperation> operations,
      ITrace trace,
      int retryAfterMilliseconds = 0)
    {
      this.results = new List<TransactionalBatchOperationResult>();
      for (int index = 0; index < operations.Count; ++index)
      {
        TransactionalBatchOperationResult batchOperationResult = new TransactionalBatchOperationResult(this.StatusCode)
        {
          SubStatusCode = this.SubStatusCode,
          RetryAfter = TimeSpan.FromMilliseconds((double) retryAfterMilliseconds),
          SessionToken = this.Headers.Session,
          ActivityId = this.ActivityId
        };
        batchOperationResult.Trace = trace;
        this.results.Add(batchOperationResult);
      }
    }

    private static async Task<TransactionalBatchResponse> PopulateFromContentAsync(
      Stream content,
      ResponseMessage responseMessage,
      ServerBatchRequest serverRequest,
      CosmosSerializerCore serializer,
      ITrace trace,
      bool shouldPromoteOperationStatus)
    {
      List<TransactionalBatchOperationResult> results = new List<TransactionalBatchOperationResult>();
      if (await content.ReadRecordIOAsync((Func<ReadOnlyMemory<byte>, Result>) (record =>
      {
        TransactionalBatchOperationResult batchOperationResult;
        Result result = TransactionalBatchOperationResult.ReadOperationResult(record, out batchOperationResult);
        if (result != Result.Success)
          return result;
        batchOperationResult.Trace = trace;
        batchOperationResult.SessionToken = responseMessage.Headers.Session;
        batchOperationResult.ActivityId = responseMessage.Headers.ActivityId;
        results.Add(batchOperationResult);
        return result;
      }), resizer: new MemorySpanResizer<byte>((int) content.Length)) != Result.Success)
        return (TransactionalBatchResponse) null;
      HttpStatusCode statusCode = responseMessage.StatusCode;
      SubStatusCodes subStatusCode = responseMessage.Headers.SubStatusCode;
      if (responseMessage.StatusCode == (HttpStatusCode) 207 & shouldPromoteOperationStatus)
      {
        foreach (TransactionalBatchOperationResult batchOperationResult in results)
        {
          if (batchOperationResult.StatusCode != (HttpStatusCode) 424 && batchOperationResult.StatusCode >= HttpStatusCode.BadRequest)
          {
            statusCode = batchOperationResult.StatusCode;
            subStatusCode = batchOperationResult.SubStatusCode;
            break;
          }
        }
      }
      return new TransactionalBatchResponse(statusCode, subStatusCode, responseMessage.ErrorMessage, responseMessage.Headers, trace, serverRequest.Operations, serializer)
      {
        results = results
      };
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.Operations == null)
        return;
      foreach (ItemBatchOperation operation in (IEnumerable<ItemBatchOperation>) this.Operations)
        operation.Dispose();
      this.Operations = (IReadOnlyList<ItemBatchOperation>) null;
    }
  }
}
