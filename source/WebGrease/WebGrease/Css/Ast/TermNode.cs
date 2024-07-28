// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.TermNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class TermNode : AstNode
  {
    public TermNode(
      string unaryOperator,
      string numberBasedValue,
      string stringBasedValue,
      string hexColor,
      FunctionNode functionNode,
      ReadOnlyCollection<ImportantCommentNode> importantComments,
      string replacementTokenBasedValue = null)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (!string.IsNullOrWhiteSpace(numberBasedValue))
        flag1 = true;
      if (!string.IsNullOrWhiteSpace(stringBasedValue))
      {
        if (flag1)
          flag2 = true;
        else
          flag1 = true;
      }
      if (!string.IsNullOrWhiteSpace(hexColor))
      {
        if (flag1)
          flag2 = true;
        else
          flag1 = true;
      }
      if (functionNode != null && flag1)
        flag2 = true;
      if (flag2)
        throw new AstException(CssStrings.ExpectedSingleValue);
      this.UnaryOperator = unaryOperator;
      this.NumberBasedValue = numberBasedValue;
      this.StringBasedValue = stringBasedValue;
      this.Hexcolor = hexColor;
      this.FunctionNode = functionNode;
      this.ImportantComments = importantComments ?? new List<ImportantCommentNode>().AsReadOnly();
      this.IsBinary = false;
      this.ReplacementTokenBasedValue = replacementTokenBasedValue;
    }

    public string ReplacementTokenBasedValue { get; set; }

    public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }

    public bool IsBinary { get; set; }

    public string UnaryOperator { get; private set; }

    public string NumberBasedValue { get; private set; }

    public string StringBasedValue { get; private set; }

    public string Hexcolor { get; private set; }

    public FunctionNode FunctionNode { get; private set; }

    public bool Equals(TermNode termNode)
    {
      bool flag = termNode.IsBinary == this.IsBinary && termNode.UnaryOperator == this.UnaryOperator && termNode.NumberBasedValue == this.NumberBasedValue && termNode.StringBasedValue == this.StringBasedValue && termNode.Hexcolor == this.Hexcolor;
      return this.FunctionNode != null && termNode.FunctionNode != null ? flag && termNode.FunctionNode.Equals(this.FunctionNode) : this.FunctionNode == null && termNode.FunctionNode == null && flag;
    }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitTermNode(this);
  }
}
