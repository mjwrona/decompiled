// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.FastODataPrimitiveSerializer
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class FastODataPrimitiveSerializer : ODataPrimitiveSerializer
  {
    private static readonly ODataNullValue NullValue = new ODataNullValue();

    public override ODataValue CreateODataValue(
      object value,
      IEdmTypeReference expectedType,
      ODataSerializerContext writeContext)
    {
      if (value == null)
        return (ODataValue) FastODataPrimitiveSerializer.NullValue;
      Type underlyingTypeOrSelf = FastODataPrimitiveSerializer.GetUnderlyingTypeOrSelf(value.GetType());
      TypeCode typeCode = Type.GetTypeCode(underlyingTypeOrSelf);
      ODataPrimitiveValue odataValue = new ODataPrimitiveValue(FastODataPrimitiveSerializer.PreparePrimitiveType(value, expectedType, underlyingTypeOrSelf, typeCode));
      if (writeContext != null && !FastODataPrimitiveSerializer.ShouldSuppressTypeNameSerialization(odataValue.Value, typeCode, writeContext.MetadataLevel))
        odataValue.TypeAnnotation = new ODataTypeAnnotation(expectedType.FullName());
      return (ODataValue) odataValue;
    }

    public override ODataPrimitiveValue CreateODataPrimitiveValue(
      object value,
      IEdmPrimitiveTypeReference primitiveType,
      ODataSerializerContext writeContext)
    {
      throw new NotImplementedException(AnalyticsResources.METHOD_SHOULD_NOT_BE_CALLED());
    }

    public static Type GetUnderlyingTypeOrSelf(Type type)
    {
      Type underlyingType = Nullable.GetUnderlyingType(type);
      return (object) underlyingType != null ? underlyingType : type;
    }

    private static object PreparePrimitiveType(
      object value,
      IEdmTypeReference primitiveType,
      Type type,
      TypeCode typeCode)
    {
      if (primitiveType != null && primitiveType.IsDate() && typeCode == TypeCode.DateTime)
        return (object) (Date) (DateTime) value;
      return primitiveType != null && primitiveType.IsTimeOfDay() && type == typeof (TimeSpan) ? (object) (TimeOfDay) (TimeSpan) value : value;
    }

    private static bool ShouldSuppressTypeNameSerialization(
      object value,
      TypeCode typeCode,
      ODataMetadataLevel metadataLevel)
    {
      return metadataLevel != ODataMetadataLevel.FullMetadata || FastODataPrimitiveSerializer.CanTypeBeInferredInJson(value, typeCode);
    }

    private static bool CanTypeBeInferredInJson(object value, TypeCode typeCode)
    {
      switch (typeCode)
      {
        case TypeCode.Boolean:
        case TypeCode.Int32:
        case TypeCode.String:
          return true;
        case TypeCode.Double:
          double d = (double) value;
          return !double.IsNaN(d) && !double.IsInfinity(d);
        default:
          return false;
      }
    }
  }
}
