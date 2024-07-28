// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.ArraySyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class ArraySyntax : ValueSyntax
  {
    private SyntaxToken? _openBracket;
    private SyntaxToken? _closeBracket;

    public ArraySyntax()
      : base(SyntaxKind.Array)
    {
      SyntaxList<ArrayItemSyntax> syntaxList = new SyntaxList<ArrayItemSyntax>();
      syntaxList.Parent = (SyntaxNode) this;
      this.Items = syntaxList;
    }

    public ArraySyntax(int[] values)
      : this()
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      this.OpenBracket = SyntaxFactory.Token(TokenKind.OpenBracket);
      this.CloseBracket = SyntaxFactory.Token(TokenKind.CloseBracket);
      for (int index = 0; index < values.Length; ++index)
      {
        ArrayItemSyntax node = new ArrayItemSyntax()
        {
          Value = (ValueSyntax) new IntegerValueSyntax((long) values[index])
        };
        if (index + 1 < values.Length)
        {
          node.Comma = SyntaxFactory.Token(TokenKind.Comma);
          node.Comma.AddTrailingWhitespace<SyntaxToken>();
        }
        this.Items.Add(node);
      }
    }

    public ArraySyntax(string[] values)
      : this()
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      this.OpenBracket = SyntaxFactory.Token(TokenKind.OpenBracket);
      this.CloseBracket = SyntaxFactory.Token(TokenKind.CloseBracket);
      for (int index = 0; index < values.Length; ++index)
      {
        ArrayItemSyntax node = new ArrayItemSyntax()
        {
          Value = (ValueSyntax) new StringValueSyntax(values[index])
        };
        if (index + 1 < values.Length)
        {
          node.Comma = SyntaxFactory.Token(TokenKind.Comma);
          node.Comma.AddTrailingWhitespace<SyntaxToken>();
        }
        this.Items.Add(node);
      }
    }

    public SyntaxToken? OpenBracket
    {
      get => this._openBracket;
      set => this.ParentToThis<SyntaxToken>(ref this._openBracket, value, TokenKind.OpenBracket);
    }

    public SyntaxList<ArrayItemSyntax> Items { get; }

    public SyntaxToken? CloseBracket
    {
      get => this._closeBracket;
      set => this.ParentToThis<SyntaxToken>(ref this._closeBracket, value, TokenKind.CloseBracket);
    }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 3;

    protected override SyntaxNode? GetChildImpl(int index)
    {
      if (index == 0)
        return (SyntaxNode) this.OpenBracket;
      return index == 1 ? (SyntaxNode) this.Items : (SyntaxNode) this.CloseBracket;
    }

    protected override string ToDebuggerDisplay() => string.Format("{0} Count = {1}", (object) base.ToDebuggerDisplay(), (object) this.Items.ChildrenCount);
  }
}
