// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxToken
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using Tomlyn.Helpers;
using Tomlyn.Model;


#nullable enable
namespace Tomlyn.Syntax
{
  public class SyntaxToken : SyntaxNode
  {
    public SyntaxToken()
      : base(SyntaxKind.Token)
    {
    }

    public SyntaxToken(TokenKind tokenKind, string? text)
      : this()
    {
      this.TokenKind = tokenKind;
      this.Text = text;
    }

    public TokenKind TokenKind { get; set; }

    public string? Text { get; set; }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 0;

    protected override SyntaxNode? GetChildImpl(int index) => (SyntaxNode) null;

    protected override string ToDebuggerDisplay() => string.Format("{0}: {1} {2}", (object) base.ToDebuggerDisplay(), (object) this.TokenKind, this.Text != null ? (object) TomlFormatHelper.ToString(this.Text, TomlPropertyDisplayKind.Default) : (object) string.Empty);
  }
}
