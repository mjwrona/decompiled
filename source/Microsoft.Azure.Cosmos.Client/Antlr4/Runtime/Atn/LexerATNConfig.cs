// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerATNConfig
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal class LexerATNConfig : ATNConfig
  {
    private readonly LexerActionExecutor lexerActionExecutor;
    private readonly bool passedThroughNonGreedyDecision;

    public LexerATNConfig(ATNState state, int alt, PredictionContext context)
      : base(state, alt, context)
    {
      this.passedThroughNonGreedyDecision = false;
      this.lexerActionExecutor = (LexerActionExecutor) null;
    }

    public LexerATNConfig(
      ATNState state,
      int alt,
      PredictionContext context,
      LexerActionExecutor lexerActionExecutor)
      : base(state, alt, context, SemanticContext.NONE)
    {
      this.lexerActionExecutor = lexerActionExecutor;
      this.passedThroughNonGreedyDecision = false;
    }

    public LexerATNConfig(LexerATNConfig c, ATNState state)
      : base((ATNConfig) c, state, c.context, c.semanticContext)
    {
      this.lexerActionExecutor = c.lexerActionExecutor;
      this.passedThroughNonGreedyDecision = LexerATNConfig.checkNonGreedyDecision(c, state);
    }

    public LexerATNConfig(
      LexerATNConfig c,
      ATNState state,
      LexerActionExecutor lexerActionExecutor)
      : base((ATNConfig) c, state, c.context, c.semanticContext)
    {
      this.lexerActionExecutor = lexerActionExecutor;
      this.passedThroughNonGreedyDecision = LexerATNConfig.checkNonGreedyDecision(c, state);
    }

    public LexerATNConfig(LexerATNConfig c, ATNState state, PredictionContext context)
      : base((ATNConfig) c, state, context, c.semanticContext)
    {
      this.lexerActionExecutor = c.lexerActionExecutor;
      this.passedThroughNonGreedyDecision = LexerATNConfig.checkNonGreedyDecision(c, state);
    }

    public LexerActionExecutor getLexerActionExecutor() => this.lexerActionExecutor;

    public bool hasPassedThroughNonGreedyDecision() => this.passedThroughNonGreedyDecision;

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(7), this.state.stateNumber), this.alt), (object) this.context), (object) this.semanticContext), this.passedThroughNonGreedyDecision ? 1 : 0), (object) this.lexerActionExecutor), 6);

    public override bool Equals(ATNConfig other)
    {
      if (this == other)
        return true;
      if (!(other is LexerATNConfig))
        return false;
      LexerATNConfig lexerAtnConfig = (LexerATNConfig) other;
      return this.passedThroughNonGreedyDecision == lexerAtnConfig.passedThroughNonGreedyDecision && (this.lexerActionExecutor == null ? (lexerAtnConfig.lexerActionExecutor == null ? 1 : 0) : (this.lexerActionExecutor.Equals((object) lexerAtnConfig.lexerActionExecutor) ? 1 : 0)) != 0 && base.Equals(other);
    }

    private static bool checkNonGreedyDecision(LexerATNConfig source, ATNState target)
    {
      if (source.passedThroughNonGreedyDecision)
        return true;
      return target is DecisionState && ((DecisionState) target).nonGreedy;
    }
  }
}
