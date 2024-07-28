// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainRootHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MultiDomainRootHandler
  {
    private IVssRequestContext requestContext;
    private HttpRequestMessage requestMessage;
    private IDomainId domainId;

    public MultiDomainRootHandler(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      IDomainId domainId)
    {
      this.requestContext = requestContext;
      this.requestMessage = requestMessage;
      this.domainId = domainId;
    }

    public async Task<HttpResponseMessage> AddRootAsync(string dedupId, string name, string scope)
    {
      if (!this.requestContext.AllowMultiDomainOperations(this.domainId))
        throw new FeatureDisabledException("Multi-Domain");
      ArgumentUtility.CheckForNull<string>(dedupId, nameof (dedupId));
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      ArgumentUtility.CheckForNull<string>(scope, nameof (scope));
      IDedupStore service = this.requestContext.GetService<IDedupStore>();
      DedupIdentifier dedupIdentifier = DedupIdentifier.Create(dedupId);
      IVssRequestContext requestContext = this.requestContext;
      IDomainId domainId = this.domainId;
      DedupIdentifier dedupId1 = dedupIdentifier;
      IdBlobReference rootRef = new IdBlobReference(name, scope);
      await service.PutRootAsync(requestContext, domainId, dedupId1, rootRef).ConfigureAwait(true);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK
      };
    }

    public async Task<HttpResponseMessage> DeleteRootAsync(
      string dedupId,
      string name,
      string scope)
    {
      if (!this.requestContext.AllowMultiDomainOperations(this.domainId))
        throw new FeatureDisabledException("Multi-Domain");
      ArgumentUtility.CheckForNull<string>(dedupId, nameof (dedupId));
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      ArgumentUtility.CheckForNull<string>(scope, nameof (scope));
      IDedupStore service = this.requestContext.GetService<IDedupStore>();
      DedupIdentifier dedupIdentifier = DedupIdentifier.Create(dedupId);
      IVssRequestContext requestContext = this.requestContext;
      IDomainId domainId = this.domainId;
      DedupIdentifier dedupId1 = dedupIdentifier;
      IdBlobReference rootRef = new IdBlobReference(name, scope);
      await service.DeleteRootAsync(requestContext, domainId, dedupId1, rootRef).ConfigureAwait(true);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK
      };
    }
  }
}
