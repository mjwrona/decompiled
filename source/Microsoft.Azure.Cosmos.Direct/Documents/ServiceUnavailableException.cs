// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ServiceUnavailableException
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents
{
  [Serializable]
  internal sealed class ServiceUnavailableException : DocumentClientException
  {
    public static ServiceUnavailableException Create(
      SubStatusCodes? subStatusCode,
      Exception innerException = null,
      HttpResponseHeaders headers = null,
      Uri requestUri = null)
    {
      return new ServiceUnavailableException(ServiceUnavailableException.GetExceptionMessage(subStatusCode), innerException, headers, subStatusCode);
    }

    public static ServiceUnavailableException Create(
      INameValueCollection headers,
      SubStatusCodes? subStatusCode,
      Uri requestUri = null)
    {
      return new ServiceUnavailableException(ServiceUnavailableException.GetExceptionMessage(subStatusCode), headers, subStatusCode, requestUri);
    }

    public ServiceUnavailableException()
      : this(RMResources.ServiceUnavailable, (Exception) null, (HttpResponseHeaders) null, new SubStatusCodes?(SubStatusCodes.Unknown))
    {
    }

    public ServiceUnavailableException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null, new SubStatusCodes?(SubStatusCodes.Unknown))
    {
    }

    public ServiceUnavailableException(
      string message,
      SubStatusCodes subStatusCode,
      Uri requestUri = null)
      : this(message, (Exception) null, (HttpResponseHeaders) null, new SubStatusCodes?(subStatusCode), requestUri)
    {
    }

    public ServiceUnavailableException(
      string message,
      Exception innerException,
      SubStatusCodes subStatusCode,
      Uri requestUri = null)
      : this(message, innerException, (HttpResponseHeaders) null, new SubStatusCodes?(subStatusCode), requestUri)
    {
    }

    public ServiceUnavailableException(
      string message,
      HttpResponseHeaders headers,
      SubStatusCodes? subStatusCode,
      Uri requestUri = null)
      : this(message, (Exception) null, headers, subStatusCode, requestUri)
    {
    }

    public ServiceUnavailableException(Exception innerException, SubStatusCodes subStatusCode)
      : this(RMResources.ServiceUnavailable, innerException, (HttpResponseHeaders) null, new SubStatusCodes?(subStatusCode))
    {
    }

    public ServiceUnavailableException(
      string message,
      INameValueCollection headers,
      SubStatusCodes? subStatusCode,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.ServiceUnavailable), subStatusCode, requestUri)
    {
      this.SetDescription();
    }

    public ServiceUnavailableException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      SubStatusCodes? subStatusCode,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.ServiceUnavailable), requestUri, subStatusCode)
    {
      this.SetDescription();
    }

    private ServiceUnavailableException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.ServiceUnavailable))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Service Unavailable";

    private static string GetExceptionMessage(SubStatusCodes? subStatusCode)
    {
      if (subStatusCode.HasValue)
      {
        switch (subStatusCode.GetValueOrDefault())
        {
          case SubStatusCodes.TransportGenerated410:
            return RMResources.TransportGenerated410;
          case SubStatusCodes.TimeoutGenerated410:
            return RMResources.TimeoutGenerated410;
          case SubStatusCodes.TransportGenerated503:
            return RMResources.TransportGenerated503;
          case SubStatusCodes.Client_CPUOverload:
            return RMResources.Client_CPUOverload;
          case SubStatusCodes.Client_ThreadStarvation:
            return RMResources.Client_ThreadStarvation;
          case SubStatusCodes.Channel_Closed:
            return RMResources.ChannelClosed;
          case SubStatusCodes.Server_NameCacheIsStaleExceededRetryLimit:
            return RMResources.Server_NameCacheIsStaleExceededRetryLimit;
          case SubStatusCodes.Server_PartitionKeyRangeGoneExceededRetryLimit:
            return RMResources.Server_PartitionKeyRangeGoneExceededRetryLimit;
          case SubStatusCodes.Server_CompletingSplitExceededRetryLimit:
            return RMResources.Server_CompletingSplitExceededRetryLimit;
          case SubStatusCodes.Server_CompletingPartitionMigrationExceededRetryLimit:
            return RMResources.Server_CompletingPartitionMigrationExceededRetryLimit;
          case SubStatusCodes.ServerGenerated410:
            return RMResources.ServerGenerated410;
          case SubStatusCodes.Server_GlobalStrongWriteBarrierNotMet:
            return RMResources.Server_GlobalStrongWriteBarrierNotMet;
          case SubStatusCodes.Server_ReadQuorumNotMet:
            return RMResources.Server_ReadQuorumNotMet;
          case SubStatusCodes.ServerGenerated503:
            return RMResources.ServerGenerated503;
          case SubStatusCodes.Server_NoValidStoreResponse:
            return RMResources.Server_NoValidStoreResponse;
        }
      }
      return RMResources.ServiceUnavailable;
    }
  }
}
