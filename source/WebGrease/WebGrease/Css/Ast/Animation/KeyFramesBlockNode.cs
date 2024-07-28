// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.Animation.KeyFramesBlockNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.Animation
{
  public sealed class KeyFramesBlockNode : AstNode
  {
    public KeyFramesBlockNode(
      ReadOnlyCollection<string> keyFramesSelectors,
      ReadOnlyCollection<DeclarationNode> declarationNodes)
    {
      this.KeyFramesSelectors = keyFramesSelectors;
      this.DeclarationNodes = declarationNodes ?? new List<DeclarationNode>(0).AsReadOnly();
    }

    public ReadOnlyCollection<string> KeyFramesSelectors { get; private set; }

    public ReadOnlyCollection<DeclarationNode> DeclarationNodes { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitKeyFramesBlockNode(this);
  }
}
