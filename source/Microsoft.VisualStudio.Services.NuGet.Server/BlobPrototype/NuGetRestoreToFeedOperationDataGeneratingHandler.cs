// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRestoreToFeedOperationDataGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRestoreToFeedOperationDataGeneratingHandler : 
    IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, NuGetRestoreToFeedOperationData>,
    IHaveInputType<PackageRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<NuGetRestoreToFeedOperationData>
  {
    private readonly IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, byte[]> readNuspecHandler;

    public NuGetRestoreToFeedOperationDataGeneratingHandler(
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, byte[]> readNuspecHandler)
    {
      this.readNuspecHandler = readNuspecHandler;
    }

    public async Task<NuGetRestoreToFeedOperationData> Handle(
      PackageRequest<VssNuGetPackageIdentity> input)
    {
      IPackageIdentity packageId = (IPackageIdentity) input.PackageId;
      return new NuGetRestoreToFeedOperationData(packageId, await this.readNuspecHandler.Handle(input));
    }
  }
}
