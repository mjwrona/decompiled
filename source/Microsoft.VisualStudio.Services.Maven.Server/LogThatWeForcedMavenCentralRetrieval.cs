// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.LogThatWeForcedMavenCentralRetrieval
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class LogThatWeForcedMavenCentralRetrieval : 
    IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>>,
    IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, NullResult>,
    IHaveInputType<IPackageFileRequest<MavenPackageIdentity, IStorageId>>,
    IHaveOutputType<NullResult>
  {
    private readonly ICache<string, object> cache;

    public LogThatWeForcedMavenCentralRetrieval(ICache<string, object> cache) => this.cache = cache;

    public Task<NullResult> Handle(
      IPackageFileRequest<MavenPackageIdentity, IStorageId> request)
    {
      if (request.AdditionalData is TryAllUpstreamsStorageId)
        this.cache.Set("Packaging.Properties.ForcedMavenCentralRetrieval", (object) request.PackageId.ToString());
      return NullResult.NullTask;
    }
  }
}
