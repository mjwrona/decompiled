// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmStructuredValue
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmStructuredValue : EdmValue, IEdmStructuredValue, IEdmValue, IEdmElement
  {
    private readonly IEnumerable<IEdmPropertyValue> propertyValues;
    private readonly Cache<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>> propertiesDictionaryCache;
    private static readonly Func<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>> ComputePropertiesDictionaryFunc = (Func<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>>) (me => me.ComputePropertiesDictionary());

    public EdmStructuredValue(
      IEdmStructuredTypeReference type,
      IEnumerable<IEdmPropertyValue> propertyValues)
      : base((IEdmTypeReference) type)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmPropertyValue>>(propertyValues, nameof (propertyValues));
      this.propertyValues = propertyValues;
      if (propertyValues == null || propertyValues.Count<IEdmPropertyValue>() <= 5)
        return;
      this.propertiesDictionaryCache = new Cache<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>>();
    }

    public IEnumerable<IEdmPropertyValue> PropertyValues => this.propertyValues;

    public override EdmValueKind ValueKind => EdmValueKind.Structured;

    private Dictionary<string, IEdmPropertyValue> PropertiesDictionary => this.propertiesDictionaryCache != null ? this.propertiesDictionaryCache.GetValue(this, EdmStructuredValue.ComputePropertiesDictionaryFunc, (Func<EdmStructuredValue, Dictionary<string, IEdmPropertyValue>>) null) : (Dictionary<string, IEdmPropertyValue>) null;

    public IEdmPropertyValue FindPropertyValue(string propertyName)
    {
      Dictionary<string, IEdmPropertyValue> propertiesDictionary = this.PropertiesDictionary;
      if (propertiesDictionary != null)
      {
        IEdmPropertyValue propertyValue;
        propertiesDictionary.TryGetValue(propertyName, out propertyValue);
        return propertyValue;
      }
      foreach (IEdmPropertyValue propertyValue in this.propertyValues)
      {
        if (propertyValue.Name == propertyName)
          return propertyValue;
      }
      return (IEdmPropertyValue) null;
    }

    private Dictionary<string, IEdmPropertyValue> ComputePropertiesDictionary()
    {
      Dictionary<string, IEdmPropertyValue> propertiesDictionary = new Dictionary<string, IEdmPropertyValue>();
      foreach (IEdmPropertyValue propertyValue in this.propertyValues)
        propertiesDictionary[propertyValue.Name] = propertyValue;
      return propertiesDictionary;
    }
  }
}
