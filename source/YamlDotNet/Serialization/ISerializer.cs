// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ISerializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.IO;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
  public interface ISerializer
  {
    void Serialize(TextWriter writer, object graph);

    string Serialize(object graph);

    void Serialize(TextWriter writer, object graph, Type type);

    void Serialize(IEmitter emitter, object graph);

    void Serialize(IEmitter emitter, object graph, Type type);
  }
}
