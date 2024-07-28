// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeDeserializers.TypeConverterNodeDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
  public sealed class TypeConverterNodeDeserializer : INodeDeserializer
  {
    private readonly IEnumerable<IYamlTypeConverter> converters;

    public TypeConverterNodeDeserializer(IEnumerable<IYamlTypeConverter> converters) => this.converters = converters != null ? converters : throw new ArgumentNullException(nameof (converters));

    bool INodeDeserializer.Deserialize(
      IParser parser,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value)
    {
      IYamlTypeConverter yamlTypeConverter = this.converters.FirstOrDefault<IYamlTypeConverter>((Func<IYamlTypeConverter, bool>) (c => c.Accepts(expectedType)));
      if (yamlTypeConverter == null)
      {
        value = (object) null;
        return false;
      }
      value = yamlTypeConverter.ReadYaml(parser, expectedType);
      return true;
    }
  }
}
