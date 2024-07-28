// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.DefaultPrimitiveValueConverter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Globalization;

namespace Microsoft.OData.Edm
{
  internal class DefaultPrimitiveValueConverter : IPrimitiveValueConverter
  {
    internal static readonly IPrimitiveValueConverter Instance = (IPrimitiveValueConverter) new DefaultPrimitiveValueConverter();

    private DefaultPrimitiveValueConverter()
    {
    }

    public object ConvertToUnderlyingType(object value)
    {
      switch (value)
      {
        case ushort _:
          return (object) Convert.ToInt32(value, (IFormatProvider) CultureInfo.InvariantCulture);
        case uint _:
          return (object) Convert.ToInt64(value, (IFormatProvider) CultureInfo.InvariantCulture);
        case ulong _:
          return (object) Convert.ToDecimal(value, (IFormatProvider) CultureInfo.InvariantCulture);
        default:
          return value;
      }
    }

    public object ConvertFromUnderlyingType(object value)
    {
      switch (value)
      {
        case int _:
          return (object) Convert.ToUInt16(value, (IFormatProvider) CultureInfo.InvariantCulture);
        case long _:
          return (object) Convert.ToUInt32(value, (IFormatProvider) CultureInfo.InvariantCulture);
        case Decimal _:
          return (object) Convert.ToUInt64(value, (IFormatProvider) CultureInfo.InvariantCulture);
        default:
          return value;
      }
    }
  }
}
