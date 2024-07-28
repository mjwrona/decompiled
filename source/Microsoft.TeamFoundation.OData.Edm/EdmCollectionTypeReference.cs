// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmCollectionTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmCollectionTypeReference : 
    EdmTypeReference,
    IEdmCollectionTypeReference,
    IEdmTypeReference,
    IEdmElement
  {
    public EdmCollectionTypeReference(IEdmCollectionType collectionType)
      : base((IEdmType) collectionType, EdmCollectionTypeReference.GetIsNullable(collectionType))
    {
    }

    private static bool GetIsNullable(IEdmCollectionType collectionType)
    {
      IEdmTypeReference elementType = collectionType.ElementType;
      return elementType == null || elementType is IEdmEntityTypeReference || collectionType.ElementType.IsNullable;
    }
  }
}
