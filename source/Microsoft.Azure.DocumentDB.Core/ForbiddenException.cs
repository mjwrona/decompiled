// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ForbiddenException
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;

namespace Microsoft.Azure.Documents
{
  [Serializable]
  internal sealed class ForbiddenException : DocumentClientException
  {
    public ForbiddenException()
      : this(RMResources.Forbidden)
    {
    }

    public static ForbiddenException CreateWithClientIpAddress(IPAddress clientIpAddress) => clientIpAddress.AddressFamily == AddressFamily.InterNetworkV6 ? new ForbiddenException() : new ForbiddenException(clientIpAddress);

    private ForbiddenException(IPAddress clientIpAddress)
      : this(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.ForbiddenClientIpAddress, (object) clientIpAddress.ToString()))
    {
      this.ClientIpAddress = clientIpAddress;
    }

    public ForbiddenException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public ForbiddenException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public ForbiddenException(Exception innerException)
      : this(RMResources.Forbidden, innerException, (HttpResponseHeaders) null)
    {
    }

    public ForbiddenException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public ForbiddenException(string message, SubStatusCodes subStatusCode)
      : base(message, HttpStatusCode.Forbidden, subStatusCode)
    {
      this.SetDescription();
    }

    public ForbiddenException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Forbidden), requestUri)
    {
      this.SetDescription();
    }

    public ForbiddenException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Forbidden), requestUri)
    {
      this.SetDescription();
    }

    public IPAddress ClientIpAddress { get; private set; }

    private void SetDescription() => this.StatusDescription = "Forbidden";
  }
}
