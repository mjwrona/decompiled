// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestRateTooLargeException
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
  internal sealed class RequestRateTooLargeException : DocumentClientException
  {
    public RequestRateTooLargeException()
      : this(RMResources.TooManyRequests)
    {
    }

    public RequestRateTooLargeException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public RequestRateTooLargeException(
      string message,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public RequestRateTooLargeException(string message, SubStatusCodes subStatus)
      : base(message, (HttpStatusCode) 429, subStatus)
    {
    }

    public RequestRateTooLargeException(Exception innerException)
      : this(RMResources.TooManyRequests, innerException, (HttpResponseHeaders) null)
    {
    }

    public RequestRateTooLargeException(
      string message,
      INameValueCollection headers,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?((HttpStatusCode) 429), requestUri)
    {
      this.SetDescription();
    }

    public RequestRateTooLargeException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?((HttpStatusCode) 429), requestUri)
    {
      this.SetDescription();
    }

    private RequestRateTooLargeException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?((HttpStatusCode) 429))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Too Many Requests";
  }
}
