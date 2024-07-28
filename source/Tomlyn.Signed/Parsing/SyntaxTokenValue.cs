// Decompiled with JetBrains decompiler
// Type: Tomlyn.Parsing.SyntaxTokenValue
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using Tomlyn.Syntax;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Parsing
{
  public readonly struct SyntaxTokenValue : IEquatable<SyntaxTokenValue>
  {
    public readonly TokenKind Kind;
    public readonly TextPosition Start;
    public readonly TextPosition End;
    public readonly object? Value;

    public SyntaxTokenValue(TokenKind kind, TextPosition start, TextPosition end, object? value = null)
    {
      if (start.Offset > end.Offset)
        throw new ArgumentOutOfRangeException(nameof (start), "[start] index must be <= to [end]");
      this.Kind = kind;
      this.Start = start;
      this.End = end;
      this.Value = value;
    }

    public override string ToString() => string.Format("{0}({1}:{2})", (object) this.Kind, (object) this.Start, (object) this.End);

    public string? GetText(string text)
    {
      if (this.Kind == TokenKind.Eof)
        return "<eof>";
      return this.End.Offset >= text.Length ? (string) null : text.Substring(this.Start.Offset, this.End.Offset - this.Start.Offset + 1);
    }

    internal string? GetText<TTextView>(TTextView text) where TTextView : IStringView => this.Kind == TokenKind.Eof ? "<eof>" : text.GetString(this.Start.Offset, this.End.Offset - this.Start.Offset + 1);

    public bool Equals(SyntaxTokenValue other) => this.Kind == other.Kind && this.Start.Equals(other.Start) && this.End.Equals(other.End);

    public override bool Equals(object? obj) => obj != null && obj is SyntaxTokenValue other && this.Equals(other);

    public override int GetHashCode() => ((int) this.Kind * 397 ^ this.Start.GetHashCode()) * 397 ^ this.End.GetHashCode();

    public static bool operator ==(SyntaxTokenValue left, SyntaxTokenValue right) => left.Equals(right);

    public static bool operator !=(SyntaxTokenValue left, SyntaxTokenValue right) => !left.Equals(right);
  }
}
