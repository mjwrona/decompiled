// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResultQueryAdapter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestResultQueryAdapter : QueryAdapter<TestCaseResult>
  {
    private List<string> referenceNames;

    public TestResultQueryAdapter(TestManagerRequestContext testContext)
      : base(testContext)
    {
      this.TestResultId = this.AddField(nameof (TestResultId), TestManagementServerResources.QueryColumnNameTestResultId, true, (Func<TestCaseResult, int>) (res => res.TestResultId), (Dictionary<int, string>) null);
      this.TestCaseId = this.AddField(nameof (TestCaseId), TestManagementServerResources.QueryColumnNameTestCaseId, true, (Func<TestCaseResult, int>) (res => res.TestCaseId), (Dictionary<int, string>) null);
      this.LastUpdated = this.AddField(nameof (LastUpdated), TestManagementServerResources.QueryColumnNameLastUpdated, true, (Func<TestCaseResult, DateTime>) (res => res.LastUpdated));
      this.ErrorMessage = this.AddField(nameof (ErrorMessage), TestManagementServerResources.QueryColumnNameErrorMessage, false, (Func<TestCaseResult, string>) (res => res.ErrorMessage));
      this.DateCompleted = this.AddField(nameof (DateCompleted), TestManagementResources.QueryColumnNameDateCompleted, true, (Func<TestCaseResult, DateTime>) (res => res.DateCompleted));
      this.Duration = this.AddTimeSpanField(nameof (Duration), TestManagementResources.QueryColumnNameDuration, true, (Func<TestCaseResult, long>) (res => res.Duration));
      this.Owner = this.InitializeOwner(testContext);
      this.Priority = this.AddField((QueryField<TestCaseResult>) new QueryPriorityField<TestCaseResult>(testContext, nameof (Priority), TestManagementServerResources.QueryColumnNamePriority, true, (Func<TestCaseResult, string>) (res => res.Priority != byte.MaxValue ? res.Priority.ToString() : string.Empty), (Dictionary<string, string>) null));
      this.TestCaseTitle = this.AddField(nameof (TestCaseTitle), TestManagementServerResources.QueryColumnNameTestCaseTitle, true, (Func<TestCaseResult, string>) (res => res.TestCaseTitle));
      this.ComputerName = this.AddField(nameof (ComputerName), TestManagementServerResources.QueryColumnNameComputerName, true, (Func<TestCaseResult, string>) (res => res.ComputerName));
      this.AutomatedTestName = this.AddField(nameof (AutomatedTestName), TestManagementServerResources.QueryColumnNameAutomatedTestName, true, (Func<TestCaseResult, string>) (res => res.AutomatedTestName));
      this.AutomatedTestStorage = this.AddField(nameof (AutomatedTestStorage), TestManagementServerResources.QueryColumnNameAutomatedTestStorage, true, (Func<TestCaseResult, string>) (res => res.AutomatedTestStorage));
      this.AutomatedTestType = this.AddField(nameof (AutomatedTestType), TestManagementServerResources.QueryColumnNameAutomatedTestType, true, (Func<TestCaseResult, string>) (res => res.AutomatedTestType));
      this.RunBy = this.AddTFIdentityField(nameof (RunBy), TestManagementServerResources.QueryColumnNameRunBy, true, (Func<TestCaseResult, Guid>) (res => res.RunBy));
      this.Outcome = this.AddField((QueryField<TestCaseResult>) new QueryEnumField<TestCaseResult, TestOutcome>(testContext, nameof (Outcome), TestManagementResources.QueryColumnNameOutcome, true, (Func<TestCaseResult, int>) (res => (int) res.Outcome), TestOutcomeConverter.GetEnumToDisplayNamesMap()));
      this.FailureType = this.AddField((QueryField<TestCaseResult>) new TestFailureTypeQueryField(testContext));
      this.ResolutionState = this.AddField((QueryField<TestCaseResult>) new TestResolutionStateQueryField(testContext));
      this.Configuration = this.AddField((QueryField<TestCaseResult>) new TestConfigurationQueryField(testContext));
      this.InitializeReferencenNames();
    }

    private void InitializeReferencenNames()
    {
      this.referenceNames = new List<string>();
      this.referenceNames.Add(this.Outcome.ReferenceName);
      this.referenceNames.Add(this.TestCaseTitle.ReferenceName);
      this.referenceNames.Add(this.Priority.ReferenceName);
      this.referenceNames.Add(this.Duration.ReferenceName);
      this.referenceNames.Add(this.Owner.ReferenceName);
      this.referenceNames.Add(this.Configuration.ReferenceName);
      this.referenceNames.Add(this.ComputerName.ReferenceName);
      this.referenceNames.Add(this.ErrorMessage.ReferenceName);
      this.referenceNames.Add(this.TestCaseId.ReferenceName);
    }

    private QueryField<TestCaseResult> InitializeOwner(TestManagerRequestContext testContext) => this.AddField("Owner", TestManagementServerResources.QueryColumnNameOwner, true, (Func<TestCaseResult, string>) (res => res.OwnerName));

    public QueryField<TestCaseResult> Outcome { get; private set; }

    public QueryField<TestCaseResult> TestCaseTitle { get; private set; }

    public QueryField<TestCaseResult> ErrorMessage { get; private set; }

    public QueryField<TestCaseResult> DateCompleted { get; private set; }

    public QueryField<TestCaseResult> Duration { get; private set; }

    public QueryField<TestCaseResult> Priority { get; private set; }

    public QueryField<TestCaseResult> Owner { get; private set; }

    public QueryField<TestCaseResult> TestResultId { get; private set; }

    public QueryField<TestCaseResult> TestCaseId { get; private set; }

    public QueryField<TestCaseResult> LastUpdated { get; private set; }

    public QueryField<TestCaseResult> ComputerName { get; private set; }

    public QueryField<TestCaseResult> FailureType { get; private set; }

    public QueryField<TestCaseResult> AutomatedTestName { get; private set; }

    public QueryField<TestCaseResult> AutomatedTestStorage { get; private set; }

    public QueryField<TestCaseResult> AutomatedTestType { get; private set; }

    public QueryField<TestCaseResult> RunBy { get; private set; }

    public QueryField<TestCaseResult> ResolutionState { get; private set; }

    public QueryField<TestCaseResult> Configuration { get; private set; }

    public override List<string> GetQueryResultItemIds(QueryModel query)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestResultQueryAdapter.GetQueryResultItemIds");
        int num = int.Parse(query.Id);
        StringBuilder stringBuilder1 = new StringBuilder();
        stringBuilder1.Append(TestManagementConstants.All_Results);
        StringBuilder stringBuilder2 = new StringBuilder();
        string filterString = this.GetFilterString(query.Filter);
        if (num > 0)
        {
          stringBuilder2.Append("(" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Results_TestRun_Clause, (object) num) + ")");
          if (!string.IsNullOrEmpty(filterString))
          {
            stringBuilder2.Append(" " + TestManagementConstants.Wiql_AND + " ");
            stringBuilder2.Append("(" + filterString + ")");
          }
        }
        else if (!string.IsNullOrEmpty(filterString))
          stringBuilder2.Append("(" + filterString + ")");
        if (!string.IsNullOrEmpty(filterString) || num > 0)
          stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_Where_Clause, (object) ("(" + stringBuilder2?.ToString() + ")")));
        string orderByString = this.GetOrderByString(query.Columns == null ? (IEnumerable<QuerySortColumn>) null : query.Columns.SortColumns);
        if (!string.IsNullOrEmpty(orderByString))
          stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.OrderBy_Clause, (object) orderByString));
        Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery resultsStoreQuery = this.TestContext.GetResultsStoreQuery(stringBuilder1.ToString());
        List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
        List<LegacyTestCaseResultIdentifier> excessIds;
        new Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext).GetTestResultsByQuery((TestManagementRequestContext) this.TestContext.TestRequestContext, ResultsStoreQueryContractConverter.Convert(resultsStoreQuery), 0, out excessIds);
        IEnumerable<TestCaseResultIdentifier> source = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds);
        return (source != null ? (IEnumerable<TestCaseResultIdentifier>) source.ToList<TestCaseResultIdentifier>() : (IEnumerable<TestCaseResultIdentifier>) null).Select<TestCaseResultIdentifier, string>((Func<TestCaseResultIdentifier, string>) (resId => TestResultsHelper.GetTestResultId(resId.TestRunId, resId.TestResultId))).ToList<string>();
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestResultQueryAdapter.GetQueryResultItemIds");
      }
    }

    public override List<TestCaseResult> FetchItemsByIds(IEnumerable<string> resultIds)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestResultQueryAdapter.FetchItemsByIds");
        return new TestResultHelper(this.TestContext).GetTestCaseResults(resultIds.Select<string, TestCaseResultIdentifier>((Func<string, TestCaseResultIdentifier>) (resId => TestResultsHelper.GetTestCaseResultIdentifier(resId))).Select<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>((Func<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>) (id => new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev(id, 0))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>(), out List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> _, out List<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> _, out List<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> _);
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestResultQueryAdapter.FetchItemsByIds");
      }
    }

    protected override QueryResultDataRowModel CreateDataRowWithId(TestCaseResult item)
    {
      TestResultQueryResultDataRowModel dataRowWithId = new TestResultQueryResultDataRowModel();
      dataRowWithId.Id = TestResultsHelper.GetTestResultId(item.TestRunId, item.TestResultId);
      dataRowWithId.Title = item.TestCaseTitle;
      dataRowWithId.Outcome = item.Outcome;
      return (QueryResultDataRowModel) dataRowWithId;
    }

    public override QueryColumnInfoModel GetTitleColumn() => throw new NotImplementedException();

    public override FilterModel GetTestRunTitleFilter() => throw new NotImplementedException();

    public List<string> GetDefaultColumnReferenceNames() => this.referenceNames;

    public override QueryColumnInfoModel GetDefaultColumns()
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestResultQueryAdapter.GetDefaultColumns");
        QueryColumnInfoModel defaultColumns = new QueryColumnInfoModel();
        List<QueryDisplayColumn> queryDisplayColumnList = new List<QueryDisplayColumn>();
        List<QuerySortColumn> querySortColumnList = new List<QuerySortColumn>();
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.Outcome.DisplayName,
          Width = 115
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.TestCaseTitle.DisplayName,
          Width = 430
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.Priority.DisplayName,
          Width = 50
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.Duration.DisplayName,
          Width = 100
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.Owner.DisplayName,
          Width = 110
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.Configuration.DisplayName,
          Width = 115
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.ComputerName.DisplayName,
          Width = 110
        });
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = this.ErrorMessage.DisplayName,
          Width = 430,
          UnSortable = true
        });
        defaultColumns.DisplayColumns = (IEnumerable<QueryDisplayColumn>) queryDisplayColumnList;
        defaultColumns.SortColumns = (IEnumerable<QuerySortColumn>) querySortColumnList;
        return defaultColumns;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestResultQueryAdapter.GetDefaultColumns");
      }
    }
  }
}
