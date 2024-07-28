// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso.SimulateACRPushAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso
{
  [Export(typeof (ConsumerActionImplementation))]
  public class SimulateACRPushAction : ConsumerActionImplementation
  {
    private const string c_id = "simulateACRPush";
    private const string c_messageBusName = "Microsoft.VisualStudio.Services.ServiceHooks.Server";
    private static readonly string s_layer = typeof (SimulateACRPushAction).Name;
    private static readonly string s_area = typeof (SimulateACRPushAction).Namespace;
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "acr.push"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "acr.push",
        new string[1]
        {
          ExternalAzureContainerRegistryPushEvent.CurrentVersion.ToString()
        }
      }
    };

    public static string ConsumerActionId => "simulateACRPush";

    public override string ConsumerId => "vso";

    public override string Id => "simulateACRPush";

    public override string Name => VsoConsumerResources.SimulateImagePushActionNameInACR;

    public override string Description => VsoConsumerResources.SimulateImagePushActionDescription;

    public override string[] SupportedEventTypes => SimulateACRPushAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => SimulateACRPushAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>();

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      try
      {
        ExternalAzureContainerRegistryPushEvent registryPushEvent = JsonConvert.DeserializeObject<ExternalAzureContainerRegistryPushEvent>(JsonConvert.SerializeObject(eventArgs.Notification.Details.Event.Resource));
        registryPushEvent.Properties = (IDictionary<string, string>) new Dictionary<string, string>(eventArgs.Notification.Details.PublisherInputs);
        requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.VisualStudio.Services.ServiceHooks.Server", new object[1]
        {
          (object) registryPushEvent
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063816, SimulateACRPushAction.s_area, SimulateACRPushAction.s_layer, ex);
      }
      return (ActionTask) new NoopActionTask(VsoConsumerResources.NoopAction_PublishToServiceBus);
    }
  }
}
