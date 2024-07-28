// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.FailedPredicateException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;
using System.Globalization;

namespace Antlr4.Runtime
{
  [Serializable]
  internal class FailedPredicateException : RecognitionException
  {
    private const long serialVersionUID = 5379330841495778709;
    private readonly int ruleIndex;
    private readonly int predicateIndex;
    private readonly string predicate;

    public FailedPredicateException(Parser recognizer)
      : this(recognizer, (string) null)
    {
    }

    public FailedPredicateException(Parser recognizer, string predicate)
      : this(recognizer, predicate, (string) null)
    {
    }

    public FailedPredicateException(Parser recognizer, string predicate, string message)
      : base(FailedPredicateException.FormatMessage(predicate, message), (IRecognizer) recognizer, recognizer.InputStream, recognizer.RuleContext)
    {
      AbstractPredicateTransition predicateTransition = (AbstractPredicateTransition) recognizer.Interpreter.atn.states[recognizer.State].Transition(0);
      if (predicateTransition is PredicateTransition)
      {
        this.ruleIndex = ((PredicateTransition) predicateTransition).ruleIndex;
        this.predicateIndex = ((PredicateTransition) predicateTransition).predIndex;
      }
      else
      {
        this.ruleIndex = 0;
        this.predicateIndex = 0;
      }
      this.predicate = predicate;
      this.OffendingToken = recognizer.CurrentToken;
    }

    public virtual int RuleIndex => this.ruleIndex;

    public virtual int PredIndex => this.predicateIndex;

    [Nullable]
    public virtual string Predicate => this.predicate;

    [return: NotNull]
    private static string FormatMessage(string predicate, string message) => message != null ? message : string.Format((IFormatProvider) CultureInfo.CurrentCulture, "failed predicate: {{{0}}}?", (object) predicate);
  }
}
