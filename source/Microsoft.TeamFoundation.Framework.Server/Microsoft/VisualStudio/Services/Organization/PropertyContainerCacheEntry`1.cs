// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyContainerCacheEntry`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class PropertyContainerCacheEntry<T> where T : ICloneable, IPropertyContainer
  {
    internal T PropertyContainer { get; private set; }

    internal HashSet<string> PropertyNames { get; private set; }

    public PropertyContainerCacheEntry(T propertyContainer, HashSet<string> propertyNames)
    {
      this.PropertyContainer = propertyContainer;
      this.PropertyNames = propertyNames;
    }
  }
}
