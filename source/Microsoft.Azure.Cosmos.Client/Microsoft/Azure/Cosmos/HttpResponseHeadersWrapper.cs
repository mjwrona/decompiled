// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.HttpResponseHeadersWrapper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class HttpResponseHeadersWrapper : 
    CosmosMessageHeadersInternal,
    INameValueCollection,
    IEnumerable
  {
    private readonly HttpResponseHeaders httpResponseHeaders;
    private readonly HttpContentHeaders httpContentHeaders;
    private readonly Lazy<DictionaryNameValueCollection> dictionaryNameValueCollection;

    public override INameValueCollection INameValueCollection => (INameValueCollection) this;

    public HttpResponseHeadersWrapper(
      HttpResponseHeaders responseHeaders,
      HttpContentHeaders httpContentHeaders)
    {
      IEnumerable<string> values;
      if (responseHeaders.TryGetValues("x-ms-alt-content-path", out values))
      {
        responseHeaders.Remove("x-ms-alt-content-path");
        foreach (string stringToUnescape in values)
          responseHeaders.Add("x-ms-alt-content-path", Uri.UnescapeDataString(stringToUnescape));
      }
      this.httpResponseHeaders = responseHeaders;
      this.httpContentHeaders = httpContentHeaders;
      this.dictionaryNameValueCollection = new Lazy<DictionaryNameValueCollection>((Func<DictionaryNameValueCollection>) (() => new DictionaryNameValueCollection()));
    }

    public override void Add(string key, string value)
    {
      if (this.httpResponseHeaders.TryAddWithoutValidation(key, value) || this.httpContentHeaders != null && this.httpContentHeaders.TryAddWithoutValidation(key, value))
        return;
      this.dictionaryNameValueCollection.Value.Add(key, value);
    }

    public override void Add(INameValueCollection collection) => this.dictionaryNameValueCollection.Value.Add(collection);

    public void Clear()
    {
      this.httpResponseHeaders.Clear();
      if (this.httpContentHeaders != null)
        this.httpContentHeaders.Clear();
      if (!this.dictionaryNameValueCollection.IsValueCreated)
        return;
      this.dictionaryNameValueCollection.Value.Clear();
    }

    public override bool TryGetValue(string headerName, out string value)
    {
      value = this.Get(headerName);
      return value != null;
    }

    public override string[] AllKeys() => this.Keys().ToArray<string>();

    public INameValueCollection Clone()
    {
      INameValueCollection nameValueCollection = (INameValueCollection) new DictionaryNameValueCollection();
      foreach (KeyValuePair<string, IEnumerable<string>> allItem in this.AllItems())
      {
        foreach (string str in allItem.Value)
          nameValueCollection.Add(allItem.Key, str);
      }
      return nameValueCollection;
    }

    public override int Count()
    {
      int num = 0;
      if (this.dictionaryNameValueCollection.IsValueCreated)
        num = this.dictionaryNameValueCollection.Value.Count();
      if (this.httpContentHeaders != null)
        num += this.httpContentHeaders.Count<KeyValuePair<string, IEnumerable<string>>>();
      return this.httpResponseHeaders.Count<KeyValuePair<string, IEnumerable<string>>>() + num;
    }

    public override string Get(string key)
    {
      IEnumerable<string> values;
      if (this.httpResponseHeaders.TryGetValues(key, out values) || this.httpContentHeaders != null && this.httpContentHeaders.TryGetValues(key, out values))
        return this.JoinHeaders(values);
      return this.dictionaryNameValueCollection.IsValueCreated ? this.dictionaryNameValueCollection.Value.Get(key) : (string) null;
    }

    public override IEnumerator<string> GetEnumerator() => this.Keys().GetEnumerator();

    private IEnumerable<KeyValuePair<string, IEnumerable<string>>> AllItems()
    {
      foreach (KeyValuePair<string, IEnumerable<string>> httpResponseHeader in (HttpHeaders) this.httpResponseHeaders)
        yield return httpResponseHeader;
      if (this.httpContentHeaders != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> httpContentHeader in (HttpHeaders) this.httpContentHeaders)
          yield return httpContentHeader;
      }
      if (this.dictionaryNameValueCollection.IsValueCreated)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in this.dictionaryNameValueCollection.Value)
          yield return keyValuePair;
      }
    }

    public override string[] GetValues(string key) => this.httpResponseHeaders.GetValues(key).ToArray<string>();

    public IEnumerable<string> Keys()
    {
      foreach (KeyValuePair<string, IEnumerable<string>> allItem in this.AllItems())
        yield return allItem.Key;
    }

    public override void Remove(string key)
    {
      if (this.httpResponseHeaders.TryGetValues(key, out IEnumerable<string> _))
      {
        this.httpResponseHeaders.Remove(key);
      }
      else
      {
        if (this.httpContentHeaders != null && this.httpContentHeaders.Remove(key) || !this.dictionaryNameValueCollection.IsValueCreated)
          return;
        this.dictionaryNameValueCollection.Value.Remove(key);
      }
    }

    public override void Set(string key, string value)
    {
      this.Remove(key);
      this.Add(key, value);
    }

    public NameValueCollection ToNameValueCollection()
    {
      NameValueCollection nameValueCollection = new NameValueCollection();
      foreach (KeyValuePair<string, IEnumerable<string>> allItem in this.AllItems())
        nameValueCollection.Add(allItem.Key, this.JoinHeaders(allItem.Value));
      return nameValueCollection;
    }

    private string JoinHeaders(IEnumerable<string> headerValues) => headerValues != null ? string.Join(",", headerValues) : (string) null;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
