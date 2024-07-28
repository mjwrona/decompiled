// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionIsMigratingException
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents.Routing
{
  [Serializable]
  internal sealed class PartitionIsMigratingException : DocumentClientException
  {
    public PartitionIsMigratingException()
      : this(RMResources.Gone)
    {
    }

    public PartitionIsMigratingException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public PartitionIsMigratingException(
      string message,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public PartitionIsMigratingException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public PartitionIsMigratingException(Exception innerException)
      : this(RMResources.Gone, innerException, (HttpResponseHeaders) null)
    {
    }

    public PartitionIsMigratingException(
      string message,
      INameValueCollection headers,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Gone), requestUri)
    {
      this.SetSubstatus();
      this.SetDescription();
    }

    public PartitionIsMigratingException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Gone), requestUri)
    {
      this.SetSubstatus();
      this.SetDescription();
    }

    private PartitionIsMigratingException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?(HttpStatusCode.Gone))
    {
      this.SetSubstatus();
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "Partition is migrating";

    private void SetSubstatus() => this.Headers["x-ms-substatus"] = 1008U.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
