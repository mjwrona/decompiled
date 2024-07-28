// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestPointStatistic
  {
    private const int TestManagementStart = 1015000;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte TestPointState { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte ResultState { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte ResultOutcome { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte FailureType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int ResolutionStateId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int Count { get; set; }

    internal static List<TestPointStatistic> Query(
      TestManagementRequestContext context,
      int planId,
      ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
      string whereClause = string.Empty;
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectUriFromName))
        return new List<TestPointStatistic>();
      try
      {
        TestPointStatistic.LogQueryInformation(context, query);
        QueryEngine queryEngine = (QueryEngine) new ServerQueryEngine(context, query, TestPlanningWiqlConstants.TestPlanningTablesForWiql);
        if (planId > 0)
          queryEngine.AppendClause("PlanId = " + planId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        whereClause = queryEngine.GenerateWhereClauseInMultipleProjects();
        List<KeyValuePair<int, string>> valueLists = queryEngine.GenerateValueLists();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          return planningDatabase.QueryTestPointStatistics(whereClause, valueLists);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(1015000, TraceLevel.Error, "TestManagement", "QueryEngine", "QueryTestPointStatistics Error. Query:{0} WhereClause:{1}", (object) query.ToString(), (object) whereClause);
        throw;
      }
    }

    private static void LogQueryInformation(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Query", (object) query?.ToString());
      properties.Add("Command", (object) context.RequestContext.Command());
      context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", "QueryTestPointStatistics", properties);
    }
  }
}
