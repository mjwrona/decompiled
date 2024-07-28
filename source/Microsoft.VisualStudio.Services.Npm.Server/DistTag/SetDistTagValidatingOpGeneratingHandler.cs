// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.DistTag.SetDistTagValidatingOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.DistTag
{
  public class SetDistTagValidatingOpGeneratingHandler : 
    IAsyncHandler<
    #nullable disable
    PackageRequest<NpmPackageIdentity, string>, NpmDistTagSetOperationData>,
    IHaveInputType<PackageRequest<NpmPackageIdentity, string>>,
    IHaveOutputType<NpmDistTagSetOperationData>
  {
    private readonly INpmMetadataService metadataService;
    public List<INpmMetadataEntry> metadataList;

    public SetDistTagValidatingOpGeneratingHandler(INpmMetadataService metadataService) => this.metadataService = metadataService;

    public async Task<NpmDistTagSetOperationData> Handle(
      PackageRequest<NpmPackageIdentity, string> request)
    {
      string tag = request.AdditionalData;
      if (NpmVersionUtils.TryParseNpmPackageVersion(tag, out SemanticVersion _))
        throw new InvalidDistTagException(Resources.Error_DistTagIsSemanticVersion());
      List<INpmMetadataEntry> versionStatesAsync = await this.metadataService.GetPackageVersionStatesAsync(new PackageNameQuery<INpmMetadataEntry>((IPackageNameRequest) ((IPackageRequest<IPackageIdentity<NpmPackageName, SemanticVersion>>) request).ToPackageNameRequest<NpmPackageName, SemanticVersion>()));
      INpmMetadataEntry metadataEntry1;
      if (versionStatesAsync != null && tag == "latest")
      {
        if (!(versionStatesAsync != null ? versionStatesAsync.Max<INpmMetadataEntry, SemanticVersion>((Func<INpmMetadataEntry, SemanticVersion>) (x => x.PackageIdentity.Version)) : (SemanticVersion) null).Equals(request.PackageId.Version))
          throw new InvalidRequestException(Resources.Error_InvalidLatestPackageVersion());
        metadataEntry1 = versionStatesAsync?.Find((Predicate<INpmMetadataEntry>) (metadataEntry => metadataEntry.PackageIdentity.Version.Equals(request.PackageId.Version)));
      }
      else
        metadataEntry1 = versionStatesAsync?.Find((Predicate<INpmMetadataEntry>) (metadataEntry => metadataEntry.PackageIdentity.Version.Equals(request.PackageId.Version)));
      if (metadataEntry1 == null || metadataEntry1.IsDeleted() || metadataEntry1.IsPermanentlyDeleted())
        throw new PackageNotFoundException(Resources.Error_PackageVersionNotFound((object) request.PackageId.Name.FullName, (object) request.PackageId.Version.DisplayVersion, (object) request.Feed.FullyQualifiedName));
      PackageStateMachineValidator.ThrowIfUningestedUpstreamPackage<NpmPackageIdentity>((IMetadataEntry) metadataEntry1, (IPackageRequest<NpmPackageIdentity>) request);
      NpmDistTagSetOperationData setOperationData = new NpmDistTagSetOperationData((IPackageName) request.PackageId.Name, request.PackageId.Version, tag);
      tag = (string) null;
      return setOperationData;
    }
  }
}
