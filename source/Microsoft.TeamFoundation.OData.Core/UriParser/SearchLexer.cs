// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SearchLexer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.OData.UriParser
{
  [DebuggerDisplay("SearchLexer ({text} @ {textPos} [{token}])")]
  internal sealed class SearchLexer : ExpressionLexer
  {
    internal static readonly Regex InvalidWordPattern = new Regex("([^\\p{L}\\p{Nl}])");
    private const char EscapeChar = '\\';
    private const string EscapeSequenceSet = "\\\"";
    private static readonly HashSet<string> KeyWords = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal)
    {
      "AND",
      "OR",
      "NOT"
    };
    private bool isEscape;

    internal SearchLexer(string expression)
      : base(expression, true, false)
    {
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This parser method is all about the switch statement and would be harder to maintain if it were broken up.")]
    protected override ExpressionToken NextTokenImplementation(out Exception error)
    {
      error = (Exception) null;
      this.ParseWhitespace();
      int textPos = this.textPos;
      char? ch = this.ch;
      ExpressionTokenKind expressionTokenKind;
      if (ch.HasValue)
      {
        switch (ch.GetValueOrDefault())
        {
          case '"':
            this.AdvanceToNextOccurenceOfWithEscape(this.ch.Value);
            if (this.textPos == this.TextLen)
              throw ExpressionLexer.ParseError(Strings.ExpressionLexer_UnterminatedStringLiteral((object) this.textPos, (object) this.Text));
            this.NextChar();
            expressionTokenKind = ExpressionTokenKind.StringLiteral;
            goto label_11;
          case '(':
            this.NextChar();
            expressionTokenKind = ExpressionTokenKind.OpenParen;
            goto label_11;
          case ')':
            this.NextChar();
            expressionTokenKind = ExpressionTokenKind.CloseParen;
            goto label_11;
        }
      }
      if (this.textPos == this.TextLen)
      {
        expressionTokenKind = ExpressionTokenKind.End;
      }
      else
      {
        expressionTokenKind = ExpressionTokenKind.Identifier;
        do
        {
          this.NextChar();
        }
        while (this.ch.HasValue && SearchLexer.IsValidSearchTermChar(this.ch.Value));
      }
label_11:
      this.token.Kind = expressionTokenKind;
      this.token.Text = this.Text.Substring(textPos, this.textPos - textPos);
      this.token.Position = textPos;
      if (this.token.Kind == ExpressionTokenKind.StringLiteral)
      {
        this.token.Text = this.token.Text.Substring(1, this.token.Text.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
        if (string.IsNullOrEmpty(this.token.Text))
          throw ExpressionLexer.ParseError(Strings.ExpressionToken_IdentifierExpected((object) this.token.Position));
      }
      if (this.token.Kind == ExpressionTokenKind.Identifier && !SearchLexer.KeyWords.Contains(this.token.Text))
      {
        Match match = SearchLexer.InvalidWordPattern.Match(this.token.Text);
        if (match.Success)
        {
          int index = match.Groups[0].Index;
          throw ExpressionLexer.ParseError(Strings.ExpressionLexer_InvalidCharacter((object) this.token.Text[index], (object) (this.token.Position + index), (object) this.Text));
        }
        this.token.Kind = ExpressionTokenKind.StringLiteral;
      }
      return this.token;
    }

    private static bool IsValidSearchTermChar(char val) => !char.IsWhiteSpace(val) && val != ')';

    private void NextCharWithEscape()
    {
      this.isEscape = false;
      this.NextChar();
      char? ch = this.ch;
      int? nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
      int num = 92;
      if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
        return;
      this.isEscape = true;
      this.NextChar();
      if (!this.ch.HasValue || "\\\"".IndexOf(this.ch.Value) < 0)
        throw ExpressionLexer.ParseError(Strings.ExpressionLexer_InvalidEscapeSequence((object) this.ch, (object) this.textPos, (object) this.Text));
    }

    private void AdvanceToNextOccurenceOfWithEscape(char endingValue)
    {
      this.NextCharWithEscape();
      while (this.ch.HasValue)
      {
        char? ch = this.ch;
        int? nullable = ch.HasValue ? new int?((int) ch.GetValueOrDefault()) : new int?();
        int num = (int) endingValue;
        if (nullable.GetValueOrDefault() == num & nullable.HasValue && !this.isEscape)
          break;
        this.NextCharWithEscape();
      }
    }
  }
}
