// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmTypeExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
  public static class EdmTypeExtensions
  {
    public static bool IsDeltaFeed(this IEdmType type)
    {
      if (type == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      return type.GetType() == typeof (EdmDeltaCollectionType);
    }

    public static bool IsDeltaResource(this IEdmObject resource)
    {
      if (resource == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resource));
      return resource is EdmDeltaEntityObject || resource is EdmDeltaComplexObject;
    }
  }
}
