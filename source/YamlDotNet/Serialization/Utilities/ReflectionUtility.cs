// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Utilities.ReflectionUtility
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.Utilities
{
  internal static class ReflectionUtility
  {
    public static Type GetImplementedGenericInterface(Type type, Type genericInterfaceType)
    {
      foreach (Type implementedInterface in ReflectionUtility.GetImplementedInterfaces(type))
      {
        if (implementedInterface.IsGenericType() && (object) implementedInterface.GetGenericTypeDefinition() == (object) genericInterfaceType)
          return implementedInterface;
      }
      return (Type) null;
    }

    public static IEnumerable<Type> GetImplementedInterfaces(Type type)
    {
      if (type.IsInterface())
        yield return type;
      Type[] typeArray = type.GetInterfaces();
      for (int index = 0; index < typeArray.Length; ++index)
        yield return typeArray[index];
      typeArray = (Type[]) null;
    }
  }
}
