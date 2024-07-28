// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestRunQueryAdapter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestRunQueryAdapter : QueryAdapter<Microsoft.TeamFoundation.TestManagement.Server.TestRun>
  {
    public TestRunQueryAdapter(TestManagerRequestContext testContext)
      : base(testContext)
    {
      TestRunQueryAdapter testRunQueryAdapter = this;
      this.BuildNumber = this.AddField(nameof (BuildNumber), TestManagementServerResources.QueryColumnNameBuildNumber, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, string>) (run => run.BuildNumber));
      this.BuildPlatform = this.AddField(nameof (BuildPlatform), TestManagementServerResources.QueryColumnNameBuildPlatform, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, string>) (run => run.BuildPlatform));
      this.BuildFlavor = this.AddField(nameof (BuildFlavor), TestManagementServerResources.QueryColumnNameBuildFlavor, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, string>) (run => run.BuildFlavor));
      this.Comment = this.AddField(nameof (Comment), TestManagementServerResources.QueryColumnNameComment, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, string>) (run => run.Comment));
      this.CompleteDate = this.AddField(nameof (CompleteDate), TestManagementServerResources.QueryColumnNameCompleteDate, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, DateTime>) (run => testContext.TfsRequestContext.GetTimeZone().ConvertToLocal(run.CompleteDate)));
      this.CreationDate = this.AddField(nameof (CreationDate), TestManagementServerResources.QueryColumnNameCreationDate, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, DateTime>) (run => testContext.TfsRequestContext.GetTimeZone().ConvertToLocal(run.CreationDate)));
      this.FailedTests = this.AddField(nameof (FailedTests), TestManagementServerResources.QueryColumnNameFailedTests, false, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, int>) (run =>
      {
        int num = run.TotalTests - (run.PassedTests + run.NotApplicableTests + run.IncompleteTests);
        return num < 0 ? 0 : num;
      }), (Dictionary<int, string>) null);
      this.NotApplicableTests = this.AddField(nameof (NotApplicableTests), TestManagementServerResources.QueryColumnNameNotApplicableTests, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, int>) (run => run.NotApplicableTests), (Dictionary<int, string>) null);
      this.PassedTests = this.AddField(nameof (PassedTests), TestManagementServerResources.QueryColumnNamePassedTests, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, int>) (run => run.PassedTests), (Dictionary<int, string>) null);
      this.State = this.AddField((QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) new QueryEnumField<Microsoft.TeamFoundation.TestManagement.Server.TestRun, Microsoft.TeamFoundation.TestManagement.Client.TestRunState>(testContext, nameof (State), TestManagementResources.QueryColumnNameState, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, int>) (run => (int) run.State), TestRunStateConverter.GetEnumToDisplayNamesMap()));
      this.RunId = this.AddField("TestRunId", TestManagementServerResources.QueryColumnNameTestRunId, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, int>) (run => run.TestRunId), (Dictionary<int, string>) null);
      this.Title = this.AddField(nameof (Title), TestManagementServerResources.QueryColumnNameTitle, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, string>) (run => run.Title));
      this.TotalTests = this.AddField(nameof (TotalTests), TestManagementServerResources.QueryColumnNameTotalTests, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, int>) (run => run.TotalTests), (Dictionary<int, string>) null);
      this.IncompleteTests = this.AddField(nameof (IncompleteTests), TestManagementServerResources.QueryColumnNameIncompleteTests, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, int>) (run => run.IncompleteTests), (Dictionary<int, string>) null);
      this.PassRate = this.AddField(nameof (PassRate), TestManagementServerResources.QueryColumnNamePassRate, false, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, string>) (run => run.TotalTests != 0 ? string.Format("{0:0.#}%", (object) TestRunQueryAdapter.RoundDown(testRunQueryAdapter.GetPassRate(run) * 100f, 1)) : string.Empty));
      this.Owner = this.AddTFIdentityField(nameof (Owner), TestManagementServerResources.QueryColumnNameOwner, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, Guid>) (run => run.Owner));
      this.IsAutomated = this.AddField(nameof (IsAutomated), TestManagementServerResources.QueryColumnNameIsAutomated, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, bool>) (run => run.IsAutomated));
      this.ErrorMessage = this.AddField(nameof (ErrorMessage), TestManagementServerResources.QueryColumnNameErrorMessage, true, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, string>) (run => run.ErrorMessage));
      this.TestSettings = this.AddField((QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) new TestSettingsTypeQueryField(this.TestContext));
    }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> State { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> Title { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> RunId { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> CompleteDate { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> CreationDate { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> TotalTests { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> PassedTests { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> NotApplicableTests { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> FailedTests { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> Comment { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> BuildNumber { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> PassRate { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> Owner { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> BuildPlatform { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> BuildFlavor { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> IncompleteTests { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> IsAutomated { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> ErrorMessage { get; private set; }

    public QueryField<Microsoft.TeamFoundation.TestManagement.Server.TestRun> TestSettings { get; private set; }

    public override QueryColumnInfoModel GetTitleColumn()
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestRunQueryAdapter.GetTitleColumn");
        QueryColumnInfoModel titleColumn = new QueryColumnInfoModel();
        List<QueryDisplayColumn> queryDisplayColumnList = new List<QueryDisplayColumn>();
        List<QuerySortColumn> querySortColumnList = new List<QuerySortColumn>();
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.Title.DisplayName,
          Width = 50
        });
        titleColumn.DisplayColumns = (IEnumerable<QueryDisplayColumn>) queryDisplayColumnList;
        titleColumn.SortColumns = (IEnumerable<QuerySortColumn>) querySortColumnList;
        return titleColumn;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestRunQueryAdapter.GetTitleColumn");
      }
    }

    public override QueryColumnInfoModel GetDefaultColumns()
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestRunQueryAdapter.GetDefaultColumns");
        QueryColumnInfoModel defaultColumns = new QueryColumnInfoModel();
        List<QueryDisplayColumn> queryDisplayColumnList = new List<QueryDisplayColumn>();
        List<QuerySortColumn> querySortColumnList = new List<QuerySortColumn>();
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.State.DisplayName,
          Width = 135
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.RunId.DisplayName,
          Width = 50
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.Title.DisplayName,
          Width = 450
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.CompleteDate.DisplayName,
          Width = 180
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.BuildNumber.DisplayName,
          Width = 200
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.FailedTests.DisplayName,
          Width = 80,
          UnSortable = true
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.PassRate.DisplayName,
          Width = 80,
          UnSortable = true
        });
        querySortColumnList.Add(new QuerySortColumn()
        {
          Name = this.RunId.DisplayName,
          Order = "desc"
        });
        defaultColumns.DisplayColumns = (IEnumerable<QueryDisplayColumn>) queryDisplayColumnList;
        defaultColumns.SortColumns = (IEnumerable<QuerySortColumn>) querySortColumnList;
        return defaultColumns;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestRunQueryAdapter.GetDefaultColumns");
      }
    }

    public override FilterModel GetDefaultFilter() => new FilterModel()
    {
      Clauses = (ICollection<FilterClause>) new List<FilterClause>()
      {
        new FilterClause()
        {
          FieldName = this.CreationDate.DisplayName,
          Operator = this.OperatorCollection.GTE.DisplayName,
          Value = string.Format("{0} - 7", (object) TestManagementServerResources.TodayMacro)
        }
      }
    };

    public override FilterModel GetTestRunTitleFilter() => new FilterModel()
    {
      Clauses = (ICollection<FilterClause>) new List<FilterClause>()
      {
        new FilterClause()
        {
          FieldName = this.RunId.DisplayName,
          Operator = this.OperatorCollection.EqualTo.DisplayName,
          Value = string.Empty
        }
      }
    };

    public override List<string> GetQueryResultItemIds(QueryModel query)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestRunQueryAdapter.GetQueryResultItemIds");
        string orderByString = this.GetOrderByString(query.Columns == null ? (IEnumerable<QuerySortColumn>) null : query.Columns.SortColumns);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(TestManagementConstants.All_Runs);
        string filterString = this.GetFilterString(query.Filter);
        if (!string.IsNullOrEmpty(filterString))
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_Where_Clause, (object) ("(" + filterString + ")")));
        if (!string.IsNullOrEmpty(orderByString))
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.OrderBy_Clause, (object) orderByString));
        ResultsStoreQuery resultsStoreQuery = this.TestContext.GetResultsStoreQuery(stringBuilder.ToString());
        List<string> first = new List<string>();
        this.TestContext.Trace(1015090, TraceLevel.Verbose, "TestResultsInsights", "BusinessLayer", "Wiql Query: " + resultsStoreQuery.QueryText);
        if (!TCMServiceDataMigrationRestHelper.IsMigrationCompleted(this.TestContext.TestRequestContext.RequestContext))
          first = Microsoft.TeamFoundation.TestManagement.Server.TestRun.QueryTestRunIds((TestManagementRequestContext) this.TestContext.TestRequestContext, resultsStoreQuery).Select<int, string>((Func<int, string>) (id => id.ToString())).ToList<string>();
        if (this.TestContext.TestRequestContext.IsFeatureEnabled("WebAccess.TestManagement.QueryRunsHubDataFromTcmService"))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.QueryModel queryModel = new Microsoft.TeamFoundation.TestManagement.WebApi.QueryModel(stringBuilder.ToString());
          IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRuns = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
          bool flag = true;
          if (!this.TestContext.TestRequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
            flag = this.TestContext.TestRequestContext.TcmServiceHelper.TryGetTestRunsByQuery(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, queryModel, true, false, 0, int.MaxValue, out testRuns);
          else
            testRuns = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() => this.TcmHttpClient.GetTestRunsByQueryAsync(queryModel, this.TestContext.CurrentProjectGuid, true, new bool?(false), new int?(0), new int?(int.MaxValue))?.Result));
          if (flag)
            first = first.Concat<string>(testRuns.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun, string>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun, string>) (run => run.Id.ToString()))).ToList<string>();
        }
        return first;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestRunQueryAdapter.GetQueryResultItemIds");
      }
    }

    private static float RoundDown(float passRate, int numberOfDigits)
    {
      int num = (int) Math.Pow(10.0, (double) numberOfDigits);
      return (float) Math.Truncate((double) passRate * (double) num) / (float) num;
    }

    private float GetPassRate(Microsoft.TeamFoundation.TestManagement.Server.TestRun run) => run.TotalTests == 0 ? 0.0f : (float) run.PassedTests / (float) run.TotalTests;

    public override List<Microsoft.TeamFoundation.TestManagement.Server.TestRun> FetchItemsByIds(
      IEnumerable<string> runIds)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestRunQueryAdapter.FetchTestRuns");
        if (!runIds.Any<string>())
          return new List<Microsoft.TeamFoundation.TestManagement.Server.TestRun>();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(TestManagementConstants.All_Runs);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[TestRunId] IN ({0})", (object) string.Join(",", runIds));
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_Where_Clause, (object) ("(" + str + ")")));
        List<Microsoft.TeamFoundation.TestManagement.Server.TestRun> first = new List<Microsoft.TeamFoundation.TestManagement.Server.TestRun>();
        if (this.TestContext.TestRequestContext.IsFeatureEnabled("WebAccess.TestManagement.QueryRunsHubDataFromTcmService"))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.QueryModel queryModel = new Microsoft.TeamFoundation.TestManagement.WebApi.QueryModel(stringBuilder.ToString());
          IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRuns = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
          bool flag = true;
          if (!this.TestContext.TestRequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
            flag = this.TestContext.TestRequestContext.TcmServiceHelper.TryGetTestRunsByQuery(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, queryModel, false, true, 0, runIds.Count<string>(), out testRuns);
          else
            testRuns = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() => this.TcmHttpClient.GetTestRunsByQueryAsync(queryModel, this.TestContext.CurrentProjectGuid, false, new bool?(true), new int?(0), new int?(runIds.Count<string>()))?.Result));
          if (flag)
            first = testRuns.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun, Microsoft.TeamFoundation.TestManagement.Server.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun, Microsoft.TeamFoundation.TestManagement.Server.TestRun>) (run => this.ConvertFromWebApiTestRun(run))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>();
        }
        if (runIds.Count<string>() > first.Count && !TCMServiceDataMigrationRestHelper.IsMigrationCompleted(this.TestContext.TestRequestContext.RequestContext))
        {
          ResultsStoreQuery resultsStoreQuery = this.TestContext.GetResultsStoreQuery(stringBuilder.ToString());
          first = first.Concat<Microsoft.TeamFoundation.TestManagement.Server.TestRun>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) Microsoft.TeamFoundation.TestManagement.Server.TestRun.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, resultsStoreQuery)).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>();
        }
        return first;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestRunQueryAdapter.FetchTestRuns");
      }
    }

    protected override QueryResultDataRowModel CreateDataRowWithId(Microsoft.TeamFoundation.TestManagement.Server.TestRun item)
    {
      TestRunQueryResultDataRowModel dataRowWithId = new TestRunQueryResultDataRowModel();
      dataRowWithId.Id = item.TestRunId.ToString();
      dataRowWithId.Title = item.Title;
      dataRowWithId.State = item.State;
      return (QueryResultDataRowModel) dataRowWithId;
    }

    private Microsoft.TeamFoundation.TestManagement.Server.TestRun ConvertFromWebApiTestRun(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun webApiRun)
    {
      Microsoft.TeamFoundation.TestManagement.Server.TestRun testRun = new Microsoft.TeamFoundation.TestManagement.Server.TestRun();
      testRun.TestRunId = webApiRun.Id;
      testRun.Revision = webApiRun.Revision;
      testRun.Iteration = webApiRun.Iteration;
      testRun.Title = webApiRun.Name;
      testRun.IsAutomated = webApiRun.IsAutomated;
      testRun.TotalTests = webApiRun.TotalTests;
      testRun.IncompleteTests = webApiRun.IncompleteTests;
      testRun.NotApplicableTests = webApiRun.NotApplicableTests;
      testRun.PassedTests = webApiRun.PassedTests;
      testRun.UnanalyzedTests = webApiRun.UnanalyzedTests;
      testRun.BuildNumber = webApiRun.BuildConfiguration?.Number ?? webApiRun.Build?.Id;
      testRun.CompleteDate = webApiRun.CompletedDate;
      testRun.StartDate = webApiRun.StartedDate;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState result;
      if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState>(webApiRun.State, true, out result))
        testRun.State = (byte) result;
      return testRun;
    }

    internal TcmHttpClient TcmHttpClient => this.TestContext.TestRequestContext.RequestContext.GetClient<TcmHttpClient>();
  }
}
