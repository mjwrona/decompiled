// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestEntityTooLargeException
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
  internal sealed class RequestEntityTooLargeException : DocumentClientException
  {
    public RequestEntityTooLargeException()
      : this(RMResources.RequestEntityTooLarge)
    {
    }

    public RequestEntityTooLargeException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public RequestEntityTooLargeException(
      string message,
      HttpResponseHeaders httpHeaders,
      Uri requestUri = null)
      : this(message, (Exception) null, httpHeaders, requestUri)
    {
    }

    public RequestEntityTooLargeException(Exception innerException)
      : this(RMResources.RequestEntityTooLarge, innerException, (HttpResponseHeaders) null)
    {
    }

    public RequestEntityTooLargeException(
      string message,
      INameValueCollection headers,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.RequestEntityTooLarge), requestUri)
    {
      this.SetDescription();
    }

    public RequestEntityTooLargeException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.RequestEntityTooLarge), requestUri)
    {
      this.SetDescription();
    }

    private RequestEntityTooLargeException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.RequestEntityTooLarge))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Request Entity Too Large";
  }
}
