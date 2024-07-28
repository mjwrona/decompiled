// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.UnknownEnum
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  public static class UnknownEnum
  {
    private const string UnknownName = "Unknown";

    public static T Parse<T>(string stringValue) => (T) UnknownEnum.Parse(typeof (T), stringValue);

    public static object Parse(Type enumType, string stringValue)
    {
      Type underlyingType = Nullable.GetUnderlyingType(enumType);
      enumType = underlyingType != (Type) null ? underlyingType : enumType;
      string[] names = Enum.GetNames(enumType);
      if (!string.IsNullOrEmpty(stringValue))
      {
        string str = ((IEnumerable<string>) names).FirstOrDefault<string>((Func<string, bool>) (name => string.Equals(name, stringValue, StringComparison.OrdinalIgnoreCase)));
        if (str != null)
          return Enum.Parse(enumType, str);
        foreach (FieldInfo field in enumType.GetFields())
        {
          if (((IEnumerable<object>) field.GetCustomAttributes(typeof (EnumMemberAttribute), false)).FirstOrDefault<object>() is EnumMemberAttribute enumMemberAttribute && string.Equals(enumMemberAttribute.Value, stringValue, StringComparison.OrdinalIgnoreCase))
            return field.GetValue((object) null);
        }
      }
      return Enum.Parse(enumType, "Unknown");
    }
  }
}
