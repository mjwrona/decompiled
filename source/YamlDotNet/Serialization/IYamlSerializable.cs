// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.IYamlSerializable
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
  [Obsolete("Please use IYamlConvertible instead")]
  public interface IYamlSerializable
  {
    void ReadYaml(IParser parser);

    void WriteYaml(IEmitter emitter);
  }
}
