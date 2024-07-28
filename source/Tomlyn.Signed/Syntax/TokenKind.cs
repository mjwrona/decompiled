// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.TokenKind
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

namespace Tomlyn.Syntax
{
  public enum TokenKind
  {
    Invalid,
    Eof,
    Whitespaces,
    NewLine,
    Comment,
    OffsetDateTimeByZ,
    OffsetDateTimeByNumber,
    LocalDateTime,
    LocalDate,
    LocalTime,
    Integer,
    IntegerHexa,
    IntegerOctal,
    IntegerBinary,
    Float,
    String,
    StringMulti,
    StringLiteral,
    StringLiteralMulti,
    Comma,
    Dot,
    Equal,
    OpenBracket,
    OpenBracketDouble,
    CloseBracket,
    CloseBracketDouble,
    OpenBrace,
    CloseBrace,
    True,
    False,
    Infinite,
    PositiveInfinite,
    NegativeInfinite,
    Nan,
    PositiveNan,
    NegativeNan,
    BasicKey,
  }
}
