// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.TableArraySyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class TableArraySyntax : TableSyntaxBase
  {
    public TableArraySyntax()
      : base(SyntaxKind.TableArray)
    {
    }

    public TableArraySyntax(string name)
      : this()
    {
      this.Name = name != null ? new KeySyntax(name) : throw new ArgumentNullException(nameof (name));
      this.OpenBracket = SyntaxFactory.Token(TokenKind.OpenBracketDouble);
      this.CloseBracket = SyntaxFactory.Token(TokenKind.CloseBracketDouble);
      this.EndOfLineToken = SyntaxFactory.NewLine();
    }

    public TableArraySyntax(KeySyntax name)
      : this()
    {
      this.Name = name ?? throw new ArgumentNullException(nameof (name));
      this.OpenBracket = SyntaxFactory.Token(TokenKind.OpenBracketDouble);
      this.CloseBracket = SyntaxFactory.Token(TokenKind.CloseBracketDouble);
      this.EndOfLineToken = SyntaxFactory.NewLine();
    }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    internal override TokenKind OpenTokenKind => TokenKind.OpenBracketDouble;

    internal override TokenKind CloseTokenKind => TokenKind.CloseBracketDouble;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + " [[" + (this.Name != null ? this.Name.ToString() : string.Empty) + "]]";
  }
}
