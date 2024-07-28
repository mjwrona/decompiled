// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Jenkins.JenkinsArtifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Jenkins
{
  public class JenkinsArtifact : FirstPartyArtifactTypeBase
  {
    private const string JenkinsDefinitionId = "definition";
    private const string JenkinsConnectionId = "connection";
    private const string VisibleRule = "visibleRule";
    private const string JenkinsJobTypeId = "jenkinsJobType";
    private const string JenkinsPathSeparator = "/";
    private readonly Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>> serviceEndpointsRetriever;
    private readonly Func<IVssRequestContext, Guid, Guid, ServiceEndpoint> serviceEndpointRetriever;
    private readonly Func<string, ServiceEndpoint, HttpResponseMessage> jenkinsInfoRetriever;
    private readonly Func<string, ServiceEndpoint, HttpResponseMessage> jenkinsHeadRetriever;

    public JenkinsArtifact()
      : this(JenkinsArtifact.\u003C\u003EO.\u003C0\u003E__GetServiceEndpoints ?? (JenkinsArtifact.\u003C\u003EO.\u003C0\u003E__GetServiceEndpoints = new Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>>(ServiceEndpointHelper.GetServiceEndpoints)), ServiceEndpointHelper.GetServiceEndpoint, JenkinsArtifact.\u003C\u003EO.\u003C1\u003E__GetJenkinsInfo ?? (JenkinsArtifact.\u003C\u003EO.\u003C1\u003E__GetJenkinsInfo = new Func<string, ServiceEndpoint, HttpResponseMessage>(JenkinsArtifact.GetJenkinsInfo)), JenkinsArtifact.\u003C\u003EO.\u003C2\u003E__PerformHeadRequest ?? (JenkinsArtifact.\u003C\u003EO.\u003C2\u003E__PerformHeadRequest = new Func<string, ServiceEndpoint, HttpResponseMessage>(JenkinsArtifact.PerformHeadRequest)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required for testablity.")]
    protected JenkinsArtifact(
      Func<IVssRequestContext, Guid, string, IEnumerable<ServiceEndpoint>> serviceEndpointsRetriever,
      Func<IVssRequestContext, Guid, Guid, ServiceEndpoint> serviceEndpointRetriever,
      Func<string, ServiceEndpoint, HttpResponseMessage> getJenkinsInfo,
      Func<string, ServiceEndpoint, HttpResponseMessage> getJenkinsHeader)
    {
      this.serviceEndpointsRetriever = serviceEndpointsRetriever;
      this.serviceEndpointRetriever = serviceEndpointRetriever;
      this.jenkinsInfoRetriever = getJenkinsInfo;
      this.jenkinsHeadRetriever = getJenkinsHeader;
    }

    public override string Name => "Jenkins";

    public override string DisplayName => Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsArtifactDisplayName;

    public override string EndpointTypeId => "Jenkins";

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

    public override bool IsWorkitemsTraceabilitySupported => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceName,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceEndPointDescription,
        InputMode = InputMode.Combo,
        Id = "connection",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobName,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobDescription,
        InputMode = InputMode.Combo,
        Id = "definition",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MinLength = new int?(1)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "connection"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = "Jenkins Job Type",
        Description = "Specifies the Jenkins Job Type",
        InputMode = InputMode.None,
        Id = "jenkinsJobType",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "connection",
          "definition"
        },
        Properties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "visibleRule",
            (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == invalidjobName", (object) "definition")
          }
        },
        HasDynamicValueInformation = false
      },
      new InputDescriptor()
      {
        Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForArtifacts,
        Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForArtifacts,
        InputMode = InputMode.None,
        Id = "artifacts",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String,
          MinLength = new int?(0),
          MaxLength = new int?(260)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "connection",
          "definition",
          "jenkinsJobType"
        },
        HasDynamicValueInformation = true
      }
    };

    public override InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      string errorMessage;
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
        DefaultValue = string.Empty,
        PossibleValues = possibleValues,
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
      InputValue inputValue = serverArtifact != null ? JenkinsArtifact.GetServiceDetails(serverArtifact) : throw new ArgumentNullException(nameof (serverArtifact));
      InputValue jobDetails = JenkinsArtifact.GetJobDetails(serverArtifact);
      string str = JsonConvert.SerializeObject((object) new Dictionary<string, string>()
      {
        {
          "RelativePath",
          serverArtifact.Path
        },
        {
          "ConnectionName",
          inputValue.Value
        },
        {
          "JobName",
          jobDetails.Value
        }
      });
      return new AgentArtifactDefinition()
      {
        Name = serverArtifact.Name,
        Alias = serverArtifact.Alias,
        Version = serverArtifact.ArtifactVersion,
        ArtifactType = AgentArtifactType.Jenkins,
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
      Guid endpointId = ServiceEndpointHelper.GetEndpointId(ArtifactTypeBase.GetSourceInput(sourceInputs, "connection"));
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(requestContext, projectInfo.Id, endpointId);
      string absoluteUri = serviceEndpoint.Url != (Uri) null ? serviceEndpoint.Url.AbsoluteUri : (string) null;
      bool flag = JenkinsArtifact.IsMultiBranchPipeline(sourceInputs);
      string str = "builds[displayName,id,result]";
      if (flag)
        str = "jobs[name,builds[displayName,id,result]]";
      string jenkinsJobUrlInfix = JenkinsArtifact.GetJenkinsJobUrlInfix(sourceInputs);
      List<InputValue> versions = new List<InputValue>();
      HttpResponseMessage httpResponseMessage = this.jenkinsInfoRetriever(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/api/json?tree={2}", (object) absoluteUri, (object) jenkinsJobUrlInfix, (object) str), serviceEndpoint);
      if (httpResponseMessage.IsSuccessStatusCode)
      {
        JObject jobject = JObject.Parse(httpResponseMessage.Content.ReadAsStringAsync().Result);
        if (flag)
        {
          foreach (JToken jtoken in (JArray) jobject["jobs"])
          {
            JArray jobs = (JArray) jtoken[(object) "builds"];
            string branchName = (string) jtoken[(object) "name"];
            JenkinsArtifact.GetJenkinsBuilds(jobs, versions, JenkinsArtifact.GetBranchNamePrefix(branchName));
          }
        }
        else
          JenkinsArtifact.GetJenkinsBuilds((JArray) jobject["builds"], versions, string.Empty);
      }
      return (IList<InputValue>) versions;
    }

    private static string GetJenkinsJobUrlInfix(IDictionary<string, string> sourceInputs)
    {
      string jenkinsJobUrlInfix = string.Empty;
      string sourceInput = ArtifactTypeBase.GetSourceInput(sourceInputs, "definition");
      if (!string.IsNullOrEmpty(sourceInput))
        jenkinsJobUrlInfix = JenkinsArtifact.GetJobUrlInfixFromJobName(sourceInput);
      return jenkinsJobUrlInfix;
    }

    private static string GetJobUrlInfixFromJobName(string jobName) => new MustacheTemplateEngine().EvaluateTemplate("{{#splitAndPrefix definition '/' '/job/'}}{{/splitAndPrefix}}", EndpointMustacheHelper.GetMergedContext(JToken.FromObject((object) new Dictionary<string, string>()
    {
      {
        "definition",
        jobName
      }
    }), (JToken) null));

    private static void GetJenkinsBuilds(
      JArray jobs,
      List<InputValue> versions,
      string buildPrefix)
    {
      if (jobs == null)
        return;
      versions.AddRange(jobs.Where<JToken>((Func<JToken, bool>) (b => ((string) b[(object) "result"]).Equals("SUCCESS"))).Select<JToken, InputValue>((Func<JToken, InputValue>) (build => new InputValue()
      {
        Value = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) buildPrefix, (object) (string) build[(object) "id"]),
        DisplayValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) buildPrefix, (object) (string) build[(object) "displayName"])
      })));
    }

    public override InputValue GetLatestVersion(
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
      Guid endpointId = ServiceEndpointHelper.GetEndpointId(ArtifactTypeBase.GetSourceInput(sourceInputs, "connection"));
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(requestContext, projectInfo.Id, endpointId);
      string absoluteUri = serviceEndpoint.Url != (Uri) null ? serviceEndpoint.Url.AbsoluteUri : (string) null;
      string str1 = "lastSuccessfulBuild[id,displayName]";
      bool flag = JenkinsArtifact.IsMultiBranchPipeline(sourceInputs);
      if (flag)
        str1 = "jobs[name,lastSuccessfulBuild[id,displayName,timestamp]]";
      string jenkinsJobUrlInfix = JenkinsArtifact.GetJenkinsJobUrlInfix(sourceInputs);
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/api/json?tree={2}", (object) absoluteUri, (object) jenkinsJobUrlInfix, (object) str1);
      InputValue latestVersion = (InputValue) null;
      HttpResponseMessage httpResponseMessage = this.jenkinsInfoRetriever(str2, serviceEndpoint);
      if (httpResponseMessage.IsSuccessStatusCode)
      {
        JObject result = JObject.Parse(httpResponseMessage.Content.ReadAsStringAsync().Result);
        if (flag)
        {
          string template = new MustacheTemplateEngine().EvaluateTemplate("{{#selectMaxOfLong jobs 'lastSuccessfulBuild.timestamp'}}{ \"branchName\": \"{{name}}\", \"buildId\": \"{{lastSuccessfulBuild.id}}\", \"displayName\": \"{{{lastSuccessfulBuild.displayName}}}\" }{{/selectMaxOfLong}}", EndpointMustacheHelper.GetMergedContext((JToken) result, (JToken) null));
          if (!string.IsNullOrEmpty(template))
          {
            JToken jtoken = JToken.Parse(template);
            string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) jtoken[(object) "branchName"], (object) jtoken[(object) "buildId"]);
            string str4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) jtoken[(object) "branchName"], (object) jtoken[(object) "displayName"]);
            latestVersion = new InputValue()
            {
              Value = str3,
              DisplayValue = str4
            };
          }
        }
        else
        {
          JToken jtoken = result["lastSuccessfulBuild"];
          if (jtoken.HasValues)
            latestVersion = new InputValue()
            {
              Value = (string) jtoken[(object) "id"],
              DisplayValue = (string) jtoken[(object) "displayName"]
            };
        }
      }
      return latestVersion;
    }

    protected override IDictionary<string, ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Dictionary<string, InputValue> dictionary = artifactSource != null ? artifactSource.SourceData : throw new ArgumentNullException(nameof (artifactSource));
      InputValue inputValue = (InputValue) null;
      InputValue versionData = (InputValue) null;
      Dictionary<string, ConfigurationVariableValue> configurationVariables = new Dictionary<string, ConfigurationVariableValue>();
      if (dictionary.TryGetValue("version", out versionData))
      {
        string buildId = versionData.Value;
        if (!JenkinsArtifact.IsMultiBranchPipeline(buildId))
          buildId = JenkinsArtifact.TryParseBuildId(versionData, true).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        configurationVariables.Add("buildNumber", new ConfigurationVariableValue()
        {
          Value = versionData.DisplayValue
        });
        configurationVariables.Add("buildId", new ConfigurationVariableValue()
        {
          Value = buildId
        });
      }
      if (dictionary.TryGetValue("definition", out inputValue))
        configurationVariables.Add("definitionName", new ConfigurationVariableValue()
        {
          Value = inputValue.Value
        });
      return (IDictionary<string, ConfigurationVariableValue>) configurationVariables;
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

    private static int TryParseBuildId(InputValue versionData, bool shouldThrow)
    {
      if (versionData == null)
        return 0;
      int result;
      if (!string.IsNullOrEmpty(versionData.Value) && int.TryParse(versionData.Value, out result))
        return result;
      if (shouldThrow)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidBuildId, (object) versionData.Value));
      return 0;
    }

    private static bool IsMultiBranchPipeline(string buildId) => !string.IsNullOrEmpty(buildId) && buildId.IndexOf("/", StringComparison.OrdinalIgnoreCase) != -1;

    private static bool IsMultiBranchPipeline(IDictionary<string, string> sourceInputs)
    {
      string empty = string.Empty;
      return sourceInputs != null && sourceInputs.TryGetValue("jenkinsJobType", out empty) && string.Equals(empty, "org.jenkinsci.plugins.workflow.multibranch.WorkflowMultiBranchProject", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetBranchNamePrefix(string branchName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/", (object) branchName);

    private static HttpResponseMessage GetJenkinsInfo(
      string jenkinsUrl,
      ServiceEndpoint serviceEndpoint)
    {
      using (WebRequestHandler webRequestHandler = new WebRequestHandler())
      {
        using (HttpClient httpClient = new HttpClient((HttpMessageHandler) webRequestHandler))
        {
          JenkinsArtifact.SetupHttpClient(httpClient, webRequestHandler, serviceEndpoint);
          try
          {
            return httpClient.GetAsync(jenkinsUrl).Result;
          }
          catch (Exception ex)
          {
            throw new JenkinsServerUnavailableException(ex.Message);
          }
        }
      }
    }

    private static HttpResponseMessage PerformHeadRequest(
      string jenkinsUrl,
      ServiceEndpoint serviceEndpoint)
    {
      using (WebRequestHandler webRequestHandler = new WebRequestHandler())
      {
        using (HttpClient httpClient = new HttpClient((HttpMessageHandler) webRequestHandler))
        {
          JenkinsArtifact.SetupHttpClient(httpClient, webRequestHandler, serviceEndpoint);
          using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, jenkinsUrl))
          {
            try
            {
              return httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
            }
            catch (Exception ex)
            {
              throw new JenkinsServerUnavailableException(ex.Message);
            }
          }
        }
      }
    }

    private static void SetupHttpClient(
      HttpClient httpClient,
      WebRequestHandler requestHandler,
      ServiceEndpoint serviceEndpoint)
    {
      if (!(serviceEndpoint.Authorization.Scheme == "UsernamePassword"))
        throw new ReleaseManagementUnauthorizedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.AuthorizationSchemeNotUserNamePassword, (object) serviceEndpoint.Authorization.Scheme));
      string parameter1 = serviceEndpoint.Authorization.Parameters["Username"];
      string parameter2 = serviceEndpoint.Authorization.Parameters["Password"];
      httpClient.Timeout = TimeSpan.FromSeconds(30.0);
      httpClient.DefaultRequestHeaders.Authorization = JenkinsArtifact.CreateBasicAuthenticationHeader(parameter1, parameter2);
      string str;
      bool result;
      if (((!serviceEndpoint.Data.TryGetValue("acceptUntrustedCerts", out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0)) & (result ? 1 : 0)) == 0)
        return;
      requestHandler.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) ((sender, certificate, chain, sslPolicyErrors) => true);
    }

    private static InputValue GetServiceDetails(AgentArtifactDefinition artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      InputValue serviceDetails;
      if (!artifact.SourceData.TryGetValue("connection", out serviceDetails) || serviceDetails == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectDetailsNotAvailable, (object) artifact.SourceDataKeys, (object) "connection"));
      return serviceDetails;
    }

    private static InputValue GetJobDetails(AgentArtifactDefinition artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      string errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionDetailsNotAvailable, (object) artifact.SourceDataKeys, (object) "definition");
      return FirstPartyArtifactTypeBase.GetDetailsFromSourceInputs((IDictionary<string, InputValue>) artifact.SourceData, "definition", true, errorMessage);
    }

    private static AuthenticationHeaderValue CreateBasicAuthenticationHeader(
      string username,
      string password)
    {
      return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) (username ?? string.Empty), (object) (password ?? string.Empty)))));
    }

    private static void HandleNonSuccessfulResponse(
      HttpResponseMessage response,
      out string errorMessage)
    {
      errorMessage = response.StatusCode == HttpStatusCode.Unauthorized ? Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsUnauthorized : Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsErrorInAccess;
    }

    private static void HandleNonSuccessfulResponseWhileRetrievingArtifactContents(
      HttpResponseMessage response,
      string itemPath,
      out string errorMessage)
    {
      if (response.StatusCode == HttpStatusCode.Unauthorized)
        errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsUnauthorized;
      else if (response.StatusCode == HttpStatusCode.NotFound)
        errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.FileNotFoundInArtifact, (object) itemPath);
      else
        errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsErrorInAccess;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set good error message.")]
    private static IEnumerable<JenkinsArtifactData> ToJenkinsArtifactData(
      HttpResponseMessage httpResponse)
    {
      IList<JenkinsArtifactData> jenkinsArtifactData = (IList<JenkinsArtifactData>) new List<JenkinsArtifactData>();
      if (httpResponse.Content == null)
        return (IEnumerable<JenkinsArtifactData>) jenkinsArtifactData;
      string result = httpResponse.Content.ReadAsStringAsync().Result;
      if (string.IsNullOrWhiteSpace(result))
        return (IEnumerable<JenkinsArtifactData>) jenkinsArtifactData;
      try
      {
        JToken source = JObject.Parse(result)["artifacts"];
        return !source.HasValues ? (IEnumerable<JenkinsArtifactData>) jenkinsArtifactData : (IEnumerable<JenkinsArtifactData>) source.Select<JToken, JenkinsArtifactData>((Func<JToken, JenkinsArtifactData>) (artifact => new JenkinsArtifactData()
        {
          RelativePath = (string) artifact[(object) "relativePath"],
          DisplayPath = (string) artifact[(object) "displayPath"]
        })).ToList<JenkinsArtifactData>();
      }
      catch (Exception ex)
      {
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsErrorInParsingResponse, (object) result));
      }
    }

    private static string GetJenkinsJobsUrl(string jenkinsEndpoint)
    {
      string template = new MustacheTemplateEngine().EvaluateTemplate("api/json?{{#recursiveFormat 15 'tree=jobs[name,displayName{0}]' ',jobs[name,displayName{0}]'}}", (JToken) null);
      return !string.IsNullOrEmpty(template) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) jenkinsEndpoint, (object) template) : string.Empty;
    }

    private static List<InputValue> GetJenkinsJobResult(string jsonString)
    {
      string template1 = "{{#addField jobs 'parentPath' 'name' '/'}}{{#recursiveSelect jobs}}{{#notEquals _class 'com.cloudbees.hudson.plugins.folder.Folder'}}{{#notEquals _class 'org.jenkinsci.plugins.workflow.job.WorkflowJob'}}{ \"Value\" : \"{{#if parentPath}}{{parentPath}}/{{/if}}{{name}}\", \"DisplayValue\" : \"{{#if parentPath}}{{parentPath}}/{{/if}}{{{displayName}}}\" }{{/notEquals}}{{/notEquals}}{{/recursiveSelect}}{{/addField}}";
      List<InputValue> jenkinsJobResult = new List<InputValue>();
      JToken mergedContext = EndpointMustacheHelper.GetMergedContext(JToken.Parse(jsonString), (JToken) null);
      string template2 = new MustacheTemplateEngine().EvaluateTemplate(template1, mergedContext);
      if (!string.IsNullOrEmpty(template2))
      {
        foreach (JToken json in (IEnumerable<JToken>) JToken.Parse(template2))
        {
          JToken jtoken = JToken.Parse((string) json);
          jenkinsJobResult.Add(new InputValue()
          {
            Value = (string) jtoken[(object) "Value"],
            DisplayValue = (string) jtoken[(object) "DisplayValue"]
          });
        }
      }
      return jenkinsJobResult;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    private IList<InputValue> GetPossibleValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues,
      out string errorMessage)
    {
      errorMessage = (string) null;
      string str = (string) null;
      List<InputValue> possibleValues = new List<InputValue>();
      try
      {
        string servicesId = (string) null;
        switch (inputId)
        {
          case "connection":
            IEnumerable<ServiceEndpoint> source = this.serviceEndpointsRetriever(context, projectInfo.Id, "Jenkins");
            possibleValues.AddRange(source.Select<ServiceEndpoint, InputValue>((Func<ServiceEndpoint, InputValue>) (se => new InputValue()
            {
              Value = se.Id.ToString(),
              DisplayValue = se.Name
            })));
            break;
          case "definition":
            if (currentInputValues.TryGetValue("connection", out servicesId))
              return this.GetJenkinsSources(context, projectInfo, servicesId, out errorMessage, out str);
            errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceEndPointIdNotPresent;
            break;
          case "jenkinsJobType":
            if (!context.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks"))
              return (IList<InputValue>) new List<InputValue>()
              {
                new InputValue()
                {
                  Value = string.Empty,
                  DisplayValue = string.Empty
                }
              };
            if (currentInputValues.TryGetValue("connection", out servicesId))
            {
              string jobName;
              if (currentInputValues.TryGetValue("definition", out jobName))
                return this.GetJenkinsJobType(context, projectInfo, servicesId, jobName, out errorMessage, out str);
              errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobIdNotPresent;
              break;
            }
            errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceEndPointIdNotPresent;
            break;
          case "artifacts":
            if (currentInputValues.TryGetValue("connection", out servicesId))
            {
              string jobId;
              if (currentInputValues.TryGetValue("definition", out jobId))
              {
                string artifactSourceVersionId = string.Empty;
                FirstPartyArtifactTypeBase.HasArtifactSourceVersion(currentInputValues, out artifactSourceVersionId);
                string empty = string.Empty;
                currentInputValues.TryGetValue("jenkinsJobType", out empty);
                return this.GetBuildArtifacts(context, projectInfo, servicesId, jobId, empty, artifactSourceVersionId, out errorMessage, out str);
              }
              errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobIdNotPresent;
              break;
            }
            errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceEndPointIdNotPresent;
            break;
          case "artifactItemContent":
            if (currentInputValues.TryGetValue("connection", out servicesId))
            {
              string jobId;
              if (currentInputValues.TryGetValue("definition", out jobId))
              {
                string empty = string.Empty;
                if (currentInputValues.TryGetValue("itemPath", out empty))
                  return this.GetArtifactContent(context, projectInfo, servicesId, jobId, empty, out errorMessage, out str);
                errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ArtifactItemPathNotPresent;
                break;
              }
              errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobIdNotPresent;
              break;
            }
            errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceEndPointIdNotPresent;
            break;
          default:
            errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InputTypeNotSupported, (object) inputId);
            break;
        }
      }
      catch (AggregateException ex)
      {
        errorMessage = ex.Flatten().InnerException.ToString();
        if (ex.InnerException.GetType() == typeof (TaskCanceledException))
        {
          if (str != null)
            errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsConnectionNotAvailable, (object) str);
        }
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
      }
      return (IList<InputValue>) possibleValues;
    }

    private IList<InputValue> GetJenkinsJobType(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string servicesId,
      string jobName,
      out string errorMessage,
      out string jenkinsEndpoint)
    {
      IList<InputValue> jenkinsJobType = (IList<InputValue>) new List<InputValue>();
      errorMessage = (string) null;
      jenkinsEndpoint = (string) null;
      if (string.IsNullOrEmpty(jobName))
      {
        errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobIdNotPresent;
        return jenkinsJobType;
      }
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(requestContext, projectInfo.Id, Guid.Parse(servicesId));
      jenkinsEndpoint = serviceEndpoint.Url != (Uri) null ? serviceEndpoint.Url.AbsoluteUri : (string) null;
      HttpResponseMessage response = this.jenkinsInfoRetriever(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/api/json", (object) jenkinsEndpoint, (object) JenkinsArtifact.GetJobUrlInfixFromJobName(jobName)), serviceEndpoint);
      if (response.IsSuccessStatusCode)
      {
        string str = (string) JObject.Parse(response.Content.ReadAsStringAsync().Result)["_class"];
        if (string.IsNullOrEmpty(str))
          str = "none";
        jenkinsJobType.Add(new InputValue() { Value = str });
      }
      else
        JenkinsArtifact.HandleNonSuccessfulResponse(response, out errorMessage);
      return jenkinsJobType;
    }

    private IList<InputValue> GetJenkinsSources(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string servicesId,
      out string errorMessage,
      out string jenkinsEndPoint)
    {
      IList<InputValue> jenkinsSources = (IList<InputValue>) new List<InputValue>();
      errorMessage = (string) null;
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(context, projectInfo.Id, ServiceEndpointHelper.GetEndpointId(servicesId));
      jenkinsEndPoint = serviceEndpoint.Url != (Uri) null ? serviceEndpoint.Url.AbsoluteUri : (string) null;
      string empty = string.Empty;
      HttpResponseMessage response = this.jenkinsInfoRetriever(!context.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks") ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/api/json?tree=jobs[name,displayName]", (object) jenkinsEndPoint) : JenkinsArtifact.GetJenkinsJobsUrl(jenkinsEndPoint), serviceEndpoint);
      if (response.IsSuccessStatusCode)
        jenkinsSources = !context.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks") ? (IList<InputValue>) JObject.Parse(response.Content.ReadAsStringAsync().Result)["jobs"].Select<JToken, InputValue>((Func<JToken, InputValue>) (build => new InputValue()
        {
          Value = (string) build[(object) "name"],
          DisplayValue = (string) build[(object) "displayName"]
        })).ToList<InputValue>() : (IList<InputValue>) JenkinsArtifact.GetJenkinsJobResult(response.Content.ReadAsStringAsync().Result);
      else
        JenkinsArtifact.HandleNonSuccessfulResponse(response, out errorMessage);
      return jenkinsSources;
    }

    private IList<InputValue> GetBuildArtifacts(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string servicesId,
      string jobId,
      string jobType,
      string buildNumber,
      out string errorMessage,
      out string jenkinsEndpoint)
    {
      IList<InputValue> buildArtifacts = (IList<InputValue>) new List<InputValue>();
      string str = buildNumber;
      errorMessage = (string) null;
      jenkinsEndpoint = (string) null;
      if (string.IsNullOrEmpty(jobId))
      {
        errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobIdNotPresent;
        return buildArtifacts;
      }
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(requestContext, projectInfo.Id, Guid.Parse(servicesId));
      jenkinsEndpoint = serviceEndpoint.Url != (Uri) null ? serviceEndpoint.Url.AbsoluteUri : (string) null;
      if (string.IsNullOrEmpty(str))
      {
        Dictionary<string, string> sourceInputs = new Dictionary<string, string>()
        {
          {
            "connection",
            servicesId
          },
          {
            "definition",
            jobId
          },
          {
            "jenkinsJobType",
            jobType
          }
        };
        InputValue latestVersion = this.GetLatestVersion(requestContext, (IDictionary<string, string>) sourceInputs, projectInfo);
        if (latestVersion == null || string.IsNullOrEmpty(latestVersion.Value))
          return buildArtifacts;
        str = latestVersion.Value;
      }
      if (string.Equals(jobType, "org.jenkinsci.plugins.workflow.multibranch.WorkflowMultiBranchProject", StringComparison.OrdinalIgnoreCase))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "job/{0}", (object) str);
      HttpResponseMessage httpResponseMessage = this.jenkinsInfoRetriever(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/{2}/api/json?tree=artifacts[*]", (object) jenkinsEndpoint, (object) JenkinsArtifact.GetJobUrlInfixFromJobName(jobId), (object) str), serviceEndpoint);
      if (httpResponseMessage.IsSuccessStatusCode)
        buildArtifacts = (IList<InputValue>) JenkinsArtifact.ToJenkinsArtifactData(httpResponseMessage).Select<JenkinsArtifactData, InputValue>((Func<JenkinsArtifactData, InputValue>) (artifact => new InputValue()
        {
          Value = artifact.RelativePath,
          DisplayValue = artifact.DisplayPath
        })).ToList<InputValue>();
      else
        JenkinsArtifact.HandleNonSuccessfulResponse(httpResponseMessage, out errorMessage);
      return buildArtifacts;
    }

    private IList<InputValue> GetArtifactContent(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string servicesId,
      string jobId,
      string itemPath,
      out string errorMessage,
      out string jenkinsEndpoint)
    {
      IList<InputValue> artifactContent = (IList<InputValue>) new List<InputValue>();
      errorMessage = (string) null;
      jenkinsEndpoint = (string) null;
      if (string.IsNullOrEmpty(jobId))
      {
        errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.JenkinsJobIdNotPresent;
        return artifactContent;
      }
      ServiceEndpoint serviceEndpoint = this.serviceEndpointRetriever(requestContext, projectInfo.Id, Guid.Parse(servicesId));
      jenkinsEndpoint = serviceEndpoint.Url != (Uri) null ? serviceEndpoint.Url.AbsoluteUri : (string) null;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/job/{1}/ws/{2}", (object) jenkinsEndpoint, (object) jobId, (object) itemPath);
      HttpResponseMessage response1 = this.jenkinsHeadRetriever(str, serviceEndpoint);
      if (response1.IsSuccessStatusCode)
      {
        long? contentLength = response1.Content.Headers.ContentLength;
        long num = 2097152;
        if (contentLength.GetValueOrDefault() < num & contentLength.HasValue)
        {
          HttpResponseMessage response2 = this.jenkinsInfoRetriever(str, serviceEndpoint);
          if (response2.IsSuccessStatusCode)
          {
            string result = response2.Content.ReadAsStringAsync().Result;
            artifactContent.Add(new InputValue()
            {
              Data = (IDictionary<string, object>) new Dictionary<string, object>()
              {
                {
                  "artifactItemContent",
                  (object) result
                }
              }
            });
          }
          else
            JenkinsArtifact.HandleNonSuccessfulResponseWhileRetrievingArtifactContents(response2, itemPath, out errorMessage);
        }
        else
          errorMessage = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.FileSizeIsTooLarge;
      }
      else
        JenkinsArtifact.HandleNonSuccessfulResponseWhileRetrievingArtifactContents(response1, itemPath, out errorMessage);
      return artifactContent;
    }
  }
}
