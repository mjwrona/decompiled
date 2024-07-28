// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ForbiddenException
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents
{
  [Serializable]
  internal sealed class ForbiddenException : DocumentClientException
  {
    public ForbiddenException()
      : this(RMResources.Forbidden)
    {
    }

    public static ForbiddenException CreateWithClientIpAddress(
      IPAddress clientIpAddress,
      bool isPrivateIpPacket)
    {
      ForbiddenException withClientIpAddress;
      if (clientIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
      {
        withClientIpAddress = new ForbiddenException(isPrivateIpPacket ? RMResources.ForbiddenPrivateEndpoint : RMResources.ForbiddenServiceEndpoint);
      }
      else
      {
        withClientIpAddress = new ForbiddenException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.ForbiddenPublicIpv4, (object) clientIpAddress.ToString()));
        withClientIpAddress.ClientIpAddress = clientIpAddress;
      }
      return withClientIpAddress;
    }

    public ForbiddenException(string message)
      : this(message, (Exception) null, (INameValueCollection) null)
    {
    }

    public ForbiddenException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public ForbiddenException(Exception innerException)
      : this(RMResources.Forbidden, innerException, (INameValueCollection) null)
    {
    }

    public ForbiddenException(string message, Exception innerException)
      : this(message, innerException, (INameValueCollection) null)
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

    public ForbiddenException(
      string message,
      Exception innerException,
      INameValueCollection headers)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Forbidden))
    {
      this.SetDescription();
    }

    public IPAddress ClientIpAddress { get; private set; }

    private ForbiddenException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.Forbidden))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Forbidden";
  }
}
