// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Deserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization
{
  public sealed class Deserializer : IDeserializer
  {
    private readonly IValueDeserializer valueDeserializer;

    public Deserializer()
      : this(new DeserializerBuilder().BuildValueDeserializer())
    {
    }

    private Deserializer(IValueDeserializer valueDeserializer) => this.valueDeserializer = valueDeserializer != null ? valueDeserializer : throw new ArgumentNullException(nameof (valueDeserializer));

    public static Deserializer FromValueDeserializer(IValueDeserializer valueDeserializer) => new Deserializer(valueDeserializer);

    public T Deserialize<T>(string input)
    {
      using (StringReader input1 = new StringReader(input))
        return (T) this.Deserialize((TextReader) input1, typeof (T));
    }

    public T Deserialize<T>(TextReader input) => (T) this.Deserialize(input, typeof (T));

    public object Deserialize(TextReader input) => this.Deserialize(input, typeof (object));

    public object Deserialize(string input, Type type)
    {
      using (StringReader input1 = new StringReader(input))
        return this.Deserialize((TextReader) input1, type);
    }

    public object Deserialize(TextReader input, Type type) => this.Deserialize((IParser) new Parser(input), type);

    public T Deserialize<T>(IParser parser) => (T) this.Deserialize(parser, typeof (T));

    public object Deserialize(IParser parser) => this.Deserialize(parser, typeof (object));

    public object Deserialize(IParser parser, Type type)
    {
      if (parser == null)
        throw new ArgumentNullException("reader");
      if ((object) type == null)
        throw new ArgumentNullException(nameof (type));
      bool flag1 = parser.Allow<StreamStart>() != null;
      bool flag2 = parser.Allow<DocumentStart>() != null;
      object obj = (object) null;
      if (!parser.Accept<DocumentEnd>() && !parser.Accept<StreamEnd>())
      {
        using (SerializerState state = new SerializerState())
        {
          obj = this.valueDeserializer.DeserializeValue(parser, type, state, this.valueDeserializer);
          state.OnDeserialization();
        }
      }
      if (flag2)
        parser.Expect<DocumentEnd>();
      if (flag1)
        parser.Expect<StreamEnd>();
      return obj;
    }
  }
}
