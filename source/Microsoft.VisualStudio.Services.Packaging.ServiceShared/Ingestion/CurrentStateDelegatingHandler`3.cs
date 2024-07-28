// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CurrentStateDelegatingHandler`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class CurrentStateDelegatingHandler<TPackageId, TEntry, TRequest> : 
    IAsyncHandler<TRequest>,
    IAsyncHandler<TRequest, NullResult>,
    IHaveInputType<TRequest>,
    IHaveOutputType<NullResult>
    where TPackageId : IPackageIdentity
    where TEntry : IMetadataEntry<TPackageId>, IPackageFiles
    where TRequest : IPackageRequest<TPackageId>
  {
    private readonly IAsyncHandler<PackageRequest<TPackageId>, TEntry> metadataFetchingHandler;
    private readonly IAsyncHandler<(TRequest, TEntry), NullResult>[] handlers;

    public CurrentStateDelegatingHandler(
      IAsyncHandler<PackageRequest<TPackageId>, TEntry> metadataFetchingHandler,
      params IAsyncHandler<(TRequest, TEntry), NullResult>[] handlers)
    {
      this.metadataFetchingHandler = metadataFetchingHandler;
      this.handlers = handlers;
    }

    public async Task<NullResult> Handle(TRequest request)
    {
      TEntry entry = await this.metadataFetchingHandler.Handle(new PackageRequest<TPackageId>(request.Feed, request.PackageId));
      IAsyncHandler<(TRequest, TEntry), NullResult>[] asyncHandlerArray = this.handlers;
      for (int index = 0; index < asyncHandlerArray.Length; ++index)
      {
        NullResult nullResult = await asyncHandlerArray[index].Handle((request, entry));
      }
      asyncHandlerArray = (IAsyncHandler<(TRequest, TEntry), NullResult>[]) null;
      NullResult nullResult1 = (NullResult) null;
      entry = default (TEntry);
      return nullResult1;
    }
  }
}
