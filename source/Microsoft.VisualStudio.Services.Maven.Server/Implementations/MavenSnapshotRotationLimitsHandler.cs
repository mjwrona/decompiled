// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.MavenSnapshotRotationLimitsHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations
{
  public class MavenSnapshotRotationLimitsHandler : 
    IAsyncHandler<FeedCore, SnapshotRotationLimits>,
    IHaveInputType<FeedCore>,
    IHaveOutputType<SnapshotRotationLimits>
  {
    private readonly IRegistryService registryService;

    public MavenSnapshotRotationLimitsHandler(IRegistryService registryService) => this.registryService = registryService;

    public Task<SnapshotRotationLimits> Handle(FeedCore request)
    {
      int minimumSnapshotInstanceCount = this.registryService.GetValue<int>(new RegistryQuery("/Configuration/Feed/SnapshotRetention/MinimumSnapshotInstanceCount", false), 25);
      return Task.FromResult<SnapshotRotationLimits>(new SnapshotRotationLimits(this.registryService.GetValue<int>(new RegistryQuery("/Configuration/Packaging/Maven/SnapshotRetentionRotationTargetCount", false), 30), minimumSnapshotInstanceCount));
    }
  }
}
