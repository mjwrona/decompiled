// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.InternalServerErrorException
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
  internal sealed class InternalServerErrorException : DocumentClientException
  {
    public InternalServerErrorException()
      : this(RMResources.InternalServerError)
    {
    }

    public InternalServerErrorException(SubStatusCodes subStatusCode)
      : base(RMResources.InternalServerError, HttpStatusCode.InternalServerError, subStatusCode)
    {
    }

    public InternalServerErrorException(string message, Uri requestUri = null)
      : this(message, (Exception) null, (HttpResponseHeaders) null, requestUri)
    {
    }

    public InternalServerErrorException(
      string message,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public InternalServerErrorException(Exception innerException)
      : this(RMResources.InternalServerError, innerException)
    {
    }

    public InternalServerErrorException(string message, Exception innerException, Uri requestUri = null)
      : this(message, innerException, (HttpResponseHeaders) null, requestUri)
    {
    }

    public InternalServerErrorException(
      string message,
      INameValueCollection headers,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.InternalServerError), requestUri)
    {
      this.SetDescription();
    }

    public InternalServerErrorException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.InternalServerError), requestUri)
    {
      this.SetDescription();
    }

    private InternalServerErrorException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.InternalServerError))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Internal Server Error";
  }
}
