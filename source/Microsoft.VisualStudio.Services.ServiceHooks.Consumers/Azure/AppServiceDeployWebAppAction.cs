// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.AppServiceDeployWebAppAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  [Export(typeof (ConsumerActionImplementation))]
  public class AppServiceDeployWebAppAction : HttpRequestAction
  {
    private const int c_sessionTokenTimeoutMinutes = 15;
    private const string c_sessionTokenScope = "vso.code";
    public const string ConsumerActionId = "deployWebApp";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "git.push"
    };
    private static readonly Guid s_sessionTokenClientId = new Guid("F4BE7C3C-B663-4F87-8ACB-EB9E391BC251");
    private static readonly SessionTokenConfigurationDescriptor s_sessionTokenConfigDescriptor = new SessionTokenConfigurationDescriptor()
    {
      ClientId = AppServiceDeployWebAppAction.s_sessionTokenClientId,
      TimeoutMinutes = 15,
      Scope = "vso.code",
      Required = true,
      TokenType = SessionTokenType.Compact,
      TokenNameBuilder = new Func<IVssRequestContext, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, string>(AppServiceDeployWebAppAction.SessionTokenNameBuilder)
    };

    public override string Id => "deployWebApp";

    public override string ConsumerId => "azureAppService";

    public override string Name => AzureConsumerResources.AppServiceDeployWebAppActionName;

    public override string Description => AzureConsumerResources.AppServiceDeployWebAppActionDescription;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      HttpRequestAction.BuildUrlInputDescriptor(),
      HttpRequestAction.BuildBasicAuthUsernameInputDescriptor(),
      HttpRequestAction.BuildBasicAuthPasswordInputDescriptor()
    };

    public override string[] SupportedEventTypes => AppServiceDeployWebAppAction.s_supportedEventTypes;

    protected override EventResourceDetails GetDefaultResourceDetailsToSend(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      return !EventTransformerConsumerActionImplementation.IsCollectionScoped(notification.Details?.PublisherInputs) ? EventResourceDetails.All : EventResourceDetails.Minimal;
    }

    protected override EventMessages GetDefaultMessagesToSend(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification) => EventMessages.None;

    protected override EventMessages GetDefaultDetailedMessagesToSend(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification) => EventMessages.None;

    public override SessionTokenConfigurationDescriptor GetSessionTokenConfigurationDescriptor() => AppServiceDeployWebAppAction.s_sessionTokenConfigDescriptor;

    public static string SessionTokenNameBuilder(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      string str1 = (string) null;
      IDictionary<string, string> consumerInputs = notification?.Details?.ConsumerInputs;
      if (consumerInputs != null)
      {
        string uriString;
        if (consumerInputs.TryGetValue("url", out uriString))
        {
          try
          {
            Uri uri = new Uri(uriString);
            if (uri.Host.EndsWith(".scm.azurewebsites.net"))
            {
              string str2 = uri.Host.Substring(0, uri.Host.IndexOf('.'));
              str1 = string.Format(notification.SubscriptionId != Guid.Empty ? AzureConsumerResources.AppServiceDeployWebAppTokenName : AzureConsumerResources.AppServiceDeployWebAppTestTokenName, (object) str2);
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
      return str1;
    }
  }
}
