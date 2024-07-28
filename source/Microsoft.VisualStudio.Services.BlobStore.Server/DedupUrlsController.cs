// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUrlsController
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
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "urls", ResourceVersion = 1)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public class DedupUrlsController : BlobControllerBase
  {
    private const int ThrottlingThresholdForGetUrlsBatchAsync = 87;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpPost]
    [ControllerMethodTraceFilter(5707090)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "GetUrlsBatchAsync", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> GetUrlsBatchAsync(bool allowEdge = false)
    {
      DedupUrlsController dedupUrlsController = this;
      return await new MultiDomainDedupUrlsHandler(dedupUrlsController.TfsRequestContext, dedupUrlsController.Request, WellKnownDomainIds.DefaultDomainId).GetUrlsBatchAsync(allowEdge);
    }
  }
}
