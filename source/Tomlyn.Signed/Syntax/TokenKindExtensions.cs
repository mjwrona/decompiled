// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.TokenKindExtensions
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll


#nullable enable
namespace Tomlyn.Syntax
{
  public static class TokenKindExtensions
  {
    public static bool IsHidden(this TokenKind tokenKind, bool hideNewLine = true) => tokenKind == TokenKind.Whitespaces || tokenKind == TokenKind.Comment || tokenKind == TokenKind.NewLine & hideNewLine;

    public static string? ToText(this TokenKind kind)
    {
      switch (kind)
      {
        case TokenKind.Comma:
          return ",";
        case TokenKind.Dot:
          return ".";
        case TokenKind.Equal:
          return "=";
        case TokenKind.OpenBracket:
          return "[";
        case TokenKind.OpenBracketDouble:
          return "[[";
        case TokenKind.CloseBracket:
          return "]";
        case TokenKind.CloseBracketDouble:
          return "]]";
        case TokenKind.OpenBrace:
          return "{";
        case TokenKind.CloseBrace:
          return "}";
        case TokenKind.True:
          return "true";
        case TokenKind.False:
          return "false";
        case TokenKind.Infinite:
          return "inf";
        case TokenKind.PositiveInfinite:
          return "+inf";
        case TokenKind.NegativeInfinite:
          return "-inf";
        case TokenKind.Nan:
          return "nan";
        case TokenKind.PositiveNan:
          return "+nan";
        case TokenKind.NegativeNan:
          return "-nan";
        default:
          return (string) null;
      }
    }

    public static bool IsFloat(this TokenKind kind)
    {
      switch (kind)
      {
        case TokenKind.Float:
        case TokenKind.Infinite:
        case TokenKind.PositiveInfinite:
        case TokenKind.NegativeInfinite:
        case TokenKind.Nan:
        case TokenKind.PositiveNan:
        case TokenKind.NegativeNan:
          return true;
        default:
          return false;
      }
    }

    public static bool IsInteger(this TokenKind kind)
    {
      switch (kind)
      {
        case TokenKind.Integer:
        case TokenKind.IntegerHexa:
        case TokenKind.IntegerOctal:
        case TokenKind.IntegerBinary:
          return true;
        default:
          return false;
      }
    }

    public static bool IsDateTime(this TokenKind kind)
    {
      switch (kind)
      {
        case TokenKind.OffsetDateTimeByZ:
        case TokenKind.OffsetDateTimeByNumber:
        case TokenKind.LocalDateTime:
        case TokenKind.LocalDate:
        case TokenKind.LocalTime:
          return true;
        default:
          return false;
      }
    }

    public static bool IsString(this TokenKind kind)
    {
      switch (kind)
      {
        case TokenKind.String:
        case TokenKind.StringMulti:
        case TokenKind.StringLiteral:
        case TokenKind.StringLiteralMulti:
          return true;
        default:
          return false;
      }
    }

    public static bool IsTrivia(this TokenKind kind)
    {
      switch (kind)
      {
        case TokenKind.Whitespaces:
        case TokenKind.NewLine:
        case TokenKind.Comment:
          return true;
        default:
          return false;
      }
    }

    public static bool IsToken(this TokenKind kind) => kind == TokenKind.NewLine || (uint) (kind - 19) <= 16U;
  }
}
