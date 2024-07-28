// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.PropertyCacheHandler
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OData
{
  internal class PropertyCacheHandler
  {
    private const string PropertyTypeDelimiter = "-";
    private readonly Stack<PropertyCache> cacheStack = new Stack<PropertyCache>();
    private readonly Stack<int> scopeLevelStack = new Stack<int>();
    private readonly Dictionary<IEdmStructuredType, PropertyCache> cacheDictionary = new Dictionary<IEdmStructuredType, PropertyCache>();
    private PropertyCache currentPropertyCache;
    private int resourceSetScopeLevel;
    private int currentResourceScopeLevel;

    public PropertySerializationInfo GetProperty(
      IEdmModel model,
      string name,
      IEdmStructuredType owningType)
    {
      int num = this.currentResourceScopeLevel - this.resourceSetScopeLevel;
      string str = num == 1 ? string.Empty : num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      string uniqueName = owningType != null ? owningType.FullTypeName() + "-" + str + name : str + name;
      return this.currentPropertyCache.GetProperty(model, name, uniqueName, owningType);
    }

    public void SetCurrentResourceScopeLevel(int level) => this.currentResourceScopeLevel = level;

    public void EnterResourceSetScope(IEdmStructuredType resourceType, int scopeLevel)
    {
      PropertyCache propertyCache;
      if (resourceType != null)
      {
        if (!this.cacheDictionary.TryGetValue(resourceType, out propertyCache))
        {
          propertyCache = new PropertyCache();
          this.cacheDictionary[resourceType] = propertyCache;
        }
      }
      else
        propertyCache = new PropertyCache();
      this.cacheStack.Push(this.currentPropertyCache);
      this.currentPropertyCache = propertyCache;
      this.scopeLevelStack.Push(this.resourceSetScopeLevel);
      this.resourceSetScopeLevel = scopeLevel;
    }

    public void LeaveResourceSetScope()
    {
      this.resourceSetScopeLevel = this.scopeLevelStack.Pop();
      this.currentPropertyCache = this.cacheStack.Pop();
    }

    public bool InResourceSetScope() => this.resourceSetScopeLevel > 0;
  }
}
