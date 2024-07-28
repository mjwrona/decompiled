// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub.GitHubHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub
{
  public class GitHubHelper
  {
    private readonly GitHubHelper.OutFuncGetCommit<GitHubAuthentication, string, string, string, GitHubData.V3.CommitListItem> getCommitFunc;
    private readonly GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.CommitListItem[]> getCommitsFunc;
    private readonly GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Branch[]> getBranchesFunc;
    private readonly GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Repository> getRepoFunc;
    private readonly GitHubHelper.OutFuncGetPagedRepo<GitHubAuthentication, bool, string, string, GitHubData.V3.Repository[]> getPagedReposFunc;
    private readonly GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Repository[]> searchReposFunc;
    private readonly GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.ContentData[]> getContentFunc;
    private readonly GitHubHelper.OutFuncGetFileContent<GitHubAuthentication, string, string, string, string, string> getFileContentFunc;
    private readonly Func<IVssRequestContext, Guid, Guid, ServiceEndpoint> serviceEndpointRetriever;
    private const string IsArtifactEditOperation = "isArtifactEditOperation";
    private const int CharactersToDisplayInCommitId = 9;
    [StaticSafe]
    private static readonly IDictionary<string, string> InputValueNotPresentErrorsDictionary = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "branch",
        Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BranchDetailsNotAvailable
      },
      {
        "connection",
        Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceEndPointIdNotPresent
      },
      {
        "definition",
        Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDetailsNotAvailable
      }
    };

    public static bool GetInputValue(
      IDictionary<string, string> currentInputValues,
      string inputName,
      out string outputValue,
      out string errorMessage)
    {
      if (!currentInputValues.IsNullOrEmpty<KeyValuePair<string, string>>() && currentInputValues.TryGetValue(inputName, out outputValue))
      {
        errorMessage = string.Empty;
        return true;
      }
      outputValue = string.Empty;
      errorMessage = GitHubHelper.InputValueNotPresentErrorsDictionary[inputName];
      return false;
    }

    public static IList<ReleaseTriggerBase> GetGitHubTriggers(
      ReleaseDefinition definition,
      bool includeBuildGitHubTriggers)
    {
      return (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>(GitHubHelper.GetGitHubPullRequestTriggers(definition, includeBuildGitHubTriggers).Concat<ReleaseTriggerBase>((IEnumerable<ReleaseTriggerBase>) GitHubHelper.GetGitHubSourceRepoTriggers(definition)));
    }

    public static IList<ReleaseTriggerBase> GetGitHubSourceRepoTriggers(ReleaseDefinition definition)
    {
      IList<ReleaseTriggerBase> sourceRepoTriggers = (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
      if (definition != null && definition.Triggers != null)
      {
        foreach (ReleaseTriggerBase trigger in (IEnumerable<ReleaseTriggerBase>) definition.Triggers)
        {
          if (trigger.TriggerType == ReleaseTriggerType.SourceRepo)
          {
            SourceRepoTrigger sourceRepoTrigger = (SourceRepoTrigger) trigger;
            ArtifactSource artifactSource = definition.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == sourceRepoTrigger.Alias)).SingleOrDefault<ArtifactSource>();
            if (artifactSource != null && artifactSource.IsGitHubArtifact)
              sourceRepoTriggers.Add(trigger);
          }
        }
      }
      return sourceRepoTriggers;
    }

    public static IList<ReleaseTriggerBase> GetGitHubPullRequestTriggers(
      ReleaseDefinition definition,
      bool includeBuildGitHubTriggers)
    {
      IList<ReleaseTriggerBase> pullRequestTriggers = (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
      if (definition != null && definition.Triggers != null)
      {
        foreach (ReleaseTriggerBase trigger in (IEnumerable<ReleaseTriggerBase>) definition.Triggers)
        {
          if (trigger.TriggerType == ReleaseTriggerType.PullRequest)
          {
            PullRequestTrigger pullRequestTrigger = (PullRequestTrigger) trigger;
            if (pullRequestTrigger != null)
            {
              ArtifactSource artifact = definition.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == pullRequestTrigger.ArtifactAlias)).SingleOrDefault<ArtifactSource>();
              if (artifact != null && (artifact.IsGitHubArtifact || includeBuildGitHubTriggers && GitHubHelper.IsBuildArtifactWithGitHubRepo(artifact, pullRequestTrigger)))
                pullRequestTriggers.Add((ReleaseTriggerBase) pullRequestTrigger);
            }
          }
        }
      }
      return pullRequestTriggers;
    }

    public static bool IsBuildArtifactWithGitHubRepo(
      ArtifactSource artifact,
      PullRequestTrigger pullRequestTrigger)
    {
      return artifact != null && artifact.IsBuildArtifact && pullRequestTrigger != null && pullRequestTrigger.PullRequestConfiguration != null && pullRequestTrigger.PullRequestConfiguration.CodeRepositoryReference != null && pullRequestTrigger.PullRequestConfiguration.CodeRepositoryReference.SystemType.Equals((object) PullRequestSystemType.GitHub);
    }

    public GitHubHelper()
      : this((GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Repository[]>) ((IVssRequestContext requestContext, GitHubAuthentication authentication, string criteria, out string errorMessage) => GitHubHelper.UnwrapGitHubResult<GitHubData.V3.Repository[]>(GitHubHelper.GetGitHubHttpClient(requestContext).GetUserReposByCriteria(authentication, criteria), out errorMessage)), (GitHubHelper.OutFuncGetPagedRepo<GitHubAuthentication, bool, string, string, GitHubData.V3.Repository[]>) ((IVssRequestContext requestContext, GitHubAuthentication authentication, bool fetchPaginatedResults, out string nextToken, out string errorMessage) => GitHubHelper.UnwrapGitHubResult<GitHubData.V3.Repository[]>(GitHubHelper.FetchRepositories(requestContext, authentication, fetchPaginatedResults, out nextToken), out errorMessage)), (GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Branch[]>) ((IVssRequestContext requestContext, GitHubAuthentication a, string b, out string c) => GitHubHelper.UnwrapGitHubResult<GitHubData.V3.Branch[]>(GitHubHelper.GetGitHubHttpClient(requestContext).GetRepoBranches(a, (string) null, b), out c)), (GitHubHelper.OutFuncGetCommit<GitHubAuthentication, string, string, string, GitHubData.V3.CommitListItem>) ((IVssRequestContext requestContext, GitHubAuthentication a, string b, string c, out string d) => GitHubHelper.UnwrapGitHubResult<GitHubData.V3.CommitListItem>(GitHubHelper.GetGitHubHttpClient(requestContext).GetCommit(a, b, c), out d)), (GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.CommitListItem[]>) ((IVssRequestContext requestContext, GitHubAuthentication a, string b, out string c) => GitHubHelper.UnwrapGitHubResult<GitHubData.V3.CommitListItem[]>(GitHubHelper.GetGitHubHttpClient(requestContext).GetCommits(a, b), out c)), (GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Repository>) ((IVssRequestContext requestContext, GitHubAuthentication a, string b, out string c) => GitHubHelper.UnwrapGitHubResult<GitHubData.V3.Repository>(GitHubHelper.GetGitHubHttpClient(requestContext).GetUserRepo(a, b), out c)), (GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.ContentData[]>) ((IVssRequestContext requestContext, GitHubAuthentication a, string b, out string c) => GitHubHelper.UnwrapGitHubResult<GitHubData.V3.ContentData[]>(GitHubHelper.GetGitHubHttpClient(requestContext).GetContent(a, b), out c)), (GitHubHelper.OutFuncGetFileContent<GitHubAuthentication, string, string, string, string, string>) ((IVssRequestContext requestContext, GitHubAuthentication a, string b, string c, string d, out string e) => GitHubHelper.UnwrapGitHubResult<string>(GitHubHelper.GetGitHubHttpClient(requestContext).GetFileContent(a, b, c, d), out e)), ServiceEndpointHelper.GetServiceEndpoint)
    {
    }

    private static GitHubResult<GitHubData.V3.Repository[]> FetchRepositories(
      IVssRequestContext requestContext,
      GitHubAuthentication authentication,
      bool fetchPaginatedResults,
      out string nextToken)
    {
      nextToken = (string) null;
      GitHubHelper.TraceFetchingRepositories(requestContext, authentication.Scheme.ToString(), authentication.InstallationId.ToString((IFormatProvider) CultureInfo.CurrentCulture));
      if (authentication.Scheme.Equals((object) GitHubAuthScheme.InstallationToken))
        return GitHubHelper.GetGitHubHttpClient(requestContext).GetInstallationRepos(authentication);
      if (authentication.Scheme == GitHubAuthScheme.ApplicationOAuthToken)
        return GitHubHelper.GetGitHubHttpClient(requestContext).GetUserInstallationRepositories(authentication, authentication.InstallationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return fetchPaginatedResults ? GitHubHelper.GetGitHubHttpClient(requestContext).GetFirstPageOfUserRepos(authentication, out nextToken) : GitHubHelper.GetGitHubHttpClient(requestContext).GetUserRepos(authentication);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void TraceFetchingRepositories(
      IVssRequestContext requestContext,
      string scheme,
      string id)
    {
      requestContext.Trace(1976491, TraceLevel.Verbose, "ArtifactExtensions", "ReleaseManagementService", string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.FetchingRepositoriesTraceMessage, (object) scheme, (object) id));
    }

    public List<InputValue> FetchRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      out string errorMessage)
    {
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(requestContext, projectId, endpointId);
      GitHubAuthentication hubAuthentication = serviceEndpoint.GetGitHubAuthentication(requestContext, projectId);
      GitHubHelper.TraceFetchingRepositories(requestContext, serviceEndpoint.Authorization.Scheme, endpointId.ToString());
      GitHubData.V3.Repository[] source = serviceEndpoint != null && hubAuthentication != null ? GitHubHelper.UnwrapGitHubResult<GitHubData.V3.Repository[]>(GitHubHelper.FetchRepositories(requestContext, hubAuthentication, false, out string _), out errorMessage) : throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProblemConnectingService);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.Repository>) source).Select<GitHubData.V3.Repository, InputValue>((Func<GitHubData.V3.Repository, InputValue>) (repo =>
      {
        InputValue inputValue = new InputValue()
        {
          DisplayValue = repo.Full_name,
          Value = repo.Full_name,
          Data = (IDictionary<string, object>) new Dictionary<string, object>()
        };
        inputValue.Data.Add("gitHubRepositoryId", (object) repo.Id);
        return inputValue;
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    protected GitHubHelper(
      GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Repository[]> searchReposFunc,
      GitHubHelper.OutFuncGetPagedRepo<GitHubAuthentication, bool, string, string, GitHubData.V3.Repository[]> getPagedReposFunc,
      GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Branch[]> getBranchesFunc,
      GitHubHelper.OutFuncGetCommit<GitHubAuthentication, string, string, string, GitHubData.V3.CommitListItem> getCommitFunc,
      GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.CommitListItem[]> getCommitsFunc,
      GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.Repository> getRepoFunc,
      GitHubHelper.OutFunc<GitHubAuthentication, string, string, GitHubData.V3.ContentData[]> getContentFunc,
      GitHubHelper.OutFuncGetFileContent<GitHubAuthentication, string, string, string, string, string> getFileContentFunc,
      Func<IVssRequestContext, Guid, Guid, ServiceEndpoint> serviceEndpointRetriever)
    {
      this.searchReposFunc = searchReposFunc;
      this.getPagedReposFunc = getPagedReposFunc;
      this.getBranchesFunc = getBranchesFunc;
      this.getCommitFunc = getCommitFunc;
      this.getCommitsFunc = getCommitsFunc;
      this.getRepoFunc = getRepoFunc;
      this.getContentFunc = getContentFunc;
      this.getFileContentFunc = getFileContentFunc;
      this.serviceEndpointRetriever = serviceEndpointRetriever;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to pass the error message to the caller")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    public IList<InputValue> GetInputValues(
      IVssRequestContext requestContext,
      Guid projectId,
      string inputId,
      IDictionary<string, string> currentInputValues,
      out string errorMessage)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (currentInputValues == null)
        throw new ArgumentNullException(nameof (currentInputValues));
      List<InputValue> inputValues = new List<InputValue>();
      errorMessage = (string) null;
      try
      {
        GitHubAuthentication authenticationScheme = this.GetAuthenticationScheme(requestContext, projectId, currentInputValues);
        if (inputId != null)
        {
          switch (inputId.Length)
          {
            case 6:
              if (inputId == "branch")
              {
                string outputValue;
                if (GitHubHelper.GetInputValue(currentInputValues, "definition", out outputValue, out errorMessage))
                {
                  inputValues = outputValue == null ? new List<InputValue>() : this.GetRepoBranches(requestContext, authenticationScheme, outputValue, out errorMessage);
                  goto label_60;
                }
                else
                  goto label_60;
              }
              else
                goto label_57;
            case 7:
              if (inputId == "commits")
              {
                string outputValue1;
                if (GitHubHelper.GetInputValue(currentInputValues, "definition", out outputValue1, out errorMessage))
                {
                  string outputValue2;
                  if (GitHubHelper.GetInputValue(currentInputValues, "branch", out outputValue2, out errorMessage))
                  {
                    string a;
                    currentInputValues.TryGetValue("defaultVersionType", out a);
                    GitHubData.V3.Repository repository = this.getRepoFunc(requestContext, authenticationScheme, outputValue1, out errorMessage);
                    if (string.Equals(a, "specificVersionType", StringComparison.OrdinalIgnoreCase))
                    {
                      string sha;
                      if (!currentInputValues.TryGetValue("defaultVersionSpecific", out sha) || string.IsNullOrEmpty(sha))
                        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoSpecificVersionValueAvailableForSpecificVersionType));
                      inputValues = new List<InputValue>();
                      inputValues.Add(repository == null ? (InputValue) null : this.GetCommit(requestContext, authenticationScheme, repository.Commits_url.Replace("/commits{/sha}", string.Empty), sha, outputValue2, out errorMessage));
                      goto label_60;
                    }
                    else
                    {
                      inputValues = repository == null ? new List<InputValue>() : this.GetBranchCommits(requestContext, authenticationScheme, repository.Commits_url.Replace("{/sha}", string.Empty), outputValue2, false, out errorMessage);
                      goto label_60;
                    }
                  }
                  else
                    goto label_60;
                }
                else
                  goto label_60;
              }
              else
                goto label_57;
            case 9:
              if (inputId == "artifacts")
                break;
              goto label_57;
            case 10:
              if (inputId == "definition")
              {
                if (GitHubHelper.IsArtifactEditingMode(currentInputValues) && currentInputValues.ContainsKey("definition"))
                {
                  inputValues.Add(new InputValue()
                  {
                    Value = currentInputValues["definition"],
                    DisplayValue = currentInputValues["definition"]
                  });
                  goto label_60;
                }
                else
                {
                  int num = requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.EnablePaginationForGitHubRepositories") ? 1 : 0;
                  string nextToken = string.Empty;
                  if (num == 0)
                  {
                    inputValues = this.GetUserGitRepos(requestContext, authenticationScheme, false, out nextToken, out errorMessage);
                    goto label_60;
                  }
                  else
                  {
                    string searchCriteria;
                    if (currentInputValues.TryGetValue("name", out searchCriteria) && !string.IsNullOrEmpty(searchCriteria))
                    {
                      inputValues = this.GetUserGitReposByCriteria(requestContext, authenticationScheme, searchCriteria, out errorMessage);
                      goto label_60;
                    }
                    else
                    {
                      bool flag = false;
                      inputValues = this.GetFirstPageOfUserRepos(requestContext, authenticationScheme, true, out nextToken, out errorMessage);
                      if (!string.IsNullOrEmpty(nextToken))
                        flag = true;
                      currentInputValues["callbackRequired"] = flag.ToString();
                      goto label_60;
                    }
                  }
                }
              }
              else
                goto label_57;
            case 13:
              if (inputId == "artifactItems")
                break;
              goto label_57;
            case 18:
              if (inputId == "defaultVersionType")
              {
                if (GitHubHelper.GetInputValue(currentInputValues, "definition", out string _, out errorMessage))
                {
                  if (GitHubHelper.GetInputValue(currentInputValues, "connection", out string _, out errorMessage))
                  {
                    inputValues.Add(new InputValue()
                    {
                      Value = "latestFromBranchType",
                      DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.LatestFromBranchType
                    });
                    inputValues.Add(new InputValue()
                    {
                      Value = "specificVersionType",
                      DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.SpecificCommitType
                    });
                    inputValues.Add(new InputValue()
                    {
                      Value = "selectDuringReleaseCreationType",
                      DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.SelectDuringReleaseCreationType
                    });
                    goto label_60;
                  }
                  else
                    goto label_60;
                }
                else
                  goto label_60;
              }
              else
                goto label_57;
            case 19:
              if (inputId == "artifactItemContent")
              {
                string outputValue3;
                if (GitHubHelper.GetInputValue(currentInputValues, "definition", out outputValue3, out errorMessage))
                {
                  string outputValue4;
                  if (GitHubHelper.GetInputValue(currentInputValues, "branch", out outputValue4, out errorMessage))
                  {
                    string str1;
                    currentInputValues.TryGetValue("itemPath", out str1);
                    string str2 = this.getFileContentFunc(requestContext, authenticationScheme, outputValue3, outputValue4, str1, out errorMessage);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                      inputValues.Add(new InputValue()
                      {
                        Data = (IDictionary<string, object>) new Dictionary<string, object>()
                        {
                          {
                            "artifactItemContent",
                            (object) str2
                          }
                        }
                      });
                      goto label_60;
                    }
                    else if (GitHubHttpClient.s_fileTooBigErrorCode.Equals(errorMessage, StringComparison.OrdinalIgnoreCase))
                    {
                      long num = GitHubHttpClient.s_maxFileSizeSupported / 1048576L;
                      errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.FileSize0IsTooLarge, (object) num);
                      goto label_60;
                    }
                    else
                    {
                      errorMessage = !"NotFound".Equals(errorMessage, StringComparison.OrdinalIgnoreCase) ? (!"Unauthorized".Equals(errorMessage, StringComparison.OrdinalIgnoreCase) ? Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.UnknownError : Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.Artifact_Unauthorized) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.FileNotFoundInArtifact, (object) str1, (object) outputValue3);
                      goto label_60;
                    }
                  }
                  else
                    goto label_60;
                }
                else
                  goto label_60;
              }
              else
                goto label_57;
            case 22:
              if (inputId == "defaultVersionSpecific")
              {
                string outputValue5;
                if (GitHubHelper.GetInputValue(currentInputValues, "definition", out outputValue5, out errorMessage))
                {
                  string outputValue6;
                  if (GitHubHelper.GetInputValue(currentInputValues, "defaultVersionType", out outputValue6, out errorMessage))
                  {
                    string outputValue7;
                    if (GitHubHelper.GetInputValue(currentInputValues, "branch", out outputValue7, out errorMessage))
                    {
                      if (string.Equals(outputValue6, "specificVersionType", StringComparison.OrdinalIgnoreCase))
                      {
                        GitHubData.V3.Repository repository = this.getRepoFunc(requestContext, authenticationScheme, outputValue5, out errorMessage);
                        inputValues = repository == null ? new List<InputValue>() : this.GetBranchCommits(requestContext, authenticationScheme, repository.Commits_url?.Replace("{/sha}", string.Empty), outputValue7, false, out errorMessage);
                        goto label_60;
                      }
                      else
                        goto label_60;
                    }
                    else
                      goto label_60;
                  }
                  else
                    goto label_60;
                }
                else
                  goto label_60;
              }
              else
                goto label_57;
            default:
              goto label_57;
          }
          string outputValue8;
          if (GitHubHelper.GetInputValue(currentInputValues, "definition", out outputValue8, out errorMessage))
          {
            string outputValue9;
            if (GitHubHelper.GetInputValue(currentInputValues, "branch", out outputValue9, out errorMessage))
            {
              string str3;
              currentInputValues.TryGetValue("itemPath", out str3);
              string str4;
              currentInputValues.TryGetValue("defaultVersionSpecific", out str4);
              string contentUrl = (string) null;
              GitHubData.V3.Repository repository = this.getRepoFunc(requestContext, authenticationScheme, outputValue8, out errorMessage);
              if (repository != null && repository.Contents_url != null)
                contentUrl = repository.Contents_url.Replace("/{+path}", string.Empty);
              if (!string.IsNullOrEmpty(str3))
                contentUrl = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) contentUrl, (object) str3);
              string str5 = !string.IsNullOrWhiteSpace(str4) ? str4 : outputValue9;
              if (!string.IsNullOrWhiteSpace(str5))
                contentUrl = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?ref={1}", (object) contentUrl, (object) str5);
              inputValues = contentUrl == null ? new List<InputValue>() : this.GetArtifacts(requestContext, authenticationScheme, contentUrl, out errorMessage);
              goto label_60;
            }
            else
              goto label_60;
          }
          else
            goto label_60;
        }
label_57:
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InputTypeNotSupported, (object) inputId);
      }
      catch (AggregateException ex)
      {
        errorMessage = ex.Flatten().InnerException.ToString();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1900000, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
        errorMessage = ex.Message;
      }
label_60:
      return (IList<InputValue>) inputValues;
    }

    public static bool IsArtifactEditingMode(IDictionary<string, string> currentValues)
    {
      if (currentValues == null)
        throw new ArgumentNullException(nameof (currentValues));
      bool result;
      return currentValues.ContainsKey("isArtifactEditOperation") && bool.TryParse(currentValues["isArtifactEditOperation"], out result) && result;
    }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "6#", Justification = "By Design")]
    public GitHubResult<GitHubData.V3.Status> CreateStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      string serviceEndpointGuid,
      string repoNameWithOwner,
      string commitId,
      string state,
      string targetUrl,
      string description,
      string context)
    {
      Dictionary<string, string> currentInputValues = new Dictionary<string, string>()
      {
        {
          "connection",
          serviceEndpointGuid
        }
      };
      GitHubAuthentication authenticationScheme = this.GetAuthenticationScheme(requestContext, projectId, (IDictionary<string, string>) currentInputValues);
      string repoUrl = new GitHubApiRoot().RepositoryUri(repoNameWithOwner).AbsoluteUri();
      return GitHubHelper.GetGitHubHttpClient(requestContext).CreateStatus(authenticationScheme, repoUrl, commitId, state, targetUrl, description, context);
    }

    public GitHubAuthentication GetAuthenticationScheme(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> currentInputValues)
    {
      string outputValue;
      string errorMessage;
      if (!GitHubHelper.GetInputValue(currentInputValues, "connection", out outputValue, out errorMessage))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(errorMessage);
      Guid endpointId = ServiceEndpointHelper.GetEndpointId(outputValue);
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(requestContext, projectId, endpointId);
      GitHubAuthentication hubAuthentication = serviceEndpoint.GetGitHubAuthentication(requestContext, projectId);
      return serviceEndpoint != null && hubAuthentication != null ? hubAuthentication : throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProblemConnectingService);
    }

    public GitHubResult<GitHubData.V3.DeploymentStatus> CreateDeploymentStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      string serviceEndpointGuid,
      string statusUrl,
      string state,
      string logUrl,
      string environmentUrl,
      string description,
      bool autoInactive)
    {
      IDictionary<string, string> currentInputValues = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "connection",
          serviceEndpointGuid
        }
      };
      GitHubAuthentication authenticationScheme = this.GetAuthenticationScheme(requestContext, projectId, currentInputValues);
      return GitHubHelper.GetGitHubHttpClient(requestContext).CreateDeploymentStatus(authenticationScheme, statusUrl, state, logUrl, environmentUrl, description, autoInactive);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static GitHubHttpClient GetGitHubHttpClient(IVssRequestContext requestContext)
    {
      IGitHubAppAccessTokenProvider extension = requestContext.GetExtension<IGitHubAppAccessTokenProvider>();
      if (extension != null)
        extension.Initialize((object) requestContext);
      else
        requestContext.Trace(ExternalProvidersTracePoints.LoadAppExtensionFailed, TraceLevel.Warning, "ReleaseManagementService", "Service", "Unable to load the {0} extension.", (object) "IGitHubAppAccessTokenProvider");
      return GitHubHttpClientFactory.Create(requestContext, extension);
    }

    private static T UnwrapGitHubResult<T>(GitHubResult<T> result, out string errorMessage)
    {
      errorMessage = result.ErrorMessage;
      return result.Result;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By design.")]
    private List<InputValue> GetUserGitRepos(
      IVssRequestContext requestContext,
      GitHubAuthentication gitHubAuthentication,
      bool fetchPaginatedResults,
      out string nextToken,
      out string errorMessage)
    {
      GitHubData.V3.Repository[] source = this.getPagedReposFunc(requestContext, gitHubAuthentication, fetchPaginatedResults, out nextToken, out errorMessage);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.Repository>) source).Select<GitHubData.V3.Repository, InputValue>((Func<GitHubData.V3.Repository, InputValue>) (repo =>
      {
        InputValue userGitRepos = new InputValue()
        {
          DisplayValue = repo.Full_name,
          Value = repo.Full_name,
          Data = (IDictionary<string, object>) new Dictionary<string, object>()
        };
        userGitRepos.Data.Add("gitHubRepositoryId", (object) repo.Id);
        return userGitRepos;
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    public List<InputValue> GetFirstPageOfUserRepos(
      IVssRequestContext requestContext,
      GitHubAuthentication gitHubAuthentication,
      bool fetchPaginatedResults,
      out string nextToken,
      out string errorMessage)
    {
      GitHubData.V3.Repository[] source = this.getPagedReposFunc(requestContext, gitHubAuthentication, fetchPaginatedResults, out nextToken, out errorMessage);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.Repository>) source).Select<GitHubData.V3.Repository, InputValue>((Func<GitHubData.V3.Repository, InputValue>) (repo =>
      {
        InputValue firstPageOfUserRepos = new InputValue()
        {
          DisplayValue = repo.Full_name,
          Value = repo.Full_name,
          Data = (IDictionary<string, object>) new Dictionary<string, object>()
        };
        firstPageOfUserRepos.Data.Add("gitHubRepositoryId", (object) repo.Id);
        return firstPageOfUserRepos;
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    public List<InputValue> GetUserGitReposByCriteria(
      IVssRequestContext requestContext,
      GitHubAuthentication gitHubAuthentication,
      string searchCriteria,
      out string errorMessage)
    {
      GitHubData.V3.Repository[] source = this.searchReposFunc(requestContext, gitHubAuthentication, searchCriteria, out errorMessage);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.Repository>) source).Select<GitHubData.V3.Repository, InputValue>((Func<GitHubData.V3.Repository, InputValue>) (repo =>
      {
        InputValue gitReposByCriteria = new InputValue()
        {
          DisplayValue = repo.Full_name,
          Value = repo.Full_name,
          Data = (IDictionary<string, object>) new Dictionary<string, object>()
        };
        gitReposByCriteria.Data.Add("gitHubRepositoryId", (object) repo.Id);
        return gitReposByCriteria;
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    private List<InputValue> GetRepoBranches(
      IVssRequestContext requestContext,
      GitHubAuthentication gitHubAuthentication,
      string repositoryName,
      out string errorMessage)
    {
      GitHubData.V3.Branch[] source = this.getBranchesFunc(requestContext, gitHubAuthentication, repositoryName, out errorMessage);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.Branch>) source).Select<GitHubData.V3.Branch, InputValue>((Func<GitHubData.V3.Branch, InputValue>) (branch => new InputValue()
      {
        DisplayValue = branch.Name,
        Value = branch.Name
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    private InputValue GetCommit(
      IVssRequestContext requestContext,
      GitHubAuthentication gitHubAuthentication,
      string commitsUrl,
      string sha,
      string branchName,
      out string errorMessage)
    {
      GitHubData.V3.CommitListItem commitListItem = this.getCommitFunc(requestContext, gitHubAuthentication, commitsUrl, sha, out errorMessage);
      if (commitListItem == null)
        return new InputValue();
      return new InputValue()
      {
        DisplayValue = ArtifactTypeUtility.GetCommitDisplayValue(commitListItem.Sha, commitListItem.Commit.Message, 9, false),
        Value = commitListItem.Sha,
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "branch",
            (object) branchName
          },
          {
            "commitMessage",
            (object) ArtifactTypeUtility.RemoveNewLineCharacters(commitListItem.Commit.Message)
          }
        }
      };
    }

    private List<InputValue> GetBranchCommits(
      IVssRequestContext requestContext,
      GitHubAuthentication gitHubAuthentication,
      string commitsUrl,
      string branchName,
      bool appendCommitMessage,
      out string errorMessage)
    {
      string format = "{0}?sha={1}";
      if (string.IsNullOrEmpty(branchName))
        format = "{0}";
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) commitsUrl, (object) branchName);
      GitHubData.V3.CommitListItem[] source = this.getCommitsFunc(requestContext, gitHubAuthentication, str, out errorMessage);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.CommitListItem>) source).Select<GitHubData.V3.CommitListItem, InputValue>((Func<GitHubData.V3.CommitListItem, InputValue>) (commit => new InputValue()
      {
        DisplayValue = commit == null ? string.Empty : ArtifactTypeUtility.GetCommitDisplayValue(commit.Sha, commit.Commit == null ? (string) null : commit.Commit.Message, 9, appendCommitMessage),
        Value = commit.Sha,
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "branch",
            (object) branchName
          },
          {
            "commitMessage",
            commit.Commit != null ? (object) ArtifactTypeUtility.RemoveNewLineCharacters(commit.Commit.Message) : (object) string.Empty
          }
        }
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    private List<InputValue> GetArtifacts(
      IVssRequestContext requestContext,
      GitHubAuthentication gitHubAuthentication,
      string contentUrl,
      out string errorMessage)
    {
      GitHubData.V3.ContentData[] source = this.getContentFunc(requestContext, gitHubAuthentication, contentUrl, out errorMessage);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.ContentData>) source).Select<GitHubData.V3.ContentData, InputValue>((Func<GitHubData.V3.ContentData, InputValue>) (item => new InputValue()
      {
        Value = item.Path,
        DisplayValue = item.Name,
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "itemType",
            string.Equals(item.Type, "dir", StringComparison.OrdinalIgnoreCase) ? (object) "folder" : (object) "file"
          }
        }
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This is required to mock the external methods")]
    public delegate TResult OutFunc<TArg1, TArg2, TArg3, TResult>(
      IVssRequestContext requestContext,
      TArg1 arg1,
      TArg2 arg2,
      out TArg3 arg3);

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This is required to mock the external methods")]
    public delegate TResult OutFuncGetRepo<TArg1, TArg2, TResult>(
      IVssRequestContext requestContext,
      TArg1 arg1,
      out TArg2 arg2);

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This is required to mock the external methods")]
    public delegate TResult OutFuncGetCommit<TArg1, TArg2, TArg3, TArg4, TResult>(
      IVssRequestContext requestContext,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      out TArg4 arg4);

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This is required to mock the external methods")]
    public delegate TResult OutFuncGetPagedRepo<TArg1, TArg2, TArg3, TArg4, TResult>(
      IVssRequestContext requestContext,
      TArg1 arg1,
      TArg2 arg2,
      out TArg3 arg3,
      out TArg4 arg4);

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This is required to mock the external methods")]
    public delegate TResult OutFuncGetFileContent<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
      IVssRequestContext requestContext,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      out TArg5 arg5);
  }
}
