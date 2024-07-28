// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Grafana.AddAnnotationAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Grafana
{
  [Export(typeof (ConsumerActionImplementation))]
  public class AddAnnotationAction : EventTransformerConsumerActionImplementation
  {
    private const string c_space = " ";
    private const string c_slash = "/";
    private const string c_urlFormat = "{0}{1}";
    private const string c_webUriKey = "web";
    private const string c_hrefUriKey = "href";
    private const string c_allDashboardsInputvalue = "All";
    private const string c_id = "addAnnotation";
    private const string c_tokenAuthHeaderScheme = "Bearer";
    private const string c_contentTypeJson = "application/json";
    private const string c_httpHeaderAuthorization = "Authorization";
    private const string c_getDashboardsApiUrlFormat = "{0}api/search?type=dash-db";
    private const string c_addAnnotationApiUrlFormat = "{0}api/annotations";
    private const int c_defaultRequestTimeoutSeconds = 30;
    private const string dashboardIdConstant = "dashboardId";
    private const string isRegionConstant = "isRegion";
    private const string timeConstant = "time";
    private const string timeEndConstant = "timeEnd";
    private const string tagsConstant = "tags";
    private const string textConstant = "text";
    private const string deploymentConstant = "deployment";
    private const string nameConstant = "name";
    private const string deploymentStatusConstant = "deploymentStatus";
    private const string completedOnConstant = "completedOn";
    private const string startedOnConstant = "startedOn";
    private const string releaseConstant = "release";
    private const string releaseEnvironmentConstant = "releaseEnvironment";
    private const string linksConstant = "_links";
    public const string AnnotationDeploymentDurationWindowInputId = "annotationDeploymentDurationWindow";
    public const string TagsInputId = "tags";
    public const string TextInputId = "text";
    public const string DashboardIdInputId = "dashboardId";
    public const string RegistryPathUrlFormatAddAnnotation = "/Service/ServiceHooks/GrafanaConsumer/AddAnnotationAction/UrlFormatAddAnnotation";
    public const string RegistryPathUrlFormatQueryDashboards = "/Service/ServiceHooks/GrafanaConsumer/AddAnnotationAction/UrlFormatQueryDashboards";
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "ms.vss-release.deployment-completed-event"
    };
    private static readonly string[] s_resourceVersion30Preview1 = new string[1]
    {
      "3.0-preview.1"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "ms.vss-release.deployment-completed-event",
        AddAnnotationAction.s_resourceVersion30Preview1
      }
    };
    private static readonly TimeSpan s_defaultRequestTimeout = TimeSpan.FromSeconds(30.0);
    private static readonly IDictionary<string, Func<JObject, bool, string, JObject>> s_eventAttachmentBuilders = (IDictionary<string, Func<JObject, bool, string, JObject>>) new Dictionary<string, Func<JObject, bool, string, JObject>>()
    {
      {
        "ms.vss-release.deployment-completed-event",
        new Func<JObject, bool, string, JObject>(AddAnnotationAction.CreatePayloadForReleaseDeploymentCompleted)
      }
    };

    public static string ConsumerActionId => "addAnnotation";

    public override string Id => "addAnnotation";

    public override string ConsumerId => "grafana";

    public override string Name => GrafanaConsumerResources.AddAnnotationActionName;

    public override string Description => GrafanaConsumerResources.AddAnnotationActionDescription;

    public override string[] SupportedEventTypes => AddAnnotationAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => AddAnnotationAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = GrafanaConsumerResources.AddAnnotationAction_TagsInputName,
        Description = GrafanaConsumerResources.AddAnnotationAction_TagsInputDescription,
        InputMode = InputMode.TextBox,
        Id = "tags",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        }
      },
      new InputDescriptor()
      {
        Name = GrafanaConsumerResources.AddAnnotationAction_DeploymentDurationInputName,
        Description = GrafanaConsumerResources.AddAnnotationAction_DeploymentDurationInputDescription,
        InputMode = InputMode.CheckBox,
        Id = "annotationDeploymentDurationWindow",
        IsConfidential = false
      },
      new InputDescriptor()
      {
        Name = GrafanaConsumerResources.AddAnnotationAction_TextInputName,
        Description = GrafanaConsumerResources.AddAnnotationAction_TextInputDescription,
        InputMode = InputMode.TextBox,
        Id = "text",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = false
        }
      },
      new InputDescriptor()
      {
        Name = GrafanaConsumerResources.AddAnnotationAction_DashboardIdInputName,
        Description = GrafanaConsumerResources.AddAnnotationAction_DashboardIdInputDescription,
        InputMode = InputMode.Combo,
        Id = "dashboardId",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = false
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "url",
          "apiToken"
        },
        HasDynamicValueInformation = true
      }
    };

    protected IList<InputValue> GetPossibleValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues,
      out string errorMessage)
    {
      IList<InputValue> possibleValues = (IList<InputValue>) new List<InputValue>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      errorMessage = (string) null;
      string grafanaUrl;
      string apiToken;
      if (service != null && currentConsumerInputValues.TryGetValue("url", out grafanaUrl) && currentConsumerInputValues.TryGetValue("apiToken", out apiToken))
      {
        if (inputId.Equals("dashboardId"))
        {
          try
          {
            string dashboardsApiUrl = this.GetDashboardsApiUrl(requestContext, service, grafanaUrl);
            using (HttpClient httpClient = this.CreateHttpClient(requestContext, apiToken))
              this.GetDashboardsInputValues(requestContext, httpClient, dashboardsApiUrl, ref possibleValues, ref errorMessage);
          }
          catch (Exception ex)
          {
            errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, GrafanaConsumerResources.QueryExceptionFormat, (object) ex.Message);
          }
        }
        else
        {
          errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, GrafanaConsumerResources.AddAnnotationAction_QueryDashboardValues_InvalidInputIdError, (object) inputId);
          throw new InvalidOperationException(errorMessage);
        }
      }
      return possibleValues;
    }

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      string errorMessage = (string) null;
      IList<InputValue> possibleValues = this.GetPossibleValues(requestContext, inputId, currentConsumerInputValues, out errorMessage);
      InputValuesError inputValuesError1;
      if (errorMessage == null)
      {
        inputValuesError1 = (InputValuesError) null;
      }
      else
      {
        inputValuesError1 = new InputValuesError();
        inputValuesError1.Message = errorMessage;
      }
      InputValuesError inputValuesError2 = inputValuesError1;
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = string.Empty,
        PossibleValues = possibleValues,
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = true,
        Error = inputValuesError2
      };
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Event>(raisedEvent, nameof (raisedEvent));
      ArgumentUtility.CheckForNull<HandleEventArgs>(eventArgs, nameof (eventArgs));
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("url", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("apiToken", true);
      ArgumentUtility.CheckStringForNullOrEmpty(consumerInput1, "url");
      ArgumentUtility.CheckStringForNullOrEmpty(consumerInput2, "apiToken");
      string annotationApiiUrl = this.GetAddAnnotationApiiUrl(requestContext, consumerInput1);
      JObject jobject = this.TransformEvent(raisedEvent, eventArgs);
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, annotationApiiUrl);
      httpRequestMessage.Content = (HttpContent) new StringContent(jobject.ToString(), Encoding.UTF8, "application/json");
      HttpRequestStringRepresentationBuilder representationBuilder = new HttpRequestStringRepresentationBuilder(httpRequestMessage);
      representationBuilder.AppendContent(jobject.ToString());
      httpRequestMessage.Headers.Authorization = this.BuildAuthorizationHeader(consumerInput2);
      representationBuilder.AddHeader("Authorization", "Bearer " + SecurityHelper.GetMaskedValue(consumerInput2), false);
      return (ActionTask) new HttpActionTask(httpRequestMessage, representationBuilder.ToString());
    }

    private JObject TransformEvent(Event raisedEvent, HandleEventArgs eventArgs)
    {
      JObject annotationPayload = new JObject();
      try
      {
        if (AddAnnotationAction.s_eventAttachmentBuilders.ContainsKey(raisedEvent.EventType))
        {
          if (!(raisedEvent.Resource is JObject jobject) && raisedEvent.Resource != null)
            jobject = JObject.FromObject(raisedEvent.Resource, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings));
          bool result;
          bool.TryParse(eventArgs.Notification.GetConsumerInput("annotationDeploymentDurationWindow"), out result);
          string consumerInput = eventArgs.Notification.GetConsumerInput("text");
          annotationPayload = AddAnnotationAction.s_eventAttachmentBuilders[raisedEvent.EventType](jobject, result, consumerInput) ?? new JObject();
          this.AddDashboardIdToPayload(eventArgs, ref annotationPayload);
          this.AddTagsToPayload(eventArgs, ref annotationPayload);
        }
        return annotationPayload;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, GrafanaConsumerResources.ErrorWhileCreatingGrafanaAnnotationPayLoad, (object) ex.Message), ex.InnerException);
      }
    }

    private void GetDashboardsInputValues(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      string getDashboardsUrl,
      ref IList<InputValue> possibleValues,
      ref string errorMessage)
    {
      HttpResponseMessage result = httpClient.PauseTimerAndGetResult(requestContext, getDashboardsUrl);
      if (result.IsSuccessStatusCode)
      {
        Task<string> task = result.Content.ReadAsStringAsync();
        task.Wait(AddAnnotationAction.s_defaultRequestTimeout);
        object obj1 = (object) JArray.Parse(task.Result);
        possibleValues.Add(new InputValue()
        {
          Value = "All",
          DisplayValue = "All"
        });
        // ISSUE: reference to a compiler-generated field
        if (AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (AddAnnotationAction)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        foreach (object obj2 in AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__4.Target((CallSite) AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__4, obj1))
        {
          IList<InputValue> inputValueList = possibleValues;
          InputValue inputValue1 = new InputValue();
          InputValue inputValue2 = inputValue1;
          // ISSUE: reference to a compiler-generated field
          if (AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (AddAnnotationAction)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, string> target1 = AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__1.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, string>> p1 = AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__1;
          // ISSUE: reference to a compiler-generated field
          if (AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (AddAnnotationAction), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj3 = AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__0.Target((CallSite) AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__0, obj2);
          string str1 = target1((CallSite) p1, obj3);
          inputValue2.Value = str1;
          InputValue inputValue3 = inputValue1;
          // ISSUE: reference to a compiler-generated field
          if (AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (AddAnnotationAction)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, string> target2 = AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__3.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, string>> p3 = AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__3;
          // ISSUE: reference to a compiler-generated field
          if (AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "title", typeof (AddAnnotationAction), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj4 = AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__2.Target((CallSite) AddAnnotationAction.\u003C\u003Eo__61.\u003C\u003Ep__2, obj2);
          string str2 = target2((CallSite) p3, obj4);
          inputValue3.DisplayValue = str2;
          InputValue inputValue4 = inputValue1;
          inputValueList.Add(inputValue4);
        }
      }
      else if (result.StatusCode == HttpStatusCode.Unauthorized)
        errorMessage = GrafanaConsumerResources.SuppliedTokenNotAuthorized;
      else
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, GrafanaConsumerResources.QueryResponseFailureFormat, (object) result.ReasonPhrase);
    }

    private string GetDashboardsApiUrl(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      string grafanaUrl)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) grafanaUrl, grafanaUrl.EndsWith("/") ? (object) string.Empty : (object) "/");
      return string.Format(registryService.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/GrafanaConsumer/AddAnnotationAction/UrlFormatAddAnnotation", true, "{0}api/search?type=dash-db"), (object) str);
    }

    private string GetAddAnnotationApiiUrl(IVssRequestContext requestContext, string grafanaUrl)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) grafanaUrl, grafanaUrl.EndsWith("/") ? (object) string.Empty : (object) "/");
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) "/Service/ServiceHooks/GrafanaConsumer/AddAnnotationAction/UrlFormatAddAnnotation";
      return string.Format(service.GetValue<string>(requestContext1, in local, true, "{0}api/annotations"), (object) str);
    }

    private static JObject CreatePayloadForReleaseDeploymentCompleted(
      JObject resource,
      bool annotationDeploymentWindow,
      string text)
    {
      JObject deploymentCompleted = new JObject();
      JToken jtokenFromJobject = AddAnnotationAction.GetJTokenFromJObject(resource, "deployment");
      JToken childTokenFromJtoken1 = AddAnnotationAction.GetChildTokenFromJToken(jtokenFromJobject, "completedOn");
      ArgumentUtility.CheckForNull<JToken>(childTokenFromJtoken1, "deploymentCompletedOnJToken");
      long timeInMilliseconds1 = AddAnnotationAction.ConvertToUnixEpochTimeInMilliseconds((DateTime) childTokenFromJtoken1);
      if (annotationDeploymentWindow)
      {
        JToken childTokenFromJtoken2 = AddAnnotationAction.GetChildTokenFromJToken(jtokenFromJobject, "startedOn");
        ArgumentUtility.CheckForNull<JToken>(childTokenFromJtoken2, "deploymentStartedOnJToken");
        long timeInMilliseconds2 = AddAnnotationAction.ConvertToUnixEpochTimeInMilliseconds((DateTime) childTokenFromJtoken2);
        deploymentCompleted.Add("isRegion", (JToken) true);
        deploymentCompleted.Add("time", (JToken) timeInMilliseconds2);
        deploymentCompleted.Add("timeEnd", (JToken) timeInMilliseconds1);
      }
      else
        deploymentCompleted.Add("time", (JToken) timeInMilliseconds1);
      if (!string.IsNullOrEmpty(text))
      {
        deploymentCompleted.Add(nameof (text), (JToken) text);
      }
      else
      {
        string stringVar = AddAnnotationAction.BuildDeploymentCompletedEventDefaultAnnotationDescription(jtokenFromJobject);
        ArgumentUtility.CheckStringForNullOrEmpty(stringVar, "defaultAnnotationDescription");
        deploymentCompleted.Add(nameof (text), (JToken) stringVar);
      }
      return deploymentCompleted;
    }

    private JObject AddTagsToPayload(HandleEventArgs eventArgs, ref JObject annotationPayload)
    {
      string consumerInput = eventArgs.Notification.GetConsumerInput("tags", true);
      ArgumentUtility.CheckForNull<string>(consumerInput, "tags");
      string[] o = consumerInput.Split(',');
      if (o.Length == 0)
        throw new InvalidOperationException(GrafanaConsumerResources.InvalidTagLength);
      annotationPayload.Add("tags", (JToken) JArray.FromObject((object) o));
      return annotationPayload;
    }

    private JObject AddDashboardIdToPayload(
      HandleEventArgs eventArgs,
      ref JObject annotationPayload)
    {
      string consumerInput = eventArgs.Notification.GetConsumerInput("dashboardId");
      if (!string.IsNullOrEmpty(consumerInput) && !consumerInput.Equals("All"))
        annotationPayload.Add("dashboardId", (JToken) Convert.ToInt64(consumerInput));
      return annotationPayload;
    }

    private static string BuildDeploymentCompletedEventDefaultAnnotationDescription(
      JToken deployment)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string deploymentStatus = AddAnnotationAction.GetDeploymentStatus(deployment);
      string valueFromDeployment1 = AddAnnotationAction.GetReleaseLinkValueFromDeployment(deployment);
      string valueFromDeployment2 = AddAnnotationAction.GetReleaseEnviromentLinkValueFromDeployment(deployment);
      string str;
      if (string.IsNullOrEmpty(valueFromDeployment1) || string.IsNullOrEmpty(valueFromDeployment2))
        str = GrafanaConsumerResources.AnnotationDefaultDescriptionIfDeploymentIsNotThere;
      else
        str = AddAnnotationAction.GetDefaultAnnotationDescription(GrafanaConsumerResources.AddAnnotationAction_TextDefaultDescription, new string[3]
        {
          valueFromDeployment1,
          valueFromDeployment2,
          deploymentStatus
        });
      return str;
    }

    private static string GetDefaultAnnotationDescription(
      string descriptionFormat,
      string[] descriptionParametersInOrder)
    {
      return string.Format(descriptionFormat, (object[]) descriptionParametersInOrder);
    }

    private static string GetReleaseLinkValueFromDeployment(JToken deployment)
    {
      JToken childTokenFromJtoken = AddAnnotationAction.GetChildTokenFromJToken(deployment, "release");
      if (!childTokenFromJtoken.HasValues)
        return (string) null;
      string stringFromJtoken = AddAnnotationAction.GetStringFromJToken(childTokenFromJtoken, "name");
      return AddAnnotationAction.BuildGrafanaTextLink(AddAnnotationAction.GetStringFromJToken(AddAnnotationAction.GetChildTokenFromJToken(AddAnnotationAction.GetChildTokenFromJToken(childTokenFromJtoken, "_links"), "web"), "href"), stringFromJtoken);
    }

    private static string GetReleaseEnviromentLinkValueFromDeployment(JToken deployment)
    {
      JToken childTokenFromJtoken = AddAnnotationAction.GetChildTokenFromJToken(deployment, "releaseEnvironment");
      if (!childTokenFromJtoken.HasValues)
        return (string) null;
      string stringFromJtoken = AddAnnotationAction.GetStringFromJToken(childTokenFromJtoken, "name");
      return AddAnnotationAction.BuildGrafanaTextLink(AddAnnotationAction.GetStringFromJToken(AddAnnotationAction.GetChildTokenFromJToken(AddAnnotationAction.GetChildTokenFromJToken(childTokenFromJtoken, "_links"), "web"), "href"), stringFromJtoken);
    }

    private static string GetDeploymentStatus(JToken deployment) => AddAnnotationAction.GetStringFromJToken(deployment, "deploymentStatus");

    private static string BuildGrafanaTextLink(string url, string name) => string.Format(GrafanaConsumerResources.GrafanaTextLinkFormat, (object) url, (object) name);

    private static long ConvertToUnixEpochTimeInMilliseconds(DateTime dateTime) => Convert.ToInt64((dateTime.ToUniversalTime() - AddAnnotationAction.UnixEpoch).TotalMilliseconds);

    private HttpClient CreateHttpClient(IVssRequestContext requestContext, string apiToken)
    {
      HttpClient httpClient = this.GetHttpClient(requestContext);
      httpClient.Timeout = AddAnnotationAction.s_defaultRequestTimeout;
      httpClient.DefaultRequestHeaders.Authorization = this.BuildAuthorizationHeader(apiToken);
      return httpClient;
    }

    private AuthenticationHeaderValue BuildAuthorizationHeader(string authToken) => new AuthenticationHeaderValue("Bearer", HttpEncodeHelper.EncodeHeaderValue(authToken));

    private static JToken GetJTokenFromJObject(JObject resource, string propertyName)
    {
      try
      {
        return resource[propertyName];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format(GrafanaConsumerResources.AddAnnotationAction_ErrorWhileExtractingJToken, (object) propertyName), ex);
      }
    }

    private static JToken GetChildTokenFromJToken(JToken resource, string propertyName)
    {
      try
      {
        return resource[(object) propertyName];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format(GrafanaConsumerResources.AddAnnotationAction_ErrorWhileExtractingJToken, (object) propertyName), ex);
      }
    }

    private static string GetStringFromJToken(JToken resource, string propertyName)
    {
      try
      {
        return (string) resource[(object) propertyName];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format(GrafanaConsumerResources.AddAnnotationAction_ErrorWhileExtractingJToken, (object) propertyName), ex);
      }
    }
  }
}
