// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.TerminalNodeImpl
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Tree
{
  internal class TerminalNodeImpl : ITerminalNode, IParseTree, ISyntaxTree, ITree
  {
    private IToken _symbol;
    private IRuleNode _parent;

    public TerminalNodeImpl(IToken symbol) => this._symbol = symbol;

    public virtual IParseTree GetChild(int i) => (IParseTree) null;

    ITree ITree.GetChild(int i) => (ITree) this.GetChild(i);

    public virtual IToken Symbol => this._symbol;

    public virtual IRuleNode Parent
    {
      get => this._parent;
      set => this._parent = value;
    }

    IParseTree IParseTree.Parent => (IParseTree) this.Parent;

    ITree ITree.Parent => (ITree) this.Parent;

    public virtual IToken Payload => this.Symbol;

    object ITree.Payload => (object) this.Payload;

    public virtual Interval SourceInterval
    {
      get
      {
        if (this.Symbol == null)
          return Interval.Invalid;
        int tokenIndex = this.Symbol.TokenIndex;
        return new Interval(tokenIndex, tokenIndex);
      }
    }

    public virtual int ChildCount => 0;

    public virtual T Accept<T>(IParseTreeVisitor<T> visitor) => visitor.VisitTerminal((ITerminalNode) this);

    public virtual string GetText() => this.Symbol != null ? this.Symbol.Text : (string) null;

    public virtual string ToStringTree(Parser parser) => this.ToString();

    public override string ToString()
    {
      if (this.Symbol == null)
        return "<null>";
      return this.Symbol.Type == -1 ? "<EOF>" : this.Symbol.Text;
    }

    public virtual string ToStringTree() => this.ToString();
  }
}
