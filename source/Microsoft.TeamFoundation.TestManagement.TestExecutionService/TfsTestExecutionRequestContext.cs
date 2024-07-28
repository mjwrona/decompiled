// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsTestExecutionRequestContext
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestExecution.Server;
using Microsoft.TeamFoundation.TestImpact.Server;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TfsTestExecutionRequestContext : TestExecutionRequestContext
  {
    public TfsTestExecutionRequestContext(IVssRequestContext requestContext)
      : base(requestContext)
    {
      this.SupportsRftWorkflow = true;
    }

    public TfsTestExecutionRequestContext(
      IVssRequestContext requestContext,
      string area,
      string layer)
      : base(requestContext, area, layer)
    {
      this.SupportsRftWorkflow = true;
    }

    public override ITestResultsHttpClient TestResultsHttpClient
    {
      get
      {
        if (this.m_testResultsClient == null)
          this.m_testResultsClient = (ITestResultsHttpClient) this.RequestContext.GetClient<TestManagementHttpClient>();
        return this.m_testResultsClient;
      }
      set => this.m_testResultsClient = value;
    }

    public override TestImpactRequestContext TestImpactRequestContext
    {
      get
      {
        if (this.m_testImpactRequestContext == null)
          this.m_testImpactRequestContext = (TestImpactRequestContext) new TfsTestImpactRequestContext(this.RequestContext);
        return this.m_testImpactRequestContext;
      }
    }

    public override Guid TestAgentArtifactKindId => DtaConstants.TestAgentArtifactKindId;

    public override Guid AutomationRunArtifactKindId => DtaConstants.AutomationRunArtifactKindId;

    public override Guid HealthMonitorJobId => new Guid(DtaConstants.HealthMonitorJobId);

    public override string WorkFlowJobExtensionName => DtaConstants.WorkFlowJobExtensionName;

    public override string WorkFlowJobName => DtaConstants.WorkFlowJobName;
  }
}
