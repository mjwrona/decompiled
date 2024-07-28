// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.TableSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class TableSyntax : TableSyntaxBase
  {
    public TableSyntax()
      : base(SyntaxKind.Table)
    {
    }

    public TableSyntax(string name)
      : this()
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      this.OpenBracket = SyntaxFactory.Token(TokenKind.OpenBracket);
      this.Name = new KeySyntax(name);
      this.CloseBracket = SyntaxFactory.Token(TokenKind.CloseBracket);
      this.EndOfLineToken = SyntaxFactory.NewLine();
    }

    public TableSyntax(KeySyntax name)
      : this()
    {
      this.Name = name ?? throw new ArgumentNullException(nameof (name));
      this.OpenBracket = SyntaxFactory.Token(TokenKind.OpenBracket);
      this.CloseBracket = SyntaxFactory.Token(TokenKind.CloseBracket);
      this.EndOfLineToken = SyntaxFactory.NewLine();
    }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    internal override TokenKind OpenTokenKind => TokenKind.OpenBracket;

    internal override TokenKind CloseTokenKind => TokenKind.CloseBracket;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + " [" + (this.Name != null ? this.Name.ToString() : string.Empty) + "]";
  }
}
