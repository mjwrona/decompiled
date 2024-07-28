// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.BooleanValueSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using Tomlyn.Helpers;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class BooleanValueSyntax : ValueSyntax
  {
    private SyntaxToken? _token;
    private bool _value;

    public BooleanValueSyntax()
      : base(SyntaxKind.Boolean)
    {
    }

    public BooleanValueSyntax(bool value)
      : this()
    {
      TokenKind tokenKind = value ? TokenKind.True : TokenKind.False;
      this.Token = new SyntaxToken(tokenKind, tokenKind.ToText());
      this.Value = value;
    }

    public SyntaxToken? Token
    {
      get => this._token;
      set => this.ParentToThis<SyntaxToken>(ref this._token, value, TokenKind.True, TokenKind.False);
    }

    public bool Value
    {
      get => this._value;
      set => this._value = value;
    }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 1;

    protected override SyntaxNode? GetChildImpl(int index) => (SyntaxNode) this.Token;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + TomlFormatHelper.ToString(this.Value);
  }
}
