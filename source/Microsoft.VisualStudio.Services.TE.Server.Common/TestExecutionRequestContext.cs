// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionRequestContext
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionRequestContext
  {
    private IdentityService m_identityService;
    private DefaultTestExecutionSecurityManager m_testExecutionSecurityManager;
    private IProjectServiceHelper m_projectServiceHelper;
    private TestPlanHttpClient m_testPlanClient;
    protected TestImpactRequestContext m_testImpactRequestContext;
    protected ITestResultsHttpClient m_testResultsClient;

    public TestExecutionRequestContext(IVssRequestContext requestContext)
    {
      this.RequestContext = requestContext;
      this.Logger = new TCMLogger(requestContext);
    }

    public TestExecutionRequestContext(
      IVssRequestContext requestContext,
      string area,
      string layer)
    {
      this.RequestContext = requestContext;
      this.Logger = new TCMLogger(requestContext, area, layer);
    }

    public IVssRequestContext RequestContext { get; }

    public virtual IdentityService IdentityService
    {
      get
      {
        if (this.m_identityService == null)
          this.m_identityService = this.RequestContext.GetService<IdentityService>();
        return this.m_identityService;
      }
    }

    public virtual IProjectServiceHelper ProjectServiceHelper
    {
      get
      {
        if (this.m_projectServiceHelper == null)
          this.m_projectServiceHelper = (IProjectServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.ProjectServiceHelper(this.RequestContext);
        return this.m_projectServiceHelper;
      }
      set => this.m_projectServiceHelper = value;
    }

    public virtual ITestResultsHttpClient TestResultsHttpClient
    {
      get
      {
        if (this.m_testResultsClient == null)
          this.m_testResultsClient = (ITestResultsHttpClient) this.RequestContext.GetClient<Microsoft.VisualStudio.Services.TestResults.WebApi.TestResultsHttpClient>();
        return this.m_testResultsClient;
      }
      set => this.m_testResultsClient = value;
    }

    public virtual TestPlanHttpClient TestPlanningHttpClient
    {
      get
      {
        if (this.m_testPlanClient == null)
          this.m_testPlanClient = this.RequestContext.GetClient<TestPlanHttpClient>();
        return this.m_testPlanClient;
      }
      set => this.m_testPlanClient = value;
    }

    public string E2EId => this.RequestContext == null ? string.Empty : this.RequestContext.E2EId.ToString();

    public virtual TestImpactRequestContext TestImpactRequestContext
    {
      get
      {
        if (this.m_testImpactRequestContext == null)
          this.m_testImpactRequestContext = new TestImpactRequestContext(this.RequestContext);
        return this.m_testImpactRequestContext;
      }
    }

    public virtual Guid TestAgentArtifactKindId => DtaConstants.TcmTestAgentArtifactKindId;

    public virtual Guid AutomationRunArtifactKindId => DtaConstants.TcmAutomationRunArtifactKindId;

    public virtual Guid HealthMonitorJobId => DtaConstants.TcmHealthMonitorJobId;

    public virtual string WorkFlowJobExtensionName => DtaConstants.TcmWorkFlowJobExtensionName;

    public virtual string WorkFlowJobName => DtaConstants.TcmWorkFlowJobName;

    public virtual bool SupportsRftWorkflow { get; set; }

    public DefaultTestExecutionSecurityManager SecurityManager
    {
      get
      {
        if (this.m_testExecutionSecurityManager == null)
          this.m_testExecutionSecurityManager = new DefaultTestExecutionSecurityManager();
        return this.m_testExecutionSecurityManager;
      }
      set => this.m_testExecutionSecurityManager = value;
    }

    public virtual TCMLogger Logger { get; private set; }
  }
}
