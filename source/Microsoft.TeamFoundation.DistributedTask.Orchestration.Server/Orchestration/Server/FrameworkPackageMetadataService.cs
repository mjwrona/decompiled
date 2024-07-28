// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FrameworkPackageMetadataService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class FrameworkPackageMetadataService : 
    IDistributedTaskPackageMetadataService,
    IVssFrameworkService
  {
    private const string c_layer = "FrameworkPoolService";

    public PackageMetadata GetPackage(
      IVssRequestContext requestContext,
      string packageType,
      string platform,
      string version)
    {
      using (new MethodScope(requestContext, "FrameworkPoolService", nameof (GetPackage)))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrEmpty(platform, nameof (platform), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version), "DistributedTask");
        return requestContext.GetClient<TaskAgentHttpClient>().GetPackageAsync(packageType, platform, version).SyncResult<PackageMetadata>();
      }
    }

    public IList<PackageMetadata> GetPackages(
      IVssRequestContext requestContext,
      string packageType,
      string platform = null,
      int? top = null)
    {
      using (new MethodScope(requestContext, "FrameworkPoolService", nameof (GetPackages)))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType), "DistributedTask");
        return (IList<PackageMetadata>) requestContext.GetClient<TaskAgentHttpClient>().GetPackagesAsync(packageType, platform, top).SyncResult<List<PackageMetadata>>();
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
