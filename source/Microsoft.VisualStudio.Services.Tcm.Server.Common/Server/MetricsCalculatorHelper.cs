// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MetricsCalculatorHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class MetricsCalculatorHelper
  {
    internal static PipelineTestMetrics CalculatePipelineTestMetricsForAParticularNodeFromServerOM(
      IVssRequestContext requestContext,
      PipelineReference currentContext,
      RunSummaryAndResultInsightsInPipeline runSummaryAndResultInsightsInPipeline,
      int prevPipelineId,
      bool resultSummaryFlag,
      bool resultsAnalysisFlag,
      bool runSummaryFlag)
    {
      requestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", "MetricsCalculatorHelper.CalculatePipelineTestMetricsForAParticularNodeFromServerOM");
      PipelineTestMetrics nodeFromServerOm = new PipelineTestMetrics()
      {
        CurrentContext = currentContext
      };
      if (runSummaryAndResultInsightsInPipeline == null)
        return nodeFromServerOm;
      int currentTotalTestCount = 0;
      int currentPassTestCount = 0;
      int currentFailTestCount = 0;
      int currentNonImpactedTestCount = 0;
      int currentOtherTestCount = 0;
      long aggregatedRunDurationInMs1 = 0;
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome> dictionary = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome>();
      if (resultSummaryFlag)
        nodeFromServerOm.ResultSummary = new ResultSummary()
        {
          ResultSummaryByRunState = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, ResultsSummaryByOutcome>()
        };
      if (resultSummaryFlag | resultsAnalysisFlag && runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome != null)
      {
        foreach (IGrouping<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, RunSummaryByOutcomeInPipeline> source in runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome.GroupBy<RunSummaryByOutcomeInPipeline, Microsoft.TeamFoundation.TestManagement.Client.TestRunState>((Func<RunSummaryByOutcomeInPipeline, Microsoft.TeamFoundation.TestManagement.Client.TestRunState>) (rsbo => rsbo.TestRunState)))
        {
          long aggregatedRunDurationInMs2;
          int notReportedTestCount;
          Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome> aggregateDataByOutCome = MetricsCalculatorHelper.CalculateAggregateDataByOutCome(requestContext, source.ToList<RunSummaryByOutcomeInPipeline>(), out aggregatedRunDurationInMs2, out notReportedTestCount);
          int totalTests;
          int passedCount;
          int failedCount;
          int notImpactedCount;
          int othersCount;
          MetricsCalculatorHelper.CalculateCountOfResultsForDifferentOutcome(aggregateDataByOutCome, out totalTests, out passedCount, out failedCount, out notImpactedCount, out othersCount);
          if (resultSummaryFlag)
            nodeFromServerOm.ResultSummary.ResultSummaryByRunState[(Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) source.Key] = new ResultsSummaryByOutcome()
            {
              TotalTestCount = totalTests,
              NotReportedTestCount = notReportedTestCount,
              Duration = TimeSpan.FromMilliseconds((double) aggregatedRunDurationInMs2),
              AggregatedResultDetailsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome>) aggregateDataByOutCome
            };
          if (resultsAnalysisFlag && (source.Key == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed || source.Key == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation))
          {
            currentTotalTestCount += totalTests;
            currentPassTestCount += passedCount;
            currentFailTestCount += failedCount;
            currentNonImpactedTestCount += notImpactedCount;
            currentOtherTestCount += othersCount;
          }
        }
      }
      if (resultsAnalysisFlag | runSummaryFlag && runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome != null)
      {
        List<RunSummaryByOutcomeInPipeline> list = runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome.Where<RunSummaryByOutcomeInPipeline>((Func<RunSummaryByOutcomeInPipeline, bool>) (rsbo => rsbo.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed || rsbo.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation)).ToList<RunSummaryByOutcomeInPipeline>();
        MetricsCalculatorHelper.CalculateAggregateDataByOutCome(requestContext, list, out aggregatedRunDurationInMs1, out int _);
      }
      if (runSummaryFlag)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary runSummary = new Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary()
        {
          TotalRunsCount = 0,
          NoConfigRunsCount = 0,
          RunSummaryByState = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int>(),
          RunSummaryByOutcome = (IDictionary<TestRunOutcome, int>) new Dictionary<TestRunOutcome, int>()
        };
        if (runSummaryAndResultInsightsInPipeline.RunSummaryByCounts != null && runSummaryAndResultInsightsInPipeline.RunSummaryByCounts.Count == 1)
          runSummary.TotalRunsCount = runSummaryAndResultInsightsInPipeline.RunSummaryByCounts[0].RunsCount;
        if (runSummaryAndResultInsightsInPipeline.NoConfigRunSummaryByCounts != null && runSummaryAndResultInsightsInPipeline.NoConfigRunSummaryByCounts.Count == 1)
          runSummary.NoConfigRunsCount = runSummaryAndResultInsightsInPipeline.NoConfigRunSummaryByCounts[0].RunsCount;
        foreach (RunSummaryByStateInPipeline byStateInPipeline in (IEnumerable<RunSummaryByStateInPipeline>) runSummaryAndResultInsightsInPipeline.RunSummaryByState ?? Enumerable.Empty<RunSummaryByStateInPipeline>())
        {
          if (!runSummary.RunSummaryByState.ContainsKey((Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) byStateInPipeline.RunState))
            runSummary.RunSummaryByState[(Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) byStateInPipeline.RunState] = byStateInPipeline.RunsCount;
          else
            runSummary.RunSummaryByState[(Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) byStateInPipeline.RunState] += byStateInPipeline.RunsCount;
        }
        if (runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome != null)
        {
          List<RunSummaryByOutcomeInPipeline> list = runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome.Where<RunSummaryByOutcomeInPipeline>((Func<RunSummaryByOutcomeInPipeline, bool>) (rsbo => rsbo.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed || rsbo.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation)).ToList<RunSummaryByOutcomeInPipeline>();
          runSummary.RunSummaryByOutcome = (IDictionary<TestRunOutcome, int>) MetricsCalculatorHelper.CalculateAggregateRunSummaryByOutcome(list);
        }
        runSummary.Duration = TimeSpan.FromMilliseconds((double) aggregatedRunDurationInMs1);
        nodeFromServerOm.RunSummary = runSummary;
      }
      if (resultsAnalysisFlag)
      {
        ResultsAnalysis resultsAnalysis = new ResultsAnalysis();
        if (prevPipelineId > 0)
        {
          resultsAnalysis.PreviousContext = new PipelineReference()
          {
            PipelineId = prevPipelineId
          };
          if (currentContext.StageReference != null)
            resultsAnalysis.PreviousContext.StageReference = new StageReference()
            {
              StageName = currentContext.StageReference.StageName
            };
          if (currentContext.PhaseReference != null)
            resultsAnalysis.PreviousContext.PhaseReference = new PhaseReference()
            {
              PhaseName = currentContext.PhaseReference.PhaseName
            };
          if (currentContext.JobReference != null)
            resultsAnalysis.PreviousContext.JobReference = new JobReference()
            {
              JobName = currentContext.JobReference.JobName
            };
        }
        resultsAnalysis.TestFailuresAnalysis = MetricsCalculatorHelper.CalculateTestFailuresAnalysis(runSummaryAndResultInsightsInPipeline.ResultInsights);
        resultsAnalysis.ResultsDifference = MetricsCalculatorHelper.CalculateAggregatedResultsDifference(requestContext, runSummaryAndResultInsightsInPipeline.PreviousRunSummaryByOutcome, currentTotalTestCount, currentPassTestCount, currentFailTestCount, currentNonImpactedTestCount, currentOtherTestCount, aggregatedRunDurationInMs1);
        nodeFromServerOm.ResultsAnalysis = resultsAnalysis;
      }
      requestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", "MetricsCalculatorHelper.CalculatePipelineTestMetricsForAParticularNodeFromServerOM");
      return nodeFromServerOm;
    }

    internal static PipelineTestMetrics CalculatePipelineTestMatricsForEachNodeFromServerOM(
      IVssRequestContext requestContext,
      PipelineReference pipelineReference,
      RunSummaryAndResultInsightsInPipeline runSummaryAndResultInsightsInPipeline,
      int prevPipelineId,
      bool resultsByOutcomeFlag,
      bool resultsAnalysisFlag,
      bool runSummaryFlag,
      PipelineNodeHierarchyLevel requiredResultLevel)
    {
      requestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", "MetricsCalculatorHelper.CalculatePipelineTestMatricsForEachNodeFromServerOM");
      try
      {
        if (runSummaryAndResultInsightsInPipeline == null)
          return new PipelineTestMetrics()
          {
            CurrentContext = pipelineReference
          };
        if (requiredResultLevel == PipelineNodeHierarchyLevel.Job)
          return MetricsCalculatorHelper.CalculatePipelineTestMetricsForAParticularNodeFromServerOM(requestContext, pipelineReference, runSummaryAndResultInsightsInPipeline, prevPipelineId, resultsByOutcomeFlag, resultsAnalysisFlag, runSummaryFlag);
        List<PipelineTestMetrics> source1 = new List<PipelineTestMetrics>();
        Dictionary<PipelineReference, RunSummaryAndResultInsightsInPipeline> dictionary = new Dictionary<PipelineReference, RunSummaryAndResultInsightsInPipeline>((IEqualityComparer<PipelineReference>) new PipelineReferenceComparer());
        if (runSummaryAndResultInsightsInPipeline.RunSummaryByCounts != null)
        {
          foreach (IGrouping<PipelineReference, RunSummaryByCountInPipeline> source2 in runSummaryAndResultInsightsInPipeline.RunSummaryByCounts.GroupBy<RunSummaryByCountInPipeline, PipelineReference>((Func<RunSummaryByCountInPipeline, PipelineReference>) (rsbc => rsbc.PipelineReference), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer()))
          {
            if (!dictionary.ContainsKey(source2.Key))
              dictionary[source2.Key] = new RunSummaryAndResultInsightsInPipeline()
              {
                RunSummaryByCounts = source2.ToList<RunSummaryByCountInPipeline>()
              };
            else
              dictionary[source2.Key].RunSummaryByCounts = source2.ToList<RunSummaryByCountInPipeline>();
          }
        }
        if (runSummaryAndResultInsightsInPipeline.NoConfigRunSummaryByCounts != null)
        {
          foreach (IGrouping<PipelineReference, RunSummaryByCountInPipeline> source3 in runSummaryAndResultInsightsInPipeline.NoConfigRunSummaryByCounts.GroupBy<RunSummaryByCountInPipeline, PipelineReference>((Func<RunSummaryByCountInPipeline, PipelineReference>) (ncrsbc => ncrsbc.PipelineReference), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer()))
          {
            if (!dictionary.ContainsKey(source3.Key))
              dictionary[source3.Key] = new RunSummaryAndResultInsightsInPipeline()
              {
                NoConfigRunSummaryByCounts = source3.ToList<RunSummaryByCountInPipeline>()
              };
            else
              dictionary[source3.Key].NoConfigRunSummaryByCounts = source3.ToList<RunSummaryByCountInPipeline>();
          }
        }
        if (runSummaryAndResultInsightsInPipeline.RunSummaryByState != null)
        {
          foreach (IGrouping<PipelineReference, RunSummaryByStateInPipeline> source4 in runSummaryAndResultInsightsInPipeline.RunSummaryByState.GroupBy<RunSummaryByStateInPipeline, PipelineReference>((Func<RunSummaryByStateInPipeline, PipelineReference>) (rsbs => rsbs.PipelineReference), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer()))
          {
            if (!dictionary.ContainsKey(source4.Key))
              dictionary[source4.Key] = new RunSummaryAndResultInsightsInPipeline()
              {
                RunSummaryByState = source4.ToList<RunSummaryByStateInPipeline>()
              };
            else
              dictionary[source4.Key].RunSummaryByState = source4.ToList<RunSummaryByStateInPipeline>();
          }
        }
        if (runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome != null)
        {
          foreach (IGrouping<PipelineReference, RunSummaryByOutcomeInPipeline> source5 in runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome.GroupBy<RunSummaryByOutcomeInPipeline, PipelineReference>((Func<RunSummaryByOutcomeInPipeline, PipelineReference>) (crs => crs.PipelineReference), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer()))
          {
            if (!dictionary.ContainsKey(source5.Key))
              dictionary[source5.Key] = new RunSummaryAndResultInsightsInPipeline()
              {
                CurrentRunSummaryByOutcome = source5.ToList<RunSummaryByOutcomeInPipeline>()
              };
            else
              dictionary[source5.Key].CurrentRunSummaryByOutcome = source5.ToList<RunSummaryByOutcomeInPipeline>();
          }
        }
        if (runSummaryAndResultInsightsInPipeline.PreviousRunSummaryByOutcome != null)
        {
          foreach (IGrouping<PipelineReference, RunSummaryByOutcomeInPipeline> source6 in runSummaryAndResultInsightsInPipeline.PreviousRunSummaryByOutcome.GroupBy<RunSummaryByOutcomeInPipeline, PipelineReference>((Func<RunSummaryByOutcomeInPipeline, PipelineReference>) (prs => prs.PipelineReference), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer()))
          {
            if (!dictionary.ContainsKey(source6.Key))
              dictionary[source6.Key] = new RunSummaryAndResultInsightsInPipeline()
              {
                PreviousRunSummaryByOutcome = source6.ToList<RunSummaryByOutcomeInPipeline>()
              };
            else
              dictionary[source6.Key].PreviousRunSummaryByOutcome = source6.ToList<RunSummaryByOutcomeInPipeline>();
          }
        }
        if (runSummaryAndResultInsightsInPipeline.ResultInsights != null)
        {
          foreach (IGrouping<PipelineReference, ResultInsightsInPipeline> source7 in runSummaryAndResultInsightsInPipeline.ResultInsights.GroupBy<ResultInsightsInPipeline, PipelineReference>((Func<ResultInsightsInPipeline, PipelineReference>) (rin => rin.PipelineReference), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer()))
          {
            if (!dictionary.ContainsKey(source7.Key))
              dictionary[source7.Key] = new RunSummaryAndResultInsightsInPipeline()
              {
                ResultInsights = source7.ToList<ResultInsightsInPipeline>()
              };
            else
              dictionary[source7.Key].ResultInsights = source7.ToList<ResultInsightsInPipeline>();
          }
        }
        foreach (KeyValuePair<PipelineReference, RunSummaryAndResultInsightsInPipeline> keyValuePair in dictionary)
        {
          PipelineReference key = keyValuePair.Key;
          key.PipelineId = pipelineReference.PipelineId;
          source1.Add(MetricsCalculatorHelper.CalculatePipelineTestMetricsForAParticularNodeFromServerOM(requestContext, key, keyValuePair.Value, prevPipelineId, resultsByOutcomeFlag, resultsAnalysisFlag, runSummaryFlag));
        }
        List<PipelineTestMetrics> source8 = new List<PipelineTestMetrics>();
        foreach (IGrouping<PipelineReference, PipelineTestMetrics> source9 in source1.GroupBy<PipelineTestMetrics, PipelineReference>((Func<PipelineTestMetrics, PipelineReference>) (mt => mt.CurrentContext), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer(PipelineNodeHierarchyLevel.Phase)))
          source8.Add(MetricsCalculatorHelper.AggregatePipelineTestMetrics(requestContext, source9.ToList<PipelineTestMetrics>(), PipelineNodeHierarchyLevel.Phase, runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome));
        if (requiredResultLevel == PipelineNodeHierarchyLevel.Phase)
          return source8.Count == 1 ? source8[0] : throw new Exception(string.Format(ServerResources.MetricsCalculationException, (object) "Phase"));
        List<PipelineTestMetrics> source10 = new List<PipelineTestMetrics>();
        foreach (IGrouping<PipelineReference, PipelineTestMetrics> source11 in source8.GroupBy<PipelineTestMetrics, PipelineReference>((Func<PipelineTestMetrics, PipelineReference>) (mt => mt.CurrentContext), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer(PipelineNodeHierarchyLevel.Stage)))
          source10.Add(MetricsCalculatorHelper.AggregatePipelineTestMetrics(requestContext, source11.ToList<PipelineTestMetrics>(), PipelineNodeHierarchyLevel.Stage, runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome));
        if (requiredResultLevel == PipelineNodeHierarchyLevel.Stage)
          return source10.Count == 1 ? source10[0] : throw new Exception(string.Format(ServerResources.MetricsCalculationException, (object) "Stage"));
        List<PipelineTestMetrics> pipelineTestMetricsList = new List<PipelineTestMetrics>();
        foreach (IGrouping<PipelineReference, PipelineTestMetrics> source12 in source10.GroupBy<PipelineTestMetrics, PipelineReference>((Func<PipelineTestMetrics, PipelineReference>) (mt => mt.CurrentContext), (IEqualityComparer<PipelineReference>) new PipelineReferenceComparer(PipelineNodeHierarchyLevel.PipelineInstance)))
          pipelineTestMetricsList.Add(MetricsCalculatorHelper.AggregatePipelineTestMetrics(requestContext, source12.ToList<PipelineTestMetrics>(), PipelineNodeHierarchyLevel.PipelineInstance, runSummaryAndResultInsightsInPipeline.CurrentRunSummaryByOutcome));
        return pipelineTestMetricsList.Count == 1 ? pipelineTestMetricsList[0] : throw new Exception(string.Format(ServerResources.MetricsCalculationException, (object) "PipelineInstance"));
      }
      finally
      {
        requestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", "MetricsCalculatorHelper.CalculatePipelineTestMatricsForEachNodeFromServerOM");
      }
    }

    internal static PipelineTestMetrics AggregatePipelineTestMetrics(
      IVssRequestContext requestContext,
      List<PipelineTestMetrics> listOfChildMetrics,
      PipelineNodeHierarchyLevel level,
      List<RunSummaryByOutcomeInPipeline> runSummaryByOutcomes)
    {
      if (listOfChildMetrics == null || !listOfChildMetrics.Any<PipelineTestMetrics>())
      {
        PipelineTestMetrics pipelineTestMetrics = new PipelineTestMetrics();
      }
      PipelineTestMetrics result = new PipelineTestMetrics()
      {
        SummaryAtChild = listOfChildMetrics
      };
      result.CurrentContext = MetricsCalculatorHelper.GetPipelineReferenceForANodeLevel(listOfChildMetrics[0].CurrentContext, level);
      result.ResultSummary = listOfChildMetrics[0].ResultSummary;
      result.RunSummary = listOfChildMetrics[0].RunSummary;
      result.ResultsAnalysis = listOfChildMetrics[0].ResultsAnalysis != null ? new ResultsAnalysis(listOfChildMetrics[0].ResultsAnalysis) : (ResultsAnalysis) null;
      if (result.ResultsAnalysis?.PreviousContext != null)
        result.ResultsAnalysis.PreviousContext = MetricsCalculatorHelper.GetPipelineReferenceForANodeLevel(result.ResultsAnalysis.PreviousContext, level);
      for (int index = 1; index < listOfChildMetrics.Count; ++index)
      {
        result.ResultSummary = MetricsCalculatorHelper.MergeResultsSummary(result.ResultSummary, listOfChildMetrics[index].ResultSummary);
        result.RunSummary = MetricsCalculatorHelper.MergeRunSummary(result.RunSummary, listOfChildMetrics[index].RunSummary);
        result.ResultsAnalysis = MetricsCalculatorHelper.MergeResultsAnalysis(result.ResultsAnalysis, listOfChildMetrics[index].ResultsAnalysis, level);
      }
      if (listOfChildMetrics.Count > 1 && runSummaryByOutcomes != null)
      {
        PipelineReferenceComparer compare = new PipelineReferenceComparer(level);
        List<RunSummaryByOutcomeInPipeline> list1 = runSummaryByOutcomes.Where<RunSummaryByOutcomeInPipeline>((Func<RunSummaryByOutcomeInPipeline, bool>) (rsbo => compare.Equals(result.CurrentContext, rsbo.PipelineReference))).ToList<RunSummaryByOutcomeInPipeline>();
        ResultSummary resultSummary = result.ResultSummary;
        int num;
        if (resultSummary == null)
        {
          num = 0;
        }
        else
        {
          Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, ResultsSummaryByOutcome> summaryByRunState = resultSummary.ResultSummaryByRunState;
          bool? nullable = summaryByRunState != null ? new bool?(summaryByRunState.Any<KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, ResultsSummaryByOutcome>>()) : new bool?();
          bool flag = true;
          num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
        }
        if (num != 0)
        {
          foreach (IGrouping<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, RunSummaryByOutcomeInPipeline> source in list1.GroupBy<RunSummaryByOutcomeInPipeline, Microsoft.TeamFoundation.TestManagement.Client.TestRunState>((Func<RunSummaryByOutcomeInPipeline, Microsoft.TeamFoundation.TestManagement.Client.TestRunState>) (rs => rs.TestRunState)))
          {
            if (result.ResultSummary.ResultSummaryByRunState.ContainsKey((Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) source.Key))
            {
              long aggregatedRunDurationInMs;
              MetricsCalculatorHelper.CalculateAggregateDataByOutCome(requestContext, source.ToList<RunSummaryByOutcomeInPipeline>(), out aggregatedRunDurationInMs, out int _);
              result.ResultSummary.ResultSummaryByRunState[(Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) source.Key].Duration = TimeSpan.FromMilliseconds((double) aggregatedRunDurationInMs);
            }
          }
        }
        if (result.RunSummary != null)
        {
          List<RunSummaryByOutcomeInPipeline> list2 = list1.Where<RunSummaryByOutcomeInPipeline>((Func<RunSummaryByOutcomeInPipeline, bool>) (rsbo => rsbo.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed || rsbo.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation)).ToList<RunSummaryByOutcomeInPipeline>();
          long aggregatedRunDurationInMs;
          MetricsCalculatorHelper.CalculateAggregateDataByOutCome(requestContext, list2, out aggregatedRunDurationInMs, out int _);
          result.RunSummary.Duration = TimeSpan.FromMilliseconds((double) aggregatedRunDurationInMs);
        }
      }
      return result;
    }

    internal static ResultsAnalysis MergeResultsAnalysis(
      ResultsAnalysis ra1,
      ResultsAnalysis ra2,
      PipelineNodeHierarchyLevel level)
    {
      if (ra1 == null && ra2 == null)
        return (ResultsAnalysis) null;
      return new ResultsAnalysis()
      {
        TestFailuresAnalysis = MetricsCalculatorHelper.MergeTestResultFailuresAnalysis(ra1?.TestFailuresAnalysis, ra2?.TestFailuresAnalysis),
        ResultsDifference = MetricsCalculatorHelper.MergeAggregatedResultsDifference(ra1?.ResultsDifference, ra2?.ResultsDifference),
        PreviousContext = MetricsCalculatorHelper.GetPipelineReferenceForANodeLevel(ra1 != null ? ra1.PreviousContext : ra2.PreviousContext, level)
      };
    }

    internal static TestResultFailuresAnalysis MergeTestResultFailuresAnalysis(
      TestResultFailuresAnalysis trfa1,
      TestResultFailuresAnalysis trfa2)
    {
      if (trfa1 == null && trfa2 == null)
        return (TestResultFailuresAnalysis) null;
      return new TestResultFailuresAnalysis()
      {
        NewFailures = MetricsCalculatorHelper.MergeTestFailureDetails(trfa1?.NewFailures, trfa2?.NewFailures),
        ExistingFailures = MetricsCalculatorHelper.MergeTestFailureDetails(trfa1?.ExistingFailures, trfa2?.ExistingFailures),
        FixedTests = MetricsCalculatorHelper.MergeTestFailureDetails(trfa1?.FixedTests, trfa2?.FixedTests)
      };
    }

    internal static TestFailureDetails MergeTestFailureDetails(
      TestFailureDetails fd1,
      TestFailureDetails fd2)
    {
      if (fd1 == null && fd2 == null)
        return (TestFailureDetails) null;
      TestFailureDetails testFailureDetails = new TestFailureDetails()
      {
        Count = 0
      };
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
      if (fd1 != null)
      {
        testFailureDetails.Count += fd1.Count;
        if (fd1.TestResults != null && fd1.TestResults.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>())
          resultIdentifierList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) fd1.TestResults);
      }
      if (fd2 != null)
      {
        testFailureDetails.Count += fd2.Count;
        if (fd2.TestResults != null && fd2.TestResults.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>())
          resultIdentifierList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) fd2.TestResults);
      }
      testFailureDetails.TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList;
      return testFailureDetails;
    }

    internal static AggregatedResultsDifference MergeAggregatedResultsDifference(
      AggregatedResultsDifference ard1,
      AggregatedResultsDifference ard2)
    {
      if (ard1 == null && ard2 == null)
        return (AggregatedResultsDifference) null;
      AggregatedResultsDifference resultsDifference = new AggregatedResultsDifference()
      {
        IncreaseInTotalTests = 0,
        IncreaseInFailures = 0,
        IncreaseInPassedTests = 0,
        IncreaseInOtherTests = 0,
        IncreaseInDuration = TimeSpan.FromMilliseconds(0.0)
      };
      if (ard1 != null)
      {
        resultsDifference.IncreaseInTotalTests = ard1.IncreaseInTotalTests;
        resultsDifference.IncreaseInFailures = ard1.IncreaseInFailures;
        resultsDifference.IncreaseInPassedTests = ard1.IncreaseInPassedTests;
        resultsDifference.IncreaseInOtherTests = ard1.IncreaseInOtherTests;
        resultsDifference.IncreaseInDuration = ard1.IncreaseInDuration;
      }
      if (ard2 != null)
      {
        resultsDifference.IncreaseInTotalTests += ard2.IncreaseInTotalTests;
        resultsDifference.IncreaseInFailures += ard2.IncreaseInFailures;
        resultsDifference.IncreaseInPassedTests += ard2.IncreaseInPassedTests;
        resultsDifference.IncreaseInOtherTests += ard2.IncreaseInOtherTests;
        resultsDifference.IncreaseInDuration = Validator.CheckOverflowAndGetSafeValue(resultsDifference.IncreaseInDuration, ard2.IncreaseInDuration);
      }
      return resultsDifference;
    }

    internal static Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary MergeRunSummary(
      Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary sum1,
      Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary sum2)
    {
      if (sum1 == null && sum2 == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary) null;
      Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary runSummary = new Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary();
      runSummary.TotalRunsCount = 0;
      runSummary.NoConfigRunsCount = 0;
      runSummary.Duration = TimeSpan.FromMilliseconds(0.0);
      runSummary.RunSummaryByState = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int>();
      runSummary.RunSummaryByOutcome = (IDictionary<TestRunOutcome, int>) new Dictionary<TestRunOutcome, int>();
      if (sum1 != null)
      {
        runSummary.TotalRunsCount += sum1.TotalRunsCount;
        runSummary.NoConfigRunsCount += sum1.NoConfigRunsCount;
        runSummary.Duration = sum1.Duration;
      }
      if (sum2 != null)
      {
        runSummary.TotalRunsCount += sum2.TotalRunsCount;
        runSummary.NoConfigRunsCount += sum2.NoConfigRunsCount;
        runSummary.Duration = Validator.CheckOverflowAndGetSafeValue(runSummary.Duration, sum2.Duration);
      }
      if (sum1 != null && sum1.RunSummaryByState != null)
      {
        foreach (KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int> keyValuePair in (IEnumerable<KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int>>) sum1.RunSummaryByState)
          runSummary.RunSummaryByState[keyValuePair.Key] = keyValuePair.Value;
      }
      if (sum2 != null && sum2.RunSummaryByState != null)
      {
        foreach (KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int> keyValuePair in (IEnumerable<KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, int>>) sum2.RunSummaryByState)
        {
          if (!runSummary.RunSummaryByState.ContainsKey(keyValuePair.Key))
            runSummary.RunSummaryByState[keyValuePair.Key] = keyValuePair.Value;
          else
            runSummary.RunSummaryByState[keyValuePair.Key] += keyValuePair.Value;
        }
      }
      if (sum1 != null && sum1.RunSummaryByOutcome != null)
      {
        foreach (KeyValuePair<TestRunOutcome, int> keyValuePair in (IEnumerable<KeyValuePair<TestRunOutcome, int>>) sum1.RunSummaryByOutcome)
          runSummary.RunSummaryByOutcome[keyValuePair.Key] = keyValuePair.Value;
      }
      if (sum2 != null && sum2.RunSummaryByOutcome != null)
      {
        foreach (KeyValuePair<TestRunOutcome, int> keyValuePair in (IEnumerable<KeyValuePair<TestRunOutcome, int>>) sum2.RunSummaryByOutcome)
        {
          if (!runSummary.RunSummaryByOutcome.ContainsKey(keyValuePair.Key))
            runSummary.RunSummaryByOutcome[keyValuePair.Key] = keyValuePair.Value;
          else
            runSummary.RunSummaryByOutcome[keyValuePair.Key] += keyValuePair.Value;
        }
      }
      return runSummary;
    }

    internal static ResultSummary MergeResultsSummary(ResultSummary resSum1, ResultSummary resSum2)
    {
      if (resSum1 == null && resSum2 == null)
        return (ResultSummary) null;
      ResultSummary resultSummary = new ResultSummary()
      {
        ResultSummaryByRunState = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, ResultsSummaryByOutcome>()
      };
      if (resSum1 != null && resSum1.ResultSummaryByRunState != null)
      {
        foreach (KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, ResultsSummaryByOutcome> keyValuePair in resSum1.ResultSummaryByRunState)
          resultSummary.ResultSummaryByRunState[keyValuePair.Key] = keyValuePair.Value;
      }
      if (resSum2 != null && resSum2.ResultSummaryByRunState != null)
      {
        foreach (KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, ResultsSummaryByOutcome> keyValuePair in resSum2.ResultSummaryByRunState)
          resultSummary.ResultSummaryByRunState[keyValuePair.Key] = !resultSummary.ResultSummaryByRunState.ContainsKey(keyValuePair.Key) ? keyValuePair.Value : MetricsCalculatorHelper.MergeResultsSummaryByOutcome(resultSummary.ResultSummaryByRunState[keyValuePair.Key], keyValuePair.Value);
      }
      return resultSummary;
    }

    internal static ResultsSummaryByOutcome MergeResultsSummaryByOutcome(
      ResultsSummaryByOutcome sum1,
      ResultsSummaryByOutcome sum2)
    {
      if (sum1 == null && sum2 == null)
        return (ResultsSummaryByOutcome) null;
      ResultsSummaryByOutcome summaryByOutcome = new ResultsSummaryByOutcome();
      summaryByOutcome.TotalTestCount = 0;
      summaryByOutcome.NotReportedTestCount = 0;
      summaryByOutcome.Duration = TimeSpan.FromMilliseconds(0.0);
      summaryByOutcome.AggregatedResultDetailsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome>();
      if (sum1 != null)
      {
        summaryByOutcome.TotalTestCount += sum1.TotalTestCount;
        summaryByOutcome.NotReportedTestCount += sum1.NotReportedTestCount;
        summaryByOutcome.Duration = sum1.Duration;
      }
      if (sum2 != null)
      {
        summaryByOutcome.TotalTestCount += sum2.TotalTestCount;
        summaryByOutcome.NotReportedTestCount += sum2.NotReportedTestCount;
        summaryByOutcome.Duration = Validator.CheckOverflowAndGetSafeValue(summaryByOutcome.Duration, sum2.Duration);
      }
      if (sum1 != null && sum1.AggregatedResultDetailsByOutcome != null && sum1.AggregatedResultDetailsByOutcome.Values.Any<AggregatedResultDetailsByOutcome>())
      {
        foreach (AggregatedResultDetailsByOutcome detailsByOutcome in (IEnumerable<AggregatedResultDetailsByOutcome>) sum1.AggregatedResultDetailsByOutcome.Values)
          summaryByOutcome.AggregatedResultDetailsByOutcome[detailsByOutcome.Outcome] = new AggregatedResultDetailsByOutcome()
          {
            Outcome = detailsByOutcome.Outcome,
            Count = detailsByOutcome.Count,
            Duration = detailsByOutcome.Duration,
            RerunResultCount = detailsByOutcome.RerunResultCount
          };
      }
      if (sum2 != null && sum2.AggregatedResultDetailsByOutcome != null && sum2.AggregatedResultDetailsByOutcome.Values.Any<AggregatedResultDetailsByOutcome>())
      {
        foreach (AggregatedResultDetailsByOutcome detailsByOutcome in (IEnumerable<AggregatedResultDetailsByOutcome>) sum2.AggregatedResultDetailsByOutcome.Values)
        {
          if (summaryByOutcome.AggregatedResultDetailsByOutcome.ContainsKey(detailsByOutcome.Outcome))
          {
            summaryByOutcome.AggregatedResultDetailsByOutcome[detailsByOutcome.Outcome].Count += detailsByOutcome.Count;
            summaryByOutcome.AggregatedResultDetailsByOutcome[detailsByOutcome.Outcome].RerunResultCount += detailsByOutcome.RerunResultCount;
            summaryByOutcome.AggregatedResultDetailsByOutcome[detailsByOutcome.Outcome].Duration = Validator.CheckOverflowAndGetSafeValue(summaryByOutcome.AggregatedResultDetailsByOutcome[detailsByOutcome.Outcome].Duration, detailsByOutcome.Duration);
          }
          else
            summaryByOutcome.AggregatedResultDetailsByOutcome[detailsByOutcome.Outcome] = new AggregatedResultDetailsByOutcome()
            {
              Outcome = detailsByOutcome.Outcome,
              Count = detailsByOutcome.Count,
              RerunResultCount = detailsByOutcome.RerunResultCount,
              Duration = detailsByOutcome.Duration
            };
        }
      }
      return summaryByOutcome;
    }

    internal static PipelineReference GetPipelineReferenceForANodeLevel(
      PipelineReference pipelineReference,
      PipelineNodeHierarchyLevel level)
    {
      if (pipelineReference == null)
        return (PipelineReference) null;
      PipelineReference referenceForAnodeLevel = new PipelineReference()
      {
        PipelineId = pipelineReference.PipelineId
      };
      if (level >= PipelineNodeHierarchyLevel.Stage && pipelineReference.StageReference != null)
        referenceForAnodeLevel.StageReference = new StageReference()
        {
          StageName = pipelineReference.StageReference.StageName
        };
      if (level >= PipelineNodeHierarchyLevel.Phase && pipelineReference.PhaseReference != null)
        referenceForAnodeLevel.PhaseReference = new PhaseReference()
        {
          PhaseName = pipelineReference.PhaseReference.PhaseName
        };
      if (level >= PipelineNodeHierarchyLevel.Job && pipelineReference.JobReference != null)
        referenceForAnodeLevel.JobReference = new JobReference()
        {
          JobName = pipelineReference.JobReference.JobName
        };
      return referenceForAnodeLevel;
    }

    internal static AggregatedResultsDifference CalculateAggregatedResultsDifference(
      IVssRequestContext requestContext,
      List<RunSummaryByOutcomeInPipeline> prevRunSummaryByOutcome,
      int currentTotalTestCount,
      int currentPassTestCount,
      int currentFailTestCount,
      int currentNonImpactedTestCount,
      int currentOtherTestCount,
      long currentAggregatedRunDurationInMs)
    {
      AggregatedResultsDifference resultsDifference = new AggregatedResultsDifference();
      int totalTests = 0;
      int passedCount = 0;
      int failedCount = 0;
      int othersCount = 0;
      int notImpactedCount = 0;
      long aggregatedRunDurationInMs = 0;
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome> dictionary = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome>();
      MetricsCalculatorHelper.CalculateCountOfResultsForDifferentOutcome(MetricsCalculatorHelper.CalculateAggregateDataByOutCome(requestContext, prevRunSummaryByOutcome, out aggregatedRunDurationInMs, out int _), out totalTests, out passedCount, out failedCount, out notImpactedCount, out othersCount);
      resultsDifference.IncreaseInTotalTests = currentTotalTestCount - totalTests;
      resultsDifference.IncreaseInFailures = currentFailTestCount - failedCount;
      resultsDifference.IncreaseInPassedTests = currentPassTestCount - passedCount;
      resultsDifference.IncreaseInNonImpactedTests = currentNonImpactedTestCount - notImpactedCount;
      resultsDifference.IncreaseInOtherTests = currentOtherTestCount - othersCount;
      resultsDifference.IncreaseInDuration = TimeSpan.FromMilliseconds((double) (currentAggregatedRunDurationInMs - aggregatedRunDurationInMs));
      return resultsDifference;
    }

    internal static Dictionary<TestRunOutcome, int> CalculateAggregateRunSummaryByOutcome(
      List<RunSummaryByOutcomeInPipeline> summaryByOutcome)
    {
      Dictionary<TestRunOutcome, int> summaryByOutcome1 = new Dictionary<TestRunOutcome, int>();
      if (summaryByOutcome == null)
        return summaryByOutcome1;
      foreach (IEnumerable<RunSummaryByOutcomeInPipeline> source in summaryByOutcome.GroupBy<RunSummaryByOutcomeInPipeline, int>((Func<RunSummaryByOutcomeInPipeline, int>) (rs => rs.TestRunId)))
      {
        int passedCount;
        int failedCount;
        int nonImpactedCount;
        MetricsCalculatorHelper.CalculateCountOfResultsForDifferentOutcome(source.ToList<RunSummaryByOutcomeInPipeline>(), out int _, out passedCount, out failedCount, out nonImpactedCount, out int _);
        TestRunOutcome key = failedCount != 0 || passedCount <= 0 ? (failedCount <= 0 ? (nonImpactedCount <= 0 ? TestRunOutcome.Others : TestRunOutcome.NotImpacted) : TestRunOutcome.Failed) : TestRunOutcome.Passed;
        if (!summaryByOutcome1.ContainsKey(key))
          summaryByOutcome1[key] = 1;
        else
          ++summaryByOutcome1[key];
      }
      return summaryByOutcome1;
    }

    internal static Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome> CalculateAggregateDataByOutCome(
      IVssRequestContext requestContext,
      List<RunSummaryByOutcomeInPipeline> runSummaryByOutcome,
      out long aggregatedRunDurationInMs,
      out int notReportedTestCount)
    {
      bool flag = false;
      Microsoft.TeamFoundation.TestManagement.Client.TestOutcome testOutcome = Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.NotExecuted;
      aggregatedRunDurationInMs = 0L;
      notReportedTestCount = 0;
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome> aggregateDataByOutCome = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome>();
      SortedSet<RunSummaryByOutcomeInPipeline> runSummary = new SortedSet<RunSummaryByOutcomeInPipeline>((IComparer<RunSummaryByOutcomeInPipeline>) new TestRunComparer2());
      if (requestContext.IsFeatureEnabled("TestManagement.Server.TRIReportCustomization"))
        flag = true;
      foreach (RunSummaryByOutcomeInPipeline outcomeInPipeline in (IEnumerable<RunSummaryByOutcomeInPipeline>) runSummaryByOutcome ?? Enumerable.Empty<RunSummaryByOutcomeInPipeline>())
      {
        if ((int) outcomeInPipeline.ResultMetadata == (int) Convert.ToByte((object) ResultMetadata.Flaky))
          notReportedTestCount += outcomeInPipeline.ResultCount;
        else if (flag && outcomeInPipeline.TestOutcome == testOutcome)
        {
          notReportedTestCount += outcomeInPipeline.ResultCount;
          runSummary.Add(outcomeInPipeline);
        }
        else
        {
          runSummary.Add(outcomeInPipeline);
          if (!aggregateDataByOutCome.ContainsKey((Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) outcomeInPipeline.TestOutcome))
          {
            aggregateDataByOutCome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) outcomeInPipeline.TestOutcome] = new AggregatedResultDetailsByOutcome()
            {
              Outcome = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) outcomeInPipeline.TestOutcome,
              Count = outcomeInPipeline.ResultCount,
              RerunResultCount = (int) outcomeInPipeline.ResultMetadata == (int) Convert.ToByte((object) ResultMetadata.Rerun) ? outcomeInPipeline.ResultCount : 0,
              Duration = TimeSpan.FromMilliseconds((double) Validator.CheckOverflowAndGetSafeValue(outcomeInPipeline.ResultDuration, 0L))
            };
          }
          else
          {
            aggregateDataByOutCome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) outcomeInPipeline.TestOutcome].Count += outcomeInPipeline.ResultCount;
            aggregateDataByOutCome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) outcomeInPipeline.TestOutcome].RerunResultCount += (int) outcomeInPipeline.ResultMetadata == (int) Convert.ToByte((object) ResultMetadata.Rerun) ? outcomeInPipeline.ResultCount : 0;
            long safeValue = Validator.CheckOverflowAndGetSafeValue((long) aggregateDataByOutCome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) outcomeInPipeline.TestOutcome].Duration.TotalMilliseconds, outcomeInPipeline.ResultDuration);
            aggregateDataByOutCome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) outcomeInPipeline.TestOutcome].Duration = TimeSpan.FromMilliseconds((double) safeValue);
          }
        }
      }
      aggregatedRunDurationInMs = TestManagementServiceUtility.CalculateEffectiveTestRunDurationInPipeline(runSummary);
      return aggregateDataByOutCome;
    }

    internal static void CalculateCountOfResultsForDifferentOutcome(
      List<RunSummaryByOutcomeInPipeline> summaryByOutcome,
      out int totalTests,
      out int passedCount,
      out int failedCount,
      out int nonImpactedCount,
      out int othersCount)
    {
      othersCount = 0;
      passedCount = 0;
      failedCount = 0;
      nonImpactedCount = 0;
      totalTests = 0;
      if (summaryByOutcome == null || !summaryByOutcome.Any<RunSummaryByOutcomeInPipeline>())
        return;
      MetricsCalculatorHelper.CalculateCountOfResultsForDifferentOutcomeInternal(summaryByOutcome.Select<RunSummaryByOutcomeInPipeline, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>((Func<RunSummaryByOutcomeInPipeline, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>) (x => new KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>((Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) x.TestOutcome, x.ResultCount))).ToList<KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>(), out totalTests, out passedCount, out failedCount, out nonImpactedCount, out othersCount);
    }

    internal static void CalculateCountOfResultsForDifferentOutcome(
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultDetailsByOutcome> aggregatedResultsByOutcomeMap,
      out int totalTests,
      out int passedCount,
      out int failedCount,
      out int notImpactedCount,
      out int othersCount)
    {
      othersCount = 0;
      passedCount = 0;
      failedCount = 0;
      totalTests = 0;
      notImpactedCount = 0;
      if (aggregatedResultsByOutcomeMap == null || !aggregatedResultsByOutcomeMap.Values.Any<AggregatedResultDetailsByOutcome>())
        return;
      MetricsCalculatorHelper.CalculateCountOfResultsForDifferentOutcomeInternal(aggregatedResultsByOutcomeMap.Values.Select<AggregatedResultDetailsByOutcome, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>((Func<AggregatedResultDetailsByOutcome, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>) (x => new KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>(x.Outcome, x.Count))).ToList<KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>(), out totalTests, out passedCount, out failedCount, out notImpactedCount, out othersCount);
    }

    internal static void CalculateCountOfResultsForDifferentOutcomeInternal(
      List<KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>> countByOutcome,
      out int totalTests,
      out int passedCount,
      out int failedCount,
      out int nonImpactedCount,
      out int othersCount)
    {
      othersCount = 0;
      passedCount = 0;
      failedCount = 0;
      nonImpactedCount = 0;
      totalTests = 0;
      foreach (KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int> keyValue in countByOutcome)
      {
        int num = keyValue.Value;
        switch (keyValue.Key)
        {
          case Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Passed:
            passedCount += num;
            totalTests += num;
            continue;
          case Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed:
            failedCount += num;
            totalTests += num;
            continue;
          case Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.NotImpacted:
            nonImpactedCount += num;
            totalTests += num;
            continue;
          default:
            totalTests += num;
            othersCount += num;
            continue;
        }
      }
    }

    internal static TestResultFailuresAnalysis CalculateTestFailuresAnalysis(
      List<ResultInsightsInPipeline> resultInsights)
    {
      TestResultFailuresAnalysis failuresAnalysis = new TestResultFailuresAnalysis();
      if (resultInsights != null && resultInsights.Count > 0)
      {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList1 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList3 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
        foreach (ResultInsightsInPipeline insightsInPipeline in (IEnumerable<ResultInsightsInPipeline>) resultInsights ?? Enumerable.Empty<ResultInsightsInPipeline>())
        {
          num1 += insightsInPipeline.NewFailures;
          num2 += insightsInPipeline.ExistingFailures;
          num3 += insightsInPipeline.FixedTests;
          int testRunId = insightsInPipeline.TestRunId;
          if (!string.IsNullOrEmpty(insightsInPipeline.NewFailedResults))
            resultIdentifierList1.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) MetricsCalculatorHelper.GetResultIds(insightsInPipeline.NewFailedResults, testRunId));
          if (!string.IsNullOrEmpty(insightsInPipeline.ExistingFailedResults))
            resultIdentifierList2.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) MetricsCalculatorHelper.GetResultIds(insightsInPipeline.ExistingFailedResults, testRunId));
          if (!string.IsNullOrEmpty(insightsInPipeline.FixedTestResults))
            resultIdentifierList3.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) MetricsCalculatorHelper.GetResultIds(insightsInPipeline.FixedTestResults, testRunId));
        }
        failuresAnalysis.NewFailures = new TestFailureDetails()
        {
          Count = num1,
          TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList1
        };
        failuresAnalysis.ExistingFailures = new TestFailureDetails()
        {
          Count = num2,
          TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList2
        };
        failuresAnalysis.FixedTests = new TestFailureDetails()
        {
          Count = num3,
          TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList3
        };
      }
      return failuresAnalysis;
    }

    private static List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> GetResultIds(
      string failureDetails,
      int runId)
    {
      string[] strArray = failureDetails.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIds = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
      foreach (string str in strArray)
        resultIds.Add(new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier()
        {
          TestRunId = runId,
          TestResultId = Convert.ToInt32(str)
        });
      return resultIds;
    }
  }
}
