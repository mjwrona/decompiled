// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher.IContentStitcherService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher
{
  [DefaultServiceImplementation(typeof (ContentStitcherService))]
  public interface IContentStitcherService : IVssFrameworkService
  {
    Task<HttpResponseMessage> GetStitchedFileAsync(
      IDomainId domainId,
      DedupIdentifier nodeId,
      IVssRequestContext requestContext,
      string fileName = null,
      bool useGzipCompression = false);

    Task<HttpResponseMessage> GetSignedUriAsync(
      IDomainId domainId,
      DedupIdentifier nodeId,
      IVssRequestContext requestContext,
      string fileName = null);

    Task<HttpResponseMessage> GetZipAsync(
      IDomainId domainId,
      ZippedContentRequest zippedContentRequest,
      IVssRequestContext requestContext,
      string fileName = null);
  }
}
