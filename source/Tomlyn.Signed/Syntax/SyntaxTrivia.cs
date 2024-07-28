// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxTrivia
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using Tomlyn.Helpers;
using Tomlyn.Model;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class SyntaxTrivia : SyntaxNodeBase
  {
    public SyntaxTrivia()
    {
    }

    public SyntaxTrivia(TokenKind kind, string text)
    {
      this.Kind = kind.IsTrivia() ? kind : throw new ArgumentOutOfRangeException(nameof (kind), string.Format("The kind `{0}` is not a trivia", (object) kind));
      this.Text = text ?? throw new ArgumentNullException(nameof (text));
    }

    public TokenKind Kind { get; set; }

    public string? Text { get; set; }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + " " + (this.Text != null ? TomlFormatHelper.ToString(this.Text, TomlPropertyDisplayKind.Default) : string.Empty);
  }
}
