// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ATNConfig
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System.Text;

namespace Antlr4.Runtime.Atn
{
  internal class ATNConfig
  {
    private static readonly int SUPPRESS_PRECEDENCE_FILTER = 1073741824;
    public readonly ATNState state;
    public readonly int alt;
    public PredictionContext context;
    public int reachesIntoOuterContext;
    public readonly SemanticContext semanticContext;

    public ATNConfig(ATNConfig old)
    {
      this.state = old.state;
      this.alt = old.alt;
      this.context = old.context;
      this.semanticContext = old.semanticContext;
      this.reachesIntoOuterContext = old.reachesIntoOuterContext;
    }

    public ATNConfig(ATNState state, int alt, PredictionContext context)
      : this(state, alt, context, SemanticContext.NONE)
    {
    }

    public ATNConfig(
      ATNState state,
      int alt,
      PredictionContext context,
      SemanticContext semanticContext)
    {
      this.state = state;
      this.alt = alt;
      this.context = context;
      this.semanticContext = semanticContext;
    }

    public ATNConfig(ATNConfig c, ATNState state)
      : this(c, state, c.context, c.semanticContext)
    {
    }

    public ATNConfig(ATNConfig c, ATNState state, SemanticContext semanticContext)
      : this(c, state, c.context, semanticContext)
    {
    }

    public ATNConfig(ATNConfig c, SemanticContext semanticContext)
      : this(c, c.state, c.context, semanticContext)
    {
    }

    public ATNConfig(ATNConfig c, ATNState state, PredictionContext context)
      : this(c, state, context, c.semanticContext)
    {
    }

    public ATNConfig(
      ATNConfig c,
      ATNState state,
      PredictionContext context,
      SemanticContext semanticContext)
    {
      this.state = state;
      this.alt = c.alt;
      this.context = context;
      this.semanticContext = semanticContext;
      this.reachesIntoOuterContext = c.reachesIntoOuterContext;
    }

    public int OuterContextDepth => this.reachesIntoOuterContext & ~ATNConfig.SUPPRESS_PRECEDENCE_FILTER;

    public bool IsPrecedenceFilterSuppressed => (this.reachesIntoOuterContext & ATNConfig.SUPPRESS_PRECEDENCE_FILTER) != 0;

    public void SetPrecedenceFilterSuppressed(bool value)
    {
      if (value)
        this.reachesIntoOuterContext |= ATNConfig.SUPPRESS_PRECEDENCE_FILTER;
      else
        this.reachesIntoOuterContext &= ~ATNConfig.SUPPRESS_PRECEDENCE_FILTER;
    }

    public override bool Equals(object o) => o is ATNConfig && this.Equals((ATNConfig) o);

    public virtual bool Equals(ATNConfig other)
    {
      if (this == other)
        return true;
      return other != null && this.state.stateNumber == other.state.stateNumber && this.alt == other.alt && (this.context == other.context || this.context != null && this.context.Equals((object) other.context)) && this.semanticContext.Equals((object) other.semanticContext) && this.IsPrecedenceFilterSuppressed == other.IsPrecedenceFilterSuppressed;
    }

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(7), this.state.stateNumber), this.alt), (object) this.context), (object) this.semanticContext), 4);

    public override string ToString() => this.ToString((IRecognizer) null, true);

    public string ToString(IRecognizer recog, bool showAlt)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('(');
      stringBuilder.Append((object) this.state);
      if (showAlt)
      {
        stringBuilder.Append(",");
        stringBuilder.Append(this.alt);
      }
      if (this.context != null)
      {
        stringBuilder.Append(",[");
        stringBuilder.Append(this.context.ToString());
        stringBuilder.Append("]");
      }
      if (this.semanticContext != null && this.semanticContext != SemanticContext.NONE)
      {
        stringBuilder.Append(",");
        stringBuilder.Append((object) this.semanticContext);
      }
      if (this.OuterContextDepth > 0)
        stringBuilder.Append(",up=").Append(this.OuterContextDepth);
      stringBuilder.Append(')');
      return stringBuilder.ToString();
    }
  }
}
