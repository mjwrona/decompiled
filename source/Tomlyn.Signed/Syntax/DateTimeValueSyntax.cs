// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.DateTimeValueSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using Tomlyn.Helpers;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class DateTimeValueSyntax : ValueSyntax
  {
    private SyntaxToken? _token;

    public DateTimeValueSyntax(SyntaxKind kind)
      : base(DateTimeValueSyntax.CheckDateTimeKind(kind))
    {
    }

    public SyntaxToken? Token
    {
      get => this._token;
      set => this.ParentToThis<SyntaxToken, string>(ref this._token, value, value != null && value.TokenKind.IsDateTime(), string.Format("The token kind `{0}` is not a datetime token", (object) value?.TokenKind));
    }

    public TomlDateTime Value { get; set; }

    public override int ChildrenCount => 1;

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    protected override SyntaxNode? GetChildImpl(int index) => (SyntaxNode) this.Token;

    private static SyntaxKind CheckDateTimeKind(SyntaxKind kind)
    {
      switch (kind)
      {
        case SyntaxKind.OffsetDateTimeByZ:
        case SyntaxKind.OffsetDateTimeByNumber:
        case SyntaxKind.LocalDateTime:
        case SyntaxKind.LocalDate:
        case SyntaxKind.LocalTime:
          return kind;
        default:
          throw new ArgumentOutOfRangeException(nameof (kind), (object) kind, (string) null);
      }
    }

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + TomlFormatHelper.ToString(this.Value);
  }
}
