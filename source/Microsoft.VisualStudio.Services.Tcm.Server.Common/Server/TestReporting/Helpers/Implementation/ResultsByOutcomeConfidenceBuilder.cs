// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.Implementation.ResultsByOutcomeConfidenceBuilder
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Models;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.Implementation
{
  internal class ResultsByOutcomeConfidenceBuilder : DefaultResultsByCategoryBuilder
  {
    public ResultsByOutcomeConfidenceBuilder(ReportingOptions reportingOptions)
      : base(reportingOptions)
    {
    }

    public override IDictionary<TestOutcome, AggregatedResultsByOutcome> GetAggregatesByCategory(
      IList<RunSummaryByCategory> runSummaryByCategory)
    {
      IDictionary<TestOutcome, AggregatedResultsByOutcome> aggregatesByCategory = this.GetResultsByCategoryInternal(runSummaryByCategory);
      if (!this.ReportingOptions.IncludeUnreliableTestResults)
        aggregatesByCategory = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>();
      return aggregatesByCategory;
    }

    protected override bool CompareCategoryValue(object value1, object value2)
    {
      float result1;
      float result2;
      return value1 != null && value2 != null && float.TryParse(value1.ToString(), out result1) && float.TryParse(value2.ToString(), out result2) && (double) result1 == (double) result2;
    }

    protected override bool ShouldInclude(object value)
    {
      float result;
      return value != null && float.TryParse(value.ToString(), out result) && (double) result == 0.0;
    }
  }
}
