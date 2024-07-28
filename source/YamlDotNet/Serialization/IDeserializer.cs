// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.IDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.IO;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
  public interface IDeserializer
  {
    T Deserialize<T>(string input);

    T Deserialize<T>(TextReader input);

    object Deserialize(TextReader input);

    object Deserialize(string input, Type type);

    object Deserialize(TextReader input, Type type);

    T Deserialize<T>(IParser parser);

    object Deserialize(IParser parser);

    object Deserialize(IParser parser, Type type);
  }
}
