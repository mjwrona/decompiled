// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreResponse
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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

    public string[] ResponseHeaderNames { get; set; }

    public string[] ResponseHeaderValues { get; set; }

    public Stream ResponseBody { get; set; }

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

    public bool TryGetHeaderValue(string attribute, out string value, StringComparison comparator = StringComparison.OrdinalIgnoreCase) => this.TryGetHeaderValue(attribute, out value, out int? _);

    public void UpsertHeaderValue(string headerName, string headerValue)
    {
      int? attributePosition;
      if (this.TryGetHeaderValue(headerName, out string _, out attributePosition))
      {
        this.ResponseHeaderValues[attributePosition.Value] = headerValue;
      }
      else
      {
        string[] responseHeaderNames = this.ResponseHeaderNames;
        Array.Resize<string>(ref responseHeaderNames, this.ResponseHeaderNames.Length + 1);
        responseHeaderNames[responseHeaderNames.Length - 1] = headerName;
        string[] responseHeaderValues = this.ResponseHeaderValues;
        Array.Resize<string>(ref responseHeaderValues, this.ResponseHeaderValues.Length + 1);
        responseHeaderValues[responseHeaderValues.Length - 1] = headerValue;
        this.ResponseHeaderNames = responseHeaderNames;
        this.ResponseHeaderValues = responseHeaderValues;
      }
    }

    private bool TryGetHeaderValue(
      string attribute,
      out string value,
      out int? attributePosition,
      StringComparison comparator = StringComparison.OrdinalIgnoreCase)
    {
      value = (string) null;
      attributePosition = new int?();
      if (this.ResponseHeaderNames == null || this.ResponseHeaderValues == null || this.ResponseHeaderNames.Length != this.ResponseHeaderValues.Length)
        return false;
      for (int index = 0; index < this.ResponseHeaderNames.Length; ++index)
      {
        if (string.Equals(this.ResponseHeaderNames[index], attribute, comparator))
        {
          value = this.ResponseHeaderValues[index];
          attributePosition = new int?(index);
          return true;
        }
      }
      return false;
    }

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
