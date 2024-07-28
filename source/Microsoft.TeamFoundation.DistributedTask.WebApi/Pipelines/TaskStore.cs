// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TaskStore : ITaskStore
  {
    private readonly TaskStoreFeatureFlags m_featureFlags;
    private IDictionary<Guid, IDictionary<string, TaskDefinition>> m_tasks;
    private IDictionary<string, IDictionary<Guid, IList<TaskDefinition>>> m_nameLookup;
    private IDictionary<Guid, IDictionary<string, string>> m_overrideLookup;
    private IDictionary<Tuple<Guid, string>, IList<string>> m_buildConfigExceptionLookup;
    private static readonly Guid s_publishBuildArtifacts_v0_ID = new Guid("1d341bb0-2106-458c-8422-d00bcea6512a");
    private static readonly TaskDefinition[] WellKnownTaskDefinitions = new TaskDefinition[2]
    {
      PipelineConstants.CheckoutTask,
      PipelineArtifactConstants.DownloadTask
    };

    public TaskStore(params TaskDefinition[] tasks)
      : this((IEnumerable<TaskDefinition>) tasks)
    {
    }

    internal TaskStore(
      IEnumerable<TaskDefinition> tasks,
      ITaskResolver resolver,
      IDictionary<Guid, IDictionary<string, string>> overrideLookup,
      IDictionary<Tuple<Guid, string>, IList<string>> buildConfigExceptionLookup,
      TaskStoreFeatureFlags featureFlags)
      : this(tasks, resolver, overrideLookup, buildConfigExceptionLookup)
    {
      this.m_featureFlags = featureFlags;
    }

    public TaskStore(
      IEnumerable<TaskDefinition> tasks,
      ITaskResolver resolver = null,
      IDictionary<Guid, IDictionary<string, string>> overrideLookup = null,
      IDictionary<Tuple<Guid, string>, IList<string>> buildConfigExceptionLookup = null,
      Func<string, bool> featureCallback = null)
      : this(tasks, resolver, overrideLookup, buildConfigExceptionLookup, featureCallback, (TaskStoreFeatureFlags) null)
    {
    }

    internal TaskStore(
      IEnumerable<TaskDefinition> tasks,
      ITaskResolver resolver,
      IDictionary<Guid, IDictionary<string, string>> overrideLookup,
      IDictionary<Tuple<Guid, string>, IList<string>> buildConfigExceptionLookup,
      Func<string, bool> featureCallback,
      TaskStoreFeatureFlags featureFlags)
    {
      if (featureCallback != null && featureFlags != null)
        throw new ArgumentException("both featureCallback and featureFlags cannot be null", nameof (featureFlags));
      this.m_featureFlags = featureFlags ?? new TaskStoreFeatureFlags(featureCallback);
      this.m_nameLookup = (IDictionary<string, IDictionary<Guid, IList<TaskDefinition>>>) new Dictionary<string, IDictionary<Guid, IList<TaskDefinition>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_tasks = (IDictionary<Guid, IDictionary<string, TaskDefinition>>) new Dictionary<Guid, IDictionary<string, TaskDefinition>>();
      this.Resolver = resolver;
      this.m_overrideLookup = overrideLookup;
      this.m_buildConfigExceptionLookup = buildConfigExceptionLookup;
      tasks = (IEnumerable<TaskDefinition>) ((IEnumerable<TaskDefinition>) TaskStore.WellKnownTaskDefinitions).Concat<TaskDefinition>((tasks != null ? tasks.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x =>
      {
        if (!(x.Id == TaskStore.s_publishBuildArtifacts_v0_ID))
          return true;
        TaskVersion version = x.Version;
        return ((object) version != null ? (version.Major == 0 ? 1 : 0) : 0) == 0;
      })) : (IEnumerable<TaskDefinition>) null) ?? Enumerable.Empty<TaskDefinition>()).ToList<TaskDefinition>();
      foreach (TaskDefinition task in tasks)
        this.AddVersion(task);
      foreach (TaskDefinition taskDefinition in tasks.GroupBy(x => new
      {
        Id = x.Id,
        Major = x.Version.Major
      }).Select<IGrouping<\u003C\u003Ef__AnonymousType39<Guid, int>, TaskDefinition>, TaskDefinition>(x => x.OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (y => y.Version)).First<TaskDefinition>()))
      {
        if (!string.IsNullOrEmpty(taskDefinition.Name))
        {
          IDictionary<Guid, IList<TaskDefinition>> dictionary;
          if (!this.m_nameLookup.TryGetValue(taskDefinition.Name, out dictionary))
          {
            dictionary = (IDictionary<Guid, IList<TaskDefinition>>) new Dictionary<Guid, IList<TaskDefinition>>();
            this.m_nameLookup.Add(taskDefinition.Name, dictionary);
          }
          IList<TaskDefinition> taskDefinitionList;
          if (!dictionary.TryGetValue(taskDefinition.Id, out taskDefinitionList))
          {
            taskDefinitionList = (IList<TaskDefinition>) new List<TaskDefinition>();
            dictionary.Add(taskDefinition.Id, taskDefinitionList);
          }
          taskDefinitionList.Add(taskDefinition);
          if (!string.IsNullOrEmpty(taskDefinition.ContributionIdentifier))
          {
            string key = taskDefinition.ContributionIdentifier + "." + taskDefinition.Name;
            if (!this.m_nameLookup.TryGetValue(key, out dictionary))
            {
              dictionary = (IDictionary<Guid, IList<TaskDefinition>>) new Dictionary<Guid, IList<TaskDefinition>>();
              this.m_nameLookup.Add(key, dictionary);
            }
            if (!dictionary.TryGetValue(taskDefinition.Id, out taskDefinitionList))
            {
              taskDefinitionList = (IList<TaskDefinition>) new List<TaskDefinition>();
              dictionary.Add(taskDefinition.Id, taskDefinitionList);
            }
            taskDefinitionList.Add(taskDefinition);
          }
        }
      }
    }

    public ITaskResolver Resolver { get; }

    public TaskDefinition ResolveTask(Guid taskId, string versionSpec) => this.ResolveTaskWithCircularCheck(taskId, versionSpec, new TaskStore.VisitedVersionHashset());

    private TaskDefinition ResolveTaskWithCircularCheck(
      Guid taskId,
      string versionSpec,
      TaskStore.VisitedVersionHashset circularCheck)
    {
      string key = taskId.ToString() + (object) '+' + versionSpec;
      if (circularCheck.Contains(key))
        throw new InvalidOperationException(TaskStore.GetCircularReferenceMessage(taskId, versionSpec, (IEnumerable<string>) circularCheck));
      circularCheck.Add(key);
      try
      {
        TaskDefinition task = (TaskDefinition) null;
        if (string.IsNullOrEmpty(versionSpec))
          versionSpec = "*";
        IDictionary<string, TaskDefinition> dictionary;
        if (this.m_tasks.TryGetValue(taskId, out dictionary))
          task = TaskVersionSpec.Parse(versionSpec).Match((IEnumerable<TaskDefinition>) dictionary.Values);
        if (task == null && this.Resolver != null)
        {
          task = this.Resolver.Resolve(taskId, versionSpec);
          if (task != null)
            this.AddVersion(task);
        }
        TaskDefinition taskDefinition1 = this.HandleBuildConfigs(task, taskId, circularCheck);
        if (taskDefinition1 != null)
          task = taskDefinition1;
        TaskDefinition taskDefinition2 = this.HandleOverrides(task, circularCheck);
        if (taskDefinition2 != null)
          task = taskDefinition2;
        return task;
      }
      finally
      {
        circularCheck.Remove(key);
      }
    }

    private TaskDefinition HandleBuildConfigs(
      TaskDefinition task,
      Guid taskId,
      TaskStore.VisitedVersionHashset circularCheck)
    {
      if (task?.BuildConfigMapping != null)
        task = !this.m_featureFlags.UseNode16_225BuildConfigState || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node16_225BuildConfig) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node16_225BuildConfig) ? (!this.m_featureFlags.UseNode16BuildConfigState || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node16_219BuildConfig) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node16_219BuildConfig) ? (!this.m_featureFlags.UseNode20_225BuildConfigState || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_225BuildConfig) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_225BuildConfig) ? (!this.m_featureFlags.UseNode20_228BuildConfigState || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_228BuildConfig) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_228BuildConfig) ? (!this.m_featureFlags.UseNode20_229BuildConfigState1 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig1) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig1) ? (!this.m_featureFlags.UseNode20_229BuildConfigState2 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig2) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig2) ? (!this.m_featureFlags.UseNode20_229BuildConfigState3 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig3) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig3) ? (!this.m_featureFlags.UseNode20_229BuildConfigState4 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig4) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig4) ? (!this.m_featureFlags.UseNode20_229BuildConfigState5 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig5) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig5) ? (!this.m_featureFlags.UseNode20_229BuildConfigState6 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig6) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig6) ? (!this.m_featureFlags.UseNode20_229BuildConfigState7 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig7) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig7) ? (!this.m_featureFlags.UseNode20_229BuildConfigState8 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig8) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig8) ? (!this.m_featureFlags.UseNode20_229BuildConfigState9 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig9) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig9) ? (!this.m_featureFlags.UseNode20_229BuildConfigState10 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig10) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig10) ? (!this.m_featureFlags.UseNode20_229BuildConfigState11 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig11) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig11) ? (!this.m_featureFlags.UseNode20_229BuildConfigState12 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig12) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig12) ? (!this.m_featureFlags.UseNode20_229BuildConfigState13 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig13) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig13) ? (!this.m_featureFlags.UseNode20_229BuildConfigState14 || !task.BuildConfigMapping.ContainsKey(TaskStoreBuildConfigs.Node20_229BuildConfig14) || this.HasBuildConfigException(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig14) ? this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Default, circularCheck) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig14, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig13, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig12, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig11, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig10, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig9, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig8, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig7, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig6, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig5, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig4, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig3, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig2, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_229BuildConfig1, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_228BuildConfig, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node20_225BuildConfig, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node16_219BuildConfig, circularCheck)) : this.LookupTaskByBuildConfigMappingAndReturnMatchedVersion(taskId, task, TaskStoreBuildConfigs.Node16_225BuildConfig, circularCheck);
      return task;
    }

    internal static string GetCircularReferenceMessage(
      Guid taskId,
      string versionSpec,
      IEnumerable<string> visitedVersions)
    {
      return string.Format("Circular reference detected while resolving taskId={0} versionSpec={1} visitedVersions={2}", (object) taskId, (object) versionSpec, (object) string.Join(",", visitedVersions));
    }

    private TaskDefinition LookupTaskByBuildConfigMappingAndReturnMatchedVersion(
      Guid taskId,
      TaskDefinition match,
      string targetBuildConfig,
      TaskStore.VisitedVersionHashset circularCheck)
    {
      string str;
      if (match.BuildConfigMapping.TryGetValue(targetBuildConfig, out str) && !match.Version.Equals(new TaskVersion(str)))
        match = this.ResolveTaskWithCircularCheck(taskId, str, circularCheck);
      return match;
    }

    public TaskDefinition ResolveTask(string name, string versionSpec)
    {
      Guid result;
      if (!Guid.TryParse(name, out result))
      {
        IDictionary<Guid, IList<TaskDefinition>> source;
        if (!this.m_nameLookup.TryGetValue(name, out source))
          return (TaskDefinition) null;
        if (source.Count == 1)
        {
          result = source.Keys.Single<Guid>();
        }
        else
        {
          List<Guid> list = source.Where<KeyValuePair<Guid, IList<TaskDefinition>>>((Func<KeyValuePair<Guid, IList<TaskDefinition>>, bool>) (pair => pair.Value.All<TaskDefinition>((Func<TaskDefinition, bool>) (taskDefinition => string.IsNullOrEmpty(taskDefinition.ContributionIdentifier))))).Select<KeyValuePair<Guid, IList<TaskDefinition>>, Guid>((Func<KeyValuePair<Guid, IList<TaskDefinition>>, Guid>) (pair => pair.Key)).ToList<Guid>();
          if (list.Count == 1)
          {
            result = list[0];
          }
          else
          {
            IEnumerable<string> values = source.Values.Select<IList<TaskDefinition>, TaskDefinition>((Func<IList<TaskDefinition>, TaskDefinition>) (x => x.FirstOrDefault<TaskDefinition>())).Select<TaskDefinition, string>((Func<TaskDefinition, string>) (y => y.ContributionIdentifier + "." + y.Name));
            throw new AmbiguousTaskSpecificationException(PipelineStrings.AmbiguousTaskSpecification((object) name, (object) string.Join(", ", values)));
          }
        }
      }
      return this.ResolveTask(result, versionSpec);
    }

    private void AddVersion(TaskDefinition task)
    {
      IDictionary<string, TaskDefinition> dictionary;
      if (!this.m_tasks.TryGetValue(task.Id, out dictionary))
      {
        dictionary = (IDictionary<string, TaskDefinition>) new Dictionary<string, TaskDefinition>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_tasks.Add(task.Id, dictionary);
      }
      dictionary[(string) task.Version] = task;
    }

    public IDictionary<Guid, IDictionary<string, string>> TaskOverrides() => this.m_overrideLookup;

    public IDictionary<Tuple<Guid, string>, IList<string>> TaskBuildConfigExceptions() => this.m_buildConfigExceptionLookup;

    private bool HasBuildConfigException(
      Guid taskId,
      TaskDefinition task,
      string targetBuildConfig)
    {
      IDictionary<Tuple<Guid, string>, IList<string>> dictionary = this.TaskBuildConfigExceptions();
      IList<string> stringList;
      return dictionary != null && dictionary.TryGetValue(new Tuple<Guid, string>(taskId, task.Version.Major.ToString()), out stringList) && stringList != null && stringList.Contains(targetBuildConfig);
    }

    private TaskDefinition HandleOverrides(
      TaskDefinition task,
      TaskStore.VisitedVersionHashset circularCheck)
    {
      if (task == null)
        return (TaskDefinition) null;
      IDictionary<Guid, IDictionary<string, string>> dictionary1 = this.TaskOverrides();
      IDictionary<string, string> dictionary2;
      string versionSpec;
      if (dictionary1 != null && dictionary1.TryGetValue(task.Id, out dictionary2) && dictionary2 != null && dictionary2.TryGetValue((string) task.Version, out versionSpec))
      {
        TaskDefinition taskDefinition = this.ResolveTaskWithCircularCheck(task.Id, versionSpec, circularCheck);
        if (taskDefinition != null)
          task = taskDefinition;
      }
      return task;
    }

    public class VisitedVersionHashset : KeyedCollection<string, string>
    {
      protected override string GetKeyForItem(string item) => item;
    }
  }
}
