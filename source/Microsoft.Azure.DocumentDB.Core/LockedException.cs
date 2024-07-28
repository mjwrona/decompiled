// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.LockedException
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
  internal sealed class LockedException : DocumentClientException
  {
    public LockedException()
      : this(RMResources.Locked)
    {
    }

    public LockedException(string message, SubStatusCodes subStatusCode)
      : base(message, (HttpStatusCode) 423, subStatusCode)
    {
    }

    public LockedException(string message, Uri requestUri = null)
      : this(message, (Exception) null, (HttpResponseHeaders) null, requestUri)
    {
    }

    public LockedException(string message, Exception innerException, Uri requestUri = null)
      : this(message, innerException, (HttpResponseHeaders) null, requestUri)
    {
    }

    public LockedException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public LockedException(Exception innerException)
      : this(RMResources.Locked, innerException)
    {
    }

    public LockedException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?((HttpStatusCode) 423), requestUri)
    {
      this.SetDescription();
    }

    public LockedException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?((HttpStatusCode) 423), requestUri)
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Locked";
  }
}
