// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BatchOperation
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public abstract class BatchOperation
  {
    internal List<RESTCommand<NullType>> Operations { get; } = new List<RESTCommand<NullType>>();

    internal string BatchID { get; } = Guid.NewGuid().ToString();

    internal Task<IList<BlobBatchSubOperationResponse>> ExecuteAsync(
      CloudBlobClient client,
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      requestOptions = requestOptions ?? BlobRequestOptions.BaseDefaultRequestOptions;
      BlobRequestOptions blobRequestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, client);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<IList<BlobBatchSubOperationResponse>>(BatchOperation.BatchImpl(this, client, requestOptions), blobRequestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    private static RESTCommand<IList<BlobBatchSubOperationResponse>> BatchImpl(
      BatchOperation batch,
      CloudBlobClient client,
      BlobRequestOptions requestOptions)
    {
      RESTCommand<IList<BlobBatchSubOperationResponse>> cmd1 = new RESTCommand<IList<BlobBatchSubOperationResponse>>(client.Credentials, client.StorageUri, client.HttpClient);
      requestOptions.ApplyToStorageCommand<IList<BlobBatchSubOperationResponse>>(cmd1);
      List<BlobBatchSubOperationResponse> results = new List<BlobBatchSubOperationResponse>();
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOnly;
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<IList<BlobBatchSubOperationResponse>>, HttpResponseMessage, Exception, OperationContext, IList<BlobBatchSubOperationResponse>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<IList<BlobBatchSubOperationResponse>>(HttpStatusCode.Accepted, resp, (IList<BlobBatchSubOperationResponse>) null, (StorageCommandBase<IList<BlobBatchSubOperationResponse>>) cmd, ex));
      cmd1.BuildContent = (Func<RESTCommand<IList<BlobBatchSubOperationResponse>>, OperationContext, HttpContent>) ((cmd, ctx) => BlobHttpRequestMessageFactory.WriteBatchBody(client, cmd, batch, ctx));
      cmd1.BuildRequest = (Func<RESTCommand<IList<BlobBatchSubOperationResponse>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.PrepareBatchRequest(uri, client.BufferManager, serverTimeout, batch, cnt, ctx, client.GetCanonicalizer(), client.Credentials));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<IList<BlobBatchSubOperationResponse>>, HttpResponseMessage, OperationContext, CancellationToken, Task<IList<BlobBatchSubOperationResponse>>>) ((cmd, resp, ctx, ct) => BlobHttpResponseParsers.BatchPostProcessAsync((IList<BlobBatchSubOperationResponse>) results, cmd, (IList<HttpStatusCode>) new HttpStatusCode[2]
      {
        HttpStatusCode.Accepted,
        HttpStatusCode.OK
      }, resp, ctx, ct));
      return cmd1;
    }
  }
}
