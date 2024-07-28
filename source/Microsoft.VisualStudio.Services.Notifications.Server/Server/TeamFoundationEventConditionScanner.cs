// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationEventConditionScanner
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class TeamFoundationEventConditionScanner
  {
    private TextReader reader;
    private StringBuilder spelling;
    private char currentChar;
    private static readonly char EOT;
    private Func<char, bool> IsValidTokenChar;

    public TeamFoundationEventConditionScanner(string input, Func<char, bool> isValidTokenChar)
    {
      this.reader = (TextReader) new StringReader(input ?? string.Empty);
      this.IsValidTokenChar = isValidTokenChar;
      this.Take();
    }

    private void TakeIt()
    {
      this.spelling.Append(this.currentChar);
      this.Take();
    }

    private void Take()
    {
      int num = this.reader.Read();
      if (num == -1)
        num = (int) TeamFoundationEventConditionScanner.EOT;
      this.currentChar = (char) num;
    }

    internal Token NextToken()
    {
      while (this.IsWhiteSpace())
        this.Take();
      if ((int) this.currentChar == (int) TeamFoundationEventConditionScanner.EOT)
        return (Token) null;
      this.spelling = new StringBuilder();
      switch (this.currentChar)
      {
        case '"':
        case '\'':
          char currentChar = this.currentChar;
          this.Take();
          while ((int) this.currentChar != (int) currentChar)
          {
            if (this.currentChar == '\n')
              throw new ParseException(CoreRes.EventConditionUnexpectedEndOfLine());
            if ((int) this.currentChar == (int) TeamFoundationEventConditionScanner.EOT)
              throw new ParseException(CoreRes.EventConditionUnexpectedEndOfFile());
            this.TakeIt();
          }
          this.Take();
          return (Token) new XPathToken((byte) 1, string.Intern(this.spelling.ToString()));
        case '(':
          this.TakeIt();
          return (Token) new ConstantToken((byte) 20, (string) null);
        case ')':
          this.TakeIt();
          return (Token) new ConstantToken((byte) 21, (string) null);
        case '+':
          this.TakeIt();
          return (Token) new ConstantToken((byte) 18, (string) null);
        case '-':
          this.TakeIt();
          return (Token) new ConstantToken((byte) 19, (string) null);
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
          this.TakeIt();
          while (this.IsDigit())
            this.TakeIt();
          return (Token) new ConstantToken((byte) 0, this.spelling.ToString());
        case '<':
          this.TakeIt();
          if (this.currentChar == '=')
          {
            this.TakeIt();
            return (Token) new ConstantToken((byte) 10, (string) null);
          }
          if (this.currentChar != '>')
            return (Token) new ConstantToken((byte) 8, (string) null);
          this.TakeIt();
          return (Token) new ConstantToken((byte) 13, (string) null);
        case '=':
          this.TakeIt();
          return (Token) new ConstantToken((byte) 12, (string) null);
        case '>':
          this.TakeIt();
          if (this.currentChar != '=')
            return (Token) new ConstantToken((byte) 9, (string) null);
          this.TakeIt();
          return (Token) new ConstantToken((byte) 11, (string) null);
        default:
          if (!this.IsValidTokenChar(this.currentChar))
            throw new ParseException(CoreRes.EventConditionSyntaxError((object) this.currentChar));
          this.TakeIt();
          while (this.IsValidTokenChar(this.currentChar) || this.IsDigit())
            this.TakeIt();
          string lower = this.spelling.ToString().ToLower(CultureInfo.InvariantCulture);
          for (int type = 0; type < Token.spellings.Length; ++type)
          {
            if (lower.Equals(Token.spellings[type], StringComparison.InvariantCultureIgnoreCase))
              return (Token) new XPathToken((byte) type, Token.spellings[type]);
          }
          return (Token) new XPathToken((byte) 1, string.Intern(this.spelling.ToString()));
      }
    }

    private bool IsValidFieldCharacter(char currentChar) => char.IsLetter(currentChar) || currentChar == '$' || currentChar == '.';

    private bool IsDigit() => char.IsDigit(this.currentChar);

    private bool IsWhiteSpace() => char.IsWhiteSpace(this.currentChar);
  }
}
