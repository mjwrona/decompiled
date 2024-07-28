// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.FunctionNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class FunctionNode : AstNode
  {
    private static string[] BinaryOpererableFunctionNames = new string[4]
    {
      "-webkit-calc",
      "calc",
      "min",
      "max"
    };
    private static string[] binaryOperators = new string[2]
    {
      "-",
      "+"
    };

    public FunctionNode(string functionName, ExprNode exprNode)
    {
      this.FunctionName = functionName;
      this.ExprNode = exprNode;
      if (this.ExprNode == null)
        return;
      this.ExprNode.UsesBinary = this.usesBinary();
    }

    public string FunctionName { get; private set; }

    public ExprNode ExprNode { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitFunctionNode(this);

    private bool usesBinary() => Array.IndexOf<string>(FunctionNode.BinaryOpererableFunctionNames, this.FunctionName) > -1;

    public bool Equals(FunctionNode functionNode) => this.FunctionName == functionNode.FunctionName && this.ExprNode.Equals(functionNode.ExprNode);

    public static bool IsBinaryOperator(string binaryOperator) => Array.IndexOf<string>(FunctionNode.binaryOperators, binaryOperator) > -1;
  }
}
