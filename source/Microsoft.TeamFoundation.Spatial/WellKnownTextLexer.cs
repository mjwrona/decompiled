// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.WellKnownTextLexer
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.IO;

namespace Microsoft.Spatial
{
  internal class WellKnownTextLexer : TextLexerBase
  {
    public WellKnownTextLexer(TextReader text)
      : base(text)
    {
    }

    protected override bool MatchTokenType(char nextChar, int? activeTokenType, out int tokenType)
    {
      switch (nextChar)
      {
        case '\t':
        case '\n':
        case '\r':
        case ' ':
          tokenType = 8;
          return false;
        case '(':
          tokenType = 4;
          return true;
        case ')':
          tokenType = 5;
          return true;
        case '+':
        case '-':
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          tokenType = 2;
          return false;
        case ',':
          tokenType = 7;
          return true;
        case '.':
          tokenType = 6;
          return true;
        case ';':
          tokenType = 3;
          return true;
        case '=':
          tokenType = 1;
          return true;
        case 'E':
        case 'e':
          int? nullable = activeTokenType;
          int num = 2;
          tokenType = !(nullable.GetValueOrDefault() == num & nullable.HasValue) ? 0 : 2;
          return false;
        default:
          if ((nextChar < 'A' || nextChar > 'Z') && (nextChar < 'a' || nextChar > 'z'))
            throw new FormatException(Strings.WellKnownText_UnexpectedCharacter((object) nextChar));
          tokenType = 0;
          return false;
      }
    }
  }
}
