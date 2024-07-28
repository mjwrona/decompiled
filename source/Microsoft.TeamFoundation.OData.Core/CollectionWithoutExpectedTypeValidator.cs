// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.CollectionWithoutExpectedTypeValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;

namespace Microsoft.OData
{
  internal sealed class CollectionWithoutExpectedTypeValidator
  {
    private readonly bool itemTypeDerivedFromCollectionValue;
    private string itemTypeName;
    private IEdmPrimitiveType primitiveItemType;
    private EdmTypeKind itemTypeKind;

    internal CollectionWithoutExpectedTypeValidator(string itemTypeNameFromCollection)
    {
      if (itemTypeNameFromCollection == null)
        return;
      this.itemTypeName = CollectionWithoutExpectedTypeValidator.GetItemTypeFullName(itemTypeNameFromCollection);
      this.itemTypeKind = CollectionWithoutExpectedTypeValidator.ComputeExpectedTypeKind(this.itemTypeName, out this.primitiveItemType);
      this.itemTypeDerivedFromCollectionValue = true;
    }

    internal string ItemTypeNameFromCollection => !this.itemTypeDerivedFromCollectionValue ? (string) null : this.itemTypeName;

    internal EdmTypeKind ItemTypeKindFromCollection => !this.itemTypeDerivedFromCollectionValue ? EdmTypeKind.None : this.itemTypeKind;

    internal void ValidateCollectionItem(
      string collectionItemTypeName,
      EdmTypeKind collectionItemTypeKind)
    {
      if (collectionItemTypeKind != EdmTypeKind.Primitive && collectionItemTypeKind != EdmTypeKind.Enum && collectionItemTypeKind != EdmTypeKind.Complex)
        throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind((object) collectionItemTypeKind));
      if (this.itemTypeDerivedFromCollectionValue)
      {
        collectionItemTypeName = collectionItemTypeName ?? this.itemTypeName;
        this.ValidateCollectionItemTypeNameAndKind(collectionItemTypeName, collectionItemTypeKind);
      }
      else
      {
        if (this.itemTypeKind == EdmTypeKind.None)
        {
          this.itemTypeKind = collectionItemTypeName == null ? collectionItemTypeKind : CollectionWithoutExpectedTypeValidator.ComputeExpectedTypeKind(collectionItemTypeName, out this.primitiveItemType);
          if (collectionItemTypeName == null)
          {
            this.itemTypeKind = collectionItemTypeKind;
            if (this.itemTypeKind == EdmTypeKind.Primitive)
            {
              this.itemTypeName = "Edm.String";
              this.primitiveItemType = EdmCoreModel.Instance.GetString(false).PrimitiveDefinition();
            }
            else
            {
              this.itemTypeName = (string) null;
              this.primitiveItemType = (IEdmPrimitiveType) null;
            }
          }
          else
          {
            this.itemTypeKind = CollectionWithoutExpectedTypeValidator.ComputeExpectedTypeKind(collectionItemTypeName, out this.primitiveItemType);
            this.itemTypeName = collectionItemTypeName;
          }
        }
        if (collectionItemTypeName == null && collectionItemTypeKind == EdmTypeKind.Primitive)
          collectionItemTypeName = "Edm.String";
        this.ValidateCollectionItemTypeNameAndKind(collectionItemTypeName, collectionItemTypeKind);
      }
    }

    private static EdmTypeKind ComputeExpectedTypeKind(
      string typeName,
      out IEdmPrimitiveType primitiveItemType)
    {
      IEdmSchemaType declaredType = EdmCoreModel.Instance.FindDeclaredType(typeName);
      if (declaredType != null)
      {
        primitiveItemType = (IEdmPrimitiveType) declaredType;
        return EdmTypeKind.Primitive;
      }
      primitiveItemType = (IEdmPrimitiveType) null;
      return EdmTypeKind.Complex;
    }

    private static string GetItemTypeFullName(string typeName)
    {
      IEdmSchemaType declaredType = EdmCoreModel.Instance.FindDeclaredType(typeName);
      return declaredType != null ? declaredType.FullName() : typeName;
    }

    private void ValidateCollectionItemTypeNameAndKind(
      string collectionItemTypeName,
      EdmTypeKind collectionItemTypeKind)
    {
      if (this.itemTypeKind != collectionItemTypeKind)
        throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind((object) collectionItemTypeKind, (object) this.itemTypeKind));
      if (this.itemTypeKind == EdmTypeKind.Primitive)
      {
        if (string.IsNullOrEmpty(this.itemTypeName) || !this.itemTypeName.Equals(collectionItemTypeName, StringComparison.OrdinalIgnoreCase))
        {
          if (this.primitiveItemType.IsSpatial())
          {
            EdmPrimitiveTypeKind primitiveTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(collectionItemTypeName);
            IEdmPrimitiveType primitiveType = EdmCoreModel.Instance.GetPrimitiveType(primitiveTypeKind);
            if (this.itemTypeDerivedFromCollectionValue)
            {
              if (EdmLibraryExtensions.IsAssignableFrom(this.primitiveItemType, primitiveType))
                return;
            }
            else
            {
              IEdmPrimitiveType commonBaseType = this.primitiveItemType.GetCommonBaseType(primitiveType);
              if (commonBaseType != null)
              {
                this.primitiveItemType = commonBaseType;
                this.itemTypeName = commonBaseType.FullTypeName();
                return;
              }
            }
          }
          throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName((object) collectionItemTypeName, (object) this.itemTypeName));
        }
      }
      else if (string.CompareOrdinal(this.itemTypeName, collectionItemTypeName) != 0)
        throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName((object) collectionItemTypeName, (object) this.itemTypeName));
    }
  }
}
