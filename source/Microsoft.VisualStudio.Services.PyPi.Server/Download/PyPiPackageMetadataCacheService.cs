// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Download.PyPiPackageMetadataCacheService
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Download
{
  public class PyPiPackageMetadataCacheService : 
    PackageMetadataCacheService,
    IPyPiPackageMetadataCacheService,
    IPackageMetadataCacheService,
    IVssFrameworkService
  {
    protected override Guid StorageInvalidationNotificationGuid => new Guid("6e2ebe01-55f3-4836-bd8f-db0c02594d60");
  }
}
