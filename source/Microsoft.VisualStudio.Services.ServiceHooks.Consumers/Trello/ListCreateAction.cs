// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Trello.ListCreateAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Trello
{
  [Export(typeof (ConsumerActionImplementation))]
  public class ListCreateAction : TrelloActionBase
  {
    private const string c_id = "createList";
    private const int c_listNameInputMaxLength = 16384;
    private const string c_listNameInputValueHint = "{{{message.text}}}";
    public const string ListNameInputId = "listName";
    private static string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public static string ConsumerActionId => "createList";

    public override string Id => "createList";

    public override string ConsumerId => "trello";

    public override string Name => TrelloConsumerResources.ListCreateActionName;

    public override string Description => TrelloConsumerResources.ListCreateActionDescription;

    public override string[] SupportedEventTypes => ListCreateAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => ListCreateAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = TrelloConsumerResources.ListCreateAction_InputBoardName,
        Description = TrelloConsumerResources.ListCreateAction_InputBoardDescription,
        InputMode = InputMode.Combo,
        Id = "boardId",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(64),
          MinLength = new int?(8)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "userToken"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = TrelloConsumerResources.ListCreateAction_InputAddToBottomName,
        Description = TrelloConsumerResources.ListCreateAction_InputAddToBottomDescription,
        InputMode = InputMode.CheckBox,
        Id = "addToBottom",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Boolean,
          IsRequired = false
        },
        Values = new InputValues()
        {
          DefaultValue = "False"
        }
      },
      new InputDescriptor()
      {
        Name = string.Format(TrelloConsumerResources.ListCreateAction_InputListNameName, (object) CommonConsumerResources.InputDescriptor_SupportsTemplatesIndicationLabel),
        Description = TrelloConsumerResources.ListCreateAction_InputListNameDescription,
        InputMode = InputMode.TextBox,
        Id = "listName",
        IsConfidential = false,
        GroupName = CommonConsumerResources.InputDescriptor_AdvancedGroupName,
        ValueHint = "{{{message.text}}}",
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = false,
          MaxLength = new int?(16384)
        }
      }
    };

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
        IsReadOnly = false,
        Error = inputValuesError2
      };
    }

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string selectedBoardValue = (string) null;
      consumerInputValues.TryGetValue("boardId", out selectedBoardValue);
      if (selectedBoardValue == null)
        throw new ArgumentNullException();
      string errorMessage = (string) null;
      InputValue inputValue = this.GetPossibleValues(requestContext, "boardId", consumerInputValues, out errorMessage).FirstOrDefault<InputValue>((Func<InputValue, bool>) (v => v.Value == selectedBoardValue));
      return string.Format(TrelloConsumerResources.ListCreateAction_DescriptionFormat, inputValue == null ? (object) selectedBoardValue : (object) inputValue.DisplayValue);
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs e)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      string consumerInput1 = e.Notification.GetConsumerInput("boardId", true);
      string consumerInput2 = e.Notification.GetConsumerInput("userToken", true);
      string consumerInput3 = e.Notification.GetConsumerInput("addToBottom");
      string str1 = TemplateRunner.Run(e.Notification.GetConsumerInput("listName"), raisedEvent);
      if (string.IsNullOrWhiteSpace(str1))
        str1 = raisedEvent.Message.Text;
      string str2 = str1.Substring(0, Math.Min(str1.Length, 16384));
      HttpMethod post = HttpMethod.Post;
      CachedRegistryService registryService1 = service;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/ServiceHooks/TrelloConsumer/ListCreateAction/UrlFormatCreateList";
      ref RegistryQuery local1 = ref registryQuery;
      string requestUri = string.Format(registryService1.GetValue<string>(requestContext1, in local1, true, "https://api.trello.com/1/lists?key={0}&token={1}"), (object) TrelloConsumer.ApplicationKey, (object) consumerInput2);
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(post, requestUri);
      string content = new JObject()
      {
        {
          "name",
          (JToken) str2
        },
        {
          "idBoard",
          (JToken) consumerInput1
        },
        {
          "pos",
          (JToken) (consumerInput3 == null ? "top" : (Convert.ToBoolean(consumerInput3) ? "bottom" : "top"))
        }
      }.ToString();
      httpRequestMessage.Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json");
      HttpRequestStringRepresentationBuilder representationBuilder1 = new HttpRequestStringRepresentationBuilder(httpRequestMessage);
      HttpRequestStringRepresentationBuilder representationBuilder2 = representationBuilder1;
      CachedRegistryService registryService2 = service;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) "/Service/ServiceHooks/TrelloConsumer/ListCreateAction/UrlFormatCreateList";
      ref RegistryQuery local2 = ref registryQuery;
      string str3 = string.Format(registryService2.GetValue<string>(requestContext2, in local2, true, "https://api.trello.com/1/lists?key={0}&token={1}"), (object) TrelloConsumer.ApplicationKey, (object) SecurityHelper.GetMaskedValue(consumerInput2));
      representationBuilder2.ConfidentialRequestUri = str3;
      representationBuilder1.AppendContent(content);
      return (ActionTask) new HttpActionTask(httpRequestMessage, representationBuilder1.ToString());
    }
  }
}
