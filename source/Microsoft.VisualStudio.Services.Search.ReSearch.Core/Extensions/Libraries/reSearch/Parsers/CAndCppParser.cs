// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.CAndCppParser
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters;
using Microsoft.Windows.Tools.Search.SourceCodeParsers.Cpp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  [SuppressMessage("Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix")]
  public class CAndCppParser : ContentParser, IParser, IDisposable
  {
    private IEnumerable<CodeSymbol> m_symbols = (IEnumerable<CodeSymbol>) new List<CodeSymbol>();
    [StaticSafe]
    private static readonly Dictionary<CodeTokenKind, CodeTokenKind[]> s_kindsToSplit = new Dictionary<CodeTokenKind, CodeTokenKind[]>()
    {
      {
        CodeTokenKind.ClassOrStructOrNamespaceDeclaration,
        new CodeTokenKind[3]
        {
          CodeTokenKind.ClassDeclaration,
          CodeTokenKind.StructDeclaration,
          CodeTokenKind.NamespaceDeclaration
        }
      },
      {
        CodeTokenKind.FunctionOrMethodDefinition,
        new CodeTokenKind[2]
        {
          CodeTokenKind.FunctionDefinition,
          CodeTokenKind.MethodDefinition
        }
      }
    };
    private bool m_disposed;
    private CppParser m_cppParser;
    [StaticSafe]
    private static object s_lockObject = new object();

    internal IEnumerable<CodeSymbol> Symbols => this.m_symbols;

    public CAndCppParser(string hintFileDirectory, int maxFileSizeSupportedInBytes)
      : base(maxFileSizeSupportedInBytes)
    {
      this.m_cppParser = new CppParser();
      this.AddHintFiles(hintFileDirectory);
      this.m_cppParser.CollectIdentifierReferences = true;
      this.m_disposed = false;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      if (disposing && this.m_cppParser != null)
      {
        this.m_cppParser.Dispose();
        this.m_cppParser = (CppParser) null;
      }
      this.m_disposed = true;
    }

    private void AddHintFiles(string hintFileDirectory)
    {
      try
      {
        foreach (string file in Directory.GetFiles(hintFileDirectory, "*.hint"))
          this.m_cppParser.AddHintFile(file);
      }
      catch (DirectoryNotFoundException ex)
      {
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
    }

    protected override IParsedContent ParseContent()
    {
      List<CodeSymbol> codeSymbolList = new List<CodeSymbol>();
      List<CodeSymbol> intoCodeSymbols = this.ParseIntoCodeSymbols();
      intoCodeSymbols.Sort((Comparison<CodeSymbol>) ((x, y) =>
      {
        int content = (int) x.CharacterOffset - (int) y.CharacterOffset;
        if (content == 0)
        {
          if (x.SymbolType != CodeTokenKind.MacroReference && y.SymbolType == CodeTokenKind.MacroReference)
            content = 1;
          else if (x.SymbolType == CodeTokenKind.MacroReference && y.SymbolType != CodeTokenKind.MacroReference)
            content = -1;
        }
        return content;
      }));
      this.m_symbols = this.RemoveIncorrectMacroReferences((IEnumerable<CodeSymbol>) intoCodeSymbols);
      this.m_symbols = this.MergeTokens(this.m_symbols, new TextTokenizer()
      {
        AllowUnderscore = true
      }.Tokenize(this.Content));
      this.m_symbols = this.FilterDuplicates(this.m_symbols);
      this.m_symbols = this.FixupTokensFromMacroExpansion(this.m_symbols);
      this.m_symbols = this.SplitSymbols(this.m_symbols);
      return (IParsedContent) new CodeParsedContent((IEnumerable<TextToken>) this.m_symbols, true, this.Content);
    }

    private IEnumerable<CodeSymbol> SplitSymbols(IEnumerable<CodeSymbol> input)
    {
      foreach (CodeSymbol symbol in input)
      {
        CodeTokenKind[] codeTokenKindArray1;
        if (CAndCppParser.s_kindsToSplit.TryGetValue(symbol.SymbolType, out codeTokenKindArray1))
        {
          CodeTokenKind[] codeTokenKindArray = codeTokenKindArray1;
          for (int index = 0; index < codeTokenKindArray.Length; ++index)
          {
            CodeTokenKind codeTokenKind = codeTokenKindArray[index];
            CodeSymbol codeSymbol = new CodeSymbol();
            codeSymbol.CharacterOffset = symbol.CharacterOffset;
            codeSymbol.ScopeBegin = symbol.ScopeBegin;
            codeSymbol.ScopeBeginLine = symbol.ScopeBeginLine;
            codeSymbol.ScopeEnd = symbol.ScopeEnd;
            codeSymbol.ScopeEndLine = symbol.ScopeEndLine;
            codeSymbol.ScopeLevel = symbol.ScopeLevel;
            codeSymbol.SymbolType = codeTokenKind;
            codeSymbol.TokenOffset = symbol.TokenOffset;
            codeSymbol.Value = symbol.Value;
            yield return codeSymbol;
          }
          codeTokenKindArray = (CodeTokenKind[]) null;
        }
        else
          yield return symbol;
      }
    }

    private bool IsDuplicateToken(CodeSymbol previousToken, CodeSymbol currentToken)
    {
      if (previousToken == null)
        return false;
      if (currentToken == null)
        throw new ArgumentNullException(nameof (currentToken));
      return (int) previousToken.CharacterOffset == (int) currentToken.CharacterOffset && (int) previousToken.ScopeBegin == (int) currentToken.ScopeBegin && (int) previousToken.ScopeEnd == (int) currentToken.ScopeEnd && (int) previousToken.ScopeLevel == (int) currentToken.ScopeLevel && previousToken.SymbolType == currentToken.SymbolType && string.Compare(previousToken.Value, currentToken.Value, StringComparison.Ordinal) == 0;
    }

    private bool HasSamePosition(CodeSymbol previousToken, CodeSymbol currentToken)
    {
      if (previousToken == null)
        return false;
      if (currentToken == null)
        throw new ArgumentNullException(nameof (currentToken));
      return (int) previousToken.CharacterOffset == (int) currentToken.CharacterOffset;
    }

    private IEnumerable<CodeSymbol> FilterTokens(
      IEnumerable<CodeSymbol> tokens,
      Func<CodeSymbol, CodeSymbol, CAndCppParser.FilterAction> filter,
      int queueDepth = 2)
    {
      Queue<CodeSymbol> queue = new Queue<CodeSymbol>();
      CodeSymbol previousToken = (CodeSymbol) null;
      foreach (CodeSymbol token in tokens)
      {
        switch (filter(previousToken, token))
        {
          case CAndCppParser.FilterAction.IncludeAndKeepPrevious:
            queue.Enqueue(token);
            if (queue.Count > queueDepth)
            {
              yield return queue.Dequeue();
              continue;
            }
            continue;
          case CAndCppParser.FilterAction.IncludeAndSetAsPrevious:
            previousToken = token;
            goto case CAndCppParser.FilterAction.IncludeAndKeepPrevious;
          case CAndCppParser.FilterAction.Remove:
            continue;
          default:
            throw new InvalidOperationException("Invalid FilterAction returned.");
        }
      }
      while (queue.Count > 0)
        yield return queue.Dequeue();
    }

    private IEnumerable<CodeSymbol> FilterDuplicates(IEnumerable<CodeSymbol> tokens) => this.FilterTokens(tokens, (Func<CodeSymbol, CodeSymbol, CAndCppParser.FilterAction>) ((previous, current) =>
    {
      if (!this.IsDuplicateToken(previous, current))
        return CAndCppParser.FilterAction.IncludeAndSetAsPrevious;
      if (previous.ScopeBeginLine == 0 && previous.ScopeEndLine == 0)
      {
        previous.ScopeBeginLine = current.ScopeBeginLine;
        previous.ScopeEndLine = current.ScopeEndLine;
      }
      return CAndCppParser.FilterAction.Remove;
    }));

    private IEnumerable<CodeSymbol> FixupTokensFromMacroExpansion(IEnumerable<CodeSymbol> tokens) => this.FilterTokens(tokens, (Func<CodeSymbol, CodeSymbol, CAndCppParser.FilterAction>) ((previous, current) =>
    {
      if (!this.HasSamePosition(previous, current) || previous.SymbolType != CodeTokenKind.MacroReference)
        return CAndCppParser.FilterAction.IncludeAndSetAsPrevious;
      if (previous.Value.Length != current.Value.Length)
      {
        current.IsExtendedSymbol = true;
        current.SymbolLengthExtended = (uint) previous.Value.Length;
      }
      return CAndCppParser.FilterAction.IncludeAndKeepPrevious;
    }));

    private IEnumerable<CodeSymbol> RemoveIncorrectMacroReferences(IEnumerable<CodeSymbol> tokens)
    {
      CodeSymbol codeSymbol = (CodeSymbol) null;
      foreach (CodeSymbol current in tokens)
      {
        if (codeSymbol != null && (codeSymbol.SymbolType != CodeTokenKind.MacroReference || current.SymbolType != CodeTokenKind.MacroDefinition || (int) codeSymbol.CharacterOffset != (int) current.CharacterOffset || (int) codeSymbol.TokenOffset != (int) current.TokenOffset || codeSymbol.IsExtendedSymbol != current.IsExtendedSymbol || (int) codeSymbol.SymbolLengthExtended != (int) current.SymbolLengthExtended || !(codeSymbol.Value == current.Value)))
          yield return codeSymbol;
        codeSymbol = current;
      }
      if (codeSymbol != null)
        yield return codeSymbol;
    }

    private CodeSymbol GetCodeSymbolFromTextToken(TextToken textToken)
    {
      CodeSymbol symbolFromTextToken = new CodeSymbol();
      symbolFromTextToken.CharacterOffset = textToken.CharacterOffset;
      symbolFromTextToken.SymbolType = CodeTokenKind.Unknown;
      symbolFromTextToken.Value = textToken.Value;
      return symbolFromTextToken;
    }

    private IEnumerable<CodeSymbol> MergeTokens(
      IEnumerable<CodeSymbol> inputCodeSymbols,
      IEnumerable<TextToken> inputTextTokens)
    {
      IEnumerator<CodeSymbol> codeSymbols = inputCodeSymbols.GetEnumerator();
      IEnumerator<TextToken> textTokens = inputTextTokens.GetEnumerator();
      bool hasCodeSymbol = codeSymbols.MoveNext();
      bool hasTextToken = textTokens.MoveNext();
      uint previousCharacterOffset = 0;
      uint tokenOffset = 0;
      while (hasCodeSymbol | hasTextToken)
      {
        bool emitCodeSymbol;
        if (hasCodeSymbol)
        {
          if (hasTextToken)
          {
            CodeSymbol current1 = codeSymbols.Current;
            TextToken current2 = textTokens.Current;
            if (current1.CharacterOffset > current2.CharacterOffset)
            {
              emitCodeSymbol = false;
            }
            else
            {
              if ((int) current1.CharacterOffset == (int) current2.CharacterOffset && current1.Value.Length == current2.Value.Length)
                hasTextToken = textTokens.MoveNext();
              emitCodeSymbol = true;
            }
          }
          else
            emitCodeSymbol = true;
        }
        else
          emitCodeSymbol = false;
        CodeSymbol codeSymbol = !emitCodeSymbol ? this.GetCodeSymbolFromTextToken(textTokens.Current) : codeSymbols.Current;
        if (codeSymbol.CharacterOffset > previousCharacterOffset)
          ++tokenOffset;
        codeSymbol.TokenOffset = tokenOffset;
        previousCharacterOffset = codeSymbol.CharacterOffset;
        yield return codeSymbol;
        if (emitCodeSymbol)
          hasCodeSymbol = codeSymbols.MoveNext();
        else
          hasTextToken = textTokens.MoveNext();
      }
    }

    private void TokenizeCodeSymbol(
      List<TextToken> tokenizedSymbol,
      CodeSymbol codeSymbol,
      List<CodeSymbol> codeSymbols)
    {
      foreach (TextToken textToken in tokenizedSymbol)
      {
        CodeSymbol codeSymbol1 = new CodeSymbol();
        codeSymbol1.CharacterOffset = textToken.CharacterOffset;
        codeSymbol1.SymbolType = codeSymbol.SymbolType;
        codeSymbol1.Value = textToken.Value;
        codeSymbols.Add(codeSymbol1);
      }
    }

    private bool IsHeaderBegin(CodeTokenKind kind, char c)
    {
      if (kind == CodeTokenKind.HeaderReference && c == '"')
        return true;
      return kind == CodeTokenKind.SystemHeaderReference && c == '<';
    }

    private List<CodeSymbol> ParseIntoCodeSymbols()
    {
      List<CodeSymbol> codeSymbols = new List<CodeSymbol>();
      if (!string.IsNullOrEmpty(this.Content))
      {
        lock (CAndCppParser.s_lockObject)
        {
          TextTokenizer textTokenizer = new TextTokenizer();
          textTokenizer.AllowUnderscore = true;
          this.m_cppParser.Parse(this.Content);
          Encoding.UTF8.GetBytes(this.Content);
          Tag tag = (Tag) null;
          int startIndex = 0;
          foreach (Tag allTag in this.m_cppParser.AllTags)
          {
            CodeSymbol codeSymbol1 = this.TransformTagIntoCodeSymbol(allTag);
            if (codeSymbol1.SymbolType == CodeTokenKind.HeaderReference || codeSymbol1.SymbolType == CodeTokenKind.SystemHeaderReference)
            {
              uint characterOffset = codeSymbol1.CharacterOffset;
              while ((long) characterOffset < (long) this.Content.Length && !this.IsHeaderBegin(codeSymbol1.SymbolType, this.Content[(int) characterOffset]))
                ++characterOffset;
              if ((long) characterOffset < (long) this.Content.Length)
                ++characterOffset;
              textTokenizer.CharacterOffsetBase = characterOffset;
              this.TokenizeCodeSymbol(new List<TextToken>(textTokenizer.Tokenize(codeSymbol1.Value)), codeSymbol1, codeSymbols);
            }
            else
            {
              textTokenizer.CharacterOffsetBase = codeSymbol1.CharacterOffset;
              switch (codeSymbol1.SymbolType)
              {
                case CodeTokenKind.ArgumentDeclaration:
                  if (tag != null && (int) allTag.ParentTagId == (int) tag.TagId)
                  {
                    int num = this.Content.IndexOf(codeSymbol1.Value, startIndex, StringComparison.Ordinal);
                    if (num != -1)
                    {
                      codeSymbol1.CharacterOffset = (uint) num;
                      startIndex = num + codeSymbol1.Value.Length;
                      codeSymbols.Add(codeSymbol1);
                      continue;
                    }
                    continue;
                  }
                  break;
                case CodeTokenKind.Comment:
                  if (this.UseTextTokenizer)
                  {
                    this.TokenizeCodeSymbol(new List<TextToken>(textTokenizer.Tokenize(codeSymbol1.Value)), codeSymbol1, codeSymbols);
                    continue;
                  }
                  codeSymbols.Add(codeSymbol1);
                  continue;
                case CodeTokenKind.MacroDefinition:
                  tag = allTag;
                  startIndex = (int) codeSymbol1.CharacterOffset + codeSymbol1.Value.Length;
                  break;
              }
              List<TextToken> textTokenList = new List<TextToken>(textTokenizer.Tokenize(codeSymbol1.Value));
              if (textTokenList.Count == 1)
              {
                if (!(this.Content.Substring((int) codeSymbol1.CharacterOffset, codeSymbol1.Value.Length) != codeSymbol1.Value))
                {
                  codeSymbol1.CharacterOffset = textTokenList[0].CharacterOffset;
                  codeSymbol1.Value = textTokenList[0].Value;
                  codeSymbols.Add(codeSymbol1);
                }
              }
              else
              {
                foreach (TextToken textToken in textTokenList)
                {
                  string b = textToken.Value;
                  if ((long) textToken.CharacterOffset + (long) b.Length <= (long) this.Content.Length && string.Equals(this.Content.Substring((int) textToken.CharacterOffset, b.Length), b, StringComparison.OrdinalIgnoreCase))
                  {
                    CodeSymbol codeSymbol2 = new CodeSymbol();
                    codeSymbol2.CharacterOffset = textToken.CharacterOffset;
                    codeSymbol2.SymbolType = CodeTokenKind.Unknown;
                    codeSymbol2.Value = b;
                    codeSymbols.Add(codeSymbol2);
                  }
                }
              }
            }
          }
        }
      }
      return codeSymbols;
    }

    private CodeTokenKind GetCodeTokenKindFromTag(Tag tag)
    {
      string kindAndUse = tag.KindAndUse;
      if (kindAndUse != null)
      {
        switch (kindAndUse.Length)
        {
          case 14:
            if (kindAndUse == "TYPE_REFERENCE")
              return CodeTokenKind.TypeReference;
            break;
          case 15:
            switch (kindAndUse[0])
            {
              case 'M':
                if (kindAndUse == "MACRO_REFERENCE")
                  return CodeTokenKind.MacroReference;
                break;
              case 'T':
                if (kindAndUse == "TYPE_DEFINITION")
                  return CodeTokenKind.TypeDefinition;
                break;
            }
            break;
          case 16:
            switch (kindAndUse[0])
            {
              case 'C':
                if (kindAndUse == "CLASS_DEFINITION")
                  return CodeTokenKind.ClassDefinition;
                break;
              case 'H':
                if (kindAndUse == "HEADER_REFERENCE")
                  return CodeTokenKind.HeaderReference;
                break;
              case 'M':
                if (kindAndUse == "MACRO_DEFINITION")
                  return CodeTokenKind.MacroDefinition;
                break;
              case 'U':
                if (kindAndUse == "UNION_DEFINITION")
                  return CodeTokenKind.UnionDefinition;
                break;
            }
            break;
          case 17:
            switch (kindAndUse[0])
            {
              case 'C':
                if (kindAndUse == "CLASS_DECLARATION")
                  return CodeTokenKind.ClassDeclaration;
                break;
              case 'F':
                if (kindAndUse == "FIELD_DECLARATION")
                  return CodeTokenKind.FieldDeclaration;
                break;
              case 'M':
                if (kindAndUse == "METHOD_DEFINITION")
                  return CodeTokenKind.MethodDefinition;
                break;
              case 'S':
                if (kindAndUse == "STRUCT_DEFINITION")
                  return CodeTokenKind.StructDefinition;
                break;
              case 'U':
                if (kindAndUse == "UNION_DECLARATION")
                  return CodeTokenKind.UnionDeclaration;
                break;
            }
            break;
          case 18:
            switch (kindAndUse[1])
            {
              case 'A':
                if (kindAndUse == "MACRO_UNDEFINITION")
                  return CodeTokenKind.MacroUndefinition;
                break;
              case 'E':
                if (kindAndUse == "METHOD_DECLARATION")
                  return CodeTokenKind.MethodDeclaration;
                break;
              case 'T':
                if (kindAndUse == "STRUCT_DECLARATION")
                  return CodeTokenKind.StructDeclaration;
                break;
            }
            break;
          case 19:
            switch (kindAndUse[0])
            {
              case 'C':
                if (kindAndUse == "COMMENT_DECLARATION")
                  return CodeTokenKind.Comment;
                break;
              case 'F':
                if (kindAndUse == "FUNCTION_DEFINITION")
                  return CodeTokenKind.FunctionDefinition;
                break;
              case 'N':
                if (kindAndUse == "NAMESPACE_REFERENCE")
                  return CodeTokenKind.NamespaceReference;
                break;
              case 'O':
                if (kindAndUse == "OPERATOR_DEFINITION")
                  return CodeTokenKind.OperatorDefinition;
                break;
            }
            break;
          case 20:
            switch (kindAndUse[1])
            {
              case 'D':
                if (kindAndUse == "IDENTIFIER_REFERENCE")
                  return CodeTokenKind.IdentifierReference;
                break;
              case 'N':
                if (kindAndUse == "INTERFACE_DEFINITION")
                  return CodeTokenKind.InterfaceDefinition;
                break;
              case 'P':
                if (kindAndUse == "OPERATOR_DECLARATION")
                  return CodeTokenKind.OperatorDeclaration;
                break;
              case 'R':
                if (kindAndUse == "ARGUMENT_DECLARATION")
                  return CodeTokenKind.ArgumentDeclaration;
                break;
              case 'U':
                if (kindAndUse == "FUNCTION_DECLARATION")
                  return CodeTokenKind.FunctionDeclaration;
                break;
            }
            break;
          case 21:
            switch (kindAndUse[0])
            {
              case 'B':
                if (kindAndUse == "BASE_TYPE_DECLARATION")
                  return CodeTokenKind.BaseTypeDeclaration;
                break;
              case 'D':
                if (kindAndUse == "DESTRUCTOR_DEFINITION")
                  return CodeTokenKind.DestructorDefinition;
                break;
              case 'E':
                if (kindAndUse == "ENUMERATOR_DEFINITION")
                  return CodeTokenKind.EnumeratorDefinition;
                break;
              case 'I':
                if (kindAndUse == "INTERFACE_DECLARATION")
                  return CodeTokenKind.InterfaceDeclaration;
                break;
              case 'N':
                if (kindAndUse == "NAMESPACE_DECLARATION")
                  return CodeTokenKind.NamespaceDeclaration;
                break;
            }
            break;
          case 22:
            switch (kindAndUse[0])
            {
              case 'C':
                if (kindAndUse == "CONSTRUCTOR_DEFINITION")
                  return CodeTokenKind.ConstructorDefinition;
                break;
              case 'D':
                if (kindAndUse == "DESTRUCTOR_DECLARATION")
                  return CodeTokenKind.DestructorDeclaration;
                break;
              case 'E':
                if (kindAndUse == "ENUMERATOR_DECLARATION")
                  return CodeTokenKind.EnumeratorDeclaration;
                break;
            }
            break;
          case 23:
            switch (kindAndUse[0])
            {
              case 'C':
                if (kindAndUse == "CONSTRUCTOR_DECLARATION")
                  return CodeTokenKind.ConstructorDeclaration;
                break;
              case 'R':
                if (kindAndUse == "RETURN_TYPE_DECLARATION")
                  return CodeTokenKind.ReturnTypeDeclaration;
                break;
              case 'S':
                if (kindAndUse == "SYSTEM_HEADER_REFERENCE")
                  return CodeTokenKind.SystemHeaderReference;
                break;
              case 'U':
                if (kindAndUse == "UNKNOWN_KIND_DEFINITION")
                  return CodeTokenKind.UnknownKindDefinition;
                break;
            }
            break;
          case 24:
            if (kindAndUse == "UNKNOWN_KIND_DECLARATION")
              return CodeTokenKind.UnknownKindDeclaration;
            break;
          case 26:
            switch (kindAndUse[0])
            {
              case 'C':
                if (kindAndUse == "CONSTANT_FIELD_DECLARATION")
                  return CodeTokenKind.ConstantFieldDeclaration;
                break;
              case 'F':
                if (kindAndUse == "FRIEND_FUNCTION_DEFINITION")
                  return CodeTokenKind.FriendFunctionDefinition;
                break;
              case 'G':
                if (kindAndUse == "GLOBAL_CONSTANT_DEFINITION")
                  return CodeTokenKind.GlobalConstantDefinition;
                break;
            }
            break;
          case 27:
            switch (kindAndUse[1])
            {
              case 'L':
                switch (kindAndUse)
                {
                  case "GLOBAL_CONSTANT_DECLARATION":
                    return CodeTokenKind.GlobalConstantDeclaration;
                  case "GLOBAL_VARIABLE_DECLARATION":
                    return CodeTokenKind.GlobalVariableDeclaration;
                }
                break;
              case 'N':
                if (kindAndUse == "ENUMERATOR_ITEM_DECLARATION")
                  return CodeTokenKind.EnumeratorItemDeclaration;
                break;
              case 'R':
                if (kindAndUse == "FRIEND_FUNCTION_DECLARATION")
                  return CodeTokenKind.FriendFunctionDeclaration;
                break;
              case 'X':
                switch (kindAndUse)
                {
                  case "EXTERN_VARIABLE_DECLARATION":
                    return CodeTokenKind.ExternVariableDeclaration;
                  case "EXTERN_FUNCTION_DECLARATION":
                    return CodeTokenKind.ExternFunctionDeclaration;
                }
                break;
            }
            break;
          case 29:
            switch (kindAndUse[0])
            {
              case 'F':
                if (kindAndUse == "FUNCTION_OR_METHOD_DEFINITION")
                  return CodeTokenKind.FunctionOrMethodDefinition;
                break;
              case 'T':
                if (kindAndUse == "TEMPLATE_ARGUMENT_DECLARATION")
                  return CodeTokenKind.TemplateArgumentDeclaration;
                break;
            }
            break;
          case 34:
            switch (kindAndUse[0])
            {
              case 'F':
                if (kindAndUse == "FRIEND_CLASS_OR_STRUCT_DECLARATION")
                  return CodeTokenKind.FriendClassOrStructDeclaration;
                break;
              case 'T':
                if (kindAndUse == "TEMPLATE_SPECIFICATION_DECLARATION")
                  return CodeTokenKind.TemplateSpecificationDeclaration;
                break;
            }
            break;
          case 40:
            if (kindAndUse == "CLASS_OR_STRUCT_OR_NAMESPACE_DECLARATION")
              return CodeTokenKind.ClassOrStructOrNamespaceDeclaration;
            break;
        }
      }
      return CodeTokenKind.Unknown;
    }

    private CodeSymbol TransformTagIntoCodeSymbol(Tag tag)
    {
      CodeSymbol codeSymbol = new CodeSymbol();
      codeSymbol.CharacterOffset = tag.Offset;
      codeSymbol.SymbolType = this.GetCodeTokenKindFromTag(tag);
      if (tag.ScopeSpan != null)
      {
        codeSymbol.ScopeBeginLine = (int) tag.ScopeSpan.StartLine;
        codeSymbol.ScopeEndLine = (int) tag.ScopeSpan.EndLine;
      }
      if (codeSymbol.SymbolType == CodeTokenKind.Comment)
        codeSymbol.Value = this.Content.Substring((int) tag.ScopeSpan.StartOffset, (int) tag.ScopeSpan.EndOffset - (int) tag.ScopeSpan.StartOffset);
      else
        codeSymbol.Value = tag.Identifier;
      return codeSymbol;
    }

    internal void AddSymbol(List<CodeSymbol> symbolList, CodeSymbol symbol)
    {
      if (symbol == null)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080153, "Indexing Pipeline", "Parse", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CodeSymbol should not be null. Stack = '{0}'", (object) this.GetCurrentStackFrames()));
      else if (symbol.Value == null)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080153, "Indexing Pipeline", "Parse", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Symbol Value should not be null. CodeTokenKind = '{0}' CharacterOffset = '{1}' TokenOffset = '{2}' Stack = '{3}'", (object) symbol.SymbolType, (object) symbol.CharacterOffset, (object) symbol.TokenOffset, (object) this.GetCurrentStackFrames()));
      else
        symbolList.Add(symbol);
    }

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

    public enum FilterAction
    {
      IncludeAndKeepPrevious,
      IncludeAndSetAsPrevious,
      Remove,
    }
  }
}
