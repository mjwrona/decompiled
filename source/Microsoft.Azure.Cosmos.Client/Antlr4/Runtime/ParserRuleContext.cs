// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.ParserRuleContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;

namespace Antlr4.Runtime
{
  internal class ParserRuleContext : RuleContext
  {
    public static readonly ParserRuleContext EMPTY = new ParserRuleContext();
    public IList<IParseTree> children;
    private IToken _start;
    private IToken _stop;
    public RecognitionException exception;

    public ParserRuleContext()
    {
    }

    public static ParserRuleContext EmptyContext => ParserRuleContext.EMPTY;

    public virtual void CopyFrom(ParserRuleContext ctx)
    {
      this.Parent = ctx.Parent;
      this.invokingState = ctx.invokingState;
      this._start = ctx._start;
      this._stop = ctx._stop;
      if (ctx.children == null)
        return;
      this.children = (IList<IParseTree>) new List<IParseTree>();
      foreach (IParseTree child in (IEnumerable<IParseTree>) ctx.children)
      {
        if (child is ErrorNodeImpl errorNodeImpl)
        {
          this.children.Add((IParseTree) errorNodeImpl);
          errorNodeImpl.Parent = (IRuleNode) this;
        }
      }
    }

    public ParserRuleContext(ParserRuleContext parent, int invokingStateNumber)
      : base((RuleContext) parent, invokingStateNumber)
    {
    }

    public virtual void EnterRule(IParseTreeListener listener)
    {
    }

    public virtual void ExitRule(IParseTreeListener listener)
    {
    }

    public virtual void AddChild(ITerminalNode t)
    {
      if (this.children == null)
        this.children = (IList<IParseTree>) new List<IParseTree>();
      this.children.Add((IParseTree) t);
    }

    public virtual void AddChild(RuleContext ruleInvocation)
    {
      if (this.children == null)
        this.children = (IList<IParseTree>) new List<IParseTree>();
      this.children.Add((IParseTree) ruleInvocation);
    }

    public virtual void RemoveLastChild()
    {
      if (this.children == null)
        return;
      this.children.RemoveAt(this.children.Count - 1);
    }

    public virtual ITerminalNode AddChild(IToken matchedToken)
    {
      TerminalNodeImpl t = new TerminalNodeImpl(matchedToken);
      this.AddChild((ITerminalNode) t);
      t.Parent = (IRuleNode) this;
      return (ITerminalNode) t;
    }

    public virtual IErrorNode AddErrorNode(IToken badToken)
    {
      ErrorNodeImpl t = new ErrorNodeImpl(badToken);
      this.AddChild((ITerminalNode) t);
      t.Parent = (IRuleNode) this;
      return (IErrorNode) t;
    }

    public override IParseTree GetChild(int i) => this.children == null || i < 0 || i >= this.children.Count ? (IParseTree) null : this.children[i];

    public virtual T GetChild<T>(int i) where T : IParseTree
    {
      if (this.children == null || i < 0 || i >= this.children.Count)
        return default (T);
      int num = -1;
      foreach (IParseTree child in (IEnumerable<IParseTree>) this.children)
      {
        if (child is T)
        {
          ++num;
          if (num == i)
            return (T) child;
        }
      }
      return default (T);
    }

    public virtual ITerminalNode GetToken(int ttype, int i)
    {
      if (this.children == null || i < 0 || i >= this.children.Count)
        return (ITerminalNode) null;
      int num = -1;
      foreach (IParseTree child in (IEnumerable<IParseTree>) this.children)
      {
        if (child is ITerminalNode)
        {
          ITerminalNode token = (ITerminalNode) child;
          if (token.Symbol.Type == ttype)
          {
            ++num;
            if (num == i)
              return token;
          }
        }
      }
      return (ITerminalNode) null;
    }

    public virtual ITerminalNode[] GetTokens(int ttype)
    {
      if (this.children == null)
        return Antlr4.Runtime.Sharpen.Collections.EmptyList<ITerminalNode>();
      List<ITerminalNode> terminalNodeList = (List<ITerminalNode>) null;
      foreach (IParseTree child in (IEnumerable<IParseTree>) this.children)
      {
        if (child is ITerminalNode)
        {
          ITerminalNode terminalNode = (ITerminalNode) child;
          if (terminalNode.Symbol.Type == ttype)
          {
            if (terminalNodeList == null)
              terminalNodeList = new List<ITerminalNode>();
            terminalNodeList.Add(terminalNode);
          }
        }
      }
      return terminalNodeList == null ? Antlr4.Runtime.Sharpen.Collections.EmptyList<ITerminalNode>() : terminalNodeList.ToArray();
    }

    public virtual T GetRuleContext<T>(int i) where T : ParserRuleContext => this.GetChild<T>(i);

    public virtual T[] GetRuleContexts<T>() where T : ParserRuleContext
    {
      if (this.children == null)
        return Antlr4.Runtime.Sharpen.Collections.EmptyList<T>();
      List<T> objList = (List<T>) null;
      foreach (IParseTree child in (IEnumerable<IParseTree>) this.children)
      {
        if (child is T)
        {
          if (objList == null)
            objList = new List<T>();
          objList.Add((T) child);
        }
      }
      return objList == null ? Antlr4.Runtime.Sharpen.Collections.EmptyList<T>() : objList.ToArray();
    }

    public override int ChildCount => this.children == null ? 0 : this.children.Count;

    public override Interval SourceInterval => this._start == null || this._stop == null ? Interval.Invalid : Interval.Of(this._start.TokenIndex, this._stop.TokenIndex);

    public virtual IToken Start
    {
      get => this._start;
      set => this._start = value;
    }

    public virtual IToken Stop
    {
      get => this._stop;
      set => this._stop = value;
    }

    public virtual string ToInfoString(Parser recognizer)
    {
      List<string> stringList = new List<string>((IEnumerable<string>) recognizer.GetRuleInvocationStack((RuleContext) this));
      stringList.Reverse();
      return nameof (ParserRuleContext) + stringList?.ToString() + "{start=" + this._start?.ToString() + ", stop=" + this._stop?.ToString() + "}";
    }
  }
}
