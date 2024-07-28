// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoDatabasePerformanceStatisticsQueryRestriction
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoDatabasePerformanceStatisticsQueryRestriction : KustoQueryRestriction
  {
    internal const string ServiceFilterFormat = "where Service = '{0}'";
    internal const string ScaleUnitFilterFormat = "where ScaleUnit = '{0}'";
    internal const string PeriodStartFilterFormat = "where PeriodStart > {0} and PeriodStart < {1}";
    private readonly string s_dynamicExpressionFormat = "dynamic([{0}])";
    private readonly string s_dateFrom = "dateFrom";
    private readonly string s_dateTo = "dateTo";

    public KustoDatabasePerformanceStatisticsQueryRestriction(
      IVssRequestContext requestContext,
      string[] requestedTables)
      : base(requestContext, requestedTables, new KustoQueryConfig()
      {
        KustoClusterDestination = KustoClusterDestination.VSO,
        QueryType = KustoQueryType.Default
      })
    {
    }

    public override string Apply(string query) => KustoQueryHelper.Concat(this.m_restrictingStatements, query);

    protected override string BuildRestrictingStatements(
      IVssRequestContext requestContext,
      string[] requestedTables)
    {
      List<string> stringList = new List<string>();
      List<string> list = ((IEnumerable<string>) requestedTables).ToList<string>();
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      int num1 = 6;
      if (requestContext.IsFeatureEnabled("VisualStudio.Service.Framework.EnableExtendedLookbackRightSizing"))
        num1 = 13;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) FrameworkServerConstants.SqlRightSizingQueryDateFrom;
      int defaultValue = num1;
      int num2 = service.GetValue<int>(requestContext1, in local, defaultValue);
      stringList.Add(new KustoLetStatement(this.s_dateFrom, string.Format("ago({0}d)", (object) num2)).ToString());
      stringList.Add(new KustoLetStatement(this.s_dateTo, "now()").ToString());
      IEnumerable<string> values = requestContext.GetService<TeamFoundationDatabaseManagementService>().QueryDatabases(requestContext, true).Where<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (db => db.IsEligibleForRightSizing())).ToList<ITeamFoundationDatabaseProperties>().Select<ITeamFoundationDatabaseProperties, string>((Func<ITeamFoundationDatabaseProperties, string>) (d => string.Format("\"{0}\"", (object) d.DatabaseName)));
      stringList.Add(new KustoLetStatement(KustoWellKnownDataSources.Databases, string.Format(this.s_dynamicExpressionFormat, (object) string.Join(",", values))).ToString());
      if (list.Contains(KustoWellKnownDataSources.DatabasePerformanceStatistics))
        stringList.Add(new KustoLetStatement(KustoWellKnownDataSources.DatabasePerformanceStatistics, DatabaseRightSizingQuery.DatabasePerformanceStatistics).ToString());
      if (list.Contains(KustoWellKnownDataSources.DatabaseRecentlyUpdated))
        stringList.Add(new KustoLetStatement(KustoWellKnownDataSources.DatabaseRecentlyUpdated, DatabaseRightSizingQuery.DatabaseRecentlyUpdated).ToString());
      stringList.Add((string) (KustoStatement) new KustoRestrictStatement(requestedTables));
      return KustoQueryHelper.Concat(stringList.ToArray());
    }
  }
}
