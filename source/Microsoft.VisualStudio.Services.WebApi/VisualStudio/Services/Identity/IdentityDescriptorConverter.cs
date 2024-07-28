// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityDescriptorConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType.Equals(typeof (string)) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType.Equals(typeof (string)) || base.CanConvertTo(context, destinationType);

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (value is string)
      {
        string[] strArray = (value as string).Split(new string[1]
        {
          ";"
        }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 2)
          return (object) new IdentityDescriptor(strArray[0], strArray[1]);
      }
      return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return destinationType.Equals(typeof (string)) ? (object) (value as IdentityDescriptor)?.ToString() ?? (object) string.Empty : base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
