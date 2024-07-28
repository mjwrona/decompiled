// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpressionLexer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Microsoft.OData.UriParser
{
  [DebuggerDisplay("ExpressionLexer ({text} @ {textPos} [{token}])")]
  internal class ExpressionLexer
  {
    protected readonly string Text;
    protected readonly int TextLen;
    protected int textPos;
    protected char? ch;
    protected ExpressionToken token;
    private static readonly HashSet<UnicodeCategory> AdditionalUnicodeCategoriesForIdentifier = new HashSet<UnicodeCategory>((IEqualityComparer<UnicodeCategory>) new ExpressionLexer.UnicodeCategoryEqualityComparer())
    {
      UnicodeCategory.LetterNumber,
      UnicodeCategory.NonSpacingMark,
      UnicodeCategory.SpacingCombiningMark,
      UnicodeCategory.ConnectorPunctuation,
      UnicodeCategory.Format
    };
    private readonly bool useSemicolonDelimiter;
    private readonly bool parsingFunctionParameters;
    private bool ignoreWhitespace;

    internal ExpressionLexer(string expression, bool moveToFirstToken, bool useSemicolonDelimiter)
      : this(expression, moveToFirstToken, useSemicolonDelimiter, false)
    {
    }

    internal ExpressionLexer(
      string expression,
      bool moveToFirstToken,
      bool useSemicolonDelimiter,
      bool parsingFunctionParameters)
    {
      this.ignoreWhitespace = true;
      this.Text = expression;
      this.TextLen = this.Text.Length;
      this.useSemicolonDelimiter = useSemicolonDelimiter;
      this.parsingFunctionParameters = parsingFunctionParameters;
      this.SetTextPos(0);
      if (!moveToFirstToken)
        return;
      this.NextToken();
    }

    internal ExpressionToken CurrentToken
    {
      get => this.token;
      set => this.token = value;
    }

    internal string ExpressionText => this.Text;

    internal int Position => this.token.Position;

    protected bool IsValidWhiteSpace => this.ch.HasValue && char.IsWhiteSpace(this.ch.Value);

    private bool IsValidDigit => this.ch.HasValue && char.IsDigit(this.ch.Value);

    private bool IsValidStartingCharForIdentifier
    {
      get
      {
        if (!this.ch.HasValue)
          return false;
        if (!char.IsLetter(this.ch.Value))
        {
          char? ch = this.ch;
          int? nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
          int num1 = 95;
          if (!(nullable.GetValueOrDefault() == num1 & nullable.HasValue))
          {
            ch = this.ch;
            nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
            int num2 = 36;
            if (!(nullable.GetValueOrDefault() == num2 & nullable.HasValue))
              return Microsoft.OData.PlatformHelper.GetUnicodeCategory(this.ch.Value) == UnicodeCategory.LetterNumber;
          }
        }
        return true;
      }
    }

    private bool IsValidNonStartingCharForIdentifier
    {
      get
      {
        if (!this.ch.HasValue)
          return false;
        return char.IsLetterOrDigit(this.ch.Value) || ExpressionLexer.AdditionalUnicodeCategoriesForIdentifier.Contains(Microsoft.OData.PlatformHelper.GetUnicodeCategory(this.ch.Value));
      }
    }

    internal bool TryPeekNextToken(out ExpressionToken resultToken, out Exception error)
    {
      int textPos = this.textPos;
      char? ch = this.ch;
      ExpressionToken token = this.token;
      resultToken = this.NextTokenImplementation(out error);
      this.textPos = textPos;
      this.ch = ch;
      this.token = token;
      return error == null;
    }

    internal ExpressionToken NextToken()
    {
      Exception error = (Exception) null;
      ExpressionToken expressionToken = this.NextTokenImplementation(out error);
      if (error != null)
        throw error;
      return expressionToken;
    }

    internal string ReadDottedIdentifier(bool acceptStar)
    {
      this.ValidateToken(ExpressionTokenKind.Identifier);
      StringBuilder stringBuilder = (StringBuilder) null;
      string text = this.CurrentToken.Text;
      this.NextToken();
      while (this.CurrentToken.Kind == ExpressionTokenKind.Dot)
      {
        this.NextToken();
        if (this.CurrentToken.Kind != ExpressionTokenKind.Identifier && this.CurrentToken.Kind != ExpressionTokenKind.QuotedLiteral)
        {
          if (this.CurrentToken.Kind != ExpressionTokenKind.Star)
            throw ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_SyntaxError((object) this.textPos, (object) this.Text));
          if (!acceptStar || this.PeekNextToken().Kind != ExpressionTokenKind.End && this.PeekNextToken().Kind != ExpressionTokenKind.Comma)
            throw ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_SyntaxError((object) this.textPos, (object) this.Text));
        }
        if (stringBuilder == null)
          stringBuilder = new StringBuilder(text, text.Length + 1 + this.CurrentToken.Text.Length);
        stringBuilder.Append('.');
        stringBuilder.Append(this.CurrentToken.Text);
        this.NextToken();
      }
      if (stringBuilder != null)
        text = stringBuilder.ToString();
      return text;
    }

    internal ExpressionToken PeekNextToken()
    {
      ExpressionToken resultToken;
      Exception error;
      this.TryPeekNextToken(out resultToken, out error);
      if (error != null)
        throw error;
      return resultToken;
    }

    internal bool ExpandIdentifierAsFunction()
    {
      if (this.token.Kind != ExpressionTokenKind.Identifier)
        return false;
      int textPos = this.textPos;
      char? ch = this.ch;
      ExpressionToken token = this.token;
      bool ignoreWhitespace = this.ignoreWhitespace;
      this.ignoreWhitespace = false;
      int position = this.token.Position;
      do
        ;
      while (this.MoveNextWhenMatch(ExpressionTokenKind.Dot) && this.MoveNextWhenMatch(ExpressionTokenKind.Identifier));
      bool flag = false;
      if (this.CurrentToken.Kind == ExpressionTokenKind.Identifier)
      {
        int num;
        switch (this.PeekNextToken().Kind)
        {
          case ExpressionTokenKind.OpenParen:
            num = 1;
            break;
          case ExpressionTokenKind.ParenthesesExpression:
            num = this.CurrentToken.Text == "in" ? 1 : 0;
            break;
          default:
            num = 0;
            break;
        }
        flag = num != 0;
      }
      if (flag)
      {
        this.token.Text = this.Text.Substring(position, this.textPos - position);
        this.token.Position = position;
      }
      else
      {
        this.textPos = textPos;
        this.ch = ch;
        this.token = token;
      }
      this.ignoreWhitespace = ignoreWhitespace;
      return flag;
    }

    internal void ValidateToken(ExpressionTokenKind t)
    {
      if (this.token.Kind != t)
        throw ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_SyntaxError((object) this.textPos, (object) this.Text));
    }

    internal string AdvanceThroughExpandOption()
    {
      int textPos = this.textPos;
      while (true)
      {
        char? ch1 = this.ch;
        int? nullable1 = ch1.HasValue ? new int?((int) ch1.GetValueOrDefault()) : new int?();
        int num1 = 39;
        if (nullable1.GetValueOrDefault() == num1 & nullable1.HasValue)
          this.AdvanceToNextOccurenceOf('\'');
        char? ch2 = this.ch;
        int? nullable2 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
        int num2 = 40;
        if (nullable2.GetValueOrDefault() == num2 & nullable2.HasValue)
        {
          this.NextChar();
          this.AdvanceThroughBalancedParentheticalExpression();
        }
        else
        {
          char? ch3 = this.ch;
          int? nullable3 = ch3.HasValue ? new int?((int) ch3.GetValueOrDefault()) : new int?();
          int num3 = 59;
          if (!(nullable3.GetValueOrDefault() == num3 & nullable3.HasValue))
          {
            char? ch4 = this.ch;
            int? nullable4 = ch4.HasValue ? new int?((int) ch4.GetValueOrDefault()) : new int?();
            int num4 = 41;
            if (!(nullable4.GetValueOrDefault() == num4 & nullable4.HasValue))
            {
              if (this.ch.HasValue)
                this.NextChar();
              else
                goto label_9;
            }
            else
              break;
          }
          else
            break;
        }
      }
      string str = this.Text.Substring(textPos, this.textPos - textPos);
      goto label_11;
label_9:
      str = this.Text.Substring(textPos);
label_11:
      this.NextToken();
      return str;
    }

    internal string AdvanceThroughBalancedParentheticalExpression()
    {
      int position = this.Position;
      this.AdvanceThroughBalancedExpression('(', ')');
      return this.Text.Substring(position, this.textPos - position);
    }

    internal ExpressionLexer.ExpressionLexerPosition SnapshotPosition() => new ExpressionLexer.ExpressionLexerPosition(this, new int?(this.textPos), new ExpressionToken?(this.token));

    internal void RestorePosition(ExpressionLexer.ExpressionLexerPosition position)
    {
      if (position.TextPos.HasValue)
        this.SetTextPos(position.TextPos.Value);
      if (!position.Token.HasValue)
        return;
      this.token = position.Token.Value;
    }

    protected static Exception ParseError(string message) => (Exception) new ODataException(message);

    protected void NextChar()
    {
      if (this.textPos < this.TextLen)
      {
        ++this.textPos;
        if (this.textPos < this.TextLen)
        {
          this.ch = new char?(this.Text[this.textPos]);
          return;
        }
      }
      this.ch = new char?();
    }

    protected void ParseWhitespace()
    {
      while (this.IsValidWhiteSpace)
        this.NextChar();
    }

    protected void AdvanceToNextOccurenceOf(char endingValue)
    {
      this.NextChar();
      while (this.ch.HasValue)
      {
        char? ch = this.ch;
        int? nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
        int num = (int) endingValue;
        if (nullable.GetValueOrDefault() == num & nullable.HasValue)
          break;
        this.NextChar();
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This parser method is all about the switch statement and would be harder to maintain if it were broken up.")]
    protected virtual ExpressionToken NextTokenImplementation(out Exception error)
    {
      error = (Exception) null;
      if (this.ignoreWhitespace)
        this.ParseWhitespace();
      int textPos1 = this.textPos;
      char? ch1 = this.ch;
      ExpressionTokenKind id;
      if (ch1.HasValue)
      {
        switch (ch1.GetValueOrDefault())
        {
          case '\'':
            char endingValue = this.ch.Value;
            do
            {
              this.AdvanceToNextOccurenceOf(endingValue);
              if (this.textPos == this.TextLen)
                error = ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_UnterminatedStringLiteral((object) this.textPos, (object) this.Text));
              this.NextChar();
            }
            while (this.ch.HasValue && (int) this.ch.Value == (int) endingValue);
            id = ExpressionTokenKind.StringLiteral;
            goto label_50;
          case '(':
            if (this.CurrentToken.Text == "in")
            {
              this.NextChar();
              this.AdvanceThroughBalancedExpression('(', ')');
              id = ExpressionTokenKind.ParenthesesExpression;
              goto label_50;
            }
            else
            {
              this.NextChar();
              id = ExpressionTokenKind.OpenParen;
              goto label_50;
            }
          case ')':
            this.NextChar();
            id = ExpressionTokenKind.CloseParen;
            goto label_50;
          case '*':
            this.NextChar();
            id = ExpressionTokenKind.Star;
            goto label_50;
          case ',':
            this.NextChar();
            id = ExpressionTokenKind.Comma;
            goto label_50;
          case '-':
            bool flag = this.textPos + 1 < this.TextLen;
            if (flag && char.IsDigit(this.Text[this.textPos + 1]))
            {
              id = this.ParseFromDigit();
              if (!ExpressionLexerUtils.IsNumeric(id))
                this.SetTextPos(textPos1);
              else
                goto label_50;
            }
            else if (flag && (int) this.Text[textPos1 + 1] == (int) "INF"[0])
            {
              this.NextChar();
              this.ParseIdentifier();
              string text = this.Text.Substring(textPos1 + 1, this.textPos - textPos1 - 1);
              if (ExpressionLexerUtils.IsInfinityLiteralDouble(text))
              {
                id = ExpressionTokenKind.DoubleLiteral;
                goto label_50;
              }
              else if (ExpressionLexerUtils.IsInfinityLiteralSingle(text))
              {
                id = ExpressionTokenKind.SingleLiteral;
                goto label_50;
              }
              else
                this.SetTextPos(textPos1);
            }
            this.NextChar();
            id = ExpressionTokenKind.Minus;
            goto label_50;
          case '.':
            this.NextChar();
            id = ExpressionTokenKind.Dot;
            goto label_50;
          case '/':
            this.NextChar();
            id = ExpressionTokenKind.Slash;
            goto label_50;
          case ':':
            this.NextChar();
            id = ExpressionTokenKind.Colon;
            goto label_50;
          case '=':
            this.NextChar();
            id = ExpressionTokenKind.Equal;
            goto label_50;
          case '?':
            this.NextChar();
            id = ExpressionTokenKind.Question;
            goto label_50;
          case '[':
            this.NextChar();
            this.AdvanceThroughBalancedExpression('[', ']');
            id = ExpressionTokenKind.BracketedExpression;
            goto label_50;
          case '{':
            this.NextChar();
            this.AdvanceThroughBalancedExpression('{', '}');
            id = ExpressionTokenKind.BracedExpression;
            goto label_50;
        }
      }
      if (this.IsValidWhiteSpace)
      {
        this.ParseWhitespace();
        id = ExpressionTokenKind.Unknown;
      }
      else if (this.IsValidStartingCharForIdentifier)
      {
        this.ParseIdentifier();
        char? ch2 = this.ch;
        int? nullable = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
        int num = 45;
        id = !(nullable.GetValueOrDefault() == num & nullable.HasValue) || !this.TryParseGuid(textPos1) ? ExpressionTokenKind.Identifier : ExpressionTokenKind.GuidLiteral;
      }
      else if (this.IsValidDigit)
        id = this.ParseFromDigit();
      else if (this.textPos == this.TextLen)
      {
        id = ExpressionTokenKind.End;
      }
      else
      {
        int? nullable;
        if (this.useSemicolonDelimiter)
        {
          char? ch3 = this.ch;
          nullable = ch3.HasValue ? new int?((int) ch3.GetValueOrDefault()) : new int?();
          int num = 59;
          if (nullable.GetValueOrDefault() == num & nullable.HasValue)
          {
            this.NextChar();
            id = ExpressionTokenKind.SemiColon;
            goto label_50;
          }
        }
        char? ch4 = this.ch;
        nullable = ch4.HasValue ? new int?((int) ch4.GetValueOrDefault()) : new int?();
        int num1 = 64;
        if (nullable.GetValueOrDefault() == num1 & nullable.HasValue)
        {
          this.NextChar();
          if (this.textPos == this.TextLen)
          {
            error = ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_SyntaxError((object) this.textPos, (object) this.Text));
            id = ExpressionTokenKind.Unknown;
          }
          else if (!this.IsValidStartingCharForIdentifier)
          {
            error = ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_InvalidCharacter((object) this.ch, (object) this.textPos, (object) this.Text));
            id = ExpressionTokenKind.Unknown;
          }
          else
          {
            int textPos2 = this.textPos;
            this.ParseIdentifier(true);
            string str = this.ExpressionText.Substring(textPos2, this.textPos - textPos2);
            id = !this.parsingFunctionParameters || str.Contains(".") ? ExpressionTokenKind.Identifier : ExpressionTokenKind.ParameterAlias;
          }
        }
        else
        {
          error = ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_InvalidCharacter((object) this.ch, (object) this.textPos, (object) this.Text));
          id = ExpressionTokenKind.Unknown;
        }
      }
label_50:
      this.token.Kind = id;
      this.token.Text = this.Text.Substring(textPos1, this.textPos - textPos1);
      this.token.Position = textPos1;
      this.HandleTypePrefixedLiterals();
      return this.token;
    }

    private bool MoveNextWhenMatch(ExpressionTokenKind id)
    {
      ExpressionToken expressionToken = this.PeekNextToken();
      if (id != expressionToken.Kind)
        return false;
      this.NextToken();
      return true;
    }

    private void HandleTypePrefixedLiterals()
    {
      if (this.token.Kind != ExpressionTokenKind.Identifier)
        return;
      char? ch = this.ch;
      int? nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
      int num = 39;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
      {
        IEdmTypeReference customLiteralPrefix = CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(this.token.Text);
        if (customLiteralPrefix != null)
          this.token.SetCustomEdmTypeLiteral(customLiteralPrefix);
        else
          this.token.Kind = this.GetBuiltInTypesLiteralPrefixWithQuotedValue(this.token.Text);
        this.HandleQuotedValues();
      }
      else
      {
        ExpressionTokenKind? typesLiteralPrefix = ExpressionLexer.GetBuiltInTypesLiteralPrefix(this.token.Text);
        if (!typesLiteralPrefix.HasValue)
          return;
        this.token.Kind = typesLiteralPrefix.Value;
      }
    }

    private void HandleQuotedValues()
    {
      int position = this.token.Position;
      int? nullable;
      int num1;
      do
      {
        this.NextChar();
        char? ch;
        if (this.ch.HasValue)
        {
          ch = this.ch;
          nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
          int num2 = 39;
          if (!(nullable.GetValueOrDefault() == num2 & nullable.HasValue))
            continue;
        }
        if (!this.ch.HasValue)
          throw ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_UnterminatedLiteral((object) this.textPos, (object) this.Text));
        this.NextChar();
        if (this.ch.HasValue)
        {
          ch = this.ch;
          nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
          num1 = 39;
        }
        else
          break;
      }
      while (nullable.GetValueOrDefault() == num1 & nullable.HasValue);
      this.token.Text = this.Text.Substring(position, this.textPos - position);
    }

    private static ExpressionTokenKind? GetBuiltInTypesLiteralPrefix(string tokenText)
    {
      if (ExpressionLexerUtils.IsInfinityOrNaNDouble(tokenText))
        return new ExpressionTokenKind?(ExpressionTokenKind.DoubleLiteral);
      if (ExpressionLexerUtils.IsInfinityOrNanSingle(tokenText))
        return new ExpressionTokenKind?(ExpressionTokenKind.SingleLiteral);
      switch (tokenText)
      {
        case "true":
        case "false":
          return new ExpressionTokenKind?(ExpressionTokenKind.BooleanLiteral);
        case "null":
          return new ExpressionTokenKind?(ExpressionTokenKind.NullLiteral);
        default:
          return new ExpressionTokenKind?();
      }
    }

    private ExpressionTokenKind GetBuiltInTypesLiteralPrefixWithQuotedValue(string tokenText)
    {
      if (string.Equals(tokenText, "duration", StringComparison.OrdinalIgnoreCase))
        return ExpressionTokenKind.DurationLiteral;
      if (string.Equals(tokenText, "binary", StringComparison.OrdinalIgnoreCase))
        return ExpressionTokenKind.BinaryLiteral;
      if (string.Equals(tokenText, "geography", StringComparison.OrdinalIgnoreCase))
        return ExpressionTokenKind.GeographyLiteral;
      if (string.Equals(tokenText, "geometry", StringComparison.OrdinalIgnoreCase))
        return ExpressionTokenKind.GeometryLiteral;
      if (string.Equals(tokenText, "null", StringComparison.OrdinalIgnoreCase))
        throw ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_SyntaxError((object) this.textPos, (object) this.Text));
      return ExpressionTokenKind.QuotedLiteral;
    }

    private ExpressionTokenKind ParseFromDigit()
    {
      int textPos = this.textPos;
      char ch1 = this.ch.Value;
      this.NextChar();
      char? ch2;
      ExpressionTokenKind fromDigit;
      if (ch1 == '0')
      {
        ch2 = this.ch;
        int? nullable1 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
        int num1 = 120;
        if (!(nullable1.GetValueOrDefault() == num1 & nullable1.HasValue))
        {
          ch2 = this.ch;
          int? nullable2 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
          int num2 = 88;
          if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
            goto label_5;
        }
        fromDigit = ExpressionTokenKind.BinaryLiteral;
        do
        {
          this.NextChar();
        }
        while (this.ch.HasValue && UriParserHelper.IsCharHexDigit(this.ch.Value));
        goto label_42;
      }
label_5:
      ExpressionTokenKind guessedKind = ExpressionTokenKind.IntegerLiteral;
      while (this.IsValidDigit)
        this.NextChar();
      ch2 = this.ch;
      int? nullable3 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
      int num3 = 45;
      if (nullable3.GetValueOrDefault() == num3 & nullable3.HasValue)
      {
        if (this.TryParseDate(textPos))
          return ExpressionTokenKind.DateLiteral;
        if (this.TryParseDateTimeoffset(textPos))
          return ExpressionTokenKind.DateTimeOffsetLiteral;
        if (this.TryParseGuid(textPos))
          return ExpressionTokenKind.GuidLiteral;
      }
      ch2 = this.ch;
      int? nullable4 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
      int num4 = 58;
      if (nullable4.GetValueOrDefault() == num4 & nullable4.HasValue && this.TryParseTimeOfDay(textPos))
        return ExpressionTokenKind.TimeOfDayLiteral;
      if (this.ch.HasValue && char.IsLetter(this.ch.Value) && this.TryParseGuid(textPos))
        return ExpressionTokenKind.GuidLiteral;
      ch2 = this.ch;
      int? nullable5 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
      int num5 = 46;
      if (nullable5.GetValueOrDefault() == num5 & nullable5.HasValue)
      {
        guessedKind = ExpressionTokenKind.DoubleLiteral;
        this.NextChar();
        this.ValidateDigit();
        do
        {
          this.NextChar();
        }
        while (this.IsValidDigit);
      }
      ch2 = this.ch;
      int? nullable6 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
      int num6 = 69;
      if (!(nullable6.GetValueOrDefault() == num6 & nullable6.HasValue))
      {
        ch2 = this.ch;
        int? nullable7 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
        int num7 = 101;
        if (!(nullable7.GetValueOrDefault() == num7 & nullable7.HasValue))
          goto label_29;
      }
      guessedKind = ExpressionTokenKind.DoubleLiteral;
      this.NextChar();
      ch2 = this.ch;
      int? nullable8 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
      int num8 = 43;
      if (!(nullable8.GetValueOrDefault() == num8 & nullable8.HasValue))
      {
        ch2 = this.ch;
        int? nullable9 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
        int num9 = 45;
        if (!(nullable9.GetValueOrDefault() == num9 & nullable9.HasValue))
          goto label_27;
      }
      this.NextChar();
label_27:
      this.ValidateDigit();
      do
      {
        this.NextChar();
      }
      while (this.IsValidDigit);
label_29:
      ch2 = this.ch;
      int? nullable10 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
      int num10 = 77;
      if (!(nullable10.GetValueOrDefault() == num10 & nullable10.HasValue))
      {
        ch2 = this.ch;
        int? nullable11 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
        int num11 = 109;
        if (!(nullable11.GetValueOrDefault() == num11 & nullable11.HasValue))
        {
          ch2 = this.ch;
          int? nullable12 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
          int num12 = 100;
          if (!(nullable12.GetValueOrDefault() == num12 & nullable12.HasValue))
          {
            ch2 = this.ch;
            int? nullable13 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
            int num13 = 68;
            if (!(nullable13.GetValueOrDefault() == num13 & nullable13.HasValue))
            {
              ch2 = this.ch;
              int? nullable14 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
              int num14 = 76;
              if (!(nullable14.GetValueOrDefault() == num14 & nullable14.HasValue))
              {
                ch2 = this.ch;
                int? nullable15 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
                int num15 = 108;
                if (!(nullable15.GetValueOrDefault() == num15 & nullable15.HasValue))
                {
                  ch2 = this.ch;
                  int? nullable16 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
                  int num16 = 102;
                  if (!(nullable16.GetValueOrDefault() == num16 & nullable16.HasValue))
                  {
                    ch2 = this.ch;
                    int? nullable17 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
                    int num17 = 70;
                    if (!(nullable17.GetValueOrDefault() == num17 & nullable17.HasValue))
                    {
                      fromDigit = ExpressionLexer.MakeBestGuessOnNoSuffixStr(this.Text.Substring(textPos, this.textPos - textPos), guessedKind);
                      goto label_42;
                    }
                  }
                  fromDigit = ExpressionTokenKind.SingleLiteral;
                  this.NextChar();
                  goto label_42;
                }
              }
              fromDigit = ExpressionTokenKind.Int64Literal;
              this.NextChar();
              goto label_42;
            }
          }
          fromDigit = ExpressionTokenKind.DoubleLiteral;
          this.NextChar();
          goto label_42;
        }
      }
      fromDigit = ExpressionTokenKind.DecimalLiteral;
      this.NextChar();
label_42:
      return fromDigit;
    }

    private bool TryParseGuid(int tokenPos)
    {
      int textPos = this.textPos;
      if (UriUtils.TryUriStringToGuid(this.ParseLiteral(tokenPos), out Guid _))
        return true;
      this.textPos = textPos;
      this.ch = new char?(this.Text[textPos]);
      return false;
    }

    private bool TryParseDateTimeoffset(int tokenPos)
    {
      int textPos = this.textPos;
      if (UriUtils.ConvertUriStringToDateTimeOffset(this.ParseLiteral(tokenPos), out DateTimeOffset _))
        return true;
      this.textPos = textPos;
      this.ch = new char?(this.Text[textPos]);
      return false;
    }

    private bool TryParseDate(int tokenPos)
    {
      int textPos = this.textPos;
      if (UriUtils.TryUriStringToDate(this.ParseLiteral(tokenPos), out Date _))
        return true;
      this.textPos = textPos;
      this.ch = new char?(this.Text[textPos]);
      return false;
    }

    private bool TryParseTimeOfDay(int tokenPos)
    {
      int textPos = this.textPos;
      if (UriUtils.TryUriStringToTimeOfDay(this.ParseLiteral(tokenPos), out TimeOfDay _))
        return true;
      this.textPos = textPos;
      this.ch = new char?(this.Text[textPos]);
      return false;
    }

    private string ParseLiteral(int tokenPos)
    {
      int? nullable1;
      int num1;
      do
      {
        this.NextChar();
        if (this.ch.HasValue)
        {
          char? ch1 = this.ch;
          int? nullable2 = ch1.HasValue ? new int?((int) ch1.GetValueOrDefault()) : new int?();
          int num2 = 44;
          if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
          {
            char? ch2 = this.ch;
            int? nullable3 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
            int num3 = 41;
            if (!(nullable3.GetValueOrDefault() == num3 & nullable3.HasValue))
            {
              char? ch3 = this.ch;
              nullable1 = ch3.HasValue ? new int?((int) ch3.GetValueOrDefault()) : new int?();
              num1 = 32;
            }
            else
              break;
          }
          else
            break;
        }
        else
          break;
      }
      while (!(nullable1.GetValueOrDefault() == num1 & nullable1.HasValue));
      if (!this.ch.HasValue)
        this.NextChar();
      return this.Text.Substring(tokenPos, this.textPos - tokenPos);
    }

    private static ExpressionTokenKind MakeBestGuessOnNoSuffixStr(
      string numericStr,
      ExpressionTokenKind guessedKind)
    {
      int result1 = 0;
      long result2 = 0;
      float result3 = 0.0f;
      double result4 = 0.0;
      Decimal result5 = 0M;
      if (guessedKind == ExpressionTokenKind.IntegerLiteral)
      {
        if (int.TryParse(numericStr, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
          return ExpressionTokenKind.IntegerLiteral;
        if (long.TryParse(numericStr, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          return ExpressionTokenKind.Int64Literal;
      }
      bool flag1 = float.TryParse(numericStr, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3);
      bool flag2 = double.TryParse(numericStr, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result4);
      bool flag3 = Decimal.TryParse(numericStr, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, (IFormatProvider) CultureInfo.InvariantCulture, out result5);
      if (flag2 & flag3)
      {
        Decimal result6;
        bool flag4 = Decimal.TryParse(result4.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result6);
        Decimal result7;
        bool flag5 = Decimal.TryParse(result4.ToString("N29", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result7);
        if (flag4 && result6 != result5 || !flag4 & flag5 && result7 != result5)
          return ExpressionTokenKind.DecimalLiteral;
      }
      if (flag1 & flag2 && double.Parse(result3.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture) != result4)
        return ExpressionTokenKind.DoubleLiteral;
      if (flag1)
        return ExpressionTokenKind.SingleLiteral;
      if (flag2)
        return ExpressionTokenKind.DoubleLiteral;
      throw new ODataException(Microsoft.OData.Strings.ExpressionLexer_InvalidNumericString((object) numericStr));
    }

    private void AdvanceThroughBalancedExpression(char startingCharacter, char endingCharacter)
    {
      int num1 = 1;
      while (num1 > 0)
      {
        char? ch1 = this.ch;
        int? nullable1 = ch1.HasValue ? new int?((int) ch1.GetValueOrDefault()) : new int?();
        int num2 = 39;
        if (nullable1.GetValueOrDefault() == num2 & nullable1.HasValue)
          this.AdvanceToNextOccurenceOf('\'');
        char? ch2 = this.ch;
        int? nullable2 = ch2.HasValue ? new int?((int) ch2.GetValueOrDefault()) : new int?();
        int num3 = (int) startingCharacter;
        if (nullable2.GetValueOrDefault() == num3 & nullable2.HasValue)
        {
          ++num1;
        }
        else
        {
          char? ch3 = this.ch;
          int? nullable3 = ch3.HasValue ? new int?((int) ch3.GetValueOrDefault()) : new int?();
          int num4 = (int) endingCharacter;
          if (nullable3.GetValueOrDefault() == num4 & nullable3.HasValue)
            --num1;
        }
        if (!this.ch.HasValue)
          throw new ODataException(Microsoft.OData.Strings.ExpressionLexer_UnbalancedBracketExpression);
        this.NextChar();
      }
    }

    private void ParseIdentifier(bool includingDots = false)
    {
      int? nullable;
      int num;
      do
      {
        do
        {
          this.NextChar();
        }
        while (this.IsValidNonStartingCharForIdentifier);
        if (includingDots)
        {
          char? ch = this.ch;
          nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
          num = 46;
        }
        else
          goto label_3;
      }
      while (nullable.GetValueOrDefault() == num & nullable.HasValue);
      goto label_4;
label_3:
      return;
label_4:;
    }

    private void SetTextPos(int pos)
    {
      this.textPos = pos;
      this.ch = this.textPos < this.TextLen ? new char?(this.Text[this.textPos]) : new char?();
    }

    private void ValidateDigit()
    {
      if (!this.IsValidDigit)
        throw ExpressionLexer.ParseError(Microsoft.OData.Strings.ExpressionLexer_DigitExpected((object) this.textPos, (object) this.Text));
    }

    internal class ExpressionLexerPosition
    {
      public ExpressionLexerPosition(ExpressionLexer lexer, int? textPos, ExpressionToken? token)
      {
        this.Lexer = lexer;
        this.TextPos = textPos;
        this.Token = token;
      }

      public ExpressionLexer Lexer { get; private set; }

      public int? TextPos { get; private set; }

      public ExpressionToken? Token { get; private set; }
    }

    private sealed class UnicodeCategoryEqualityComparer : IEqualityComparer<UnicodeCategory>
    {
      public bool Equals(UnicodeCategory x, UnicodeCategory y) => x == y;

      public int GetHashCode(UnicodeCategory obj) => (int) obj;
    }
  }
}
