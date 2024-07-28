// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestExecutionApiController
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(0.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "TestExecution")]
  [FeatureEnabled("TestExecutionService.RESTAPI")]
  public class TestExecutionApiController : TfsApiController
  {
    protected const int TestExecutionTracePointStart = 1015700;
    protected const int TestExecutionTracePointEnd = 1015999;
    protected static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (TestExecutionServiceInvalidOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TestExecutionObjectAlreadyExistsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TestExecutionObjectNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (TestEnvironmentAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      }
    };
    private ITestExecutionService _testExecutionService;
    private TestExecutionRequestContext _testExecutionRequestContext;
    private const string _testExecutionArea = "TestExecution";

    public ITestExecutionService TestExecutionService
    {
      get => this._testExecutionService ?? (this._testExecutionService = this.TfsRequestContext.GetService<ITestExecutionService>());
      set => this._testExecutionService = value;
    }

    public TestExecutionRequestContext TestExecutionRequestContext
    {
      get
      {
        if (this._testExecutionRequestContext == null)
          this._testExecutionRequestContext = (TestExecutionRequestContext) new TfsTestExecutionRequestContext(this.TfsRequestContext);
        return this._testExecutionRequestContext;
      }
      set => this._testExecutionRequestContext = value;
    }

    public override string TraceArea => "TestExecution";

    public override string ActivityLogArea => "TestExecution";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TestExecutionApiController.s_httpExceptions;
  }
}
