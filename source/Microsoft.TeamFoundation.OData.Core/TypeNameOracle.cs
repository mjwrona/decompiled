// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.TypeNameOracle
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData
{
  internal class TypeNameOracle
  {
    internal static IEdmType ResolveAndValidateTypeName(
      IEdmModel model,
      string typeName,
      EdmTypeKind expectedTypeKind,
      bool? expectStructuredType,
      IWriterValidator writerValidator)
    {
      switch (typeName)
      {
        case null:
          if (model.IsUserModel())
            throw new ODataException(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
          return (IEdmType) null;
        case "":
          throw new ODataException(Strings.ValidationUtils_TypeNameMustNotBeEmpty);
        default:
          if (!model.IsUserModel())
            return (IEdmType) null;
          IEdmType edmType = MetadataUtils.ResolveTypeNameForWrite(model, typeName);
          if (edmType == null)
            throw new ODataException(Strings.ValidationUtils_UnrecognizedTypeName((object) typeName));
          if (edmType.TypeKind != EdmTypeKind.Untyped)
            writerValidator.ValidateTypeKind(edmType.TypeKind, expectedTypeKind, expectStructuredType, edmType);
          return edmType;
      }
    }

    internal static IEdmStructuredType ResolveAndValidateTypeFromTypeName(
      IEdmModel model,
      IEdmStructuredType expectedType,
      string typeName,
      IWriterValidator writerValidator)
    {
      if (typeName == null && expectedType != null)
        return expectedType;
      IEdmType type = TypeNameOracle.ResolveAndValidateTypeName(model, typeName, EdmTypeKind.None, new bool?(true), writerValidator);
      IEdmTypeReference edmTypeReference = TypeNameOracle.ResolveTypeFromMetadataAndValue(expectedType.ToTypeReference(), type == null ? (IEdmTypeReference) null : type.ToTypeReference(), writerValidator);
      if (edmTypeReference != null && edmTypeReference.IsUntyped())
        return (IEdmStructuredType) new EdmUntypedStructuredType();
      return edmTypeReference != null ? edmTypeReference.ToStructuredType() : (IEdmStructuredType) null;
    }

    internal static IEdmTypeReference ResolveAndValidateTypeForPrimitiveValue(
      ODataPrimitiveValue primitiveValue)
    {
      return (IEdmTypeReference) EdmLibraryExtensions.GetPrimitiveTypeReference(primitiveValue.Value.GetType());
    }

    internal static IEdmTypeReference ResolveAndValidateTypeForEnumValue(
      IEdmModel model,
      ODataEnumValue enumValue,
      bool isOpenPropertyType)
    {
      TypeNameOracle.ValidateIfTypeNameMissing(enumValue.TypeName, model, isOpenPropertyType);
      return (IEdmTypeReference) null;
    }

    internal static IEdmTypeReference ResolveAndValidateTypeForResourceValue(
      IEdmModel model,
      IEdmTypeReference typeReferenceFromMetadata,
      ODataResourceValue resourceValue,
      bool isOpenPropertyType,
      IWriterValidator writerValidator)
    {
      string typeName = resourceValue.TypeName;
      TypeNameOracle.ValidateIfTypeNameMissing(typeName, model, isOpenPropertyType);
      IEdmType edmType = typeName == null ? (IEdmType) null : TypeNameOracle.ResolveAndValidateTypeName(model, typeName, EdmTypeKind.Complex, new bool?(true), writerValidator);
      if (typeReferenceFromMetadata != null)
        writerValidator.ValidateTypeKind(EdmTypeKind.Complex, typeReferenceFromMetadata.TypeKind(), new bool?(true), edmType);
      return TypeNameOracle.ResolveTypeFromMetadataAndValue(typeReferenceFromMetadata, edmType == null ? (IEdmTypeReference) null : edmType.ToTypeReference(), writerValidator);
    }

    internal static IEdmTypeReference ResolveAndValidateTypeForCollectionValue(
      IEdmModel model,
      IEdmTypeReference typeReferenceFromMetadata,
      ODataCollectionValue collectionValue,
      bool isOpenPropertyType,
      IWriterValidator writerValidator)
    {
      string typeName = collectionValue.TypeName;
      TypeNameOracle.ValidateIfTypeNameMissing(typeName, model, isOpenPropertyType);
      IEdmType edmType = typeName == null ? (IEdmType) null : TypeNameOracle.ResolveAndValidateTypeName(model, typeName, EdmTypeKind.Collection, new bool?(false), writerValidator);
      if (typeReferenceFromMetadata != null)
        writerValidator.ValidateTypeKind(EdmTypeKind.Collection, typeReferenceFromMetadata.TypeKind(), new bool?(false), edmType);
      IEdmTypeReference typeReference = TypeNameOracle.ResolveTypeFromMetadataAndValue(typeReferenceFromMetadata, edmType == null ? (IEdmTypeReference) null : edmType.ToTypeReference(), writerValidator);
      if (typeReference != null)
      {
        if (typeReferenceFromMetadata != null)
          typeReference = typeReferenceFromMetadata;
        typeReference = (IEdmTypeReference) ValidationUtils.ValidateCollectionType(typeReference);
      }
      return typeReference;
    }

    internal static bool TryGetTypeNameFromAnnotation(ODataValue value, out string propertyName)
    {
      if (value.TypeAnnotation != null)
      {
        propertyName = value.TypeAnnotation.TypeName;
        return true;
      }
      propertyName = (string) null;
      return false;
    }

    protected static string GetTypeNameFromValue(object value)
    {
      switch (value)
      {
        case ODataPrimitiveValue odataPrimitiveValue:
          IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(odataPrimitiveValue.Value.GetType());
          return primitiveTypeReference != null ? primitiveTypeReference.FullName() : (string) null;
        case ODataEnumValue odataEnumValue:
          return odataEnumValue.TypeName;
        case ODataResourceValue odataResourceValue:
          return odataResourceValue.TypeName;
        case ODataCollectionValue odataCollectionValue:
          return EdmLibraryExtensions.GetCollectionTypeFullName(odataCollectionValue.TypeName);
        case ODataBinaryStreamValue _:
          return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Binary, true).FullName();
        case ODataStreamReferenceValue _:
          return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Stream, true).FullName();
        default:
          return (EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType()) ?? throw new ODataException(Strings.ValidationUtils_UnsupportedPrimitiveType((object) value.GetType().FullName))).FullName();
      }
    }

    private static void ValidateIfTypeNameMissing(
      string typeName,
      IEdmModel model,
      bool isOpenPropertyType)
    {
      if (((typeName != null ? 0 : (model.IsUserModel() ? 1 : 0)) & (isOpenPropertyType ? 1 : 0)) != 0)
        throw new ODataException(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
    }

    private static IEdmTypeReference ResolveTypeFromMetadataAndValue(
      IEdmTypeReference typeReferenceFromMetadata,
      IEdmTypeReference typeReferenceFromValue,
      IWriterValidator writerValidator)
    {
      if (typeReferenceFromMetadata == null)
        return typeReferenceFromValue;
      if (typeReferenceFromValue == null)
        return typeReferenceFromMetadata;
      writerValidator.ValidateTypeReference(typeReferenceFromMetadata, typeReferenceFromValue);
      return typeReferenceFromValue;
    }
  }
}
