// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.TableSyntaxBase
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll


#nullable enable
namespace Tomlyn.Syntax
{
  public abstract class TableSyntaxBase : SyntaxNode
  {
    private SyntaxToken? _openBracket;
    private KeySyntax? _name;
    private SyntaxToken? _closeBracket;
    private SyntaxToken? _endOfLineToken;

    internal TableSyntaxBase(SyntaxKind kind)
      : base(kind)
    {
      SyntaxList<KeyValueSyntax> syntaxList = new SyntaxList<KeyValueSyntax>();
      syntaxList.Parent = (SyntaxNode) this;
      this.Items = syntaxList;
    }

    public SyntaxToken? OpenBracket
    {
      get => this._openBracket;
      set => this.ParentToThis<SyntaxToken>(ref this._openBracket, value, this.OpenTokenKind);
    }

    public KeySyntax? Name
    {
      get => this._name;
      set => this.ParentToThis<KeySyntax>(ref this._name, value);
    }

    public SyntaxToken? CloseBracket
    {
      get => this._closeBracket;
      set => this.ParentToThis<SyntaxToken>(ref this._closeBracket, value, this.CloseTokenKind);
    }

    public SyntaxToken? EndOfLineToken
    {
      get => this._endOfLineToken;
      set => this.ParentToThis<SyntaxToken>(ref this._endOfLineToken, value, TokenKind.NewLine, TokenKind.Eof);
    }

    public SyntaxList<KeyValueSyntax> Items { get; }

    public override int ChildrenCount => 5;

    internal abstract TokenKind OpenTokenKind { get; }

    internal abstract TokenKind CloseTokenKind { get; }

    protected override SyntaxNode? GetChildImpl(int index)
    {
      switch (index)
      {
        case 0:
          return (SyntaxNode) this.OpenBracket;
        case 1:
          return (SyntaxNode) this.Name;
        case 2:
          return (SyntaxNode) this.CloseBracket;
        case 3:
          return (SyntaxNode) this.EndOfLineToken;
        default:
          return (SyntaxNode) this.Items;
      }
    }
  }
}
