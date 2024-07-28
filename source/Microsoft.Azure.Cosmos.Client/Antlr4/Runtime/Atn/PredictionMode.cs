// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.PredictionMode
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  [Serializable]
  internal sealed class PredictionMode
  {
    public static readonly PredictionMode SLL = new PredictionMode();
    public static readonly PredictionMode LL = new PredictionMode();
    public static readonly PredictionMode LL_EXACT_AMBIG_DETECTION = new PredictionMode();

    public static bool HasSLLConflictTerminatingPrediction(
      PredictionMode mode,
      ATNConfigSet configSet)
    {
      if (PredictionMode.AllConfigsInRuleStopStates((IEnumerable<ATNConfig>) configSet.configs))
        return true;
      if (mode == PredictionMode.SLL && configSet.hasSemanticContext)
      {
        ATNConfigSet atnConfigSet = new ATNConfigSet();
        foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
          atnConfigSet.Add(new ATNConfig(config, SemanticContext.NONE));
        configSet = atnConfigSet;
      }
      return PredictionMode.HasConflictingAltSet((IEnumerable<BitSet>) PredictionMode.GetConflictingAltSubsets((IEnumerable<ATNConfig>) configSet.configs)) && !PredictionMode.HasStateAssociatedWithOneAlt((IEnumerable<ATNConfig>) configSet.configs);
    }

    public static bool HasConfigInRuleStopState(IEnumerable<ATNConfig> configs)
    {
      foreach (ATNConfig config in configs)
      {
        if (config.state is RuleStopState)
          return true;
      }
      return false;
    }

    public static bool AllConfigsInRuleStopStates(IEnumerable<ATNConfig> configs)
    {
      foreach (ATNConfig config in configs)
      {
        if (!(config.state is RuleStopState))
          return false;
      }
      return true;
    }

    public static int ResolvesToJustOneViableAlt(IEnumerable<BitSet> altsets) => PredictionMode.GetSingleViableAlt(altsets);

    public static bool AllSubsetsConflict(IEnumerable<BitSet> altsets) => !PredictionMode.HasNonConflictingAltSet(altsets);

    public static bool HasNonConflictingAltSet(IEnumerable<BitSet> altsets)
    {
      foreach (BitSet altset in altsets)
      {
        if (altset.Cardinality() == 1)
          return true;
      }
      return false;
    }

    public static bool HasConflictingAltSet(IEnumerable<BitSet> altsets)
    {
      foreach (BitSet altset in altsets)
      {
        if (altset.Cardinality() > 1)
          return true;
      }
      return false;
    }

    public static bool AllSubsetsEqual(IEnumerable<BitSet> altsets)
    {
      IEnumerator<BitSet> enumerator = altsets.GetEnumerator();
      enumerator.MoveNext();
      BitSet current = enumerator.Current;
      while (enumerator.MoveNext())
      {
        if (!enumerator.Current.Equals((object) current))
          return false;
      }
      return true;
    }

    public static int GetUniqueAlt(IEnumerable<BitSet> altsets)
    {
      BitSet alts = PredictionMode.GetAlts(altsets);
      return alts.Cardinality() == 1 ? alts.NextSetBit(0) : 0;
    }

    public static BitSet GetAlts(IEnumerable<BitSet> altsets)
    {
      BitSet alts = new BitSet();
      foreach (BitSet altset in altsets)
        alts.Or(altset);
      return alts;
    }

    [return: NotNull]
    public static ICollection<BitSet> GetConflictingAltSubsets(IEnumerable<ATNConfig> configs)
    {
      PredictionMode.AltAndContextMap altAndContextMap = new PredictionMode.AltAndContextMap();
      foreach (ATNConfig config in configs)
      {
        BitSet bitSet;
        if (!altAndContextMap.TryGetValue(config, out bitSet))
        {
          bitSet = new BitSet();
          altAndContextMap[config] = bitSet;
        }
        bitSet.Set(config.alt);
      }
      return (ICollection<BitSet>) altAndContextMap.Values;
    }

    [return: NotNull]
    public static IDictionary<ATNState, BitSet> GetStateToAltMap(IEnumerable<ATNConfig> configs)
    {
      IDictionary<ATNState, BitSet> stateToAltMap = (IDictionary<ATNState, BitSet>) new Dictionary<ATNState, BitSet>();
      foreach (ATNConfig config in configs)
      {
        BitSet bitSet;
        if (!stateToAltMap.TryGetValue(config.state, out bitSet))
        {
          bitSet = new BitSet();
          stateToAltMap[config.state] = bitSet;
        }
        bitSet.Set(config.alt);
      }
      return stateToAltMap;
    }

    public static bool HasStateAssociatedWithOneAlt(IEnumerable<ATNConfig> configs)
    {
      foreach (BitSet bitSet in (IEnumerable<BitSet>) PredictionMode.GetStateToAltMap(configs).Values)
      {
        if (bitSet.Cardinality() == 1)
          return true;
      }
      return false;
    }

    public static int GetSingleViableAlt(IEnumerable<BitSet> altsets)
    {
      BitSet bitSet = new BitSet();
      foreach (BitSet altset in altsets)
      {
        int index = altset.NextSetBit(0);
        bitSet.Set(index);
        if (bitSet.Cardinality() > 1)
          return 0;
      }
      return bitSet.NextSetBit(0);
    }

    internal class AltAndContextMap : Dictionary<ATNConfig, BitSet>
    {
      public AltAndContextMap()
        : base((IEqualityComparer<ATNConfig>) PredictionMode.AltAndContextConfigEqualityComparator.Instance)
      {
      }
    }

    private sealed class AltAndContextConfigEqualityComparator : EqualityComparer<ATNConfig>
    {
      public static readonly PredictionMode.AltAndContextConfigEqualityComparator Instance = new PredictionMode.AltAndContextConfigEqualityComparator();

      private AltAndContextConfigEqualityComparator()
      {
      }

      public override int GetHashCode(ATNConfig o) => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(7), o.state.stateNumber), (object) o.context), 2);

      public override bool Equals(ATNConfig a, ATNConfig b)
      {
        if (a == b)
          return true;
        return a != null && b != null && a.state.stateNumber == b.state.stateNumber && a.context.Equals((object) b.context);
      }
    }
  }
}
