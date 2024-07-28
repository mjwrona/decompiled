// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.ToTraceStringExtensionMethods
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Text;

namespace Microsoft.OData.Edm
{
  public static class ToTraceStringExtensionMethods
  {
    public static string ToTraceString(this IEdmSchemaType schemaType) => ToTraceStringExtensionMethods.ToTraceString(schemaType);

    public static string ToTraceString(this IEdmSchemaElement schemaElement) => schemaElement.FullName();

    public static string ToTraceString(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      switch (type.TypeKind)
      {
        case EdmTypeKind.Collection:
          return ToTraceStringExtensionMethods.ToTraceString((IEdmCollectionType) type);
        case EdmTypeKind.EntityReference:
          return ToTraceStringExtensionMethods.ToTraceString((IEdmEntityReferenceType) type);
        default:
          return !(type is IEdmSchemaType schemaType) ? "UnknownType" : ToTraceStringExtensionMethods.ToTraceString(schemaType);
      }
    }

    public static string ToTraceString(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      StringBuilder sb = new StringBuilder();
      sb.Append('[');
      if (type.Definition != null)
      {
        sb.Append(type.Definition.ToTraceString());
        sb.AppendKeyValue("Nullable", type.IsNullable.ToString());
        if (type.IsPrimitive())
          sb.AppendFacets(type.AsPrimitive());
      }
      sb.Append(']');
      return sb.ToString();
    }

    public static string ToTraceString(this IEdmProperty property)
    {
      EdmUtil.CheckArgumentNull<IEdmProperty>(property, nameof (property));
      return (property.Name != null ? property.Name : "") + ":" + (property.Type != null ? property.Type.ToTraceString() : "");
    }

    private static string ToTraceString(this IEdmEntityReferenceType type) => EdmTypeKind.EntityReference.ToString() + "(" + (type.EntityType != null ? ToTraceStringExtensionMethods.ToTraceString(type.EntityType) : "") + ")";

    private static string ToTraceString(this IEdmCollectionType type) => EdmTypeKind.Collection.ToString() + "(" + (type.ElementType != null ? type.ElementType.ToTraceString() : "") + ")";

    private static void AppendFacets(this StringBuilder sb, IEdmPrimitiveTypeReference type)
    {
      switch (ExtensionMethods.PrimitiveKind(type))
      {
        case EdmPrimitiveTypeKind.Binary:
          sb.AppendBinaryFacets(type.AsBinary());
          break;
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Duration:
          sb.AppendTemporalFacets(type.AsTemporal());
          break;
        case EdmPrimitiveTypeKind.Decimal:
          sb.AppendDecimalFacets(type.AsDecimal());
          break;
        case EdmPrimitiveTypeKind.String:
          sb.AppendStringFacets(type.AsString());
          break;
        case EdmPrimitiveTypeKind.Geography:
        case EdmPrimitiveTypeKind.GeographyPoint:
        case EdmPrimitiveTypeKind.GeographyLineString:
        case EdmPrimitiveTypeKind.GeographyPolygon:
        case EdmPrimitiveTypeKind.GeographyCollection:
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
        case EdmPrimitiveTypeKind.Geometry:
        case EdmPrimitiveTypeKind.GeometryPoint:
        case EdmPrimitiveTypeKind.GeometryLineString:
        case EdmPrimitiveTypeKind.GeometryPolygon:
        case EdmPrimitiveTypeKind.GeometryCollection:
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          sb.AppendSpatialFacets(type.AsSpatial());
          break;
      }
    }

    private static void AppendBinaryFacets(this StringBuilder sb, IEdmBinaryTypeReference type)
    {
      if (!type.IsUnbounded && !type.MaxLength.HasValue)
        return;
      sb.AppendKeyValue("MaxLength", type.IsUnbounded ? "max" : type.MaxLength.ToString());
    }

    private static void AppendStringFacets(this StringBuilder sb, IEdmStringTypeReference type)
    {
      if (type.IsUnbounded || type.MaxLength.HasValue)
        sb.AppendKeyValue("MaxLength", type.IsUnbounded ? "max" : type.MaxLength.ToString());
      if (!type.IsUnicode.HasValue)
        return;
      sb.AppendKeyValue("Unicode", type.IsUnicode.ToString());
    }

    private static void AppendTemporalFacets(this StringBuilder sb, IEdmTemporalTypeReference type)
    {
      if (!type.Precision.HasValue)
        return;
      sb.AppendKeyValue("Precision", type.Precision.ToString());
    }

    private static void AppendDecimalFacets(this StringBuilder sb, IEdmDecimalTypeReference type)
    {
      int? nullable;
      if (type.Precision.HasValue)
      {
        StringBuilder sb1 = sb;
        nullable = type.Precision;
        string str = nullable.ToString();
        sb1.AppendKeyValue("Precision", str);
      }
      nullable = type.Scale;
      if (!nullable.HasValue)
        return;
      StringBuilder sb2 = sb;
      nullable = type.Scale;
      string str1 = nullable.ToString();
      sb2.AppendKeyValue("Scale", str1);
    }

    private static void AppendSpatialFacets(this StringBuilder sb, IEdmSpatialTypeReference type) => sb.AppendKeyValue("SRID", type.SpatialReferenceIdentifier.HasValue ? type.SpatialReferenceIdentifier.ToString() : "Variable");

    private static void AppendKeyValue(this StringBuilder sb, string key, string value)
    {
      sb.Append(' ');
      sb.Append(key);
      sb.Append('=');
      sb.Append(value);
    }
  }
}
