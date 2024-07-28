// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyBag
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public sealed class PropertyBag
  {
    private readonly PropertiesCollection m_originalProperties;
    private readonly PropertiesCollection m_updatedProperties;
    private int m_duplicateKeyCount;

    public PropertyBag()
    {
      this.m_originalProperties = new PropertiesCollection();
      this.m_updatedProperties = new PropertiesCollection();
    }

    public PropertyBag(IDictionary<string, object> properties)
    {
      PropertyValidation.ValidateDictionary(properties);
      this.m_originalProperties = new PropertiesCollection(properties);
      this.m_updatedProperties = new PropertiesCollection();
    }

    public int Count => this.m_originalProperties.Count + this.m_updatedProperties.Count - this.m_duplicateKeyCount;

    public IEnumerable<string> Keys => (IEnumerable<string>) this.m_originalProperties.Keys.Union<string>((IEnumerable<string>) this.m_updatedProperties.Keys).Distinct<string>().ToList<string>();

    public PropertiesCollection UpdatedProperties => this.m_updatedProperties;

    public object this[string key]
    {
      get => this.Get(key);
      set => this.Set(key, value);
    }

    public void Add(string key, object value)
    {
      if (this.m_originalProperties.ContainsKey(key))
        throw new ArgumentException("An element with the same key already exists.", nameof (key));
      this.m_updatedProperties.Add(key, value);
    }

    public void Set(string key, object value)
    {
      if (this.m_originalProperties.ContainsKey(key) && !this.m_updatedProperties.ContainsKey(key))
        ++this.m_duplicateKeyCount;
      this.m_updatedProperties[key] = value;
    }

    public bool Remove(string key)
    {
      bool flag = this.m_updatedProperties.Remove(key);
      int num = this.m_originalProperties.Remove(key) ? 1 : 0;
      if ((num & (flag ? 1 : 0)) != 0)
        --this.m_duplicateKeyCount;
      return (num | (flag ? 1 : 0)) != 0;
    }

    public object Get(string key)
    {
      object obj;
      if (!this.TryGetValue(key, out obj))
        throw new KeyNotFoundException("Key " + key + " not found.");
      return obj;
    }

    public void Clear()
    {
      this.m_originalProperties.Clear();
      this.m_updatedProperties.Clear();
      this.m_duplicateKeyCount = 0;
    }

    public void Reset(IDictionary<string, object> properties)
    {
      PropertyValidation.ValidateDictionary(properties);
      this.Clear();
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
        this.m_originalProperties[property.Key] = property.Value;
    }

    public bool ContainsKey(string key) => this.m_originalProperties.ContainsKey(key) || this.m_updatedProperties.ContainsKey(key);

    public object GetValue(string key, object defaultValue)
    {
      object obj;
      if (!this.TryGetValue(key, out obj))
        obj = defaultValue;
      return obj;
    }

    public T GetValue<T>(string key, T defaultValue)
    {
      T obj;
      if (!this.TryGetValue<T>(key, out obj))
        obj = defaultValue;
      return obj;
    }

    public bool TryGetValue(string key, out object value) => this.m_updatedProperties.ContainsKey(key) ? this.m_updatedProperties.TryGetValue(key, out value) : this.m_originalProperties.TryGetValue(key, out value);

    public bool TryGetValue<T>(string key, out T value) => this.m_updatedProperties.ContainsKey(key) ? this.m_updatedProperties.TryGetValidatedValue<T>(key, out value) : this.m_originalProperties.TryGetValidatedValue<T>(key, out value);

    public PropertyBag Clone()
    {
      PropertyBag propertyBag = new PropertyBag((IDictionary<string, object>) this.m_originalProperties);
      foreach (KeyValuePair<string, object> updatedProperty in (IEnumerable<KeyValuePair<string, object>>) this.m_updatedProperties)
        propertyBag[updatedProperty.Key] = updatedProperty.Value;
      return propertyBag;
    }

    private bool Equals(PropertyBag obj)
    {
      if (this == obj)
        return true;
      return object.Equals((object) this.m_duplicateKeyCount, (object) obj.m_duplicateKeyCount) && object.Equals((object) this.m_originalProperties, (object) obj.m_originalProperties) && object.Equals((object) this.m_updatedProperties, (object) obj.m_updatedProperties);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((PropertyBag) obj);
    }

    public override int GetHashCode()
    {
      int hashCode = 1231 * 3037 + this.m_duplicateKeyCount.GetHashCode();
      foreach (KeyValuePair<string, object> originalProperty in (IEnumerable<KeyValuePair<string, object>>) this.m_originalProperties)
      {
        int num1 = hashCode * 3037;
        string key = originalProperty.Key;
        int? nullable1;
        int? nullable2;
        if (key == null)
        {
          nullable1 = new int?();
          nullable2 = nullable1;
        }
        else
          nullable2 = new int?(key.GetHashCode());
        int? nullable3 = nullable2;
        int? nullable4;
        if (!nullable3.HasValue)
        {
          nullable1 = new int?();
          nullable4 = nullable1;
        }
        else
          nullable4 = new int?(num1 + nullable3.GetValueOrDefault());
        nullable1 = nullable4;
        hashCode = nullable1.GetValueOrDefault();
        int num2 = hashCode * 3037;
        object obj = originalProperty.Value;
        int? nullable5;
        if (obj == null)
        {
          nullable1 = new int?();
          nullable5 = nullable1;
        }
        else
          nullable5 = new int?(obj.GetHashCode());
        nullable3 = nullable5;
        int? nullable6;
        if (!nullable3.HasValue)
        {
          nullable1 = new int?();
          nullable6 = nullable1;
        }
        else
          nullable6 = new int?(num2 + nullable3.GetValueOrDefault());
        nullable1 = nullable6;
        hashCode = nullable1.GetValueOrDefault();
      }
      foreach (KeyValuePair<string, object> updatedProperty in (IEnumerable<KeyValuePair<string, object>>) this.m_updatedProperties)
      {
        int num3 = hashCode * 3037;
        string key = updatedProperty.Key;
        int? nullable7;
        int? nullable8;
        if (key == null)
        {
          nullable7 = new int?();
          nullable8 = nullable7;
        }
        else
          nullable8 = new int?(key.GetHashCode());
        int? nullable9 = nullable8;
        int? nullable10;
        if (!nullable9.HasValue)
        {
          nullable7 = new int?();
          nullable10 = nullable7;
        }
        else
          nullable10 = new int?(num3 + nullable9.GetValueOrDefault());
        nullable7 = nullable10;
        hashCode = nullable7.GetValueOrDefault();
        int num4 = hashCode * 3037;
        object obj = updatedProperty.Value;
        int? nullable11;
        if (obj == null)
        {
          nullable7 = new int?();
          nullable11 = nullable7;
        }
        else
          nullable11 = new int?(obj.GetHashCode());
        nullable9 = nullable11;
        int? nullable12;
        if (!nullable9.HasValue)
        {
          nullable7 = new int?();
          nullable12 = nullable7;
        }
        else
          nullable12 = new int?(num4 + nullable9.GetValueOrDefault());
        nullable7 = nullable12;
        hashCode = nullable7.GetValueOrDefault();
      }
      return hashCode;
    }
  }
}
