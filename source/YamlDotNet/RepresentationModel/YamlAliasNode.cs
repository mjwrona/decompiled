// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.YamlAliasNode
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel
{
  [Serializable]
  internal class YamlAliasNode : YamlNode
  {
    internal YamlAliasNode(string anchor) => this.Anchor = anchor;

    internal override void ResolveAliases(DocumentLoadingState state) => throw new NotSupportedException("Resolving an alias on an alias node does not make sense");

    internal override void Emit(IEmitter emitter, EmitterState state) => throw new NotSupportedException("A YamlAliasNode is an implementation detail and should never be saved.");

    public override void Accept(IYamlVisitor visitor) => throw new NotSupportedException("A YamlAliasNode is an implementation detail and should never be visited.");

    public override bool Equals(object obj) => obj is YamlAliasNode other && this.Equals((YamlNode) other) && YamlNode.SafeEquals((object) this.Anchor, (object) other.Anchor);

    public override int GetHashCode() => base.GetHashCode();

    internal override string ToString(RecursionLevel level) => "*" + this.Anchor;

    internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      YamlAliasNode yamlAliasNode = this;
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
      this.\u003C\u003E2__current = (YamlNode) yamlAliasNode;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    public override YamlNodeType NodeType => YamlNodeType.Alias;
  }
}
