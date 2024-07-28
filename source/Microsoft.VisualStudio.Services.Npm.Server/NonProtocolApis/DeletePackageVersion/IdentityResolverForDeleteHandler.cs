// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion.IdentityResolverForDeleteHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion
{
  public class IdentityResolverForDeleteHandler : 
    IAsyncHandler<PackageNameRequest<NpmPackageName, RevisionAndVersions>, RawPackageRequest>,
    IHaveInputType<PackageNameRequest<NpmPackageName, RevisionAndVersions>>,
    IHaveOutputType<RawPackageRequest>
  {
    private readonly IAsyncHandler<PackageNameRequest<NpmPackageName>, RevisionAndVersions> revisionAndVersionsFromMetadataProvider;

    public IdentityResolverForDeleteHandler(
      IAsyncHandler<PackageNameRequest<NpmPackageName>, RevisionAndVersions> revisionAndVersionsFromMetadataProvider)
    {
      this.revisionAndVersionsFromMetadataProvider = revisionAndVersionsFromMetadataProvider;
    }

    public async Task<RawPackageRequest> Handle(
      PackageNameRequest<NpmPackageName, RevisionAndVersions> request)
    {
      RevisionAndVersions revisionAndVersions = (await this.revisionAndVersionsFromMetadataProvider.Handle((PackageNameRequest<NpmPackageName>) request)).ThrowIfNull<RevisionAndVersions>((Func<Exception>) (() => (Exception) new PackageNotFoundException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageNotFound((object) request.PackageName.FullName, (object) request.Feed.FullyQualifiedName))));
      if (request.AdditionalData.Revision != revisionAndVersions.Revision)
        throw new RevisionMismatchException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_RevisionMismatch());
      List<string> source1 = request.AdditionalData.Versions ?? new List<string>();
      List<string> versions = revisionAndVersions.Versions;
      if (!source1.Any<string>() && !versions.Any<string>())
        return (RawPackageRequest) null;
      List<string> source2 = new List<string>();
      foreach (string str in versions)
      {
        if (!source1.Contains(str))
          source2.Add(str);
      }
      if (source2.Count != 1)
        throw new DeletePackageWithMultipleVersionsNotSupportedException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_MultipleVersionsUnpublished((object) source2.Count));
      return new RawPackageRequest((IFeedRequest) request, request.PackageName.FullName, source2.First<string>());
    }
  }
}
