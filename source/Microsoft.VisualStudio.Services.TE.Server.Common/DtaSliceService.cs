// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaSliceService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  internal class DtaSliceService : IDtaSliceService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IEnumerable<TestAutomationRunSlice> QuerySlicesByTestRunId(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200241, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaSliceService:QuerySlicesByTestRunId() requested for testRunId: {0}", (object) testRunId));
      IEnumerable<TestAutomationRunSlice> automationRunSlices = (IEnumerable<TestAutomationRunSlice>) null;
      try
      {
        using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
          automationRunSlices = component.QuerySlicesByTestRunId(testRunId);
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200242, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaSliceService:QuerySlicesByTestRunId() thrown exception {0} for testRunId: {1}", (object) ex, (object) testRunId));
      }
      requestContext.RequestContext.Trace(6200243, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaSliceService:QuerySlicesByTestRunId() returned no results: {0}", (object) testRunId));
      return automationRunSlices;
    }

    public TestAutomationRunSlice QuerySliceBySliceId(
      TestExecutionRequestContext requestContext,
      int sliceId)
    {
      requestContext.RequestContext.Trace(6200241, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaSliceService:QuerySliceBySliceId() requested for sliceId: {0}", (object) sliceId));
      TestAutomationRunSlice automationRunSlice = (TestAutomationRunSlice) null;
      try
      {
        using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
          automationRunSlice = component.QuerySliceById(sliceId);
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200242, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaSliceService:QuerySliceBySliceId() thrown exception {0} for sliceId: {1}", (object) ex, (object) sliceId));
      }
      requestContext.RequestContext.Trace(6200243, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaSliceService:QuerySliceBySliceId() returned no results: {0}", (object) sliceId));
      return automationRunSlice;
    }
  }
}
