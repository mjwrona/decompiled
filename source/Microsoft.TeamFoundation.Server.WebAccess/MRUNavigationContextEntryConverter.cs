// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MRUNavigationContextEntryConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class MRUNavigationContextEntryConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof (string) || base.CanConvertTo(context, destinationType);

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return value is MRUNavigationContextEntry ? (object) ConvertUtility.DataContractJson<MRUNavigationContextEntry>((MRUNavigationContextEntry) value) : base.ConvertTo(context, culture, value, destinationType);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      return value is string ? (object) ConvertUtility.FromDataContractJson<MRUNavigationContextEntry>((string) value) : base.ConvertFrom(context, culture, value);
    }
  }
}
