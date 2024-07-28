// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainRootController
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
  [VersionedApiControllerCustomName(Area = "domains", ResourceName = "roots", ResourceVersion = 1)]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [RequestContentTypeRestriction(AllowStream = true)]
  [ClientIgnore]
  public sealed class MultiDomainRootController : BlobControllerBase
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpPut]
    [ControllerMethodTraceFilter(5707140)]
    public async Task<HttpResponseMessage> AddRootAsync(
      string domainId,
      string dedupId,
      string name,
      string scope)
    {
      MultiDomainRootController domainRootController = this;
      return await new MultiDomainRootHandler(domainRootController.TfsRequestContext, domainRootController.Request, DomainIdFactory.Create(domainId)).AddRootAsync(dedupId, name, scope);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(5707150)]
    public async Task<HttpResponseMessage> DeleteRootAsync(
      string domainId,
      string dedupId,
      string name,
      string scope)
    {
      MultiDomainRootController domainRootController = this;
      return await new MultiDomainRootHandler(domainRootController.TfsRequestContext, domainRootController.Request, DomainIdFactory.Create(domainId)).DeleteRootAsync(dedupId, name, scope);
    }
  }
}
