// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.ParseTreeWalker
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Tree
{
  internal class ParseTreeWalker
  {
    public static readonly ParseTreeWalker Default = new ParseTreeWalker();

    public virtual void Walk(IParseTreeListener listener, IParseTree t)
    {
      switch (t)
      {
        case IErrorNode _:
          listener.VisitErrorNode((IErrorNode) t);
          break;
        case ITerminalNode _:
          listener.VisitTerminal((ITerminalNode) t);
          break;
        default:
          IRuleNode r = (IRuleNode) t;
          this.EnterRule(listener, r);
          int childCount = r.ChildCount;
          for (int i = 0; i < childCount; ++i)
            this.Walk(listener, r.GetChild(i));
          this.ExitRule(listener, r);
          break;
      }
    }

    protected internal virtual void EnterRule(IParseTreeListener listener, IRuleNode r)
    {
      ParserRuleContext ruleContext = (ParserRuleContext) r.RuleContext;
      listener.EnterEveryRule(ruleContext);
      ruleContext.EnterRule(listener);
    }

    protected internal virtual void ExitRule(IParseTreeListener listener, IRuleNode r)
    {
      ParserRuleContext ruleContext = (ParserRuleContext) r.RuleContext;
      ruleContext.ExitRule(listener);
      listener.ExitEveryRule(ruleContext);
    }
  }
}
