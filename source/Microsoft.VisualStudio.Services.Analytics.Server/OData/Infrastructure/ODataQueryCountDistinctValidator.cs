// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQueryCountDistinctValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataQueryCountDistinctValidator : IODataValidator
  {
    public string RequiredFeatureFlag => "Analytics.OData.BlockCountDistinct";

    public Action WarningCallback { get; set; }

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      if (queryOptions.Apply == null)
        return;
      IEnumerable<TransformationNode> transformations = queryOptions.Apply.ApplyClause.Transformations;
      if (transformations.OfType<AggregateTransformationNode>().SelectMany<AggregateTransformationNode, AggregateExpression>((Func<AggregateTransformationNode, IEnumerable<AggregateExpression>>) (a => a.AggregateExpressions.OfType<AggregateExpression>())).Concat<AggregateExpression>(transformations.OfType<GroupByTransformationNode>().Where<GroupByTransformationNode>((Func<GroupByTransformationNode, bool>) (t => t.ChildTransformations != null)).Select<GroupByTransformationNode, TransformationNode>((Func<GroupByTransformationNode, TransformationNode>) (t => t.ChildTransformations)).OfType<AggregateTransformationNode>().SelectMany<AggregateTransformationNode, AggregateExpression>((Func<AggregateTransformationNode, IEnumerable<AggregateExpression>>) (a => a.AggregateExpressions.OfType<AggregateExpression>()))).Any<AggregateExpression>((Func<AggregateExpression, bool>) (e => e.Method == AggregationMethod.CountDistinct)))
        throw new ODataException(AnalyticsResources.ODATA_QUERY_WITH_COUNTDISTINCT_NOT_SUPPORTED());
    }
  }
}
