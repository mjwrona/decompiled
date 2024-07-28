// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultsControllerBase
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public abstract class TestResultsControllerBase : TfsProjectApiController
  {
    private TfsTestManagementRequestContext m_testManagementRequestContext;
    private PlannedTestResultsHelper m_plannedTestResultsHelper;
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static TestResultsControllerBase()
    {
      TestResultsControllerBase.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (TestObjectNotFoundException), HttpStatusCode.NotFound);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (InvalidPropertyException), HttpStatusCode.BadRequest);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectInUseException), HttpStatusCode.Forbidden);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException), HttpStatusCode.NotFound);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException), HttpStatusCode.NotFound);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (ProjectDoesNotExistWithNameException), HttpStatusCode.NotFound);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (ProjectNotFoundException), HttpStatusCode.NotFound);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (InvalidStructurePathException), HttpStatusCode.BadRequest);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (TestManagementValidationException), HttpStatusCode.BadRequest);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (MissingLicenseException), HttpStatusCode.Forbidden);
      TestResultsControllerBase.s_httpExceptions.Add(typeof (WorkItemTrackingFieldDefinitionNotFoundException), HttpStatusCode.BadRequest);
    }

    public override string ActivityLogArea => "Test Results";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TestResultsControllerBase.s_httpExceptions;

    protected TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = new TfsTestManagementRequestContext(this.TfsRequestContext);
        return (TestManagementRequestContext) this.m_testManagementRequestContext;
      }
    }

    internal TestResultsHttpClient TestResultsHttpClient => this.TestManagementRequestContext.RequestContext.GetClient<TestResultsHttpClient>();

    internal PlannedTestResultsHelper PlannedTestResultsHelper
    {
      get
      {
        if (this.m_plannedTestResultsHelper == null)
          this.m_plannedTestResultsHelper = new PlannedTestResultsHelper(this.TestManagementRequestContext);
        return this.m_plannedTestResultsHelper;
      }
    }

    protected ITeamFoundationTestManagementResultService ResultService => this.TfsRequestContext.GetService<ITeamFoundationTestManagementResultService>();
  }
}
