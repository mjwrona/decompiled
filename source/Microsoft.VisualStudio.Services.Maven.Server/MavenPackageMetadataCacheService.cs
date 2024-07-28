// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPackageMetadataCacheService
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPackageMetadataCacheService : 
    PackageMetadataCacheService,
    IMavenPackageMetadataCacheService,
    IPackageMetadataCacheService,
    IVssFrameworkService
  {
    protected override Guid StorageInvalidationNotificationGuid => new Guid("BF22D481-DD18-49F2-8FB9-AEE03473C428");
  }
}
