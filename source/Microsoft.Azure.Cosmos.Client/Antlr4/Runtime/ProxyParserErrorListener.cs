// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.ProxyParserErrorListener
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;

namespace Antlr4.Runtime
{
  internal class ProxyParserErrorListener : 
    ProxyErrorListener<IToken>,
    IParserErrorListener,
    IAntlrErrorListener<IToken>
  {
    public ProxyParserErrorListener(ICollection<IAntlrErrorListener<IToken>> delegates)
      : base((IEnumerable<IAntlrErrorListener<IToken>>) delegates)
    {
    }

    public virtual void ReportAmbiguity(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      bool exact,
      BitSet ambigAlts,
      ATNConfigSet configs)
    {
      foreach (IAntlrErrorListener<IToken> antlrErrorListener in this.Delegates)
      {
        if (antlrErrorListener is IParserErrorListener)
          ((IParserErrorListener) antlrErrorListener).ReportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs);
      }
    }

    public virtual void ReportAttemptingFullContext(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      BitSet conflictingAlts,
      SimulatorState conflictState)
    {
      foreach (IAntlrErrorListener<IToken> antlrErrorListener in this.Delegates)
      {
        if (antlrErrorListener is IParserErrorListener)
          ((IParserErrorListener) antlrErrorListener).ReportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, conflictState);
      }
    }

    public virtual void ReportContextSensitivity(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      int prediction,
      SimulatorState acceptState)
    {
      foreach (IAntlrErrorListener<IToken> antlrErrorListener in this.Delegates)
      {
        if (antlrErrorListener is IParserErrorListener)
          ((IParserErrorListener) antlrErrorListener).ReportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, acceptState);
      }
    }
  }
}
