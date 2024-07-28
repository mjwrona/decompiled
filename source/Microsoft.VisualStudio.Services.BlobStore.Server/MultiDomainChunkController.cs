// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainChunkController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "domains", ResourceName = "chunks", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public sealed class MultiDomainChunkController : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "DedupStore",
      Layer = nameof (MultiDomainChunkController)
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5707050)]
    public async Task<HttpResponseMessage> GetChunkAsync(
      string domainId,
      string dedupId,
      bool allowEdge = false,
      bool redirect = true)
    {
      MultiDomainChunkController domainChunkController = this;
      return await new MultiDomainChunkHandler(domainChunkController.TfsRequestContext, domainChunkController.Request, DomainIdFactory.Create(domainId)).GetChunkAsync(dedupId, allowEdge, redirect);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5707060)]
    public async Task<HttpResponseMessage> PutChunkAsync(
      string domainId,
      string dedupId,
      string keepUntil)
    {
      MultiDomainChunkController domainChunkController = this;
      return await new MultiDomainChunkHandler(domainChunkController.TfsRequestContext, domainChunkController.Request, DomainIdFactory.Create(domainId)).PutChunkAsync(dedupId, keepUntil);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5707070)]
    public async Task<HttpResponseMessage> PutChunksAsync(string domainId, string keepUntil)
    {
      MultiDomainChunkController domainChunkController = this;
      return await new MultiDomainChunkHandler(domainChunkController.TfsRequestContext, domainChunkController.Request, DomainIdFactory.Create(domainId)).PutChunksAsync(keepUntil);
    }

    [HttpPost]
    [ControllerMethodTraceFilter(5707080)]
    public async Task<HttpResponseMessage> TryReferenceAsync(
      string domainId,
      string dedupId,
      string keepUntil)
    {
      MultiDomainChunkController domainChunkController = this;
      return await new MultiDomainChunkHandler(domainChunkController.TfsRequestContext, domainChunkController.Request, DomainIdFactory.Create(domainId)).TryReferenceAsync(dedupId, keepUntil);
    }
  }
}
