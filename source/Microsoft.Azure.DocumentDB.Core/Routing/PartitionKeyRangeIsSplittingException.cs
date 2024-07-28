// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionKeyRangeIsSplittingException
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Documents.Routing
{
  [Serializable]
  internal sealed class PartitionKeyRangeIsSplittingException : DocumentClientException
  {
    public PartitionKeyRangeIsSplittingException()
      : this(RMResources.Gone)
    {
    }

    public PartitionKeyRangeIsSplittingException(string message)
      : this(message, (Exception) null, (HttpResponseHeaders) null)
    {
    }

    public PartitionKeyRangeIsSplittingException(
      string message,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : this(message, (Exception) null, headers, requestUri)
    {
    }

    public PartitionKeyRangeIsSplittingException(string message, Exception innerException)
      : this(message, innerException, (HttpResponseHeaders) null)
    {
    }

    public PartitionKeyRangeIsSplittingException(Exception innerException)
      : this(RMResources.Gone, innerException, (HttpResponseHeaders) null)
    {
    }

    public PartitionKeyRangeIsSplittingException(
      string message,
      INameValueCollection headers,
      Uri requestUri = null)
      : base(message, (Exception) null, headers, new HttpStatusCode?(HttpStatusCode.Gone), requestUri)
    {
      this.SetSubstatus();
      this.SetDescription();
    }

    public PartitionKeyRangeIsSplittingException(
      string message,
      Exception innerException,
      HttpResponseHeaders headers,
      Uri requestUri = null)
      : base(message, innerException, headers, new HttpStatusCode?(HttpStatusCode.Gone), requestUri)
    {
      this.SetSubstatus();
      this.SetDescription();
    }

    private void SetDescription() => this.StatusDescription = "InvalidPartition";

    private void SetSubstatus() => this.Headers["x-ms-substatus"] = 1007U.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
