// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PreconditionFailedException
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
  internal sealed class PreconditionFailedException : DocumentClientException
  {
    public PreconditionFailedException()
      : this(RMResources.PreconditionFailed)
    {
    }

    public PreconditionFailedException(string message, SubStatusCodes? substatusCode = null)
      : this(message, (Exception) null, (HttpResponseHeaders) null, substatusCode: substatusCode)
    {
    }

    public PreconditionFailedException(string message, HttpResponseHeaders headers, Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public PreconditionFailedException(Exception innerException)
      : this(RMResources.PreconditionFailed, innerException, (HttpResponseHeaders) null)
    {
    }

    public PreconditionFailedException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public PreconditionFailedException(
      string message,
      INameValueCollection headers,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.PreconditionFailed), requestUri)
    {
      this.SetDescription();
    }

    public PreconditionFailedException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null,
      SubStatusCodes? substatusCode = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.PreconditionFailed), requestUri, substatusCode)
    {
      this.SetDescription();
    }

    private PreconditionFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.PreconditionFailed))
    {
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Precondition Failed";
  }
}
