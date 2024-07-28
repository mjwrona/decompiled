// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentJobRequestMessageUtil
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class AgentJobRequestMessageUtil
  {
    public static AgentJobRequestMessage Convert(Microsoft.TeamFoundation.DistributedTask.WebApi.AgentJobRequestMessage message)
    {
      List<JobStep> steps = new List<JobStep>();
      foreach (TaskInstance task in message.Tasks)
      {
        TaskStep taskStep = new TaskStep(task);
        steps.Add((JobStep) taskStep);
      }
      Dictionary<string, VariableValue> variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<MaskHint> maskHintSet = new HashSet<MaskHint>();
      JobResources jobResources = new JobResources();
      WorkspaceOptions workspaceOptions = new WorkspaceOptions();
      message.Environment.Extract(variables, maskHintSet, jobResources);
      if (string.Equals(message.Plan.PlanType, "Build", StringComparison.OrdinalIgnoreCase))
      {
        ServiceEndpoint serviceEndpoint = jobResources.Endpoints.FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Data.ContainsKey("repositoryId"))) ?? jobResources.Endpoints.FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Type == "Bitbucket" || x.Type == "Git" || x.Type == "TfsGit" || x.Type == "GitHub" || x.Type == "GitHubEnterprise" || x.Type == "TfsVersionControl"));
        if (serviceEndpoint != null)
        {
          TaskStep taskStep = new TaskStep();
          taskStep.Id = Guid.NewGuid();
          taskStep.DisplayName = PipelineConstants.CheckoutTask.FriendlyName;
          taskStep.Name = "__system_checkout";
          taskStep.Reference = new TaskStepDefinitionReference()
          {
            Id = PipelineConstants.CheckoutTask.Id,
            Name = PipelineConstants.CheckoutTask.Name,
            Version = (string) PipelineConstants.CheckoutTask.Version
          };
          taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.Repository] = "__legacy_repo_endpoint";
          RepositoryResource repositoryResource1 = new RepositoryResource();
          repositoryResource1.Alias = "__legacy_repo_endpoint";
          repositoryResource1.Properties.Set<string>(RepositoryPropertyNames.Name, serviceEndpoint.Name);
          string str1;
          serviceEndpoint.Data.TryGetValue("repositoryId", out str1);
          repositoryResource1.Id = string.IsNullOrEmpty(str1) ? "__legacy_repo_endpoint" : str1;
          RepositoryResource repositoryResource2 = repositoryResource1;
          ServiceEndpointReference endpointReference = new ServiceEndpointReference();
          endpointReference.Id = Guid.Empty;
          endpointReference.Name = (ExpressionValue<string>) serviceEndpoint.Name;
          repositoryResource2.Endpoint = endpointReference;
          repositoryResource1.Type = AgentJobRequestMessageUtil.ConvertLegacySourceType(serviceEndpoint.Type);
          repositoryResource1.Url = serviceEndpoint.Url;
          VariableValue variableValue1;
          if (variables.TryGetValue("build.sourceVersion", out variableValue1) && !string.IsNullOrEmpty(variableValue1?.Value))
            repositoryResource1.Version = variableValue1.Value;
          VariableValue variableValue2;
          if (variables.TryGetValue("build.sourceBranch", out variableValue2) && !string.IsNullOrEmpty(variableValue2?.Value))
            repositoryResource1.Properties.Set<string>(RepositoryPropertyNames.Ref, variableValue2.Value);
          VersionInfo versionInfo = (VersionInfo) null;
          VariableValue variableValue3;
          if (variables.TryGetValue("build.sourceVersionAuthor", out variableValue3) && !string.IsNullOrEmpty(variableValue3?.Value))
          {
            versionInfo = new VersionInfo();
            versionInfo.Author = variableValue3.Value;
          }
          VariableValue variableValue4;
          if (variables.TryGetValue("build.sourceVersionMessage", out variableValue4) && !string.IsNullOrEmpty(variableValue4?.Value))
          {
            if (versionInfo == null)
              versionInfo = new VersionInfo();
            versionInfo.Message = variableValue4.Value;
          }
          if (versionInfo != null)
            repositoryResource1.Properties.Set<VersionInfo>(RepositoryPropertyNames.VersionInfo, versionInfo);
          if (repositoryResource1.Type == RepositoryTypes.Tfvc)
          {
            VariableValue variableValue5;
            if (variables.TryGetValue("build.sourceTfvcShelveset", out variableValue5) && !string.IsNullOrEmpty(variableValue5?.Value))
              repositoryResource1.Properties.Set<string>(RepositoryPropertyNames.Shelveset, variableValue5.Value);
            AgentJobRequestMessageUtil.LegacyBuildWorkspace legacyBuildWorkspace = JsonUtility.FromString<AgentJobRequestMessageUtil.LegacyBuildWorkspace>(serviceEndpoint.Data["tfvcWorkspaceMapping"]);
            if (legacyBuildWorkspace != null)
            {
              IList<WorkspaceMapping> workspaceMappingList = (IList<WorkspaceMapping>) new List<WorkspaceMapping>();
              foreach (AgentJobRequestMessageUtil.LegacyMappingDetails mapping in legacyBuildWorkspace.Mappings)
                workspaceMappingList.Add(new WorkspaceMapping()
                {
                  ServerPath = mapping.ServerPath,
                  LocalPath = mapping.LocalPath,
                  Exclude = string.Equals(mapping.MappingType, "cloak", StringComparison.OrdinalIgnoreCase)
                });
              repositoryResource1.Properties.Set<IList<WorkspaceMapping>>(RepositoryPropertyNames.Mappings, workspaceMappingList);
            }
          }
          else if (repositoryResource1.Type == RepositoryTypes.Svn)
          {
            AgentJobRequestMessageUtil.LegacySvnWorkspace legacySvnWorkspace = JsonUtility.FromString<AgentJobRequestMessageUtil.LegacySvnWorkspace>(serviceEndpoint.Data["svnWorkspaceMapping"]);
            if (legacySvnWorkspace != null)
            {
              IList<WorkspaceMapping> workspaceMappingList = (IList<WorkspaceMapping>) new List<WorkspaceMapping>();
              foreach (AgentJobRequestMessageUtil.LegacySvnMappingDetails mapping in legacySvnWorkspace.Mappings)
                workspaceMappingList.Add(new WorkspaceMapping()
                {
                  ServerPath = mapping.ServerPath,
                  LocalPath = mapping.LocalPath,
                  Depth = mapping.Depth,
                  IgnoreExternals = mapping.IgnoreExternals,
                  Revision = mapping.Revision
                });
              repositoryResource1.Properties.Set<IList<WorkspaceMapping>>(RepositoryPropertyNames.Mappings, workspaceMappingList);
            }
          }
          string str2;
          serviceEndpoint.Data.TryGetValue("clean", out str2);
          taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.Clean] = string.IsNullOrEmpty(str2) ? bool.FalseString : str2;
          string str3;
          bool result1;
          if (((!serviceEndpoint.Data.TryGetValue("checkoutSubmodules", out str3) ? 0 : (bool.TryParse(str3, out result1) ? 1 : 0)) & (result1 ? 1 : 0)) != 0)
          {
            string str4;
            bool result2;
            int num1 = !serviceEndpoint.Data.TryGetValue("checkoutNestedSubmodules", out str4) ? 0 : (bool.TryParse(str4, out result2) ? 1 : 0);
            int num2 = result2 ? 1 : 0;
            taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.Submodules] = (num1 & num2) == 0 ? PipelineConstants.CheckoutTaskInputs.SubmodulesOptions.True : PipelineConstants.CheckoutTaskInputs.SubmodulesOptions.Recursive;
          }
          if (serviceEndpoint.Data.ContainsKey("fetchDepth"))
            taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.FetchDepth] = serviceEndpoint.Data["fetchDepth"];
          if (serviceEndpoint.Data.ContainsKey("gitLfsSupport"))
            taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.Lfs] = serviceEndpoint.Data["gitLfsSupport"];
          if (serviceEndpoint.Data.ContainsKey("fetchTags"))
            taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.FetchTags] = serviceEndpoint.Data["fetchTags"];
          if (VariableUtility.GetEnableAccessTokenType((IDictionary<string, VariableValue>) variables) == EnableAccessTokenType.Variable)
            taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.PersistCredentials] = bool.TrueString;
          bool result3;
          string a;
          if (bool.TryParse(str2, out result3) & result3 && serviceEndpoint.Data.TryGetValue("cleanOptions", out a) && !string.IsNullOrEmpty(a))
          {
            if (string.Equals(a, "1", StringComparison.OrdinalIgnoreCase))
              workspaceOptions.Clean = PipelineConstants.WorkspaceCleanOptions.Outputs;
            else if (string.Equals(a, "2", StringComparison.OrdinalIgnoreCase))
              workspaceOptions.Clean = PipelineConstants.WorkspaceCleanOptions.Resources;
            else if (string.Equals(a, "3", StringComparison.OrdinalIgnoreCase))
              workspaceOptions.Clean = PipelineConstants.WorkspaceCleanOptions.All;
          }
          VariableValue variableValue6;
          variables.TryGetValue("build.syncSources", out variableValue6);
          string str5;
          serviceEndpoint.Data.TryGetValue("skipSyncSource", out str5);
          bool result4;
          if (!string.IsNullOrEmpty(variableValue6?.Value) && bool.TryParse(variableValue6?.Value, out result4) && !result4)
          {
            taskStep.Condition = bool.FalseString;
          }
          else
          {
            bool result5;
            if (bool.TryParse(str5, out result5) & result5)
              taskStep.Condition = bool.FalseString;
          }
          steps.Insert(0, (JobStep) taskStep);
          jobResources.Repositories.Add(repositoryResource1);
        }
      }
      return new AgentJobRequestMessage(message.Plan, message.Timeline, message.JobId, message.JobName, message.JobRefName, (string) null, (IDictionary<string, string>) null, (IDictionary<string, VariableValue>) variables, (IList<MaskHint>) maskHintSet.ToList<MaskHint>(), jobResources, workspaceOptions, (IEnumerable<JobStep>) steps)
      {
        RequestId = message.RequestId
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AgentJobRequestMessage Convert(
      AgentJobRequestMessage message)
    {
      IDictionary<string, string> dictionary = string.IsNullOrEmpty(message.JobContainer) ? message.JobSidecarContainers : throw new NotSupportedException(message.JobContainer);
      if ((dictionary != null ? (dictionary.Count > 0 ? 1 : 0) : 0) != 0)
        throw new NotSupportedException(string.Join(", ", (IEnumerable<string>) message.JobSidecarContainers.Keys));
      if (message.Resources.Repositories.Count > 1)
        throw new NotSupportedException(string.Join(", ", message.Resources.Repositories.Select<RepositoryResource, string>((Func<RepositoryResource, string>) (x => x.Alias))));
      if (message.Steps.Where<JobStep>((Func<JobStep, bool>) (x => x.IsCheckoutTask())).Count<JobStep>() > 1)
        throw new NotSupportedException(PipelineConstants.CheckoutTask.Id.ToString("D"));
      List<TaskInstance> tasks = new List<TaskInstance>();
      foreach (JobStep step in (IEnumerable<JobStep>) message.Steps)
      {
        if (step.Type != StepType.Task)
          throw new NotSupportedException(step.Type.ToString());
        if (!step.IsCheckoutTask())
        {
          TaskInstance legacyTaskInstance = (step as TaskStep).ToLegacyTaskInstance();
          tasks.Add(legacyTaskInstance);
        }
      }
      if (message.Resources != null)
      {
        foreach (ServiceEndpoint endpoint in message.Resources.Endpoints)
        {
          if (!string.Equals(endpoint.Name, "SystemVssConnection", StringComparison.OrdinalIgnoreCase) && endpoint.Id != Guid.Empty)
            endpoint.Name = endpoint.Id.ToString("D");
        }
        foreach (SecureFile secureFile in message.Resources.SecureFiles)
        {
          if (!string.IsNullOrEmpty(secureFile.Ticket))
            message.MaskHints.Add(new MaskHint()
            {
              Type = MaskType.Regex,
              Value = Regex.Escape(secureFile.Ticket)
            });
        }
      }
      if (string.Equals(message.Plan.PlanType, "Build", StringComparison.OrdinalIgnoreCase))
      {
        JobResources resources = message.Resources;
        RepositoryResource repoResource = resources != null ? resources.Repositories.SingleOrDefault<RepositoryResource>() : (RepositoryResource) null;
        if (repoResource != null)
        {
          ServiceEndpoint serviceEndpoint1 = new ServiceEndpoint();
          serviceEndpoint1.Name = repoResource.Properties.Get<string>(RepositoryPropertyNames.Name);
          serviceEndpoint1.Type = AgentJobRequestMessageUtil.ConvertToLegacySourceType(repoResource.Type);
          serviceEndpoint1.Url = repoResource.Url;
          if (repoResource.Endpoint != null)
          {
            ServiceEndpoint serviceEndpoint2 = message.Resources.Endpoints.First<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x =>
            {
              if (x.Id == repoResource.Endpoint.Id && x.Id != Guid.Empty)
                return true;
              return string.Equals(x.Name, repoResource.Endpoint.Name?.Literal, StringComparison.OrdinalIgnoreCase) && x.Id == Guid.Empty && repoResource.Endpoint.Id == Guid.Empty;
            }));
            EndpointAuthorization endpointAuthorization = serviceEndpoint2.Authorization?.Clone();
            if (endpointAuthorization != null)
            {
              if (endpointAuthorization.Scheme == "Token")
              {
                string str1;
                if (serviceEndpoint2.Authorization.Parameters.TryGetValue("AccessToken", out str1))
                {
                  serviceEndpoint1.Authorization = new EndpointAuthorization()
                  {
                    Scheme = "UsernamePassword",
                    Parameters = {
                      {
                        "Username",
                        "x-access-token"
                      },
                      {
                        "Password",
                        str1
                      }
                    }
                  };
                }
                else
                {
                  string str2;
                  if (serviceEndpoint2.Authorization.Parameters.TryGetValue("ApiToken", out str2))
                    serviceEndpoint1.Authorization = new EndpointAuthorization()
                    {
                      Scheme = "UsernamePassword",
                      Parameters = {
                        {
                          "Username",
                          str2
                        },
                        {
                          "Password",
                          "x-oauth-basic"
                        }
                      }
                    };
                }
              }
              else if (endpointAuthorization.Scheme == "PersonalAccessToken")
              {
                string str;
                if (serviceEndpoint2.Authorization.Parameters.TryGetValue("AccessToken", out str))
                  serviceEndpoint1.Authorization = new EndpointAuthorization()
                  {
                    Scheme = "UsernamePassword",
                    Parameters = {
                      {
                        "Username",
                        "pat"
                      },
                      {
                        "Password",
                        str
                      }
                    }
                  };
              }
              else
                serviceEndpoint1.Authorization = endpointAuthorization;
            }
            string str3;
            if (serviceEndpoint2.Data.TryGetValue("acceptUntrustedCerts", out str3))
              serviceEndpoint1.Data["acceptUntrustedCerts"] = str3;
            string str4;
            if (serviceEndpoint2.Data.TryGetValue("realmName", out str4))
              serviceEndpoint1.Data["realmName"] = str4;
          }
          serviceEndpoint1.Data["repositoryId"] = repoResource.Id;
          serviceEndpoint1.Data["clean"] = bool.FalseString;
          serviceEndpoint1.Data["checkoutSubmodules"] = bool.FalseString;
          serviceEndpoint1.Data["checkoutNestedSubmodules"] = bool.FalseString;
          serviceEndpoint1.Data["fetchDepth"] = "0";
          serviceEndpoint1.Data["gitLfsSupport"] = bool.FalseString;
          serviceEndpoint1.Data["skipSyncSource"] = bool.FalseString;
          serviceEndpoint1.Data["cleanOptions"] = "0";
          serviceEndpoint1.Data["rootFolder"] = (string) null;
          if (repoResource.Type == RepositoryTypes.Tfvc)
          {
            IList<WorkspaceMapping> workspaceMappingList = repoResource.Properties.Get<IList<WorkspaceMapping>>(RepositoryPropertyNames.Mappings);
            if (workspaceMappingList != null)
            {
              AgentJobRequestMessageUtil.LegacyBuildWorkspace toSerialize = new AgentJobRequestMessageUtil.LegacyBuildWorkspace();
              foreach (WorkspaceMapping workspaceMapping in (IEnumerable<WorkspaceMapping>) workspaceMappingList)
                toSerialize.Mappings.Add(new AgentJobRequestMessageUtil.LegacyMappingDetails()
                {
                  ServerPath = workspaceMapping.ServerPath,
                  LocalPath = workspaceMapping.LocalPath,
                  MappingType = workspaceMapping.Exclude ? "cloak" : "map"
                });
              serviceEndpoint1.Data["tfvcWorkspaceMapping"] = JsonUtility.ToString((object) toSerialize);
            }
          }
          else if (repoResource.Type == RepositoryTypes.Svn)
          {
            IList<WorkspaceMapping> workspaceMappingList = repoResource.Properties.Get<IList<WorkspaceMapping>>(RepositoryPropertyNames.Mappings);
            if (workspaceMappingList != null)
            {
              AgentJobRequestMessageUtil.LegacySvnWorkspace toSerialize = new AgentJobRequestMessageUtil.LegacySvnWorkspace();
              foreach (WorkspaceMapping workspaceMapping in (IEnumerable<WorkspaceMapping>) workspaceMappingList)
                toSerialize.Mappings.Add(new AgentJobRequestMessageUtil.LegacySvnMappingDetails()
                {
                  ServerPath = workspaceMapping.ServerPath,
                  LocalPath = workspaceMapping.LocalPath,
                  Depth = workspaceMapping.Depth,
                  IgnoreExternals = workspaceMapping.IgnoreExternals,
                  Revision = workspaceMapping.Revision
                });
              serviceEndpoint1.Data["svnWorkspaceMapping"] = JsonUtility.ToString((object) toSerialize);
            }
          }
          else if (repoResource.Type == RepositoryTypes.Git)
          {
            VariableValue variableValue;
            serviceEndpoint1.Data["onpremtfsgit"] = !message.Variables.TryGetValue(WellKnownDistributedTaskVariables.ServerType, out variableValue) || !string.Equals(variableValue?.Value, "Hosted", StringComparison.OrdinalIgnoreCase) ? bool.TrueString : bool.FalseString;
          }
          if (!message.Variables.ContainsKey("build.repository.id") || string.IsNullOrEmpty(message.Variables["build.repository.id"]?.Value))
            message.Variables["build.repository.id"] = (VariableValue) repoResource.Id;
          if (!message.Variables.ContainsKey("build.repository.name") || string.IsNullOrEmpty(message.Variables["build.repository.name"]?.Value))
            message.Variables["build.repository.name"] = (VariableValue) repoResource.Properties.Get<string>(RepositoryPropertyNames.Name);
          if (!message.Variables.ContainsKey("build.repository.uri") || string.IsNullOrEmpty(message.Variables["build.repository.uri"]?.Value))
            message.Variables["build.repository.uri"] = (VariableValue) repoResource.Url.AbsoluteUri;
          VersionInfo versionInfo = repoResource.Properties.Get<VersionInfo>(RepositoryPropertyNames.VersionInfo);
          if (!message.Variables.ContainsKey("build.sourceVersionAuthor") || string.IsNullOrEmpty(message.Variables["build.sourceVersionAuthor"]?.Value))
            message.Variables["build.sourceVersionAuthor"] = (VariableValue) versionInfo?.Author;
          if (!message.Variables.ContainsKey("build.sourceVersionMessage") || string.IsNullOrEmpty(message.Variables["build.sourceVersionMessage"]?.Value))
            message.Variables["build.sourceVersionMessage"] = (VariableValue) versionInfo?.Message;
          if (!message.Variables.ContainsKey("build.sourceVersion") || string.IsNullOrEmpty(message.Variables["build.sourceVersion"]?.Value))
            message.Variables["build.sourceVersion"] = (VariableValue) repoResource.Version;
          if (!message.Variables.ContainsKey("build.sourceBranch") || string.IsNullOrEmpty(message.Variables["build.sourceBranch"]?.Value))
            message.Variables["build.sourceBranch"] = (VariableValue) repoResource.Properties.Get<string>(RepositoryPropertyNames.Ref);
          if (repoResource.Type == RepositoryTypes.Tfvc)
          {
            string str = repoResource.Properties.Get<string>(RepositoryPropertyNames.Shelveset);
            if (!string.IsNullOrEmpty(str) && (!message.Variables.ContainsKey("build.sourceTfvcShelveset") || string.IsNullOrEmpty(message.Variables["build.sourceTfvcShelveset"]?.Value)))
              message.Variables["build.sourceTfvcShelveset"] = (VariableValue) str;
          }
          if (message.Steps.FirstOrDefault<JobStep>((Func<JobStep, bool>) (x => x.IsCheckoutTask())) is TaskStep taskStep)
          {
            string str5;
            serviceEndpoint1.Data["clean"] = !taskStep.Inputs.TryGetValue(PipelineConstants.CheckoutTaskInputs.Clean, out str5) || string.IsNullOrEmpty(str5) ? bool.FalseString : str5;
            string a;
            if (taskStep.Inputs.TryGetValue(PipelineConstants.CheckoutTaskInputs.Submodules, out a) && !string.IsNullOrEmpty(a))
            {
              serviceEndpoint1.Data["checkoutSubmodules"] = bool.TrueString;
              if (string.Equals(a, PipelineConstants.CheckoutTaskInputs.SubmodulesOptions.Recursive, StringComparison.OrdinalIgnoreCase))
                serviceEndpoint1.Data["checkoutNestedSubmodules"] = bool.TrueString;
            }
            string str6;
            if (taskStep.Inputs.TryGetValue(PipelineConstants.CheckoutTaskInputs.FetchDepth, out str6) && !string.IsNullOrEmpty(str6))
              serviceEndpoint1.Data["fetchDepth"] = str6;
            string str7;
            if (taskStep.Inputs.TryGetValue(PipelineConstants.CheckoutTaskInputs.FetchTags, out str7) && !string.IsNullOrEmpty(str7))
              serviceEndpoint1.Data["fetchTags"] = str7;
            string str8;
            if (taskStep.Inputs.TryGetValue(PipelineConstants.CheckoutTaskInputs.Lfs, out str8) && !string.IsNullOrEmpty(str8))
              serviceEndpoint1.Data["gitLfsSupport"] = str8;
            if (string.Equals(taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.Repository], PipelineConstants.NoneAlias, StringComparison.OrdinalIgnoreCase))
              serviceEndpoint1.Data["skipSyncSource"] = bool.TrueString;
            else if (string.Equals(taskStep.Inputs[PipelineConstants.CheckoutTaskInputs.Repository], PipelineConstants.DesignerRepo, StringComparison.OrdinalIgnoreCase) && taskStep.Condition == bool.FalseString)
              serviceEndpoint1.Data["skipSyncSource"] = bool.TrueString;
          }
          serviceEndpoint1.Data["cleanOptions"] = "0";
          if (message.Workspace != null)
          {
            if (string.Equals(message.Workspace.Clean, PipelineConstants.WorkspaceCleanOptions.Outputs, StringComparison.OrdinalIgnoreCase))
              serviceEndpoint1.Data["cleanOptions"] = "1";
            else if (string.Equals(message.Workspace.Clean, PipelineConstants.WorkspaceCleanOptions.Resources, StringComparison.OrdinalIgnoreCase))
              serviceEndpoint1.Data["cleanOptions"] = "2";
            else if (string.Equals(message.Workspace.Clean, PipelineConstants.WorkspaceCleanOptions.All, StringComparison.OrdinalIgnoreCase))
              serviceEndpoint1.Data["cleanOptions"] = "3";
          }
          message.Resources.Endpoints.Add(serviceEndpoint1);
        }
      }
      JobEnvironment environment = new JobEnvironment(message.Variables, message.MaskHints, message.Resources);
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.AgentJobRequestMessage(message.Plan, message.Timeline, message.JobId, message.JobDisplayName, message.JobName, environment, (IEnumerable<TaskInstance>) tasks)
      {
        RequestId = message.RequestId
      };
    }

    private static string ConvertLegacySourceType(string legacySourceType)
    {
      if (string.Equals(legacySourceType, "Bitbucket", StringComparison.OrdinalIgnoreCase))
        return RepositoryTypes.Bitbucket;
      if (string.Equals(legacySourceType, "Git", StringComparison.OrdinalIgnoreCase))
        return RepositoryTypes.ExternalGit;
      if (string.Equals(legacySourceType, "TfsGit", StringComparison.OrdinalIgnoreCase))
        return RepositoryTypes.Git;
      if (string.Equals(legacySourceType, "GitHub", StringComparison.OrdinalIgnoreCase))
        return RepositoryTypes.GitHub;
      if (string.Equals(legacySourceType, "GitHubEnterprise", StringComparison.OrdinalIgnoreCase))
        return RepositoryTypes.GitHubEnterprise;
      if (string.Equals(legacySourceType, "Svn", StringComparison.OrdinalIgnoreCase))
        return RepositoryTypes.Svn;
      if (string.Equals(legacySourceType, "TfsVersionControl", StringComparison.OrdinalIgnoreCase))
        return RepositoryTypes.Tfvc;
      throw new NotSupportedException(legacySourceType);
    }

    private static string ConvertToLegacySourceType(string pipelineSourceType)
    {
      if (string.Equals(pipelineSourceType, RepositoryTypes.Bitbucket, StringComparison.OrdinalIgnoreCase))
        return "Bitbucket";
      if (string.Equals(pipelineSourceType, RepositoryTypes.ExternalGit, StringComparison.OrdinalIgnoreCase))
        return "Git";
      if (string.Equals(pipelineSourceType, RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase))
        return "TfsGit";
      if (string.Equals(pipelineSourceType, RepositoryTypes.GitHub, StringComparison.OrdinalIgnoreCase))
        return "GitHub";
      if (string.Equals(pipelineSourceType, RepositoryTypes.GitHubEnterprise, StringComparison.OrdinalIgnoreCase))
        return "GitHubEnterprise";
      if (string.Equals(pipelineSourceType, RepositoryTypes.Svn, StringComparison.OrdinalIgnoreCase))
        return "Svn";
      if (string.Equals(pipelineSourceType, RepositoryTypes.Tfvc, StringComparison.OrdinalIgnoreCase))
        return "TfsVersionControl";
      throw new NotSupportedException(pipelineSourceType);
    }

    private static class LegacyRepositoryTypes
    {
      public const string TfsVersionControl = "TfsVersionControl";
      public const string TfsGit = "TfsGit";
      public const string Git = "Git";
      public const string GitHub = "GitHub";
      public const string GitHubEnterprise = "GitHubEnterprise";
      public const string Bitbucket = "Bitbucket";
      public const string Svn = "Svn";
    }

    [DataContract]
    private class LegacyMappingDetails
    {
      [DataMember(Name = "serverPath")]
      public string ServerPath { get; set; }

      [DataMember(Name = "mappingType")]
      public string MappingType { get; set; }

      [DataMember(Name = "localPath")]
      public string LocalPath { get; set; }
    }

    [DataContract]
    private class LegacyBuildWorkspace
    {
      [DataMember(Name = "mappings")]
      private List<AgentJobRequestMessageUtil.LegacyMappingDetails> m_mappings;

      public List<AgentJobRequestMessageUtil.LegacyMappingDetails> Mappings
      {
        get
        {
          if (this.m_mappings == null)
            this.m_mappings = new List<AgentJobRequestMessageUtil.LegacyMappingDetails>();
          return this.m_mappings;
        }
      }
    }

    [DataContract]
    private class LegacySvnMappingDetails
    {
      [DataMember(Name = "serverPath")]
      public string ServerPath { get; set; }

      [DataMember(Name = "localPath")]
      public string LocalPath { get; set; }

      [DataMember(Name = "revision")]
      public string Revision { get; set; }

      [DataMember(Name = "depth")]
      public int Depth { get; set; }

      [DataMember(Name = "ignoreExternals")]
      public bool IgnoreExternals { get; set; }
    }

    [DataContract]
    private class LegacySvnWorkspace
    {
      [DataMember(Name = "mappings")]
      private List<AgentJobRequestMessageUtil.LegacySvnMappingDetails> m_Mappings;

      public List<AgentJobRequestMessageUtil.LegacySvnMappingDetails> Mappings
      {
        get
        {
          if (this.m_Mappings == null)
            this.m_Mappings = new List<AgentJobRequestMessageUtil.LegacySvnMappingDetails>();
          return this.m_Mappings;
        }
      }
    }
  }
}
