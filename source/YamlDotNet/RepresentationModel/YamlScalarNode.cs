// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.YamlScalarNode
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
  [DebuggerDisplay("{Value}")]
  [Serializable]
  public sealed class YamlScalarNode : YamlNode, IYamlConvertible
  {
    public string Value { get; set; }

    public ScalarStyle Style { get; set; }

    internal YamlScalarNode(IParser parser, DocumentLoadingState state) => this.Load(parser, state);

    private void Load(IParser parser, DocumentLoadingState state)
    {
      Scalar yamlEvent = parser.Expect<Scalar>();
      this.Load((NodeEvent) yamlEvent, state);
      this.Value = yamlEvent.Value;
      this.Style = yamlEvent.Style;
    }

    public YamlScalarNode()
    {
    }

    public YamlScalarNode(string value) => this.Value = value;

    internal override void ResolveAliases(DocumentLoadingState state) => throw new NotSupportedException("Resolving an alias on a scalar node does not make sense");

    internal override void Emit(IEmitter emitter, EmitterState state) => emitter.Emit((ParsingEvent) new Scalar(this.Anchor, this.Tag, this.Value, this.Style, this.Tag == null, false));

    public override void Accept(IYamlVisitor visitor) => visitor.Visit(this);

    public override bool Equals(object obj) => obj is YamlScalarNode other && this.Equals((YamlNode) other) && YamlNode.SafeEquals((object) this.Value, (object) other.Value);

    public override int GetHashCode() => YamlNode.CombineHashCodes(base.GetHashCode(), YamlNode.GetHashCode((object) this.Value));

    public static explicit operator string(YamlScalarNode value) => value.Value;

    internal override string ToString(RecursionLevel level) => this.Value;

    internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      YamlScalarNode yamlScalarNode = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (YamlNode) yamlScalarNode;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    public override YamlNodeType NodeType => YamlNodeType.Scalar;

    void IYamlConvertible.Read(
      IParser parser,
      Type expectedType,
      ObjectDeserializer nestedObjectDeserializer)
    {
      this.Load(parser, new DocumentLoadingState());
    }

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => this.Emit(emitter, new EmitterState());
  }
}
