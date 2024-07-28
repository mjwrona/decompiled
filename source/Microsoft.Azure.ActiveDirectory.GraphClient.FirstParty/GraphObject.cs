// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.GraphObject
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class GraphObject
  {
    internal GraphObject()
    {
      this.NonSerializedProperties = new Dictionary<string, object>();
      this.ChangedProperties = new HashSet<string>();
    }

    [JsonProperty("odata.type")]
    public string ODataTypeName { get; set; }

    public JToken TokenDictionary { get; set; }

    public IList<string> PropertiesMaterializedFromDeserialization { get; set; }

    public HashSet<string> ChangedProperties { get; set; }

    public Dictionary<string, object> NonSerializedProperties { get; set; }

    public object this[string propertyName]
    {
      get
      {
        if (this.NonSerializedProperties.ContainsKey(propertyName))
          return this.NonSerializedProperties[propertyName];
        if (Utils.GetGraphObjectPropertyName(propertyName) != null)
        {
          PropertyInfo property = this.GetType().GetProperty(Utils.GetGraphObjectPropertyName(propertyName));
          if (property != (PropertyInfo) null)
            return property.GetValue((object) this, (object[]) null);
        }
        return (object) null;
      }
      set
      {
        string objectPropertyName = Utils.GetGraphObjectPropertyName(propertyName);
        if (objectPropertyName != null)
        {
          PropertyInfo property = this.GetType().GetProperty(objectPropertyName);
          if (!(property != (PropertyInfo) null))
            throw new ArgumentException(string.Format("Given property {0} is not found on the entity {1}.", (object) propertyName, (object) this.GetType().Name), propertyName);
          property.SetValue((object) this, value);
        }
        else
          this.NonSerializedProperties[propertyName] = value;
      }
    }

    public virtual void ValidateProperties(bool isCreate)
    {
      foreach (string changedProperty in this.ChangedProperties)
      {
        if (Utils.GetLinkAttribute(this.GetType(), changedProperty) != null)
          throw new PropertyValidationException("Link cannot be specified during add / update.");
      }
    }

    public virtual string ToJson(bool updatedPropertiesOnly) => updatedPropertiesOnly ? JsonConvert.SerializeObject((object) Utils.GetSerializableGraphObject(this)) : JsonConvert.SerializeObject((object) this);
  }
}
