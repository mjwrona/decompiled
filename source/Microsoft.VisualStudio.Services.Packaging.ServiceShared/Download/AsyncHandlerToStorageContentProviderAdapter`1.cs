// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.AsyncHandlerToStorageContentProviderAdapter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class AsyncHandlerToStorageContentProviderAdapter<TPackageId> : 
    ISpecificStorageContentProvider
    where TPackageId : class, IPackageIdentity
  {
    private readonly IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult?> handler;

    public AsyncHandlerToStorageContentProviderAdapter(
      IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult?> handler)
    {
      this.handler = handler;
    }

    public Task<ContentResult?> GetContentOrDefault(
      IPackageFileRequest request,
      IStorageId storageId)
    {
      return this.handler.Handle(request.WithPackage<TPackageId>((TPackageId) request.PackageId).WithFile<TPackageId>(request.FilePath).WithData<TPackageId, IStorageId>(storageId));
    }
  }
}
