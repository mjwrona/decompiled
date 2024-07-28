// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RememberLastDocumentMetadataHandlerFromService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RememberLastDocumentMetadataHandlerFromService<TPackageIdentity, TMetadataEntry> : 
    IAsyncHandler<
    #nullable disable
    IPackageRequest<TPackageIdentity>, TMetadataEntry>,
    IHaveInputType<IPackageRequest<TPackageIdentity>>,
    IHaveOutputType<TMetadataEntry>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService;
    private RememberLastDocumentMetadataHandlerFromService<TPackageIdentity, TMetadataEntry>.State state;

    public RememberLastDocumentMetadataHandlerFromService(
      IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService)
    {
      this.metadataService = metadataService;
    }

    public async Task<TMetadataEntry> Handle(IPackageRequest<TPackageIdentity> request)
    {
      RememberLastDocumentMetadataHandlerFromService<TPackageIdentity, TMetadataEntry>.State state = this.state;
      if (state == null || !PackageNameComparer.NormalizedName.Equals(state.PackageName, request.PackageId.Name))
      {
        IPackageName packageName = request.PackageId.Name;
        state = new RememberLastDocumentMetadataHandlerFromService<TPackageIdentity, TMetadataEntry>.State(packageName, await this.metadataService.GetPackageVersionStatesAsync(new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) request.WithPackageName<IPackageName>(request.PackageId.Name))));
        packageName = (IPackageName) null;
        Interlocked.MemoryBarrier();
        this.state = state;
      }
      return state.Entries.FirstOrDefault<TMetadataEntry>((Func<TMetadataEntry, bool>) (x =>
      {
        PackageVersionComparer normalizedVersion = PackageVersionComparer.NormalizedVersion;
        TPackageIdentity packageIdentity = x.PackageIdentity;
        IPackageVersion version1 = packageIdentity.Version;
        packageIdentity = request.PackageId;
        IPackageVersion version2 = packageIdentity.Version;
        return normalizedVersion.Equals(version1, version2);
      }));
    }

    private class State
    {
      public State(IPackageName packageName, List<TMetadataEntry> entries)
      {
        this.PackageName = packageName;
        this.Entries = entries;
      }

      public IPackageName PackageName { get; }

      public List<TMetadataEntry> Entries { get; }
    }
  }
}
