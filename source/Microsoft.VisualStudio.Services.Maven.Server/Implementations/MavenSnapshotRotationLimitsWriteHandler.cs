// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.MavenSnapshotRotationLimitsWriteHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations
{
  public class MavenSnapshotRotationLimitsWriteHandler : 
    IAsyncHandler<SnapshotRotationLimits>,
    IAsyncHandler<SnapshotRotationLimits, NullResult>,
    IHaveInputType<SnapshotRotationLimits>,
    IHaveOutputType<NullResult>
  {
    private readonly IRegistryWriterService registryService;

    public MavenSnapshotRotationLimitsWriteHandler(IRegistryWriterService registryService) => this.registryService = registryService;

    public Task<NullResult> Handle(SnapshotRotationLimits request)
    {
      this.registryService.Write((IEnumerable<RegistryItem>) new RegistryItem[2]
      {
        new RegistryItem("/Configuration/Feed/SnapshotRetention/MinimumSnapshotInstanceCount", request.MinimumSnapshotInstanceCount.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
        new RegistryItem("/Configuration/Packaging/Maven/SnapshotRetentionRotationTargetCount", request.RotationTargetCount.ToString((IFormatProvider) CultureInfo.InvariantCulture))
      });
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
