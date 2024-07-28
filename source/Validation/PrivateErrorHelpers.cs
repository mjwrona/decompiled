// Decompiled with JetBrains decompiler
// Type: Validation.PrivateErrorHelpers
// Assembly: Validation, Version=2.2.0.0, Culture=neutral, PublicKeyToken=2fc06f0d701809a7
// MVID: B008DAAB-8462-4DA1-958C-4C90CA316797
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Validation.dll

using System;
using System.Globalization;
using System.Reflection;

namespace Validation
{
  internal static class PrivateErrorHelpers
  {
    internal static Type TrimGenericWrapper(Type type, Type wrapper)
    {
      Type[] genericArguments;
      return type.GetTypeInfo().IsGenericType && (object) type.GetGenericTypeDefinition() == (object) wrapper && (genericArguments = TypeInfoExtensions.GetGenericArguments(type.GetTypeInfo())).Length == 1 ? genericArguments[0] : type;
    }

    internal static string Format(string format, params object[] arguments) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, arguments);
  }
}
