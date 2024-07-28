// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.HintStrategyFactory
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class HintStrategyFactory
  {
    private static readonly IList<IHintStrategy> _defaultStrategies = (IList<IHintStrategy>) new List<IHintStrategy>()
    {
      (IHintStrategy) new ForceAssumeJoinPredicateDependsOnFilterStrategy(),
      (IHintStrategy) new SnapshotHintStrategy(),
      (IHintStrategy) new ForcePartitionFilterStrategy(),
      (IHintStrategy) new TestResultHintStrategy(SqlOptions.TestResultJoinOptimization, typeof (TestResult)),
      (IHintStrategy) new TestResultHintStrategy(SqlOptions.TestResultRecompile, typeof (TestResult)),
      (IHintStrategy) new TestResultHintStrategy(SqlOptions.TestResultJoinOptimization, typeof (TestResultDaily)),
      (IHintStrategy) new TestResultHintStrategy(SqlOptions.TestResultRecompile, typeof (TestResultDaily)),
      (IHintStrategy) new UseHashJoinForFilterHint(SqlOptions.HashJoinFilterHint, (ICollection<Type>) new Type[3]
      {
        typeof (WorkItemSnapshot),
        typeof (WorkItem),
        typeof (WorkItemRevision)
      }, (ICollection<Type>) new Type[1]{ typeof (Tag) }),
      (IHintStrategy) new UseHashJoinForBurndownQuery(),
      (IHintStrategy) new UseNoHitViewsForRollupQuery(),
      (IHintStrategy) new UseLoopJoinForRollupQuery()
    };
    private readonly IList<IHintStrategy> _strategies;
    private Type _entitySetType;
    private IQueryable _query;
    private ODataQueryOptions _odataQueryOptions;

    public HintStrategyFactory(
      Type entitySetType,
      IQueryable query,
      ODataQueryOptions odataQueryOptions)
      : this(entitySetType, query, odataQueryOptions, HintStrategyFactory._defaultStrategies)
    {
    }

    internal HintStrategyFactory(
      Type entitySetType,
      IQueryable query,
      ODataQueryOptions odataQueryOptions,
      IList<IHintStrategy> strategies)
    {
      this._entitySetType = entitySetType;
      this._query = query;
      this._odataQueryOptions = odataQueryOptions;
      this._strategies = strategies;
    }

    public SqlOptions ApplyOptions(QueryType queryType, SqlOptions supportedOptions = ~SqlOptions.None)
    {
      SqlOptions sqlOptions = SqlOptions.None;
      foreach (IHintStrategy strategy in (IEnumerable<IHintStrategy>) this._strategies)
        sqlOptions |= supportedOptions & strategy.GetOptions(this._entitySetType, queryType, this._query, this._odataQueryOptions);
      return sqlOptions;
    }
  }
}
