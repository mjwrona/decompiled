// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.StorageIdCache.CargoPackageMetadataCacheService
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using System;

namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.StorageIdCache
{
  public class CargoPackageMetadataCacheService : 
    PackageMetadataCacheService,
    ICargoPackageMetadataCacheService,
    IPackageMetadataCacheService,
    IVssFrameworkService
  {
    protected override Guid StorageInvalidationNotificationGuid => new Guid("EB6F0871-17B4-4DA9-8C7E-4F3CC6584EE8");
  }
}
