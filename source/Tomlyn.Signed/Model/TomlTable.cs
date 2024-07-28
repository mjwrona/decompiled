// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.TomlTable
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections;
using System.Collections.Generic;
using Tomlyn.Syntax;


#nullable enable
namespace Tomlyn.Model
{
  public sealed class TomlTable : 
    TomlObject,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable,
    ITomlMetadataProvider
  {
    private readonly List<KeyValuePair<string, TomlTable.ValueHolder>> _order;
    private readonly Dictionary<string, TomlTable.ValueHolder> _map;

    public TomlTable()
      : this(false)
    {
    }

    public TomlTable(bool inline)
      : base(inline ? ObjectKind.InlineTable : ObjectKind.Table)
    {
      this._order = new List<KeyValuePair<string, TomlTable.ValueHolder>>();
      this._map = new Dictionary<string, TomlTable.ValueHolder>();
    }

    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      foreach (KeyValuePair<string, TomlTable.ValueHolder> keyValuePair in this._order)
        yield return new KeyValuePair<string, object>(keyValuePair.Key, keyValuePair.Value.Target);
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) => this.Add(item.Key, item.Value);

    public void Clear()
    {
      this._map.Clear();
      this._order.Clear();
    }

    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
    {
      foreach (KeyValuePair<string, TomlTable.ValueHolder> keyValuePair in this._order)
      {
        if (keyValuePair.Key == item.Key)
          return keyValuePair.Value.Target == item.Value;
      }
      return false;
    }

    void ICollection<KeyValuePair<string, object>>.CopyTo(
      KeyValuePair<string, object>[] array,
      int arrayIndex)
    {
      if (arrayIndex + this._order.Count > array.Length)
        throw new ArgumentOutOfRangeException(nameof (arrayIndex));
      for (int index = 0; index < this._order.Count; ++index)
      {
        KeyValuePair<string, TomlTable.ValueHolder> keyValuePair = this._order[index];
        array[index + arrayIndex] = new KeyValuePair<string, object>(keyValuePair.Key, keyValuePair.Value.Target);
      }
    }

    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
    {
      foreach (KeyValuePair<string, TomlTable.ValueHolder> keyValuePair in this._order)
      {
        if (keyValuePair.Key == item.Key && keyValuePair.Value == item.Value)
        {
          this.Remove(item.Key);
          return true;
        }
      }
      return false;
    }

    public int Count => this._map.Count;

    public bool IsReadOnly => false;

    public void Add(string key, object value)
    {
      TomlTable.ValueHolder valueHolder = value != null ? new TomlTable.ValueHolder(value) : throw new ArgumentNullException(nameof (value));
      this._map.Add(key, valueHolder);
      this._order.Add(new KeyValuePair<string, TomlTable.ValueHolder>(key, valueHolder));
    }

    public bool ContainsKey(string key) => this._map.ContainsKey(key);

    public bool Remove(string key)
    {
      if (!this._map.Remove(key))
        return false;
      for (int index = this._order.Count - 1; index >= 0; --index)
      {
        if (this._order[index].Key == key)
        {
          this._order.RemoveAt(index);
          break;
        }
      }
      return true;
    }

    public bool TryGetValue(string key, out object value)
    {
      value = (object) null;
      TomlTable.ValueHolder valueHolder;
      if (!this._map.TryGetValue(key, out valueHolder))
        return false;
      value = valueHolder.Target;
      return true;
    }

    public object this[string key]
    {
      get => this._map[key].Target;
      set
      {
        TomlTable.ValueHolder valueHolder;
        if (this._map.TryGetValue(key, out valueHolder))
          valueHolder.Target = value;
        else
          this.Add(key, value);
      }
    }

    public ICollection<string> Keys
    {
      get
      {
        List<string> keys = new List<string>();
        foreach (KeyValuePair<string, TomlTable.ValueHolder> keyValuePair in this._order)
          keys.Add(keyValuePair.Key);
        return (ICollection<string>) keys;
      }
    }

    public ICollection<object> Values
    {
      get
      {
        List<object> values = new List<object>();
        foreach (KeyValuePair<string, TomlTable.ValueHolder> keyValuePair in this._order)
          values.Add(keyValuePair.Value.Target);
        return (ICollection<object>) values;
      }
    }

    public static TomlTable From(DocumentSyntax documentSyntax)
    {
      if (documentSyntax == null)
        throw new ArgumentNullException(nameof (documentSyntax));
      return !documentSyntax.HasErrors ? documentSyntax.ToModel<TomlTable>() : throw new InvalidOperationException(string.Format("The document has errors: {0}", (object) documentSyntax.Diagnostics));
    }

    private class ValueHolder
    {
      public ValueHolder(object target) => this.Target = target;

      public object Target { get; set; }
    }
  }
}
