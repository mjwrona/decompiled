// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server.TestPlanning;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestManagementController : TfsProjectApiController
  {
    private TfsTestManagementRequestContext m_testManagementRequestContext;
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();
    private SuitesHelper m_suitesHelper;
    private SuiteTestCaseHelper m_suiteTestCaseHelper;
    private TestCaseHelper m_testCaseHelper;
    private TestPlansHelper m_plansHelper;
    private RevisedTestPlansHelper m_revisedPlansHelper;
    private RevisedTestEntityCountHelper m_revisedTestEntityCountHelper;
    private RevisedPointsHelper m_revisedPointsHelper;
    private RevisedTestSuitesHelper m_revisedTestSuitesHelper;
    private ITeamFoundationTestManagementPointService m_testManagementPointService;

    static TestManagementController()
    {
      TestManagementController.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      TestManagementController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      TestManagementController.s_httpExceptions.Add(typeof (TestObjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementController.s_httpExceptions.Add(typeof (InvalidPropertyException), HttpStatusCode.BadRequest);
      TestManagementController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      TestManagementController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectInUseException), HttpStatusCode.Forbidden);
      TestManagementController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementController.s_httpExceptions.Add(typeof (ProjectDoesNotExistWithNameException), HttpStatusCode.NotFound);
      TestManagementController.s_httpExceptions.Add(typeof (ProjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementController.s_httpExceptions.Add(typeof (InvalidStructurePathException), HttpStatusCode.BadRequest);
      TestManagementController.s_httpExceptions.Add(typeof (TestManagementValidationException), HttpStatusCode.BadRequest);
      TestManagementController.s_httpExceptions.Add(typeof (MissingLicenseException), HttpStatusCode.Forbidden);
      TestManagementController.s_httpExceptions.Add(typeof (WorkItemTrackingFieldDefinitionNotFoundException), HttpStatusCode.BadRequest);
      TestManagementController.s_httpExceptions.Add(typeof (TestManagementInvalidOperationException), HttpStatusCode.BadRequest);
      TestManagementController.s_httpExceptions.Add(typeof (WorkItemUnauthorizedAccessException), HttpStatusCode.NotFound);
    }

    public override string ActivityLogArea => "Test Management REST";

    public static T InvokeAction<T>(Func<T> func)
    {
      try
      {
        return func();
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        throw;
      }
    }

    internal TestResultsHttpClient TestResultsHttpClient => this.TestManagementRequestContext.RequestContext.GetClient<TestResultsHttpClient>();

    internal TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = new TfsTestManagementRequestContext(this.TfsRequestContext);
        return (TestManagementRequestContext) this.m_testManagementRequestContext;
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TestManagementController.s_httpExceptions;

    internal TestPlansHelper PlansHelper
    {
      get
      {
        if (this.m_plansHelper == null)
          this.m_plansHelper = new TestPlansHelper(this.TestManagementRequestContext);
        return this.m_plansHelper;
      }
    }

    internal RevisedTestPlansHelper RevisedTestPlansHelper
    {
      get
      {
        if (this.m_revisedPlansHelper == null)
          this.m_revisedPlansHelper = new RevisedTestPlansHelper(this.TestManagementRequestContext);
        return this.m_revisedPlansHelper;
      }
    }

    internal TestCaseHelper TestCaseHelper
    {
      get
      {
        if (this.m_testCaseHelper == null)
          this.m_testCaseHelper = new TestCaseHelper(this.TestManagementRequestContext);
        return this.m_testCaseHelper;
      }
    }

    internal RevisedTestEntityCountHelper RevisedTestEntityCountHelper
    {
      get
      {
        if (this.m_revisedTestEntityCountHelper == null)
          this.m_revisedTestEntityCountHelper = new RevisedTestEntityCountHelper(this.TestManagementRequestContext);
        return this.m_revisedTestEntityCountHelper;
      }
    }

    internal RevisedPointsHelper RevisedPointsHelper
    {
      get
      {
        if (this.m_revisedPointsHelper == null)
          this.m_revisedPointsHelper = new RevisedPointsHelper(this.TestManagementRequestContext);
        return this.m_revisedPointsHelper;
      }
    }

    internal SuitesHelper SuitesHelper
    {
      get
      {
        if (this.m_suitesHelper == null)
          this.m_suitesHelper = new SuitesHelper(this.TestManagementRequestContext);
        return this.m_suitesHelper;
      }
    }

    internal RevisedTestSuitesHelper RevisedTestSuitesHelper
    {
      get
      {
        if (this.m_revisedTestSuitesHelper == null)
          this.m_revisedTestSuitesHelper = new RevisedTestSuitesHelper(this.TestManagementRequestContext);
        return this.m_revisedTestSuitesHelper;
      }
    }

    internal SuiteTestCaseHelper SuiteTestCaseHelper
    {
      get
      {
        if (this.m_suiteTestCaseHelper == null)
          this.m_suiteTestCaseHelper = new SuiteTestCaseHelper(this.TestManagementRequestContext);
        return this.m_suiteTestCaseHelper;
      }
    }

    internal ITeamFoundationTestManagementPointService TestManagementPointService
    {
      get
      {
        if (this.m_testManagementPointService == null)
          this.m_testManagementPointService = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementPointService>();
        return this.m_testManagementPointService;
      }
    }
  }
}
