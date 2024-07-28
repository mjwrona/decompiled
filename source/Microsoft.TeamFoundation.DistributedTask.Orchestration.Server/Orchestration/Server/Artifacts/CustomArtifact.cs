// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.CustomArtifact
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class CustomArtifact : ArtifactTypeBase
  {
    public const string ServicesId = "connection";
    private readonly ArtifactContributionDefinition artifactContributionDefinition;
    private readonly Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>> serviceEndpointsRetriever;
    private readonly Func<IVssRequestContext, Guid, string, ServiceEndpointType> serviceEndpointTypeRetriever;
    private readonly Func<IVssRequestContext, Guid, Guid, ServiceEndpoint> serviceEndpointRetriever;
    private readonly Func<IVssRequestContext, Guid, DataSourceBinding, IDictionary<string, string>, IDictionary<string, string>, IList<InputValue>> executeServiceEndpointRequest;
    private const string ArtifactTypeVariable = "artifactType";
    private const string VersionVariable = "version";

    public CustomArtifact(
      string contributionId,
      ArtifactContributionDefinition artifactContributionDefinition)
      : this(contributionId, CustomArtifact.\u003C\u003EO.\u003C0\u003E__GetServiceEndpoints ?? (CustomArtifact.\u003C\u003EO.\u003C0\u003E__GetServiceEndpoints = new Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>>(ServiceEndpointHelper.GetServiceEndpoints)), CustomArtifact.\u003C\u003EO.\u003C1\u003E__GetServiceEndpointType ?? (CustomArtifact.\u003C\u003EO.\u003C1\u003E__GetServiceEndpointType = new Func<IVssRequestContext, Guid, string, ServiceEndpointType>(ServiceEndpointHelper.GetServiceEndpointType)), ServiceEndpointHelper.GetServiceEndpoint, CustomArtifact.\u003C\u003EO.\u003C2\u003E__ExecuteServiceEndpointRequest ?? (CustomArtifact.\u003C\u003EO.\u003C2\u003E__ExecuteServiceEndpointRequest = new Func<IVssRequestContext, Guid, DataSourceBinding, IDictionary<string, string>, IDictionary<string, string>, IList<InputValue>>(ServiceEndpointHelper.ExecuteServiceEndpointRequest)), artifactContributionDefinition)
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected CustomArtifact(
      string contributionId,
      Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>> serviceEndpointsRetriever,
      Func<IVssRequestContext, Guid, string, ServiceEndpointType> serviceEndpointTypeRetriever,
      Func<IVssRequestContext, Guid, Guid, ServiceEndpoint> serviceEndpointRetriever,
      Func<IVssRequestContext, Guid, DataSourceBinding, IDictionary<string, string>, IDictionary<string, string>, IList<InputValue>> executeServiceEndpointRequest,
      ArtifactContributionDefinition artifactContributionDefinition)
    {
      this.artifactContributionDefinition = artifactContributionDefinition;
      this.serviceEndpointsRetriever = serviceEndpointsRetriever;
      this.serviceEndpointTypeRetriever = serviceEndpointTypeRetriever;
      this.serviceEndpointRetriever = serviceEndpointRetriever;
      this.executeServiceEndpointRequest = executeServiceEndpointRequest;
      string str;
      if (contributionId == null)
        str = string.Empty;
      else if (!this.IsMsExtension(contributionId))
        str = contributionId;
      else
        str = ((IEnumerable<string>) contributionId.Split('.')).Last<string>();
      this.Type = str;
    }

    public override Guid ArtifactDownloadTaskId
    {
      get
      {
        Guid result;
        return Guid.TryParse(this.artifactContributionDefinition.DownloadTaskId, out result) ? result : Guid.Empty;
      }
    }

    public override IDictionary<string, string> TaskInputMapping => this.artifactContributionDefinition.TaskInputMapping;

    public override string ArtifactType => this.artifactContributionDefinition.ArtifactType;

    public override IDictionary<string, string> TaskInputDefaultValues { get; }

    public override string Type { get; }

    public override IDictionary<string, Guid> ArtifactDownloadTaskIds => this.artifactContributionDefinition.DownloadTaskIds;

    public override IDictionary<Guid, IDictionary<string, string>> TaskInputMappings => this.artifactContributionDefinition.TaskInputMappings;

    public IDictionary<string, string> TaskInputDefaultVaules => (IDictionary<string, string>) new Dictionary<string, string>();

    public string ArtifactTypeId => this.artifactContributionDefinition.Name;

    public override string Name => this.ArtifactTypeId;

    public override string DisplayName => this.artifactContributionDefinition.DisplayName;

    public override string EndpointTypeId => this.artifactContributionDefinition.EndpointTypeId ?? this.artifactContributionDefinition.Name;

    public override IDictionary<string, string> YamlInputMapping => this.artifactContributionDefinition.YamlInputMapping;

    public override bool ResolveStep(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      resolvedSteps = (IList<TaskStep>) new List<TaskStep>();
      return false;
    }

    public override string UniqueSourceIdentifier => this.artifactContributionDefinition.UniqueSourceIdentifier;

    public override sealed IList<InputDescriptor> InputDescriptors => this.artifactContributionDefinition.InputDescriptors == null ? (IList<InputDescriptor>) null : (IList<InputDescriptor>) this.artifactContributionDefinition.InputDescriptors.ToList<InputDescriptor>();

    public override sealed ArtifactTriggerConfiguration ArtifactTriggerConfiguration => this.artifactContributionDefinition.ArtifactTriggerConfiguration;

    public override bool IsCommitsTraceabilitySupported => this.artifactContributionDefinition.IsCommitsTraceabilitySupported;

    public override bool IsWorkitemsTraceabilitySupported => this.artifactContributionDefinition.IsWorkitemsTraceabilitySupported;

    public override void FillQueuePipelineDataParameters(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      WebHookEventPayloadInputMapper inputMapper)
    {
      foreach (KeyValuePair<string, string> allInputValue in (IEnumerable<KeyValuePair<string, string>>) inputMapper.GetAllInputValues())
        build.TriggerInfo.Add(allInputValue.Key, allInputValue.Value);
      if (this.YamlInputMapping == null || !this.YamlInputMapping.Any<KeyValuePair<string, string>>())
        return;
      foreach (string key in (IEnumerable<string>) this.YamlInputMapping.Keys)
      {
        if (build.TriggerInfo.ContainsKey(this.YamlInputMapping[key]))
          build.TriggerInfo[key] = build.TriggerInfo[this.YamlInputMapping[key]];
      }
    }

    public override InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      string errorMessage;
      IList<InputValue> possibleValues = this.GetPossibleValues(context, projectInfo, inputId, out errorMessage, currentInputValues);
      InputValuesError inputValuesError1;
      if (errorMessage != null)
        inputValuesError1 = new InputValuesError()
        {
          Message = errorMessage
        };
      else
        inputValuesError1 = (InputValuesError) null;
      InputValuesError inputValuesError2 = inputValuesError1;
      bool flag = false;
      if (this.InputDescriptors != null)
      {
        InputDescriptor inputDescriptor = this.InputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (t => t.Id == inputId));
        if (inputDescriptor != null && inputDescriptor.Values != null)
          flag = inputDescriptor.Values.IsLimitedToPossibleValues;
      }
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = string.Empty,
        PossibleValues = possibleValues,
        IsLimitedToPossibleValues = flag,
        IsDisabled = false,
        IsReadOnly = false,
        Error = inputValuesError2
      };
    }

    private bool IsMsExtension(string typeContributionId) => string.Equals(typeContributionId.Split('.')[0], "ms", StringComparison.OrdinalIgnoreCase);

    private static string GetEndpointId(
      DataSourceBinding dataSourceBinding,
      IDictionary<string, string> currentInputValues)
    {
      if (!string.IsNullOrEmpty(dataSourceBinding.EndpointId))
        return new MustacheTemplateParser().ReplaceValues(dataSourceBinding.EndpointId, (object) currentInputValues);
      return currentInputValues.ContainsKey("connection") ? currentInputValues["connection"] : (string) null;
    }

    private static void UpdateRequestContent(
      Dictionary<string, string> currentInputValues,
      DataSourceBinding dataSourceBinding,
      ProjectInfo projectInfo)
    {
      if (dataSourceBinding == null || string.IsNullOrEmpty(dataSourceBinding.RequestContent))
        return;
      EndpointStringResolver endpointStringResolver = new EndpointStringResolver(EndpointMustacheHelper.CreateReplacementContext((ServiceEndpoint) null, currentInputValues, projectInfo.Id));
      string str = endpointStringResolver.ResolveVariablesInMustacheFormat(endpointStringResolver.ResolveVariablesInDollarFormat((ServiceEndpoint) null, dataSourceBinding.RequestContent, new Dictionary<string, string>((IDictionary<string, string>) currentInputValues), throwIfInvalidVariable: false));
      dataSourceBinding.RequestContent = str;
    }

    private static void UpdateCurrentInputValuesForDataSourceParameters(
      Dictionary<string, string> currentInputValues,
      DataSourceBinding dataSourceBinding,
      ProjectInfo projectInfo)
    {
      if (dataSourceBinding == null || dataSourceBinding.Parameters == null || !dataSourceBinding.Parameters.Any<KeyValuePair<string, string>>())
        return;
      foreach (string key in dataSourceBinding.Parameters.Keys)
      {
        if (!currentInputValues.ContainsKey(key))
        {
          string parameter = dataSourceBinding.Parameters[key];
          EndpointStringResolver endpointStringResolver = new EndpointStringResolver(EndpointMustacheHelper.CreateReplacementContext((ServiceEndpoint) null, currentInputValues, projectInfo.Id));
          string str = endpointStringResolver.ResolveVariablesInMustacheFormat(endpointStringResolver.ResolveVariablesInDollarFormat((ServiceEndpoint) null, parameter, new Dictionary<string, string>((IDictionary<string, string>) currentInputValues), throwIfInvalidVariable: false));
          if (!string.IsNullOrEmpty(str))
            currentInputValues.Add(key, str);
        }
      }
    }

    private IList<InputValue> GetPossibleValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      out string errorMessage,
      IDictionary<string, string> currentInputValues)
    {
      errorMessage = (string) null;
      List<InputValue> possibleValues = new List<InputValue>();
      try
      {
        if (this.InputDescriptors != null)
        {
          InputDescriptor inputDescriptor = this.InputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (t => t.Id == inputId));
          if (inputDescriptor != null && !string.IsNullOrWhiteSpace(inputDescriptor.Type) && inputDescriptor.Type.StartsWith("connectedService:", StringComparison.OrdinalIgnoreCase))
          {
            IEnumerable<ServiceEndpoint> source = this.serviceEndpointsRetriever(context, projectInfo.Id, inputDescriptor.Type.Split(':')[1].Trim());
            possibleValues.AddRange(source.Select<ServiceEndpoint, InputValue>((Func<ServiceEndpoint, InputValue>) (endpoint => new InputValue()
            {
              Value = endpoint.Id.ToString(),
              DisplayValue = endpoint.Name
            })));
            return (IList<InputValue>) possibleValues;
          }
        }
        if (inputId == "connection")
        {
          IEnumerable<ServiceEndpoint> source = this.serviceEndpointsRetriever(context, projectInfo.Id, this.artifactContributionDefinition.EndpointTypeId);
          possibleValues.AddRange(source.Select<ServiceEndpoint, InputValue>((Func<ServiceEndpoint, InputValue>) (endpoint => new InputValue()
          {
            Value = endpoint.Id.ToString(),
            DisplayValue = endpoint.Name
          })));
        }
        else
        {
          IDictionary<string, string> dictionary1 = (IDictionary<string, string>) new Dictionary<string, string>(currentInputValues, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          if (string.Equals(inputId, WellKnownArtifactInputs.ArtifactItems, StringComparison.OrdinalIgnoreCase))
          {
            if (!dictionary1.ContainsKey("artifactType"))
            {
              errorMessage = TaskResources.NoArtifactTypeError();
              return (IList<InputValue>) possibleValues;
            }
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>(this.artifactContributionDefinition.BrowsableArtifactTypeMapping ?? (IDictionary<string, string>) new Dictionary<string, string>(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            if (!dictionary2.ContainsKey(dictionary1["artifactType"]))
            {
              errorMessage = TaskResources.ArtifactTypeNotBrowsableError();
              return (IList<InputValue>) possibleValues;
            }
            inputId = dictionary2[dictionary1["artifactType"]];
          }
          DataSourceBinding sourceBindingForInput = this.GetDataSourceBindingForInput(inputId);
          if (sourceBindingForInput != null)
          {
            possibleValues = this.GetPossibleValuesFromDataSource(context, projectInfo, sourceBindingForInput, inputId, dictionary1);
            currentInputValues.Clear();
            currentInputValues.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) dictionary1);
          }
          else
            errorMessage = !string.Equals(inputId, WellKnownArtifactInputs.Artifacts, StringComparison.OrdinalIgnoreCase) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.DataSourceBindingMissing((object) inputId)) : TaskResources.ArtifactTypeNotBrowsableError();
        }
      }
      catch (AggregateException ex)
      {
        errorMessage = ex.Flatten().InnerException.ToString();
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
      }
      return (IList<InputValue>) possibleValues;
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

    private List<InputValue> GetPossibleValuesFromDataSource(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      DataSourceBinding dataSourceBinding,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      List<InputValue> valuesFromDataSource = new List<InputValue>();
      dataSourceBinding.EndpointId = CustomArtifact.GetEndpointId(dataSourceBinding, currentInputValues);
      if (!currentInputValues.ContainsKey("version") && currentInputValues.ContainsKey("defaultVersionType") && string.Equals(currentInputValues.GetDefaultVersionType(), "specificVersionType") && currentInputValues.ContainsKey("defaultVersionSpecific") && !string.IsNullOrEmpty(currentInputValues["defaultVersionSpecific"]) && (string.Equals(inputId, WellKnownArtifactInputs.Artifacts, StringComparison.OrdinalIgnoreCase) || string.Equals(inputId, WellKnownArtifactInputs.ArtifactItems, StringComparison.OrdinalIgnoreCase) || string.Equals(inputId, "artifactItemContent", StringComparison.OrdinalIgnoreCase)))
        currentInputValues.Add("version", currentInputValues["defaultVersionSpecific"]);
      if (!currentInputValues.ContainsKey("version") && (string.Equals(inputId, WellKnownArtifactInputs.Artifacts, StringComparison.OrdinalIgnoreCase) || string.Equals(inputId, WellKnownArtifactInputs.ArtifactDetails, StringComparison.OrdinalIgnoreCase) || string.Equals(inputId, WellKnownArtifactInputs.ArtifactItems, StringComparison.OrdinalIgnoreCase) || string.Equals(inputId, "artifactItemContent", StringComparison.OrdinalIgnoreCase)))
      {
        InputValue latestVersion = this.GetLatestVersion(context, currentInputValues, projectInfo);
        currentInputValues.Add("version", latestVersion.Value);
      }
      Dictionary<string, string> currentInputValues1 = new Dictionary<string, string>(currentInputValues);
      CustomArtifact.UpdateCurrentInputValuesForDataSourceParameters(currentInputValues1, dataSourceBinding, projectInfo);
      CustomArtifact.UpdateRequestContent(currentInputValues1, dataSourceBinding, projectInfo);
      InputValue[] array = this.executeServiceEndpointRequest(context, projectInfo.Id, dataSourceBinding, (IDictionary<string, string>) currentInputValues1, currentInputValues).ToArray<InputValue>();
      if (!string.Equals(inputId, WellKnownArtifactInputs.VersionsInput, StringComparison.OrdinalIgnoreCase) && !string.Equals(inputId, "defaultVersionSpecific", StringComparison.OrdinalIgnoreCase))
        Array.Sort<InputValue>(array, (Comparison<InputValue>) ((x, y) => string.Compare(x.DisplayValue, y.DisplayValue, StringComparison.OrdinalIgnoreCase)));
      IEnumerable<InputValue> list = (IEnumerable<InputValue>) ((IEnumerable<InputValue>) array).ToList<InputValue>();
      if (string.Equals(inputId, WellKnownArtifactInputs.Artifacts, StringComparison.OrdinalIgnoreCase))
      {
        foreach (InputValue inputValue in list)
        {
          if (inputValue.Data != null && !inputValue.Data.ContainsKey("itemType"))
            inputValue.Data.Add("itemType", (object) "folder");
        }
      }
      if (string.Equals(inputId, WellKnownArtifactInputs.ArtifactItems, StringComparison.OrdinalIgnoreCase))
      {
        foreach (InputValue inputValue in list)
        {
          string displayValue = inputValue.DisplayValue;
          string str;
          if (displayValue == null)
            str = (string) null;
          else
            str = ((IEnumerable<string>) displayValue.Split(new char[2]
            {
              '/',
              '\\'
            }, StringSplitOptions.RemoveEmptyEntries)).Last<string>();
          inputValue.DisplayValue = str;
        }
      }
      valuesFromDataSource.AddRange(list);
      return valuesFromDataSource;
    }

    private DataSourceBinding GetDataSourceBindingForInput(string inputId)
    {
      IEnumerable<DataSourceBinding> dataSourceBindings = this.artifactContributionDefinition.DataSourceBindings;
      return dataSourceBindings == null ? (DataSourceBinding) null : dataSourceBindings.FirstOrDefault<DataSourceBinding>((Func<DataSourceBinding, bool>) (binding => string.Compare(binding.Target, inputId, StringComparison.OrdinalIgnoreCase) == 0));
    }

    public override AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      InputValue inputValue = (InputValue) null;
      Dictionary<string, string> details = new Dictionary<string, string>()
      {
        {
          "RelativePath",
          serverArtifact.Path
        },
        {
          "ArtifactType",
          this.artifactContributionDefinition.ArtifactType
        }
      };
      if (serverArtifact.SourceData != null && serverArtifact.SourceData.TryGetValue("connection", out inputValue))
        details.Add("ConnectionName", inputValue.Value);
      DataSourceBinding sourceBindingForInput = this.GetDataSourceBindingForInput(WellKnownArtifactInputs.ArtifactDetails);
      if (this.ArtifactType == null || this.ArtifactType.Equals("Build"))
      {
        if (inputValue == null)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.InvalidServiceEndpointId((object) string.Empty)));
        Guid result;
        if (!Guid.TryParse(inputValue.Value, out result))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.InvalidServiceEndpointId((object) string.Empty)));
        if (sourceBindingForInput == null)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.ArtifactDataSourceBindingNotFound((object) WellKnownArtifactInputs.ArtifactDetails, (object) this.ArtifactTypeId)));
        this.PrepareArtifactDetails(requestContext, projectId, serverArtifact, result, sourceBindingForInput, details);
      }
      return new AgentArtifactDefinition()
      {
        Name = serverArtifact.Name,
        Alias = serverArtifact.Alias,
        Version = serverArtifact.ArtifactVersion,
        ArtifactType = AgentArtifactType.Custom,
        Details = JsonConvert.SerializeObject((object) details)
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
      IList<InputValue> versionsAfterElevation = this.GetVersionsAfterElevation(requestContext, sourceInputs, projectInfo, WellKnownArtifactInputs.VersionsInput);
      foreach (InputValue inputValue in (IEnumerable<InputValue>) versionsAfterElevation)
      {
        if (inputValue != null)
        {
          if (inputValue.Data == null)
            inputValue.Data = (IDictionary<string, object>) new Dictionary<string, object>();
          if (!inputValue.Data.ContainsKey("branch"))
          {
            string str;
            sourceInputs.TryGetValue("branch", out str);
            inputValue.Data.Add("branch", (object) str);
          }
        }
      }
      return versionsAfterElevation;
    }

    public override InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      string empty = string.Empty;
      if (sourceInputs != null)
      {
        string a;
        sourceInputs.TryGetValue("defaultVersionType", out a);
        if (string.Equals(a, "specificVersionType", StringComparison.OrdinalIgnoreCase))
          sourceInputs.TryGetValue("defaultVersionSpecific", out empty);
      }
      InputValue inputValue;
      if (string.IsNullOrWhiteSpace(empty))
        inputValue = this.GetVersionsAfterElevation(requestContext, sourceInputs, projectInfo, WellKnownArtifactInputs.LatestVersionInput).FirstOrDefault<InputValue>();
      else
        inputValue = new InputValue()
        {
          Value = empty,
          DisplayValue = empty
        };
      return inputValue != null ? inputValue : throw new InvalidRequestException(TaskResources.NoVersionFound());
    }

    public override bool SupportsLatestVersionDataSourceBinding() => this.GetDataSourceBindingForInput(WellKnownArtifactInputs.LatestVersionInput) != null;

    private void PrepareArtifactDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact,
      Guid endpointId,
      DataSourceBinding dataSourceBinding,
      Dictionary<string, string> details)
    {
      Dictionary<string, string> dictionary = serverArtifact.SourceData.Select<KeyValuePair<string, InputValue>, KeyValuePair<string, string>>((Func<KeyValuePair<string, InputValue>, KeyValuePair<string, string>>) (x => new KeyValuePair<string, string>(x.Key, x.Value.Value))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, string>) (x => x.Value));
      if (dictionary.ContainsKey("version"))
        dictionary["version"] = serverArtifact.ArtifactVersion;
      else
        dictionary.Add("version", serverArtifact.ArtifactVersion);
      ServiceEndpointType endpointType = this.serviceEndpointTypeRetriever(requestContext, projectId, this.artifactContributionDefinition.EndpointTypeId);
      ServiceEndpoint endpoint = this.serviceEndpointRetriever(requestContext, projectId, endpointId);
      if (endpoint == null)
        throw new InvalidRequestException(TaskResources.NoEndpointPresent());
      if (endpoint.Authorization == null)
        throw new InvalidRequestException(TaskResources.NoEndpointAuthorizationPresent());
      DataSource dataSource = endpointType.DataSources.FirstOrDefault<DataSource>((Func<DataSource, bool>) (x => x.Name == dataSourceBinding.DataSourceName));
      ServiceEndpointAuthenticationScheme authenticationScheme = endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (x => x.Scheme == endpoint.Authorization.Scheme));
      CustomArtifactDetails customArtifactDetails = new CustomArtifactDetails()
      {
        ArtifactsUrl = dataSource?.EndpointUrl,
        ResultSelector = dataSource?.ResultSelector,
        ResultTemplate = dataSourceBinding.ResultTemplate,
        ArtifactVariables = dictionary,
        AuthorizationHeaders = authenticationScheme?.AuthorizationHeaders,
        ArtifactTypeStreamMapping = this.artifactContributionDefinition.ArtifactTypeStreamMapping
      };
      this.AddDetailsToFetchArtifactVersions(endpointType, customArtifactDetails);
      details.Add("ArtifactDetails", JsonConvert.SerializeObject((object) customArtifactDetails));
    }

    private void AddDetailsToFetchArtifactVersions(
      ServiceEndpointType endpointType,
      CustomArtifactDetails customArtifactDetails)
    {
      DataSourceBinding dataSourceBinding = this.GetDataSourceBindingForInput(WellKnownArtifactInputs.VersionsInput);
      if (dataSourceBinding == null)
        return;
      DataSource dataSource = endpointType.DataSources.FirstOrDefault<DataSource>((Func<DataSource, bool>) (x => x.Name == dataSourceBinding.DataSourceName));
      customArtifactDetails.VersionsUrl = dataSource?.EndpointUrl;
      customArtifactDetails.VersionsResultSelector = dataSource?.ResultSelector;
      customArtifactDetails.VersionsResultTemplate = dataSourceBinding.ResultTemplate;
    }

    private IList<InputValue> GetVersionsAfterElevation(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo,
      string wellKnownArtifactInput)
    {
      string empty = string.Empty;
      DataSourceBinding sourceBindingForInput = this.GetDataSourceBindingForInput(wellKnownArtifactInput);
      if (sourceBindingForInput == null)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.ArtifactDataSourceBindingNotFound((object) wellKnownArtifactInput, (object) this.ArtifactTypeId)));
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) sourceInputs);
      CustomArtifact.UpdateCurrentInputValuesForDataSourceParameters(dictionary, sourceBindingForInput, projectInfo);
      sourceBindingForInput.EndpointId = CustomArtifact.GetEndpointId(sourceBindingForInput, (IDictionary<string, string>) dictionary);
      if (sourceBindingForInput.EndpointId.IsNullOrEmpty<char>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.InvalidServiceEndpointId((object) empty)));
      IVssRequestContext vssRequestContext = requestContext;
      if (!InternalServiceHelper.IsInternalService(sourceBindingForInput.EndpointId))
        vssRequestContext = requestContext.Elevate();
      return this.executeServiceEndpointRequest(vssRequestContext, projectInfo.Id, sourceBindingForInput, (IDictionary<string, string>) dictionary, (IDictionary<string, string>) null);
    }

    private Uri GetArtifactSourceUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId,
      string inputId)
    {
      Uri artifactSourceUrl = (Uri) null;
      try
      {
        DataSourceBinding sourceBindingForInput = this.GetDataSourceBindingForInput(inputId);
        if (sourceBindingForInput != null)
        {
          if (sourceBindingForInput.ResultTemplate != null)
          {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (serverArtifact.SourceData != null && serverArtifact.SourceData.Any<KeyValuePair<string, InputValue>>())
            {
              foreach (KeyValuePair<string, InputValue> keyValuePair in serverArtifact.SourceData)
                parameters[keyValuePair.Key] = keyValuePair.Value.Value;
            }
            Guid result = Guid.Empty;
            ServiceEndpoint serviceEndpoint = (ServiceEndpoint) null;
            if (parameters.ContainsKey("connection") && Guid.TryParse(parameters["connection"], out result))
              serviceEndpoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext.Elevate(), projectId, result);
            JToken replacementContext = EndpointMustacheHelper.CreateReplacementContext(serviceEndpoint, parameters, projectId);
            artifactSourceUrl = new Uri(new MustacheTemplateEngine().EvaluateTemplate(sourceBindingForInput.ResultTemplate, replacementContext));
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, TraceLevel.Warning, "DistributedTask", nameof (CustomArtifact), ex);
      }
      return artifactSourceUrl;
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

    protected override IDictionary<string, ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Dictionary<string, InputValue> dictionary = artifactSource != null ? artifactSource.SourceData : throw new ArgumentNullException(nameof (artifactSource));
      Dictionary<string, ConfigurationVariableValue> configurationVariables = new Dictionary<string, ConfigurationVariableValue>();
      InputValue inputValue1;
      if (dictionary.TryGetValue("version", out inputValue1))
      {
        configurationVariables.Add("buildNumber", new ConfigurationVariableValue()
        {
          Value = inputValue1.DisplayValue
        });
        configurationVariables.Add("buildId", new ConfigurationVariableValue()
        {
          Value = inputValue1.Value
        });
      }
      InputValue inputValue2;
      if (dictionary.TryGetValue("definition", out inputValue2))
      {
        configurationVariables.Add("defintionId", new ConfigurationVariableValue()
        {
          Value = inputValue2.Value
        });
        configurationVariables.Add("definitionId", new ConfigurationVariableValue()
        {
          Value = inputValue2.Value
        });
        configurationVariables.Add("definitionName", new ConfigurationVariableValue()
        {
          Value = inputValue2.DisplayValue
        });
      }
      return (IDictionary<string, ConfigurationVariableValue>) configurationVariables;
    }

    public override Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      return serverArtifact == null || !serverArtifact.SourceData.ContainsKey("version") ? (Uri) null : this.GetArtifactSourceUrl(requestContext, serverArtifact, projectId, WellKnownArtifactInputs.ArtifactSourceVersionUrl);
    }

    public override Uri GetArtifactSourceDefinitionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      return this.GetArtifactSourceUrl(requestContext, serverArtifact, projectId, WellKnownArtifactInputs.ArtifactSourceDefinitionUrl);
    }
  }
}
