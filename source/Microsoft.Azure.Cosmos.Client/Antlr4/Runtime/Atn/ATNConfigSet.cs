// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ATNConfigSet
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime.Atn
{
  internal class ATNConfigSet
  {
    protected bool readOnly;
    public ConfigHashSet configLookup;
    public ArrayList<ATNConfig> configs = new ArrayList<ATNConfig>(7);
    public int uniqueAlt;
    public BitSet conflictingAlts;
    public bool hasSemanticContext;
    public bool dipsIntoOuterContext;
    public readonly bool fullCtx;
    private int cachedHashCode = -1;

    public ATNConfigSet(bool fullCtx)
    {
      this.configLookup = new ConfigHashSet();
      this.fullCtx = fullCtx;
    }

    public ATNConfigSet()
      : this(true)
    {
    }

    public ATNConfigSet(ATNConfigSet old)
      : this(old.fullCtx)
    {
      this.AddAll((ICollection<ATNConfig>) old.configs);
      this.uniqueAlt = old.uniqueAlt;
      this.conflictingAlts = old.conflictingAlts;
      this.hasSemanticContext = old.hasSemanticContext;
      this.dipsIntoOuterContext = old.dipsIntoOuterContext;
    }

    public bool Add(ATNConfig config) => this.Add(config, (MergeCache) null);

    public bool Add(ATNConfig config, MergeCache mergeCache)
    {
      if (this.readOnly)
        throw new Exception("This set is readonly");
      if (config.semanticContext != SemanticContext.NONE)
        this.hasSemanticContext = true;
      if (config.OuterContextDepth > 0)
        this.dipsIntoOuterContext = true;
      ATNConfig orAdd = this.configLookup.GetOrAdd(config);
      if (orAdd == config)
      {
        this.cachedHashCode = -1;
        this.configs.Add(config);
        return true;
      }
      bool rootIsWildcard = !this.fullCtx;
      PredictionContext predictionContext = PredictionContext.Merge(orAdd.context, config.context, rootIsWildcard, mergeCache);
      orAdd.reachesIntoOuterContext = Math.Max(orAdd.reachesIntoOuterContext, config.reachesIntoOuterContext);
      if (config.IsPrecedenceFilterSuppressed)
        orAdd.SetPrecedenceFilterSuppressed(true);
      orAdd.context = predictionContext;
      return true;
    }

    public List<ATNConfig> Elements => (List<ATNConfig>) this.configs;

    public HashSet<ATNState> GetStates()
    {
      HashSet<ATNState> states = new HashSet<ATNState>();
      foreach (ATNConfig config in (List<ATNConfig>) this.configs)
        states.Add(config.state);
      return states;
    }

    public BitSet GetAlts()
    {
      BitSet alts = new BitSet();
      foreach (ATNConfig config in (List<ATNConfig>) this.configs)
        alts.Set(config.alt);
      return alts;
    }

    public List<SemanticContext> GetPredicates()
    {
      List<SemanticContext> predicates = new List<SemanticContext>();
      foreach (ATNConfig config in (List<ATNConfig>) this.configs)
      {
        if (config.semanticContext != SemanticContext.NONE)
          predicates.Add(config.semanticContext);
      }
      return predicates;
    }

    public ATNConfig Get(int i) => this.configs[i];

    public void OptimizeConfigs(ATNSimulator interpreter)
    {
      if (this.readOnly)
        throw new Exception("This set is readonly");
      if (this.configLookup.Count == 0)
        return;
      foreach (ATNConfig config in (List<ATNConfig>) this.configs)
        config.context = interpreter.getCachedContext(config.context);
    }

    public bool AddAll(ICollection<ATNConfig> coll)
    {
      foreach (ATNConfig config in (IEnumerable<ATNConfig>) coll)
        this.Add(config);
      return false;
    }

    public override bool Equals(object o)
    {
      if (o == this)
        return true;
      if (!(o is ATNConfigSet))
        return false;
      ATNConfigSet atnConfigSet = (ATNConfigSet) o;
      return this.configs != null && this.configs.Equals((List<ATNConfig>) atnConfigSet.configs) && this.fullCtx == atnConfigSet.fullCtx && this.uniqueAlt == atnConfigSet.uniqueAlt && this.conflictingAlts == atnConfigSet.conflictingAlts && this.hasSemanticContext == atnConfigSet.hasSemanticContext && this.dipsIntoOuterContext == atnConfigSet.dipsIntoOuterContext;
    }

    public override int GetHashCode()
    {
      if (!this.IsReadOnly)
        return this.configs.GetHashCode();
      if (this.cachedHashCode == -1)
        this.cachedHashCode = this.configs.GetHashCode();
      return this.cachedHashCode;
    }

    public int Count => this.configs.Count;

    public bool Empty => this.configs.Count == 0;

    public bool Contains(object o) => this.configLookup != null ? this.configLookup.ContainsKey((ATNConfig) o) : throw new Exception("This method is not implemented for readonly sets.");

    public void Clear()
    {
      if (this.readOnly)
        throw new Exception("This set is readonly");
      this.configs.Clear();
      this.cachedHashCode = -1;
      this.configLookup.Clear();
    }

    public bool IsReadOnly
    {
      get => this.readOnly;
      set
      {
        this.readOnly = value;
        this.configLookup = (ConfigHashSet) null;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('[');
      List<ATNConfig> elements = this.Elements;
      if (elements.Count > 0)
      {
        foreach (ATNConfig atnConfig in elements)
        {
          stringBuilder.Append(atnConfig.ToString());
          stringBuilder.Append(", ");
        }
        stringBuilder.Length -= 2;
      }
      stringBuilder.Append(']');
      if (this.hasSemanticContext)
        stringBuilder.Append(",hasSemanticContext=").Append(this.hasSemanticContext);
      if (this.uniqueAlt != 0)
        stringBuilder.Append(",uniqueAlt=").Append(this.uniqueAlt);
      if (this.conflictingAlts != null)
        stringBuilder.Append(",conflictingAlts=").Append((object) this.conflictingAlts);
      if (this.dipsIntoOuterContext)
        stringBuilder.Append(",dipsIntoOuterContext");
      return stringBuilder.ToString();
    }
  }
}
