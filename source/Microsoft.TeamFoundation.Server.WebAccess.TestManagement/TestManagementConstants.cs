// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementConstants
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public static class TestManagementConstants
  {
    public static readonly string Wiql_Where_Clause = "WHERE {0}";
    public static readonly string Wiql_And_Clause = "AND {0}";
    public static readonly string Wiql_All_TestPlanOnTestPlanTable = "SELECT * FROM TestPlan";
    public static readonly string Wiql_All_TestPlan = "SELECT * FROM WorkItems WHERE [System.TeamProject] = @project AND [System.WorkItemType] IN GROUP '" + WitCategoryRefName.TestPlan + "'";
    public static readonly string OrderBy_Clause = " ORDER BY {0}";
    public static readonly string Wiql_All_Points = "SELECT * FROM TestPoint ";
    public static readonly string Wiql_All_Points_InASuite = TestManagementConstants.Wiql_All_Points + " WHERE SuiteId = {0} ";
    public static readonly string Wiql_All_Points_InASuite_Recursive = TestManagementConstants.Wiql_All_Points + " WHERE RecursiveSuiteId = {0}";
    public static readonly string Wiql_Points_OutCome_Clause = " [LastResultOutcome] = '{0}'";
    public static readonly string Wiql_Points_OutCome_Not_Clause = " [LastResultOutcome] <> '{0}'";
    public static readonly string Wiql_Points_State_Clause = " [PointState] = '{0}'";
    public static readonly string Wiql_Points_AssignedTo_Clause = " (AssignedTo = '{0}')";
    public static readonly string Wiql_Points_Configuration_Clause = "[ConfigurationId] = {0}";
    public static readonly string All_Results = "SELECT * FROM TestResult ";
    public static readonly string All_Runs = "SELECT * FROM TestRun ";
    public static readonly string Results_TestRun_Clause = " [TestRunId] = {0}";
    public static readonly string Wiql_All_Configurations = "SELECT * FROM TestConfiguration";
    public static readonly string Wiql_All_TestSettings = "SELECT * FROM TestSettings";
    public static readonly string Wiql_AND = "AND";
    public static readonly string Wiql_OR = "OR";
    public static readonly int c_firstTestCaseResultId = 100000;
    public const string TestManagementChartsFavoriteId = "Microsoft.Teamfoundation.TestManagement.Charts";
    public const string TestManagementChartsPinScope = "Pinning.TestManagement.Charts";
    public const int RECENT_RUNS = 5;
    public const string QueryItem_TestRun = "TestRun";
    public const string QueryItem_TestResult = "TestResult";
    public const string RECENT_RUNS_QUERY_ID = "8AC4BE34-5582-43AF-8830-6797C47F5870";
    public const string RECENT_RESULT_ROOT_QUERY_ID = "6618FEF0-354F-4CDF-960A-D01E5E00B173";
    public const string RUNS_TITLE_QUERY_ID = "3018B3CE-1579-4538-9556-A6092E4C1E1A";
    public const int c_INITIAL_PAYLOAD_SIZE = 200;

    public static string AllPointsInASuite(int suiteId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_All_Points_InASuite, (object) suiteId);

    public static string AllPointsInASuiteRecursive(int suiteId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_All_Points_InASuite_Recursive, (object) suiteId);
  }
}
