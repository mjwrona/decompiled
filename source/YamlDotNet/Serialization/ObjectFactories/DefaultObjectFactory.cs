// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectFactories.DefaultObjectFactory
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.ObjectFactories
{
  public sealed class DefaultObjectFactory : IObjectFactory
  {
    private static readonly Dictionary<Type, Type> defaultInterfaceImplementations = new Dictionary<Type, Type>()
    {
      {
        typeof (IEnumerable<>),
        typeof (List<>)
      },
      {
        typeof (ICollection<>),
        typeof (List<>)
      },
      {
        typeof (IList<>),
        typeof (List<>)
      },
      {
        typeof (IDictionary<,>),
        typeof (Dictionary<,>)
      }
    };

    public object Create(Type type)
    {
      Type type1;
      if (type.IsInterface() && DefaultObjectFactory.defaultInterfaceImplementations.TryGetValue(type.GetGenericTypeDefinition(), out type1))
        type = type1.MakeGenericType(type.GetGenericArguments());
      try
      {
        return Activator.CreateInstance(type);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format("Failed to create an instance of type '{0}'.", (object) type), ex);
      }
    }
  }
}
