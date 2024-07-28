// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementPointService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TeamFoundationTestManagementPointService : 
    TfsTestManagementService,
    ITeamFoundationTestManagementPointService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementPointService()
    {
    }

    public TeamFoundationTestManagementPointService(TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
    }

    public TestPointsQuery GetPointsByQuery(
      IVssRequestContext requestContext,
      string projectId,
      TestPointsQuery query,
      int skip,
      int top,
      bool includeNames = false)
    {
      requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementPointService.GetPointsByQuery projectId = {0}, skip = {1}, skip = {2}", (object) projectId, (object) skip, (object) top);
      return this.ExecuteAction<TestPointsQuery>(requestContext, "TeamFoundationTestManagementPointService.GetPointsByQuery", (Func<TestPointsQuery>) (() =>
      {
        this.ValidateInputForTestPointQuery(query, skip, top);
        TfsTestManagementRequestContext managementRequestContext = this.GetTfsTestManagementRequestContext(requestContext);
        TeamProjectReference projectReference = this.GetProjectReference(requestContext, projectId);
        string name = projectReference.Name;
        List<TestPoint> testPointList1 = new List<TestPoint>();
        IList<string> witFields = query.WitFields;
        List<string> workItemFields = this.AddDefaultWorkItemFields((witFields != null ? witFields.ToList<string>() : (List<string>) null) ?? new List<string>());
        IList<IdentityRef> testers = query.PointsFilter.Testers;
        List<string> displayNames = (testers != null ? testers.Where<IdentityRef>((Func<IdentityRef, bool>) (t => !string.IsNullOrEmpty(t.DisplayName))).ToList<IdentityRef>().Select<IdentityRef, string>((Func<IdentityRef, string>) (tester => tester.DisplayName)).Distinct<string>().ToList<string>() : (List<string>) null) ?? new List<string>();
        List<Guid> list = this.GetTfsIdentitiesByNames((TestManagementRequestContext) managementRequestContext, displayNames).Values.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id)).ToList<Guid>();
        IList<string> configurationNames = query.PointsFilter.ConfigurationNames;
        List<string> stringList = (configurationNames != null ? configurationNames.ToList<string>() : (List<string>) null) ?? new List<string>();
        List<TestPoint> pointsByQuery = TestPoint.GetPointsByQuery((TestManagementRequestContext) managementRequestContext, name, query.PointsFilter.TestcaseIds.Distinct<int>().ToArray<int>(), stringList.ToArray(), ((IEnumerable<Guid>) list.ToArray()).ToArray<Guid>(), workItemFields.ToArray(), skip, top, includeNames);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPointList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
        TestPointDataContractConverter contractConverter = new TestPointDataContractConverter();
        foreach (TestPoint testPoint in pointsByQuery)
        {
          string resourceUrl = testPoint != null ? UrlBuildHelper.GetResourceUrl(requestContext, TestManagementServerConstants.TFSServiceInstanceType, "Test", TestManagementResourceIds.TestPointProject, (object) new
          {
            pointIds = testPoint.PointId,
            project = name,
            planId = testPoint.PlanId,
            suiteId = testPoint.SuiteId
          }) : (string) null;
          testPointList2.Add(contractConverter.ConvertTestPointToDataContract(testPoint, projectReference, workItemFields, resourceUrl));
        }
        query.Points = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>) testPointList2;
        return query;
      }), 1015060, "TestManagement", "BusinessLayer");
    }

    public List<TestPoint> GetPoints(
      IVssRequestContext tfsRequestContext,
      string projectName,
      int planId,
      List<int> pointIds,
      string[] testCaseProperties)
    {
      return this.ExecuteAction<List<TestPoint>>(tfsRequestContext, "TeamFoundationTestManagementPointService.GetPoints", (Func<List<TestPoint>>) (() =>
      {
        this.CheckForViewTestResultPermission((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(tfsRequestContext), projectName);
        List<TestPoint> testPointList = new List<TestPoint>();
        List<int> deletedIds = new List<int>();
        return TestPoint.Fetch((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(tfsRequestContext), projectName, planId, pointIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (p => new IdAndRev(p, 0))).ToArray<IdAndRev>(), testCaseProperties, deletedIds);
      }), 1015060, "TestManagement", "BusinessLayer");
    }

    public List<TestPointRecord> QueryTestPointsByOutcomeMigrationDate(
      IVssRequestContext requestContext,
      int batchSize,
      TestPointWatermark fromWatermark,
      out TestPointWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      string str = "TeamFoundationTestManagementPointService.QueryTestPointsByOutcomeMigrationDate";
      try
      {
        requestContext.TraceEnter(1015060, "TestManagement", "BusinessLayer", str);
        List<TestPointRecord> source;
        using (PerfManager.Measure(requestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestPlanningDatabase replicaAwareComponent = TestPlanningDatabase.CreateReadReplicaAwareComponent(requestContext))
            source = replicaAwareComponent.QueryTestPointsByOutcomeMigrationDate(batchSize, fromWatermark, out toWatermark, dataSource);
        }
        this.UpdateTesterIdentites(requestContext, source.Cast<TestPointHistoryRecord>().ToList<TestPointHistoryRecord>());
        return source;
      }
      finally
      {
        requestContext.TraceLeave(1015060, "TestManagement", "BusinessLayer", str);
      }
    }

    public List<TestPointHistoryRecord> QueryTestPointHistoryByWatermarkDate(
      IVssRequestContext requestContext,
      int batchSize,
      TestPointHistoryWatermark fromWatermark,
      out TestPointHistoryWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      string str = "TeamFoundationTestManagementPointService.QueryTestPointHistoryByWatermarkDate";
      try
      {
        requestContext.TraceEnter(1015060, "TestManagement", "BusinessLayer", str);
        List<TestPointHistoryRecord> pointRecords;
        using (PerfManager.Measure(requestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestPlanningDatabase replicaAwareComponent = TestPlanningDatabase.CreateReadReplicaAwareComponent(requestContext))
            pointRecords = replicaAwareComponent.QueryTestPointHistoryByWatermarkDate(batchSize, fromWatermark, out toWatermark, dataSource);
        }
        this.UpdateTesterIdentites(requestContext, pointRecords);
        return pointRecords;
      }
      finally
      {
        requestContext.TraceLeave(1015060, "TestManagement", "BusinessLayer", str);
      }
    }

    private void ValidateInputForTestPointQuery(TestPointsQuery query, int skip, int top)
    {
      ArgumentUtility.CheckForNull<TestPointsQuery>(query, "TestPointQuery");
      ArgumentUtility.CheckForNull<PointsFilter>(query.PointsFilter, "PointsFilter");
      ArgumentUtility.CheckForNull<IList<int>>(query.PointsFilter.TestcaseIds, "TestcaseIds");
      ArgumentUtility.CheckForOutOfRange(query.PointsFilter.TestcaseIds.Count, "TestcaseIds", 1);
      if (query.PointsFilter.TestcaseIds.Count > 200)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxTestCaseIdsError, (object) 200));
      if (query.PointsFilter.ConfigurationNames != null)
        ArgumentUtility.CheckForOutOfRange(query.PointsFilter.ConfigurationNames.Count, "ConfigurationNames", 0, 200);
      if (query.PointsFilter.Testers != null)
        ArgumentUtility.CheckForOutOfRange(query.PointsFilter.Testers.Count, "Testers", 0, 200);
      ArgumentUtility.CheckForOutOfRange(top, nameof (top), 0, 1000);
      ArgumentUtility.CheckForNonnegativeInt(skip, nameof (skip));
    }

    private List<string> AddDefaultWorkItemFields(List<string> workItemFields)
    {
      if (!workItemFields.Contains(WorkItemFieldNames.AutomationStatus))
        workItemFields.Add(WorkItemFieldNames.AutomationStatus);
      return workItemFields;
    }

    private void UpdateTesterIdentites(
      IVssRequestContext requestContext,
      List<TestPointHistoryRecord> pointRecords)
    {
      try
      {
        List<Guid> list = pointRecords.Select<TestPointHistoryRecord, Guid>((Func<TestPointHistoryRecord, Guid>) (record => (Guid?) record.Tester?.Id ?? Guid.Empty)).Distinct<Guid>().Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty)).ToList<Guid>();
        Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentitiesEx(this.GetTestManagementRequestContext(requestContext), (IList<Guid>) list).ToDictionary<KeyValuePair<Guid, Tuple<string, string>>, Guid, string>((Func<KeyValuePair<Guid, Tuple<string, string>>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, Tuple<string, string>>, string>) (x => x.Value == null ? (string) null : this.GetDistinctDisplayNameWithEmailId(x.Value.Item1, x.Value.Item2)));
        foreach (TestPointHistoryRecord pointRecord in pointRecords)
        {
          Guid key = (Guid?) pointRecord.Tester?.Id ?? Guid.Empty;
          if (key != Guid.Empty && dictionary.ContainsKey(key))
            pointRecord.Tester.DisplayName = dictionary[key];
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015060, "TestManagement", "BusinessLayer", ex);
      }
    }

    private string GetDistinctDisplayNameWithEmailId(string displayName, string uniqueName)
    {
      string[] strArray1;
      if (uniqueName == null)
        strArray1 = (string[]) null;
      else
        strArray1 = uniqueName.Split('\\');
      string[] strArray2 = strArray1;
      uniqueName = strArray2 == null || strArray2.Length != 2 ? uniqueName : strArray2[1];
      return IdentityHelper.GetDistinctDisplayName(displayName, uniqueName);
    }
  }
}
