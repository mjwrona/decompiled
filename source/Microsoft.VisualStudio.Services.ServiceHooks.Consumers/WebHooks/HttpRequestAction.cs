// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks.HttpRequestAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks
{
  [Export(typeof (ConsumerActionImplementation))]
  public class HttpRequestAction : EventTransformerConsumerActionImplementation
  {
    private const string c_space = " ";
    private const string c_crLf = "\r\n";
    private const string c_newLine = "\n";
    private const string c_id = "httpRequest";
    private const int c_basicAuthUsernameMinLength = 1;
    private const int c_basicAuthUsernameMaxLength = 256;
    private const int c_basicAuthPasswordMinLength = 1;
    private const int c_basicAuthPasswordMaxLength = 256;
    private const string c_basicAuthHeaderScheme = "Basic";
    private const string c_basicAuthTokenFormat = "{0}:{1}";
    private const string c_httpHeaderKeyValueSeparator = ":";
    private const string c_httpHeadersDelimiterChars = "\r\n\f";
    private const string c_contentTypeJson = "application/json";
    private const string c_httpHeaderAuthorization = "Authorization";
    private const string c_httpHeadersDelimiterExpression = "[\r\n\f]+";
    private const string c_basicAuthCredsPattern = "^[\\S]+$";
    private const string c_httpHeadersPattern = "^([^:\\s]+:.*[\\n]*)*$";
    public const string UrlInputId = "url";
    public const string AcceptUntrustedCertsInputId = "acceptUntrustedCerts";
    public const string HeadersInputId = "httpHeaders";
    public const string BasicAuthUsernameInputId = "basicAuthUsername";
    public const string BasicAuthPasswordInputId = "basicAuthPassword";
    private static readonly char[] s_httpHeadersDelimiterSplit = "\r\n\f".ToCharArray();
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public static string ConsumerActionId => "httpRequest";

    public override string Id => "httpRequest";

    public override string ConsumerId => "webHooks";

    public override string Name => WebHooksConsumerResources.HttpRequestActionName;

    public override string Description => WebHooksConsumerResources.HttpRequestActionDescription;

    public override string[] SupportedEventTypes => HttpRequestAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => HttpRequestAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      HttpRequestAction.BuildUrlInputDescriptor(),
      HttpRequestAction.BuildAcceptUntrustedCertsInputDescriptor(),
      HttpRequestAction.BuildBasicAuthUsernameInputDescriptor(),
      HttpRequestAction.BuildBasicAuthPasswordInputDescriptor(),
      HttpRequestAction.BuildHeadersInputDescriptor()
    }.Union<InputDescriptor>(EventTransformerConsumerActionImplementation.BuildAllPayloadControllersInputDescriptors()).ToList<InputDescriptor>();

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string uriString = (string) null;
      consumerInputValues.TryGetValue("url", out uriString);
      if (uriString == null)
        throw new ArgumentNullException();
      try
      {
        return string.Format(WebHooksConsumerResources.HttpRequestAction_DescriptionFormat, (object) new Uri(uriString).Host);
      }
      catch (UriFormatException ex)
      {
        return uriString;
      }
    }

    public override void ValidateConsumerInputs(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputs)
    {
      base.ValidateConsumerInputs(requestContext, consumerInputs);
      string uriString;
      if (consumerInputs.ContainsKey("basicAuthPassword") && consumerInputs.TryGetValue("url", out uriString) && new Uri(uriString).Scheme != Uri.UriSchemeHttps)
        throw new ServiceHookException(WebHooksConsumerResources.InvalidInputs_UriSchemeMustBeHttpsWhenConfidentialInputIncluded);
    }

    public virtual ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs e)
    {
      string targetUrl = this.GetTargetUrl(requestContext, e);
      bool result;
      bool.TryParse(e.Notification.GetConsumerInput("acceptUntrustedCerts"), out result);
      string consumerInput1 = e.Notification.GetConsumerInput("basicAuthUsername");
      string consumerInput2 = e.Notification.GetConsumerInput("basicAuthPassword");
      string consumerInput3 = e.Notification.GetConsumerInput("httpHeaders");
      WebHookEvent webHookEvent = new WebHookEvent();
      webHookEvent.CloneEventProperties(raisedEvent);
      webHookEvent.NotificationId = e.Notification.Id;
      webHookEvent.SubscriptionId = e.Notification.SubscriptionId;
      JObject jObject = EventTransformerConsumerActionImplementation.TransformEvent(e.Notification, (Event) webHookEvent, this.GetDefaultResourceDetailsToSend(e.Notification), this.GetDefaultMessagesToSend(e.Notification), this.GetDefaultDetailedMessagesToSend(e.Notification));
      HttpRequestMessage httpRequestMessage = (HttpRequestMessage) new ServiceHooksHttpRequestMessage(requestContext, e.Notification, HttpMethod.Post, targetUrl);
      string stringRepresentation = EventTransformerConsumerActionImplementation.GetStringRepresentation(jObject);
      if (webHookEvent.SessionToken != null && !string.IsNullOrWhiteSpace(webHookEvent.SessionToken.Token))
        jObject["sessionToken"][(object) "token"] = (JToken) SecurityHelper.GetMaskedValue(webHookEvent.SessionToken.Token);
      string requestBody = EventTransformerConsumerActionImplementation.GetStringRepresentation(jObject, false, Formatting.Indented).Replace("\r\n", "\n");
      httpRequestMessage.Content = (HttpContent) new StringContent(stringRepresentation, Encoding.UTF8, "application/json");
      HttpRequestStringRepresentationBuilder requestAsString = new HttpRequestStringRepresentationBuilder(httpRequestMessage, requestBody);
      this.AddBasicAuthenticationHeaders(consumerInput1, consumerInput2, httpRequestMessage, requestAsString);
      this.AddHeaders(consumerInput3, httpRequestMessage, requestAsString);
      return (ActionTask) new HttpActionTask(httpRequestMessage, requestAsString.ToString(), result);
    }

    public virtual string GetTargetUrl(IVssRequestContext requestContext, HandleEventArgs e) => e.Notification.GetConsumerInput("url", true);

    protected static InputDescriptor BuildHeadersInputDescriptor() => new InputDescriptor()
    {
      Name = WebHooksConsumerResources.HttpRequestAction_HeadersInputName,
      Description = WebHooksConsumerResources.HttpRequestAction_HeadersInputDescription,
      InputMode = InputMode.TextArea,
      Id = "httpHeaders",
      IsConfidential = false,
      Validation = new InputValidation()
      {
        DataType = InputDataType.String,
        IsRequired = false,
        Pattern = "^([^:\\s]+:.*[\\n]*)*$"
      }
    };

    protected static InputDescriptor BuildUrlInputDescriptor() => new InputDescriptor()
    {
      Name = WebHooksConsumerResources.HttpRequestAction_UrlInputName,
      Description = WebHooksConsumerResources.HttpRequestAction_UrlInputDescription,
      InputMode = InputMode.TextBox,
      Id = "url",
      IsConfidential = false,
      Validation = new InputValidation()
      {
        DataType = InputDataType.Uri,
        IsRequired = true
      }
    };

    protected static InputDescriptor BuildAcceptUntrustedCertsInputDescriptor() => new InputDescriptor()
    {
      Name = WebHooksConsumerResources.HttpRequestAction_AcceptUntrustedCertsInputName,
      Description = WebHooksConsumerResources.HttpRequestAction_AcceptUntrustedCertsInputDescription,
      InputMode = InputMode.CheckBox,
      Id = "acceptUntrustedCerts",
      IsConfidential = false
    };

    protected static InputDescriptor BuildBasicAuthPasswordInputDescriptor() => new InputDescriptor()
    {
      Name = WebHooksConsumerResources.HttpRequestAction_BasicAuthPasswordName,
      Description = WebHooksConsumerResources.HttpRequestAction_BasicAuthPasswordDescription,
      InputMode = InputMode.PasswordBox,
      Id = "basicAuthPassword",
      IsConfidential = true,
      Validation = new InputValidation()
      {
        MinLength = new int?(1),
        MaxLength = new int?(256),
        DataType = InputDataType.String,
        IsRequired = false,
        Pattern = "^[\\S]+$"
      },
      DependencyInputIds = (IList<string>) new List<string>()
      {
        "url"
      }
    };

    protected static InputDescriptor BuildBasicAuthUsernameInputDescriptor() => new InputDescriptor()
    {
      Name = WebHooksConsumerResources.HttpRequestAction_BasicAuthUsernameName,
      Description = WebHooksConsumerResources.HttpRequestAction_BasicAuthUsernameDescription,
      InputMode = InputMode.TextBox,
      Id = "basicAuthUsername",
      IsConfidential = false,
      Validation = new InputValidation()
      {
        MinLength = new int?(1),
        MaxLength = new int?(256),
        DataType = InputDataType.String,
        IsRequired = false,
        Pattern = "^[\\S]+$"
      }
    };

    private void AddBasicAuthenticationHeaders(
      string basicAuthUsername,
      string basicAuthPassword,
      HttpRequestMessage request,
      HttpRequestStringRepresentationBuilder requestAsString)
    {
      if (string.IsNullOrWhiteSpace(basicAuthUsername) && string.IsNullOrWhiteSpace(basicAuthPassword))
        return;
      string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", basicAuthUsername == null ? (object) string.Empty : (object) basicAuthUsername.Trim(), basicAuthPassword == null ? (object) string.Empty : (object) basicAuthPassword.Trim())));
      request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
      requestAsString.AddHeader("Authorization", "Basic " + SecurityHelper.GetMaskedValue(base64String), false);
    }

    private void AddHeaders(
      string headers,
      HttpRequestMessage request,
      HttpRequestStringRepresentationBuilder requestAsString)
    {
      if (string.IsNullOrEmpty(headers))
        return;
      string[] strArray = headers.Split(HttpRequestAction.s_httpHeadersDelimiterSplit, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
        return;
      foreach (string str1 in strArray)
      {
        int length = str1.IndexOf(":");
        if (length != -1)
        {
          string str2 = str1.Substring(0, length).Trim();
          string headerValue = str1.Substring(length + 1).Trim();
          if (!request.Headers.Contains(str2) || request.Headers.Remove(str2) && requestAsString.RemoveFirstHeader(str2))
          {
            request.Headers.Add(str2, headerValue);
            requestAsString.AddHeader(str2, headerValue, true);
          }
        }
      }
    }
  }
}
