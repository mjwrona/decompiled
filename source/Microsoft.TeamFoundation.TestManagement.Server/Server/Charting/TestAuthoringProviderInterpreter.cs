// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestAuthoringProviderInterpreter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Charting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestAuthoringProviderInterpreter : 
    WitProviderInterpreter,
    IInterpretTimedData<DatedWorkItemFieldData>,
    IInterpretRecord<DatedWorkItemFieldData>,
    IProvideFilteredData<DatedWorkItemFieldData>
  {
    private const string c_BatchingInChartCreationFF = "TestManagement.Server.EnableBatchingInChartsCreation";
    private const int c_ChartCreationBatchSize = 2000;
    private const string c_ChartCreationBatchSizeRegistryPath = "/Service/TestManagement/Settings/ChartCreationBatchSize";

    public override IEnumerable<DatedWorkItemFieldData> GetFilteredData(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformInstructions<DatedWorkItemFieldData>> instructions)
    {
      int result = 0;
      IEnumerable<TransformOptions> options1 = instructions.Select<TransformInstructions<DatedWorkItemFieldData>, TransformOptions>((Func<TransformInstructions<DatedWorkItemFieldData>, TransformOptions>) (o => o.Options));
      if (this.m_witRequestContext == null)
      {
        this.m_witRequestContext = requestContext.WitContext();
        this.m_requestContext = requestContext;
      }
      TransformOptions options2 = instructions.FirstOrDefault<TransformInstructions<DatedWorkItemFieldData>>().Options;
      if (string.IsNullOrEmpty(options2.Filter))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter"));
      string s;
      if (!TestReportsHelper.GetTransformFilterMap(options2.Filter).TryGetValue("suiteId", out s))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter[suiteId]"));
      if (string.IsNullOrEmpty(s) || !int.TryParse(s, out result))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter[suiteId]"));
      List<int> intList = TestCaseHelper.FetchTestCaseIdsRecursive((TestManagementRequestContext) new TfsTestManagementRequestContext(requestContext), transformSecurityInformation.ProjectId.Value, result);
      string testCaseIdListString1 = "-1";
      this.m_fieldsMap = WitChartingDalWrapper.GetFieldIdMap(requestContext, options1);
      this.m_identityFieldsMap = WitChartingDalWrapper.GetIdentityFieldsMap(requestContext, options1);
      IEnumerable<DateTime> asOfDates = instructions.SelectMany<TransformInstructions<DatedWorkItemFieldData>, DateTime>((Func<TransformInstructions<DatedWorkItemFieldData>, IEnumerable<DateTime>>) (o => o.HistorySamplePoints)).Distinct<DateTime>();
      int num = requestContext.IsFeatureEnabled("TestManagement.Server.EnableBatchingInChartsCreation") ? 1 : 0;
      List<DatedWorkItemFieldData> filteredData = new List<DatedWorkItemFieldData>();
      if (num != 0)
      {
        int count1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/ChartCreationBatchSize", 2000);
        for (int count2 = 0; count2 < intList.Count; count2 += count1)
        {
          string testCaseIdListString2 = string.Join<int>(",", (IEnumerable<int>) intList.Skip<int>(count2).Take<int>(count1).ToArray<int>());
          string wiqlQuery = TestAuthoringWiqlHelper.GetWiqlQuery((IList<string>) this.m_fieldsMap.Keys.ToList<string>(), testCaseIdListString2);
          filteredData.AddRange(WitProviderInterpreter.GetWorkItemFieldValues(this.m_requestContext, transformSecurityInformation.ProjectId, wiqlQuery, asOfDates, (IDictionary) null, (IEnumerable<string>) this.m_fieldsMap.Keys));
        }
        return (IEnumerable<DatedWorkItemFieldData>) filteredData;
      }
      if (intList.Count != 0)
        testCaseIdListString1 = string.Join<int>(",", (IEnumerable<int>) intList);
      string wiqlQuery1 = TestAuthoringWiqlHelper.GetWiqlQuery((IList<string>) this.m_fieldsMap.Keys.ToList<string>(), testCaseIdListString1);
      return WitProviderInterpreter.GetWorkItemFieldValues(this.m_requestContext, transformSecurityInformation.ProjectId, wiqlQuery1, asOfDates, (IDictionary) null, (IEnumerable<string>) this.m_fieldsMap.Keys);
    }

    public override bool SamplePoint(
      DatedWorkItemFieldData record,
      TransformInstructions<DatedWorkItemFieldData> instructions,
      out AggregatedRecord sampledRecord)
    {
      return base.SamplePoint(record, instructions, out sampledRecord);
    }

    public override object GetRecordValue(string recordPropertyName, DatedWorkItemFieldData record) => base.GetRecordValue(recordPropertyName, record);
  }
}
