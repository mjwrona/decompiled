// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultWorkItemDataHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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
  internal class DefaultWorkItemDataHelper : IWorkItemFieldDataHelper
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

    public void ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestCaseResult[] results,
      bool populateDataRowCount)
    {
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] PopulateTestResultFromWorkItem(
      TestManagementRequestContext tcmRequestContext,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testCaseResults,
      int testPlanId)
    {
      return testCaseResults;
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
        Dictionary<int, WorkItem> dictionary1 = (Dictionary<int, WorkItem>) null;
        Dictionary<int, int> dictionary2 = (Dictionary<int, int>) null;
        if (((IEnumerable<TestCaseResult>) results).Any<TestCaseResult>((System.Func<TestCaseResult, bool>) (result => result.TestCaseId > 0)))
        {
          context.TraceInfo("BusinessLayer", "PopulateResultsFromTestCases - Querying WIT start");
          IList<WorkItem> workItems = context.WorkItemServiceHelper.GetWorkItems((IList<int>) ((IEnumerable<int>) testCaseIds).Distinct<int>().ToList<int>(), (IList<string>) ((IEnumerable<string>) DefaultWorkItemDataHelper.m_populateResultsFromTestCasesFields).Union<string>((IEnumerable<string>) new string[1]
          {
            DefaultWorkItemDataHelper.DataTablePropertyName
          }).ToList<string>(), WorkItemErrorPolicy.Omit);
          dictionary1 = workItems.Where<WorkItem>((System.Func<WorkItem, bool>) (wi => wi.Id.HasValue)).ToDictionary<WorkItem, int>((System.Func<WorkItem, int>) (wi => wi.Id.Value));
          if (populateDataRowCount && context.WorkItemServiceHelper.GetField(projectId.GuidId, DefaultWorkItemDataHelper.DataTablePropertyName) != null)
            dictionary2 = this.GetDataRowCountMap(context, projectId.GuidId, (IEnumerable<WorkItem>) workItems, (IEnumerable<int>) testCaseIds);
          context.TraceInfo("BusinessLayer", "PopulateResultsFromTestCases - Querying WIT end");
        }
        foreach (TestCaseResult result in results)
        {
          if (result.TestCaseId > 0)
          {
            WorkItem testCase;
            if (!dictionary1.TryGetValue(result.TestCaseId, out testCase))
              throw new TestObjectNotFoundException(context.RequestContext, result.TestCaseId, ObjectTypes.TestCase);
            this.PopulateResultFromTestCase(context, result, testCase);
            int num = 0;
            if (dictionary2 != null && dictionary2.TryGetValue(result.TestCaseId, out num))
              result.DataRowCount = num;
          }
        }
        this.PopulateResultsOwnerField(context, results);
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
      Guid projectId,
      IEnumerable<WorkItem> workItems,
      IEnumerable<int> testCaseIds)
    {
      if (workItems == null || testCaseIds == null || context == null)
        return (Dictionary<int, int>) null;
      Dictionary<int, int> dataRowCountMap = new Dictionary<int, int>();
      if (workItems == null)
        return (Dictionary<int, int>) null;
      foreach (WorkItem workItem in workItems)
      {
        if (workItem != null)
        {
          int? id = workItem.Id;
          if (id.HasValue)
          {
            TestManagementRequestContext context1 = context;
            IEnumerable<int> source = testCaseIds;
            id = workItem.Id;
            int num1 = id.Value;
            int num2 = source.Contains<int>(num1) ? 1 : 0;
            context1.TraceAndDebugAssert("BusinessLayer", num2 != 0, "Wrong test case returned.");
            int dataCount = this.GetDataCount(context, projectId, workItem);
            Dictionary<int, int> dictionary = dataRowCountMap;
            id = workItem.Id;
            int key = id.Value;
            int num3 = dataCount;
            dictionary.Add(key, num3);
            continue;
          }
        }
        return (Dictionary<int, int>) null;
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

    private int GetDataCount(
      TestManagementRequestContext context,
      Guid projectId,
      WorkItem workItem)
    {
      string field = workItem.Fields[DefaultWorkItemDataHelper.DataTablePropertyName] as string;
      return this.GetDataCountFromString(context, projectId, field);
    }

    private int GetDataCountFromString(
      TestManagementRequestContext context,
      Guid projectId,
      string dataString)
    {
      DataSet dataSetFromXml = DefaultWorkItemDataHelper.GetDataSetFromXml(dataString);
      if (dataSetFromXml == null)
      {
        ISharedParameterData parameterDataFromJson = DefaultWorkItemDataHelper.GetSharedParameterDataFromJson(context, projectId, dataString);
        if (parameterDataFromJson != null)
        {
          ISharedParameterDataRows parameterValues = parameterDataFromJson.ParameterValues;
          if (parameterValues != null)
          {
            List<ISharedParameterDataRow> rows = parameterValues.Rows;
            if (rows != null)
            {
              TestCaseParameterDataInfo parameterDataInfo = DefaultWorkItemDataHelper.ReadParameterDataInfoFromJson(dataString);
              if (parameterDataInfo != null && parameterDataInfo.GetSharedParamNames() != null)
              {
                List<string> list = parameterDataInfo.GetSharedParamNames().Select<string, string>((System.Func<string, string>) (paramName => XmlConvert.EncodeName(paramName))).ToList<string>();
                return rows.Count<ISharedParameterDataRow>() - this.GetEmptyTrailingRowsCount(rows, list);
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
      Guid projectId,
      string dataRowValue)
    {
      try
      {
        if (!string.IsNullOrEmpty(dataRowValue))
        {
          TestCaseParameterDataInfo parameterDataInfo = DefaultWorkItemDataHelper.ReadParameterDataInfoFromJson(dataRowValue);
          if (parameterDataInfo != null && ((IEnumerable<int>) parameterDataInfo.SharedParameterDataSetIds).Count<int>() > 0)
          {
            string field = DefaultWorkItemDataHelper.GetSharedParameterDataSetFromParameterDataInfo(context, projectId, parameterDataInfo).Fields[WorkItemFieldNames.Parameters] as string;
            return !string.IsNullOrEmpty(field) ? (ISharedParameterData) DefaultWorkItemDataHelper.GetSharedParameterDataFromParametersFieldValue(field) : (ISharedParameterData) null;
          }
        }
        return (ISharedParameterData) null;
      }
      catch (InvalidOperationException ex)
      {
        return (ISharedParameterData) null;
      }
    }

    private static WorkItem GetSharedParameterDataSetFromParameterDataInfo(
      TestManagementRequestContext context,
      Guid projectId,
      TestCaseParameterDataInfo parameterDataInfo)
    {
      int num = ((IEnumerable<int>) parameterDataInfo.SharedParameterDataSetIds).First<int>();
      if (context.WorkItemServiceHelper.GetField(projectId, WorkItemFieldNames.Parameters) == null)
        return (WorkItem) null;
      return context.WorkItemServiceHelper.GetWorkItems(projectId, (IList<int>) new List<int>()
      {
        num
      }, (IList<string>) new List<string>()
      {
        WorkItemFieldNames.Parameters
      }, WorkItemExpand.Fields, WorkItemErrorPolicy.Omit).First<WorkItem>();
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
      WorkItem testCase)
    {
      try
      {
        context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) testCase, nameof (testCase));
        TestManagementRequestContext context1 = context;
        int testCaseId = result.TestCaseId;
        int? id = testCase.Id;
        int valueOrDefault = id.GetValueOrDefault();
        int num = testCaseId == valueOrDefault & id.HasValue ? 1 : 0;
        context1.TraceAndDebugAssert("BusinessLayer", num != 0, "Invalid test case id");
        result.TestCaseRevision = testCase.Fields["System.Rev"] is int ? (int) testCase.Fields["System.Rev"] : 0;
        result.TestCaseTitle = testCase.Fields["System.Title"] as string;
        result.AutomatedTestId = testCase.Fields[WorkItemFieldNames.TestId] as string;
        result.AutomatedTestName = testCase.Fields[WorkItemFieldNames.TestName] as string;
        result.AutomatedTestStorage = testCase.Fields[WorkItemFieldNames.Storage] as string;
        result.AutomatedTestType = testCase.Fields[WorkItemFieldNames.TestType] as string;
        if (result.Priority == byte.MaxValue)
          result.Priority = testCase.Fields[WorkItemFieldNames.Priority] is byte ? (byte) testCase.Fields[WorkItemFieldNames.Priority] : byte.MaxValue;
        if (!(result.Owner == Guid.Empty))
          return;
        result.OwnerName = testCase.Fields["System.AssignedTo"] as string;
      }
      catch (TeamFoundationServerException ex1)
      {
        try
        {
          context.TraceError("BusinessLayer", "PopulateResultFromTestCase: Exception while processing CaseId={0}", (object) testCase.Id);
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
