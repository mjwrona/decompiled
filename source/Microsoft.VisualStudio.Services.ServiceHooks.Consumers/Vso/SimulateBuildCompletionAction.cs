// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso.SimulateBuildCompletionAction
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
  public class SimulateBuildCompletionAction : ConsumerActionImplementation
  {
    private const string c_id = "simulateBuildCompletion";
    private static readonly string s_layer = typeof (SimulateBuildCompletionAction).Name;
    private static readonly string s_area = typeof (SimulateBuildCompletionAction).Namespace;
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "externalbuild.completed"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "externalbuild.completed",
        new string[1]
        {
          ExternalBuildCompletionEvent.CurrentVersion.ToString()
        }
      }
    };

    public static string ConsumerActionId => "simulateBuildCompletion";

    public override string ConsumerId => "vso";

    public override string Id => "simulateBuildCompletion";

    public override string Name => VsoConsumerResources.SimulateBuildCompletionActionName;

    public override string Description => VsoConsumerResources.SimulateBuildCompletionActionDescription;

    public override string[] SupportedEventTypes => SimulateBuildCompletionAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => SimulateBuildCompletionAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>();

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      try
      {
        ExternalBuildCompletionEvent eventPayload = JsonConvert.DeserializeObject<ExternalBuildCompletionEvent>(JsonConvert.SerializeObject(eventArgs.Notification.Details.Event.Resource));
        eventPayload.Properties = (IDictionary<string, string>) new Dictionary<string, string>(eventArgs.Notification.Details.PublisherInputs);
        requestContext.SimulateExternalEvent((object) eventPayload);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063800, SimulateBuildCompletionAction.s_area, SimulateBuildCompletionAction.s_layer, ex);
      }
      return (ActionTask) new NoopActionTask(VsoConsumerResources.NoopAction_PublishToServiceBus);
    }
  }
}
