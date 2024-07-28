// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.TomlArray
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Collections;
using System.Collections.Generic;


#nullable enable
namespace Tomlyn.Model
{
  public sealed class TomlArray : 
    TomlObject,
    IList<object?>,
    ICollection<object?>,
    IEnumerable<object?>,
    IEnumerable
  {
    private readonly List<object?> _list;

    public TomlArray()
      : base(ObjectKind.Array)
    {
      this._list = new List<object>();
    }

    public TomlArray(int capacity)
      : base(ObjectKind.Array)
    {
      this._list = new List<object>(capacity);
    }

    public List<object?>.Enumerator GetEnumerator() => this._list.GetEnumerator();

    IEnumerator<object?> IEnumerable<object?>.GetEnumerator() => (IEnumerator<object>) this._list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void Add(object? item) => this._list.Add(item);

    public void Clear() => this._list.Clear();

    public bool Contains(object? item) => item != null && this._list.Contains(item);

    public void CopyTo(object?[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

    public bool Remove(object? item) => item != null && this._list.Remove(item);

    public int Count => this._list.Count;

    public bool IsReadOnly => false;

    public int IndexOf(object? item) => this._list.IndexOf(item);

    public void Insert(int index, object? item) => this._list.Insert(index, item);

    public void RemoveAt(int index) => this._list.RemoveAt(index);

    public object? this[int index]
    {
      get => this._list[index];
      set => this._list[index] = value;
    }
  }
}
