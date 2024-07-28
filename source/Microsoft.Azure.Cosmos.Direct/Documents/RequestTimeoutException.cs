// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestTimeoutException
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents
{
  [Serializable]
  internal sealed class RequestTimeoutException : DocumentClientException
  {
    public RequestTimeoutException()
      : this(RMResources.RequestTimeout)
    {
    }

    public RequestTimeoutException(string message, Uri requestUri = null)
      : this(message, (Exception) null, (HttpResponseHeaders) null, requestUri)
    {
    }

    public RequestTimeoutException(string message, Exception innerException, Uri requestUri = null)
      : this(message, innerException, (HttpResponseHeaders) null, requestUri)
    {
    }

    public RequestTimeoutException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public RequestTimeoutException(Exception innerException, Uri requestUri = null)
      : this(RMResources.RequestTimeout, innerException, requestUri)
    {
    }

    public RequestTimeoutException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.RequestTimeout), requestUri)
    {
      this.SetDescription();
    }

    public RequestTimeoutException(
      string message,
      Exception innerException,
      Uri requestUri = null,
      string localIpAddress = null)
      : this(message, innerException, (HttpResponseHeaders) null, requestUri)
    {
      this.LocalIp = localIpAddress;
    }

    public RequestTimeoutException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.RequestTimeout), requestUri)
    {
      this.SetDescription();
    }

    public override string Message => !string.IsNullOrEmpty(this.LocalIp) ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddIpAddress, (object) base.Message, (object) this.LocalIp) : base.Message;

    internal string LocalIp { get; set; }

    private RequestTimeoutException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.RequestTimeout))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Request timed out";
  }
}
