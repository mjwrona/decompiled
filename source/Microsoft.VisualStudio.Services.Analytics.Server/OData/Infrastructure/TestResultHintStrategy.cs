// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.TestResultHintStrategy
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class TestResultHintStrategy : IHintStrategy
  {
    private SqlOptions _hintOption;
    private Type _tableType;

    public TestResultHintStrategy(SqlOptions sqlOptionsToReturn, Type type)
    {
      this._hintOption = sqlOptionsToReturn == SqlOptions.TestResultJoinOptimization || sqlOptionsToReturn == SqlOptions.TestResultRecompile ? sqlOptionsToReturn : throw new ArgumentException(AnalyticsResources.UNSUPPORTED_SQL_OPTIONS((object) sqlOptionsToReturn, (object) nameof (sqlOptionsToReturn)));
      this._tableType = type;
    }

    public SqlOptions GetOptions(
      Type entitySetType,
      QueryType queryType,
      IQueryable query,
      ODataQueryOptions odataQueryOptions)
    {
      return queryType == QueryType.Raw && entitySetType == this._tableType && odataQueryOptions.Apply != null && odataQueryOptions.Apply.ApplyClause.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind == TransformationNodeKind.GroupBy)) && odataQueryOptions.Apply.ApplyClause.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind == TransformationNodeKind.Filter)) && odataQueryOptions.Apply.ApplyClause.Transformations.OfType<GroupByTransformationNode>().First<GroupByTransformationNode>().GroupingProperties.SelectMany<GroupByPropertyNode, GroupByPropertyNode>((Func<GroupByPropertyNode, IEnumerable<GroupByPropertyNode>>) (p => (IEnumerable<GroupByPropertyNode>) p.ChildTransformations)).Select<GroupByPropertyNode, SingleValueNode>((Func<GroupByPropertyNode, SingleValueNode>) (t => t.Expression)).OfType<SingleValuePropertyAccessNode>().Any<SingleValuePropertyAccessNode>((Func<SingleValuePropertyAccessNode, bool>) (n => n.Source.Kind == QueryNodeKind.SingleNavigationNode && ((SingleResourceNode) n.Source).NavigationSource.Name == "Tests")) ? this._hintOption : SqlOptions.None;
    }
  }
}
