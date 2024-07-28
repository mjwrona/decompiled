// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.BaseErrorListener
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using System.IO;

namespace Antlr4.Runtime
{
  internal class BaseErrorListener : IParserErrorListener, IAntlrErrorListener<IToken>
  {
    public virtual void SyntaxError(
      TextWriter output,
      IRecognizer recognizer,
      IToken offendingSymbol,
      int line,
      int charPositionInLine,
      string msg,
      RecognitionException e)
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
    }

    public virtual void ReportAttemptingFullContext(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      BitSet conflictingAlts,
      SimulatorState conflictState)
    {
    }

    public virtual void ReportContextSensitivity(
      Parser recognizer,
      DFA dfa,
      int startIndex,
      int stopIndex,
      int prediction,
      SimulatorState acceptState)
    {
    }
  }
}
