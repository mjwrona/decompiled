// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.EdmObjectHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System.Collections;

namespace Microsoft.AspNet.OData.Formatter
{
  internal static class EdmObjectHelper
  {
    public static IEdmObject ConvertToEdmObject(
      this IEnumerable enumerable,
      IEdmCollectionTypeReference collectionType)
    {
      IEdmTypeReference type = collectionType.ElementType();
      if (type.IsEntity())
      {
        EdmEntityObjectCollection edmObject = new EdmEntityObjectCollection(collectionType);
        foreach (EdmEntityObject edmEntityObject in enumerable)
          edmObject.Add((IEdmEntityObject) edmEntityObject);
        return (IEdmObject) edmObject;
      }
      if (type.IsComplex())
      {
        EdmComplexObjectCollection edmObject = new EdmComplexObjectCollection(collectionType);
        foreach (EdmComplexObject edmComplexObject in enumerable)
          edmObject.Add((IEdmComplexObject) edmComplexObject);
        return (IEdmObject) edmObject;
      }
      if (!type.IsEnum())
        return (IEdmObject) null;
      EdmEnumObjectCollection edmObject1 = new EdmEnumObjectCollection(collectionType);
      foreach (EdmEnumObject edmEnumObject in enumerable)
        edmObject1.Add((IEdmEnumObject) edmEnumObject);
      return (IEdmObject) edmObject1;
    }
  }
}
