// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreResponse
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Documents
{
  internal sealed class StoreResponse : IRetriableResponse
  {
    private Lazy<SubStatusCodes> subStatusCode;

    public int Status { get; set; }

    public INameValueCollection Headers { get; set; }

    public Stream ResponseBody { get; set; }

    public TransportRequestStats TransportRequestStats { get; set; }

    public long LSN
    {
      get
      {
        long result = -1;
        string s;
        return this.TryGetHeaderValue("lsn", out s) && long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : -1L;
      }
    }

    public string PartitionKeyRangeId
    {
      get
      {
        string str;
        return this.TryGetHeaderValue("x-ms-documentdb-partitionkeyrangeid", out str) ? str : (string) null;
      }
    }

    public long CollectionPartitionIndex
    {
      get
      {
        long result = -1;
        string s;
        return this.TryGetHeaderValue("collection-partition-index", out s) && long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : -1L;
      }
    }

    public long CollectionServiceIndex
    {
      get
      {
        long result = -1;
        string s;
        return this.TryGetHeaderValue("collection-service-index", out s) && long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : -1L;
      }
    }

    public string Continuation
    {
      get
      {
        string str;
        return this.TryGetHeaderValue("x-ms-continuation", out str) ? str : (string) null;
      }
    }

    public SubStatusCodes SubStatusCode => this.subStatusCode.Value;

    public HttpStatusCode StatusCode => (HttpStatusCode) this.Status;

    public StoreResponse() => this.subStatusCode = new Lazy<SubStatusCodes>(new Func<SubStatusCodes>(this.GetSubStatusCode));

    public bool TryGetHeaderValue(string attribute, out string value)
    {
      value = (string) null;
      if (this.Headers == null)
        return false;
      value = this.Headers.Get(attribute);
      return value != null;
    }

    public void UpsertHeaderValue(string headerName, string headerValue) => this.Headers[headerName] = headerValue;

    private SubStatusCodes GetSubStatusCode()
    {
      SubStatusCodes subStatusCode = SubStatusCodes.Unknown;
      string s;
      if (this.TryGetHeaderValue("x-ms-substatus", out s))
      {
        uint result = 0;
        if (uint.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          subStatusCode = (SubStatusCodes) result;
      }
      return subStatusCode;
    }
  }
}
