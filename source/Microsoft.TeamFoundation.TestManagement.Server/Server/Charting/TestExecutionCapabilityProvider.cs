// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionCapabilityProvider
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  public class TestExecutionCapabilityProvider : IDataServiceCapabilityProvider, IDataServicesService
  {
    public IVssRequestContext RequestContext { get; set; }

    public string GetScopeName() => FeatureProviderScopes.TestReports;

    public string GetArtifactPluralName(IVssRequestContext requestContext) => TestReportData.Measure();

    public IEnumerable<FixedIntervalDateRange> GetHistoryRanges(IVssRequestContext requestContext) => DefaultDateRanges.GetDefaultOptions();

    public static IEnumerable<FixedIntervalDateRange> GetRangeOptions() => DefaultDateRanges.GetDefaultOptions();

    public IEnumerable<FieldInfo> GetFields(IVssRequestContext requestContext)
    {
      List<FieldInfo> fields = new List<FieldInfo>();
      foreach (TestReportData.DimensionNameLabelPair dimension in new TestExecutionReportMetadata().Dimensions())
      {
        List<FieldInfo> fieldInfoList = fields;
        FieldInfo fieldInfo = new FieldInfo();
        fieldInfo.Name = dimension.Name;
        fieldInfo.LabelText = dimension.Label;
        fieldInfo.IsGroupable = true;
        fieldInfoList.Add(fieldInfo);
      }
      return (IEnumerable<FieldInfo>) fields;
    }
  }
}
