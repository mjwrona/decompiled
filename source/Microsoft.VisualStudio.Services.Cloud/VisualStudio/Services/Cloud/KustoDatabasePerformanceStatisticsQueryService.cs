// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoDatabasePerformanceStatisticsQueryService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoDatabasePerformanceStatisticsQueryService : 
    KustoQueryServiceBase,
    IKustoDatabasePerformanceStatisticsQueryService,
    IVssFrameworkService
  {
    private static readonly string[] s_databasePerformanceStatistics = new string[2]
    {
      KustoWellKnownDataSources.DatabasePerformanceStatistics,
      KustoWellKnownDataSources.DatabaseRecentlyUpdated
    };

    protected override string Layer => nameof (KustoDatabasePerformanceStatisticsQueryService);

    public override void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<DatabaseRightSizingRecommendation> QueryDatabaseRightSizingRecommendations(
      IVssRequestContext requestContext,
      string query)
    {
      return this.Query<DatabaseRightSizingRecommendation>(requestContext, KustoDatabasePerformanceStatisticsQueryService.s_databasePerformanceStatistics, query, KustoQueryConfig.DefaultConfig);
    }

    public IEnumerable<DatabasePerformanceStatistics> QueryDatabasePerformanceStatistics(
      IVssRequestContext requestContext,
      string query)
    {
      return this.Query<DatabasePerformanceStatistics>(requestContext, KustoDatabasePerformanceStatisticsQueryService.s_databasePerformanceStatistics, query, KustoQueryConfig.DefaultConfig);
    }

    protected override KustoQueryRestriction GetQueryRestrictions(
      IVssRequestContext requestContext,
      string[] requestedTables)
    {
      return (KustoQueryRestriction) new KustoDatabasePerformanceStatisticsQueryRestriction(requestContext, requestedTables);
    }
  }
}
