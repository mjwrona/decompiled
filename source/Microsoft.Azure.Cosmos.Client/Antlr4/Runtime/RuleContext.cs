// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.RuleContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime
{
  internal class RuleContext : IRuleNode, IParseTree, ISyntaxTree, ITree
  {
    private RuleContext _parent;
    public int invokingState = -1;

    public RuleContext()
    {
    }

    public RuleContext(RuleContext parent, int invokingState)
    {
      this._parent = parent;
      this.invokingState = invokingState;
    }

    public static RuleContext GetChildContext(RuleContext parent, int invokingState) => new RuleContext(parent, invokingState);

    public virtual int Depth()
    {
      int num = 0;
      RuleContext ruleContext = this;
      while (ruleContext != null)
      {
        ruleContext = ruleContext._parent;
        ++num;
      }
      return num;
    }

    public virtual bool IsEmpty => this.invokingState == -1;

    public virtual Interval SourceInterval => Interval.Invalid;

    RuleContext IRuleNode.RuleContext => this;

    public virtual RuleContext Parent
    {
      get => this._parent;
      set => this._parent = value;
    }

    IRuleNode IRuleNode.Parent => (IRuleNode) this.Parent;

    IParseTree IParseTree.Parent => (IParseTree) this.Parent;

    ITree ITree.Parent => (ITree) this.Parent;

    public virtual RuleContext Payload => this;

    object ITree.Payload => (object) this.Payload;

    public virtual string GetText()
    {
      if (this.ChildCount == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      for (int i = 0; i < this.ChildCount; ++i)
        stringBuilder.Append(this.GetChild(i).GetText());
      return stringBuilder.ToString();
    }

    public virtual int RuleIndex => -1;

    public virtual int getAltNumber() => 0;

    public virtual void setAltNumber(int altNumber)
    {
    }

    public virtual IParseTree GetChild(int i) => (IParseTree) null;

    ITree ITree.GetChild(int i) => (ITree) this.GetChild(i);

    public virtual int ChildCount => 0;

    public virtual T Accept<T>(IParseTreeVisitor<T> visitor) => visitor.VisitChildren((IRuleNode) this);

    public virtual string ToStringTree(Parser recog) => Trees.ToStringTree((ITree) this, recog);

    public virtual string ToStringTree(IList<string> ruleNames) => Trees.ToStringTree((ITree) this, ruleNames);

    public virtual string ToStringTree() => this.ToStringTree((IList<string>) null);

    public override string ToString() => this.ToString((IList<string>) null, (RuleContext) null);

    public string ToString(IRecognizer recog) => this.ToString(recog, (RuleContext) ParserRuleContext.EmptyContext);

    public string ToString(IList<string> ruleNames) => this.ToString(ruleNames, (RuleContext) null);

    public virtual string ToString(IRecognizer recog, RuleContext stop)
    {
      string[] ruleNames = recog?.RuleNames;
      return this.ToString(ruleNames != null ? Arrays.AsList<string>(ruleNames) : (IList<string>) null, stop);
    }

    public virtual string ToString(IList<string> ruleNames, RuleContext stop)
    {
      StringBuilder stringBuilder = new StringBuilder();
      RuleContext ruleContext = this;
      stringBuilder.Append("[");
      for (; ruleContext != null && ruleContext != stop; ruleContext = ruleContext.Parent)
      {
        if (ruleNames == null)
        {
          if (!ruleContext.IsEmpty)
            stringBuilder.Append(ruleContext.invokingState);
        }
        else
        {
          int ruleIndex = ruleContext.RuleIndex;
          string str = ruleIndex < 0 || ruleIndex >= ruleNames.Count ? ruleIndex.ToString() : ruleNames[ruleIndex];
          stringBuilder.Append(str);
        }
        if (ruleContext.Parent != null && (ruleNames != null || !ruleContext.Parent.IsEmpty))
          stringBuilder.Append(" ");
      }
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
