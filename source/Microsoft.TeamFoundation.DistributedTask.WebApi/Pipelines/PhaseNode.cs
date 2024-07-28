// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [KnownType(typeof (Phase))]
  [KnownType(typeof (ProviderPhase))]
  [JsonConverter(typeof (PhaseNodeJsonConverter))]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PhaseNode : IGraphNode
  {
    internal static readonly HashSet<string> s_nonOverridableVariables = new HashSet<string>((IEnumerable<string>) new string[2]
    {
      WellKnownDistributedTaskVariables.AccessTokenScope,
      WellKnownDistributedTaskVariables.JobParallelismTag
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    [JsonConverter(typeof (PhaseNode.PhaseVariablesJsonConverter))]
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private List<IVariable> m_variables;
    [DataMember(Name = "Dependencies", EmitDefaultValue = false)]
    private List<PhaseDependency> m_dependencies;
    [DataMember(Name = "DependsOn", EmitDefaultValue = false)]
    private HashSet<string> m_dependsOn;

    protected PhaseNode()
    {
    }

    protected PhaseNode(PhaseNode nodeToCopy)
    {
      this.Name = nodeToCopy.Name;
      this.DisplayName = nodeToCopy.DisplayName;
      this.Condition = nodeToCopy.Condition;
      this.ContinueOnError = nodeToCopy.ContinueOnError;
      this.Target = nodeToCopy.Target?.Clone();
      HashSet<string> dependsOn = nodeToCopy.m_dependsOn;
      // ISSUE: explicit non-virtual call
      if ((dependsOn != null ? (__nonvirtual (dependsOn.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_dependsOn = new HashSet<string>((IEnumerable<string>) nodeToCopy.m_dependsOn, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (nodeToCopy.m_variables == null || nodeToCopy.m_variables.Count <= 0)
        return;
      this.m_variables = new List<IVariable>((IEnumerable<IVariable>) nodeToCopy.m_variables);
    }

    [DataMember(EmitDefaultValue = false)]
    public abstract PhaseType Type { get; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<bool>))]
    public ExpressionValue<bool> ContinueOnError { get; set; }

    public ISet<string> DependsOn
    {
      get
      {
        if (this.m_dependsOn == null)
          this.m_dependsOn = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_dependsOn;
      }
    }

    [DataMember]
    public bool Skip => false;

    [DataMember(EmitDefaultValue = false)]
    public PhaseTarget Target { get; set; }

    public IList<IVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new List<IVariable>();
        return (IList<IVariable>) this.m_variables;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public ResourceReferences ExplicitResources { get; set; }

    public virtual void Validate(PipelineBuildContext context, ValidationResult result)
    {
      if (this.Target == null)
        result.Errors.Add(new PipelineValidationError(PipelineStrings.PhaseTargetRequired((object) this.Name)));
      else if (this.Target.Type != PhaseTargetType.Queue && this.Target.Type != PhaseTargetType.Server && this.Target.Type != PhaseTargetType.Pool)
      {
        result.Errors.Add(new PipelineValidationError(PipelineStrings.UnsupportedTargetType((object) this.Name, (object) this.Target.Type)));
      }
      else
      {
        if (string.IsNullOrEmpty(this.Condition))
        {
          this.Condition = GraphCondition<PhaseInstance>.Default;
        }
        else
        {
          PhaseCondition phaseCondition = new PhaseCondition(this.Condition);
        }
        List<IVariable> variables = this.m_variables;
        // ISSUE: explicit non-virtual call
        if ((variables != null ? (__nonvirtual (variables.Count) > 0 ? 1 : 0) : 0) != 0)
        {
          List<IVariable> collection = new List<IVariable>();
          foreach (IVariable variable1 in (IEnumerable<IVariable>) this.Variables)
          {
            switch (variable1)
            {
              case Variable variable2:
                if (!PhaseNode.s_nonOverridableVariables.Contains(variable2.Name))
                  break;
                continue;
              case VariableGroupReference queue:
                if (context.EnvironmentVersion < 2)
                {
                  result.Errors.Add(new PipelineValidationError(PipelineStrings.PhaseVariableGroupNotSupported((object) this.Name, (object) queue)));
                  continue;
                }
                result.ReferencedResources.VariableGroups.Add(queue);
                if (context.BuildOptions.ValidateResources && context.ResourceStore.VariableGroups.Get(queue) == null)
                {
                  result.UnauthorizedResources.VariableGroups.Add(queue);
                  result.Errors.Add(new PipelineValidationError(PipelineStrings.VariableGroupNotFoundForPhase((object) this.Name, (object) queue)));
                  break;
                }
                break;
            }
            collection.Add(variable1);
          }
          this.m_variables.Clear();
          this.m_variables.AddRange((IEnumerable<IVariable>) collection);
        }
        ResourceReferences explicitResources1 = this.ExplicitResources;
        if ((explicitResources1 != null ? (explicitResources1.Repositories.Count > 0 ? 1 : 0) : 0) != 0)
        {
          foreach (string repository in (IEnumerable<string>) this.ExplicitResources.Repositories)
          {
            RepositoryResource repositoryResource = context.ResourceStore?.Repositories.Get(repository);
            if (repositoryResource == null)
            {
              result.Errors.Add(new PipelineValidationError(PipelineStrings.RepositoryResourceNotFoundExplicit((object) this.Name, (object) repository)));
            }
            else
            {
              result.ReferencedResources.Repositories.Add(repositoryResource);
              if (repositoryResource.Endpoint != null)
              {
                context.ReferencedResources.AddEndpointReference(repositoryResource.Endpoint);
                IResourceStore resourceStore = context.ResourceStore;
                if ((resourceStore != null ? resourceStore.GetEndpoint(repositoryResource.Endpoint) : (ServiceEndpoint) null) == null)
                  result.Errors.Add(new PipelineValidationError(PipelineStrings.RepositoryResourceEndpointFoundExplicit((object) repository, (object) repositoryResource.Endpoint)));
              }
            }
          }
        }
        ResourceReferences explicitResources2 = this.ExplicitResources;
        if ((explicitResources2 != null ? (explicitResources2.Queues.Count > 0 ? 1 : 0) : 0) == 0)
          return;
        foreach (string queue in (IEnumerable<string>) this.ExplicitResources.Queues)
        {
          AgentQueueReference agentQueueReference = new AgentQueueReference();
          agentQueueReference.Name = (ExpressionValue<string>) queue;
          AgentQueueReference reference = agentQueueReference;
          TaskAgentQueue taskAgentQueue = context.ResourceStore?.Queues.Get(reference);
          if (taskAgentQueue == null)
          {
            result.Errors.Add(new PipelineValidationError(PipelineStrings.PoolResourceNotFoundExplicit((object) this.Name, (object) queue)));
          }
          else
          {
            reference.Id = taskAgentQueue.Id;
            result.ReferencedResources.Queues.Add(reference);
          }
        }
      }
    }

    protected virtual void UpdateJobContextVariablesFromJob(JobExecutionContext jobContext, Job job)
    {
      jobContext.Variables[WellKnownDistributedTaskVariables.JobDisplayName] = (VariableValue) job.DisplayName;
      jobContext.Variables[WellKnownDistributedTaskVariables.JobId] = (VariableValue) job.Id.ToString("D");
      jobContext.Variables[WellKnownDistributedTaskVariables.JobName] = (VariableValue) job.Name;
      jobContext.Variables[WellKnownDistributedTaskVariables.JobTimeout] = (VariableValue) job.TimeoutInMinutes.ToString("D");
    }

    protected static string ResolveContainerResource(JobExecutionContext context, string inputAlias)
    {
      string str = inputAlias;
      if (inputAlias.Contains(":"))
      {
        IResourceStore resourceStore = context.ResourceStore;
        ContainerResource resource = resourceStore != null ? resourceStore.Containers.GetAll().FirstOrDefault<ContainerResource>((Func<ContainerResource, bool>) (x => x.Endpoint == null && x.Properties.Count == 1 && string.Equals(x.Image, inputAlias, StringComparison.Ordinal))) : (ContainerResource) null;
        if (resource == null)
        {
          ContainerResource containerResource = new ContainerResource();
          containerResource.Alias = Guid.NewGuid().ToString("N");
          containerResource.Image = inputAlias;
          resource = containerResource;
          context.ResourceStore?.Containers.Add(resource);
        }
        str = resource.Alias;
      }
      else
      {
        IResourceStore resourceStore1 = context.ResourceStore;
        ContainerResource containerResource = resourceStore1 != null ? resourceStore1.Containers.GetAll().FirstOrDefault<ContainerResource>((Func<ContainerResource, bool>) (x => string.Equals(x.Alias, inputAlias, StringComparison.Ordinal))) : (ContainerResource) null;
        if (containerResource != null && ExpressionValue.IsExpression(containerResource.Image))
        {
          string evaluatedImageName = ExpressionValue.FromExpression<string>(containerResource.Image).GetValue((IPipelineContext) context).Value;
          IResourceStore resourceStore2 = context.ResourceStore;
          ContainerResource resource = resourceStore2 != null ? resourceStore2.Containers.GetAll().FirstOrDefault<ContainerResource>((Func<ContainerResource, bool>) (x => x.Endpoint == null && x.Properties.Count == 1 && string.Equals(x.Image, evaluatedImageName, StringComparison.Ordinal))) : (ContainerResource) null;
          if (resource == null)
          {
            resource = containerResource.Clone();
            resource.Alias = Guid.NewGuid().ToString("N");
            resource.Image = evaluatedImageName;
            context.ResourceStore?.Containers.Add(resource);
          }
          str = resource.Alias;
        }
      }
      return str;
    }

    protected static void UpdateJobContextReferencedContainers(
      JobExecutionContext context,
      string containerAlias)
    {
      ContainerResource containerResource = context.ResourceStore?.Containers.Get(containerAlias);
      if (containerResource == null)
        throw new ResourceNotFoundException(PipelineStrings.ContainerResourceNotFound((object) containerAlias));
      context.ReferencedResources.Containers.Add(containerResource);
      if (containerResource.Endpoint == null)
        return;
      context.ReferencedResources.AddEndpointReference(containerResource.Endpoint);
      IResourceStore resourceStore = context.ResourceStore;
      if ((resourceStore != null ? resourceStore.GetEndpoint(containerResource.Endpoint) : (ServiceEndpoint) null) == null)
        throw new ResourceNotFoundException(PipelineStrings.ContainerEndpointNotFound((object) containerAlias, (object) containerResource.Endpoint));
    }

    protected static void UpdateJobContextReferencedRepositories(
      JobExecutionContext context,
      string repoAlias)
    {
      RepositoryResource repositoryResource = context.ResourceStore?.Repositories.Get(repoAlias);
      if (repositoryResource == null)
        throw new ResourceNotFoundException(PipelineStrings.RepositoryResourceNotFoundExplicit((object) context.Job.Name, (object) repoAlias));
      context.ReferencedResources.Repositories.Add(repositoryResource);
      if (repositoryResource.Endpoint == null)
        return;
      context.ReferencedResources.AddEndpointReference(repositoryResource.Endpoint);
      IResourceStore resourceStore = context.ResourceStore;
      if ((resourceStore != null ? resourceStore.GetEndpoint(repositoryResource.Endpoint) : (ServiceEndpoint) null) == null)
        throw new ResourceNotFoundException(PipelineStrings.RepositoryResourceEndpointFoundExplicit((object) repoAlias, (object) repositoryResource.Endpoint));
    }

    protected static void UpdateJobContextReferencedQueues(
      JobExecutionContext context,
      string queueName)
    {
      AgentQueueReference agentQueueReference = new AgentQueueReference();
      agentQueueReference.Name = (ExpressionValue<string>) queueName;
      AgentQueueReference reference = agentQueueReference;
      reference.Id = (context.ResourceStore?.Queues.Get(reference) ?? throw new ResourceNotFoundException(PipelineStrings.PoolResourceNotFoundExplicit((object) context.Job.Name, (object) queueName))).Id;
      context.ReferencedResources.Queues.Add(reference);
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      List<PhaseDependency> dependencies = this.m_dependencies;
      // ISSUE: explicit non-virtual call
      if ((dependencies != null ? (__nonvirtual (dependencies.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_dependsOn = new HashSet<string>(this.m_dependencies.Select<PhaseDependency, string>((Func<PhaseDependency, string>) (x => x.Scope)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_dependencies = (List<PhaseDependency>) null;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      HashSet<string> dependsOn = this.m_dependsOn;
      // ISSUE: explicit non-virtual call
      if ((dependsOn != null ? (__nonvirtual (dependsOn.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_dependsOn = (HashSet<string>) null;
      List<IVariable> variables = this.m_variables;
      // ISSUE: explicit non-virtual call
      if ((variables != null ? (__nonvirtual (variables.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_variables = (List<IVariable>) null;
    }

    private class PhaseVariablesJsonConverter : JsonConverter
    {
      public override bool CanConvert(System.Type objectType) => true;

      public override bool CanWrite => true;

      public override object ReadJson(
        JsonReader reader,
        System.Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        if (reader.TokenType == JsonToken.StartArray)
          return (object) serializer.Deserialize<IList<IVariable>>(reader);
        if (reader.TokenType == JsonToken.StartObject)
        {
          IDictionary<string, string> source = serializer.Deserialize<IDictionary<string, string>>(reader);
          if (source != null && source.Count > 0)
            return (object) source.Select<KeyValuePair<string, string>, Variable>((Func<KeyValuePair<string, string>, Variable>) (x => new Variable()
            {
              Name = x.Key,
              Value = x.Value
            })).Cast<IVariable>().ToList<IVariable>();
        }
        return (object) null;
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        if (!(value is IList<IVariable> source) || source.Count <= 0)
          return;
        if (source.Any<IVariable>((Func<IVariable, bool>) (x => x is VariableGroupReference)))
        {
          serializer.Serialize(writer, (object) source);
        }
        else
        {
          Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (Variable variable in source.OfType<Variable>())
            dictionary[variable.Name] = variable.Value;
          serializer.Serialize(writer, (object) dictionary);
        }
      }
    }
  }
}
