// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.AbstractParseTreeVisitor`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Tree
{
  internal abstract class AbstractParseTreeVisitor<Result> : IParseTreeVisitor<Result>
  {
    public virtual Result Visit(IParseTree tree) => tree.Accept<Result>((IParseTreeVisitor<Result>) this);

    public virtual Result VisitChildren(IRuleNode node)
    {
      Result result = this.DefaultResult;
      int childCount = node.ChildCount;
      for (int i = 0; i < childCount && this.ShouldVisitNextChild(node, result); ++i)
      {
        Result nextResult = node.GetChild(i).Accept<Result>((IParseTreeVisitor<Result>) this);
        result = this.AggregateResult(result, nextResult);
      }
      return result;
    }

    public virtual Result VisitTerminal(ITerminalNode node) => this.DefaultResult;

    public virtual Result VisitErrorNode(IErrorNode node) => this.DefaultResult;

    protected internal virtual Result DefaultResult => default (Result);

    protected internal virtual Result AggregateResult(Result aggregate, Result nextResult) => nextResult;

    protected internal virtual bool ShouldVisitNextChild(IRuleNode node, Result currentResult) => true;
  }
}
