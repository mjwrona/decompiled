// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.OptionallyRescheduleJobTemplateOffRegistry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class OptionallyRescheduleJobTemplateOffRegistry : IJobScheduler
  {
    public const int DefaultRescheduleSeconds = -1;
    public const int MinimumRequiredSeconds = 0;
    private readonly IJobQueuer jobQueuer;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IRegistryService registryService;
    private readonly IRandomProvider rng;
    private readonly string minRegistryKey;
    private readonly string maxRegistryKey;

    public OptionallyRescheduleJobTemplateOffRegistry(
      IJobQueuer jobQueuer,
      IFeatureFlagService featureFlagService,
      IRegistryService registryService,
      IRandomProvider rng,
      string minRegistryKey,
      string maxRegistryKey)
    {
      this.featureFlagService = featureFlagService;
      this.registryService = registryService;
      this.rng = rng;
      this.minRegistryKey = minRegistryKey;
      this.maxRegistryKey = maxRegistryKey;
      this.jobQueuer = jobQueuer;
    }

    public bool RescheduleJob(TeamFoundationJobDefinition jobDefinition)
    {
      if (this.featureFlagService.IsEnabled("Packaging.UpdateUpstreamJobSchedule"))
      {
        int minValue = this.registryService.GetValue<int>((RegistryQuery) this.minRegistryKey, -1);
        int maxValue = this.registryService.GetValue<int>((RegistryQuery) this.maxRegistryKey, -1);
        if (minValue > 0)
        {
          int num = maxValue > minValue ? this.rng.Next(minValue, maxValue) : minValue;
          this.jobQueuer.QueueJob((JobId) jobDefinition.JobId, JobPriorityLevel.Idle, TimeSpan.FromSeconds((double) num));
          return true;
        }
      }
      return false;
    }
  }
}
