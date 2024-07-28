// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.EventTransformerConsumerActionImplementation
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  public abstract class EventTransformerConsumerActionImplementation : ConsumerActionImplementation
  {
    public const string ResourceDetailsToSendInputId = "resourceDetailsToSend";
    public const string MessagesToSendInputId = "messagesToSend";
    public const string DetailedMessagesToSend = "detailedMessagesToSend";
    private static readonly Dictionary<string, Func<InputDescriptor>> s_reservedInputDescriptors = new Dictionary<string, Func<InputDescriptor>>()
    {
      {
        "resourceDetailsToSend",
        new Func<InputDescriptor>(EventTransformerConsumerActionImplementation.BuildResourceDetailsToSendInputDescriptor)
      },
      {
        "messagesToSend",
        new Func<InputDescriptor>(EventTransformerConsumerActionImplementation.BuildMessagesToSendInputDescriptor)
      },
      {
        "detailedMessagesToSend",
        new Func<InputDescriptor>(EventTransformerConsumerActionImplementation.BuildDetailedMessagesToSendInputDescriptor)
      }
    };

    protected virtual EventResourceDetails GetDefaultResourceDetailsToSend(Notification notification) => !EventTransformerConsumerActionImplementation.IsCollectionScoped(notification.Details?.PublisherInputs) ? EventResourceDetails.All : EventResourceDetails.Minimal;

    protected virtual EventMessages GetDefaultMessagesToSend(Notification notification) => !EventTransformerConsumerActionImplementation.IsCollectionScoped(notification.Details?.PublisherInputs) ? EventMessages.All : EventMessages.None;

    protected virtual EventMessages GetDefaultDetailedMessagesToSend(Notification notification) => !EventTransformerConsumerActionImplementation.IsCollectionScoped(notification.Details?.PublisherInputs) ? EventMessages.All : EventMessages.None;

    protected static bool IsCollectionScoped(IDictionary<string, string> publisherInputs) => publisherInputs == null || !publisherInputs.ContainsKey("projectId");

    protected static IEnumerable<InputDescriptor> BuildAllPayloadControllersInputDescriptors() => EventTransformerConsumerActionImplementation.s_reservedInputDescriptors.Select<KeyValuePair<string, Func<InputDescriptor>>, InputDescriptor>((Func<KeyValuePair<string, Func<InputDescriptor>>, InputDescriptor>) (x => x.Value()));

    protected static InputDescriptor BuildResourceDetailsToSendInputDescriptor() => new InputDescriptor()
    {
      Id = "resourceDetailsToSend",
      Name = CommonConsumerResources.EventTransformerConsumerAction_ResourceDetailsToSendInputName,
      Description = CommonConsumerResources.EventTransformerConsumerAction_ResourceDetailsToSendInputDescription,
      IsConfidential = false,
      InputMode = InputMode.Combo,
      Validation = new InputValidation()
      {
        DataType = InputDataType.String,
        IsRequired = false
      },
      Values = new InputValues()
      {
        DefaultValue = string.Empty,
        InputId = "resourceDetailsToSend",
        IsLimitedToPossibleValues = true,
        PossibleValues = (IList<InputValue>) EventTransformerConsumerActionImplementation.BuildResourceDetailsToSendPossibleValues()
      }
    };

    protected static InputDescriptor BuildMessagesToSendInputDescriptor() => new InputDescriptor()
    {
      Id = "messagesToSend",
      Name = CommonConsumerResources.EventTransformerConsumerAction_MessagesToSendInputName,
      Description = CommonConsumerResources.EventTransformerConsumerAction_MessagesToSendInputDescription,
      IsConfidential = false,
      InputMode = InputMode.Combo,
      Validation = new InputValidation()
      {
        DataType = InputDataType.String,
        IsRequired = false
      },
      Values = new InputValues()
      {
        DefaultValue = string.Empty,
        InputId = "messagesToSend",
        IsLimitedToPossibleValues = true,
        PossibleValues = (IList<InputValue>) EventTransformerConsumerActionImplementation.BuildMessagesToSendPossibleValues()
      }
    };

    protected static InputDescriptor BuildDetailedMessagesToSendInputDescriptor() => new InputDescriptor()
    {
      Id = "detailedMessagesToSend",
      Name = CommonConsumerResources.EventTransformerConsumerAction_DetailedMessagesToSendInputName,
      Description = CommonConsumerResources.EventTransformerConsumerAction_DetailedMessagesToSendInputDescription,
      IsConfidential = false,
      InputMode = InputMode.Combo,
      Validation = new InputValidation()
      {
        DataType = InputDataType.String,
        IsRequired = false
      },
      Values = new InputValues()
      {
        DefaultValue = string.Empty,
        InputId = "detailedMessagesToSend",
        IsLimitedToPossibleValues = true,
        PossibleValues = (IList<InputValue>) EventTransformerConsumerActionImplementation.BuildMessagesToSendPossibleValues()
      }
    };

    protected static JObject TransformEvent(
      Notification notification,
      Event raisedEvent,
      EventResourceDetails defaultResourceDetailsToSend,
      EventMessages defaultMessagesToSend,
      EventMessages defaultDetailedMessagesToSend)
    {
      return EventTransformer.TransformEvent(raisedEvent, notification.GetConsumerInput("resourceDetailsToSend").ToEventResourceDetailsValue(defaultResourceDetailsToSend), notification.GetConsumerInput("messagesToSend").ToEventMessagesValue(defaultMessagesToSend), notification.GetConsumerInput("detailedMessagesToSend").ToEventMessagesValue(defaultDetailedMessagesToSend));
    }

    protected static string GetStringRepresentation(
      JObject jObject,
      bool useDefaultFormatting = true,
      Formatting formatting = Formatting.None)
    {
      return jObject.GetStringRepresentation(useDefaultFormatting, formatting);
    }

    private static List<InputValue> BuildMessagesToSendPossibleValues() => new List<InputValue>()
    {
      new InputValue()
      {
        Value = string.Empty,
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventMessages_All
      },
      new InputValue()
      {
        Value = "Text".ToLower(),
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventMessages_Text
      },
      new InputValue()
      {
        Value = "Html".ToLower(),
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventMessages_Html
      },
      new InputValue()
      {
        Value = "Markdown".ToLower(),
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventMessages_Markdown
      },
      new InputValue()
      {
        Value = "None".ToLower(),
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventMessages_None
      }
    };

    private static List<InputValue> BuildResourceDetailsToSendPossibleValues() => new List<InputValue>()
    {
      new InputValue()
      {
        Value = string.Empty,
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventResourceDetails_All
      },
      new InputValue()
      {
        Value = "Minimal".ToLower(),
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventResourceDetails_Minimal
      },
      new InputValue()
      {
        Value = "None".ToLower(),
        DisplayValue = CommonConsumerResources.EventTransformerConsumerAction_EventResourceDetails_None
      }
    };
  }
}
