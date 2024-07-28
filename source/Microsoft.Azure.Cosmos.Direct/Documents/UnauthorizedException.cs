// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UnauthorizedException
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
  internal sealed class UnauthorizedException : DocumentClientException
  {
    public UnauthorizedException()
      : this(RMResources.Unauthorized)
    {
    }

    public UnauthorizedException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public UnauthorizedException(string message, SubStatusCodes subStatusCode)
      : this(message, (Exception) null, (HttpResponseHeaders) null, subStatusCode: new SubStatusCodes?(subStatusCode))
    {
    }

    public UnauthorizedException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public UnauthorizedException(Exception innerException)
      : this(RMResources.Unauthorized, innerException, (HttpResponseHeaders) null)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public UnauthorizedException(
      string message,
      Exception innerException,
      SubStatusCodes subStatusCode)
      : this(message, innerException, (HttpResponseHeaders) null, subStatusCode: new SubStatusCodes?(subStatusCode))
    {
    }

    public UnauthorizedException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Unauthorized), requestUri)
    {
      this.SetDescription();
    }

    public UnauthorizedException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null,
      SubStatusCodes? subStatusCode = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Unauthorized), requestUri, subStatusCode)
    {
      this.SetDescription();
    }

    private UnauthorizedException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.Unauthorized))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Unauthorized";
  }
}
