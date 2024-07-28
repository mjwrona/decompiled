// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub.GitHubEndpointHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub
{
  public class GitHubEndpointHelper : GitHubHelper
  {
    private const int CharactersToDisplayInCommitId = 9;
    private const string FileNotFoundErrorCode = "FileNotFound";
    private const string FileTooBigErrorCode = "FileTooBig";
    private const long MaxFileSizeSupported = 524288;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Installation Token Repos data sources will be added soon. This is a placeholder.")]
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Repository[]> getRepositoriesFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.RepositorySearch> searchRepositoriesByNameFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Branch[]> getBranchesFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Repository> getRepositoryFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitListItem> getCommitDetailsFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitListItem[]> getCommitsFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitsDiff> getCommitsDiffFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData[]> getContentsFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData[]> getContentsBranchFunc;
    private readonly GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData> getFileContentData;

    public GitHubEndpointHelper()
      : this((GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Repository[]>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.Repository[]>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "UserRepositoriesForGitHubArtifact"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.RepositorySearch>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.RepositorySearch>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "SearchRepositoriesByName"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Branch[]>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.Branch[]>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "RepoBranches"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Repository>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.Repository>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "Repository"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitListItem>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.CommitListItem>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "CommitDetails"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitListItem[]>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.CommitListItem[]>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "Commits"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitsDiff>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.CommitsDiff>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "CommitsDiff"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData[]>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.ContentData[]>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "Contents"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData[]>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.ContentData[]>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "ContentsBranch"), out errorMessage)), (GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData>) ((IVssRequestContext requestContext, Guid projectId, Guid endpointId, IDictionary<string, string> parameters, out string errorMessage) => GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<GitHubData.V3.ContentData>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, endpointId, parameters, "Contents"), out errorMessage)))
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1006", Justification = "By design")]
    protected GitHubEndpointHelper(
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Repository[]> getRepositoriesFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.RepositorySearch> searchRepositoriesByNameFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Branch[]> getBranchesFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.Repository> getRepositoryFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitListItem> getCommitDetailsFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitListItem[]> getCommitsFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.CommitsDiff> getCommitsDiffFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData[]> getContentsFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData[]> getContentsBranchFunc,
      GitHubEndpointHelper.OutFunc1<Guid, Guid, IDictionary<string, string>, string, GitHubData.V3.ContentData> getFileContentData)
    {
      this.getBranchesFunc = getBranchesFunc;
      this.getRepositoriesFunc = getRepositoriesFunc;
      this.searchRepositoriesByNameFunc = searchRepositoriesByNameFunc;
      this.getRepositoryFunc = getRepositoryFunc;
      this.getCommitDetailsFunc = getCommitDetailsFunc;
      this.getCommitsFunc = getCommitsFunc;
      this.getCommitsDiffFunc = getCommitsDiffFunc;
      this.getContentsFunc = getContentsFunc;
      this.getContentsBranchFunc = getContentsBranchFunc;
      this.getFileContentData = getFileContentData;
    }

    private static Guid GetServiceEndpointId(IDictionary<string, string> currentInputValues)
    {
      string outputValue;
      string errorMessage;
      if (!GitHubHelper.GetInputValue(currentInputValues, "connection", out outputValue, out errorMessage))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(errorMessage);
      return ServiceEndpointHelper.GetEndpointId(outputValue);
    }

    private static T UnwrapServiceEndpointRequestResult<T>(
      ServiceEndpointRequestResult result,
      out string errorMessage)
    {
      if (result.StatusCode == HttpStatusCode.OK && result.Result != null && !result.Result.IsNullOrEmpty<JToken>())
      {
        string json = result.Result[(object) 0]?.ToString();
        errorMessage = result.ErrorMessage;
        T obj;
        ref T local = ref obj;
        JsonUtilities.TryDeserialize<T>(json, out local);
        return obj;
      }
      errorMessage = result.ErrorMessage;
      return default (T);
    }

    private static ServiceEndpointRequestResult ExecuteRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      IDictionary<string, string> parameters,
      string dataSourceName)
    {
      IServiceEndpointProxyService2 service = requestContext.GetService<IServiceEndpointProxyService2>();
      ServiceEndpointRequest serviceEndpointRequest1 = new ServiceEndpointRequest()
      {
        DataSourceDetails = new DataSourceDetails()
        {
          DataSourceName = dataSourceName
        }
      };
      if (!parameters.IsNullOrEmpty<KeyValuePair<string, string>>())
        serviceEndpointRequest1.DataSourceDetails.Parameters.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) parameters);
      IVssRequestContext requestContext1 = requestContext;
      Guid scopeIdentifier = projectId;
      string endpointId1 = endpointId.ToString();
      ServiceEndpointRequest serviceEndpointRequest2 = serviceEndpointRequest1;
      ServiceEndpointRequestResult endpointRequestResult = service.ExecuteServiceEndpointRequest(requestContext1, scopeIdentifier, endpointId1, serviceEndpointRequest2);
      HttpStatusCode? nullable1;
      if (endpointRequestResult.StatusCode >= HttpStatusCode.OK && endpointRequestResult.StatusCode <= (HttpStatusCode) 299)
      {
        IVssRequestContext requestContext2 = requestContext;
        string str1 = serviceEndpointRequest1 != null ? serviceEndpointRequest1.Serialize<ServiceEndpointRequest>() : (string) null;
        nullable1 = endpointRequestResult?.StatusCode;
        string str2 = nullable1.ToString();
        string errorMessage = endpointRequestResult?.ErrorMessage;
        string format = str1 + str2 + errorMessage;
        object[] objArray = Array.Empty<object>();
        requestContext2.TraceAlways(1976491, TraceLevel.Error, "ArtifactExtensions", "ReleaseManagementService", format, objArray);
      }
      IVssRequestContext requestContext3 = requestContext;
      string str3 = serviceEndpointRequest1 != null ? serviceEndpointRequest1.Serialize<ServiceEndpointRequest>() : (string) null;
      string errorMessage1 = endpointRequestResult?.ErrorMessage;
      HttpStatusCode? nullable2;
      if (endpointRequestResult == null)
      {
        nullable1 = new HttpStatusCode?();
        nullable2 = nullable1;
      }
      else
        nullable2 = new HttpStatusCode?(endpointRequestResult.StatusCode);
      nullable1 = nullable2;
      string str4 = nullable1.ToString();
      string message = str3 + errorMessage1 + str4;
      requestContext3.Trace(1976491, TraceLevel.Verbose, "ArtifactExtensions", "ReleaseManagementService", message);
      return endpointRequestResult;
    }

    private static string GetFileContent(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId,
      Dictionary<string, string> dataSourceParameters,
      GitHubData.V3.ContentData fileContentData,
      out string errorMessage)
    {
      return GitHubEndpointHelper.UnwrapServiceEndpointRequestResult<string>(GitHubEndpointHelper.ExecuteRequest(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, fileContentData.Download_url), out errorMessage);
    }

    private static InputValue ToInputValueCommit(
      GitHubData.V3.CommitListItem commit,
      Dictionary<string, string> dataSourceParameters)
    {
      if (commit == null)
        return new InputValue();
      return new InputValue()
      {
        DisplayValue = ArtifactTypeUtility.GetCommitDisplayValue(commit.Sha, commit.Commit.Message, 9, false),
        Value = commit.Sha,
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "branch",
            (object) dataSourceParameters["branch".ToFriendlyName()]
          },
          {
            "commitMessage",
            (object) ArtifactTypeUtility.RemoveNewLineCharacters(commit.Commit.Message)
          }
        }
      };
    }

    private static void VerifyResponseIsAccepted(
      GitHubData.V3.ContentData fileContentData,
      out string errorMessage)
    {
      ArgumentUtility.CheckForNull<GitHubData.V3.ContentData>(fileContentData, nameof (fileContentData));
      errorMessage = string.Empty;
      if (!string.Equals(fileContentData.Type, GitHubConstants.ContentType.File, StringComparison.OrdinalIgnoreCase))
        errorMessage = "FileNotFound";
      if (fileContentData.Size <= 524288)
        return;
      errorMessage = "FileTooBig";
    }

    public new IList<InputValue> GetInputValues(
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
      Dictionary<string, string> dataSourceParameters = new Dictionary<string, string>();
      try
      {
        Guid serviceEndpointId = GitHubEndpointHelper.GetServiceEndpointId(currentInputValues);
        if (inputId != null)
        {
          switch (inputId.Length)
          {
            case 6:
              if (inputId == "branch")
              {
                List<string> requiredParameters = new List<string>()
                {
                  "definition"
                };
                if (InputQueryValueUtilities.GetRequiredParameters(currentInputValues, (IReadOnlyCollection<string>) requiredParameters, out dataSourceParameters, out errorMessage))
                {
                  inputValues = dataSourceParameters.IsNullOrEmpty<KeyValuePair<string, string>>() ? new List<InputValue>() : this.GetRepoBranches(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, out errorMessage);
                  goto label_47;
                }
                else
                  goto label_47;
              }
              else
                goto label_44;
            case 7:
              if (inputId == "commits")
              {
                List<string> requiredParameters = new List<string>()
                {
                  "definition",
                  "branch"
                };
                if (InputQueryValueUtilities.GetRequiredParameters(currentInputValues, (IReadOnlyCollection<string>) requiredParameters, out dataSourceParameters, out errorMessage))
                {
                  if (this.GetRepository(requestContext, projectId, serviceEndpointId, dataSourceParameters, out errorMessage) == null)
                  {
                    inputValues = new List<InputValue>();
                    goto label_47;
                  }
                  else
                  {
                    string a;
                    currentInputValues.TryGetValue("defaultVersionType", out a);
                    if (string.Equals(a, "specificVersionType", StringComparison.OrdinalIgnoreCase))
                    {
                      string str;
                      if (!currentInputValues.TryGetValue("defaultVersionSpecific", out str) || string.IsNullOrEmpty(str))
                        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoSpecificVersionValueAvailableForSpecificVersionType));
                      dataSourceParameters.Add("defaultVersionSpecific".ToFriendlyName(), str);
                      inputValues = this.GetCommitDetails(requestContext, projectId, serviceEndpointId, dataSourceParameters, out errorMessage);
                      goto label_47;
                    }
                    else
                    {
                      inputValues = this.GetBranchCommits(requestContext, projectId, serviceEndpointId, dataSourceParameters, out errorMessage);
                      goto label_47;
                    }
                  }
                }
                else
                  goto label_47;
              }
              else
                goto label_44;
            case 9:
              if (inputId == "artifacts")
                break;
              goto label_44;
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
                  goto label_47;
                }
                else
                {
                  currentInputValues["callbackRequired"] = true.ToString();
                  string searchText;
                  inputValues = !currentInputValues.TryGetValue("name", out searchText) || string.IsNullOrEmpty(searchText) ? this.GetUserGitRepos(requestContext, projectId, serviceEndpointId, out errorMessage) : this.SearchUserRepositoriesByName(requestContext, projectId, serviceEndpointId, searchText, out errorMessage);
                  goto label_47;
                }
              }
              else
                goto label_44;
            case 13:
              if (inputId == "artifactItems")
                break;
              goto label_44;
            case 18:
              if (inputId == "defaultVersionType")
              {
                if (currentInputValues.IsInputValueNotNull("definition"))
                {
                  if (currentInputValues.IsInputValueNotNull("connection"))
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
                    goto label_47;
                  }
                  else
                    goto label_47;
                }
                else
                  goto label_47;
              }
              else
                goto label_44;
            case 19:
              if (inputId == "artifactItemContent")
              {
                List<string> requiredParameters = new List<string>()
                {
                  "definition",
                  "branch"
                };
                if (InputQueryValueUtilities.GetRequiredParameters(currentInputValues, (IReadOnlyCollection<string>) requiredParameters, out dataSourceParameters, out errorMessage))
                {
                  string enumerable;
                  if (currentInputValues.TryGetValue("itemPath", out enumerable) && !enumerable.IsNullOrEmpty<char>())
                    dataSourceParameters.Add("itemPath".ToFriendlyName(), enumerable);
                  inputValues = this.GetFiles(requestContext, projectId, serviceEndpointId, dataSourceParameters, out errorMessage);
                  goto label_47;
                }
                else
                  goto label_47;
              }
              else
                goto label_44;
            case 22:
              if (inputId == "defaultVersionSpecific")
              {
                List<string> requiredParameters = new List<string>()
                {
                  "definition",
                  "defaultVersionType",
                  "branch"
                };
                if (InputQueryValueUtilities.GetRequiredParameters(currentInputValues, (IReadOnlyCollection<string>) requiredParameters, out dataSourceParameters, out errorMessage))
                {
                  string outputValue;
                  if (GitHubHelper.GetInputValue(currentInputValues, "defaultVersionType", out outputValue, out errorMessage))
                  {
                    if (outputValue.Equals("specificVersionType", StringComparison.OrdinalIgnoreCase))
                    {
                      inputValues = this.GetRepository(requestContext, projectId, serviceEndpointId, dataSourceParameters, out errorMessage) != null ? this.GetBranchCommits(requestContext, projectId, serviceEndpointId, dataSourceParameters, out errorMessage) : new List<InputValue>();
                      goto label_47;
                    }
                    else
                      goto label_47;
                  }
                  else
                    goto label_47;
                }
                else
                  goto label_47;
              }
              else
                goto label_44;
            default:
              goto label_44;
          }
          List<string> requiredParameters1 = new List<string>()
          {
            "definition",
            "branch",
            "connection"
          };
          if (currentInputValues.IsInputValueNotNull("specificVersionType"))
          {
            if (InputQueryValueUtilities.GetRequiredParameters(currentInputValues, (IReadOnlyCollection<string>) requiredParameters1, out dataSourceParameters, out errorMessage))
            {
              string enumerable;
              if (currentInputValues.TryGetValue("itemPath", out enumerable) && !enumerable.IsNullOrEmpty<char>())
                dataSourceParameters.Add("itemPath".ToFriendlyName(), enumerable);
              inputValues = this.GetArtifacts(requestContext, projectId, serviceEndpointId, dataSourceParameters, out errorMessage);
              goto label_47;
            }
            else
              goto label_47;
          }
          else
            goto label_47;
        }
label_44:
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InputTypeNotSupported, (object) inputId);
      }
      catch (AggregateException ex)
      {
        errorMessage = ex.Flatten().InnerException?.ToString();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1900000, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
        errorMessage = ex.Message;
      }
label_47:
      return (IList<InputValue>) inputValues;
    }

    private List<InputValue> GetFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId,
      Dictionary<string, string> dataSourceParameters,
      out string errorMessage)
    {
      List<InputValue> files = new List<InputValue>();
      GitHubData.V3.ContentData fileContentData = this.getFileContentData(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, out errorMessage);
      if (errorMessage.IsNullOrEmpty<char>())
        GitHubEndpointHelper.VerifyResponseIsAccepted(fileContentData, out errorMessage);
      string enumerable = string.Empty;
      if (errorMessage.IsNullOrEmpty<char>())
        enumerable = GitHubEndpointHelper.GetFileContent(requestContext, projectId, serviceEndpointId, dataSourceParameters, fileContentData, out errorMessage);
      if (errorMessage.IsNullOrEmpty<char>() && !enumerable.IsNullOrEmpty<char>())
        files.Add(new InputValue()
        {
          Data = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "artifactItemContent",
              (object) enumerable
            }
          }
        });
      return files;
    }

    [SuppressMessage("Microsoft.Design", "CA1021", Justification = "By design")]
    public void GetCommitsDiff(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId,
      string repositoryName,
      string startVersion,
      string endVersion,
      out GitHubData.V3.CommitListItem[] commits,
      out string errorMessage)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("definition".ToFriendlyName(), repositoryName);
      dictionary.Add("base", startVersion);
      dictionary.Add("head", endVersion);
      commits = (GitHubData.V3.CommitListItem[]) null;
      if (startVersion.IsNullOrEmpty<char>())
      {
        commits = this.getCommitsFunc(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dictionary, out errorMessage);
      }
      else
      {
        GitHubData.V3.CommitsDiff commitsDiff = this.getCommitsDiffFunc(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dictionary, out errorMessage);
        if (commitsDiff == null)
          return;
        commits = commitsDiff.Commits;
        ref GitHubData.V3.CommitListItem[] local = ref commits;
        GitHubData.V3.CommitListItem[] source = commits;
        GitHubData.V3.CommitListItem[] array = source != null ? ((IEnumerable<GitHubData.V3.CommitListItem>) source).OrderByDescending<GitHubData.V3.CommitListItem, string>((Func<GitHubData.V3.CommitListItem, string>) (commit => commit?.Commit?.Author?.Date)).ToArray<GitHubData.V3.CommitListItem>() : (GitHubData.V3.CommitListItem[]) null;
        local = array;
      }
    }

    private List<InputValue> GetArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId,
      Dictionary<string, string> dataSourceParameters,
      out string errorMessage)
    {
      GitHubData.V3.ContentData[] source = !dataSourceParameters.ContainsKey("itemPath".ToFriendlyName()) ? this.getContentsBranchFunc(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, out errorMessage) : this.getContentsFunc(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, out errorMessage);
      return !string.IsNullOrEmpty(errorMessage) || source == null ? new List<InputValue>() : ((IEnumerable<GitHubData.V3.ContentData>) source).Select<GitHubData.V3.ContentData, InputValue>((Func<GitHubData.V3.ContentData, InputValue>) (item => new InputValue()
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
      })).ToList<InputValue>();
    }

    private List<InputValue> GetBranchCommits(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId,
      Dictionary<string, string> dataSourceParameters,
      out string errorMessage)
    {
      GitHubData.V3.CommitListItem[] source = this.getCommitsFunc(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, out errorMessage);
      return !string.IsNullOrEmpty(errorMessage) || source == null ? new List<InputValue>() : ((IEnumerable<GitHubData.V3.CommitListItem>) source).Select<GitHubData.V3.CommitListItem, InputValue>((Func<GitHubData.V3.CommitListItem, InputValue>) (commit => new InputValue()
      {
        DisplayValue = commit == null ? string.Empty : ArtifactTypeUtility.GetCommitDisplayValue(commit.Sha, commit.Commit == null ? (string) null : commit.Commit.Message, 9, false),
        Value = commit?.Sha,
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "branch",
            (object) dataSourceParameters["branch".ToFriendlyName()]
          },
          {
            "commitMessage",
            commit?.Commit != null ? (object) ArtifactTypeUtility.RemoveNewLineCharacters(commit.Commit.Message) : (object) string.Empty
          }
        }
      })).ToList<InputValue>();
    }

    private List<InputValue> GetCommitDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId,
      Dictionary<string, string> dataSourceParameters,
      out string errorMessage)
    {
      return new List<InputValue>()
      {
        GitHubEndpointHelper.ToInputValueCommit(this.getCommitDetailsFunc(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, out errorMessage), dataSourceParameters)
      };
    }

    private GitHubData.V3.Repository GetRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId,
      Dictionary<string, string> dataSourceParameters,
      out string errorMessage)
    {
      return this.getRepositoryFunc(requestContext, projectId, serviceEndpointId, (IDictionary<string, string>) dataSourceParameters, out errorMessage);
    }

    private List<InputValue> SearchUserRepositoriesByName(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      string searchText,
      out string errorMessage)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      dictionary.Add("name".ToFriendlyName(), searchText);
      return ((IEnumerable<GitHubData.V3.Repository>) this.searchRepositoriesByNameFunc(requestContext, projectId, endpointId, dictionary, out errorMessage).Items).Select<GitHubData.V3.Repository, InputValue>((Func<GitHubData.V3.Repository, InputValue>) (repo => new InputValue()
      {
        DisplayValue = repo.Full_name,
        Value = repo.Full_name
      })).ToList<InputValue>();
    }

    private List<InputValue> GetUserGitRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      out string errorMessage)
    {
      return this.FetchRepositories(requestContext, projectId, endpointId, out errorMessage);
    }

    private List<InputValue> GetRepoBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      IDictionary<string, string> dataSourceParameters,
      out string errorMessage)
    {
      GitHubData.V3.Branch[] source = this.getBranchesFunc(requestContext, projectId, endpointId, dataSourceParameters, out errorMessage);
      return string.IsNullOrEmpty(errorMessage) && source != null ? ((IEnumerable<GitHubData.V3.Branch>) source).Select<GitHubData.V3.Branch, InputValue>((Func<GitHubData.V3.Branch, InputValue>) (branch => new InputValue()
      {
        DisplayValue = branch.Name,
        Value = branch.Name
      })).ToList<InputValue>() : (List<InputValue>) null;
    }

    public List<InputValue> GetRepoBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      string repositoryName,
      out string errorMessage)
    {
      IDictionary<string, string> dataSourceParameters = (IDictionary<string, string>) new Dictionary<string, string>();
      dataSourceParameters.Add("definition".ToFriendlyName(), repositoryName);
      return this.GetRepoBranches(requestContext, projectId, endpointId, dataSourceParameters, out errorMessage);
    }

    [SuppressMessage("Microsoft.Design", "CA1021", Justification = "By design")]
    public delegate TResult OutFunc1<TArg1, TArg2, TArg3, TArg4, TResult>(
      IVssRequestContext requestContext,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      out TArg4 arg4);
  }
}
