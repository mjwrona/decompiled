// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.KnownFlagsEnumTypeConverter
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  public class KnownFlagsEnumTypeConverter : EnumConverter
  {
    public KnownFlagsEnumTypeConverter(Type type)
      : base(type)
    {
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (!(value is string stringValue))
        return base.ConvertFrom(context, culture, value);
      try
      {
        return FlagsEnum.ParseKnownFlags(this.EnumType, stringValue);
      }
      catch (Exception ex)
      {
        throw new FormatException(PipelinesWebApiResources.InvalidFlagsEnumValue((object) stringValue, (object) this.EnumType), ex);
      }
    }
  }
}
