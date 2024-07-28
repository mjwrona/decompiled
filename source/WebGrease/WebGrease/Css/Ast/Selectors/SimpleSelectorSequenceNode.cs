// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.Selectors.SimpleSelectorSequenceNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.Selectors
{
  public sealed class SimpleSelectorSequenceNode : AstNode
  {
    public SimpleSelectorSequenceNode(
      TypeSelectorNode typeSelectorNode,
      UniversalSelectorNode universalSelectorNode,
      string separator,
      ReadOnlyCollection<HashClassAtNameAttribPseudoNegationNode> simpleSelectorValues)
    {
      this.TypeSelectorNode = typeSelectorNode;
      this.UniversalSelectorNode = universalSelectorNode;
      this.Separator = separator ?? string.Empty;
      this.HashClassAttribPseudoNegationNodes = simpleSelectorValues ?? new List<HashClassAtNameAttribPseudoNegationNode>(0).AsReadOnly();
    }

    public TypeSelectorNode TypeSelectorNode { get; private set; }

    public UniversalSelectorNode UniversalSelectorNode { get; private set; }

    public string Separator { get; private set; }

    public ReadOnlyCollection<HashClassAtNameAttribPseudoNegationNode> HashClassAttribPseudoNegationNodes { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitSimpleSelectorSequenceNode(this);
  }
}
