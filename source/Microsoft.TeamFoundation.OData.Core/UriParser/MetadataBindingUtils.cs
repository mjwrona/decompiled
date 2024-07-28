// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.MetadataBindingUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class MetadataBindingUtils
  {
    internal static SingleValueNode ConvertToTypeIfNeeded(
      SingleValueNode source,
      IEdmTypeReference targetTypeReference)
    {
      if (targetTypeReference == null)
        return source;
      if (source.TypeReference == null)
        return (SingleValueNode) new ConvertNode(source, targetTypeReference);
      if (source.TypeReference.IsEquivalentTo(targetTypeReference))
        return source.TypeReference.IsTypeDefinition() ? (SingleValueNode) new ConvertNode(source, targetTypeReference) : source;
      if (targetTypeReference.IsStructured() || targetTypeReference.IsStructuredCollectionType())
        return (SingleValueNode) new ConvertNode(source, targetTypeReference);
      if (source is ConstantNode constantNode && constantNode.Value != null && source.TypeReference.IsString() && targetTypeReference.IsEnum())
      {
        string memberName = constantNode.Value.ToString();
        if (!(targetTypeReference.Definition as IEdmEnumType).Members.Any<IEdmEnumMember>((Func<IEdmEnumMember, bool>) (m => string.Compare(m.Name, memberName, StringComparison.Ordinal) == 0)))
          throw new ODataException(Microsoft.OData.Strings.Binder_IsNotValidEnumConstant((object) memberName));
        string uriLiteral = ODataUriUtils.ConvertToUriLiteral(constantNode.Value, ODataVersion.V4);
        return (SingleValueNode) new ConstantNode((object) new ODataEnumValue(constantNode.Value.ToString(), targetTypeReference.Definition.ToString()), uriLiteral, targetTypeReference);
      }
      if (!TypePromotionUtils.CanConvertTo(source, source.TypeReference, targetTypeReference))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_CannotConvertToType((object) source.TypeReference.FullName(), (object) targetTypeReference.FullName()));
      if (source.TypeReference.IsEnum() && constantNode != null)
        return (SingleValueNode) new ConstantNode(constantNode.Value, ODataUriUtils.ConvertToUriLiteral(constantNode.Value, ODataVersion.V4), targetTypeReference);
      object primitiveValue;
      if (!MetadataUtilsCommon.TryGetConstantNodePrimitiveLDMF(source, out primitiveValue) || primitiveValue == null)
        return (SingleValueNode) new ConvertNode(source, targetTypeReference);
      object constantValue = ODataUriConversionUtils.CoerceNumericType(primitiveValue, targetTypeReference.AsPrimitive().Definition as IEdmPrimitiveType);
      if (string.IsNullOrEmpty(constantNode.LiteralText))
        return (SingleValueNode) new ConstantNode(constantValue);
      ConstantNode source1 = new ConstantNode(constantValue, constantNode.LiteralText);
      if (!(source1.TypeReference is IEdmDecimalTypeReference typeReference))
        return (SingleValueNode) source1;
      IEdmDecimalTypeReference decimalTypeReference = (IEdmDecimalTypeReference) targetTypeReference;
      int? precision = typeReference.Precision;
      int? nullable = decimalTypeReference.Precision;
      if (precision.GetValueOrDefault() == nullable.GetValueOrDefault() & precision.HasValue == nullable.HasValue)
      {
        nullable = typeReference.Scale;
        int? scale = decimalTypeReference.Scale;
        if (nullable.GetValueOrDefault() == scale.GetValueOrDefault() & nullable.HasValue == scale.HasValue)
          return (SingleValueNode) source1;
      }
      return (SingleValueNode) new ConvertNode((SingleValueNode) source1, targetTypeReference);
    }

    internal static IEdmType GetEdmType(this QueryNode segment)
    {
      switch (segment)
      {
        case SingleValueNode singleValueNode:
          return singleValueNode.TypeReference?.Definition;
        case CollectionNode collectionNode:
          return collectionNode.ItemType?.Definition;
        default:
          return (IEdmType) null;
      }
    }

    internal static IEdmTypeReference GetEdmTypeReference(this QueryNode segment)
    {
      switch (segment)
      {
        case SingleValueNode singleValueNode:
          return singleValueNode.TypeReference;
        case CollectionNode collectionNode:
          return collectionNode.ItemType;
        default:
          return (IEdmTypeReference) null;
      }
    }
  }
}
