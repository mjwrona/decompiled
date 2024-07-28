// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.Transition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.ObjectModel;

namespace Antlr4.Runtime.Atn
{
  internal abstract class Transition
  {
    public static readonly ReadOnlyCollection<string> serializationNames = new ReadOnlyCollection<string>(Arrays.AsList<string>("INVALID", "EPSILON", "RANGE", "RULE", "PREDICATE", "ATOM", "ACTION", "SET", "NOT_SET", "WILDCARD", "PRECEDENCE"));
    [NotNull]
    public ATNState target;

    protected internal Transition(ATNState target) => this.target = target != null ? target : throw new ArgumentNullException("target cannot be null.");

    public abstract TransitionType TransitionType { get; }

    public virtual bool IsEpsilon => false;

    public virtual IntervalSet Label => (IntervalSet) null;

    public abstract bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol);
  }
}
