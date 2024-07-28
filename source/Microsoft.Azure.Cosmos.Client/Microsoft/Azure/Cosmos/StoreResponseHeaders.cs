// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.StoreResponseHeaders
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class StoreResponseHeaders : CosmosMessageHeadersInternal
  {
    private readonly StoreResponseNameValueCollection storeResponseNameValueCollection;

    public override string RequestCharge
    {
      get => this.storeResponseNameValueCollection.RequestCharge;
      set => this.storeResponseNameValueCollection.RequestCharge = value;
    }

    public override string ActivityId
    {
      get => this.storeResponseNameValueCollection.ActivityId;
      set => this.storeResponseNameValueCollection.ActivityId = value;
    }

    public override string ETag
    {
      get => this.storeResponseNameValueCollection.ETag;
      set => this.storeResponseNameValueCollection.ETag = value;
    }

    public override string SubStatus
    {
      get => this.storeResponseNameValueCollection.SubStatus;
      set => this.storeResponseNameValueCollection.SubStatus = value;
    }

    public override string QueryMetrics
    {
      get => this.storeResponseNameValueCollection.QueryMetrics;
      set => this.storeResponseNameValueCollection.QueryMetrics = value;
    }

    public override string BackendRequestDurationMilliseconds
    {
      get => this.storeResponseNameValueCollection.BackendRequestDurationMilliseconds;
      set => this.storeResponseNameValueCollection.BackendRequestDurationMilliseconds = value;
    }

    public override string Continuation
    {
      get => this.storeResponseNameValueCollection.Continuation;
      set => this.storeResponseNameValueCollection.Continuation = value;
    }

    public override string SessionToken
    {
      get => this.storeResponseNameValueCollection.SessionToken;
      set => this.storeResponseNameValueCollection.SessionToken = value;
    }

    public override string PartitionKeyRangeId
    {
      get => this.storeResponseNameValueCollection.PartitionKeyRangeId;
      set => this.storeResponseNameValueCollection.PartitionKeyRangeId = value;
    }

    public override INameValueCollection INameValueCollection => (INameValueCollection) this.storeResponseNameValueCollection;

    public StoreResponseHeaders(
      StoreResponseNameValueCollection storeResponseNameValueCollection)
    {
      this.storeResponseNameValueCollection = storeResponseNameValueCollection ?? throw new ArgumentNullException(nameof (storeResponseNameValueCollection));
    }

    public override void Add(string headerName, string value) => this.storeResponseNameValueCollection.Add(headerName, value);

    public override void Add(string headerName, IEnumerable<string> values) => this.storeResponseNameValueCollection.Add(headerName, values);

    public override void Set(string headerName, string value) => this.storeResponseNameValueCollection.Set(headerName, value);

    public override string Get(string headerName) => this.storeResponseNameValueCollection.Get(headerName);

    public override bool TryGetValue(string headerName, out string value)
    {
      value = this.storeResponseNameValueCollection.Get(headerName);
      return value != null;
    }

    public override void Remove(string headerName) => this.storeResponseNameValueCollection.Remove(headerName);

    public override string[] AllKeys() => this.storeResponseNameValueCollection.AllKeys();

    public override IEnumerator<string> GetEnumerator() => this.storeResponseNameValueCollection.Keys().GetEnumerator();

    public override int Count() => this.storeResponseNameValueCollection.Count();

    public override string[] GetValues(string key) => this.storeResponseNameValueCollection.GetValues(key);
  }
}
