// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.CssParser
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WebGrease.Configuration;
using WebGrease.Css.Ast;

namespace WebGrease.Css
{
  [GeneratedCode("ANTLR", "3.3.1.7705")]
  [CLSCompliant(false)]
  public class CssParser : Parser
  {
    public const int EOF = -1;
    public const int A = 4;
    public const int AND = 5;
    public const int ANGLE = 6;
    public const int AT_NAME = 7;
    public const int B = 8;
    public const int BACKWARD_SLASH = 9;
    public const int C = 10;
    public const int CHARSET_SYM = 11;
    public const int CIRCLE_BEGIN = 12;
    public const int CIRCLE_END = 13;
    public const int CLASS_IDENT = 14;
    public const int COLON = 15;
    public const int COMMA = 16;
    public const int COMMENTS = 17;
    public const int CURLY_BEGIN = 18;
    public const int CURLY_END = 19;
    public const int D = 20;
    public const int DASHMATCH = 21;
    public const int DIGITS = 22;
    public const int DIMENSION = 23;
    public const int DOCUMENT_SYM = 24;
    public const int DOMAIN_FUNCTION = 25;
    public const int E = 26;
    public const int EMPTY_COMMENT = 27;
    public const int EQUALS = 28;
    public const int ESCAPE = 29;
    public const int F = 30;
    public const int FORWARD_SLASH = 31;
    public const int FREQ = 32;
    public const int FROM = 33;
    public const int G = 34;
    public const int GREATER = 35;
    public const int H = 36;
    public const int HASH = 37;
    public const int HASH_IDENT = 38;
    public const int HEXDIGIT = 39;
    public const int I = 40;
    public const int IDENT = 41;
    public const int IMPORTANT_COMMENTS = 42;
    public const int IMPORTANT_SYM = 43;
    public const int IMPORT_SYM = 44;
    public const int INCLUDES = 45;
    public const int K = 46;
    public const int KEYFRAMES_SYM = 47;
    public const int L = 48;
    public const int LENGTH = 49;
    public const int LETTER = 50;
    public const int M = 51;
    public const int MEDIA_SYM = 52;
    public const int MINUS = 53;
    public const int MSIE_EXPRESSION = 54;
    public const int MSIE_IMAGE_TRANSFORM = 55;
    public const int N = 56;
    public const int NAME = 57;
    public const int NAMESPACE_SYM = 58;
    public const int NL = 59;
    public const int NMCHAR = 60;
    public const int NMSTART = 61;
    public const int NONASCII = 62;
    public const int NOT = 63;
    public const int NUMBER = 64;
    public const int O = 65;
    public const int ONLY = 66;
    public const int P = 67;
    public const int PAGE_SYM = 68;
    public const int PERCENTAGE = 69;
    public const int PIPE = 70;
    public const int PLUS = 71;
    public const int PREFIXMATCH = 72;
    public const int R = 73;
    public const int REGEXP_FUNCTION = 74;
    public const int RELATIVELENGTH = 75;
    public const int REPLACEMENTTOKEN = 76;
    public const int RESOLUTION = 77;
    public const int S = 78;
    public const int SEMICOLON = 79;
    public const int SPACE_AFTER_UNICODE = 80;
    public const int SPEECH = 81;
    public const int SQUARE_BEGIN = 82;
    public const int SQUARE_END = 83;
    public const int STAR = 84;
    public const int STRING = 85;
    public const int SUBSTRINGMATCH = 86;
    public const int SUFFIXMATCH = 87;
    public const int T = 88;
    public const int TILDE = 89;
    public const int TIME = 90;
    public const int TO = 91;
    public const int U = 92;
    public const int UNICODE = 93;
    public const int UNICODE_ESCAPE_HACK = 94;
    public const int UNICODE_NULLTERM = 95;
    public const int UNICODE_RANGE = 96;
    public const int UNICODE_TAB = 97;
    public const int UNICODE_ZEROS = 98;
    public const int URI = 99;
    public const int URL = 100;
    public const int URLPREFIX_FUNCTION = 101;
    public const int V = 102;
    public const int W = 103;
    public const int WG_DPI_SYM = 104;
    public const int WS = 105;
    public const int WS_FRAGMENT = 106;
    public const int X = 107;
    public const int Y = 108;
    public const int Z = 109;
    public const int ATIDENTIFIER = 110;
    public const int ATTRIBIDENTIFIER = 111;
    public const int ATTRIBNAME = 112;
    public const int ATTRIBOPERATOR = 113;
    public const int ATTRIBOPERATORVALUE = 114;
    public const int ATTRIBVALUE = 115;
    public const int CHARSET = 116;
    public const int CLASSIDENTIFIER = 117;
    public const int COLONS = 118;
    public const int COMBINATOR = 119;
    public const int COMBINATOR_SIMPLE_SELECTOR = 120;
    public const int COMBINATOR_SIMPLE_SELECTOR_SEQUENCES = 121;
    public const int DECLARATION = 122;
    public const int DECLARATIONS = 123;
    public const int DOCUMENT = 124;
    public const int DOCUMENT_MATCHNAME = 125;
    public const int DOCUMENT_SYMBOL = 126;
    public const int ELEMENT_NAME = 127;
    public const int EXPR = 128;
    public const int FUNCTIONAL_PSEUDO = 129;
    public const int FUNCTIONBASEDVALUE = 130;
    public const int FUNCTIONNAME = 131;
    public const int FUNCTIONPARAM = 132;
    public const int HASHCLASSATNAMEATTRIBPSEUDONEGATION = 133;
    public const int HASHCLASSATNAMEATTRIBPSEUDONEGATIONNODES = 134;
    public const int HASHIDENTIFIER = 135;
    public const int HEXBASEDVALUE = 136;
    public const int IDENTBASEDVALUE = 137;
    public const int IMPORT = 138;
    public const int IMPORTANT = 139;
    public const int IMPORTS = 140;
    public const int KEYFRAMES = 141;
    public const int KEYFRAMES_BLOCK = 142;
    public const int KEYFRAMES_BLOCKS = 143;
    public const int KEYFRAMES_SELECTOR = 144;
    public const int KEYFRAMES_SELECTORS = 145;
    public const int KEYFRAMES_SYMBOL = 146;
    public const int MEDIA = 147;
    public const int MEDIA_EXPRESSION = 148;
    public const int MEDIA_EXPRESSIONS = 149;
    public const int MEDIA_FEATURE = 150;
    public const int MEDIA_QUERY = 151;
    public const int MEDIA_QUERY_LIST = 152;
    public const int MEDIA_TYPE = 153;
    public const int NAMESPACE = 154;
    public const int NAMESPACES = 155;
    public const int NAMESPACE_PREFIX = 156;
    public const int NEGATIONIDENTIFIER = 157;
    public const int NEGATION_ARG = 158;
    public const int NOT_TEXT = 159;
    public const int NUMBERBASEDVALUE = 160;
    public const int ONLY_TEXT = 161;
    public const int OPERATOR = 162;
    public const int PAGE = 163;
    public const int PROPERTY = 164;
    public const int PSEUDOIDENTIFIER = 165;
    public const int PSEUDONAME = 166;
    public const int PSEUDO_PAGE = 167;
    public const int REPLACEMENT = 168;
    public const int REPLACEMENTTOKENBASEDVALUE = 169;
    public const int REPLACEMENTTOKENIDENTIFIER = 170;
    public const int RULESET = 171;
    public const int RULESETS = 172;
    public const int SELECTOR = 173;
    public const int SELECTORS_GROUP = 174;
    public const int SELECTOR_EXPRESSION = 175;
    public const int SELECTOR_NAMESPACE_PREFIX = 176;
    public const int SIMPLE_SELECTOR_SEQUENCE = 177;
    public const int STAR_TEXT = 178;
    public const int STRINGBASEDVALUE = 179;
    public const int STYLESHEET = 180;
    public const int TERM = 181;
    public const int TERMWITHOPERATOR = 182;
    public const int TERMWITHOPERATORS = 183;
    public const int TYPE_SELECTOR = 184;
    public const int UNARY = 185;
    public const int UNIVERSAL = 186;
    public const int URIBASEDVALUE = 187;
    public const int URIHASH = 188;
    public const int WG_DPI = 189;
    public const int WHITESPACE = 190;
    private readonly IList<Exception> _exceptions = (IList<Exception>) new List<Exception>();
    private static char[] _semicolon = new char[1]{ ';' };
    internal static readonly string[] tokenNames = new string[191]
    {
      "<invalid>",
      "<EOR>",
      "<DOWN>",
      "<UP>",
      nameof (A),
      nameof (AND),
      nameof (ANGLE),
      nameof (AT_NAME),
      nameof (B),
      nameof (BACKWARD_SLASH),
      nameof (C),
      nameof (CHARSET_SYM),
      nameof (CIRCLE_BEGIN),
      nameof (CIRCLE_END),
      nameof (CLASS_IDENT),
      nameof (COLON),
      nameof (COMMA),
      nameof (COMMENTS),
      nameof (CURLY_BEGIN),
      nameof (CURLY_END),
      nameof (D),
      nameof (DASHMATCH),
      nameof (DIGITS),
      nameof (DIMENSION),
      nameof (DOCUMENT_SYM),
      nameof (DOMAIN_FUNCTION),
      nameof (E),
      nameof (EMPTY_COMMENT),
      nameof (EQUALS),
      nameof (ESCAPE),
      nameof (F),
      nameof (FORWARD_SLASH),
      nameof (FREQ),
      nameof (FROM),
      nameof (G),
      nameof (GREATER),
      nameof (H),
      nameof (HASH),
      nameof (HASH_IDENT),
      nameof (HEXDIGIT),
      nameof (I),
      nameof (IDENT),
      nameof (IMPORTANT_COMMENTS),
      nameof (IMPORTANT_SYM),
      nameof (IMPORT_SYM),
      nameof (INCLUDES),
      nameof (K),
      nameof (KEYFRAMES_SYM),
      nameof (L),
      nameof (LENGTH),
      nameof (LETTER),
      nameof (M),
      nameof (MEDIA_SYM),
      nameof (MINUS),
      nameof (MSIE_EXPRESSION),
      nameof (MSIE_IMAGE_TRANSFORM),
      nameof (N),
      nameof (NAME),
      nameof (NAMESPACE_SYM),
      nameof (NL),
      nameof (NMCHAR),
      nameof (NMSTART),
      nameof (NONASCII),
      nameof (NOT),
      nameof (NUMBER),
      nameof (O),
      nameof (ONLY),
      nameof (P),
      nameof (PAGE_SYM),
      nameof (PERCENTAGE),
      nameof (PIPE),
      nameof (PLUS),
      nameof (PREFIXMATCH),
      nameof (R),
      nameof (REGEXP_FUNCTION),
      nameof (RELATIVELENGTH),
      nameof (REPLACEMENTTOKEN),
      nameof (RESOLUTION),
      nameof (S),
      nameof (SEMICOLON),
      nameof (SPACE_AFTER_UNICODE),
      nameof (SPEECH),
      nameof (SQUARE_BEGIN),
      nameof (SQUARE_END),
      nameof (STAR),
      nameof (STRING),
      nameof (SUBSTRINGMATCH),
      nameof (SUFFIXMATCH),
      nameof (T),
      nameof (TILDE),
      nameof (TIME),
      nameof (TO),
      nameof (U),
      nameof (UNICODE),
      nameof (UNICODE_ESCAPE_HACK),
      nameof (UNICODE_NULLTERM),
      nameof (UNICODE_RANGE),
      nameof (UNICODE_TAB),
      nameof (UNICODE_ZEROS),
      nameof (URI),
      nameof (URL),
      nameof (URLPREFIX_FUNCTION),
      nameof (V),
      nameof (W),
      nameof (WG_DPI_SYM),
      nameof (WS),
      nameof (WS_FRAGMENT),
      nameof (X),
      nameof (Y),
      nameof (Z),
      nameof (ATIDENTIFIER),
      nameof (ATTRIBIDENTIFIER),
      nameof (ATTRIBNAME),
      nameof (ATTRIBOPERATOR),
      nameof (ATTRIBOPERATORVALUE),
      nameof (ATTRIBVALUE),
      nameof (CHARSET),
      nameof (CLASSIDENTIFIER),
      nameof (COLONS),
      nameof (COMBINATOR),
      nameof (COMBINATOR_SIMPLE_SELECTOR),
      nameof (COMBINATOR_SIMPLE_SELECTOR_SEQUENCES),
      nameof (DECLARATION),
      nameof (DECLARATIONS),
      nameof (DOCUMENT),
      nameof (DOCUMENT_MATCHNAME),
      nameof (DOCUMENT_SYMBOL),
      nameof (ELEMENT_NAME),
      nameof (EXPR),
      nameof (FUNCTIONAL_PSEUDO),
      nameof (FUNCTIONBASEDVALUE),
      nameof (FUNCTIONNAME),
      nameof (FUNCTIONPARAM),
      nameof (HASHCLASSATNAMEATTRIBPSEUDONEGATION),
      nameof (HASHCLASSATNAMEATTRIBPSEUDONEGATIONNODES),
      nameof (HASHIDENTIFIER),
      nameof (HEXBASEDVALUE),
      nameof (IDENTBASEDVALUE),
      nameof (IMPORT),
      nameof (IMPORTANT),
      nameof (IMPORTS),
      nameof (KEYFRAMES),
      nameof (KEYFRAMES_BLOCK),
      nameof (KEYFRAMES_BLOCKS),
      nameof (KEYFRAMES_SELECTOR),
      nameof (KEYFRAMES_SELECTORS),
      nameof (KEYFRAMES_SYMBOL),
      nameof (MEDIA),
      nameof (MEDIA_EXPRESSION),
      nameof (MEDIA_EXPRESSIONS),
      nameof (MEDIA_FEATURE),
      nameof (MEDIA_QUERY),
      nameof (MEDIA_QUERY_LIST),
      nameof (MEDIA_TYPE),
      nameof (NAMESPACE),
      nameof (NAMESPACES),
      nameof (NAMESPACE_PREFIX),
      nameof (NEGATIONIDENTIFIER),
      nameof (NEGATION_ARG),
      nameof (NOT_TEXT),
      nameof (NUMBERBASEDVALUE),
      nameof (ONLY_TEXT),
      nameof (OPERATOR),
      nameof (PAGE),
      nameof (PROPERTY),
      nameof (PSEUDOIDENTIFIER),
      nameof (PSEUDONAME),
      nameof (PSEUDO_PAGE),
      nameof (REPLACEMENT),
      nameof (REPLACEMENTTOKENBASEDVALUE),
      nameof (REPLACEMENTTOKENIDENTIFIER),
      nameof (RULESET),
      nameof (RULESETS),
      nameof (SELECTOR),
      nameof (SELECTORS_GROUP),
      nameof (SELECTOR_EXPRESSION),
      nameof (SELECTOR_NAMESPACE_PREFIX),
      nameof (SIMPLE_SELECTOR_SEQUENCE),
      nameof (STAR_TEXT),
      nameof (STRINGBASEDVALUE),
      nameof (STYLESHEET),
      nameof (TERM),
      nameof (TERMWITHOPERATOR),
      nameof (TERMWITHOPERATORS),
      nameof (TYPE_SELECTOR),
      nameof (UNARY),
      nameof (UNIVERSAL),
      nameof (URIBASEDVALUE),
      nameof (URIHASH),
      nameof (WG_DPI),
      nameof (WHITESPACE)
    };
    private ITreeAdaptor adaptor;
    private CssParser.DFA25 dfa25;
    private CssParser.DFA33 dfa33;
    private CssParser.DFA48 dfa48;
    private CssParser.DFA54 dfa54;
    private CssParser.DFA65 dfa65;

    public static StyleSheetNode Parse(
      IWebGreaseContext context,
      string cssContent,
      bool shouldLogDiagnostics = true)
    {
      return CssParser.ParseStyleSheet(context, cssContent, shouldLogDiagnostics);
    }

    public static StyleSheetNode Parse(FileInfo cssFile, bool shouldLogDiagnostics = true)
    {
      string fullName = cssFile.FullName;
      Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Parsing {0} ", new object[1]
      {
        (object) fullName
      }));
      return CssParser.ParseStyleSheet((IWebGreaseContext) new WebGreaseContext(new WebGreaseConfiguration()), File.ReadAllText(fullName), shouldLogDiagnostics);
    }

    public override void ReportError(RecognitionException e)
    {
      if (e == null)
        return;
      this._exceptions.Add((Exception) e);
      base.ReportError(e);
    }

    private static StyleSheetNode ParseStyleSheet(
      IWebGreaseContext context,
      string cssContent,
      bool shouldLogDiagnostics)
    {
      CssParser parser = new CssParser((ITokenStream) new CommonTokenStream((ITokenSource) new CssLexer((ICharStream) new ANTLRStringStream(cssContent))));
      CommonTree commonTree = context.SectionedAction(nameof (CssParser), "Antlr").Execute<CommonTree>((Func<CommonTree>) (() =>
      {
        if (shouldLogDiagnostics)
        {
          TextWriterTraceListener writerTraceListener = Trace.Listeners.OfType<TextWriterTraceListener>().FirstOrDefault<TextWriterTraceListener>();
          if (writerTraceListener != null)
            parser.TraceDestination = writerTraceListener.Writer;
        }
        return parser.main().Tree as CommonTree;
      }));
      if (commonTree == null)
        return (StyleSheetNode) null;
      return context.SectionedAction(nameof (CssParser), "CreateObjects").Execute<StyleSheetNode>((Func<StyleSheetNode>) (() =>
      {
        if (shouldLogDiagnostics)
          CssParser.LogDiagnostics(cssContent, commonTree);
        if (parser.NumberOfSyntaxErrors > 0)
          throw new AggregateException("Syntax errors found.", (IEnumerable<Exception>) parser._exceptions);
        return CommonTreeTransformer.CreateStyleSheetNode(commonTree);
      }));
    }

    private static void LogDiagnostics(string css, CommonTree commonTree)
    {
      Trace.WriteLine("Input Css:");
      Trace.WriteLine("____________________________________________________");
      Trace.WriteLine(css);
      Trace.WriteLine("____________________________________________________");
      Trace.WriteLine("Css String Tree:");
      Trace.WriteLine("____________________________________________________");
      Trace.WriteLine(commonTree.ToStringTree());
      Trace.WriteLine("____________________________________________________");
      Trace.WriteLine("Css Common Tree:");
      Trace.WriteLine("____________________________________________________");
      CssParser.LogTree(commonTree);
      Trace.WriteLine("____________________________________________________");
    }

    private static void LogTree(CommonTree tree)
    {
      Stack<Tuple<int, CommonTree>> tupleStack = new Stack<Tuple<int, CommonTree>>();
      tupleStack.Push(new Tuple<int, CommonTree>(0, tree));
      while (tupleStack.Count > 0)
      {
        Tuple<int, CommonTree> tuple = tupleStack.Pop();
        int num = tuple.Item1;
        CommonTree commonTree1 = tuple.Item2;
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < num; ++index)
          stringBuilder.Append("---");
        Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
        {
          (object) stringBuilder,
          (object) commonTree1
        }));
        IList<ITree> children = commonTree1.Children;
        if (children != null)
        {
          foreach (CommonTree commonTree2 in children.OfType<CommonTree>().Reverse<CommonTree>())
            tupleStack.Push(new Tuple<int, CommonTree>(num + 1, commonTree2));
        }
      }
    }

    private CommonToken GetWhitespaceToken()
    {
      if (this.input.Index > 0)
      {
        IToken token = this.input.Get(this.input.Index - 1);
        if (token != null && token.Type == 105 && token.Text != null && string.IsNullOrWhiteSpace(token.Text))
          return new CommonToken(190, token.Text.Length.ToString());
      }
      return new CommonToken(190, "0");
    }

    private static CommonToken TrimMsieExpression(string text)
    {
      if (text.EndsWith(";"))
        text = text.TrimEnd(CssParser._semicolon);
      return new CommonToken(54, text);
    }

    public CssParser(ITokenStream input)
      : this(input, new RecognizerSharedState())
    {
    }

    public CssParser(ITokenStream input, RecognizerSharedState state)
      : base(input, state)
    {
      this.TreeAdaptor = (ITreeAdaptor) null ?? (ITreeAdaptor) new CommonTreeAdaptor();
    }

    public ITreeAdaptor TreeAdaptor
    {
      get => this.adaptor;
      set => this.adaptor = value;
    }

    public override string[] TokenNames => CssParser.tokenNames;

    public override string GrammarFileName => "Css\\CssParser.g3";

    [GrammarRule("main")]
    public CssParser.main_return main()
    {
      CssParser.main_return mainReturn = new CssParser.main_return(this);
      mainReturn.Start = (CommonToken) this.input.LT(1);
      try
      {
        object obj = this.adaptor.Nil();
        this.PushFollow(CssParser.Follow._styleSheet_in_main653);
        CssParser.styleSheet_return styleSheetReturn = this.styleSheet();
        this.PopFollow();
        if (this.state.failed)
          return mainReturn;
        if (this.state.backtracking == 0)
          this.adaptor.AddChild(obj, styleSheetReturn.Tree);
        CommonToken payload = (CommonToken) this.Match((IIntStream) this.input, -1, CssParser.Follow._EOF_in_main659);
        if (this.state.failed)
          return mainReturn;
        if (this.state.backtracking == 0)
        {
          object child = this.adaptor.Create((IToken) payload);
          this.adaptor.AddChild(obj, child);
        }
        mainReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          mainReturn.Tree = this.adaptor.RulePostProcessing(obj);
          this.adaptor.SetTokenBoundaries(mainReturn.Tree, (IToken) mainReturn.Start, (IToken) mainReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        mainReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) mainReturn.Start, this.input.LT(-1), ex);
      }
      return mainReturn;
    }

    [GrammarRule("styleSheet")]
    private CssParser.styleSheet_return styleSheet()
    {
      CssParser.styleSheet_return styleSheetReturn = new CssParser.styleSheet_return(this);
      styleSheetReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token CHARSET_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token STRING");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token SEMICOLON");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule styleimport");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule namespace");
      RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule styleSheetRulesOrComment");
      try
      {
        int num1 = 2;
        if (this.input.LA(1) == 11)
          num1 = 1;
        if (num1 == 1)
        {
          CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 11, CssParser.Follow._CHARSET_SYM_in_styleSheet683);
          if (this.state.failed)
            return styleSheetReturn;
          if (this.state.backtracking == 0)
            rewriteRuleTokenStream1.Add((object) el1);
          CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 85, CssParser.Follow._STRING_in_styleSheet685);
          if (this.state.failed)
            return styleSheetReturn;
          if (this.state.backtracking == 0)
            rewriteRuleTokenStream2.Add((object) el2);
          CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 79, CssParser.Follow._SEMICOLON_in_styleSheet687);
          if (this.state.failed)
            return styleSheetReturn;
          if (this.state.backtracking == 0)
            rewriteRuleTokenStream3.Add((object) el3);
        }
        while (true)
        {
          CssParser.styleimport_return styleimportReturn;
          do
          {
            int num2 = 2;
            if (this.input.LA(1) == 44)
              num2 = 1;
            if (num2 == 1)
            {
              this.PushFollow(CssParser.Follow._styleimport_in_styleSheet691);
              styleimportReturn = this.styleimport();
              this.PopFollow();
              if (this.state.failed)
                return styleSheetReturn;
            }
            else
              goto label_23;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream1.Add(styleimportReturn.Tree);
        }
label_23:
        while (true)
        {
          CssParser.namespace_return namespaceReturn;
          do
          {
            int num3 = 2;
            if (this.input.LA(1) == 58)
              num3 = 1;
            if (num3 == 1)
            {
              this.PushFollow(CssParser.Follow._namespace_in_styleSheet694);
              namespaceReturn = this.@namespace();
              this.PopFollow();
              if (this.state.failed)
                return styleSheetReturn;
            }
            else
              goto label_30;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream2.Add(namespaceReturn.Tree);
        }
label_30:
        while (true)
        {
          CssParser.styleSheetRulesOrComment_return rulesOrCommentReturn;
          do
          {
            int num4 = 2;
            int num5 = this.input.LA(1);
            if (num5 == 7 || num5 >= 14 && num5 <= 15 || num5 == 24 || num5 == 38 || num5 >= 41 && num5 <= 42 || num5 == 47 || num5 == 52 || num5 == 68 || num5 == 70 || num5 == 76 || num5 == 82 || num5 == 84 || num5 == 104)
              num4 = 1;
            if (num4 == 1)
            {
              this.PushFollow(CssParser.Follow._styleSheetRulesOrComment_in_styleSheet697);
              rulesOrCommentReturn = this.styleSheetRulesOrComment();
              this.PopFollow();
              if (this.state.failed)
                return styleSheetReturn;
            }
            else
              goto label_37;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream3.Add(rulesOrCommentReturn.Tree);
        }
label_37:
        if (this.state.backtracking == 0)
        {
          styleSheetReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", styleSheetReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(180, "STYLESHEET"), oldRoot1);
          if (rewriteRuleTokenStream2.HasNext)
          {
            object oldRoot2 = this.adaptor.Nil();
            object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(116, "CHARSET"), oldRoot2);
            object oldRoot3 = this.adaptor.Nil();
            object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(179, "STRINGBASEDVALUE"), oldRoot3);
            this.adaptor.AddChild(obj4, rewriteRuleTokenStream2.NextNode());
            this.adaptor.AddChild(obj3, obj4);
            this.adaptor.AddChild(obj2, obj3);
          }
          rewriteRuleTokenStream2.Reset();
          if (ruleSubtreeStream1.HasNext)
          {
            object oldRoot4 = this.adaptor.Nil();
            object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(140, "IMPORTS"), oldRoot4);
            while (ruleSubtreeStream1.HasNext)
              this.adaptor.AddChild(obj5, ruleSubtreeStream1.NextTree());
            ruleSubtreeStream1.Reset();
            this.adaptor.AddChild(obj2, obj5);
          }
          ruleSubtreeStream1.Reset();
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot5 = this.adaptor.Nil();
            object obj6 = this.adaptor.BecomeRoot(this.adaptor.Create(155, "NAMESPACES"), oldRoot5);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj6, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj6);
          }
          ruleSubtreeStream2.Reset();
          while (ruleSubtreeStream3.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream3.NextTree());
          ruleSubtreeStream3.Reset();
          this.adaptor.AddChild(obj1, obj2);
          styleSheetReturn.Tree = obj1;
        }
        styleSheetReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          styleSheetReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(styleSheetReturn.Tree, (IToken) styleSheetReturn.Start, (IToken) styleSheetReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        styleSheetReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) styleSheetReturn.Start, this.input.LT(-1), ex);
      }
      return styleSheetReturn;
    }

    [GrammarRule("styleSheetRulesOrComment")]
    private CssParser.styleSheetRulesOrComment_return styleSheetRulesOrComment()
    {
      CssParser.styleSheetRulesOrComment_return rulesOrCommentReturn = new CssParser.styleSheetRulesOrComment_return(this);
      rulesOrCommentReturn.Start = (CommonToken) this.input.LT(1);
      object obj = (object) null;
      try
      {
        int num1 = this.input.LA(1);
        int num2;
        if (num1 == 42)
          num2 = 1;
        else if (num1 == 7 || num1 >= 14 && num1 <= 15 || num1 == 24 || num1 == 38 || num1 == 41 || num1 == 47 || num1 == 52 || num1 == 68 || num1 == 70 || num1 == 76 || num1 == 82 || num1 == 84 || num1 == 104)
        {
          num2 = 2;
        }
        else
        {
          if (this.state.backtracking <= 0)
            throw new NoViableAltException("", 5, 0, (IIntStream) this.input);
          this.state.failed = true;
          return rulesOrCommentReturn;
        }
        switch (num2)
        {
          case 1:
            obj = this.adaptor.Nil();
            CommonToken payload = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_styleSheetRulesOrComment756);
            if (this.state.failed)
              return rulesOrCommentReturn;
            if (this.state.backtracking == 0)
            {
              object child = this.adaptor.Create((IToken) payload);
              this.adaptor.AddChild(obj, child);
              break;
            }
            break;
          case 2:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._styleSheetrules_in_styleSheetRulesOrComment764);
            CssParser.styleSheetrules_return sheetrulesReturn = this.styleSheetrules();
            this.PopFollow();
            if (this.state.failed)
              return rulesOrCommentReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, sheetrulesReturn.Tree);
              break;
            }
            break;
        }
        rulesOrCommentReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          rulesOrCommentReturn.Tree = this.adaptor.RulePostProcessing(obj);
          this.adaptor.SetTokenBoundaries(rulesOrCommentReturn.Tree, (IToken) rulesOrCommentReturn.Start, (IToken) rulesOrCommentReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        rulesOrCommentReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) rulesOrCommentReturn.Start, this.input.LT(-1), ex);
      }
      return rulesOrCommentReturn;
    }

    [GrammarRule("styleimport")]
    private CssParser.styleimport_return styleimport()
    {
      CssParser.styleimport_return styleimportReturn = new CssParser.styleimport_return(this);
      styleimportReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token IMPORT_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token SEMICOLON");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule stringoruri");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule media_query_list");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 44, CssParser.Follow._IMPORT_SYM_in_styleimport784);
        if (this.state.failed)
          return styleimportReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        this.PushFollow(CssParser.Follow._stringoruri_in_styleimport786);
        CssParser.stringoruri_return stringoruriReturn = this.stringoruri();
        this.PopFollow();
        if (this.state.failed)
          return styleimportReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(stringoruriReturn.Tree);
        int num = 2;
        switch (this.input.LA(1))
        {
          case 12:
          case 41:
          case 63:
          case 66:
            num = 1;
            break;
        }
        if (num == 1)
        {
          this.PushFollow(CssParser.Follow._media_query_list_in_styleimport788);
          CssParser.media_query_list_return mediaQueryListReturn = this.media_query_list();
          this.PopFollow();
          if (this.state.failed)
            return styleimportReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream2.Add(mediaQueryListReturn.Tree);
        }
        CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 79, CssParser.Follow._SEMICOLON_in_styleimport791);
        if (this.state.failed)
          return styleimportReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el2);
        if (this.state.backtracking == 0)
        {
          styleimportReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", styleimportReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(138, "IMPORT"), oldRoot);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          if (ruleSubtreeStream2.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          styleimportReturn.Tree = obj1;
        }
        styleimportReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          styleimportReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(styleimportReturn.Tree, (IToken) styleimportReturn.Start, (IToken) styleimportReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        styleimportReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) styleimportReturn.Start, this.input.LT(-1), ex);
      }
      return styleimportReturn;
    }

    [GrammarRule("namespace")]
    private CssParser.namespace_return @namespace()
    {
      CssParser.namespace_return namespaceReturn = new CssParser.namespace_return(this);
      namespaceReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token NAMESPACE_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token SEMICOLON");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule namespace_prefix");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule stringoruri");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 58, CssParser.Follow._NAMESPACE_SYM_in_namespace826);
        if (this.state.failed)
          return namespaceReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        int num = 2;
        if (this.input.LA(1) == 41)
          num = 1;
        if (num == 1)
        {
          this.PushFollow(CssParser.Follow._namespace_prefix_in_namespace828);
          CssParser.namespace_prefix_return namespacePrefixReturn = this.namespace_prefix();
          this.PopFollow();
          if (this.state.failed)
            return namespaceReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(namespacePrefixReturn.Tree);
        }
        this.PushFollow(CssParser.Follow._stringoruri_in_namespace831);
        CssParser.stringoruri_return stringoruriReturn = this.stringoruri();
        this.PopFollow();
        if (this.state.failed)
          return namespaceReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream2.Add(stringoruriReturn.Tree);
        CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 79, CssParser.Follow._SEMICOLON_in_namespace833);
        if (this.state.failed)
          return namespaceReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el2);
        if (this.state.backtracking == 0)
        {
          namespaceReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", namespaceReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(154, "NAMESPACE"), oldRoot);
          if (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          this.adaptor.AddChild(obj1, obj2);
          namespaceReturn.Tree = obj1;
        }
        namespaceReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          namespaceReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(namespaceReturn.Tree, (IToken) namespaceReturn.Start, (IToken) namespaceReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        namespaceReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) namespaceReturn.Start, this.input.LT(-1), ex);
      }
      return namespaceReturn;
    }

    [GrammarRule("namespace_prefix")]
    private CssParser.namespace_prefix_return namespace_prefix()
    {
      CssParser.namespace_prefix_return namespacePrefixReturn = new CssParser.namespace_prefix_return(this);
      namespacePrefixReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      try
      {
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_namespace_prefix865);
        if (this.state.failed)
          return namespacePrefixReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          namespacePrefixReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", namespacePrefixReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(156, "NAMESPACE_PREFIX"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          namespacePrefixReturn.Tree = obj1;
        }
        namespacePrefixReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          namespacePrefixReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(namespacePrefixReturn.Tree, (IToken) namespacePrefixReturn.Start, (IToken) namespacePrefixReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        namespacePrefixReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) namespacePrefixReturn.Start, this.input.LT(-1), ex);
      }
      return namespacePrefixReturn;
    }

    [GrammarRule("wg_dpi")]
    private CssParser.wg_dpi_return wg_dpi()
    {
      CssParser.wg_dpi_return wgDpiReturn = new CssParser.wg_dpi_return(this);
      wgDpiReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token WG_DPI_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token NUMBER");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token SEMICOLON");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 104, CssParser.Follow._WG_DPI_SYM_in_wg_dpi894);
        if (this.state.failed)
          return wgDpiReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 64, CssParser.Follow._NUMBER_in_wg_dpi896);
        if (this.state.failed)
          return wgDpiReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el2);
        CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 79, CssParser.Follow._SEMICOLON_in_wg_dpi898);
        if (this.state.failed)
          return wgDpiReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream3.Add((object) el3);
        if (this.state.backtracking == 0)
        {
          wgDpiReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", wgDpiReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(189, "WG_DPI"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          wgDpiReturn.Tree = obj1;
        }
        wgDpiReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          wgDpiReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(wgDpiReturn.Tree, (IToken) wgDpiReturn.Start, (IToken) wgDpiReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        wgDpiReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) wgDpiReturn.Start, this.input.LT(-1), ex);
      }
      return wgDpiReturn;
    }

    [GrammarRule("media")]
    private CssParser.media_return media()
    {
      CssParser.media_return mediaReturn = new CssParser.media_return(this);
      mediaReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token MEDIA_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule media_query_list");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule ruleset");
      RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule page");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 52, CssParser.Follow._MEDIA_SYM_in_media930);
        if (this.state.failed)
          return mediaReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        int num1 = 2;
        switch (this.input.LA(1))
        {
          case 12:
          case 41:
          case 63:
          case 66:
            num1 = 1;
            break;
        }
        if (num1 == 1)
        {
          this.PushFollow(CssParser.Follow._media_query_list_in_media932);
          CssParser.media_query_list_return mediaQueryListReturn = this.media_query_list();
          this.PopFollow();
          if (this.state.failed)
            return mediaReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(mediaQueryListReturn.Tree);
        }
        CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 18, CssParser.Follow._CURLY_BEGIN_in_media935);
        if (this.state.failed)
          return mediaReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el2);
        while (true)
        {
          CssParser.page_return pageReturn;
          do
          {
            CssParser.ruleset_return rulesetReturn;
            do
            {
              int num2 = 3;
              int num3 = this.input.LA(1);
              if (num3 == 7 || num3 >= 14 && num3 <= 15 || num3 == 38 || num3 == 41 || num3 == 70 || num3 == 76 || num3 == 82 || num3 == 84)
                num2 = 1;
              else if (num3 == 68)
                num2 = 2;
              switch (num2)
              {
                case 1:
                  this.PushFollow(CssParser.Follow._ruleset_in_media939);
                  rulesetReturn = this.ruleset();
                  this.PopFollow();
                  if (this.state.failed)
                    return mediaReturn;
                  continue;
                case 2:
                  goto label_25;
                default:
                  goto label_29;
              }
            }
            while (this.state.backtracking != 0);
            ruleSubtreeStream2.Add(rulesetReturn.Tree);
            continue;
label_25:
            this.PushFollow(CssParser.Follow._page_in_media943);
            pageReturn = this.page();
            this.PopFollow();
            if (this.state.failed)
              return mediaReturn;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream3.Add(pageReturn.Tree);
        }
label_29:
        CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 19, CssParser.Follow._CURLY_END_in_media948);
        if (this.state.failed)
          return mediaReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream3.Add((object) el3);
        if (this.state.backtracking == 0)
        {
          mediaReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", mediaReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(147, "MEDIA"), oldRoot1);
          if (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot2 = this.adaptor.Nil();
            object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(172, "RULESETS"), oldRoot2);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj3, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj3);
          }
          ruleSubtreeStream2.Reset();
          if (ruleSubtreeStream3.HasNext)
          {
            object oldRoot3 = this.adaptor.Nil();
            object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(163, "PAGE"), oldRoot3);
            while (ruleSubtreeStream3.HasNext)
              this.adaptor.AddChild(obj4, ruleSubtreeStream3.NextTree());
            ruleSubtreeStream3.Reset();
            this.adaptor.AddChild(obj2, obj4);
          }
          ruleSubtreeStream3.Reset();
          this.adaptor.AddChild(obj1, obj2);
          mediaReturn.Tree = obj1;
        }
        mediaReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          mediaReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(mediaReturn.Tree, (IToken) mediaReturn.Start, (IToken) mediaReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        mediaReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) mediaReturn.Start, this.input.LT(-1), ex);
      }
      return mediaReturn;
    }

    [GrammarRule("media_query_list")]
    private CssParser.media_query_list_return media_query_list()
    {
      CssParser.media_query_list_return mediaQueryListReturn = new CssParser.media_query_list_return(this);
      mediaQueryListReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token COMMA");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule media_query");
      try
      {
        this.PushFollow(CssParser.Follow._media_query_in_media_query_list997);
        CssParser.media_query_return mediaQueryReturn1 = this.media_query();
        this.PopFollow();
        if (this.state.failed)
          return mediaQueryListReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(mediaQueryReturn1.Tree);
        while (true)
        {
          CssParser.media_query_return mediaQueryReturn2;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 16)
              num = 1;
            if (num == 1)
            {
              CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 16, CssParser.Follow._COMMA_in_media_query_list1000);
              if (this.state.failed)
                return mediaQueryListReturn;
              if (this.state.backtracking == 0)
                rewriteRuleTokenStream.Add((object) el);
              this.PushFollow(CssParser.Follow._media_query_in_media_query_list1002);
              mediaQueryReturn2 = this.media_query();
              this.PopFollow();
              if (this.state.failed)
                return mediaQueryListReturn;
            }
            else
              goto label_16;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream1.Add(mediaQueryReturn2.Tree);
        }
label_16:
        if (this.state.backtracking == 0)
        {
          mediaQueryListReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", mediaQueryListReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(152, "MEDIA_QUERY_LIST"), oldRoot);
          while (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj1, obj2);
          mediaQueryListReturn.Tree = obj1;
        }
        mediaQueryListReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          mediaQueryListReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(mediaQueryListReturn.Tree, (IToken) mediaQueryListReturn.Start, (IToken) mediaQueryListReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        mediaQueryListReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) mediaQueryListReturn.Start, this.input.LT(-1), ex);
      }
      return mediaQueryListReturn;
    }

    [GrammarRule("media_query")]
    private CssParser.media_query_return media_query()
    {
      CssParser.media_query_return mediaQueryReturn = new CssParser.media_query_return(this);
      mediaQueryReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token ONLY");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token NOT");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token AND");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule media_type");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule media_expression");
      try
      {
        int num1;
        switch (this.input.LA(1))
        {
          case 12:
            num1 = 2;
            break;
          case 41:
          case 63:
          case 66:
            num1 = 1;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 14, 0, (IIntStream) this.input);
            this.state.failed = true;
            return mediaQueryReturn;
        }
        switch (num1)
        {
          case 1:
            int num2 = 3;
            switch (this.input.LA(1))
            {
              case 63:
                num2 = 2;
                break;
              case 66:
                num2 = 1;
                break;
            }
            switch (num2)
            {
              case 1:
                CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 66, CssParser.Follow._ONLY_in_media_query1036);
                if (this.state.failed)
                  return mediaQueryReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream1.Add((object) el1);
                  break;
                }
                break;
              case 2:
                CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 63, CssParser.Follow._NOT_in_media_query1040);
                if (this.state.failed)
                  return mediaQueryReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream2.Add((object) el2);
                  break;
                }
                break;
            }
            this.PushFollow(CssParser.Follow._media_type_in_media_query1044);
            CssParser.media_type_return mediaTypeReturn = this.media_type();
            this.PopFollow();
            if (this.state.failed)
              return mediaQueryReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream1.Add(mediaTypeReturn.Tree);
            while (true)
            {
              CssParser.media_expression_return expressionReturn;
              do
              {
                int num3 = 2;
                if (this.input.LA(1) == 5)
                  num3 = 1;
                if (num3 == 1)
                {
                  CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 5, CssParser.Follow._AND_in_media_query1047);
                  if (this.state.failed)
                    return mediaQueryReturn;
                  if (this.state.backtracking == 0)
                    rewriteRuleTokenStream3.Add((object) el3);
                  this.PushFollow(CssParser.Follow._media_expression_in_media_query1049);
                  expressionReturn = this.media_expression();
                  this.PopFollow();
                  if (this.state.failed)
                    return mediaQueryReturn;
                }
                else
                  goto label_35;
              }
              while (this.state.backtracking != 0);
              ruleSubtreeStream2.Add(expressionReturn.Tree);
            }
label_35:
            if (this.state.backtracking == 0)
            {
              mediaQueryReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", mediaQueryReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot1 = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(151, "MEDIA_QUERY"), oldRoot1);
              if (rewriteRuleTokenStream1.HasNext)
              {
                object oldRoot2 = this.adaptor.Nil();
                object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(161, "ONLY_TEXT"), oldRoot2);
                this.adaptor.AddChild(obj3, rewriteRuleTokenStream1.NextNode());
                this.adaptor.AddChild(obj2, obj3);
              }
              rewriteRuleTokenStream1.Reset();
              if (rewriteRuleTokenStream2.HasNext)
              {
                object oldRoot3 = this.adaptor.Nil();
                object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(159, "NOT_TEXT"), oldRoot3);
                this.adaptor.AddChild(obj4, rewriteRuleTokenStream2.NextNode());
                this.adaptor.AddChild(obj2, obj4);
              }
              rewriteRuleTokenStream2.Reset();
              this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
              if (ruleSubtreeStream2.HasNext)
              {
                object oldRoot4 = this.adaptor.Nil();
                object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(149, "MEDIA_EXPRESSIONS"), oldRoot4);
                while (ruleSubtreeStream2.HasNext)
                  this.adaptor.AddChild(obj5, ruleSubtreeStream2.NextTree());
                ruleSubtreeStream2.Reset();
                this.adaptor.AddChild(obj2, obj5);
              }
              ruleSubtreeStream2.Reset();
              this.adaptor.AddChild(obj1, obj2);
              mediaQueryReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            this.PushFollow(CssParser.Follow._media_expression_in_media_query1087);
            CssParser.media_expression_return expressionReturn1 = this.media_expression();
            this.PopFollow();
            if (this.state.failed)
              return mediaQueryReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream2.Add(expressionReturn1.Tree);
            while (true)
            {
              CssParser.media_expression_return expressionReturn2;
              do
              {
                int num4 = 2;
                if (this.input.LA(1) == 5)
                  num4 = 1;
                if (num4 == 1)
                {
                  CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 5, CssParser.Follow._AND_in_media_query1090);
                  if (this.state.failed)
                    return mediaQueryReturn;
                  if (this.state.backtracking == 0)
                    rewriteRuleTokenStream3.Add((object) el4);
                  this.PushFollow(CssParser.Follow._media_expression_in_media_query1092);
                  expressionReturn2 = this.media_expression();
                  this.PopFollow();
                  if (this.state.failed)
                    return mediaQueryReturn;
                }
                else
                  goto label_61;
              }
              while (this.state.backtracking != 0);
              ruleSubtreeStream2.Add(expressionReturn2.Tree);
            }
label_61:
            if (this.state.backtracking == 0)
            {
              mediaQueryReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", mediaQueryReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot5 = this.adaptor.Nil();
              object obj6 = this.adaptor.BecomeRoot(this.adaptor.Create(151, "MEDIA_QUERY"), oldRoot5);
              object oldRoot6 = this.adaptor.Nil();
              object obj7 = this.adaptor.BecomeRoot(this.adaptor.Create(149, "MEDIA_EXPRESSIONS"), oldRoot6);
              while (ruleSubtreeStream2.HasNext)
                this.adaptor.AddChild(obj7, ruleSubtreeStream2.NextTree());
              ruleSubtreeStream2.Reset();
              this.adaptor.AddChild(obj6, obj7);
              this.adaptor.AddChild(obj1, obj6);
              mediaQueryReturn.Tree = obj1;
              break;
            }
            break;
        }
        mediaQueryReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          mediaQueryReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(mediaQueryReturn.Tree, (IToken) mediaQueryReturn.Start, (IToken) mediaQueryReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        mediaQueryReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) mediaQueryReturn.Start, this.input.LT(-1), ex);
      }
      return mediaQueryReturn;
    }

    [GrammarRule("media_type")]
    private CssParser.media_type_return media_type()
    {
      CssParser.media_type_return mediaTypeReturn = new CssParser.media_type_return(this);
      mediaTypeReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      try
      {
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_media_type1122);
        if (this.state.failed)
          return mediaTypeReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          mediaTypeReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", mediaTypeReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(153, "MEDIA_TYPE"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          mediaTypeReturn.Tree = obj1;
        }
        mediaTypeReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          mediaTypeReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(mediaTypeReturn.Tree, (IToken) mediaTypeReturn.Start, (IToken) mediaTypeReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        mediaTypeReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) mediaTypeReturn.Start, this.input.LT(-1), ex);
      }
      return mediaTypeReturn;
    }

    [GrammarRule("media_expression")]
    private CssParser.media_expression_return media_expression()
    {
      CssParser.media_expression_return expressionReturn = new CssParser.media_expression_return(this);
      expressionReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token CIRCLE_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token COLON");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token CIRCLE_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule media_feature");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule expr");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 12, CssParser.Follow._CIRCLE_BEGIN_in_media_expression1145);
        if (this.state.failed)
          return expressionReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        this.PushFollow(CssParser.Follow._media_feature_in_media_expression1147);
        CssParser.media_feature_return mediaFeatureReturn = this.media_feature();
        this.PopFollow();
        if (this.state.failed)
          return expressionReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(mediaFeatureReturn.Tree);
        int num = 2;
        if (this.input.LA(1) == 15)
          num = 1;
        if (num == 1)
        {
          CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_media_expression1150);
          if (this.state.failed)
            return expressionReturn;
          if (this.state.backtracking == 0)
            rewriteRuleTokenStream2.Add((object) el2);
          this.PushFollow(CssParser.Follow._expr_in_media_expression1152);
          CssParser.expr_return exprReturn = this.expr();
          this.PopFollow();
          if (this.state.failed)
            return expressionReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream2.Add(exprReturn.Tree);
        }
        CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 13, CssParser.Follow._CIRCLE_END_in_media_expression1156);
        if (this.state.failed)
          return expressionReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream3.Add((object) el3);
        if (this.state.backtracking == 0)
        {
          expressionReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", expressionReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(148, "MEDIA_EXPRESSION"), oldRoot);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          if (ruleSubtreeStream2.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          expressionReturn.Tree = obj1;
        }
        expressionReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          expressionReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(expressionReturn.Tree, (IToken) expressionReturn.Start, (IToken) expressionReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        expressionReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) expressionReturn.Start, this.input.LT(-1), ex);
      }
      return expressionReturn;
    }

    [GrammarRule("media_feature")]
    private CssParser.media_feature_return media_feature()
    {
      CssParser.media_feature_return mediaFeatureReturn = new CssParser.media_feature_return(this);
      mediaFeatureReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token REPLACEMENTTOKEN");
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 41:
            num = 1;
            break;
          case 76:
            num = 2;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 16, 0, (IIntStream) this.input);
            this.state.failed = true;
            return mediaFeatureReturn;
        }
        switch (num)
        {
          case 1:
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_media_feature1183);
            if (this.state.failed)
              return mediaFeatureReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream1.Add((object) el1);
            if (this.state.backtracking == 0)
            {
              mediaFeatureReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", mediaFeatureReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(150, "MEDIA_FEATURE"), oldRoot);
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
              this.adaptor.AddChild(obj1, obj2);
              mediaFeatureReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 76, CssParser.Follow._REPLACEMENTTOKEN_in_media_feature1197);
            if (this.state.failed)
              return mediaFeatureReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el2);
            if (this.state.backtracking == 0)
            {
              mediaFeatureReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", mediaFeatureReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(150, "MEDIA_FEATURE"), oldRoot);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream2.NextNode());
              this.adaptor.AddChild(obj1, obj3);
              mediaFeatureReturn.Tree = obj1;
              break;
            }
            break;
        }
        mediaFeatureReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          mediaFeatureReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(mediaFeatureReturn.Tree, (IToken) mediaFeatureReturn.Start, (IToken) mediaFeatureReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        mediaFeatureReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) mediaFeatureReturn.Start, this.input.LT(-1), ex);
      }
      return mediaFeatureReturn;
    }

    [GrammarRule("page")]
    private CssParser.page_return page()
    {
      CssParser.page_return pageReturn = new CssParser.page_return(this);
      pageReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token PAGE_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token SEMICOLON");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule pseudo_page");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule declaration");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 68, CssParser.Follow._PAGE_SYM_in_page1224);
        if (this.state.failed)
          return pageReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        int num1 = 2;
        if (this.input.LA(1) == 15)
          num1 = 1;
        if (num1 == 1)
        {
          this.PushFollow(CssParser.Follow._pseudo_page_in_page1226);
          CssParser.pseudo_page_return pseudoPageReturn = this.pseudo_page();
          this.PopFollow();
          if (this.state.failed)
            return pageReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(pseudoPageReturn.Tree);
        }
        CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 18, CssParser.Follow._CURLY_BEGIN_in_page1229);
        if (this.state.failed)
          return pageReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el2);
        while (true)
        {
          CommonToken el3;
          do
          {
            int num2;
            do
            {
              int num3 = 2;
              int num4 = this.input.LA(1);
              if (num4 >= 41 && num4 <= 42 || num4 == 76 || num4 == 84)
                num3 = 1;
              if (num3 == 1)
              {
                this.PushFollow(CssParser.Follow._declaration_in_page1232);
                CssParser.declaration_return declarationReturn = this.declaration();
                this.PopFollow();
                if (this.state.failed)
                  return pageReturn;
                if (this.state.backtracking == 0)
                  ruleSubtreeStream2.Add(declarationReturn.Tree);
                num2 = 2;
                if (this.input.LA(1) == 79)
                  num2 = 1;
              }
              else
                goto label_30;
            }
            while (num2 != 1);
            el3 = (CommonToken) this.Match((IIntStream) this.input, 79, CssParser.Follow._SEMICOLON_in_page1234);
            if (this.state.failed)
              return pageReturn;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream3.Add((object) el3);
        }
label_30:
        CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 19, CssParser.Follow._CURLY_END_in_page1239);
        if (this.state.failed)
          return pageReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream4.Add((object) el4);
        if (this.state.backtracking == 0)
        {
          pageReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", pageReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(163, "PAGE"), oldRoot1);
          if (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot2 = this.adaptor.Nil();
            object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(123, "DECLARATIONS"), oldRoot2);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj3, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj3);
          }
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          pageReturn.Tree = obj1;
        }
        pageReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          pageReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(pageReturn.Tree, (IToken) pageReturn.Start, (IToken) pageReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        pageReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) pageReturn.Start, this.input.LT(-1), ex);
      }
      return pageReturn;
    }

    [GrammarRule("pseudo_page")]
    private CssParser.pseudo_page_return pseudo_page()
    {
      CssParser.pseudo_page_return pseudoPageReturn = new CssParser.pseudo_page_return(this);
      pseudoPageReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token COLON");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_pseudo_page1280);
        if (this.state.failed)
          return pseudoPageReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_pseudo_page1282);
        if (this.state.failed)
          return pseudoPageReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el2);
        if (this.state.backtracking == 0)
        {
          pseudoPageReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", pseudoPageReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(167, "PSEUDO_PAGE"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          pseudoPageReturn.Tree = obj1;
        }
        pseudoPageReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          pseudoPageReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(pseudoPageReturn.Tree, (IToken) pseudoPageReturn.Start, (IToken) pseudoPageReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        pseudoPageReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) pseudoPageReturn.Start, this.input.LT(-1), ex);
      }
      return pseudoPageReturn;
    }

    [GrammarRule("operator")]
    private CssParser.operator_return @operator()
    {
      CssParser.operator_return operatorReturn = new CssParser.operator_return(this);
      operatorReturn.Start = (CommonToken) this.input.LT(1);
      try
      {
        object obj = this.adaptor.Nil();
        CommonToken payload = (CommonToken) this.input.LT(1);
        if (this.input.LA(1) == 16 || this.input.LA(1) == 28 || this.input.LA(1) == 31 || this.input.LA(1) == 84)
        {
          this.input.Consume();
          if (this.state.backtracking == 0)
            this.adaptor.AddChild(obj, this.adaptor.Create((IToken) payload));
          this.state.errorRecovery = false;
          this.state.failed = false;
          operatorReturn.Stop = (CommonToken) this.input.LT(-1);
          if (this.state.backtracking == 0)
          {
            operatorReturn.Tree = this.adaptor.RulePostProcessing(obj);
            this.adaptor.SetTokenBoundaries(operatorReturn.Tree, (IToken) operatorReturn.Start, (IToken) operatorReturn.Stop);
          }
        }
        else
        {
          if (this.state.backtracking <= 0)
            throw new MismatchedSetException((BitSet) null, (IIntStream) this.input);
          this.state.failed = true;
          return operatorReturn;
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        operatorReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) operatorReturn.Start, this.input.LT(-1), ex);
      }
      return operatorReturn;
    }

    [GrammarRule("unary_operator")]
    private CssParser.unary_operator_return unary_operator()
    {
      CssParser.unary_operator_return unaryOperatorReturn = new CssParser.unary_operator_return(this);
      unaryOperatorReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token MINUS");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token PLUS");
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 53:
            num = 1;
            break;
          case 71:
            num = 2;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 20, 0, (IIntStream) this.input);
            this.state.failed = true;
            return unaryOperatorReturn;
        }
        switch (num)
        {
          case 1:
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 53, CssParser.Follow._MINUS_in_unary_operator1349);
            if (this.state.failed)
              return unaryOperatorReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream1.Add((object) el1);
            if (this.state.backtracking == 0)
            {
              unaryOperatorReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", unaryOperatorReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(185, "UNARY"), oldRoot);
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
              this.adaptor.AddChild(obj1, obj2);
              unaryOperatorReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 71, CssParser.Follow._PLUS_in_unary_operator1365);
            if (this.state.failed)
              return unaryOperatorReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el2);
            if (this.state.backtracking == 0)
            {
              unaryOperatorReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", unaryOperatorReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(185, "UNARY"), oldRoot);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream2.NextNode());
              this.adaptor.AddChild(obj1, obj3);
              unaryOperatorReturn.Tree = obj1;
              break;
            }
            break;
        }
        unaryOperatorReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          unaryOperatorReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(unaryOperatorReturn.Tree, (IToken) unaryOperatorReturn.Start, (IToken) unaryOperatorReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        unaryOperatorReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) unaryOperatorReturn.Start, this.input.LT(-1), ex);
      }
      return unaryOperatorReturn;
    }

    [GrammarRule("property")]
    private CssParser.property_return property()
    {
      CssParser.property_return propertyReturn = new CssParser.property_return(this);
      propertyReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token STAR");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token IMPORTANT_COMMENTS");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token REPLACEMENTTOKEN");
      try
      {
        int num1;
        switch (this.input.LA(1))
        {
          case 41:
          case 84:
            num1 = 1;
            break;
          case 76:
            num1 = 2;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 23, 0, (IIntStream) this.input);
            this.state.failed = true;
            return propertyReturn;
        }
        switch (num1)
        {
          case 1:
            int num2 = 2;
            if (this.input.LA(1) == 84)
              num2 = 1;
            if (num2 == 1)
            {
              CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 84, CssParser.Follow._STAR_in_property1394);
              if (this.state.failed)
                return propertyReturn;
              if (this.state.backtracking == 0)
                rewriteRuleTokenStream1.Add((object) el);
            }
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_property1398);
            if (this.state.failed)
              return propertyReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el1);
            while (true)
            {
              CommonToken el2;
              do
              {
                int num3 = 2;
                if (this.input.LA(1) == 42)
                  num3 = 1;
                if (num3 == 1)
                {
                  el2 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_property1400);
                  if (this.state.failed)
                    return propertyReturn;
                }
                else
                  goto label_26;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream3.Add((object) el2);
            }
label_26:
            if (this.state.backtracking == 0)
            {
              propertyReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", propertyReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(164, "PROPERTY"), oldRoot);
              if (rewriteRuleTokenStream1.HasNext)
                this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
              rewriteRuleTokenStream1.Reset();
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
              while (rewriteRuleTokenStream3.HasNext)
                this.adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
              rewriteRuleTokenStream3.Reset();
              this.adaptor.AddChild(obj1, obj2);
              propertyReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 76, CssParser.Follow._REPLACEMENTTOKEN_in_property1424);
            if (this.state.failed)
              return propertyReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream4.Add((object) el3);
            if (this.state.backtracking == 0)
            {
              propertyReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", propertyReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(164, "PROPERTY"), oldRoot);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream4.NextNode());
              this.adaptor.AddChild(obj1, obj3);
              propertyReturn.Tree = obj1;
              break;
            }
            break;
        }
        propertyReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          propertyReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(propertyReturn.Tree, (IToken) propertyReturn.Start, (IToken) propertyReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        propertyReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) propertyReturn.Start, this.input.LT(-1), ex);
      }
      return propertyReturn;
    }

    [GrammarRule("ruleset")]
    private CssParser.ruleset_return ruleset()
    {
      CssParser.ruleset_return rulesetReturn = new CssParser.ruleset_return(this);
      rulesetReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token SEMICOLON");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token IMPORTANT_COMMENTS");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule selectors_group");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule declaration");
      try
      {
        this.PushFollow(CssParser.Follow._selectors_group_in_ruleset1454);
        CssParser.selectors_group_return selectorsGroupReturn = this.selectors_group();
        this.PopFollow();
        if (this.state.failed)
          return rulesetReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(selectorsGroupReturn.Tree);
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 18, CssParser.Follow._CURLY_BEGIN_in_ruleset1460);
        if (this.state.failed)
          return rulesetReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        while (true)
        {
          CommonToken el2;
          do
          {
            int num1;
            do
            {
              int num2;
              try
              {
                num2 = this.dfa25.Predict((IIntStream) this.input);
              }
              catch (NoViableAltException ex)
              {
                throw;
              }
              if (num2 == 1)
              {
                this.PushFollow(CssParser.Follow._declaration_in_ruleset1467);
                CssParser.declaration_return declarationReturn = this.declaration();
                this.PopFollow();
                if (this.state.failed)
                  return rulesetReturn;
                if (this.state.backtracking == 0)
                  ruleSubtreeStream2.Add(declarationReturn.Tree);
                num1 = 2;
                if (this.input.LA(1) == 79)
                  num1 = 1;
              }
              else
                goto label_24;
            }
            while (num1 != 1);
            el2 = (CommonToken) this.Match((IIntStream) this.input, 79, CssParser.Follow._SEMICOLON_in_ruleset1469);
            if (this.state.failed)
              return rulesetReturn;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream2.Add((object) el2);
        }
label_24:
        while (true)
        {
          CommonToken el3;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 42)
              num = 1;
            if (num == 1)
            {
              el3 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_ruleset1475);
              if (this.state.failed)
                return rulesetReturn;
            }
            else
              goto label_31;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream3.Add((object) el3);
        }
label_31:
        CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 19, CssParser.Follow._CURLY_END_in_ruleset1482);
        if (this.state.failed)
          return rulesetReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream4.Add((object) el4);
        if (this.state.backtracking == 0)
        {
          rulesetReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", rulesetReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(171, "RULESET"), oldRoot1);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot2 = this.adaptor.Nil();
            object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(123, "DECLARATIONS"), oldRoot2);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj3, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj3);
          }
          ruleSubtreeStream2.Reset();
          while (rewriteRuleTokenStream3.HasNext)
            this.adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
          rewriteRuleTokenStream3.Reset();
          this.adaptor.AddChild(obj1, obj2);
          rulesetReturn.Tree = obj1;
        }
        rulesetReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          rulesetReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(rulesetReturn.Tree, (IToken) rulesetReturn.Start, (IToken) rulesetReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        rulesetReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) rulesetReturn.Start, this.input.LT(-1), ex);
      }
      return rulesetReturn;
    }

    [GrammarRule("selectors_group")]
    private CssParser.selectors_group_return selectors_group()
    {
      CssParser.selectors_group_return selectorsGroupReturn = new CssParser.selectors_group_return(this);
      selectorsGroupReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token COMMA");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule selector");
      try
      {
        this.PushFollow(CssParser.Follow._selector_in_selectors_group1523);
        CssParser.selector_return selectorReturn1 = this.selector();
        this.PopFollow();
        if (this.state.failed)
          return selectorsGroupReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(selectorReturn1.Tree);
        while (true)
        {
          CssParser.selector_return selectorReturn2;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 16)
              num = 1;
            if (num == 1)
            {
              CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 16, CssParser.Follow._COMMA_in_selectors_group1526);
              if (this.state.failed)
                return selectorsGroupReturn;
              if (this.state.backtracking == 0)
                rewriteRuleTokenStream.Add((object) el);
              this.PushFollow(CssParser.Follow._selector_in_selectors_group1528);
              selectorReturn2 = this.selector();
              this.PopFollow();
              if (this.state.failed)
                return selectorsGroupReturn;
            }
            else
              goto label_16;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream1.Add(selectorReturn2.Tree);
        }
label_16:
        if (this.state.backtracking == 0)
        {
          selectorsGroupReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", selectorsGroupReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(174, "SELECTORS_GROUP"), oldRoot);
          while (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj1, obj2);
          selectorsGroupReturn.Tree = obj1;
        }
        selectorsGroupReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          selectorsGroupReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(selectorsGroupReturn.Tree, (IToken) selectorsGroupReturn.Start, (IToken) selectorsGroupReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        selectorsGroupReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) selectorsGroupReturn.Start, this.input.LT(-1), ex);
      }
      return selectorsGroupReturn;
    }

    [GrammarRule("selector")]
    private CssParser.selector_return selector()
    {
      CssParser.selector_return selectorReturn = new CssParser.selector_return(this);
      selectorReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule simple_selector_sequence");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule combinator_simple_selector_sequence");
      try
      {
        this.PushFollow(CssParser.Follow._simple_selector_sequence_in_selector1559);
        CssParser.simple_selector_sequence_return selectorSequenceReturn1 = this.simple_selector_sequence();
        this.PopFollow();
        if (this.state.failed)
          return selectorReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(selectorSequenceReturn1.Tree);
        while (true)
        {
          CssParser.combinator_simple_selector_sequence_return selectorSequenceReturn2;
          do
          {
            int num1 = 2;
            int num2 = this.input.LA(1);
            if (num2 == 7 || num2 >= 14 && num2 <= 15 || num2 == 35 || num2 == 38 || num2 == 41 || num2 >= 70 && num2 <= 71 || num2 == 76 || num2 == 82 || num2 == 84 || num2 == 89 || num2 == 105)
              num1 = 1;
            if (num1 == 1)
            {
              this.PushFollow(CssParser.Follow._combinator_simple_selector_sequence_in_selector1562);
              selectorSequenceReturn2 = this.combinator_simple_selector_sequence();
              this.PopFollow();
              if (this.state.failed)
                return selectorReturn;
            }
            else
              goto label_12;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream2.Add(selectorSequenceReturn2.Tree);
        }
label_12:
        if (this.state.backtracking == 0)
        {
          selectorReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", selectorReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(173, "SELECTOR"), oldRoot1);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot2 = this.adaptor.Nil();
            object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(121, "COMBINATOR_SIMPLE_SELECTOR_SEQUENCES"), oldRoot2);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj3, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj3);
          }
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          selectorReturn.Tree = obj1;
        }
        selectorReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          selectorReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(selectorReturn.Tree, (IToken) selectorReturn.Start, (IToken) selectorReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        selectorReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) selectorReturn.Start, this.input.LT(-1), ex);
      }
      return selectorReturn;
    }

    [GrammarRule("combinator_simple_selector_sequence")]
    private CssParser.combinator_simple_selector_sequence_return combinator_simple_selector_sequence()
    {
      CssParser.combinator_simple_selector_sequence_return selectorSequenceReturn1 = new CssParser.combinator_simple_selector_sequence_return(this);
      selectorSequenceReturn1.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule combinator");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule simple_selector_sequence");
      try
      {
        this.PushFollow(CssParser.Follow._combinator_in_combinator_simple_selector_sequence1601);
        CssParser.combinator_return combinatorReturn = this.combinator();
        this.PopFollow();
        if (this.state.failed)
          return selectorSequenceReturn1;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(combinatorReturn.Tree);
        this.PushFollow(CssParser.Follow._simple_selector_sequence_in_combinator_simple_selector_sequence1603);
        CssParser.simple_selector_sequence_return selectorSequenceReturn2 = this.simple_selector_sequence();
        this.PopFollow();
        if (this.state.failed)
          return selectorSequenceReturn1;
        if (this.state.backtracking == 0)
          ruleSubtreeStream2.Add(selectorSequenceReturn2.Tree);
        if (this.state.backtracking == 0)
        {
          selectorSequenceReturn1.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", selectorSequenceReturn1?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(120, "COMBINATOR_SIMPLE_SELECTOR"), oldRoot);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          this.adaptor.AddChild(obj1, obj2);
          selectorSequenceReturn1.Tree = obj1;
        }
        selectorSequenceReturn1.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          selectorSequenceReturn1.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(selectorSequenceReturn1.Tree, (IToken) selectorSequenceReturn1.Start, (IToken) selectorSequenceReturn1.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        selectorSequenceReturn1.Tree = this.adaptor.ErrorNode(this.input, (IToken) selectorSequenceReturn1.Start, this.input.LT(-1), ex);
      }
      return selectorSequenceReturn1;
    }

    [GrammarRule("combinator")]
    private CssParser.combinator_return combinator()
    {
      CssParser.combinator_return combinatorReturn = new CssParser.combinator_return(this);
      combinatorReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      CommonToken commonToken = (CommonToken) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token PLUS");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token GREATER");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token TILDE");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule whitespace");
      try
      {
        int num1 = this.input.LA(1);
        int num2;
        if (num1 == 35 || num1 == 71 || num1 == 89)
          num2 = 1;
        else if (num1 == 7 || num1 >= 14 && num1 <= 15 || num1 == 38 || num1 == 41 || num1 == 70 || num1 == 76 || num1 == 82 || num1 == 84 || num1 == 105)
        {
          num2 = 2;
        }
        else
        {
          if (this.state.backtracking <= 0)
            throw new NoViableAltException("", 30, 0, (IIntStream) this.input);
          this.state.failed = true;
          return combinatorReturn;
        }
        switch (num2)
        {
          case 1:
            int num3;
            switch (this.input.LA(1))
            {
              case 35:
                num3 = 2;
                break;
              case 71:
                num3 = 1;
                break;
              case 89:
                num3 = 3;
                break;
              default:
                if (this.state.backtracking <= 0)
                  throw new NoViableAltException("", 29, 0, (IIntStream) this.input);
                this.state.failed = true;
                return combinatorReturn;
            }
            switch (num3)
            {
              case 1:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 71, CssParser.Follow._PLUS_in_combinator1644);
                if (this.state.failed)
                  return combinatorReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream1.Add((object) commonToken);
                  break;
                }
                break;
              case 2:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 35, CssParser.Follow._GREATER_in_combinator1655);
                if (this.state.failed)
                  return combinatorReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream2.Add((object) commonToken);
                  break;
                }
                break;
              case 3:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 89, CssParser.Follow._TILDE_in_combinator1666);
                if (this.state.failed)
                  return combinatorReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream3.Add((object) commonToken);
                  break;
                }
                break;
            }
            if (this.state.backtracking == 0)
            {
              combinatorReturn.Tree = obj1;
              RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token combinatorValue", (object) commonToken);
              RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", combinatorReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(119, "COMBINATOR"), oldRoot);
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream4.NextNode());
              this.adaptor.AddChild(obj1, obj2);
              combinatorReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            this.PushFollow(CssParser.Follow._whitespace_in_combinator1687);
            CssParser.whitespace_return whitespaceReturn = this.whitespace();
            this.PopFollow();
            if (this.state.failed)
              return combinatorReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream1.Add(whitespaceReturn.Tree);
            if (this.state.backtracking == 0)
            {
              combinatorReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", combinatorReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(119, "COMBINATOR"), oldRoot);
              this.adaptor.AddChild(obj3, ruleSubtreeStream1.NextTree());
              this.adaptor.AddChild(obj1, obj3);
              combinatorReturn.Tree = obj1;
              break;
            }
            break;
        }
        combinatorReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          combinatorReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(combinatorReturn.Tree, (IToken) combinatorReturn.Start, (IToken) combinatorReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        combinatorReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) combinatorReturn.Start, this.input.LT(-1), ex);
      }
      return combinatorReturn;
    }

    [GrammarRule("whitespace")]
    private CssParser.whitespace_return whitespace()
    {
      CssParser.whitespace_return whitespaceReturn = new CssParser.whitespace_return(this);
      whitespaceReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      CommonToken oneElement = (CommonToken) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token WS");
      try
      {
        int num = 2;
        if (this.input.LA(1) == 105)
        {
          this.input.LA(2);
          if (this.EvaluatePredicate(new Action(this.synpred1_CssParser_fragment)))
            num = 1;
        }
        if (num == 1)
        {
          CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 105, CssParser.Follow._WS_in_whitespace1728);
          if (this.state.failed)
            return whitespaceReturn;
          if (this.state.backtracking == 0)
            rewriteRuleTokenStream1.Add((object) el);
        }
        if (this.state.backtracking == 0)
          oneElement = this.GetWhitespaceToken();
        if (this.state.backtracking == 0)
        {
          whitespaceReturn.Tree = obj1;
          RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token ws", (object) oneElement);
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", whitespaceReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(190, "WHITESPACE"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          whitespaceReturn.Tree = obj1;
        }
        whitespaceReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          whitespaceReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(whitespaceReturn.Tree, (IToken) whitespaceReturn.Start, (IToken) whitespaceReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        whitespaceReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) whitespaceReturn.Start, this.input.LT(-1), ex);
      }
      return whitespaceReturn;
    }

    [GrammarRule("simple_selector_sequence")]
    private CssParser.simple_selector_sequence_return simple_selector_sequence()
    {
      CssParser.simple_selector_sequence_return selectorSequenceReturn = new CssParser.simple_selector_sequence_return(this);
      selectorSequenceReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule universal");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule type_selector");
      RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule whitespace");
      RewriteRuleSubtreeStream ruleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule hashclassatnameattribpseudonegation");
      try
      {
        int num1 = this.input.LA(1);
        int num2;
        if (num1 == 41 || num1 == 70 || num1 == 84)
          num2 = 1;
        else if (num1 == 76 && this.EvaluatePredicate(new Action(this.synpred5_CssParser_fragment)))
          num2 = 2;
        else if (num1 == 38 && this.EvaluatePredicate(new Action(this.synpred5_CssParser_fragment)))
          num2 = 2;
        else if (num1 == 14 && this.EvaluatePredicate(new Action(this.synpred5_CssParser_fragment)))
          num2 = 2;
        else if (num1 == 7 && this.EvaluatePredicate(new Action(this.synpred5_CssParser_fragment)))
          num2 = 2;
        else if (num1 == 82 && this.EvaluatePredicate(new Action(this.synpred5_CssParser_fragment)))
          num2 = 2;
        else if (num1 == 15 && this.EvaluatePredicate(new Action(this.synpred5_CssParser_fragment)))
        {
          num2 = 2;
        }
        else
        {
          if (this.state.backtracking <= 0)
            throw new NoViableAltException("", 34, 0, (IIntStream) this.input);
          this.state.failed = true;
          return selectorSequenceReturn;
        }
        switch (num2)
        {
          case 1:
            int num3;
            switch (this.input.LA(1))
            {
              case 41:
                this.input.LA(2);
                if (this.EvaluatePredicate(new Action(this.synpred2_CssParser_fragment)))
                {
                  num3 = 1;
                  break;
                }
                if (this.EvaluatePredicate(new Action(this.synpred3_CssParser_fragment)))
                {
                  num3 = 2;
                  break;
                }
                if (this.state.backtracking <= 0)
                  throw new NoViableAltException("", 32, 1, (IIntStream) this.input);
                this.state.failed = true;
                return selectorSequenceReturn;
              case 70:
                switch (this.input.LA(2))
                {
                  case 41:
                    if (this.EvaluatePredicate(new Action(this.synpred3_CssParser_fragment)))
                    {
                      num3 = 2;
                      goto label_50;
                    }
                    else
                      break;
                  case 84:
                    this.input.LA(3);
                    if (this.EvaluatePredicate(new Action(this.synpred2_CssParser_fragment)))
                    {
                      num3 = 1;
                      goto label_50;
                    }
                    else if (this.EvaluatePredicate(new Action(this.synpred3_CssParser_fragment)))
                    {
                      num3 = 2;
                      goto label_50;
                    }
                    else
                    {
                      if (this.state.backtracking <= 0)
                        throw new NoViableAltException("", 32, 6, (IIntStream) this.input);
                      this.state.failed = true;
                      return selectorSequenceReturn;
                    }
                }
                if (this.state.backtracking <= 0)
                  throw new NoViableAltException("", 32, 3, (IIntStream) this.input);
                this.state.failed = true;
                return selectorSequenceReturn;
              case 84:
                this.input.LA(2);
                if (this.EvaluatePredicate(new Action(this.synpred2_CssParser_fragment)))
                {
                  num3 = 1;
                  break;
                }
                if (this.EvaluatePredicate(new Action(this.synpred3_CssParser_fragment)))
                {
                  num3 = 2;
                  break;
                }
                if (this.state.backtracking <= 0)
                  throw new NoViableAltException("", 32, 2, (IIntStream) this.input);
                this.state.failed = true;
                return selectorSequenceReturn;
              default:
                if (this.state.backtracking <= 0)
                  throw new NoViableAltException("", 32, 0, (IIntStream) this.input);
                this.state.failed = true;
                return selectorSequenceReturn;
            }
label_50:
            switch (num3)
            {
              case 1:
                this.PushFollow(CssParser.Follow._universal_in_simple_selector_sequence1783);
                CssParser.universal_return universalReturn = this.universal();
                this.PopFollow();
                if (this.state.failed)
                  return selectorSequenceReturn;
                if (this.state.backtracking == 0)
                {
                  ruleSubtreeStream1.Add(universalReturn.Tree);
                  break;
                }
                break;
              case 2:
                this.PushFollow(CssParser.Follow._type_selector_in_simple_selector_sequence1793);
                CssParser.type_selector_return typeSelectorReturn = this.type_selector();
                this.PopFollow();
                if (this.state.failed)
                  return selectorSequenceReturn;
                if (this.state.backtracking == 0)
                {
                  ruleSubtreeStream2.Add(typeSelectorReturn.Tree);
                  break;
                }
                break;
            }
            this.PushFollow(CssParser.Follow._whitespace_in_simple_selector_sequence1797);
            CssParser.whitespace_return whitespaceReturn = this.whitespace();
            this.PopFollow();
            if (this.state.failed)
              return selectorSequenceReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream3.Add(whitespaceReturn.Tree);
            int num4;
            try
            {
              num4 = this.dfa33.Predict((IIntStream) this.input);
            }
            catch (NoViableAltException ex)
            {
              throw;
            }
            if (num4 == 1)
            {
              this.PushFollow(CssParser.Follow._hashclassatnameattribpseudonegation_in_simple_selector_sequence1806);
              CssParser.hashclassatnameattribpseudonegation_return hashclassatnameattribpseudonegationReturn = this.hashclassatnameattribpseudonegation();
              this.PopFollow();
              if (this.state.failed)
                return selectorSequenceReturn;
              if (this.state.backtracking == 0)
                ruleSubtreeStream4.Add(hashclassatnameattribpseudonegationReturn.Tree);
            }
            if (this.state.backtracking == 0)
            {
              selectorSequenceReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream5 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", selectorSequenceReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot1 = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(177, "SIMPLE_SELECTOR_SEQUENCE"), oldRoot1);
              if (ruleSubtreeStream2.HasNext)
                this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
              ruleSubtreeStream2.Reset();
              if (ruleSubtreeStream1.HasNext)
                this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
              ruleSubtreeStream1.Reset();
              if (ruleSubtreeStream3.HasNext)
                this.adaptor.AddChild(obj2, ruleSubtreeStream3.NextTree());
              ruleSubtreeStream3.Reset();
              if (ruleSubtreeStream4.HasNext)
              {
                object oldRoot2 = this.adaptor.Nil();
                object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(134, "HASHCLASSATNAMEATTRIBPSEUDONEGATIONNODES"), oldRoot2);
                this.adaptor.AddChild(obj3, ruleSubtreeStream4.NextTree());
                this.adaptor.AddChild(obj2, obj3);
              }
              ruleSubtreeStream4.Reset();
              this.adaptor.AddChild(obj1, obj2);
              selectorSequenceReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            this.PushFollow(CssParser.Follow._hashclassatnameattribpseudonegation_in_simple_selector_sequence1848);
            CssParser.hashclassatnameattribpseudonegation_return hashclassatnameattribpseudonegationReturn1 = this.hashclassatnameattribpseudonegation();
            this.PopFollow();
            if (this.state.failed)
              return selectorSequenceReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream4.Add(hashclassatnameattribpseudonegationReturn1.Tree);
            if (this.state.backtracking == 0)
            {
              selectorSequenceReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream6 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", selectorSequenceReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot3 = this.adaptor.Nil();
              object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(177, "SIMPLE_SELECTOR_SEQUENCE"), oldRoot3);
              object oldRoot4 = this.adaptor.Nil();
              object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(134, "HASHCLASSATNAMEATTRIBPSEUDONEGATIONNODES"), oldRoot4);
              this.adaptor.AddChild(obj5, ruleSubtreeStream4.NextTree());
              this.adaptor.AddChild(obj4, obj5);
              this.adaptor.AddChild(obj1, obj4);
              selectorSequenceReturn.Tree = obj1;
              break;
            }
            break;
        }
        selectorSequenceReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          selectorSequenceReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(selectorSequenceReturn.Tree, (IToken) selectorSequenceReturn.Start, (IToken) selectorSequenceReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        selectorSequenceReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) selectorSequenceReturn.Start, this.input.LT(-1), ex);
      }
      return selectorSequenceReturn;
    }

    [GrammarRule("hashclassatnameattribpseudonegation")]
    private CssParser.hashclassatnameattribpseudonegation_return hashclassatnameattribpseudonegation()
    {
      CssParser.hashclassatnameattribpseudonegation_return hashclassatnameattribpseudonegationReturn = new CssParser.hashclassatnameattribpseudonegation_return(this);
      hashclassatnameattribpseudonegationReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token REPLACEMENTTOKEN");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule hash");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule class");
      RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule atname");
      RewriteRuleSubtreeStream ruleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule attrib");
      RewriteRuleSubtreeStream ruleSubtreeStream5 = new RewriteRuleSubtreeStream(this.adaptor, "rule pseudo");
      RewriteRuleSubtreeStream ruleSubtreeStream6 = new RewriteRuleSubtreeStream(this.adaptor, "rule negation");
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 7:
            num = 4;
            break;
          case 14:
            num = 3;
            break;
          case 15:
            switch (this.input.LA(2))
            {
              case 15:
              case 33:
              case 41:
              case 55:
              case 91:
                num = 6;
                break;
              case 63:
                num = 7;
                break;
              default:
                if (this.state.backtracking <= 0)
                  throw new NoViableAltException("", 35, 6, (IIntStream) this.input);
                this.state.failed = true;
                return hashclassatnameattribpseudonegationReturn;
            }
            break;
          case 38:
            num = 2;
            break;
          case 76:
            num = 1;
            break;
          case 82:
            num = 5;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 35, 0, (IIntStream) this.input);
            this.state.failed = true;
            return hashclassatnameattribpseudonegationReturn;
        }
        switch (num)
        {
          case 1:
            CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 76, CssParser.Follow._REPLACEMENTTOKEN_in_hashclassatnameattribpseudonegation1878);
            if (this.state.failed)
              return hashclassatnameattribpseudonegationReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream.Add((object) el);
            if (this.state.backtracking == 0)
            {
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream7 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashclassatnameattribpseudonegationReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot1 = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(rewriteRuleTokenStream.NextNode(), oldRoot1);
              object oldRoot2 = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(170, "REPLACEMENTTOKENIDENTIFIER"), oldRoot2);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream.NextNode());
              this.adaptor.AddChild(obj2, obj3);
              this.adaptor.AddChild(obj1, obj2);
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            this.PushFollow(CssParser.Follow._hash_in_hashclassatnameattribpseudonegation1902);
            CssParser.hash_return hashReturn = this.hash();
            this.PopFollow();
            if (this.state.failed)
              return hashclassatnameattribpseudonegationReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream1.Add(hashReturn.Tree);
            if (this.state.backtracking == 0)
            {
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream8 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashclassatnameattribpseudonegationReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(133, "HASHCLASSATNAMEATTRIBPSEUDONEGATION"), oldRoot);
              this.adaptor.AddChild(obj4, ruleSubtreeStream1.NextTree());
              this.adaptor.AddChild(obj1, obj4);
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              break;
            }
            break;
          case 3:
            this.PushFollow(CssParser.Follow._class_in_hashclassatnameattribpseudonegation1922);
            CssParser.class_return classReturn = this.@class();
            this.PopFollow();
            if (this.state.failed)
              return hashclassatnameattribpseudonegationReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream2.Add(classReturn.Tree);
            if (this.state.backtracking == 0)
            {
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream9 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashclassatnameattribpseudonegationReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(133, "HASHCLASSATNAMEATTRIBPSEUDONEGATION"), oldRoot);
              this.adaptor.AddChild(obj5, ruleSubtreeStream2.NextTree());
              this.adaptor.AddChild(obj1, obj5);
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              break;
            }
            break;
          case 4:
            this.PushFollow(CssParser.Follow._atname_in_hashclassatnameattribpseudonegation1942);
            CssParser.atname_return atnameReturn = this.atname();
            this.PopFollow();
            if (this.state.failed)
              return hashclassatnameattribpseudonegationReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream3.Add(atnameReturn.Tree);
            if (this.state.backtracking == 0)
            {
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream10 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashclassatnameattribpseudonegationReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj6 = this.adaptor.BecomeRoot(this.adaptor.Create(133, "HASHCLASSATNAMEATTRIBPSEUDONEGATION"), oldRoot);
              this.adaptor.AddChild(obj6, ruleSubtreeStream3.NextTree());
              this.adaptor.AddChild(obj1, obj6);
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              break;
            }
            break;
          case 5:
            this.PushFollow(CssParser.Follow._attrib_in_hashclassatnameattribpseudonegation1962);
            CssParser.attrib_return attribReturn = this.attrib();
            this.PopFollow();
            if (this.state.failed)
              return hashclassatnameattribpseudonegationReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream4.Add(attribReturn.Tree);
            if (this.state.backtracking == 0)
            {
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream11 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashclassatnameattribpseudonegationReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj7 = this.adaptor.BecomeRoot(this.adaptor.Create(133, "HASHCLASSATNAMEATTRIBPSEUDONEGATION"), oldRoot);
              this.adaptor.AddChild(obj7, ruleSubtreeStream4.NextTree());
              this.adaptor.AddChild(obj1, obj7);
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              break;
            }
            break;
          case 6:
            this.PushFollow(CssParser.Follow._pseudo_in_hashclassatnameattribpseudonegation1982);
            CssParser.pseudo_return pseudoReturn = this.pseudo();
            this.PopFollow();
            if (this.state.failed)
              return hashclassatnameattribpseudonegationReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream5.Add(pseudoReturn.Tree);
            if (this.state.backtracking == 0)
            {
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream12 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashclassatnameattribpseudonegationReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj8 = this.adaptor.BecomeRoot(this.adaptor.Create(133, "HASHCLASSATNAMEATTRIBPSEUDONEGATION"), oldRoot);
              this.adaptor.AddChild(obj8, ruleSubtreeStream5.NextTree());
              this.adaptor.AddChild(obj1, obj8);
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              break;
            }
            break;
          case 7:
            this.PushFollow(CssParser.Follow._negation_in_hashclassatnameattribpseudonegation2002);
            CssParser.negation_return negationReturn = this.negation();
            this.PopFollow();
            if (this.state.failed)
              return hashclassatnameattribpseudonegationReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream6.Add(negationReturn.Tree);
            if (this.state.backtracking == 0)
            {
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream13 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashclassatnameattribpseudonegationReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj9 = this.adaptor.BecomeRoot(this.adaptor.Create(133, "HASHCLASSATNAMEATTRIBPSEUDONEGATION"), oldRoot);
              this.adaptor.AddChild(obj9, ruleSubtreeStream6.NextTree());
              this.adaptor.AddChild(obj1, obj9);
              hashclassatnameattribpseudonegationReturn.Tree = obj1;
              break;
            }
            break;
        }
        hashclassatnameattribpseudonegationReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          hashclassatnameattribpseudonegationReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(hashclassatnameattribpseudonegationReturn.Tree, (IToken) hashclassatnameattribpseudonegationReturn.Start, (IToken) hashclassatnameattribpseudonegationReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        hashclassatnameattribpseudonegationReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) hashclassatnameattribpseudonegationReturn.Start, this.input.LT(-1), ex);
      }
      return hashclassatnameattribpseudonegationReturn;
    }

    [GrammarRule("type_selector")]
    private CssParser.type_selector_return type_selector()
    {
      CssParser.type_selector_return typeSelectorReturn = new CssParser.type_selector_return(this);
      typeSelectorReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule selector_namespace_prefix");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule element_name");
      try
      {
        int num = 2;
        switch (this.input.LA(1))
        {
          case 41:
            this.input.LA(2);
            if (this.EvaluatePredicate(new Action(this.synpred6_CssParser_fragment)))
            {
              num = 1;
              break;
            }
            break;
          case 70:
            if (this.EvaluatePredicate(new Action(this.synpred6_CssParser_fragment)))
            {
              num = 1;
              break;
            }
            break;
          case 84:
            this.input.LA(2);
            if (this.EvaluatePredicate(new Action(this.synpred6_CssParser_fragment)))
            {
              num = 1;
              break;
            }
            break;
        }
        if (num == 1)
        {
          this.PushFollow(CssParser.Follow._selector_namespace_prefix_in_type_selector2047);
          CssParser.selector_namespace_prefix_return namespacePrefixReturn = this.selector_namespace_prefix();
          this.PopFollow();
          if (this.state.failed)
            return typeSelectorReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(namespacePrefixReturn.Tree);
        }
        this.PushFollow(CssParser.Follow._element_name_in_type_selector2051);
        CssParser.element_name_return elementNameReturn = this.element_name();
        this.PopFollow();
        if (this.state.failed)
          return typeSelectorReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream2.Add(elementNameReturn.Tree);
        if (this.state.backtracking == 0)
        {
          typeSelectorReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", typeSelectorReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(184, "TYPE_SELECTOR"), oldRoot);
          if (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          this.adaptor.AddChild(obj1, obj2);
          typeSelectorReturn.Tree = obj1;
        }
        typeSelectorReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          typeSelectorReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(typeSelectorReturn.Tree, (IToken) typeSelectorReturn.Start, (IToken) typeSelectorReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        typeSelectorReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) typeSelectorReturn.Start, this.input.LT(-1), ex);
      }
      return typeSelectorReturn;
    }

    [GrammarRule("selector_namespace_prefix")]
    private CssParser.selector_namespace_prefix_return selector_namespace_prefix()
    {
      CssParser.selector_namespace_prefix_return namespacePrefixReturn = new CssParser.selector_namespace_prefix_return(this);
      namespacePrefixReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token PIPE");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule element_name");
      try
      {
        int num = 2;
        switch (this.input.LA(1))
        {
          case 41:
          case 84:
            num = 1;
            break;
        }
        if (num == 1)
        {
          this.PushFollow(CssParser.Follow._element_name_in_selector_namespace_prefix2085);
          CssParser.element_name_return elementNameReturn = this.element_name();
          this.PopFollow();
          if (this.state.failed)
            return namespacePrefixReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(elementNameReturn.Tree);
        }
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 70, CssParser.Follow._PIPE_in_selector_namespace_prefix2088);
        if (this.state.failed)
          return namespacePrefixReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          namespacePrefixReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", namespacePrefixReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(176, "SELECTOR_NAMESPACE_PREFIX"), oldRoot);
          if (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj1, obj2);
          namespacePrefixReturn.Tree = obj1;
        }
        namespacePrefixReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          namespacePrefixReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(namespacePrefixReturn.Tree, (IToken) namespacePrefixReturn.Start, (IToken) namespacePrefixReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        namespacePrefixReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) namespacePrefixReturn.Start, this.input.LT(-1), ex);
      }
      return namespacePrefixReturn;
    }

    [GrammarRule("element_name")]
    private CssParser.element_name_return element_name()
    {
      CssParser.element_name_return elementNameReturn = new CssParser.element_name_return(this);
      elementNameReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token STAR");
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 41:
            num = 1;
            break;
          case 84:
            num = 2;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 38, 0, (IIntStream) this.input);
            this.state.failed = true;
            return elementNameReturn;
        }
        switch (num)
        {
          case 1:
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_element_name2117);
            if (this.state.failed)
              return elementNameReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream1.Add((object) el1);
            if (this.state.backtracking == 0)
            {
              elementNameReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", elementNameReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create((int) sbyte.MaxValue, "ELEMENT_NAME"), oldRoot);
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
              this.adaptor.AddChild(obj1, obj2);
              elementNameReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 84, CssParser.Follow._STAR_in_element_name2137);
            if (this.state.failed)
              return elementNameReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el2);
            if (this.state.backtracking == 0)
            {
              elementNameReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", elementNameReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create((int) sbyte.MaxValue, "ELEMENT_NAME"), oldRoot);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream2.NextNode());
              this.adaptor.AddChild(obj1, obj3);
              elementNameReturn.Tree = obj1;
              break;
            }
            break;
        }
        elementNameReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          elementNameReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(elementNameReturn.Tree, (IToken) elementNameReturn.Start, (IToken) elementNameReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        elementNameReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) elementNameReturn.Start, this.input.LT(-1), ex);
      }
      return elementNameReturn;
    }

    [GrammarRule("universal")]
    private CssParser.universal_return universal()
    {
      CssParser.universal_return universalReturn = new CssParser.universal_return(this);
      universalReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token STAR");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule selector_namespace_prefix");
      try
      {
        int num1 = 2;
        int num2 = this.input.LA(1);
        if (num2 == 41 && this.EvaluatePredicate(new Action(this.synpred7_CssParser_fragment)))
          num1 = 1;
        else if (num2 == 84)
        {
          this.input.LA(2);
          if (this.EvaluatePredicate(new Action(this.synpred7_CssParser_fragment)))
            num1 = 1;
        }
        else if (num2 == 70 && this.EvaluatePredicate(new Action(this.synpred7_CssParser_fragment)))
          num1 = 1;
        if (num1 == 1)
        {
          this.PushFollow(CssParser.Follow._selector_namespace_prefix_in_universal2174);
          CssParser.selector_namespace_prefix_return namespacePrefixReturn = this.selector_namespace_prefix();
          this.PopFollow();
          if (this.state.failed)
            return universalReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(namespacePrefixReturn.Tree);
        }
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 84, CssParser.Follow._STAR_in_universal2178);
        if (this.state.failed)
          return universalReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          universalReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", universalReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(186, "UNIVERSAL"), oldRoot);
          if (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj1, obj2);
          universalReturn.Tree = obj1;
        }
        universalReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          universalReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(universalReturn.Tree, (IToken) universalReturn.Start, (IToken) universalReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        universalReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) universalReturn.Start, this.input.LT(-1), ex);
      }
      return universalReturn;
    }

    [GrammarRule("class")]
    private CssParser.class_return @class()
    {
      CssParser.class_return classReturn = new CssParser.class_return(this);
      classReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token CLASS_IDENT");
      try
      {
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 14, CssParser.Follow._CLASS_IDENT_in_class2207);
        if (this.state.failed)
          return classReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          classReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", classReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(117, "CLASSIDENTIFIER"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          classReturn.Tree = obj1;
        }
        classReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          classReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(classReturn.Tree, (IToken) classReturn.Start, (IToken) classReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        classReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) classReturn.Start, this.input.LT(-1), ex);
      }
      return classReturn;
    }

    [GrammarRule("attrib")]
    private CssParser.attrib_return attrib()
    {
      CssParser.attrib_return attribReturn = new CssParser.attrib_return(this);
      attribReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      CommonToken commonToken1 = (CommonToken) null;
      CommonToken commonToken2 = (CommonToken) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token SQUARE_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token PREFIXMATCH");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token SUFFIXMATCH");
      RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(this.adaptor, "token SUBSTRINGMATCH");
      RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(this.adaptor, "token EQUALS");
      RewriteRuleTokenStream rewriteRuleTokenStream7 = new RewriteRuleTokenStream(this.adaptor, "token INCLUDES");
      RewriteRuleTokenStream rewriteRuleTokenStream8 = new RewriteRuleTokenStream(this.adaptor, "token DASHMATCH");
      RewriteRuleTokenStream rewriteRuleTokenStream9 = new RewriteRuleTokenStream(this.adaptor, "token STRING");
      RewriteRuleTokenStream rewriteRuleTokenStream10 = new RewriteRuleTokenStream(this.adaptor, "token SQUARE_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule selector_namespace_prefix");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 82, CssParser.Follow._SQUARE_BEGIN_in_attrib2246);
        if (this.state.failed)
          return attribReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        int num1 = 2;
        switch (this.input.LA(1))
        {
          case 41:
            if (this.input.LA(2) == 70)
            {
              num1 = 1;
              break;
            }
            break;
          case 70:
          case 84:
            num1 = 1;
            break;
        }
        if (num1 == 1)
        {
          this.PushFollow(CssParser.Follow._selector_namespace_prefix_in_attrib2257);
          CssParser.selector_namespace_prefix_return namespacePrefixReturn = this.selector_namespace_prefix();
          this.PopFollow();
          if (this.state.failed)
            return attribReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(namespacePrefixReturn.Tree);
        }
        CommonToken commonToken3 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_attrib2262);
        if (this.state.failed)
          return attribReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) commonToken3);
        int num2 = 2;
        switch (this.input.LA(1))
        {
          case 21:
          case 28:
          case 45:
          case 72:
          case 86:
          case 87:
            num2 = 1;
            break;
        }
        if (num2 == 1)
        {
          int num3;
          switch (this.input.LA(1))
          {
            case 21:
              num3 = 6;
              break;
            case 28:
              num3 = 4;
              break;
            case 45:
              num3 = 5;
              break;
            case 72:
              num3 = 1;
              break;
            case 86:
              num3 = 3;
              break;
            case 87:
              num3 = 2;
              break;
            default:
              if (this.state.backtracking <= 0)
                throw new NoViableAltException("", 41, 0, (IIntStream) this.input);
              this.state.failed = true;
              return attribReturn;
          }
          switch (num3)
          {
            case 1:
              commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 72, CssParser.Follow._PREFIXMATCH_in_attrib2289);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream3.Add((object) commonToken1);
                break;
              }
              break;
            case 2:
              commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 87, CssParser.Follow._SUFFIXMATCH_in_attrib2293);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream4.Add((object) commonToken1);
                break;
              }
              break;
            case 3:
              commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 86, CssParser.Follow._SUBSTRINGMATCH_in_attrib2297);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream5.Add((object) commonToken1);
                break;
              }
              break;
            case 4:
              commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 28, CssParser.Follow._EQUALS_in_attrib2301);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream6.Add((object) commonToken1);
                break;
              }
              break;
            case 5:
              commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 45, CssParser.Follow._INCLUDES_in_attrib2305);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream7.Add((object) commonToken1);
                break;
              }
              break;
            case 6:
              commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 21, CssParser.Follow._DASHMATCH_in_attrib2309);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream8.Add((object) commonToken1);
                break;
              }
              break;
          }
          int num4;
          switch (this.input.LA(1))
          {
            case 41:
              num4 = 1;
              break;
            case 85:
              num4 = 2;
              break;
            default:
              if (this.state.backtracking <= 0)
                throw new NoViableAltException("", 42, 0, (IIntStream) this.input);
              this.state.failed = true;
              return attribReturn;
          }
          switch (num4)
          {
            case 1:
              commonToken2 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_attrib2327);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream2.Add((object) commonToken2);
                break;
              }
              break;
            case 2:
              CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 85, CssParser.Follow._STRING_in_attrib2329);
              if (this.state.failed)
                return attribReturn;
              if (this.state.backtracking == 0)
              {
                rewriteRuleTokenStream9.Add((object) el2);
                break;
              }
              break;
          }
        }
        CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 83, CssParser.Follow._SQUARE_END_in_attrib2347);
        if (this.state.failed)
          return attribReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream10.Add((object) el3);
        if (this.state.backtracking == 0)
        {
          attribReturn.Tree = obj1;
          RewriteRuleTokenStream rewriteRuleTokenStream11 = new RewriteRuleTokenStream(this.adaptor, "token attributeName", (object) commonToken3);
          RewriteRuleTokenStream rewriteRuleTokenStream12 = new RewriteRuleTokenStream(this.adaptor, "token attributeOperator", (object) commonToken1);
          RewriteRuleTokenStream rewriteRuleTokenStream13 = new RewriteRuleTokenStream(this.adaptor, "token attribvalue", (object) commonToken2);
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", attribReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(111, "ATTRIBIDENTIFIER"), oldRoot1);
          if (ruleSubtreeStream1.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          ruleSubtreeStream1.Reset();
          object oldRoot2 = this.adaptor.Nil();
          object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(112, "ATTRIBNAME"), oldRoot2);
          this.adaptor.AddChild(obj3, rewriteRuleTokenStream11.NextNode());
          this.adaptor.AddChild(obj2, obj3);
          if (rewriteRuleTokenStream12.HasNext || rewriteRuleTokenStream13.HasNext || rewriteRuleTokenStream9.HasNext)
          {
            object oldRoot3 = this.adaptor.Nil();
            object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(114, "ATTRIBOPERATORVALUE"), oldRoot3);
            object oldRoot4 = this.adaptor.Nil();
            object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(113, "ATTRIBOPERATOR"), oldRoot4);
            this.adaptor.AddChild(obj5, rewriteRuleTokenStream12.NextNode());
            this.adaptor.AddChild(obj4, obj5);
            object oldRoot5 = this.adaptor.Nil();
            object obj6 = this.adaptor.BecomeRoot(this.adaptor.Create(115, "ATTRIBVALUE"), oldRoot5);
            if (rewriteRuleTokenStream13.HasNext)
              this.adaptor.AddChild(obj6, rewriteRuleTokenStream13.NextNode());
            rewriteRuleTokenStream13.Reset();
            if (rewriteRuleTokenStream9.HasNext)
            {
              object oldRoot6 = this.adaptor.Nil();
              object obj7 = this.adaptor.BecomeRoot(this.adaptor.Create(179, "STRINGBASEDVALUE"), oldRoot6);
              this.adaptor.AddChild(obj7, rewriteRuleTokenStream9.NextNode());
              this.adaptor.AddChild(obj6, obj7);
            }
            rewriteRuleTokenStream9.Reset();
            this.adaptor.AddChild(obj4, obj6);
            this.adaptor.AddChild(obj2, obj4);
          }
          rewriteRuleTokenStream12.Reset();
          rewriteRuleTokenStream13.Reset();
          rewriteRuleTokenStream9.Reset();
          this.adaptor.AddChild(obj1, obj2);
          attribReturn.Tree = obj1;
        }
        attribReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          attribReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(attribReturn.Tree, (IToken) attribReturn.Start, (IToken) attribReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        attribReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) attribReturn.Start, this.input.LT(-1), ex);
      }
      return attribReturn;
    }

    [GrammarRule("pseudo")]
    private CssParser.pseudo_return pseudo()
    {
      CssParser.pseudo_return pseudoReturn = new CssParser.pseudo_return(this);
      pseudoReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      CommonToken commonToken1 = (CommonToken) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token COLON");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule functional_pseudo");
      try
      {
        if (this.input.LA(1) == 15)
        {
          int num1;
          switch (this.input.LA(2))
          {
            case 15:
              switch (this.input.LA(3))
              {
                case 33:
                case 55:
                case 91:
                  num1 = 2;
                  break;
                case 41:
                  int num2 = this.input.LA(4);
                  if (num2 == 12)
                  {
                    num1 = 2;
                    break;
                  }
                  if (num2 == -1 || num2 == 7 || num2 >= 13 && num2 <= 16 || num2 == 18 || num2 == 35 || num2 == 38 || num2 == 41 || num2 >= 70 && num2 <= 71 || num2 == 76 || num2 == 82 || num2 == 84 || num2 == 89 || num2 == 105)
                  {
                    num1 = 1;
                    break;
                  }
                  if (this.state.backtracking <= 0)
                    throw new NoViableAltException("", 46, 3, (IIntStream) this.input);
                  this.state.failed = true;
                  return pseudoReturn;
                default:
                  if (this.state.backtracking <= 0)
                    throw new NoViableAltException("", 46, 2, (IIntStream) this.input);
                  this.state.failed = true;
                  return pseudoReturn;
              }
              break;
            case 33:
            case 55:
            case 91:
              num1 = 2;
              break;
            case 41:
              int num3 = this.input.LA(3);
              if (num3 == 12)
              {
                num1 = 2;
                break;
              }
              if (num3 == -1 || num3 == 7 || num3 >= 13 && num3 <= 16 || num3 == 18 || num3 == 35 || num3 == 38 || num3 == 41 || num3 >= 70 && num3 <= 71 || num3 == 76 || num3 == 82 || num3 == 84 || num3 == 89 || num3 == 105)
              {
                num1 = 1;
                break;
              }
              if (this.state.backtracking <= 0)
                throw new NoViableAltException("", 46, 3, (IIntStream) this.input);
              this.state.failed = true;
              return pseudoReturn;
            default:
              if (this.state.backtracking <= 0)
                throw new NoViableAltException("", 46, 1, (IIntStream) this.input);
              this.state.failed = true;
              return pseudoReturn;
          }
          switch (num1)
          {
            case 1:
              CommonToken commonToken2 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_pseudo2420);
              if (this.state.failed)
                return pseudoReturn;
              if (this.state.backtracking == 0)
                rewriteRuleTokenStream1.Add((object) commonToken2);
              int num4 = 2;
              if (this.input.LA(1) == 15)
                num4 = 1;
              if (num4 == 1)
              {
                commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_pseudo2424);
                if (this.state.failed)
                  return pseudoReturn;
                if (this.state.backtracking == 0)
                  rewriteRuleTokenStream1.Add((object) commonToken1);
              }
              CommonToken commonToken3 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_pseudo2429);
              if (this.state.failed)
                return pseudoReturn;
              if (this.state.backtracking == 0)
                rewriteRuleTokenStream2.Add((object) commonToken3);
              if (this.state.backtracking == 0)
              {
                pseudoReturn.Tree = obj1;
                RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token c1", (object) commonToken2);
                RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token c2", (object) commonToken1);
                RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(this.adaptor, "token pseudoName", (object) commonToken3);
                RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", pseudoReturn?.Tree);
                obj1 = this.adaptor.Nil();
                object oldRoot1 = this.adaptor.Nil();
                object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(165, "PSEUDOIDENTIFIER"), oldRoot1);
                object oldRoot2 = this.adaptor.Nil();
                object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(118, "COLONS"), oldRoot2);
                this.adaptor.AddChild(obj3, rewriteRuleTokenStream3.NextNode());
                if (rewriteRuleTokenStream4.HasNext)
                  this.adaptor.AddChild(obj3, rewriteRuleTokenStream4.NextNode());
                rewriteRuleTokenStream4.Reset();
                this.adaptor.AddChild(obj2, obj3);
                object oldRoot3 = this.adaptor.Nil();
                object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(166, "PSEUDONAME"), oldRoot3);
                this.adaptor.AddChild(obj4, rewriteRuleTokenStream5.NextNode());
                this.adaptor.AddChild(obj2, obj4);
                this.adaptor.AddChild(obj1, obj2);
                pseudoReturn.Tree = obj1;
                break;
              }
              break;
            case 2:
              CommonToken commonToken4 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_pseudo2467);
              if (this.state.failed)
                return pseudoReturn;
              if (this.state.backtracking == 0)
                rewriteRuleTokenStream1.Add((object) commonToken4);
              int num5 = 2;
              if (this.input.LA(1) == 15)
                num5 = 1;
              if (num5 == 1)
              {
                commonToken1 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_pseudo2471);
                if (this.state.failed)
                  return pseudoReturn;
                if (this.state.backtracking == 0)
                  rewriteRuleTokenStream1.Add((object) commonToken1);
              }
              this.PushFollow(CssParser.Follow._functional_pseudo_in_pseudo2474);
              CssParser.functional_pseudo_return functionalPseudoReturn = this.functional_pseudo();
              this.PopFollow();
              if (this.state.failed)
                return pseudoReturn;
              if (this.state.backtracking == 0)
                ruleSubtreeStream1.Add(functionalPseudoReturn.Tree);
              if (this.state.backtracking == 0)
              {
                pseudoReturn.Tree = obj1;
                RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(this.adaptor, "token c1", (object) commonToken4);
                RewriteRuleTokenStream rewriteRuleTokenStream7 = new RewriteRuleTokenStream(this.adaptor, "token c2", (object) commonToken1);
                RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", pseudoReturn?.Tree);
                obj1 = this.adaptor.Nil();
                object oldRoot4 = this.adaptor.Nil();
                object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(165, "PSEUDOIDENTIFIER"), oldRoot4);
                object oldRoot5 = this.adaptor.Nil();
                object obj6 = this.adaptor.BecomeRoot(this.adaptor.Create(118, "COLONS"), oldRoot5);
                this.adaptor.AddChild(obj6, rewriteRuleTokenStream6.NextNode());
                if (rewriteRuleTokenStream7.HasNext)
                  this.adaptor.AddChild(obj6, rewriteRuleTokenStream7.NextNode());
                rewriteRuleTokenStream7.Reset();
                this.adaptor.AddChild(obj5, obj6);
                this.adaptor.AddChild(obj5, ruleSubtreeStream1.NextTree());
                this.adaptor.AddChild(obj1, obj5);
                pseudoReturn.Tree = obj1;
                break;
              }
              break;
          }
          pseudoReturn.Stop = (CommonToken) this.input.LT(-1);
          if (this.state.backtracking == 0)
          {
            pseudoReturn.Tree = this.adaptor.RulePostProcessing(obj1);
            this.adaptor.SetTokenBoundaries(pseudoReturn.Tree, (IToken) pseudoReturn.Start, (IToken) pseudoReturn.Stop);
          }
        }
        else
        {
          if (this.state.backtracking <= 0)
            throw new NoViableAltException("", 46, 0, (IIntStream) this.input);
          this.state.failed = true;
          return pseudoReturn;
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        pseudoReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) pseudoReturn.Start, this.input.LT(-1), ex);
      }
      return pseudoReturn;
    }

    [GrammarRule("functional_pseudo")]
    private CssParser.functional_pseudo_return functional_pseudo()
    {
      CssParser.functional_pseudo_return functionalPseudoReturn = new CssParser.functional_pseudo_return(this);
      functionalPseudoReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token CIRCLE_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule beginfunc");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule selectorexpression");
      try
      {
        this.PushFollow(CssParser.Follow._beginfunc_in_functional_pseudo2515);
        CssParser.beginfunc_return beginfuncReturn = this.beginfunc();
        this.PopFollow();
        if (this.state.failed)
          return functionalPseudoReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(beginfuncReturn.Tree);
        this.PushFollow(CssParser.Follow._selectorexpression_in_functional_pseudo2517);
        CssParser.selectorexpression_return selectorexpressionReturn = this.selectorexpression();
        this.PopFollow();
        if (this.state.failed)
          return functionalPseudoReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream2.Add(selectorexpressionReturn.Tree);
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 13, CssParser.Follow._CIRCLE_END_in_functional_pseudo2519);
        if (this.state.failed)
          return functionalPseudoReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          functionalPseudoReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", functionalPseudoReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(129, "FUNCTIONAL_PSEUDO"), oldRoot1);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          object oldRoot2 = this.adaptor.Nil();
          object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(175, "SELECTOR_EXPRESSION"), oldRoot2);
          this.adaptor.AddChild(obj3, ruleSubtreeStream2.NextTree());
          this.adaptor.AddChild(obj2, obj3);
          this.adaptor.AddChild(obj1, obj2);
          functionalPseudoReturn.Tree = obj1;
        }
        functionalPseudoReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          functionalPseudoReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(functionalPseudoReturn.Tree, (IToken) functionalPseudoReturn.Start, (IToken) functionalPseudoReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        functionalPseudoReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) functionalPseudoReturn.Start, this.input.LT(-1), ex);
      }
      return functionalPseudoReturn;
    }

    [GrammarRule("selectorexpression")]
    private CssParser.selectorexpression_return selectorexpression()
    {
      CssParser.selectorexpression_return selectorexpressionReturn = new CssParser.selectorexpression_return(this);
      selectorexpressionReturn.Start = (CommonToken) this.input.LT(1);
      try
      {
        object obj = this.adaptor.Nil();
        int num1 = 0;
        while (true)
        {
          int num2 = 2;
          switch (this.input.LA(1))
          {
            case 23:
            case 41:
            case 53:
            case 64:
            case 71:
            case 85:
            case 168:
              num2 = 1;
              break;
          }
          if (num2 == 1)
          {
            CommonToken payload = (CommonToken) this.input.LT(1);
            if (this.input.LA(1) == 23 || this.input.LA(1) == 41 || this.input.LA(1) == 53 || this.input.LA(1) == 64 || this.input.LA(1) == 71 || this.input.LA(1) == 85 || this.input.LA(1) == 168)
            {
              this.input.Consume();
              if (this.state.backtracking == 0)
                this.adaptor.AddChild(obj, this.adaptor.Create((IToken) payload));
              this.state.errorRecovery = false;
              this.state.failed = false;
              ++num1;
            }
            else
              break;
          }
          else
            goto label_12;
        }
        if (this.state.backtracking <= 0)
          throw new MismatchedSetException((BitSet) null, (IIntStream) this.input);
        this.state.failed = true;
        return selectorexpressionReturn;
label_12:
        if (num1 < 1)
        {
          if (this.state.backtracking <= 0)
            throw new EarlyExitException(47, (IIntStream) this.input);
          this.state.failed = true;
          return selectorexpressionReturn;
        }
        selectorexpressionReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          selectorexpressionReturn.Tree = this.adaptor.RulePostProcessing(obj);
          this.adaptor.SetTokenBoundaries(selectorexpressionReturn.Tree, (IToken) selectorexpressionReturn.Start, (IToken) selectorexpressionReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        selectorexpressionReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) selectorexpressionReturn.Start, this.input.LT(-1), ex);
      }
      return selectorexpressionReturn;
    }

    [GrammarRule("negation")]
    private CssParser.negation_return negation()
    {
      CssParser.negation_return negationReturn = new CssParser.negation_return(this);
      negationReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token COLON");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token NOT");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token CIRCLE_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token CIRCLE_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule negation_arg");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_negation2594);
        if (this.state.failed)
          return negationReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 63, CssParser.Follow._NOT_in_negation2596);
        if (this.state.failed)
          return negationReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el2);
        CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 12, CssParser.Follow._CIRCLE_BEGIN_in_negation2598);
        if (this.state.failed)
          return negationReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream3.Add((object) el3);
        this.PushFollow(CssParser.Follow._negation_arg_in_negation2601);
        CssParser.negation_arg_return negationArgReturn = this.negation_arg();
        this.PopFollow();
        if (this.state.failed)
          return negationReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(negationArgReturn.Tree);
        CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 13, CssParser.Follow._CIRCLE_END_in_negation2603);
        if (this.state.failed)
          return negationReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream4.Add((object) el4);
        if (this.state.backtracking == 0)
        {
          negationReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", negationReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(157, "NEGATIONIDENTIFIER"), oldRoot1);
          object oldRoot2 = this.adaptor.Nil();
          object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(158, "NEGATION_ARG"), oldRoot2);
          this.adaptor.AddChild(obj3, ruleSubtreeStream1.NextTree());
          this.adaptor.AddChild(obj2, obj3);
          this.adaptor.AddChild(obj1, obj2);
          negationReturn.Tree = obj1;
        }
        negationReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          negationReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(negationReturn.Tree, (IToken) negationReturn.Start, (IToken) negationReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        negationReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) negationReturn.Start, this.input.LT(-1), ex);
      }
      return negationReturn;
    }

    [GrammarRule("negation_arg")]
    private CssParser.negation_arg_return negation_arg()
    {
      CssParser.negation_arg_return negationArgReturn = new CssParser.negation_arg_return(this);
      negationArgReturn.Start = (CommonToken) this.input.LT(1);
      object obj = (object) null;
      try
      {
        int num;
        try
        {
          num = this.dfa48.Predict((IIntStream) this.input);
        }
        catch (NoViableAltException ex)
        {
          throw;
        }
        switch (num)
        {
          case 1:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._universal_in_negation_arg2640);
            CssParser.universal_return universalReturn = this.universal();
            this.PopFollow();
            if (this.state.failed)
              return negationArgReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, universalReturn.Tree);
              break;
            }
            break;
          case 2:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._type_selector_in_negation_arg2643);
            CssParser.type_selector_return typeSelectorReturn = this.type_selector();
            this.PopFollow();
            if (this.state.failed)
              return negationArgReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, typeSelectorReturn.Tree);
              break;
            }
            break;
          case 3:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._hash_in_negation_arg2645);
            CssParser.hash_return hashReturn = this.hash();
            this.PopFollow();
            if (this.state.failed)
              return negationArgReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, hashReturn.Tree);
              break;
            }
            break;
          case 4:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._class_in_negation_arg2647);
            CssParser.class_return classReturn = this.@class();
            this.PopFollow();
            if (this.state.failed)
              return negationArgReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, classReturn.Tree);
              break;
            }
            break;
          case 5:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._attrib_in_negation_arg2649);
            CssParser.attrib_return attribReturn = this.attrib();
            this.PopFollow();
            if (this.state.failed)
              return negationArgReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, attribReturn.Tree);
              break;
            }
            break;
          case 6:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._pseudo_in_negation_arg2651);
            CssParser.pseudo_return pseudoReturn = this.pseudo();
            this.PopFollow();
            if (this.state.failed)
              return negationArgReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, pseudoReturn.Tree);
              break;
            }
            break;
        }
        negationArgReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          negationArgReturn.Tree = this.adaptor.RulePostProcessing(obj);
          this.adaptor.SetTokenBoundaries(negationArgReturn.Tree, (IToken) negationArgReturn.Start, (IToken) negationArgReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        negationArgReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) negationArgReturn.Start, this.input.LT(-1), ex);
      }
      return negationArgReturn;
    }

    [GrammarRule("atname")]
    private CssParser.atname_return atname()
    {
      CssParser.atname_return atnameReturn = new CssParser.atname_return(this);
      atnameReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token AT_NAME");
      try
      {
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 7, CssParser.Follow._AT_NAME_in_atname2666);
        if (this.state.failed)
          return atnameReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          atnameReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", atnameReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(110, "ATIDENTIFIER"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          atnameReturn.Tree = obj1;
        }
        atnameReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          atnameReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(atnameReturn.Tree, (IToken) atnameReturn.Start, (IToken) atnameReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        atnameReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) atnameReturn.Start, this.input.LT(-1), ex);
      }
      return atnameReturn;
    }

    [GrammarRule("declaration")]
    private CssParser.declaration_return declaration()
    {
      CssParser.declaration_return declarationReturn = new CssParser.declaration_return(this);
      declarationReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token IMPORTANT_COMMENTS");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token COLON");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule property");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule expr");
      RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule prio");
      try
      {
        while (true)
        {
          CommonToken el;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 42)
              num = 1;
            if (num == 1)
            {
              el = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_declaration2698);
              if (this.state.failed)
                return declarationReturn;
            }
            else
              goto label_8;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream1.Add((object) el);
        }
label_8:
        this.PushFollow(CssParser.Follow._property_in_declaration2701);
        CssParser.property_return propertyReturn = this.property();
        this.PopFollow();
        if (this.state.failed)
          return declarationReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(propertyReturn.Tree);
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 15, CssParser.Follow._COLON_in_declaration2703);
        if (this.state.failed)
          return declarationReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream2.Add((object) el1);
        this.PushFollow(CssParser.Follow._expr_in_declaration2705);
        CssParser.expr_return exprReturn = this.expr();
        this.PopFollow();
        if (this.state.failed)
          return declarationReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream2.Add(exprReturn.Tree);
        int num1 = 2;
        if (this.input.LA(1) == 43)
          num1 = 1;
        if (num1 == 1)
        {
          this.PushFollow(CssParser.Follow._prio_in_declaration2707);
          CssParser.prio_return prioReturn = this.prio();
          this.PopFollow();
          if (this.state.failed)
            return declarationReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream3.Add(prioReturn.Tree);
        }
        if (this.state.backtracking == 0)
        {
          declarationReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", declarationReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(122, "DECLARATION"), oldRoot);
          while (rewriteRuleTokenStream1.HasNext)
            this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
          rewriteRuleTokenStream1.Reset();
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          if (ruleSubtreeStream3.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream3.NextTree());
          ruleSubtreeStream3.Reset();
          this.adaptor.AddChild(obj1, obj2);
          declarationReturn.Tree = obj1;
        }
        declarationReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          declarationReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(declarationReturn.Tree, (IToken) declarationReturn.Start, (IToken) declarationReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        declarationReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) declarationReturn.Start, this.input.LT(-1), ex);
      }
      return declarationReturn;
    }

    [GrammarRule("stringoruri")]
    private CssParser.stringoruri_return stringoruri()
    {
      CssParser.stringoruri_return stringoruriReturn = new CssParser.stringoruri_return(this);
      stringoruriReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token STRING");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token URI");
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 85:
            num = 1;
            break;
          case 99:
            num = 2;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 51, 0, (IIntStream) this.input);
            this.state.failed = true;
            return stringoruriReturn;
        }
        switch (num)
        {
          case 1:
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 85, CssParser.Follow._STRING_in_stringoruri2747);
            if (this.state.failed)
              return stringoruriReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream1.Add((object) el1);
            if (this.state.backtracking == 0)
            {
              stringoruriReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", stringoruriReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(179, "STRINGBASEDVALUE"), oldRoot);
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
              this.adaptor.AddChild(obj1, obj2);
              stringoruriReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 99, CssParser.Follow._URI_in_stringoruri2767);
            if (this.state.failed)
              return stringoruriReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el2);
            if (this.state.backtracking == 0)
            {
              stringoruriReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", stringoruriReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(187, "URIBASEDVALUE"), oldRoot);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream2.NextNode());
              this.adaptor.AddChild(obj1, obj3);
              stringoruriReturn.Tree = obj1;
              break;
            }
            break;
        }
        stringoruriReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          stringoruriReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(stringoruriReturn.Tree, (IToken) stringoruriReturn.Start, (IToken) stringoruriReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        stringoruriReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) stringoruriReturn.Start, this.input.LT(-1), ex);
      }
      return stringoruriReturn;
    }

    [GrammarRule("styleSheetrules")]
    private CssParser.styleSheetrules_return styleSheetrules()
    {
      CssParser.styleSheetrules_return sheetrulesReturn = new CssParser.styleSheetrules_return(this);
      sheetrulesReturn.Start = (CommonToken) this.input.LT(1);
      object obj = (object) null;
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 7:
          case 14:
          case 15:
          case 38:
          case 41:
          case 70:
          case 76:
          case 82:
          case 84:
            num = 1;
            break;
          case 24:
            num = 5;
            break;
          case 47:
            num = 4;
            break;
          case 52:
            num = 2;
            break;
          case 68:
            num = 3;
            break;
          case 104:
            num = 6;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 52, 0, (IIntStream) this.input);
            this.state.failed = true;
            return sheetrulesReturn;
        }
        switch (num)
        {
          case 1:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._ruleset_in_styleSheetrules2796);
            CssParser.ruleset_return rulesetReturn = this.ruleset();
            this.PopFollow();
            if (this.state.failed)
              return sheetrulesReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, rulesetReturn.Tree);
              break;
            }
            break;
          case 2:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._media_in_styleSheetrules2798);
            CssParser.media_return mediaReturn = this.media();
            this.PopFollow();
            if (this.state.failed)
              return sheetrulesReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, mediaReturn.Tree);
              break;
            }
            break;
          case 3:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._page_in_styleSheetrules2800);
            CssParser.page_return pageReturn = this.page();
            this.PopFollow();
            if (this.state.failed)
              return sheetrulesReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, pageReturn.Tree);
              break;
            }
            break;
          case 4:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._keyframes_in_styleSheetrules2802);
            CssParser.keyframes_return keyframesReturn = this.keyframes();
            this.PopFollow();
            if (this.state.failed)
              return sheetrulesReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, keyframesReturn.Tree);
              break;
            }
            break;
          case 5:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._document_in_styleSheetrules2804);
            CssParser.document_return documentReturn = this.document();
            this.PopFollow();
            if (this.state.failed)
              return sheetrulesReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, documentReturn.Tree);
              break;
            }
            break;
          case 6:
            obj = this.adaptor.Nil();
            this.PushFollow(CssParser.Follow._wg_dpi_in_styleSheetrules2806);
            CssParser.wg_dpi_return wgDpiReturn = this.wg_dpi();
            this.PopFollow();
            if (this.state.failed)
              return sheetrulesReturn;
            if (this.state.backtracking == 0)
            {
              this.adaptor.AddChild(obj, wgDpiReturn.Tree);
              break;
            }
            break;
        }
        sheetrulesReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          sheetrulesReturn.Tree = this.adaptor.RulePostProcessing(obj);
          this.adaptor.SetTokenBoundaries(sheetrulesReturn.Tree, (IToken) sheetrulesReturn.Start, (IToken) sheetrulesReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        sheetrulesReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) sheetrulesReturn.Start, this.input.LT(-1), ex);
      }
      return sheetrulesReturn;
    }

    [GrammarRule("prio")]
    private CssParser.prio_return prio()
    {
      CssParser.prio_return prioReturn = new CssParser.prio_return(this);
      prioReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token IMPORTANT_SYM");
      try
      {
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 43, CssParser.Follow._IMPORTANT_SYM_in_prio2826);
        if (this.state.failed)
          return prioReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          prioReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", prioReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(139, "IMPORTANT"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          prioReturn.Tree = obj1;
        }
        prioReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          prioReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(prioReturn.Tree, (IToken) prioReturn.Start, (IToken) prioReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        prioReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) prioReturn.Start, this.input.LT(-1), ex);
      }
      return prioReturn;
    }

    [GrammarRule("expr")]
    private CssParser.expr_return expr()
    {
      CssParser.expr_return exprReturn = new CssParser.expr_return(this);
      exprReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token IMPORTANT_COMMENTS");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule term");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule termwithoperator");
      try
      {
        while (true)
        {
          CommonToken el;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 42)
              num = 1;
            if (num == 1)
            {
              el = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_expr2856);
              if (this.state.failed)
                return exprReturn;
            }
            else
              goto label_8;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream.Add((object) el);
        }
label_8:
        this.PushFollow(CssParser.Follow._term_in_expr2859);
        CssParser.term_return termReturn = this.term();
        this.PopFollow();
        if (this.state.failed)
          return exprReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(termReturn.Tree);
        while (true)
        {
          CssParser.termwithoperator_return termwithoperatorReturn;
          do
          {
            int num;
            try
            {
              num = this.dfa54.Predict((IIntStream) this.input);
            }
            catch (NoViableAltException ex)
            {
              throw;
            }
            if (num == 1)
            {
              this.PushFollow(CssParser.Follow._termwithoperator_in_expr2862);
              termwithoperatorReturn = this.termwithoperator();
              this.PopFollow();
              if (this.state.failed)
                return exprReturn;
            }
            else
              goto label_20;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream2.Add(termwithoperatorReturn.Tree);
        }
label_20:
        if (this.state.backtracking == 0)
        {
          exprReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", exprReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(128, "EXPR"), oldRoot1);
          while (rewriteRuleTokenStream.HasNext)
            this.adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
          rewriteRuleTokenStream.Reset();
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot2 = this.adaptor.Nil();
            object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(183, "TERMWITHOPERATORS"), oldRoot2);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj3, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj3);
          }
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          exprReturn.Tree = obj1;
        }
        exprReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          exprReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(exprReturn.Tree, (IToken) exprReturn.Start, (IToken) exprReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        exprReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) exprReturn.Start, this.input.LT(-1), ex);
      }
      return exprReturn;
    }

    [GrammarRule("termwithoperator")]
    private CssParser.termwithoperator_return termwithoperator()
    {
      CssParser.termwithoperator_return termwithoperatorReturn = new CssParser.termwithoperator_return(this);
      termwithoperatorReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule operator");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule term");
      try
      {
        int num = 2;
        switch (this.input.LA(1))
        {
          case 16:
          case 28:
          case 31:
          case 84:
            num = 1;
            break;
        }
        if (num == 1)
        {
          this.PushFollow(CssParser.Follow._operator_in_termwithoperator2902);
          CssParser.operator_return operatorReturn = this.@operator();
          this.PopFollow();
          if (this.state.failed)
            return termwithoperatorReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream1.Add(operatorReturn.Tree);
        }
        this.PushFollow(CssParser.Follow._term_in_termwithoperator2905);
        CssParser.term_return termReturn = this.term();
        this.PopFollow();
        if (this.state.failed)
          return termwithoperatorReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream2.Add(termReturn.Tree);
        if (this.state.backtracking == 0)
        {
          termwithoperatorReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termwithoperatorReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(182, "TERMWITHOPERATOR"), oldRoot1);
          if (ruleSubtreeStream1.HasNext)
          {
            object oldRoot2 = this.adaptor.Nil();
            object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(162, "OPERATOR"), oldRoot2);
            this.adaptor.AddChild(obj3, ruleSubtreeStream1.NextTree());
            this.adaptor.AddChild(obj2, obj3);
          }
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          this.adaptor.AddChild(obj1, obj2);
          termwithoperatorReturn.Tree = obj1;
        }
        termwithoperatorReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          termwithoperatorReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(termwithoperatorReturn.Tree, (IToken) termwithoperatorReturn.Start, (IToken) termwithoperatorReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        termwithoperatorReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) termwithoperatorReturn.Start, this.input.LT(-1), ex);
      }
      return termwithoperatorReturn;
    }

    [GrammarRule("term")]
    private CssParser.term_return term()
    {
      CssParser.term_return termReturn = new CssParser.term_return(this);
      termReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      CommonToken commonToken = (CommonToken) null;
      CommonToken oneElement = (CommonToken) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token NUMBER");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token PERCENTAGE");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token LENGTH");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token RELATIVELENGTH");
      RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(this.adaptor, "token ANGLE");
      RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(this.adaptor, "token TIME");
      RewriteRuleTokenStream rewriteRuleTokenStream7 = new RewriteRuleTokenStream(this.adaptor, "token FREQ");
      RewriteRuleTokenStream rewriteRuleTokenStream8 = new RewriteRuleTokenStream(this.adaptor, "token RESOLUTION");
      RewriteRuleTokenStream rewriteRuleTokenStream9 = new RewriteRuleTokenStream(this.adaptor, "token SPEECH");
      RewriteRuleTokenStream rewriteRuleTokenStream10 = new RewriteRuleTokenStream(this.adaptor, "token IMPORTANT_COMMENTS");
      RewriteRuleTokenStream rewriteRuleTokenStream11 = new RewriteRuleTokenStream(this.adaptor, "token URI");
      RewriteRuleTokenStream rewriteRuleTokenStream12 = new RewriteRuleTokenStream(this.adaptor, "token MSIE_EXPRESSION");
      RewriteRuleTokenStream rewriteRuleTokenStream13 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleTokenStream rewriteRuleTokenStream14 = new RewriteRuleTokenStream(this.adaptor, "token STRING");
      RewriteRuleTokenStream rewriteRuleTokenStream15 = new RewriteRuleTokenStream(this.adaptor, "token REPLACEMENTTOKEN");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule unary_operator");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule hash");
      RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule function");
      try
      {
        int num1;
        try
        {
          num1 = this.dfa65.Predict((IIntStream) this.input);
        }
        catch (NoViableAltException ex)
        {
          throw;
        }
        switch (num1)
        {
          case 1:
            int num2 = 2;
            switch (this.input.LA(1))
            {
              case 53:
              case 71:
                num2 = 1;
                break;
            }
            if (num2 == 1)
            {
              this.PushFollow(CssParser.Follow._unary_operator_in_term2943);
              CssParser.unary_operator_return unaryOperatorReturn = this.unary_operator();
              this.PopFollow();
              if (this.state.failed)
                return termReturn;
              if (this.state.backtracking == 0)
                ruleSubtreeStream1.Add(unaryOperatorReturn.Tree);
            }
            int num3;
            switch (this.input.LA(1))
            {
              case 6:
                num3 = 5;
                break;
              case 32:
                num3 = 7;
                break;
              case 49:
                num3 = 3;
                break;
              case 64:
                num3 = 1;
                break;
              case 69:
                num3 = 2;
                break;
              case 75:
                num3 = 4;
                break;
              case 77:
                num3 = 8;
                break;
              case 81:
                num3 = 9;
                break;
              case 90:
                num3 = 6;
                break;
              default:
                if (this.state.backtracking <= 0)
                  throw new NoViableAltException("", 57, 0, (IIntStream) this.input);
                this.state.failed = true;
                return termReturn;
            }
            switch (num3)
            {
              case 1:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 64, CssParser.Follow._NUMBER_in_term2951);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream1.Add((object) commonToken);
                  break;
                }
                break;
              case 2:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 69, CssParser.Follow._PERCENTAGE_in_term2959);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream2.Add((object) commonToken);
                  break;
                }
                break;
              case 3:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 49, CssParser.Follow._LENGTH_in_term2967);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream3.Add((object) commonToken);
                  break;
                }
                break;
              case 4:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 75, CssParser.Follow._RELATIVELENGTH_in_term2975);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream4.Add((object) commonToken);
                  break;
                }
                break;
              case 5:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 6, CssParser.Follow._ANGLE_in_term2983);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream5.Add((object) commonToken);
                  break;
                }
                break;
              case 6:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 90, CssParser.Follow._TIME_in_term2991);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream6.Add((object) commonToken);
                  break;
                }
                break;
              case 7:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 32, CssParser.Follow._FREQ_in_term2999);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream7.Add((object) commonToken);
                  break;
                }
                break;
              case 8:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 77, CssParser.Follow._RESOLUTION_in_term3007);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream8.Add((object) commonToken);
                  break;
                }
                break;
              case 9:
                commonToken = (CommonToken) this.Match((IIntStream) this.input, 81, CssParser.Follow._SPEECH_in_term3015);
                if (this.state.failed)
                  return termReturn;
                if (this.state.backtracking == 0)
                {
                  rewriteRuleTokenStream9.Add((object) commonToken);
                  break;
                }
                break;
            }
            while (true)
            {
              CommonToken el;
              do
              {
                int num4 = 2;
                if (this.input.LA(1) == 42)
                  num4 = 1;
                if (num4 == 1)
                {
                  el = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_term3020);
                  if (this.state.failed)
                    return termReturn;
                }
                else
                  goto label_69;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream10.Add((object) el);
            }
label_69:
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleTokenStream rewriteRuleTokenStream16 = new RewriteRuleTokenStream(this.adaptor, "token t", (object) commonToken);
              RewriteRuleSubtreeStream ruleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot1 = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot1);
              if (ruleSubtreeStream1.HasNext)
                this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
              ruleSubtreeStream1.Reset();
              object oldRoot2 = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(160, "NUMBERBASEDVALUE"), oldRoot2);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream16.NextNode());
              this.adaptor.AddChild(obj2, obj3);
              while (rewriteRuleTokenStream10.HasNext)
                this.adaptor.AddChild(obj2, rewriteRuleTokenStream10.NextNode());
              rewriteRuleTokenStream10.Reset();
              this.adaptor.AddChild(obj1, obj2);
              termReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 99, CssParser.Follow._URI_in_term3052);
            if (this.state.failed)
              return termReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream11.Add((object) el1);
            while (true)
            {
              CommonToken el2;
              do
              {
                int num5 = 2;
                if (this.input.LA(1) == 42)
                  num5 = 1;
                if (num5 == 1)
                {
                  el2 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_term3054);
                  if (this.state.failed)
                    return termReturn;
                }
                else
                  goto label_87;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream10.Add((object) el2);
            }
label_87:
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream5 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot3 = this.adaptor.Nil();
              object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot3);
              object oldRoot4 = this.adaptor.Nil();
              object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(187, "URIBASEDVALUE"), oldRoot4);
              this.adaptor.AddChild(obj5, rewriteRuleTokenStream11.NextNode());
              this.adaptor.AddChild(obj4, obj5);
              while (rewriteRuleTokenStream10.HasNext)
                this.adaptor.AddChild(obj4, rewriteRuleTokenStream10.NextNode());
              rewriteRuleTokenStream10.Reset();
              this.adaptor.AddChild(obj1, obj4);
              termReturn.Tree = obj1;
              break;
            }
            break;
          case 3:
            CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 54, CssParser.Follow._MSIE_EXPRESSION_in_term3088);
            if (this.state.failed)
              return termReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream12.Add((object) el3);
            if (this.state.backtracking == 0)
              oneElement = CssParser.TrimMsieExpression(el3?.Text);
            while (true)
            {
              CommonToken el4;
              do
              {
                int num6 = 2;
                if (this.input.LA(1) == 42)
                  num6 = 1;
                if (num6 == 1)
                {
                  el4 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_term3093);
                  if (this.state.failed)
                    return termReturn;
                }
                else
                  goto label_105;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream10.Add((object) el4);
            }
label_105:
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleTokenStream rewriteRuleTokenStream17 = new RewriteRuleTokenStream(this.adaptor, "token exp", (object) oneElement);
              RewriteRuleSubtreeStream ruleSubtreeStream6 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot5 = this.adaptor.Nil();
              object obj6 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot5);
              object oldRoot6 = this.adaptor.Nil();
              object obj7 = this.adaptor.BecomeRoot(this.adaptor.Create(179, "STRINGBASEDVALUE"), oldRoot6);
              this.adaptor.AddChild(obj7, rewriteRuleTokenStream17.NextNode());
              this.adaptor.AddChild(obj6, obj7);
              while (rewriteRuleTokenStream10.HasNext)
                this.adaptor.AddChild(obj6, rewriteRuleTokenStream10.NextNode());
              rewriteRuleTokenStream10.Reset();
              this.adaptor.AddChild(obj1, obj6);
              termReturn.Tree = obj1;
              break;
            }
            break;
          case 4:
            CommonToken el5 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_term3122);
            if (this.state.failed)
              return termReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream13.Add((object) el5);
            while (true)
            {
              CommonToken el6;
              do
              {
                int num7 = 2;
                if (this.input.LA(1) == 42)
                  num7 = 1;
                if (num7 == 1)
                {
                  el6 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_term3124);
                  if (this.state.failed)
                    return termReturn;
                }
                else
                  goto label_121;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream10.Add((object) el6);
            }
label_121:
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream7 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot7 = this.adaptor.Nil();
              object obj8 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot7);
              object oldRoot8 = this.adaptor.Nil();
              object obj9 = this.adaptor.BecomeRoot(this.adaptor.Create(137, "IDENTBASEDVALUE"), oldRoot8);
              this.adaptor.AddChild(obj9, rewriteRuleTokenStream13.NextNode());
              this.adaptor.AddChild(obj8, obj9);
              while (rewriteRuleTokenStream10.HasNext)
                this.adaptor.AddChild(obj8, rewriteRuleTokenStream10.NextNode());
              rewriteRuleTokenStream10.Reset();
              this.adaptor.AddChild(obj1, obj8);
              termReturn.Tree = obj1;
              break;
            }
            break;
          case 5:
            CommonToken el7 = (CommonToken) this.Match((IIntStream) this.input, 85, CssParser.Follow._STRING_in_term3152);
            if (this.state.failed)
              return termReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream14.Add((object) el7);
            while (true)
            {
              CommonToken el8;
              do
              {
                int num8 = 2;
                if (this.input.LA(1) == 42)
                  num8 = 1;
                if (num8 == 1)
                {
                  el8 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_term3154);
                  if (this.state.failed)
                    return termReturn;
                }
                else
                  goto label_137;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream10.Add((object) el8);
            }
label_137:
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream8 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot9 = this.adaptor.Nil();
              object obj10 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot9);
              object oldRoot10 = this.adaptor.Nil();
              object obj11 = this.adaptor.BecomeRoot(this.adaptor.Create(179, "STRINGBASEDVALUE"), oldRoot10);
              this.adaptor.AddChild(obj11, rewriteRuleTokenStream14.NextNode());
              this.adaptor.AddChild(obj10, obj11);
              while (rewriteRuleTokenStream10.HasNext)
                this.adaptor.AddChild(obj10, rewriteRuleTokenStream10.NextNode());
              rewriteRuleTokenStream10.Reset();
              this.adaptor.AddChild(obj1, obj10);
              termReturn.Tree = obj1;
              break;
            }
            break;
          case 6:
            this.PushFollow(CssParser.Follow._hash_in_term3182);
            CssParser.hash_return hashReturn = this.hash();
            this.PopFollow();
            if (this.state.failed)
              return termReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream2.Add(hashReturn.Tree);
            while (true)
            {
              CommonToken el9;
              do
              {
                int num9 = 2;
                if (this.input.LA(1) == 42)
                  num9 = 1;
                if (num9 == 1)
                {
                  el9 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_term3184);
                  if (this.state.failed)
                    return termReturn;
                }
                else
                  goto label_153;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream10.Add((object) el9);
            }
label_153:
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream9 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot11 = this.adaptor.Nil();
              object obj12 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot11);
              object oldRoot12 = this.adaptor.Nil();
              object obj13 = this.adaptor.BecomeRoot(this.adaptor.Create(136, "HEXBASEDVALUE"), oldRoot12);
              this.adaptor.AddChild(obj13, ruleSubtreeStream2.NextTree());
              this.adaptor.AddChild(obj12, obj13);
              while (rewriteRuleTokenStream10.HasNext)
                this.adaptor.AddChild(obj12, rewriteRuleTokenStream10.NextNode());
              rewriteRuleTokenStream10.Reset();
              this.adaptor.AddChild(obj1, obj12);
              termReturn.Tree = obj1;
              break;
            }
            break;
          case 7:
            CommonToken el10 = (CommonToken) this.Match((IIntStream) this.input, 76, CssParser.Follow._REPLACEMENTTOKEN_in_term3209);
            if (this.state.failed)
              return termReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream15.Add((object) el10);
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream10 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot13 = this.adaptor.Nil();
              object obj14 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot13);
              object oldRoot14 = this.adaptor.Nil();
              object obj15 = this.adaptor.BecomeRoot(this.adaptor.Create(169, "REPLACEMENTTOKENBASEDVALUE"), oldRoot14);
              this.adaptor.AddChild(obj15, rewriteRuleTokenStream15.NextNode());
              this.adaptor.AddChild(obj14, obj15);
              this.adaptor.AddChild(obj1, obj14);
              termReturn.Tree = obj1;
              break;
            }
            break;
          case 8:
            this.PushFollow(CssParser.Follow._function_in_term3233);
            CssParser.function_return functionReturn = this.function();
            this.PopFollow();
            if (this.state.failed)
              return termReturn;
            if (this.state.backtracking == 0)
              ruleSubtreeStream3.Add(functionReturn.Tree);
            while (true)
            {
              CommonToken el11;
              do
              {
                int num10 = 2;
                if (this.input.LA(1) == 42)
                  num10 = 1;
                if (num10 == 1)
                {
                  el11 = (CommonToken) this.Match((IIntStream) this.input, 42, CssParser.Follow._IMPORTANT_COMMENTS_in_term3235);
                  if (this.state.failed)
                    return termReturn;
                }
                else
                  goto label_175;
              }
              while (this.state.backtracking != 0);
              rewriteRuleTokenStream10.Add((object) el11);
            }
label_175:
            if (this.state.backtracking == 0)
            {
              termReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream11 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", termReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj16 = this.adaptor.BecomeRoot(this.adaptor.Create(181, "TERM"), oldRoot);
              this.adaptor.AddChild(obj16, ruleSubtreeStream3.NextTree());
              while (rewriteRuleTokenStream10.HasNext)
                this.adaptor.AddChild(obj16, rewriteRuleTokenStream10.NextNode());
              rewriteRuleTokenStream10.Reset();
              this.adaptor.AddChild(obj1, obj16);
              termReturn.Tree = obj1;
              break;
            }
            break;
        }
        termReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          termReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(termReturn.Tree, (IToken) termReturn.Start, (IToken) termReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        termReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) termReturn.Start, this.input.LT(-1), ex);
      }
      return termReturn;
    }

    [GrammarRule("hash")]
    private CssParser.hash_return hash()
    {
      CssParser.hash_return hashReturn = new CssParser.hash_return(this);
      hashReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token HASH_IDENT");
      try
      {
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 38, CssParser.Follow._HASH_IDENT_in_hash3268);
        if (this.state.failed)
          return hashReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          hashReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", hashReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(135, "HASHIDENTIFIER"), oldRoot);
          this.adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
          this.adaptor.AddChild(obj1, obj2);
          hashReturn.Tree = obj1;
        }
        hashReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          hashReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(hashReturn.Tree, (IToken) hashReturn.Start, (IToken) hashReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        hashReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) hashReturn.Start, this.input.LT(-1), ex);
      }
      return hashReturn;
    }

    [GrammarRule("function")]
    private CssParser.function_return function()
    {
      CssParser.function_return functionReturn = new CssParser.function_return(this);
      functionReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token CIRCLE_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule beginfunc");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule expr");
      try
      {
        this.PushFollow(CssParser.Follow._beginfunc_in_function3300);
        CssParser.beginfunc_return beginfuncReturn = this.beginfunc();
        this.PopFollow();
        if (this.state.failed)
          return functionReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(beginfuncReturn.Tree);
        int num1 = 2;
        int num2 = this.input.LA(1);
        if (num2 == 6 || num2 >= 32 && num2 <= 33 || num2 == 38 || num2 >= 41 && num2 <= 42 || num2 == 49 || num2 >= 53 && num2 <= 55 || num2 == 64 || num2 == 69 || num2 == 71 || num2 >= 75 && num2 <= 77 || num2 == 81 || num2 == 85 || num2 >= 90 && num2 <= 91 || num2 == 99)
          num1 = 1;
        if (num1 == 1)
        {
          this.PushFollow(CssParser.Follow._expr_in_function3302);
          CssParser.expr_return exprReturn = this.expr();
          this.PopFollow();
          if (this.state.failed)
            return functionReturn;
          if (this.state.backtracking == 0)
            ruleSubtreeStream2.Add(exprReturn.Tree);
        }
        CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 13, CssParser.Follow._CIRCLE_END_in_function3305);
        if (this.state.failed)
          return functionReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream.Add((object) el);
        if (this.state.backtracking == 0)
        {
          functionReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", functionReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(130, "FUNCTIONBASEDVALUE"), oldRoot);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          if (ruleSubtreeStream2.HasNext)
            this.adaptor.AddChild(obj2, ruleSubtreeStream2.NextTree());
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          functionReturn.Tree = obj1;
        }
        functionReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          functionReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(functionReturn.Tree, (IToken) functionReturn.Start, (IToken) functionReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        functionReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) functionReturn.Start, this.input.LT(-1), ex);
      }
      return functionReturn;
    }

    [GrammarRule("beginfunc")]
    private CssParser.beginfunc_return beginfunc()
    {
      CssParser.beginfunc_return beginfuncReturn = new CssParser.beginfunc_return(this);
      beginfuncReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token CIRCLE_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token FROM");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token TO");
      RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(this.adaptor, "token MSIE_IMAGE_TRANSFORM");
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 33:
            num = 2;
            break;
          case 41:
            num = 1;
            break;
          case 55:
            num = 4;
            break;
          case 91:
            num = 3;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 67, 0, (IIntStream) this.input);
            this.state.failed = true;
            return beginfuncReturn;
        }
        switch (num)
        {
          case 1:
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_beginfunc3337);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream1.Add((object) el1);
            CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 12, CssParser.Follow._CIRCLE_BEGIN_in_beginfunc3339);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el2);
            if (this.state.backtracking == 0)
            {
              beginfuncReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", beginfuncReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(131, "FUNCTIONNAME"), oldRoot);
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
              this.adaptor.AddChild(obj1, obj2);
              beginfuncReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 33, CssParser.Follow._FROM_in_beginfunc3361);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream3.Add((object) el3);
            CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 12, CssParser.Follow._CIRCLE_BEGIN_in_beginfunc3363);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el4);
            if (this.state.backtracking == 0)
            {
              beginfuncReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", beginfuncReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(131, "FUNCTIONNAME"), oldRoot);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream3.NextNode());
              this.adaptor.AddChild(obj1, obj3);
              beginfuncReturn.Tree = obj1;
              break;
            }
            break;
          case 3:
            CommonToken el5 = (CommonToken) this.Match((IIntStream) this.input, 91, CssParser.Follow._TO_in_beginfunc3383);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream4.Add((object) el5);
            CommonToken el6 = (CommonToken) this.Match((IIntStream) this.input, 12, CssParser.Follow._CIRCLE_BEGIN_in_beginfunc3385);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el6);
            if (this.state.backtracking == 0)
            {
              beginfuncReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", beginfuncReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(131, "FUNCTIONNAME"), oldRoot);
              this.adaptor.AddChild(obj4, rewriteRuleTokenStream4.NextNode());
              this.adaptor.AddChild(obj1, obj4);
              beginfuncReturn.Tree = obj1;
              break;
            }
            break;
          case 4:
            CommonToken el7 = (CommonToken) this.Match((IIntStream) this.input, 55, CssParser.Follow._MSIE_IMAGE_TRANSFORM_in_beginfunc3406);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream5.Add((object) el7);
            CommonToken el8 = (CommonToken) this.Match((IIntStream) this.input, 12, CssParser.Follow._CIRCLE_BEGIN_in_beginfunc3408);
            if (this.state.failed)
              return beginfuncReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el8);
            if (this.state.backtracking == 0)
            {
              beginfuncReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", beginfuncReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(131, "FUNCTIONNAME"), oldRoot);
              this.adaptor.AddChild(obj5, rewriteRuleTokenStream5.NextNode());
              this.adaptor.AddChild(obj1, obj5);
              beginfuncReturn.Tree = obj1;
              break;
            }
            break;
        }
        beginfuncReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          beginfuncReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(beginfuncReturn.Tree, (IToken) beginfuncReturn.Start, (IToken) beginfuncReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        beginfuncReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) beginfuncReturn.Start, this.input.LT(-1), ex);
      }
      return beginfuncReturn;
    }

    [GrammarRule("keyframes")]
    private CssParser.keyframes_return keyframes()
    {
      CssParser.keyframes_return keyframesReturn = new CssParser.keyframes_return(this);
      keyframesReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token KEYFRAMES_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token IDENT");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token STRING");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule keyframes_block");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 47, CssParser.Follow._KEYFRAMES_SYM_in_keyframes3438);
        if (this.state.failed)
          return keyframesReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        int num1;
        switch (this.input.LA(1))
        {
          case 41:
            num1 = 1;
            break;
          case 85:
            num1 = 2;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 68, 0, (IIntStream) this.input);
            this.state.failed = true;
            return keyframesReturn;
        }
        switch (num1)
        {
          case 1:
            CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 41, CssParser.Follow._IDENT_in_keyframes3441);
            if (this.state.failed)
              return keyframesReturn;
            if (this.state.backtracking == 0)
            {
              rewriteRuleTokenStream2.Add((object) el2);
              break;
            }
            break;
          case 2:
            CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 85, CssParser.Follow._STRING_in_keyframes3443);
            if (this.state.failed)
              return keyframesReturn;
            if (this.state.backtracking == 0)
            {
              rewriteRuleTokenStream3.Add((object) el3);
              break;
            }
            break;
        }
        CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 18, CssParser.Follow._CURLY_BEGIN_in_keyframes3446);
        if (this.state.failed)
          return keyframesReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream4.Add((object) el4);
        while (true)
        {
          CssParser.keyframes_block_return keyframesBlockReturn;
          do
          {
            int num2 = 2;
            switch (this.input.LA(1))
            {
              case 33:
              case 69:
              case 91:
                num2 = 1;
                break;
            }
            if (num2 == 1)
            {
              this.PushFollow(CssParser.Follow._keyframes_block_in_keyframes3448);
              keyframesBlockReturn = this.keyframes_block();
              this.PopFollow();
              if (this.state.failed)
                return keyframesReturn;
            }
            else
              goto label_31;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream1.Add(keyframesBlockReturn.Tree);
        }
label_31:
        CommonToken el5 = (CommonToken) this.Match((IIntStream) this.input, 19, CssParser.Follow._CURLY_END_in_keyframes3451);
        if (this.state.failed)
          return keyframesReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream5.Add((object) el5);
        if (this.state.backtracking == 0)
        {
          keyframesReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", keyframesReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(141, "KEYFRAMES"), oldRoot1);
          object oldRoot2 = this.adaptor.Nil();
          object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(146, "KEYFRAMES_SYMBOL"), oldRoot2);
          this.adaptor.AddChild(obj3, rewriteRuleTokenStream1.NextNode());
          this.adaptor.AddChild(obj2, obj3);
          if (rewriteRuleTokenStream2.HasNext)
          {
            object oldRoot3 = this.adaptor.Nil();
            object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(137, "IDENTBASEDVALUE"), oldRoot3);
            this.adaptor.AddChild(obj4, rewriteRuleTokenStream2.NextNode());
            this.adaptor.AddChild(obj2, obj4);
          }
          rewriteRuleTokenStream2.Reset();
          if (rewriteRuleTokenStream3.HasNext)
          {
            object oldRoot4 = this.adaptor.Nil();
            object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(179, "STRINGBASEDVALUE"), oldRoot4);
            this.adaptor.AddChild(obj5, rewriteRuleTokenStream3.NextNode());
            this.adaptor.AddChild(obj2, obj5);
          }
          rewriteRuleTokenStream3.Reset();
          if (ruleSubtreeStream1.HasNext)
          {
            object oldRoot5 = this.adaptor.Nil();
            object obj6 = this.adaptor.BecomeRoot(this.adaptor.Create(143, "KEYFRAMES_BLOCKS"), oldRoot5);
            while (ruleSubtreeStream1.HasNext)
              this.adaptor.AddChild(obj6, ruleSubtreeStream1.NextTree());
            ruleSubtreeStream1.Reset();
            this.adaptor.AddChild(obj2, obj6);
          }
          ruleSubtreeStream1.Reset();
          this.adaptor.AddChild(obj1, obj2);
          keyframesReturn.Tree = obj1;
        }
        keyframesReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          keyframesReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(keyframesReturn.Tree, (IToken) keyframesReturn.Start, (IToken) keyframesReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        keyframesReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) keyframesReturn.Start, this.input.LT(-1), ex);
      }
      return keyframesReturn;
    }

    [GrammarRule("keyframes_block")]
    private CssParser.keyframes_block_return keyframes_block()
    {
      CssParser.keyframes_block_return keyframesBlockReturn = new CssParser.keyframes_block_return(this);
      keyframesBlockReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token SEMICOLON");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule keyframes_selectors");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule declaration");
      try
      {
        this.PushFollow(CssParser.Follow._keyframes_selectors_in_keyframes_block3507);
        CssParser.keyframes_selectors_return keyframesSelectorsReturn = this.keyframes_selectors();
        this.PopFollow();
        if (this.state.failed)
          return keyframesBlockReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(keyframesSelectorsReturn.Tree);
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 18, CssParser.Follow._CURLY_BEGIN_in_keyframes_block3509);
        if (this.state.failed)
          return keyframesBlockReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        while (true)
        {
          CommonToken el2;
          do
          {
            int num1;
            do
            {
              int num2 = 2;
              int num3 = this.input.LA(1);
              if (num3 >= 41 && num3 <= 42 || num3 == 76 || num3 == 84)
                num2 = 1;
              if (num2 == 1)
              {
                this.PushFollow(CssParser.Follow._declaration_in_keyframes_block3512);
                CssParser.declaration_return declarationReturn = this.declaration();
                this.PopFollow();
                if (this.state.failed)
                  return keyframesBlockReturn;
                if (this.state.backtracking == 0)
                  ruleSubtreeStream2.Add(declarationReturn.Tree);
                num1 = 2;
                if (this.input.LA(1) == 79)
                  num1 = 1;
              }
              else
                goto label_23;
            }
            while (num1 != 1);
            el2 = (CommonToken) this.Match((IIntStream) this.input, 79, CssParser.Follow._SEMICOLON_in_keyframes_block3514);
            if (this.state.failed)
              return keyframesBlockReturn;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream2.Add((object) el2);
        }
label_23:
        CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 19, CssParser.Follow._CURLY_END_in_keyframes_block3519);
        if (this.state.failed)
          return keyframesBlockReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream3.Add((object) el3);
        if (this.state.backtracking == 0)
        {
          keyframesBlockReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", keyframesBlockReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(142, "KEYFRAMES_BLOCK"), oldRoot1);
          object oldRoot2 = this.adaptor.Nil();
          object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(145, "KEYFRAMES_SELECTORS"), oldRoot2);
          this.adaptor.AddChild(obj3, ruleSubtreeStream1.NextTree());
          this.adaptor.AddChild(obj2, obj3);
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot3 = this.adaptor.Nil();
            object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(123, "DECLARATIONS"), oldRoot3);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj4, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj4);
          }
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          keyframesBlockReturn.Tree = obj1;
        }
        keyframesBlockReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          keyframesBlockReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(keyframesBlockReturn.Tree, (IToken) keyframesBlockReturn.Start, (IToken) keyframesBlockReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        keyframesBlockReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) keyframesBlockReturn.Start, this.input.LT(-1), ex);
      }
      return keyframesBlockReturn;
    }

    [GrammarRule("keyframes_selectors")]
    private CssParser.keyframes_selectors_return keyframes_selectors()
    {
      CssParser.keyframes_selectors_return keyframesSelectorsReturn = new CssParser.keyframes_selectors_return(this);
      keyframesSelectorsReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(this.adaptor, "token COMMA");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule keyframes_selector");
      try
      {
        this.PushFollow(CssParser.Follow._keyframes_selector_in_keyframes_selectors3561);
        CssParser.keyframes_selector_return keyframesSelectorReturn1 = this.keyframes_selector();
        this.PopFollow();
        if (this.state.failed)
          return keyframesSelectorsReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(keyframesSelectorReturn1.Tree);
        while (true)
        {
          CssParser.keyframes_selector_return keyframesSelectorReturn2;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 16)
              num = 1;
            if (num == 1)
            {
              CommonToken el = (CommonToken) this.Match((IIntStream) this.input, 16, CssParser.Follow._COMMA_in_keyframes_selectors3564);
              if (this.state.failed)
                return keyframesSelectorsReturn;
              if (this.state.backtracking == 0)
                rewriteRuleTokenStream.Add((object) el);
              this.PushFollow(CssParser.Follow._keyframes_selector_in_keyframes_selectors3566);
              keyframesSelectorReturn2 = this.keyframes_selector();
              this.PopFollow();
              if (this.state.failed)
                return keyframesSelectorsReturn;
            }
            else
              goto label_16;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream1.Add(keyframesSelectorReturn2.Tree);
        }
label_16:
        if (this.state.backtracking == 0)
        {
          keyframesSelectorsReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", keyframesSelectorsReturn?.Tree);
          obj1 = this.adaptor.Nil();
          while (ruleSubtreeStream1.HasNext)
          {
            object oldRoot = this.adaptor.Nil();
            object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(144, "KEYFRAMES_SELECTOR"), oldRoot);
            this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
            this.adaptor.AddChild(obj1, obj2);
          }
          ruleSubtreeStream1.Reset();
          keyframesSelectorsReturn.Tree = obj1;
        }
        keyframesSelectorsReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          keyframesSelectorsReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(keyframesSelectorsReturn.Tree, (IToken) keyframesSelectorsReturn.Start, (IToken) keyframesSelectorsReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        keyframesSelectorsReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) keyframesSelectorsReturn.Start, this.input.LT(-1), ex);
      }
      return keyframesSelectorsReturn;
    }

    [GrammarRule("keyframes_selector")]
    private CssParser.keyframes_selector_return keyframes_selector()
    {
      CssParser.keyframes_selector_return keyframesSelectorReturn = new CssParser.keyframes_selector_return(this);
      keyframesSelectorReturn.Start = (CommonToken) this.input.LT(1);
      try
      {
        object obj = this.adaptor.Nil();
        CommonToken payload = (CommonToken) this.input.LT(1);
        if (this.input.LA(1) == 33 || this.input.LA(1) == 69 || this.input.LA(1) == 91)
        {
          this.input.Consume();
          if (this.state.backtracking == 0)
            this.adaptor.AddChild(obj, this.adaptor.Create((IToken) payload));
          this.state.errorRecovery = false;
          this.state.failed = false;
          keyframesSelectorReturn.Stop = (CommonToken) this.input.LT(-1);
          if (this.state.backtracking == 0)
          {
            keyframesSelectorReturn.Tree = this.adaptor.RulePostProcessing(obj);
            this.adaptor.SetTokenBoundaries(keyframesSelectorReturn.Tree, (IToken) keyframesSelectorReturn.Start, (IToken) keyframesSelectorReturn.Stop);
          }
        }
        else
        {
          if (this.state.backtracking <= 0)
            throw new MismatchedSetException((BitSet) null, (IIntStream) this.input);
          this.state.failed = true;
          return keyframesSelectorReturn;
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        keyframesSelectorReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) keyframesSelectorReturn.Start, this.input.LT(-1), ex);
      }
      return keyframesSelectorReturn;
    }

    [GrammarRule("document")]
    private CssParser.document_return document()
    {
      CssParser.document_return documentReturn = new CssParser.document_return(this);
      documentReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token DOCUMENT_SYM");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token S");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_BEGIN");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token CURLY_END");
      RewriteRuleSubtreeStream ruleSubtreeStream1 = new RewriteRuleSubtreeStream(this.adaptor, "rule document_match_function");
      RewriteRuleSubtreeStream ruleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule ruleset");
      try
      {
        CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 24, CssParser.Follow._DOCUMENT_SYM_in_document3619);
        if (this.state.failed)
          return documentReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream1.Add((object) el1);
        while (true)
        {
          CommonToken el2;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 78)
              num = 1;
            if (num == 1)
            {
              el2 = (CommonToken) this.Match((IIntStream) this.input, 78, CssParser.Follow._S_in_document3621);
              if (this.state.failed)
                return documentReturn;
            }
            else
              goto label_12;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream2.Add((object) el2);
        }
label_12:
        this.PushFollow(CssParser.Follow._document_match_function_in_document3624);
        CssParser.document_match_function_return matchFunctionReturn = this.document_match_function();
        this.PopFollow();
        if (this.state.failed)
          return documentReturn;
        if (this.state.backtracking == 0)
          ruleSubtreeStream1.Add(matchFunctionReturn.Tree);
        while (true)
        {
          CommonToken el3;
          do
          {
            int num = 2;
            if (this.input.LA(1) == 78)
              num = 1;
            if (num == 1)
            {
              el3 = (CommonToken) this.Match((IIntStream) this.input, 78, CssParser.Follow._S_in_document3626);
              if (this.state.failed)
                return documentReturn;
            }
            else
              goto label_23;
          }
          while (this.state.backtracking != 0);
          rewriteRuleTokenStream2.Add((object) el3);
        }
label_23:
        CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 18, CssParser.Follow._CURLY_BEGIN_in_document3629);
        if (this.state.failed)
          return documentReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream3.Add((object) el4);
        while (true)
        {
          CssParser.ruleset_return rulesetReturn;
          do
          {
            int num1 = 2;
            int num2 = this.input.LA(1);
            if (num2 == 7 || num2 >= 14 && num2 <= 15 || num2 == 38 || num2 == 41 || num2 == 70 || num2 == 76 || num2 == 82 || num2 == 84)
              num1 = 1;
            if (num1 == 1)
            {
              this.PushFollow(CssParser.Follow._ruleset_in_document3631);
              rulesetReturn = this.ruleset();
              this.PopFollow();
              if (this.state.failed)
                return documentReturn;
            }
            else
              goto label_34;
          }
          while (this.state.backtracking != 0);
          ruleSubtreeStream2.Add(rulesetReturn.Tree);
        }
label_34:
        CommonToken el5 = (CommonToken) this.Match((IIntStream) this.input, 19, CssParser.Follow._CURLY_END_in_document3634);
        if (this.state.failed)
          return documentReturn;
        if (this.state.backtracking == 0)
          rewriteRuleTokenStream4.Add((object) el5);
        if (this.state.backtracking == 0)
        {
          documentReturn.Tree = obj1;
          RewriteRuleSubtreeStream ruleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", documentReturn?.Tree);
          obj1 = this.adaptor.Nil();
          object oldRoot1 = this.adaptor.Nil();
          object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(124, "DOCUMENT"), oldRoot1);
          object oldRoot2 = this.adaptor.Nil();
          object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(126, "DOCUMENT_SYMBOL"), oldRoot2);
          this.adaptor.AddChild(obj3, rewriteRuleTokenStream1.NextNode());
          this.adaptor.AddChild(obj2, obj3);
          this.adaptor.AddChild(obj2, ruleSubtreeStream1.NextTree());
          if (ruleSubtreeStream2.HasNext)
          {
            object oldRoot3 = this.adaptor.Nil();
            object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(172, "RULESETS"), oldRoot3);
            while (ruleSubtreeStream2.HasNext)
              this.adaptor.AddChild(obj4, ruleSubtreeStream2.NextTree());
            ruleSubtreeStream2.Reset();
            this.adaptor.AddChild(obj2, obj4);
          }
          ruleSubtreeStream2.Reset();
          this.adaptor.AddChild(obj1, obj2);
          documentReturn.Tree = obj1;
        }
        documentReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          documentReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(documentReturn.Tree, (IToken) documentReturn.Start, (IToken) documentReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        documentReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) documentReturn.Start, this.input.LT(-1), ex);
      }
      return documentReturn;
    }

    [GrammarRule("document_match_function")]
    private CssParser.document_match_function_return document_match_function()
    {
      CssParser.document_match_function_return matchFunctionReturn = new CssParser.document_match_function_return(this);
      matchFunctionReturn.Start = (CommonToken) this.input.LT(1);
      object obj1 = (object) null;
      RewriteRuleTokenStream rewriteRuleTokenStream1 = new RewriteRuleTokenStream(this.adaptor, "token URI");
      RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(this.adaptor, "token URLPREFIX_FUNCTION");
      RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(this.adaptor, "token DOMAIN_FUNCTION");
      RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(this.adaptor, "token REGEXP_FUNCTION");
      try
      {
        int num;
        switch (this.input.LA(1))
        {
          case 25:
            num = 3;
            break;
          case 74:
            num = 4;
            break;
          case 99:
            num = 1;
            break;
          case 101:
            num = 2;
            break;
          default:
            if (this.state.backtracking <= 0)
              throw new NoViableAltException("", 76, 0, (IIntStream) this.input);
            this.state.failed = true;
            return matchFunctionReturn;
        }
        switch (num)
        {
          case 1:
            CommonToken el1 = (CommonToken) this.Match((IIntStream) this.input, 99, CssParser.Follow._URI_in_document_match_function3678);
            if (this.state.failed)
              return matchFunctionReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream1.Add((object) el1);
            if (this.state.backtracking == 0)
            {
              matchFunctionReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", matchFunctionReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj2 = this.adaptor.BecomeRoot(this.adaptor.Create(125, "DOCUMENT_MATCHNAME"), oldRoot);
              this.adaptor.AddChild(obj2, rewriteRuleTokenStream1.NextNode());
              this.adaptor.AddChild(obj1, obj2);
              matchFunctionReturn.Tree = obj1;
              break;
            }
            break;
          case 2:
            CommonToken el2 = (CommonToken) this.Match((IIntStream) this.input, 101, CssParser.Follow._URLPREFIX_FUNCTION_in_document_match_function3699);
            if (this.state.failed)
              return matchFunctionReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream2.Add((object) el2);
            if (this.state.backtracking == 0)
            {
              matchFunctionReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", matchFunctionReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj3 = this.adaptor.BecomeRoot(this.adaptor.Create(125, "DOCUMENT_MATCHNAME"), oldRoot);
              this.adaptor.AddChild(obj3, rewriteRuleTokenStream2.NextNode());
              this.adaptor.AddChild(obj1, obj3);
              matchFunctionReturn.Tree = obj1;
              break;
            }
            break;
          case 3:
            CommonToken el3 = (CommonToken) this.Match((IIntStream) this.input, 25, CssParser.Follow._DOMAIN_FUNCTION_in_document_match_function3720);
            if (this.state.failed)
              return matchFunctionReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream3.Add((object) el3);
            if (this.state.backtracking == 0)
            {
              matchFunctionReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", matchFunctionReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj4 = this.adaptor.BecomeRoot(this.adaptor.Create(125, "DOCUMENT_MATCHNAME"), oldRoot);
              this.adaptor.AddChild(obj4, rewriteRuleTokenStream3.NextNode());
              this.adaptor.AddChild(obj1, obj4);
              matchFunctionReturn.Tree = obj1;
              break;
            }
            break;
          case 4:
            CommonToken el4 = (CommonToken) this.Match((IIntStream) this.input, 74, CssParser.Follow._REGEXP_FUNCTION_in_document_match_function3740);
            if (this.state.failed)
              return matchFunctionReturn;
            if (this.state.backtracking == 0)
              rewriteRuleTokenStream4.Add((object) el4);
            if (this.state.backtracking == 0)
            {
              matchFunctionReturn.Tree = obj1;
              RewriteRuleSubtreeStream ruleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule retval", matchFunctionReturn?.Tree);
              obj1 = this.adaptor.Nil();
              object oldRoot = this.adaptor.Nil();
              object obj5 = this.adaptor.BecomeRoot(this.adaptor.Create(125, "DOCUMENT_MATCHNAME"), oldRoot);
              this.adaptor.AddChild(obj5, rewriteRuleTokenStream4.NextNode());
              this.adaptor.AddChild(obj1, obj5);
              matchFunctionReturn.Tree = obj1;
              break;
            }
            break;
        }
        matchFunctionReturn.Stop = (CommonToken) this.input.LT(-1);
        if (this.state.backtracking == 0)
        {
          matchFunctionReturn.Tree = this.adaptor.RulePostProcessing(obj1);
          this.adaptor.SetTokenBoundaries(matchFunctionReturn.Tree, (IToken) matchFunctionReturn.Start, (IToken) matchFunctionReturn.Stop);
        }
      }
      catch (RecognitionException ex)
      {
        this.ReportError(ex);
        this.Recover((IIntStream) this.input, ex);
        matchFunctionReturn.Tree = this.adaptor.ErrorNode(this.input, (IToken) matchFunctionReturn.Start, this.input.LT(-1), ex);
      }
      return matchFunctionReturn;
    }

    public void synpred1_CssParser_fragment()
    {
      this.Match((IIntStream) this.input, 105, CssParser.Follow._WS_in_synpred1_CssParser1723);
      if (!this.state.failed)
        ;
    }

    public void synpred2_CssParser_fragment()
    {
      this.PushFollow(CssParser.Follow._universal_in_synpred2_CssParser1778);
      this.universal();
      this.PopFollow();
      if (!this.state.failed)
        ;
    }

    public void synpred3_CssParser_fragment()
    {
      this.PushFollow(CssParser.Follow._type_selector_in_synpred3_CssParser1788);
      this.type_selector();
      this.PopFollow();
      if (!this.state.failed)
        ;
    }

    public void synpred4_CssParser_fragment()
    {
      this.PushFollow(CssParser.Follow._hashclassatnameattribpseudonegation_in_synpred4_CssParser1801);
      this.hashclassatnameattribpseudonegation();
      this.PopFollow();
      if (!this.state.failed)
        ;
    }

    public void synpred5_CssParser_fragment()
    {
      this.PushFollow(CssParser.Follow._hashclassatnameattribpseudonegation_in_synpred5_CssParser1843);
      this.hashclassatnameattribpseudonegation();
      this.PopFollow();
      if (!this.state.failed)
        ;
    }

    public void synpred6_CssParser_fragment()
    {
      this.PushFollow(CssParser.Follow._selector_namespace_prefix_in_synpred6_CssParser2042);
      this.selector_namespace_prefix();
      this.PopFollow();
      if (!this.state.failed)
        ;
    }

    public void synpred7_CssParser_fragment()
    {
      this.PushFollow(CssParser.Follow._selector_namespace_prefix_in_synpred7_CssParser2169);
      this.selector_namespace_prefix();
      this.PopFollow();
      if (!this.state.failed)
        ;
    }

    public void synpred8_CssParser_fragment()
    {
      this.PushFollow(CssParser.Follow._universal_in_synpred8_CssParser2635);
      this.universal();
      this.PopFollow();
      if (!this.state.failed)
        ;
    }

    private bool EvaluatePredicate(Action fragment)
    {
      ++this.state.backtracking;
      int marker = this.input.Mark();
      try
      {
        fragment();
      }
      catch (RecognitionException ex)
      {
        Console.Error.WriteLine("impossible: " + (object) ex);
      }
      bool predicate = !this.state.failed;
      this.input.Rewind(marker);
      --this.state.backtracking;
      this.state.failed = false;
      return predicate;
    }

    protected override void InitDFAs()
    {
      base.InitDFAs();
      this.dfa25 = new CssParser.DFA25((BaseRecognizer) this);
      this.dfa33 = new CssParser.DFA33((BaseRecognizer) this, new SpecialStateTransitionHandler(this.SpecialStateTransition33));
      this.dfa48 = new CssParser.DFA48((BaseRecognizer) this, new SpecialStateTransitionHandler(this.SpecialStateTransition48));
      this.dfa54 = new CssParser.DFA54((BaseRecognizer) this);
      this.dfa65 = new CssParser.DFA65((BaseRecognizer) this);
    }

    private int SpecialStateTransition33(DFA dfa, int s, IIntStream _input)
    {
      ITokenStream input = (ITokenStream) _input;
      int stateNumber = s;
      switch (s)
      {
        case 0:
          input.LA(1);
          int index1 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index1);
          if (s >= 0)
            return s;
          break;
        case 1:
          input.LA(1);
          int index2 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index2);
          if (s >= 0)
            return s;
          break;
        case 2:
          input.LA(1);
          int index3 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index3);
          if (s >= 0)
            return s;
          break;
        case 3:
          input.LA(1);
          int index4 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index4);
          if (s >= 0)
            return s;
          break;
        case 4:
          input.LA(1);
          int index5 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index5);
          if (s >= 0)
            return s;
          break;
        case 5:
          input.LA(1);
          int index6 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index6);
          if (s >= 0)
            return s;
          break;
        case 6:
          input.LA(1);
          int index7 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index7);
          if (s >= 0)
            return s;
          break;
        case 7:
          input.LA(1);
          int index8 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred4_CssParser_fragment)) ? 7 : 8;
          input.Seek(index8);
          if (s >= 0)
            return s;
          break;
      }
      if (this.state.backtracking > 0)
      {
        this.state.failed = true;
        return -1;
      }
      NoViableAltException nvae = new NoViableAltException(dfa.Description, 33, stateNumber, (IIntStream) input);
      dfa.Error(nvae);
      throw nvae;
    }

    private int SpecialStateTransition48(DFA dfa, int s, IIntStream _input)
    {
      ITokenStream input = (ITokenStream) _input;
      int stateNumber = s;
      switch (s)
      {
        case 0:
          input.LA(1);
          int index1 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred8_CssParser_fragment)) ? 8 : 9;
          input.Seek(index1);
          if (s >= 0)
            return s;
          break;
        case 1:
          input.LA(1);
          int index2 = input.Index;
          input.Rewind();
          s = -1;
          s = !this.EvaluatePredicate(new Action(this.synpred8_CssParser_fragment)) ? 8 : 9;
          input.Seek(index2);
          if (s >= 0)
            return s;
          break;
      }
      if (this.state.backtracking > 0)
      {
        this.state.failed = true;
        return -1;
      }
      NoViableAltException nvae = new NoViableAltException(dfa.Description, 48, stateNumber, (IIntStream) input);
      dfa.Error(nvae);
      throw nvae;
    }

    public sealed class main_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public main_return(CssParser grammar)
      {
      }
    }

    private sealed class styleSheet_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public styleSheet_return(CssParser grammar)
      {
      }
    }

    private sealed class styleSheetRulesOrComment_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public styleSheetRulesOrComment_return(CssParser grammar)
      {
      }
    }

    private sealed class styleimport_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public styleimport_return(CssParser grammar)
      {
      }
    }

    private sealed class namespace_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public namespace_return(CssParser grammar)
      {
      }
    }

    private sealed class namespace_prefix_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public namespace_prefix_return(CssParser grammar)
      {
      }
    }

    private sealed class wg_dpi_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public wg_dpi_return(CssParser grammar)
      {
      }
    }

    private sealed class media_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public media_return(CssParser grammar)
      {
      }
    }

    private sealed class media_query_list_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public media_query_list_return(CssParser grammar)
      {
      }
    }

    private sealed class media_query_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public media_query_return(CssParser grammar)
      {
      }
    }

    private sealed class media_type_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public media_type_return(CssParser grammar)
      {
      }
    }

    private sealed class media_expression_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public media_expression_return(CssParser grammar)
      {
      }
    }

    private sealed class media_feature_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public media_feature_return(CssParser grammar)
      {
      }
    }

    private sealed class page_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public page_return(CssParser grammar)
      {
      }
    }

    private sealed class pseudo_page_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public pseudo_page_return(CssParser grammar)
      {
      }
    }

    private sealed class operator_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public operator_return(CssParser grammar)
      {
      }
    }

    private sealed class unary_operator_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public unary_operator_return(CssParser grammar)
      {
      }
    }

    private sealed class property_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public property_return(CssParser grammar)
      {
      }
    }

    private sealed class ruleset_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public ruleset_return(CssParser grammar)
      {
      }
    }

    private sealed class selectors_group_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public selectors_group_return(CssParser grammar)
      {
      }
    }

    private sealed class selector_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public selector_return(CssParser grammar)
      {
      }
    }

    private sealed class combinator_simple_selector_sequence_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public combinator_simple_selector_sequence_return(CssParser grammar)
      {
      }
    }

    private sealed class combinator_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public combinator_return(CssParser grammar)
      {
      }
    }

    private sealed class whitespace_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public whitespace_return(CssParser grammar)
      {
      }
    }

    private sealed class simple_selector_sequence_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public simple_selector_sequence_return(CssParser grammar)
      {
      }
    }

    private sealed class hashclassatnameattribpseudonegation_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public hashclassatnameattribpseudonegation_return(CssParser grammar)
      {
      }
    }

    private sealed class type_selector_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public type_selector_return(CssParser grammar)
      {
      }
    }

    private sealed class selector_namespace_prefix_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public selector_namespace_prefix_return(CssParser grammar)
      {
      }
    }

    private sealed class element_name_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public element_name_return(CssParser grammar)
      {
      }
    }

    private sealed class universal_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public universal_return(CssParser grammar)
      {
      }
    }

    private sealed class class_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public class_return(CssParser grammar)
      {
      }
    }

    private sealed class attrib_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public attrib_return(CssParser grammar)
      {
      }
    }

    private sealed class pseudo_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public pseudo_return(CssParser grammar)
      {
      }
    }

    private sealed class functional_pseudo_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public functional_pseudo_return(CssParser grammar)
      {
      }
    }

    private sealed class selectorexpression_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public selectorexpression_return(CssParser grammar)
      {
      }
    }

    private sealed class negation_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public negation_return(CssParser grammar)
      {
      }
    }

    private sealed class negation_arg_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public negation_arg_return(CssParser grammar)
      {
      }
    }

    private sealed class atname_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public atname_return(CssParser grammar)
      {
      }
    }

    private sealed class declaration_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public declaration_return(CssParser grammar)
      {
      }
    }

    private sealed class stringoruri_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public stringoruri_return(CssParser grammar)
      {
      }
    }

    private sealed class styleSheetrules_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public styleSheetrules_return(CssParser grammar)
      {
      }
    }

    private sealed class prio_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public prio_return(CssParser grammar)
      {
      }
    }

    private sealed class expr_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public expr_return(CssParser grammar)
      {
      }
    }

    private sealed class termwithoperator_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public termwithoperator_return(CssParser grammar)
      {
      }
    }

    private sealed class term_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public term_return(CssParser grammar)
      {
      }
    }

    private sealed class hash_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public hash_return(CssParser grammar)
      {
      }
    }

    private sealed class function_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public function_return(CssParser grammar)
      {
      }
    }

    private sealed class beginfunc_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public beginfunc_return(CssParser grammar)
      {
      }
    }

    private sealed class keyframes_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public keyframes_return(CssParser grammar)
      {
      }
    }

    private sealed class keyframes_block_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public keyframes_block_return(CssParser grammar)
      {
      }
    }

    private sealed class keyframes_selectors_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public keyframes_selectors_return(CssParser grammar)
      {
      }
    }

    private sealed class keyframes_selector_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public keyframes_selector_return(CssParser grammar)
      {
      }
    }

    private sealed class document_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public document_return(CssParser grammar)
      {
      }
    }

    private sealed class document_match_function_return : 
      ParserRuleReturnScope<CommonToken>,
      IAstRuleReturnScope<object>,
      IAstRuleReturnScope,
      IRuleReturnScope
    {
      private object _tree;

      public object Tree
      {
        get => this._tree;
        set => this._tree = value;
      }

      object IAstRuleReturnScope.Tree => this.Tree;

      public document_match_function_return(CssParser grammar)
      {
      }
    }

    private class DFA25 : DFA
    {
      private const string DFA25_eotS = "\u0004\uFFFF";
      private const string DFA25_eofS = "\u0004\uFFFF";
      private const string DFA25_minS = "\u0002\u0013\u0002\uFFFF";
      private const string DFA25_maxS = "\u0002T\u0002\uFFFF";
      private const string DFA25_acceptS = "\u0002\uFFFF\u0001\u0002\u0001\u0001";
      private const string DFA25_specialS = "\u0004\uFFFF}>";
      private static readonly string[] DFA25_transitionS = new string[4]
      {
        "\u0001\u0002\u0015\uFFFF\u0001\u0003\u0001\u0001!\uFFFF\u0001\u0003\a\uFFFF\u0001\u0003",
        "\u0001\u0002\u0015\uFFFF\u0001\u0003\u0001\u0001!\uFFFF\u0001\u0003\a\uFFFF\u0001\u0003",
        "",
        ""
      };
      private static readonly short[] DFA25_eot = DFA.UnpackEncodedString("\u0004\uFFFF");
      private static readonly short[] DFA25_eof = DFA.UnpackEncodedString("\u0004\uFFFF");
      private static readonly char[] DFA25_min = DFA.UnpackEncodedStringToUnsignedChars("\u0002\u0013\u0002\uFFFF");
      private static readonly char[] DFA25_max = DFA.UnpackEncodedStringToUnsignedChars("\u0002T\u0002\uFFFF");
      private static readonly short[] DFA25_accept = DFA.UnpackEncodedString("\u0002\uFFFF\u0001\u0002\u0001\u0001");
      private static readonly short[] DFA25_special = DFA.UnpackEncodedString("\u0004\uFFFF}>");
      private static readonly short[][] DFA25_transition;

      static DFA25()
      {
        int length = CssParser.DFA25.DFA25_transitionS.Length;
        CssParser.DFA25.DFA25_transition = new short[length][];
        for (int index = 0; index < length; ++index)
          CssParser.DFA25.DFA25_transition[index] = DFA.UnpackEncodedString(CssParser.DFA25.DFA25_transitionS[index]);
      }

      public DFA25(BaseRecognizer recognizer)
      {
        this.recognizer = recognizer;
        this.decisionNumber = 25;
        this.eot = CssParser.DFA25.DFA25_eot;
        this.eof = CssParser.DFA25.DFA25_eof;
        this.min = CssParser.DFA25.DFA25_min;
        this.max = CssParser.DFA25.DFA25_max;
        this.accept = CssParser.DFA25.DFA25_accept;
        this.special = CssParser.DFA25.DFA25_special;
        this.transition = CssParser.DFA25.DFA25_transition;
      }

      public override string Description => "()* loopback of 266:5: ( declaration ( SEMICOLON )? )*";

      public override void Error(NoViableAltException nvae)
      {
      }
    }

    private class DFA33 : DFA
    {
      private const string DFA33_eotS = "D\uFFFF";
      private const string DFA33_eofS = "D\uFFFF";
      private const string DFA33_minS = "\u0001\a\u0004\0\u0001)\u0001\u000F\u0002\uFFFF\u0001\u0015\u0001F\u0001)\u0001\f\u0001!\u0001\0\u0003\f\u0006)\u0001\0\u0001\u0015\u0001\u000E\u0003\u0017\u0002S\u0002\r\u0001)\u0002\r\u0001)\u0001\u000F\u0001\r\u0001\0\u0002\r\u0001\u0015\u0001F\u0001)\u0001!\u0004\f\u0001\0\u0006)\u0001\r\u0001\u0015\u0004\u0017\u0002S\u0002\r";
      private const string DFA33_maxS = "\u0001i\u0004\0\u0001T\u0001[\u0002\uFFFF\u0001W\u0001F\u0001)\u0001\f\u0001[\u0001\0\u0003\f\u0006U\u0001\0\u0001W\u0001T\u0003¨\u0002S\u0002F\u0001T\u0002\r\u0001T\u0001[\u0001¨\u0001\0\u0002\r\u0001W\u0001F\u0001)\u0001[\u0001\r\u0003\f\u0001\0\u0006U\u0001\r\u0001W\u0004¨\u0002S\u0001¨\u0001\r";
      private const string DFA33_acceptS = "\a\uFFFF\u0001\u0002\u0001\u0001;\uFFFF";
      private const string DFA33_specialS = "\u0001\uFFFF\u0001\0\u0001\u0001\u0001\u0002\u0001\u0003\t\uFFFF\u0001\u0004\t\uFFFF\u0001\u0005\u000F\uFFFF\u0001\u0006\n\uFFFF\u0001\a\u0010\uFFFF}>";
      private static readonly string[] DFA33_transitionS = new string[68]
      {
        "\u0001\u0004\u0006\uFFFF\u0001\u0003\u0001\u0006\u0001\a\u0001\uFFFF\u0001\a\u0010\uFFFF\u0001\a\u0002\uFFFF\u0001\u0002\u0002\uFFFF\u0001\a\u001C\uFFFF\u0002\a\u0004\uFFFF\u0001\u0001\u0005\uFFFF\u0001\u0005\u0001\uFFFF\u0001\a\u0004\uFFFF\u0001\a\u000F\uFFFF\u0001\a",
        "\u0001\uFFFF",
        "\u0001\uFFFF",
        "\u0001\uFFFF",
        "\u0001\uFFFF",
        "\u0001\t\u001C\uFFFF\u0001\v\r\uFFFF\u0001\n",
        "\u0001\r\u0011\uFFFF\u0001\u000F\a\uFFFF\u0001\u000E\r\uFFFF\u0001\u0011\a\uFFFF\u0001\f\u001B\uFFFF\u0001\u0010",
        "",
        "",
        "\u0001\u0017\u0006\uFFFF\u0001\u0015\u0010\uFFFF\u0001\u0016\u0018\uFFFF\u0001\v\u0001\uFFFF\u0001\u0012\n\uFFFF\u0001\u0018\u0002\uFFFF\u0001\u0014\u0001\u0013",
        "\u0001\v",
        "\u0001\u0019",
        "\u0001\u001A",
        "\u0001\u000F\a\uFFFF\u0001\u000E\r\uFFFF\u0001\u0011#\uFFFF\u0001\u0010",
        "\u0001\uFFFF",
        "\u0001\u001B",
        "\u0001\u001C",
        "\u0001\u001D",
        "\u0001\u001E+\uFFFF\u0001\u001F",
        "\u0001\u001E+\uFFFF\u0001\u001F",
        "\u0001\u001E+\uFFFF\u0001\u001F",
        "\u0001\u001E+\uFFFF\u0001\u001F",
        "\u0001\u001E+\uFFFF\u0001\u001F",
        "\u0001\u001E+\uFFFF\u0001\u001F",
        "\u0001\uFFFF",
        "\u0001\u0017\u0006\uFFFF\u0001\u0015\u0010\uFFFF\u0001\u0016\u001A\uFFFF\u0001\u0012\n\uFFFF\u0001\u0018\u0002\uFFFF\u0001\u0014\u0001\u0013",
        "\u0001$\u0001&\u0016\uFFFF\u0001#\u0002\uFFFF\u0001 \u001C\uFFFF\u0001\"\v\uFFFF\u0001%\u0001\uFFFF\u0001!",
        "\u0001'\u0011\uFFFF\u0001'\v\uFFFF\u0001'\n\uFFFF\u0001'\u0006\uFFFF\u0001'\r\uFFFF\u0001'R\uFFFF\u0001'",
        "\u0001'\u0011\uFFFF\u0001'\v\uFFFF\u0001'\n\uFFFF\u0001'\u0006\uFFFF\u0001'\r\uFFFF\u0001'R\uFFFF\u0001'",
        "\u0001'\u0011\uFFFF\u0001'\v\uFFFF\u0001'\n\uFFFF\u0001'\u0006\uFFFF\u0001'\r\uFFFF\u0001'R\uFFFF\u0001'",
        "\u0001\u0018",
        "\u0001\u0018",
        "\u0001(8\uFFFF\u0001\"",
        "\u0001(8\uFFFF\u0001\"",
        "\u0001**\uFFFF\u0001)",
        "\u0001(",
        "\u0001(",
        "\u0001+\u001C\uFFFF\u0001-\r\uFFFF\u0001,",
        "\u0001.\u0011\uFFFF\u00010\a\uFFFF\u0001/\r\uFFFF\u00012#\uFFFF\u00011",
        "\u00013\t\uFFFF\u0001'\u0011\uFFFF\u0001'\v\uFFFF\u0001'\n\uFFFF\u0001'\u0006\uFFFF\u0001'\r\uFFFF\u0001'R\uFFFF\u0001'",
        "\u0001\uFFFF",
        "\u0001(",
        "\u0001(",
        "\u00019\u0006\uFFFF\u00017\u0010\uFFFF\u00018\u0018\uFFFF\u0001-\u0001\uFFFF\u00014\n\uFFFF\u0001:\u0002\uFFFF\u00016\u00015",
        "\u0001-",
        "\u0001;",
        "\u00010\a\uFFFF\u0001/\r\uFFFF\u00012#\uFFFF\u00011",
        "\u0001<\u0001(",
        "\u0001=",
        "\u0001>",
        "\u0001?",
        "\u0001\uFFFF",
        "\u0001@+\uFFFF\u0001A",
        "\u0001@+\uFFFF\u0001A",
        "\u0001@+\uFFFF\u0001A",
        "\u0001@+\uFFFF\u0001A",
        "\u0001@+\uFFFF\u0001A",
        "\u0001@+\uFFFF\u0001A",
        "\u0001(",
        "\u00019\u0006\uFFFF\u00017\u0010\uFFFF\u00018\u001A\uFFFF\u00014\n\uFFFF\u0001:\u0002\uFFFF\u00016\u00015",
        "\u0001B\u0011\uFFFF\u0001B\v\uFFFF\u0001B\n\uFFFF\u0001B\u0006\uFFFF\u0001B\r\uFFFF\u0001BR\uFFFF\u0001B",
        "\u0001B\u0011\uFFFF\u0001B\v\uFFFF\u0001B\n\uFFFF\u0001B\u0006\uFFFF\u0001B\r\uFFFF\u0001BR\uFFFF\u0001B",
        "\u0001B\u0011\uFFFF\u0001B\v\uFFFF\u0001B\n\uFFFF\u0001B\u0006\uFFFF\u0001B\r\uFFFF\u0001BR\uFFFF\u0001B",
        "\u0001B\u0011\uFFFF\u0001B\v\uFFFF\u0001B\n\uFFFF\u0001B\u0006\uFFFF\u0001B\r\uFFFF\u0001BR\uFFFF\u0001B",
        "\u0001:",
        "\u0001:",
        "\u0001C\t\uFFFF\u0001B\u0011\uFFFF\u0001B\v\uFFFF\u0001B\n\uFFFF\u0001B\u0006\uFFFF\u0001B\r\uFFFF\u0001BR\uFFFF\u0001B",
        "\u0001("
      };
      private static readonly short[] DFA33_eot = DFA.UnpackEncodedString("D\uFFFF");
      private static readonly short[] DFA33_eof = DFA.UnpackEncodedString("D\uFFFF");
      private static readonly char[] DFA33_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\a\u0004\0\u0001)\u0001\u000F\u0002\uFFFF\u0001\u0015\u0001F\u0001)\u0001\f\u0001!\u0001\0\u0003\f\u0006)\u0001\0\u0001\u0015\u0001\u000E\u0003\u0017\u0002S\u0002\r\u0001)\u0002\r\u0001)\u0001\u000F\u0001\r\u0001\0\u0002\r\u0001\u0015\u0001F\u0001)\u0001!\u0004\f\u0001\0\u0006)\u0001\r\u0001\u0015\u0004\u0017\u0002S\u0002\r");
      private static readonly char[] DFA33_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001i\u0004\0\u0001T\u0001[\u0002\uFFFF\u0001W\u0001F\u0001)\u0001\f\u0001[\u0001\0\u0003\f\u0006U\u0001\0\u0001W\u0001T\u0003¨\u0002S\u0002F\u0001T\u0002\r\u0001T\u0001[\u0001¨\u0001\0\u0002\r\u0001W\u0001F\u0001)\u0001[\u0001\r\u0003\f\u0001\0\u0006U\u0001\r\u0001W\u0004¨\u0002S\u0001¨\u0001\r");
      private static readonly short[] DFA33_accept = DFA.UnpackEncodedString("\a\uFFFF\u0001\u0002\u0001\u0001;\uFFFF");
      private static readonly short[] DFA33_special = DFA.UnpackEncodedString("\u0001\uFFFF\u0001\0\u0001\u0001\u0001\u0002\u0001\u0003\t\uFFFF\u0001\u0004\t\uFFFF\u0001\u0005\u000F\uFFFF\u0001\u0006\n\uFFFF\u0001\a\u0010\uFFFF}>");
      private static readonly short[][] DFA33_transition;

      static DFA33()
      {
        int length = CssParser.DFA33.DFA33_transitionS.Length;
        CssParser.DFA33.DFA33_transition = new short[length][];
        for (int index = 0; index < length; ++index)
          CssParser.DFA33.DFA33_transition[index] = DFA.UnpackEncodedString(CssParser.DFA33.DFA33_transitionS[index]);
      }

      public DFA33(
        BaseRecognizer recognizer,
        SpecialStateTransitionHandler specialStateTransition)
        : base(specialStateTransition)
      {
        this.recognizer = recognizer;
        this.decisionNumber = 33;
        this.eot = CssParser.DFA33.DFA33_eot;
        this.eof = CssParser.DFA33.DFA33_eof;
        this.min = CssParser.DFA33.DFA33_min;
        this.max = CssParser.DFA33.DFA33_max;
        this.accept = CssParser.DFA33.DFA33_accept;
        this.special = CssParser.DFA33.DFA33_special;
        this.transition = CssParser.DFA33.DFA33_transition;
      }

      public override string Description => "323:82: ( ( hashclassatnameattribpseudonegation )=> hashclassatnameattribpseudonegation )?";

      public override void Error(NoViableAltException nvae)
      {
      }
    }

    private class DFA48 : DFA
    {
      private const string DFA48_eotS = "\v\uFFFF";
      private const string DFA48_eofS = "\v\uFFFF";
      private const string DFA48_minS = "\u0001\u000E\u0001\r\u0001\0\u0001)\u0006\uFFFF\u0001\0";
      private const string DFA48_maxS = "\u0001T\u0001F\u0001\0\u0001T\u0006\uFFFF\u0001\0";
      private const string DFA48_acceptS = "\u0004\uFFFF\u0001\u0003\u0001\u0004\u0001\u0005\u0001\u0006\u0001\u0002\u0001\u0001\u0001\uFFFF";
      private const string DFA48_specialS = "\u0002\uFFFF\u0001\0\a\uFFFF\u0001\u0001}>";
      private static readonly string[] DFA48_transitionS = new string[11]
      {
        "\u0001\u0005\u0001\a\u0016\uFFFF\u0001\u0004\u0002\uFFFF\u0001\u0001\u001C\uFFFF\u0001\u0003\v\uFFFF\u0001\u0006\u0001\uFFFF\u0001\u0002",
        "\u0001\b8\uFFFF\u0001\u0003",
        "\u0001\uFFFF",
        "\u0001\b*\uFFFF\u0001\n",
        "",
        "",
        "",
        "",
        "",
        "",
        "\u0001\uFFFF"
      };
      private static readonly short[] DFA48_eot = DFA.UnpackEncodedString("\v\uFFFF");
      private static readonly short[] DFA48_eof = DFA.UnpackEncodedString("\v\uFFFF");
      private static readonly char[] DFA48_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\u000E\u0001\r\u0001\0\u0001)\u0006\uFFFF\u0001\0");
      private static readonly char[] DFA48_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001T\u0001F\u0001\0\u0001T\u0006\uFFFF\u0001\0");
      private static readonly short[] DFA48_accept = DFA.UnpackEncodedString("\u0004\uFFFF\u0001\u0003\u0001\u0004\u0001\u0005\u0001\u0006\u0001\u0002\u0001\u0001\u0001\uFFFF");
      private static readonly short[] DFA48_special = DFA.UnpackEncodedString("\u0002\uFFFF\u0001\0\a\uFFFF\u0001\u0001}>");
      private static readonly short[][] DFA48_transition;

      static DFA48()
      {
        int length = CssParser.DFA48.DFA48_transitionS.Length;
        CssParser.DFA48.DFA48_transition = new short[length][];
        for (int index = 0; index < length; ++index)
          CssParser.DFA48.DFA48_transition[index] = DFA.UnpackEncodedString(CssParser.DFA48.DFA48_transitionS[index]);
      }

      public DFA48(
        BaseRecognizer recognizer,
        SpecialStateTransitionHandler specialStateTransition)
        : base(specialStateTransition)
      {
        this.recognizer = recognizer;
        this.decisionNumber = 48;
        this.eot = CssParser.DFA48.DFA48_eot;
        this.eof = CssParser.DFA48.DFA48_eof;
        this.min = CssParser.DFA48.DFA48_min;
        this.max = CssParser.DFA48.DFA48_max;
        this.accept = CssParser.DFA48.DFA48_accept;
        this.special = CssParser.DFA48.DFA48_special;
        this.transition = CssParser.DFA48.DFA48_transition;
      }

      public override string Description => "460:1: negation_arg : ( ( ( universal )=> universal ) | type_selector | hash | class | attrib | pseudo );";

      public override void Error(NoViableAltException nvae)
      {
      }
    }

    private class DFA54 : DFA
    {
      private const string DFA54_eotS = "\b\uFFFF";
      private const string DFA54_eofS = "\b\uFFFF";
      private const string DFA54_minS = "\u0001\u0006\u0001\uFFFF\u0003\u0006\u0001\uFFFF\u0002\u0006";
      private const string DFA54_maxS = "\u0001c\u0001\uFFFF\u0003c\u0001\uFFFF\u0002c";
      private const string DFA54_acceptS = "\u0001\uFFFF\u0001\u0002\u0003\uFFFF\u0001\u0001\u0002\uFFFF";
      private const string DFA54_specialS = "\b\uFFFF}>";
      private static readonly string[] DFA54_transitionS = new string[8]
      {
        "\u0001\u0005\u0006\uFFFF\u0001\u0001\u0002\uFFFF\u0001\u0005\u0002\uFFFF\u0001\u0001\b\uFFFF\u0001\u0005\u0002\uFFFF\u0003\u0005\u0004\uFFFF\u0001\u0005\u0002\uFFFF\u0001\u0003\u0002\u0001\u0005\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\b\uFFFF\u0001\u0005\u0004\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0003\uFFFF\u0001\u0005\u0001\u0004\u0001\u0005\u0001\uFFFF\u0001\u0001\u0001\uFFFF\u0001\u0005\u0002\uFFFF\u0001\u0002\u0001\u0005\u0004\uFFFF\u0002\u0005\a\uFFFF\u0001\u0005",
        "",
        "\u0001\u0005\u0019\uFFFF\u0002\u0005\u0004\uFFFF\u0001\u0005\u0002\uFFFF\u0001\u0003\a\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\b\uFFFF\u0001\u0005\u0004\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\u0003\uFFFF\u0001\u0005\u0003\uFFFF\u0001\u0005\u0004\uFFFF\u0002\u0005\a\uFFFF\u0001\u0005",
        "\u0001\u0005\u0005\uFFFF\u0002\u0005\u0001\uFFFF\u0001\u0001\u0001\u0005\u0002\uFFFF\u0001\u0005\b\uFFFF\u0001\u0005\u0002\uFFFF\u0003\u0005\u0004\uFFFF\u0001\u0005\u0002\uFFFF\u0001\u0005\u0001\u0006\u0001\u0005\u0005\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\b\uFFFF\u0001\u0005\u0004\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\u0001\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0002\uFFFF\u0002\u0005\u0004\uFFFF\u0002\u0005\a\uFFFF\u0001\u0005",
        "\u0001\u0005\u0006\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0001\u0001\u0005\u0002\uFFFF\u0001\u0005\b\uFFFF\u0001\u0005\u0002\uFFFF\u0003\u0005\u0004\uFFFF\u0001\u0005\u0002\uFFFF\u0003\u0005\u0005\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\b\uFFFF\u0001\u0005\u0004\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\u0001\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0002\uFFFF\u0002\u0005\u0004\uFFFF\u0002\u0005\a\uFFFF\u0001\u0005",
        "",
        "\u0001\u0005\u0006\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0001\u0001\u0005\u0002\uFFFF\u0001\u0005\b\uFFFF\u0001\u0005\u0002\uFFFF\u0003\u0005\u0004\uFFFF\u0001\u0005\u0002\uFFFF\u0001\u0005\u0001\a\u0001\u0005\u0005\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\b\uFFFF\u0001\u0005\u0004\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\u0001\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0002\uFFFF\u0002\u0005\u0004\uFFFF\u0002\u0005\a\uFFFF\u0001\u0005",
        "\u0001\u0005\u0006\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0001\u0001\u0005\u0002\uFFFF\u0001\u0005\b\uFFFF\u0001\u0005\u0002\uFFFF\u0003\u0005\u0004\uFFFF\u0001\u0005\u0002\uFFFF\u0001\u0005\u0001\a\u0001\u0005\u0005\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\b\uFFFF\u0001\u0005\u0004\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0003\uFFFF\u0003\u0005\u0001\uFFFF\u0001\u0005\u0001\uFFFF\u0001\u0005\u0002\uFFFF\u0002\u0005\u0004\uFFFF\u0002\u0005\a\uFFFF\u0001\u0005"
      };
      private static readonly short[] DFA54_eot = DFA.UnpackEncodedString("\b\uFFFF");
      private static readonly short[] DFA54_eof = DFA.UnpackEncodedString("\b\uFFFF");
      private static readonly char[] DFA54_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\u0006\u0001\uFFFF\u0003\u0006\u0001\uFFFF\u0002\u0006");
      private static readonly char[] DFA54_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001c\u0001\uFFFF\u0003c\u0001\uFFFF\u0002c");
      private static readonly short[] DFA54_accept = DFA.UnpackEncodedString("\u0001\uFFFF\u0001\u0002\u0003\uFFFF\u0001\u0001\u0002\uFFFF");
      private static readonly short[] DFA54_special = DFA.UnpackEncodedString("\b\uFFFF}>");
      private static readonly short[][] DFA54_transition;

      static DFA54()
      {
        int length = CssParser.DFA54.DFA54_transitionS.Length;
        CssParser.DFA54.DFA54_transition = new short[length][];
        for (int index = 0; index < length; ++index)
          CssParser.DFA54.DFA54_transition[index] = DFA.UnpackEncodedString(CssParser.DFA54.DFA54_transitionS[index]);
      }

      public DFA54(BaseRecognizer recognizer)
      {
        this.recognizer = recognizer;
        this.decisionNumber = 54;
        this.eot = CssParser.DFA54.DFA54_eot;
        this.eof = CssParser.DFA54.DFA54_eof;
        this.min = CssParser.DFA54.DFA54_min;
        this.max = CssParser.DFA54.DFA54_max;
        this.accept = CssParser.DFA54.DFA54_accept;
        this.special = CssParser.DFA54.DFA54_special;
        this.transition = CssParser.DFA54.DFA54_transition;
      }

      public override string Description => "()* loopback of 500:29: ( termwithoperator )*";

      public override void Error(NoViableAltException nvae)
      {
      }
    }

    private class DFA65 : DFA
    {
      private const string DFA65_eotS = "\n\uFFFF";
      private const string DFA65_eofS = "\n\uFFFF";
      private const string DFA65_minS = "\u0001\u0006\u0003\uFFFF\u0001\u0006\u0005\uFFFF";
      private const string DFA65_maxS = "\u0001c\u0003\uFFFF\u0001c\u0005\uFFFF";
      private const string DFA65_acceptS = "\u0001\uFFFF\u0001\u0001\u0001\u0002\u0001\u0003\u0001\uFFFF\u0001\u0005\u0001\u0006\u0001\a\u0001\b\u0001\u0004";
      private const string DFA65_specialS = "\n\uFFFF}>";
      private static readonly string[] DFA65_transitionS = new string[10]
      {
        "\u0001\u0001\u0019\uFFFF\u0001\u0001\u0001\b\u0004\uFFFF\u0001\u0006\u0002\uFFFF\u0001\u0004\a\uFFFF\u0001\u0001\u0003\uFFFF\u0001\u0001\u0001\u0003\u0001\b\b\uFFFF\u0001\u0001\u0004\uFFFF\u0001\u0001\u0001\uFFFF\u0001\u0001\u0003\uFFFF\u0001\u0001\u0001\a\u0001\u0001\u0003\uFFFF\u0001\u0001\u0003\uFFFF\u0001\u0005\u0004\uFFFF\u0001\u0001\u0001\b\a\uFFFF\u0001\u0002",
        "",
        "",
        "",
        "\u0001\t\u0005\uFFFF\u0001\b\u0001\t\u0002\uFFFF\u0001\t\u0002\uFFFF\u0001\t\b\uFFFF\u0001\t\u0002\uFFFF\u0003\t\u0004\uFFFF\u0001\t\u0002\uFFFF\u0003\t\u0005\uFFFF\u0001\t\u0003\uFFFF\u0003\t\b\uFFFF\u0001\t\u0004\uFFFF\u0001\t\u0001\uFFFF\u0001\t\u0003\uFFFF\u0003\t\u0001\uFFFF\u0001\t\u0001\uFFFF\u0001\t\u0002\uFFFF\u0002\t\u0004\uFFFF\u0002\t\a\uFFFF\u0001\t",
        "",
        "",
        "",
        "",
        ""
      };
      private static readonly short[] DFA65_eot = DFA.UnpackEncodedString("\n\uFFFF");
      private static readonly short[] DFA65_eof = DFA.UnpackEncodedString("\n\uFFFF");
      private static readonly char[] DFA65_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\u0006\u0003\uFFFF\u0001\u0006\u0005\uFFFF");
      private static readonly char[] DFA65_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001c\u0003\uFFFF\u0001c\u0005\uFFFF");
      private static readonly short[] DFA65_accept = DFA.UnpackEncodedString("\u0001\uFFFF\u0001\u0001\u0001\u0002\u0001\u0003\u0001\uFFFF\u0001\u0005\u0001\u0006\u0001\a\u0001\b\u0001\u0004");
      private static readonly short[] DFA65_special = DFA.UnpackEncodedString("\n\uFFFF}>");
      private static readonly short[][] DFA65_transition;

      static DFA65()
      {
        int length = CssParser.DFA65.DFA65_transitionS.Length;
        CssParser.DFA65.DFA65_transition = new short[length][];
        for (int index = 0; index < length; ++index)
          CssParser.DFA65.DFA65_transition[index] = DFA.UnpackEncodedString(CssParser.DFA65.DFA65_transitionS[index]);
      }

      public DFA65(BaseRecognizer recognizer)
      {
        this.recognizer = recognizer;
        this.decisionNumber = 65;
        this.eot = CssParser.DFA65.DFA65_eot;
        this.eof = CssParser.DFA65.DFA65_eof;
        this.min = CssParser.DFA65.DFA65_min;
        this.max = CssParser.DFA65.DFA65_max;
        this.accept = CssParser.DFA65.DFA65_accept;
        this.special = CssParser.DFA65.DFA65_special;
        this.transition = CssParser.DFA65.DFA65_transition;
      }

      public override string Description => "509:1: term : ( ( ( unary_operator )? (t= NUMBER |t= PERCENTAGE |t= LENGTH |t= RELATIVELENGTH |t= ANGLE |t= TIME |t= FREQ |t= RESOLUTION |t= SPEECH ) ) ( IMPORTANT_COMMENTS )* -> ^( TERM ( unary_operator )? ^( NUMBERBASEDVALUE $t) ( IMPORTANT_COMMENTS )* ) | URI ( IMPORTANT_COMMENTS )* -> ^( TERM ^( URIBASEDVALUE URI ) ( IMPORTANT_COMMENTS )* ) | (exp= ( MSIE_EXPRESSION ) ) ( IMPORTANT_COMMENTS )* -> ^( TERM ^( STRINGBASEDVALUE $exp) ( IMPORTANT_COMMENTS )* ) | IDENT ( IMPORTANT_COMMENTS )* -> ^( TERM ^( IDENTBASEDVALUE IDENT ) ( IMPORTANT_COMMENTS )* ) | STRING ( IMPORTANT_COMMENTS )* -> ^( TERM ^( STRINGBASEDVALUE STRING ) ( IMPORTANT_COMMENTS )* ) | hash ( IMPORTANT_COMMENTS )* -> ^( TERM ^( HEXBASEDVALUE hash ) ( IMPORTANT_COMMENTS )* ) | REPLACEMENTTOKEN -> ^( TERM ^( REPLACEMENTTOKENBASEDVALUE REPLACEMENTTOKEN ) ) | function ( IMPORTANT_COMMENTS )* -> ^( TERM function ( IMPORTANT_COMMENTS )* ) );";

      public override void Error(NoViableAltException nvae)
      {
      }
    }

    private static class Follow
    {
      public static readonly BitSet _styleSheet_in_main653 = new BitSet(new ulong[1]);
      public static readonly BitSet _EOF_in_main659 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _CHARSET_SYM_in_styleSheet683 = new BitSet(new ulong[2]
      {
        0UL,
        2097152UL
      });
      public static readonly BitSet _STRING_in_styleSheet685 = new BitSet(new ulong[2]
      {
        0UL,
        32768UL
      });
      public static readonly BitSet _SEMICOLON_in_styleSheet687 = new BitSet(new ulong[2]
      {
        292899177417982082UL,
        1099512942672UL
      });
      public static readonly BitSet _styleimport_in_styleSheet691 = new BitSet(new ulong[2]
      {
        292899177417982082UL,
        1099512942672UL
      });
      public static readonly BitSet _namespace_in_styleSheet694 = new BitSet(new ulong[2]
      {
        292881585231937666UL,
        1099512942672UL
      });
      public static readonly BitSet _styleSheetRulesOrComment_in_styleSheet697 = new BitSet(new ulong[2]
      {
        4651209080225922UL,
        1099512942672UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_styleSheetRulesOrComment756 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _styleSheetrules_in_styleSheetRulesOrComment764 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IMPORT_SYM_in_styleimport784 = new BitSet(new ulong[2]
      {
        0UL,
        34361835520UL
      });
      public static readonly BitSet _stringoruri_in_styleimport786 = new BitSet(new ulong[2]
      {
        9223374235878035456UL,
        32772UL
      });
      public static readonly BitSet _media_query_list_in_styleimport788 = new BitSet(new ulong[2]
      {
        0UL,
        32768UL
      });
      public static readonly BitSet _SEMICOLON_in_styleimport791 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _NAMESPACE_SYM_in_namespace826 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        34361835520UL
      });
      public static readonly BitSet _namespace_prefix_in_namespace828 = new BitSet(new ulong[2]
      {
        0UL,
        34361835520UL
      });
      public static readonly BitSet _stringoruri_in_namespace831 = new BitSet(new ulong[2]
      {
        0UL,
        32768UL
      });
      public static readonly BitSet _SEMICOLON_in_namespace833 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IDENT_in_namespace_prefix865 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _WG_DPI_SYM_in_wg_dpi894 = new BitSet(new ulong[2]
      {
        0UL,
        1UL
      });
      public static readonly BitSet _NUMBER_in_wg_dpi896 = new BitSet(new ulong[2]
      {
        0UL,
        32768UL
      });
      public static readonly BitSet _SEMICOLON_in_wg_dpi898 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _MEDIA_SYM_in_media930 = new BitSet(new ulong[2]
      {
        9223374235878297600UL,
        4UL
      });
      public static readonly BitSet _media_query_list_in_media932 = new BitSet(new ulong[1]
      {
        262144UL
      });
      public static readonly BitSet _CURLY_BEGIN_in_media935 = new BitSet(new ulong[2]
      {
        2473901736064UL,
        1314896UL
      });
      public static readonly BitSet _ruleset_in_media939 = new BitSet(new ulong[2]
      {
        2473901736064UL,
        1314896UL
      });
      public static readonly BitSet _page_in_media943 = new BitSet(new ulong[2]
      {
        2473901736064UL,
        1314896UL
      });
      public static readonly BitSet _CURLY_END_in_media948 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _media_query_in_media_query_list997 = new BitSet(new ulong[1]
      {
        65538UL
      });
      public static readonly BitSet _COMMA_in_media_query_list1000 = new BitSet(new ulong[2]
      {
        9223374235878035456UL,
        4UL
      });
      public static readonly BitSet _media_query_in_media_query_list1002 = new BitSet(new ulong[1]
      {
        65538UL
      });
      public static readonly BitSet _ONLY_in_media_query1036 = new BitSet(new ulong[2]
      {
        9223374235878031360UL,
        4UL
      });
      public static readonly BitSet _NOT_in_media_query1040 = new BitSet(new ulong[2]
      {
        9223374235878031360UL,
        4UL
      });
      public static readonly BitSet _media_type_in_media_query1044 = new BitSet(new ulong[1]
      {
        34UL
      });
      public static readonly BitSet _AND_in_media_query1047 = new BitSet(new ulong[2]
      {
        9223374235878035456UL,
        4UL
      });
      public static readonly BitSet _media_expression_in_media_query1049 = new BitSet(new ulong[1]
      {
        34UL
      });
      public static readonly BitSet _media_expression_in_media_query1087 = new BitSet(new ulong[1]
      {
        34UL
      });
      public static readonly BitSet _AND_in_media_query1090 = new BitSet(new ulong[2]
      {
        9223374235878035456UL,
        4UL
      });
      public static readonly BitSet _media_expression_in_media_query1092 = new BitSet(new ulong[1]
      {
        34UL
      });
      public static readonly BitSet _IDENT_in_media_type1122 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _CIRCLE_BEGIN_in_media_expression1145 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        4096UL
      });
      public static readonly BitSet _media_feature_in_media_expression1147 = new BitSet(new ulong[1]
      {
        40960UL
      });
      public static readonly BitSet _COLON_in_media_expression1150 = new BitSet(new ulong[2]
      {
        63620229569183808UL,
        34563307681UL
      });
      public static readonly BitSet _expr_in_media_expression1152 = new BitSet(new ulong[1]
      {
        8192UL
      });
      public static readonly BitSet _CIRCLE_END_in_media_expression1156 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IDENT_in_media_feature1183 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _REPLACEMENTTOKEN_in_media_feature1197 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _PAGE_SYM_in_page1224 = new BitSet(new ulong[1]
      {
        294912UL
      });
      public static readonly BitSet _pseudo_page_in_page1226 = new BitSet(new ulong[1]
      {
        262144UL
      });
      public static readonly BitSet _CURLY_BEGIN_in_page1229 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1052672UL
      });
      public static readonly BitSet _declaration_in_page1232 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1085440UL
      });
      public static readonly BitSet _SEMICOLON_in_page1234 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1052672UL
      });
      public static readonly BitSet _CURLY_END_in_page1239 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _COLON_in_pseudo_page1280 = new BitSet(new ulong[1]
      {
        2199023255552UL
      });
      public static readonly BitSet _IDENT_in_pseudo_page1282 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _set_in_operator1314 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _MINUS_in_unary_operator1349 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _PLUS_in_unary_operator1365 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _STAR_in_property1394 = new BitSet(new ulong[1]
      {
        2199023255552UL
      });
      public static readonly BitSet _IDENT_in_property1398 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_property1400 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _REPLACEMENTTOKEN_in_property1424 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _selectors_group_in_ruleset1454 = new BitSet(new ulong[1]
      {
        262144UL
      });
      public static readonly BitSet _CURLY_BEGIN_in_ruleset1460 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1052672UL
      });
      public static readonly BitSet _declaration_in_ruleset1467 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1085440UL
      });
      public static readonly BitSet _SEMICOLON_in_ruleset1469 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1052672UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_ruleset1475 = new BitSet(new ulong[1]
      {
        4398047035392UL
      });
      public static readonly BitSet _CURLY_END_in_ruleset1482 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _selector_in_selectors_group1523 = new BitSet(new ulong[1]
      {
        65538UL
      });
      public static readonly BitSet _COMMA_in_selectors_group1526 = new BitSet(new ulong[2]
      {
        2473901211776UL,
        1314880UL
      });
      public static readonly BitSet _selector_in_selectors_group1528 = new BitSet(new ulong[1]
      {
        65538UL
      });
      public static readonly BitSet _simple_selector_sequence_in_selector1559 = new BitSet(new ulong[2]
      {
        34359738370UL,
        2199056810112UL
      });
      public static readonly BitSet _combinator_simple_selector_sequence_in_selector1562 = new BitSet(new ulong[2]
      {
        34359738370UL,
        2199056810112UL
      });
      public static readonly BitSet _combinator_in_combinator_simple_selector_sequence1601 = new BitSet(new ulong[2]
      {
        2473901211776UL,
        1314880UL
      });
      public static readonly BitSet _simple_selector_sequence_in_combinator_simple_selector_sequence1603 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _PLUS_in_combinator1644 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _GREATER_in_combinator1655 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _TILDE_in_combinator1666 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _whitespace_in_combinator1687 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _WS_in_whitespace1728 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _universal_in_simple_selector_sequence1783 = new BitSet(new ulong[2]
      {
        2473901211776UL,
        2199024570432UL
      });
      public static readonly BitSet _type_selector_in_simple_selector_sequence1793 = new BitSet(new ulong[2]
      {
        2473901211776UL,
        2199024570432UL
      });
      public static readonly BitSet _whitespace_in_simple_selector_sequence1797 = new BitSet(new ulong[2]
      {
        2473901211778UL,
        1314880UL
      });
      public static readonly BitSet _hashclassatnameattribpseudonegation_in_simple_selector_sequence1806 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _hashclassatnameattribpseudonegation_in_simple_selector_sequence1848 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _REPLACEMENTTOKEN_in_hashclassatnameattribpseudonegation1878 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _hash_in_hashclassatnameattribpseudonegation1902 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _class_in_hashclassatnameattribpseudonegation1922 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _atname_in_hashclassatnameattribpseudonegation1942 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _attrib_in_hashclassatnameattribpseudonegation1962 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _pseudo_in_hashclassatnameattribpseudonegation1982 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _negation_in_hashclassatnameattribpseudonegation2002 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _selector_namespace_prefix_in_type_selector2047 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        1048576UL
      });
      public static readonly BitSet _element_name_in_type_selector2051 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _element_name_in_selector_namespace_prefix2085 = new BitSet(new ulong[2]
      {
        0UL,
        64UL
      });
      public static readonly BitSet _PIPE_in_selector_namespace_prefix2088 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IDENT_in_element_name2117 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _STAR_in_element_name2137 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _selector_namespace_prefix_in_universal2174 = new BitSet(new ulong[2]
      {
        0UL,
        1048576UL
      });
      public static readonly BitSet _STAR_in_universal2178 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _CLASS_IDENT_in_class2207 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _SQUARE_BEGIN_in_attrib2246 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        1048640UL
      });
      public static readonly BitSet _selector_namespace_prefix_in_attrib2257 = new BitSet(new ulong[1]
      {
        2199023255552UL
      });
      public static readonly BitSet _IDENT_in_attrib2262 = new BitSet(new ulong[2]
      {
        35184642621440UL,
        13107456UL
      });
      public static readonly BitSet _PREFIXMATCH_in_attrib2289 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        2097152UL
      });
      public static readonly BitSet _SUFFIXMATCH_in_attrib2293 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        2097152UL
      });
      public static readonly BitSet _SUBSTRINGMATCH_in_attrib2297 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        2097152UL
      });
      public static readonly BitSet _EQUALS_in_attrib2301 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        2097152UL
      });
      public static readonly BitSet _INCLUDES_in_attrib2305 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        2097152UL
      });
      public static readonly BitSet _DASHMATCH_in_attrib2309 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        2097152UL
      });
      public static readonly BitSet _IDENT_in_attrib2327 = new BitSet(new ulong[2]
      {
        0UL,
        524288UL
      });
      public static readonly BitSet _STRING_in_attrib2329 = new BitSet(new ulong[2]
      {
        0UL,
        524288UL
      });
      public static readonly BitSet _SQUARE_END_in_attrib2347 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _COLON_in_pseudo2420 = new BitSet(new ulong[1]
      {
        2199023288320UL
      });
      public static readonly BitSet _COLON_in_pseudo2424 = new BitSet(new ulong[1]
      {
        2199023255552UL
      });
      public static readonly BitSet _IDENT_in_pseudo2429 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _COLON_in_pseudo2467 = new BitSet(new ulong[2]
      {
        63620229569216576UL,
        34563307681UL
      });
      public static readonly BitSet _COLON_in_pseudo2471 = new BitSet(new ulong[2]
      {
        63620229569216576UL,
        34563307681UL
      });
      public static readonly BitSet _functional_pseudo_in_pseudo2474 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _beginfunc_in_functional_pseudo2515 = new BitSet(new ulong[3]
      {
        9009398286385152UL,
        2097281UL,
        1099511627776UL
      });
      public static readonly BitSet _selectorexpression_in_functional_pseudo2517 = new BitSet(new ulong[1]
      {
        8192UL
      });
      public static readonly BitSet _CIRCLE_END_in_functional_pseudo2519 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _set_in_selectorexpression2561 = new BitSet(new ulong[3]
      {
        9009398286385154UL,
        2097281UL,
        1099511627776UL
      });
      public static readonly BitSet _COLON_in_negation2594 = new BitSet(new ulong[1]
      {
        9223372036854775808UL
      });
      public static readonly BitSet _NOT_in_negation2596 = new BitSet(new ulong[1]
      {
        4096UL
      });
      public static readonly BitSet _CIRCLE_BEGIN_in_negation2598 = new BitSet(new ulong[2]
      {
        2473901211648UL,
        1310784UL
      });
      public static readonly BitSet _negation_arg_in_negation2601 = new BitSet(new ulong[1]
      {
        8192UL
      });
      public static readonly BitSet _CIRCLE_END_in_negation2603 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _universal_in_negation_arg2640 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _type_selector_in_negation_arg2643 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _hash_in_negation_arg2645 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _class_in_negation_arg2647 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _attrib_in_negation_arg2649 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _pseudo_in_negation_arg2651 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _AT_NAME_in_atname2666 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_declaration2698 = new BitSet(new ulong[2]
      {
        6597069766656UL,
        1052672UL
      });
      public static readonly BitSet _property_in_declaration2701 = new BitSet(new ulong[1]
      {
        32768UL
      });
      public static readonly BitSet _COLON_in_declaration2703 = new BitSet(new ulong[2]
      {
        63620229569183808UL,
        34563307681UL
      });
      public static readonly BitSet _expr_in_declaration2705 = new BitSet(new ulong[1]
      {
        8796093022210UL
      });
      public static readonly BitSet _prio_in_declaration2707 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _STRING_in_stringoruri2747 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _URI_in_stringoruri2767 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _ruleset_in_styleSheetrules2796 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _media_in_styleSheetrules2798 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _page_in_styleSheetrules2800 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _keyframes_in_styleSheetrules2802 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _document_in_styleSheetrules2804 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _wg_dpi_in_styleSheetrules2806 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IMPORTANT_SYM_in_prio2826 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_expr2856 = new BitSet(new ulong[2]
      {
        63620229569183808UL,
        34563307681UL
      });
      public static readonly BitSet _term_in_expr2859 = new BitSet(new ulong[2]
      {
        63620231985168450UL,
        34564356257UL
      });
      public static readonly BitSet _termwithoperator_in_expr2862 = new BitSet(new ulong[2]
      {
        63620231985168450UL,
        34564356257UL
      });
      public static readonly BitSet _operator_in_termwithoperator2902 = new BitSet(new ulong[2]
      {
        63620229569183808UL,
        34563307681UL
      });
      public static readonly BitSet _term_in_termwithoperator2905 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _unary_operator_in_term2943 = new BitSet(new ulong[2]
      {
        562954248388672UL,
        67250209UL
      });
      public static readonly BitSet _NUMBER_in_term2951 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _PERCENTAGE_in_term2959 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _LENGTH_in_term2967 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _RELATIVELENGTH_in_term2975 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _ANGLE_in_term2983 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _TIME_in_term2991 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _FREQ_in_term2999 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _RESOLUTION_in_term3007 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _SPEECH_in_term3015 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_term3020 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _URI_in_term3052 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_term3054 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _MSIE_EXPRESSION_in_term3088 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_term3093 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IDENT_in_term3122 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_term3124 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _STRING_in_term3152 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_term3154 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _hash_in_term3182 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_term3184 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _REPLACEMENTTOKEN_in_term3209 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _function_in_term3233 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _IMPORTANT_COMMENTS_in_term3235 = new BitSet(new ulong[1]
      {
        4398046511106UL
      });
      public static readonly BitSet _HASH_IDENT_in_hash3268 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _beginfunc_in_function3300 = new BitSet(new ulong[2]
      {
        63620229569192000UL,
        34563307681UL
      });
      public static readonly BitSet _expr_in_function3302 = new BitSet(new ulong[1]
      {
        8192UL
      });
      public static readonly BitSet _CIRCLE_END_in_function3305 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _IDENT_in_beginfunc3337 = new BitSet(new ulong[1]
      {
        4096UL
      });
      public static readonly BitSet _CIRCLE_BEGIN_in_beginfunc3339 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _FROM_in_beginfunc3361 = new BitSet(new ulong[1]
      {
        4096UL
      });
      public static readonly BitSet _CIRCLE_BEGIN_in_beginfunc3363 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _TO_in_beginfunc3383 = new BitSet(new ulong[1]
      {
        4096UL
      });
      public static readonly BitSet _CIRCLE_BEGIN_in_beginfunc3385 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _MSIE_IMAGE_TRANSFORM_in_beginfunc3406 = new BitSet(new ulong[1]
      {
        4096UL
      });
      public static readonly BitSet _CIRCLE_BEGIN_in_beginfunc3408 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _KEYFRAMES_SYM_in_keyframes3438 = new BitSet(new ulong[2]
      {
        2199023255552UL,
        2097152UL
      });
      public static readonly BitSet _IDENT_in_keyframes3441 = new BitSet(new ulong[1]
      {
        262144UL
      });
      public static readonly BitSet _STRING_in_keyframes3443 = new BitSet(new ulong[1]
      {
        262144UL
      });
      public static readonly BitSet _CURLY_BEGIN_in_keyframes3446 = new BitSet(new ulong[2]
      {
        8590458880UL,
        134217760UL
      });
      public static readonly BitSet _keyframes_block_in_keyframes3448 = new BitSet(new ulong[2]
      {
        8590458880UL,
        134217760UL
      });
      public static readonly BitSet _CURLY_END_in_keyframes3451 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _keyframes_selectors_in_keyframes_block3507 = new BitSet(new ulong[1]
      {
        262144UL
      });
      public static readonly BitSet _CURLY_BEGIN_in_keyframes_block3509 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1052672UL
      });
      public static readonly BitSet _declaration_in_keyframes_block3512 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1085440UL
      });
      public static readonly BitSet _SEMICOLON_in_keyframes_block3514 = new BitSet(new ulong[2]
      {
        6597070290944UL,
        1052672UL
      });
      public static readonly BitSet _CURLY_END_in_keyframes_block3519 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _keyframes_selector_in_keyframes_selectors3561 = new BitSet(new ulong[1]
      {
        65538UL
      });
      public static readonly BitSet _COMMA_in_keyframes_selectors3564 = new BitSet(new ulong[2]
      {
        8589934592UL,
        134217760UL
      });
      public static readonly BitSet _keyframes_selector_in_keyframes_selectors3566 = new BitSet(new ulong[1]
      {
        65538UL
      });
      public static readonly BitSet _set_in_keyframes_selector3596 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _DOCUMENT_SYM_in_document3619 = new BitSet(new ulong[2]
      {
        33554432UL,
        171798709248UL
      });
      public static readonly BitSet _S_in_document3621 = new BitSet(new ulong[2]
      {
        33554432UL,
        171798709248UL
      });
      public static readonly BitSet _document_match_function_in_document3624 = new BitSet(new ulong[2]
      {
        262144UL,
        16384UL
      });
      public static readonly BitSet _S_in_document3626 = new BitSet(new ulong[2]
      {
        262144UL,
        16384UL
      });
      public static readonly BitSet _CURLY_BEGIN_in_document3629 = new BitSet(new ulong[2]
      {
        2473901736064UL,
        1314880UL
      });
      public static readonly BitSet _ruleset_in_document3631 = new BitSet(new ulong[2]
      {
        2473901736064UL,
        1314880UL
      });
      public static readonly BitSet _CURLY_END_in_document3634 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _URI_in_document_match_function3678 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _URLPREFIX_FUNCTION_in_document_match_function3699 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _DOMAIN_FUNCTION_in_document_match_function3720 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _REGEXP_FUNCTION_in_document_match_function3740 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _WS_in_synpred1_CssParser1723 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _universal_in_synpred2_CssParser1778 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _type_selector_in_synpred3_CssParser1788 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _hashclassatnameattribpseudonegation_in_synpred4_CssParser1801 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _hashclassatnameattribpseudonegation_in_synpred5_CssParser1843 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _selector_namespace_prefix_in_synpred6_CssParser2042 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _selector_namespace_prefix_in_synpred7_CssParser2169 = new BitSet(new ulong[1]
      {
        2UL
      });
      public static readonly BitSet _universal_in_synpred8_CssParser2635 = new BitSet(new ulong[1]
      {
        2UL
      });
    }
  }
}
