// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmControllerBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Tcm
{
  public abstract class TcmControllerBase : TfsProjectApiController
  {
    private TestManagementRequestContext m_testManagementRequestContext;
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static TcmControllerBase()
    {
      TcmControllerBase.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      TcmControllerBase.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      TcmControllerBase.s_httpExceptions.Add(typeof (InvalidPropertyException), HttpStatusCode.Conflict);
      TcmControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      TcmControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectInUseException), HttpStatusCode.Forbidden);
      TcmControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException), HttpStatusCode.NotFound);
      TcmControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException), HttpStatusCode.NotFound);
      TcmControllerBase.s_httpExceptions.Add(typeof (ProjectDoesNotExistWithNameException), HttpStatusCode.NotFound);
      TcmControllerBase.s_httpExceptions.Add(typeof (CoverageSummaryStatusConflictException), (HttpStatusCode) 425);
      TcmControllerBase.s_httpExceptions.Add(typeof (UnsuccessfulQueueInvokerJobException), HttpStatusCode.InternalServerError);
    }

    public override string ActivityLogArea => "Test Results";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TcmControllerBase.s_httpExceptions;

    protected TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = new TestManagementRequestContext(this.TfsRequestContext);
        return this.m_testManagementRequestContext;
      }
    }

    protected string GetHeaderValue(HttpRequestHeaders requestHeaders, string headerName)
    {
      string headerValue = (string) null;
      IEnumerable<string> values;
      if (requestHeaders != null && requestHeaders.TryGetValues(headerName, out values))
        headerValue = values.FirstOrDefault<string>();
      return headerValue;
    }
  }
}
