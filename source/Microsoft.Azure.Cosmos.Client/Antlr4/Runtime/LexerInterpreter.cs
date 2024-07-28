// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.LexerInterpreter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antlr4.Runtime
{
  internal class LexerInterpreter : Lexer
  {
    private readonly string grammarFileName;
    private readonly ATN atn;
    private readonly string[] ruleNames;
    private readonly string[] channelNames;
    private readonly string[] modeNames;
    [NotNull]
    private readonly IVocabulary vocabulary;
    protected DFA[] decisionToDFA;
    protected PredictionContextCache sharedContextCache = new PredictionContextCache();

    [Obsolete("Use constructor with channelNames argument")]
    public LexerInterpreter(
      string grammarFileName,
      IVocabulary vocabulary,
      IEnumerable<string> ruleNames,
      IEnumerable<string> modeNames,
      ATN atn,
      ICharStream input)
      : this(grammarFileName, vocabulary, ruleNames, (IEnumerable<string>) new string[0], modeNames, atn, input)
    {
    }

    public LexerInterpreter(
      string grammarFileName,
      IVocabulary vocabulary,
      IEnumerable<string> ruleNames,
      IEnumerable<string> channelNames,
      IEnumerable<string> modeNames,
      ATN atn,
      ICharStream input)
      : base(input)
    {
      if (atn.grammarType != ATNType.Lexer)
        throw new ArgumentException("The ATN must be a lexer ATN.");
      this.grammarFileName = grammarFileName;
      this.atn = atn;
      this.ruleNames = ruleNames.ToArray<string>();
      this.channelNames = channelNames.ToArray<string>();
      this.modeNames = modeNames.ToArray<string>();
      this.vocabulary = vocabulary;
      this.decisionToDFA = new DFA[atn.NumberOfDecisions];
      for (int decision = 0; decision < this.decisionToDFA.Length; ++decision)
        this.decisionToDFA[decision] = new DFA(atn.GetDecisionState(decision), decision);
      this.Interpreter = new LexerATNSimulator((Lexer) this, atn, this.decisionToDFA, this.sharedContextCache);
    }

    public override ATN Atn => this.atn;

    public override string GrammarFileName => this.grammarFileName;

    public override string[] RuleNames => this.ruleNames;

    public override string[] ChannelNames => this.channelNames;

    public override string[] ModeNames => this.modeNames;

    public override IVocabulary Vocabulary => this.vocabulary;
  }
}
