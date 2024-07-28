// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQuerySnapshotValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataQuerySnapshotValidator : IODataValidator
  {
    public string RequiredFeatureFlag => "Analytics.OData.BlockSnapshotWithoutAggregation";

    public Action WarningCallback { get; set; }

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      if (!typeof (ISnapshot).IsAssignableFrom(queryOptions.Context.ElementClrType))
        return;
      ApplyQueryOption apply = queryOptions.Apply;
      if ((apply != null ? (apply.ApplyClause.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind != TransformationNodeKind.Filter)) ? 1 : 0) : 0) != 0)
        return;
      if (queryOptions.RawValues.SkipToken != null || queryOptions.RawValues.Skip != null || queryOptions.RawValues.OrderBy != null)
        throw new ODataException(AnalyticsResources.ODATA_SNAPSHOT_WITHOUT_AGGREGATION());
      request.ODataWarnings().Add(AnalyticsResources.ODATA_SNAPSHOT_WITHOUT_AGGREGATION());
      Action warningCallback = this.WarningCallback;
      if (warningCallback == null)
        return;
      warningCallback();
    }
  }
}
