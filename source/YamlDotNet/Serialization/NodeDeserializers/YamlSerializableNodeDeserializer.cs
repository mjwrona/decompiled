// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeDeserializers.YamlSerializableNodeDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
  public sealed class YamlSerializableNodeDeserializer : INodeDeserializer
  {
    private readonly IObjectFactory objectFactory;

    public YamlSerializableNodeDeserializer(IObjectFactory objectFactory) => this.objectFactory = objectFactory;

    public bool Deserialize(
      IParser parser,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value)
    {
      if (typeof (IYamlSerializable).IsAssignableFrom(expectedType))
      {
        IYamlSerializable yamlSerializable = (IYamlSerializable) this.objectFactory.Create(expectedType);
        yamlSerializable.ReadYaml(parser);
        value = (object) yamlSerializable;
        return true;
      }
      value = (object) null;
      return false;
    }
  }
}
