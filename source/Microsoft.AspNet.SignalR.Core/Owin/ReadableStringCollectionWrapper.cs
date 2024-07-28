// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Owin.ReadableStringCollectionWrapper
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.Owin;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Owin
{
  internal class ReadableStringCollectionWrapper : 
    INameValueCollection,
    IEnumerable<KeyValuePair<string, string>>,
    IEnumerable
  {
    private readonly IReadableStringCollection _readableStringCollection;

    public ReadableStringCollectionWrapper(IReadableStringCollection readableStringCollection) => this._readableStringCollection = readableStringCollection;

    public string this[string key] => this._readableStringCollection[key];

    public IEnumerable<string> GetValues(string key) => (IEnumerable<string>) this._readableStringCollection.GetValues(key);

    public string Get(string key) => this._readableStringCollection.Get(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => this.GetEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    private IEnumerable<KeyValuePair<string, string>> GetEnumerable()
    {
      foreach (KeyValuePair<string, string[]> readableString in (IEnumerable<KeyValuePair<string, string[]>>) this._readableStringCollection)
        yield return new KeyValuePair<string, string>(readableString.Key, this._readableStringCollection.Get(readableString.Key));
    }
  }
}
