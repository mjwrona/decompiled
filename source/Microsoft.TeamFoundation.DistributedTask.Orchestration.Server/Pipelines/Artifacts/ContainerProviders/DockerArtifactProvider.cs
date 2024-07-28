// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders.DockerArtifactProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders
{
  public class DockerArtifactProvider : IContainerProvider
  {
    private const int MaxRetries = 3;

    public void SetContainerResourceVersion(ContainerResource container, string version)
    {
      string str1;
      if (!container.Properties.TryGetValue<string>("image", out str1) || string.IsNullOrEmpty(str1))
        return;
      string str2 = str1.Split(':')[0] + ":" + version;
      container.Properties.Set<string>("image", str2);
    }

    public void Validate(ContainerResource containerResource)
    {
      string str;
      containerResource.Properties.TryGetValue<string>("image", out str);
      if (string.IsNullOrEmpty(str))
        throw new ResourceValidationException(TaskResources.KeyNotFoundForDocker((object) "image", (object) containerResource.Alias));
    }

    public IList<IVariable> GetVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerResource containerResource,
      IDictionary<string, string> triggerProperties)
    {
      IList<IVariable> variables = (IList<IVariable>) new List<IVariable>();
      if (containerResource.Endpoint != null && !string.IsNullOrEmpty(containerResource.Endpoint.Name.ToString()))
      {
        string connectionId = this.GetConnectionId(requestContext, projectId, containerResource.Endpoint.Name.ToString());
        if (connectionId != null)
        {
          string str;
          containerResource.Properties.TryGetValue<string>("image", out str);
          string[] strArray = str.Split(':');
          string empty = string.Empty;
          if (strArray.Length == 2)
          {
            str = strArray[0];
            empty = strArray[1];
          }
          this.AddVariableToDockerVariables(variables, containerResource.Alias, "type", "DockerHub");
          this.AddVariableToDockerVariables(variables, containerResource.Alias, "repository", str);
          IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
          dictionary.Add("connection", connectionId);
          dictionary.Add("definition", str);
          if (!string.IsNullOrEmpty(empty))
            dictionary.Add("tag", empty);
          IVssRequestContext requestContext1 = requestContext;
          ProjectInfo projectInfo = new ProjectInfo();
          projectInfo.Id = projectId;
          ContainerResource containerResource1 = containerResource;
          IDictionary<string, string> sourceInputs = dictionary;
          IList<IVariable> dockerVariables = variables;
          this.AddVariablesFromDataSourcesToDockerVariables(requestContext1, projectInfo, containerResource1, sourceInputs, dockerVariables);
          string digest = (variables.FirstOrDefault<IVariable>((Func<IVariable, bool>) (x => (x as Variable).Name == WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "digest"))) as Variable).Value;
          this.AddVariableToDockerVariables(variables, containerResource.Alias, "URI", this.GetDockerURI(str, digest));
        }
      }
      return variables;
    }

    private void AddVariablesFromDataSourcesToDockerVariables(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IDictionary<string, string> sourceInputs,
      IList<IVariable> dockerVariables)
    {
      CustomArtifact artifactType = requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, "DockerHub") as CustomArtifact;
      this.RetryHelper(requestContext, artifactType, projectInfo, containerResource, dockerVariables, sourceInputs, new DockerArtifactProvider.ResolveDockerDataSource(this.GetDockerTagAndDigest));
    }

    private void GetDockerTagAndDigest(
      IVssRequestContext requestContext,
      CustomArtifact dockerArtifactType,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IList<IVariable> dockerVariables,
      IDictionary<string, string> sourceInputs)
    {
      string inputId = sourceInputs.ContainsKey("tag") ? WellKnownArtifactInputs.SpecificTagAndDigest : WellKnownArtifactInputs.LatestTagAndDigest;
      InputValues inputValues = dockerArtifactType.GetInputValues(requestContext, projectInfo, inputId, sourceInputs);
      IList<InputValue> source = inputValues != null ? inputValues.PossibleValues : throw new ArgumentException(TaskResources.FailedToGetDockerTagAndDigest((object) containerResource.Alias, (object) inputValues?.Error?.Message));
      int? nullable = source != null ? new int?(source.Count<InputValue>()) : new int?();
      int num = 1;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
      {
        this.AddVariableToDockerVariables(dockerVariables, containerResource.Alias, "tag", inputValues.PossibleValues[0].DisplayValue);
        this.AddVariableToDockerVariables(dockerVariables, containerResource.Alias, "digest", inputValues.PossibleValues[0].Value);
      }
    }

    private void RetryHelper(
      IVssRequestContext requestContext,
      CustomArtifact dockerArtifact,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IList<IVariable> dockerVariables,
      IDictionary<string, string> sourceInputs,
      DockerArtifactProvider.ResolveDockerDataSource resolveDataSource)
    {
      int num = 0;
label_1:
      try
      {
        resolveDataSource(requestContext, dockerArtifact, projectInfo, containerResource, dockerVariables, sourceInputs);
      }
      catch (Exception ex)
      {
        ++num;
        if (num >= 3)
          throw;
        else
          goto label_1;
      }
    }

    private string GetConnectionId(
      IVssRequestContext requestContext,
      Guid projectId,
      string connection)
    {
      Guid result;
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> source;
      if (Guid.TryParse(connection, out result))
        source = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, "dockerregistry", (IEnumerable<string>) null, (IEnumerable<Guid>) new List<Guid>()
        {
          result
        }, true, false, ServiceEndpointActionFilter.None);
      else
        source = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, "dockerregistry", (IEnumerable<string>) null, (IEnumerable<string>) new List<string>()
        {
          connection
        }, true, false, ServiceEndpointActionFilter.None);
      if (source.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>() == 1)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = source.First<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>();
        string a = (string) null;
        serviceEndpoint.Data?.TryGetValue("registryType", out a);
        if (string.Equals(a, "DockerHub", StringComparison.OrdinalIgnoreCase))
          return serviceEndpoint.Id.ToString();
        requestContext.Trace(10016122, TraceLevel.Warning, "DistributedTask", "ContainerArtifact", TaskResources.DockerHubEndpointNotFound((object) connection, (object) a));
        return (string) null;
      }
      if (source.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>() > 1)
        throw new ArgumentException(TaskResources.AmbigousServiceEndpointUsed((object) connection));
      throw new ArgumentException(TaskResources.EndpointOfTypeNotFound((object) "dockerregistry", (object) connection));
    }

    private void AddVariableToDockerVariables(
      IList<IVariable> dockerVariables,
      string alias,
      string variable,
      string variableValue)
    {
      if (string.IsNullOrEmpty(variableValue))
        return;
      string variableKey = WellKnownContainerArtifactVariables.GetVariableKey(alias, variable);
      dockerVariables.Add((IVariable) new Variable()
      {
        Name = variableKey,
        Value = variableValue
      });
    }

    private string GetDockerURI(string repository, string digest) => string.Format("{0}/{1}@{2}", (object) "docker.io", (object) repository, (object) digest);

    private delegate void ResolveDockerDataSource(
      IVssRequestContext requestContext,
      CustomArtifact acrArtifactType,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IList<IVariable> dockerVariables,
      IDictionary<string, string> sourceInputs);
  }
}
