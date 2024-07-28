// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.Comment
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;

namespace YamlDotNet.Core.Events
{
  public class Comment : ParsingEvent
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
      this.Value = value;
      this.IsInline = isInline;
    }

    internal override EventType Type => EventType.Comment;

    public override void Accept(IParsingEventVisitor visitor) => visitor.Visit(this);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} Comment [{1}]", new object[2]
    {
      this.IsInline ? (object) "Inline" : (object) "Block",
      (object) this.Value
    });
  }
}
