// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.PropertyBag
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class PropertyBag
  {
    public PropertyBag() => this.Bag = (IDictionary<string, string>) new Dictionary<string, string>();

    public void AddOrUpdateProperties(IDictionary<string, string> properties)
    {
      if (properties == null)
        return;
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) properties)
      {
        string key = property.Key.Trim();
        if (!string.IsNullOrEmpty(key) && property.Value != null)
          this.Bag[key] = property.Value.Trim();
      }
    }

    public void AddOrUpdateProperties(string key, string value)
    {
      if (string.IsNullOrEmpty(key) || value == null)
        return;
      this.Bag[key] = value.Trim();
    }

    public string GetProperty(string key)
    {
      string property = (string) null;
      if (!string.IsNullOrEmpty(key))
        this.Bag.TryGetValue(key.Trim(), out property);
      return property;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "bag")]
    private IDictionary<string, string> Bag { get; set; }
  }
}
