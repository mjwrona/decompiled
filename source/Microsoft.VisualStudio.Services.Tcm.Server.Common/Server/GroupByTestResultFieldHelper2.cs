// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GroupByTestResultFieldHelper2
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class GroupByTestResultFieldHelper2 : GroupByTestResultFieldHelper
  {
    public override string GenerateSqlStatement(
      string groupByFieldStringWithOutcome,
      string propertiesToFetch,
      string filterClause,
      string orderBy)
    {
      string v2UnifyingViewsV2 = TestManagementDynamicSqlBatchStatements.dynprc_QueryAggregatedResultsForBuildV2_UnifyingViews_V2;
      string str = propertiesToFetch + ", " + groupByFieldStringWithOutcome;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, v2UnifyingViewsV2, (object) string.Empty, (object) str, (object) filterClause, (object) orderBy);
    }
  }
}
