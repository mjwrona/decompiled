// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.InlineTableSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class InlineTableSyntax : ValueSyntax
  {
    private SyntaxToken? _openBrace;
    private SyntaxToken? _closeBrace;

    public InlineTableSyntax()
      : base(SyntaxKind.InlineTable)
    {
      SyntaxList<InlineTableItemSyntax> syntaxList = new SyntaxList<InlineTableItemSyntax>();
      syntaxList.Parent = (SyntaxNode) this;
      this.Items = syntaxList;
    }

    public InlineTableSyntax(params KeyValueSyntax[] keyValues)
      : this()
    {
      if (keyValues == null)
        throw new ArgumentNullException(nameof (keyValues));
      this.OpenBrace = SyntaxFactory.Token(TokenKind.OpenBrace).AddTrailingWhitespace<SyntaxToken>();
      this.CloseBrace = SyntaxFactory.Token(TokenKind.CloseBrace).AddLeadingWhitespace<SyntaxToken>();
      for (int index = 0; index < keyValues.Length; ++index)
        this.Items.Add(new InlineTableItemSyntax(keyValues[index])
        {
          Comma = index + 1 < keyValues.Length ? SyntaxFactory.Token(TokenKind.Comma).AddTrailingWhitespace<SyntaxToken>() : (SyntaxToken) null
        });
    }

    public SyntaxToken? OpenBrace
    {
      get => this._openBrace;
      set => this.ParentToThis<SyntaxToken>(ref this._openBrace, value, TokenKind.OpenBrace);
    }

    public SyntaxList<InlineTableItemSyntax> Items { get; }

    public SyntaxToken? CloseBrace
    {
      get => this._closeBrace;
      set => this.ParentToThis<SyntaxToken>(ref this._closeBrace, value, TokenKind.CloseBrace);
    }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 3;

    protected override SyntaxNode? GetChildImpl(int index)
    {
      if (index == 0)
        return (SyntaxNode) this.OpenBrace;
      return index == 1 ? (SyntaxNode) this.Items : (SyntaxNode) this.CloseBrace;
    }

    protected override string ToDebuggerDisplay() => string.Format("{0} Count = {1}", (object) base.ToDebuggerDisplay(), (object) this.Items.ChildrenCount);
  }
}
