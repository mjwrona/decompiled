// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion.NpmRevisionAndVersionsProviderHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion
{
  public class NpmRevisionAndVersionsProviderHandler : 
    IAsyncHandler<
    #nullable disable
    PackageNameRequest<NpmPackageName>, RevisionAndVersions>,
    IHaveInputType<PackageNameRequest<NpmPackageName>>,
    IHaveOutputType<RevisionAndVersions>
  {
    private readonly IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService;

    public NpmRevisionAndVersionsProviderHandler(
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService)
    {
      this.metadataService = metadataService;
    }

    public async Task<RevisionAndVersions> Handle(PackageNameRequest<NpmPackageName> request)
    {
      QueryOptions<INpmMetadataEntry> queryOptions = new QueryOptions<INpmMetadataEntry>().OnlyProjecting((Expression<Func<INpmMetadataEntry, object>>) (e => (object) e.DeletedDate)).OnlyProjecting((Expression<Func<INpmMetadataEntry, object>>) (e => (object) e.PermanentDeletedDate)).OnlyProjecting((Expression<Func<INpmMetadataEntry, object>>) (e => e.PackageStorageId)).WithFilter((Func<INpmMetadataEntry, bool>) (e => !e.IsDeleted()));
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService = this.metadataService;
      PackageNameQuery<INpmMetadataEntry> packageNameRequest = new PackageNameQuery<INpmMetadataEntry>((IPackageNameRequest) request);
      packageNameRequest.Options = queryOptions;
      MetadataDocument<INpmMetadataEntry> statesDocumentAsync = await metadataService.GetPackageVersionStatesDocumentAsync(packageNameRequest);
      if (statesDocumentAsync == null)
        return (RevisionAndVersions) null;
      string revision = statesDocumentAsync.GetRevision();
      return new RevisionAndVersions()
      {
        Revision = revision,
        Versions = statesDocumentAsync.Entries.Select<INpmMetadataEntry, string>((Func<INpmMetadataEntry, string>) (e => e.PackageIdentity.Version.ToString())).ToList<string>()
      };
    }
  }
}
