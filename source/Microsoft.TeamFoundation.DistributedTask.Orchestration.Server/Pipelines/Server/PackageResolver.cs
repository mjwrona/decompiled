// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.PackageResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class PackageResolver : IPackageResolver
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly IDistributedTaskPackageMetadataService m_packageMetadataService;

    public PackageResolver(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_packageMetadataService = requestContext.GetService<IDistributedTaskPackageMetadataService>();
    }

    public IList<PackageMetadata> GetPackages(string packageType) => (IList<PackageMetadata>) new PackageMetadata[1]
    {
      this.m_packageMetadataService.GetPackages(this.m_requestContext, packageType, "win-x64", new int?(1)).FirstOrDefault<PackageMetadata>() ?? this.m_packageMetadataService.GetPackages(this.m_requestContext, packageType, "win7-x64", new int?(1)).First<PackageMetadata>()
    };
  }
}
