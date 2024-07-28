// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc.TfvcArtifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Newtonsoft.Json;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc
{
  public class TfvcArtifact : ArtifactTypeBase
  {
    public const int ChangesetsCount = 100;
    private const string ProjectId = "project";
    private const string DefinitionId = "definition";
    private const string DefaultVersionTypeId = "defaultVersionType";
    private const string DefaultVersionSpecificInputId = "defaultVersionSpecific";
    private const string VisibleRule = "visibleRule";
    [StaticSafe]
    private static readonly IDictionary<string, string> InputValueNotPresentErrorsDictionary = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "project",
        Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectDetailsNotAvailable
      }
    };
    private readonly ITfvcClient tfvcClient;

    public override string Name => "TFVC";

    public override string DisplayName => Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.TfvcDisplayName;

    public override string EndpointTypeId => "TFVC";

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

    public override bool IsCommitsTraceabilitySupported => false;

    public override bool IsWorkitemsTraceabilitySupported => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Id = "project",
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForProject,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForProject,
        InputMode = InputMode.Combo,
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(300)
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Id = "definition",
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryLabel,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDescription,
        InputMode = InputMode.Combo,
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(300)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "project"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Id = "defaultVersionType",
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersion,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersion,
        InputMode = InputMode.Combo,
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
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersionSpecificChangeset,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersionSpecificChangeset,
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
          "defaultVersionType"
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
        Id = "artifacts",
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForArtifacts,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForArtifacts,
        InputMode = InputMode.None,
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String,
          MaxLength = new int?(300)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "project",
          "definition",
          "defaultVersionType",
          "defaultVersionSpecific"
        },
        HasDynamicValueInformation = true
      }
    };

    public TfvcArtifact()
      : this((ITfvcClient) new TfvcClient())
    {
    }

    protected TfvcArtifact(ITfvcClient tfvcClient) => this.tfvcClient = tfvcClient;

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
      string errorMessage = (string) null;
      IList<InputValue> possibleValues = this.GetPossibleValues(context, projectInfo, inputId, currentInputValues, out errorMessage);
      InputValuesError inputValuesError1;
      if (errorMessage != null)
        inputValuesError1 = new InputValuesError()
        {
          Message = errorMessage
        };
      else
        inputValuesError1 = (InputValuesError) null;
      InputValuesError inputValuesError2 = inputValuesError1;
      return new InputValues()
      {
        InputId = inputId,
        PossibleValues = possibleValues,
        DefaultValue = string.Empty,
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = false,
        Error = inputValuesError2
      };
    }

    public override AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      string str = JsonConvert.SerializeObject((object) new Dictionary<string, string>()
      {
        {
          "project",
          ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "project", true, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectDetailsNotAvailable, (object) serverArtifact.SourceDataKeys, (object) "project")).Value
        },
        {
          "repository",
          ArtifactTypeBase.GetSourceInput((IDictionary<string, InputValue>) serverArtifact.SourceData, "definition", true, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDetailsNotAvailable, (object) serverArtifact.SourceDataKeys, (object) "definition")).Value
        }
      });
      return new AgentArtifactDefinition()
      {
        Name = serverArtifact.Name,
        Alias = serverArtifact.Alias,
        Version = serverArtifact.ArtifactVersion,
        ArtifactType = AgentArtifactType.Tfvc,
        Details = str
      };
    }

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
      Guid projectId;
      string errorMessage;
      if (!TfvcArtifact.GetProjectId(sourceInputs, out projectId, out errorMessage))
        throw new InvalidRequestException(errorMessage);
      string empty = string.Empty;
      List<InputValue> availableVersions = new List<InputValue>();
      string a;
      sourceInputs.TryGetValue("defaultVersionType", out a);
      if (string.Equals(a, "specificVersionType", StringComparison.OrdinalIgnoreCase))
      {
        InputValue inputValue = sourceInputs.TryGetValue("defaultVersionSpecific", out empty) && !string.IsNullOrEmpty(empty) ? new InputValue()
        {
          DisplayValue = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) "Changeset", (object) empty),
          Value = empty
        } : throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoSpecificVersionValueAvailableForSpecificVersionType));
        availableVersions.Add(inputValue);
      }
      else
        availableVersions = new TfvcHelper(this.tfvcClient).GetChangesets(requestContext, projectId, 100).Select<TfvcChangesetRef, InputValue>((Func<TfvcChangesetRef, InputValue>) (changeset => new InputValue()
        {
          DisplayValue = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) "Changeset", (object) changeset.ChangesetId),
          Value = changeset.ChangesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        })).ToList<InputValue>();
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
      return (IList<Change>) new List<Change>();
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

    protected override IDictionary<string, ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Dictionary<string, InputValue> dictionary = artifactSource != null ? artifactSource.SourceData : throw new ArgumentNullException(nameof (artifactSource));
      Dictionary<string, ConfigurationVariableValue> configurationVariables = new Dictionary<string, ConfigurationVariableValue>();
      InputValue inputValue1;
      if (dictionary.TryGetValue("definition", out inputValue1))
      {
        configurationVariables.Add("definitionId", new ConfigurationVariableValue()
        {
          Value = inputValue1.Value
        });
        configurationVariables.Add("definitionName", new ConfigurationVariableValue()
        {
          Value = inputValue1.DisplayValue
        });
      }
      InputValue inputValue2;
      if (dictionary.TryGetValue("version", out inputValue2))
      {
        configurationVariables.Add("buildId", new ConfigurationVariableValue()
        {
          Value = inputValue2.Value
        });
        configurationVariables.Add("buildNumber", new ConfigurationVariableValue()
        {
          Value = inputValue2.DisplayValue
        });
      }
      InputValue inputValue3;
      if (dictionary.TryGetValue("project", out inputValue3))
      {
        configurationVariables.Add("projectId", new ConfigurationVariableValue()
        {
          Value = inputValue3.Value
        });
        configurationVariables.Add("projectName", new ConfigurationVariableValue()
        {
          Value = inputValue3.DisplayValue
        });
      }
      return (IDictionary<string, ConfigurationVariableValue>) configurationVariables;
    }

    public override Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      Uri sourceVersionUrl = (Uri) null;
      if (serverArtifact.SourceData.ContainsKey("version") && serverArtifact.SourceData.ContainsKey("project"))
      {
        string str1 = ArtifactTypeBase.GetUriFromRequestContext(requestContext).ToString();
        string str2 = ArtifactTypeBase.GetCollectionId(requestContext).ToString("D");
        string str3 = serverArtifact.SourceData["version"].Value;
        sourceVersionUrl = new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_permalink/_versionControl/changeset/{1}?collectionId={2}&projectId={3}", (object) str1, (object) str3, (object) str2, (object) serverArtifact.SourceData["project"].Value), UriKind.Absolute);
      }
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
      if (serverArtifact.SourceData.ContainsKey("project"))
        sourceDefinitionUrl = new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_permalink/_versionControl?collectionId={1}&projectId={2}", (object) ArtifactTypeBase.GetUriFromRequestContext(requestContext).ToString(), (object) ArtifactTypeBase.GetCollectionId(requestContext).ToString("D"), (object) serverArtifact.SourceData["project"].Value), UriKind.Absolute);
      return sourceDefinitionUrl;
    }

    public override string GetDefaultSourceAlias(ArtifactSource artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      return artifact.DefinitionsData.DisplayValue;
    }

    private static bool GetProjectId(
      IDictionary<string, string> currentInputValues,
      out Guid projectId,
      out string errorMessage)
    {
      projectId = Guid.Empty;
      string inputValue;
      if (!TfvcArtifact.GetInputValue(currentInputValues, "project", out inputValue, out errorMessage))
        return false;
      if (Guid.TryParse(inputValue, out projectId))
        return true;
      errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidProject;
      return false;
    }

    private static bool GetInputValue(
      IDictionary<string, string> currentInputValues,
      string inputName,
      out string inputValue,
      out string errorMessage)
    {
      if (currentInputValues.TryGetValue(inputName, out inputValue) && inputValue != null)
      {
        errorMessage = string.Empty;
        return true;
      }
      string str = string.Join(",", (IEnumerable<string>) currentInputValues.Keys);
      errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, TfvcArtifact.InputValueNotPresentErrorsDictionary[inputName], (object) str, (object) "project");
      return false;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    private IList<InputValue> GetPossibleValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues,
      out string errorMessage)
    {
      errorMessage = (string) null;
      List<InputValue> possibleValues = new List<InputValue>();
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
                possibleValues.Add(new InputValue()
                {
                  Value = projectInfo.Id.ToString(),
                  DisplayValue = projectInfo.Name
                });
                possibleValues.AddRange(source.Select<ProjectInfo, InputValue>((Func<ProjectInfo, InputValue>) (info => new InputValue()
                {
                  Value = info.Id.ToString(),
                  DisplayValue = info.Name
                })));
                goto label_30;
              }
              else
                goto label_27;
            case 9:
              if (inputId == "artifacts")
                break;
              goto label_27;
            case 10:
              if (inputId == "definition")
              {
                Guid projectId;
                if (TfvcArtifact.GetProjectId(currentInputValues, out projectId, out errorMessage))
                {
                  string projectPath;
                  if (!this.HasTfvcSupport(context, projectId, out projectPath))
                  {
                    errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ExpectedRepositoryNotFound, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.TfvcDisplayName);
                    goto label_30;
                  }
                  else
                  {
                    possibleValues.Add(new InputValue()
                    {
                      Value = projectId.ToString(),
                      DisplayValue = projectPath
                    });
                    goto label_30;
                  }
                }
                else
                  goto label_30;
              }
              else
                goto label_27;
            case 13:
              if (inputId == "artifactItems")
                break;
              goto label_27;
            case 18:
              if (inputId == "defaultVersionType")
              {
                if (TfvcArtifact.GetProjectId(currentInputValues, out Guid _, out errorMessage))
                {
                  if (TfvcArtifact.GetInputValue(currentInputValues, "definition", out string _, out errorMessage))
                  {
                    possibleValues.Add(new InputValue()
                    {
                      Value = "selectDuringReleaseCreationType",
                      DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.SelectDuringReleaseCreationType
                    });
                    possibleValues.Add(new InputValue()
                    {
                      Value = "latestType",
                      DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.LatestType
                    });
                    possibleValues.Add(new InputValue()
                    {
                      Value = "specificVersionType",
                      DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.SpecificChangesetType
                    });
                    goto label_30;
                  }
                  else
                    goto label_30;
                }
                else
                  goto label_30;
              }
              else
                goto label_27;
            case 19:
              if (inputId == "artifactItemContent")
              {
                Guid projectId;
                if (TfvcArtifact.GetProjectId(currentInputValues, out projectId, out errorMessage))
                {
                  string projectName = ProjectHelper.GetProjectName(context, projectId);
                  string path = '$'.ToString() + "/" + projectName + "/" + currentInputValues["itemPath"];
                  string itemContent = new TfvcHelper(this.tfvcClient).GetItemContent(context, path);
                  possibleValues.Add(new InputValue()
                  {
                    Data = (IDictionary<string, object>) new Dictionary<string, object>()
                    {
                      {
                        "artifactItemContent",
                        (object) itemContent
                      }
                    }
                  });
                  goto label_30;
                }
                else
                  goto label_30;
              }
              else
                goto label_27;
            case 22:
              if (inputId == "defaultVersionSpecific")
              {
                string empty = string.Empty;
                if (currentInputValues.TryGetValue("defaultVersionType", out empty))
                {
                  if (string.Equals(empty, "specificVersionType", StringComparison.OrdinalIgnoreCase))
                  {
                    currentInputValues.Remove("defaultVersionType");
                    return this.GetAvailableVersions(context, currentInputValues, projectInfo);
                  }
                  goto label_30;
                }
                else
                  goto label_30;
              }
              else
                goto label_27;
            default:
              goto label_27;
          }
          Guid projectId1;
          if (TfvcArtifact.GetProjectId(currentInputValues, out projectId1, out errorMessage))
          {
            string empty;
            if (!currentInputValues.TryGetValue("itemPath", out empty))
              empty = string.Empty;
            string defaultVersionSpecific;
            currentInputValues.TryGetValue("defaultVersionSpecific", out defaultVersionSpecific);
            possibleValues.AddRange((IEnumerable<InputValue>) this.GetArtifacts(context, projectId1, empty, defaultVersionSpecific));
            goto label_30;
          }
          else
            goto label_30;
        }
label_27:
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InputTypeNotSupported, (object) inputId);
      }
      catch (AggregateException ex)
      {
        errorMessage = ex.InnerException.Message;
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
      }
label_30:
      return (IList<InputValue>) possibleValues;
    }

    private bool HasTfvcSupport(IVssRequestContext context, Guid projectId, out string projectPath)
    {
      TfvcVersionDescriptor versionDescriptor = new TfvcVersionDescriptor()
      {
        VersionOption = TfvcVersionOption.None,
        VersionType = TfvcVersionType.Latest,
        Version = (string) null
      };
      IList<TfvcItem> items = new TfvcHelper(this.tfvcClient).GetItems(context, string.Empty, VersionControlRecursionType.OneLevel, versionDescriptor);
      string projectName = ProjectHelper.GetProjectName(context, projectId);
      projectPath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) '$', (object) projectName);
      string projectPathLocal = projectPath;
      Func<TfvcItem, bool> predicate = (Func<TfvcItem, bool>) (x => string.Equals(x.Path, projectPathLocal, StringComparison.OrdinalIgnoreCase));
      return items.FirstOrDefault<TfvcItem>(predicate) != null;
    }

    private IList<InputValue> GetArtifacts(
      IVssRequestContext context,
      Guid projectId,
      string scopePath,
      string defaultVersionSpecific)
    {
      string projectName = ProjectHelper.GetProjectName(context, projectId);
      string str = '$'.ToString() + "/";
      string rootWithProjectFolder = str + projectName;
      if (string.IsNullOrEmpty(scopePath))
        scopePath = rootWithProjectFolder;
      else if (!scopePath.StartsWith(str, StringComparison.OrdinalIgnoreCase))
        scopePath = rootWithProjectFolder + "/" + scopePath;
      TfvcVersionDescriptor versionDescriptor = new TfvcVersionDescriptor()
      {
        VersionOption = TfvcVersionOption.None,
        VersionType = TfvcVersionType.Latest,
        Version = defaultVersionSpecific
      };
      return (IList<InputValue>) new TfvcHelper(this.tfvcClient).GetItems(context, projectId, scopePath, VersionControlRecursionType.OneLevel, versionDescriptor).Where<TfvcItem>((Func<TfvcItem, bool>) (item => !item.Path.Equals(scopePath, StringComparison.OrdinalIgnoreCase))).ToList<TfvcItem>().Select<TfvcItem, InputValue>((Func<TfvcItem, InputValue>) (item => new InputValue()
      {
        Value = item.Path.StartsWith(rootWithProjectFolder + "/", StringComparison.OrdinalIgnoreCase) ? item.Path.Substring(rootWithProjectFolder.Length + 1) : item.Path,
        DisplayValue = ((IEnumerable<string>) item.Path.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.RemoveEmptyEntries)).Last<string>(),
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "itemType",
            item.IsFolder ? (object) "folder" : (object) "file"
          }
        }
      })).ToList<InputValue>();
    }
  }
}
