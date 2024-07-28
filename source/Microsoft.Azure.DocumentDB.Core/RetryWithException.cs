// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RetryWithException
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
  internal sealed class RetryWithException : DocumentClientException
  {
    public RetryWithException(string retryMessage)
      : this(retryMessage, (INameValueCollection) null)
    {
    }

    public RetryWithException(Exception innerException)
      : base(RMResources.RetryWith, innerException, (HttpResponseHeaders) null, new HttpStatusCode?((HttpStatusCode) 449))
    {
    }

    public RetryWithException(string retryMessage, HttpResponseHeaders headers, Uri requestUri = null)
      : base(retryMessage, (Exception) null, headers, new HttpStatusCode?((HttpStatusCode) 449), requestUri)
    {
      this.SetDescription();
    }

    public RetryWithException(string retryMessage, INameValueCollection headers, Uri requestUri = null)
      : base(retryMessage, (Exception) null, headers, new HttpStatusCode?((HttpStatusCode) 449), requestUri)
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Retry the request";
  }
}
