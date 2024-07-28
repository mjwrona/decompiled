// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub.GitHubArtifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub
{
  public class GitHubArtifact : ArtifactTypeBase
  {
    private const string IsSearchable = "isSearchable";
    private readonly Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>> serviceEndpointsRetriever;
    private GitHubArtifact.OutFunc<IVssRequestContext, Guid, string, IDictionary<string, string>, string, IList<InputValue>> getInputValueFunc;

    public GitHubArtifact()
      : this(GitHubArtifact.\u003C\u003EO.\u003C0\u003E__GetServiceEndpoints ?? (GitHubArtifact.\u003C\u003EO.\u003C0\u003E__GetServiceEndpoints = new Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>>(ServiceEndpointHelper.GetServiceEndpoints)), new GitHubArtifact.OutFunc<IVssRequestContext, Guid, string, IDictionary<string, string>, string, IList<InputValue>>(new GitHubHelper().GetInputValues))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required for testablity.")]
    protected GitHubArtifact(
      Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>> serviceEndpointsRetriever,
      GitHubArtifact.OutFunc<IVssRequestContext, Guid, string, IDictionary<string, string>, string, IList<InputValue>> getInputValueFunc)
    {
      this.serviceEndpointsRetriever = serviceEndpointsRetriever;
      this.getInputValueFunc = getInputValueFunc;
    }

    public override string Name => "GitHub";

    public override string DisplayName => Resources.GitHubArtifactDisplayName;

    public override string EndpointTypeId => "GitHub";

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

    public override string UniqueSourceIdentifier => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{{{{0}}}}}:{{{{{1}}}}}", (object) "connection", (object) "definition");

    public override bool IsCommitsTraceabilitySupported => true;

    public override bool IsWorkitemsTraceabilitySupported => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = Resources.ServiceName,
        Description = Resources.ServiceConnectionDescriptionGitHub,
        InputMode = InputMode.Combo,
        Id = "connection",
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
        Name = Resources.RepositoryLabel,
        Description = Resources.RepositoryDescription,
        InputMode = InputMode.Combo,
        Type = "gitHubRepositoryPicker",
        Id = "definition",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "connection"
        },
        Properties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "isSearchable",
            (object) true
          }
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = "gitHubRepositoryId",
        Description = "gitHubRepositoryId",
        InputMode = InputMode.None,
        Id = "gitHubRepositoryId",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.Number
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Resources.DefaultBranchDisplayName,
        Description = Resources.DefaultBranchDescription,
        InputMode = InputMode.Combo,
        Id = "branch",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "connection",
          "definition"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Resources.DisplayNameForDefaultVersion,
        Description = Resources.DescriptionForDefaultVersion,
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
          "connection",
          "definition",
          "branch"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Resources.DisplayNameForDefaultVersionSpecificCommit,
        Description = Resources.DescriptionForDefaultVersionSpecificCommit,
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
          "connection",
          "definition",
          "defaultVersionType",
          "branch"
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
        Name = Resources.CheckoutSubmodulesInput,
        Description = Resources.CheckoutSubmodulesInput,
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
        Name = Resources.SubmoduleCheckoutRecursionLevelInput,
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
              DisplayValue = Resources.CheckoutSubmoduleTopLevelOnlyLabel
            },
            new InputValue()
            {
              Value = bool.TrueString,
              DisplayValue = Resources.CheckoutSubmoduleAllNestedLabel
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
        Name = Resources.CheckoutFromLFSInput,
        Description = Resources.CheckoutFromLFSDescription,
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
        Name = Resources.CheckoutDepthInput,
        Description = Resources.CheckoutDepthDescription,
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
        Name = Resources.DisplayNameForArtifacts,
        Description = Resources.DisplayNameForArtifacts,
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
          "connection",
          "definition",
          "defaultVersionType",
          "branch",
          "defaultVersionSpecific"
        },
        HasDynamicValueInformation = true
      }
    };

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
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
      string errorMessage;
      IList<InputValue> possibleValues = this.GetPossibleValues(context, projectInfo.Id, inputId, currentInputValues, out errorMessage);
      bool flag = !string.Equals(inputId, "definition");
      InputValues inputValues = new InputValues();
      inputValues.InputId = inputId;
      inputValues.PossibleValues = possibleValues;
      inputValues.DefaultValue = string.Empty;
      inputValues.IsLimitedToPossibleValues = flag;
      inputValues.IsDisabled = false;
      inputValues.IsReadOnly = false;
      InputValuesError inputValuesError;
      if (errorMessage != null)
        inputValuesError = new InputValuesError()
        {
          Message = errorMessage
        };
      else
        inputValuesError = (InputValuesError) null;
      inputValues.Error = inputValuesError;
      return inputValues;
    }

    public override AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      InputValue sourceInput1 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "connection", true, Resources.ServiceEndPointIdNotPresent);
      InputValue sourceInput2 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "definition", true, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.RepositoryDetailsNotAvailable, (object) serverArtifact.SourceDataKeys, (object) "definition"));
      InputValue sourceInput3 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "gitHubRepositoryId", false, string.Empty);
      InputValue sourceInput4 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "checkoutSubmodules", false, string.Empty);
      InputValue sourceInput5 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "checkoutNestedSubmodules", false, string.Empty);
      InputValue sourceInput6 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "gitLfsSupport", false, string.Empty);
      InputValue sourceInput7 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "fetchDepth", false, string.Empty);
      InputValue sourceInput8 = ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "version", false, string.Empty);
      string empty = string.Empty;
      object obj;
      if (sourceInput8?.Data != null && sourceInput8.Data.TryGetValue("branch", out obj) && obj != null)
        empty = obj.ToString();
      string str = JsonConvert.SerializeObject((object) new Dictionary<string, string>()
      {
        {
          "ConnectionName",
          sourceInput1.Value
        },
        {
          "repository",
          sourceInput2.Value
        },
        {
          "gitHubRepositoryId",
          sourceInput3?.Value
        },
        {
          "branch",
          empty
        },
        {
          "checkoutSubmodules",
          sourceInput4 != null ? sourceInput4.Value : string.Empty
        },
        {
          "checkoutNestedSubmodules",
          sourceInput5 != null ? sourceInput5.Value : string.Empty
        },
        {
          "gitLfsSupport",
          sourceInput6 != null ? sourceInput6.Value : string.Empty
        },
        {
          "fetchDepth",
          sourceInput7 != null ? sourceInput7.Value : string.Empty
        }
      });
      return new AgentArtifactDefinition()
      {
        Name = serverArtifact.Name,
        Alias = serverArtifact.Alias,
        Version = serverArtifact.ArtifactVersion,
        ArtifactType = AgentArtifactType.GitHub,
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
      string errorMessage;
      IList<InputValue> possibleValues = this.GetPossibleValues(requestContext, projectInfo.Id, "commits", sourceInputs, out errorMessage);
      if (errorMessage.IsNullOrEmpty<char>())
        return possibleValues;
      requestContext.Trace(1900000, TraceLevel.Error, "ReleaseManagementService", "Service", errorMessage);
      throw new ReleaseManagementExternalServiceException(errorMessage);
    }

    public override InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      return this.GetAvailableVersions(requestContext, sourceInputs, projectInfo).FirstOrDefault<InputValue>();
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should catch all exceptions because we can try without authentication too")]
    public override bool? UsesExternalAndPublicSourceRepo(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      Guid projectId,
      object artifactContext)
    {
      if (requestContext == null || sourceData == null)
        return new bool?();
      InputValue inputValue1;
      InputValue inputValue2;
      Guid result;
      if (!sourceData.TryGetValue("definition", out inputValue1) || string.IsNullOrEmpty(inputValue1?.Value) || !sourceData.TryGetValue("connection", out inputValue2) || string.IsNullOrEmpty(inputValue2.Value) || !Guid.TryParse(inputValue2.Value, out result))
        return new bool?(false);
      GitHubAuthentication authentication = (GitHubAuthentication) null;
      try
      {
        ServiceEndpoint serviceEndpoint = ServiceEndpointHelper.GetServiceEndpoint(requestContext, projectId, result);
        if (serviceEndpoint != null)
          authentication = serviceEndpoint.GetGitHubAuthentication(requestContext, projectId);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1971017, TraceLevel.Error, "ReleaseManagementService", "Service", ex.Message);
      }
      string repository = inputValue1.Value;
      return new bool?(ArtifactTypeUtility.IsGitHubRepoPublic(requestContext, authentication, repository));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is because of dependency on many classes.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
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
      IList<Change> changes = (IList<Change>) new List<Change>();
      InputValue inputValue1;
      InputValue inputValue2;
      if (sourceData.TryGetValue("connection", out inputValue1) && inputValue1 != null && sourceData.TryGetValue("definition", out inputValue2) && inputValue2 != null)
      {
        string input = inputValue1.Value;
        string repositoryName = inputValue2.Value;
        Guid result;
        if (!string.IsNullOrEmpty(input) && Guid.TryParse(input, out result) && !string.IsNullOrEmpty(repositoryName))
        {
          GitHubData.V3.CommitListItem[] commits;
          string errorMessage;
          new GitHubEndpointHelper().GetCommitsDiff(requestContext, projectInfo.Id, result, repositoryName, startVersion?.Value, endVersion?.Value, out commits, out errorMessage);
          if (!((IEnumerable<GitHubData.V3.CommitListItem>) commits).IsNullOrEmpty<GitHubData.V3.CommitListItem>() && errorMessage.IsNullOrEmpty<char>())
            changes = ArtifactTypeUtility.ConvertGitHubCommitsToChanges(top > 0 ? (IEnumerable<GitHubData.V3.CommitListItem>) ((IEnumerable<GitHubData.V3.CommitListItem>) commits).Take<GitHubData.V3.CommitListItem>(top).ToArray<GitHubData.V3.CommitListItem>() : (IEnumerable<GitHubData.V3.CommitListItem>) commits);
        }
      }
      return changes;
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Justification = "By design")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "3#", Justification = "By design")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Currently only using in telemetry should not fail")]
    public override IList<Change> GetChangesBetweenArtifactSource(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineArtifactSource currentReleaseArtifactSource,
      PipelineArtifactSource lastReleaseArtifactSource,
      int top)
    {
      if (currentReleaseArtifactSource == null)
        return (IList<Change>) new List<Change>();
      try
      {
        return this.GetChanges(requestContext, currentReleaseArtifactSource.SourceData, lastReleaseArtifactSource?.Version, currentReleaseArtifactSource.Version, new ProjectInfo()
        {
          Id = projectId
        }, top, (object) null);
      }
      catch
      {
        return (IList<Change>) new List<Change>();
      }
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
        Value = "GitHub"
      });
      configurationVariables.Add("type", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
      {
        Value = "GitHub"
      });
      InputValue inputValue1;
      if (dictionary.TryGetValue("version", out inputValue1))
      {
        configurationVariables.Add("buildId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue1.Value
        });
        configurationVariables.Add("buildNumber", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue1.DisplayValue
        });
        configurationVariables.Add("sourceVersion", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue1.Value
        });
      }
      InputValue inputValue2;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestId, out inputValue2))
        configurationVariables.Add("pullrequest.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue2.Value
        });
      InputValue inputValue3;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestTargetBranch, out inputValue3))
      {
        configurationVariables.Add("pullrequest.targetBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue3.Value
        });
        configurationVariables.Add("pullrequest.targetBranchName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = ArtifactTypeBase.RefToBranchName(inputValue3.Value)
        });
      }
      InputValue inputValue4;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestSourceBranch, out inputValue4))
      {
        configurationVariables.Add("pullrequest.sourceBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue4.Value
        });
        configurationVariables.Add("pullrequest.sourceBranchName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = ArtifactTypeBase.RefToBranchName(inputValue4.Value)
        });
      }
      InputValue inputValue5;
      InputValue inputValue6;
      if (dictionary.TryGetValue("definition", out inputValue5) && dictionary.TryGetValue("branch", out inputValue6))
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://github.com/{0}", (object) inputValue5.Value);
        configurationVariables.Add("definitionId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue5.Value
        });
        configurationVariables.Add("definitionName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) inputValue5.Value, (object) inputValue6.Value)
        });
        configurationVariables.Add("repository.name", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue5.DisplayValue
        });
        configurationVariables.Add("repository.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue5.Value
        });
        configurationVariables.Add("sourceBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue6.Value
        });
        configurationVariables.Add("buildUri", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = str
        });
        configurationVariables.Add("repository.uri", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = str
        });
      }
      return (IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>) configurationVariables;
    }

    public override Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      Uri sourceVersionUrl = (Uri) null;
      InputValue inputValue1;
      InputValue inputValue2;
      if (serverArtifact.SourceData.TryGetValue("version", out inputValue1) && !string.IsNullOrEmpty(inputValue1?.Value) && serverArtifact.SourceData.TryGetValue("definition", out inputValue2) && !string.IsNullOrEmpty(inputValue2?.Value))
        sourceVersionUrl = new GitHubRoot().RepositoryUri(inputValue2.Value).AppendPathSegments("commit", inputValue1.Value).Uri;
      return sourceVersionUrl;
    }

    public override Uri GetArtifactSourceDefinitionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      Uri sourceDefinitionUrl = (Uri) null;
      InputValue inputValue;
      if (serverArtifact.SourceData.TryGetValue("definition", out inputValue) && !string.IsNullOrEmpty(inputValue?.Value))
        sourceDefinitionUrl = new GitHubRoot().RepositoryUri(inputValue.Value).Uri;
      return sourceDefinitionUrl;
    }

    public override string GetDefaultSourceAlias(ArtifactSource artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      return artifact.DefinitionsData.DisplayValue;
    }

    private IList<InputValue> GetPossibleValues(
      IVssRequestContext requestContext,
      Guid projectId,
      string inputId,
      IDictionary<string, string> currentValues,
      out string errorMessage)
    {
      errorMessage = (string) null;
      if (string.Equals(inputId, "connection", StringComparison.OrdinalIgnoreCase))
      {
        List<InputValue> possibleValues = new List<InputValue>();
        possibleValues.AddRange(this.serviceEndpointsRetriever(requestContext, projectId, "GitHub").Select<ServiceEndpoint, InputValue>((Func<ServiceEndpoint, InputValue>) (se => new InputValue()
        {
          Value = se.Id.ToString(),
          DisplayValue = se.Name
        })));
        return (IList<InputValue>) possibleValues;
      }
      if (!requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.EnableGitHubDataSourcesForGitHubArtifact"))
        return this.getInputValueFunc(requestContext, projectId, inputId, currentValues, out errorMessage);
      this.getInputValueFunc = new GitHubArtifact.OutFunc<IVssRequestContext, Guid, string, IDictionary<string, string>, string, IList<InputValue>>(new GitHubEndpointHelper().GetInputValues);
      return this.getInputValueFunc(requestContext, projectId, inputId, currentValues, out errorMessage);
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "This is required to mock the external methods")]
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This is required to mock the external methods")]
    public delegate TResult OutFunc<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      out TArg5 arg5);
  }
}
