// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.NodeController
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
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "nodes", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public class NodeController : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "DedupStore",
      Layer = nameof (NodeController)
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5708010)]
    public async Task<HttpResponseMessage> GetNodeAsync(
      string dedupId,
      bool allowEdge = false,
      bool redirect = true)
    {
      NodeController nodeController = this;
      return await new MultiDomainNodeHandler(nodeController.TfsRequestContext, nodeController.Request, WellKnownDomainIds.DefaultDomainId).GetNodeAsync(dedupId, allowEdge, redirect);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5708020)]
    public async Task<HttpResponseMessage> PutNodeAsync(string dedupId, string keepUntil)
    {
      NodeController nodeController = this;
      return await new MultiDomainNodeHandler(nodeController.TfsRequestContext, nodeController.Request, WellKnownDomainIds.DefaultDomainId).PutNodeAsync(dedupId, keepUntil);
    }

    [HttpPost]
    [ControllerMethodTraceFilter(5708030)]
    public async Task<HttpResponseMessage> TryReferenceAsync(string dedupId, string keepUntil)
    {
      NodeController nodeController = this;
      return await new MultiDomainNodeHandler(nodeController.TfsRequestContext, nodeController.Request, WellKnownDomainIds.DefaultDomainId).TryReferenceAsync(dedupId, keepUntil);
    }
  }
}
