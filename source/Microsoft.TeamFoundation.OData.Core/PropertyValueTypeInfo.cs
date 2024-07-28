// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.PropertyValueTypeInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
  internal class PropertyValueTypeInfo
  {
    public PropertyValueTypeInfo(string typeName, IEdmTypeReference typeReference)
    {
      this.TypeName = typeName;
      this.TypeReference = typeReference;
      if (typeReference == null)
        return;
      this.FullName = typeReference.FullName();
      this.IsPrimitive = typeReference.IsPrimitive();
      this.IsComplex = typeReference.IsComplex();
      this.PrimitiveTypeKind = this.IsPrimitive ? ExtensionMethods.PrimitiveKind(typeReference.AsPrimitive()) : EdmPrimitiveTypeKind.None;
    }

    public IEdmTypeReference TypeReference { get; private set; }

    public string FullName { get; private set; }

    public bool IsPrimitive { get; private set; }

    public bool IsComplex { get; private set; }

    public string TypeName { get; private set; }

    public EdmPrimitiveTypeKind PrimitiveTypeKind { get; private set; }
  }
}
