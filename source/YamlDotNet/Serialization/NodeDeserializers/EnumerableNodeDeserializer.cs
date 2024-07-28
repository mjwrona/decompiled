// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeDeserializers.EnumerableNodeDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
  public sealed class EnumerableNodeDeserializer : INodeDeserializer
  {
    bool INodeDeserializer.Deserialize(
      IParser parser,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value)
    {
      Type type1;
      if ((object) expectedType == (object) typeof (IEnumerable))
      {
        type1 = typeof (object);
      }
      else
      {
        Type genericInterface = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof (IEnumerable<>));
        if ((object) genericInterface != (object) expectedType)
        {
          value = (object) null;
          return false;
        }
        type1 = genericInterface.GetGenericArguments()[0];
      }
      Type type2 = typeof (List<>).MakeGenericType(type1);
      value = nestedObjectDeserializer(parser, type2);
      return true;
    }
  }
}
