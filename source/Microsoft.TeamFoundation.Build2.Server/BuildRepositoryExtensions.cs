// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRepositoryExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildRepositoryExtensions
  {
    internal static JobEndpoint ToJobEndpoint(
      this BuildRepository buildRepository,
      string tokenType = null)
    {
      JobEndpoint jobEndpoint = new JobEndpoint()
      {
        Name = buildRepository.Name,
        Type = buildRepository.Type,
        Url = buildRepository.Url.AbsoluteUri,
        Data = {
          {
            "repositoryId",
            buildRepository.Id
          },
          {
            "rootFolder",
            buildRepository.RootFolder
          },
          {
            "clean",
            buildRepository.Clean
          },
          {
            "checkoutSubmodules",
            buildRepository.CheckoutSubmodules.ToString()
          }
        }
      };
      string str1;
      if (buildRepository.Properties.TryGetValue("username", out str1) && !string.IsNullOrEmpty(str1))
      {
        string str2;
        if (!buildRepository.Properties.TryGetValue("password", out str2) || str2 == null)
          str2 = "";
        jobEndpoint.Data.Add("username", str1);
        jobEndpoint.Data.Add("password", str2);
        if (!string.IsNullOrEmpty(tokenType))
          jobEndpoint.Data.Add(nameof (tokenType), tokenType);
      }
      return jobEndpoint;
    }

    public static RepositoryResource ToRepositoryResource(
      this BuildRepository buildRepository,
      IVssRequestContext requestContext,
      string repositoryAlias,
      string sourceBranch = null,
      string sourceVersion = null,
      bool includeCheckoutOptions = false)
    {
      RepositoryResource repositoryResource = new RepositoryResource();
      repositoryResource.Alias = repositoryAlias;
      repositoryResource.Id = buildRepository.Id;
      RepositoryResource repository = repositoryResource;
      repository.Type = !string.Equals(buildRepository.Type, "Git", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(buildRepository.Type, "Bitbucket", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(buildRepository.Type, "GitHub", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(buildRepository.Type, "TfsGit", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(buildRepository.Type, "TfsVersionControl", StringComparison.OrdinalIgnoreCase) ? buildRepository.Type : Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Tfvc) : Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Git) : Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.GitHub) : Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Bitbucket) : Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.ExternalGit;
      Guid serviceEndpointId;
      if (buildRepository.TryGetServiceEndpointId(out serviceEndpointId))
        repository.Endpoint = new Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference()
        {
          Id = serviceEndpointId
        };
      if (!string.IsNullOrEmpty(sourceVersion))
        repository.Version = sourceVersion;
      requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, buildRepository.Type).SetProperties(requestContext, repository, buildRepository, sourceBranch, sourceVersion);
      if (includeCheckoutOptions)
        repository.Properties.Set<CheckoutOptions>(RepositoryPropertyNames.CheckoutOptions, buildRepository.ToCheckoutOptions());
      return repository;
    }

    public static TaskStep ToCheckoutTask(
      this BuildRepository buildRepository,
      IVssRequestContext requestContext,
      bool persistCredentials = false)
    {
      TaskStep taskStep = new TaskStep();
      taskStep.Enabled = true;
      taskStep.Reference = new TaskStepDefinitionReference()
      {
        Id = PipelineConstants.CheckoutTask.Id,
        Version = (string) PipelineConstants.CheckoutTask.Version,
        Name = PipelineConstants.CheckoutTask.Name
      };
      TaskStep checkoutTask = taskStep;
      checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Repository] = PipelineConstants.DesignerRepo;
      checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Clean] = !string.IsNullOrEmpty(buildRepository.Clean) ? buildRepository.Clean : bool.FalseString;
      if (string.Equals(buildRepository.Type, "Git", StringComparison.OrdinalIgnoreCase) || string.Equals(buildRepository.Type, "TfsGit", StringComparison.OrdinalIgnoreCase) || string.Equals(buildRepository.Type, "Bitbucket", StringComparison.OrdinalIgnoreCase) || string.Equals(buildRepository.Type, "GitHub", StringComparison.OrdinalIgnoreCase) || string.Equals(buildRepository.Type, "GitHubEnterprise", StringComparison.OrdinalIgnoreCase))
      {
        if (buildRepository.CheckoutSubmodules)
        {
          string str;
          bool result;
          int num1 = !buildRepository.Properties.TryGetValue("checkoutNestedSubmodules", out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0);
          int num2 = result ? 1 : 0;
          checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Submodules] = (num1 & num2) == 0 ? PipelineConstants.CheckoutTaskInputs.SubmodulesOptions.True : PipelineConstants.CheckoutTaskInputs.SubmodulesOptions.Recursive;
        }
        if (buildRepository.Properties.ContainsKey("fetchDepth"))
          checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.FetchDepth] = buildRepository.Properties["fetchDepth"];
        if (buildRepository.Properties.ContainsKey("gitLfsSupport"))
          checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Lfs] = buildRepository.Properties["gitLfsSupport"];
        string str1;
        if (buildRepository.Properties.TryGetValue("fetchTags", out str1) && !string.IsNullOrEmpty(str1))
          checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.FetchTags] = str1;
        else if (requestContext.IsFeatureEnabled("Build2.FetchTagsOffByDefault"))
          checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.FetchTags] = "false";
      }
      if (persistCredentials)
        checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.PersistCredentials] = persistCredentials.ToString();
      bool result1;
      if (((!buildRepository.Properties.ContainsKey("skipSyncSource") ? 0 : (bool.TryParse(buildRepository.Properties["skipSyncSource"], out result1) ? 1 : 0)) & (result1 ? 1 : 0)) != 0)
        checkoutTask.Condition = bool.FalseString;
      return checkoutTask;
    }

    public static CheckoutOptions ToCheckoutOptions(this BuildRepository buildRepository)
    {
      CheckoutOptions checkoutOptions = new CheckoutOptions();
      if (!string.IsNullOrEmpty(buildRepository.Clean))
        checkoutOptions.Clean = buildRepository.Clean;
      if (buildRepository.CheckoutSubmodules)
      {
        string str;
        bool result;
        int num1 = !buildRepository.Properties.TryGetValue("checkoutNestedSubmodules", out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0);
        int num2 = result ? 1 : 0;
        checkoutOptions.Submodules = (num1 & num2) == 0 ? PipelineConstants.CheckoutTaskInputs.SubmodulesOptions.True : PipelineConstants.CheckoutTaskInputs.SubmodulesOptions.Recursive;
      }
      string str1;
      if (buildRepository.Properties.TryGetValue("fetchDepth", out str1) && !string.IsNullOrEmpty(str1))
        checkoutOptions.FetchDepth = str1;
      string str2;
      if (buildRepository.Properties.TryGetValue("gitLfsSupport", out str2) && !string.IsNullOrEmpty(str2))
        checkoutOptions.Lfs = str2;
      string str3;
      if (buildRepository.Properties.TryGetValue("fetchTags", out str3) && !string.IsNullOrEmpty(str3))
        checkoutOptions.FetchTags = str3;
      return checkoutOptions;
    }

    public static Microsoft.TeamFoundation.DistributedTask.Pipelines.WorkspaceOptions ToWorkspaceOptions(
      this BuildRepository buildRepository)
    {
      Microsoft.TeamFoundation.DistributedTask.Pipelines.WorkspaceOptions workspaceOptions = (Microsoft.TeamFoundation.DistributedTask.Pipelines.WorkspaceOptions) null;
      string str;
      RepositoryCleanOptions result;
      if (buildRepository.Properties.TryGetValue("cleanOptions", out str) && Enum.TryParse<RepositoryCleanOptions>(str, out result) && result != RepositoryCleanOptions.Source)
      {
        workspaceOptions = new Microsoft.TeamFoundation.DistributedTask.Pipelines.WorkspaceOptions();
        switch (result)
        {
          case RepositoryCleanOptions.SourceAndOutputDir:
            workspaceOptions.Clean = PipelineConstants.WorkspaceCleanOptions.Outputs;
            break;
          case RepositoryCleanOptions.SourceDir:
            workspaceOptions.Clean = PipelineConstants.WorkspaceCleanOptions.Resources;
            break;
          case RepositoryCleanOptions.AllBuildDir:
            workspaceOptions.Clean = PipelineConstants.WorkspaceCleanOptions.All;
            break;
        }
      }
      return workspaceOptions;
    }

    public static ServiceEndpoint ToServiceEndpoint(
      this BuildRepository buildRepository,
      string tokenType = null,
      ServiceEndpoint originalEndpoint = null)
    {
      Dictionary<string, string> dictionary = originalEndpoint?.Data == null ? new Dictionary<string, string>() : new Dictionary<string, string>(originalEndpoint?.Data);
      dictionary.Add("repositoryId", buildRepository.Id);
      dictionary.Add("rootFolder", buildRepository.RootFolder);
      dictionary.Add("clean", buildRepository.Clean);
      dictionary.Add("checkoutSubmodules", buildRepository.CheckoutSubmodules.ToString());
      ServiceEndpoint serviceEndpoint = new ServiceEndpoint()
      {
        Name = buildRepository.Name,
        Type = buildRepository.Type,
        Url = buildRepository.Url,
        Data = (IDictionary<string, string>) dictionary
      };
      string str1;
      if (buildRepository.Properties.TryGetValue("username", out str1) && !string.IsNullOrEmpty(str1))
      {
        string str2;
        if (!buildRepository.Properties.TryGetValue("password", out str2) || str2 == null)
          str2 = "";
        if (string.IsNullOrEmpty(str2) && !string.IsNullOrEmpty(tokenType))
          str2 = tokenType;
        serviceEndpoint.Authorization = new EndpointAuthorization()
        {
          Scheme = "UsernamePassword",
          Parameters = {
            {
              "Username",
              str1
            },
            {
              "Password",
              str2
            }
          }
        };
      }
      return serviceEndpoint;
    }

    public static BuildResult GetLabelSourcesCondition(this BuildRepository repository)
    {
      BuildResult result = BuildResult.None;
      string str;
      if (repository != null && repository.Properties.TryGetValue("labelSources", out str))
        Enum.TryParse<BuildResult>(str, out result);
      return result;
    }

    public static string GetLabelSourcesFormat(this BuildRepository repository)
    {
      string str;
      return repository.Properties.TryGetValue("labelSourcesFormat", out str) ? str : (string) null;
    }

    public static bool TryGetServiceEndpointId(
      this BuildRepository repository,
      out Guid serviceEndpointId)
    {
      serviceEndpointId = Guid.Empty;
      string input;
      return repository != null && repository.Properties != null && repository.Properties.TryGetValue("connectedServiceId", out input) && Guid.TryParse(input, out serviceEndpointId);
    }

    public static void CopyPropertyToEndpointData(
      this BuildRepository repository,
      ServiceEndpoint endpoint,
      string propertyKey,
      string dataKey)
    {
      if (repository == null || endpoint == null || !repository.Properties.ContainsKey(propertyKey) || string.IsNullOrEmpty(repository.Properties[propertyKey]))
        return;
      endpoint.Data[dataKey] = repository.Properties[propertyKey];
    }

    public static void CopyFetchTagsPropertyToEndpointData(
      this BuildRepository repository,
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      if (endpoint == null)
        return;
      string str;
      if (repository.Properties.TryGetValue("fetchTags", out str) && !string.IsNullOrEmpty(str))
      {
        endpoint.Data["fetchTags"] = str;
      }
      else
      {
        if (!requestContext.IsFeatureEnabled("Build2.FetchTagsOffByDefault"))
          return;
        endpoint.Data["fetchTags"] = "false";
      }
    }

    public static bool HasTriggers(this BuildRepository repository)
    {
      if (repository == null)
        return false;
      string a1;
      if (repository.Properties.TryGetValue("hasCITrigger", out a1))
        return string.Equals(a1, "true", StringComparison.OrdinalIgnoreCase);
      string a2;
      return repository.Properties.TryGetValue("hasPRTrigger", out a2) && string.Equals(a2, "true", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsReportBuildStatusEnabled(this BuildRepository repository)
    {
      string str;
      bool result;
      return repository != null && ((!repository.Properties.TryGetValue("reportBuildStatus", out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0;
    }

    public static void FixBuildRepositoryType(this BuildRepository repository)
    {
      if (string.Equals(repository.Type, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.ExternalGit, StringComparison.OrdinalIgnoreCase))
        repository.Type = "Git";
      else if (string.Equals(repository.Type, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase))
      {
        repository.Type = "TfsGit";
      }
      else
      {
        if (!string.Equals(repository.Type, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Tfvc, StringComparison.OrdinalIgnoreCase))
          return;
        repository.Type = "TfsVersionControl";
      }
    }
  }
}
