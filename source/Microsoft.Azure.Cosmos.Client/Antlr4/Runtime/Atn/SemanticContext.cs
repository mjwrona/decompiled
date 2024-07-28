// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.SemanticContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antlr4.Runtime.Atn
{
  internal abstract class SemanticContext
  {
    public static readonly SemanticContext NONE = (SemanticContext) new SemanticContext.Predicate();

    public abstract bool Eval<Symbol, ATNInterpreter>(
      Recognizer<Symbol, ATNInterpreter> parser,
      RuleContext parserCallStack)
      where ATNInterpreter : ATNSimulator;

    public virtual SemanticContext EvalPrecedence<Symbol, ATNInterpreter>(
      Recognizer<Symbol, ATNInterpreter> parser,
      RuleContext parserCallStack)
      where ATNInterpreter : ATNSimulator
    {
      return this;
    }

    public static SemanticContext AndOp(SemanticContext a, SemanticContext b)
    {
      if (a == null || a == SemanticContext.NONE)
        return b;
      if (b == null || b == SemanticContext.NONE)
        return a;
      SemanticContext.AND and = new SemanticContext.AND(a, b);
      return and.opnds.Length == 1 ? and.opnds[0] : (SemanticContext) and;
    }

    public static SemanticContext OrOp(SemanticContext a, SemanticContext b)
    {
      if (a == null)
        return b;
      if (b == null)
        return a;
      if (a == SemanticContext.NONE || b == SemanticContext.NONE)
        return SemanticContext.NONE;
      SemanticContext.OR or = new SemanticContext.OR(a, b);
      return or.opnds.Length == 1 ? or.opnds[0] : (SemanticContext) or;
    }

    private static IList<SemanticContext.PrecedencePredicate> FilterPrecedencePredicates(
      HashSet<SemanticContext> collection)
    {
      if (!collection.OfType<SemanticContext.PrecedencePredicate>().Any<SemanticContext.PrecedencePredicate>())
        Antlr4.Runtime.Sharpen.Collections.EmptyList<SemanticContext.PrecedencePredicate>();
      List<SemanticContext.PrecedencePredicate> list = collection.OfType<SemanticContext.PrecedencePredicate>().ToList<SemanticContext.PrecedencePredicate>();
      collection.ExceptWith(list.Cast<SemanticContext>());
      return (IList<SemanticContext.PrecedencePredicate>) list;
    }

    internal class Predicate : SemanticContext
    {
      public readonly int ruleIndex;
      public readonly int predIndex;
      public readonly bool isCtxDependent;

      protected internal Predicate()
      {
        this.ruleIndex = -1;
        this.predIndex = -1;
        this.isCtxDependent = false;
      }

      public Predicate(int ruleIndex, int predIndex, bool isCtxDependent)
      {
        this.ruleIndex = ruleIndex;
        this.predIndex = predIndex;
        this.isCtxDependent = isCtxDependent;
      }

      public override bool Eval<Symbol, ATNInterpreter>(
        Recognizer<Symbol, ATNInterpreter> parser,
        RuleContext parserCallStack)
      {
        RuleContext _localctx = this.isCtxDependent ? parserCallStack : (RuleContext) null;
        return parser.Sempred(_localctx, this.ruleIndex, this.predIndex);
      }

      public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(), this.ruleIndex), this.predIndex), this.isCtxDependent ? 1 : 0), 3);

      public override bool Equals(object obj)
      {
        if (!(obj is SemanticContext.Predicate))
          return false;
        if (this == obj)
          return true;
        SemanticContext.Predicate predicate = (SemanticContext.Predicate) obj;
        return this.ruleIndex == predicate.ruleIndex && this.predIndex == predicate.predIndex && this.isCtxDependent == predicate.isCtxDependent;
      }

      public override string ToString() => "{" + this.ruleIndex.ToString() + ":" + this.predIndex.ToString() + "}?";
    }

    internal class PrecedencePredicate : 
      SemanticContext,
      IComparable<SemanticContext.PrecedencePredicate>
    {
      public readonly int precedence;

      protected internal PrecedencePredicate() => this.precedence = 0;

      public PrecedencePredicate(int precedence) => this.precedence = precedence;

      public override bool Eval<Symbol, ATNInterpreter>(
        Recognizer<Symbol, ATNInterpreter> parser,
        RuleContext parserCallStack)
      {
        return parser.Precpred(parserCallStack, this.precedence);
      }

      public override SemanticContext EvalPrecedence<Symbol, ATNInterpreter>(
        Recognizer<Symbol, ATNInterpreter> parser,
        RuleContext parserCallStack)
      {
        return parser.Precpred(parserCallStack, this.precedence) ? SemanticContext.NONE : (SemanticContext) null;
      }

      public virtual int CompareTo(SemanticContext.PrecedencePredicate o) => this.precedence - o.precedence;

      public override int GetHashCode() => 31 * 1 + this.precedence;

      public override bool Equals(object obj)
      {
        if (!(obj is SemanticContext.PrecedencePredicate))
          return false;
        return this == obj || this.precedence == ((SemanticContext.PrecedencePredicate) obj).precedence;
      }

      public override string ToString() => "{" + this.precedence.ToString() + ">=prec}?";
    }

    internal abstract class Operator : SemanticContext
    {
      [NotNull]
      public abstract ICollection<SemanticContext> Operands { get; }
    }

    internal class AND : SemanticContext.Operator
    {
      [NotNull]
      public readonly SemanticContext[] opnds;

      public AND(SemanticContext a, SemanticContext b)
      {
        HashSet<SemanticContext> semanticContextSet = new HashSet<SemanticContext>();
        if (a is SemanticContext.AND)
          semanticContextSet.UnionWith((IEnumerable<SemanticContext>) ((SemanticContext.AND) a).opnds);
        else
          semanticContextSet.Add(a);
        if (b is SemanticContext.AND)
          semanticContextSet.UnionWith((IEnumerable<SemanticContext>) ((SemanticContext.AND) b).opnds);
        else
          semanticContextSet.Add(b);
        IList<SemanticContext.PrecedencePredicate> source = SemanticContext.FilterPrecedencePredicates(semanticContextSet);
        if (source.Count > 0)
        {
          SemanticContext.PrecedencePredicate precedencePredicate = source.Min<SemanticContext.PrecedencePredicate>();
          semanticContextSet.Add((SemanticContext) precedencePredicate);
        }
        this.opnds = semanticContextSet.ToArray<SemanticContext>();
      }

      public override ICollection<SemanticContext> Operands => (ICollection<SemanticContext>) Arrays.AsList<SemanticContext>(this.opnds);

      public override bool Equals(object obj)
      {
        if (this == obj)
          return true;
        return obj is SemanticContext.AND && Arrays.Equals<SemanticContext>(this.opnds, ((SemanticContext.AND) obj).opnds);
      }

      public override int GetHashCode() => MurmurHash.HashCode<SemanticContext>(this.opnds, typeof (SemanticContext.AND).GetHashCode());

      public override bool Eval<Symbol, ATNInterpreter>(
        Recognizer<Symbol, ATNInterpreter> parser,
        RuleContext parserCallStack)
      {
        foreach (SemanticContext opnd in this.opnds)
        {
          if (!opnd.Eval<Symbol, ATNInterpreter>(parser, parserCallStack))
            return false;
        }
        return true;
      }

      public override SemanticContext EvalPrecedence<Symbol, ATNInterpreter>(
        Recognizer<Symbol, ATNInterpreter> parser,
        RuleContext parserCallStack)
      {
        bool flag = false;
        IList<SemanticContext> semanticContextList = (IList<SemanticContext>) new List<SemanticContext>();
        foreach (SemanticContext opnd in this.opnds)
        {
          SemanticContext semanticContext = opnd.EvalPrecedence<Symbol, ATNInterpreter>(parser, parserCallStack);
          flag |= semanticContext != opnd;
          if (semanticContext == null)
            return (SemanticContext) null;
          if (semanticContext != SemanticContext.NONE)
            semanticContextList.Add(semanticContext);
        }
        if (!flag)
          return (SemanticContext) this;
        if (semanticContextList.Count == 0)
          return SemanticContext.NONE;
        SemanticContext a = semanticContextList[0];
        for (int index = 1; index < semanticContextList.Count; ++index)
          a = SemanticContext.AndOp(a, semanticContextList[index]);
        return a;
      }

      public override string ToString() => Utils.Join<SemanticContext>("&&", (IEnumerable<SemanticContext>) this.opnds);
    }

    internal class OR : SemanticContext.Operator
    {
      [NotNull]
      public readonly SemanticContext[] opnds;

      public OR(SemanticContext a, SemanticContext b)
      {
        HashSet<SemanticContext> semanticContextSet = new HashSet<SemanticContext>();
        if (a is SemanticContext.OR)
          semanticContextSet.UnionWith((IEnumerable<SemanticContext>) ((SemanticContext.OR) a).opnds);
        else
          semanticContextSet.Add(a);
        if (b is SemanticContext.OR)
          semanticContextSet.UnionWith((IEnumerable<SemanticContext>) ((SemanticContext.OR) b).opnds);
        else
          semanticContextSet.Add(b);
        IList<SemanticContext.PrecedencePredicate> source = SemanticContext.FilterPrecedencePredicates(semanticContextSet);
        if (source.Count > 0)
        {
          SemanticContext.PrecedencePredicate precedencePredicate = source.Max<SemanticContext.PrecedencePredicate>();
          semanticContextSet.Add((SemanticContext) precedencePredicate);
        }
        this.opnds = semanticContextSet.ToArray<SemanticContext>();
      }

      public override ICollection<SemanticContext> Operands => (ICollection<SemanticContext>) Arrays.AsList<SemanticContext>(this.opnds);

      public override bool Equals(object obj)
      {
        if (this == obj)
          return true;
        return obj is SemanticContext.OR && Arrays.Equals<SemanticContext>(this.opnds, ((SemanticContext.OR) obj).opnds);
      }

      public override int GetHashCode() => MurmurHash.HashCode<SemanticContext>(this.opnds, typeof (SemanticContext.OR).GetHashCode());

      public override bool Eval<Symbol, ATNInterpreter>(
        Recognizer<Symbol, ATNInterpreter> parser,
        RuleContext parserCallStack)
      {
        foreach (SemanticContext opnd in this.opnds)
        {
          if (opnd.Eval<Symbol, ATNInterpreter>(parser, parserCallStack))
            return true;
        }
        return false;
      }

      public override SemanticContext EvalPrecedence<Symbol, ATNInterpreter>(
        Recognizer<Symbol, ATNInterpreter> parser,
        RuleContext parserCallStack)
      {
        bool flag = false;
        IList<SemanticContext> semanticContextList = (IList<SemanticContext>) new List<SemanticContext>();
        foreach (SemanticContext opnd in this.opnds)
        {
          SemanticContext semanticContext = opnd.EvalPrecedence<Symbol, ATNInterpreter>(parser, parserCallStack);
          flag |= semanticContext != opnd;
          if (semanticContext == SemanticContext.NONE)
            return SemanticContext.NONE;
          if (semanticContext != null)
            semanticContextList.Add(semanticContext);
        }
        if (!flag)
          return (SemanticContext) this;
        if (semanticContextList.Count == 0)
          return (SemanticContext) null;
        SemanticContext a = semanticContextList[0];
        for (int index = 1; index < semanticContextList.Count; ++index)
          a = SemanticContext.OrOp(a, semanticContextList[index]);
        return a;
      }

      public override string ToString() => Utils.Join<SemanticContext>("||", (IEnumerable<SemanticContext>) this.opnds);
    }
  }
}
