// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Tokens.Scalar
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core.Tokens
{
  [Serializable]
  public class Scalar : Token
  {
    private readonly string value;
    private readonly ScalarStyle style;

    public string Value => this.value;

    public ScalarStyle Style => this.style;

    public Scalar(string value)
      : this(value, ScalarStyle.Any)
    {
    }

    public Scalar(string value, ScalarStyle style)
      : this(value, style, Mark.Empty, Mark.Empty)
    {
    }

    public Scalar(string value, ScalarStyle style, Mark start, Mark end)
      : base(start, end)
    {
      this.value = value;
      this.style = style;
    }
  }
}
