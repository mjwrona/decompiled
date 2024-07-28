// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.ParallelResourceMigrationSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class ParallelResourceMigrationSettings : 
    IParallelMigrationIndividualSettings,
    IParallelMigrationOverallSettings
  {
    public int GroupIndex { get; private set; }

    public int TotalGroups { get; private set; }

    public int InContainerParallelism { get; private set; }

    public int MaxConcurrentJobsPerJobAgent { get; private set; }

    public ParallelResourceMigrationSettings(
      int groupIndex,
      int totalGroups,
      int inContainerParallelism,
      int maxConcurrentJobsPerJobAgent)
    {
      this.GroupIndex = groupIndex;
      this.TotalGroups = totalGroups;
      this.InContainerParallelism = inContainerParallelism;
      this.MaxConcurrentJobsPerJobAgent = maxConcurrentJobsPerJobAgent;
    }
  }
}
