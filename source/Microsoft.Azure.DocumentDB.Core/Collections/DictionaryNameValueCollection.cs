// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.DictionaryNameValueCollection
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Collections
{
  internal sealed class DictionaryNameValueCollection : INameValueCollection, IEnumerable
  {
    private static StringComparer defaultStringComparer = StringComparer.OrdinalIgnoreCase;
    private readonly Dictionary<string, DictionaryNameValueCollection.CompositeValue> dictionary;
    private DictionaryNameValueCollection.CompositeValue nullValue;
    private NameValueCollection nvc;

    public DictionaryNameValueCollection() => this.dictionary = new Dictionary<string, DictionaryNameValueCollection.CompositeValue>((IEqualityComparer<string>) DictionaryNameValueCollection.defaultStringComparer);

    public DictionaryNameValueCollection(StringComparer comparer) => this.dictionary = new Dictionary<string, DictionaryNameValueCollection.CompositeValue>((IEqualityComparer<string>) comparer);

    public DictionaryNameValueCollection(int capacity)
      : this(capacity, DictionaryNameValueCollection.defaultStringComparer)
    {
    }

    private DictionaryNameValueCollection(int capacity, StringComparer comparer) => this.dictionary = new Dictionary<string, DictionaryNameValueCollection.CompositeValue>(capacity, comparer == null ? (IEqualityComparer<string>) DictionaryNameValueCollection.defaultStringComparer : (IEqualityComparer<string>) comparer);

    public DictionaryNameValueCollection(INameValueCollection c)
      : this(c.Count())
    {
      if (c == null)
        throw new ArgumentNullException(nameof (c));
      this.Add(c);
    }

    public DictionaryNameValueCollection(NameValueCollection c)
      : this(c.Count)
    {
      if (c == null)
        throw new ArgumentNullException(nameof (c));
      foreach (string str1 in (NameObjectCollectionBase) c)
      {
        foreach (string str2 in c.GetValues(str1))
          this.Add(str1, str2);
      }
    }

    public void Add(string key, string value)
    {
      if (key == null)
      {
        if (this.nullValue == null)
          this.nullValue = new DictionaryNameValueCollection.CompositeValue(value);
        else
          this.nullValue.Add(value);
      }
      else
      {
        DictionaryNameValueCollection.CompositeValue compositeValue;
        this.dictionary.TryGetValue(key, out compositeValue);
        if (compositeValue != null)
          compositeValue.Add(value);
        else
          this.dictionary.Add(key, new DictionaryNameValueCollection.CompositeValue(value));
      }
    }

    public void Add(INameValueCollection c)
    {
      if (c == null)
        throw new ArgumentNullException(nameof (c));
      if (c is DictionaryNameValueCollection nameValueCollection)
      {
        foreach (string key in nameValueCollection.dictionary.Keys)
        {
          if (!this.dictionary.ContainsKey(key))
            this.dictionary[key] = new DictionaryNameValueCollection.CompositeValue();
          this.dictionary[key].Add(nameValueCollection.dictionary[key]);
        }
        if (nameValueCollection.nullValue == null)
          return;
        if (this.nullValue == null)
          this.nullValue = new DictionaryNameValueCollection.CompositeValue();
        this.nullValue.Add(nameValueCollection.nullValue);
      }
      else
      {
        foreach (string key in (IEnumerable) c)
        {
          foreach (string str in c.GetValues(key))
            this.Add(key, str);
        }
      }
    }

    public void Set(string key, string value)
    {
      if (key == null)
      {
        if (this.nullValue == null)
          this.nullValue = new DictionaryNameValueCollection.CompositeValue(value);
        else
          this.nullValue.Reset(value);
      }
      else
      {
        DictionaryNameValueCollection.CompositeValue compositeValue;
        this.dictionary.TryGetValue(key, out compositeValue);
        if (compositeValue != null)
          compositeValue.Reset(value);
        else
          this.dictionary.Add(key, new DictionaryNameValueCollection.CompositeValue(value));
      }
    }

    public string Get(string key)
    {
      DictionaryNameValueCollection.CompositeValue compositeValue = (DictionaryNameValueCollection.CompositeValue) null;
      if (key == null)
        compositeValue = this.nullValue;
      else
        this.dictionary.TryGetValue(key, out compositeValue);
      return compositeValue?.Value;
    }

    public string[] GetValues(string key)
    {
      DictionaryNameValueCollection.CompositeValue compositeValue = (DictionaryNameValueCollection.CompositeValue) null;
      if (key == null)
        compositeValue = this.nullValue;
      else
        this.dictionary.TryGetValue(key, out compositeValue);
      return compositeValue?.Values;
    }

    public void Remove(string key)
    {
      if (key == null)
        this.nullValue = (DictionaryNameValueCollection.CompositeValue) null;
      else
        this.dictionary.Remove(key);
    }

    public void Clear()
    {
      this.nullValue = (DictionaryNameValueCollection.CompositeValue) null;
      this.dictionary.Clear();
    }

    public IEnumerable<string> Keys
    {
      get
      {
        foreach (string key in this.dictionary.Keys)
          yield return key;
        if (this.nullValue != null)
          yield return (string) null;
      }
    }

    public int Count() => this.dictionary.Count + (this.nullValue != null ? 1 : 0);

    public string this[string key]
    {
      get => this.Get(key);
      set => this.Set(key, value);
    }

    public IEnumerator GetEnumerator() => (IEnumerator) this.Keys.GetEnumerator();

    public INameValueCollection Clone() => (INameValueCollection) new DictionaryNameValueCollection((INameValueCollection) this);

    public string[] AllKeys()
    {
      string[] strArray1 = new string[this.Count()];
      int num1 = 0;
      foreach (string key in this.dictionary.Keys)
        strArray1[num1++] = key;
      if (this.nullValue != null)
      {
        string[] strArray2 = strArray1;
        int index = num1;
        int num2 = index + 1;
        strArray2[index] = (string) null;
      }
      return strArray1;
    }

    IEnumerable<string> INameValueCollection.Keys() => this.Keys;

    public NameValueCollection ToNameValueCollection()
    {
      if (this.nvc == null)
      {
        lock (this)
        {
          if (this.nvc == null)
          {
            this.nvc = new NameValueCollection(this.dictionary.Count, (IEqualityComparer) this.dictionary.Comparer);
            foreach (string str1 in this)
            {
              if (this.GetValues(str1) == null)
              {
                this.nvc.Add(str1, (string) null);
              }
              else
              {
                foreach (string str2 in this.GetValues(str1))
                  this.nvc.Add(str1, str2);
              }
            }
          }
        }
      }
      return this.nvc;
    }

    private class CompositeValue
    {
      private List<string> values;

      internal CompositeValue() => this.values = new List<string>();

      private static string Convert(List<string> values) => string.Join(",", (IEnumerable<string>) values);

      public CompositeValue(string value)
        : this()
      {
        this.Add(value);
      }

      public void Add(string value)
      {
        if (value == null)
          return;
        this.values.Add(value);
      }

      public void Reset(string value)
      {
        this.values.Clear();
        this.Add(value);
      }

      public string[] Values => this.values.Count <= 0 ? (string[]) null : this.values.ToArray();

      public string Value => this.values.Count <= 0 ? (string) null : DictionaryNameValueCollection.CompositeValue.Convert(this.values);

      public void Add(DictionaryNameValueCollection.CompositeValue cv) => this.values.AddRange((IEnumerable<string>) cv.values);
    }
  }
}
