// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.TomlTableArray
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections;
using System.Collections.Generic;


#nullable enable
namespace Tomlyn.Model
{
  public sealed class TomlTableArray : 
    TomlObject,
    IList<TomlTable>,
    ICollection<TomlTable>,
    IEnumerable<TomlTable>,
    IEnumerable
  {
    private readonly List<TomlTable> _items;

    public TomlTableArray()
      : base(ObjectKind.TableArray)
    {
      this._items = new List<TomlTable>();
    }

    internal TomlTableArray(int capacity)
      : base(ObjectKind.TableArray)
    {
      this._items = new List<TomlTable>(capacity);
    }

    public List<TomlTable>.Enumerator GetEnumerator() => this._items.GetEnumerator();

    IEnumerator<TomlTable> IEnumerable<TomlTable>.GetEnumerator() => (IEnumerator<TomlTable>) this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void Add(TomlTable item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._items.Add(item);
    }

    public void Clear() => this._items.Clear();

    public bool Contains(TomlTable item) => item != null ? this._items.Contains(item) : throw new ArgumentNullException(nameof (item));

    public void CopyTo(TomlTable[] array, int arrayIndex) => this._items.CopyTo(array, arrayIndex);

    public bool Remove(TomlTable item) => item != null ? this._items.Remove(item) : throw new ArgumentNullException(nameof (item));

    public int Count => this._items.Count;

    public bool IsReadOnly => false;

    public int IndexOf(TomlTable item) => item != null ? this._items.IndexOf(item) : throw new ArgumentNullException(nameof (item));

    public void Insert(int index, TomlTable item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._items.Insert(index, item);
    }

    public void RemoveAt(int index) => this._items.RemoveAt(index);

    public TomlTable this[int index]
    {
      get => this._items[index];
      set
      {
        List<TomlTable> items = this._items;
        int index1 = index;
        items[index1] = value ?? throw new ArgumentNullException(nameof (value));
      }
    }
  }
}
