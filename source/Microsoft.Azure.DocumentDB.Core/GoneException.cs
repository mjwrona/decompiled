// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GoneException
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Documents
{
  [Serializable]
  internal sealed class GoneException : DocumentClientException
  {
    public GoneException()
      : this(RMResources.Gone)
    {
    }

    public GoneException(string message, Uri requestUri = null)
      : this(message, (Exception) null, (HttpResponseHeaders) null, requestUri)
    {
    }

    public GoneException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public GoneException(
      string message,
      Exception innerException,
      Uri requestUri = null,
      string localIpAddress = null)
      : this(message, innerException, (HttpResponseHeaders) null, requestUri)
    {
      this.LocalIp = localIpAddress;
    }

    public GoneException(Exception innerException)
      : this(RMResources.Gone, innerException, (HttpResponseHeaders) null)
    {
    }

    public GoneException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Gone), requestUri)
    {
      this.SetDescription();
    }

    public GoneException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Gone), requestUri)
    {
      this.SetDescription();
    }

    internal string LocalIp { get; set; }

    public override string Message => !string.IsNullOrEmpty(this.LocalIp) ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddIpAddress, (object) base.Message, (object) this.LocalIp) : base.Message;

    private void SetDescription() => this.StatusDescription = "Gone";
  }
}
