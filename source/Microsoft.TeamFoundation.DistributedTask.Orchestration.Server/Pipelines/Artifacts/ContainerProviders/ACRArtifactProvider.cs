// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders.ACRArtifactProvider
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
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders
{
  public class ACRArtifactProvider : IContainerProvider
  {
    private const int MaxRetries = 3;

    public void SetContainerResourceVersion(ContainerResource container, string version)
    {
      string str1;
      if (!container.Properties.TryGetValue<string>("repository", out str1) || string.IsNullOrEmpty(str1))
        return;
      string str2 = str1.Split(':')[0] + ":" + version;
      container.Properties.Set<string>("repository", str2);
    }

    public void Validate(ContainerResource container)
    {
      string str1;
      container.Properties.TryGetValue<string>("azureSubscription", out str1);
      if (string.IsNullOrEmpty(str1))
        throw new ResourceValidationException(TaskResources.KeyNotFoundForACR((object) "azureSubscription", (object) container.Alias));
      string str2;
      container.Properties.TryGetValue<string>("resourceGroup", out str2);
      if (string.IsNullOrEmpty(str2))
        throw new ResourceValidationException(TaskResources.KeyNotFoundForACR((object) "resourceGroup", (object) container.Alias));
      string str3;
      container.Properties.TryGetValue<string>("registry", out str3);
      if (string.IsNullOrEmpty(str3))
        throw new ResourceValidationException(TaskResources.KeyNotFoundForACR((object) "registry", (object) container.Alias));
      string str4;
      container.Properties.TryGetValue<string>("repository", out str4);
      if (string.IsNullOrEmpty(str4))
        throw new ResourceValidationException(TaskResources.KeyNotFoundForACR((object) "repository", (object) container.Alias));
    }

    public IList<IVariable> GetVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerResource containerResource,
      IDictionary<string, string> triggerProperties = null)
    {
      IList<IVariable> variables = (IList<IVariable>) new List<IVariable>();
      string azureSubscription;
      containerResource.Properties.TryGetValue<string>("azureSubscription", out azureSubscription);
      containerResource.Properties.TryGetValue<string>("resourceGroup", out string _);
      string str1;
      containerResource.Properties.TryGetValue<string>("registry", out str1);
      string str2;
      containerResource.Properties.TryGetValue<string>("repository", out str2);
      string[] strArray = str2.Split(':');
      string str3 = string.Empty;
      if (strArray.Length == 2)
      {
        str2 = strArray[0];
        str3 = strArray[1];
      }
      string str4;
      if (triggerProperties != null && ContainerProviderHelper.IsTriggeringArtifact(containerResource, triggerProperties) && triggerProperties.TryGetValue("tag", out str4))
        str3 = str4;
      this.AddVariableToACRVariables(variables, containerResource.Alias, "type", "ACR");
      this.AddVariableToACRVariables(variables, containerResource.Alias, "registry", str1);
      this.AddVariableToACRVariables(variables, containerResource.Alias, "repository", str2);
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      dictionary.Add("connection", this.GetConnectionId(requestContext, projectId, azureSubscription));
      dictionary.Add("registryurl", this.GetACRRegistryUrl(str1));
      dictionary.Add("definition", str2);
      if (!string.IsNullOrEmpty(str3))
        dictionary.Add("tag", str3);
      IVssRequestContext requestContext1 = requestContext;
      ProjectInfo projectInfo = new ProjectInfo();
      projectInfo.Id = projectId;
      ContainerResource containerResource1 = containerResource;
      IDictionary<string, string> sourceInputs = dictionary;
      IList<IVariable> acrVariables = variables;
      this.AddVariablesFromDataSourcesToACRVariables(requestContext1, projectInfo, containerResource1, sourceInputs, acrVariables);
      string digest = (variables.FirstOrDefault<IVariable>((Func<IVariable, bool>) (x => (x as Variable).Name == WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "digest"))) as Variable).Value;
      this.AddVariableToACRVariables(variables, containerResource.Alias, "URI", this.GetACRURI(str1, str2, digest));
      return variables;
    }

    private void AddVariablesFromDataSourcesToACRVariables(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IDictionary<string, string> sourceInputs,
      IList<IVariable> acrVariables)
    {
      CustomArtifact artifactType = requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, "AzureContainerRepository") as CustomArtifact;
      this.RetryHelper(requestContext, artifactType, projectInfo, containerResource, acrVariables, sourceInputs, new ACRArtifactProvider.ResolveACRDataSource(this.GetACRTagAndDigest));
      this.RetryHelper(requestContext, artifactType, projectInfo, containerResource, acrVariables, sourceInputs, new ACRArtifactProvider.ResolveACRDataSource(this.GetACRLocation));
    }

    private void GetACRTagAndDigest(
      IVssRequestContext requestContext,
      CustomArtifact acrArtifactType,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IList<IVariable> acrVariables,
      IDictionary<string, string> sourceInputs)
    {
      string inputId = sourceInputs.ContainsKey("tag") ? WellKnownArtifactInputs.SpecificTagAndDigest : WellKnownArtifactInputs.LatestTagAndDigest;
      InputValues inputValues = acrArtifactType.GetInputValues(requestContext, projectInfo, inputId, sourceInputs);
      IList<InputValue> source = inputValues != null ? inputValues.PossibleValues : throw new ArgumentException(TaskResources.FailedToGetACRTagAndDigest((object) containerResource.Alias, (object) inputValues?.Error?.Message));
      int? nullable = source != null ? new int?(source.Count<InputValue>()) : new int?();
      int num = 1;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
      {
        this.AddVariableToACRVariables(acrVariables, containerResource.Alias, "tag", inputValues.PossibleValues[0].DisplayValue);
        this.AddVariableToACRVariables(acrVariables, containerResource.Alias, "digest", inputValues.PossibleValues[0].Value);
      }
    }

    private void GetACRLocation(
      IVssRequestContext requestContext,
      CustomArtifact acrArtifactType,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IList<IVariable> acrVariables,
      IDictionary<string, string> sourceInputs)
    {
      InputValues inputValues = acrArtifactType.GetInputValues(requestContext, projectInfo, WellKnownArtifactInputs.Location, sourceInputs);
      IList<InputValue> source = inputValues != null ? inputValues.PossibleValues : throw new ArgumentException(TaskResources.FailedToGetACRLocation((object) containerResource.Alias, (object) inputValues?.Error?.Message));
      int? nullable = source != null ? new int?(source.Count<InputValue>()) : new int?();
      int num = 1;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        this.AddVariableToACRVariables(acrVariables, containerResource.Alias, "location", inputValues.PossibleValues[0].Value);
    }

    private void RetryHelper(
      IVssRequestContext requestContext,
      CustomArtifact acrArtifact,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IList<IVariable> acrVariables,
      IDictionary<string, string> sourceInputs,
      ACRArtifactProvider.ResolveACRDataSource resolveDataSource)
    {
      int num = 0;
label_1:
      try
      {
        resolveDataSource(requestContext, acrArtifact, projectInfo, containerResource, acrVariables, sourceInputs);
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
      string azureSubscription)
    {
      Guid result;
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> source;
      if (Guid.TryParse(azureSubscription, out result))
        source = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, "AzureRM", (IEnumerable<string>) null, (IEnumerable<Guid>) new List<Guid>()
        {
          result
        }, true, false, ServiceEndpointActionFilter.None);
      else
        source = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, "AzureRM", (IEnumerable<string>) null, (IEnumerable<string>) new List<string>()
        {
          azureSubscription
        }, true, false, ServiceEndpointActionFilter.None);
      if (source.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>() == 1)
        return source.First<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>().Id.ToString();
      if (source.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>() > 1)
        throw new ArgumentException(TaskResources.AmbigousServiceEndpointUsed((object) azureSubscription));
      throw new ArgumentException(TaskResources.EndpointOfTypeNotFound((object) "AzureRM", (object) azureSubscription));
    }

    private void AddVariableToACRVariables(
      IList<IVariable> acrVariables,
      string alias,
      string variable,
      string variableValue)
    {
      if (string.IsNullOrEmpty(variableValue))
        return;
      string variableKey = WellKnownContainerArtifactVariables.GetVariableKey(alias, variable);
      acrVariables.Add((IVariable) new Variable()
      {
        Name = variableKey,
        Value = variableValue
      });
    }

    private string GetACRRegistryUrl(string registry) => string.Format("{0}.azurecr.io", (object) registry).ToLower();

    private string GetACRURI(string registry, string repository, string digest) => string.Format("{0}/{1}@{2}", (object) this.GetACRRegistryUrl(registry), (object) repository, (object) digest);

    private delegate void ResolveACRDataSource(
      IVssRequestContext requestContext,
      CustomArtifact acrArtifactType,
      ProjectInfo projectInfo,
      ContainerResource containerResource,
      IList<IVariable> acrVariables,
      IDictionary<string, string> sourceInputs);
  }
}
