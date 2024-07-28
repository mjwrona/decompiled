// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupIdentifierTypeConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class DedupIdentifierTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      return value is string ? (object) DedupIdentifier.Create(value as string) : base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return destinationType == typeof (string) ? (object) ((DedupIdentifier) value).ValueString : base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
