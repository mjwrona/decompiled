// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.YamlNode
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
  [Serializable]
  public abstract class YamlNode
  {
    private const int MaximumRecursionLevel = 1000;
    internal const string MaximumRecursionLevelReachedToStringValue = "WARNING! INFINITE RECURSION!";

    public string Anchor { get; set; }

    public string Tag { get; set; }

    public Mark Start { get; private set; }

    public Mark End { get; private set; }

    internal void Load(NodeEvent yamlEvent, DocumentLoadingState state)
    {
      this.Tag = yamlEvent.Tag;
      if (yamlEvent.Anchor != null)
      {
        this.Anchor = yamlEvent.Anchor;
        state.AddAnchor(this);
      }
      this.Start = yamlEvent.Start;
      this.End = yamlEvent.End;
    }

    internal static YamlNode ParseNode(IParser parser, DocumentLoadingState state)
    {
      if (parser.Accept<Scalar>())
        return (YamlNode) new YamlScalarNode(parser, state);
      if (parser.Accept<SequenceStart>())
        return (YamlNode) new YamlSequenceNode(parser, state);
      if (parser.Accept<MappingStart>())
        return (YamlNode) new YamlMappingNode(parser, state);
      AnchorAlias anchorAlias = parser.Accept<AnchorAlias>() ? parser.Expect<AnchorAlias>() : throw new ArgumentException("The current event is of an unsupported type.", "events");
      return state.GetNode(anchorAlias.Value, false, anchorAlias.Start, anchorAlias.End) ?? (YamlNode) new YamlAliasNode(anchorAlias.Value);
    }

    internal abstract void ResolveAliases(DocumentLoadingState state);

    internal void Save(IEmitter emitter, EmitterState state)
    {
      if (!string.IsNullOrEmpty(this.Anchor) && !state.EmittedAnchors.Add(this.Anchor))
        emitter.Emit((ParsingEvent) new AnchorAlias(this.Anchor));
      else
        this.Emit(emitter, state);
    }

    internal abstract void Emit(IEmitter emitter, EmitterState state);

    public abstract void Accept(IYamlVisitor visitor);

    protected bool Equals(YamlNode other) => YamlNode.SafeEquals((object) this.Tag, (object) other.Tag);

    protected static bool SafeEquals(object first, object second)
    {
      if (first != null)
        return first.Equals(second);
      return second == null || second.Equals(first);
    }

    public override int GetHashCode() => YamlNode.GetHashCode((object) this.Tag);

    protected static int GetHashCode(object value) => value != null ? value.GetHashCode() : 0;

    protected static int CombineHashCodes(int h1, int h2) => (h1 << 5) + h1 ^ h2;

    public override string ToString() => this.ToString(new RecursionLevel(1000));

    internal abstract string ToString(RecursionLevel level);

    public IEnumerable<YamlNode> AllNodes => this.SafeAllNodes(new RecursionLevel(1000));

    internal abstract IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level);

    public abstract YamlNodeType NodeType { get; }

    public static implicit operator YamlNode(string value) => (YamlNode) new YamlScalarNode(value);

    public static implicit operator YamlNode(string[] sequence) => (YamlNode) new YamlSequenceNode(((IEnumerable<string>) sequence).Select<string, YamlNode>((Func<string, YamlNode>) (i => (YamlNode) i)));

    public static explicit operator string(YamlNode scalar) => ((YamlScalarNode) scalar).Value;

    public YamlNode this[int index] => ((YamlSequenceNode) this).Children[index];

    public YamlNode this[YamlNode key] => ((YamlMappingNode) this).Children[key];
  }
}
