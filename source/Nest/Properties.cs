// Decompiled with JetBrains decompiler
// Type: Nest.Properties
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class Properties : 
    IsADictionaryBase<PropertyName, IProperty>,
    IProperties,
    IIsADictionary<PropertyName, IProperty>,
    IDictionary<PropertyName, IProperty>,
    ICollection<KeyValuePair<PropertyName, IProperty>>,
    IEnumerable<KeyValuePair<PropertyName, IProperty>>,
    IEnumerable,
    IIsADictionary
  {
    private readonly IConnectionSettingsValues _settings;

    public Properties()
    {
    }

    public Properties(IDictionary<PropertyName, IProperty> container)
      : base(container)
    {
    }

    public Properties(Dictionary<PropertyName, IProperty> container)
      : base((IDictionary<PropertyName, IProperty>) container)
    {
    }

    internal Properties(IConnectionSettingsValues values) => this._settings = values;

    public void Add(PropertyName name, IProperty property) => this.BackingDictionary.Add(this.Sanitize(name), property);

    protected override PropertyName Sanitize(PropertyName key)
    {
      string str = this._settings?.Inferrer.PropertyName(key);
      return str == null ? key : (PropertyName) str;
    }
  }
}
