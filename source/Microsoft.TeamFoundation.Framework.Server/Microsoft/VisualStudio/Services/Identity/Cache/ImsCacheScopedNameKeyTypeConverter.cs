// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheScopedNameKeyTypeConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal sealed class ImsCacheScopedNameKeyTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof (string) || base.CanConvertTo(context, destinationType);

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      ImsCacheScopedNameKey result;
      return value is string input && ImsCacheScopedNameKey.TryParse(input, out result) ? (object) result : base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return value is ImsCacheScopedNameKey cacheScopedNameKey && destinationType == typeof (string) ? (object) cacheScopedNameKey.Serialize() : base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool IsValid(ITypeDescriptorContext context, object value) => value is string input ? ImsCacheScopedNameKey.TryParse(input, out ImsCacheScopedNameKey _) : base.IsValid(context, value);
  }
}
