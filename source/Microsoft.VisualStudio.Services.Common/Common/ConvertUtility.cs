// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ConvertUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Common
{
  public class ConvertUtility
  {
    public static object ChangeType(object value, Type type) => ConvertUtility.ChangeType(value, type, (IFormatProvider) CultureInfo.CurrentCulture);

    public static object ChangeType(object value, Type type, IFormatProvider provider)
    {
      if (!type.IsOfType(typeof (Nullable<>)))
        return Convert.ChangeType(value, type, provider);
      NullableConverter nullableConverter = new NullableConverter(type);
      return nullableConverter.ConvertTo(value, nullableConverter.UnderlyingType);
    }
  }
}
