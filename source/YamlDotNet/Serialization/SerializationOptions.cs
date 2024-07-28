// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.SerializationOptions
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Serialization
{
  [Flags]
  public enum SerializationOptions
  {
    None = 0,
    Roundtrip = 1,
    DisableAliases = 2,
    EmitDefaults = 4,
    JsonCompatible = 8,
    DefaultToStaticType = 16, // 0x00000010
  }
}
