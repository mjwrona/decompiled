// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.NuGetPackageVersionCountsRetryCountProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  internal class NuGetPackageVersionCountsRetryCountProvider : IRetryCountProvider
  {
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IRegistryService registryService;
    public static readonly RegistryQuery AppTierIterationCountQuery = (RegistryQuery) "/Configuration/Packaging/NuGet/PackageVersionCountsAggregation/ATMaxSaveRetryCount";
    public static readonly RegistryQuery CpjIterationCountQuery = (RegistryQuery) "/Configuration/Packaging/NuGet/PackageVersionCountsAggregation/CpjMaxSaveRetryCount";
    public const int DefaultAppTierIterationCount = 2;
    public const int DefaultCpjIterationCount = 20;

    public NuGetPackageVersionCountsRetryCountProvider(
      IExecutionEnvironment executionEnvironment,
      IRegistryService registryService)
    {
      this.executionEnvironment = executionEnvironment;
      this.registryService = registryService;
    }

    public int Get()
    {
      int num = this.executionEnvironment.IsHostProcessType(HostProcessType.ApplicationTier) ? 1 : 0;
      return this.registryService.GetValue<int>(num != 0 ? NuGetPackageVersionCountsRetryCountProvider.AppTierIterationCountQuery : NuGetPackageVersionCountsRetryCountProvider.CpjIterationCountQuery, num != 0 ? 2 : 20);
    }
  }
}
