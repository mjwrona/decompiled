// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.UniqueResourceIdentiferHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  public static class UniqueResourceIdentiferHelper
  {
    public static string GenerateUniqueResourceIdentifier(
      IVssRequestContext requestContext,
      Guid projectId,
      string artifactType,
      IDictionary<string, string> properties)
    {
      IArtifactType artifactType1 = requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, artifactType);
      return UniqueResourceIdentiferHelper.GetNormalizedResourceIdentifier(requestContext, projectId, properties, artifactType1);
    }

    public static string GetNormalizedResourceIdentifier(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> resourceProperties,
      IArtifactType artifactType)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(resourceProperties, nameof (resourceProperties));
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      Guid result1 = Guid.Empty;
      string input1;
      if (resourceProperties.TryGetValue("connection", out input1))
        Guid.TryParse(input1, out result1);
      string normalizedEndpointUrl = UniqueResourceIdentiferHelper.GetNormalizedEndpointUrl(requestContext, projectId, result1);
      if (!string.IsNullOrEmpty(normalizedEndpointUrl))
      {
        if (artifactType.ArtifactTriggerConfiguration != null && artifactType.ArtifactTriggerConfiguration.IsWebhookSupportedAtServerLevel)
          return normalizedEndpointUrl;
        IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>(resourceProperties);
        dictionary["connection"] = normalizedEndpointUrl;
        string projectName = string.Empty;
        IProjectService service = requestContext.GetService<IProjectService>();
        string input2;
        Guid result2;
        if (dictionary.TryGetValue(PipelinePropertyNames.ProjectId, out input2) && Guid.TryParse(input2, out result2))
        {
          dictionary["project"] = input2;
          projectName = service.GetProjectName(requestContext, result2);
        }
        else if (dictionary.TryGetValue(PipelinePropertyNames.Project, out projectName))
        {
          Guid projectId1 = service.GetProjectId(requestContext, projectName);
          dictionary["project"] = projectId1.ToString();
        }
        string source;
        if (dictionary.TryGetValue(PipelinePropertyNames.Source, out source) || dictionary.ContainsKey(PipelinePropertyNames.DefinitionId))
          dictionary["definition"] = ArtifactTriggerHelper.ResolveArtifactSource(requestContext, artifactType, projectName, source, dictionary);
        UniqueResourceIdentiferHelper.ResolveYamlInputMappings(artifactType, dictionary);
        return new MustacheTemplateParser().ReplaceValues(artifactType.UniqueSourceIdentifier, (object) dictionary);
      }
      requestContext.Trace(100161007, TraceLevel.Error, "Deployment", "ArtifactTrigger", "Cannot get endpoint {0} from project {1}.", (object) result1, (object) projectId);
      return string.Empty;
    }

    private static void ResolveYamlInputMappings(
      IArtifactType artifactType,
      IDictionary<string, string> parameters)
    {
      if (parameters == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (artifactType.YamlInputMapping ?? (IDictionary<string, string>) new Dictionary<string, string>()))
      {
        if (parameters.ContainsKey(keyValuePair.Key))
          parameters[keyValuePair.Value] = parameters[keyValuePair.Key];
      }
    }

    private static string GetNormalizedEndpointUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId)
    {
      string normalizedEndpointUrl = string.Empty;
      if (endpointId.Equals(Guid.Empty))
      {
        normalizedEndpointUrl = "self";
      }
      else
      {
        ServiceEndpoint serviceEndpoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectId, endpointId);
        if (serviceEndpoint != null && serviceEndpoint.Url != (Uri) null)
          normalizedEndpointUrl = serviceEndpoint.Url.ToString().TrimEnd('/');
      }
      return normalizedEndpointUrl;
    }
  }
}
