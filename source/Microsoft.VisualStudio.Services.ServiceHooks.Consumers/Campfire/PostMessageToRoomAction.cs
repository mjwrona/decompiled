// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Campfire.PostMessageToRoomAction
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
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Campfire
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class PostMessageToRoomAction : ConsumerActionImplementation
  {
    private const string c_id = "postMessageToRoom";
    private const int c_roomIdInputMinValue = 1;
    private const string c_basicAuthDummyPassword = "X";
    private const int c_defaultRequestTimeoutSeconds = 30;
    private const string c_campfireBaseUrl = "https://{0}.campfirenow.com";
    private const string c_urlFormatGetAllRooms = "https://{0}.campfirenow.com/rooms.json";
    private const string c_urlFormatSendRoomNotification = "https://{0}.campfirenow.com/room/{1}/speak.json";
    private const string c_registryPathUrlFormatGetAllRooms = "/Service/ServiceHooks/CampfireConsumer/PostMessageToRoomAction/UrlFormatGetAllRooms";
    private const string c_registryPathUrlFormatSendRoomNotification = "/Service/ServiceHooks/CampfireConsumer/PostMessageToRoomAction/UrlFormatSendRoomNotification";
    public const string RoomIdInputId = "roomId";
    public const string ShowDetailsInputId = "showDetails";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();
    private static readonly TimeSpan s_defaultRequestTimeout = TimeSpan.FromSeconds(30.0);

    public override string ConsumerId => "campfire";

    public override string Id => "postMessageToRoom";

    public override string Name => CampfireConsumerResources.PostMessageToRoomActionName;

    public override string Description => CampfireConsumerResources.PostMessageToRoomActionDescription;

    public override string[] SupportedEventTypes => PostMessageToRoomAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostMessageToRoomAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = CampfireConsumerResources.PostMessageToRoomAction_RoomIdInputName,
        Description = CampfireConsumerResources.PostMessageToRoomAction_RoomIdInputDescription,
        InputMode = InputMode.Combo,
        Id = "roomId",
        IsConfidential = false,
        HasDynamicValueInformation = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.Number,
          MinValue = new Decimal?((Decimal) 1)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "accountName",
          "authToken"
        }
      },
      new InputDescriptor()
      {
        Name = CampfireConsumerResources.PostMessageToRoomAction_ShowDetailsInputName,
        Description = CampfireConsumerResources.PostMessageToRoomAction_ShowDetailsInputDescription,
        InputMode = InputMode.CheckBox,
        Id = "showDetails",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.Boolean
        }
      }
    };

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      if (inputId != "roomId")
        return base.GetInputValues(requestContext, inputId, currentConsumerInputValues);
      InputValues defaultsForRoomCombo = PostMessageToRoomAction.GetDefaultsForRoomCombo();
      string accountName;
      string authToken;
      if (!CampfireConsumer.TryGetConsumerInputs(currentConsumerInputValues, out accountName, out authToken))
        return defaultsForRoomCombo;
      try
      {
        string allRoomsUrl = PostMessageToRoomAction.BuildGetAllRoomsUrl(requestContext, accountName);
        using (HttpClient httpClient = this.CreateHttpClient(requestContext, authToken))
        {
          HttpResponseMessage result = httpClient.PauseTimerAndGetResult(requestContext, allRoomsUrl);
          if (result.IsSuccessStatusCode)
          {
            foreach (JToken jtoken in (JArray) JObject.Parse(result.Content.ReadAsStringAsync().Result)["rooms"])
              defaultsForRoomCombo.PossibleValues.Add(new InputValue()
              {
                Value = (string) jtoken[(object) "id"],
                DisplayValue = (string) jtoken[(object) "name"]
              });
          }
          else if (result.StatusCode == HttpStatusCode.Unauthorized)
            defaultsForRoomCombo.Error = new InputValuesError()
            {
              Message = CampfireConsumerResources.PostMessageToRoomAction_QueryError_SuppliedCredentialsNotAuthorized
            };
          else
            defaultsForRoomCombo.Error = new InputValuesError()
            {
              Message = string.Format(CampfireConsumerResources.PostMessageToRoomAction_QueryError_ResponseFailureFormat, (object) result.ReasonPhrase)
            };
        }
        return defaultsForRoomCombo;
      }
      catch (Exception ex)
      {
        Exception exception = ex is AggregateException ? ex.InnerException : ex;
        defaultsForRoomCombo.Error = new InputValuesError()
        {
          Message = string.Format(CampfireConsumerResources.PostMessageToRoomAction_QueryError_ExceptionFormat, (object) exception.GetBaseException().Message)
        };
      }
      return defaultsForRoomCombo;
    }

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string roomId = (string) null;
      consumerInputValues.TryGetValue("roomId", out roomId);
      if (roomId == null)
        throw new ArgumentNullException();
      InputValue inputValue = this.GetInputValues(requestContext, "roomId", consumerInputValues).PossibleValues.FirstOrDefault<InputValue>((Func<InputValue, bool>) (r => r.Value == roomId));
      return string.Format(CampfireConsumerResources.PostMessageToRoomActionDetailFormat, inputValue == null ? (object) roomId : (object) inputValue.DisplayValue);
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("accountName", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("authToken", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("roomId", true);
      bool result;
      bool.TryParse(eventArgs.Notification.GetConsumerInput("showDetails"), out result);
      string message = result ? raisedEvent.DetailedMessage.Text : raisedEvent.Message.Text;
      string url = PostMessageToRoomAction.BuildSendRoomNotificationUrl(requestContext, consumerInput1, consumerInput3);
      ServiceHooksHttpRequestMessage request = new ServiceHooksHttpRequestMessage(requestContext, eventArgs.Notification, HttpMethod.Post, url, consumerInput2, "X");
      string roomPayload = PostMessageToRoomAction.BuildPostMessageToRoomPayload(message);
      request.Content = (HttpContent) new StringContent(roomPayload, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask((HttpRequestMessage) request, request.BuildHttpRequestStringRepresentation(roomPayload));
    }

    private static string BuildGetAllRoomsUrl(IVssRequestContext requestContext, string accountName) => string.Format(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/CampfireConsumer/PostMessageToRoomAction/UrlFormatGetAllRooms", true, "https://{0}.campfirenow.com/rooms.json"), (object) accountName);

    private static string BuildSendRoomNotificationUrl(
      IVssRequestContext requestContext,
      string accountName,
      string roomId)
    {
      return string.Format(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/CampfireConsumer/PostMessageToRoomAction/UrlFormatSendRoomNotification", true, "https://{0}.campfirenow.com/room/{1}/speak.json"), (object) accountName, (object) roomId);
    }

    private static string BuildPostMessageToRoomPayload(string message) => JsonConvert.SerializeObject((object) JObject.FromObject((object) new
    {
      message = new{ body = message }
    }));

    private HttpClient CreateHttpClient(IVssRequestContext requestContext, string authToken)
    {
      HttpClient httpClient = this.GetHttpClient(requestContext);
      httpClient.Timeout = PostMessageToRoomAction.s_defaultRequestTimeout;
      httpClient.SetBasicAuthentication(authToken, "X");
      ServiceHooksHttpRequestMessage.AddUserAgentHeaders(httpClient.DefaultRequestHeaders);
      return httpClient;
    }

    private static InputValues GetDefaultsForRoomCombo() => new InputValues()
    {
      InputId = "roomId",
      IsDisabled = false,
      IsLimitedToPossibleValues = true,
      IsReadOnly = false,
      PossibleValues = (IList<InputValue>) new List<InputValue>()
    };
  }
}
