// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.CommonUtil
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal static class CommonUtil
  {
    private static readonly Type[] UnsupportedTypes = new Type[9]
    {
      typeof (IDynamicMetaObjectProvider),
      typeof (Tuple<>),
      typeof (Tuple<,>),
      typeof (Tuple<,,>),
      typeof (Tuple<,,,>),
      typeof (Tuple<,,,,>),
      typeof (Tuple<,,,,,>),
      typeof (Tuple<,,,,,,>),
      typeof (Tuple<,,,,,,,>)
    };

    internal static bool IsUnsupportedType(Type type)
    {
      if (type.IsGenericType)
        type = type.GetGenericTypeDefinition();
      return ((IEnumerable<Type>) CommonUtil.UnsupportedTypes).Any<Type>((Func<Type, bool>) (t => t.IsAssignableFrom(type)));
    }

    internal static bool IsClientType(Type t) => t.GetInterface(typeof (ITableEntity).FullName, false) != (Type) null;
  }
}
