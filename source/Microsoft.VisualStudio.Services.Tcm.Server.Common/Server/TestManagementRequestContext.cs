// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementRequestContext
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementRequestContext
  {
    protected IVssRequestContext m_requestContext;
    protected IdentityService m_identityService;
    protected TeamFoundationEventService m_eventService;
    protected Guid m_userTeamFoundationId;
    protected TestManagementHost m_testManagementHost;
    protected ICommonStructureServiceHelper m_cssHelper;
    protected ITcmServiceHelper m_tcmHelper;
    protected ITfsServiceHelper m_tfsHelper;
    protected ITestPointOutcomeHelper m_testPointOutcomeHelper;
    protected IMergeDataHelper m_mergeDataHelper;
    protected IWorkItemServiceHelper m_WorkItemServiceHelper;
    protected IWorkItemFieldDataHelper m_workItemFieldDataHelper;
    protected IProjectServiceHelper m_projectServiceHelper;
    protected IPlannedTestResultsHelper m_plannedTestResultsHelper;
    protected IFeedbackHelper m_feedbackHelper;
    private string m_userTeamFoundationName;
    private string m_userDistinctTeamFoundationName;
    protected Dictionary<string, RestApiResourceDetails> m_resourceMappings;

    public TestManagementRequestContext(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_testManagementHost = requestContext.GetService<TestManagementHost>();
      this.Logger = new TCMLogger(requestContext);
    }

    public TestManagementRequestContext(
      IVssRequestContext requestContext,
      string area,
      string layer)
    {
      this.m_requestContext = requestContext;
      this.m_testManagementHost = requestContext.GetService<TestManagementHost>();
      this.Logger = new TCMLogger(requestContext, area, layer);
    }

    public virtual IVssRequestContext RequestContext => this.m_requestContext;

    public virtual TestManagementHost TestManagementHost => this.m_testManagementHost;

    public virtual IdentityService IdentityService
    {
      get
      {
        if (this.m_identityService == null)
          this.m_identityService = this.RequestContext.GetService<IdentityService>();
        return this.m_identityService;
      }
    }

    public virtual ITeamFoundationEventService EventService
    {
      get
      {
        if (this.m_eventService == null)
          this.m_eventService = this.RequestContext.GetService<TeamFoundationEventService>();
        return (ITeamFoundationEventService) this.m_eventService;
      }
    }

    public virtual ISecurityManager SecurityManager => this.TestManagementHost.SecurityManager;

    public virtual IdentityDescriptor UserPrincipal => this.RequestContext.UserContext;

    public virtual Guid UserTeamFoundationId
    {
      get
      {
        if (this.m_userTeamFoundationId == Guid.Empty && this.UserPrincipal != (IdentityDescriptor) null)
          this.UpdateCurrentUserIdentityDetails();
        return this.m_userTeamFoundationId;
      }
    }

    public virtual string UserTeamFoundationName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_userTeamFoundationName) && this.UserPrincipal != (IdentityDescriptor) null)
          this.UpdateCurrentUserIdentityDetails();
        return this.m_userTeamFoundationName;
      }
    }

    public virtual string UserDistinctTeamFoundationName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_userDistinctTeamFoundationName) && this.UserPrincipal != (IdentityDescriptor) null)
          this.UpdateCurrentUserIdentityDetails();
        return this.m_userDistinctTeamFoundationName;
      }
    }

    public virtual string UserSID => this.UserPrincipal.Identifier;

    public virtual ICssCache AreaPathsCache => this.m_testManagementHost.Replicator.GetAreaPathsCache(this);

    public virtual ICssCache IterationsCache => this.m_testManagementHost.Replicator.GetIterationsCache(this);

    public virtual ICommonStructureServiceHelper CSSHelper
    {
      get
      {
        if (this.m_cssHelper == null)
          this.m_cssHelper = (ICommonStructureServiceHelper) new DefaultCommonStructureServiceHelper(this.RequestContext);
        return this.m_cssHelper;
      }
    }

    public virtual ITcmServiceHelper TcmServiceHelper
    {
      get
      {
        if (this.m_tcmHelper == null)
          this.m_tcmHelper = (ITcmServiceHelper) new DefaultTcmServiceHelper(this.RequestContext);
        return this.m_tcmHelper;
      }
    }

    public virtual ITfsServiceHelper TfsServiceHelper
    {
      get
      {
        if (this.m_tfsHelper == null)
          this.m_tfsHelper = (ITfsServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.TfsServiceHelper(this.RequestContext);
        return this.m_tfsHelper;
      }
    }

    public virtual ITestPointOutcomeHelper TestPointOutcomeHelper
    {
      get
      {
        if (this.m_testPointOutcomeHelper == null)
          this.m_testPointOutcomeHelper = (ITestPointOutcomeHelper) new DefaultTestPointOutcomeHelper();
        return this.m_testPointOutcomeHelper;
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
    }

    public virtual IWorkItemServiceHelper WorkItemServiceHelper
    {
      get
      {
        if (this.m_WorkItemServiceHelper == null)
          this.m_WorkItemServiceHelper = (IWorkItemServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.WorkItemServiceHelper(this.RequestContext);
        return this.m_WorkItemServiceHelper;
      }
    }

    public virtual IMergeDataHelper MergeDataHelper
    {
      get
      {
        if (this.m_mergeDataHelper == null)
          this.m_mergeDataHelper = (IMergeDataHelper) new MergeTcmDataHelper();
        return this.m_mergeDataHelper;
      }
    }

    public virtual IWorkItemFieldDataHelper WorkItemFieldDataHelper
    {
      get
      {
        if (this.m_workItemFieldDataHelper == null)
          this.m_workItemFieldDataHelper = (IWorkItemFieldDataHelper) new DefaultWorkItemDataHelper();
        return this.m_workItemFieldDataHelper;
      }
    }

    public virtual IPlannedTestResultsHelper PlannedTestResultsHelper
    {
      get
      {
        if (this.m_plannedTestResultsHelper == null)
          this.m_plannedTestResultsHelper = (IPlannedTestResultsHelper) new DefaultPlannedTestResultsHelper();
        return this.m_plannedTestResultsHelper;
      }
    }

    internal virtual IFeedbackHelper FeedbackHelper
    {
      get
      {
        if (this.m_feedbackHelper == null)
          this.m_feedbackHelper = (IFeedbackHelper) new DefaultFeedbackHelper();
        return this.m_feedbackHelper;
      }
    }

    public virtual bool IsDataTierOld => this.m_requestContext.GetService<ITeamFoundationResourceManagementService>().GetServiceVersion(this.m_requestContext, "TestManagement", "Default").Version != 66;

    public virtual bool IsTcmService => this.m_requestContext.ServiceInstanceType() == TestManagementServerConstants.TCMServiceInstanceType || this.m_requestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    public virtual Dictionary<string, RestApiResourceDetails> ResourceMappings
    {
      get
      {
        if (this.m_resourceMappings == null)
        {
          this.m_resourceMappings = new Dictionary<string, RestApiResourceDetails>();
          this.m_resourceMappings.Add(ResourceMappingConstants.TestRun, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestRunProject
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestResult, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestResultProject
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestActionResult, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestActionResultsProject
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestIterationDetails, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestIterationsProject
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestResultAttachments, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestResultAttachmentsProject
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestResultParameters, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestResultParametersProject
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestRunAttachments, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestRunAttachmentsProject
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestResultAttachmentsV2, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
            Area = "Test",
            ResourceId = TestManagementResourceIds.TestResultAttachmentsProjectV2
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.TestRunLogStoreAttachments, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TCMServiceInstanceType,
            Area = "testresults",
            ResourceId = TestResultsResourceIds.TestRunLogStoreAttachmentsLocation
          });
          this.m_resourceMappings.Add(ResourceMappingConstants.CodeCoverage, new RestApiResourceDetails()
          {
            ServiceInstanceType = TestManagementServerConstants.TCMServiceInstanceType,
            Area = "tcm",
            ResourceId = TcmResourceIds.DownloadCodeCoverageProject
          });
        }
        return this.m_resourceMappings;
      }
    }

    public virtual Dictionary<string, Guid> JobMappings => this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? JobDefinitionData.TfsJobDefinitions : JobDefinitionData.TcmJobDefinitions;

    public virtual TCMLogger Logger { get; private set; }

    public bool IsFeatureEnabled(string featureName) => this.RequestContext.IsFeatureEnabled(featureName);

    private void UpdateCurrentUserIdentityDetails()
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.RequestContext.GetUserIdentity();
      if (userIdentity == null)
        return;
      this.m_userTeamFoundationId = userIdentity.Id;
      this.m_userTeamFoundationName = userIdentity.DisplayName;
      this.m_userDistinctTeamFoundationName = IdentityHelper.GetDistinctDisplayName(userIdentity);
    }
  }
}
