// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.Animation.KeyFramesNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.Animation
{
  public sealed class KeyFramesNode : StyleSheetRuleNode
  {
    public KeyFramesNode(
      string keyFramesSymbol,
      string identValue,
      string stringValue,
      ReadOnlyCollection<KeyFramesBlockNode> keyFramesBlockNodes)
    {
      this.KeyFramesSymbol = keyFramesSymbol;
      this.IdentValue = identValue;
      this.StringValue = stringValue;
      this.KeyFramesBlockNodes = keyFramesBlockNodes ?? new List<KeyFramesBlockNode>(0).AsReadOnly();
    }

    public string KeyFramesSymbol { get; private set; }

    public string IdentValue { get; private set; }

    public string StringValue { get; private set; }

    public ReadOnlyCollection<KeyFramesBlockNode> KeyFramesBlockNodes { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitKeyFramesNode(this);
  }
}
