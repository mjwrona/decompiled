// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.PropertyCollection
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class PropertyCollection : List<PropertyPair>
  {
    public string this[string name]
    {
      get => this.FindProperty(name)?.Value;
      set
      {
        PropertyPair property;
        if (!this.TryGetProperty(name, out property))
          this.Add(new PropertyPair(name, value));
        else
          property.Value = value;
      }
    }

    public PropertyPair FindProperty(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.Find((Predicate<PropertyPair>) (p => string.Equals(name, p.Name, StringComparison.OrdinalIgnoreCase)));
    }

    public bool TryGetProperty(string name, out PropertyPair property)
    {
      property = this.FindProperty(name);
      return property != null;
    }

    public bool RemoveProperty(string name)
    {
      PropertyPair property;
      return this.TryGetProperty(name, out property) && this.Remove(property);
    }

    public T ParseProperty<T>(string name, T defaultValue)
    {
      PropertyPair property;
      return !this.TryGetProperty(name, out property) ? defaultValue : PropertyCollection.FromString<T>(property.Value, defaultValue);
    }

    public static T FromString<T>(string value, T defaultValue) => value == null ? defaultValue : PropertyCollection.FromString<T>(value);

    public static T FromString<T>(string value) => typeof (T).IsAssignableFrom(typeof (string)) ? (T) value : (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFromInvariantString(value);

    public override string ToString() => string.Join<PropertyPair>(",", (IEnumerable<PropertyPair>) this);

    public PropertyCollection Clone()
    {
      PropertyCollection clone = new PropertyCollection();
      this.ForEach((Action<PropertyPair>) (p => clone.Add(new PropertyPair(p.Name, p.Value))));
      return clone;
    }
  }
}
