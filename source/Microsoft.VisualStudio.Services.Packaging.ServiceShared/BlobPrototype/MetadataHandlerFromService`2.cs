// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MetadataHandlerFromService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MetadataHandlerFromService<TPackageIdentity, TMetadataEntry> : 
    IAsyncHandler<IPackageRequest<TPackageIdentity>, TMetadataEntry>,
    IHaveInputType<IPackageRequest<TPackageIdentity>>,
    IHaveOutputType<TMetadataEntry>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService;

    public MetadataHandlerFromService(
      IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService)
    {
      this.metadataService = metadataService;
    }

    public Task<TMetadataEntry> Handle(IPackageRequest<TPackageIdentity> request) => this.metadataService.GetPackageVersionStateAsync(request);
  }
}
