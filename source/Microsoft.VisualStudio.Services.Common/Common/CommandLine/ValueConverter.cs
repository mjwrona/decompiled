// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.ValueConverter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [Serializable]
  public abstract class ValueConverter : IValueConvertible
  {
    public static Type ValueConverterAssignableType = typeof (IValueConvertible);
    private static ValueConverter defaultValueConverter = (ValueConverter) new NoValueConverter();
    private static Collection<ValueConverter> defaultConverters = new Collection<ValueConverter>()
    {
      ValueConverter.defaultValueConverter,
      (ValueConverter) new BooleanConverter(),
      (ValueConverter) new ByteConverter(),
      (ValueConverter) new DateTimeConverter(),
      (ValueConverter) new DateTimeOffsetConverter(),
      (ValueConverter) new DecimalConverter(),
      (ValueConverter) new DoubleConverter(),
      (ValueConverter) new GuidConverter(),
      (ValueConverter) new Int16Converter(),
      (ValueConverter) new Int32Converter(),
      (ValueConverter) new Int64Converter(),
      (ValueConverter) new NullableBooleanConverter(),
      (ValueConverter) new NullableByteConverter(),
      (ValueConverter) new NullableDateTimeConverter(),
      (ValueConverter) new NullableDateTimeOffsetConverter(),
      (ValueConverter) new NullableInt16Converter(),
      (ValueConverter) new NullableInt32Converter(),
      (ValueConverter) new NullableInt64Converter(),
      (ValueConverter) new NullableTimeSpanConverter(),
      (ValueConverter) new NullableUInt16Converter(),
      (ValueConverter) new NullableUInt32Converter(),
      (ValueConverter) new NullableUInt64Converter(),
      (ValueConverter) new SByteConverter(),
      (ValueConverter) new SingleConverter(),
      (ValueConverter) new TimeSpanConverter(),
      (ValueConverter) new UInt16Converter(),
      (ValueConverter) new UInt32Converter(),
      (ValueConverter) new UInt64Converter()
    };

    protected abstract Type ResultType { get; }

    public static IValueConvertible None => (IValueConvertible) ValueConverter.defaultValueConverter;

    public object Convert(string value)
    {
      try
      {
        return this.ConvertValue(value);
      }
      catch (OptionValueConversionException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new OptionValueConversionException(value, this.ResultType, ex);
      }
    }

    public static IValueConvertible GetDefaultConverter(Type memberType)
    {
      if (memberType == (Type) null)
        throw new ArgumentNullException(nameof (memberType));
      IValueConvertible defaultConverter1 = (IValueConvertible) null;
      foreach (ValueConverter defaultConverter2 in ValueConverter.defaultConverters)
      {
        if (defaultConverter2.ResultType == memberType)
        {
          defaultConverter1 = (IValueConvertible) defaultConverter2;
          break;
        }
      }
      return defaultConverter1;
    }

    public static void ValidateConverterType(Type converterType)
    {
      converterType.GetInterfaces();
      if (!((IEnumerable<Type>) converterType.GetInterfaces()).Any<Type>((Func<Type, bool>) (i => i == ValueConverter.ValueConverterAssignableType)))
        throw new OptionValidationException(CommonResources.ErrorInvalidValueConverterDataType((object) converterType.GetType().FullName, (object) ValueConverter.ValueConverterAssignableType.FullName));
    }

    protected abstract object ConvertValue(string value);
  }
}
