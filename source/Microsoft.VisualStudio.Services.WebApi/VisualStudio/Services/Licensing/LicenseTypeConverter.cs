// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseTypeConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal sealed class LicenseTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof (string) || base.CanConvertTo(context, destinationType);

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      return value is string text ? (object) License.Parse(text) : base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      License license = value as License;
      return license != (License) null && destinationType == typeof (string) ? (object) license.ToString() : base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool IsValid(ITypeDescriptorContext context, object value) => value is string text ? License.TryParse(text, out License _) : base.IsValid(context, value);
  }
}
