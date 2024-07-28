// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters.ParsedContentWriterV3
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters
{
  internal class ParsedContentWriterV3 : IParsedContentWriter
  {
    public const int UnknownSize = -1;
    private List<CodeTokenKind> m_codeTokenKindsToBeFurtherTokenized = new List<CodeTokenKind>()
    {
      CodeTokenKind.JavaLiteral,
      CodeTokenKind.JavaComment,
      CodeTokenKind.JavaPackageDeclaration,
      CodeTokenKind.JavaImportDeclaration
    };

    private IWriter OutputBuffer { get; set; }

    private bool WrittenTokens { get; set; }

    private long Size { get; set; }

    private ParsedData Response { get; set; }

    private void WriteDocumentType(DocumentType documentType)
    {
      if (this.Size != -1L)
      {
        this.OutputBuffer.WriteUInt32(2U);
        this.OutputBuffer.WriteVInt64(this.Size);
      }
      this.OutputBuffer.WriteUInt32((uint) documentType);
      this.WrittenTokens = false;
    }

    private void WriteEndMarker() => this.OutputBuffer.WriteVUInt32(uint.MaxValue);

    private void WriteTextToken(TextToken token)
    {
      this.OutputBuffer.WriteVUInt32(token.CharacterOffset);
      this.OutputBuffer.WriteVUInt32(token.TokenOffset);
      byte[] bytes = Encoding.UTF8.GetBytes(token.Value);
      this.OutputBuffer.WriteVUInt32((uint) bytes.Length);
      this.OutputBuffer.WriteBytes(bytes);
    }

    private void WriteTextTokens(IEnumerable<TextToken> tokens)
    {
      this.WriteDocumentType(DocumentType.Text);
      foreach (TextToken token in tokens)
        this.WriteTextToken(token);
      this.WriteEndMarker();
      this.WrittenTokens = true;
    }

    private void WriteCodeSymbols(IEnumerable<TextToken> symbols)
    {
      IEnumerable<CodeSymbol> codeSymbols = symbols.AsEnumerable<TextToken>().Cast<CodeSymbol>();
      this.WriteDocumentType(DocumentType.Cpp);
      foreach (CodeSymbol codeSymbol in codeSymbols)
      {
        byte[] bytes = Encoding.UTF8.GetBytes(codeSymbol.Value);
        this.OutputBuffer.WriteVUInt32(codeSymbol.CharacterOffset);
        this.OutputBuffer.WriteVUInt32(codeSymbol.TokenOffset);
        this.OutputBuffer.WriteVUInt32((uint) bytes.Length);
        this.OutputBuffer.WriteBytes(bytes);
        if (codeSymbol.IsExtendedSymbol)
        {
          this.OutputBuffer.WriteVUInt32(52U);
          this.OutputBuffer.WriteVUInt32(codeSymbol.SymbolLengthExtended);
        }
        this.OutputBuffer.WriteVUInt32((uint) codeSymbol.SymbolType);
        if (CodeSymbol.HasScope(codeSymbol.SymbolType))
        {
          this.OutputBuffer.WriteVInt32(codeSymbol.ScopeBeginLine);
          this.OutputBuffer.WriteVInt32(codeSymbol.ScopeEndLine);
        }
      }
      this.WriteEndMarker();
      this.WrittenTokens = true;
    }

    public virtual ParsedData WriteParsedContent(IParsedContent parsedContent)
    {
      CodeParsedContent parsedContent1 = parsedContent as CodeParsedContent;
      this.Response = new ParsedData();
      this.Size = (long) Encoding.UTF8.GetByteCount(parsedContent1.Content);
      using (DynamicByteArray dynamicByteArray = new DynamicByteArray(4096U))
      {
        this.OutputBuffer = (IWriter) dynamicByteArray;
        dynamicByteArray.Position = 0U;
        if (parsedContent1.HasCodeSymbols)
        {
          this.ParseCodeSymbols(parsedContent1);
          this.WriteCodeSymbols(parsedContent1.Symbols);
        }
        else
          this.WriteTextTokens(parsedContent1.Symbols);
        this.Response.Content = new byte[(int) dynamicByteArray.Position];
        Array.Copy((Array) dynamicByteArray.GetArray(), (Array) this.Response.Content, this.Response.Content.Length);
      }
      if (!this.WrittenTokens)
        this.WriteDocumentType(DocumentType.Text);
      this.OutputBuffer = (IWriter) null;
      return this.Response;
    }

    protected virtual List<CodeSymbol> TokenizeCodeSymbols(List<CodeSymbol> symbols)
    {
      TextTokenizer textTokenizer = new TextTokenizer();
      foreach (CodeSymbol codeSymbol1 in symbols.Where<CodeSymbol>((Func<CodeSymbol, bool>) (y => this.m_codeTokenKindsToBeFurtherTokenized.Contains(y.SymbolType))).ToList<CodeSymbol>())
      {
        textTokenizer.CharacterOffsetBase = codeSymbol1.CharacterOffset;
        textTokenizer.AllowUnderscore = true;
        symbols.Remove(codeSymbol1);
        foreach (TextToken textToken in textTokenizer.Tokenize(codeSymbol1.Value))
        {
          CodeSymbol codeSymbol2 = new CodeSymbol();
          codeSymbol2.Value = textToken.Value;
          codeSymbol2.SymbolType = codeSymbol1.SymbolType;
          codeSymbol2.CharacterOffset = textToken.CharacterOffset;
          CodeSymbol codeSymbol3 = codeSymbol2;
          symbols.Add(codeSymbol3);
        }
      }
      return this.Sort(symbols);
    }

    internal virtual IParsedContent ParseCodeSymbols(CodeParsedContent parsedContent)
    {
      if (parsedContent != null && parsedContent.HasCodeSymbols)
      {
        IEnumerable<CodeSymbol> source = parsedContent.Symbols.AsEnumerable<TextToken>().Cast<CodeSymbol>();
        parsedContent.Symbols = (IEnumerable<TextToken>) this.TokenizeCodeSymbols(source.ToList<CodeSymbol>());
      }
      return (IParsedContent) parsedContent;
    }

    internal List<CodeSymbol> Sort(List<CodeSymbol> symbols)
    {
      symbols.Sort((Comparison<CodeSymbol>) ((x, y) => (int) x.CharacterOffset - (int) y.CharacterOffset));
      for (int index = 0; index < symbols.Count; ++index)
        symbols[index].TokenOffset = (uint) index;
      return symbols;
    }
  }
}
