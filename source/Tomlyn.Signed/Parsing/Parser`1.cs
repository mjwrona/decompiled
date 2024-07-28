// Decompiled with JetBrains decompiler
// Type: Tomlyn.Parsing.Parser`1
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using Tomlyn.Syntax;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Parsing
{
  internal class Parser<TSourceView> where TSourceView : ISourceView
  {
    private readonly ITokenProvider<TSourceView> _lexer;
    private SyntaxTokenValue _previousToken;
    private SyntaxTokenValue _token;
    private bool _hideNewLine;
    private readonly List<SyntaxTrivia> _currentTrivias;
    private TableSyntaxBase? _currentTable;
    private DiagnosticsBag? _diagnostics;

    public Parser(ITokenProvider<TSourceView> lexer)
    {
      this._lexer = lexer;
      this._currentTrivias = new List<SyntaxTrivia>();
    }

    private SourceSpan CurrentSpan => this.GetSpanForToken(this._token);

    public DocumentSyntax Run()
    {
      DocumentSyntax syntax = new DocumentSyntax();
      this._diagnostics = syntax.Diagnostics;
      this._currentTable = (TableSyntaxBase) null;
      this._hideNewLine = true;
      this.NextToken();
      SyntaxNode nextEntry;
      while (this.TryParseTableEntry(out nextEntry))
      {
        if (nextEntry != null)
        {
          if (nextEntry is TableSyntaxBase node)
          {
            this._currentTable = node;
            Parser<TSourceView>.AddToListAndUpdateSpan<TableSyntaxBase>(syntax.Tables, node);
          }
          else if (this._currentTable == null)
            Parser<TSourceView>.AddToListAndUpdateSpan<KeyValueSyntax>(syntax.KeyValues, (KeyValueSyntax) nextEntry);
          else
            Parser<TSourceView>.AddToListAndUpdateSpan<KeyValueSyntax>(this._currentTable.Items, (KeyValueSyntax) nextEntry);
        }
      }
      if (this._currentTable != null)
      {
        this.Close<TableSyntaxBase>(this._currentTable);
        this._currentTable = (TableSyntaxBase) null;
      }
      this.Close<DocumentSyntax>(syntax);
      if (this._lexer.HasErrors)
      {
        foreach (DiagnosticMessage error in this._lexer.Errors)
          this.Log(error);
      }
      return syntax;
    }

    private static void AddToListAndUpdateSpan<TSyntaxNode>(
      SyntaxList<TSyntaxNode> list,
      TSyntaxNode node)
      where TSyntaxNode : SyntaxNode
    {
      if (list.ChildrenCount == 0)
      {
        list.Span.FileName = node.Span.FileName;
        list.Span.Start = node.Span.Start;
      }
      else
        list.Span.End = node.Span.End;
      list.Add(node);
    }

    private bool TryParseTableEntry(out SyntaxNode? nextEntry)
    {
      nextEntry = (SyntaxNode) null;
      while (true)
      {
        switch (this._token.Kind)
        {
          case TokenKind.Eof:
            goto label_2;
          case TokenKind.String:
          case TokenKind.StringLiteral:
          case TokenKind.BasicKey:
            goto label_3;
          case TokenKind.OpenBracket:
          case TokenKind.OpenBracketDouble:
            goto label_4;
          default:
            this.LogError("Unexpected token [" + this.ToPrintable(this._token) + "] found");
            this.NextToken();
            continue;
        }
      }
label_2:
      return false;
label_3:
      nextEntry = (SyntaxNode) this.ParseKeyValue(true);
      return true;
label_4:
      nextEntry = (SyntaxNode) this.ParseTableOrTableArray();
      return true;
    }

    private KeyValueSyntax ParseKeyValue(bool expectEndOfLine)
    {
      bool hideNewLine = this._hideNewLine;
      this._hideNewLine = false;
      try
      {
        KeyValueSyntax syntax = this.Open<KeyValueSyntax>();
        syntax.Key = this.ParseKey();
        if (this._token.Kind != TokenKind.Equal)
        {
          this.LogError("Expecting `=` after a key instead of " + this.ToPrintable(this._token));
          this.Close<KeyValueSyntax>(syntax);
          this.SkipAfterEndOfLine();
        }
        else
        {
          this._lexer.State = LexerState.Value;
          try
          {
            syntax.EqualToken = this.EatToken();
            syntax.Value = this.ParseValue();
          }
          finally
          {
            this._lexer.State = LexerState.Key;
          }
          if (expectEndOfLine && this._token.Kind != TokenKind.Eof)
            syntax.EndOfLineToken = this.EatToken(TokenKind.NewLine);
          this.Close<KeyValueSyntax>(syntax);
        }
        return syntax;
      }
      finally
      {
        this._hideNewLine = hideNewLine;
      }
    }

    private ValueSyntax? ParseValue()
    {
      switch (this._token.Kind)
      {
        case TokenKind.NewLine:
          this.LogError("Unexpected token end-of-line found while expecting a value");
          break;
        case TokenKind.OffsetDateTimeByZ:
        case TokenKind.OffsetDateTimeByNumber:
        case TokenKind.LocalDateTime:
        case TokenKind.LocalDate:
        case TokenKind.LocalTime:
          return (ValueSyntax) this.ParseDateTime();
        case TokenKind.Integer:
        case TokenKind.IntegerHexa:
        case TokenKind.IntegerOctal:
        case TokenKind.IntegerBinary:
          return (ValueSyntax) this.ParseInteger();
        case TokenKind.Float:
        case TokenKind.Infinite:
        case TokenKind.PositiveInfinite:
        case TokenKind.NegativeInfinite:
        case TokenKind.Nan:
        case TokenKind.PositiveNan:
        case TokenKind.NegativeNan:
          return (ValueSyntax) this.ParseFloat(this._token.Kind);
        case TokenKind.String:
        case TokenKind.StringMulti:
        case TokenKind.StringLiteral:
        case TokenKind.StringLiteralMulti:
          return (ValueSyntax) this.ParseString();
        case TokenKind.OpenBracket:
          return (ValueSyntax) this.ParseArray();
        case TokenKind.OpenBrace:
          return (ValueSyntax) this.ParseInlineTable();
        case TokenKind.True:
        case TokenKind.False:
          return (ValueSyntax) this.ParseBoolean();
        default:
          this.LogError("Unexpected token `" + this.ToPrintable(this._token) + "` for a value");
          this.NextToken();
          break;
      }
      return (ValueSyntax) null;
    }

    private bool IsCurrentValue()
    {
      switch (this._token.Kind)
      {
        case TokenKind.OffsetDateTimeByZ:
        case TokenKind.OffsetDateTimeByNumber:
        case TokenKind.LocalDateTime:
        case TokenKind.LocalDate:
        case TokenKind.LocalTime:
        case TokenKind.Integer:
        case TokenKind.IntegerHexa:
        case TokenKind.IntegerOctal:
        case TokenKind.IntegerBinary:
        case TokenKind.Float:
        case TokenKind.String:
        case TokenKind.StringMulti:
        case TokenKind.StringLiteral:
        case TokenKind.StringLiteralMulti:
        case TokenKind.OpenBracket:
        case TokenKind.OpenBrace:
        case TokenKind.True:
        case TokenKind.False:
        case TokenKind.Infinite:
        case TokenKind.PositiveInfinite:
        case TokenKind.NegativeInfinite:
          return true;
        default:
          return false;
      }
    }

    private BooleanValueSyntax ParseBoolean()
    {
      BooleanValueSyntax syntax = this.Open<BooleanValueSyntax>();
      syntax.Value = (bool) (this._token.Value ?? (object) false);
      syntax.Token = this.EatToken();
      return this.Close<BooleanValueSyntax>(syntax);
    }

    private DateTimeValueSyntax ParseDateTime()
    {
      DateTimeValueSyntax syntax;
      switch (this._token.Kind)
      {
        case TokenKind.OffsetDateTimeByZ:
          syntax = this.Open<DateTimeValueSyntax>(new DateTimeValueSyntax(SyntaxKind.OffsetDateTimeByZ));
          break;
        case TokenKind.OffsetDateTimeByNumber:
          syntax = this.Open<DateTimeValueSyntax>(new DateTimeValueSyntax(SyntaxKind.OffsetDateTimeByNumber));
          break;
        case TokenKind.LocalDateTime:
          syntax = this.Open<DateTimeValueSyntax>(new DateTimeValueSyntax(SyntaxKind.LocalDateTime));
          break;
        case TokenKind.LocalDate:
          syntax = this.Open<DateTimeValueSyntax>(new DateTimeValueSyntax(SyntaxKind.LocalDate));
          break;
        case TokenKind.LocalTime:
          syntax = this.Open<DateTimeValueSyntax>(new DateTimeValueSyntax(SyntaxKind.LocalTime));
          break;
        default:
          throw new InvalidOperationException("The datetime kind `{_token.Kind}` is not supported");
      }
      syntax.Value = (TomlDateTime) (this._token.Value ?? (object) new TomlDateTime());
      syntax.Token = this.EatToken();
      return this.Close<DateTimeValueSyntax>(syntax);
    }

    private IntegerValueSyntax ParseInteger()
    {
      IntegerValueSyntax syntax = this.Open<IntegerValueSyntax>();
      syntax.Value = (long) (this._token.Value ?? (object) 0L);
      syntax.Token = this.EatToken();
      return this.Close<IntegerValueSyntax>(syntax);
    }

    private FloatValueSyntax ParseFloat(TokenKind kind)
    {
      FloatValueSyntax syntax = this.Open<FloatValueSyntax>();
      syntax.Value = (double) (this._token.Value ?? (object) 0.0);
      syntax.Token = this.EatToken();
      return this.Close<FloatValueSyntax>(syntax);
    }

    private ArraySyntax ParseArray()
    {
      ArraySyntax syntax = this.Open<ArraySyntax>();
      bool hideNewLine = this._hideNewLine;
      this._hideNewLine = true;
      syntax.OpenBracket = this.EatToken(TokenKind.OpenBracket);
      try
      {
        bool flag = false;
        while (this._token.Kind != TokenKind.CloseBracket)
        {
          if (!flag)
          {
            ArrayItemSyntax arrayItemSyntax = this.Open<ArrayItemSyntax>();
            arrayItemSyntax.Value = this.ParseValue();
            if (this._token.Kind == TokenKind.Comma)
              arrayItemSyntax.Comma = this.EatToken();
            else if (this.IsCurrentValue())
              this.LogError("Missing a `,` (token: comma) to separate items in an array");
            else
              flag = true;
            this.Close<ArrayItemSyntax>(arrayItemSyntax);
            Parser<TSourceView>.AddToListAndUpdateSpan<ArrayItemSyntax>(syntax.Items, arrayItemSyntax);
          }
          else
          {
            this.LogError(string.Format("Unexpected token `{0}` (token: `{1}`). Expecting a closing `]` for an array", (object) this.ToPrintable(this._token), (object) this._token.Kind));
            goto label_13;
          }
        }
        this._hideNewLine = hideNewLine;
        syntax.CloseBracket = this.EatToken();
      }
      finally
      {
        this._hideNewLine = hideNewLine;
      }
label_13:
      return this.Close<ArraySyntax>(syntax);
    }

    private InlineTableSyntax ParseInlineTable()
    {
      InlineTableSyntax syntax = this.Open<InlineTableSyntax>();
      LexerState state = this._lexer.State;
      this._lexer.State = LexerState.Key;
      bool hideNewLine = this._hideNewLine;
      this._hideNewLine = false;
      syntax.OpenBrace = this.EatToken(TokenKind.OpenBrace);
      try
      {
        bool? nullable = new bool?();
        SourceSpan span = syntax.OpenBrace.Span;
        while (this._token.Kind != TokenKind.CloseBrace)
        {
          if ((!nullable.HasValue || !nullable.Value) && (this._token.Kind == TokenKind.BasicKey || this._token.Kind == TokenKind.String || this._token.Kind == TokenKind.StringLiteral))
          {
            InlineTableItemSyntax inlineTableItemSyntax = this.Open<InlineTableItemSyntax>();
            inlineTableItemSyntax.KeyValue = this.ParseKeyValue(false);
            if (this._token.Kind == TokenKind.Comma)
            {
              inlineTableItemSyntax.Comma = this.EatToken();
              nullable = new bool?(false);
            }
            else
              nullable = new bool?(true);
            this.Close<InlineTableItemSyntax>(inlineTableItemSyntax);
            Parser<TSourceView>.AddToListAndUpdateSpan<InlineTableItemSyntax>(syntax.Items, inlineTableItemSyntax);
            SyntaxToken comma = inlineTableItemSyntax.Comma;
            span = comma != null ? comma.Span : inlineTableItemSyntax.KeyValue.Span;
          }
          else
          {
            this.LogError(string.Format("Unexpected token `{0}` while parsing inline table. Expecting a bare key or string instead of `{1}`", (object) this._token.Kind, (object) this.ToPrintable(this._token)));
            goto label_13;
          }
        }
        if (nullable.HasValue && !nullable.Value)
          this.LogError(span, "Unexpected trailing comma `,` not permitted after the last key/value pair in an inline table.");
        this._hideNewLine = hideNewLine;
        this._lexer.State = state;
        syntax.CloseBrace = this.EatToken();
      }
      finally
      {
        this._lexer.State = state;
        this._hideNewLine = hideNewLine;
      }
label_13:
      return this.Close<InlineTableSyntax>(syntax);
    }

    private TableSyntaxBase ParseTableOrTableArray()
    {
      if (this._currentTable != null)
        this.Close<TableSyntaxBase>(this._currentTable);
      bool flag = this._token.Kind == TokenKind.OpenBracketDouble;
      bool hideNewLine = this._hideNewLine;
      this._hideNewLine = false;
      TableSyntaxBase tableOrTableArray = flag ? (TableSyntaxBase) this.Open<TableArraySyntax>() : (TableSyntaxBase) this.Open<TableSyntax>();
      try
      {
        tableOrTableArray.OpenBracket = this.EatToken();
        tableOrTableArray.Name = this.ParseKey();
        tableOrTableArray.CloseBracket = this.EatToken(flag ? TokenKind.CloseBracketDouble : TokenKind.CloseBracket);
        if (this._token.Kind != TokenKind.Eof)
          tableOrTableArray.EndOfLineToken = this.EatToken(TokenKind.NewLine);
      }
      finally
      {
        this._hideNewLine = hideNewLine;
      }
      return tableOrTableArray;
    }

    private KeySyntax ParseKey()
    {
      KeySyntax syntax = this.Open<KeySyntax>();
      syntax.Key = this.ParseBaseKey();
      while (this._token.Kind == TokenKind.Dot)
        Parser<TSourceView>.AddToListAndUpdateSpan<DottedKeyItemSyntax>(syntax.DotKeys, this.ParseDotKey());
      return this.Close<KeySyntax>(syntax);
    }

    private BareKeyOrStringValueSyntax? ParseBaseKey()
    {
      if (this._token.Kind == TokenKind.BasicKey)
        return (BareKeyOrStringValueSyntax) this.ParseBasicKey();
      if (this._token.Kind == TokenKind.String || this._token.Kind == TokenKind.StringLiteral)
        return (BareKeyOrStringValueSyntax) this.ParseString();
      this.LogError("Unexpected token `" + this.ToPrintable(this._token) + "` for a base key");
      this.NextToken();
      return (BareKeyOrStringValueSyntax) null;
    }

    private StringValueSyntax ParseString()
    {
      StringValueSyntax syntax = this.Open<StringValueSyntax>();
      StringValueSyntax stringValueSyntax = syntax;
      if (!(this._token.Value is string empty))
        empty = string.Empty;
      stringValueSyntax.Value = empty;
      syntax.Token = this.EatToken();
      return this.Close<StringValueSyntax>(syntax);
    }

    private DottedKeyItemSyntax ParseDotKey()
    {
      DottedKeyItemSyntax syntax = this.Open<DottedKeyItemSyntax>();
      syntax.Dot = this.EatToken();
      syntax.Key = this.ParseBaseKey();
      return this.Close<DottedKeyItemSyntax>(syntax);
    }

    private BareKeySyntax ParseBasicKey()
    {
      BareKeySyntax syntax = this.Open<BareKeySyntax>();
      syntax.Key = this.EatToken(TokenKind.BasicKey);
      return this.Close<BareKeySyntax>(syntax);
    }

    private SyntaxToken EatToken(TokenKind tokenKind)
    {
      SyntaxToken syntax;
      if (this._token.Kind == tokenKind)
      {
        syntax = this.Open<SyntaxToken>();
      }
      else
      {
        InvalidSyntaxToken invalidSyntaxToken = this.Open<InvalidSyntaxToken>();
        invalidSyntaxToken.InvalidKind = this._token.Kind;
        syntax = (SyntaxToken) invalidSyntaxToken;
        string text = tokenKind.ToText();
        string str1;
        if (text == null)
          str1 = "while expecting token `" + tokenKind.ToString().ToLowerInvariant() + "`";
        else
          str1 = "while expecting `" + text + "` (token: `" + tokenKind.ToString().ToLowerInvariant() + "`)";
        string str2 = str1;
        if (this._token.Kind == TokenKind.Invalid)
          this.LogError("Unexpected token found `" + this.ToPrintable(this._token) + "` " + str2);
        else
          this.LogError("Unexpected token found `" + this.ToPrintable(this._token) + "` (token: `" + this._token.Kind.ToString().ToLowerInvariant() + "`) " + str2);
      }
      syntax.TokenKind = tokenKind;
      syntax.Text = this._token.Kind.ToText() ?? this._token.GetText<TSourceView>(this._lexer.Source);
      if (tokenKind == TokenKind.NewLine)
        this._hideNewLine = true;
      this.NextToken();
      return this.Close<SyntaxToken>(syntax);
    }

    private SyntaxToken EatToken()
    {
      SyntaxToken syntax = this.Open<SyntaxToken>();
      syntax.TokenKind = this._token.Kind;
      syntax.Text = this._token.Kind.ToText() ?? this._token.GetText<TSourceView>(this._lexer.Source);
      this.NextToken();
      return this.Close<SyntaxToken>(syntax);
    }

    private void SkipAfterEndOfLine()
    {
      while (!this.IsEolOrEof())
        this.NextToken();
      if (this._token.Kind == TokenKind.Eof)
        return;
      this.NextToken();
    }

    private bool IsEolOrEof() => this._token.Kind == TokenKind.NewLine || this._token.Kind == TokenKind.Eof;

    private T Open<T>() where T : SyntaxNode, new() => this.Open<T>(this._token);

    private T Open<T>(T syntax) where T : SyntaxNode => this.Open<T>(syntax, this._token);

    private T Open<T>(T syntax, SyntaxTokenValue startToken) where T : SyntaxNode
    {
      syntax.Span = new SourceSpan(this._lexer.Source.SourcePath, startToken.Start, new TextPosition());
      if (this._currentTrivias.Count > 0)
      {
        syntax.LeadingTrivia = new List<SyntaxTrivia>((IEnumerable<SyntaxTrivia>) this._currentTrivias);
        this._currentTrivias.Clear();
      }
      return syntax;
    }

    private T Open<T>(SyntaxTokenValue startToken) where T : SyntaxNode, new() => this.Open<T>(new T(), startToken);

    private T Close<T>(T syntax) where T : SyntaxNode
    {
      syntax.Span.End = this._previousToken.End;
      if (this._currentTrivias.Count > 0)
      {
        syntax.TrailingTrivia = new List<SyntaxTrivia>((IEnumerable<SyntaxTrivia>) this._currentTrivias);
        this._currentTrivias.Clear();
      }
      return syntax;
    }

    private string? ToPrintable(SyntaxTokenValue localToken) => this.ToText(localToken).ToPrintableString();

    private string? ToText(SyntaxTokenValue localToken) => localToken.GetText<TSourceView>(this._lexer.Source);

    private string? ToPrintable(SourceSpan span) => this._lexer.Source.GetString(span.Offset, span.Length).ToPrintableString();

    private void NextToken()
    {
      this._previousToken = this._token;
      bool flag;
      while ((flag = this._lexer.MoveNext()) && this._lexer.Token.Kind.IsHidden(this._hideNewLine))
      {
        List<SyntaxTrivia> currentTrivias = this._currentTrivias;
        SyntaxTrivia syntaxTrivia = new SyntaxTrivia();
        syntaxTrivia.Span = new SourceSpan(this._lexer.Source.SourcePath, this._lexer.Token.Start, this._lexer.Token.End);
        syntaxTrivia.Kind = this._lexer.Token.Kind;
        syntaxTrivia.Text = this._lexer.Token.GetText<TSourceView>(this._lexer.Source);
        currentTrivias.Add(syntaxTrivia);
      }
      this._token = flag ? this._lexer.Token : new SyntaxTokenValue(TokenKind.Eof, new TextPosition(), new TextPosition());
    }

    private void LogError(string text) => this.LogError(this._token, text);

    private void LogError(SyntaxTokenValue tokenArg, string text) => this.LogError(this.GetSpanForToken(tokenArg), text);

    private SourceSpan GetSpanForToken(SyntaxTokenValue tokenArg) => new SourceSpan(this._lexer.Source.SourcePath, tokenArg.Start, tokenArg.End);

    private void LogError(SourceSpan span, string text) => this.Log(new DiagnosticMessage(DiagnosticMessageKind.Error, span, text));

    private void LogError(SyntaxNode node, string message) => this.LogError(node, node.Span, message);

    private void LogError(SyntaxNode node, SourceSpan span, string message) => this.LogError(span, "Error while parsing " + node.GetType().Name + ": " + message);

    private void Log(DiagnosticMessage diagnosticMessage)
    {
      if (diagnosticMessage == null)
        throw new ArgumentNullException(nameof (diagnosticMessage));
      this._diagnostics.Add(diagnosticMessage);
    }
  }
}
