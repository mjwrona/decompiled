// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestReportsDataTransformPipeline
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Charting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestReportsDataTransformPipeline : 
    IDataTransformPipeline,
    IDataServicesService,
    IChartTransformer
  {
    internal TestReportsDataTransformPipeline(IVssRequestContext context)
    {
      this.RequestContext = context;
      this.TCMRequestContext = new TfsTestManagementRequestContext(context);
    }

    public IVssRequestContext RequestContext { get; set; }

    public TfsTestManagementRequestContext TCMRequestContext { get; set; }

    public IEnumerable<TransformResult> GetResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformOptions> options)
    {
      try
      {
        this.TCMRequestContext.TraceEnter("BusinessLayer", "TestReportsDataTransformPipeline.GetResults()");
        List<TransformResult> results = new List<TransformResult>();
        if (options == null || options.Count<TransformOptions>() == 0)
          return (IEnumerable<TransformResult>) results;
        this.GetTestExecutionChartResults(requestContext, transformSecurityInformation, options, results);
        this.GetTestAuthoringChartResults(requestContext, transformSecurityInformation, options, results);
        this.GetTestRunSummaryChartResults(requestContext, transformSecurityInformation, options, results);
        return (IEnumerable<TransformResult>) results;
      }
      finally
      {
        this.TCMRequestContext.TraceLeave("BusinessLayer", "TestReportsDataTransformPipeline.GetResults()");
      }
    }

    private void GetTestExecutionChartResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformOptions> options,
      List<TransformResult> results)
    {
      try
      {
        this.TCMRequestContext.TraceEnter("BusinessLayer", "TestReportsDataTransformPipeline.GetTestExecutionChartResults()");
        IEnumerable<TransformOptions> transformOptionses = options.Where<TransformOptions>((Func<TransformOptions, bool>) (s => s.Filter.ToLowerInvariant().Contains("execution")));
        if (transformOptionses == null || transformOptionses.Count<TransformOptions>() <= 0)
          return;
        TestExecutionDataTransformPipeline<DatedTestFieldData> transformPipeline = new TestExecutionDataTransformPipeline<DatedTestFieldData>();
        transformPipeline.RequestContext = requestContext;
        TestExecutionProviderInterpreter providerInterpreter = new TestExecutionProviderInterpreter();
        transformPipeline.DataInterpreter = (IInterpretTimedData<DatedTestFieldData>) providerInterpreter;
        transformPipeline.DataProvider = (IProvideFilteredData<DatedTestFieldData>) providerInterpreter;
        results.AddRange(transformPipeline.GetResults(requestContext, transformSecurityInformation, transformOptionses));
      }
      finally
      {
        this.TCMRequestContext.TraceLeave("BusinessLayer", "TestReportsDataTransformPipeline.GetTestExecutionChartResults()");
      }
    }

    private void GetTestAuthoringChartResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformOptions> options,
      List<TransformResult> results)
    {
      try
      {
        this.TCMRequestContext.TraceEnter("BusinessLayer", "TestReportsDataTransformPipeline.GetTestAuthoringChartResults()");
        IEnumerable<TransformOptions> transformOptionses = options.Where<TransformOptions>((Func<TransformOptions, bool>) (s => s.Filter.ToLowerInvariant().Contains("authoring")));
        if (transformOptionses == null || transformOptionses.Count<TransformOptions>() <= 0)
          return;
        TestAuthoringDataTransformPipeline<DatedWorkItemFieldData> transformPipeline = new TestAuthoringDataTransformPipeline<DatedWorkItemFieldData>();
        transformPipeline.RequestContext = requestContext;
        TestAuthoringProviderInterpreter providerInterpreter = new TestAuthoringProviderInterpreter();
        transformPipeline.DataInterpreter = (IInterpretTimedData<DatedWorkItemFieldData>) providerInterpreter;
        transformPipeline.DataProvider = (IProvideFilteredData<DatedWorkItemFieldData>) providerInterpreter;
        results.AddRange(transformPipeline.GetResults(requestContext, transformSecurityInformation, transformOptionses));
      }
      finally
      {
        this.TCMRequestContext.TraceLeave("BusinessLayer", "TestReportsDataTransformPipeline.GetTestAuthoringChartResults()");
      }
    }

    private void GetTestRunSummaryChartResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformOptions> options,
      List<TransformResult> results)
    {
      try
      {
        this.TCMRequestContext.TraceEnter("BusinessLayer", "TestReportsDataTransformPipeline.GetTestRunSummaryChartResults()");
        IEnumerable<TransformOptions> transformOptionses = options.Where<TransformOptions>((Func<TransformOptions, bool>) (s => s.Filter.ToLowerInvariant().Contains("runsummary")));
        if (transformOptionses == null || transformOptionses.Count<TransformOptions>() <= 0)
          return;
        TestRunDataTransformPipeline<DatedTestFieldData> transformPipeline = new TestRunDataTransformPipeline<DatedTestFieldData>();
        transformPipeline.RequestContext = requestContext;
        TestRunProviderInterpreter providerInterpreter = new TestRunProviderInterpreter();
        transformPipeline.DataInterpreter = (IInterpretTimedData<DatedTestFieldData>) providerInterpreter;
        transformPipeline.DataProvider = (IProvideFilteredData<DatedTestFieldData>) providerInterpreter;
        results.AddRange(transformPipeline.GetResults(requestContext, transformSecurityInformation, transformOptionses));
      }
      finally
      {
        this.TCMRequestContext.TraceLeave("BusinessLayer", "TestReportsDataTransformPipeline.GetTestRunSummaryChartResults()");
      }
    }

    public IInterpretQueryText GetQueryTextInterpreter() => (IInterpretQueryText) null;
  }
}
