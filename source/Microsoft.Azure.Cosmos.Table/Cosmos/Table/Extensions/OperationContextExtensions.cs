// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.OperationContextExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class OperationContextExtensions
  {
    internal static RequestResult ToRequestResult<T>(this ResourceResponse<T> response) where T : Resource, new() => new RequestResult()
    {
      HttpStatusCode = 202,
      HttpStatusMessage = (string) null,
      ServiceRequestID = response.ActivityId,
      Etag = (string) null,
      RequestDate = (string) null,
      RequestCharge = new double?(response.RequestCharge)
    };

    internal static RequestResult ToRequestResult<T>(this FeedResponse<T> response) => new RequestResult()
    {
      HttpStatusCode = 202,
      HttpStatusMessage = (string) null,
      ServiceRequestID = response.ActivityId,
      Etag = (string) null,
      RequestDate = (string) null,
      RequestCharge = new double?(response.RequestCharge)
    };

    internal static RequestResult ToRequestResult(
      this StorageException storageException,
      string serviceRequestId)
    {
      return new RequestResult()
      {
        Exception = (Exception) storageException,
        ExtendedErrorInformation = storageException.RequestInformation.ExtendedErrorInformation,
        HttpStatusCode = storageException.RequestInformation.HttpStatusCode,
        HttpStatusMessage = storageException.RequestInformation.HttpStatusMessage,
        ServiceRequestID = serviceRequestId,
        RequestCharge = storageException.RequestInformation.RequestCharge
      };
    }
  }
}
