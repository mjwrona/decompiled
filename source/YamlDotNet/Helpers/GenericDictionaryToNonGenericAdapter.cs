// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Helpers.GenericDictionaryToNonGenericAdapter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace YamlDotNet.Helpers
{
  internal sealed class GenericDictionaryToNonGenericAdapter : IDictionary, ICollection, IEnumerable
  {
    private readonly object genericDictionary;
    private readonly Type genericDictionaryType;
    private readonly MethodInfo indexerSetter;

    public GenericDictionaryToNonGenericAdapter(
      object genericDictionary,
      Type genericDictionaryType)
    {
      this.genericDictionary = genericDictionary;
      this.genericDictionaryType = genericDictionaryType;
      this.indexerSetter = genericDictionaryType.GetPublicProperty(nameof (Item)).GetSetMethod();
    }

    public void Add(object key, object value) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(object key) => throw new NotSupportedException();

    public IDictionaryEnumerator GetEnumerator() => (IDictionaryEnumerator) new GenericDictionaryToNonGenericAdapter.DictionaryEnumerator(this.genericDictionary, this.genericDictionaryType);

    public bool IsFixedSize => throw new NotSupportedException();

    public bool IsReadOnly => throw new NotSupportedException();

    public ICollection Keys => throw new NotSupportedException();

    public void Remove(object key) => throw new NotSupportedException();

    public ICollection Values => throw new NotSupportedException();

    public object this[object key]
    {
      get => throw new NotSupportedException();
      set => this.indexerSetter.Invoke(this.genericDictionary, new object[2]
      {
        key,
        value
      });
    }

    public void CopyTo(Array array, int index) => throw new NotSupportedException();

    public int Count => throw new NotSupportedException();

    public bool IsSynchronized => throw new NotSupportedException();

    public object SyncRoot => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.genericDictionary).GetEnumerator();

    private class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
    {
      private readonly IEnumerator enumerator;
      private readonly MethodInfo getKeyMethod;
      private readonly MethodInfo getValueMethod;

      public DictionaryEnumerator(object genericDictionary, Type genericDictionaryType)
      {
        Type type = typeof (KeyValuePair<,>).MakeGenericType(genericDictionaryType.GetGenericArguments());
        this.getKeyMethod = type.GetPublicProperty(nameof (Key)).GetGetMethod();
        this.getValueMethod = type.GetPublicProperty(nameof (Value)).GetGetMethod();
        this.enumerator = ((IEnumerable) genericDictionary).GetEnumerator();
      }

      public DictionaryEntry Entry => new DictionaryEntry(this.Key, this.Value);

      public object Key => this.getKeyMethod.Invoke(this.enumerator.Current, (object[]) null);

      public object Value => this.getValueMethod.Invoke(this.enumerator.Current, (object[]) null);

      public object Current => (object) this.Entry;

      public bool MoveNext() => this.enumerator.MoveNext();

      public void Reset() => this.enumerator.Reset();
    }
  }
}
