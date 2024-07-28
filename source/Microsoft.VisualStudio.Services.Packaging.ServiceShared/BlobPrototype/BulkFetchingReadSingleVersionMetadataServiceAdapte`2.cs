// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.BulkFetchingReadSingleVersionMetadataServiceAdapter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class BulkFetchingReadSingleVersionMetadataServiceAdapter<TPackageIdentity, TMetadataEntry> : 
    IReadSingleVersionMetadataService<
    #nullable disable
    TPackageIdentity, TMetadataEntry>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService;
    private IComparer<IPackageVersion> reverseVersionComparer;
    private volatile BulkFetchingReadSingleVersionMetadataServiceAdapter<TPackageIdentity, TMetadataEntry>.State state;

    public BulkFetchingReadSingleVersionMetadataServiceAdapter(
      IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService,
      IComparer<IPackageVersion> reverseVersionComparer)
    {
      this.metadataService = metadataService;
      this.reverseVersionComparer = reverseVersionComparer;
    }

    public async Task<TMetadataEntry> GetPackageVersionStateAsync(
      IPackageRequest<TPackageIdentity> request)
    {
      BulkFetchingReadSingleVersionMetadataServiceAdapter<TPackageIdentity, TMetadataEntry>.State state = this.state;
      TPackageIdentity packageId;
      if (!(state == (BulkFetchingReadSingleVersionMetadataServiceAdapter<TPackageIdentity, TMetadataEntry>.State) null))
      {
        PackageNameComparer normalizedName = PackageNameComparer.NormalizedName;
        IPackageName packageName = state.PackageName;
        packageId = request.PackageId;
        IPackageName name = packageId.Name;
        if (normalizedName.Equals(packageName, name))
          goto label_4;
      }
      packageId = request.PackageId;
      IPackageName PackageName = packageId.Name;
      IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService = this.metadataService;
      IPackageRequest<TPackageIdentity> feedRequest = request;
      packageId = request.PackageId;
      IPackageName name1 = packageId.Name;
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest = new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) feedRequest.WithPackageName<IPackageName>(name1));
      state = new BulkFetchingReadSingleVersionMetadataServiceAdapter<TPackageIdentity, TMetadataEntry>.State(PackageName, (IReadOnlyList<TMetadataEntry>) await metadataService.GetPackageVersionStatesAsync(packageNameQueryRequest));
      PackageName = (IPackageName) null;
      Interlocked.MemoryBarrier();
      this.state = state;
label_4:
      packageId = request.PackageId;
      return PackagingUtils.BinarySearch<IPackageVersion, TMetadataEntry>(packageId.Version, state.Entries, (Func<TMetadataEntry, IPackageVersion>) (metadataEntry => metadataEntry.PackageIdentity.Version), this.reverseVersionComparer, out int _);
    }

    private record State(IPackageName PackageName, IReadOnlyList<TMetadataEntry> Entries);
  }
}
