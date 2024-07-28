// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git.GitArtifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By Design")]
  public class GitArtifact : ArtifactTypeBase
  {
    public const int TotalCommits = 100;
    private const string GitDefinitionId = "definition";
    private const string ProjectId = "project";
    private const string BranchesId = "branches";
    private const string DefaultVersionSpecificInputId = "defaultVersionSpecific";
    private const string DefaultVersionTypeId = "defaultVersionType";
    private const string VisibleRule = "visibleRule";
    private const string DefaultHash = "0000000000000000000000000000000000000000";
    private const string ReportBuildStatusContextGenre = "continuous-integration";
    private const string ReportBuildStatusContextFormat = "build/{0}";
    private const int CharactersToDisplayInCommitId = 8;
    private const string VersionControlFeatureId = "ms.vss-code.version-control";
    private bool isUseNewBrandingEnabled;
    private IGitData gitData;

    public static string GitName => "Git";

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It is not a property")]
    public static InputValues GetDefaultVersionTypes()
    {
      List<InputValue> inputValueList = new List<InputValue>();
      InputValue inputValue = new InputValue()
      {
        Value = "latestFromBranchType",
        DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.LatestFromBranchType
      };
      inputValueList.Add(inputValue);
      inputValueList.Add(new InputValue()
      {
        Value = "specificVersionType",
        DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.SpecificCommitType
      });
      inputValueList.Add(new InputValue()
      {
        Value = "selectDuringReleaseCreationType",
        DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.SelectDuringReleaseCreationType
      });
      return new InputValues()
      {
        InputId = "defaultVersionType",
        DefaultValue = inputValue.DisplayValue,
        PossibleValues = (IList<InputValue>) inputValueList,
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = false,
        Error = (InputValuesError) null
      };
    }

    public override string Name => GitArtifact.GitName;

    public override string DisplayName => !this.isUseNewBrandingEnabled ? Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.GitArtifactDisplayNameOld : Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.GitArtifactDisplayName;

    public override string EndpointTypeId => "Git";

    public override Guid ArtifactDownloadTaskId { get; }

    public override IDictionary<string, Guid> ArtifactDownloadTaskIds { get; }

    public override IDictionary<Guid, IDictionary<string, string>> TaskInputMappings { get; }

    public override IDictionary<string, string> TaskInputDefaultValues { get; }

    public override string Type { get; }

    public override IDictionary<string, string> TaskInputMapping { get; }

    public override bool ResolveStep(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      throw new NotImplementedException();
    }

    public override string UniqueSourceIdentifier => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{{{{0}}}}}:{{{{{1}}}}}", (object) "project", (object) "definition");

    public override bool IsCommitsTraceabilitySupported => true;

    public override bool IsWorkitemsTraceabilitySupported => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForProject,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForProject,
        InputMode = InputMode.Combo,
        Id = "project",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.Guid
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryLabel,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDescription,
        InputMode = InputMode.Combo,
        Id = "definition",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.Guid,
          MinLength = new int?(1)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "project"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DefaultBranchDisplayName,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DefaultBranchDescription,
        InputMode = InputMode.Combo,
        Id = "branches",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "project",
          "definition"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersion,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersion,
        InputMode = InputMode.Combo,
        Id = "defaultVersionType",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "project",
          "definition",
          "branches"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersionSpecificCommit,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersionSpecificCommit,
        InputMode = InputMode.Combo,
        Id = "defaultVersionSpecific",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "project",
          "definition",
          "defaultVersionType",
          "branches"
        },
        Properties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "visibleRule",
            (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1}", (object) "defaultVersionType", (object) "specificVersionType")
          }
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutSubmodulesInput,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutSubmodulesInput,
        InputMode = InputMode.CheckBox,
        Id = "checkoutSubmodules",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false
        },
        HasDynamicValueInformation = false
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.SubmoduleCheckoutRecursionLevelInput,
        Description = string.Empty,
        InputMode = InputMode.Combo,
        Id = "checkoutNestedSubmodules",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false
        },
        Values = new InputValues()
        {
          InputId = "checkoutNestedSubmodules",
          DefaultValue = bool.TrueString,
          PossibleValues = (IList<InputValue>) new List<InputValue>()
          {
            new InputValue()
            {
              Value = bool.FalseString,
              DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutSubmoduleTopLevelOnlyLabel
            },
            new InputValue()
            {
              Value = bool.TrueString,
              DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutSubmoduleAllNestedLabel
            }
          },
          IsLimitedToPossibleValues = true,
          IsDisabled = false,
          IsReadOnly = false,
          Error = (InputValuesError) null
        },
        Properties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "visibleRule",
            (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1}", (object) "checkoutSubmodules", (object) bool.TrueString)
          }
        },
        HasDynamicValueInformation = false
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutFromLFSInput,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutFromLFSDescription,
        InputMode = InputMode.CheckBox,
        Id = "gitLfsSupport",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false
        },
        HasDynamicValueInformation = false
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutDepthInput,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CheckoutDepthDescription,
        InputMode = InputMode.TextBox,
        Id = "fetchDepth",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false
        },
        HasDynamicValueInformation = false
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForArtifacts,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForArtifacts,
        InputMode = InputMode.None,
        Id = "artifacts",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "project",
          "definition",
          "defaultVersionType",
          "branches",
          "defaultVersionSpecific"
        },
        HasDynamicValueInformation = true
      }
    };

    public GitArtifact(bool isUseNewBrandingEnabled)
      : this(isUseNewBrandingEnabled, (IGitData) new GitData())
    {
    }

    protected GitArtifact(bool isUseNewBrandingEnabled, IGitData gitData)
    {
      this.isUseNewBrandingEnabled = isUseNewBrandingEnabled;
      this.gitData = gitData;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By Design")]
    public override InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      if (currentInputValues == null)
        throw new ArgumentNullException(nameof (currentInputValues));
      string errorMessage = (string) null;
      List<InputValue> inputValueList = new List<InputValue>();
      InputValues inputValues = new InputValues()
      {
        InputId = inputId,
        PossibleValues = (IList<InputValue>) inputValueList,
        DefaultValue = string.Empty,
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = false,
        Error = (InputValuesError) null
      };
      try
      {
        if (inputId != null)
        {
          switch (inputId.Length)
          {
            case 7:
              if (inputId == "project")
              {
                IList<ProjectInfo> projects = ProjectHelper.GetProjects(context);
                projects.RemoveAll<ProjectInfo>((Func<ProjectInfo, bool>) (project => project.Id == projectInfo.Id));
                IOrderedEnumerable<ProjectInfo> source = projects.OrderBy<ProjectInfo, string>((Func<ProjectInfo, string>) (x => x.Name));
                if (GitArtifact.IsCodeFeatureEnabled(context, projectInfo.Id))
                  inputValueList.Add(new InputValue()
                  {
                    Value = projectInfo.Id.ToString(),
                    DisplayValue = projectInfo.Name
                  });
                inputValueList.AddRange(source.Select<ProjectInfo, InputValue>((Func<ProjectInfo, InputValue>) (info => new InputValue()
                {
                  Value = info.Id.ToString(),
                  DisplayValue = info.Name
                })));
                goto label_54;
              }
              else
                break;
            case 8:
              if (inputId == "branches")
              {
                string outputValue;
                if (GitArtifact.GetInputValue(currentInputValues, "definition", out outputValue, out errorMessage))
                {
                  IList<GitBranchStats> branches = new GitHelper(this.gitData).GetBranches(context, outputValue, out errorMessage);
                  if (branches == null || branches.Count == 0)
                  {
                    errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BranchNotFoundError, (object) "definition");
                    goto label_54;
                  }
                  else
                  {
                    IOrderedEnumerable<GitBranchStats> source = branches.OrderBy<GitBranchStats, string>((Func<GitBranchStats, string>) (x => x.Name));
                    inputValueList.AddRange(source.Select<GitBranchStats, InputValue>((Func<GitBranchStats, InputValue>) (b => new InputValue()
                    {
                      Value = b.Name,
                      DisplayValue = b.Name
                    })));
                    goto label_54;
                  }
                }
                else
                  goto label_54;
              }
              else
                break;
            case 9:
              if (inputId == "artifacts")
              {
                string outputValue1;
                if (GitArtifact.GetInputValue(currentInputValues, "definition", out outputValue1, out errorMessage))
                {
                  string outputValue2;
                  if (GitArtifact.GetInputValue(currentInputValues, "branches", out outputValue2, out errorMessage))
                  {
                    string commit;
                    currentInputValues.TryGetValue("defaultVersionSpecific", out commit);
                    GitArtifact.AddGitItemsToList(new GitHelper(this.gitData).GetItems(context, outputValue1, outputValue2, commit, out errorMessage), inputValues);
                    goto label_54;
                  }
                  else
                    goto label_54;
                }
                else
                  goto label_54;
              }
              else
                break;
            case 10:
              if (inputId == "definition")
              {
                Guid result;
                if (!Guid.TryParse(currentInputValues["project"], out result) || result == Guid.Empty)
                {
                  errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidProject);
                  goto label_54;
                }
                else
                {
                  IList<GitRepository> repositories = new GitHelper(this.gitData).GetRepositories(context, result);
                  if (repositories == null || repositories.Count == 0)
                  {
                    errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ExpectedRepositoryNotFound, (object) this.DisplayName);
                    goto label_54;
                  }
                  else
                  {
                    IOrderedEnumerable<GitRepository> source = repositories.OrderBy<GitRepository, string>((Func<GitRepository, string>) (x => x.Name));
                    inputValueList.AddRange(source.Select<GitRepository, InputValue>((Func<GitRepository, InputValue>) (repository => new InputValue()
                    {
                      Value = repository.Id.ToString(),
                      DisplayValue = repository.Name
                    })));
                    goto label_54;
                  }
                }
              }
              else
                break;
            case 13:
              if (inputId == "artifactItems")
              {
                string currentInputValue1 = currentInputValues["itemPath"];
                string currentInputValue2 = currentInputValues["definition"];
                string currentInputValue3 = currentInputValues["branches"];
                GitArtifact.AddGitItemsToList((IList<GitItem>) new GitHelper(this.gitData).GetItemsBatch(context, currentInputValue2, currentInputValue3, currentInputValue1, out errorMessage).FirstOrDefault<List<GitItem>>(), inputValues);
                goto label_54;
              }
              else
                break;
            case 18:
              if (inputId == "defaultVersionType")
              {
                if (GitArtifact.GetInputValue(currentInputValues, "definition", out string _, out errorMessage))
                {
                  if (GitArtifact.GetInputValue(currentInputValues, "project", out string _, out errorMessage))
                    return GitArtifact.GetDefaultVersionTypes();
                  goto label_54;
                }
                else
                  goto label_54;
              }
              else
                break;
            case 19:
              if (inputId == "artifactItemContent")
              {
                string currentInputValue4 = currentInputValues["itemPath"];
                string currentInputValue5 = currentInputValues["definition"];
                string currentInputValue6 = currentInputValues["branches"];
                GitArtifact.AddGitItemContentToList(new GitHelper(this.gitData).GetItemContent(context, currentInputValue5, currentInputValue6, currentInputValue4, out errorMessage), inputValues);
                goto label_54;
              }
              else
                break;
            case 22:
              if (inputId == "defaultVersionSpecific")
              {
                string outputValue3;
                if (GitArtifact.GetInputValue(currentInputValues, "definition", out outputValue3, out errorMessage))
                {
                  string outputValue4;
                  if (GitArtifact.GetInputValue(currentInputValues, "defaultVersionType", out outputValue4, out errorMessage))
                  {
                    string outputValue5;
                    if (GitArtifact.GetInputValue(currentInputValues, "branches", out outputValue5, out errorMessage))
                    {
                      if (string.Equals(outputValue4, "specificVersionType", StringComparison.OrdinalIgnoreCase))
                      {
                        IList<GitCommitRef> commits = new GitHelper(this.gitData).GetCommits(context, outputValue3, outputValue5, 100, out errorMessage);
                        if (commits != null)
                        {
                          using (IEnumerator<GitCommitRef> enumerator = commits.GetEnumerator())
                          {
                            while (enumerator.MoveNext())
                            {
                              GitCommitRef current = enumerator.Current;
                              InputValue inputValue = new InputValue()
                              {
                                DisplayValue = ArtifactTypeUtility.GetCommitDisplayValue(current.CommitId, current.Comment, 8, false),
                                Value = current.CommitId
                              };
                              inputValue.Data = (IDictionary<string, object>) new Dictionary<string, object>();
                              inputValue.Data.Add("commitMessage", (object) ArtifactTypeUtility.RemoveNewLineCharacters(current.Comment));
                              inputValueList.Add(inputValue);
                            }
                            goto label_54;
                          }
                        }
                        else
                          goto label_54;
                      }
                      else
                        goto label_54;
                    }
                    else
                      goto label_54;
                  }
                  else
                    goto label_54;
                }
                else
                  goto label_54;
              }
              else
                break;
          }
        }
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InputTypeNotSupported, (object) inputId);
      }
      catch (AggregateException ex)
      {
        errorMessage = ex.Flatten().InnerException.ToString();
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
      }
label_54:
      InputValuesError inputValuesError1;
      if (errorMessage != null)
        inputValuesError1 = new InputValuesError()
        {
          Message = errorMessage
        };
      else
        inputValuesError1 = (InputValuesError) null;
      InputValuesError inputValuesError2 = inputValuesError1;
      inputValues.Error = inputValuesError2;
      return inputValues;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is logged for diagnosability")]
    private static bool IsCodeFeatureEnabled(IVssRequestContext requestContext, Guid projectId)
    {
      try
      {
        FeatureManagementHttpClient featureManagementHttpClient = requestContext.GetClient<FeatureManagementHttpClient>(ServiceInstanceTypes.TFS);
        Func<Task<ContributedFeatureState>> func = (Func<Task<ContributedFeatureState>>) (() => featureManagementHttpClient.GetFeatureStateForScopeAsync("ms.vss-code.version-control", "host", "project", projectId.ToString()));
        return requestContext.ExecuteAsyncAndSyncResult<ContributedFeatureState>(func).State != 0;
      }
      catch (Exception ex)
      {
        requestContext.Trace(1900044, TraceLevel.Error, "ReleaseManagementService", "JobLayer", "Ignoring exception that occurred while checking whether code feature is enabled or not. Exception: {0}", (object) ex.ToString());
      }
      return true;
    }

    public override AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      InputValue sourceInput1 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "project", true, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectDetailsNotAvailable, (object) serverArtifact.SourceDataKeys, (object) "project"));
      InputValue sourceInput2 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "definition", true, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionDetailsNotAvailable, (object) serverArtifact.SourceDataKeys, (object) "definition"));
      InputValue sourceInput3 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "checkoutSubmodules", false, string.Empty);
      InputValue sourceInput4 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "checkoutNestedSubmodules", false, string.Empty);
      InputValue sourceInput5 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "gitLfsSupport", false, string.Empty);
      InputValue sourceInput6 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "fetchDepth", false, string.Empty);
      InputValue sourceInput7 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "version", false, string.Empty);
      string empty = string.Empty;
      object obj;
      if (sourceInput7?.Data != null && sourceInput7.Data.TryGetValue("branch", out obj) && obj != null)
        empty = obj.ToString();
      string str = JsonConvert.SerializeObject((object) new Dictionary<string, string>()
      {
        {
          "RelativePath",
          serverArtifact.Path
        },
        {
          "ProjectId",
          sourceInput1.Value
        },
        {
          "RepositoryId",
          sourceInput2.Value
        },
        {
          "Branch",
          empty
        },
        {
          "checkoutSubmodules",
          sourceInput3 != null ? sourceInput3.Value : string.Empty
        },
        {
          "checkoutNestedSubmodules",
          sourceInput4 != null ? sourceInput4.Value : string.Empty
        },
        {
          "gitLfsSupport",
          sourceInput5 != null ? sourceInput5.Value : string.Empty
        },
        {
          "fetchDepth",
          sourceInput6 != null ? sourceInput6.Value : string.Empty
        }
      });
      return new AgentArtifactDefinition()
      {
        Name = serverArtifact.Name,
        Alias = serverArtifact.Alias,
        Version = serverArtifact.ArtifactVersion,
        ArtifactType = AgentArtifactType.TFGit,
        Details = str
      };
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    public override IList<InputValue> GetAvailableVersions(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (sourceInputs == null)
        throw new ArgumentNullException(nameof (sourceInputs));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      string input;
      if (!sourceInputs.TryGetValue("project", out input))
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectDetailsMissing);
      Guid result;
      if (!Guid.TryParse(input, out result))
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidProject);
      string repositoryId;
      if (!sourceInputs.TryGetValue("definition", out repositoryId))
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDetailsMissing);
      List<InputValue> availableVersions = new List<InputValue>();
      string branch;
      if (!sourceInputs.TryGetValue("branches", out branch))
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BranchDetailsMissing);
      string a;
      sourceInputs.TryGetValue("defaultVersionType", out a);
      string errorMessage;
      if (string.Equals(a, "specificVersionType", StringComparison.OrdinalIgnoreCase))
      {
        string commitId;
        if (!sourceInputs.TryGetValue("defaultVersionSpecific", out commitId) || string.IsNullOrEmpty(commitId))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoSpecificVersionValueAvailableForSpecificVersionType));
        GitCommit commit = new GitHelper(this.gitData).GetCommit(requestContext, result, repositoryId, commitId, out errorMessage);
        if (commit != null)
        {
          InputValue inputValue = new InputValue()
          {
            DisplayValue = ArtifactTypeUtility.GetCommitDisplayValue(commit.CommitId, commit.Comment, 8, false),
            Value = commit.CommitId
          };
          inputValue.Data = (IDictionary<string, object>) new Dictionary<string, object>();
          inputValue.Data.Add("branch", (object) branch);
          availableVersions.Add(inputValue);
        }
      }
      else
      {
        foreach (GitCommitRef commit in (IEnumerable<GitCommitRef>) new GitHelper(this.gitData).GetCommits(requestContext, repositoryId, branch, 100, out errorMessage))
        {
          InputValue inputValue = new InputValue()
          {
            DisplayValue = ArtifactTypeUtility.GetCommitDisplayValue(commit.CommitId, commit.Comment, 8, false),
            Value = commit.CommitId
          };
          inputValue.Data = (IDictionary<string, object>) new Dictionary<string, object>();
          inputValue.Data.Add("branch", (object) branch);
          inputValue.Data.Add("commitMessage", (object) ArtifactTypeUtility.RemoveNewLineCharacters(commit.Comment));
          availableVersions.Add(inputValue);
        }
      }
      return (IList<InputValue>) availableVersions;
    }

    public override InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      return this.GetAvailableVersions(requestContext, sourceInputs, projectInfo).FirstOrDefault<InputValue>();
    }

    public override IList<Change> GetChanges(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (sourceData == null)
        throw new ArgumentNullException(nameof (sourceData));
      if (endVersion == null)
        throw new ArgumentNullException(nameof (endVersion));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      List<Change> changes = new List<Change>();
      GitHttpClient gitHttpClient = requestContext.GetClient<GitHttpClient>();
      GitQueryCommitsCriteria searchCriteria = new GitQueryCommitsCriteria();
      InputValue repositoryId;
      sourceData.TryGetValue("definition", out repositoryId);
      if (startVersion != null)
      {
        searchCriteria.ItemVersion = new GitVersionDescriptor()
        {
          VersionType = GitVersionType.Commit,
          Version = startVersion?.Value
        };
        searchCriteria.CompareVersion = new GitVersionDescriptor()
        {
          VersionType = GitVersionType.Commit,
          Version = endVersion.Value
        };
      }
      else
        searchCriteria.ItemVersion = new GitVersionDescriptor()
        {
          VersionType = GitVersionType.Commit,
          Version = endVersion.Value
        };
      Func<Task<List<GitCommitRef>>> func = (Func<Task<List<GitCommitRef>>>) (() => gitHttpClient.GetCommitsAsync(projectInfo.Name, repositoryId.Value, searchCriteria));
      List<GitCommitRef> result = requestContext.ExecuteAsyncAndGetResult<List<GitCommitRef>>(func);
      if (result != null)
      {
        foreach (GitCommitRef gitCommitRef in result.Take<GitCommitRef>(top))
        {
          IdentityRef identityRef = new IdentityRef()
          {
            Id = (string) null,
            DisplayName = gitCommitRef.Author?.Name
          };
          if (!string.IsNullOrEmpty(gitCommitRef.Author?.ImageUrl))
          {
            identityRef.Links = new ReferenceLinks();
            identityRef.Links.AddLink("avatar", gitCommitRef.Author.ImageUrl);
          }
          changes.Add(new Change()
          {
            Id = gitCommitRef.CommitId,
            Author = identityRef,
            Message = gitCommitRef.Comment,
            DisplayUri = new Uri(gitCommitRef.Url),
            Timestamp = new DateTime?(Convert.ToDateTime((object) gitCommitRef.Author.Date, (IFormatProvider) CultureInfo.InvariantCulture)),
            ChangeType = "TfsGit",
            Location = new Uri(gitCommitRef.Url),
            PushedBy = (IdentityRef) null
          });
        }
      }
      return (IList<Change>) changes;
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Justification = "By design")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "3#", Justification = "By design")]
    public override IList<Change> GetChangesBetweenArtifactSource(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineArtifactSource currentReleaseArtifactSource,
      PipelineArtifactSource lastReleaseArtifactSource,
      int top)
    {
      return (IList<Change>) new List<Change>();
    }

    public override IList<WorkItemRef> GetWorkItems(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext,
      GetConfig getConfig)
    {
      return (IList<WorkItemRef>) new List<WorkItemRef>();
    }

    public override IDictionary<string, string> GetFormatMaskTokensFromReleaseArtifactInstance(
      IDictionary<string, InputValue> sourceInputs)
    {
      Dictionary<string, string> artifactInstance = new Dictionary<string, string>(base.GetFormatMaskTokensFromReleaseArtifactInstance(sourceInputs));
      if (sourceInputs == null)
        throw new ArgumentNullException(nameof (sourceInputs));
      InputValue inputValue1;
      if (sourceInputs.TryGetValue("version", out inputValue1))
        artifactInstance["build.buildNumber"] = inputValue1.DisplayValue;
      InputValue inputValue2;
      if (sourceInputs.TryGetValue("definition", out inputValue2))
      {
        artifactInstance["Build.DefinitionId"] = inputValue2.Value;
        artifactInstance["build.definitionName"] = inputValue2.DisplayValue;
      }
      return (IDictionary<string, string>) artifactInstance;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions must be caught inorder to prevent them from being thrown")]
    protected override IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Dictionary<string, InputValue> dictionary = artifactSource != null ? artifactSource.SourceData : throw new ArgumentNullException(nameof (artifactSource));
      Dictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> configurationVariables = new Dictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>();
      configurationVariables.Add("repository.provider", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
      {
        Value = "Git"
      });
      configurationVariables.Add("type", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
      {
        Value = "Git"
      });
      InputValue inputValue1;
      if (dictionary.TryGetValue("branches", out inputValue1))
        configurationVariables.Add("sourceBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue1.DisplayValue
        });
      InputValue inputValue2;
      if (dictionary.TryGetValue("version", out inputValue2))
      {
        configurationVariables.Add("buildNumber", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue2.DisplayValue
        });
        configurationVariables.Add("buildId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue2.Value
        });
        configurationVariables.Add("sourceVersion", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue2.Value
        });
      }
      InputValue inputValue3;
      if (dictionary.TryGetValue("project", out inputValue3))
      {
        configurationVariables.Add("projectId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue3.Value
        });
        configurationVariables.Add("projectName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue3.DisplayValue
        });
      }
      InputValue inputValue4;
      if (dictionary.TryGetValue("definition", out inputValue4))
      {
        configurationVariables.Add("definitionName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue4.DisplayValue
        });
        configurationVariables.Add("defintionId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue4.Value
        });
        configurationVariables.Add("definitionId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue4.Value
        });
        configurationVariables.Add("repository.name", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue4.DisplayValue
        });
        configurationVariables.Add("repository.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue4.Value
        });
      }
      InputValue inputValue5;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestId, out inputValue5))
        configurationVariables.Add("pullrequest.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue5.Value
        });
      InputValue inputValue6;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestTargetBranch, out inputValue6))
      {
        configurationVariables.Add("pullrequest.targetBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue6.Value
        });
        configurationVariables.Add("pullrequest.targetBranchName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = ArtifactTypeBase.RefToBranchName(inputValue6.Value)
        });
      }
      InputValue inputValue7;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestSourceBranch, out inputValue7))
      {
        configurationVariables.Add("pullrequest.sourceBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue7.Value
        });
        configurationVariables.Add("pullrequest.sourceBranchName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = ArtifactTypeBase.RefToBranchName(inputValue7.Value)
        });
      }
      Guid result;
      if (Guid.TryParse(inputValue3?.Value, out result) && inputValue4 != null)
      {
        try
        {
          GitRepository repository = new GitHelper((IGitData) new GitData()).GetRepository(context, result, inputValue4.Value);
          configurationVariables.Add("repository.uri", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
          {
            Value = repository.RemoteUrl
          });
        }
        catch (Exception ex)
        {
          context.TraceException(1971046, "ReleaseManagementService", "Service", ex);
        }
      }
      return (IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>) configurationVariables;
    }

    public override Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      return (Uri) null;
    }

    public override Uri GetArtifactSourceDefinitionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      return (Uri) null;
    }

    public override string GetDefaultSourceAlias(ArtifactSource artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      return artifact.DefinitionsData.DisplayValue;
    }

    private static void AddGitItemsToList(IList<GitItem> gitItems, InputValues inputValues)
    {
      if (gitItems == null)
        return;
      foreach (GitItem gitItem in (IEnumerable<GitItem>) gitItems)
      {
        string str = gitItem.Path.TrimStart('/');
        if (!string.IsNullOrEmpty(str))
          inputValues.PossibleValues.Add(new InputValue()
          {
            Value = str,
            DisplayValue = str,
            Data = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "itemType",
                gitItem.GitObjectType == GitObjectType.Tree ? (object) "folder" : (object) "file"
              }
            }
          });
      }
    }

    private static void AddGitItemContentToList(string gitItem, InputValues inputValues)
    {
      if (string.IsNullOrEmpty(gitItem))
        return;
      inputValues.PossibleValues.Add(new InputValue()
      {
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "artifactItemContent",
            (object) gitItem
          }
        }
      });
    }

    private static string GetInputValueNotPresentErrorMessage(string inputName)
    {
      switch (inputName)
      {
        case "branches":
          return Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BranchDetailsNotAvailable;
        case "definition":
          return Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDetailsNotAvailable;
        case "project":
          return Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectDetailsNotAvailable;
        default:
          return string.Empty;
      }
    }

    private static bool GetInputValue(
      IDictionary<string, string> currentInputValues,
      string inputName,
      out string outputValue,
      out string errorMessage)
    {
      if (currentInputValues.TryGetValue(inputName, out outputValue))
      {
        errorMessage = string.Empty;
        return true;
      }
      errorMessage = GitArtifact.GetInputValueNotPresentErrorMessage(inputName);
      return false;
    }
  }
}
