// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReferenceBatchController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "blob", ResourceName = "referencesbatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  public sealed class ReferenceBatchController : BlobControllerBase
  {
    private const long MaxRequestContentLengthDefault = 20971520;
    private const int ThrottlingThresholdForTryAddReferencesAsync = 87;
    private const int ThrottlingThresholdForDeleteReferencesAsync = 75;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpPost]
    [ControllerMethodTraceFilter(5706000)]
    [MaxContentLengthActionFilter("/Configuration/BlobStore", 20971520)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "TryAddReferences", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> TryAddReferencesAsync()
    {
      ReferenceBatchController referenceBatchController = this;
      return await new MultiDomainReferenceBatchHandler(referenceBatchController.TfsRequestContext, referenceBatchController.Request, WellKnownDomainIds.DefaultDomainId).TryAddReferencesAsync();
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(5706020)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "DeleteReferences", 75, 4096, 5, -1)]
    public async Task<HttpResponseMessage> DeleteReferencesAsync()
    {
      ReferenceBatchController referenceBatchController = this;
      return await new MultiDomainReferenceBatchHandler(referenceBatchController.TfsRequestContext, referenceBatchController.Request, WellKnownDomainIds.DefaultDomainId).DeleteReferencesAsync();
    }
  }
}
