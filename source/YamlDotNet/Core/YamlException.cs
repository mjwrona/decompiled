// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.YamlException
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  [Serializable]
  public class YamlException : Exception
  {
    public Mark Start { get; private set; }

    public Mark End { get; private set; }

    public YamlException()
    {
    }

    public YamlException(string message)
      : base(message)
    {
    }

    public YamlException(Mark start, Mark end, string message)
      : this(start, end, message, (Exception) null)
    {
    }

    public YamlException(Mark start, Mark end, string message, Exception innerException)
      : base(string.Format("({0}) - ({1}): {2}", (object) start, (object) end, (object) message), innerException)
    {
      this.Start = start;
      this.End = end;
    }

    public YamlException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
