// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.Implementation.DefaultResultsByCategoryBuilder
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.Interface;
using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Models;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.Implementation
{
  internal class DefaultResultsByCategoryBuilder : IResultsByCategoryBuilder
  {
    private readonly ReportingOptions m_reportingOptions;

    protected ReportingOptions ReportingOptions => this.m_reportingOptions;

    public DefaultResultsByCategoryBuilder(ReportingOptions reportingOptions) => this.m_reportingOptions = reportingOptions;

    public virtual IDictionary<TestOutcome, AggregatedResultsByOutcome> GetAggregatesByCategory(
      IList<RunSummaryByCategory> runSummaryByCategory)
    {
      return this.GetResultsByCategoryInternal(runSummaryByCategory);
    }

    public virtual List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> FilterResultsByCategory(
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> resultsByCategory)
    {
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      if (resultsByCategory != null)
      {
        foreach (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult in resultsByCategory)
        {
          if (testCaseResult.CustomFields != null && testCaseResult.CustomFields.Any<TestExtensionField>())
          {
            TestExtensionField testExtensionField = testCaseResult.CustomFields.FirstOrDefault<TestExtensionField>((Func<TestExtensionField, bool>) (c => string.Equals(c.Field.Name, this.m_reportingOptions.GroupByCategory, StringComparison.OrdinalIgnoreCase)));
            if (testExtensionField != null && this.ShouldInclude(testExtensionField.Value))
              testCaseResultList.Add(testCaseResult);
          }
        }
      }
      return testCaseResultList;
    }

    protected IDictionary<TestOutcome, AggregatedResultsByOutcome> GetResultsByCategoryInternal(
      IList<RunSummaryByCategory> runSummaryByCategory)
    {
      Dictionary<TestOutcome, AggregatedResultsByOutcome> categoryInternal = new Dictionary<TestOutcome, AggregatedResultsByOutcome>();
      if (runSummaryByCategory != null && runSummaryByCategory.Any<RunSummaryByCategory>())
      {
        foreach (RunSummaryByCategory summaryByCategory in (IEnumerable<RunSummaryByCategory>) runSummaryByCategory)
        {
          if (this.ShouldInclude(summaryByCategory.CategoryValue))
          {
            TestOutcome testOutcome = (TestOutcome) summaryByCategory.TestOutcome;
            if (categoryInternal.ContainsKey(testOutcome) && this.CompareCategoryValue(categoryInternal[testOutcome].GroupByValue, summaryByCategory.CategoryValue))
            {
              categoryInternal[testOutcome].Count += summaryByCategory.ResultCount;
              TimeSpan increment = TimeSpan.FromMilliseconds((double) Validator.CheckOverflowAndGetSafeValue(summaryByCategory.ResultDuration, 0L));
              categoryInternal[testOutcome].Duration = Validator.CheckOverflowAndGetSafeValue(categoryInternal[testOutcome].Duration, increment);
            }
            else
            {
              AggregatedResultsByOutcome resultsByOutcome = new AggregatedResultsByOutcome()
              {
                GroupByField = summaryByCategory.CategoryField,
                GroupByValue = summaryByCategory.CategoryValue,
                Outcome = (TestOutcome) summaryByCategory.TestOutcome,
                Count = summaryByCategory.ResultCount,
                Duration = TimeSpan.FromMilliseconds((double) Validator.CheckOverflowAndGetSafeValue(summaryByCategory.ResultDuration, 0L))
              };
              categoryInternal.Add(testOutcome, resultsByOutcome);
            }
          }
        }
      }
      return (IDictionary<TestOutcome, AggregatedResultsByOutcome>) categoryInternal;
    }

    protected virtual bool ShouldInclude(object value) => true;

    protected virtual bool CompareCategoryValue(object value1, object value2) => value1.Equals(value2);
  }
}
