// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Helpers.GenericCollectionToNonGenericAdapter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Reflection;

namespace YamlDotNet.Helpers
{
  internal sealed class GenericCollectionToNonGenericAdapter : IList, ICollection, IEnumerable
  {
    private readonly object genericCollection;
    private readonly MethodInfo addMethod;
    private readonly MethodInfo indexerSetter;
    private readonly MethodInfo countGetter;

    public GenericCollectionToNonGenericAdapter(
      object genericCollection,
      Type genericCollectionType,
      Type genericListType)
    {
      this.genericCollection = genericCollection;
      this.addMethod = genericCollectionType.GetPublicInstanceMethod("Add");
      this.countGetter = genericCollectionType.GetPublicProperty(nameof (Count)).GetGetMethod();
      if ((object) genericListType == null)
        return;
      this.indexerSetter = genericListType.GetPublicProperty(nameof (Item)).GetSetMethod();
    }

    public int Add(object value)
    {
      int num = (int) this.countGetter.Invoke(this.genericCollection, (object[]) null);
      this.addMethod.Invoke(this.genericCollection, new object[1]
      {
        value
      });
      return num;
    }

    public void Clear() => throw new NotSupportedException();

    public bool Contains(object value) => throw new NotSupportedException();

    public int IndexOf(object value) => throw new NotSupportedException();

    public void Insert(int index, object value) => throw new NotSupportedException();

    public bool IsFixedSize => throw new NotSupportedException();

    public bool IsReadOnly => throw new NotSupportedException();

    public void Remove(object value) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();

    public object this[int index]
    {
      get => throw new NotSupportedException();
      set => this.indexerSetter.Invoke(this.genericCollection, new object[2]
      {
        (object) index,
        value
      });
    }

    public void CopyTo(Array array, int index) => throw new NotSupportedException();

    public int Count => throw new NotSupportedException();

    public bool IsSynchronized => throw new NotSupportedException();

    public object SyncRoot => throw new NotSupportedException();

    public IEnumerator GetEnumerator() => ((IEnumerable) this.genericCollection).GetEnumerator();
  }
}
