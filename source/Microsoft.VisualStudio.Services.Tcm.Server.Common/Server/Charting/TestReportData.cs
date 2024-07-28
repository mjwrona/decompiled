// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestReportData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestReportData
  {
    public virtual List<TestReportData.DimensionNameLabelPair> Dimensions() => new List<TestReportData.DimensionNameLabelPair>()
    {
      new TestReportData.DimensionNameLabelPair("Configuration", ServerResources.ChartDimensionConfiguration),
      new TestReportData.DimensionNameLabelPair("FailureType", ServerResources.ChartDimensionFailureType),
      new TestReportData.DimensionNameLabelPair("Outcome", ServerResources.ChartDimensionOutcome),
      new TestReportData.DimensionNameLabelPair("Priority", ServerResources.ChartDimensionPriority),
      new TestReportData.DimensionNameLabelPair("Resolution", ServerResources.ChartDimensionResolution),
      new TestReportData.DimensionNameLabelPair("RunBy", ServerResources.ChartDimensionRunBy)
    };

    public static string Measure() => ServerResources.ChartMeasureTests;

    public struct DimensionNameLabelPair
    {
      public DimensionNameLabelPair(string name, string label)
        : this()
      {
        this.Name = name;
        this.Label = label;
      }

      public string Name { get; private set; }

      public string Label { get; private set; }
    }
  }
}
