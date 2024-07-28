// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Extensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal static class Extensions
  {
    private static readonly char[] NewLineCharacters = new char[2]
    {
      '\r',
      '\n'
    };

    internal static bool IsSuccess(this HttpStatusCode httpStatusCode) => httpStatusCode >= HttpStatusCode.OK && httpStatusCode <= (HttpStatusCode) 299;

    public static void Add(
      this INameValueCollection nameValueCollection,
      string headerName,
      IEnumerable<string> values)
    {
      if (headerName == null)
        throw new ArgumentNullException(nameof (headerName));
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      nameValueCollection.Add(headerName, string.Join(",", values));
    }

    public static T GetHeaderValue<T>(this INameValueCollection nameValueCollection, string key)
    {
      string nameValue = nameValueCollection[key];
      if (string.IsNullOrEmpty(nameValue))
        return default (T);
      return typeof (T) == typeof (double) ? (T) (ValueType) double.Parse(nameValue, (IFormatProvider) CultureInfo.InvariantCulture) : (T) nameValue;
    }

    internal static ResponseMessage ToCosmosResponseMessage(
      this DocumentServiceResponse documentServiceResponse,
      RequestMessage requestMessage,
      RequestChargeTracker requestChargeTracker)
    {
      Headers headers = new Headers(documentServiceResponse.Headers);
      if (requestChargeTracker != null && headers.RequestCharge < requestChargeTracker.TotalRequestCharge)
      {
        headers.RequestCharge = requestChargeTracker.TotalRequestCharge;
        DefaultTrace.TraceWarning("Header RequestCharge {0} is less than the RequestChargeTracker: {1}; URI {2}, OperationType: {3}", (object) headers.RequestCharge, (object) requestChargeTracker.TotalRequestCharge, (object) requestMessage?.RequestUriString, (object) requestMessage?.OperationType);
      }
      if (documentServiceResponse.RequestStats == null || documentServiceResponse.RequestStats.ContactedReplicas.Count == 0 && documentServiceResponse.RequestStats.FailedReplicas.Count == 0)
      {
        string activityId = headers.ActivityId;
        DateTime utcNow = DateTime.UtcNow;
        int statusCode = (int) documentServiceResponse.StatusCode;
        int subStatusCode = (int) documentServiceResponse.SubStatusCode;
        DateTime responseTimeUtc = utcNow;
        double requestCharge = headers.RequestCharge;
        HttpMethod method = requestMessage?.Method;
        string requestUriString = requestMessage?.RequestUriString;
        string session1 = requestMessage?.Headers?.Session;
        string session2 = headers.Session;
        string durationMilliseconds = headers.BackendRequestDurationMilliseconds;
        PointOperationStatisticsTraceDatum statisticsTraceDatum = new PointOperationStatisticsTraceDatum(activityId, (HttpStatusCode) statusCode, (SubStatusCodes) subStatusCode, responseTimeUtc, requestCharge, (string) null, method, requestUriString, session1, session2, durationMilliseconds);
        requestMessage.Trace.AddDatum("PointOperationStatisticsTraceDatum", (TraceDatum) statisticsTraceDatum);
      }
      if (!documentServiceResponse.StatusCode.IsSuccess())
        return CosmosExceptionFactory.Create(documentServiceResponse, headers, requestMessage).ToCosmosResponseMessage(requestMessage);
      return new ResponseMessage(documentServiceResponse.StatusCode, requestMessage, headers, (CosmosException) null, (ITrace) null)
      {
        Content = documentServiceResponse.ResponseBody
      };
    }

    internal static ResponseMessage ToCosmosResponseMessage(
      this DocumentClientException documentClientException,
      RequestMessage requestMessage)
    {
      CosmosException cosmosException = CosmosExceptionFactory.Create(documentClientException, requestMessage?.Trace ?? (ITrace) NoOpTrace.Singleton);
      if (requestMessage?.Trace != null)
      {
        PointOperationStatisticsTraceDatum statisticsTraceDatum = new PointOperationStatisticsTraceDatum(cosmosException.Headers.ActivityId, cosmosException.StatusCode, cosmosException.Headers.SubStatusCode, DateTime.UtcNow, cosmosException.Headers.RequestCharge, documentClientException.ToString(), requestMessage?.Method, requestMessage?.RequestUriString, requestMessage?.Headers?.Session, cosmosException.Headers.Session, cosmosException.Headers.BackendRequestDurationMilliseconds);
        requestMessage?.Trace.AddDatum("Point Operation Statistics", (TraceDatum) statisticsTraceDatum);
      }
      if (!documentClientException.StatusCode.HasValue)
        throw cosmosException;
      ResponseMessage cosmosResponseMessage = cosmosException.ToCosmosResponseMessage(requestMessage);
      if (requestMessage == null)
        return cosmosResponseMessage;
      requestMessage.Properties.Remove("DocumentClientException");
      requestMessage.Properties.Add("DocumentClientException", (object) documentClientException);
      return cosmosResponseMessage;
    }

    internal static void TraceException(Exception exception)
    {
      if (exception is AggregateException aggregateException)
      {
        foreach (Exception innerException in aggregateException.InnerExceptions)
          Extensions.TraceExceptionInternal(innerException);
      }
      else
        Extensions.TraceExceptionInternal(exception);
    }

    public static async Task<IDisposable> UsingWaitAsync(
      this SemaphoreSlim semaphoreSlim,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      IDisposable disposable;
      using (trace.StartChild("Using Wait"))
      {
        await semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
        disposable = (IDisposable) new UsableSemaphoreWrapper(semaphoreSlim);
      }
      return disposable;
    }

    private static void TraceExceptionInternal(Exception exception)
    {
      for (; exception != null; exception = exception.InnerException)
      {
        Uri uri = (Uri) null;
        if (exception is SocketException socketException)
          DefaultTrace.TraceWarning("Exception {0}: RequesteUri: {1}, SocketErrorCode: {2}, {3}, {4}", (object) exception.GetType(), (object) uri, (object) socketException.SocketErrorCode, (object) exception.Message, (object) exception.StackTrace);
        else
          DefaultTrace.TraceWarning("Exception {0}: RequestUri: {1}, {2}, {3}", (object) exception.GetType(), (object) uri, (object) exception.Message, (object) exception.StackTrace);
      }
    }
  }
}
