// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimizationStrategy
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryOptimizationStrategy
  {
    public const int DefaultTrialLimit = 5;
    public const int DefaultSlowRunTimeMinThresholdInMs = 3000;
    public const float DefaultDesiredOptimizationRatio = 0.7f;
    public const float DefaultOptimizedNormalCriteriaSlackRatio = 0.85f;
    public const int DefaultToleratedSlownessInMs = 5000;
    public const int DefaultMaxDaysInNotOptimizable = 7;
    private static ConcurrentDictionary<QueryCategory, QueryOptimizationStrategy> InstancesByCategories = new ConcurrentDictionary<QueryCategory, QueryOptimizationStrategy>();
    private static IEnumerable<QueryOptimizationStrategy.OptimizationTargetDefinition> OptimizationsByCategories = (IEnumerable<QueryOptimizationStrategy.OptimizationTargetDefinition>) new List<QueryOptimizationStrategy.OptimizationTargetDefinition>()
    {
      new QueryOptimizationStrategy.OptimizationTargetDefinition()
      {
        Category = QueryCategory.FullTextSearchQuery,
        TargetedOptimizations = (IEnumerable<QueryOptimization>) new List<QueryOptimization>()
        {
          QueryOptimization.ForceFullTextIndex,
          QueryOptimization.DoNotForceFullTextIndex,
          QueryOptimization.FullTextJoinForceOrder,
          QueryOptimization.FullTextSearchResultInTempTable
        }
      },
      new QueryOptimizationStrategy.OptimizationTargetDefinition()
      {
        Category = QueryCategory.CustomLatestTableQuery,
        TargetedOptimizations = (IEnumerable<QueryOptimization>) new List<QueryOptimization>()
        {
          QueryOptimization.ForceCustomTablePK
        }
      },
      new QueryOptimizationStrategy.OptimizationTargetDefinition()
      {
        Category = QueryCategory.None,
        TargetedOptimizations = (IEnumerable<QueryOptimization>) new List<QueryOptimization>()
        {
          QueryOptimization.ForceOrder
        }
      },
      new QueryOptimizationStrategy.OptimizationTargetDefinition()
      {
        Category = QueryCategory.LowerLevelOrClauseQuery,
        TargetedOptimizations = (IEnumerable<QueryOptimization>) new List<QueryOptimization>()
        {
          QueryOptimization.MoveOrClauseUp
        }
      }
    };
    private static IDictionary<QueryCategory, List<QueryOptimization>> OptimizationOrderOverridesByCategories = (IDictionary<QueryCategory, List<QueryOptimization>>) new Dictionary<QueryCategory, List<QueryOptimization>>()
    {
      {
        QueryCategory.FullTextSearchQuery,
        new List<QueryOptimization>()
        {
          QueryOptimization.FullTextJoinForceOrder,
          QueryOptimization.DoNotForceFullTextIndex,
          QueryOptimization.ForceFullTextIndex,
          QueryOptimization.FullTextSearchResultInTempTable,
          QueryOptimization.ForceOrder
        }
      },
      {
        QueryCategory.LowerLevelOrClauseQuery,
        new List<QueryOptimization>()
        {
          QueryOptimization.MoveOrClauseUp,
          QueryOptimization.ForceOrder
        }
      },
      {
        QueryCategory.CustomLatestTableQuery | QueryCategory.LowerLevelOrClauseQuery,
        new List<QueryOptimization>()
        {
          QueryOptimization.ForceCustomTablePK,
          QueryOptimization.MoveOrClauseUp,
          QueryOptimization.ForceOrder
        }
      },
      {
        QueryCategory.FullTextSearchQuery | QueryCategory.CustomLatestTableQuery,
        new List<QueryOptimization>()
        {
          QueryOptimization.FullTextJoinForceOrder,
          QueryOptimization.DoNotForceFullTextIndex,
          QueryOptimization.FullTextSearchResultInTempTable,
          QueryOptimization.ForceFullTextIndex,
          QueryOptimization.ForceCustomTablePK,
          QueryOptimization.ForceOrder
        }
      },
      {
        QueryCategory.FullTextSearchQuery | QueryCategory.LowerLevelOrClauseQuery,
        new List<QueryOptimization>()
        {
          QueryOptimization.FullTextJoinForceOrder,
          QueryOptimization.DoNotForceFullTextIndex,
          QueryOptimization.FullTextSearchResultInTempTable,
          QueryOptimization.ForceFullTextIndex,
          QueryOptimization.MoveOrClauseUp,
          QueryOptimization.ForceOrder
        }
      },
      {
        QueryCategory.FullTextSearchQuery | QueryCategory.CustomLatestTableQuery | QueryCategory.LowerLevelOrClauseQuery,
        new List<QueryOptimization>()
        {
          QueryOptimization.DoNotForceFullTextIndex,
          QueryOptimization.FullTextJoinForceOrder,
          QueryOptimization.ForceCustomTablePK,
          QueryOptimization.FullTextSearchResultInTempTable,
          QueryOptimization.ForceFullTextIndex,
          QueryOptimization.MoveOrClauseUp,
          QueryOptimization.ForceOrder
        }
      }
    };

    public static Capture<int> TrialLimit { get; } = Capture.Create<int>(5);

    public static Capture<int> SlowRunTimeMinThresholdInMs { get; } = Capture.Create<int>(3000);

    public static Capture<float> DesiredOptimizationRatio { get; } = Capture.Create<float>(0.7f);

    public static Capture<int> ToleratedSlownessInMs { get; } = Capture.Create<int>(5000);

    public static Capture<float> OptimizedNormalCriteriaSlackRatio { get; } = Capture.Create<float>(0.85f);

    public static Capture<int> MaxDaysInNotOptimizable { get; } = Capture.Create<int>(7);

    public static void SetConfigurations(
      int stablizationCount,
      int optimizationRatio,
      int slowRunTimeMinThreshold)
    {
      QueryOptimizationStrategy.TrialLimit.Value = stablizationCount;
      QueryOptimizationStrategy.DesiredOptimizationRatio.Value = (float) optimizationRatio;
      QueryOptimizationStrategy.SlowRunTimeMinThresholdInMs.Value = slowRunTimeMinThreshold;
    }

    public static QueryOptimizationStrategy GetInstance(QueryCategory category)
    {
      QueryCategory optimizableCategories = QueryCategory.None;
      foreach (QueryOptimizationStrategy.OptimizationTargetDefinition optimizationsByCategory in QueryOptimizationStrategy.OptimizationsByCategories)
      {
        if (category.HasFlag((Enum) optimizationsByCategory.Category))
          optimizableCategories |= optimizationsByCategory.Category;
      }
      return QueryOptimizationStrategy.InstancesByCategories.GetOrAdd(optimizableCategories, (Func<QueryCategory, QueryOptimizationStrategy>) (c =>
      {
        List<QueryOptimization> list = QueryOptimizationStrategy.OptimizationsByCategories.Where<QueryOptimizationStrategy.OptimizationTargetDefinition>((Func<QueryOptimizationStrategy.OptimizationTargetDefinition, bool>) (x => optimizableCategories.HasFlag((Enum) x.Category))).SelectMany<QueryOptimizationStrategy.OptimizationTargetDefinition, QueryOptimization>((Func<QueryOptimizationStrategy.OptimizationTargetDefinition, IEnumerable<QueryOptimization>>) (x => x.TargetedOptimizations)).ToList<QueryOptimization>();
        List<QueryOptimization> queryOptimizationList;
        QueryOptimizationStrategy.OptimizationOrderOverridesByCategories.TryGetValue(category, out queryOptimizationList);
        return new QueryOptimizationStrategy(c, (IList<QueryOptimization>) list, (IList<QueryOptimization>) (queryOptimizationList ?? list));
      }));
    }

    private QueryCategory OptimizableCategories { get; }

    private IList<QueryOptimization> IndexOptimizationSequence { get; }

    private IList<QueryOptimization> TrueOptimizationIterationSequence { get; }

    private QueryOptimizationStrategy(
      QueryCategory optimizableCategories,
      IList<QueryOptimization> indexSequence,
      IList<QueryOptimization> iterationSequence)
    {
      if (!new HashSet<QueryOptimization>((IEnumerable<QueryOptimization>) indexSequence).SetEquals((IEnumerable<QueryOptimization>) new HashSet<QueryOptimization>((IEnumerable<QueryOptimization>) iterationSequence)))
        throw new ArgumentException("indexSequence and iterationSequence must contain the same elements");
      this.OptimizableCategories = optimizableCategories;
      this.IndexOptimizationSequence = indexSequence;
      this.TrueOptimizationIterationSequence = iterationSequence;
    }

    public QueryOptimization GetOptimization(short currentOptimizationIndex) => this.IndexOptimizationSequence.Count == 0 || currentOptimizationIndex < (short) 0 || (int) currentOptimizationIndex >= this.IndexOptimizationSequence.Count ? QueryOptimization.None : this.IndexOptimizationSequence[(int) currentOptimizationIndex];

    public short? GetNextOptimizationIndex(IVssRequestContext rc, short currentOptimizationIndex)
    {
      if (!WorkItemTrackingFeatureFlags.IsOverrideQueryAutoOptimizationOrderEnabled(rc))
        return this.GetNextOptimizationIndexWithoutOverriddenOrder(currentOptimizationIndex);
      if (currentOptimizationIndex < (short) -1 || (int) currentOptimizationIndex >= this.IndexOptimizationSequence.Count)
        return new short?();
      QueryOptimization optimization = this.GetOptimization(currentOptimizationIndex);
      return optimization == this.TrueOptimizationIterationSequence[this.TrueOptimizationIterationSequence.Count - 1] ? new short?() : new short?((short) this.IndexOptimizationSequence.IndexOf(this.TrueOptimizationIterationSequence[this.TrueOptimizationIterationSequence.IndexOf(optimization) + 1]));
    }

    public short? GetNextOptimizationIndexWithoutOverriddenOrder(short currentOptimizationIndex) => currentOptimizationIndex < (short) -1 || (int) currentOptimizationIndex + 1 >= this.IndexOptimizationSequence.Count ? new short?() : new short?((short) ((int) currentOptimizationIndex + 1));

    public bool Equals(QueryOptimizationStrategy other) => this.OptimizableCategories == other.OptimizableCategories;

    public static QueryOptimizationStrategy GetInstance(IEnumerable<QueryOptimization> opts)
    {
      ArgumentUtility.CheckForNull<IEnumerable<QueryOptimization>>(opts, nameof (opts));
      return new QueryOptimizationStrategy(QueryCategory.None, (IList<QueryOptimization>) opts.ToList<QueryOptimization>(), (IList<QueryOptimization>) opts.ToList<QueryOptimization>());
    }

    public static void ClearCachedInstances() => QueryOptimizationStrategy.InstancesByCategories.Clear();

    private class OptimizationTargetDefinition
    {
      public QueryCategory Category { get; set; }

      public IEnumerable<QueryOptimization> TargetedOptimizations { get; set; }
    }
  }
}
