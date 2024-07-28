// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadPackageFileFromContentResultAsResponseMessageHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadPackageFileFromContentResultAsResponseMessageHandler<TPackageId> : 
    IAsyncHandler<IPackageFileRequest<TPackageId, ContentResult>, HttpResponseMessage>,
    IHaveInputType<IPackageFileRequest<TPackageId, ContentResult>>,
    IHaveOutputType<HttpResponseMessage>
    where TPackageId : IPackageIdentity
  {
    public Task<HttpResponseMessage> Handle(
      IPackageFileRequest<TPackageId, ContentResult> request)
    {
      ContentResult additionalData = request.AdditionalData;
      if (additionalData == null)
        return (Task<HttpResponseMessage>) null;
      if (additionalData.RedirectToUri != (Uri) null)
        return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.SeeOther)
        {
          Headers = {
            Location = additionalData.RedirectToUri
          }
        });
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(request.Feed);
      VssServerStreamContent serverStreamContent = new VssServerStreamContent(additionalData.Stream, (object) securedObjectReadOnly);
      serverStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      serverStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = additionalData.FileName.Replace('/', '_').Replace('\\', '_')
      };
      return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) serverStreamContent
      });
    }
  }
}
