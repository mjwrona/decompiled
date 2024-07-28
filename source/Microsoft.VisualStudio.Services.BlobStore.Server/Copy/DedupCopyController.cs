// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Copy.DedupCopyController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Copy
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "copy", ResourceVersion = 1)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public class DedupCopyController : BlobControllerBase
  {
    private static readonly Guid PackagingInstanceType = new Guid("00000030-0000-8888-8000-000000000000");

    [HttpPost]
    [ControllerMethodTraceFilter(5702121)]
    public async Task<HttpResponseMessage> CopyDedupsAsync(string domainId)
    {
      DedupCopyController dedupCopyController = this;
      if (!ServicePrincipals.IsServicePrincipal(dedupCopyController.TfsRequestContext, dedupCopyController.TfsRequestContext.UserContext) && dedupCopyController.TfsRequestContext.GetUserIdentity().Id != DedupCopyController.PackagingInstanceType)
        throw new UnauthorizedBlobstoreRequestException(dedupCopyController.TfsRequestContext.GetUserIdentity().DisplayName);
      DedupCopyBatch copyBatch = JsonConvert.DeserializeObject<DedupCopyBatch>(await dedupCopyController.Request.Content.ReadAsStringAsync().ConfigureAwait(true));
      await dedupCopyController.Validate(copyBatch);
      XmlNode jobData = (XmlNode) JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject((object) new DedupContentCopyRequest()
      {
        CopyFromDomainId = copyBatch.SourceDomainId,
        CopyToDomainId = DomainIdFactory.Create(domainId),
        CopyFromHostId = copyBatch.SourceHostId,
        DedupIds = copyBatch.DedupIds
      }), "jobData", true);
      Guid guid = dedupCopyController.TfsRequestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(dedupCopyController.TfsRequestContext, "Dedup content copy job", "Microsoft.VisualStudio.Services.BlobStore.Server.Copy.DedupContentCopyJob", jobData, false);
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage()
      {
        Content = (HttpContent) new StringContent(guid.ToString()),
        StatusCode = HttpStatusCode.Accepted
      };
      copyBatch = (DedupCopyBatch) null;
      return httpResponseMessage;
    }

    private async Task Validate(DedupCopyBatch copyBatch)
    {
      DedupCopyController dedupCopyController = this;
      IVssRequestContext vssRequestContext = dedupCopyController.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, copyBatch.SourceHostId, Microsoft.VisualStudio.Services.Content.Common.ServiceInstanceTypes.BlobStore);
      if (hostUri == (Uri) null)
        throw new ServiceHostDoesNotExistException(copyBatch.SourceHostId);
      IList<DedupDownloadInfo> downloadInfoAsync = await new DomainHttpClientWrapper(copyBatch.SourceDomainId, (IDomainDedupStoreHttpClient) ((ICreateClient) dedupCopyController.TfsRequestContext.ClientProvider).CreateClient<DomainDedupStoreHttpClient>(dedupCopyController.TfsRequestContext, hostUri, "Empty", (ApiResourceLocationCollection) null)).GetBatchDownloadInfoAsync(copyBatch.DedupIds, false, dedupCopyController.TfsRequestContext.CancellationToken);
    }
  }
}
