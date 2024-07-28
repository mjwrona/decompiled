// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.IntegerValueSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Globalization;
using Tomlyn.Helpers;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class IntegerValueSyntax : ValueSyntax
  {
    private SyntaxToken? _token;

    public IntegerValueSyntax()
      : base(SyntaxKind.Integer)
    {
    }

    public IntegerValueSyntax(long value)
      : this()
    {
      this.Token = new SyntaxToken(TokenKind.Integer, value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.Value = value;
    }

    public SyntaxToken? Token
    {
      get => this._token;
      set => this.ParentToThis<SyntaxToken, string>(ref this._token, value, value != null && value.TokenKind.IsInteger(), "decimal/hex/binary/octal integer");
    }

    public long Value { get; set; }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 1;

    protected override SyntaxNode? GetChildImpl(int index) => (SyntaxNode) this.Token;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + TomlFormatHelper.ToString((float) this.Value);
  }
}
