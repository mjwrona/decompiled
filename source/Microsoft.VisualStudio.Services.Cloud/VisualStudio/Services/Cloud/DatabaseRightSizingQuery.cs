// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DatabaseRightSizingQuery
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class DatabaseRightSizingQuery
  {
    public static readonly string SqlRightSizingRecommendationQuery = EmbeddedResourceUtil.GetResourceAsString("Microsoft.VisualStudio.Services.Cloud.Kusto.Queries.SqlRightSizingRecommendationQuery.csl");
    internal static readonly string DatabaseRecentlyUpdated = EmbeddedResourceUtil.GetResourceAsString("Microsoft.VisualStudio.Services.Cloud.Kusto.Queries.DatabaseRecentlyUpdatedStmt.csl");
    internal static readonly string DatabasePerformanceStatistics = EmbeddedResourceUtil.GetResourceAsString("Microsoft.VisualStudio.Services.Cloud.Kusto.Queries.DatabasePerformanceStatisticsStmt.csl");
    private const string c_namespaceWithResourcePrefix = "Microsoft.VisualStudio.Services.Cloud.Kusto.Queries";
  }
}
