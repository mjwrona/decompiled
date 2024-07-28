// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoActivityLogQueryRestriction
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoActivityLogQueryRestriction : KustoQueryRestriction
  {
    internal const string HostFilterFormat = "where HostId =~ '{0}'";
    internal const string ViewLambdaFormat = "view () {{ {0} }}";
    internal const string VsoDbPrefix = "vsoDb.";

    public KustoActivityLogQueryRestriction(
      IVssRequestContext requestContext,
      string[] requestedTables,
      KustoQueryConfig kustoQueryConfig)
      : base(requestContext, requestedTables, kustoQueryConfig)
    {
    }

    protected override string BuildRestrictingStatements(
      IVssRequestContext requestContext,
      string[] requestedTables)
    {
      List<string> stringList = new List<string>();
      List<string> list = ((IEnumerable<string>) requestedTables).ToList<string>();
      if (list.Contains(KustoWellKnownDataSources.ActivityLog))
        stringList.Add(new KustoLetStatement(KustoWellKnownDataSources.ActivityLog, this.BuildFilteredViewExpression(requestContext, KustoWellKnownDataSources.ActivityLogFiltered)).ToString());
      if (list.Contains(KustoWellKnownDataSources.ActivityLogMapping))
        stringList.Add(new KustoLetStatement(KustoWellKnownDataSources.ActivityLogMapping, KustoWellKnownDataSources.ActivityLogMappingFiltered).ToString());
      if (list.Contains(KustoWellKnownDataSources.XEventDataRPCCompleted))
        stringList.Add(new KustoLetStatement(KustoWellKnownDataSources.XEventDataRPCCompleted, this.BuildFilteredViewExpression(requestContext, KustoWellKnownDataSources.XEventDataRPCCompletedFiltered)).ToString());
      if (list.Contains(KustoWellKnownDataSources.OrchestrationPlanContext))
        stringList.Add(new KustoLetStatement(KustoWellKnownDataSources.OrchestrationPlanContext, this.BuildFilteredViewExpression(requestContext, KustoWellKnownDataSources.OrchestrationPlanContextFiltered)).ToString());
      if (this.m_kustoQueryConfig.QueryType == KustoQueryType.Default)
        stringList.Add((string) (KustoStatement) new KustoRestrictStatement(requestedTables));
      return KustoQueryHelper.Concat(stringList.ToArray());
    }

    public override string Apply(string query)
    {
      query = KustoQueryHelper.Concat(this.m_restrictingStatements, query);
      return this.m_kustoQueryConfig.KustoClusterDestination == KustoClusterDestination.UNION && this.m_kustoQueryConfig.QueryType == KustoQueryType.MacroExpand ? KustoActivityLogQueryRestriction.GetUnionMacroExpandQuery(query) : query;
    }

    private string BuildFilteredViewExpression(
      IVssRequestContext requestContext,
      string sourceTable)
    {
      string str = string.Format("where HostId =~ '{0}'", (object) requestContext.ServiceHost.InstanceId);
      KustoTabularExpressionStatement expressionStatement = new KustoTabularExpressionStatement(this.GetTableName(sourceTable), new string[1]
      {
        str
      });
      return this.m_kustoQueryConfig.QueryType == KustoQueryType.MacroExpand ? (string) (KustoStatement) expressionStatement : string.Format("view () {{ {0} }}", (object) expressionStatement);
    }

    private static string GetUnionTableName(string tableName) => "vsoDb." + tableName;

    private static string GetVsoTableName(string tableName) => tableName;

    private string GetTableName(string tableName) => this.m_kustoQueryConfig.KustoClusterDestination == KustoClusterDestination.UNION ? KustoActivityLogQueryRestriction.GetUnionTableName(tableName) : KustoActivityLogQueryRestriction.GetVsoTableName(tableName);

    private static string GetUnionMacroExpandQuery(string query) => "macro-expand force_remote = true VsoUnion as vsoDb (" + query + ")";
  }
}
