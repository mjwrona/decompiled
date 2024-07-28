// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Converters.GuidConverter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
  public class GuidConverter : IYamlTypeConverter
  {
    private readonly bool jsonCompatible;

    public GuidConverter(bool jsonCompatible) => this.jsonCompatible = jsonCompatible;

    public bool Accepts(Type type) => (object) type == (object) typeof (Guid);

    public object ReadYaml(IParser parser, Type type)
    {
      string g = ((Scalar) parser.Current).Value;
      parser.MoveNext();
      return (object) new Guid(g);
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
      Guid guid = (Guid) value;
      emitter.Emit((ParsingEvent) new Scalar((string) null, (string) null, guid.ToString("D"), this.jsonCompatible ? ScalarStyle.DoubleQuoted : ScalarStyle.Any, true, false));
    }
  }
}
