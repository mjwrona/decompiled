// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPublicRepositoryPackageMemoryCacheService
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public class NuGetPublicRepositoryPackageMemoryCacheService : 
    PublicRepoPackageMemoryCacheService<NuGetPubCachePackageNameFile>,
    INuGetPublicRepositoryPackageMemoryCacheService,
    IPublicRepoPackageMemoryCacheService<NuGetPubCachePackageNameFile>,
    IVssFrameworkService
  {
    public NuGetPublicRepositoryPackageMemoryCacheService()
      : base(new Guid("F7173A49-7BEC-4D04-BF7C-9229063D7EAC"), (IIdentityResolver) NuGetIdentityResolver.Instance)
    {
    }
  }
}
