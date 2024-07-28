// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Serializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
  public sealed class Serializer : ISerializer
  {
    private readonly IValueSerializer valueSerializer;

    public Serializer()
      : this(new SerializerBuilder().BuildValueSerializer())
    {
    }

    private Serializer(IValueSerializer valueSerializer) => this.valueSerializer = valueSerializer != null ? valueSerializer : throw new ArgumentNullException(nameof (valueSerializer));

    public static Serializer FromValueSerializer(IValueSerializer valueSerializer) => new Serializer(valueSerializer);

    public void Serialize(TextWriter writer, object graph) => this.Serialize((IEmitter) new Emitter(writer), graph);

    public string Serialize(object graph)
    {
      using (StringWriter writer = new StringWriter())
      {
        this.Serialize((TextWriter) writer, graph);
        return writer.ToString();
      }
    }

    public void Serialize(TextWriter writer, object graph, Type type) => this.Serialize((IEmitter) new Emitter(writer), graph, type);

    public void Serialize(IEmitter emitter, object graph)
    {
      if (emitter == null)
        throw new ArgumentNullException(nameof (emitter));
      this.EmitDocument(emitter, graph, (Type) null);
    }

    public void Serialize(IEmitter emitter, object graph, Type type)
    {
      if (emitter == null)
        throw new ArgumentNullException(nameof (emitter));
      if ((object) type == null)
        throw new ArgumentNullException(nameof (type));
      this.EmitDocument(emitter, graph, type);
    }

    private void EmitDocument(IEmitter emitter, object graph, Type type)
    {
      emitter.Emit((ParsingEvent) new StreamStart());
      emitter.Emit((ParsingEvent) new DocumentStart());
      this.valueSerializer.SerializeValue(emitter, graph, type);
      emitter.Emit((ParsingEvent) new DocumentEnd(true));
      emitter.Emit((ParsingEvent) new StreamEnd());
    }
  }
}
