// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PackageFilteringUpstreamMetadataServiceDecorator`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class PackageFilteringUpstreamMetadataServiceDecorator<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> : 
    IUpstreamMetadataService<
    #nullable disable
    TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
    where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> upstreamMetadataService;
    private readonly IConverter<TPackageIdentity, Exception> packageIdentityToExceptionConverter;
    private readonly IConverter<(TPackageName, TPackageVersion), TPackageIdentity> nameAndVersionToPackageIdentityConverter;

    public PackageFilteringUpstreamMetadataServiceDecorator(
      IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> upstreamMetadataService,
      IConverter<TPackageIdentity, Exception> packageIdentityToExceptionConverter,
      IConverter<(TPackageName, TPackageVersion), TPackageIdentity> nameAndVersionToPackageIdentityConverter)
    {
      this.upstreamMetadataService = upstreamMetadataService;
      this.packageIdentityToExceptionConverter = packageIdentityToExceptionConverter;
      this.nameAndVersionToPackageIdentityConverter = nameAndVersionToPackageIdentityConverter;
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<TPackageVersion>>> GetPackageVersionsAsync(
      IFeedRequest downstreamFeedRequest,
      TPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      return (IReadOnlyList<VersionWithSourceChain<TPackageVersion>>) (await this.upstreamMetadataService.GetPackageVersionsAsync(downstreamFeedRequest, packageName)).Where<VersionWithSourceChain<TPackageVersion>>((Func<VersionWithSourceChain<TPackageVersion>, bool>) (version => this.\u003C\u003E4__this.IsPackageIdentityValid(packageName, version.Version))).ToList<VersionWithSourceChain<TPackageVersion>>();
    }

    public async Task<IEnumerable<TMetadataEntry>> GetPackageVersionStatesAsync(
      IFeedRequest downstreamFeedRequest,
      TPackageName name,
      IEnumerable<TPackageVersion> versions)
    {
      // ISSUE: reference to a compiler-generated field
      List<TPackageVersion> list = versions.Where<TPackageVersion>((Func<TPackageVersion, bool>) (version => this.\u003C\u003E4__this.IsPackageIdentityValid(name, version))).ToList<TPackageVersion>();
      // ISSUE: reference to a compiler-generated field
      return (await this.upstreamMetadataService.GetPackageVersionStatesAsync(downstreamFeedRequest, name, (IEnumerable<TPackageVersion>) list)).Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (entry => this.\u003C\u003E4__this.IsPackageIdentityValid(entry.PackageIdentity)));
    }

    public async Task<IEnumerable<TMetadataEntry>> UpdateEntriesWithTransientStateAsync(
      IFeedRequest downstreamFeedRequest,
      TPackageName packageName,
      IEnumerable<TMetadataEntry> entries,
      ICommitLogEntryHeader fakeCommitHeader)
    {
      PackageFilteringUpstreamMetadataServiceDecorator<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> serviceDecorator = this;
      // ISSUE: reference to a compiler-generated method
      List<TMetadataEntry> list = entries.Where<TMetadataEntry>(new Func<TMetadataEntry, bool>(serviceDecorator.\u003CUpdateEntriesWithTransientStateAsync\u003Eb__6_0)).ToList<TMetadataEntry>();
      // ISSUE: reference to a compiler-generated method
      return (await serviceDecorator.upstreamMetadataService.UpdateEntriesWithTransientStateAsync(downstreamFeedRequest, packageName, (IEnumerable<TMetadataEntry>) list, fakeCommitHeader)).Where<TMetadataEntry>(new Func<TMetadataEntry, bool>(serviceDecorator.\u003CUpdateEntriesWithTransientStateAsync\u003Eb__6_1));
    }

    public async Task<object> GetPackageNameMetadata(
      IFeedRequest downstreamFeedRequest,
      TPackageName name)
    {
      return await this.upstreamMetadataService.GetPackageNameMetadata(downstreamFeedRequest, name);
    }

    private bool IsPackageIdentityValid(TPackageName name, TPackageVersion version) => this.IsPackageIdentityValid(this.nameAndVersionToPackageIdentityConverter.Convert((name, version)));

    protected bool IsPackageIdentityValid(TPackageIdentity identity) => this.packageIdentityToExceptionConverter.Convert(identity) == null;
  }
}
