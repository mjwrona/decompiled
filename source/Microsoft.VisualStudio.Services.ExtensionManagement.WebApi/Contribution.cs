// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.Contribution
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class Contribution : ContributionBase
  {
    private bool m_hashCalculated;
    private int m_hashCode;
    private Dictionary<string, object> m_associatedObjects;
    private object m_associatedObjectsWriteLock = new object();

    [DataMember(Order = 100, EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(Order = 110, EmitDefaultValue = false)]
    public List<string> Targets { get; set; }

    [DataMember(Order = 120, EmitDefaultValue = false)]
    public JObject Properties { get; set; }

    [DataMember(Order = 130, EmitDefaultValue = false)]
    public List<ContributionConstraint> Constraints { get; set; }

    [DataMember(Order = 140, EmitDefaultValue = false)]
    public List<string> Includes { get; set; }

    [DataMember(Order = 150, EmitDefaultValue = false)]
    public IEnumerable<string> RestrictedTo { get; set; }

    public override int GetHashCode()
    {
      if (!this.m_hashCalculated)
      {
        int hashCode = base.GetHashCode();
        if (this.Type != null)
          hashCode ^= this.Type.GetHashCode();
        if (this.Properties != null)
          hashCode ^= this.Properties.ToString().GetHashCode();
        if (this.Targets != null)
        {
          foreach (string target in this.Targets)
            hashCode ^= target.GetHashCode();
        }
        if (this.Includes != null)
        {
          foreach (string include in this.Includes)
            hashCode ^= include.GetHashCode();
        }
        if (this.Constraints != null)
        {
          foreach (ContributionConstraint constraint in this.Constraints)
            hashCode ^= constraint.GetHashCode();
        }
        this.m_hashCode = hashCode;
        this.m_hashCalculated = true;
      }
      return this.m_hashCode;
    }

    public override string ToString() => this.Id;

    public T GetAssociatedObject<T>(string key)
    {
      object obj1;
      return this.m_associatedObjects != null && this.m_associatedObjects.TryGetValue(key, out obj1) && obj1 is T obj2 ? obj2 : default (T);
    }

    public T SetAssociatedObject<T>(string key, T value)
    {
      lock (this.m_associatedObjectsWriteLock)
      {
        object obj1;
        if (this.m_associatedObjects != null && this.m_associatedObjects.TryGetValue(key, out obj1) && obj1 is T obj2)
          return obj2;
        Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
        Dictionary<string, object> associatedObjects = this.m_associatedObjects;
        if (associatedObjects != null)
        {
          foreach (KeyValuePair<string, object> keyValuePair in associatedObjects)
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        dictionary[key] = (object) value;
        this.m_associatedObjects = dictionary;
        return value;
      }
    }
  }
}
