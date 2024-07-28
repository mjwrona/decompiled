// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineBuildContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PipelineBuildContext : PipelineContextBase
  {
    private IInputValidator m_inputValidator;
    private IList<IPhaseProvider> m_phaseProviders;
    private BuildOptions m_buildOptions;

    public PipelineBuildContext()
      : this(new BuildOptions())
    {
    }

    public PipelineBuildContext(IPipelineContext context, BuildOptions options)
      : base(context)
    {
      this.m_buildOptions = options ?? new BuildOptions();
    }

    public PipelineBuildContext(
      BuildOptions buildOptions,
      ICounterStore counterStore = null,
      IResourceStore resourceStore = null,
      IList<IStepProvider> stepProviders = null,
      ITaskStore taskStore = null,
      IPackageStore packageStore = null,
      IInputValidator inputValidator = null,
      IPipelineTraceWriter trace = null,
      EvaluationOptions expressionOptions = null,
      IList<IPhaseProvider> phaseProviders = null,
      IDictionary<string, bool> featureFlags = null)
      : base(counterStore, packageStore, resourceStore, taskStore, stepProviders, trace: trace, expressionOptions: expressionOptions, featureFlags: featureFlags)
    {
      this.m_buildOptions = buildOptions ?? new BuildOptions();
      this.m_inputValidator = inputValidator;
      this.m_phaseProviders = phaseProviders;
    }

    public BuildOptions BuildOptions => this.m_buildOptions;

    public IInputValidator InputValidator => this.m_inputValidator;

    public IReadOnlyList<IPhaseProvider> PhaseProviders => (IReadOnlyList<IPhaseProvider>) this.m_phaseProviders.ToList<IPhaseProvider>();

    internal ValidationResult Validate(PipelineProcess process)
    {
      ValidationResult result = new ValidationResult();
      if (this.ResourceStore != null)
      {
        foreach (ContainerResource containerResource in this.ResourceStore.Containers.GetAll().Where<ContainerResource>((Func<ContainerResource, bool>) (x => x.Endpoint != null)))
        {
          result.ReferencedResources.AddEndpointReference(containerResource.Endpoint);
          if (this.BuildOptions.ValidateResources)
          {
            ServiceEndpoint endpoint = this.ResourceStore.GetEndpoint(containerResource.Endpoint);
            if (endpoint == null)
            {
              result.UnauthorizedResources.AddEndpointReference(containerResource.Endpoint);
              result.Errors.Add(new PipelineValidationError(PipelineStrings.ServiceEndpointNotFound((object) containerResource.Endpoint)));
            }
            else if (!endpoint.Type.Equals("dockerregistry", StringComparison.OrdinalIgnoreCase))
              result.Errors.Add(new PipelineValidationError(PipelineStrings.ContainerResourceInvalidRegistryEndpointType((object) containerResource.Alias, (object) endpoint.Type, (object) endpoint.Name)));
            else
              containerResource.Endpoint = new ServiceEndpointReference()
              {
                Id = endpoint.Id
              };
          }
        }
        foreach (RepositoryResource repositoryResource in this.ResourceStore.Repositories.GetAll())
        {
          Dictionary<string, JToken> dictionary = new Dictionary<string, JToken>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (KeyValuePair<string, JToken> keyValuePair in (IEnumerable<KeyValuePair<string, JToken>>) repositoryResource.Properties.GetItems())
            dictionary[keyValuePair.Key] = this.ExpandVariables(keyValuePair.Value);
          foreach (KeyValuePair<string, JToken> keyValuePair in dictionary)
            repositoryResource.Properties.Set<JToken>(keyValuePair.Key, keyValuePair.Value);
        }
        if (this.EnvironmentVersion > 1)
        {
          RepositoryResource repositoryResource1 = (RepositoryResource) null;
          RepositoryResource repositoryResource2 = this.ResourceStore.Repositories.Get(PipelineConstants.SelfAlias);
          if (repositoryResource2 == null)
          {
            RepositoryResource repositoryResource3 = this.ResourceStore.Repositories.Get(PipelineConstants.DesignerRepo);
            if (repositoryResource3 != null)
              repositoryResource1 = repositoryResource3;
          }
          else
            repositoryResource1 = repositoryResource2;
          if (repositoryResource1 != null)
          {
            result.ReferencedResources.Repositories.Add(repositoryResource1);
            if (repositoryResource1.Endpoint != null)
            {
              result.ReferencedResources.AddEndpointReference(repositoryResource1.Endpoint);
              if (this.BuildOptions.ValidateResources)
              {
                ServiceEndpoint endpoint = this.ResourceStore.GetEndpoint(repositoryResource1.Endpoint);
                if (endpoint == null)
                {
                  result.UnauthorizedResources?.AddEndpointReference(repositoryResource1.Endpoint);
                  result.Errors.Add(new PipelineValidationError(PipelineStrings.ServiceEndpointNotFound((object) repositoryResource1.Endpoint)));
                }
                else
                  repositoryResource1.Endpoint = new ServiceEndpointReference()
                  {
                    Id = endpoint.Id
                  };
              }
            }
          }
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      GraphValidator.Validate<Stage>(this, result, PipelineBuildContext.\u003C\u003EO.\u003C0\u003E__StageNameWhenNoNameIsProvided ?? (PipelineBuildContext.\u003C\u003EO.\u003C0\u003E__StageNameWhenNoNameIsProvided = new Func<object, string>(PipelineStrings.StageNameWhenNoNameIsProvided)), (string) null, process.Stages, PipelineBuildContext.\u003C\u003EO.\u003C1\u003E__GetErrorMessage ?? (PipelineBuildContext.\u003C\u003EO.\u003C1\u003E__GetErrorMessage = new GraphValidator.ErrorFormatter(Stage.GetErrorMessage)));
      return result;
    }
  }
}
