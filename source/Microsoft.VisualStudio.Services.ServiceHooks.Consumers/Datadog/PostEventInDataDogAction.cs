// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Datadog.PostEventInDataDogAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Datadog
{
  [Export(typeof (ConsumerActionImplementation))]
  public class PostEventInDataDogAction : HttpRequestAction
  {
    private const string c_id = "postEventInDatadog";
    private const string c_contentTypeJson = "application/json";
    private const int c_apiTokenLength = 32;
    private const string c_apiKeyInputPattern = "^[a-zA-Z0-9]+$";
    private const string c_euAccountTypeInputValue = "eu";
    private const string c_standardAccountTypeInputValue = "standard";
    private const string c_govAccountTypeInputValue = "gov";
    private const string c_us3AccountTypeInputValue = "us3";
    private const string c_us5AccountTypeInputValue = "us5";
    private const string c_ap1AccountTypeInputValue = "ap1";
    private const string c_standardUrl = "https://app.datadoghq.com/intake/webhook/azuredevops?api_key=";
    private const string c_euUrl = "https://app.datadoghq.eu/intake/webhook/azuredevops?api_key=";
    private const string c_govUrl = "https://app.ddog-gov.com/intake/webhook/azuredevops?api_key=";
    private const string c_us3Url = "https://us3.datadoghq.com/intake/webhook/azuredevops?api_key=";
    private const string c_us5Url = "https://us5.datadoghq.com/intake/webhook/azuredevops?api_key=";
    private const string c_ap1Url = "https://ap1.datadoghq.com/intake/webhook/azuredevops?api_key=";
    public const string ApiKeyInputId = "apiKey";
    public const string AccountTypeInputId = "accountType";
    private static readonly string[] s_resourceVersion10 = new string[1]
    {
      "1.0"
    };
    private static readonly string[] s_resourceVersion10Preview1 = new string[1]
    {
      "1.0-preview.1"
    };
    private static readonly string[] s_resourceVersion30Preview1 = new string[1]
    {
      "3.0-preview.1"
    };
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "build.complete",
        PostEventInDataDogAction.s_resourceVersion10
      },
      {
        "git.push",
        PostEventInDataDogAction.s_resourceVersion10
      },
      {
        "git.pullrequest.created",
        PostEventInDataDogAction.s_resourceVersion10Preview1
      },
      {
        "git.pullrequest.updated",
        PostEventInDataDogAction.s_resourceVersion10Preview1
      },
      {
        "tfvc.checkin",
        PostEventInDataDogAction.s_resourceVersion10
      },
      {
        "workitem.created",
        PostEventInDataDogAction.s_resourceVersion10
      },
      {
        "workitem.updated",
        PostEventInDataDogAction.s_resourceVersion10
      },
      {
        "workitem.commented",
        PostEventInDataDogAction.s_resourceVersion10
      },
      {
        "message.posted",
        PostEventInDataDogAction.s_resourceVersion10
      },
      {
        "ms.vss-release.release-created-event",
        PostEventInDataDogAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.release-abandoned-event",
        PostEventInDataDogAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-approval-pending-event",
        PostEventInDataDogAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-approval-completed-event",
        PostEventInDataDogAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-started-event",
        PostEventInDataDogAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-completed-event",
        PostEventInDataDogAction.s_resourceVersion30Preview1
      }
    };

    public override string ConsumerId => "datadog";

    public override string Id => "postEventInDatadog";

    public override string Name => DatadogConsumerResources.PostEventInDatadogActionName;

    public override string Description => DatadogConsumerResources.PostEventInDatadogActionDescription;

    public override string[] SupportedEventTypes => PostEventInDataDogAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostEventInDataDogAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = DatadogConsumerResources.PostEventInDatadogAction_ApiKeyInputName,
        Description = DatadogConsumerResources.PostEventInDatadogAction_ApiKeyInputDescription,
        InputMode = InputMode.TextBox,
        Id = "apiKey",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(32),
          MinLength = new int?(32),
          Pattern = "^[a-zA-Z0-9]+$"
        }
      },
      new InputDescriptor()
      {
        Name = DatadogConsumerResources.PostEventInDatadogAction_InputAccountTypeName,
        Description = DatadogConsumerResources.PostEventInDatadogAction_InputAccountTypeDescription,
        InputMode = InputMode.Combo,
        Id = "accountType",
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        },
        Values = new InputValues()
        {
          DefaultValue = "standard",
          InputId = "resourceDetailsToSend",
          IsLimitedToPossibleValues = true,
          PossibleValues = (IList<InputValue>) new List<InputValue>()
          {
            new InputValue()
            {
              Value = "standard",
              DisplayValue = DatadogConsumerResources.StandardAccountTypeName
            },
            new InputValue()
            {
              Value = "eu",
              DisplayValue = DatadogConsumerResources.EUAccountTypeName
            },
            new InputValue()
            {
              Value = "us3",
              DisplayValue = DatadogConsumerResources.US3AccountTypeName
            },
            new InputValue()
            {
              Value = "us5",
              DisplayValue = DatadogConsumerResources.US5AccountTypeName
            },
            new InputValue()
            {
              Value = "ap1",
              DisplayValue = DatadogConsumerResources.AP1AccountTypeName
            },
            new InputValue()
            {
              Value = "gov",
              DisplayValue = DatadogConsumerResources.GovAccountTypeName
            }
          }
        }
      }
    };

    public override string GetTargetUrl(IVssRequestContext requestContext, HandleEventArgs e)
    {
      string str;
      switch (e.Notification.GetConsumerInput("accountType", true))
      {
        case "standard":
          str = "https://app.datadoghq.com/intake/webhook/azuredevops?api_key=";
          break;
        case "eu":
          str = "https://app.datadoghq.eu/intake/webhook/azuredevops?api_key=";
          break;
        case "gov":
          str = "https://app.ddog-gov.com/intake/webhook/azuredevops?api_key=";
          break;
        case "us3":
          str = "https://us3.datadoghq.com/intake/webhook/azuredevops?api_key=";
          break;
        case "us5":
          str = "https://us5.datadoghq.com/intake/webhook/azuredevops?api_key=";
          break;
        case "ap1":
          str = "https://ap1.datadoghq.com/intake/webhook/azuredevops?api_key=";
          break;
        default:
          throw new ArgumentException("accountType");
      }
      string consumerInput = e.Notification.GetConsumerInput("apiKey", true);
      return str + consumerInput;
    }

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      return DatadogConsumerResources.PostEventInDatadogActionDetailedDescription;
    }
  }
}
