// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GoneException
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
  internal sealed class GoneException : DocumentClientException
  {
    public GoneException()
      : this(RMResources.Gone, SubStatusCodes.Unknown)
    {
    }

    public GoneException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null, new SubStatusCodes?(SubStatusCodes.Unknown))
    {
    }

    public GoneException(string message, SubStatusCodes subStatusCode, Uri requestUri = null)
      : this(message, (Exception) null, (HttpResponseHeaders) null, new SubStatusCodes?(subStatusCode), requestUri)
    {
    }

    public GoneException(
      string message,
      HttpResponseHeaders headers,
      SubStatusCodes? subStatusCode,
      Uri requestUri = null)
      : this(message, (Exception) null, headers, subStatusCode, requestUri)
    {
    }

    public GoneException(
      string message,
      Exception innerException,
      SubStatusCodes subStatusCode,
      Uri requestUri = null,
      string localIpAddress = null)
      : this(message, innerException, (HttpResponseHeaders) null, new SubStatusCodes?(subStatusCode), requestUri)
    {
      this.LocalIp = localIpAddress;
    }

    public GoneException(Exception innerException, SubStatusCodes subStatusCode)
      : this(RMResources.Gone, innerException, (HttpResponseHeaders) null, new SubStatusCodes?(subStatusCode))
    {
    }

    public GoneException(
      string message,
      INameValueCollection headers,
      SubStatusCodes? substatusCode,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Gone), substatusCode, requestUri)
    {
      this.SetDescription();
    }

    public GoneException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      SubStatusCodes? subStatusCode,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Gone), requestUri, subStatusCode)
    {
      this.SetDescription();
    }

    internal string LocalIp { get; set; }

    private GoneException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.Gone))
    {
    }

    public override string Message => !string.IsNullOrEmpty(this.LocalIp) ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddIpAddress, (object) base.Message, (object) this.LocalIp) : base.Message;

    private void SetDescription() => this.StatusDescription = "Gone";
  }
}
