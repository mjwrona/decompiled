// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.ChartingHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  public static class ChartingHelper
  {
    internal static void ValidateDimensions(List<string> dimensions, string source)
    {
      TestReportData testReportData = (TestReportData) null;
      if (string.Equals(source, "execution", StringComparison.OrdinalIgnoreCase))
        testReportData = (TestReportData) new TestExecutionReportMetadata();
      if (string.Equals(source, "runsummary", StringComparison.OrdinalIgnoreCase))
        testReportData = (TestReportData) new TestRunReportdata();
      if (testReportData == null)
        return;
      List<string> list1 = testReportData.Dimensions().Select<TestReportData.DimensionNameLabelPair, string>((Func<TestReportData.DimensionNameLabelPair, string>) (d => d.Name)).ToList<string>();
      List<string> list2 = dimensions.Except<string>((IEnumerable<string>) list1, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
      if (list2 != null && list2.Count > 0)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidChartDimension, (object) string.Join(",", (IEnumerable<string>) list2)));
    }
  }
}
