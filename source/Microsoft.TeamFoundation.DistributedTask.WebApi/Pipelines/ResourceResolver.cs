// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public static class ResourceResolver
  {
    public const string c_secureFileInputType = "secureFile";
    private const string c_endpointInputTypePrefix = "connectedService:";

    public static IEnumerable<PipelineValidationError> ResolveResources(
      IPipelineContext context,
      string phaseName,
      BuildOptions options,
      PipelineResources referencedResources,
      PipelineResources unauthorizedResources,
      TaskStep step,
      TaskInputDefinition input,
      string inputAlias,
      string inputValue,
      bool throwOnFailure = false)
    {
      ArgumentUtility.CheckForNull<IPipelineContext>(context, nameof (context));
      ArgumentUtility.CheckStringForNullOrEmpty(phaseName, nameof (phaseName));
      ArgumentUtility.CheckForNull<BuildOptions>(options, nameof (options));
      ArgumentUtility.CheckForNull<PipelineResources>(referencedResources, nameof (referencedResources));
      ArgumentUtility.CheckForNull<TaskStep>(step, nameof (step));
      ArgumentUtility.CheckForNull<TaskInputDefinition>(input, nameof (input));
      ArgumentUtility.CheckForNull<string>(input.Name, "Name");
      if (string.IsNullOrEmpty(inputValue))
        return Enumerable.Empty<PipelineValidationError>();
      List<PipelineValidationError> errors = new List<PipelineValidationError>();
      if (input.InputType.StartsWith("connectedService:", StringComparison.OrdinalIgnoreCase))
      {
        List<string> values = new List<string>();
        string str1 = input.InputType.Remove(0, "connectedService:".Length);
        foreach (string str2 in ((IEnumerable<string>) inputValue.Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))))
        {
          string endpointId = context.ExpandVariables(str2);
          ServiceEndpoint endpoint = context.ResourceStore.GetEndpoint(endpointId);
          Guid id;
          if (endpoint != null)
          {
            id = endpoint.Id;
            endpointId = id.ToString();
          }
          referencedResources.AddEndpointReference(endpointId);
          if (options.ValidateResources)
          {
            if (endpoint == null)
            {
              ResourceResolver.ThrowOrAddError((IList<PipelineValidationError>) errors, throwOnFailure, PipelineStrings.ServiceEndpointNotFoundForInput((object) phaseName, (object) step.Name, (object) inputAlias, (object) endpointId), true);
              values.Add(endpointId);
              unauthorizedResources?.AddEndpointReference(endpointId);
            }
            else if (endpoint.IsDisabled)
            {
              ResourceResolver.ThrowOrAddError((IList<PipelineValidationError>) errors, throwOnFailure, PipelineStrings.ServiceEndpointDisabled((object) phaseName, (object) step.Name, (object) inputAlias, (object) endpointId), true);
              values.Add(endpointId);
              unauthorizedResources?.AddEndpointReference(endpointId);
            }
            else
            {
              if (!string.IsNullOrEmpty(str1))
              {
                List<string> list = ((IEnumerable<string>) str1.Split(new char[1]
                {
                  ':'
                }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())).ToList<string>();
                if (list.Count >= 1)
                {
                  string str3 = list[0];
                  if (!str3.Equals(endpoint.Type, StringComparison.OrdinalIgnoreCase))
                    ResourceResolver.ThrowOrAddError((IList<PipelineValidationError>) errors, throwOnFailure, PipelineStrings.StepInputEndpointTypeMismatch((object) phaseName, (object) step.Name, (object) inputAlias, (object) str3, (object) endpoint.Name, (object) endpoint.Type));
                  else if (list.Count > 1 && !string.IsNullOrEmpty(endpoint.Authorization?.Scheme))
                  {
                    string str4 = list[1];
                    List<string> stringList;
                    if (str4 == null)
                      stringList = (List<string>) null;
                    else
                      stringList = ((IEnumerable<string>) str4.Split(new char[1]
                      {
                        ','
                      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())).ToList<string>();
                    List<string> source = stringList;
                    // ISSUE: explicit non-virtual call
                    if (source != null && __nonvirtual (source.Count) > 0 && !source.Any<string>((Func<string, bool>) (x => x.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))))
                      ResourceResolver.ThrowOrAddError((IList<PipelineValidationError>) errors, throwOnFailure, PipelineStrings.StepInputEndpointAuthSchemeMismatch((object) phaseName, (object) step.Name, (object) inputAlias, (object) str3, (object) list[1], (object) endpoint.Name, (object) endpoint.Type, (object) endpoint.Authorization.Scheme));
                  }
                }
              }
              List<string> stringList1 = values;
              id = endpoint.Id;
              string str5 = id.ToString("D");
              stringList1.Add(str5);
            }
          }
          else
            values.Add(endpointId);
        }
        step.Inputs[input.Name] = string.Join(",", (IEnumerable<string>) values);
      }
      else if (input.InputType.Equals("secureFile", StringComparison.OrdinalIgnoreCase))
      {
        List<string> values = new List<string>();
        foreach (string str in ((IEnumerable<string>) inputValue.Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))))
        {
          string fileId = context.ExpandVariables(str);
          SecureFile file = context.ResourceStore.GetFile(fileId);
          if (file != null)
            fileId = file.Id.ToString();
          referencedResources.AddSecureFileReference(fileId);
          if (options.ValidateResources)
          {
            if (file == null)
            {
              ResourceResolver.ThrowOrAddError((IList<PipelineValidationError>) errors, throwOnFailure, PipelineStrings.SecureFileNotFoundForInput((object) phaseName, (object) step.Name, (object) inputAlias, (object) fileId), true);
              values.Add(fileId);
              unauthorizedResources?.AddSecureFileReference(fileId);
            }
            else
              values.Add(file.Id.ToString("D"));
          }
          else
            values.Add(fileId);
        }
        step.Inputs[input.Name] = string.Join(",", (IEnumerable<string>) values);
      }
      else if (input.InputType.Equals("repository", StringComparison.OrdinalIgnoreCase))
      {
        string str = ResourceResolver.ResolveRepositoryResource(context, options, referencedResources, unauthorizedResources, inputValue, throwOnFailure, errors);
        step.Inputs[input.Name] = str;
      }
      return (IEnumerable<PipelineValidationError>) errors;
    }

    private static void ThrowOrAddError(
      IList<PipelineValidationError> errors,
      bool throwOnFailure,
      string message,
      bool useResourceNotFoundException = false)
    {
      if (throwOnFailure)
      {
        if (useResourceNotFoundException)
          throw new ResourceNotFoundException(message);
        throw new PipelineValidationException(message);
      }
      errors.Add(new PipelineValidationError(message));
    }

    private static string ResolveRepositoryResource(
      IPipelineContext context,
      BuildOptions options,
      PipelineResources referencedResources,
      PipelineResources unauthorizedResources,
      string inputValue,
      bool throwOnFailure,
      List<PipelineValidationError> errors)
    {
      string str = inputValue;
      if (string.Equals(inputValue, PipelineConstants.NoneAlias))
      {
        RepositoryResource repository = context.ResourceStore.Repositories.Get(PipelineConstants.SelfAlias);
        if (repository != null)
          ResourceResolver.AddRepositoryResource(context, repository, options, referencedResources, unauthorizedResources, errors, throwOnFailure);
      }
      else
      {
        RepositoryResource repository = context.ResourceStore.Repositories.Get(inputValue);
        if (repository != null)
        {
          ResourceResolver.AddRepositoryResource(context, repository, options, referencedResources, unauthorizedResources, errors, throwOnFailure);
        }
        else
        {
          RepositoryResource newRepository;
          if (ResourceResolver.TryCreateInlineResource(context, inputValue, out newRepository))
          {
            ResourceResolver.AddRepositoryResource(context, newRepository, options, referencedResources, unauthorizedResources, errors, throwOnFailure);
            str = newRepository.Alias;
            context.ResourceStore.Repositories.Add(newRepository);
          }
          else if (options.ValidateResources)
            ResourceResolver.ThrowOrAddError((IList<PipelineValidationError>) errors, throwOnFailure, PipelineStrings.RepositoryResourceNotFound((object) inputValue), true);
        }
      }
      return str;
    }

    internal static bool TryCreateInlineResource(
      IPipelineContext context,
      string inputValue,
      out RepositoryResource newRepository)
    {
      newRepository = (RepositoryResource) null;
      if (context == null || string.IsNullOrEmpty(inputValue))
        return false;
      int length = inputValue.IndexOf("://", StringComparison.OrdinalIgnoreCase);
      if (length < 0)
        return false;
      string str = (string) null;
      string repoType = ResourceResolver.NormalizeRepositoryType(inputValue.Substring(0, length));
      int num = inputValue.IndexOf("@", length + "://".Length, StringComparison.OrdinalIgnoreCase);
      string repoName;
      if (num >= 0)
      {
        repoName = inputValue.Substring(length + "://".Length, num - length - "://".Length);
        str = inputValue.Substring(num + 1);
      }
      else
        repoName = inputValue.Substring(length + "://".Length);
      ref RepositoryResource local = ref newRepository;
      RepositoryResource repositoryResource = new RepositoryResource();
      repositoryResource.Alias = inputValue.Replace("$", "_");
      repositoryResource.Type = repoType;
      local = repositoryResource;
      newRepository.Properties.Set<string>(RepositoryPropertyNames.Name, repoName);
      if (!string.IsNullOrEmpty(str))
        newRepository.Properties.Set<string>(RepositoryPropertyNames.Ref, str);
      newRepository.Properties.Set<string>(RepositoryPropertyNames.Url, ResourceResolver.GetRepositoryUrl(context, repoType, repoName)?.AbsoluteUri);
      return true;
    }

    internal static string NormalizeRepositoryType(string repoType)
    {
      if (ResourceResolver.IsAzureRepos(repoType))
        repoType = RepositoryTypes.Git;
      else if (ResourceResolver.IsGitHub(repoType))
        repoType = RepositoryTypes.GitHub;
      else if (ResourceResolver.IsBitbucket(repoType))
        repoType = RepositoryTypes.Bitbucket;
      return repoType;
    }

    private static bool IsAzureRepos(string repoType) => string.Equals(repoType, RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase);

    private static bool IsGitHub(string repoType) => string.Equals(repoType, RepositoryTypes.GitHub, StringComparison.OrdinalIgnoreCase);

    private static bool IsBitbucket(string repoType) => string.Equals(repoType, RepositoryTypes.Bitbucket, StringComparison.OrdinalIgnoreCase);

    internal static Uri GetRepositoryUrl(
      IPipelineContext context,
      string repoType,
      string repoName)
    {
      if (string.IsNullOrEmpty(repoType) || string.IsNullOrEmpty(repoName))
        return (Uri) null;
      if (ResourceResolver.IsAzureRepos(repoType))
      {
        string str = context.Variables[WellKnownDistributedTaskVariables.CollectionUrl].Value.TrimEnd('/');
        string stringToEscape = context.Variables[WellKnownDistributedTaskVariables.TeamProject].Value;
        if (repoName.Contains("/"))
        {
          string[] strArray = repoName.Split('/');
          return new Uri(str + "/" + Uri.EscapeDataString(strArray[0]) + "/_git/" + Uri.EscapeDataString(strArray[1]));
        }
        return new Uri(str + "/" + Uri.EscapeDataString(stringToEscape) + "/_git/" + Uri.EscapeDataString(repoName));
      }
      if (ResourceResolver.IsGitHub(repoType) && repoName.Contains("/"))
      {
        string[] strArray = repoName.Split('/');
        return new Uri("https://github.com/" + Uri.EscapeDataString(strArray[0]) + "/" + Uri.EscapeDataString(strArray[1]) + ".git");
      }
      if (!ResourceResolver.IsBitbucket(repoType) || !repoName.Contains("/"))
        return (Uri) null;
      string[] strArray1 = repoName.Split('/');
      return new Uri("https://bitbucket.org/" + Uri.EscapeDataString(strArray1[0]) + "/" + Uri.EscapeDataString(strArray1[1]) + ".git");
    }

    private static void AddRepositoryResource(
      IPipelineContext context,
      RepositoryResource repository,
      BuildOptions options,
      PipelineResources referencedResources,
      PipelineResources unauthorizedResources,
      List<PipelineValidationError> errors,
      bool throwOnFailure)
    {
      referencedResources.Repositories.Add(repository);
      if (repository.Endpoint == null)
        return;
      referencedResources.AddEndpointReference(repository.Endpoint);
      if (!options.ValidateResources)
        return;
      ServiceEndpoint endpoint = context.ResourceStore.GetEndpoint(repository.Endpoint);
      if (endpoint == null)
      {
        ResourceResolver.ThrowOrAddError((IList<PipelineValidationError>) errors, throwOnFailure, PipelineStrings.ServiceEndpointNotFound((object) repository.Endpoint), true);
        unauthorizedResources?.AddEndpointReference(repository.Endpoint);
      }
      else
        repository.Endpoint = new ServiceEndpointReference()
        {
          Id = endpoint.Id
        };
    }
  }
}
