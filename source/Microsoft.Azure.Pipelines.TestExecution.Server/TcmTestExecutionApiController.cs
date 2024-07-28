// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.Server.TcmTestExecutionApiController
// Assembly: Microsoft.Azure.Pipelines.TestExecution.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A35ABAC4-7A2F-41DF-9E6F-54457266EDD3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestExecution.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Pipelines.TestExecution.Server
{
  [ControllerApiVersion(0.1)]
  [VersionedApiControllerCustomName(Area = "TestExecution", ResourceName = "TestExecution")]
  [FeatureEnabled("TestExecutionService.RESTAPI")]
  public class TcmTestExecutionApiController : TfsProjectApiController
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
          this._testExecutionRequestContext = new TestExecutionRequestContext(this.TfsRequestContext);
        return this._testExecutionRequestContext;
      }
      set => this._testExecutionRequestContext = value;
    }

    public override string TraceArea => "TestExecution";

    public override string ActivityLogArea => "TestExecution";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TcmTestExecutionApiController.s_httpExceptions;
  }
}
