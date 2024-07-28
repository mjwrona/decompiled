// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.DocumentEnd
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;

namespace YamlDotNet.Core.Events
{
  public class DocumentEnd : ParsingEvent
  {
    private readonly bool isImplicit;

    public override int NestingIncrease => -1;

    internal override EventType Type => EventType.DocumentEnd;

    public bool IsImplicit => this.isImplicit;

    public DocumentEnd(bool isImplicit, Mark start, Mark end)
      : base(start, end)
    {
      this.isImplicit = isImplicit;
    }

    public DocumentEnd(bool isImplicit)
      : this(isImplicit, Mark.Empty, Mark.Empty)
    {
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Document end [isImplicit = {0}]", new object[1]
    {
      (object) this.isImplicit
    });

    public override void Accept(IParsingEventVisitor visitor) => visitor.Visit(this);
  }
}
