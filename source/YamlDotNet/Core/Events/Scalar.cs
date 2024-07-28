// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.Scalar
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;

namespace YamlDotNet.Core.Events
{
  public class Scalar : NodeEvent
  {
    private readonly string value;
    private readonly ScalarStyle style;
    private readonly bool isPlainImplicit;
    private readonly bool isQuotedImplicit;

    internal override EventType Type => EventType.Scalar;

    public string Value => this.value;

    public ScalarStyle Style => this.style;

    public bool IsPlainImplicit => this.isPlainImplicit;

    public bool IsQuotedImplicit => this.isQuotedImplicit;

    public override bool IsCanonical => !this.isPlainImplicit && !this.isQuotedImplicit;

    public Scalar(
      string anchor,
      string tag,
      string value,
      ScalarStyle style,
      bool isPlainImplicit,
      bool isQuotedImplicit,
      Mark start,
      Mark end)
      : base(anchor, tag, start, end)
    {
      this.value = value;
      this.style = style;
      this.isPlainImplicit = isPlainImplicit;
      this.isQuotedImplicit = isQuotedImplicit;
    }

    public Scalar(
      string anchor,
      string tag,
      string value,
      ScalarStyle style,
      bool isPlainImplicit,
      bool isQuotedImplicit)
      : this(anchor, tag, value, style, isPlainImplicit, isQuotedImplicit, Mark.Empty, Mark.Empty)
    {
    }

    public Scalar(string value)
      : this((string) null, (string) null, value, ScalarStyle.Any, true, true, Mark.Empty, Mark.Empty)
    {
    }

    public Scalar(string tag, string value)
      : this((string) null, tag, value, ScalarStyle.Any, true, true, Mark.Empty, Mark.Empty)
    {
    }

    public Scalar(string anchor, string tag, string value)
      : this(anchor, tag, value, ScalarStyle.Any, true, true, Mark.Empty, Mark.Empty)
    {
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Scalar [anchor = {0}, tag = {1}, value = {2}, style = {3}, isPlainImplicit = {4}, isQuotedImplicit = {5}]", (object) this.Anchor, (object) this.Tag, (object) this.value, (object) this.style, (object) this.isPlainImplicit, (object) this.isQuotedImplicit);

    public override void Accept(IParsingEventVisitor visitor) => visitor.Visit(this);
  }
}
