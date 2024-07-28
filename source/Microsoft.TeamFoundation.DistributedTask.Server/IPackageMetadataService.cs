// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IPackageMetadataService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (PlatformPackageMetadataService))]
  public interface IPackageMetadataService : 
    IDistributedTaskPackageMetadataService,
    IVssFrameworkService
  {
    Dictionary<string, string> GetLatestPackageDownloadUrls(
      IVssRequestContext requestContext,
      string packageType);

    PackageVersion GetLatestPackageVersion(
      IVssRequestContext requestContext,
      string packageType,
      string platform);

    PackageMetadata GetLatestCompatiblePackage(
      IVssRequestContext requestContext,
      string packageType,
      TaskAgent taskAgent);
  }
}
