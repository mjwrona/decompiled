// Decompiled with JetBrains decompiler
// Type: XPathLexer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using System.CodeDom.Compiler;
using System.Text;

[GeneratedCode("ANTLR", "4.8")]
internal class XPathLexer : Lexer
{
  protected static DFA[] decisionToDFA;
  protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
  public const int TokenRef = 1;
  public const int RuleRef = 2;
  public const int Anywhere = 3;
  public const int Root = 4;
  public const int Wildcard = 5;
  public const int Bang = 6;
  public const int ID = 7;
  public const int String = 8;
  public static string[] channelNames = new string[2]
  {
    "DEFAULT_TOKEN_CHANNEL",
    "HIDDEN"
  };
  public static string[] modeNames = new string[1]
  {
    "DEFAULT_MODE"
  };
  public static readonly string[] ruleNames = new string[8]
  {
    nameof (Anywhere),
    nameof (Root),
    nameof (Wildcard),
    nameof (Bang),
    nameof (ID),
    "NameChar",
    "NameStartChar",
    nameof (String)
  };
  private static readonly string[] _LiteralNames = new string[7]
  {
    null,
    null,
    null,
    "'//'",
    "'/'",
    "'*'",
    "'!'"
  };
  private static readonly string[] _SymbolicNames = new string[9]
  {
    null,
    nameof (TokenRef),
    nameof (RuleRef),
    nameof (Anywhere),
    nameof (Root),
    nameof (Wildcard),
    nameof (Bang),
    nameof (ID),
    nameof (String)
  };
  public static readonly IVocabulary DefaultVocabulary = (IVocabulary) new Antlr4.Runtime.Vocabulary(XPathLexer._LiteralNames, XPathLexer._SymbolicNames);
  private static string _serializedATN = XPathLexer._serializeATN();
  public static readonly ATN _ATN = new ATNDeserializer().Deserialize(XPathLexer._serializedATN.ToCharArray());

  public XPathLexer(ICharStream input)
    : base(input)
  {
    this.Interpreter = new LexerATNSimulator((Lexer) this, XPathLexer._ATN, XPathLexer.decisionToDFA, XPathLexer.sharedContextCache);
  }

  [NotNull]
  public override IVocabulary Vocabulary => XPathLexer.DefaultVocabulary;

  public override string GrammarFileName => "XPathLexer.g4";

  public override string[] RuleNames => XPathLexer.ruleNames;

  public override string[] ChannelNames => XPathLexer.channelNames;

  public override string[] ModeNames => XPathLexer.modeNames;

  public override string SerializedAtn => XPathLexer._serializedATN;

  static XPathLexer()
  {
    XPathLexer.decisionToDFA = new DFA[XPathLexer._ATN.NumberOfDecisions];
    for (int decision = 0; decision < XPathLexer._ATN.NumberOfDecisions; ++decision)
      XPathLexer.decisionToDFA[decision] = new DFA(XPathLexer._ATN.GetDecisionState(decision), decision);
  }

  public override void Action(RuleContext _localctx, int ruleIndex, int actionIndex)
  {
    if (ruleIndex != 4)
      return;
    this.ID_action(_localctx, actionIndex);
  }

  private void ID_action(RuleContext _localctx, int actionIndex)
  {
    if (actionIndex != 0)
      return;
    if (char.IsUpper(this.Text[0]))
      this.Type = 1;
    else
      this.Type = 2;
  }

  private static string _serializeATN()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("\u0003а훑舆괭䐗껱趀ꫝ\u0002\n4");
    stringBuilder.Append("\b\u0001\u0004\u0002\t\u0002\u0004\u0003\t\u0003\u0004\u0004\t\u0004\u0004\u0005\t\u0005\u0004\u0006\t\u0006");
    stringBuilder.Append("\u0004\a\t\a\u0004\b\t\b\u0004\t\t\t\u0003\u0002\u0003\u0002\u0003\u0002\u0003\u0003\u0003\u0003\u0003");
    stringBuilder.Append("\u0004\u0003\u0004\u0003\u0005\u0003\u0005\u0003\u0006\u0003\u0006\a\u0006\u001F\n\u0006\f\u0006\u000E\u0006\"");
    stringBuilder.Append("\v\u0006\u0003\u0006\u0003\u0006\u0003\a\u0003\a\u0005\a(\n\a\u0003\b\u0003\b\u0003\t\u0003\t\a");
    stringBuilder.Append("\t.\n\t\f\t\u000E\t1\v\t\u0003\t\u0003\t\u0003/\u0002\n\u0003\u0005\u0005\u0006\a\a");
    stringBuilder.Append("\t\b\v\t\r\u0002\u000F\u0002\u0011\n\u0003\u0002\u0004\a\u00022;aa\u00B9\u00B9");
    stringBuilder.Append("̂ͱ⁁⁂\u000F\u0002C\\c|ÂØÚøú");
    stringBuilder.Append("́ͲͿ\u0381 \u200E\u200F\u2072↑Ⰲ⿱");
    stringBuilder.Append("〃\uD801車\uFDD1ﷲ\uFFFF4\u0002\u0003\u0003\u0002\u0002\u0002\u0002");
    stringBuilder.Append("\u0005\u0003\u0002\u0002\u0002\u0002\a\u0003\u0002\u0002\u0002\u0002\t\u0003\u0002\u0002\u0002\u0002\v\u0003\u0002");
    stringBuilder.Append("\u0002\u0002\u0002\u0011\u0003\u0002\u0002\u0002\u0003\u0013\u0003\u0002\u0002\u0002\u0005\u0016\u0003\u0002\u0002");
    stringBuilder.Append("\u0002\a\u0018\u0003\u0002\u0002\u0002\t\u001A\u0003\u0002\u0002\u0002\v\u001C\u0003\u0002\u0002\u0002\r");
    stringBuilder.Append("'\u0003\u0002\u0002\u0002\u000F)\u0003\u0002\u0002\u0002\u0011+\u0003\u0002\u0002\u0002\u0013\u0014\a1");
    stringBuilder.Append("\u0002\u0002\u0014\u0015\a1\u0002\u0002\u0015\u0004\u0003\u0002\u0002\u0002\u0016\u0017\a1");
    stringBuilder.Append("\u0002\u0002\u0017\u0006\u0003\u0002\u0002\u0002\u0018\u0019\a,\u0002\u0002\u0019\b\u0003\u0002\u0002\u0002");
    stringBuilder.Append("\u001A\u001B\a#\u0002\u0002\u001B\n\u0003\u0002\u0002\u0002\u001C \u0005\u000F\b\u0002\u001D\u001F");
    stringBuilder.Append("\u0005\r\a\u0002\u001E\u001D\u0003\u0002\u0002\u0002\u001F\"\u0003\u0002\u0002\u0002 \u001E\u0003\u0002");
    stringBuilder.Append("\u0002\u0002 !\u0003\u0002\u0002\u0002!#\u0003\u0002\u0002\u0002\" \u0003\u0002\u0002\u0002#$\b\u0006\u0002");
    stringBuilder.Append("\u0002$\f\u0003\u0002\u0002\u0002%(\u0005\u000F\b\u0002&(\t\u0002\u0002\u0002'%\u0003\u0002\u0002\u0002");
    stringBuilder.Append("'&\u0003\u0002\u0002\u0002(\u000E\u0003\u0002\u0002\u0002)*\t\u0003\u0002\u0002*\u0010\u0003\u0002\u0002\u0002");
    stringBuilder.Append("+/\a)\u0002\u0002,.\v\u0002\u0002\u0002-,\u0003\u0002\u0002\u0002.1\u0003\u0002\u0002\u0002/0");
    stringBuilder.Append("\u0003\u0002\u0002\u0002/-\u0003\u0002\u0002\u000202\u0003\u0002\u0002\u00021/\u0003\u0002\u0002");
    stringBuilder.Append("\u000223\a)\u0002\u00023\u0012\u0003\u0002\u0002\u0002\u0006\u0002 '/\u0003\u0003\u0006");
    stringBuilder.Append("\u0002");
    return stringBuilder.ToString();
  }
}
