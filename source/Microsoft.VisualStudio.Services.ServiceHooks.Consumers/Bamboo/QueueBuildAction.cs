// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Bamboo.QueueBuildAction
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
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Bamboo
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class QueueBuildAction : ConsumerActionImplementation
  {
    private const string c_id = "queueBuild";
    private const string c_forwardSlash = "/";
    private const int c_defaultRequestTimeoutSeconds = 30;
    private const string c_formatPlansUrl = "{0}rest/api/latest/plan.json?os_authType=basic&max-result={1}";
    private const string c_formatBuildQueueUrl = "{0}rest/api/latest/queue/{1}?os_authType=basic";
    private const int c_defaultResultCount = 200;
    private const string c_defaultResultCountRegistryPath = "/Service/ServiceHooks/BambooConsumer/QueueBuildAction/BuildPlansResultCount";
    public const string PlanNameInputId = "planName";
    private static readonly string[] s_supportedEventTypes = new string[3]
    {
      "git.push",
      "build.complete",
      "tfvc.checkin"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();
    private static readonly TimeSpan s_defaultRequestTimeout = TimeSpan.FromSeconds(30.0);

    public override string ConsumerId => "bamboo";

    public override string Id => "queueBuild";

    public override string Name => BambooConsumerResources.QueueBuildActionName;

    public override string Description => BambooConsumerResources.QueueBuildActionDescription;

    public override string[] SupportedEventTypes => QueueBuildAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => QueueBuildAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "username",
          "password",
          "serverBaseUrl"
        },
        Description = BambooConsumerResources.QueueBuildAction_PlanNameInputDescription,
        GroupName = (string) null,
        HasDynamicValueInformation = true,
        Id = "planName",
        InputMode = InputMode.Combo,
        IsConfidential = false,
        Name = BambooConsumerResources.QueueBuildAction_PlanNameInputName,
        UseInDefaultDescription = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        ValueHint = (string) null,
        Values = (InputValues) null
      }
    };

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      return inputId != "planName" ? base.GetInputValues(requestContext, inputId, currentConsumerInputValues) : this.GetAvailableBuildPlans(requestContext, currentConsumerInputValues);
    }

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string uriString;
      consumerInputValues.TryGetValue("serverBaseUrl", out uriString);
      string str1;
      consumerInputValues.TryGetValue("planName", out str1);
      if (uriString != null)
      {
        if (str1 != null)
        {
          string str2;
          try
          {
            str2 = new Uri(uriString).Host;
          }
          catch (UriFormatException ex)
          {
            str2 = uriString;
          }
          return string.Format(BambooConsumerResources.QueueBuildAction_DetailedDescriptionFormat, (object) str2, (object) str1);
        }
      }
      throw new ArgumentNullException();
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("serverBaseUrl", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("username", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("password", true);
      string consumerInput4 = eventArgs.Notification.GetConsumerInput("planName", true);
      if (!consumerInput1.EndsWith("/"))
        consumerInput1 += "/";
      string url = string.Format("{0}rest/api/latest/queue/{1}?os_authType=basic", (object) consumerInput1, (object) Uri.EscapeDataString(consumerInput4));
      ServiceHooksHttpRequestMessage request = new ServiceHooksHttpRequestMessage(requestContext, eventArgs.Notification, HttpMethod.Post, url, consumerInput2, consumerInput3);
      return (ActionTask) new HttpActionTask((HttpRequestMessage) request, request.BuildHttpRequestStringRepresentation(string.Empty));
    }

    private InputValues GetAvailableBuildPlans(
      IVssRequestContext requestContext,
      IDictionary<string, string> currentConsumerInputValues)
    {
      InputValues availableBuildPlans = new InputValues()
      {
        InputId = "planName",
        IsLimitedToPossibleValues = true,
        PossibleValues = (IList<InputValue>) new List<InputValue>()
      };
      string serverBaseUrl;
      string username;
      string password;
      if (!BambooConsumer.TryGetConsumerInputs(currentConsumerInputValues, out serverBaseUrl, out username, out password))
        return availableBuildPlans;
      int num = requestContext.GetService<ICachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/ServiceHooks/BambooConsumer/QueueBuildAction/BuildPlansResultCount", 200);
      try
      {
        using (HttpClient httpClient = this.CreateHttpClient(requestContext, HttpMethod.Get, username, password))
        {
          if (!serverBaseUrl.EndsWith("/"))
            serverBaseUrl += "/";
          string requestUri = string.Format("{0}rest/api/latest/plan.json?os_authType=basic&max-result={1}", (object) serverBaseUrl, (object) num);
          HttpResponseMessage result = httpClient.PauseTimerAndGetResult(requestContext, requestUri);
          if (result.IsSuccessStatusCode)
          {
            foreach (JToken jtoken1 in (JArray) JObject.Parse(result.Content.ReadAsStringAsync().Result)["plans"][(object) "plan"])
            {
              JToken jtoken2 = jtoken1[(object) "planKey"];
              availableBuildPlans.PossibleValues.Add(new InputValue()
              {
                Value = (string) jtoken2[(object) "key"],
                DisplayValue = (string) jtoken1[(object) "name"]
              });
            }
          }
          else if (result.StatusCode == HttpStatusCode.Unauthorized)
            availableBuildPlans.Error = new InputValuesError()
            {
              Message = BambooConsumerResources.QueueBuildAction_QueryError_SuppliedCredentialsNotAuthorized
            };
          else
            availableBuildPlans.Error = new InputValuesError()
            {
              Message = string.Format(BambooConsumerResources.QueueBuildAction_QueryError_ResponseFailureFormat, (object) result.ReasonPhrase)
            };
        }
        return availableBuildPlans;
      }
      catch (Exception ex)
      {
        Exception exception = ex is AggregateException ? ex.InnerException : ex;
        availableBuildPlans.Error = new InputValuesError()
        {
          Message = string.Format(BambooConsumerResources.QueueBuildAction_QueryError_ExceptionFormat, (object) exception.GetBaseException().Message)
        };
        return availableBuildPlans;
      }
    }

    private HttpClient CreateHttpClient(
      IVssRequestContext requestContext,
      HttpMethod method,
      string username,
      string password)
    {
      HttpClient httpClient = this.GetHttpClient(requestContext);
      httpClient.Timeout = QueueBuildAction.s_defaultRequestTimeout;
      httpClient.SetBasicAuthentication(username, password);
      return httpClient;
    }
  }
}
