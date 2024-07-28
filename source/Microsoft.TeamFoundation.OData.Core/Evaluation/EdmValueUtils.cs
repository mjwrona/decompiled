// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.EdmValueUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.Spatial;
using System;

namespace Microsoft.OData.Evaluation
{
  internal static class EdmValueUtils
  {
    internal static IEdmDelayedValue ConvertPrimitiveValue(
      object primitiveValue,
      IEdmPrimitiveTypeReference type)
    {
      switch (primitiveValue)
      {
        case bool flag:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Boolean);
          return (IEdmDelayedValue) new EdmBooleanConstant(type, flag);
        case byte num1:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Byte);
          return (IEdmDelayedValue) new EdmIntegerConstant(type, (long) num1);
        case sbyte num2:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.SByte);
          return (IEdmDelayedValue) new EdmIntegerConstant(type, (long) num2);
        case short num3:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Int16);
          return (IEdmDelayedValue) new EdmIntegerConstant(type, (long) num3);
        case int num4:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Int32);
          return (IEdmDelayedValue) new EdmIntegerConstant(type, (long) num4);
        case long num5:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Int64);
          return (IEdmDelayedValue) new EdmIntegerConstant(type, num5);
        case Decimal num6:
          return (IEdmDelayedValue) new EdmDecimalConstant((IEdmDecimalTypeReference) EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Decimal), num6);
        case float num7:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Single);
          return (IEdmDelayedValue) new EdmFloatingConstant(type, (double) num7);
        case double num8:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Double);
          return (IEdmDelayedValue) new EdmFloatingConstant(type, num8);
        case string str:
          return (IEdmDelayedValue) new EdmStringConstant((IEdmStringTypeReference) EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.String), str);
        default:
          return EdmValueUtils.ConvertPrimitiveValueWithoutTypeCode(primitiveValue, type);
      }
    }

    internal static object ToClrValue(this IEdmPrimitiveValue edmValue)
    {
      EdmPrimitiveTypeKind primitiveKind = edmValue.Type.PrimitiveKind();
      switch (edmValue.ValueKind)
      {
        case EdmValueKind.Binary:
          return (object) ((IEdmBinaryValue) edmValue).Value;
        case EdmValueKind.Boolean:
          return (object) ((IEdmBooleanValue) edmValue).Value;
        case EdmValueKind.DateTimeOffset:
          return (object) ((IEdmDateTimeOffsetValue) edmValue).Value;
        case EdmValueKind.Decimal:
          return (object) ((IEdmDecimalValue) edmValue).Value;
        case EdmValueKind.Floating:
          return EdmValueUtils.ConvertFloatingValue((IEdmFloatingValue) edmValue, primitiveKind);
        case EdmValueKind.Guid:
          return (object) ((IEdmGuidValue) edmValue).Value;
        case EdmValueKind.Integer:
          return EdmValueUtils.ConvertIntegerValue((IEdmIntegerValue) edmValue, primitiveKind);
        case EdmValueKind.String:
          return (object) ((IEdmStringValue) edmValue).Value;
        case EdmValueKind.Duration:
          return (object) ((IEdmDurationValue) edmValue).Value;
        case EdmValueKind.Date:
          return (object) ((IEdmDateValue) edmValue).Value;
        case EdmValueKind.TimeOfDay:
          return (object) ((IEdmTimeOfDayValue) edmValue).Value;
        default:
          throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_CannotConvertTypeToClrValue((object) edmValue.ValueKind));
      }
    }

    internal static bool TryGetStreamProperty(
      IEdmStructuredValue entityInstance,
      string streamPropertyName,
      out IEdmProperty streamProperty)
    {
      streamProperty = (IEdmProperty) null;
      if (streamPropertyName != null)
      {
        streamProperty = entityInstance.Type.AsEntity().FindProperty(streamPropertyName);
        if (streamProperty == null)
          return false;
      }
      return true;
    }

    internal static object GetPrimitivePropertyClrValue(
      this IEdmStructuredValue structuredValue,
      string propertyName)
    {
      IEdmStructuredTypeReference type = structuredValue.Type.AsStructured();
      IEdmPropertyValue propertyValue = structuredValue.FindPropertyValue(propertyName);
      if (propertyValue == null)
        throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_PropertyDoesntExist((object) type.FullName(), (object) propertyName));
      if (propertyValue.Value.ValueKind == EdmValueKind.Null)
        return (object) null;
      if (!(propertyValue.Value is IEdmPrimitiveValue edmValue))
        throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_NonPrimitiveValue((object) propertyValue.Name, (object) type.FullName()));
      return edmValue.ToClrValue();
    }

    private static object ConvertFloatingValue(
      IEdmFloatingValue floatingValue,
      EdmPrimitiveTypeKind primitiveKind)
    {
      double num = floatingValue.Value;
      return primitiveKind == EdmPrimitiveTypeKind.Single ? (object) Convert.ToSingle(num) : (object) num;
    }

    private static object ConvertIntegerValue(
      IEdmIntegerValue integerValue,
      EdmPrimitiveTypeKind primitiveKind)
    {
      long num = integerValue.Value;
      switch (primitiveKind)
      {
        case EdmPrimitiveTypeKind.Byte:
          return (object) Convert.ToByte(num);
        case EdmPrimitiveTypeKind.Int16:
          return (object) Convert.ToInt16(num);
        case EdmPrimitiveTypeKind.Int32:
          return (object) Convert.ToInt32(num);
        case EdmPrimitiveTypeKind.SByte:
          return (object) Convert.ToSByte(num);
        default:
          return (object) num;
      }
    }

    private static IEdmDelayedValue ConvertPrimitiveValueWithoutTypeCode(
      object primitiveValue,
      IEdmPrimitiveTypeReference type)
    {
      switch (primitiveValue)
      {
        case byte[] numArray:
          return (IEdmDelayedValue) new EdmBinaryConstant((IEdmBinaryTypeReference) EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Binary), numArray);
        case Date date:
          return (IEdmDelayedValue) new EdmDateConstant(EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Date), date);
        case DateTimeOffset dateTimeOffset:
          return (IEdmDelayedValue) new EdmDateTimeOffsetConstant((IEdmTemporalTypeReference) EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.DateTimeOffset), dateTimeOffset);
        case Guid guid:
          type = EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Guid);
          return (IEdmDelayedValue) new EdmGuidConstant(type, guid);
        case TimeOfDay timeOfDay:
          return (IEdmDelayedValue) new EdmTimeOfDayConstant((IEdmTemporalTypeReference) EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.TimeOfDay), timeOfDay);
        case TimeSpan timeSpan:
          return (IEdmDelayedValue) new EdmDurationConstant((IEdmTemporalTypeReference) EdmValueUtils.EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Duration), timeSpan);
        case ISpatial _:
          throw new NotImplementedException();
        default:
          throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_UnsupportedPrimitiveType((object) primitiveValue.GetType().FullName));
      }
    }

    private static IEdmPrimitiveTypeReference EnsurePrimitiveType(
      IEdmPrimitiveTypeReference type,
      EdmPrimitiveTypeKind primitiveKindFromValue)
    {
      if (type == null)
      {
        type = EdmCoreModel.Instance.GetPrimitive(primitiveKindFromValue, true);
      }
      else
      {
        EdmPrimitiveTypeKind primitiveKind = type.PrimitiveDefinition().PrimitiveKind;
        if (primitiveKind != primitiveKindFromValue)
        {
          string p0 = type.FullName();
          if (p0 == null)
            throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName((object) primitiveKind.ToString(), (object) primitiveKindFromValue.ToString()));
          throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_IncorrectPrimitiveTypeKind((object) p0, (object) primitiveKindFromValue.ToString(), (object) primitiveKind.ToString()));
        }
      }
      return type;
    }
  }
}
