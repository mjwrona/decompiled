// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.RoslynParser
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  public abstract class RoslynParser : ContentParser, IParser
  {
    protected List<CodeSymbol> SymbolsList { get; set; } = new List<CodeSymbol>();

    protected RoslynParser(int maxFileSizeSupportedInBytes)
      : base(maxFileSizeSupportedInBytes)
    {
    }

    internal IEnumerable<CodeSymbol> Symbols => (IEnumerable<CodeSymbol>) this.SymbolsList;

    protected override IParsedContent ParseContent()
    {
      this.SymbolsList.Clear();
      this.SymbolsList = this.ParseIntoCodeSymbols();
      this.SymbolsList.Sort((Comparison<CodeSymbol>) ((x, y) => (int) x.CharacterOffset - (int) y.CharacterOffset));
      for (int index = 0; index < this.SymbolsList.Count; ++index)
        this.SymbolsList[index].TokenOffset = (uint) index;
      return (IParsedContent) new CodeParsedContent((IEnumerable<TextToken>) this.SymbolsList, true, this.Content);
    }

    protected abstract List<CodeSymbol> ParseIntoCodeSymbols();

    protected abstract void ParseTriva(SyntaxTriviaList syntaxTriviaList, int depth);

    internal List<CodeSymbol> ParseAndScope(RoslynParser.Parser p, SyntaxNode node, int depth)
    {
      int count = this.SymbolsList.Count;
      p(node, depth);
      return this.SymbolsList.GetRange(count, this.SymbolsList.Count - count);
    }

    internal void AddSymbol(SyntaxToken token, CodeTokenKind kind, int depth)
    {
      int count = this.SymbolsList.Count;
      if (token.HasLeadingTrivia)
        this.ParseTriva(token.LeadingTrivia, depth);
      if (token.HasTrailingTrivia)
        this.ParseTriva(token.TrailingTrivia, depth + 1);
      foreach (TextToken textToken in this.Tokenize(token.Text, (uint) token.Span.Start))
      {
        CodeSymbol symbol = new CodeSymbol();
        symbol.Value = textToken.Value;
        symbol.SymbolType = kind;
        symbol.CharacterOffset = textToken.CharacterOffset;
        symbol.ScopeBegin = (uint) count;
        symbol.ScopeEnd = (uint) this.SymbolsList.Count;
        this.AddSymbol(this.SymbolsList, symbol);
      }
    }

    internal List<CodeSymbol> Tokenize(SyntaxTrivia triva, CodeTokenKind kind)
    {
      List<CodeSymbol> codeSymbolList = new List<CodeSymbol>();
      triva.ToFullString();
      foreach (TextToken textToken in this.Tokenize(triva.ToFullString(), (uint) triva.FullSpan.Start))
      {
        CodeSymbol codeSymbol1 = new CodeSymbol();
        codeSymbol1.Value = textToken.Value;
        codeSymbol1.SymbolType = kind;
        codeSymbol1.CharacterOffset = textToken.CharacterOffset;
        codeSymbol1.ScopeBegin = (uint) this.SymbolsList.Count;
        codeSymbol1.ScopeEnd = (uint) this.SymbolsList.Count;
        CodeSymbol codeSymbol2 = codeSymbol1;
        codeSymbolList.Add(codeSymbol2);
      }
      return codeSymbolList;
    }

    internal IEnumerable<TextToken> Tokenize(string token, uint offSet) => new TextTokenizer()
    {
      AllowUnderscore = true,
      CharacterOffsetBase = offSet
    }.Tokenize(token);

    internal void AddSymbol(List<CodeSymbol> symbolList, CodeSymbol symbol)
    {
      if (symbol == null)
        Tracer.TraceWarning(1080153, "Indexing Pipeline", "Parse", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CodeSymbol should not be null. Stack = '{0}'", (object) this.GetCurrentStackFrames()));
      else if (symbol.Value == null)
        Tracer.TraceWarning(1080153, "Indexing Pipeline", "Parse", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Symbol Value should not be null. CodeTokenKind = '{0}' CharacterOffset = '{1}' TokenOffset = '{2}' Stack = '{3}'", (object) symbol.SymbolType, (object) symbol.CharacterOffset, (object) symbol.TokenOffset, (object) this.GetCurrentStackFrames()));
      else
        symbolList.Add(symbol);
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "depth+1", Justification = "depth is not likely to exceed int.Max range.")]
    protected void AddLiteralSymbol(SyntaxToken token, CodeTokenKind kind, int depth)
    {
      if (this.UseTextTokenizer)
      {
        this.AddSymbol(token, kind, depth);
      }
      else
      {
        if (token.HasLeadingTrivia)
          this.ParseTriva(token.LeadingTrivia, depth);
        if (token.HasTrailingTrivia)
        {
          if (depth == int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof (depth), "input must be less than Int32.MaxValue");
          this.ParseTriva(token.TrailingTrivia, depth + 1);
        }
        CodeSymbol symbol = new CodeSymbol();
        symbol.Value = token.Text;
        symbol.SymbolType = kind;
        symbol.CharacterOffset = (uint) token.Span.Start;
        this.AddSymbol(this.SymbolsList, symbol);
      }
    }

    protected void AddSymbolForFullyQualifiedName(
      SyntaxNode syntaxNode,
      CodeTokenKind kind,
      int depth)
    {
      if (this.UseTextTokenizer)
      {
        this.AddSymbol(syntaxNode, kind, depth);
      }
      else
      {
        CodeSymbol symbol = new CodeSymbol();
        symbol.Value = syntaxNode.NormalizeWhitespace<SyntaxNode>().GetText().ToString();
        symbol.SymbolType = kind;
        symbol.CharacterOffset = (uint) syntaxNode.SpanStart;
        this.AddSymbol(this.SymbolsList, symbol);
      }
    }

    protected void ParseCommentTriva(SyntaxTrivia triva, CodeTokenKind codeTokenKind)
    {
      if (this.UseTextTokenizer)
      {
        foreach (CodeSymbol symbol in this.Tokenize(triva, codeTokenKind))
          this.AddSymbol(this.SymbolsList, symbol);
      }
      else
      {
        CodeSymbol symbol = new CodeSymbol();
        symbol.Value = triva.ToFullString();
        symbol.SymbolType = codeTokenKind;
        symbol.CharacterOffset = (uint) triva.FullSpan.Start;
        this.AddSymbol(this.SymbolsList, symbol);
      }
    }

    protected abstract void AddSymbol(SyntaxNode syntaxNode, CodeTokenKind kind, int depth);

    private string GetCurrentStackFrames()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (StackFrame frame in new StackTrace().GetFrames())
      {
        MethodBase method = frame.GetMethod();
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}' : '{1}'", (object) method.Name, method.ReflectedType != (Type) null ? (object) method.ReflectedType.Name : (object) string.Empty));
      }
      return stringBuilder.ToString();
    }

    internal delegate void Parser(SyntaxNode node, int depth);
  }
}
