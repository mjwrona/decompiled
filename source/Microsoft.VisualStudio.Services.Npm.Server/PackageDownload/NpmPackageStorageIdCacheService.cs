// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageDownload.NpmPackageStorageIdCacheService
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageDownload
{
  public class NpmPackageStorageIdCacheService : 
    PackageMetadataCacheService,
    INpmPackageMetadataCacheService,
    IPackageMetadataCacheService,
    IVssFrameworkService
  {
    protected override Guid StorageInvalidationNotificationGuid => new Guid("6017d08c-c893-4ccd-949a-6337e52f0d1e");
  }
}
