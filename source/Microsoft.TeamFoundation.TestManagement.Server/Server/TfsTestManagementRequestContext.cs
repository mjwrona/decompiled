// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsTestManagementRequestContext
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.Server.TcmService;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TfsTestManagementRequestContext : TestManagementRequestContext
  {
    protected ITeamFoundationCatalogService m_catalogService;
    protected PlannedResultsTCMServiceHelper m_plannedTestingTCMServiceHelper;
    protected ILegacyTCMServiceHelper m_legacyTCMServiceHelper;

    public TfsTestManagementRequestContext(IVssRequestContext requestContext)
      : base(requestContext)
    {
      this.m_testManagementHost = (TestManagementHost) requestContext.GetService<TfsTestManagementHost>();
    }

    public override ICommonStructureServiceHelper CSSHelper
    {
      get
      {
        if (this.m_cssHelper == null)
          this.m_cssHelper = (ICommonStructureServiceHelper) new CommonStructureServiceHelper(this.RequestContext);
        return this.m_cssHelper;
      }
    }

    public override ITcmServiceHelper TcmServiceHelper
    {
      get
      {
        if (this.m_tcmHelper == null)
          this.m_tcmHelper = (ITcmServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.TcmServiceHelper(this.RequestContext);
        return this.m_tcmHelper;
      }
    }

    public ILegacyTCMServiceHelper LegacyTcmServiceHelper
    {
      get
      {
        if (this.m_legacyTCMServiceHelper == null)
          this.m_legacyTCMServiceHelper = (ILegacyTCMServiceHelper) new LegacyTCMServiceHelper(this.RequestContext);
        return this.m_legacyTCMServiceHelper;
      }
    }

    public PlannedResultsTCMServiceHelper PlannedTestingTCMServiceHelper
    {
      get
      {
        if (this.m_plannedTestingTCMServiceHelper == null)
          this.m_plannedTestingTCMServiceHelper = new PlannedResultsTCMServiceHelper();
        return this.m_plannedTestingTCMServiceHelper;
      }
    }

    public override ITestPointOutcomeHelper TestPointOutcomeHelper
    {
      get
      {
        if (this.m_testPointOutcomeHelper == null)
          this.m_testPointOutcomeHelper = (ITestPointOutcomeHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestPointOutcomeHelper();
        return this.m_testPointOutcomeHelper;
      }
    }

    public override ITfsServiceHelper TfsServiceHelper
    {
      get
      {
        if (this.m_tfsHelper == null)
          this.m_tfsHelper = (ITfsServiceHelper) new DefaultTfsServiceHelper(this.RequestContext);
        return this.m_tfsHelper;
      }
    }

    public virtual ITeamFoundationCatalogService CatalogService
    {
      get
      {
        if (this.m_catalogService == null)
        {
          this.m_catalogService = this.m_requestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationCatalogService>();
          if (this.m_catalogService == null)
            throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CannotAccessTfsSubsystem, (object) typeof (ITeamFoundationCatalogService).Name));
        }
        return this.m_catalogService;
      }
    }

    public override IWorkItemFieldDataHelper WorkItemFieldDataHelper
    {
      get
      {
        if (this.m_workItemFieldDataHelper == null)
          this.m_workItemFieldDataHelper = (IWorkItemFieldDataHelper) new Microsoft.TeamFoundation.TestManagement.Server.WorkItemFieldDataHelper();
        return this.m_workItemFieldDataHelper;
      }
    }

    public override IPlannedTestResultsHelper PlannedTestResultsHelper
    {
      get
      {
        if (this.m_plannedTestResultsHelper == null)
          this.m_plannedTestResultsHelper = (IPlannedTestResultsHelper) new Microsoft.TeamFoundation.TestManagement.Server.PlannedTestResultsHelper((TestManagementRequestContext) this);
        return this.m_plannedTestResultsHelper;
      }
    }

    public override Dictionary<string, RestApiResourceDetails> ResourceMappings
    {
      get
      {
        Dictionary<string, RestApiResourceDetails> resourceMappings = base.ResourceMappings;
        resourceMappings[ResourceMappingConstants.CodeCoverage] = new RestApiResourceDetails()
        {
          ServiceInstanceType = TestManagementServerConstants.TFSServiceInstanceType,
          Area = "Test",
          ResourceId = Guid.Empty
        };
        return resourceMappings;
      }
    }

    public override bool IsTcmService => false;

    public override Dictionary<string, Guid> JobMappings => JobDefinitionData.TfsJobDefinitions;

    internal override IFeedbackHelper FeedbackHelper
    {
      get
      {
        if (this.m_feedbackHelper == null)
          this.m_feedbackHelper = (IFeedbackHelper) new Microsoft.TeamFoundation.TestManagement.Server.FeedbackHelper();
        return this.m_feedbackHelper;
      }
    }
  }
}
