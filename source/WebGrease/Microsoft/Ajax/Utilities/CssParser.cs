// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CssParser
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  public class CssParser
  {
    private CssScanner m_scanner;
    private CssToken m_currentToken;
    private StringBuilder m_parsed;
    private bool m_noOutput;
    private string m_lastOutputString;
    private bool m_mightNeedSpace;
    private bool m_skippedSpace;
    private int m_lineLength;
    private bool m_noColorAbbreviation;
    private bool m_encounteredNewLine;
    private bool m_outputNewLine = true;
    private bool m_forceNewLine;
    private readonly HashSet<string> m_namespaces;
    private CodeSettings m_jsSettings;
    private static Regex s_vendorSpecific = new Regex("^(\\-(?<vendor>[^\\-]+)\\-)?(?<root>.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_regexHack1 = new Regex("/\\*([^*]|(\\*+[^*/]))*\\**\\\\\\*/(?<inner>.*?)/\\*([^*]|(\\*+[^*/]))*\\*+/", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_regexHack2 = new Regex("/\\*/\\*//\\*/(?<inner>.*?)/\\*([^*]|(\\*+[^*/]))*\\*+/", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_regexHack3 = new Regex("/\\*/\\*/(?<inner>.*?)/\\*([^*]|(\\*+[^*/]))*\\*+/", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_regexHack4 = new Regex("(?<=\\w\\s+)/\\*([^*]|(\\*+[^*/]))*\\*+/\\s*(?=:)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_regexHack5 = new Regex("(?<=[\\w/]\\s*:)\\s*/\\*([^*]|(\\*+[^*/]))*\\*+/", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_regexHack6 = new Regex("(?<=\\w)/\\*([^*]|(\\*+[^*/]))*\\*+/\\s*(?=:)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_regexHack7 = new Regex("/\\*(\\s?)\\*/", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex s_rrggbb = new Regex("^\\#(?<r>[0-9a-fA-F])\\k<r>(?<g>[0-9a-fA-F])\\k<g>(?<b>[0-9a-fA-F])\\k<b>$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private bool m_parsingColorValue;
    private static Regex s_valueReplacement = new Regex("/\\*\\s*\\[(?<id>\\w+)\\]\\s*\\*/", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
    private string m_valueReplacement;

    public CssSettings Settings { get; set; }

    public string FileContext { get; set; }

    public CodeSettings JSSettings
    {
      get => this.m_jsSettings;
      set
      {
        if (value != null)
        {
          this.m_jsSettings = value.Clone();
          this.m_jsSettings.SourceMode = JavaScriptSourceMode.Expression;
        }
        else
        {
          CodeSettings codeSettings = new CodeSettings();
          codeSettings.KillSwitch = 1048576L;
          codeSettings.SourceMode = JavaScriptSourceMode.Expression;
          this.m_jsSettings = codeSettings;
        }
      }
    }

    private TokenType CurrentTokenType => this.m_currentToken == null ? TokenType.None : this.m_currentToken.TokenType;

    private string CurrentTokenText => this.m_currentToken == null ? string.Empty : this.m_currentToken.Text;

    public CssParser()
    {
      this.Settings = new CssSettings();
      this.JSSettings = (CodeSettings) null;
      this.m_namespaces = new HashSet<string>();
    }

    public string Parse(string source)
    {
      this.m_namespaces.Clear();
      if (source.IsNullOrWhiteSpace())
      {
        source = string.Empty;
      }
      else
      {
        bool flag = false;
        try
        {
          source = this.HandleCharset(source);
          if (this.Settings.CommentMode == CssComment.Hacks)
          {
            source = CssParser.s_regexHack1.Replace(source, "/*! \\*/${inner}/*!*/");
            source = CssParser.s_regexHack2.Replace(source, "/*!/*//*/${inner}/**/");
            source = CssParser.s_regexHack3.Replace(source, "/*!/*/${inner}/*!*/");
            source = CssParser.s_regexHack4.Replace(source, "/*!*/");
            source = CssParser.s_regexHack5.Replace(source, "/*!*/");
            source = CssParser.s_regexHack6.Replace(source, "/*!*/");
            source = CssParser.s_regexHack7.Replace(source, "/*!*/");
            this.Settings.CommentMode = CssComment.Important;
            flag = true;
          }
          using (StringReader reader = new StringReader(source))
          {
            this.m_scanner = new CssScanner((TextReader) reader);
            this.m_scanner.AllowEmbeddedAspNetBlocks = this.Settings.AllowEmbeddedAspNetBlocks;
            this.m_scanner.ScannerError += (EventHandler<ContextErrorEventArgs>) ((sender, ea) =>
            {
              ea.Error.File = this.FileContext;
              this.OnCssError(ea.Error);
            });
            this.m_scanner.ContextChange += (EventHandler<CssScannerContextChangeEventArgs>) ((sender, ea) => this.FileContext = ea.FileContext);
            this.m_parsed = new StringBuilder();
            int num1 = (int) this.NextToken();
            switch (this.Settings.CssType)
            {
              case CssType.DeclarationList:
                this.SkipIfSpace();
                int declarationList = (int) this.ParseDeclarationList(false);
                break;
              default:
                int stylesheet = (int) this.ParseStylesheet();
                break;
            }
            if (!this.m_scanner.EndOfFile)
            {
              int num2 = 1050;
              this.OnCssError(new ContextError()
              {
                IsError = true,
                Severity = 0,
                Subcategory = ContextError.GetSubcategory(0),
                File = this.FileContext,
                ErrorNumber = num2,
                ErrorCode = "CSS{0}".FormatInvariant((object) (num2 & (int) ushort.MaxValue)),
                StartLine = this.m_currentToken.Context.Start.Line,
                StartColumn = this.m_currentToken.Context.Start.Char,
                Message = CssStrings.ExpectedEndOfFile
              });
            }
            source = this.m_parsed.ToString();
            this.m_parsed = (StringBuilder) null;
          }
        }
        finally
        {
          if (flag)
            this.Settings.CommentMode = CssComment.Hacks;
        }
      }
      return source;
    }

    private string HandleCharset(string source)
    {
      if (source.StartsWith("/*/#SOURCE", StringComparison.OrdinalIgnoreCase))
      {
        int index = source.IndexOfAny(new char[2]
        {
          '\n',
          '\r'
        });
        if (index >= 0)
        {
          if (source[index] == '\r' && source[index + 1] == '\n')
            ++index;
          source = source.Substring(index + 1);
        }
      }
      if (source.StartsWith("ï»¿", StringComparison.Ordinal))
      {
        string strB = "@charset ";
        if (string.CompareOrdinal(source, 3, strB, 0, strB.Length) != 0 || source[3 + strB.Length] != '"' && source[3 + strB.Length] != '\'' || string.Compare(source, 4 + strB.Length, "ascii", 0, 5, StringComparison.OrdinalIgnoreCase) != 0)
          this.ReportError(1, CssErrorCode.PossibleCharsetError);
        source = source.Substring(3);
      }
      else if (source.StartsWith("þÿ\0\0", StringComparison.Ordinal) || source.StartsWith("\0\0ÿþ", StringComparison.Ordinal))
      {
        this.ReportError(0, CssErrorCode.PossibleCharsetError);
        source = source.Substring(4);
      }
      else if (source.StartsWith("þÿ", StringComparison.Ordinal) || source.StartsWith("ÿþ", StringComparison.Ordinal))
      {
        this.ReportError(0, CssErrorCode.PossibleCharsetError);
        source = source.Substring(2);
      }
      else if (source.Length > 0 && source[0] == '\uFEFF')
        source = source.Substring(1);
      return source;
    }

    private CssParser.Parsed ParseStylesheet()
    {
      CssParser.Parsed stylesheet = CssParser.Parsed.False;
      this.SkipSemicolons();
      if (this.CurrentTokenType == TokenType.CharacterSetSymbol)
      {
        int charset = (int) this.ParseCharset();
      }
      this.ParseSCDOCDCComments();
      while (this.ParseImport() == CssParser.Parsed.True)
        this.ParseSCDOCDCComments();
      while (this.ParseNamespace() == CssParser.Parsed.True)
        this.ParseSCDOCDCComments();
      while (this.ParseRule() == CssParser.Parsed.True || this.ParseMedia() == CssParser.Parsed.True || this.ParsePage() == CssParser.Parsed.True || this.ParseFontFace() == CssParser.Parsed.True || this.ParseKeyFrames() == CssParser.Parsed.True || this.ParseAtKeyword() == CssParser.Parsed.True || this.ParseAspNetBlock() == CssParser.Parsed.True)
        this.ParseSCDOCDCComments();
      while (!this.m_scanner.EndOfFile)
      {
        this.ReportError(0, CssErrorCode.UnexpectedToken, (object) this.CurrentTokenText);
        int num = (int) this.NextToken();
        this.ParseSCDOCDCComments();
        while (this.ParseRule() == CssParser.Parsed.True || this.ParseMedia() == CssParser.Parsed.True || this.ParsePage() == CssParser.Parsed.True || this.ParseFontFace() == CssParser.Parsed.True || this.ParseAtKeyword() == CssParser.Parsed.True || this.ParseAspNetBlock() == CssParser.Parsed.True)
          this.ParseSCDOCDCComments();
      }
      return stylesheet;
    }

    private CssParser.Parsed ParseCharset()
    {
      this.AppendCurrent();
      this.SkipSpace();
      if (this.CurrentTokenType != TokenType.String)
      {
        this.ReportError(0, CssErrorCode.ExpectedCharset, (object) this.CurrentTokenText);
        this.SkipToEndOfStatement();
        this.AppendCurrent();
      }
      else
      {
        this.Append((object) ' ');
        this.AppendCurrent();
        this.SkipSpace();
        if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ";")
        {
          this.ReportError(0, CssErrorCode.ExpectedSemicolon, (object) this.CurrentTokenText);
          this.SkipToEndOfStatement();
          this.AppendCurrent();
        }
        else
        {
          this.Append((object) ';');
          int num = (int) this.NextToken();
        }
      }
      return CssParser.Parsed.True;
    }

    private void ParseSCDOCDCComments()
    {
      while (this.CurrentTokenType == TokenType.Space || this.CurrentTokenType == TokenType.Comment || this.CurrentTokenType == TokenType.CommentOpen || this.CurrentTokenType == TokenType.CommentClose || this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ";")
      {
        if (this.CurrentTokenType != TokenType.Space && this.CurrentTokenType != TokenType.Character)
          this.AppendCurrent();
        int num = (int) this.NextToken();
      }
    }

    private CssParser.Parsed ParseAtKeyword()
    {
      CssParser.Parsed atKeyword = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.AtKeyword)
      {
        if (!this.CurrentTokenText.StartsWith("@-", StringComparison.OrdinalIgnoreCase))
          this.ReportError(2, CssErrorCode.UnexpectedAtKeyword, (object) this.CurrentTokenText);
        this.SkipToEndOfStatement();
        this.AppendCurrent();
        this.SkipSpace();
        this.NewLine();
        atKeyword = CssParser.Parsed.True;
      }
      else if (this.CurrentTokenType == TokenType.CharacterSetSymbol)
      {
        this.ReportError(2, CssErrorCode.UnexpectedCharset, (object) this.CurrentTokenText);
        atKeyword = this.ParseCharset();
      }
      return atKeyword;
    }

    private CssParser.Parsed ParseAspNetBlock()
    {
      CssParser.Parsed aspNetBlock = CssParser.Parsed.False;
      if (this.Settings.AllowEmbeddedAspNetBlocks && this.CurrentTokenType == TokenType.AspNetBlock)
      {
        this.AppendCurrent();
        this.SkipSpace();
        aspNetBlock = CssParser.Parsed.True;
      }
      return aspNetBlock;
    }

    private CssParser.Parsed ParseNamespace()
    {
      CssParser.Parsed parsed = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.NamespaceSymbol)
      {
        this.NewLine();
        this.AppendCurrent();
        this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Identifier)
        {
          this.Append((object) ' ');
          this.AppendCurrent();
          if (!this.m_namespaces.Add(this.CurrentTokenText))
            this.ReportError(1, CssErrorCode.DuplicateNamespaceDeclaration, (object) this.CurrentTokenText);
          this.SkipSpace();
        }
        if (this.CurrentTokenType != TokenType.String && this.CurrentTokenType != TokenType.Uri)
        {
          this.ReportError(0, CssErrorCode.ExpectedNamespace, (object) this.CurrentTokenText);
          this.SkipToEndOfStatement();
          this.AppendCurrent();
        }
        else
        {
          this.Append((object) ' ');
          this.AppendCurrent();
          this.SkipSpace();
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ";")
          {
            this.Append((object) ';');
            this.SkipSpace();
            this.NewLine();
          }
          else
          {
            this.ReportError(0, CssErrorCode.ExpectedSemicolon, (object) this.CurrentTokenText);
            this.SkipToEndOfStatement();
            this.AppendCurrent();
          }
        }
        parsed = CssParser.Parsed.True;
      }
      return parsed;
    }

    private void ValidateNamespace(string namespaceIdent)
    {
      if (string.IsNullOrEmpty(namespaceIdent) || !(namespaceIdent != "*") || this.m_namespaces.Contains(namespaceIdent))
        return;
      this.ReportError(0, CssErrorCode.UndeclaredNamespace, (object) namespaceIdent);
    }

    private CssParser.Parsed ParseKeyFrames()
    {
      CssParser.Parsed keyFrames = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.KeyFramesSymbol)
      {
        keyFrames = CssParser.Parsed.True;
        this.NewLine();
        this.AppendCurrent();
        this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Identifier || this.CurrentTokenType == TokenType.String)
        {
          if (this.CurrentTokenType == TokenType.Identifier || this.Settings.OutputMode == OutputMode.MultipleLines)
            this.Append((object) ' ');
          this.AppendCurrent();
          this.SkipSpace();
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "{")
        {
          if (this.Settings.BlocksStartOnSameLine == BlockStart.NewLine || this.Settings.BlocksStartOnSameLine == BlockStart.UseSource && this.m_encounteredNewLine)
            this.NewLine();
          else if (this.Settings.OutputMode == OutputMode.MultipleLines)
            this.Append((object) ' ');
          this.AppendCurrent();
          this.Indent();
          this.NewLine();
          this.SkipSpace();
          this.ParseKeyFrameBlocks();
          this.Unindent();
          this.NewLine();
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "}")
          {
            this.NewLine();
            this.AppendCurrent();
            this.SkipSpace();
          }
          else
          {
            this.ReportError(0, CssErrorCode.ExpectedClosingBrace, (object) this.CurrentTokenText);
            this.SkipToEndOfDeclaration();
          }
        }
        else
        {
          this.ReportError(0, CssErrorCode.ExpectedOpenBrace, (object) this.CurrentTokenText);
          this.SkipToEndOfStatement();
        }
      }
      return keyFrames;
    }

    private void ParseKeyFrameBlocks()
    {
      while (this.ParseKeyFrameSelectors() == CssParser.Parsed.True)
      {
        int declarationBlock = (int) this.ParseDeclarationBlock(false);
        this.m_forceNewLine = true;
      }
      this.m_forceNewLine = false;
    }

    private CssParser.Parsed ParseKeyFrameSelectors()
    {
      CssParser.Parsed keyFrameSelectors = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Percentage)
      {
        this.AppendCurrent();
        this.SkipSpace();
        keyFrameSelectors = CssParser.Parsed.True;
      }
      else if (this.CurrentTokenType == TokenType.Identifier)
      {
        string upperInvariant = this.CurrentTokenText.ToUpperInvariant();
        if (string.CompareOrdinal(upperInvariant, "FROM") == 0 || string.CompareOrdinal(upperInvariant, "TO") == 0)
        {
          this.AppendCurrent();
          this.SkipSpace();
          keyFrameSelectors = CssParser.Parsed.True;
        }
      }
      while (keyFrameSelectors == CssParser.Parsed.True && this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ",")
      {
        this.AppendCurrent();
        if (this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Percentage)
        {
          this.AppendCurrent();
          this.SkipSpace();
        }
        else if (this.CurrentTokenType == TokenType.Identifier)
        {
          string upperInvariant = this.CurrentTokenText.ToUpperInvariant();
          if (string.CompareOrdinal(upperInvariant, "FROM") == 0 || string.CompareOrdinal(upperInvariant, "TO") == 0)
          {
            this.AppendCurrent();
            this.SkipSpace();
          }
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedPercentageFromOrTo, (object) this.CurrentTokenText);
      }
      return keyFrameSelectors;
    }

    private CssParser.Parsed ParseImport()
    {
      CssParser.Parsed import = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.ImportSymbol)
      {
        this.NewLine();
        this.AppendCurrent();
        this.SkipSpace();
        if (this.CurrentTokenType != TokenType.String && this.CurrentTokenType != TokenType.Uri)
        {
          this.ReportError(0, CssErrorCode.ExpectedImport, (object) this.CurrentTokenText);
          this.SkipToEndOfStatement();
          this.AppendCurrent();
        }
        else
        {
          if (this.CurrentTokenType == TokenType.Uri || this.Settings.OutputMode == OutputMode.MultipleLines)
            this.Append((object) ' ');
          this.AppendCurrent();
          this.SkipSpace();
          int mediaQueryList = (int) this.ParseMediaQueryList(false);
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ";")
          {
            this.Append((object) ';');
            this.NewLine();
          }
          else
          {
            this.ReportError(0, CssErrorCode.ExpectedSemicolon, (object) this.CurrentTokenText);
            this.SkipToEndOfStatement();
            this.AppendCurrent();
          }
        }
        this.SkipSpace();
        import = CssParser.Parsed.True;
      }
      return import;
    }

    private CssParser.Parsed ParseMedia()
    {
      CssParser.Parsed media = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.MediaSymbol)
      {
        this.NewLine();
        this.AppendCurrent();
        this.SkipSpace();
        bool flag = false;
        if (this.ParseMediaQueryList(true) == CssParser.Parsed.True)
        {
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "{")
          {
            if (this.Settings.BlocksStartOnSameLine == BlockStart.NewLine || this.Settings.BlocksStartOnSameLine == BlockStart.UseSource && this.m_encounteredNewLine)
              this.NewLine();
            else if (this.Settings.OutputMode == OutputMode.MultipleLines)
              this.Append((object) ' ');
            this.AppendCurrent();
            this.Indent();
            flag = true;
            this.SkipSpace();
            while (this.ParseRule() == CssParser.Parsed.True || this.ParseMedia() == CssParser.Parsed.True || this.ParsePage() == CssParser.Parsed.True || this.ParseFontFace() == CssParser.Parsed.True || this.ParseAtKeyword() == CssParser.Parsed.True || this.ParseAspNetBlock() == CssParser.Parsed.True)
              this.ParseSCDOCDCComments();
          }
          else
            this.SkipToEndOfStatement();
          if (this.CurrentTokenType == TokenType.Character)
          {
            if (this.CurrentTokenText == ";")
            {
              this.AppendCurrent();
              if (flag)
                this.Unindent();
              this.NewLine();
            }
            else if (this.CurrentTokenText == "}")
            {
              if (flag)
                this.Unindent();
              this.NewLine();
              this.AppendCurrent();
            }
            else
            {
              this.SkipToEndOfStatement();
              this.AppendCurrent();
            }
          }
          else
          {
            this.SkipToEndOfStatement();
            this.AppendCurrent();
          }
          this.SkipSpace();
          media = CssParser.Parsed.True;
        }
        else
          this.SkipToEndOfStatement();
      }
      return media;
    }

    private CssParser.Parsed ParseMediaQueryList(bool mightNeedSpace)
    {
      CssParser.Parsed mediaQuery = this.ParseMediaQuery(mightNeedSpace);
      while (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ",")
      {
        this.AppendCurrent();
        this.SkipSpace();
        if (this.ParseMediaQuery(false) != CssParser.Parsed.True)
          this.ReportError(0, CssErrorCode.ExpectedMediaQuery, (object) this.CurrentTokenText);
      }
      return mediaQuery;
    }

    private CssParser.Parsed ParseMediaQuery(bool firstQuery)
    {
      CssParser.Parsed mediaQuery = CssParser.Parsed.False;
      bool flag = firstQuery;
      if (this.CurrentTokenType == TokenType.Identifier && (string.Compare(this.CurrentTokenText, "ONLY", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.CurrentTokenText, "NOT", StringComparison.OrdinalIgnoreCase) == 0))
      {
        if (firstQuery || this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        this.AppendCurrent();
        this.SkipSpace();
        flag = true;
      }
      if (this.CurrentTokenType == TokenType.Identifier)
      {
        if (flag || this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        this.AppendCurrent();
        this.SkipSpace();
        flag = true;
        mediaQuery = CssParser.Parsed.True;
      }
      else if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "(")
      {
        this.ParseMediaQueryExpression();
        flag = true;
        mediaQuery = CssParser.Parsed.True;
      }
      else if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ";")
        this.ReportError(0, CssErrorCode.ExpectedMediaIdentifier, (object) this.CurrentTokenText);
      while (this.CurrentTokenType == TokenType.Identifier && string.Compare(this.CurrentTokenText, "AND", StringComparison.OrdinalIgnoreCase) == 0 || this.CurrentTokenType == TokenType.Function && string.Compare(this.CurrentTokenText, "AND(", StringComparison.OrdinalIgnoreCase) == 0)
      {
        if (flag || this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        if (this.CurrentTokenType == TokenType.Function)
        {
          this.ReportError(1, CssErrorCode.MediaQueryRequiresSpace, (object) this.CurrentTokenText);
          this.Append((object) "and (");
          this.SkipSpace();
          this.ParseMediaQueryExpression();
        }
        else
        {
          this.AppendCurrent();
          this.SkipSpace();
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "(")
          {
            this.Append((object) ' ');
            this.ParseMediaQueryExpression();
          }
          else
          {
            this.ReportError(0, CssErrorCode.ExpectedMediaQueryExpression, (object) this.CurrentTokenText);
            break;
          }
        }
      }
      return mediaQuery;
    }

    private void ParseMediaQueryExpression()
    {
      if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "(")
      {
        this.AppendCurrent();
        this.SkipSpace();
      }
      if (this.CurrentTokenType == TokenType.Identifier)
      {
        this.AppendCurrent();
        this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ":")
        {
          this.AppendCurrent();
          this.SkipSpace();
          if (this.Settings.OutputMode == OutputMode.MultipleLines)
            this.Append((object) ' ');
          if (this.ParseExpr() != CssParser.Parsed.True)
            this.ReportError(0, CssErrorCode.ExpectedExpression, (object) this.CurrentTokenText);
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
          {
            this.AppendCurrent();
            this.SkipSpace();
          }
          else
            this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
        }
        else if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
        {
          this.AppendCurrent();
          this.SkipSpace();
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
      }
      else
        this.ReportError(0, CssErrorCode.ExpectedMediaFeature, (object) this.CurrentTokenText);
    }

    private CssParser.Parsed ParseDeclarationBlock(bool allowMargins)
    {
      if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != "{")
      {
        this.ReportError(0, CssErrorCode.ExpectedOpenBrace, (object) this.CurrentTokenText);
        this.SkipToEndOfStatement();
        this.AppendCurrent();
        this.SkipSpace();
      }
      else
      {
        if (this.Settings.BlocksStartOnSameLine == BlockStart.NewLine || this.Settings.BlocksStartOnSameLine == BlockStart.UseSource && this.m_encounteredNewLine)
          this.NewLine();
        else if (this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        this.Append((object) '{');
        this.Indent();
        this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "}")
        {
          this.Unindent();
          this.AppendCurrent();
          this.SkipSpace();
        }
        else
        {
          int declarationList = (int) this.ParseDeclarationList(allowMargins);
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "}")
          {
            this.Unindent();
            this.NewLine();
            this.Append((object) '}');
            this.SkipSpace();
          }
          else if (this.m_scanner.EndOfFile)
            this.ReportError(0, CssErrorCode.UnexpectedEndOfFile);
          else
            this.ReportError(0, CssErrorCode.ExpectedClosingBrace, (object) this.CurrentTokenText);
        }
      }
      return CssParser.Parsed.True;
    }

    private CssParser.Parsed ParseDeclarationList(bool allowMargins)
    {
      CssParser.Parsed declarationList = CssParser.Parsed.Empty;
      while (!this.m_scanner.EndOfFile)
      {
        if (this.m_lineLength >= this.Settings.LineBreakThreshold)
          this.AddNewLine();
        CssParser.Parsed declaration = this.ParseDeclaration();
        if (declarationList == CssParser.Parsed.Empty && declaration != CssParser.Parsed.Empty)
          declarationList = declaration;
        bool flag = false;
        if (allowMargins && declaration == CssParser.Parsed.Empty)
          flag = this.ParseMargin() == CssParser.Parsed.True;
        if (!flag && (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ";" && this.CurrentTokenText != "}") && !this.m_scanner.EndOfFile)
        {
          this.ReportError(0, CssErrorCode.ExpectedSemicolonOrClosingBrace, (object) this.CurrentTokenText);
          this.SkipToEndOfDeclaration();
        }
        if (this.m_scanner.EndOfFile)
        {
          if (this.Settings.TermSemicolons)
            this.Append((object) ';');
        }
        else
        {
          if (this.CurrentTokenText == "}")
          {
            if (this.Settings.TermSemicolons && declaration == CssParser.Parsed.True)
            {
              this.Append((object) ';');
              break;
            }
            break;
          }
          if (this.CurrentTokenText == ";")
          {
            if (this.Settings.TermSemicolons)
            {
              this.Append((object) ';');
              this.SkipSpace();
            }
            else
            {
              string str = this.NextSignificantToken();
              if (this.m_scanner.EndOfFile)
              {
                if (str.Length > 0)
                {
                  if (str != "/* */" && str != "/**/")
                    this.Append((object) ';');
                  this.Append((object) str);
                  this.m_outputNewLine = true;
                  this.m_lineLength = 0;
                  break;
                }
                break;
              }
              if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != "}" && this.CurrentTokenText != ";" || str.Length > 0 && str != "/* */" && str != "/**/")
                this.Append((object) ';');
              if (str.Length > 0)
              {
                this.Append((object) str);
                this.m_outputNewLine = true;
                this.m_lineLength = 0;
              }
            }
          }
        }
      }
      return declarationList;
    }

    private CssParser.Parsed ParsePage()
    {
      CssParser.Parsed page = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.PageSymbol)
      {
        this.NewLine();
        this.AppendCurrent();
        this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Identifier)
        {
          this.Append((object) ' ');
          this.AppendCurrent();
          int num = (int) this.NextToken();
        }
        int pseudoPage = (int) this.ParsePseudoPage();
        if (this.CurrentTokenType == TokenType.Space)
          this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "{")
        {
          page = this.ParseDeclarationBlock(true);
          this.NewLine();
        }
        else
        {
          this.SkipToEndOfStatement();
          this.AppendCurrent();
          this.SkipSpace();
        }
      }
      return page;
    }

    private CssParser.Parsed ParsePseudoPage()
    {
      CssParser.Parsed pseudoPage = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ":")
      {
        this.Append((object) ':');
        int num1 = (int) this.NextToken();
        if (this.CurrentTokenType != TokenType.Identifier)
          this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
        this.AppendCurrent();
        int num2 = (int) this.NextToken();
        pseudoPage = CssParser.Parsed.True;
      }
      return pseudoPage;
    }

    private CssParser.Parsed ParseMargin()
    {
      CssParser.Parsed margin = CssParser.Parsed.Empty;
      switch (this.CurrentTokenType)
      {
        case TokenType.TopLeftCornerSymbol:
        case TokenType.TopLeftSymbol:
        case TokenType.TopCenterSymbol:
        case TokenType.TopRightSymbol:
        case TokenType.TopRightCornerSymbol:
        case TokenType.BottomLeftCornerSymbol:
        case TokenType.BottomLeftSymbol:
        case TokenType.BottomCenterSymbol:
        case TokenType.BottomRightSymbol:
        case TokenType.BottomRightCornerSymbol:
        case TokenType.LeftTopSymbol:
        case TokenType.LeftMiddleSymbol:
        case TokenType.LeftBottomSymbol:
        case TokenType.RightTopSymbol:
        case TokenType.RightMiddleSymbol:
        case TokenType.RightBottomSymbol:
          this.NewLine();
          this.AppendCurrent();
          this.SkipSpace();
          margin = this.ParseDeclarationBlock(false);
          this.NewLine();
          break;
      }
      return margin;
    }

    private CssParser.Parsed ParseFontFace()
    {
      CssParser.Parsed fontFace = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.FontFaceSymbol)
      {
        this.NewLine();
        this.AppendCurrent();
        this.SkipSpace();
        fontFace = this.ParseDeclarationBlock(false);
        this.NewLine();
      }
      return fontFace;
    }

    private CssParser.Parsed ParseOperator()
    {
      CssParser.Parsed parsed = CssParser.Parsed.Empty;
      if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "/" || this.CurrentTokenText == ","))
      {
        this.AppendCurrent();
        this.SkipSpace();
        parsed = CssParser.Parsed.True;
      }
      return parsed;
    }

    private CssParser.Parsed ParseCombinator()
    {
      CssParser.Parsed combinator = CssParser.Parsed.Empty;
      if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "+" || this.CurrentTokenText == ">" || this.CurrentTokenText == "~"))
      {
        this.AppendCurrent();
        this.SkipSpace();
        combinator = CssParser.Parsed.True;
      }
      return combinator;
    }

    private CssParser.Parsed ParseRule()
    {
      if (this.m_lineLength >= this.Settings.LineBreakThreshold)
        this.AddNewLine();
      this.m_forceNewLine = true;
      CssParser.Parsed rule = this.ParseSelector();
      if (rule == CssParser.Parsed.True)
      {
        if (this.m_scanner.EndOfFile)
          this.ReportError(0, CssErrorCode.UnexpectedEndOfFile);
        while (!this.m_scanner.EndOfFile)
        {
          if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != "," && this.CurrentTokenText != "{")
          {
            this.ReportError(0, CssErrorCode.ExpectedCommaOrOpenBrace, (object) this.CurrentTokenText);
            this.SkipToEndOfStatement();
            this.AppendCurrent();
            this.SkipSpace();
            break;
          }
          if (this.CurrentTokenText == "{")
          {
            if (this.m_lastOutputString == "first-letter" || this.m_lastOutputString == "first-line")
              this.Append((object) ' ');
            rule = this.ParseDeclarationBlock(false);
            break;
          }
          this.Append((object) ',');
          if (this.m_lineLength >= this.Settings.LineBreakThreshold)
            this.AddNewLine();
          else if (this.Settings.OutputMode == OutputMode.MultipleLines)
            this.Append((object) ' ');
          this.SkipSpace();
          if (this.ParseSelector() != CssParser.Parsed.True)
          {
            if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "{")
            {
              this.ReportError(4, CssErrorCode.ExpectedSelector, (object) this.CurrentTokenText);
            }
            else
            {
              this.ReportError(0, CssErrorCode.ExpectedSelector, (object) this.CurrentTokenText);
              this.SkipToEndOfStatement();
              this.AppendCurrent();
              this.SkipSpace();
              break;
            }
          }
        }
      }
      return rule;
    }

    private CssParser.Parsed ParseSelector()
    {
      CssParser.Parsed selector = this.ParseSimpleSelector();
      if (selector == CssParser.Parsed.False && this.CurrentTokenType != TokenType.None)
      {
        CssContext context = this.m_currentToken.Context;
        string currentTokenText = this.CurrentTokenText;
        selector = this.ParseCombinator();
        if (selector == CssParser.Parsed.True)
          this.ReportError(4, CssErrorCode.HackGeneratesInvalidCss, context, (object) currentTokenText);
      }
      if (selector == CssParser.Parsed.True)
      {
        bool flag = this.SkipIfSpace();
        while (!this.m_scanner.EndOfFile)
        {
          if (this.ParseCombinator() != CssParser.Parsed.True)
          {
            if (this.CurrentTokenType != TokenType.Character || !(this.CurrentTokenText == ",") && !(this.CurrentTokenText == "{"))
            {
              if (flag)
                this.Append((object) ' ');
            }
            else
              break;
          }
          if (this.ParseSimpleSelector() == CssParser.Parsed.False)
          {
            this.ReportError(0, CssErrorCode.ExpectedSelector, (object) this.CurrentTokenText);
            break;
          }
          flag = this.SkipIfSpace();
        }
      }
      return selector;
    }

    private CssParser.Parsed ParseSimpleSelector()
    {
      CssParser.Parsed simpleSelector = this.ParseElementName();
      while (!this.m_scanner.EndOfFile)
      {
        if (this.CurrentTokenType == TokenType.Hash)
        {
          this.AppendCurrent();
          int num = (int) this.NextToken();
          simpleSelector = CssParser.Parsed.True;
        }
        else if (this.ParseClass() == CssParser.Parsed.True)
          simpleSelector = CssParser.Parsed.True;
        else if (this.ParseAttrib() == CssParser.Parsed.True)
          simpleSelector = CssParser.Parsed.True;
        else if (this.ParsePseudo() == CssParser.Parsed.True)
          simpleSelector = CssParser.Parsed.True;
        else
          break;
      }
      return simpleSelector;
    }

    private CssParser.Parsed ParseClass()
    {
      CssParser.Parsed parsed = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ".")
      {
        this.AppendCurrent();
        int num1 = (int) this.NextToken();
        if (this.CurrentTokenType == TokenType.Identifier)
        {
          this.AppendCurrent();
          int num2 = (int) this.NextToken();
          parsed = CssParser.Parsed.True;
        }
        else if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "%")
        {
          this.UpdateIfReplacementToken();
          if (this.CurrentTokenType == TokenType.ReplacementToken)
          {
            this.AppendCurrent();
            int num3 = (int) this.NextToken();
            parsed = CssParser.Parsed.True;
          }
          else
            this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
      }
      else if (this.CurrentTokenType == TokenType.Dimension || this.CurrentTokenType == TokenType.Number)
      {
        string rawNumber = this.m_scanner.RawNumber;
        if (rawNumber != null && rawNumber.StartsWith(".", StringComparison.Ordinal))
        {
          parsed = CssParser.Parsed.True;
          int num4 = (int) this.NextToken();
          if (this.CurrentTokenType == TokenType.Identifier)
          {
            rawNumber += this.CurrentTokenText;
            int num5 = (int) this.NextToken();
          }
          this.ReportError(2, CssErrorCode.PossibleInvalidClassName, (object) rawNumber);
          this.Append((object) rawNumber);
        }
      }
      return parsed;
    }

    private CssParser.Parsed ParseElementName()
    {
      CssParser.Parsed elementName = CssParser.Parsed.False;
      bool flag = false;
      if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "|")
      {
        flag = true;
        this.AppendCurrent();
        int num = (int) this.NextToken();
      }
      if (this.CurrentTokenType == TokenType.Identifier || this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "*")
      {
        string currentTokenText = flag ? (string) null : this.CurrentTokenText;
        this.AppendCurrent();
        int num1 = (int) this.NextToken();
        elementName = CssParser.Parsed.True;
        if (!flag && this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "|")
        {
          this.ValidateNamespace(currentTokenText);
          this.AppendCurrent();
          int num2 = (int) this.NextToken();
          if (this.CurrentTokenType == TokenType.Identifier || this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "*")
          {
            this.AppendCurrent();
            int num3 = (int) this.NextToken();
          }
          else
          {
            elementName = CssParser.Parsed.False;
            this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
          }
        }
      }
      else if (flag)
        this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
      return elementName;
    }

    private CssParser.Parsed ParseAttrib()
    {
      CssParser.Parsed attrib = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "[")
      {
        this.Append((object) '[');
        this.SkipSpace();
        bool flag = false;
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "|")
        {
          flag = true;
          this.AppendCurrent();
          int num = (int) this.NextToken();
        }
        if (this.CurrentTokenType == TokenType.Identifier || this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "*")
        {
          string currentTokenText = flag ? (string) null : this.CurrentTokenText;
          this.AppendCurrent();
          this.SkipSpace();
          if (!flag && this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "|")
          {
            this.ValidateNamespace(currentTokenText);
            this.AppendCurrent();
            this.SkipSpace();
            if (this.CurrentTokenType == TokenType.Identifier || this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "*")
            {
              this.AppendCurrent();
              this.SkipSpace();
            }
            else
              this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
          }
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "=" || this.CurrentTokenType == TokenType.Includes || this.CurrentTokenType == TokenType.DashMatch || this.CurrentTokenType == TokenType.PrefixMatch || this.CurrentTokenType == TokenType.SuffixMatch || this.CurrentTokenType == TokenType.SubstringMatch)
        {
          this.AppendCurrent();
          this.SkipSpace();
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "%")
          {
            this.UpdateIfReplacementToken();
            if (this.CurrentTokenType != TokenType.ReplacementToken)
              this.ReportError(0, CssErrorCode.ExpectedIdentifierOrString, (object) this.CurrentTokenText);
          }
          else if (this.CurrentTokenType != TokenType.Identifier && this.CurrentTokenType != TokenType.String)
            this.ReportError(0, CssErrorCode.ExpectedIdentifierOrString, (object) this.CurrentTokenText);
          this.AppendCurrent();
          this.SkipSpace();
        }
        if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != "]")
          this.ReportError(0, CssErrorCode.ExpectedClosingBracket, (object) this.CurrentTokenText);
        this.Append((object) ']');
        int num1 = (int) this.NextToken();
        attrib = CssParser.Parsed.True;
      }
      return attrib;
    }

    private CssParser.Parsed ParsePseudo()
    {
      CssParser.Parsed pseudo = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ":")
      {
        this.Append((object) ':');
        int num1 = (int) this.NextToken();
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ":")
        {
          this.Append((object) ':');
          int num2 = (int) this.NextToken();
        }
        switch (this.CurrentTokenType)
        {
          case TokenType.Identifier:
            this.AppendCurrent();
            int num3 = (int) this.NextToken();
            break;
          case TokenType.Function:
            this.AppendCurrent();
            this.SkipSpace();
            int expression1 = (int) this.ParseExpression();
            while (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ",")
            {
              this.AppendCurrent();
              int num4 = (int) this.NextToken();
              int expression2 = (int) this.ParseExpression();
            }
            if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ")")
              this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
            this.AppendCurrent();
            int num5 = (int) this.NextToken();
            break;
          case TokenType.Not:
            this.AppendCurrent();
            this.SkipSpace();
            int simpleSelector = (int) this.ParseSimpleSelector();
            this.SkipIfSpace();
            if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ")")
              this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
            this.AppendCurrent();
            int num6 = (int) this.NextToken();
            break;
          default:
            this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
            break;
        }
        pseudo = CssParser.Parsed.True;
      }
      return pseudo;
    }

    private CssParser.Parsed ParseExpression()
    {
      CssParser.Parsed expression = CssParser.Parsed.Empty;
      while (true)
      {
        switch (this.CurrentTokenType)
        {
          case TokenType.Space:
            int num1 = (int) this.NextToken();
            continue;
          case TokenType.String:
          case TokenType.Identifier:
          case TokenType.Dimension:
          case TokenType.Number:
            expression = CssParser.Parsed.True;
            this.AppendCurrent();
            int num2 = (int) this.NextToken();
            continue;
          case TokenType.Character:
            if (this.CurrentTokenText == "+" || this.CurrentTokenText == "-")
            {
              expression = CssParser.Parsed.True;
              this.AppendCurrent();
              int num3 = (int) this.NextToken();
              continue;
            }
            goto label_6;
          default:
            goto label_7;
        }
      }
label_6:
      return expression;
label_7:
      return expression;
    }

    private CssParser.Parsed ParseDeclaration()
    {
      CssParser.Parsed declaration = CssParser.Parsed.Empty;
      string str = (string) null;
      if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "*" || this.CurrentTokenText == "."))
      {
        this.ReportError(4, CssErrorCode.HackGeneratesInvalidCss, (object) this.CurrentTokenText);
        str = this.CurrentTokenText;
        int num = (int) this.NextToken();
      }
      if (this.CurrentTokenType == TokenType.Identifier)
      {
        string currentTokenText = this.CurrentTokenText;
        this.NewLine();
        if (str != null)
          this.Append((object) str);
        this.AppendCurrent();
        this.SkipSpaceComment();
        if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ":")
        {
          this.ReportError(0, CssErrorCode.ExpectedColon, (object) this.CurrentTokenText);
          this.SkipToEndOfDeclaration();
          return CssParser.Parsed.True;
        }
        this.Append((object) ':');
        if (this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        this.SkipSpace();
        if (this.m_valueReplacement != null)
        {
          this.Append((object) this.m_valueReplacement);
          this.m_valueReplacement = (string) null;
          this.m_noOutput = true;
          int expr = (int) this.ParseExpr();
          this.m_noOutput = false;
        }
        else
        {
          this.m_parsingColorValue = CssParser.MightContainColorNames(currentTokenText);
          CssParser.Parsed expr = this.ParseExpr();
          this.m_parsingColorValue = false;
          if (expr != CssParser.Parsed.True)
          {
            this.ReportError(0, CssErrorCode.ExpectedExpression, (object) this.CurrentTokenText);
            this.SkipToEndOfDeclaration();
            return CssParser.Parsed.True;
          }
        }
        int prio = (int) this.ParsePrio();
        declaration = CssParser.Parsed.True;
      }
      return declaration;
    }

    private CssParser.Parsed ParsePrio()
    {
      CssParser.Parsed prio = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.ImportantSymbol)
      {
        if (this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        this.AppendCurrent();
        this.SkipSpace();
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "!")
        {
          this.ReportError(4, CssErrorCode.HackGeneratesInvalidCss, (object) this.CurrentTokenText);
          this.AppendCurrent();
          this.SkipSpace();
        }
        prio = CssParser.Parsed.True;
      }
      else if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "!")
      {
        if (this.Settings.OutputMode == OutputMode.MultipleLines)
          this.Append((object) ' ');
        this.AppendCurrent();
        int num = (int) this.NextToken();
        if (this.CurrentTokenType == TokenType.Identifier)
        {
          this.ReportError(4, CssErrorCode.HackGeneratesInvalidCss, (object) this.CurrentTokenText);
          this.AppendCurrent();
          this.SkipSpace();
          prio = CssParser.Parsed.True;
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedIdentifier, (object) this.CurrentTokenText);
      }
      return prio;
    }

    private CssParser.Parsed ParseExpr()
    {
      CssParser.Parsed term = this.ParseTerm(false);
      if (term == CssParser.Parsed.True)
      {
        while (!this.m_scanner.EndOfFile)
        {
          CssParser.Parsed parsed = this.ParseOperator();
          if (parsed != CssParser.Parsed.False && this.ParseTerm(parsed == CssParser.Parsed.Empty) == CssParser.Parsed.False)
            break;
        }
      }
      return term;
    }

    private CssParser.Parsed ParseFunctionParameters()
    {
      CssParser.Parsed functionParameters = this.ParseTerm(false);
      switch (functionParameters)
      {
        case CssParser.Parsed.True:
          while (!this.m_scanner.EndOfFile)
          {
            if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "=")
            {
              this.AppendCurrent();
              this.SkipSpace();
              int term = (int) this.ParseTerm(false);
            }
            CssParser.Parsed parsed = this.ParseOperator();
            if (parsed != CssParser.Parsed.False && this.ParseTerm(parsed == CssParser.Parsed.Empty) == CssParser.Parsed.False)
              break;
          }
          break;
        case CssParser.Parsed.False:
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
          {
            functionParameters = CssParser.Parsed.Empty;
            break;
          }
          break;
      }
      return functionParameters;
    }

    private CssParser.Parsed ParseTerm(bool wasEmpty)
    {
      CssParser.Parsed term = CssParser.Parsed.False;
      bool flag = false;
      if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "-" || this.CurrentTokenText == "+"))
      {
        if (wasEmpty)
        {
          if (this.m_skippedSpace)
            this.Append((object) ' ');
          wasEmpty = false;
        }
        this.AppendCurrent();
        int num = (int) this.NextToken();
        flag = true;
      }
      switch (this.CurrentTokenType)
      {
        case TokenType.String:
        case TokenType.Identifier:
        case TokenType.Uri:
        case TokenType.UnicodeRange:
          if (flag)
            this.ReportError(0, CssErrorCode.TokenAfterUnaryNotAllowed, (object) this.CurrentTokenText);
          if (wasEmpty)
          {
            if (this.m_skippedSpace)
              this.Append((object) ' ');
            wasEmpty = false;
          }
          this.AppendCurrent();
          this.SkipSpace();
          term = CssParser.Parsed.True;
          break;
        case TokenType.Hash:
          if (flag)
            this.ReportError(0, CssErrorCode.HashAfterUnaryNotAllowed, (object) this.CurrentTokenText);
          if (wasEmpty)
          {
            this.Append((object) ' ');
            wasEmpty = false;
          }
          if (this.ParseHexcolor() == CssParser.Parsed.False)
          {
            this.ReportError(0, CssErrorCode.ExpectedHexColor, (object) this.CurrentTokenText);
            this.AppendCurrent();
            this.SkipSpace();
          }
          term = CssParser.Parsed.True;
          break;
        case TokenType.RelativeLength:
        case TokenType.AbsoluteLength:
        case TokenType.Resolution:
        case TokenType.Angle:
        case TokenType.Time:
        case TokenType.Frequency:
        case TokenType.Percentage:
        case TokenType.Number:
          if (wasEmpty)
          {
            this.Append((object) ' ');
            wasEmpty = false;
          }
          this.AppendCurrent();
          this.SkipSpace();
          term = CssParser.Parsed.True;
          break;
        case TokenType.Dimension:
          this.ReportError(2, CssErrorCode.UnexpectedDimension, (object) this.CurrentTokenText);
          goto case TokenType.RelativeLength;
        case TokenType.Function:
          if (wasEmpty)
          {
            this.Append((object) ' ');
            wasEmpty = false;
          }
          if (this.ParseFunction() == CssParser.Parsed.False)
            this.ReportError(0, CssErrorCode.ExpectedFunction, (object) this.CurrentTokenText);
          term = CssParser.Parsed.True;
          break;
        case TokenType.ProgId:
          if (wasEmpty)
          {
            this.Append((object) ' ');
            wasEmpty = false;
          }
          if (this.ParseProgId() == CssParser.Parsed.False)
            this.ReportError(0, CssErrorCode.ExpectedProgId, (object) this.CurrentTokenText);
          term = CssParser.Parsed.True;
          break;
        case TokenType.Character:
          if (this.CurrentTokenText == "(")
          {
            if (wasEmpty)
            {
              if (this.m_skippedSpace)
                this.Append((object) ' ');
              wasEmpty = false;
            }
            this.AppendCurrent();
            this.SkipSpace();
            if (this.ParseExpr() == CssParser.Parsed.False)
              this.ReportError(0, CssErrorCode.ExpectedExpression, (object) this.CurrentTokenText);
            if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
            {
              this.AppendCurrent();
              term = CssParser.Parsed.True;
              this.m_skippedSpace = false;
              int num = (int) this.NextRawToken();
              if (this.CurrentTokenType == TokenType.Space)
                this.m_skippedSpace = true;
              if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "[")
              {
                this.AppendCurrent();
                this.SkipSpace();
                if (this.CurrentTokenType == TokenType.Number)
                {
                  this.AppendCurrent();
                  this.SkipSpace();
                  if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "]")
                  {
                    this.AppendCurrent();
                    this.SkipSpace();
                    break;
                  }
                  this.ReportError(0, CssErrorCode.ExpectedClosingBracket, (object) this.CurrentTokenText);
                  term = CssParser.Parsed.False;
                  break;
                }
                this.ReportError(0, CssErrorCode.ExpectedNumber, (object) this.CurrentTokenText);
                term = CssParser.Parsed.False;
                break;
              }
              break;
            }
            this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
            break;
          }
          if (this.CurrentTokenText == "%")
          {
            this.UpdateIfReplacementToken();
            if (this.CurrentTokenType == TokenType.ReplacementToken)
            {
              if (wasEmpty)
              {
                this.Append((object) ' ');
                wasEmpty = false;
              }
              this.AppendCurrent();
              this.SkipSpace();
              term = CssParser.Parsed.True;
              break;
            }
            goto default;
          }
          else
            goto default;
        default:
          if (flag)
          {
            this.ReportError(0, CssErrorCode.UnexpectedToken, (object) this.CurrentTokenText);
            break;
          }
          break;
      }
      return term;
    }

    private CssParser.Parsed ParseProgId()
    {
      CssParser.Parsed progId = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.ProgId)
      {
        this.ReportError(4, CssErrorCode.ProgIdIEOnly);
        this.m_noColorAbbreviation = true;
        this.AppendCurrent();
        this.SkipSpace();
        while (this.CurrentTokenType == TokenType.Identifier)
        {
          this.AppendCurrent();
          this.SkipSpace();
          if (this.CurrentTokenType != TokenType.Character && this.CurrentTokenText != "=")
            this.ReportError(0, CssErrorCode.ExpectedEqualSign, (object) this.CurrentTokenText);
          this.Append((object) '=');
          this.SkipSpace();
          if (this.ParseTerm(false) != CssParser.Parsed.True)
            this.ReportError(0, CssErrorCode.ExpectedTerm, (object) this.CurrentTokenText);
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ",")
          {
            this.Append((object) ',');
            this.SkipSpace();
          }
        }
        this.m_noColorAbbreviation = false;
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
        {
          this.Append((object) ')');
          this.SkipSpace();
        }
        else
          this.ReportError(0, CssErrorCode.UnexpectedToken, (object) this.CurrentTokenText);
        progId = CssParser.Parsed.True;
      }
      return progId;
    }

    private static string GetRoot(string text)
    {
      if (text.StartsWith("-", StringComparison.Ordinal))
      {
        Match match = CssParser.s_vendorSpecific.Match(text);
        if (match.Success)
          text = match.Result("${root}");
      }
      return text;
    }

    private CssParser.Parsed ParseFunction()
    {
      CssParser.Parsed function = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Function)
      {
        switch (CssParser.GetRoot(this.CurrentTokenText).ToUpperInvariant())
        {
          case "RGB(":
            function = this.ParseRgb();
            break;
          case "EXPRESSION(":
            function = this.ParseExpressionFunction();
            break;
          case "CALC(":
            function = this.ParseCalc();
            break;
          case "MIN(":
          case "MAX(":
            function = this.ParseMinMax();
            break;
          default:
            this.AppendCurrent();
            this.SkipSpace();
            if (this.ParseFunctionParameters() == CssParser.Parsed.False)
              this.ReportError(0, CssErrorCode.ExpectedExpression, (object) this.CurrentTokenText);
            if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
            {
              this.AppendCurrent();
              this.SkipSpace();
              function = CssParser.Parsed.True;
              break;
            }
            this.ReportError(0, CssErrorCode.UnexpectedToken, (object) this.CurrentTokenText);
            break;
        }
      }
      return function;
    }

    private CssParser.Parsed ParseRgb()
    {
      CssParser.Parsed rgb = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Function && string.Compare(this.CurrentTokenText, "rgb(", StringComparison.OrdinalIgnoreCase) == 0)
      {
        bool flag1 = false;
        bool flag2 = false;
        int[] numArray = new int[3];
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(this.CurrentTokenText.ToLowerInvariant());
        string str1 = this.NextSignificantToken();
        if (str1.Length > 0)
        {
          stringBuilder.Append(str1);
          flag1 = true;
        }
        for (int index = 0; index < 3; ++index)
        {
          if (index > 0)
          {
            if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ",")
            {
              stringBuilder.Append(',');
            }
            else
            {
              if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
              {
                this.ReportError(0, CssErrorCode.ExpectedComma, (object) this.CurrentTokenText);
                flag1 = true;
                break;
              }
              this.ReportError(0, CssErrorCode.ExpectedComma, (object) this.CurrentTokenText);
              stringBuilder.Append(this.CurrentTokenText);
              flag1 = true;
            }
            string str2 = this.NextSignificantToken();
            if (str2.Length > 0)
            {
              stringBuilder.Append(str2);
              flag1 = true;
            }
          }
          bool flag3 = false;
          if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == "-")
          {
            flag3 = true;
            string str3 = this.NextSignificantToken();
            if (str3.Length > 0)
            {
              stringBuilder.Append(str3);
              flag1 = true;
            }
          }
          string text = this.CurrentTokenText;
          if (this.CurrentTokenType != TokenType.Number && this.CurrentTokenType != TokenType.Percentage)
          {
            this.ReportError(0, CssErrorCode.ExpectedRgbNumberOrPercentage, (object) this.CurrentTokenText);
            flag1 = true;
          }
          else if (this.CurrentTokenType == TokenType.Number)
          {
            float number;
            if (text.TryParseSingleInvariant(out number))
            {
              number *= flag3 ? -1f : 1f;
              if ((double) number < 0.0)
              {
                text = "0";
                numArray[index] = 0;
              }
              else if ((double) number > (double) byte.MaxValue)
              {
                text = "255";
                numArray[index] = (int) byte.MaxValue;
              }
              else
                numArray[index] = Convert.ToInt32(number);
            }
            else
              flag1 = true;
          }
          else
          {
            float number;
            if (text.Substring(0, text.Length - 1).TryParseSingleInvariant(out number))
            {
              number *= flag3 ? -1f : 1f;
              if ((double) number < 0.0)
              {
                text = "0%";
                numArray[index] = 0;
              }
              else if ((double) number > 100.0)
              {
                text = "100%";
                numArray[index] = (int) byte.MaxValue;
              }
              else
                numArray[index] = Convert.ToInt32((float) ((double) number * (double) byte.MaxValue / 100.0));
            }
            else
              flag1 = true;
          }
          stringBuilder.Append(text);
          string str4 = this.NextSignificantToken();
          if (str4.Length > 0)
          {
            stringBuilder.Append(str4);
            flag1 = true;
          }
        }
        if (flag1)
        {
          this.Append((object) stringBuilder.ToString());
        }
        else
        {
          this.Append((object) CssParser.CrunchHexColor("#{0:x2}{1:x2}{2:x2}".FormatInvariant((object) numArray[0], (object) numArray[1], (object) numArray[2]), this.Settings.ColorNames, this.m_noColorAbbreviation));
          flag2 = true;
        }
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
        {
          if (!flag2)
            this.AppendCurrent();
          this.SkipSpace();
          rgb = CssParser.Parsed.True;
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
      }
      return rgb;
    }

    private CssParser.Parsed ParseExpressionFunction()
    {
      CssParser.Parsed expressionFunction = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Function && string.Compare(this.CurrentTokenText, "expression(", StringComparison.OrdinalIgnoreCase) == 0)
      {
        this.Append((object) this.CurrentTokenText.ToLowerInvariant());
        int num1 = (int) this.NextToken();
        StringBuilder stringBuilder = new StringBuilder();
        int num2 = 0;
        while (!this.m_scanner.EndOfFile && (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ")" || num2 > 0))
        {
          if (this.CurrentTokenType == TokenType.Function)
            ++num2;
          else if (this.CurrentTokenType == TokenType.Character)
          {
            switch (this.CurrentTokenText)
            {
              case "(":
                ++num2;
                break;
              case ")":
                --num2;
                break;
            }
          }
          stringBuilder.Append(this.CurrentTokenText);
          int num3 = (int) this.NextToken();
        }
        string source = stringBuilder.ToString();
        if (this.Settings.MinifyExpressions)
        {
          JSParser jsParser = new JSParser();
          bool containsErrors = false;
          jsParser.CompilerError += (EventHandler<ContextErrorEventArgs>) ((sender, ea) =>
          {
            this.ReportError(0, CssErrorCode.ExpressionError, (object) ea.Error.Message);
            containsErrors = true;
          });
          Block node = jsParser.Parse(new DocumentContext(source)
          {
            FileContext = this.FileContext
          }, this.m_jsSettings);
          if (node != null && !containsErrors)
            this.Append((object) OutputVisitor.Apply((AstNode) node, jsParser.Settings));
          else
            this.Append((object) source);
        }
        else
          this.Append((object) source);
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
        {
          this.AppendCurrent();
          this.SkipSpace();
          expressionFunction = CssParser.Parsed.True;
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
      }
      return expressionFunction;
    }

    private CssParser.Parsed ParseHexcolor()
    {
      CssParser.Parsed hexcolor = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Hash)
      {
        string hexColor = this.CurrentTokenText;
        bool flag = false;
        if ((hexColor.Length == 5 || hexColor.Length == 8 || hexColor.Length == 10) && hexColor.EndsWith("\t", StringComparison.Ordinal))
        {
          hexColor = hexColor.Substring(0, hexColor.Length - 1);
          flag = true;
        }
        if (hexColor.Length == 4 || hexColor.Length == 7 || hexColor.Length == 9)
        {
          hexcolor = CssParser.Parsed.True;
          this.Append((object) CssParser.CrunchHexColor(hexColor, this.Settings.ColorNames, this.m_noColorAbbreviation));
          if (flag)
            this.Append((object) "\\9");
          this.SkipSpace();
        }
      }
      return hexcolor;
    }

    private CssParser.Parsed ParseUnit()
    {
      CssParser.Parsed unit = CssParser.Parsed.Empty;
      if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "+" || this.CurrentTokenText == "-"))
      {
        this.AppendCurrent();
        int num = (int) this.NextToken();
        unit = CssParser.Parsed.False;
      }
      switch (this.CurrentTokenType)
      {
        case TokenType.RelativeLength:
        case TokenType.AbsoluteLength:
        case TokenType.Resolution:
        case TokenType.Angle:
        case TokenType.Time:
        case TokenType.Frequency:
        case TokenType.Dimension:
        case TokenType.Percentage:
        case TokenType.Number:
          this.AppendCurrent();
          this.SkipSpace();
          unit = CssParser.Parsed.True;
          break;
        case TokenType.Function:
          unit = this.ParseFunction();
          if (unit == CssParser.Parsed.Empty)
          {
            this.ReportError(0, CssErrorCode.UnexpectedFunction, (object) this.CurrentTokenText);
            unit = CssParser.Parsed.False;
            break;
          }
          break;
        case TokenType.Character:
          if (this.CurrentTokenText == "(")
          {
            this.AppendCurrent();
            this.SkipSpace();
            if (this.ParseSum() != CssParser.Parsed.True)
            {
              this.ReportError(0, CssErrorCode.ExpectedSum, (object) this.CurrentTokenText);
              unit = CssParser.Parsed.False;
              break;
            }
            if (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ")")
            {
              this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
              unit = CssParser.Parsed.False;
              break;
            }
            this.AppendCurrent();
            this.SkipSpace();
            unit = CssParser.Parsed.True;
            break;
          }
          break;
      }
      return unit;
    }

    private CssParser.Parsed ParseProduct()
    {
      CssParser.Parsed product = this.ParseUnit();
      if (product == CssParser.Parsed.True)
      {
        while (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "*" || this.CurrentTokenText == "/") || this.CurrentTokenType == TokenType.Identifier && string.Compare(this.CurrentTokenText, "mod", StringComparison.OrdinalIgnoreCase) == 0)
        {
          if (this.CurrentTokenText == "*" || this.CurrentTokenText == "/")
          {
            if (this.Settings.OutputMode == OutputMode.MultipleLines)
              this.Append((object) ' ');
            this.AppendCurrent();
            if (this.Settings.OutputMode == OutputMode.MultipleLines)
              this.Append((object) ' ');
          }
          else
            this.Append((object) " mod ");
          this.SkipSpace();
          product = this.ParseUnit();
          if (product != CssParser.Parsed.True)
          {
            this.ReportError(0, CssErrorCode.ExpectedUnit, (object) this.CurrentTokenText);
            product = CssParser.Parsed.False;
          }
        }
      }
      else
      {
        this.ReportError(0, CssErrorCode.ExpectedUnit, (object) this.CurrentTokenText);
        product = CssParser.Parsed.False;
      }
      return product;
    }

    private CssParser.Parsed ParseSum()
    {
      CssParser.Parsed sum = this.ParseProduct();
      if (sum == CssParser.Parsed.True)
      {
        while (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "+" || this.CurrentTokenText == "-"))
        {
          this.Append((object) ' ');
          this.AppendCurrent();
          this.Append((object) ' ');
          this.SkipSpace();
          sum = this.ParseProduct();
          if (sum != CssParser.Parsed.True)
          {
            this.ReportError(0, CssErrorCode.ExpectedProduct, (object) this.CurrentTokenText);
            sum = CssParser.Parsed.False;
          }
        }
      }
      else
      {
        this.ReportError(0, CssErrorCode.ExpectedProduct, (object) this.CurrentTokenText);
        sum = CssParser.Parsed.False;
      }
      return sum;
    }

    private CssParser.Parsed ParseMinMax()
    {
      CssParser.Parsed minMax = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Function && (string.Compare(this.CurrentTokenText, "min(", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.CurrentTokenText, "max(", StringComparison.OrdinalIgnoreCase) == 0))
      {
        this.Append((object) this.CurrentTokenText.ToLowerInvariant());
        this.SkipSpace();
        for (CssParser.Parsed sum = this.ParseSum(); sum == CssParser.Parsed.True && this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ","; sum = this.ParseSum())
        {
          this.AppendCurrent();
          this.SkipSpace();
        }
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
        {
          this.AppendCurrent();
          this.SkipSpace();
          minMax = CssParser.Parsed.True;
        }
        else
        {
          this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
          minMax = CssParser.Parsed.False;
        }
      }
      return minMax;
    }

    private CssParser.Parsed ParseCalc()
    {
      CssParser.Parsed calc = CssParser.Parsed.False;
      if (this.CurrentTokenType == TokenType.Function && string.Compare(CssParser.GetRoot(this.CurrentTokenText), "calc(", StringComparison.OrdinalIgnoreCase) == 0)
      {
        this.Append((object) this.CurrentTokenText.ToLowerInvariant());
        this.SkipSpace();
        if (this.ParseSum() != CssParser.Parsed.True)
          this.ReportError(0, CssErrorCode.ExpectedSum, (object) this.CurrentTokenText);
        if (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ")")
        {
          this.AppendCurrent();
          this.SkipSpace();
          calc = CssParser.Parsed.True;
        }
        else
          this.ReportError(0, CssErrorCode.ExpectedClosingParenthesis, (object) this.CurrentTokenText);
      }
      return calc;
    }

    private TokenType NextToken()
    {
      this.m_currentToken = this.m_scanner.NextToken();
      this.m_encounteredNewLine = this.m_scanner.GotEndOfLine;
      while (this.CurrentTokenType == TokenType.Comment)
      {
        if (this.AppendCurrent())
          this.NewLine();
        this.m_currentToken = this.m_scanner.NextToken();
        this.m_encounteredNewLine = this.m_encounteredNewLine || this.m_scanner.GotEndOfLine;
      }
      return this.CurrentTokenType;
    }

    private TokenType NextRawToken()
    {
      this.m_currentToken = this.m_scanner.NextToken();
      this.m_encounteredNewLine = this.m_scanner.GotEndOfLine;
      return this.CurrentTokenType;
    }

    private string NextSignificantToken()
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      this.m_currentToken = this.m_scanner.NextToken();
      this.m_encounteredNewLine = this.m_scanner.GotEndOfLine;
      while (this.CurrentTokenType == TokenType.Space || this.CurrentTokenType == TokenType.Comment)
      {
        if (this.CurrentTokenType == TokenType.Comment)
        {
          string str = this.CurrentTokenText;
          bool flag1 = str.StartsWith("/*!", StringComparison.Ordinal);
          if (flag1)
            str = this.NormalizeImportantComment(str);
          bool flag2 = this.Settings.CommentMode == CssComment.All || flag1 && this.Settings.CommentMode != CssComment.None;
          if (!flag1)
          {
            Match match = CssParser.s_valueReplacement.Match(str);
            if (match.Success)
            {
              this.m_valueReplacement = (string) null;
              IList<ResourceStrings> resourceStrings = this.Settings.ResourceStrings;
              if (resourceStrings.Count > 0)
              {
                string name = match.Result("${id}");
                for (int index = resourceStrings.Count - 1; index >= 0; --index)
                {
                  this.m_valueReplacement = resourceStrings[index][name];
                  if (this.m_valueReplacement != null)
                    break;
                }
              }
              flag2 = this.m_valueReplacement == null;
              if (flag2)
                str = CssParser.NormalizedValueReplacementComment(str);
            }
          }
          if (flag2)
          {
            if (stringBuilder == null)
              stringBuilder = new StringBuilder();
            stringBuilder.Append(str);
          }
        }
        this.m_currentToken = this.m_scanner.NextToken();
        this.m_encounteredNewLine = this.m_encounteredNewLine || this.m_scanner.GotEndOfLine;
      }
      return stringBuilder != null ? stringBuilder.ToString() : string.Empty;
    }

    private void UpdateIfReplacementToken() => this.m_currentToken = this.m_scanner.ScanReplacementToken() ?? this.m_currentToken;

    private void SkipSpace()
    {
      this.m_skippedSpace = false;
      int num1 = (int) this.NextToken();
      bool flag = this.m_encounteredNewLine;
      while (this.CurrentTokenType == TokenType.Space)
      {
        this.m_skippedSpace = true;
        int num2 = (int) this.NextToken();
        flag = flag || this.m_encounteredNewLine;
      }
      this.m_encounteredNewLine = flag;
    }

    private void SkipSpaceComment()
    {
      this.m_skippedSpace = false;
      if (this.NextRawToken() == TokenType.Space)
      {
        this.m_skippedSpace = true;
        bool flag = this.m_encounteredNewLine;
        while (this.NextRawToken() == TokenType.Space)
          flag = flag || this.m_encounteredNewLine;
        if (this.CurrentTokenType == TokenType.Comment)
        {
          if (this.Settings.CommentMode == CssComment.All || this.CurrentTokenText.StartsWith("/*!", StringComparison.Ordinal))
          {
            this.Append((object) ' ');
            this.AppendCurrent();
          }
          this.SkipSpace();
          flag = flag || this.m_encounteredNewLine;
        }
        this.m_encounteredNewLine = flag;
      }
      else
      {
        if (this.CurrentTokenType != TokenType.Comment)
          return;
        bool encounteredNewLine = this.m_encounteredNewLine;
        this.AppendCurrent();
        this.SkipSpace();
        this.m_encounteredNewLine = this.m_encounteredNewLine || encounteredNewLine;
      }
    }

    private bool SkipIfSpace()
    {
      this.m_skippedSpace = false;
      bool flag1 = this.CurrentTokenType == TokenType.Space;
      bool flag2 = this.m_encounteredNewLine;
      while (this.CurrentTokenType == TokenType.Space)
      {
        this.m_skippedSpace = true;
        int num = (int) this.NextToken();
        flag2 = flag2 || this.m_encounteredNewLine;
      }
      this.m_encounteredNewLine = flag2;
      return flag1;
    }

    private void SkipToEndOfStatement()
    {
      bool flag1 = false;
      while (!this.m_scanner.EndOfFile && (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ";"))
      {
        if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "(" || this.CurrentTokenText == "[" || this.CurrentTokenText == "{"))
        {
          bool flag2 = this.CurrentTokenText == "{";
          this.SkipToClose();
          if (flag2)
            break;
          flag1 = false;
        }
        if (this.CurrentTokenType == TokenType.Space)
        {
          flag1 = true;
        }
        else
        {
          if (flag1 && CssParser.NeedsSpaceBefore(this.CurrentTokenText) && CssParser.NeedsSpaceAfter(this.m_lastOutputString))
            this.Append((object) ' ');
          this.AppendCurrent();
          flag1 = false;
        }
        int num = (int) this.NextToken();
      }
    }

    private void SkipToEndOfDeclaration()
    {
      bool flag = false;
      while (!this.m_scanner.EndOfFile && (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != ";" && this.CurrentTokenText != "}"))
      {
        if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "(" || this.CurrentTokenText == "[" || this.CurrentTokenText == "{"))
        {
          if (flag)
            this.Append((object) ' ');
          this.SkipToClose();
          flag = false;
        }
        if (this.CurrentTokenType == TokenType.Space)
        {
          flag = true;
        }
        else
        {
          if (flag && CssParser.NeedsSpaceBefore(this.CurrentTokenText) && CssParser.NeedsSpaceAfter(this.m_lastOutputString))
            this.Append((object) ' ');
          this.AppendCurrent();
          flag = false;
        }
        this.m_skippedSpace = false;
        int num = (int) this.NextToken();
        if (this.CurrentTokenType == TokenType.Space)
          this.m_skippedSpace = true;
      }
    }

    private void SkipToClose()
    {
      bool flag = false;
      string str;
      switch (this.CurrentTokenText)
      {
        case "(":
          str = ")";
          break;
        case "[":
          str = "]";
          break;
        case "{":
          str = "}";
          break;
        default:
          throw new ArgumentException("invalid closing match");
      }
      if (this.m_skippedSpace && this.CurrentTokenText != "{")
        this.Append((object) ' ');
      this.AppendCurrent();
      this.m_skippedSpace = false;
      int num1 = (int) this.NextToken();
      if (this.CurrentTokenType == TokenType.Space)
        this.m_skippedSpace = true;
      while (!this.m_scanner.EndOfFile && (this.CurrentTokenType != TokenType.Character || this.CurrentTokenText != str))
      {
        if (this.CurrentTokenType == TokenType.Character && (this.CurrentTokenText == "(" || this.CurrentTokenText == "[" || this.CurrentTokenText == "{"))
        {
          this.SkipToClose();
          flag = false;
        }
        if (this.CurrentTokenType == TokenType.Space)
        {
          flag = true;
        }
        else
        {
          if (flag && CssParser.NeedsSpaceBefore(this.CurrentTokenText) && CssParser.NeedsSpaceAfter(this.m_lastOutputString))
            this.Append((object) ' ');
          this.AppendCurrent();
          flag = false;
        }
        this.m_skippedSpace = false;
        int num2 = (int) this.NextToken();
        if (this.CurrentTokenType == TokenType.Space)
          this.m_skippedSpace = true;
      }
    }

    private void SkipSemicolons()
    {
      while (this.CurrentTokenType == TokenType.Character && this.CurrentTokenText == ";")
      {
        int num = (int) this.NextToken();
      }
    }

    private static bool NeedsSpaceBefore(string text) => text.IfNotNull<string, bool>((Func<string, bool>) (t => !"{}()[],;".Contains(t)));

    private static bool NeedsSpaceAfter(string text) => text.IfNotNull<string, bool>((Func<string, bool>) (t => !"{}()[],;:".Contains(t)));

    private bool AppendCurrent() => this.Append((object) this.CurrentTokenText, this.CurrentTokenType);

    private bool Append(object obj, TokenType tokenType)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (!this.m_noOutput)
      {
        string str1 = obj.ToString();
        if (this.Settings.ReplacementTokens.Count > 0)
          str1 = CommonData.ReplacementToken.Replace(str1, new MatchEvaluator(this.GetReplacementValue));
        switch (tokenType)
        {
          case TokenType.String:
            StringBuilder stringBuilder = (StringBuilder) null;
            int startIndex1 = 0;
            for (int index = 0; index < str1.Length; ++index)
            {
              if (str1[index] < ' ')
              {
                if (stringBuilder == null)
                  stringBuilder = new StringBuilder();
                if (startIndex1 < index)
                  stringBuilder.Append(str1.Substring(startIndex1, index - startIndex1));
                stringBuilder.Append("\\{0:x}".FormatInvariant((object) char.ConvertToUtf32(str1, index)));
                if (index + 1 < str1.Length && CssScanner.IsH(str1[index + 1]))
                  stringBuilder.Append(' ');
                startIndex1 = index + 1;
              }
            }
            if (stringBuilder != null && startIndex1 < str1.Length)
              stringBuilder.Append(str1.Substring(startIndex1));
            str1 = stringBuilder == null ? str1 : stringBuilder.ToString();
            break;
          case TokenType.Identifier:
          case TokenType.Dimension:
            StringBuilder sb = (StringBuilder) null;
            int startIndex2 = 0;
            bool flag3 = false;
            int index1 = 0;
            int index2;
            if (tokenType == TokenType.Identifier)
            {
              index2 = str1[0] == '_' || str1[0] == '-' ? 1 : 0;
              if (index2 < str1.Length)
              {
                char ch = str1[index2];
                if (ch < '\u0080' && (ch < 'A' || 'Z' < ch) && (ch < 'a' || 'z' < ch) && ch != '\\')
                {
                  sb = new StringBuilder();
                  if (index2 > 0)
                    sb.Append(str1[0]);
                  flag3 = CssParser.EscapeCharacter(sb, str1[index2]);
                  flag2 = true;
                  startIndex2 = index2 + 1;
                }
              }
            }
            else
            {
              if (str1[0] == '+' || str1[0] == '-')
                ++index1;
              while ('0' <= str1[index1] && str1[index1] <= '9')
                ++index1;
              if (str1[index1] == '.')
                ++index1;
              while ('0' <= str1[index1] && str1[index1] <= '9')
                ++index1;
              index2 = index1 - 1;
            }
            for (int index3 = index2 + 1; index3 < str1.Length; ++index3)
            {
              char ch = str1[index3];
              if (ch < '\u0080')
              {
                switch (ch)
                {
                  case ' ':
                  case '-':
                  case '_':
                    continue;
                  case '\\':
                    ++index3;
                    continue;
                  default:
                    if (('0' > ch || ch > '9') && ('a' > ch || ch > 'z') && ('A' > ch || ch > 'Z'))
                    {
                      if (sb == null)
                        sb = new StringBuilder();
                      if (startIndex2 < index3)
                      {
                        string str2 = str1.Substring(startIndex2, index3 - startIndex2);
                        if (flag3 && CssScanner.IsH(str2[0]) || flag2 && str2[0] == ' ')
                          sb.Append(' ');
                        sb.Append(str2);
                      }
                      flag3 = CssParser.EscapeCharacter(sb, str1[index3]);
                      flag2 = true;
                      startIndex2 = index3 + 1;
                      continue;
                    }
                    continue;
                }
              }
            }
            if (sb != null)
            {
              if (startIndex2 < str1.Length)
              {
                string str3 = str1.Substring(startIndex2);
                if (flag3 && CssScanner.IsH(str3[0]) || str3[0] == ' ')
                  sb.Append(' ');
                sb.Append(str3);
                flag2 = false;
              }
              str1 = sb.ToString();
              break;
            }
            break;
        }
        bool flag4 = false;
        flag1 = tokenType != TokenType.Comment;
        if (!flag1)
        {
          if (str1.StartsWith("/*!", StringComparison.Ordinal))
          {
            if (this.Settings.CommentMode == CssComment.None)
              return false;
            str1 = this.NormalizeImportantComment(str1);
            int startIndex3 = str1.IndexOf('/');
            if (startIndex3 > 0 && this.m_outputNewLine)
              str1 = str1.Substring(startIndex3);
          }
          else
          {
            Match match = CssParser.s_valueReplacement.Match(this.CurrentTokenText);
            if (match.Success)
            {
              this.m_valueReplacement = (string) null;
              IList<ResourceStrings> resourceStrings = this.Settings.ResourceStrings;
              if (resourceStrings.Count > 0)
              {
                string name = match.Result("${id}");
                for (int index4 = resourceStrings.Count - 1; index4 >= 0; --index4)
                {
                  this.m_valueReplacement = resourceStrings[index4][name];
                  if (this.m_valueReplacement != null)
                    break;
                }
              }
              if (this.m_valueReplacement != null)
                return false;
              str1 = CssParser.NormalizedValueReplacementComment(str1);
            }
            else if (this.Settings.CommentMode != CssComment.All)
              return false;
          }
          flag4 = str1.StartsWith("/*!", StringComparison.Ordinal);
        }
        else if (this.m_parsingColorValue && (tokenType == TokenType.Identifier || tokenType == TokenType.ReplacementToken))
        {
          if (!str1.StartsWith("#", StringComparison.Ordinal))
          {
            bool flag5 = false;
            string lowerInvariant = str1.ToLowerInvariant();
            string str4;
            switch (this.Settings.ColorNames)
            {
              case CssColor.Strict:
                if (ColorSlice.StrictHexShorterThanNameAndAllNonStrict.TryGetValue(lowerInvariant, out str4))
                {
                  str1 = str4;
                  flag5 = true;
                  break;
                }
                break;
              case CssColor.Hex:
                if (ColorSlice.AllColorNames.TryGetValue(lowerInvariant, out str4))
                {
                  str1 = str4;
                  flag5 = true;
                  break;
                }
                break;
              case CssColor.Major:
                if (ColorSlice.HexShorterThanName.TryGetValue(lowerInvariant, out str4))
                {
                  str1 = str4;
                  flag5 = true;
                  break;
                }
                break;
            }
            if (this.Settings.ColorNames != CssColor.Hex && !flag5 && ColorSlice.AllColorNames.TryGetValue(lowerInvariant, out str4))
              str1 = lowerInvariant;
          }
          else if (this.CurrentTokenType == TokenType.ReplacementToken)
            str1 = CssParser.CrunchHexColor(str1, this.Settings.ColorNames, this.m_noColorAbbreviation);
        }
        if (this.m_mightNeedSpace && (CssScanner.IsH(str1[0]) || str1[0] == ' '))
        {
          if (this.m_lineLength >= this.Settings.LineBreakThreshold)
          {
            this.AddNewLine();
          }
          else
          {
            this.m_parsed.Append(' ');
            ++this.m_lineLength;
          }
        }
        if (tokenType == TokenType.Comment && flag4)
          this.AddNewLine();
        if (str1 == " ")
        {
          if (this.m_lineLength >= this.Settings.LineBreakThreshold)
          {
            this.AddNewLine();
          }
          else
          {
            this.m_parsed.Append(' ');
            ++this.m_lineLength;
          }
        }
        else
        {
          if (this.m_forceNewLine)
          {
            if (!this.m_outputNewLine && this.Settings.OutputMode == OutputMode.MultipleLines)
              this.AddNewLine();
            this.m_forceNewLine = false;
          }
          this.m_parsed.Append(str1);
          this.m_outputNewLine = false;
          if (tokenType == TokenType.Comment && flag4)
          {
            this.AddNewLine();
            this.m_lineLength = 0;
            this.m_outputNewLine = true;
          }
          else
            this.m_lineLength += str1.Length;
        }
        this.m_mightNeedSpace = flag2;
        this.m_lastOutputString = str1;
      }
      return flag1;
    }

    private string GetReplacementValue(Match match)
    {
      string text = (string) null;
      string str1 = match.Result("${token}");
      if (!str1.IsNullOrWhiteSpace() && !this.Settings.ReplacementTokens.TryGetValue(str1, out text))
      {
        string str2 = match.Result("${fallback}");
        if (!str2.IsNullOrWhiteSpace())
          this.Settings.ReplacementFallbacks.TryGetValue(str2, out text);
      }
      return text.IfNullOrWhiteSpace(string.Empty);
    }

    private static bool EscapeCharacter(StringBuilder sb, char character)
    {
      string str = "\\{0:x}".FormatInvariant((object) (int) character);
      sb.Append(str);
      return str.Length < 7;
    }

    private bool Append(object obj) => this.Append(obj, TokenType.None);

    private void NewLine()
    {
      if (this.Settings.OutputMode != OutputMode.MultipleLines || this.m_outputNewLine)
        return;
      this.AddNewLine();
      this.m_lineLength = 0;
      this.m_outputNewLine = true;
    }

    private void AddNewLine()
    {
      if (this.m_outputNewLine)
        return;
      if (this.Settings.OutputMode == OutputMode.MultipleLines)
      {
        this.m_parsed.AppendLine();
        string tabSpaces = this.Settings.TabSpaces;
        this.m_lineLength = tabSpaces.Length;
        if (this.m_lineLength > 0)
          this.m_parsed.Append(tabSpaces);
      }
      else
      {
        this.m_parsed.Append('\n');
        this.m_lineLength = 0;
      }
      this.m_outputNewLine = true;
    }

    private void Indent() => this.Settings.Indent();

    private void Unindent() => this.Settings.Unindent();

    private static string CrunchHexColor(string hexColor, CssColor colorNames, bool noAbbr)
    {
      if (!noAbbr)
        hexColor = CssParser.s_rrggbb.Replace(hexColor, "#${r}${g}${b}").ToLowerInvariant();
      if (colorNames != CssColor.Hex)
      {
        string str;
        if (ColorSlice.StrictNameShorterThanHex.TryGetValue(hexColor, out str))
          hexColor = str;
        else if (colorNames == CssColor.Major && ColorSlice.NameShorterThanHex.TryGetValue(hexColor, out str))
          hexColor = str;
      }
      return hexColor;
    }

    private static bool MightContainColorNames(string propertyName)
    {
      bool flag = propertyName.EndsWith("color", StringComparison.Ordinal);
      if (!flag)
      {
        switch (propertyName)
        {
          case "background":
          case "border-top":
          case "border-right":
          case "border-bottom":
          case "border-left":
          case "border":
          case "outline":
            flag = true;
            break;
        }
      }
      return flag;
    }

    public static string ErrorFormat(CssErrorCode errorCode) => CssStrings.ResourceManager.GetString(errorCode.ToString(), CssStrings.Culture);

    private void ReportError(
      int severity,
      CssErrorCode errorNumber,
      CssContext context,
      params object[] arguments)
    {
      string str = CssParser.ErrorFormat(errorNumber).FormatInvariant(arguments);
      this.OnCssError(new ContextError()
      {
        IsError = severity < 2,
        Severity = severity,
        Subcategory = ContextError.GetSubcategory(severity),
        File = this.FileContext,
        ErrorNumber = (int) errorNumber,
        ErrorCode = "CSS{0}".FormatInvariant((object) (int) (errorNumber & (CssErrorCode) 65535)),
        StartLine = context.IfNotNull<CssContext, int>((Func<CssContext, int>) (c => c.Start.Line)),
        StartColumn = context.IfNotNull<CssContext, int>((Func<CssContext, int>) (c => c.Start.Char)),
        Message = str
      });
    }

    private void ReportError(int severity, CssErrorCode errorNumber, params object[] arguments) => this.ReportError(severity, errorNumber, this.m_currentToken.IfNotNull<CssToken, CssContext>((Func<CssToken, CssContext>) (c => c.Context)), arguments);

    public event EventHandler<ContextErrorEventArgs> CssError;

    protected void OnCssError(ContextError cssError)
    {
      if (this.CssError == null || cssError == null || this.Settings.IgnoreAllErrors || this.Settings.IgnoreErrorCollection.Contains(cssError.ErrorCode))
        return;
      this.CssError((object) this, new ContextErrorEventArgs()
      {
        Error = cssError
      });
    }

    private static string NormalizedValueReplacementComment(string source) => CssParser.s_valueReplacement.Replace(source, "/*[${id}]*/");

    private static bool CommentContainsText(string comment)
    {
      for (int index = 0; index < comment.Length; ++index)
      {
        if (char.IsLetterOrDigit(comment[index]))
          return true;
      }
      return false;
    }

    private string NormalizeImportantComment(string source)
    {
      if (CssParser.CommentContainsText(source))
      {
        if (source[3] == '/' && source.EndsWith("/**/", StringComparison.Ordinal))
          source = "/*" + source.Substring(3);
      }
      else
        source = "/*" + source.Substring(3);
      if (this.Settings.OutputMode == OutputMode.SingleLine)
        source = source.Replace("\r\n", "\n");
      return source;
    }

    private enum Parsed
    {
      True,
      False,
      Empty,
    }
  }
}
