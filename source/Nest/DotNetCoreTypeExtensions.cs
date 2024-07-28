// Decompiled with JetBrains decompiler
// Type: Nest.DotNetCoreTypeExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal static class DotNetCoreTypeExtensions
  {
    internal static bool TryGetGenericDictionaryArguments(
      this Type type,
      out Type[] genericArguments)
    {
      Type type1 = ((IEnumerable<Type>) type.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (t =>
      {
        if (!t.IsGenericType)
          return false;
        return t.GetGenericTypeDefinition() == typeof (IDictionary<,>) || t.GetGenericTypeDefinition() == typeof (IReadOnlyDictionary<,>);
      }));
      if (type1 == (Type) null)
      {
        genericArguments = new Type[0];
        return false;
      }
      genericArguments = type1.GetGenericArguments();
      return true;
    }
  }
}
