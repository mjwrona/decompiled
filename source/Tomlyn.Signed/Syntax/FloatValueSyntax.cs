// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.FloatValueSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Globalization;
using Tomlyn.Helpers;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class FloatValueSyntax : ValueSyntax
  {
    private SyntaxToken? _token;
    private const string FloatFormat = "g16";

    public FloatValueSyntax()
      : base(SyntaxKind.Float)
    {
    }

    public FloatValueSyntax(double value)
      : this()
    {
      this.Token = !double.IsNaN(value) ? (!double.IsPositiveInfinity(value) ? (!double.IsNegativeInfinity(value) ? new SyntaxToken(TokenKind.Float, value.ToString("g16", (IFormatProvider) CultureInfo.InvariantCulture)) : new SyntaxToken(TokenKind.NegativeInfinite, TokenKind.NegativeInfinite.ToText())) : new SyntaxToken(TokenKind.PositiveInfinite, TokenKind.PositiveInfinite.ToText())) : new SyntaxToken(TokenKind.Nan, value < 0.0 ? TokenKind.NegativeNan.ToText() : TokenKind.Nan.ToText());
      this.Value = value;
    }

    public SyntaxToken? Token
    {
      get => this._token;
      set => this.ParentToThis<SyntaxToken, TokenKind>(ref this._token, value, value != null && value.TokenKind.IsFloat(), TokenKind.Float);
    }

    public double Value { get; set; }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 1;

    protected override SyntaxNode? GetChildImpl(int index) => (SyntaxNode) this.Token;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + TomlFormatHelper.ToString(this.Value);
  }
}
