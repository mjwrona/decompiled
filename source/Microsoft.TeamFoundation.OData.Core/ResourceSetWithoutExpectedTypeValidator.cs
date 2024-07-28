// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ResourceSetWithoutExpectedTypeValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData
{
  internal sealed class ResourceSetWithoutExpectedTypeValidator
  {
    private IEdmType itemType;

    public ResourceSetWithoutExpectedTypeValidator(IEdmType memberType) => this.itemType = memberType;

    internal void ValidateResource(IEdmType itemType)
    {
      if (this.itemType == null || this.itemType.TypeKind == EdmTypeKind.Untyped || this.itemType.IsEquivalentTo(itemType))
        return;
      IEdmStructuredType edmStructuredType = itemType as IEdmStructuredType;
      IEdmStructuredType itemType1 = this.itemType as IEdmStructuredType;
      if (edmStructuredType == null || itemType1 == null)
        throw new ODataException(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes((object) itemType.FullTypeName(), (object) this.itemType.FullTypeName()));
      if (!this.itemType.IsAssignableFrom(itemType))
        throw new ODataException(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes((object) itemType.FullTypeName(), (object) this.itemType.FullTypeName()));
    }
  }
}
