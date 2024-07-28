// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Extensions.PipelineTriggerExtension
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Extensions
{
  public static class PipelineTriggerExtension
  {
    private static char m_uniqueSourceIdentifierSeparator = ':';

    public static IDictionary<string, string> ToDictionary(this PipelineDefinitionTrigger trigger)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (trigger.ArtifactDefinition.Connection != Guid.Empty)
        dictionary["connection"] = trigger.ArtifactDefinition.Connection.ToString();
      if (trigger.ArtifactDefinition.Properties != null)
      {
        foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) trigger.ArtifactDefinition.Properties)
          dictionary.Add(property.Key, property.Value);
      }
      if (trigger.ArtifactDefinition.Project != null)
        dictionary[PipelinePropertyNames.ProjectId] = trigger.ArtifactDefinition.Project.Id.ToString();
      dictionary[PipelineWebHookPublisherPropertyNames.PipelineDefinitionId] = trigger.ArtifactDefinition.Source;
      dictionary["pipelineTriggerType"] = trigger.TriggerType.ToString();
      return (IDictionary<string, string>) dictionary;
    }

    public static IDictionary<string, string> ToDictionary(
      this PipelineDefinitionTrigger trigger,
      IVssRequestContext requestContext,
      IArtifactType artifactType)
    {
      IDictionary<string, string> dictionary = trigger.ToDictionary();
      string projectName = (string) null;
      if (trigger.ArtifactDefinition.Project != null)
      {
        projectName = trigger.ArtifactDefinition.Project.Name;
        if (string.IsNullOrEmpty(projectName))
          projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, trigger.ArtifactDefinition.Project.Id);
      }
      if (!string.IsNullOrEmpty(trigger.ArtifactDefinition.Source))
        dictionary[PipelineWebHookPublisherPropertyNames.PipelineDefinitionId] = ArtifactTriggerHelper.ResolveArtifactSource(requestContext, artifactType, projectName, trigger.ArtifactDefinition.Source);
      return dictionary;
    }

    public static IList<PipelineDefinitionTrigger> ToPipelineDefinitionTriggers(
      this PipelineResources resources,
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId)
    {
      string alias = "";
      IList<PipelineDefinitionTrigger> definitionTriggers = (IList<PipelineDefinitionTrigger>) new List<PipelineDefinitionTrigger>();
      try
      {
        foreach (PipelineResource resource in (IEnumerable<PipelineResource>) (resources?.Pipelines ?? (ISet<PipelineResource>) new HashSet<PipelineResource>()))
        {
          if (resource.Trigger != null)
          {
            alias = resource.Alias;
            definitionTriggers.Add(resource.Trigger.ToPipelineDefinitionTrigger(requestContext, projectId, resource, pipelineDefinitionId));
          }
        }
        foreach (ContainerResource resource in (IEnumerable<ContainerResource>) (resources?.Containers ?? (ISet<ContainerResource>) new HashSet<ContainerResource>()))
        {
          if (resource.Trigger != null)
          {
            alias = resource.Alias;
            definitionTriggers.Add(resource.Trigger.ToPipelineDefinitionTrigger(requestContext, projectId, resource, pipelineDefinitionId));
          }
        }
        foreach (BuildResource resource in (IEnumerable<BuildResource>) (resources?.Builds ?? (ISet<BuildResource>) new HashSet<BuildResource>()))
        {
          if (resource.Trigger != null)
          {
            alias = resource.Alias;
            definitionTriggers.Add(resource.Trigger.ToPipelineDefinitionTrigger(requestContext, projectId, resource, pipelineDefinitionId));
          }
        }
        if (requestContext.IsFeatureEnabled("DistributedTask.EnablePackageResourceTriggers"))
        {
          foreach (PackageResource resource in (IEnumerable<PackageResource>) (resources?.Packages ?? (ISet<PackageResource>) new HashSet<PackageResource>()))
          {
            if (resource.Trigger != null)
            {
              alias = resource.Alias;
              definitionTriggers.Add(resource.Trigger.ToPipelineDefinitionTrigger(requestContext, projectId, resource, pipelineDefinitionId));
            }
          }
        }
        if (requestContext.IsFeatureEnabled("DistributedTask.EnableWebHookResourceTriggers"))
        {
          foreach (WebhookResource resource in (IEnumerable<WebhookResource>) (resources?.Webhooks ?? (ISet<WebhookResource>) new HashSet<WebhookResource>()))
          {
            if (resource.Trigger != null)
            {
              alias = resource.Alias;
              definitionTriggers.Add(resource.Trigger.ToPipelineDefinitionTrigger(requestContext, projectId, resource, pipelineDefinitionId));
            }
          }
        }
      }
      catch (Exception ex)
      {
        PipelineTriggerExtension.AddTriggerIssue(requestContext, projectId, pipelineDefinitionId, alias);
      }
      return definitionTriggers;
    }

    public static void AddTriggerIssue(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      string alias)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
        return;
      PipelineTriggerIssues pipelineTriggerIssues = new PipelineTriggerIssues()
      {
        PipelineDefinitionId = pipelineDefinitionId,
        Alias = alias,
        BuildNumber = "",
        ErrorMessage = DeploymentResources.ErrorTriggerConfiguration(),
        isError = true
      };
      requestContext.GetService<IPipelineTriggerIssuesService>().CreatePipelineTriggerIssues(requestContext, projectId, pipelineDefinitionId, (IList<PipelineTriggerIssues>) new PipelineTriggerIssues[1]
      {
        pipelineTriggerIssues
      });
    }

    public static PipelineResourceTrigger ToPipelineResourceTrigger(this PipelineTrigger trigger)
    {
      ArgumentUtility.CheckForNull<PipelineTrigger>(trigger, nameof (trigger));
      return trigger as PipelineResourceTrigger;
    }

    public static ContainerResourceTrigger ToContainerResourceTrigger(this PipelineTrigger trigger)
    {
      ArgumentUtility.CheckForNull<PipelineTrigger>(trigger, nameof (trigger));
      return trigger as ContainerResourceTrigger;
    }

    public static BuildResourceTrigger ToBuildResourceTrigger(this PipelineTrigger trigger)
    {
      ArgumentUtility.CheckForNull<PipelineTrigger>(trigger, nameof (trigger));
      return trigger as BuildResourceTrigger;
    }

    public static PackageResourceTrigger ToPackageResourceTrigger(this PipelineTrigger trigger)
    {
      ArgumentUtility.CheckForNull<PipelineTrigger>(trigger, nameof (trigger));
      return trigger as PackageResourceTrigger;
    }

    public static WebhookResourceTrigger ToWebHookResourceTrigger(this PipelineTrigger trigger)
    {
      ArgumentUtility.CheckForNull<PipelineTrigger>(trigger, nameof (trigger));
      return trigger as WebhookResourceTrigger;
    }

    public static void UpdateUniqueResourceIdentifier(
      this ArtifactDefinitionReference artifactDefinition,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      if (artifactDefinition.Properties != null)
        dictionary1.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) artifactDefinition.Properties);
      Guid guid;
      if (artifactDefinition.Connection != Guid.Empty)
      {
        Dictionary<string, string> dictionary2 = dictionary1;
        guid = artifactDefinition.Connection;
        string str = guid.ToString();
        dictionary2["connection"] = str;
      }
      if (artifactDefinition.Project != null)
      {
        Dictionary<string, string> dictionary3 = dictionary1;
        string projectId1 = PipelinePropertyNames.ProjectId;
        guid = artifactDefinition.Project.Id;
        string str = guid.ToString();
        dictionary3[projectId1] = str;
      }
      if (!string.IsNullOrEmpty(artifactDefinition.Source))
        dictionary1[PipelinePropertyNames.Source] = artifactDefinition.Source;
      artifactDefinition.UniqueResourceIdentifier = UniqueResourceIdentiferHelper.GenerateUniqueResourceIdentifier(requestContext, projectId, artifactDefinition.ArtifactType, (IDictionary<string, string>) dictionary1);
    }

    public static void UpdateFromUniqueResourceIdentifier(
      this ArtifactDefinitionReference artifactDefinition)
    {
      string[] strArray = artifactDefinition.UniqueResourceIdentifier.Split(PipelineTriggerExtension.m_uniqueSourceIdentifierSeparator);
      if (strArray.Length <= 1)
        return;
      if (strArray.Length == 2)
      {
        artifactDefinition.Source = strArray[1];
      }
      else
      {
        artifactDefinition.Source = strArray[2];
        Guid result;
        if (!Guid.TryParse(strArray[1], out result))
          return;
        artifactDefinition.Project = new ProjectInfo()
        {
          Id = result
        };
      }
    }

    private static PipelineDefinitionTrigger ToPipelineDefinitionTrigger(
      this PipelineResourceTrigger resourceTrigger,
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource resource,
      int pipelineDefinitionId)
    {
      ArgumentUtility.CheckForNull<PipelineResourceTrigger>(resourceTrigger, nameof (resourceTrigger));
      PipelineResourceTrigger pipelineResourceTrigger = resourceTrigger.Clone();
      return new PipelineDefinitionTrigger()
      {
        Alias = resource.Alias,
        TriggerType = PipelineTriggerType.PipelineCompletion,
        PipelineDefinitionId = pipelineDefinitionId,
        TriggerContent = (PipelineTrigger) pipelineResourceTrigger,
        ArtifactDefinition = resource.GetArtifactDefinitionReference(requestContext, projectId)
      };
    }

    private static PipelineDefinitionTrigger ToPipelineDefinitionTrigger(
      this ContainerResourceTrigger resourceTrigger,
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerResource resource,
      int pipelineDefinitionId)
    {
      ArgumentUtility.CheckForNull<ContainerResourceTrigger>(resourceTrigger, nameof (resourceTrigger));
      ContainerResourceTrigger containerResourceTrigger = resourceTrigger.Clone();
      return new PipelineDefinitionTrigger()
      {
        Alias = resource.Alias,
        TriggerType = PipelineTriggerType.ContainerImage,
        PipelineDefinitionId = pipelineDefinitionId,
        TriggerContent = (PipelineTrigger) containerResourceTrigger,
        ArtifactDefinition = resource.GetArtifactDefinitionReference(requestContext, projectId)
      };
    }

    private static PipelineDefinitionTrigger ToPipelineDefinitionTrigger(
      this BuildResourceTrigger resourceTrigger,
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource resource,
      int pipelineDefinitionId)
    {
      ArgumentUtility.CheckForNull<BuildResourceTrigger>(resourceTrigger, nameof (resourceTrigger));
      BuildResourceTrigger buildResourceTrigger = resourceTrigger.Clone();
      return new PipelineDefinitionTrigger()
      {
        Alias = resource.Alias,
        TriggerType = PipelineTriggerType.BuildResourceCompletion,
        PipelineDefinitionId = pipelineDefinitionId,
        TriggerContent = (PipelineTrigger) buildResourceTrigger,
        ArtifactDefinition = resource.GetArtifactDefinitionReference(requestContext, projectId)
      };
    }

    private static PipelineDefinitionTrigger ToPipelineDefinitionTrigger(
      this PackageResourceTrigger resourceTrigger,
      IVssRequestContext requestContext,
      Guid projectId,
      PackageResource resource,
      int pipelineDefinitionId)
    {
      ArgumentUtility.CheckForNull<PackageResourceTrigger>(resourceTrigger, nameof (resourceTrigger));
      PackageResourceTrigger packageResourceTrigger = resourceTrigger.Clone();
      return new PipelineDefinitionTrigger()
      {
        Alias = resource.Alias,
        TriggerType = PipelineTriggerType.PackageUpdate,
        PipelineDefinitionId = pipelineDefinitionId,
        TriggerContent = (PipelineTrigger) packageResourceTrigger,
        ArtifactDefinition = resource.GetArtifactDefinitionReference(requestContext, projectId)
      };
    }

    private static PipelineDefinitionTrigger ToPipelineDefinitionTrigger(
      this WebhookResourceTrigger webHookResourceTrigger,
      IVssRequestContext requestContext,
      Guid projectId,
      WebhookResource resource,
      int pipelineDefinitionId)
    {
      ArgumentUtility.CheckForNull<WebhookResourceTrigger>(webHookResourceTrigger, nameof (webHookResourceTrigger));
      ArgumentUtility.CheckForNull<WebhookResource>(resource, nameof (resource));
      WebhookResourceTrigger webhookResourceTrigger = webHookResourceTrigger.Clone();
      return new PipelineDefinitionTrigger()
      {
        Alias = resource.Alias,
        TriggerType = PipelineTriggerType.WebhookTriggeredEvent,
        PipelineDefinitionId = pipelineDefinitionId,
        TriggerContent = (PipelineTrigger) webhookResourceTrigger,
        ArtifactDefinition = resource.GetArtifactDefinitionReference(requestContext, projectId)
      };
    }

    private static ArtifactDefinitionReference GetArtifactDefinitionReference(
      this PipelineResource resource,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      string str = resource.Properties.Get<string>(PipelinePropertyNames.Source);
      ProjectInfo projectInfo = new ProjectInfo()
      {
        Id = projectId
      };
      string projectName = resource.Properties.Get<string>(PipelinePropertyNames.Project);
      if (!string.IsNullOrEmpty(projectName))
        projectInfo = requestContext.GetService<IProjectService>().GetProject(requestContext, projectName);
      ArtifactDefinitionReference artifactDefinition = new ArtifactDefinitionReference()
      {
        ArtifactType = "Pipeline",
        Project = projectInfo,
        Source = str
      };
      string connection = resource.Properties.Get<string>("connection");
      if (!string.IsNullOrEmpty(connection))
        artifactDefinition.Connection = PipelineTriggerExtension.GetConnectionId(requestContext, projectInfo.Id, connection);
      artifactDefinition.UpdateUniqueResourceIdentifier(requestContext, projectInfo.Id);
      return artifactDefinition;
    }

    private static ArtifactDefinitionReference GetArtifactDefinitionReference(
      this ContainerResource resource,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      string str1 = resource.Properties.Get<string>("repository");
      if (str1 != null)
        str1 = ((IEnumerable<string>) str1.Split(':')).FirstOrDefault<string>();
      IDictionary<string, string> resourceInputs = resource.GetResourceInputs();
      string str2 = resource.Properties.Get<string>("type");
      if (str2.Equals("ACR", StringComparison.OrdinalIgnoreCase))
        str2 = "AzureContainerRepository";
      ArtifactDefinitionReference artifactDefinition = new ArtifactDefinitionReference()
      {
        ArtifactType = str2,
        Source = str1,
        Properties = resourceInputs
      };
      string connection = resource.Properties.Get<string>("connection") ?? resource.Properties.Get<string>("azureSubscription");
      if (!string.IsNullOrEmpty(connection))
        artifactDefinition.Connection = PipelineTriggerExtension.GetConnectionId(requestContext, projectId, connection);
      artifactDefinition.UpdateUniqueResourceIdentifier(requestContext, projectId);
      return artifactDefinition;
    }

    private static ArtifactDefinitionReference GetArtifactDefinitionReference(
      this BuildResource resource,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      string str1 = resource.Properties.Get<string>("type");
      string str2 = resource.Properties.Get<string>(PipelinePropertyNames.Source);
      IDictionary<string, string> resourceInputs = resource.GetResourceInputs();
      ArtifactDefinitionReference artifactDefinition = new ArtifactDefinitionReference()
      {
        ArtifactType = str1,
        Source = str2,
        Properties = resourceInputs
      };
      string connection = resource.Endpoint?.Name?.GetValue()?.Value;
      if (!string.IsNullOrEmpty(connection))
        artifactDefinition.Connection = PipelineTriggerExtension.GetConnectionId(requestContext, projectId, connection);
      artifactDefinition.UpdateUniqueResourceIdentifier(requestContext, projectId);
      return artifactDefinition;
    }

    private static ArtifactDefinitionReference GetArtifactDefinitionReference(
      this WebhookResource resource,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      string str = resource.Properties.Get<string>("type");
      IDictionary<string, string> resourceInputs = resource.GetResourceInputs();
      ArtifactDefinitionReference artifactDefinition = new ArtifactDefinitionReference()
      {
        ArtifactType = str,
        Properties = resourceInputs
      };
      string connection = resource.Endpoint?.Name?.GetValue()?.Value;
      if (!string.IsNullOrEmpty(connection))
        artifactDefinition.Connection = PipelineTriggerExtension.GetConnectionId(requestContext, projectId, connection);
      artifactDefinition.UpdateUniqueResourceIdentifier(requestContext, projectId);
      return artifactDefinition;
    }

    private static ArtifactDefinitionReference GetArtifactDefinitionReference(
      this PackageResource resource,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      string str1 = resource.Properties.Get<string>("type");
      string str2 = resource.Properties.Get<string>(PipelinePropertyNames.Source);
      IDictionary<string, string> resourceInputs = resource.GetResourceInputs();
      string connection = resource.Endpoint?.Name?.GetValue()?.Value;
      Guid connectionId = Guid.Empty;
      if (!string.IsNullOrEmpty(connection))
        connectionId = PipelineTriggerExtension.GetConnectionId(requestContext, projectId, connection);
      PipelineTriggerExtension.UpdatePackageInfo(requestContext, projectId, resourceInputs, connectionId);
      ArtifactDefinitionReference artifactDefinition = new ArtifactDefinitionReference();
      artifactDefinition.ArtifactType = str1;
      artifactDefinition.Source = str2;
      artifactDefinition.Properties = resourceInputs;
      artifactDefinition.Connection = connectionId;
      artifactDefinition.UpdateUniqueResourceIdentifier(requestContext, projectId);
      return artifactDefinition;
    }

    private static void UpdatePackageInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> packageResourceProperty,
      Guid connectionId)
    {
      string[] strArray = packageResourceProperty[PackagePropertyNames.Name].Split('/');
      packageResourceProperty[PackagePropertyNames.Repo] = strArray.Length == 2 ? strArray[0] : throw new InvalidRequestException(DeploymentResources.IncorrectPackageFormat());
      packageResourceProperty[PackagePropertyNames.Name] = strArray[1];
      string gitHubAccountName = PipelineTriggerExtension.GetGitHubAccountName(requestContext, projectId, packageResourceProperty, connectionId);
      packageResourceProperty[PackagePropertyNames.Owner] = !string.IsNullOrEmpty(gitHubAccountName) ? gitHubAccountName : throw new InvalidRequestException(DeploymentResources.CouldNotFetchUser());
    }

    private static Guid GetConnectionId(
      IVssRequestContext requestContext,
      Guid projectId,
      string connection)
    {
      Guid result;
      if (!Guid.TryParse(connection, out result))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<string>) new string[1]
        {
          connection
        }, false, true, ServiceEndpointActionFilter.Use).FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>();
        result = serviceEndpoint != null ? serviceEndpoint.Id : Guid.Empty;
      }
      return result;
    }

    private static string GetGitHubAccountName(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> properties,
      Guid connectionId)
    {
      string gitHubAccountName = string.Empty;
      GitHubResult<GitHubData.V3.User> user = PipelineTriggerExtension.GetGitHubHttpClient(requestContext).GetUser((string) null, PipelineTriggerExtension.GetAuthenticationScheme(requestContext, projectId, properties, connectionId));
      if (user != null && user.Result != null)
        gitHubAccountName = user.Result.Login;
      return gitHubAccountName;
    }

    private static GitHubHttpClient GetGitHubHttpClient(IVssRequestContext requestContext)
    {
      IGitHubAppAccessTokenProvider extension = requestContext.GetExtension<IGitHubAppAccessTokenProvider>();
      if (extension != null)
        extension.Initialize((object) requestContext);
      else
        requestContext.Trace(ExternalProvidersTracePoints.LoadAppExtensionFailed, TraceLevel.Warning, "WebhookService", "Service", "Unable to load the {0} extension.", (object) "IGitHubAppAccessTokenProvider");
      return GitHubHttpClientFactory.Create(requestContext, extension);
    }

    private static GitHubAuthentication GetAuthenticationScheme(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> currentInputValues,
      Guid connectionId)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = ServiceEndpointHelper.GetServiceEndpoint(requestContext, projectId, connectionId);
      GitHubAuthentication hubAuthentication = serviceEndpoint.GetGitHubAuthentication(requestContext, projectId);
      return serviceEndpoint != null && hubAuthentication != null ? hubAuthentication : throw new InvalidRequestException("Could not connect to the service.");
    }

    private static bool GetInputValue(
      IDictionary<string, string> currentInputValues,
      string inputName,
      out string outputValue,
      out string errorMessage)
    {
      if (currentInputValues == null || currentInputValues.Count <= 0 || !currentInputValues.ContainsKey(inputName))
      {
        outputValue = string.Empty;
        errorMessage = "Endpoint Guid not present.";
        return false;
      }
      currentInputValues.TryGetValue(inputName, out outputValue);
      errorMessage = string.Empty;
      return true;
    }
  }
}
