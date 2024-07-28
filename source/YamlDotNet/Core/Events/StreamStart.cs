// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.StreamStart
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

namespace YamlDotNet.Core.Events
{
  public class StreamStart : ParsingEvent
  {
    public override int NestingIncrease => 1;

    internal override EventType Type => EventType.StreamStart;

    public StreamStart()
      : this(Mark.Empty, Mark.Empty)
    {
    }

    public StreamStart(Mark start, Mark end)
      : base(start, end)
    {
    }

    public override string ToString() => "Stream start";

    public override void Accept(IParsingEventVisitor visitor) => visitor.Visit(this);
  }
}
