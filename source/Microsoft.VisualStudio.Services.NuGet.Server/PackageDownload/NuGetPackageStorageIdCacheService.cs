// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload.NuGetPackageStorageIdCacheService
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload
{
  public class NuGetPackageStorageIdCacheService : 
    PackageMetadataCacheService,
    INuGetPackageMetadataCacheService,
    IPackageMetadataCacheService,
    IVssFrameworkService
  {
    protected override Guid StorageInvalidationNotificationGuid => new Guid("F6A89A6C-2BED-4995-8F78-52219D02031E");
  }
}
