// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeRetryCountProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public class VersionListWithSizeRetryCountProvider : IRetryCountProvider
  {
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IRegistryService registryService;
    private readonly IProtocol protocol;
    public const int DefaultAppTierIterationCount = 2;
    public const int DefaultCpjIterationCount = 20;

    public VersionListWithSizeRetryCountProvider(
      IExecutionEnvironment executionEnvironment,
      IRegistryService registryService,
      IProtocol protocol)
    {
      this.executionEnvironment = executionEnvironment;
      this.registryService = registryService;
      this.protocol = protocol;
    }

    public int Get()
    {
      int num = this.executionEnvironment.IsHostProcessType(HostProcessType.ApplicationTier) ? 1 : 0;
      RegistryQuery iterationCountQuery1 = this.GetAppTierIterationCountQuery();
      RegistryQuery iterationCountQuery2 = this.GetCpjIterationCountQuery();
      return this.registryService.GetValue<int>(num != 0 ? iterationCountQuery1 : iterationCountQuery2, num != 0 ? 2 : 20);
    }

    public RegistryQuery GetAppTierIterationCountQuery() => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/VersionListWithSizeAggregation/ATMaxSaveRetryCount", (object) this.protocol);

    public RegistryQuery GetCpjIterationCountQuery() => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/VersionListWithSizeAggregation/CpjMaxSaveRetryCount", (object) this.protocol);
  }
}
