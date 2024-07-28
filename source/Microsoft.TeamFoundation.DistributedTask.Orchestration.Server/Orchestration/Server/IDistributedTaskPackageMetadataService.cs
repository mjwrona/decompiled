// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IDistributedTaskPackageMetadataService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (FrameworkPackageMetadataService))]
  public interface IDistributedTaskPackageMetadataService : IVssFrameworkService
  {
    PackageMetadata GetPackage(
      IVssRequestContext requestContext,
      string packageType,
      string platform,
      string version);

    IList<PackageMetadata> GetPackages(
      IVssRequestContext requestContext,
      string packageType,
      string platform = null,
      int? top = null);
  }
}
