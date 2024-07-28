// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Tokens.Comment
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core.Tokens
{
  [Serializable]
  public class Comment : Token
  {
    public string Value { get; private set; }

    public bool IsInline { get; private set; }

    public Comment(string value, bool isInline)
      : this(value, isInline, Mark.Empty, Mark.Empty)
    {
    }

    public Comment(string value, bool isInline, Mark start, Mark end)
      : base(start, end)
    {
      this.IsInline = isInline;
      this.Value = value;
    }
  }
}
