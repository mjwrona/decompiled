// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.NuGetPackageIngestionServiceFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class NuGetPackageIngestionServiceFactory : IFactory<IPackageIngestionService>
  {
    private readonly IFactory<bool> shouldBlockWriteOperationsFactory;
    private readonly IFeatureFlagService featureFlagService;
    private readonly string skipIngestionOnCsrfValidationFailurefeatureFlag;

    public NuGetPackageIngestionServiceFactory(
      IFactory<bool> shouldBlockWriteOperationsFactory,
      IFeatureFlagService featureFlagService,
      string skipIngestionOnCsrfValidationFailurefeatureFlag)
    {
      this.shouldBlockWriteOperationsFactory = shouldBlockWriteOperationsFactory;
      this.featureFlagService = featureFlagService;
      this.skipIngestionOnCsrfValidationFailurefeatureFlag = skipIngestionOnCsrfValidationFailurefeatureFlag;
    }

    public IPackageIngestionService Get()
    {
      if (!this.shouldBlockWriteOperationsFactory.Get())
        return (IPackageIngestionService) new NuGetPackageIngestionService();
      if (this.featureFlagService.IsEnabled(this.skipIngestionOnCsrfValidationFailurefeatureFlag))
        return (IPackageIngestionService) new NuGetNoopPackageIngestionService();
      throw new PotentiallyDangerousRequestException(Resources.Error_UpstreamIngestion_CannotSkipIngestion());
    }
  }
}
