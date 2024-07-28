// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.NodeEvent
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Text.RegularExpressions;

namespace YamlDotNet.Core.Events
{
  public abstract class NodeEvent : ParsingEvent
  {
    internal static readonly Regex anchorValidator = new Regex("^[0-9a-zA-Z_\\-]+$", RegexOptions.Compiled);
    private readonly string anchor;
    private readonly string tag;

    public string Anchor => this.anchor;

    public string Tag => this.tag;

    public abstract bool IsCanonical { get; }

    protected NodeEvent(string anchor, string tag, Mark start, Mark end)
      : base(start, end)
    {
      switch (anchor)
      {
        case null:
          if (tag != null && tag.Length == 0)
            throw new ArgumentException("Tag value must not be empty.", nameof (tag));
          this.anchor = anchor;
          this.tag = tag;
          break;
        case "":
          throw new ArgumentException("Anchor value must not be empty.", nameof (anchor));
        default:
          if (!NodeEvent.anchorValidator.IsMatch(anchor))
            throw new ArgumentException("Anchor value must contain alphanumerical characters only.", nameof (anchor));
          goto case null;
      }
    }

    protected NodeEvent(string anchor, string tag)
      : this(anchor, tag, Mark.Empty, Mark.Empty)
    {
    }
  }
}
