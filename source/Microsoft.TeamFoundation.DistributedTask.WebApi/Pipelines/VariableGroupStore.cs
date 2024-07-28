// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VariableGroupStore : IVariableGroupStore, IStepProvider
  {
    private readonly bool m_lazyLoadVariableGroups;
    private readonly Dictionary<string, IVariableValueProvider> m_valueProviders;
    private readonly Dictionary<int, VariableGroup> m_resourcesById = new Dictionary<int, VariableGroup>();
    private readonly Dictionary<string, List<VariableGroup>> m_resourcesByName = new Dictionary<string, List<VariableGroup>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public VariableGroupStore(
      IList<VariableGroup> resources,
      IVariableGroupResolver resolver = null,
      bool lazyLoadVariableGroups = false,
      params IVariableValueProvider[] valueProviders)
    {
      this.Resolver = resolver;
      this.Add(resources != null ? resources.ToArray<VariableGroup>() : (VariableGroup[]) null);
      this.m_lazyLoadVariableGroups = lazyLoadVariableGroups;
      if (valueProviders == null || valueProviders.Length == 0)
        return;
      this.m_valueProviders = new Dictionary<string, IVariableValueProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IVariableValueProvider valueProvider in valueProviders)
      {
        if (!this.m_valueProviders.TryAdd<string, IVariableValueProvider>(valueProvider.GroupType, valueProvider))
          throw new ArgumentException("Group type " + valueProvider.GroupType + " cannot have more than one provider", nameof (valueProviders));
      }
    }

    public IVariableGroupResolver Resolver { get; }

    public IList<VariableGroupReference> GetAuthorizedReferences()
    {
      if (!this.m_lazyLoadVariableGroups)
        return (IList<VariableGroupReference>) this.m_resourcesById.Values.Select<VariableGroup, VariableGroupReference>((Func<VariableGroup, VariableGroupReference>) (x => new VariableGroupReference()
        {
          Id = x.Id
        })).ToList<VariableGroupReference>();
      return this.Resolver == null ? (IList<VariableGroupReference>) new List<VariableGroupReference>() : this.Resolver.GetAuthorizedReferences();
    }

    public void Authorize(VariableGroupReference reference) => this.Resolver?.Authorize(reference);

    public VariableGroup Get(VariableGroupReference reference)
    {
      if (reference == null)
        return (VariableGroup) null;
      int id = reference.Id;
      string literal = reference.Name?.Literal;
      if (id == 0 && string.IsNullOrEmpty(literal))
        return (VariableGroup) null;
      VariableGroup variableGroup = (VariableGroup) null;
      if (id != 0)
      {
        if (this.m_resourcesById.TryGetValue(id, out variableGroup))
          return variableGroup;
      }
      else
      {
        List<VariableGroup> variableGroupList;
        if (!string.IsNullOrEmpty(literal) && this.m_resourcesByName.TryGetValue(literal, out variableGroupList))
          return variableGroupList.Count <= 1 ? variableGroupList[0] : throw new AmbiguousResourceSpecificationException(PipelineStrings.AmbiguousVariableGroupSpecification((object) literal));
      }
      IVariableGroupResolver resolver = this.Resolver;
      variableGroup = resolver != null ? resolver.Resolve(reference) : (VariableGroup) null;
      if (variableGroup != null)
        this.Add(variableGroup);
      return variableGroup;
    }

    public IList<TaskStep> GetPreSteps(IPipelineContext context, IReadOnlyList<JobStep> steps)
    {
      if (context.ReferencedResources.VariableGroups.Count == 0)
        return (IList<TaskStep>) null;
      if (context.EnvironmentVersion < 2 && context is PipelineExecutionContext)
        return (IList<TaskStep>) null;
      List<TaskStep> collection = new List<TaskStep>();
      foreach (VariableGroupReference group in context.ReferencedResources.VariableGroups.Where<VariableGroupReference>((Func<VariableGroupReference, bool>) (x => x.SecretStore != null && x.SecretStore.Keys.Count > 0)))
      {
        IVariableValueProvider valueProvider = this.GetValueProvider(group);
        if (valueProvider != null && !valueProvider.ShouldGetValues(context))
          collection.AddRangeIfRangeNotNull<TaskStep, List<TaskStep>>((IEnumerable<TaskStep>) valueProvider.GetSteps(context, group, (IEnumerable<string>) group.SecretStore.Keys));
      }
      return (IList<TaskStep>) collection;
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPreTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<string>> GetInputsToProvide(IPipelineContext context) => new Dictionary<Guid, List<string>>();

    public IList<TaskStep> GetPostSteps(IPipelineContext context, IReadOnlyList<JobStep> steps) => (IList<TaskStep>) new List<TaskStep>();

    public bool ResolveStep(
      IPipelineContext context,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      resolvedSteps = (IList<TaskStep>) new List<TaskStep>();
      return false;
    }

    public IVariableValueProvider GetValueProvider(VariableGroupReference group)
    {
      IVariableValueProvider variableValueProvider;
      return this.m_valueProviders != null && this.m_valueProviders.TryGetValue(group.GroupType, out variableValueProvider) ? variableValueProvider : (IVariableValueProvider) null;
    }

    private void Add(params VariableGroup[] resources)
    {
      if (resources == null || resources.Length == 0)
        return;
      foreach (VariableGroup resource in resources)
      {
        if (!this.m_resourcesById.TryGetValue(resource.Id, out VariableGroup _))
        {
          this.m_resourcesById.Add(resource.Id, resource);
          List<VariableGroup> variableGroupList;
          if (!this.m_resourcesByName.TryGetValue(resource.Name, out variableGroupList))
          {
            variableGroupList = new List<VariableGroup>();
            this.m_resourcesByName.Add(resource.Name, variableGroupList);
          }
          variableGroupList.Add(resource);
        }
      }
    }
  }
}
