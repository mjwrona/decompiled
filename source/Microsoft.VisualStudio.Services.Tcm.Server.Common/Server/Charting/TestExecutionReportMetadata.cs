// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportMetadata
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestExecutionReportMetadata : TestReportData
  {
    public override List<TestReportData.DimensionNameLabelPair> Dimensions()
    {
      List<TestReportData.DimensionNameLabelPair> dimensionNameLabelPairList = base.Dimensions();
      dimensionNameLabelPairList.Add(new TestReportData.DimensionNameLabelPair("Suite", ServerResources.ChartDimensionSuite));
      dimensionNameLabelPairList.Add(new TestReportData.DimensionNameLabelPair("Tester", ServerResources.ChartDimensionTester));
      dimensionNameLabelPairList.Add(new TestReportData.DimensionNameLabelPair("RunTypeIsAutomated", ServerResources.ChartDimensionRunType));
      return dimensionNameLabelPairList;
    }
  }
}
