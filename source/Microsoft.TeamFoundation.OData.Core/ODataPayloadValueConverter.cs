// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataPayloadValueConverter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using System;
using System.Globalization;

namespace Microsoft.OData
{
  public class ODataPayloadValueConverter
  {
    private static readonly ODataPayloadValueConverter Default = new ODataPayloadValueConverter();

    public virtual object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference) => value;

    public virtual object ConvertFromPayloadValue(object value, IEdmTypeReference edmTypeReference)
    {
      IEdmPrimitiveTypeReference primitiveTypeReference = edmTypeReference as IEdmPrimitiveTypeReference;
      if (ExtensionMethods.PrimitiveKind(primitiveTypeReference) == EdmPrimitiveTypeKind.PrimitiveType)
        return value;
      try
      {
        Type primitiveClrType = EdmLibraryExtensions.GetPrimitiveClrType(primitiveTypeReference.PrimitiveDefinition(), false);
        switch (value)
        {
          case string stringValue:
            return ODataPayloadValueConverter.ConvertStringValue(stringValue, primitiveClrType);
          case int intValue:
            return ODataPayloadValueConverter.ConvertInt32Value(intValue, primitiveClrType, primitiveTypeReference);
          case Decimal num1:
            if (primitiveClrType == typeof (long))
              return (object) Convert.ToInt64(num1);
            if (primitiveClrType == typeof (double))
              return (object) Convert.ToDouble(num1);
            if (primitiveClrType == typeof (float))
              return (object) Convert.ToSingle(num1);
            if (primitiveClrType != typeof (Decimal))
              throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertDecimal((object) primitiveTypeReference.FullName()));
            break;
          case double num2:
            if (primitiveClrType == typeof (float))
              return (object) Convert.ToSingle(num2);
            if (primitiveClrType != typeof (double))
              throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertDouble((object) primitiveTypeReference.FullName()));
            break;
          case bool _:
            if (primitiveClrType != typeof (bool))
              throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertBoolean((object) primitiveTypeReference.FullName()));
            break;
          case DateTime _:
            if (primitiveClrType != typeof (DateTime))
              throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertDateTime((object) primitiveTypeReference.FullName()));
            break;
          case DateTimeOffset _:
            if (primitiveClrType != typeof (DateTimeOffset))
              throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertDateTimeOffset((object) primitiveTypeReference.FullName()));
            break;
        }
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          throw ReaderValidationUtils.GetPrimitiveTypeConversionException(primitiveTypeReference, ex, value.ToString());
        throw;
      }
      return value;
    }

    internal static ODataPayloadValueConverter GetPayloadValueConverter(IServiceProvider container) => container == null ? ODataPayloadValueConverter.Default : container.GetRequiredService<ODataPayloadValueConverter>();

    private static object ConvertStringValue(string stringValue, Type targetType)
    {
      if (targetType == typeof (byte[]))
        return (object) Convert.FromBase64String(stringValue);
      if (targetType == typeof (Guid))
        return (object) new Guid(stringValue);
      if (targetType == typeof (TimeSpan))
        return (object) EdmValueParser.ParseDuration(stringValue);
      if (targetType == typeof (Date))
        return (object) PlatformHelper.ConvertStringToDate(stringValue);
      if (targetType == typeof (TimeOfDay))
        return (object) PlatformHelper.ConvertStringToTimeOfDay(stringValue);
      if (targetType == typeof (DateTimeOffset))
        return (object) PlatformHelper.ConvertStringToDateTimeOffset(stringValue);
      if (!(targetType == typeof (double)) && !(targetType == typeof (float)))
        return Convert.ChangeType((object) stringValue, targetType, (IFormatProvider) CultureInfo.InvariantCulture);
      if (stringValue == CultureInfo.InvariantCulture.NumberFormat.PositiveInfinitySymbol)
        stringValue = JsonValueUtils.ODataJsonPositiveInfinitySymbol;
      else if (stringValue == CultureInfo.InvariantCulture.NumberFormat.NegativeInfinitySymbol)
        stringValue = JsonValueUtils.ODataJsonNegativeInfinitySymbol;
      return Convert.ChangeType((object) stringValue, targetType, (IFormatProvider) JsonValueUtils.ODataNumberFormatInfo);
    }

    private static object ConvertInt32Value(
      int intValue,
      Type targetType,
      IEdmPrimitiveTypeReference primitiveTypeReference)
    {
      if (targetType == typeof (short))
        return (object) Convert.ToInt16(intValue);
      if (targetType == typeof (byte))
        return (object) Convert.ToByte(intValue);
      if (targetType == typeof (sbyte))
        return (object) Convert.ToSByte(intValue);
      if (targetType == typeof (float))
        return (object) Convert.ToSingle(intValue);
      if (targetType == typeof (double))
        return (object) Convert.ToDouble(intValue);
      if (targetType == typeof (Decimal))
        return (object) Convert.ToDecimal(intValue);
      if (targetType == typeof (long))
        return (object) Convert.ToInt64(intValue);
      if (targetType != typeof (int))
        throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertInt32((object) primitiveTypeReference.FullName()));
      return (object) intValue;
    }
  }
}
