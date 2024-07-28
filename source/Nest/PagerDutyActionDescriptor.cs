// Decompiled with JetBrains decompiler
// Type: Nest.PagerDutyActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PagerDutyActionDescriptor : 
    ActionsDescriptorBase<PagerDutyActionDescriptor, IPagerDutyAction>,
    IPagerDutyAction,
    IAction,
    IPagerDutyEvent
  {
    public PagerDutyActionDescriptor(string name)
      : base(name)
    {
    }

    protected override ActionType ActionType => ActionType.PagerDuty;

    string IPagerDutyEvent.Account { get; set; }

    bool? IPagerDutyEvent.AttachPayload { get; set; }

    string IPagerDutyEvent.Client { get; set; }

    string IPagerDutyEvent.ClientUrl { get; set; }

    IEnumerable<IPagerDutyContext> IPagerDutyEvent.Context { get; set; }

    string IPagerDutyEvent.Description { get; set; }

    PagerDutyEventType? IPagerDutyEvent.EventType { get; set; }

    string IPagerDutyEvent.IncidentKey { get; set; }

    public PagerDutyActionDescriptor Account(string account) => this.Assign<string>(account, (Action<IPagerDutyAction, string>) ((a, v) => a.Account = v));

    public PagerDutyActionDescriptor Description(string description) => this.Assign<string>(description, (Action<IPagerDutyAction, string>) ((a, v) => a.Description = v));

    public PagerDutyActionDescriptor EventType(PagerDutyEventType? eventType) => this.Assign<PagerDutyEventType?>(eventType, (Action<IPagerDutyAction, PagerDutyEventType?>) ((a, v) => a.EventType = v));

    public PagerDutyActionDescriptor IncidentKey(string incidentKey) => this.Assign<string>(incidentKey, (Action<IPagerDutyAction, string>) ((a, v) => a.IncidentKey = v));

    public PagerDutyActionDescriptor Client(string client) => this.Assign<string>(client, (Action<IPagerDutyAction, string>) ((a, v) => a.Client = v));

    public PagerDutyActionDescriptor ClientUrl(string url) => this.Assign<string>(url, (Action<IPagerDutyAction, string>) ((a, v) => a.ClientUrl = v));

    public PagerDutyActionDescriptor AttachPayload(bool? attach = true) => this.Assign<bool?>(attach, (Action<IPagerDutyAction, bool?>) ((a, v) => a.AttachPayload = v));

    public PagerDutyActionDescriptor Context(
      Func<PagerDutyContextsDescriptor, IPromise<IList<IPagerDutyContext>>> selector)
    {
      return this.Assign<Func<PagerDutyContextsDescriptor, IPromise<IList<IPagerDutyContext>>>>(selector, (Action<IPagerDutyAction, Func<PagerDutyContextsDescriptor, IPromise<IList<IPagerDutyContext>>>>) ((a, v) => a.Context = v != null ? (IEnumerable<IPagerDutyContext>) v(new PagerDutyContextsDescriptor())?.Value : (IEnumerable<IPagerDutyContext>) null));
    }
  }
}
