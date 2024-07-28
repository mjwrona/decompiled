// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BadRequestException
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
  internal sealed class BadRequestException : DocumentClientException
  {
    public BadRequestException()
      : this(RMResources.BadRequest)
    {
    }

    public BadRequestException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public BadRequestException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public BadRequestException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.BadRequest), requestUri)
    {
      this.SetDescription();
    }

    public BadRequestException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public BadRequestException(Exception innerException)
      : this(RMResources.BadRequest, innerException, (HttpResponseHeaders) null)
    {
    }

    public BadRequestException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.BadRequest), requestUri)
    {
      this.SetDescription();
    }

    private BadRequestException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.BadRequest))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Bad Request";
  }
}
