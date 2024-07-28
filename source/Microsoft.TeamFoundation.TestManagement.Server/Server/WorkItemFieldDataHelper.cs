// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WorkItemFieldDataHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WorkItemFieldDataHelper : IWorkItemFieldDataHelper
  {
    private static readonly string[] m_populateResultsFromTestCasesFields = new string[11]
    {
      "System.Id",
      "System.Rev",
      "System.AreaPath",
      "System.AreaId",
      "System.Title",
      WorkItemFieldNames.Priority,
      "System.AssignedTo",
      WorkItemFieldNames.TestName,
      WorkItemFieldNames.Storage,
      WorkItemFieldNames.TestType,
      WorkItemFieldNames.TestId
    };
    internal const byte UnspecifiedPriority = 255;
    internal static readonly string DataTablePropertyName = "Microsoft.VSTS.TCM.LocalDataSource";

    public virtual int GetId(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem) => workItem.Id.Value;

    public virtual string GetTitle(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
      string title;
      workItem.Fields.TryGetValue<string>("System.Title", out title);
      return title;
    }

    public virtual int GetRevisionNumber(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem) => workItem.Rev.Value;

    public virtual int GetAreaId(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
      int areaId;
      workItem.Fields.TryGetValue<int>("System.AreaId", out areaId);
      return areaId;
    }

    public virtual int GetIterationId(WorkItemFieldData workItem) => workItem.IterationId;

    public virtual string GetType(WorkItemFieldData workItem) => workItem.WorkItemType;

    public virtual string GetState(WorkItemFieldData workItem) => workItem.State;

    public virtual IdentityRef GetAssignedTo(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
      IdentityRef assignedTo;
      workItem.Fields.TryGetValue<IdentityRef>(CoreFieldReferenceNames.AssignedTo, out assignedTo);
      return assignedTo;
    }

    public virtual TargetType GetFieldValue<TargetType>(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem,
      string propertyName)
    {
      TargetType fieldValue;
      workItem.Fields.TryGetValue<TargetType>(propertyName, out fieldValue);
      return fieldValue;
    }

    public virtual string GetAreaPath(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
      string areaPath;
      workItem.Fields.TryGetValue<string>("System.AreaPath", out areaPath);
      return areaPath;
    }

    public virtual string GetIterationPath(
      IVssRequestContext requestContext,
      WorkItemFieldData workItem)
    {
      return workItem.GetIterationPath(requestContext);
    }

    public void ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired(
      TestManagementRequestContext tcmContext,
      GuidAndString projectId,
      TestCaseResult[] results,
      bool populateDataRowCount)
    {
      using (PerfManager.Measure(tcmContext.RequestContext, "BusinessLayer", "TestCaseResult.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired"))
      {
        tcmContext.SecurityManager.CheckPublishTestResultsPermission(tcmContext, projectId.String);
        ArgumentUtility.CheckForNull<TestCaseResult[]>(results, nameof (results), "Test Results");
        results = ((IEnumerable<TestCaseResult>) results).Where<TestCaseResult>((System.Func<TestCaseResult, bool>) (result => result != null)).ToArray<TestCaseResult>();
        int[] testCaseIds = new int[results.Length];
        int index = 0;
        foreach (TestCaseResult result in results)
        {
          tcmContext.TraceAndDebugAssert("BusinessLayer", result.TestRunId != 0, "TestRunId == 0");
          if (result.TestCaseId != 0)
          {
            testCaseIds[index] = result.TestCaseId;
            ++index;
          }
        }
        this.PopulateResultsFromTestCases((TestManagementRequestContext) (tcmContext as TfsTestManagementRequestContext), projectId, results, testCaseIds, populateDataRowCount);
      }
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] PopulateTestResultFromWorkItem(
      TestManagementRequestContext tcmRequestContext,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testCaseResults,
      int testPlanId)
    {
      TfsTestRunDataContractConverter contractConverter = new TfsTestRunDataContractConverter(tcmRequestContext);
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> pointResultsDict = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultsWithoutPointId = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResults).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>().ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result =>
      {
        int result1;
        if (result.TestPoint != null && !string.IsNullOrEmpty(result.TestPoint.Id) && int.TryParse(result.TestPoint.Id, out result1))
          pointResultsDict[result1] = result;
        else
          testCaseResultsWithoutPointId.Add(result);
      }));
      RunCreateModel createModel = new RunCreateModel(pointIds: pointResultsDict.Keys.ToArray<int>());
      if (projectName != null && testPlanId > 0)
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults1;
        contractConverter.PopulatePlannedResultDetailsFromCreateModel(projectName, createModel, testPlanId, out testCaseResults1);
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in testCaseResults1)
        {
          int result;
          if (testCaseResult.TestPoint != null && !string.IsNullOrEmpty(testCaseResult.TestPoint.Id) && int.TryParse(testCaseResult.TestPoint.Id, out result))
          {
            pointResultsDict[result].TestCase = testCaseResult.TestCase;
            pointResultsDict[result].Area = testCaseResult.Area;
            pointResultsDict[result].TestCaseTitle = testCaseResult.TestCaseTitle;
            pointResultsDict[result].TestCaseRevision = testCaseResult.TestCaseRevision;
            pointResultsDict[result].Configuration = testCaseResult.Configuration;
            pointResultsDict[result].TestPoint = testCaseResult.TestPoint;
            pointResultsDict[result].Owner = testCaseResult.Owner;
            pointResultsDict[result].RunBy = testCaseResult.RunBy;
            pointResultsDict[result].AutomatedTestName = testCaseResult.AutomatedTestName;
            pointResultsDict[result].AutomatedTestStorage = testCaseResult.AutomatedTestStorage;
            pointResultsDict[result].AutomatedTestId = testCaseResult.AutomatedTestId;
            pointResultsDict[result].AutomatedTestType = testCaseResult.AutomatedTestType;
            pointResultsDict[result].AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId;
            pointResultsDict[result].Priority = testCaseResult.Priority;
          }
        }
      }
      return pointResultsDict.Values.Concat<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultsWithoutPointId).ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
    }

    public virtual void PopulateResultsFromTestCases(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestCaseResult[] results,
      int[] testCaseIds,
      bool populateDataRowCount)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseResult.PopulateResultsFromTestCases"))
      {
        TcmCommonStructureNodeInfo parameter = (TcmCommonStructureNodeInfo) null;
        Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> dictionary1 = (Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) null;
        Dictionary<int, int> dictionary2 = (Dictionary<int, int>) null;
        TfsTestManagementRequestContext context1 = context as TfsTestManagementRequestContext;
        results = ((IEnumerable<TestCaseResult>) results).Where<TestCaseResult>((System.Func<TestCaseResult, bool>) (result => result != null)).ToArray<TestCaseResult>();
        if (((IEnumerable<TestCaseResult>) results).Any<TestCaseResult>((System.Func<TestCaseResult, bool>) (result => result.TestCaseId > 0)))
        {
          context.TraceInfo("BusinessLayer", "PopulateResultsFromTestCases - Querying WIT start");
          IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, ((IEnumerable<int>) testCaseIds).Distinct<int>().ToList<int>(), ((IEnumerable<string>) WorkItemFieldDataHelper.m_populateResultsFromTestCasesFields).Union<string>((IEnumerable<string>) new string[1]
          {
            WorkItemFieldDataHelper.DataTablePropertyName
          }).ToList<string>());
          dictionary1 = workItems.ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int>) (fd => this.GetId(fd)));
          string[] strArray = (string[]) null;
          if (populateDataRowCount)
          {
            string tablePropertyName = WorkItemFieldDataHelper.DataTablePropertyName;
            if (tablePropertyName != null)
              strArray = new string[1]{ tablePropertyName };
          }
          if (strArray != null)
            dictionary2 = this.GetDataRowCountMap((TestManagementRequestContext) context1, workItems, (IEnumerable<int>) testCaseIds);
          context.TraceInfo("BusinessLayer", "PopulateResultsFromTestCases - Querying WIT end");
        }
        foreach (TestCaseResult result in results)
        {
          if (result.TestCaseId > 0)
          {
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem testCase;
            if (!dictionary1.TryGetValue(result.TestCaseId, out testCase))
              throw new TestObjectNotFoundException(context.RequestContext, result.TestCaseId, ObjectTypes.TestCase);
            this.PopulateResultFromTestCase((TestManagementRequestContext) context1, result, testCase);
            int num = 0;
            if (dictionary2 != null && dictionary2.TryGetValue(result.TestCaseId, out num))
              result.DataRowCount = num;
          }
          else if (string.IsNullOrEmpty(result.TestCaseAreaUri))
          {
            if (parameter == null)
            {
              context.TraceVerbose("BusinessLayer", "Getting area nodes using CSS");
              parameter = context.CSSHelper.GetRootNodes(projectId.String).FirstOrDefault<TcmCommonStructureNodeInfo>((System.Func<TcmCommonStructureNodeInfo, bool>) (node => node.StructureType == "ProjectModelHierarchy"));
            }
            context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) parameter, "areaRoot");
            context.IfEmptyThenTraceAndDebugFail("BusinessLayer", parameter.Uri, "areaRoot.Uri");
            result.TestCaseAreaUri = parameter.Uri;
          }
        }
        this.PopulateResultsOwnerField((TestManagementRequestContext) context1, results);
      }
    }

    private void PopulateResultsOwnerField(
      TestManagementRequestContext context,
      TestCaseResult[] results)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseResult.PopulateResultsOwnerField"))
      {
        Dictionary<string, Guid> ownerNames = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (TestCaseResult result in results)
        {
          if (result.TestCaseId > 0 && result.Owner == Guid.Empty && !string.IsNullOrEmpty(result.OwnerName) && !ownerNames.ContainsKey(result.OwnerName))
            ownerNames[result.OwnerName] = Guid.Empty;
        }
        if (ownerNames.Count == 0)
          return;
        List<string> list = ownerNames.Keys.ToList<string>();
        Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identities = IdentityHelper.SearchUsersByNames(context, list);
        list.ForEach((Action<string>) (n =>
        {
          if (!identities.ContainsKey(n))
            return;
          ownerNames[n] = identities[n].Id;
        }));
        foreach (TestCaseResult result in results)
        {
          if (result.TestCaseId > 0 && result.Owner == Guid.Empty && !string.IsNullOrEmpty(result.OwnerName) && ownerNames.ContainsKey(result.OwnerName))
            result.Owner = ownerNames[result.OwnerName];
        }
      }
    }

    internal Dictionary<int, int> GetDataRowCountMap(
      TestManagementRequestContext context,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> fieldData,
      IEnumerable<int> testCaseIds)
    {
      if (fieldData == null || testCaseIds == null || context == null)
        return (Dictionary<int, int>) null;
      Dictionary<int, int> dataRowCountMap = new Dictionary<int, int>();
      if (fieldData == null)
        return (Dictionary<int, int>) null;
      foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in fieldData)
      {
        if (workItem == null)
          return (Dictionary<int, int>) null;
        context.TraceAndDebugAssert("BusinessLayer", testCaseIds.Contains<int>(this.GetId(workItem)), "Wrong test case returned.");
        int dataCount = this.GetDataCount(context, workItem);
        dataRowCountMap.Add(this.GetId(workItem), dataCount);
      }
      return dataRowCountMap;
    }

    private int GetEmptyTrailingRowsCount(
      List<ISharedParameterDataRow> parameterDataRows,
      List<string> mappedSharedParameterNames)
    {
      int trailingRowsCount = 0;
      for (int index1 = parameterDataRows.Count - 1; index1 >= 0; --index1)
      {
        List<IKeyValuePair> parameterValues = parameterDataRows[index1].ParameterValues;
        for (int index2 = 0; index2 < parameterValues.Count; ++index2)
        {
          if (!string.IsNullOrEmpty(parameterValues[index2].Value) && mappedSharedParameterNames.Contains<string>(parameterValues[index2].Key, (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase))
            return trailingRowsCount;
        }
        ++trailingRowsCount;
      }
      return trailingRowsCount;
    }

    private int GetDataCount(TestManagementRequestContext context, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
      string fieldValue = this.GetFieldValue<string>(context.RequestContext, workItem, WorkItemFieldDataHelper.DataTablePropertyName);
      return this.GetDataCountFromString(context, fieldValue);
    }

    private int GetDataCountFromString(TestManagementRequestContext context, string dataString)
    {
      DataSet dataSetFromXml = WorkItemFieldDataHelper.GetDataSetFromXml(dataString);
      if (dataSetFromXml == null)
      {
        ISharedParameterData parameterDataFromJson = WorkItemFieldDataHelper.GetSharedParameterDataFromJson(context, dataString);
        if (parameterDataFromJson != null)
        {
          ISharedParameterDataRows parameterValues = parameterDataFromJson.ParameterValues;
          if (parameterValues != null)
          {
            List<ISharedParameterDataRow> rows = parameterValues.Rows;
            if (rows != null)
            {
              TestCaseParameterDataInfo parameterDataInfo = WorkItemFieldDataHelper.ReadParameterDataInfoFromJson(dataString);
              if (parameterDataInfo != null && parameterDataInfo.GetSharedParamNames() != null)
              {
                List<string> list = parameterDataInfo.GetSharedParamNames().Select<string, string>((System.Func<string, string>) (paramName => XmlConvert.EncodeName(paramName))).ToList<string>();
                return rows.Count - this.GetEmptyTrailingRowsCount(rows, list);
              }
            }
          }
        }
        return 0;
      }
      if (dataSetFromXml.Tables == null || dataSetFromXml.Tables.Count <= 0)
        return 0;
      if (dataSetFromXml.Tables[0].Rows.Count > 0)
        return dataSetFromXml.Tables[0].Rows.Count;
      return dataSetFromXml.Tables[0].Columns.Count > 0 ? 1 : 0;
    }

    private static ISharedParameterData GetSharedParameterDataFromJson(
      TestManagementRequestContext context,
      string dataRowValue)
    {
      try
      {
        if (!string.IsNullOrEmpty(dataRowValue))
        {
          TestCaseParameterDataInfo parameterDataInfo = WorkItemFieldDataHelper.ReadParameterDataInfoFromJson(dataRowValue);
          if (parameterDataInfo != null && parameterDataInfo.SharedParameterDataSetIds.Length != 0)
          {
            object obj;
            WorkItemFieldDataHelper.GetSharedParameterDataSetFromParameterDataInfo(context, parameterDataInfo).Fields.TryGetValue(WorkItemFieldNames.Parameters, out obj);
            string parametersFieldValue = (string) obj;
            return !string.IsNullOrEmpty(parametersFieldValue) ? (ISharedParameterData) WorkItemFieldDataHelper.GetSharedParameterDataFromParametersFieldValue(parametersFieldValue) : (ISharedParameterData) null;
          }
        }
        return (ISharedParameterData) null;
      }
      catch (InvalidOperationException ex)
      {
        return (ISharedParameterData) null;
      }
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetSharedParameterDataSetFromParameterDataInfo(
      TestManagementRequestContext context,
      TestCaseParameterDataInfo parameterDataInfo)
    {
      int num = ((IEnumerable<int>) parameterDataInfo.SharedParameterDataSetIds).First<int>();
      string[] source = new string[1]
      {
        WorkItemFieldNames.Parameters
      };
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      IVssRequestContext requestContext = context.RequestContext;
      List<int> ids = new List<int>();
      ids.Add(num);
      List<string> list = ((IEnumerable<string>) source).ToList<string>();
      return service.GetWorkItems(requestContext, ids, list).First<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
    }

    private static SharedParameterData GetSharedParameterDataFromParametersFieldValue(
      string parametersFieldValue)
    {
      parametersFieldValue1 = (SharedParameterData) null;
      using (XmlReader safeReader = XmlUtility.CreateSafeReader((TextReader) new StringReader(parametersFieldValue)))
      {
        object obj = new XmlSerializer(typeof (SharedParameterData)).Deserialize(safeReader);
        if (obj != null)
        {
          if (obj is SharedParameterData parametersFieldValue1)
            return parametersFieldValue1;
        }
      }
      return parametersFieldValue1;
    }

    private static DataSet GetDataSetFromXml(string dataRowXml)
    {
      DataSet dataSetFromXml = new DataSet();
      try
      {
        if (!string.IsNullOrEmpty(dataRowXml))
        {
          using (XmlTextReader safeXmlTextReader = XmlUtility.CreateSafeXmlTextReader((TextReader) new StringReader(dataRowXml)))
          {
            safeXmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
            int content = (int) safeXmlTextReader.MoveToContent();
            int num = (int) dataSetFromXml.ReadXml((XmlReader) safeXmlTextReader, XmlReadMode.ReadSchema);
            dataSetFromXml.AcceptChanges();
          }
        }
        return dataSetFromXml;
      }
      catch (Exception ex)
      {
        if (ex.GetType() == typeof (InvalidOperationException) || ex.GetType() == typeof (XmlException))
          return (DataSet) null;
        throw;
      }
    }

    private static TestCaseParameterDataInfo ReadParameterDataInfoFromJson(string dataRowValue)
    {
      try
      {
        return JsonConvert.DeserializeObject<TestCaseParameterDataInfo>(dataRowValue, new JsonSerializerSettings()
        {
          TypeNameHandling = TypeNameHandling.None
        });
      }
      catch (JsonReaderException ex)
      {
        return (TestCaseParameterDataInfo) null;
      }
    }

    private void PopulateResultFromTestCase(
      TestManagementRequestContext context,
      TestCaseResult result,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem testCase)
    {
      try
      {
        context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) testCase, nameof (testCase));
        context.TraceAndDebugAssert("BusinessLayer", result.TestCaseId == this.GetId(testCase), "Invalid test case id");
        IdAndString idAndThrow = context.AreaPathsCache.GetIdAndThrow(context, this.GetAreaPath(testCase));
        result.TestCaseAreaUri = idAndThrow.String;
        result.AreaId = idAndThrow.Id;
        result.TestCaseRevision = this.GetRevisionNumber(testCase);
        result.TestCaseTitle = this.GetTitle(testCase);
        result.AutomatedTestId = this.GetFieldValue<string>(context.RequestContext, testCase, WorkItemFieldNames.TestId);
        result.AutomatedTestName = this.GetFieldValue<string>(context.RequestContext, testCase, WorkItemFieldNames.TestName);
        result.AutomatedTestStorage = this.GetFieldValue<string>(context.RequestContext, testCase, WorkItemFieldNames.Storage);
        result.AutomatedTestType = this.GetFieldValue<string>(context.RequestContext, testCase, WorkItemFieldNames.TestType);
        if (result.Priority == byte.MaxValue)
        {
          int fieldValue = this.GetFieldValue<int>(context.RequestContext, testCase, WorkItemFieldNames.Priority);
          result.Priority = (byte) fieldValue;
        }
        if (!(result.Owner == Guid.Empty))
          return;
        IdentityRef assignedTo = this.GetAssignedTo(testCase);
        if (assignedTo == null)
          return;
        result.OwnerName = CommonUtils.DistinctDisplayName(assignedTo);
      }
      catch (TeamFoundationServerException ex1)
      {
        try
        {
          context.TraceError("BusinessLayer", "PopulateResultFromTestCase: Exception while processing CaseId={0} Area path='{2}'[{2}]", (object) testCase.Id, (object) this.GetAreaPath(testCase), (object) this.GetAreaId(testCase));
          context.TraceException("BusinessLayer", (Exception) ex1);
        }
        catch (Exception ex2)
        {
        }
        throw;
      }
    }
  }
}
