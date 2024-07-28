// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.ExprNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class ExprNode : AstNode
  {
    private bool usesBinary;

    public ExprNode(
      TermNode termNode,
      ReadOnlyCollection<TermWithOperatorNode> termsWithOperators,
      ReadOnlyCollection<ImportantCommentNode> importantComments)
    {
      this.TermNode = termNode;
      this.TermsWithOperators = termsWithOperators ?? new List<TermWithOperatorNode>().AsReadOnly();
      this.ImportantComments = importantComments ?? new List<ImportantCommentNode>().AsReadOnly();
      this.UsesBinary = false;
    }

    public bool UsesBinary
    {
      get => this.usesBinary;
      set
      {
        this.usesBinary = value;
        foreach (TermWithOperatorNode termsWithOperator in this.TermsWithOperators)
          termsWithOperator.UsesBinary = value;
      }
    }

    public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }

    public TermNode TermNode { get; private set; }

    public ReadOnlyCollection<TermWithOperatorNode> TermsWithOperators { get; private set; }

    public bool Equals(ExprNode exprNode)
    {
      if (!exprNode.TermNode.Equals(this.TermNode) || exprNode.UsesBinary != this.UsesBinary || exprNode.TermsWithOperators.Count != this.TermsWithOperators.Count)
        return false;
      for (int index = 0; index < this.TermsWithOperators.Count; ++index)
      {
        if (!exprNode.TermsWithOperators[index].Equals(this.TermsWithOperators[index]))
          return false;
      }
      return true;
    }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitExprNode(this);
  }
}
