// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupBlobControllers.DedupPartsController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupBlobControllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "parts", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  [FeatureEnabled("Blobstore.Features.ServerDedup")]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup.ServerSideDedup]
  [CpuThrottlingActionFilter("/Configuration/BlobStore", "DedupSessionsController", 87, -1, 5, -1)]
  public class DedupPartsController : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Controller"
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5704140)]
    public async Task<HttpResponseMessage> GetPartsAsync(string domainId, Guid sessionId)
    {
      DedupPartsController dedupPartsController = this;
      IEnumerable<Part> partsAsync = await dedupPartsController.TfsRequestContext.GetService<ISessionService>().GetPartsAsync(dedupPartsController.TfsRequestContext, DomainIdFactory.Create(domainId), sessionId);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        Content = JsonSerializer.SerializeToContent<IEnumerable<Part>>(partsAsync)
      };
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5704160)]
    public async Task<HttpResponseMessage> UploadPartAsync(string domainId, Guid sessionId)
    {
      DedupPartsController dedupPartsController = this;
      long offsetFrom;
      long offsetTo;
      long totalSize;
      long? nullable;
      if (dedupPartsController.Request.Content.Headers.ContentRange != null)
      {
        nullable = dedupPartsController.Request.Content.Headers.ContentRange.From;
        if (nullable.HasValue)
        {
          nullable = dedupPartsController.Request.Content.Headers.ContentRange.To;
          if (nullable.HasValue)
          {
            nullable = dedupPartsController.Request.Content.Headers.ContentRange.Length;
            if (nullable.HasValue)
            {
              nullable = dedupPartsController.Request.Content.Headers.ContentRange.From;
              offsetFrom = nullable.Value;
              nullable = dedupPartsController.Request.Content.Headers.ContentRange.To;
              offsetTo = nullable.Value;
              nullable = dedupPartsController.Request.Content.Headers.ContentRange.Length;
              totalSize = nullable.Value;
              if (offsetTo < offsetFrom)
                throw new InvalidPartsRangeException("Range-to should be equal or greater than range-from.");
              if (offsetTo >= totalSize)
                throw new InvalidPartsRangeException("Range-to should be less than total size.");
              if (offsetFrom < 0L)
                throw new InvalidPartsRangeException("Range-from should be equal or greater than 0.");
              if (totalSize < 0L)
                throw new InvalidPartsRangeException("Total size should be equal or greater than 0.");
              goto label_13;
            }
          }
        }
      }
      offsetFrom = 0L;
      nullable = dedupPartsController.Request.Content.Headers.ContentLength;
      offsetTo = nullable.Value - 1L;
      nullable = dedupPartsController.Request.Content.Headers.ContentLength;
      totalSize = nullable.Value;
label_13:
      HttpResponseMessage httpResponseMessage;
      using (Stream file = await dedupPartsController.Request.Content.ReadAsStreamAsync())
      {
        IServerDeduplicationService service = dedupPartsController.TfsRequestContext.GetService<IServerDeduplicationService>();
        ISessionService sessionService = dedupPartsController.TfsRequestContext.GetService<ISessionService>();
        IDomainId _domainId = DomainIdFactory.Create(domainId);
        IVssRequestContext tfsRequestContext = dedupPartsController.TfsRequestContext;
        IDomainId domainId1 = _domainId;
        Stream stream = file;
        CancellationToken cancellationToken = dedupPartsController.TfsRequestContext.CancellationToken;
        UploadResult uploadResult = await service.ChunkAndUploadAsync(tfsRequestContext, domainId1, stream, cancellationToken);
        Part part = new Part()
        {
          From = offsetFrom,
          To = offsetTo,
          TotalSize = totalSize,
          RootId = uploadResult.rootId
        };
        await sessionService.AddPartAsync(dedupPartsController.TfsRequestContext, _domainId, sessionId, part);
        httpResponseMessage = new HttpResponseMessage()
        {
          StatusCode = HttpStatusCode.Created
        };
      }
      return httpResponseMessage;
    }
  }
}
