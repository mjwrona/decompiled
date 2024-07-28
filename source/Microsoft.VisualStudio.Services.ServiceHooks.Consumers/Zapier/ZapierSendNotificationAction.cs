// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zapier.ZapierSendNotificationAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zapier
{
  [Export(typeof (ConsumerActionImplementation))]
  public class ZapierSendNotificationAction : HttpRequestAction
  {
    public const string ConsumerActionId = "sendNotification";
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public override string Id => "sendNotification";

    public override string ConsumerId => "zapier";

    public override string Name => ZapierConsumerResources.SendNotificationActionName;

    public override string Description => ZapierConsumerResources.SendNotificationActionDescription;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = ZapierConsumerResources.SendNotificationAction_InputUrlName,
        Description = ZapierConsumerResources.SendNotificationAction_InputUrlDescription,
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

    public override IDictionary<string, string[]> SupportedResourceVersions => ZapierSendNotificationAction.s_supportedResourceVersions;
  }
}
