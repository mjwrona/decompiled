// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.NotFoundException
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
  internal sealed class NotFoundException : DocumentClientException
  {
    public NotFoundException()
      : this(RMResources.NotFound)
    {
    }

    public NotFoundException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public NotFoundException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public NotFoundException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public NotFoundException(Exception innerException)
      : this(RMResources.NotFound, innerException, (HttpResponseHeaders) null)
    {
    }

    public NotFoundException(Exception innerException, SubStatusCodes subStatusCode)
      : this(RMResources.NotFound, innerException, (INameValueCollection) null, new SubStatusCodes?(subStatusCode))
    {
    }

    public NotFoundException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.NotFound), requestUri)
    {
      this.SetDescription();
    }

    public NotFoundException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null,
      SubStatusCodes? subStatusCode = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.NotFound), requestUri, subStatusCode)
    {
      this.SetDescription();
    }

    public NotFoundException(
      string message,
      Exception innerException,
      INameValueCollection headers,
      SubStatusCodes? subStatusCode)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.NotFound), subStatusCode)
    {
      this.SetDescription();
    }

    private NotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.NotFound))
    {
    }

    private void SetDescription() => this.StatusDescription = "Not Found";
  }
}
