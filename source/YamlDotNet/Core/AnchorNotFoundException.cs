// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.AnchorNotFoundException
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  [Serializable]
  public class AnchorNotFoundException : YamlException
  {
    public AnchorNotFoundException()
    {
    }

    public AnchorNotFoundException(string message)
      : base(message)
    {
    }

    public AnchorNotFoundException(Mark start, Mark end, string message)
      : base(start, end, message)
    {
    }

    public AnchorNotFoundException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
