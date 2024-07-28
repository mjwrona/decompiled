// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RetryWithException
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

    private RetryWithException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?((HttpStatusCode) 449))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Retry the request";
  }
}
