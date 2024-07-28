// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UnauthorizedException
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
  internal sealed class UnauthorizedException : DocumentClientException
  {
    public UnauthorizedException()
      : this(RMResources.Unauthorized)
    {
    }

    public UnauthorizedException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public UnauthorizedException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public UnauthorizedException(Exception innerException)
      : this(RMResources.Unauthorized, innerException, (HttpResponseHeaders) null)
    {
    }

    public UnauthorizedException(string message, INameValueCollection headers, Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Unauthorized), requestUri)
    {
      this.SetDescription();
    }

    public UnauthorizedException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Unauthorized), requestUri)
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Unauthorized";
  }
}
