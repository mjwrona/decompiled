// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupBlobControllers.DedupSessionsController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupBlobControllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "sessions", ResourceVersion = 1)]
  [SetActivityLogAnonymousIdentifier]
  [FeatureEnabled("Blobstore.Features.ServerDedup")]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup.ServerSideDedup]
  [CpuThrottlingActionFilter("/Configuration/BlobStore", "DedupSessionsController", 87, -1, 5, -1)]
  public class DedupSessionsController : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Controller"
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpPost]
    [ControllerMethodTraceFilter(5704100)]
    public async Task<HttpResponseMessage> CreateSessionAsync(string domainId)
    {
      DedupSessionsController sessionsController = this;
      Guid sessionAsync = await sessionsController.TfsRequestContext.GetService<ISessionService>().CreateSessionAsync(sessionsController.TfsRequestContext, DomainIdFactory.Create(domainId));
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.Created,
        Content = (HttpContent) new StringContent(sessionAsync.ToString())
      };
    }

    [HttpGet]
    [ControllerMethodTraceFilter(5704110)]
    public async Task<HttpResponseMessage> GetSessionAsync(string domainId, Guid sessionId)
    {
      DedupSessionsController sessionsController = this;
      Session sessionAsync = await sessionsController.TfsRequestContext.GetService<ISessionService>().GetSessionAsync(sessionsController.TfsRequestContext, DomainIdFactory.Create(domainId), sessionId);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        Content = JsonSerializer.SerializeToContent<Session>(sessionAsync)
      };
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(5704120)]
    public async Task<HttpResponseMessage> DeleteSessionAsync(string domainId, Guid sessionId)
    {
      DedupSessionsController sessionsController = this;
      await sessionsController.TfsRequestContext.GetService<ISessionService>().AbandonSessionAsync(sessionsController.TfsRequestContext, DomainIdFactory.Create(domainId), sessionId);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.NoContent
      };
    }

    [HttpPatch]
    [ControllerMethodTraceFilter(5704130)]
    public async Task<HttpResponseMessage> CompleteSessionAsync(string domainId, Guid sessionId)
    {
      DedupSessionsController sessionsController = this;
      Session session = await sessionsController.TfsRequestContext.GetService<ISessionService>().CompleteSessionAsync(sessionsController.TfsRequestContext, DomainIdFactory.Create(domainId), sessionId);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        Content = (HttpContent) new StringContent(session.ContentId.ToString())
      };
    }
  }
}
