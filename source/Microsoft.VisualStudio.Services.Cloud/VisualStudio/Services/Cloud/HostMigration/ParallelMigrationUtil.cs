// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.ParallelMigrationUtil
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public static class ParallelMigrationUtil
  {
    public static void QueueCustomizedMigrationJob<T>(
      ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      string extensionName,
      Guid jobId,
      string jobName,
      T jobData,
      TimeSpan startOffset)
    {
      JobPriorityClass priorityClass = JobPriorityClass.None;
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal;
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId, jobName, extensionName, TeamFoundationSerializationUtility.SerializeToXml((object) jobData), TeamFoundationJobEnabledState.Enabled, true, true, priorityClass);
      jobService.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      IEnumerable<TeamFoundationJobReference> jobReferences = (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      };
      jobService.QueueJobs(requestContext, jobReferences, priorityLevel, (int) startOffset.TotalSeconds, false);
    }

    public static IEnumerable<U> MapByInContainerParallelism<T, U>(
      IEnumerable<T> input,
      Func<T, bool> shouldMap,
      BlobMigrationJobData jobData,
      Func<T, string, U> transform)
    {
      int inContainerParallelism = jobData.InContainerParallelism;
      Random r = new Random();
      return (IEnumerable<U>) input.Where<T>((Func<T, bool>) (e => shouldMap(e))).SelectMany<T, U>((Func<T, IEnumerable<U>>) (t => Enumerable.Range(0, inContainerParallelism).Select<int, U>((Func<int, U>) (i => transform(t, inContainerParallelism <= 1 ? (string) null : i.ToString("X" + ((int) (Math.Log((double) inContainerParallelism) / Math.Log(16.0))).ToString())))))).Union<U>(input.Where<T>((Func<T, bool>) (e => !shouldMap(e))).Select<T, U>((Func<T, U>) (e => transform(e, string.Empty)))).OrderBy<U, int>((Func<U, int>) (_ => r.Next()));
    }

    public static List<StorageMigration> GetSourceGroup(
      TargetHostMigration migrationEntry,
      IGroupIndexable data,
      StorageType? type)
    {
      List<StorageMigration> source = type.HasValue ? ((IEnumerable<StorageMigration>) migrationEntry.StorageResources).Where<StorageMigration>((Func<StorageMigration, bool>) (sr =>
      {
        int storageType = (int) sr.StorageType;
        StorageType? nullable = type;
        int valueOrDefault = (int) nullable.GetValueOrDefault();
        return storageType == valueOrDefault & nullable.HasValue;
      })).ToList<StorageMigration>() : ((IEnumerable<StorageMigration>) migrationEntry.StorageResources).ToList<StorageMigration>();
      return data.TotalGroups <= 1 ? source : ParallelMigrationUtil.GetGroup<StorageMigrationBacked>(source.Select<StorageMigration, StorageMigrationBacked>((Func<StorageMigration, StorageMigrationBacked>) (c => new StorageMigrationBacked(c))).ToList<StorageMigrationBacked>(), data, false).Select<StorageMigrationBacked, StorageMigration>((Func<StorageMigrationBacked, StorageMigration>) (ic => ic.StorageMigration)).ToList<StorageMigration>();
    }

    public static List<Tuple<StorageMigration, AzureProvider>> GetTargetGroup(
      List<Tuple<StorageMigration, AzureProvider>> tuples,
      IGroupIndexable data,
      bool skipNonSharded = false)
    {
      return data.TotalGroups <= 1 ? tuples : ParallelMigrationUtil.GetGroup<StorageMigrationAndProviderBacked>(tuples.Select<Tuple<StorageMigration, AzureProvider>, StorageMigrationAndProviderBacked>((Func<Tuple<StorageMigration, AzureProvider>, StorageMigrationAndProviderBacked>) (t => new StorageMigrationAndProviderBacked(t))).ToList<StorageMigrationAndProviderBacked>(), data, skipNonSharded).Select<StorageMigrationAndProviderBacked, Tuple<StorageMigration, AzureProvider>>((Func<StorageMigrationAndProviderBacked, Tuple<StorageMigration, AzureProvider>>) (ic => Tuple.Create<StorageMigration, AzureProvider>(ic.StorageMigration, ic.AzureProvider))).ToList<Tuple<StorageMigration, AzureProvider>>();
    }

    public static List<T> GetGroup<T>(
      List<T> containers,
      IGroupIndexable data,
      bool skipNonSharded)
      where T : IStorageMigration
    {
      containers.Sort((IComparer<T>) new StorageMigrationComparer<T>());
      int num1 = containers.Where<T>((Func<T, bool>) (c => c.IsSharded)).Count<T>();
      int groupIndex = data.GroupIndex;
      double num2 = (double) num1 / (double) data.TotalGroups;
      double num3 = (double) groupIndex * num2;
      double num4 = (double) (groupIndex + 1) * num2;
      int num5 = 0;
      List<T> group = new List<T>();
      foreach (T container in containers)
      {
        if (!container.IsSharded)
        {
          if (groupIndex == 0 && !skipNonSharded)
            group.Add(container);
        }
        else
        {
          if ((double) num5 >= num3 && (double) num5 < num4)
            group.Add(container);
          ++num5;
        }
      }
      return group;
    }
  }
}
