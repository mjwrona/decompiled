// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Trello.CardCreateAction
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
  public class CardCreateAction : TrelloActionBase
  {
    private const string c_id = "createCard";
    private const string c_propertyLabelsPattern = "^((red|orange|yellow|green|blue|purple),)*?(red|orange|yellow|green|blue|purple)+$";
    private const int c_cardNameInputMaxLength = 400;
    private const int c_cardDescriptionInputMaxLength = 16384;
    private const string c_cardNameInputValueHint = "{{{message.text}}}";
    private const string c_cardDescriptionInputValueHint = "{{{detailedMessage.markdown}}}";
    public const string CardNameInputId = "cardName";
    public const string CardDescriptionInputId = "cardDescription";
    private static string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public static string ConsumerActionId => "createCard";

    public override string Id => "createCard";

    public override string ConsumerId => "trello";

    public override string Name => TrelloConsumerResources.CardCreateActionName;

    public override string Description => TrelloConsumerResources.CardCreateActionDescription;

    public override string[] SupportedEventTypes => CardCreateAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => CardCreateAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = TrelloConsumerResources.CardCreateAction_InputBoardName,
        Description = TrelloConsumerResources.CardCreateAction_InputBoardDescription,
        HasDynamicValueInformation = true,
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
        }
      },
      new InputDescriptor()
      {
        Name = TrelloConsumerResources.CardCreateAction_InputListName,
        Description = TrelloConsumerResources.CardCreateAction_InputListDescription,
        InputMode = InputMode.Combo,
        Id = "listId",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(64),
          MinLength = new int?(9)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "boardId"
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = TrelloConsumerResources.CardCreateAction_InputLabelsName,
        Description = TrelloConsumerResources.CardCreateAction_InputLabelsDescription,
        InputMode = InputMode.TextBox,
        Id = "labels",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String,
          MaxLength = new int?(35),
          MinLength = new int?(3),
          Pattern = "^((red|orange|yellow|green|blue|purple),)*?(red|orange|yellow|green|blue|purple)+$"
        },
        Values = new InputValues()
        {
          PossibleValues = (IList<InputValue>) new List<InputValue>()
          {
            new InputValue() { Value = "red" },
            new InputValue() { Value = "orange" },
            new InputValue() { Value = "yellow" },
            new InputValue() { Value = "green" },
            new InputValue() { Value = "blue" },
            new InputValue() { Value = "purple" }
          },
          IsLimitedToPossibleValues = false
        }
      },
      new InputDescriptor()
      {
        Name = TrelloConsumerResources.CardCreateAction_InputAddToTopName,
        Description = TrelloConsumerResources.CardCreateAction_InputAddToTopDescription,
        InputMode = InputMode.CheckBox,
        Id = "addToTop",
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
        Name = string.Format(TrelloConsumerResources.CardCreateAction_InputCardNameName, (object) CommonConsumerResources.InputDescriptor_SupportsTemplatesIndicationLabel),
        Description = TrelloConsumerResources.CardCreateAction_InputCardNameDescription,
        InputMode = InputMode.TextBox,
        Id = "cardName",
        IsConfidential = false,
        GroupName = CommonConsumerResources.InputDescriptor_AdvancedGroupName,
        ValueHint = "{{{message.text}}}",
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = false,
          MaxLength = new int?(400)
        }
      },
      new InputDescriptor()
      {
        Name = string.Format(TrelloConsumerResources.CardCreateAction_InputCardDescriptionName, (object) CommonConsumerResources.InputDescriptor_SupportsTemplatesIndicationLabel),
        Description = TrelloConsumerResources.CardCreateAction_InputCardDescriptionDescription,
        InputMode = InputMode.TextArea,
        Id = "cardDescription",
        IsConfidential = false,
        GroupName = CommonConsumerResources.InputDescriptor_AdvancedGroupName,
        ValueHint = "{{{detailedMessage.markdown}}}",
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
      string errorMessage;
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
      string selectedListValue = (string) null;
      string selectedBoardValue = (string) null;
      consumerInputValues.TryGetValue("listId", out selectedListValue);
      consumerInputValues.TryGetValue("boardId", out selectedBoardValue);
      if (selectedListValue == null || selectedBoardValue == null)
        throw new ArgumentNullException();
      string errorMessage1 = (string) null;
      string errorMessage2 = (string) null;
      IList<InputValue> possibleValues = this.GetPossibleValues(requestContext, "boardId", consumerInputValues, out errorMessage1);
      InputValue inputValue1 = this.GetPossibleValues(requestContext, "listId", consumerInputValues, out errorMessage2).FirstOrDefault<InputValue>((Func<InputValue, bool>) (v => v.Value == selectedListValue));
      Func<InputValue, bool> predicate = (Func<InputValue, bool>) (v => v.Value == selectedBoardValue);
      InputValue inputValue2 = possibleValues.FirstOrDefault<InputValue>(predicate);
      return string.Format(TrelloConsumerResources.CardCreateAction_DescriptionFormat, inputValue1 == null ? (object) selectedListValue : (object) inputValue1.DisplayValue, inputValue2 == null ? (object) selectedBoardValue : (object) inputValue2.DisplayValue);
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs e)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      string consumerInput1 = e.Notification.GetConsumerInput("listId", true);
      string consumerInput2 = e.Notification.GetConsumerInput("userToken", true);
      string consumerInput3 = e.Notification.GetConsumerInput("labels");
      string consumerInput4 = e.Notification.GetConsumerInput("addToTop");
      string consumerInput5 = e.Notification.GetConsumerInput("cardName");
      string consumerInput6 = e.Notification.GetConsumerInput("cardDescription");
      TemplateRunner templateRunner = new TemplateRunner(raisedEvent);
      string str1 = templateRunner.Run(consumerInput5);
      string str2 = templateRunner.Run(consumerInput6);
      if (string.IsNullOrWhiteSpace(str1))
        str1 = raisedEvent.Message.Text;
      if (string.IsNullOrWhiteSpace(str2))
        str2 = raisedEvent.DetailedMessage.GetMarkdown();
      string str3 = str1.Substring(0, Math.Min(str1.Length, 400));
      string str4 = str2.Substring(0, Math.Min(str2.Length, 16384));
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/TrelloConsumer/CardCreateAction/UrlFormatCreateCard", true, "https://api.trello.com/1/cards?key={0}&token={1}"), (object) TrelloConsumer.ApplicationKey, (object) consumerInput2));
      string content = new JObject()
      {
        {
          "name",
          (JToken) str3
        },
        {
          "desc",
          (JToken) str4
        },
        {
          "pos",
          (JToken) (consumerInput4 == null ? "bottom" : (Convert.ToBoolean(consumerInput4) ? "top" : "bottom"))
        },
        {
          "due",
          (JToken) null
        },
        {
          "labels",
          (JToken) (consumerInput3 == null ? string.Empty : consumerInput3)
        },
        {
          "idList",
          (JToken) consumerInput1
        }
      }.ToString();
      httpRequestMessage.Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json");
      HttpRequestStringRepresentationBuilder representationBuilder = new HttpRequestStringRepresentationBuilder(httpRequestMessage);
      representationBuilder.ConfidentialRequestUri = string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/TrelloConsumer/CardCreateAction/UrlFormatCreateCard", true, "https://api.trello.com/1/cards?key={0}&token={1}"), (object) TrelloConsumer.ApplicationKey, (object) SecurityHelper.GetMaskedValue(consumerInput2));
      representationBuilder.AppendContent(content);
      return (ActionTask) new HttpActionTask(httpRequestMessage, representationBuilder.ToString());
    }
  }
}
