// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.NameValueResponseHeaders
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class NameValueResponseHeaders : CosmosMessageHeadersInternal
  {
    public override INameValueCollection INameValueCollection { get; }

    public NameValueResponseHeaders(INameValueCollection nameValueCollection) => this.INameValueCollection = nameValueCollection ?? throw new ArgumentNullException(nameof (nameValueCollection));

    public override void Add(string headerName, string value) => this.INameValueCollection.Add(headerName, value);

    public override void Add(string headerName, IEnumerable<string> values) => this.INameValueCollection.Add(headerName, values);

    public override void Set(string headerName, string value) => this.INameValueCollection.Set(headerName, value);

    public override string Get(string headerName) => this.INameValueCollection.Get(headerName);

    public override bool TryGetValue(string headerName, out string value)
    {
      value = this.INameValueCollection.Get(headerName);
      return value != null;
    }

    public override void Remove(string headerName) => this.INameValueCollection.Remove(headerName);

    public override string[] AllKeys() => this.INameValueCollection.AllKeys();

    public override IEnumerator<string> GetEnumerator() => this.INameValueCollection.Keys().GetEnumerator();

    public override int Count() => this.INameValueCollection.Count();

    public override string[] GetValues(string key) => this.INameValueCollection.GetValues(key);
  }
}
