// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HipChat.PostMessageToRoomAction
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HipChat
{
  [Export(typeof (ConsumerActionImplementation))]
  public class PostMessageToRoomAction : ConsumerActionImplementation
  {
    private const string c_id = "postMessageToRoom";
    private const string c_roomNameInputId = "roomName";
    private const string c_showDetailsInputId = "showDetails";
    private const string c_bgColorInputId = "bgColor";
    private const string c_notifyRoomInputId = "notifyRoom";
    private const int c_roomNameInputLengthMin = 1;
    private const int c_roomNameInputLengthMax = 50;
    private const string c_urlFormatSendRoomNotification = "https://api.hipchat.com/v2/room/{0}/notification";
    private const string c_urlFormatGetAllRooms = "https://api.hipchat.com/v2/room";
    private const string c_urlFormatGetSessionToken = "https://api.hipchat.com/v2/oauth/token/{0}";
    private const string c_schemeAuthorizationHeader = "Bearer";
    private const int c_defaultRequestTimeoutSeconds = 30;
    private const string c_authorizationHeaderName = "Authorization";
    private const string c_defaultColorValue = "-";
    private const string c_hipChatClientTokenType = "client";
    private const string c_hipChatUserTokenType = "user";
    public const string RegistryPathUrlFormatSendRoomNotification = "/Service/ServiceHooks/HipChatConsumer/PostMessageToRoomAction/UrlFormatSendRoomNotification";
    public const string RegistryPathUrlFormatGetAllRooms = "/Service/ServiceHooks/HipChatConsumer/PostMessageToRoomAction/UrlFormatGetAllRooms";
    public const string RegistryPathUrlFormatGetSessionToken = "/Service/ServiceHooks/HipChatConsumer/PostMessageToRoomAction/UrlFormatGetSessionToken";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();
    private static readonly List<InputValue> s_bgColorPossibleValues = PostMessageToRoomAction.BuildMessageColorPossibleValues();
    private static readonly Regex s_hipChatMessageReplacementRegEx = new Regex("(?<begin_p_tag>\\<p\\>)|(?<end_p_tag>\\</p\\>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly TimeSpan s_defaultRequestTimeout = TimeSpan.FromSeconds(30.0);

    public static string ConsumerActionId => "postMessageToRoom";

    public static string RoomNameInputId => "roomName";

    public static string BgColorInputId => "bgColor";

    public static string NotifyRoomInputId => "notifyRoom";

    public static string ShowDetailsInputId => "showDetails";

    public override string ConsumerId => HipChatConsumer.ConsumerId;

    public override string Id => "postMessageToRoom";

    public override string Name => HipChatConsumerResources.PostMessageToRoomActionName;

    public override string Description => HipChatConsumerResources.PostMessageToRoomActionDescription;

    public override string[] SupportedEventTypes => PostMessageToRoomAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostMessageToRoomAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = HipChatConsumerResources.PostMessageToRoomAction_RoomNameInputName,
        Description = HipChatConsumerResources.PostMessageToRoomAction_RoomNameInputDescription,
        InputMode = InputMode.Combo,
        Id = "roomName",
        IsConfidential = false,
        HasDynamicValueInformation = true,
        UseInDefaultDescription = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MinLength = new int?(1),
          MaxLength = new int?(50)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          HipChatConsumer.AuthTokenInputId
        }
      },
      new InputDescriptor()
      {
        Name = HipChatConsumerResources.PostMessageToRoomAction_NotifyRoomInputName,
        Description = HipChatConsumerResources.PostMessageToRoomAction_NotifyRoomInputDescription,
        InputMode = InputMode.CheckBox,
        Id = "notifyRoom",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.Boolean
        }
      },
      new InputDescriptor()
      {
        Name = HipChatConsumerResources.PostMessageToRoomAction_ShowDetailsInputName,
        Description = HipChatConsumerResources.PostMessageToRoomAction_ShowDetailsInputDescription,
        InputMode = InputMode.CheckBox,
        Id = "showDetails",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.Boolean
        }
      },
      new InputDescriptor()
      {
        Name = HipChatConsumerResources.PostMessageToRoomAction_BgColorInputName,
        Description = HipChatConsumerResources.PostMessageToRoomAction_BgColorInputDescription,
        InputMode = InputMode.Combo,
        Id = "bgColor",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String
        },
        Values = new InputValues()
        {
          IsLimitedToPossibleValues = true,
          PossibleValues = (IList<InputValue>) PostMessageToRoomAction.s_bgColorPossibleValues
        }
      }
    };

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      if (inputId != "roomName")
        return base.GetInputValues(requestContext, inputId, currentConsumerInputValues);
      InputValues defaultsForRoomCombo = this.GetDefaultsForRoomCombo();
      string str1;
      if (!currentConsumerInputValues.TryGetValue(HipChatConsumer.AuthTokenInputId, out str1))
        return defaultsForRoomCombo;
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        using (HttpClient httpClient = this.CreateHttpClient(requestContext, str1))
        {
          HttpResponseMessage result = httpClient.PauseTimerAndGetResult(requestContext, string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/HipChatConsumer/PostMessageToRoomAction/UrlFormatGetSessionToken", true, "https://api.hipchat.com/v2/oauth/token/{0}"), (object) Uri.EscapeDataString(str1)));
          if (!result.IsSuccessStatusCode)
          {
            if (result.StatusCode == HttpStatusCode.Unauthorized)
              defaultsForRoomCombo.Error = new InputValuesError()
              {
                Message = HipChatConsumerResources.PostMessageToRoomAction_QueryError_SuppliedTokenNotAuthorized
              };
            else
              defaultsForRoomCombo.Error = new InputValuesError()
              {
                Message = string.Format(HipChatConsumerResources.PostMessageToRoomAction_QueryError_ResponseFailureFormat, (object) result.ReasonPhrase)
              };
            return defaultsForRoomCombo;
          }
          JObject jobject = JObject.Parse(result.Content.ReadAsStringAsync().Result);
          switch ((string) jobject.SelectToken("owner_type", true))
          {
            case "client":
              string str2 = (string) jobject.SelectToken("client.room.name", false);
              if (str2 != null)
                defaultsForRoomCombo.PossibleValues.Add(new InputValue()
                {
                  Value = str2,
                  DisplayValue = str2
                });
              return defaultsForRoomCombo;
            case "user":
              foreach (JToken jtoken in (JArray) JObject.Parse(httpClient.GetStringAsync(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/HipChatConsumer/PostMessageToRoomAction/UrlFormatGetAllRooms", true, "https://api.hipchat.com/v2/room")).Result)["items"])
                defaultsForRoomCombo.PossibleValues.Add(new InputValue()
                {
                  Value = (string) jtoken[(object) "name"],
                  DisplayValue = (string) jtoken[(object) "name"]
                });
              return defaultsForRoomCombo;
            default:
              defaultsForRoomCombo.Error = new InputValuesError()
              {
                Message = HipChatConsumerResources.PostMessageToRoomAction_QueryError_SuppliedTokenNotAuthorized
              };
              return defaultsForRoomCombo;
          }
        }
      }
      catch (Exception ex)
      {
        Exception exception = ex is AggregateException ? ex.InnerException : ex;
        defaultsForRoomCombo.Error = new InputValuesError()
        {
          Message = string.Format(HipChatConsumerResources.PostMessageToRoomAction_QueryError_ExceptionFormat, (object) exception.GetBaseException().Message)
        };
        return defaultsForRoomCombo;
      }
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string consumerInput1 = eventArgs.Notification.GetConsumerInput(HipChatConsumer.AuthTokenInputId, true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("roomName", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("bgColor");
      bool result1;
      bool.TryParse(eventArgs.Notification.GetConsumerInput("showDetails"), out result1);
      bool result2;
      bool.TryParse(eventArgs.Notification.GetConsumerInput("notifyRoom"), out result2);
      string message = this.FixHtmlTagsOnMessage(result1 ? raisedEvent.DetailedMessage.Html : raisedEvent.Message.Html);
      HttpRequestMessage requestMessage = this.CreateRequestMessage(HttpMethod.Post, string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/HipChatConsumer/PostMessageToRoomAction/UrlFormatSendRoomNotification", true, "https://api.hipchat.com/v2/room/{0}/notification"), (object) Uri.EscapeDataString(consumerInput2)), consumerInput1);
      string roomPayload = PostMessageToRoomAction.BuildPostMessageToRoomPayload(consumerInput3, result2, message);
      requestMessage.Content = (HttpContent) new StringContent(roomPayload, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask(requestMessage, requestMessage.BuildHttpRequestStringRepresentation(roomPayload));
    }

    private static string BuildPostMessageToRoomPayload(
      string bgColorMessage,
      bool notifyRoom,
      string message)
    {
      JObject jobject = new JObject();
      jobject.Add(nameof (message), (JToken) message);
      if (bgColorMessage != null && bgColorMessage != "-")
        jobject.Add("color", (JToken) bgColorMessage);
      jobject.Add("notify", (JToken) notifyRoom);
      return jobject.ToString(CommonConsumerSettings.JsonSerializerSettings.Formatting, CommonConsumerSettings.JsonSerializerSettings.Converters.ToArray<JsonConverter>());
    }

    private HttpClient CreateHttpClient(IVssRequestContext requestContext, string authToken)
    {
      HttpClient httpClient = this.GetHttpClient(requestContext);
      httpClient.Timeout = PostMessageToRoomAction.s_defaultRequestTimeout;
      httpClient.DefaultRequestHeaders.Authorization = this.BuildAuthorizationHeader(authToken);
      return httpClient;
    }

    private HttpRequestMessage CreateRequestMessage(
      HttpMethod method,
      string requestUrl,
      string authToken)
    {
      return new HttpRequestMessage(method, requestUrl)
      {
        Headers = {
          Authorization = this.BuildAuthorizationHeader(authToken)
        }
      };
    }

    private AuthenticationHeaderValue BuildAuthorizationHeader(string authToken) => new AuthenticationHeaderValue("Bearer", HttpEncodeHelper.EncodeHeaderValue(authToken));

    private string FixHtmlTagsOnMessage(string message) => PostMessageToRoomAction.s_hipChatMessageReplacementRegEx.Replace(message, (MatchEvaluator) (m =>
    {
      if (m.Groups["begin_p_tag"].Success)
        return string.Empty;
      return m.Groups["end_p_tag"].Success ? "<br/>" : m.Value;
    }));

    private InputValues GetDefaultsForRoomCombo() => new InputValues()
    {
      InputId = "roomName",
      DefaultValue = string.Empty,
      IsDisabled = false,
      IsLimitedToPossibleValues = true,
      IsReadOnly = false,
      PossibleValues = (IList<InputValue>) new List<InputValue>()
    };

    private static List<InputValue> BuildMessageColorPossibleValues()
    {
      List<InputValue> source = new List<InputValue>()
      {
        new InputValue()
        {
          Value = "yellow",
          DisplayValue = HipChatConsumerResources.PostMessageToRoomAction_BgColorYellow
        },
        new InputValue()
        {
          Value = "red",
          DisplayValue = HipChatConsumerResources.PostMessageToRoomAction_BgColorRed
        },
        new InputValue()
        {
          Value = "green",
          DisplayValue = HipChatConsumerResources.PostMessageToRoomAction_BgColorGreen
        },
        new InputValue()
        {
          Value = "purple",
          DisplayValue = HipChatConsumerResources.PostMessageToRoomAction_BgColorPurple
        },
        new InputValue()
        {
          Value = "gray",
          DisplayValue = HipChatConsumerResources.PostMessageToRoomAction_BgColorGray
        },
        new InputValue()
        {
          Value = "random",
          DisplayValue = HipChatConsumerResources.PostMessageToRoomAction_BgColorRandom
        }
      };
      List<InputValue> inputValueList = new List<InputValue>();
      inputValueList.AddRange((IEnumerable<InputValue>) source.OrderBy<InputValue, string>((Func<InputValue, string>) (x => x.DisplayValue)));
      return inputValueList;
    }
  }
}
