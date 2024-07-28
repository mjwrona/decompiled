// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso.SimulateCommentAction
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
  public class SimulateCommentAction : ConsumerActionImplementation
  {
    private const string c_id = "simulateComment";
    private const string c_runCommandKeyword = "run";
    private static readonly string s_layer = typeof (SimulateCommentAction).Name;
    private static readonly string s_area = typeof (SimulateCommentAction).Namespace;
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "externalgit.issuecomment"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "externalgit.issuecomment",
        new string[1]{ ExternalGitPush.CurrentVersion.ToString() }
      }
    };

    public static string ConsumerActionId => "simulateComment";

    public override string ConsumerId => "vso";

    public override string Id => "simulateComment";

    public override string Name => VsoConsumerResources.SimulateCommentActionName;

    public override string Description => VsoConsumerResources.SimulateCommentActionDescription;

    public override string[] SupportedEventTypes => SimulateCommentAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => SimulateCommentAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>();

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      try
      {
        ExternalPullRequestCommentEvent requestCommentEvent = JsonConvert.DeserializeObject<ExternalPullRequestCommentEvent>(JsonConvert.SerializeObject(eventArgs.Notification.Details.Event.Resource));
        if (requestCommentEvent.Command.CommandKeyword.Equals("run"))
        {
          ExternalGitPullRequest pullRequest = requestCommentEvent.PullRequest;
          pullRequest.ProjectId = eventArgs.Notification.Details.PublisherInputs["project"];
          pullRequest.Properties = (IDictionary<string, string>) new Dictionary<string, string>(eventArgs.Notification.Details.PublisherInputs);
          requestContext.SimulateExternalEvent((object) pullRequest);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063810, SimulateCommentAction.s_area, SimulateCommentAction.s_layer, ex);
      }
      return (ActionTask) new NoopActionTask(VsoConsumerResources.NoopAction_PublishToServiceBus);
    }
  }
}
