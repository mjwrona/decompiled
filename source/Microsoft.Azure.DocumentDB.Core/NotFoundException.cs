// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.NotFoundException
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Net;
using System.Net.Http.Headers;

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

    private void SetDescription() => this.StatusDescription = "Not Found";
  }
}
