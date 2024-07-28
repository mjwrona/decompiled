// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso.SimulatePullRequestAction
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
  public class SimulatePullRequestAction : ConsumerActionImplementation
  {
    private const string c_id = "simulatePullRequest";
    private static readonly string s_layer = typeof (SimulatePullRequestAction).Name;
    private static readonly string s_area = typeof (SimulatePullRequestAction).Namespace;
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "externalgit.pullrequest"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "externalgit.push",
        new string[1]{ ExternalGitPush.CurrentVersion.ToString() }
      }
    };

    public static string ConsumerActionId => "simulatePullRequest";

    public override string ConsumerId => "vso";

    public override string Id => "simulatePullRequest";

    public override string Name => VsoConsumerResources.SimulatePullRequestActionName;

    public override string Description => VsoConsumerResources.SimulatePullRequestActionDescription;

    public override string[] SupportedEventTypes => SimulatePullRequestAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => SimulatePullRequestAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>();

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      try
      {
        ExternalGitPullRequest eventPayload = JsonConvert.DeserializeObject<ExternalGitPullRequest>(JsonConvert.SerializeObject(eventArgs.Notification.Details.Event.Resource));
        eventPayload.ProjectId = eventArgs.Notification.Details.PublisherInputs["project"];
        eventPayload.Properties = (IDictionary<string, string>) new Dictionary<string, string>(eventArgs.Notification.Details.PublisherInputs);
        eventPayload.IsFromComment = false;
        requestContext.SimulateExternalEvent((object) eventPayload);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063810, SimulatePullRequestAction.s_area, SimulatePullRequestAction.s_layer, ex);
      }
      return (ActionTask) new NoopActionTask(VsoConsumerResources.NoopAction_PublishToServiceBus);
    }
  }
}
