// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AzureBlobRetryPolicyFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class AzureBlobRetryPolicyFactory : IFactory<IRetryPolicy>
  {
    public static readonly RegistryQuery InteractiveDeltaBackoffSecondsQuery = new RegistryQuery("/Configuration/Packaging/AzureBlob/Retries/Interactive/DelaySeconds");
    public const double DefaultInteractiveDeltaBackoffSeconds = 0.1;
    public static readonly RegistryQuery InteractiveMaxAttemptsQuery = new RegistryQuery("/Configuration/Packaging/AzureBlob/Retries/Interactive/MaxAttempts");
    public const int DefaultInteractiveMaxAttempts = 3;
    public static readonly RegistryQuery BatchDeltaBackoffSecondsQuery = new RegistryQuery("/Configuration/Packaging/AzureBlob/Retries/Batch/DelaySeconds");
    public const double DefaultBatchDeltaBackoffSeconds = 0.1;
    public static readonly RegistryQuery BatchMaxAttemptsQuery = new RegistryQuery("/Configuration/Packaging/AzureBlob/Retries/Batch/MaxAttempts");
    public const int DefaultBatchMaxAttempts = 10;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IRegistryService registryService;

    public AzureBlobRetryPolicyFactory(
      IExecutionEnvironment executionEnvironment,
      IRegistryService registryService)
    {
      this.executionEnvironment = executionEnvironment;
      this.registryService = registryService;
    }

    public IRetryPolicy Get()
    {
      int num = this.executionEnvironment.IsHostProcessType(HostProcessType.ApplicationTier) ? 1 : 0;
      RegistryQuery registryQuery1 = num != 0 ? AzureBlobRetryPolicyFactory.InteractiveDeltaBackoffSecondsQuery : AzureBlobRetryPolicyFactory.BatchDeltaBackoffSecondsQuery;
      double defaultValue1 = num != 0 ? 0.1 : 0.1;
      RegistryQuery registryQuery2 = num != 0 ? AzureBlobRetryPolicyFactory.InteractiveMaxAttemptsQuery : AzureBlobRetryPolicyFactory.BatchMaxAttemptsQuery;
      int defaultValue2 = num != 0 ? 3 : 10;
      return (IRetryPolicy) new ExponentialRetry(TimeSpan.FromSeconds(this.registryService.GetValue<double>(registryQuery1, defaultValue1)), this.registryService.GetValue<int>(registryQuery2, defaultValue2));
    }
  }
}
