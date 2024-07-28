// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadPackageFileAsResponseMessageHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadPackageFileAsResponseMessageHandler<TPackageId> : 
    IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, HttpResponseMessage>,
    IHaveInputType<IPackageFileRequest<TPackageId, IStorageId>>,
    IHaveOutputType<HttpResponseMessage>
    where TPackageId : IPackageIdentity
  {
    private readonly IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult> ingestingHandler;

    public DownloadPackageFileAsResponseMessageHandler(
      IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult> ingestingHandler)
    {
      this.ingestingHandler = ingestingHandler;
    }

    public async Task<HttpResponseMessage> Handle(
      IPackageFileRequest<TPackageId, IStorageId> request)
    {
      ContentResult data = await this.ingestingHandler.Handle(request);
      return data == null ? (HttpResponseMessage) null : await new DownloadPackageFileFromContentResultAsResponseMessageHandler<TPackageId>().Handle(request.WithData<TPackageId, ContentResult>(data));
    }
  }
}
