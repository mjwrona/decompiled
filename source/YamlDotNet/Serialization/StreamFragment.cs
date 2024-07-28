// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.StreamFragment
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
  public sealed class StreamFragment : IYamlConvertible
  {
    private readonly List<ParsingEvent> events = new List<ParsingEvent>();

    public IList<ParsingEvent> Events => (IList<ParsingEvent>) this.events;

    void IYamlConvertible.Read(
      IParser parser,
      Type expectedType,
      ObjectDeserializer nestedObjectDeserializer)
    {
      this.events.Clear();
      int num = 0;
      while (parser.MoveNext())
      {
        this.events.Add(parser.Current);
        num += parser.Current.NestingIncrease;
        if (num <= 0)
          return;
      }
      throw new InvalidOperationException("The parser has reached the end before deserialization completed.");
    }

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
      foreach (ParsingEvent @event in this.events)
        emitter.Emit(@event);
    }
  }
}
