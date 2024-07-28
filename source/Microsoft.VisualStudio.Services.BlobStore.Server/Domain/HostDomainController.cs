// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Domain.HostDomainController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Domain
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "domains", ResourceName = "hostdomain", ResourceVersion = 1)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public class HostDomainController : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "Domain",
      Layer = nameof (HostDomainController)
    };
    private const int DomainTracePoint = 5709090;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) HostDomainExceptionMapping.ServerErrorMap;

    [HttpGet]
    [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
    [ControllerMethodTraceFilter(5709090)]
    public async Task<HttpResponseMessage> GetDomainAsync(string domainId)
    {
      HostDomainController domainController = this;
      IHostDomainStoreService service = domainController.TfsRequestContext.GetService<IHostDomainStoreService>();
      IDomainId idomainId = DomainIdFactory.Create(domainId);
      IVssRequestContext tfsRequestContext = domainController.TfsRequestContext;
      IDomainId domainId1 = idomainId;
      return new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<IMultiDomainInfo>(await service.GetDomainForOrganizationAsync(tfsRequestContext, domainId1) ?? throw new DomainNotFoundException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.DomainDoesNotExistsError((object) domainId))),
        StatusCode = HttpStatusCode.OK
      };
    }

    public async Task<HttpResponseMessage> GetDomainsAsync()
    {
      HostDomainController domainController = this;
      IEnumerable<IMultiDomainInfo> dataContractObject = await domainController.TfsRequestContext.GetService<IHostDomainStoreService>().GetDomainsForOrganizationAsync(domainController.TfsRequestContext).ConfigureAwait(true);
      return new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<IEnumerable<IMultiDomainInfo>>(dataContractObject),
        StatusCode = HttpStatusCode.OK
      };
    }

    [HttpPost]
    [FeatureEnabled("Blobstore.Features.ProjectDomains")]
    [ControllerMethodTraceFilter(5709090)]
    public async Task<IMultiDomainInfo> CreateProjectDomainsAsync(
      [FromBody] ProjectDomainRequest projectDomainRequest)
    {
      HostDomainController domainController = this;
      IHostDomainStoreService service = domainController.TfsRequestContext.GetService<IHostDomainStoreService>();
      Guid result1;
      if (!Guid.TryParse(projectDomainRequest.ProjectId, out result1))
        result1 = (domainController.TfsRequestContext.GetService<IProjectService>().GetProject(domainController.TfsRequestContext, projectDomainRequest.ProjectId) ?? throw new InvalidProjectDomainIdException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.InvalidProjectIdError((object) projectDomainRequest.ProjectId))).Id;
      byte result2;
      if (!ByteDomainId.TryParse(projectDomainRequest.DomainId, out result2, out string _, out bool _))
        throw new InvalidDomainIdException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.InvalidDomainIdError((object) projectDomainRequest.DomainId));
      IVssRequestContext tfsRequestContext = domainController.TfsRequestContext;
      Guid projectId = result1;
      ByteDomainId physicalDomainId = new ByteDomainId(result2);
      return await service.CreateProjectDomainsForAdminAsync(tfsRequestContext, projectId, physicalDomainId, false, false);
    }
  }
}
