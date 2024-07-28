// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.StringValueSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using Tomlyn.Helpers;
using Tomlyn.Model;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class StringValueSyntax : BareKeyOrStringValueSyntax
  {
    private SyntaxToken? _token;

    public StringValueSyntax()
      : base(SyntaxKind.String)
    {
    }

    public StringValueSyntax(string text)
      : this()
    {
      this.Token = text != null ? new SyntaxToken(TokenKind.String, "\"" + text.EscapeForToml() + "\"") : throw new ArgumentNullException(nameof (text));
      this.Value = text;
    }

    public SyntaxToken? Token
    {
      get => this._token;
      set => this.ParentToThis<SyntaxToken, string>(ref this._token, value, value != null && value.TokenKind.IsString(), "string");
    }

    public string? Value { get; set; }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 1;

    protected override SyntaxNode? GetChildImpl(int index) => (SyntaxNode) this.Token;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + (this.Value != null ? TomlFormatHelper.ToString(this.Value, TomlPropertyDisplayKind.Default) : string.Empty);
  }
}
