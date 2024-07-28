// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HockeyApp.HockeyAppSendNotificationAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HockeyApp
{
  [Export(typeof (ConsumerActionImplementation))]
  public class HockeyAppSendNotificationAction : HttpRequestAction
  {
    public const string ConsumerActionId = "sendNotification";
    public static readonly string[] s_supportedEventTypes = new string[1]
    {
      "workitem.updated"
    };
    public static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "workitem.updated",
        new string[2]{ "1.0", "3.1-preview.3" }
      }
    };

    public override string Id => "sendNotification";

    public override string ConsumerId => "hockeyApp";

    public override string Name => HockeyAppConsumerResources.SendNotificationActionName;

    public override string Description => HockeyAppConsumerResources.SendNotificationActionDescription;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = HockeyAppConsumerResources.SendNotificationAction_InputUrlName,
        Description = HockeyAppConsumerResources.SendNotificationAction_InputUrlDescription,
        InputMode = InputMode.TextBox,
        Id = "url",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Uri,
          IsRequired = true
        }
      }
    };

    protected override EventMessages GetDefaultDetailedMessagesToSend(Notification notification) => EventMessages.Text;

    protected override EventMessages GetDefaultMessagesToSend(Notification notification) => EventMessages.None;

    public override string[] SupportedEventTypes => HockeyAppSendNotificationAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => HockeyAppSendNotificationAction.s_supportedResourceVersions;
  }
}
