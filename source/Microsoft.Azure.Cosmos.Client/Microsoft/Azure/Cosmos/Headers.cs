// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Headers
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  public class Headers : IEnumerable
  {
    internal static readonly string SDKSUPPORTEDCAPABILITIES = SDKSupportedCapabilitiesHelpers.GetSDKSupportedCapabilities().ToString((IFormatProvider) CultureInfo.InvariantCulture);

    internal virtual SubStatusCodes SubStatusCode
    {
      get => Headers.GetSubStatusCodes(this.SubStatusCodeLiteral);
      set => this.SubStatusCodeLiteral = ((uint) value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public virtual string ContinuationToken
    {
      get => this.CosmosMessageHeaders.Continuation;
      internal set => this.CosmosMessageHeaders.Continuation = value;
    }

    public virtual double RequestCharge
    {
      get
      {
        string requestCharge = this.CosmosMessageHeaders.RequestCharge;
        return requestCharge == null ? 0.0 : double.Parse(requestCharge, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      internal set => this.CosmosMessageHeaders.RequestCharge = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public virtual string ActivityId
    {
      get => this.CosmosMessageHeaders.ActivityId;
      internal set => this.CosmosMessageHeaders.ActivityId = value;
    }

    public virtual string ETag
    {
      get => this.CosmosMessageHeaders.ETag;
      internal set => this.CosmosMessageHeaders.ETag = value;
    }

    public virtual string ContentType
    {
      get => this.CosmosMessageHeaders.ContentType;
      internal set => this.CosmosMessageHeaders.ContentType = value;
    }

    public virtual string Session
    {
      get => this.CosmosMessageHeaders.SessionToken;
      internal set => this.CosmosMessageHeaders.SessionToken = value;
    }

    public virtual string ContentLength
    {
      get => this.CosmosMessageHeaders.ContentLength;
      set => this.CosmosMessageHeaders.ContentLength = value;
    }

    public virtual string Location
    {
      get => this.CosmosMessageHeaders.Location;
      internal set => this.CosmosMessageHeaders.Location = value;
    }

    internal virtual string SubStatusCodeLiteral
    {
      get => this.CosmosMessageHeaders.SubStatus;
      set => this.CosmosMessageHeaders.SubStatus = value;
    }

    internal TimeSpan? RetryAfter
    {
      get => Headers.GetRetryAfter(this.RetryAfterLiteral);
      set
      {
        if (value.HasValue)
          this.RetryAfterLiteral = value.Value.TotalMilliseconds.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        else
          this.RetryAfterLiteral = (string) null;
      }
    }

    internal virtual string Authorization
    {
      get => this.CosmosMessageHeaders.Authorization;
      set => this.CosmosMessageHeaders.Authorization = value;
    }

    internal virtual string RetryAfterLiteral
    {
      get => this.CosmosMessageHeaders.RetryAfterInMilliseconds;
      set => this.CosmosMessageHeaders.RetryAfterInMilliseconds = value;
    }

    internal virtual string PartitionKey
    {
      get => this.CosmosMessageHeaders.PartitionKey;
      set => this.CosmosMessageHeaders.PartitionKey = value;
    }

    internal virtual string PartitionKeyRangeId
    {
      get => this.CosmosMessageHeaders.PartitionKeyRangeId;
      set => this.CosmosMessageHeaders.PartitionKeyRangeId = value;
    }

    internal virtual string IsUpsert
    {
      get => this.CosmosMessageHeaders.IsUpsert;
      set => this.CosmosMessageHeaders.IsUpsert = value;
    }

    internal virtual string OfferThroughput
    {
      get => this.CosmosMessageHeaders.OfferThroughput;
      set => this.CosmosMessageHeaders.OfferThroughput = value;
    }

    internal virtual string IfNoneMatch
    {
      get => this.CosmosMessageHeaders.IfNoneMatch;
      set => this.CosmosMessageHeaders.IfNoneMatch = value;
    }

    internal virtual string PageSize
    {
      get => this.CosmosMessageHeaders.PageSize;
      set => this.CosmosMessageHeaders.PageSize = value;
    }

    internal virtual string QueryMetricsText
    {
      get => this.CosmosMessageHeaders.QueryMetrics;
      set => this.CosmosMessageHeaders.QueryMetrics = value;
    }

    internal virtual string IndexUtilizationText
    {
      get => this.CosmosMessageHeaders.IndexUtilization;
      set => this.CosmosMessageHeaders.IndexUtilization = value;
    }

    internal virtual string BackendRequestDurationMilliseconds
    {
      get => this.CosmosMessageHeaders.BackendRequestDurationMilliseconds;
      set => this.CosmosMessageHeaders.BackendRequestDurationMilliseconds = value;
    }

    internal virtual string ConsistencyLevel
    {
      get => this.CosmosMessageHeaders.ConsistencyLevel;
      set => this.CosmosMessageHeaders.ConsistencyLevel = value;
    }

    internal virtual string SDKSupportedCapabilities
    {
      get => this.CosmosMessageHeaders.SDKSupportedCapabilities;
      set => this.CosmosMessageHeaders.SDKSupportedCapabilities = value;
    }

    internal virtual string ContentSerializationFormat
    {
      get => this.CosmosMessageHeaders.ContentSerializationFormat;
      set => this.CosmosMessageHeaders.ContentSerializationFormat = value;
    }

    internal virtual string ReadFeedKeyType
    {
      get => this.CosmosMessageHeaders.ReadFeedKeyType;
      set => this.CosmosMessageHeaders.ReadFeedKeyType = value;
    }

    internal virtual string StartEpk
    {
      get => this.CosmosMessageHeaders.StartEpk;
      set => this.CosmosMessageHeaders.StartEpk = value;
    }

    internal virtual string EndEpk
    {
      get => this.CosmosMessageHeaders.EndEpk;
      set => this.CosmosMessageHeaders.EndEpk = value;
    }

    internal virtual string ItemCount => this.CosmosMessageHeaders.Get("x-ms-item-count");

    public Headers() => this.CosmosMessageHeaders = (CosmosMessageHeadersInternal) new StoreRequestHeaders();

    internal Headers(INameValueCollection nameValueCollection)
    {
      CosmosMessageHeadersInternal messageHeadersInternal;
      switch (nameValueCollection)
      {
        case StoreResponseNameValueCollection storeResponseNameValueCollection:
          messageHeadersInternal = (CosmosMessageHeadersInternal) new StoreResponseHeaders(storeResponseNameValueCollection);
          break;
        case HttpResponseHeadersWrapper responseHeadersWrapper:
          messageHeadersInternal = (CosmosMessageHeadersInternal) responseHeadersWrapper;
          break;
        default:
          messageHeadersInternal = (CosmosMessageHeadersInternal) new NameValueResponseHeaders(nameValueCollection);
          break;
      }
      this.CosmosMessageHeaders = messageHeadersInternal;
    }

    public virtual string this[string headerName]
    {
      get => this.CosmosMessageHeaders[headerName];
      set => this.CosmosMessageHeaders[headerName] = value;
    }

    public virtual IEnumerator<string> GetEnumerator()
    {
      string[] strArray = this.CosmosMessageHeaders.AllKeys();
      for (int index = 0; index < strArray.Length; ++index)
        yield return strArray[index];
      strArray = (string[]) null;
    }

    public virtual void Add(string headerName, string value) => this.CosmosMessageHeaders.Add(headerName, value);

    public virtual void Add(string headerName, IEnumerable<string> values) => this.CosmosMessageHeaders.Add(headerName, values);

    public virtual void Set(string headerName, string value) => this.CosmosMessageHeaders.Set(headerName, value);

    public virtual string Get(string headerName) => this.CosmosMessageHeaders.Get(headerName);

    public virtual bool TryGetValue(string headerName, out string value) => this.CosmosMessageHeaders.TryGetValue(headerName, out value);

    public virtual string GetValueOrDefault(string headerName)
    {
      string str;
      return this.TryGetValue(headerName, out str) ? str : (string) null;
    }

    public virtual void Remove(string headerName) => this.CosmosMessageHeaders.Remove(headerName);

    public virtual string[] AllKeys() => this.CosmosMessageHeaders.AllKeys();

    public virtual T GetHeaderValue<T>(string headerName) => this.CosmosMessageHeaders.GetHeaderValue<T>(headerName);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    internal string[] GetValues(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if (this[key] == null)
        return (string[]) null;
      return new string[1]{ this[key] };
    }

    internal CosmosMessageHeadersInternal CosmosMessageHeaders { get; }

    internal static int GetIntValueOrDefault(string value)
    {
      int result;
      int.TryParse(value, out result);
      return result;
    }

    internal static SubStatusCodes GetSubStatusCodes(string value)
    {
      uint result;
      return uint.TryParse(value, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (SubStatusCodes) result : SubStatusCodes.Unknown;
    }

    internal static TimeSpan? GetRetryAfter(string value)
    {
      long result;
      return long.TryParse(value, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? new TimeSpan?(TimeSpan.FromMilliseconds((double) result)) : new TimeSpan?();
    }
  }
}
