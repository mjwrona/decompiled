// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.StoreRequestHeaders
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class StoreRequestHeaders : CosmosMessageHeadersInternal
  {
    private readonly RequestNameValueCollection requestNameValueCollection;

    public override string Continuation
    {
      get => this.requestNameValueCollection.Continuation;
      set => this.requestNameValueCollection.Continuation = value;
    }

    public override string SessionToken
    {
      get => this.requestNameValueCollection.SessionToken;
      set => this.requestNameValueCollection.SessionToken = value;
    }

    public override string PartitionKeyRangeId
    {
      get => this.requestNameValueCollection.PartitionKeyRangeId;
      set => this.requestNameValueCollection.PartitionKeyRangeId = value;
    }

    public override string PartitionKey
    {
      get => this.requestNameValueCollection.PartitionKey;
      set => this.requestNameValueCollection.PartitionKey = value;
    }

    public override string XDate
    {
      get => this.requestNameValueCollection.XDate;
      set => this.requestNameValueCollection.XDate = value;
    }

    public override string ConsistencyLevel
    {
      get => this.requestNameValueCollection.ConsistencyLevel;
      set => this.requestNameValueCollection.ConsistencyLevel = value;
    }

    public override string IfNoneMatch
    {
      get => this.requestNameValueCollection.IfNoneMatch;
      set => this.requestNameValueCollection.IfNoneMatch = value;
    }

    public override string SDKSupportedCapabilities
    {
      get => this.requestNameValueCollection.SDKSupportedCapabilities;
      set => this.requestNameValueCollection.SDKSupportedCapabilities = value;
    }

    public override string ContentSerializationFormat
    {
      get => this.requestNameValueCollection.ContentSerializationFormat;
      set => this.requestNameValueCollection.ContentSerializationFormat = value;
    }

    public override string ReadFeedKeyType
    {
      get => this.requestNameValueCollection.ReadFeedKeyType;
      set => this.requestNameValueCollection.ReadFeedKeyType = value;
    }

    public override string StartEpk
    {
      get => this.requestNameValueCollection.StartEpk;
      set => this.requestNameValueCollection.StartEpk = value;
    }

    public override string EndEpk
    {
      get => this.requestNameValueCollection.EndEpk;
      set => this.requestNameValueCollection.EndEpk = value;
    }

    public override string PageSize
    {
      get => this.requestNameValueCollection.PageSize;
      set => this.requestNameValueCollection.PageSize = value;
    }

    public override INameValueCollection INameValueCollection => (INameValueCollection) this.requestNameValueCollection;

    public StoreRequestHeaders() => this.requestNameValueCollection = new RequestNameValueCollection();

    public override void Add(string headerName, string value) => this.requestNameValueCollection.Add(headerName, value);

    public override void Add(string headerName, IEnumerable<string> values) => this.requestNameValueCollection.Add(headerName, values);

    public override void Set(string headerName, string value) => this.requestNameValueCollection.Set(headerName, value);

    public override string Get(string headerName) => this.requestNameValueCollection.Get(headerName);

    public override bool TryGetValue(string headerName, out string value)
    {
      value = this.requestNameValueCollection.Get(headerName);
      return value != null;
    }

    public override void Remove(string headerName) => this.requestNameValueCollection.Remove(headerName);

    public override string[] AllKeys() => this.requestNameValueCollection.AllKeys();

    public override IEnumerator<string> GetEnumerator() => this.requestNameValueCollection.Keys().GetEnumerator();

    public override int Count() => this.requestNameValueCollection.Count();

    public override string[] GetValues(string key) => this.requestNameValueCollection.GetValues(key);
  }
}
