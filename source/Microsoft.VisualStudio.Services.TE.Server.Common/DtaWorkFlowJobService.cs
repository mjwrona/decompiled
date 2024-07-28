// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaWorkFlowJobService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaWorkFlowJobService : IDtaWorkFlowJobService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public WorkFlowJobDetails GetWorkFlowJobDetails(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200241, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaWorkFlowJobService:GetworkFlowJobDetails() requested for testRunId: {0}", (object) testRunId));
      try
      {
        using (DtaWorkFlowJobDatabase component = requestContext.RequestContext.CreateComponent<DtaWorkFlowJobDatabase>())
        {
          WorkFlowJobDetails workFlowJobDetails = component.QueryWorkflowJob(testRunId);
          if (workFlowJobDetails != null)
            return workFlowJobDetails;
        }
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200242, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaWorkFlowJobService:GetworkFlowJobDetails() threw exception {0} for testRunId: {1}", (object) ex, (object) testRunId));
        throw;
      }
      requestContext.RequestContext.Trace(6200243, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaWorkFlowJobService:GetworkFlowJobDetails() returned no results: {0}", (object) testRunId));
      return (WorkFlowJobDetails) null;
    }

    public void UpdateWorkFlowJobDetails(
      TestExecutionRequestContext requestContext,
      WorkFlowJobDetails workFlowJobDetails)
    {
      requestContext.RequestContext.Trace(6200241, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaWorkFlowJobService:UpdateworkFlowJobDetails() requested for testRunId: {0}", (object) workFlowJobDetails.TestRunId));
      try
      {
        using (DtaWorkFlowJobDatabase component = requestContext.RequestContext.CreateComponent<DtaWorkFlowJobDatabase>())
          component.UpdateWorkFlowJob(workFlowJobDetails);
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200242, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaWorkFlowJobService:UpdateworkFlowJobDetails() threw exception {0} for testRunId: {1}", (object) ex, (object) workFlowJobDetails.TestRunId));
        throw;
      }
    }

    public void CleanUpWorkflowArtifacts(TestExecutionRequestContext requestContext, int testRunId)
    {
      try
      {
        using (DtaWorkFlowJobDatabase component = requestContext.RequestContext.CreateComponent<DtaWorkFlowJobDatabase>())
          component.CleanupTestRunArtifacts(testRunId);
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200243, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("DtaWorkFlowJobService:CleanUpWorkflowArtifacts() threw exception {0} for testRunId: {1}", (object) ex, (object) testRunId));
        throw;
      }
    }
  }
}
