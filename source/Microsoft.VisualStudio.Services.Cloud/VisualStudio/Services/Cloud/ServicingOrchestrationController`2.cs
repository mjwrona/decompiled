// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServicingOrchestrationController`2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class ServicingOrchestrationController<TRequest, TJobManager> : TfsApiController
    where TRequest : FrameworkServicingOrchestrationRequest
    where TJobManager : ServicingOrchestrationJobManager<TRequest>, new()
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ServicingOrchestrationInvalidRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ServicingOrchestrationFinalizedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (ServicingOrchestrationEntryDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ServicingOrchestrationEntryAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (ServicingOrchestrationRequestOutOfOrderException),
        HttpStatusCode.Conflict
      },
      {
        typeof (ServicingOrchestrationRequestInProgressException),
        HttpStatusCode.Conflict
      }
    };

    [HttpPost]
    public void ValidateRequest(TRequest request)
    {
      this.ValidateRequest(this.TfsRequestContext, request);
      this.TfsRequestContext.GetService<ServicingOrchestrationService<TRequest, TJobManager>>().ValidateRequest(this.TfsRequestContext, request);
    }

    [HttpPut]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage QueueRequest(TRequest request)
    {
      this.ValidateRequest(this.TfsRequestContext, request);
      this.TfsRequestContext.GetService<ServicingOrchestrationService<TRequest, TJobManager>>().QueueRequest(this.TfsRequestContext, request);
      return this.Request.CreateResponse(HttpStatusCode.Accepted);
    }

    [HttpGet]
    [ClientResponseType(typeof (ServicingOrchestrationRequestStatus), null, null)]
    public HttpResponseMessage GetRequestStatus(Guid requestId)
    {
      ServicingOrchestrationRequestStatus requestStatus = this.TfsRequestContext.GetService<ServicingOrchestrationService<TRequest, TJobManager>>().GetRequestStatus(this.TfsRequestContext, requestId);
      return requestStatus == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<ServicingOrchestrationRequestStatus>(HttpStatusCode.OK, requestStatus);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage CancelRequest(Guid requestId)
    {
      this.TfsRequestContext.GetService<ServicingOrchestrationService<TRequest, TJobManager>>().CancelRequest(this.TfsRequestContext, requestId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    public override string TraceArea => "ServicingOrchestration";

    public override string ActivityLogArea => "Framework";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ServicingOrchestrationController<TRequest, TJobManager>.s_httpExceptions;

    protected virtual void ValidateRequest(IVssRequestContext requestContext, TRequest request)
    {
    }
  }
}
