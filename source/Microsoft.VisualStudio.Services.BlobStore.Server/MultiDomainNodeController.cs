// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainNodeController
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
  [VersionedApiControllerCustomName(Area = "domains", ResourceName = "nodes", ResourceVersion = 1)]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public class MultiDomainNodeController : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "DedupStore",
      Layer = nameof (MultiDomainNodeController)
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5708040)]
    public async Task<HttpResponseMessage> GetNodeAsync(
      string domainId,
      string dedupId,
      bool allowEdge = false,
      bool redirect = true)
    {
      MultiDomainNodeController domainNodeController = this;
      return await new MultiDomainNodeHandler(domainNodeController.TfsRequestContext, domainNodeController.Request, DomainIdFactory.Create(domainId)).GetNodeAsync(dedupId, allowEdge, redirect);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5708050)]
    public async Task<HttpResponseMessage> PutNodeAsync(
      string domainId,
      string dedupId,
      string keepUntil)
    {
      MultiDomainNodeController domainNodeController = this;
      return await new MultiDomainNodeHandler(domainNodeController.TfsRequestContext, domainNodeController.Request, DomainIdFactory.Create(domainId)).PutNodeAsync(dedupId, keepUntil);
    }

    [HttpPost]
    [ControllerMethodTraceFilter(5708060)]
    public async Task<HttpResponseMessage> TryReferenceAsync(
      string domainId,
      string dedupId,
      string keepUntil)
    {
      MultiDomainNodeController domainNodeController = this;
      return await new MultiDomainNodeHandler(domainNodeController.TfsRequestContext, domainNodeController.Request, DomainIdFactory.Create(domainId)).TryReferenceAsync(dedupId, keepUntil);
    }
  }
}
