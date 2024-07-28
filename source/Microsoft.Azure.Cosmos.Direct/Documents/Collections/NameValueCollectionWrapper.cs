// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.NameValueCollectionWrapper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Collections
{
  internal class NameValueCollectionWrapper : INameValueCollection, IEnumerable
  {
    private NameValueCollection collection;

    public NameValueCollectionWrapper() => this.collection = new NameValueCollection();

    public NameValueCollectionWrapper(int capacity) => this.collection = new NameValueCollection(capacity);

    public NameValueCollectionWrapper(StringComparer comparer) => this.collection = new NameValueCollection((IEqualityComparer) comparer);

    public NameValueCollectionWrapper(NameValueCollectionWrapper values) => this.collection = new NameValueCollection(values.collection);

    public NameValueCollectionWrapper(NameValueCollection collection) => this.collection = new NameValueCollection(collection);

    public NameValueCollectionWrapper(INameValueCollection collection)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      this.collection = new NameValueCollection();
      foreach (string str in (IEnumerable) collection)
        this.collection.Add(str, collection[str]);
    }

    public static NameValueCollectionWrapper Create(NameValueCollection collection) => new NameValueCollectionWrapper()
    {
      collection = collection
    };

    public string this[string key]
    {
      get => this.collection[key];
      set => this.collection[key] = value;
    }

    public void Add(INameValueCollection c)
    {
      if (c == null)
        throw new ArgumentNullException(nameof (c));
      if (c is NameValueCollectionWrapper collectionWrapper)
      {
        this.collection.Add(collectionWrapper.collection);
      }
      else
      {
        foreach (string str1 in (IEnumerable) c)
        {
          foreach (string str2 in c.GetValues(str1))
            this.collection.Add(str1, str2);
        }
      }
    }

    public void Add(string key, string value) => this.collection.Add(key, value);

    public INameValueCollection Clone() => (INameValueCollection) new NameValueCollectionWrapper(this);

    public string Get(string key) => this.collection.Get(key);

    public IEnumerator GetEnumerator() => this.collection.GetEnumerator();

    public string[] GetValues(string key) => this.collection.GetValues(key);

    public void Remove(string key) => this.collection.Remove(key);

    public void Clear() => this.collection.Clear();

    public int Count() => this.collection.Count;

    public void Set(string key, string value) => this.collection.Set(key, value);

    public string[] AllKeys() => this.collection.AllKeys;

    public IEnumerable<string> Keys()
    {
      foreach (string key in this.collection.Keys)
        yield return key;
    }

    public NameValueCollection ToNameValueCollection() => this.collection;
  }
}
