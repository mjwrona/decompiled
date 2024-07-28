// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.SequenceStart
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;

namespace YamlDotNet.Core.Events
{
  public class SequenceStart : NodeEvent
  {
    private readonly bool isImplicit;
    private readonly SequenceStyle style;

    public override int NestingIncrease => 1;

    internal override EventType Type => EventType.SequenceStart;

    public bool IsImplicit => this.isImplicit;

    public override bool IsCanonical => !this.isImplicit;

    public SequenceStyle Style => this.style;

    public SequenceStart(
      string anchor,
      string tag,
      bool isImplicit,
      SequenceStyle style,
      Mark start,
      Mark end)
      : base(anchor, tag, start, end)
    {
      this.isImplicit = isImplicit;
      this.style = style;
    }

    public SequenceStart(string anchor, string tag, bool isImplicit, SequenceStyle style)
      : this(anchor, tag, isImplicit, style, Mark.Empty, Mark.Empty)
    {
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Sequence start [anchor = {0}, tag = {1}, isImplicit = {2}, style = {3}]", (object) this.Anchor, (object) this.Tag, (object) this.isImplicit, (object) this.style);

    public override void Accept(IParsingEventVisitor visitor) => visitor.Visit(this);
  }
}
