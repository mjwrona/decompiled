// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ArtifactsPartitionedJobSettings
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ArtifactsPartitionedJobSettings
  {
    public static readonly TimeSpan DefaultRelativeTimeSpan = TimeSpan.FromMinutes(30.0);
    private readonly RegistryEntryCollection registryEntries;
    private readonly RegistryEntryCollection deploymentLevelRegistryEntries;

    public ArtifactsPartitionedJobSettings(
      IVssRequestContext requestContext,
      string registryBasePath)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (registryBasePath == null)
        throw new ArgumentNullException(nameof (registryBasePath));
      string query = registryBasePath.TrimEnd('/') + "/*";
      this.registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) query);
      this.deploymentLevelRegistryEntries = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) query);
    }

    protected T GetRegistryEntry<T>(string key, T defaultValue) => this.registryEntries[key.TrimStart('/')].GetValue<T>(defaultValue);

    protected T GetDeploymentLevelRegistryEntry<T>(string key, T defaultValue) => this.deploymentLevelRegistryEntries[key.TrimStart('/')].GetValue<T>(defaultValue);

    public int CpuThreshold => this.GetRegistryEntry<int>("/CpuThreshold", 70);

    public int SubPartitionSize => this.GetRegistryEntry<int>("/BlobIdPartitionSize", 1);

    public int TotalPartitionSize => this.GetRegistryEntry<int>("/TotalPartitions", 1);

    public DateTime AutoJobFanoutRecommendationTimestamp => this.GetRegistryEntry<DateTime>("/AutoJobFanoutRecommendationTimestamp", DateTime.MinValue);

    public int MultiPartitionSize => this.DefaultMultiPartitionSize;

    public int DefaultMultiPartitionSize => this.GetDeploymentLevelRegistryEntry<int>("/DefaultMultiPartitionSize", 0);

    public int MaxParallelism => this.GetRegistryEntry<int>("/MaxParallelism", 0);

    public bool IsEnabledForMultiDomain => this.GetRegistryEntry<bool>("/EnableMultiDomain", false);

    public JobHelper.JobExecutionState JobExecutionState => this.GetRegistryEntry<JobHelper.JobExecutionState>("/EnabledState", JobHelper.JobExecutionState.Enabled);

    public TimeSpan JobSchedulingInterval => TimeSpan.FromSeconds((double) this.GetRegistryEntry<int>("/JobSchedulingIntervalInSeconds", 60));

    public TimeSpan JobExecutionTimeBudget => TimeSpan.FromSeconds((double) this.GetRegistryEntry<int>("/EnforcedJobExecutionTimeout", 86100));

    public TimeSpan ParentAggregationTimeout => this.JobExecutionTimeBudget - ArtifactsPartitionedJobSettings.DefaultRelativeTimeSpan;
  }
}
