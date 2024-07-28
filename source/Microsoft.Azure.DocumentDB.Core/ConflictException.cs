// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConflictException
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
  internal sealed class ConflictException : DocumentClientException
  {
    public ConflictException()
      : this(RMResources.EntityAlreadyExists)
    {
    }

    public ConflictException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public ConflictException(string message, SubStatusCodes subStatusCode)
      : base(message, HttpStatusCode.Conflict, subStatusCode)
    {
    }

    public ConflictException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public ConflictException(Exception innerException)
      : this(RMResources.EntityAlreadyExists, innerException, (HttpResponseHeaders) null)
    {
    }

    public ConflictException(Exception innerException, SubStatusCodes subStatusCode)
      : this(RMResources.EntityAlreadyExists, innerException, (HttpResponseHeaders) null, subStatusCode: new SubStatusCodes?(subStatusCode))
    {
    }

    public ConflictException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public ConflictException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Conflict), requestUri)
    {
      this.SetDescription();
    }

    public ConflictException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null,
      SubStatusCodes? subStatusCode = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Conflict), requestUri, subStatusCode)
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Conflict";
  }
}
