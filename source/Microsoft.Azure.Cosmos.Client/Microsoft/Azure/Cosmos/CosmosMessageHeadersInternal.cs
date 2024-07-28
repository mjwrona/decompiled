// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosMessageHeadersInternal
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class CosmosMessageHeadersInternal
  {
    public virtual string Authorization
    {
      get => this.GetValueOrDefault("authorization");
      set => this.SetProperty("authorization", value);
    }

    public virtual string XDate
    {
      get => this.GetValueOrDefault("x-ms-date");
      set => this.SetProperty("x-ms-date", value);
    }

    public virtual string RequestCharge
    {
      get => this.GetValueOrDefault("x-ms-request-charge");
      set => this.SetProperty("x-ms-request-charge", value);
    }

    public virtual string ActivityId
    {
      get => this.GetValueOrDefault("x-ms-activity-id");
      set => this.SetProperty("x-ms-activity-id", value);
    }

    public virtual string ETag
    {
      get => this.GetValueOrDefault("etag");
      set => this.SetProperty("etag", value);
    }

    public virtual string ContentType
    {
      get => this.GetValueOrDefault("Content-Type");
      set => this.SetProperty("Content-Type", value);
    }

    public virtual string ContentLength
    {
      get => this.GetValueOrDefault("Content-Length");
      set => this.SetProperty("Content-Length", value);
    }

    public virtual string SubStatus
    {
      get => this.GetValueOrDefault("x-ms-substatus");
      set => this.SetProperty("x-ms-substatus", value);
    }

    public virtual string RetryAfterInMilliseconds
    {
      get => this.GetValueOrDefault("x-ms-retry-after-ms");
      set => this.SetProperty("x-ms-retry-after-ms", value);
    }

    public virtual string IsUpsert
    {
      get => this.GetValueOrDefault("x-ms-documentdb-is-upsert");
      set => this.SetProperty("x-ms-documentdb-is-upsert", value);
    }

    public virtual string OfferThroughput
    {
      get => this.GetValueOrDefault("x-ms-offer-throughput");
      set => this.SetProperty("x-ms-offer-throughput", value);
    }

    public virtual string QueryMetrics
    {
      get => this.GetValueOrDefault("x-ms-documentdb-query-metrics");
      set => this.SetProperty("x-ms-documentdb-query-metrics", value);
    }

    public virtual string IndexUtilization
    {
      get => this.GetValueOrDefault("x-ms-cosmos-index-utilization");
      set => this.SetProperty("x-ms-cosmos-index-utilization", value);
    }

    public virtual string BackendRequestDurationMilliseconds
    {
      get => this.GetValueOrDefault("x-ms-request-duration-ms");
      set => this.SetProperty("x-ms-request-duration-ms", value);
    }

    public virtual string Location
    {
      get => this.GetValueOrDefault(nameof (Location));
      set => this.SetProperty(nameof (Location), value);
    }

    public virtual string Continuation
    {
      get => this.GetValueOrDefault("x-ms-continuation");
      set => this.SetProperty("x-ms-continuation", value);
    }

    public virtual string SessionToken
    {
      get => this.GetValueOrDefault("x-ms-session-token");
      set => this.SetProperty("x-ms-session-token", value);
    }

    public virtual string PartitionKey
    {
      get => this.GetValueOrDefault("x-ms-documentdb-partitionkey");
      set => this.SetProperty("x-ms-documentdb-partitionkey", value);
    }

    public virtual string PartitionKeyRangeId
    {
      get => this.GetValueOrDefault("x-ms-documentdb-partitionkeyrangeid");
      set => this.SetProperty("x-ms-documentdb-partitionkeyrangeid", value);
    }

    public virtual string IfNoneMatch
    {
      get => this.GetValueOrDefault("If-None-Match");
      set => this.SetProperty("If-None-Match", value);
    }

    public virtual string PageSize
    {
      get => this.GetValueOrDefault("x-ms-max-item-count");
      set => this.SetProperty("x-ms-max-item-count", value);
    }

    public virtual string ConsistencyLevel
    {
      get => this.GetValueOrDefault("x-ms-consistency-level");
      set => this.SetProperty("x-ms-consistency-level", value);
    }

    public virtual string SDKSupportedCapabilities
    {
      get => this.GetValueOrDefault("x-ms-cosmos-sdk-supportedcapabilities");
      set => this.SetProperty("x-ms-cosmos-sdk-supportedcapabilities", value);
    }

    public virtual string ContentSerializationFormat
    {
      get => this.GetValueOrDefault("x-ms-documentdb-content-serialization-format");
      set => this.SetProperty("x-ms-documentdb-content-serialization-format", value);
    }

    public virtual string ReadFeedKeyType
    {
      get => this.GetValueOrDefault("x-ms-read-key-type");
      set => this.SetProperty("x-ms-read-key-type", value);
    }

    public virtual string StartEpk
    {
      get => this.GetValueOrDefault("x-ms-start-epk");
      set => this.SetProperty("x-ms-start-epk", value);
    }

    public virtual string EndEpk
    {
      get => this.GetValueOrDefault("x-ms-end-epk");
      set => this.SetProperty("x-ms-end-epk", value);
    }

    public abstract INameValueCollection INameValueCollection { get; }

    public virtual string this[string headerName]
    {
      get
      {
        string str;
        return !this.TryGetValue(headerName, out str) ? (string) null : str;
      }
      set => this.Set(headerName, value);
    }

    public abstract IEnumerator<string> GetEnumerator();

    public abstract void Add(string headerName, string value);

    public abstract void Set(string headerName, string value);

    public abstract string Get(string headerName);

    public abstract bool TryGetValue(string headerName, out string value);

    public abstract void Remove(string headerName);

    public abstract string[] AllKeys();

    public abstract int Count();

    public abstract string[] GetValues(string key);

    protected void SetProperty(string headerName, string value)
    {
      if (value == null)
        this.Remove(headerName);
      else
        this.Set(headerName, value);
    }

    public virtual string GetValueOrDefault(string headerName)
    {
      string str;
      return this.TryGetValue(headerName, out str) ? str : (string) null;
    }

    public virtual void Add(string headerName, IEnumerable<string> values)
    {
      if (headerName == null)
        throw new ArgumentNullException(nameof (headerName));
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      this.Add(headerName, string.Join(",", values));
    }

    public virtual T GetHeaderValue<T>(string key)
    {
      string s = this[key];
      if (string.IsNullOrEmpty(s))
        return default (T);
      return typeof (T) == typeof (double) ? (T) (ValueType) double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture) : (T) s;
    }

    public virtual void Add(INameValueCollection collection)
    {
      foreach (string key in collection.Keys())
        this.Set(key, collection[key]);
    }
  }
}
