// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.ParallelResourceMigrationJobsInfo`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class ParallelResourceMigrationJobsInfo<T> where T : IMigrationDataSetter
  {
    internal T[] JobsData { get; set; }

    internal int ParallelJobs => this.JobsData.Length;

    internal TimeSpan DelayBetweenJobs { get; set; }

    public static ParallelResourceMigrationJobsInfo<T> PrepareResourceCopyJobs(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      CreateMigrationData<T> creator)
    {
      Guid hostId = migrationEntry.HostId;
      int totalGroups = 1;
      if (migrationEntry.HostType == TeamFoundationHostType.ProjectCollection)
      {
        totalGroups = requestContext.GetPerHostRegistry<int>(hostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_TotalStorageAccountGroups, false, 1);
        if (totalGroups < 1)
          totalGroups = 1;
      }
      int perHostRegistry1 = requestContext.GetPerHostRegistry<int>(hostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_BlobCopyInContainerParallelism, false, 1);
      int perHostRegistry2 = requestContext.GetPerHostRegistry<int>(hostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_DelayInMillisecBetweenJobs, false, 30000);
      int perHostRegistry3 = requestContext.GetPerHostRegistry<int>(hostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_MaxConcurrentJobsPerJobAgent, false, -1);
      T[] objArray = new T[totalGroups];
      for (int groupIndex = 0; groupIndex < totalGroups; ++groupIndex)
      {
        ParallelResourceMigrationSettings settings = new ParallelResourceMigrationSettings(groupIndex, totalGroups, perHostRegistry1, perHostRegistry3);
        T obj = creator(settings, migrationEntry);
        objArray[groupIndex] = obj;
      }
      return new ParallelResourceMigrationJobsInfo<T>()
      {
        JobsData = objArray,
        DelayBetweenJobs = TimeSpan.FromMilliseconds((double) perHostRegistry2)
      };
    }
  }
}
