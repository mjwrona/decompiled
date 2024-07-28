// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Jenkins.TriggerGenericBuildAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Jenkins
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class TriggerGenericBuildAction : ConsumerActionImplementation
  {
    private const string c_id = "triggerGenericBuild";
    private const int c_defaultGetAllBuildsDepth = 5;
    private const string c_urlFormatBuildUrl = "{0}/{1}/build";
    private const string c_urlFormatBuildUrlWithParameters = "{0}/{1}/buildWithParameters";
    private const string c_urlFormatGetAllBuilds = "{0}/api/json?depth={1}&tree={2}";
    private const string c_urlPathFormatBuildPath = "job/{0}";
    private const string c_urlFormatTfsPluginBuildUrl = "{0}/team-build/build/{1}";
    private const string c_urlFormatTfsPluginBuildUrlWithParameters = "{0}/team-build/buildWithParameters/{1}";
    private const string c_queryFormatTree = "jobs[name,buildable{0}]";
    private const string c_queryJobDepthSeparator = ",";
    private const string c_buildParamDelimiterChars = "\r\n\f";
    private const string c_buildParamDelimiterExpression = "[\r\n\f]?";
    private const string c_buildParamKeyValueSeparator = ":";
    private const string c_buildParamsPattern = "^((.+?)(\\s*):(.*)[\r\n\f]?)+$";
    public const string BuildNameInputId = "buildName";
    public const string BuildAuthTokenInputId = "buildAuthToken";
    public const string BuildParamsInputId = "buildParams";
    public const string BuilParameterizedInputId = "buildParameterized";
    public const string RegistryPathUrlFormatBuildUrl = "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatBuildUrl";
    public const string RegistryPathUrlFormatGetAllBuilds = "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatGetAllBuilds";
    public const string RegistryPathUrlParamDepthGetAllBuilds = "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlParamDepthGetAllBuilds";
    public const string RegistryPathUrlFormatTfsPluginBuildUrl = "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatTfsPluginBuildUrl";
    private static readonly char[] s_buildParamsDelimiterSplit = "\r\n\f".ToCharArray();
    private static readonly string[] s_supportedEventTypes = new string[5]
    {
      "git.push",
      "git.pullrequest.merged",
      "build.complete",
      "tfvc.checkin",
      "ms.vss-release.deployment-completed-event"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();
    private static readonly string s_defaultGetAllBuildsTreeParam = TriggerGenericBuildAction.BuildGetAllBuildsTreeParam(5);

    public override string ConsumerId => "jenkins";

    public override string Id => "triggerGenericBuild";

    public override string Name => JenkinsConsumerResources.TriggerGenericBuildActionName;

    public override string Description => JenkinsConsumerResources.TriggerGenericBuildActionDescription;

    public override string[] SupportedEventTypes => TriggerGenericBuildAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => TriggerGenericBuildAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = JenkinsConsumerResources.TriggerGenericBuildAction_BuildNameInputName,
        Description = JenkinsConsumerResources.TriggerGenericBuildAction_BuildNameInputDescription,
        InputMode = InputMode.Combo,
        Id = "buildName",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        HasDynamicValueInformation = true,
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "serverBaseUrl",
          "username",
          "password"
        },
        Values = new InputValues()
        {
          IsLimitedToPossibleValues = true
        }
      },
      JenkinsConsumer.GetIntegrationLeveInputDescriptor(),
      new InputDescriptor()
      {
        Name = JenkinsConsumerResources.TriggerGenericBuildAction_BuildAuthTokenInputName,
        Description = JenkinsConsumerResources.TriggerGenericBuildAction_BuildAuthTokenInputDescription,
        InputMode = InputMode.PasswordBox,
        Id = "buildAuthToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "serverBaseUrl"
        }
      },
      new InputDescriptor()
      {
        Name = JenkinsConsumerResources.TriggerGenericBuildAction_BuildParameterizedInputName,
        Description = JenkinsConsumerResources.TriggerGenericBuildAction_BuildParameterizedInputDescription,
        InputMode = InputMode.CheckBox,
        Id = "buildParameterized",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.Boolean
        }
      },
      new InputDescriptor()
      {
        Name = JenkinsConsumerResources.TriggerGenericBuildAction_BuildParamsInputName,
        Description = JenkinsConsumerResources.TriggerGenericBuildAction_BuildParamsInputDescription,
        InputMode = InputMode.TextArea,
        Id = "buildParams",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String,
          Pattern = "^((.+?)(\\s*):(.*)[\r\n\f]?)+$"
        }
      }
    };

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      switch (inputId)
      {
        case "buildName":
          return this.GetInputValuesForBuildName(requestContext, currentConsumerInputValues);
        case "useTfsPlugin":
          return JenkinsConsumer.GetInputValuesForIntegrationLevel(requestContext, currentConsumerInputValues, new Func<IVssRequestContext, HttpClient>(((ConsumerActionImplementation) this).GetHttpClient));
        default:
          return base.GetInputValues(requestContext, inputId, currentConsumerInputValues);
      }
    }

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string uriString = (string) null;
      string str1 = (string) null;
      consumerInputValues.TryGetValue("serverBaseUrl", out uriString);
      consumerInputValues.TryGetValue("buildName", out str1);
      if (uriString == null || str1 == null)
        throw new ArgumentNullException();
      string str2;
      try
      {
        str2 = new Uri(uriString).Host;
      }
      catch (UriFormatException ex)
      {
        str2 = uriString;
      }
      return string.Format(JenkinsConsumerResources.TriggerGenericBuildAction_DetailedDescriptionFormat, (object) str2, (object) str1);
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string serverBaseUrl = JenkinsConsumer.NormalizeUrl(eventArgs.Notification.GetConsumerInput("serverBaseUrl", true));
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("username", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("password", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("buildName", true);
      string consumerInput4 = eventArgs.Notification.GetConsumerInput("buildAuthToken");
      bool useTfsPluginIntegration = string.Equals(eventArgs.Notification.GetConsumerInput("useTfsPlugin"), "tfs-plugin", StringComparison.OrdinalIgnoreCase);
      bool result;
      bool.TryParse(eventArgs.Notification.GetConsumerInput("buildParameterized"), out result);
      string buildParams = result ? eventArgs.Notification.GetConsumerInput("buildParams") : (string) null;
      bool flag = raisedEvent.EventType.Equals("git.pullrequest.merged", StringComparison.Ordinal);
      raisedEvent.EventType.Equals("build.complete", StringComparison.Ordinal);
      JObject jobject = raisedEvent.ToJObject();
      JObject resource = jobject["resource"] as JObject;
      if (useTfsPluginIntegration)
        buildParams = this.TransformToJson(result, buildParams, jobject);
      else if (flag)
        buildParams = TriggerGenericBuildAction.AppendPullRequstParameters(buildParams, resource);
      string url = TriggerGenericBuildAction.BuildTriggerBuildUrl(requestContext, serverBaseUrl, consumerInput3, result, useTfsPluginIntegration);
      ServiceHooksHttpRequestMessage httpRequestMessage = new ServiceHooksHttpRequestMessage(requestContext, eventArgs.Notification, HttpMethod.Post, url, consumerInput1, consumerInput2);
      string requestAsString;
      this.AddBuildParameters((HttpRequestMessage) httpRequestMessage, consumerInput4, buildParams, out requestAsString);
      JenkinsConsumer.AddCrumbHeaderIfNeeded(requestContext, (HttpRequestMessage) httpRequestMessage, serverBaseUrl, consumerInput1, consumerInput2, new Func<IVssRequestContext, HttpClient>(((ConsumerActionImplementation) this).GetHttpClient), out string _);
      return (ActionTask) new HttpActionTask((HttpRequestMessage) httpRequestMessage, httpRequestMessage.BuildHttpRequestStringRepresentation(requestAsString));
    }

    private InputValues GetInputValuesForBuildName(
      IVssRequestContext requestContext,
      IDictionary<string, string> currentConsumerInputValues)
    {
      InputValues forBuildNameCombo = TriggerGenericBuildAction.GetDefaultsForBuildNameCombo();
      string serverBaseUrl1;
      string username;
      string password;
      if (!JenkinsConsumer.TryGetConsumerInputs(currentConsumerInputValues, out serverBaseUrl1, out username, out password))
        return forBuildNameCombo;
      string serverBaseUrl2 = JenkinsConsumer.NormalizeUrl(serverBaseUrl1);
      try
      {
        string allBuildsUrl = TriggerGenericBuildAction.BuildGetAllBuildsUrl(requestContext, serverBaseUrl2);
        using (HttpClient httpClient = JenkinsConsumer.CreateHttpClient(requestContext, new Func<IVssRequestContext, HttpClient>(((ConsumerActionImplementation) this).GetHttpClient), username, password))
        {
          HttpResponseMessage result = httpClient.PauseTimerAndGetResult(requestContext, allBuildsUrl);
          if (result.IsSuccessStatusCode)
          {
            foreach (JToken filterBuildableNode in TriggerGenericBuildAction.FilterBuildableNodes(JObject.Parse(result.Content.ReadAsStringAsync().Result)))
            {
              string buildFullName = TriggerGenericBuildAction.GetBuildFullName(filterBuildableNode);
              forBuildNameCombo.PossibleValues.Add(new InputValue()
              {
                Value = buildFullName,
                DisplayValue = buildFullName
              });
            }
          }
          else if (result.StatusCode == HttpStatusCode.Unauthorized)
            forBuildNameCombo.Error = new InputValuesError()
            {
              Message = JenkinsConsumerResources.TriggerGenericBuildAction_QueryError_SuppliedCredentialsNotAuthorized
            };
          else
            forBuildNameCombo.Error = new InputValuesError()
            {
              Message = string.Format(JenkinsConsumerResources.TriggerGenericBuildAction_QueryError_ResponseFailureFormat, (object) result.ReasonPhrase)
            };
        }
        return forBuildNameCombo;
      }
      catch (Exception ex)
      {
        Exception exception = ex is AggregateException ? ex.InnerException : ex;
        forBuildNameCombo.Error = new InputValuesError()
        {
          Message = string.Format(JenkinsConsumerResources.TriggerGenericBuildAction_QueryError_ExceptionFormat, (object) exception.GetBaseException().Message)
        };
        return forBuildNameCombo;
      }
    }

    private void AddBuildParameters(
      HttpRequestMessage request,
      string buildAuthToken,
      string buildParams,
      out string requestAsString)
    {
      Dictionary<string, string> first1 = new Dictionary<string, string>();
      Dictionary<string, string> first2 = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(buildAuthToken))
      {
        first1.Add("token", buildAuthToken);
        first2.Add("token", SecurityHelper.GetMaskedValue(buildAuthToken));
      }
      Dictionary<string, string> buildParameters = this.ParseBuildParameters(buildParams);
      FormUrlEncodedContent urlEncodedContent = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) first2.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) buildParameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (p => p.Key), (Func<KeyValuePair<string, string>, string>) (p => p.Value)));
      requestAsString = urlEncodedContent.ReadAsStringAsync().Result;
      Dictionary<string, string> dictionary = first1.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) buildParameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (p => p.Key), (Func<KeyValuePair<string, string>, string>) (p => p.Value));
      request.Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) dictionary);
    }

    private Dictionary<string, string> ParseBuildParameters(string buildParams)
    {
      Dictionary<string, string> buildParameters = new Dictionary<string, string>();
      if (!string.IsNullOrWhiteSpace(buildParams))
      {
        string[] strArray = buildParams.Split(TriggerGenericBuildAction.s_buildParamsDelimiterSplit, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length != 0)
        {
          foreach (string str1 in strArray)
          {
            int length = str1.IndexOf(":");
            if (length != -1)
            {
              string key = str1.Substring(0, length).Trim();
              string str2 = str1.Substring(length + 1).Trim();
              if (!buildParameters.ContainsKey(key) || buildParameters.Remove(key))
                buildParameters.Add(key, str2);
            }
          }
        }
      }
      return buildParameters;
    }

    private static string BuildTriggerBuildUrl(
      IVssRequestContext requestContext,
      string serverBaseUrl,
      string buildName,
      bool buildHasParams,
      bool useTfsPluginIntegration)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return useTfsPluginIntegration ? string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatTfsPluginBuildUrl", true, buildHasParams ? "{0}/team-build/buildWithParameters/{1}" : "{0}/team-build/build/{1}"), (object) serverBaseUrl, (object) buildName) : string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatBuildUrl", true, buildHasParams ? "{0}/{1}/buildWithParameters" : "{0}/{1}/build"), (object) serverBaseUrl, (object) TriggerGenericBuildAction.BuildJenkinsPathFromBuildFullName(buildName));
    }

    private static InputValues GetDefaultsForBuildNameCombo() => new InputValues()
    {
      InputId = "buildName",
      IsLimitedToPossibleValues = true,
      PossibleValues = (IList<InputValue>) new List<InputValue>()
    };

    private static string BuildGetAllBuildsUrl(
      IVssRequestContext requestContext,
      string serverBaseUrl)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int depth = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlParamDepthGetAllBuilds", true, 5);
      string str = depth == 5 ? TriggerGenericBuildAction.s_defaultGetAllBuildsTreeParam : TriggerGenericBuildAction.BuildGetAllBuildsTreeParam(depth);
      return string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatGetAllBuilds", true, "{0}/api/json?depth={1}&tree={2}"), (object) serverBaseUrl, (object) depth, (object) str);
    }

    private static string BuildGetAllBuildsTreeParam(int depth)
    {
      string format = "jobs[name,buildable{0}]";
      for (int index = 1; index < depth; ++index)
        format = string.Format(format, (object) ",jobs[name,buildable{0}]");
      return string.Format(format, (object) string.Empty);
    }

    private static IEnumerable<JToken> FilterBuildableNodes(JObject responseJObject) => responseJObject.Descendants().Where<JToken>((Func<JToken, bool>) (node => node.Type == JTokenType.Object && node[(object) "buildable"] != null && (bool) node[(object) "buildable"]));

    private static string GetBuildFullName(JToken build) => build.Ancestors().Where<JToken>((Func<JToken, bool>) (node => node.Type == JTokenType.Object && !string.IsNullOrEmpty((string) node[(object) "name"]))).Aggregate<JToken, string>((string) build[(object) "name"], (Func<string, JToken, string>) ((currPath, node) => (string) node[(object) "name"] + "/" + currPath));

    private static string BuildJenkinsPathFromBuildFullName(string buildName) => string.Join("/", ((IEnumerable<string>) buildName.Split('/')).Select<string, string>((Func<string, string>) (s => string.Format("job/{0}", (object) Uri.EscapeDataString(s)))));

    private static string AppendPullRequstParameters(string buildParams, JObject resource)
    {
      if (resource == null)
        return buildParams;
      string str1 = resource["lastMergeCommit"][(object) "commitId"].ToString();
      string str2 = resource["pullRequestId"].ToString();
      if (string.IsNullOrEmpty(buildParams))
        return "pullRequestId:" + str2 + "\ncommitId:" + str1;
      return buildParams + "\npullRequestId:" + str2 + "\ncommitId:" + str1;
    }

    private string TransformToJson(bool buildHasParams, string buildParams, JObject raisedEvent)
    {
      Dictionary<string, string> buildParameters = this.ParseBuildParameters(buildParams);
      JObject jObject = new JObject();
      if (buildHasParams)
      {
        JArray jarray = new JArray((object) buildParameters.Select<KeyValuePair<string, string>, JObject>((Func<KeyValuePair<string, string>, JObject>) (p => new JObject()
        {
          ["name"] = (JToken) p.Key,
          ["value"] = (JToken) p.Value
        })));
        jObject["parameter"] = (JToken) jarray;
      }
      jObject["team-event"] = (JToken) raisedEvent;
      return "json:" + jObject.GetStringRepresentation();
    }
  }
}
