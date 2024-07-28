// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Converters.SystemTypeConverter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
  public class SystemTypeConverter : IYamlTypeConverter
  {
    public bool Accepts(Type type) => typeof (Type).IsAssignableFrom(type);

    public object ReadYaml(IParser parser, Type type)
    {
      string typeName = ((Scalar) parser.Current).Value;
      parser.MoveNext();
      return (object) Type.GetType(typeName, true);
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
      Type type1 = (Type) value;
      emitter.Emit((ParsingEvent) new Scalar((string) null, (string) null, type1.AssemblyQualifiedName, ScalarStyle.Any, true, false));
    }
  }
}
