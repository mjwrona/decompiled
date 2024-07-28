// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class PropertyExtension
  {
    public static PropertiesCollection ToPropertiesCollection(this PropertyBag propertyBag)
    {
      if (propertyBag == null)
        return (PropertiesCollection) null;
      PropertiesCollection propertiesCollection = new PropertiesCollection();
      foreach (string key in propertyBag.Keys)
        propertiesCollection[key] = propertyBag[key];
      return propertiesCollection;
    }
  }
}
