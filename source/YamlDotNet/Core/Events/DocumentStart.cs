// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.DocumentStart
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core.Events
{
  public class DocumentStart : ParsingEvent
  {
    private readonly TagDirectiveCollection tags;
    private readonly VersionDirective version;
    private readonly bool isImplicit;

    public override int NestingIncrease => 1;

    internal override EventType Type => EventType.DocumentStart;

    public TagDirectiveCollection Tags => this.tags;

    public VersionDirective Version => this.version;

    public bool IsImplicit => this.isImplicit;

    public DocumentStart(
      VersionDirective version,
      TagDirectiveCollection tags,
      bool isImplicit,
      Mark start,
      Mark end)
      : base(start, end)
    {
      this.version = version;
      this.tags = tags;
      this.isImplicit = isImplicit;
    }

    public DocumentStart(VersionDirective version, TagDirectiveCollection tags, bool isImplicit)
      : this(version, tags, isImplicit, Mark.Empty, Mark.Empty)
    {
    }

    public DocumentStart(Mark start, Mark end)
      : this((VersionDirective) null, (TagDirectiveCollection) null, true, start, end)
    {
    }

    public DocumentStart()
      : this((VersionDirective) null, (TagDirectiveCollection) null, true, Mark.Empty, Mark.Empty)
    {
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Document start [isImplicit = {0}]", new object[1]
    {
      (object) this.isImplicit
    });

    public override void Accept(IParsingEventVisitor visitor) => visitor.Visit(this);
  }
}
