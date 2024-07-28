// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUrls5Controller
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "urls", ResourceVersion = 1)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public sealed class DedupUrls5Controller : DedupUrlsController
  {
    [HttpGet]
    [ControllerMethodTraceFilter(5707120)]
    public async Task<HttpResponseMessage> GetDownloadInfoAsync(string dedupId, bool includeChunks = false)
    {
      DedupUrls5Controller dedupUrls5Controller = this;
      return await new MultiDomainDedupUrlsHandler(dedupUrls5Controller.TfsRequestContext, dedupUrls5Controller.Request, WellKnownDomainIds.DefaultDomainId).GetDownloadInfoAsync(dedupId, includeChunks);
    }
  }
}
