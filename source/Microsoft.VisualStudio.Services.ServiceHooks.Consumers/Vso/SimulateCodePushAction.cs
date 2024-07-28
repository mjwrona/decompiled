// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso.SimulateCodePushAction
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
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso
{
  [Export(typeof (ConsumerActionImplementation))]
  public class SimulateCodePushAction : ConsumerActionImplementation
  {
    private const string c_id = "simulateCodePush";
    private static readonly string s_layer = typeof (SimulateCodePushAction).Name;
    private static readonly string s_area = typeof (SimulateCodePushAction).Namespace;
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "externalgit.push"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "externalgit.push",
        new string[1]{ ExternalGitPush.CurrentVersion.ToString() }
      }
    };

    public static string ConsumerActionId => "simulateCodePush";

    public override string ConsumerId => "vso";

    public override string Id => "simulateCodePush";

    public override string Name => VsoConsumerResources.SimulateCodePushActionName;

    public override string Description => VsoConsumerResources.SimulateCodePushActionDescription;

    public override string[] SupportedEventTypes => SimulateCodePushAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => SimulateCodePushAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>();

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      try
      {
        ExternalGitPush eventPayload = JsonConvert.DeserializeObject<ExternalGitPush>(JsonConvert.SerializeObject(eventArgs.Notification.Details.Event.Resource));
        eventPayload.ProjectId = eventArgs.Notification.Details.PublisherInputs["project"];
        eventPayload.Properties = (IDictionary<string, string>) new Dictionary<string, string>(eventArgs.Notification.Details.PublisherInputs);
        if (eventPayload.Commits != null)
        {
          List<ExternalGitCommit> externalGitCommitList = new List<ExternalGitCommit>();
          foreach (ExternalGitCommit commit in (IEnumerable<ExternalGitCommit>) eventPayload.Commits)
          {
            if (ExternalGitPushHelper.IsSkipCIMessagePresent(commit.Message))
            {
              commit.Message = "***NO_CI***";
              externalGitCommitList.Add(commit);
              eventPayload.Properties.Add("HasNoCi", bool.TrueString);
              eventPayload.Properties.Add("HasCommits", bool.TrueString);
              break;
            }
          }
          if (externalGitCommitList.Count == 0 && eventPayload.Commits.Count > 0)
          {
            ExternalGitCommit commit = eventPayload.Commits[0];
            commit.Message = string.Empty;
            externalGitCommitList.Add(commit);
            eventPayload.Properties.Add("HasCommits", bool.TrueString);
          }
          if (eventPayload.Commits.Count > 1)
          {
            ExternalGitCommit externalGitCommit = eventPayload.Commits.LastOrDefault<ExternalGitCommit>();
            if (externalGitCommit.Sha != externalGitCommitList[0].Sha && !ExternalGitPushHelper.IsSkipCIMessagePresent(externalGitCommit.Message) && externalGitCommit.AdditionalProperties != null)
            {
              bool? castedValueOrDefault1 = externalGitCommit.AdditionalProperties.GetCastedValueOrDefault<string, bool?>("Distinct");
              bool flag1 = true;
              if (castedValueOrDefault1.GetValueOrDefault() == flag1 & castedValueOrDefault1.HasValue && eventPayload.Commits.Count<ExternalGitCommit>((Func<ExternalGitCommit, bool>) (c =>
              {
                if (c.AdditionalProperties == null)
                  return false;
                bool? castedValueOrDefault2 = c.AdditionalProperties.GetCastedValueOrDefault<string, bool?>("Distinct");
                bool flag2 = false;
                return castedValueOrDefault2.GetValueOrDefault() == flag2 & castedValueOrDefault2.HasValue;
              })) == eventPayload.Commits.Count<ExternalGitCommit>() - 1)
              {
                externalGitCommit.Message = string.Empty;
                externalGitCommitList.Add(externalGitCommit);
              }
            }
          }
          eventPayload.Commits = (IList<ExternalGitCommit>) externalGitCommitList;
        }
        requestContext.SimulateExternalEvent((object) eventPayload);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063805, SimulateCodePushAction.s_area, SimulateCodePushAction.s_layer, ex);
      }
      return (ActionTask) new NoopActionTask(VsoConsumerResources.NoopAction_PublishToServiceBus);
    }
  }
}
