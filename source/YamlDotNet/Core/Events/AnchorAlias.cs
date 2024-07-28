// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.AnchorAlias
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;

namespace YamlDotNet.Core.Events
{
  public class AnchorAlias : ParsingEvent
  {
    private readonly string value;

    internal override EventType Type => EventType.Alias;

    public string Value => this.value;

    public AnchorAlias(string value, Mark start, Mark end)
      : base(start, end)
    {
      if (string.IsNullOrEmpty(value))
        throw new YamlException(start, end, "Anchor value must not be empty.");
      this.value = NodeEvent.anchorValidator.IsMatch(value) ? value : throw new YamlException(start, end, "Anchor value must contain alphanumerical characters only.");
    }

    public AnchorAlias(string value)
      : this(value, Mark.Empty, Mark.Empty)
    {
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Alias [value = {0}]", new object[1]
    {
      (object) this.value
    });

    public override void Accept(IParsingEventVisitor visitor) => visitor.Visit(this);
  }
}
