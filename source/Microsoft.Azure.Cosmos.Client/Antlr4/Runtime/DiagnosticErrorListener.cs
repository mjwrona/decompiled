// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.DiagnosticErrorListener
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;

namespace Antlr4.Runtime
{
  internal class DiagnosticErrorListener : BaseErrorListener
  {
    protected internal readonly bool exactOnly;

    public DiagnosticErrorListener()
      : this(true)
    {
    }

    public DiagnosticErrorListener(bool exactOnly) => this.exactOnly = exactOnly;

    public override void ReportAmbiguity(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      bool exact,
      BitSet ambigAlts,
      ATNConfigSet configs)
    {
      if (this.exactOnly && !exact)
        return;
      string msg = string.Format("reportAmbiguity d={0}: ambigAlts={1}, input='{2}'", (object) this.GetDecisionDescription(recognizer, dfa), (object) this.GetConflictingAlts(ambigAlts, configs), (object) ((ITokenStream) recognizer.InputStream).GetText(Interval.Of(startIndex, stopIndex)));
      recognizer.NotifyErrorListeners(msg);
    }

    public override void ReportAttemptingFullContext(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      BitSet conflictingAlts,
      SimulatorState conflictState)
    {
      string msg = string.Format("reportAttemptingFullContext d={0}, input='{1}'", (object) this.GetDecisionDescription(recognizer, dfa), (object) ((ITokenStream) recognizer.InputStream).GetText(Interval.Of(startIndex, stopIndex)));
      recognizer.NotifyErrorListeners(msg);
    }

    public override void ReportContextSensitivity(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      int prediction,
      SimulatorState acceptState)
    {
      string msg = string.Format("reportContextSensitivity d={0}, input='{1}'", (object) this.GetDecisionDescription(recognizer, dfa), (object) ((ITokenStream) recognizer.InputStream).GetText(Interval.Of(startIndex, stopIndex)));
      recognizer.NotifyErrorListeners(msg);
    }

    protected internal virtual string GetDecisionDescription(Parser recognizer, DFA dfa)
    {
      int decision = dfa.decision;
      int ruleIndex = dfa.atnStartState.ruleIndex;
      string[] ruleNames = recognizer.RuleNames;
      if (ruleIndex < 0 || ruleIndex >= ruleNames.Length)
        return decision.ToString();
      string str = ruleNames[ruleIndex];
      return string.IsNullOrEmpty(str) ? decision.ToString() : string.Format("{0} ({1})", (object) decision, (object) str);
    }

    [return: NotNull]
    protected internal virtual BitSet GetConflictingAlts(
      BitSet reportedAlts,
      ATNConfigSet configSet)
    {
      if (reportedAlts != null)
        return reportedAlts;
      BitSet conflictingAlts = new BitSet();
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
        conflictingAlts.Set(config.alt);
      return conflictingAlts;
    }
  }
}
